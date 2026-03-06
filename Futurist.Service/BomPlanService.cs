using Futurist.Repository.Command.BomPlanCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Hangfire;
using Serilog;

namespace Futurist.Service;

public class BomPlanService : IBomPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly KeycloakTokenService _keycloakTokenService;
    private readonly ILogger _logger = Log.ForContext<BomPlanService>();

    public BomPlanService(IUnitOfWork unitOfWork, MapperlyMapper mapper, KeycloakTokenService keycloakTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _keycloakTokenService = keycloakTokenService;
    }

    public async Task<ServiceResponse<SpTaskDto>> ProcessBomPlanAsync(int roomId, int verId, int year, int month)
    {
        try
        {
            var command = new ProcessBomPlanCommand
            {
                RoomId = roomId,
                VerId = verId,
                Year = year,
                Month = month,
                Timeout = 0
            };

            await _unitOfWork.BomPlanRepository.ProcessBomPlanAsync(command);

            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.CommitAsync();
            }

            _logger.Information("ProcessBomPlan {@command}", command);

            return new ServiceResponse<SpTaskDto>
            {
                Message = ServiceMessageConstants.BomPlanProcessed,
                Data = new SpTaskDto { StatusId = true, StatusName = ServiceMessageConstants.BomPlanProcessed }
            };
        }
        catch (Exception e)
        {
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.RollbackAsync();
            }

            _logger.Error(e, "ProcessBomPlan failed {@command}", e.Message);

            return new ServiceResponse<SpTaskDto>
            {
                Message = ServiceMessageConstants.BomPlanProcessFailed,
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetBomPlanRoomIdsAsync()
    {
        try
        {
            var command = new GetBomPlanRoomIdsCommand();
            var response = await _unitOfWork.BomPlanRepository.GetBomPlanRoomIdsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.BomPlanRoomIdsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetBomPlanRoomIdsAsync failed {@command}", e.Message);

            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.BomPlanRoomIdsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetBomPlanVerIdsAsync(int roomId)
    {
        try
        {
            var command = new GetBomPlanVerIdsCommand { RoomId = roomId };
            var response = await _unitOfWork.BomPlanRepository.GetBomPlanVerIdsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.BomPlanVerIdsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetBomPlanVerIdsAsync failed {@command}", e.Message);

            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.BomPlanVerIdsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetBomPlanYearsAsync(int roomId, int verId)
    {
        try
        {
            var command = new GetBomPlanYearsCommand { RoomId = roomId, VerId = verId };
            var response = await _unitOfWork.BomPlanRepository.GetBomPlanYearsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.BomPlanYearsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetBomPlanYearsAsync failed {@command}", e.Message);

            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.BomPlanYearsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetBomPlanMonthsAsync(int roomId, int verId, int year)
    {
        try
        {
            var command = new GetBomPlanMonthsCommand { RoomId = roomId, VerId = verId, Year = year };
            var response = await _unitOfWork.BomPlanRepository.GetBomPlanMonthsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.BomPlanMonthsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetBomPlanMonthsAsync failed {@command}", e.Message);

            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.BomPlanMonthsNotFound
            };
        }
    }

    public string ProcessBomPlanJob(int roomId, int verId, int year, int month)
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessBomPlanAsync));

        if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == roomId))
        {
            return ServiceMessageConstants.BomPlanJobAlreadyInProcess;
        }

        var jobId = BackgroundJob.Enqueue<IBomPlanService>(s => s.ProcessBomPlanAsync(roomId, verId, year, month));
        BackgroundJob.ContinueJobWith(jobId, () => NotifyClientsBomPlanProcessingStateChanged());

        _ = NotifyClientsBomPlanProcessingStateChanged();

        return ServiceMessageConstants.BomPlanProcessing;
    }

    public IEnumerable<int> BomPlanInProcessRoomIds()
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessBomPlanAsync));

        return processingJobsFiltered.Select(j => j.Value.Job.Args[0] as int? ?? 0);
    }

    public async Task NotifyClientsBomPlanProcessingStateChanged()
    {
        await _keycloakTokenService.Notify(new NotificationDto<IEnumerable<int>>
        {
            Method = "BomPlanProcessingStateChanged",
            Data = BomPlanInProcessRoomIds()
        });
    }
}
