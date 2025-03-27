using Futurist.Domain.Common;

namespace Futurist.Repository.Command.MupCommand;

public class MupSummaryByBatchNumberCommand : BaseCommand
{
    public ListRequest ListRequest { get; set; } = new();
}