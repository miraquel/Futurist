using Futurist.Repository.Command.BomStdCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Futurist.Service;

public class BomStdService : IBomStdService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly KeycloakTokenService _keycloakTokenService;
    private readonly ILogger _logger = Log.ForContext<BomStdService>();

    public BomStdService(IUnitOfWork unitOfWork, MapperlyMapper mapper, IConfiguration configuration, KeycloakTokenService keycloakTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _keycloakTokenService = keycloakTokenService;
    }

    public async Task<ServiceResponse<SpTaskDto>> ProcessBomStdAsync(int roomId)
    {
        try
        {
            var command = new ProcessBomStdCommand
            {
                RoomId = roomId,
                Timeout = 18000
                //DbTransaction = _unitOfWork.BeginTransaction()
            };

            var response = await _unitOfWork.BomStdRepository.ProcessBomStdAsync(command) ?? throw new NullReferenceException("BomStd processing does not return any result");

            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.CommitAsync();
            }
            
            _logger.Information("BomStd {@command}", command);

            return new ServiceResponse<SpTaskDto>
            {
                Data = _mapper.MapToDto(response),
                Message = ServiceMessageConstants.BomStdProcessed
            };
        }
        catch (Exception e)
        {
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.RollbackAsync();
            }
            
            _logger.Error(e, "BomStd processing failed {@command}", e.Message);
            
            return new ServiceResponse<SpTaskDto>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.BomStdProcessFailed
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<BomStdDto>>> BomErrorCheckAsync(int roomId)
    {
        try
        {
            var command = new BomErrorCheckCommand
            {
                RoomId = roomId
            };
        
            var response = await _unitOfWork.BomStdRepository.BomErrorCheckAsync(command);
        
            return new ServiceResponse<IEnumerable<BomStdDto>>
            {
                Message = ServiceMessageConstants.BomStdFound,
                Data = _mapper.MapToIEnumerableDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "BomStd {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<BomStdDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.BomStdNotFound
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<BomStdDto>>> BomErrorCheckPagedListAsync(PagedListRequestDto filter)
    {
        try
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
            var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessBomStdAsync));
        
            // check if the room id is already in process
            filter.Filters.TryGetValue("Room", out var room);
            if (!int.TryParse(room, out var roomId))
            {
                return new ServiceResponse<PagedListDto<BomStdDto>>
                {
                    Errors = [ServiceMessageConstants.RoomIdInvalid]
                };
            }
            
            if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == roomId))
            {
                return new ServiceResponse<PagedListDto<BomStdDto>>
                {
                    Errors = [ServiceMessageConstants.BomStdJobAlreadyInProcess]
                };
            }
            
            var command = new BomErrorCheckPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(filter)
            };
            
            var response = await _unitOfWork.BomStdRepository.BomErrorCheckPagedListAsync(command);
            
            return new ServiceResponse<PagedListDto<BomStdDto>>
            {
                Message = ServiceMessageConstants.BomStdFound,
                Data = _mapper.MapToPagedListDto(response)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "BomStd {@command}", e.Message);
            
            return new ServiceResponse<PagedListDto<BomStdDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.BomStdNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetBomStdRoomIdsAsync()
    {
        try
        {
            var command = new GetBomStdRoomIdsCommand();
            var response = await _unitOfWork.BomStdRepository.GetBomStdRoomIdsAsync(command);
        
            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.RofoRoomIdsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "BomStd {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.BomStdRoomIdsNotFound
            };
        }
    }

    public string ProcessBomStdJob(int roomId)
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessBomStdAsync));
        
        // check if the room id is already in process
        if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == roomId))
        {
            return ServiceMessageConstants.BomStdJobAlreadyInProcess;
        }
        
        var jobId = BackgroundJob.Enqueue<IBomStdService>(s => s.ProcessBomStdAsync(roomId));
        BackgroundJob.ContinueJobWith(jobId, () => NotifyClientsBomStdProcessingStateChanged());

        _ = NotifyClientsBomStdProcessingStateChanged();
        
        return ServiceMessageConstants.BomStdProcessing;
    }
    
    public IEnumerable<int> GetBomStdInProcessRoomIds()
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessBomStdAsync));
        
        // get the room ids
        return processingJobsFiltered.Select(j => j.Value.Job.Args[0] as int? ?? 0);
    }

    public async Task NotifyClientsBomStdProcessingStateChanged()
    {
        await _keycloakTokenService.Notify(new NotificationDto<IEnumerable<int>>
        {
            Method = "BomStdProcessingStateChanged",
            Data = GetBomStdInProcessRoomIds()
        });
    }
}