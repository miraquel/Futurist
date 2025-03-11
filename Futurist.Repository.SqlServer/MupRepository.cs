using System.Data;
using System.Data.SqlTypes;
using Dapper;
using Futurist.Common.Helpers;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.MupCommand;
using Futurist.Repository.Interface;
using Futurist.Repository.UnitOfWork;
using Microsoft.Data.SqlClient;

namespace Futurist.Repository.SqlServer;

public class MupRepository : IMupRepository
{
    private readonly IDbConnection _sqlConnection;

    public MupRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<IEnumerable<MupSp>> ProcessMupAsync(ProcessMupCommand command)
    {
        const string query = "EXEC CogsProjection.dbo.MupCalcRoom @Room";
        return await _sqlConnection.QueryAsync<MupSp>(
            query, 
            new { Room = command.RoomId }, 
            command.DbTransaction,
            commandTimeout: command.Timeout);
    }

    public async Task<IEnumerable<MupSp>> MupResultAsync(MupResultCommand command)
    {
	    await _sqlConnection.ExecuteAsync("SET ARITHABORT ON");
        const string query = "EXEC CogsProjection.dbo.MupSelect @Room";
        return await _sqlConnection.QueryAsync<MupSp>(
            query,
            new { Room = command.RoomId },
            command.DbTransaction);
    }

