using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Hangfire.Storage.Monitoring;

namespace Futurist.Service.Interface;

public interface IJobMonitoringService
{
    Task<ServiceResponse<JobMonitoringDto>> GetJobMonitoringAsync(int id);

    Task<ServiceResponse<PagedListDto<JobMonitoringDto>>> GetJobMonitoringPagedListAsync(
        PagedListRequestDto request);
}