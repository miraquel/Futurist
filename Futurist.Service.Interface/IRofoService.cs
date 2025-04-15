using Futurist.Service.Command.RofoCommand;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IRofoService
{
    Task<ServiceResponse<PagedListDto<RofoDto>>> GetPagedListAsync(PagedListRequestDto filter);
    Task<ServiceResponse<IEnumerable<RofoDto>>> GetRofoListAsync(int room);
    Task<ServiceResponse<RofoDto>> GetByIdAsync(int rofoId);
    Task<ServiceResponse> ImportAsync(ImportCommand serviceCommand);
    Task<ServiceResponse<IEnumerable<int>>> GetRofoRoomIdsAsync();
}