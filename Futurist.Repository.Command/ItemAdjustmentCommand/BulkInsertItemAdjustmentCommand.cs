using Futurist.Domain;

namespace Futurist.Repository.Command.ItemAdjustmentCommand;

public class BulkInsertItemAdjustmentCommand : BaseCommand
{
    public IEnumerable<ItemAdjustment> ItemAdjustments { get; set; } = new List<ItemAdjustment>();
}