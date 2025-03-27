using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.JobMonitoringCommand;

namespace Futurist.Repository.Interface;

public interface IJobMonitoringRepository
{
    Task<PagedList<JobMonitoring>> GetJobMonitoringPagedListAsync(GetJobMonitoringPagedListCommand command);
    Task<JobMonitoring?> GetJobMonitoringAsync(GetJobMonitoringCommand command);
}