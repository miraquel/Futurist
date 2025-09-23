using Futurist.Domain;
using Futurist.Repository.Command.RmForecastCommand;

namespace Futurist.Repository.Interface;

public interface IRmForecastRepository
{
    Task<IEnumerable<RmForecast>> GetAllAsync(GetAllCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<int>> GetYearsAsync(GetYearsCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<int>> GetRoomIdsAsync(GetRoomIdsCommand command, CancellationToken cancellationToken);
}