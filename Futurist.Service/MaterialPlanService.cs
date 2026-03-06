using Futurist.Repository.Command.MaterialPlanCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Hangfire;
using Serilog;

namespace Futurist.Service;

public class MaterialPlanService : IMaterialPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly KeycloakTokenService _keycloakTokenService;
    private readonly ILogger _logger = Log.ForContext<MaterialPlanService>();

    public MaterialPlanService(IUnitOfWork unitOfWork, MapperlyMapper mapper, KeycloakTokenService keycloakTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _keycloakTokenService = keycloakTokenService;
    }

    public async Task<ServiceResponse<SpTaskDto>> ProcessMaterialPlanAsync(int roomId, int verId, int year, int month)
    {
        try
        {
            var command = new ProcessMaterialPlanCommand
            {
                RoomId = roomId,
                VerId = verId,
                Year = year,
                Month = month,
                Timeout = 0
            };
            
            var response = await _unitOfWork.MaterialPlanRepository.ProcessMaterialPlanAsync(command) 
                ?? throw new NullReferenceException("Material Plan processing does not return any result");
            
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.CommitAsync();
            }
            
            _logger.Information("ProcessMaterialPlan {@command}", command);

            return new ServiceResponse<SpTaskDto>
            {
                Message = ServiceMessageConstants.MaterialPlanProcessed,
                Data = _mapper.MapToDto(response)
            };
        }
        catch (Exception e)
        {
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.RollbackAsync();
            }
            
            _logger.Error(e, "ProcessMaterialPlan failed {@command}", e.Message);
            
            return new ServiceResponse<SpTaskDto>
            {
                Message = ServiceMessageConstants.MaterialPlanProcessFailed,
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetMaterialPlanRoomIdsAsync()
    {
        try
        {
            var command = new GetMaterialPlanRoomIdsCommand();
            var response = await _unitOfWork.MaterialPlanRepository.GetMaterialPlanRoomIdsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.MaterialPlanRoomIdsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetMaterialPlanRoomIdsAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MaterialPlanRoomIdsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetMaterialPlanVerIdsAsync(int roomId)
    {
        try
        {
            var command = new GetMaterialPlanVerIdsCommand { RoomId = roomId };
            var response = await _unitOfWork.MaterialPlanRepository.GetMaterialPlanVerIdsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.MaterialPlanVerIdsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetMaterialPlanVerIdsAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MaterialPlanVerIdsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetMaterialPlanYearsAsync(int roomId, int verId)
    {
        try
        {
            var command = new GetMaterialPlanYearsCommand { RoomId = roomId, VerId = verId };
            var response = await _unitOfWork.MaterialPlanRepository.GetMaterialPlanYearsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.MaterialPlanYearsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetMaterialPlanYearsAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MaterialPlanYearsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetMaterialPlanMonthsAsync(int roomId, int verId, int year)
    {
        try
        {
            var command = new GetMaterialPlanMonthsCommand { RoomId = roomId, VerId = verId, Year = year };
            var response = await _unitOfWork.MaterialPlanRepository.GetMaterialPlanMonthsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.MaterialPlanMonthsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetMaterialPlanMonthsAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MaterialPlanMonthsNotFound
            };
        }
    }

    public string ProcessMaterialPlanJob(int roomId, int verId, int year, int month)
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessMaterialPlanAsync));
        
        // check if the room id is already in process
        if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == roomId))
        {
            return ServiceMessageConstants.MaterialPlanJobAlreadyInProcess;
        }
        
        var jobId = BackgroundJob.Enqueue<IMaterialPlanService>(s => s.ProcessMaterialPlanAsync(roomId, verId, year, month));
        BackgroundJob.ContinueJobWith(jobId, () => NotifyClientsMaterialPlanProcessingStateChanged());
        
        // Notify clients immediately that a new job has been queued
        _ = NotifyClientsMaterialPlanProcessingStateChanged();
        
        return ServiceMessageConstants.MaterialPlanProcessing;
    }

    public IEnumerable<int> MaterialPlanInProcessRoomIds()
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessMaterialPlanAsync));
        
        // get the room ids
        return processingJobsFiltered.Select(j => j.Value.Job.Args[0] as int? ?? 0);
    }

    public async Task NotifyClientsMaterialPlanProcessingStateChanged()
    {
        await _keycloakTokenService.Notify(new NotificationDto<IEnumerable<int>>
        {
            Method = "MaterialPlanProcessingStateChanged",
            Data = MaterialPlanInProcessRoomIds()
        });
    }
}
