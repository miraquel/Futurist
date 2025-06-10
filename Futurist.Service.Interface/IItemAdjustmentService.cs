using Futurist.Service.Command.ItemAdjustmentCommand;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IItemAdjustmentService
{
    Task<ServiceResponse<IEnumerable<ItemAdjustmentDto>>> GetItemAdjustmentListAsync(int room);
    Task<ServiceResponse<IEnumerable<int>>> GetItemAdjustmentRoomIdsAsync();
    Task<ServiceResponse<int>> ImportAsync(ImportCommand serviceCommand);
}