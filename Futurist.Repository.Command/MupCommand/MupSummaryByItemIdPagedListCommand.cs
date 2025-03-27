using Futurist.Domain.Common;

namespace Futurist.Repository.Command.MupCommand;

public class MupSummaryByItemIdPagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; set; } = new();
}