using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IFgCostService
{
    public Task<ServiceResponse<SpTaskDto>> CalculateFgCostAsync(int roomId);
    public Task<ServiceResponse<IEnumerable<FgCostSpDto>>> GetSummaryFgCostAsync(int roomId);
    public Task<ServiceResponse<PagedListDto<FgCostSpDto>>> GetSummaryFgCostPagedListAsync(
        PagedListRequestDto dto);
    
    public Task<ServiceResponse<IEnumerable<FgCostDetailSpDto>>> GetFgCostDetailAsync(int roomId);
    public Task<ServiceResponse<PagedListDto<FgCostDetailSpDto>>> GetFgCostDetailPagedListAsync(PagedListRequestDto dto);
    public Task<ServiceResponse<IEnumerable<FgCostDetailSpDto>>> GetFgCostDetailsByRofoIdFromSpAsync(int id);
    public Task<ServiceResponse<IEnumerable<FgCostDetailSpDto>>> GetFgCostDetailsByRofoIdAsync(int id);
    public Task<ServiceResponse<PagedListDto<FgCostDetailSpDto>>> GetFgCostDetailsByRofoIdPagedListAsync(PagedListRequestDto dto);
    
    public Task<ServiceResponse<IEnumerable<int>>> GetFgCostRoomIdsAsync();
    
    public string CalculateFgCostJob(int roomId);
    public IEnumerable<int> GetFgCostInProcessRoomIds();
    public Task NotifyClientsFgCostProcessingStateChanged();
}