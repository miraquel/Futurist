using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IMaterialActService
{
    Task<ServiceResponse<SpTaskDto>> ProcessMaterialActAsync(int year, int month);
    Task<ServiceResponse<IEnumerable<int>>> GetMaterialActYearsAsync();
    Task<ServiceResponse<IEnumerable<int>>> GetMaterialActMonthsAsync(int year);
    
    // Hangfire Job to execute ProcessMaterialActAsync
    string ProcessMaterialActJob(int year, int month);
    // Check the status of the Hangfire Job
    IEnumerable<string> MaterialActInProcessYearMonths();
    Task NotifyClientsMaterialActProcessingStateChanged();
}
