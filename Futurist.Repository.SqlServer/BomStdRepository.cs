using System.Data;
using Dapper;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command;
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

    public async Task<string?> ProcessBomStdAsync(ProcessBomStdCommand command)
    {
        const string query = "EXEC CogsProjection.dbo.BomStd_insert @Room";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.ExecuteScalarAsync<string>(query, new { Room = command.RoomId },
            transaction: command.DbTransaction);
    }

    public async Task<IEnumerable<BomStd>> BomErrorCheckAsync(BomErrorCheckCommand command)
    {
        const string query = "EXEC CogsProjection.dbo.BomStd_Check @Room";
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

        if (!string.IsNullOrEmpty(pagedListRequest.Search))
        {
            sqlBuilder.OrWhere("Room LIKE @RoomSearch", new { RoomSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("ProductId LIKE @ProductIdSearch",
                new { ProductIdSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("ProductName LIKE @ProductNameSearch",
                new { ProductNameSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("BomId LIKE @BomIdSearch", new { BomIdSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("ItemId LIKE @ItemIdSearch", new { ItemIdSearch = $"%{pagedListRequest.Search}%" });
            sqlBuilder.OrWhere("ItemName LIKE @ItemNameSearch",
                new { ItemNameSearch = $"%{pagedListRequest.Search}%" });
        }

        if (pagedListRequest.Filter.Room != 0)
        {
            sqlBuilder.Where("Room = @Room", new { pagedListRequest.Filter.Room });
        }

        if (!string.IsNullOrEmpty(pagedListRequest.Filter.ProductId))
        {
            sqlBuilder.Where("ProductId LIKE @ProductId", new { ProductId = $"%{pagedListRequest.Filter.ProductId}%" });
        }

        if (!string.IsNullOrEmpty(pagedListRequest.Filter.ProductName))
        {
            sqlBuilder.Where("ProductName LIKE @ProductName",
                new { ProductName = $"%{pagedListRequest.Filter.ProductName}%" });
        }

        if (!string.IsNullOrEmpty(pagedListRequest.Filter.BomId))
        {
            sqlBuilder.Where("BomId LIKE @BomId", new { BomId = $"%{pagedListRequest.Filter.BomId}%" });
        }

        if (!string.IsNullOrEmpty(pagedListRequest.Filter.ItemId))
        {
            sqlBuilder.Where("ItemId LIKE @ItemId", new { ItemId = $"%{pagedListRequest.Filter.ItemId}%" });
        }

        if (!string.IsNullOrEmpty(pagedListRequest.Filter.ItemName))
        {
            sqlBuilder.Where("ItemName LIKE @ItemName", new { ItemName = $"%{pagedListRequest.Filter.ItemName}%" });
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