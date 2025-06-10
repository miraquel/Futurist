using Futurist.Repository.Command.FgCostCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Hangfire;
using Serilog;

namespace Futurist.Service;

public class FgCostService : IFgCostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly KeycloakTokenService _keycloakTokenService;
    private readonly ILogger _logger = Log.ForContext<FgCostService>();

    public FgCostService(IUnitOfWork unitOfWork, MapperlyMapper mapper, KeycloakTokenService keycloakTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _keycloakTokenService = keycloakTokenService;
    }

    public async Task<ServiceResponse<SpTaskDto>> CalculateFgCostAsync(int roomId)
    {
        try
        {
            //var transaction = _unitOfWork.BeginTransaction();
            var command = new CalculateFgCostCommand
            {
                RoomId = roomId,
                Timeout = 18000
                //DbTransaction = transaction
            };
            var result = await _unitOfWork.FgCostRepository.CalculateFgCostAsync(command) ?? throw new NullReferenceException("FgCost calculation does not return any result");
            
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.CommitAsync();
            }
            
            _logger.Information("FgCost {@command}", command);
            
            return new ServiceResponse<SpTaskDto>
            {
                Data = _mapper.MapToDto(result),
                Message = ServiceMessageConstants.FgCostCalculationFailed
            };
        }
        catch (Exception e)
        {
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.RollbackAsync();
            }
            
            _logger.Error(e, "CalculateFgCost failed {@command}", e.Message);
            
            return new ServiceResponse<SpTaskDto>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostCalculationFailed
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<FgCostSpDto>>> GetSummaryFgCostAsync(int roomId)
    {
        try
        {
            var result = await _unitOfWork.FgCostRepository.GetSummaryFgCostAsync(new GetSummaryFgCostCommand { RoomId = roomId });
            return new ServiceResponse<IEnumerable<FgCostSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.SummaryFgCostFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetSummaryFgCost failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<FgCostSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.SummaryFgCostNotFound
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<FgCostSpDto>>> GetSummaryFgCostPagedListAsync(PagedListRequestDto dto)
    {
        try
        {
            var command = new GetSummaryFgCostPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(dto)
            };
            var result = await _unitOfWork.FgCostRepository.GetSummaryFgCostPagedListAsync(command);
            return new ServiceResponse<PagedListDto<FgCostSpDto>>
            {
                Data = _mapper.MapToPagedListDto(result),
                Message = ServiceMessageConstants.SummaryFgCostFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetSummaryFgCostPagedList failed {@command}", e.Message);
            return new ServiceResponse<PagedListDto<FgCostSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.SummaryFgCostNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<FgCostDetailSpDto>>> GetFgCostDetailAsync(int roomId)
    {
        try
        {
            var result = await _unitOfWork.FgCostRepository.GetFgCostDetailsAsync(new GetFgCostDetailCommand { RoomId = roomId });
            return new ServiceResponse<IEnumerable<FgCostDetailSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.FgCostDetailFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetFgCostDetail failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<FgCostDetailSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostDetailNotFound
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<FgCostDetailSpDto>>> GetFgCostDetailPagedListAsync(PagedListRequestDto dto)
    {
        try
        {
            var command = new GetFgCostDetailPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(dto)
            };
            var result = await _unitOfWork.FgCostRepository.GetFgCostDetailsPagedListAsync(command);
            
            return new ServiceResponse<PagedListDto<FgCostDetailSpDto>>
            {
                Data = _mapper.MapToPagedListDto(result),
                Message = ServiceMessageConstants.FgCostDetailFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetFgCostDetailPagedList failed {@command}", e.Message);
            
            return new ServiceResponse<PagedListDto<FgCostDetailSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostDetailNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<FgCostDetailSpDto>>> GetFgCostDetailsByRofoIdFromSpAsync(int id)
    {
        try
        {
            var command = new GetFgCostDetailsByRofoIdFromSpCommand
            {
                RofoId = id
            };
            var result = await _unitOfWork.FgCostRepository.GetFgCostDetailsByRofoIdFromSpAsync(command);
            return new ServiceResponse<IEnumerable<FgCostDetailSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.FgCostDetailFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetFgCostDetailsByRofoIdFromSp failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<FgCostDetailSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostDetailNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<FgCostDetailSpDto>>> GetFgCostDetailsByRofoIdAsync(int rofoId)
    {
        try
        {
            var command = new GetFgCostDetailsByRofoIdCommand
            {
                RofoId = rofoId
            };
            var result = await _unitOfWork.FgCostRepository.GetFgCostDetailsByRofoIdAsync(command);
            return new ServiceResponse<IEnumerable<FgCostDetailSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.FgCostDetailFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetFgCostDetailsByRofoId failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<FgCostDetailSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostDetailNotFound
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<FgCostDetailSpDto>>> GetFgCostDetailsByRofoIdPagedListAsync(PagedListRequestDto dto)
    {
        try
        {
            var command = new GetFgCostDetailsByRofoIdPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(dto)
            };
            var result = await _unitOfWork.FgCostRepository.GetFgCostDetailsByRofoIdPagedListAsync(command);
            return new ServiceResponse<PagedListDto<FgCostDetailSpDto>>
            {
                Data = _mapper.MapToPagedListDto(result),
                Message = ServiceMessageConstants.FgCostDetailFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetFgCostDetailsByRofoIdPagedList failed {@command}", e.Message);
            
            return new ServiceResponse<PagedListDto<FgCostDetailSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostDetailNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetFgCostRoomIdsAsync()
    {
        try
        {
            var result = await _unitOfWork.FgCostRepository.GetFgCostRoomIdsAsync(new GetFgCostRoomIdsCommand());
            return new ServiceResponse<IEnumerable<int>>
            {
                Data = result,
                Message = ServiceMessageConstants.FgCostRoomIdsFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetFgCostRoomIds failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostRoomIdsNotFound
            };
        }
    }

    public string CalculateFgCostJob(int roomId)
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(CalculateFgCostAsync));
        
        // check if the room id is already in process
        if (processingJobsFiltered.Any(j => j.Value.Job.Args[0] as int? == roomId))
        {
            return ServiceMessageConstants.FgCostAlreadyInProcess;
        }
        
        var jobId = BackgroundJob.Enqueue<IFgCostService>(s => s.CalculateFgCostAsync(roomId));
        BackgroundJob.ContinueJobWith(jobId, () => NotifyClientsFgCostProcessingStateChanged());

        _ = NotifyClientsFgCostProcessingStateChanged();
        
        return ServiceMessageConstants.FgCostProcessing;
    }

    public IEnumerable<int> GetFgCostInProcessRoomIds()
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var processingJobs = monitoringApi.ProcessingJobs(0, 1000);
        var processingJobsFiltered = processingJobs.Where(j => j.Value.Job.Method.Name == nameof(CalculateFgCostAsync));
        
        // get the room ids
        return processingJobsFiltered.Select(j => j.Value.Job.Args[0] as int? ?? 0);
    }

    public async Task NotifyClientsFgCostProcessingStateChanged()
    {
        await _keycloakTokenService.Notify(new NotificationDto<IEnumerable<int>>
        {
            Method = "FgCostProcessingStateChanged",
            Data = GetFgCostInProcessRoomIds()
        });
    }
}