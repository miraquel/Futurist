using Futurist.Repository.Command.JobMonitoringCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Serilog;

namespace Futurist.Service;

public class JobMonitoringService : IJobMonitoringService
{
    private readonly MapperlyMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger = Log.ForContext<JobMonitoringService>();

    public JobMonitoringService(MapperlyMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<JobMonitoringDto>> GetJobMonitoringAsync(int id)
    {
        try
        {
            var command = new GetJobMonitoringCommand
            {
                Id = id
            };
            var result = await _unitOfWork.JobMonitoringRepository.GetJobMonitoringAsync(command);
            
            if (result is null)
            {
                throw new NullReferenceException("Job Monitoring not found");
            }
            
            return new ServiceResponse<JobMonitoringDto>
            {
                Data = _mapper.MapToDto(result),
                Message = ServiceMessageConstants.JobMonitoringFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetJobMonitoringAsync failed {@command}", e.Message);
            
            return new ServiceResponse<JobMonitoringDto>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.JobMonitoringNotFound
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<JobMonitoringDto>>> GetJobMonitoringPagedListAsync(PagedListRequestDto request)
    {
        try
        {
            var command = new GetJobMonitoringPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(request)
            };
            var result = await _unitOfWork.JobMonitoringRepository.GetJobMonitoringPagedListAsync(command);
            // merge the same job id to see the time start and end
            return new ServiceResponse<PagedListDto<JobMonitoringDto>>
            {
                Data = _mapper.MapToPagedListDto(result),
                Message = ServiceMessageConstants.JobMonitoringFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetJobMonitoringPagedListAsync failed {@command}", e.Message);
            
            return new ServiceResponse<PagedListDto<JobMonitoringDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.JobMonitoringNotFound
            };
        }
    }
}