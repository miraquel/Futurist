using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.RofoCommand;

namespace Futurist.Repository.Interface;

public interface IRofoRepository
{
    // TODO: Implement the IRofoRepository
    Task<PagedList<Rofo>> GetRofoPagedListAsync(GetRofoPagedListCommand command);
    Task<IEnumerable<Rofo>> GetRofoListAsync(GetRofoListCommand command);
    Task<Rofo?> GetRofoByIdAsync(GetRofoByIdCommand command);
    Task DeleteRofoByRoomAsync(DeleteRofoByRoomCommand command);
    Task BulkInsertRofoAsync(BulkInsertRofoCommand command);
    Task<IEnumerable<int>> GetRofoRoomIdsAsync(GetRofoRoomIdsCommand command);
}