using Futurist.Domain;
using Futurist.Repository.Command.ItemForecast;

namespace Futurist.Repository.Interface;

public interface IItemForecastRepository
{
    Task<IEnumerable<ItemForecastSp>> GetItemForecastListAsync(GetItemForecastListCommand command);
    Task InsertItemForecastAsync(InsertItemForecastCommand command);
    Task<IEnumerable<int>> GetItemForecastRoomIdsAsync(GetItemForecastRoomIdsCommand command);
}