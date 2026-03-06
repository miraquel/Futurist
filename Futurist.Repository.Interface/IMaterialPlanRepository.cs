using Futurist.Domain;
using Futurist.Repository.Command.MaterialPlanCommand;

namespace Futurist.Repository.Interface;

public interface IMaterialPlanRepository
{
    Task<SpTask?> ProcessMaterialPlanAsync(ProcessMaterialPlanCommand command);
    Task<IEnumerable<int>> GetMaterialPlanRoomIdsAsync(GetMaterialPlanRoomIdsCommand command);
    Task<IEnumerable<int>> GetMaterialPlanVerIdsAsync(GetMaterialPlanVerIdsCommand command);
    Task<IEnumerable<int>> GetMaterialPlanYearsAsync(GetMaterialPlanYearsCommand command);
    Task<IEnumerable<int>> GetMaterialPlanMonthsAsync(GetMaterialPlanMonthsCommand command);
}
