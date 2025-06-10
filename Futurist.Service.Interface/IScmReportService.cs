using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IScmReportService
{
    // Domestic Reports
    Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetDomesticByProductCustomer(DateTime periodeDate);
    Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetDomesticByProduct(DateTime periodeDate);
    Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetDomesticByCustomer(DateTime periodeDate);
    Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetDomesticRawData(DateTime periodeDate);
    
    // Export Reports
    Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetExportByProductCustomer(DateTime periodeDate);
    Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetExportByProduct(DateTime periodeDate);
    Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetExportByCustomer(DateTime periodeDate);
    Task<ServiceResponse<IEnumerable<ScmReportDto>>> GetExportRawData(DateTime periodeDate);
    Task<IEnumerable<int>> GetYearsAsync();
    Task<IEnumerable<int>> GetMonthsAsync(int year);
}