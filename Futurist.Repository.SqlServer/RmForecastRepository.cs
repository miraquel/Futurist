using System.Data;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.RmForecastCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class RmForecastRepository : IRmForecastRepository
{
    private readonly IDbConnection _dbConnection;

    public RmForecastRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<RmForecast>> GetAllAsync(GetAllCommand command, CancellationToken cancellationToken)
    {
        const string storedProcedure = "Rpt_RmForecast";
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room, DbType.Int32);
        parameters.Add("@Year", command.Year, DbType.Int32);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            storedProcedure,
            parameters,
            command.DbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        return await _dbConnection.QueryAsync<RmForecast>(sqlCommand);
    }

    public async Task<IEnumerable<int>> GetYearsAsync(GetYearsCommand command, CancellationToken cancellationToken)
    {
        const string query = """
                             SELECT DISTINCT 
                                 YEAR(ForecastDate) AS [Year]
                             FROM ItemForecastRoom
                             ORDER BY [Year];
                             """;
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room, DbType.Int32);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            parameters,
            command.DbTransaction,
            cancellationToken: cancellationToken);
        return await _dbConnection.QueryAsync<int>(sqlCommand);
    }

    public async Task<IEnumerable<int>> GetRoomIdsAsync(GetRoomIdsCommand command, CancellationToken cancellationToken)
    {
        const string query = """
                             SELECT DISTINCT 
                                 Room
                             FROM ItemForecastRoom
                             ORDER BY Room;
                             """;
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            command.DbTransaction,
            cancellationToken: cancellationToken);
        return await _dbConnection.QueryAsync<int>(sqlCommand);
    }
}