using Futurist.Domain;
using Futurist.Repository.Command.MaterialActCommand;

namespace Futurist.Repository.Interface;

public interface IMaterialActRepository
{
    Task<SpTask?> ProcessMaterialActAsync(ProcessMaterialActCommand command);
    Task<IEnumerable<int>> GetMaterialActYearsAsync(GetMaterialActYearsCommand command);
    Task<IEnumerable<int>> GetMaterialActMonthsAsync(GetMaterialActMonthsCommand command);
}
