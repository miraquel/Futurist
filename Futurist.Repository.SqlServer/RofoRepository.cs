using System.Data;
using System.Data.SqlTypes;
using Dapper;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Futurist.Repository.SqlServer;

internal class RofoRepository : IRofoRepository
{
    private readonly IDbTransaction _dbTransaction;
    private readonly IDbConnection _sqlConnection;

    public RofoRepository(IDbTransaction dbTransaction, IDbConnection sqlConnection)
    {
        _dbTransaction = dbTransaction;
        _sqlConnection = sqlConnection;
    }

    public async Task<PagedList<Rofo>> GetPagedListAsync(PagedListRequest<Rofo> pagedListRequest)
    {
        const string query = "SELECT * FROM Rofo /**where**/ /**orderby**/ OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        
        SqlBuilder sqlBuilder = new();
        
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
            sqlBuilder.Where("CAST(Room AS NVARCHAR) LIKE @Room", new { Room = $"{pagedListRequest.Filter.Room}%" });
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
        var rofoList = await _sqlConnection.QueryAsync<Rofo>(queryTemplate.RawSql, queryTemplate.Parameters, _dbTransaction);
        // select from count using sql builder
        queryTemplate = sqlBuilder.AddTemplate("SELECT COUNT(*) FROM Rofo /**where**/");
        var rofoCount = await _sqlConnection.ExecuteScalarAsync<int>(queryTemplate.RawSql, queryTemplate.Parameters, _dbTransaction);
        return new PagedList<Rofo>(rofoList, pagedListRequest.PageNumber, pagedListRequest.PageSize, rofoCount);
    }

    public async Task<Rofo?> GetByIdAsync(int id)
    {
        return await _sqlConnection.QueryFirstOrDefaultAsync<Rofo>("SELECT * FROM Rofo WHERE RecId = @Id", new { Id = id }, _dbTransaction);
    }

    public async Task DeleteRofoByRoomAsync(int roomId)
    {
        await _sqlConnection.ExecuteAsync("DELETE FROM Rofo WHERE Room = @Room", new { Room = roomId }, _dbTransaction);
    }

    public async Task BulkInsertRofoAsync(IEnumerable<Rofo> rofo)
    {
        // insert new rofo
        var sqlServerConnection = _sqlConnection as SqlConnection ?? throw new InvalidOperationException("Invalid SQL connection");
        var sqlServerTransaction = _dbTransaction as SqlTransaction ?? throw new InvalidOperationException("Invalid SQL transaction");
        
        using var sqlBulkCopy = new SqlBulkCopy(sqlServerConnection, SqlBulkCopyOptions.Default, sqlServerTransaction);
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
        
        foreach (var item in rofo)
        {
            dataTable.Rows.Add(item.Room, item.RofoDate, item.ItemId, item.ItemName, item.Qty, item.QtyRem, item.CreatedBy, DateTime.Now);
        }
        
        await sqlBulkCopy.WriteToServerAsync(dataTable);
    }

    public async Task<IEnumerable<int>> GetRoomIdsAsync()
    {
        const string query = "SELECT DISTINCT Room FROM Rofo";
        return await _sqlConnection.QueryAsync<int>(query, transaction: _dbTransaction);
    }
}