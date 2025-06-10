using Futurist.Service.Command.ItemForecastCommand;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IItemForecastService
{
    Task<ServiceResponse<IEnumerable<ItemForecastSpDto>>> GetItemForecastListAsync(int room);
    Task<ServiceResponse<int>> ImportAsync(ImportCommand serviceCommand);
    Task<ServiceResponse<IEnumerable<int>>> GetItemForecastRoomIdsAsync();
}