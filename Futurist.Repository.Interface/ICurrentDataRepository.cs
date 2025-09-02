using Futurist.Domain;
using Futurist.Repository.Command.CurrentDataCommands;

namespace Futurist.Repository.Interface;

public interface ICurrentDataRepository
{
    // - Stock On Hand = SP ItemOnhand_Select
    // - PO Intransit = SP ItemPoIntransit_Select
    // - Contract = SP ItemPag_Select
    
    Task<IEnumerable<ItemOnHand>> GetItemOnHandAsync(GetItemOnHandCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<ItemPoIntransit>> GetItemPoIntransitAsync(GetItemPoIntransitCommand command, CancellationToken cancellationToken);
    Task<IEnumerable<ItemPag>> GetItemPagAsync(GetItemPagCommand command, CancellationToken cancellationToken);
}