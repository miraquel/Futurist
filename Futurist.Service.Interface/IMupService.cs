using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IMupService
{
    Task<ServiceResponse<SpTaskDto>> ProcessMupAsync(int roomId);
    Task<ServiceResponse<IEnumerable<MupSpDto>>> MupResultAsync(int roomId);
    Task<ServiceResponse<PagedListDto<MupSpDto>>> MupResultPagedListAsync(PagedListRequestDto filter);
    Task<ServiceResponse<IEnumerable<int>>> GetMupRoomIdsAsync();
    Task<ServiceResponse<IEnumerable<MupSpDto>>> MupSummaryByItemIdFromSpAsync(int roomId);
    Task<ServiceResponse<IEnumerable<MupSpDto>>> MupSummaryByItemIdAsync(ListRequestDto filter);
    Task<ServiceResponse<PagedListDto<MupSpDto>>> MupSummaryByItemIdPagedListAsync(PagedListRequestDto filter);
    Task<ServiceResponse<IEnumerable<MupSpDto>>> MupSummaryByBatchNumberFromSpAsync(int roomId);
    Task<ServiceResponse<IEnumerable<MupSpDto>>> MupSummaryByBatchNumberAsync(ListRequestDto filter);
    Task<ServiceResponse<PagedListDto<MupSpDto>>> MupSummaryByBatchNumberPagedListAsync(
        PagedListRequestDto filter);
    
    // Hangfire Job to execute ProcessMupAsync
    string ProcessMupJob(int roomId);
    // Check the status of the Hangfire Job
    IEnumerable<int> MupInProcessRoomIds();
    Task NotifyClientsMupProcessingStateChanged();
}