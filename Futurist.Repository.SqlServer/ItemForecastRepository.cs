using System.Data;
using System.Data.SqlTypes;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.ItemForecast;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class ItemForecastRepository : IItemForecastRepository
{
    private readonly IDbConnection _sqlConnection;

    public ItemForecastRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<IEnumerable<ItemForecastSp>> GetItemForecastListAsync(GetItemForecastListCommand command)
    {
        const string query = "ItemForecastRoom_UploadSelect";
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room);
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<ItemForecastSp>(
            query,
            parameters,
            command.DbTransaction,
            commandType: CommandType.StoredProcedure);
    }

    public async Task InsertItemForecastAsync(InsertItemForecastCommand command)
    {
        const string query = "ItemForecastRoom_Upload";
        var parameters = new DynamicParameters();
        
        if (command.Room > 0)
        {
            parameters.Add("@Room", command.Room);
        }
        
        if (command.ItemId != string.Empty)
        {
            parameters.Add("@ItemId", command.ItemId);
        }
        
        if (command.ForecastDate != SqlDateTime.MinValue.Value)
        {
            parameters.Add("@ForecastDate", command.ForecastDate);
        }

        if (command.ForcedPrice != 0)
        {
            parameters.Add("@ForcedPrice", command.ForcedPrice);
        }
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        await _sqlConnection.ExecuteAsync(
            query,
            parameters,
            command.DbTransaction,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<int>> GetItemForecastRoomIdsAsync(GetItemForecastRoomIdsCommand command)
    {
        const string query = """
                             SELECT DISTINCT Room FROM ItemForecastRoom
                             UNION
                             SELECT DISTINCT Room FROM FgCost
                             UNION
                             SELECT DISTINCT Room FROM Mup
                             UNION
                             SELECT DISTINCT Room FROM BomStd
                             UNION
                             SELECT DISTINCT Room FROM Rofo
                             UNION
                             SELECT DISTINCT Room FROM ItemAdj
                             ORDER BY Room;
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }
}