using System.Data;
using System.Data.SqlTypes;
using System.Transactions;
using Dapper;
using Futurist.Common.Helpers;
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
        
        if (pagedListRequest.Filters.TryGetValue(nameof(Rofo.Room), out var roomFilter) && int.TryParse(roomFilter, out var room))
        {
            sqlBuilder.Where("Room = @Room", new { Room = room });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(Rofo.RofoDate), out var rofoDateFilter) && DateTime.TryParse(rofoDateFilter, out var rofoDate))
        {
            sqlBuilder.Where("RofoDate = @RofoDate", new { RofoDate = rofoDate });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(Rofo.ItemId), out var itemIdFilter))
        {
            if (itemIdFilter.Contains('*') || itemIdFilter.Contains('%'))
            {
                itemIdFilter = itemIdFilter.Replace("*", "%");
                sqlBuilder.Where("ItemId LIKE @ItemId", new { ItemId = itemIdFilter });
            }
            else
            {
                sqlBuilder.Where("ItemId = @ItemId", new { ItemId = itemIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(Rofo.ItemName), out var itemNameFilter))
        {
            if (itemNameFilter.Contains('*') || itemNameFilter.Contains('%'))
            {
                itemNameFilter = itemNameFilter.Replace("*", "%");
                sqlBuilder.Where("ItemName LIKE @ItemName", new { ItemName = itemNameFilter });
            }
            else
            {
                sqlBuilder.Where("ItemName = @ItemName", new { ItemName = itemNameFilter });
            }
        }

        if (pagedListRequest.Filters.TryGetValue(nameof(Rofo.Qty), out var qtyFilter))
        {
            if (decimal.TryParse(qtyFilter, out var qty))
            {
                sqlBuilder.Where("Qty = @Qty", new { Qty = qty });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"Qty {match.Groups[1].Value} @Qty", new { Qty = match.Groups[2].Value });
                }
            }
        }

        if (pagedListRequest.Filters.TryGetValue(nameof(Rofo.QtyRem), out var qtyRemFilter))
        {
            if (decimal.TryParse(qtyRemFilter, out var qtyRem))
            {
                sqlBuilder.Where("QtyRem = @QtyRem", new { QtyRem = qtyRem });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyRemFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"QtyRem {match.Groups[1].Value} @QtyRem", new { QtyRem = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(Rofo.QtyRem), out var qtyRemRemFilter))
        {
            if (decimal.TryParse(qtyRemRemFilter, out var qtyRemRem))
            {
                sqlBuilder.Where("QtyRem = @QtyRem", new { QtyRem = qtyRemRem });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyRemRemFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"QtyRem {match.Groups[1].Value} @QtyRem", new { QtyRem = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(Rofo.CreatedBy), out var createdByFilter))
        {
            if (createdByFilter.Contains('*') || createdByFilter.Contains('%'))
            {
                createdByFilter = createdByFilter.Replace("*", "%");
                sqlBuilder.Where("CreatedBy LIKE @CreatedBy", new { CreatedBy = createdByFilter });
            }
            else
            {
                sqlBuilder.Where("CreatedBy = @CreatedBy", new { CreatedBy = createdByFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(Rofo.CreatedDate), out var createdDateFilter) && DateTime.TryParse(createdDateFilter, out var createdDate))
        {
            sqlBuilder.Where("CreatedDate = @CreatedDate", new { CreatedDate = createdDate });
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