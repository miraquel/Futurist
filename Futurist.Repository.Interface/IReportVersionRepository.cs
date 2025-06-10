using Futurist.Domain;
using Futurist.Repository.Command.ReportVersionCommand;

namespace Futurist.Repository.Interface;

public interface IReportVersionRepository
{
    Task<IEnumerable<FgCostVerSp>> GetAllFgCostVerAsync(GetAllFgCostVerCommand command);
    Task<IEnumerable<FgCostVerDetailSp>> GetAllFgCostVerDetailsByRofoIdAsync(GetAllFgCostVerDetailsCommand command);
    Task<IEnumerable<MupVerSp>> GetAllMupVerAsync(GetAllMupVerCommand command);
    Task<SpTask?> InsertVersionAsync(InsertVersionCommand command);
    Task<IEnumerable<int>> GetVersionRoomIdsAsync(GetVersionRoomIdsCommand command);
    Task<IEnumerable<Versions>> GetVersionsAsync(GetVersionsCommand command);
}