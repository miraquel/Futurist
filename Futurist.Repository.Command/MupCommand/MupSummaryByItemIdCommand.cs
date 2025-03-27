using Futurist.Domain.Common;

namespace Futurist.Repository.Command.MupCommand;

public class MupSummaryByItemIdCommand : BaseCommand
{
    public ListRequest ListRequest { get; set; } = new();
}