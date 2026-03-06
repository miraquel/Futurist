using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IMaterialPlanService
{
    Task<ServiceResponse<SpTaskDto>> ProcessMaterialPlanAsync(int roomId, int verId, int year, int month);
    Task<ServiceResponse<IEnumerable<int>>> GetMaterialPlanRoomIdsAsync();
    Task<ServiceResponse<IEnumerable<int>>> GetMaterialPlanVerIdsAsync(int roomId);
    Task<ServiceResponse<IEnumerable<int>>> GetMaterialPlanYearsAsync(int roomId, int verId);
    Task<ServiceResponse<IEnumerable<int>>> GetMaterialPlanMonthsAsync(int roomId, int verId, int year);
    
    // Hangfire Job to execute ProcessMaterialPlanAsync
    string ProcessMaterialPlanJob(int roomId, int verId, int year, int month);
    // Check the status of the Hangfire Job
    IEnumerable<int> MaterialPlanInProcessRoomIds();
    Task NotifyClientsMaterialPlanProcessingStateChanged();
}
