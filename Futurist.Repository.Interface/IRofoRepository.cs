using Futurist.Domain;
using Futurist.Domain.Common;

namespace Futurist.Repository.Interface;

public interface IRofoRepository
{
    // TODO: Implement the IRofoRepository
    Task<PagedList<Rofo>> GetPagedListAsync(PagedListRequest<Rofo> filter);
    Task<Rofo?> GetByIdAsync(int id);
    Task DeleteRofoByRoomAsync(int roomId);
    Task BulkInsertRofoAsync(IEnumerable<Rofo> rofo);
    Task<IEnumerable<int>> GetRoomIdsAsync();
}