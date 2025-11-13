using Futurist.Domain;
using Futurist.Repository.Command.AnlRmCommand;

namespace Futurist.Repository.Interface;

public interface IAnlRmRepository
{
    Task<IEnumerable<AnlRmPrice>> GetAnlRmpPrice(GetAnlRmPriceCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<AnlKurs>> GetAnlKursAsync(GetAnlKursCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<AnlFgPrice>> GetAnlFgPriceAsync(GetAnlFgPriceCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<AnlPmPrice>> GetAnlPmPriceAsync(GetAnlPmPriceCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<int>> GetRoomIdsAsync(GetRoomIdsCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<int>> GetRofoVerIdsAsync(GetRofoVerIdsCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<int>> GetYearsAsync(GetYearsCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<int>> GetMonthsAsync(GetMonthsCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<int>> GetVerIdsAsync(GetVerIdsCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<AnlRmPriceGroup>> GetAnlRmPriceGroupAsync(GetAnlRmPriceGroupCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<AnlCostPrice>> GetAnlCostPriceAsync(GetAnlCostPriceCommand command, CancellationToken cancellationToken);
}