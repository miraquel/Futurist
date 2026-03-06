using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IBomPlanService
{
    Task<ServiceResponse<SpTaskDto>> ProcessBomPlanAsync(int roomId, int verId, int year, int month);
    Task<ServiceResponse<IEnumerable<int>>> GetBomPlanRoomIdsAsync();
    Task<ServiceResponse<IEnumerable<int>>> GetBomPlanVerIdsAsync(int roomId);
    Task<ServiceResponse<IEnumerable<int>>> GetBomPlanYearsAsync(int roomId, int verId);
    Task<ServiceResponse<IEnumerable<int>>> GetBomPlanMonthsAsync(int roomId, int verId, int year);

    // Hangfire Job to execute ProcessBomPlanAsync
    string ProcessBomPlanJob(int roomId, int verId, int year, int month);
    // Check the status of the Hangfire Job
    IEnumerable<int> BomPlanInProcessRoomIds();
    Task NotifyClientsBomPlanProcessingStateChanged();
}
