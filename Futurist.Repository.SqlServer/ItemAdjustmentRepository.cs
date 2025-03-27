using System.Data;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.ItemAdjustmentCommand;
using Futurist.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Futurist.Repository.SqlServer;

public class ItemAdjustmentRepository : IItemAdjustmentRepository
{
    private readonly IDbConnection _sqlConnection;

    public ItemAdjustmentRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<IEnumerable<ItemAdjustment>> GetItemAdjustmentListAsync(GetItemAdjustmentListCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON");
        const string query = "EXEC CogsProjection.dbo.ItemAdj_Select @Room";
        return await _sqlConnection.QueryAsync<ItemAdjustment>(
            query,
            new { command.Room },
            command.DbTransaction);
    }

    public async Task<IEnumerable<int>> GetItemAdjustmentRoomIdsAsync(GetItemAdjustmentRoomIdsCommand command)
    {
        const string query = """
                             SELECT DISTINCT Room FROM CogsProjection.dbo.FgCost
                             UNION
                             SELECT DISTINCT Room FROM CogsProjection.dbo.Mup
                             UNION
                             SELECT DISTINCT Room FROM CogsProjection.dbo.BomStd
                             UNION
                             SELECT DISTINCT Room FROM CogsProjection.dbo.Rofo
                             UNION
                             SELECT DISTINCT Room FROM CogsProjection.dbo.ItemAdj
                             ORDER BY Room;
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }

    public async Task<string?> DeleteItemAdjustmentAsync(DeleteItemAdjustmentCommand command)
    {
        const string query = "EXEC CogsProjection.dbo.ItemAdj_Delete @Room";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.ExecuteScalarAsync<string>(
            query,
            new { command.Room },
            command.DbTransaction);
    }

    public async Task BulkInsertItemAdjustmentAsync(BulkInsertItemAdjustmentCommand command)
    {
        // insert new rofo
        var sqlServerConnection = _sqlConnection as SqlConnection ?? throw new InvalidOperationException("Invalid SQL connection");
        
        using var sqlBulkCopy = command.DbTransaction is SqlTransaction sqlServerTransaction ? new SqlBulkCopy(sqlServerConnection, SqlBulkCopyOptions.Default, sqlServerTransaction) : new SqlBulkCopy(sqlServerConnection);
        
        sqlBulkCopy.DestinationTableName = "ItemAdj";
        sqlBulkCopy.BatchSize = 1000;

        var dataTable = new DataTable();
        dataTable.Columns.Add("Room", typeof(string));
        dataTable.Columns.Add("ItemId", typeof(string));
        dataTable.Columns.Add("AdjPrice", typeof(decimal));
        dataTable.Columns.Add("RmPrice", typeof(decimal));
        dataTable.Columns.Add("PmPrice", typeof(decimal));
        dataTable.Columns.Add("CreatedBy", typeof(string));
        dataTable.Columns.Add("CreatedDate", typeof(DateTime));
        
        foreach (var item in command.ItemAdjustments)
        {
            dataTable.Rows.Add(item.Room, item.ItemId, item.Price, item.ItemId.StartsWith('1') ? item.Price : 0, item.ItemId.StartsWith('3') ? item.Price : 0, item.CreatedBy, item.CreatedDate);
        }
        
        await sqlBulkCopy.WriteToServerAsync(dataTable);
    }
}