    public async Task<PagedList<MupSp>> MupResultPagedListAsync(MupResultPagedListCommand command)
    {
        const string query = """
                             SELECT 
                             	a.Room
                             	,a.RecId as RofoId
                             	,a.ItemId as ProductId
                             	,i.SEARCHNAME as ProductName
                             	,a.RofoDate
                             	,a.Qty as QtyRofo
                             	,b.ItemId
                             	,ib.SEARCHNAME as ItemName
                             	,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
                             	,d.ItemId as ItemAllocatedId
                             	,id.SEARCHNAME as ItemAllocatedName
                             	,id.UnitId
                             	,d.InventBatch
                             	,d.Qty
                             	,d.Price
                             	,d.RmPrice
                             	,d.PmPrice
                             	,d.StdCostPrice
                             	,d.[Source]
                             	,d.RefId
                             	,id.LATESTPRICE as [LatestPurchasePrice]
                             	,isnull(d.Price/nullif(id.LATESTPRICE,0)* 100,0) as [Gap]
                             from Rofo a 
                             join Mup b ON b.RofoId = a.RecId
                             join MupTrans c ON c.MupId = b.RecId
                             join ItemTrans d ON d.RecId = c.ItemTransId
                             join AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
                             join AXGMKDW.dbo.DimItem ib ON ib.ITEMID = b.ItemId
                             join AXGMKDW.dbo.DimItem id ON id.ITEMID = d.ItemId
                             left join AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = b.ItemId
                             /**where**/
                             /**orderby**/
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                             """;

        const string countQuery = """
                                  SELECT COUNT(*)
                                  from Rofo a 
                                  join Mup b ON b.RofoId = a.RecId
                                  join MupTrans c ON c.MupId = b.RecId
                                  join ItemTrans d ON d.RecId = c.ItemTransId
                                  join AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
                                  join AXGMKDW.dbo.DimItem ib ON ib.ITEMID = b.ItemId
                                  join AXGMKDW.dbo.DimItem id ON id.ITEMID = d.ItemId
                                  left join AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = b.ItemId
                                  /**where**/
                                  """;

        var sqlBuilder = new SqlBuilder();

        var pagedListRequest = command.PagedListRequest;
        
        if (pagedListRequest.Filters.TryGetValue("Room", out var roomFilter) && int.TryParse(roomFilter, out var room))
        {
            sqlBuilder.Where("a.Room = @Room", new { Room = room });
        }
        
        if (pagedListRequest.Filters.TryGetValue("RofoId", out var rofoIdFilter) && int.TryParse(rofoIdFilter, out var rofoId))
        {
            sqlBuilder.Where("a.RecId = @RofoId", new { RofoId = rofoId });
        }
        
        if (pagedListRequest.Filters.TryGetValue("ProductId", out var productIdFilter))
        {
            if (productIdFilter.Contains('*') || productIdFilter.Contains('%'))
            {
                productIdFilter = productIdFilter.Replace("*", "%");
                sqlBuilder.Where("a.ItemId LIKE @ProductId", new { ProductId = productIdFilter });
            }
            else
            {
                sqlBuilder.Where("a.ItemId = @ProductId", new { ProductId = productIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("ProductName", out var productNameFilter))
        {
            if (productNameFilter.Contains('*') || productNameFilter.Contains('%'))
            {
                productNameFilter = productNameFilter.Replace("*", "%");
                sqlBuilder.Where("i.SEARCHNAME LIKE @ProductName", new { ProductName = productNameFilter });
            }
            else
            {
                sqlBuilder.Where("i.SEARCHNAME = @ProductName", new { ProductName = productNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("RofoDate", out var rofoDateFilter) && DateTime.TryParse(rofoDateFilter, out var rofoDate))
        {
            sqlBuilder.Where("a.RofoDate = @RofoDate", new { RofoDate = rofoDate });
        }
        
        if (pagedListRequest.Filters.TryGetValue("QtyRofo", out var qtyRofoFilter))
        {
            if (decimal.TryParse(qtyRofoFilter, out var qtyRofo))
            {
                sqlBuilder.Where("a.Qty = @QtyRofo", new { QtyRofo = qtyRofo });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyRofoFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"a.Qty {match.Groups[1].Value} @QtyRofo", new { QtyRofo = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("ItemId", out var itemIdFilter))
        {
            if (itemIdFilter.Contains('*') || itemIdFilter.Contains('%'))
            {
                itemIdFilter = itemIdFilter.Replace("*", "%");
                sqlBuilder.Where("b.ItemId LIKE @ItemId", new { ItemId = itemIdFilter });
            }
            else
            {
                sqlBuilder.Where("b.ItemId = @ItemId", new { ItemId = itemIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("ItemName", out var itemNameFilter))
        {
            if (itemNameFilter.Contains('*') || itemNameFilter.Contains('%'))
            {
                itemNameFilter = itemNameFilter.Replace("*", "%");
                sqlBuilder.Where("ib.SEARCHNAME LIKE @ItemName", new { ItemName = itemNameFilter });
            }
            else
            {
                sqlBuilder.Where("ib.SEARCHNAME = @ItemName", new { ItemName = itemNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("GroupSubstitusi", out var groupSubstitusiFilter))
        {
            if (groupSubstitusiFilter.Contains('*') || groupSubstitusiFilter.Contains('%'))
            {
                groupSubstitusiFilter = groupSubstitusiFilter.Replace("*", "%");
                sqlBuilder.Where("s.VtaMpSubstitusiGroupId LIKE @GroupSubstitusi", new { GroupSubstitusi = groupSubstitusiFilter });
            }
            else
            {
                sqlBuilder.Where("s.VtaMpSubstitusiGroupId = @GroupSubstitusi", new { GroupSubstitusi = groupSubstitusiFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("ItemAllocatedId", out var itemAllocatedIdFilter))
        {
            if (itemAllocatedIdFilter.Contains('*') || itemAllocatedIdFilter.Contains('%'))
            {
                itemAllocatedIdFilter = itemAllocatedIdFilter.Replace("*", "%");
                sqlBuilder.Where("d.ItemId LIKE @ItemAllocatedId", new { ItemAllocatedId = itemAllocatedIdFilter });
            }
            else
            {
                sqlBuilder.Where("d.ItemId = @ItemAllocatedId", new { ItemAllocatedId = itemAllocatedIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("ItemAllocatedName", out var itemAllocatedNameFilter))
        {
            if (itemAllocatedNameFilter.Contains('*') || itemAllocatedNameFilter.Contains('%'))
            {
                itemAllocatedNameFilter = itemAllocatedNameFilter.Replace("*", "%");
                sqlBuilder.Where("id.SEARCHNAME LIKE @ItemAllocatedName", new { ItemAllocatedName = itemAllocatedNameFilter });
            }
            else
            {
                sqlBuilder.Where("id.SEARCHNAME = @ItemAllocatedName", new { ItemAllocatedName = itemAllocatedNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("InventBatch", out var inventBatchFilter))
        {
            if (inventBatchFilter.Contains('*') || inventBatchFilter.Contains('%'))
            {
                inventBatchFilter = inventBatchFilter.Replace("*", "%");
                sqlBuilder.Where("d.InventBatch LIKE @InventBatch", new { InventBatch = inventBatchFilter });
            }
            else
            {
                sqlBuilder.Where("d.InventBatch = @InventBatch", new { InventBatch = inventBatchFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("Price", out var priceFilter))
        {
            if (decimal.TryParse(priceFilter, out var price))
            {
                sqlBuilder.Where("d.Price = @Price", new { Price = price });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(priceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.Price {match.Groups[1].Value} @Price", new { Price = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue("Gap", out var gapFilter))
        {
            if (decimal.TryParse(gapFilter, out var gap))
            {
                sqlBuilder.Where("isnull(d.Price/nullif(id.LATESTPRICE,0)* 100,0) = @Gap", new { Gap = gap });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(gapFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"isnull(d.Price/nullif(id.LATESTPRICE,0)* 100,0) {match.Groups[1].Value} @Gap", new { Gap = match.Groups[2].Value });
                }
            }
        }
        
		var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
		sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
			? $"b.RecId {sort}"
			: $"{pagedListRequest.SortBy} {sort}");

		sqlBuilder.AddParameters(new
			{ Offset = (pagedListRequest.PageNumber - 1) * pagedListRequest.PageSize, pagedListRequest.PageSize });

		var queryTemplate = sqlBuilder.AddTemplate(query);
		await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
		var mupList = await _sqlConnection.QueryAsync<MupSp>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
		queryTemplate = sqlBuilder.AddTemplate(countQuery);
		var mupCount = await _sqlConnection.ExecuteScalarAsync<int>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
		return new PagedList<MupSp>(mupList, pagedListRequest.PageNumber, pagedListRequest.PageSize, mupCount);
    }

    public async Task<IEnumerable<int>> GetMupRoomIdsAsync(GetMupRoomIdsCommand command)
    {
        const string query = """
                             SELECT DISTINCT Room FROM Mup
                             UNION
                             SELECT DISTINCT Room FROM BomStd
                             UNION
                             SELECT DISTINCT Room FROM Rofo
                             ORDER BY Room
                             """;
        
        return await _sqlConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }
}