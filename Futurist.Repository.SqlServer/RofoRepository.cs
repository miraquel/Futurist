using System.Data;
using System.Data.SqlTypes;
using System.Transactions;
using Dapper;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.RofoCommand;
using Futurist.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Futurist.Repository.SqlServer;

internal class RofoRepository : IRofoRepository
{
    private readonly IDbConnection _sqlConnection;

    public RofoRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<PagedList<Rofo>> GetRofoPagedListAsync(GetRofoPagedListCommand command)
    {
        const string query = "SELECT * FROM Rofo /**where**/ /**orderby**/ OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        
        SqlBuilder sqlBuilder = new();
        
        var pagedListRequest = command.PagedListRequest;
        
        if (!string.IsNullOrEmpty(pagedListRequest.Search))
        {
            sqlBuilder.OrWhere("ItemId LIKE @ItemIdSearch", new { ItemIdSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("ItemName LIKE @ItemNameSearch", new { ItemNameSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("CreatedBy LIKE @CreatedBySearch", new { CreatedBySearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("CAST(Room AS NVARCHAR) LIKE @RoomSearch", new { RoomSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("CAST(RofoDate AS NVARCHAR) LIKE @RofoDateSearch", new { RofoDateSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("CAST(Qty AS NVARCHAR) LIKE @QtySearch", new { QtySearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("CAST(QtyRem AS NVARCHAR) LIKE @QtyRemSearch", new { QtyRemSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("CAST(CreatedDate AS NVARCHAR) LIKE @CreatedDateSearch", new { CreatedDateSearch = $"%{pagedListRequest.Search}%" });
        }
        
        if (pagedListRequest.Filter.Room != 0)
        {
            sqlBuilder.Where("Room = @Room", new { pagedListRequest.Filter.Room });
        }
        if (pagedListRequest.Filter.RofoDate != SqlDateTime.MinValue.Value)
        {
            sqlBuilder.Where("RofoDate = @RofoDate", new { pagedListRequest.Filter.RofoDate });
        }
        if (!string.IsNullOrEmpty(pagedListRequest.Filter.ItemId))
        {
            sqlBuilder.Where("ItemId LIKE @ItemId", new { ItemId = $"%{pagedListRequest.Filter.ItemId}%" });
        }
        if (!string.IsNullOrEmpty(pagedListRequest.Filter.ItemName))
        {
            sqlBuilder.Where("ItemName LIKE @ItemName", new { ItemName = $"%{pagedListRequest.Filter.ItemName}%" });
        }
        if (pagedListRequest.Filter.Qty != 0)
        {
            sqlBuilder.Where("CAST(Qty AS NVARCHAR) LIKE @Qty", new { Qty = $"{pagedListRequest.Filter.Qty}%" });
        }
        if (pagedListRequest.Filter.QtyRem != 0)
        {
            sqlBuilder.Where("QtyRem = @QtyRem", new { pagedListRequest.Filter.QtyRem });
        }
        if (!string.IsNullOrEmpty(pagedListRequest.Filter.CreatedBy))
        {
            sqlBuilder.Where("CreatedBy LIKE @CreatedBy", new { CreatedBy = $"%{pagedListRequest.Filter.CreatedBy}%" });
        }
        if (pagedListRequest.Filter.CreatedDate != SqlDateTime.MinValue.Value)
        {
            sqlBuilder.Where("CreatedDate = @CreatedDate", new { pagedListRequest.Filter.CreatedDate });
        }
        
        var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
        sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
            ? $"Room {sort}"
            : $"{pagedListRequest.SortBy} {sort}");

        sqlBuilder.AddParameters(new { Offset = (pagedListRequest.PageNumber - 1) * pagedListRequest.PageSize, pagedListRequest.PageSize });
        
        var queryTemplate = sqlBuilder.AddTemplate(query);
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var rofoList = await _sqlConnection.QueryAsync<Rofo>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        // select from count using sql builder
        queryTemplate = sqlBuilder.AddTemplate("SELECT COUNT(*) FROM Rofo /**where**/");
        var rofoCount = await _sqlConnection.ExecuteScalarAsync<int>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        return new PagedList<Rofo>(rofoList, pagedListRequest.PageNumber, pagedListRequest.PageSize, rofoCount);
    }

    public async Task<Rofo?> GetRofoByIdAsync(GetRofoByIdCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryFirstOrDefaultAsync<Rofo>("SELECT * FROM Rofo WHERE RecId = @Id", new { command.Id }, command.DbTransaction); 
    }

    public async Task DeleteRofoByRoomAsync(DeleteRofoByRoomCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        await _sqlConnection.ExecuteAsync("DELETE FROM Rofo WHERE Room = @Room", new { Room = command.RoomId }, command.DbTransaction);
    }

    public async Task BulkInsertRofoAsync(BulkInsertRofoCommand command)
    {
        // insert new rofo
        var sqlServerConnection = _sqlConnection as SqlConnection ?? throw new InvalidOperationException("Invalid SQL connection");
        
        using var sqlBulkCopy = command.DbTransaction is SqlTransaction sqlServerTransaction ? new SqlBulkCopy(sqlServerConnection, SqlBulkCopyOptions.Default, sqlServerTransaction) : new SqlBulkCopy(sqlServerConnection);
        
        sqlBulkCopy.DestinationTableName = "Rofo";
        sqlBulkCopy.BatchSize = 1000;

        var dataTable = new DataTable();
        dataTable.Columns.Add("Room", typeof(string));
        dataTable.Columns.Add("RofoDate", typeof(DateTime));
        dataTable.Columns.Add("ItemId", typeof(string));
        dataTable.Columns.Add("ItemName", typeof(string));
        dataTable.Columns.Add("Qty", typeof(decimal));
        dataTable.Columns.Add("QtyRem", typeof(decimal));
        dataTable.Columns.Add("CreatedBy", typeof(string));
        dataTable.Columns.Add("CreatedDate", typeof(DateTime));
        dataTable.Columns.Add("RecId", typeof(int));
        
        foreach (var item in command.Rofos)
        {
            var itemName = item.ItemName.Length > 60 ? item.ItemName[..60] : item.ItemName;
            dataTable.Rows.Add(item.Room, item.RofoDate, item.ItemId, itemName, item.Qty, item.QtyRem, item.CreatedBy, DateTime.Now);
        }
        
        await sqlBulkCopy.WriteToServerAsync(dataTable);
    }

    public async Task<IEnumerable<int>> GetRofoRoomIdsAsync(GetRofoRoomIdsCommand command)
    {
        const string query = "SELECT DISTINCT Room FROM Rofo";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }
}