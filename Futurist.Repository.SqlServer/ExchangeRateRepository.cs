using System.Data;
using Dapper;
using Futurist.Common.Helpers;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.ExchangeRateCommand;
using Futurist.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Futurist.Repository.SqlServer;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly IDbConnection _sqlConnection;

    public ExchangeRateRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task BulkInsertExchangeRateAsync(BulkInsertExchangeRateCommand command)
    {
        // insert new rofo
        var sqlServerConnection = _sqlConnection as SqlConnection ?? throw new InvalidOperationException("Invalid SQL connection");
        
        using var sqlBulkCopy = command.DbTransaction is SqlTransaction sqlServerTransaction ? new SqlBulkCopy(sqlServerConnection, SqlBulkCopyOptions.Default, sqlServerTransaction) : new SqlBulkCopy(sqlServerConnection);
        
        sqlBulkCopy.DestinationTableName = "ExchangeRate";
        sqlBulkCopy.BatchSize = 1000;

        var dataTable = new DataTable();
        dataTable.Columns.Add("RecId", typeof(int));
        dataTable.Columns.Add("CurrencyCode", typeof(string));
        dataTable.Columns.Add("ValidFrom", typeof(DateTime));
        dataTable.Columns.Add("ValidTo", typeof(DateTime));
        dataTable.Columns.Add("ExchangeRate", typeof(decimal));
        dataTable.Columns.Add("CreatedDate", typeof(DateTime));
        dataTable.Columns.Add("CreatedBy", typeof(string));
        
        foreach (var item in command.ExchangeRates)
        {
            dataTable.Rows.Add(0, item.CurrencyCode, item.ValidFrom, item.ValidTo, item.ExchangeRate, item.CreatedDate, item.CreatedBy);
        }
        
        await sqlBulkCopy.WriteToServerAsync(dataTable);
    }

    public async Task<PagedList<ExchangeRateSp>> GetExchangeRatePagedListAsync(GetExchangeRatePagedListCommand command)
    {
        const string query = """
                             SELECT [CurrencyCode]
                             	,[ValidFrom]
                             	,[ValidTo]
                             	,[ExchangeRate]
                             FROM [ExchangeRate]
                             /**where**/
                             /**orderby**/
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                             """;
        
        const string queryCount = """
                                  SELECT COUNT(*)
                                  FROM [ExchangeRate]
                                  /**where**/
                                  """;
        
        var pagedListRequest = command.PagedListRequest;

        var sqlBuilder = new SqlBuilder();
        
        if (pagedListRequest.Filters.TryGetValue(nameof(ExchangeRateSp.CurrencyCode), out var currencyCodeFilter))
        {
            if (currencyCodeFilter.Contains('*') || currencyCodeFilter.Contains('%'))
            {
                currencyCodeFilter = currencyCodeFilter.Replace('*', '%');
                sqlBuilder.Where($"CurrencyCode LIKE @CurrencyCode", new { CurrencyCode = currencyCodeFilter });
            }
            else
            {
                sqlBuilder.Where($"CurrencyCode = @CurrencyCode", new { CurrencyCode = currencyCodeFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(ExchangeRateSp.ValidFrom), out var validFromFilter) && DateTime.TryParse(validFromFilter, out var validFrom))
        {
            sqlBuilder.Where($"ValidFrom = @ValidFrom", new { ValidFrom = validFrom });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(ExchangeRateSp.ValidTo), out var validToFilter) && DateTime.TryParse(validToFilter, out var validTo))
        {
            sqlBuilder.Where($"ValidTo = @ValidTo", new { ValidTo = validTo });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(ExchangeRateSp.ExchangeRate), out var exchangeRateFilter))
        {
            if (decimal.TryParse(exchangeRateFilter, out var exchangeRate))
            {
                sqlBuilder.Where($"ExchangeRate = @ExchangeRate", new { ExchangeRate = exchangeRate });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(exchangeRateFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"ExchangeRate {match.Groups[1].Value} @ExchangeRate", new { ExchangeRate = match.Groups[2].Value });
                }
            }
        }
        
        var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
        sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
            ? $"RecId {sort}"
            : $"{pagedListRequest.SortBy} {sort}");
        
        sqlBuilder.AddParameters(new
            { Offset = (pagedListRequest.PageNumber - 1) * pagedListRequest.PageSize, pagedListRequest.PageSize });
        
        var queryTemplate = sqlBuilder.AddTemplate(query);
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var exchangeRates = await _sqlConnection.QueryAsync<ExchangeRateSp>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        queryTemplate = sqlBuilder.AddTemplate(queryCount);
        var exchangeRateCount = await _sqlConnection.ExecuteScalarAsync<int>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        return new PagedList<ExchangeRateSp>(exchangeRates, pagedListRequest.PageNumber, pagedListRequest.PageSize, exchangeRateCount);
    }

    public async Task<ExchangeRateSp?> GetExchangeRateAsync(GetExchangeRateCommand command)
    {
        const string query = """
                             SELECT [CurrencyCode]
                             	,[ValidFrom]
                             	,[ValidTo]
                             	,[ExchangeRate]
                             FROM [ExchangeRate]
                             WHERE [CurrencyCode] = @CurrencyCode AND [ValidFrom] = @ValidFrom AND [ValidTo] = @ValidTo;
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryFirstOrDefaultAsync<ExchangeRateSp>(query, new { command.CurrencyCode, command.ValidFrom, command.ValidTo }, command.DbTransaction);
    }

    public async Task<IEnumerable<ExchangeRateSp>> GetAllExchangeRateAsync(GetAllExchangeRateCommand command)
    {
        const string query = "ExchangeRate_Select";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<ExchangeRateSp>(query, command.DbTransaction, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteExchangeRateAsync(DeleteExchangeRateCommand command)
    {
        const string query = "DELETE FROM [ExchangeRate] WHERE [CurrencyCode] = @CurrencyCode AND [ValidFrom] = @ValidFrom";
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        await _sqlConnection.ExecuteAsync(query, new { command.CurrencyCode, command.ValidFrom, command.ValidTo }, command.DbTransaction);
    }
}