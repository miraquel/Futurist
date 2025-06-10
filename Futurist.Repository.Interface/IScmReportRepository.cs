using Futurist.Domain;
using Futurist.Repository.Command.ScmReportCommand;

namespace Futurist.Repository.Interface;

public interface IScmReportRepository
{
    // Domestic Reports
    Task<IEnumerable<ScmReport>> GetDomesticByProductCustomer(GetDomesticByProductCustomerCommand command);
    Task<IEnumerable<ScmReport>> GetDomesticByProduct(GetDomesticByProductCommand command);
    Task<IEnumerable<ScmReport>> GetDomesticByCustomer(GetDomesticByCustomerCommand command);
    Task<IEnumerable<ScmReport>> GetDomesticRawData(GetDomesticRawDataCommand command);
    
    // Export Reports
    Task<IEnumerable<ScmReport>> GetExportByProductCustomer(GetExportByProductCustomerCommand command);
    Task<IEnumerable<ScmReport>> GetExportByProduct(GetExportByProductCommand command);
    Task<IEnumerable<ScmReport>> GetExportByCustomer(GetExportByCustomerCommand command);
    Task<IEnumerable<ScmReport>> GetExportRawData(GetExportRawDataCommand command);
    
    // Additional methods for years and months
    Task<IEnumerable<int>> GetYearsAsync();
    Task<IEnumerable<int>> GetMonthsAsync(int year);
}