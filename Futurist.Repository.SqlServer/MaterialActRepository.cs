using System.Data;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.MaterialActCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class MaterialActRepository : IMaterialActRepository
{
    private readonly IDbConnection _sqlConnection;

    public MaterialActRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<SpTask?> ProcessMaterialActAsync(ProcessMaterialActCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON;");
        const string query = "EXEC MaterialAct_Calc @Year, @Month";
        return await _sqlConnection.QuerySingleOrDefaultAsync<SpTask>(
            query,
            new { command.Year, command.Month },
            command.DbTransaction,
            commandTimeout: command.Timeout);
    }

    public async Task<IEnumerable<int>> GetMaterialActYearsAsync(GetMaterialActYearsCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON;");
        const string query = "SELECT DISTINCT YEAR(INVOICEDATE) AS Year FROM [AXGMKDW].[dbo].[FactSalesInvoice] ORDER BY Year DESC";
        return await _sqlConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }

    public async Task<IEnumerable<int>> GetMaterialActMonthsAsync(GetMaterialActMonthsCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON;");
        const string query = "SELECT DISTINCT MONTH(INVOICEDATE) AS Month FROM [AXGMKDW].[dbo].[FactSalesInvoice] WHERE YEAR(INVOICEDATE) = @Year ORDER BY Month DESC";
        return await _sqlConnection.QueryAsync<int>(query, new { command.Year }, transaction: command.DbTransaction);
    }
}
