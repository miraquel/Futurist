using Futurist.Domain;
using Futurist.Repository.Command.FgCostVerCommand;

namespace Futurist.Repository.Interface;

public interface IFgCostVerRepository
{
    Task<IEnumerable<FgCostVerSp>> GetAllFgCostVerAsync(GetAllFgCostVerCommand command);
    Task<SpTask?> InsertFgCostVerAsync(InsertFgCostVerCommand command);
    Task<IEnumerable<int>> GetFgCostVerRoomIdsAsync(GetFgCostVerRoomIdsCommand command);
}