using System.Data;
using System.Reflection;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.ScmReportCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class ScmReportRepository : IScmReportRepository
{
    private readonly IDbConnection _dbConnection;

    public ScmReportRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<ScmReport>> GetDomesticByProductCustomer(GetDomesticByProductCustomerCommand command)
    {
        const string storedProcedure = "Scm_SelectDomByProductCustomer";
        var parameters = new DynamicParameters();
        parameters.Add("@PeriodDate", command.PeriodeDate, DbType.DateTime);
        CustomMapping();
        return await _dbConnection.QueryAsync<ScmReport>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
     }

    public async Task<IEnumerable<ScmReport>> GetDomesticByProduct(GetDomesticByProductCommand command)
    {
        const string storedProcedure = "Scm_SelectDomByProduct";
        var parameters = new DynamicParameters();
        parameters.Add("@PeriodDate", command.PeriodeDate, DbType.DateTime);
        CustomMapping();
        return await _dbConnection.QueryAsync<ScmReport>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ScmReport>> GetDomesticByCustomer(GetDomesticByCustomerCommand command)
    {
        const string storedProcedure = "Scm_SelectDomByCustomer";
        var parameters = new DynamicParameters();
        parameters.Add("@PeriodDate", command.PeriodeDate, DbType.DateTime);
        CustomMapping();
        return await _dbConnection.QueryAsync<ScmReport>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ScmReport>> GetDomesticRawData(GetDomesticRawDataCommand command)
    {
        const string storedProcedure = "Scm_SelectDomRaw";
        var parameters = new DynamicParameters();
        parameters.Add("@PeriodDate", command.PeriodeDate, DbType.DateTime);
        CustomMapping();
        return await _dbConnection.QueryAsync<ScmReport>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ScmReport>> GetExportByProductCustomer(GetExportByProductCustomerCommand command)
    {
        const string storedProcedure = "Scm_SelectExpByProductCustomer";
        var parameters = new DynamicParameters();
        parameters.Add("@PeriodDate", command.PeriodeDate, DbType.DateTime);
        CustomMapping();
        return await _dbConnection.QueryAsync<ScmReport>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ScmReport>> GetExportByProduct(GetExportByProductCommand command)
    {
        const string storedProcedure = "Scm_SelectExpByProduct";
        var parameters = new DynamicParameters();
        parameters.Add("@PeriodDate", command.PeriodeDate, DbType.DateTime);
        CustomMapping();
        return await _dbConnection.QueryAsync<ScmReport>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ScmReport>> GetExportByCustomer(GetExportByCustomerCommand command)
    {
        const string storedProcedure = "Scm_SelectExpByCustomer";
        var parameters = new DynamicParameters();
        parameters.Add("@PeriodDate", command.PeriodeDate, DbType.DateTime);
        CustomMapping();
        return await _dbConnection.QueryAsync<ScmReport>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ScmReport>> GetExportRawData(GetExportRawDataCommand command)
    {
        const string storedProcedure = "Scm_SelectExpRaw";
        var parameters = new DynamicParameters();
        parameters.Add("@PeriodDate", command.PeriodeDate, DbType.DateTime);
        CustomMapping();
        return await _dbConnection.QueryAsync<ScmReport>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<int>> GetYearsAsync()
    {
        const string sql = """
                           SELECT DISTINCT YEAR(INVOICEDATE) AS Year
                           FROM [SCMBI].[dbo].[FactSalesInvoice]
                           ORDER BY Year DESC;
                           """;
        
        return await _dbConnection.QueryAsync<int>(sql);
    }

    public async Task<IEnumerable<int>> GetMonthsAsync(int year)
    {
        const string sql = """
                           SELECT DISTINCT MONTH(INVOICEDATE) AS Month
                           FROM [SCMBI].[dbo].[FactSalesInvoice]
                           WHERE YEAR(INVOICEDATE) = @Year
                           ORDER BY Month;
                           """;
        
        var parameters = new { Year = year };
        return await _dbConnection.QueryAsync<int>(sql, parameters);
    }

    private static void CustomMapping()
    {
        Dictionary<string, string> columnMappings = new()
        {
            { "PeriodDate", nameof(ScmReport.PeriodDate) },
            { "INVOICEDATE", nameof(ScmReport.InvoiceDate) },
            { "INVOICEID", nameof(ScmReport.InvoiceId) },
            { "CUSTID", nameof(ScmReport.CustId) },
            { "CUSTNAME", nameof(ScmReport.CustName) },
            { "DIVISI", nameof(ScmReport.Divisi) },
            { "BUSINESSUNIT", nameof(ScmReport.BusinessUnit) },
            { "ITEMID", nameof(ScmReport.ItemId) },
            { "ITEMNAME", nameof(ScmReport.ItemName) },
            { "BRAND", nameof(ScmReport.Brand) },
            { "INVENTBATCHID", nameof(ScmReport.InventBatchId) },
            { "QTY", nameof(ScmReport.Qty) },
            { "QTYINKG", nameof(ScmReport.QtyInKg) },
            { "SALESAMOUNT", nameof(ScmReport.SalesAmount) },
            { "RMPMAMOUNT", nameof(ScmReport.RmpmAmount) },
            { "STDCOST", nameof(ScmReport.StdCost) },
            { "%RMPM", nameof(ScmReport.RmpmPercentage) }
        };

        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));

        SqlMapper.SetTypeMap(typeof(ScmReport), new CustomPropertyTypeMap(
            typeof(ScmReport),
            (type, columnName) => mapper(type, columnName)!));
    }
}