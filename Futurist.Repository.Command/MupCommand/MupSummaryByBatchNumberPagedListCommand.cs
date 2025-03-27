using Futurist.Domain.Common;

namespace Futurist.Repository.Command.MupCommand;

public class MupSummaryByBatchNumberPagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; set; } = new();
}