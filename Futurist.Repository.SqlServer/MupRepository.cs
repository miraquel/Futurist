using System.Data;
using Dapper;
using Futurist.Repository.Interface;
using Futurist.Service.Dto;

namespace Futurist.Repository.SqlServer;

public class MupRepository : IMupRepository
{
    private readonly IDbConnection _sqlConnection;
    private readonly IDbTransaction _dbTransaction;

    public MupRepository(IDbConnection sqlConnection, IDbTransaction dbTransaction)
    {
        _sqlConnection = sqlConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<MupSp>> ProcessMupAsync(int roomId)
    {
        const string query = "EXEC CogsProjection.dbo.MupCalcRoom @Room";
        return await _sqlConnection.QueryAsync<MupSp>(query, new { Room = roomId }, _dbTransaction);
    }

    public async Task<IEnumerable<MupSp>> MupResultAsync(int roomId)
    {
        const string query = "EXEC CogsProjection.dbo.MupResult @Room";
        return await _sqlConnection.QueryAsync<MupSp>(query, new { Room = roomId }, _dbTransaction);
    }

    public async Task<IEnumerable<int>> GetRoomIdsAsync()
    {
        const string query = """
                             SELECT DISTINCT Room FROM Mup
                             UNION
                             SELECT DISTINCT Room FROM BomStd
                             UNION
                             SELECT DISTINCT Room FROM Rofo
                             """;
        
        return await _sqlConnection.QueryAsync<int>(query, transaction: _dbTransaction);
    }
}