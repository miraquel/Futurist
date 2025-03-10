using System.Data;
using Dapper;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.FgCostCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class FgCostRepository : IFgCostRepository
{
    private readonly IDbConnection _sqlConnection;

    public FgCostRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<string?> CalculateFgCostAsync(CalculateFgCostCommand command)
    {
        const string sql = "EXEC CogsProjection.dbo.ProjectionCalc @RoomId";
        return await _sqlConnection.ExecuteScalarAsync<string>(sql, new { command.RoomId },
            transaction: command.DbTransaction);
    }

    public Task<IEnumerable<FgCostSp>> GetSummaryFgCostAsync(GetSummaryFgCostCommand command)
    {
        const string sql = "EXEC CogsProjection.dbo.FgCost_Select @RoomId";
        return _sqlConnection.QueryAsync<FgCostSp>(sql, new { command.RoomId },
            transaction: command.DbTransaction);
    }

    public async Task<PagedList<FgCostSp>> GetSummaryFgCostPagedListAsync(GetSummaryFgCostPagedListCommand command)
    {
        const string sql = """
                           SELECT a.[Room]
                           ,a.[RofoId]
                           ,a.[ProductId]
                           ,a.[ProductName]
                           ,i.UNITID as [Unit]
                           ,i.NETWEIGHT / 1000 as [UnitInKg]
                           ,a.[RofoDate]
                           ,a.[QtyRofo]
                           ,a.[Yield]
                           ,a.[RmPrice]
                           ,a.[PmPrice]
                           ,a.[StdCostPrice]
                           ,a.[CostPrice]
                           FROM [CogsProjection].[dbo].[FgCost] a
                           JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.[ProductId]
                           /**where**/
                           /**orderby**/
                           OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                           """;
        
        const string sqlCount = """
                                SELECT COUNT(*) 
                                FROM [CogsProjection].[dbo].[FgCost] a
                                JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.[ProductId]
                                /**where**/
                                """;
        
        var sqlBuilder = new SqlBuilder();
        
        var pagedListRequest = command.PagedListRequest;
        
        // get room from filters and covert to int
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.Room), out var roomFilter) && int.TryParse(roomFilter, out var room))
        {
            sqlBuilder.Where($"a.[Room] = @room", new { room });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.RofoId), out var rofoIdFilter) && int.TryParse(rofoIdFilter, out var rofoId))
        {
            sqlBuilder.Where($"a.[RofoId] = @rofoId", new { rofoId });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.ProductId), out var productIdFilter))
        {
            sqlBuilder.Where($"a.[ProductId] = @productId", new { productId = productIdFilter });
        }

        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.ProductName), out var productNameFilter))
        {
            sqlBuilder.Where($"a.[ProductName] LIKE @productName", new { productName = $"%{productNameFilter}%" });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.Unit), out var unitFilter))
        {
            sqlBuilder.Where($"i.UNITID = @unit", new { unit = unitFilter });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.UnitInKg), out var unitInKgFilter) && decimal.TryParse(unitInKgFilter, out var unitInKg))
        {
            sqlBuilder.Where($"i.NETWEIGHT / 1000 = @unitInKg", new { unitInKg });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.RofoDate), out var rofoDateFilter) && DateTime.TryParse(rofoDateFilter, out var rofoDate))
        {
            sqlBuilder.Where($"a.[RofoDate] = @rofoDate", new { rofoDate });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.QtyRofo), out var qtyRofoFilter) && decimal.TryParse(qtyRofoFilter, out var qtyRofo))
        {
            sqlBuilder.Where($"a.[QtyRofo] = @qtyRofo", new { qtyRofo });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.Yield), out var yieldFilter) && decimal.TryParse(yieldFilter, out var yield))
        {
            sqlBuilder.Where($"a.[Yield] = @yield", new { yield });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.RmPrice), out var rmPriceFilter) && decimal.TryParse(rmPriceFilter, out var rmPrice))
        {
            sqlBuilder.Where($"a.[RmPrice] = @rmPrice", new { rmPrice });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.PmPrice), out var pmPriceFilter) && decimal.TryParse(pmPriceFilter, out var pmPrice))
        {
            sqlBuilder.Where($"a.[PmPrice] = @pmPrice", new { pmPrice });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.StdCostPrice), out var stdCostPriceFilter) && decimal.TryParse(stdCostPriceFilter, out var stdCostPrice))
        {
            sqlBuilder.Where($"a.[StdCostPrice] = @stdCostPrice", new { stdCostPrice });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.CostPrice), out var costPriceFilter) && decimal.TryParse(costPriceFilter, out var costPrice))
        {
            sqlBuilder.Where($"a.[CostPrice] = @costPrice", new { costPrice });
        }
        
        var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
        sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
            ? $"a.RecId {sort}"
            : $"{pagedListRequest.SortBy} {sort}");
        
        sqlBuilder.AddParameters(new
            { Offset = (pagedListRequest.PageNumber - 1) * pagedListRequest.PageSize, pagedListRequest.PageSize });

        var queryTemplate = sqlBuilder.AddTemplate(sql);
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var fgCostList = await _sqlConnection.QueryAsync<FgCostSp>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        queryTemplate = sqlBuilder.AddTemplate(sqlCount);
        var fgCostCount = await _sqlConnection.ExecuteScalarAsync<int>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        return new PagedList<FgCostSp>(fgCostList, pagedListRequest.PageNumber, pagedListRequest.PageSize, fgCostCount);
    }

    public Task<IEnumerable<int>> GetFgCostRoomIdsAsync(GetFgCostRoomIdsCommand command)
    {
        const string query = """
                             SELECT DISTINCT Room FROM CogsProjection.dbo.FgCost
                             UNION
                             SELECT DISTINCT Room FROM CogsProjection.dbo.Mup
                             UNION
                             SELECT DISTINCT Room FROM CogsProjection.dbo.BomStd
                             UNION
                             SELECT DISTINCT Room FROM CogsProjection.dbo.Rofo
                             ORDER BY Room;
                             """;
        
        return _sqlConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }

    public Task<IEnumerable<FgCostDetailSp>> GetFgCostDetailAsync(GetFgCostDetailCommand command)
    {
        const string sql = "EXEC CogsProjection.dbo.FgCostDetail_Select @RoomId";
        return _sqlConnection.QueryAsync<FgCostDetailSp>(sql, new { command.RoomId },
            transaction: command.DbTransaction);
    }

    public async Task<PagedList<FgCostDetailSp>> GetFgCostDetailPagedListAsync(GetFgCostDetailPagedListCommand command)
    {
        const string sql = """
                           select
                            a.room
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
                           	,d.InventBatch
                           	,d.Qty
                           	,d.Price
                           	,d.RmPrice
                           	,d.PmPrice
                           	,d.StdCostPrice
                           	,d.[Source]
                           	,d.RefId
                           	,id.LATESTPRICE as [LatestPurchasePrice]
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

        const string sqlCount = """
                                select count(*)
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
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Room), out var roomFilter) && int.TryParse(roomFilter, out var room))
        {
            sqlBuilder.Where($"a.[Room] = @room", new { room });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RofoId), out var rofoIdFilter) && int.TryParse(rofoIdFilter, out var rofoId))
        {
            sqlBuilder.Where($"a.RecId = @rofoId", new { rofoId });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ProductId), out var productIdFilter))
        {
            sqlBuilder.Where($"a.ItemId = @productId", new { productId = productIdFilter });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ProductName), out var productNameFilter))
        {
            sqlBuilder.Where($"i.SEARCHNAME LIKE @productName", new { productName = $"%{productNameFilter}%" });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemId), out var itemIdFilter))
        {
            sqlBuilder.Where($"b.ItemId = @itemId", new { itemId = itemIdFilter });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemName), out var itemNameFilter))
        {
            sqlBuilder.Where($"ib.SEARCHNAME LIKE @itemName", new { itemName = $"%{itemNameFilter}%" });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.GroupSubstitusi), out var groupSubstitusiFilter))
        {
            sqlBuilder.Where($"isnull(s.VtaMpSubstitusiGroupId,'') = @groupSubstitusi", new { groupSubstitusi = groupSubstitusiFilter });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemAllocatedId), out var itemAllocatedIdFilter))
        {
            sqlBuilder.Where($"d.ItemId = @itemAllocatedId", new { itemAllocatedId = itemAllocatedIdFilter });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemAllocatedName), out var itemAllocatedNameFilter))
        {
            sqlBuilder.Where($"id.SEARCHNAME LIKE @itemAllocatedName", new { itemAllocatedName = $"%{itemAllocatedNameFilter}%" });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.InventBatch), out var inventBatchFilter))
        {
            sqlBuilder.Where($"d.InventBatch = @inventBatch", new { inventBatch = inventBatchFilter });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Qty), out var qtyFilter) && decimal.TryParse(qtyFilter, out var qty))
        {
            sqlBuilder.Where($"d.Qty = @qty", new { qty });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Price), out var priceFilter) && decimal.TryParse(priceFilter, out var price))
        {
            sqlBuilder.Where($"d.Price = @price", new { price });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RmPrice), out var rmPriceFilter) && decimal.TryParse(rmPriceFilter, out var rmPrice))
        {
            sqlBuilder.Where($"d.RmPrice = @rmPrice", new { rmPrice });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.PmPrice), out var pmPriceFilter) && decimal.TryParse(pmPriceFilter, out var pmPrice))
        {
            sqlBuilder.Where($"d.PmPrice = @pmPrice", new { pmPrice });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.StdCostPrice), out var stdCostPriceFilter) && decimal.TryParse(stdCostPriceFilter, out var stdCostPrice))
        {
            sqlBuilder.Where($"d.StdCostPrice = @stdCostPrice", new { stdCostPrice });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Source), out var sourceFilter))
        {
            sqlBuilder.Where($"d.[Source] = @source", new { source = sourceFilter });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RefId), out var refIdFilter) && int.TryParse(refIdFilter, out var refId))
        {
            sqlBuilder.Where($"d.RefId = @refId", new { refId });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.LatestPurchasePrice), out var latestPurchasePriceFilter) && decimal.TryParse(latestPurchasePriceFilter, out var latestPurchasePrice))
        {
            sqlBuilder.Where($"id.LATESTPRICE = @latestPurchasePrice", new { latestPurchasePrice });
        }
        
        var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
        sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
            ? $"a.RecId {sort}"
            : $"{pagedListRequest.SortBy} {sort}");
        
        sqlBuilder.AddParameters(new
            { Offset = (pagedListRequest.PageNumber - 1) * pagedListRequest.PageSize, pagedListRequest.PageSize });

        var queryTemplate = sqlBuilder.AddTemplate(sql);
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var fgCostDetailList = await _sqlConnection.QueryAsync<FgCostDetailSp>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        queryTemplate = sqlBuilder.AddTemplate(sqlCount);
        var fgCostDetailCount = await _sqlConnection.ExecuteScalarAsync<int>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        return new PagedList<FgCostDetailSp>(fgCostDetailList, pagedListRequest.PageNumber, pagedListRequest.PageSize, fgCostDetailCount);
    }
}