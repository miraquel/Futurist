using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IFgCostVerService
{
    Task<ServiceResponse<IEnumerable<FgCostVerSpDto>>> GetAllFgCostVerAsync(int room);
    Task<ServiceResponse<SpTaskDto>> InsertFgCostVerAsync(int room, string notes);
    Task<ServiceResponse<IEnumerable<int>>> GetFgCostVerRoomIdsAsync();
}