using Futurist.Repository.Command.MupCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Hangfire;
using Hangfire.Storage;
using Serilog;

namespace Futurist.Service;

public class MupService : IMupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly KeycloakTokenService _keycloakTokenService;
    private readonly ILogger _logger = Log.ForContext<MupService>();

    public MupService(IUnitOfWork unitOfWork, MapperlyMapper mapper, KeycloakTokenService keycloakTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _keycloakTokenService = keycloakTokenService;
    }

    public async Task<ServiceResponse<SpTaskDto>> ProcessMupAsync(int roomId)
    {
        try
        {
            var command = new ProcessMupCommand
            {
                RoomId = roomId,
                Timeout = 0
            };
            
            var response = await _unitOfWork.MupRepository.ProcessMupAsync(command) ?? throw new NullReferenceException("Mup processing does not return any result");
            
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.CommitAsync();
            }
            
            _logger.Information("ProcessMup {@command}", command);

            return new ServiceResponse<SpTaskDto>
            {
                Message = ServiceMessageConstants.MupProcessed,
                Data = _mapper.MapToDto(response)
            };
        }
        catch (Exception e)
        {
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.RollbackAsync();
            }
            
            _logger.Error(e, "ProcessMup failed {@command}", e.Message);
            
            return new ServiceResponse<SpTaskDto>
            {
                Message = ServiceMessageConstants.MupProcessFailed,
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<MupSpDto>>> MupResultAsync(int roomId)
    {
        try
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
            var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessMupAsync));
        
            // check if the room id is already in process
            if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == roomId))
            {
                return new ServiceResponse<IEnumerable<MupSpDto>>
                {
                    Errors = [ServiceMessageConstants.MupJobAlreadyInProcess]
                };
            }
            
            var command = new MupResultCommand
            {
                RoomId = roomId
            };
            
            var response = await _unitOfWork.MupRepository.MupResultAsync(command);

            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupResultFound,
                Data = _mapper.MapToIEnumerableDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "MupResult failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MupResultNotFound
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<MupSpDto>>> MupResultPagedListAsync(PagedListRequestDto filter)
    {
        try
        {
            var command = new MupResultPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(filter)
            };
            
            var response = await _unitOfWork.MupRepository.MupResultPagedListAsync(command);
            
            return new ServiceResponse<PagedListDto<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupResultFound,
                Data = _mapper.MapToPagedListDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "MupResultPagedListAsync failed {@command}", e.Message);
            
            return new ServiceResponse<PagedListDto<MupSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MupResultNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetMupRoomIdsAsync()
    {
        try
        {
            var command = new GetMupRoomIdsCommand();
            var response = await _unitOfWork.MupRepository.GetMupRoomIdsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.RofoRoomIdsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetMupRoomIdsAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MupRoomIdsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<MupSpDto>>> MupSummaryByItemIdFromSpAsync(int roomId)
    {
        try
        {
            var command = new MupSummaryByItemIdFromSpCommand
            {
                RoomId = roomId
            };
            
            var response = await _unitOfWork.MupRepository.MupSummaryByItemIdFromSpAsync(command);
            
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByItemIdFound,
                Data = _mapper.MapToIEnumerableDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "MupSummaryByItemIdFromSpAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByItemIdNotFound,
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<MupSpDto>>> MupSummaryByItemIdAsync(ListRequestDto filter)
    {
        try
        {
            var command = new MupSummaryByItemIdCommand
            {
                ListRequest = _mapper.MapToListRequest(filter)
            };
            
            var response = await _unitOfWork.MupRepository.MupSummaryByItemIdAsync(command);
            
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByItemIdFound,
                Data = _mapper.MapToIEnumerableDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "MupSummaryByItemIdAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByItemIdNotFound,
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<MupSpDto>>> MupSummaryByItemIdPagedListAsync(PagedListRequestDto filter)
    {
        try
        {
            var command = new MupSummaryByItemIdPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(filter)
            };
            
            var response = await _unitOfWork.MupRepository.MupSummaryByItemIdPagedListAsync(command);

            return new ServiceResponse<PagedListDto<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByItemIdFound,
                Data = _mapper.MapToPagedListDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "MupSummaryByItemIdPagedListAsync failed {@command}", e.Message);
            
            return new ServiceResponse<PagedListDto<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByItemIdNotFound,
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<MupSpDto>>> MupSummaryByBatchNumberFromSpAsync(int roomId)
    {
        try
        {
            var command = new MupSummaryByBatchNumberFromSpCommand
            {
                RoomId = roomId
            };
            
            var response = await _unitOfWork.MupRepository.MupSummaryByBatchNumberFromSpAsync(command);
            
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByBatchNumberFound,
                Data = _mapper.MapToIEnumerableDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "MupSummaryByBatchNumberFromSpAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByBatchNumberNotFound,
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<MupSpDto>>> MupSummaryByBatchNumberAsync(ListRequestDto filter)
    {
        try
        {
            var command = new MupSummaryByBatchNumberCommand
            {
                ListRequest = _mapper.MapToListRequest(filter)
            };
            
            var response = await _unitOfWork.MupRepository.MupSummaryByBatchNumberAsync(command);
            
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByBatchNumberFound,
                Data = _mapper.MapToIEnumerableDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "MupSummaryByBatchNumberAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByBatchNumberNotFound,
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<MupSpDto>>> MupSummaryByBatchNumberPagedListAsync(PagedListRequestDto filter)
    {
        try
        {
            var command = new MupSummaryByBatchNumberPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(filter)
            };
            
            var response = await _unitOfWork.MupRepository.MupSummaryByBatchNumberPagedListAsync(command);
            
            return new ServiceResponse<PagedListDto<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByBatchNumberFound,
                Data = _mapper.MapToPagedListDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "MupSummaryByBatchNumberPagedListAsync failed {@command}", e.Message);
            
            return new ServiceResponse<PagedListDto<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupSummaryByBatchNumberNotFound,
                Errors = [e.Message]
            };
        }
    }

    public string ProcessMupJob(int roomId)
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessMupAsync));
        
        // check if the room id is already in process
        if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == roomId))
        {
            return ServiceMessageConstants.MupJobAlreadyInProcess;
        }
        
        var jobId = BackgroundJob.Enqueue<IMupService>(s => s.ProcessMupAsync(roomId));
        BackgroundJob.ContinueJobWith(jobId, () => NotifyClientsMupProcessingStateChanged());
        
        // Notify clients immediately that a new job has been queued
        _ = NotifyClientsMupProcessingStateChanged();
        
        return ServiceMessageConstants.MupProcessing;
    }

    public IEnumerable<int> MupInProcessRoomIds()
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessMupAsync));
        
        // get the room ids
        return processingJobsFiltered.Select(j => j.Value.Job.Args[0] as int? ?? 0);
    }

    public async Task NotifyClientsMupProcessingStateChanged()
    {
        await _keycloakTokenService.Notify(new NotificationDto<IEnumerable<int>>
        {
            Method = "FgCostProcessingStateChanged",
            Data = MupInProcessRoomIds()
        });
    }
}