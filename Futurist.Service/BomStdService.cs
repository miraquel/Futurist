using Futurist.Infrastructure.SignalR.Hubs;
using Futurist.Repository.Command.BomStdCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace Futurist.Service;

public class BomStdService : IBomStdService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly IHubContext<FuturistHub> _hubContext;

    public BomStdService(IUnitOfWork unitOfWork, MapperlyMapper mapper, IHubContext<FuturistHub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<ServiceResponse> ProcessBomStdAsync(int roomId)
    {
        try
        {
            var command = new ProcessBomStdCommand
            {
                RoomId = roomId,
                DbTransaction = _unitOfWork.BeginTransaction()
            };

            var response = await _unitOfWork.BomStdRepository.ProcessBomStdAsync(command);

            await _unitOfWork.CommitAsync();

            return new ServiceResponse
            {
                Message = response
            };
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackAsync();
            
            return new ServiceResponse
            {
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<BomStdDto>>> BomErrorCheckAsync(int roomId)
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

    public async Task<ServiceResponse<PagedListDto<BomStdDto>>> BomErrorCheckPagedListAsync(PagedListRequestDto<BomStdDto> filter)
    {
        try
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
            var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessBomStdAsync));
        
            // check if the room id is already in process
            if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == filter.Filter.Room))
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
            return new ServiceResponse<PagedListDto<BomStdDto>>
            {
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetBomStdRoomIdsAsync()
    {
        var command = new GetBomStdRoomIdsCommand();
        var response = await _unitOfWork.BomStdRepository.GetBomStdRoomIdsAsync(command);
        
        return new ServiceResponse<IEnumerable<int>>
        {
            Message = ServiceMessageConstants.RoomIdsFound,
            Data = response
        };
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
        
        _hubContext.Clients.All.SendAsync("BomStdProcessingStateChanged", GetBomStdInProcessRoomIds());
        
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
        await _hubContext.Clients.All.SendAsync("BomStdProcessingStateChanged", GetBomStdInProcessRoomIds());
    }
}