using System.Data;
using Dapper;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.BomStdCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class BomStdRepository : IBomStdRepository
{
    private readonly IDbConnection _sqlConnection;

    public BomStdRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<SpTask?> ProcessBomStdAsync(ProcessBomStdCommand command)
    {
        const string query = "EXEC BomStd_insert @Room";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QuerySingleOrDefaultAsync<SpTask>(query, new { Room = command.RoomId },
            transaction: command.DbTransaction, commandTimeout: command.Timeout);
    }

    public async Task<IEnumerable<BomStd>> BomErrorCheckAsync(BomErrorCheckCommand command)
    {
        const string query = "EXEC BomStd_Check @Room";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<BomStd>(query, new { Room = command.RoomId },
            transaction: command.DbTransaction);
    }

    public async Task<PagedList<BomStd>> BomErrorCheckPagedListAsync(BomErrorCheckPagedListCommand command)
    {
        const string query = """
                             WITH BomStdUnion AS
                             (
                                SELECT a.Room, a.ItemId as ProducId, i.SEARCHNAME as ProductName
                             	    ,ISNULL(b.BomId,'') as BomId
                             		,ISNULL(b.ItemId,'') as ItemId
                             		,ISNULL(b.ItemName,'') as ItemName
                             	FROM Rofo a
                             	LEFT JOIN BomStd b ON b.ProductId = a.ItemId
                             	JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
                             	WHERE a.Room = @Room
                             	    AND b.BomId is null
                             
                                UNION 
                             
                                SELECT a.[Room]
                             	    ,a.[ProductId], i.SEARCHNAME as ProductName
                             	    ,a.[BomId]
                             	    ,a.[ItemId]
                             	    ,a.[ItemName]
                                FROM [BomStd] a
                             	JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.[ProductId]
                                WHERE a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '6%'
                             	    AND a.Room = @Room
                             )
                             SELECT *
                             FROM BomStdUnion
                             /**where**/
                             /**orderby**/
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                             """;

        const string countQuery = """
                                  WITH BomStdUnion AS
                                  (
                                    SELECT a.Room, a.ItemId as ProductId, i.SEARCHNAME as ProductName,
                                           ISNULL(b.BomId, '') as BomId,
                                           ISNULL(b.ItemId, '') as ItemId,
                                           ISNULL(b.ItemName, '') as ItemName
                                  	FROM Rofo a
                                  	LEFT JOIN BomStd b ON b.ProductId = a.ItemId
                                  	JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
                                  	WHERE a.Room = @Room
                                      AND b.BomId IS NULL
                                  
                                    UNION 
                                  
                                    SELECT a.Room,
                                           a.ProductId, i.SEARCHNAME as ProductName,
                                           a.BomId,
                                           a.ItemId,
                                           a.ItemName
                                    FROM BomStd a
                                    JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ProductId
                                    WHERE (a.ItemId LIKE '4%' OR a.ItemId LIKE '6%')
                                  	    AND a.Room = @Room
                                  )
                                  SELECT COUNT(*)
                                  FROM BomStdUnion
                                  /**where**/
                                  """;

        var sqlBuilder = new SqlBuilder();

        var pagedListRequest = command.PagedListRequest;

        // filters
        if (pagedListRequest.Filters.TryGetValue("Room", out var roomFilter) && int.TryParse(roomFilter, out var room))
        {
            sqlBuilder.Where("Room = @Room", new { Room = room });
        }
        
        if (pagedListRequest.Filters.TryGetValue("ProductId", out var productIdFilter))
        {
            if (productIdFilter.Contains('*') || productIdFilter.Contains('%'))
            {
                productIdFilter = productIdFilter.Replace("*", "%");
                sqlBuilder.Where("ProductId LIKE @ProductId", new { ProductId = productIdFilter });
            }
            else
            {
                sqlBuilder.Where("ProductId = @ProductId", new { ProductId = productIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("ProductName", out var productNameFilter))
        {
            if (productNameFilter.Contains('*') || productNameFilter.Contains('%'))
            {
                productNameFilter = productNameFilter.Replace("*", "%");
                sqlBuilder.Where("ProductName LIKE @ProductName", new { ProductName = productNameFilter });
            }
            else
            {
                sqlBuilder.Where("ProductName = @ProductName", new { ProductName = productNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("BomId", out var bomIdFilter))
        {
            if (bomIdFilter.Contains('*') || bomIdFilter.Contains('%'))
            {
                bomIdFilter = bomIdFilter.Replace("*", "%");
                sqlBuilder.Where("BomId LIKE @BomId", new { BomId = bomIdFilter });
            }
            else
            {
                sqlBuilder.Where("BomId = @BomId", new { BomId = bomIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("ItemId", out var itemIdFilter))
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
        
        if (pagedListRequest.Filters.TryGetValue("ItemName", out var itemNameFilter))
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

        var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
        sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
            ? $"Room {sort}"
            : $"{pagedListRequest.SortBy} {sort}");

        sqlBuilder.AddParameters(new
            { Offset = (pagedListRequest.PageNumber - 1) * pagedListRequest.PageSize, pagedListRequest.PageSize });

        var queryTemplate = sqlBuilder.AddTemplate(query);
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var bomStdList = await _sqlConnection.QueryAsync<BomStd>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        queryTemplate = sqlBuilder.AddTemplate(countQuery);
        var bomStdCount = await _sqlConnection.ExecuteScalarAsync<int>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        return new PagedList<BomStd>(bomStdList, pagedListRequest.PageNumber, pagedListRequest.PageSize, bomStdCount);
    }

    public async Task<IEnumerable<int>> GetBomStdRoomIdsAsync(GetBomStdRoomIdsCommand command)
    {
        const string query = """
                             SELECT DISTINCT Room
                             FROM Rofo

                             UNION

                             SELECT DISTINCT Room
                             FROM BomStd
                             """;

        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }
}