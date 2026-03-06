using Futurist.Repository.Command.MaterialActCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Hangfire;
using Serilog;

namespace Futurist.Service;

public class MaterialActService : IMaterialActService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly KeycloakTokenService _keycloakTokenService;
    private readonly ILogger _logger = Log.ForContext<MaterialActService>();

    public MaterialActService(IUnitOfWork unitOfWork, MapperlyMapper mapper, KeycloakTokenService keycloakTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _keycloakTokenService = keycloakTokenService;
    }

    public async Task<ServiceResponse<SpTaskDto>> ProcessMaterialActAsync(int year, int month)
    {
        try
        {
            var command = new ProcessMaterialActCommand
            {
                Year = year,
                Month = month,
                Timeout = 0
            };
            
            var response = await _unitOfWork.MaterialActRepository.ProcessMaterialActAsync(command) 
                ?? throw new NullReferenceException("Material Actual processing does not return any result");
            
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.CommitAsync();
            }
            
            _logger.Information("ProcessMaterialAct {@command}", command);

            return new ServiceResponse<SpTaskDto>
            {
                Message = ServiceMessageConstants.MaterialActProcessed,
                Data = _mapper.MapToDto(response)
            };
        }
        catch (Exception e)
        {
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.RollbackAsync();
            }
            
            _logger.Error(e, "ProcessMaterialAct failed {@command}", e.Message);
            
            return new ServiceResponse<SpTaskDto>
            {
                Message = ServiceMessageConstants.MaterialActProcessFailed,
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetMaterialActYearsAsync()
    {
        try
        {
            var command = new GetMaterialActYearsCommand();
            var response = await _unitOfWork.MaterialActRepository.GetMaterialActYearsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.MaterialActYearsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetMaterialActYearsAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MaterialActYearsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetMaterialActMonthsAsync(int year)
    {
        try
        {
            var command = new GetMaterialActMonthsCommand { Year = year };
            var response = await _unitOfWork.MaterialActRepository.GetMaterialActMonthsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.MaterialActMonthsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetMaterialActMonthsAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MaterialActMonthsNotFound
            };
        }
    }

    public string ProcessMaterialActJob(int year, int month)
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessMaterialActAsync));
        
        // check if the same year+month is already in process
        if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == year && j.Value.Job.Args[1] as int? == month))
        {
            return ServiceMessageConstants.MaterialActJobAlreadyInProcess;
        }
        
        var jobId = BackgroundJob.Enqueue<IMaterialActService>(s => s.ProcessMaterialActAsync(year, month));
        BackgroundJob.ContinueJobWith(jobId, () => NotifyClientsMaterialActProcessingStateChanged());
        
        // Notify clients immediately that a new job has been queued
        _ = NotifyClientsMaterialActProcessingStateChanged();
        
        return ServiceMessageConstants.MaterialActProcessing;
    }

    public IEnumerable<string> MaterialActInProcessYearMonths()
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessMaterialActAsync));
        
        // get the year-month as "YYYY-MM" strings
        return processingJobsFiltered.Select(j =>
        {
            var year = j.Value.Job.Args[0] as int? ?? 0;
            var month = j.Value.Job.Args[1] as int? ?? 0;
            return $"{year}-{month:D2}";
        });
    }

    public async Task NotifyClientsMaterialActProcessingStateChanged()
    {
        // Send a notification signal — the client will re-query via the hub method for actual state
        await _keycloakTokenService.Notify(new NotificationDto<IEnumerable<int>>
        {
            Method = "MaterialActProcessingStateChanged",
            Data = []
        });
    }
}
