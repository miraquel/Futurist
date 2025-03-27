using Futurist.Domain.Common;

namespace Futurist.Repository.Command.ItemAdjustmentCommand;

public class GetItemAdjustmentListCommand : BaseCommand
{
    public int Room { get; set; }
}