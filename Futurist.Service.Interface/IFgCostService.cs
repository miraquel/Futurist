using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IFgCostService
{
    public Task<ServiceResponse<string?>> CalculateFgCostAsync(int roomId);
    public Task<ServiceResponse<IEnumerable<FgCostSpDto>>> GetSummaryFgCostAsync(int roomId);
    public Task<ServiceResponse<PagedListDto<FgCostSpDto>>> GetSummaryFgCostPagedListAsync(
        PagedListRequestDto<FgCostSpDto> dto);
    
    public Task<ServiceResponse<IEnumerable<FgCostDetailSpDto>>> GetFgCostDetailAsync(int roomId);
    public Task<ServiceResponse<PagedListDto<FgCostDetailSpDto>>> GetFgCostDetailPagedListAsync(PagedListRequestDto<FgCostDetailSpDto> dto);
    public Task<ServiceResponse<IEnumerable<FgCostDetailSpDto>>> GetFgCostDetailsByRofoIdAsync(int id);
    public Task<ServiceResponse<PagedListDto<FgCostDetailSpDto>>> GetFgCostDetailsByRofoIdPagedListAsync(PagedListRequestDto<FgCostDetailSpDto> dto);
    
    public Task<ServiceResponse<IEnumerable<int>>> GetFgCostRoomIdsAsync();
}