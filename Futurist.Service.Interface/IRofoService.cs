using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IRofoService
{
    Task<ServiceResponse<PagedListDto<RofoDto>>> GetPagedListAsync(PagedListRequestDto<RofoDto> filter);
    Task<ServiceResponse<RofoDto>> GetByIdAsync(int rofoId);
    Task<ServiceResponse> ImportAsync(Stream stream);
    Task<ServiceResponse<IEnumerable<int>>> GetRofoRoomIdsAsync();
}