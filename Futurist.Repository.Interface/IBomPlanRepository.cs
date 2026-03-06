using Futurist.Domain;
using Futurist.Repository.Command.BomPlanCommand;

namespace Futurist.Repository.Interface;

public interface IBomPlanRepository
{
    Task ProcessBomPlanAsync(ProcessBomPlanCommand command);
    Task<IEnumerable<int>> GetBomPlanRoomIdsAsync(GetBomPlanRoomIdsCommand command);
    Task<IEnumerable<int>> GetBomPlanVerIdsAsync(GetBomPlanVerIdsCommand command);
    Task<IEnumerable<int>> GetBomPlanYearsAsync(GetBomPlanYearsCommand command);
    Task<IEnumerable<int>> GetBomPlanMonthsAsync(GetBomPlanMonthsCommand command);
}
