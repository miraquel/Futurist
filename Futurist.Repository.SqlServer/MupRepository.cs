using System.Data;
using Dapper;
using Futurist.Repository.Interface;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;

namespace Futurist.Repository.SqlServer;

public class MupRepository : IMupRepository
{
    private readonly IDbConnection _sqlConnection;
    private readonly IUnitOfWork _unitOfWork;

    public MupRepository(IDbConnection sqlConnection, IUnitOfWork unitOfWork)
    {
        _sqlConnection = sqlConnection;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MupSp>> ProcessMupAsync(int roomId, IDbTransaction? transaction)
    {
        const string query = "EXEC CogsProjection.dbo.MupCalcRoom @Room";

        if (transaction?.Connection != null)
            return await transaction.Connection.QueryAsync<MupSp>(query, 
                new { Room = roomId }, 
                transaction,
                commandTimeout: 3600);
        
        return await _sqlConnection.QueryAsync<MupSp>(query, new { Room = roomId }, commandTimeout: 3600);
    }

    public async Task<IEnumerable<MupSp>> MupResultAsync(int roomId)
    {
        const string query = "EXEC CogsProjection.dbo.MupSelect @Room";
        return await _sqlConnection.QueryAsync<MupSp>(query, new { Room = roomId });
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
        
        return await _sqlConnection.QueryAsync<int>(query);
    }
}