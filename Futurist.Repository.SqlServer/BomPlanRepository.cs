using System.Data;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.BomPlanCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class BomPlanRepository : IBomPlanRepository
{
    private readonly IDbConnection _sqlConnection;

    public BomPlanRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task ProcessBomPlanAsync(ProcessBomPlanCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON;");
        const string query = "EXEC FgPlanCostPrice_Insert @Room, @Year, @Month, @VerId";
        await _sqlConnection.ExecuteAsync(
            query,
            new { Room = command.RoomId, command.VerId, command.Year, command.Month },
            command.DbTransaction,
            commandTimeout: command.Timeout);
    }

    public async Task<IEnumerable<int>> GetBomPlanRoomIdsAsync(GetBomPlanRoomIdsCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON;");
            const string query = "SELECT DISTINCT Room FROM RofoVer ORDER BY Room";
        return await _sqlConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }

    public async Task<IEnumerable<int>> GetBomPlanVerIdsAsync(GetBomPlanVerIdsCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON;");
        const string query = "SELECT DISTINCT VerId FROM RofoVer WHERE Room = @Room ORDER BY VerId";
        return await _sqlConnection.QueryAsync<int>(query, new { Room = command.RoomId }, command.DbTransaction);
    }

    public async Task<IEnumerable<int>> GetBomPlanYearsAsync(GetBomPlanYearsCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON;");
        const string query = "SELECT DISTINCT YEAR(RofoDate) AS Year FROM RofoVer WHERE Room = @Room AND VerId = @VerId ORDER BY Year";
        return await _sqlConnection.QueryAsync<int>(query, new { Room = command.RoomId, command.VerId }, command.DbTransaction);
    }

    public async Task<IEnumerable<int>> GetBomPlanMonthsAsync(GetBomPlanMonthsCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON;");
        const string query = "SELECT DISTINCT MONTH(RofoDate) AS Month FROM RofoVer WHERE Room = @Room AND VerId = @VerId AND YEAR(RofoDate) = @Year ORDER BY Month";
        return await _sqlConnection.QueryAsync<int>(query, new { Room = command.RoomId, command.VerId, command.Year }, command.DbTransaction);
    }
}
