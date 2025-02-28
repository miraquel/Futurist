using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IMupService
{
    Task<ServiceResponse<IEnumerable<MupSpDto>>> ProcessMupAsync(int roomId);
    Task<ServiceResponse<IEnumerable<MupSpDto>>> MupResultAsync(int roomId);
    Task<ServiceResponse<IEnumerable<int>>> GetRoomIdsAsync();
    
    // Hangfire Job to execute ProcessMupAsync
    void ProcessMupJob(int roomId);
    // Check the status of the Hangfire Job
    IEnumerable<int> MupInProcessRoomIds();
}