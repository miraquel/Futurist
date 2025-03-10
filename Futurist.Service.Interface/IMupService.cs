using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IMupService
{
    Task<ServiceResponse<IEnumerable<MupSpDto>>> ProcessMupAsync(int roomId);
    Task<ServiceResponse<IEnumerable<MupSpDto>>> MupResultAsync(int roomId);
    Task<ServiceResponse<PagedListDto<MupSpDto>>> MupResultPagedListAsync(PagedListRequestDto<MupSpDto> filter);
    Task<ServiceResponse<IEnumerable<int>>> GetMupRoomIdsAsync();
    
    // Hangfire Job to execute ProcessMupAsync
    string ProcessMupJob(int roomId);
    // Check the status of the Hangfire Job
    IEnumerable<int> MupInProcessRoomIds();
    Task NotifyClientsMupProcessingStateChanged();
}