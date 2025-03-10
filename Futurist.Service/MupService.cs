using Futurist.Infrastructure.SignalR.Hubs;
using Futurist.Repository.Command.MupCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.SignalR;

namespace Futurist.Service;

public class MupService : IMupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly IHubContext<FuturistHub> _hubContext;

    public MupService(IUnitOfWork unitOfWork, MapperlyMapper mapper, IHubContext<FuturistHub> hubContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<ServiceResponse<IEnumerable<MupSpDto>>> ProcessMupAsync(int roomId)
    {
        try
        {
            var command = new ProcessMupCommand
            {
                RoomId = roomId
            };
            
            var response = await _unitOfWork.MupRepository.ProcessMupAsync(command);

            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupProcessed,
                Data = _mapper.MapToIEnumerableDto(response)
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
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
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<MupSpDto>>> MupResultPagedListAsync(PagedListRequestDto<MupSpDto> filter)
    {
        try
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
            var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(ProcessMupAsync));
        
            // check if the room id is already in process
            if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == filter.Filter.Room))
            {
                return new ServiceResponse<PagedListDto<MupSpDto>>
                {
                    Errors = [ServiceMessageConstants.MupJobAlreadyInProcess]
                };
            }
            
            var command = new MupResultPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(filter)
            };
            
            var response = await _unitOfWork.MupRepository.MupResultPagedListAsync(command);
            
            return new ServiceResponse<PagedListDto<MupSpDto>>
            {
                Message = ServiceMessageConstants.BomStdFound,
                Data = _mapper.MapToPagedListDto(response)
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<PagedListDto<MupSpDto>>
            {
                Errors = [e.Message]
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
                Message = ServiceMessageConstants.RoomIdsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<int>>
            {
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
        _hubContext.Clients.All.SendAsync("MupProcessingStateChanged", MupInProcessRoomIds());
        
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
        await _hubContext.Clients.All.SendAsync("MupProcessingStateChanged", MupInProcessRoomIds());
    }
}