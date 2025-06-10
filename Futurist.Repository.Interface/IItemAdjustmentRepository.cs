using Futurist.Domain;
using Futurist.Repository.Command.ItemAdjustmentCommand;

namespace Futurist.Repository.Interface;

public interface IItemAdjustmentRepository
{
    public Task<IEnumerable<ItemAdjustment>> GetItemAdjustmentListAsync(GetItemAdjustmentListCommand command);
    public Task<IEnumerable<int>> GetItemAdjustmentRoomIdsAsync(GetItemAdjustmentRoomIdsCommand command);
    public Task<string?> DeleteItemAdjustmentAsync(DeleteItemAdjustmentCommand command);
    public Task BulkInsertItemAdjustmentAsync(BulkInsertItemAdjustmentCommand command);
}