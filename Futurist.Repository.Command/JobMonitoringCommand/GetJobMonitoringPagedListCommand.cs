using Futurist.Domain.Common;

namespace Futurist.Repository.Command.JobMonitoringCommand;

public class GetJobMonitoringPagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; set; } = new();
}