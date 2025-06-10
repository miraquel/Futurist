using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IReportVersionService
{
    Task<ServiceResponse<IEnumerable<FgCostVerSpDto>>> GetAllFgCostVerAsync(int room, int verId);
    Task<ServiceResponse<IEnumerable<FgCostVerDetailSpDto>>> GetAllFgCostVerDetailsByRofoIdAsync(int rofoId, int verId);
    Task<ServiceResponse<IEnumerable<MupVerSpDto>>> GetAllMupVerAsync(int room, int verId);
    Task<ServiceResponse<SpTaskDto>> InsertVersionAsync(int room, string notes);
    Task<ServiceResponse<IEnumerable<int>>> GetVersionRoomIdsAsync();
    Task<ServiceResponse<IEnumerable<VersionsDto>>> GetVersionsAsync(int room);
}