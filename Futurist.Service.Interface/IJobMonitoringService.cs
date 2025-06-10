using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IJobMonitoringService
{
    Task<ServiceResponse<JobMonitoringDto>> GetJobMonitoringAsync(int id);

    Task<ServiceResponse<PagedListDto<JobMonitoringDto>>> GetJobMonitoringPagedListAsync(
        PagedListRequestDto request);
}