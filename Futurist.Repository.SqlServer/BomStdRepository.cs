using System.Data;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class BomStdRepository : IBomStdRepository
{
    private readonly IDbTransaction _dbTransaction;
    private readonly IDbConnection _sqlConnection;

    public BomStdRepository(IDbTransaction dbTransaction, IDbConnection sqlConnection)
    {
        _dbTransaction = dbTransaction;
        _sqlConnection = sqlConnection;
    }

    public async Task<string?> ProcessBomStdAsync(int roomId)
    {
        const string query = "EXEC CogsProjection.dbo.BomStd_insert @Room";
        return await _sqlConnection.ExecuteScalarAsync<string>(query, new { Room = roomId }, _dbTransaction);
    }

    public async Task<IEnumerable<BomStd>> BomErrorCheckAsync(int roomId)
    {
        const string query = "EXEC CogsProjection.dbo.BomStd_Check @Room";
        return await _sqlConnection.QueryAsync<BomStd>(query, new { Room = roomId }, _dbTransaction);
    }

    public async Task<IEnumerable<int>> GetRoomIdsAsync()
    {
        const string query = """
                             SELECT DISTINCT Room
                             FROM Rofo

                             UNION

                             SELECT DISTINCT Room
                             FROM BomStd
                             """;
        
        return await _sqlConnection.QueryAsync<int>(query, transaction: _dbTransaction);
    }
}