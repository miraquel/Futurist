using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IBomStdService
{
    Task<ServiceResponse> ProcessBomStdAsync(int roomId);
    Task<ServiceResponse<IEnumerable<BomStdDto>>> BomErrorCheckAsync(int roomId);
    Task<ServiceResponse<PagedListDto<BomStdDto>>> BomErrorCheckPagedListAsync(PagedListRequestDto<BomStdDto> filter);
    Task<ServiceResponse<IEnumerable<int>>> GetBomStdRoomIdsAsync();

    string ProcessBomStdJob(int roomId);
    IEnumerable<int> GetBomStdInProcessRoomIds();
    Task NotifyClientsBomStdProcessingStateChanged();
}