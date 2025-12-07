using System.Data;
using System.Reflection;
using Dapper;
using Futurist.Common.Helpers;
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

    public async Task<SpTask?> CalculateFgCostAsync(CalculateFgCostCommand command)
    {
        const string sql = "EXEC ProjectionCalc @Room";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QuerySingleOrDefaultAsync<SpTask>(sql, new { command.Room },
            transaction: command.DbTransaction, commandTimeout: command.Timeout);
    }

    public async Task<IEnumerable<FgCostSp>> GetSummaryFgCostAsync(GetSummaryFgCostCommand command)
    {
        CustomMapping();
        const string sql = "EXEC FgCost_Select @Room";
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<FgCostSp>(sql, new { command.Room },
            transaction: command.DbTransaction);
    }

    public async Task<PagedList<FgCostSp>> GetSummaryFgCostPagedListAsync(GetSummaryFgCostPagedListCommand command)
    {
        CustomMapping();
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
                           ,ISNULL(s.[SalesPriceIndex],0) as [SalesPriceIndex]
                           FROM [CogsProjection].[dbo].[FgCost] a
                           JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.[ProductId]
                           LEFT JOIN (
                           	SELECT x.[ItemId], x.[SalesPriceIndex]
                           	FROM ( 
                           		SELECT a.ItemId, a.[SalesPriceIndex]
                           		FROM [SalesPrice] a
                           		JOIN (
                           			SELECT [ItemId], MAX([PeriodDate]) [MaxPeriodDate]
                           			FROM [SalesPrice]
                           			GROUP BY [ItemId]
                           		) b ON b.ItemId = a.ItemId AND b.[MaxPeriodDate] = a.[PeriodDate]
                           	) x			
                           ) s ON s.ItemId = a.[ProductId]
                           /**where**/
                           /**orderby**/
                           OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                           """;
        
        const string sqlCount = """
                                SELECT COUNT(*) 
                                FROM [CogsProjection].[dbo].[FgCost] a
                                JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.[ProductId]
                                LEFT JOIN (
                                	SELECT x.[ItemId], x.[SalesPriceIndex]
                                	FROM ( 
                                		SELECT a.ItemId, a.[SalesPriceIndex]
                                		FROM [SalesPrice] a
                                		JOIN (
                                			SELECT [ItemId], MAX([PeriodDate]) [MaxPeriodDate]
                                			FROM [SalesPrice]
                                			GROUP BY [ItemId]
                                		) b ON b.ItemId = a.ItemId AND b.[MaxPeriodDate] = a.[PeriodDate]
                                	) x			
                                ) s ON s.ItemId = a.[ProductId]
                                /**where**/
                                """;
        
        var sqlBuilder = new SqlBuilder();
        
        var pagedListRequest = command.PagedListRequest;
        
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
            if (productIdFilter.Contains('*') || productIdFilter.Contains('%'))
            {
                productIdFilter = productIdFilter.Replace('*', '%');
                sqlBuilder.Where($"a.[ProductId] LIKE @productId", new { productId = productIdFilter });
            }
            else
            {
                sqlBuilder.Where($"a.[ProductId] = @productId", new { productId = productIdFilter });
            }
        }

        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.ProductName), out var productNameFilter))
        {
            if (productNameFilter.Contains('*') || productNameFilter.Contains('%'))
            {
                productNameFilter = productNameFilter.Replace('*', '%');
                sqlBuilder.Where($"a.[ProductName] LIKE @productName", new { productName = productNameFilter });
            }
            else
            {
                sqlBuilder.Where($"a.[ProductName] = @productName", new { productName = productNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.Unit), out var unitFilter))
        {
            if (unitFilter.Contains('*') || unitFilter.Contains('%'))
            {
                unitFilter = unitFilter.Replace('*', '%');
                sqlBuilder.Where($"i.UNITID LIKE @unit", new { unit = unitFilter });
            }
            else
            {
                sqlBuilder.Where($"i.UNITID = @unit", new { unit = unitFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.UnitInKg), out var unitInKgFilter))
        {
            if (decimal.TryParse(unitInKgFilter, out var unitInKg))
            {
                sqlBuilder.Where($"i.NETWEIGHT / 1000 = @unitInKg", new { unitInKg });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(unitInKgFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"i.NETWEIGHT / 1000 {match.Groups[1].Value} @unitInKg", new { unitInKg = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.RofoDate), out var rofoDateFilter) && DateTime.TryParse(rofoDateFilter, out var rofoDate))
        {
            sqlBuilder.Where($"a.[RofoDate] = @rofoDate", new { rofoDate });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.RofoQty), out var qtyRofoFilter))
        {
            if (decimal.TryParse(qtyRofoFilter, out var qtyRofo))
            {
                sqlBuilder.Where($"a.[QtyRofo] = @qtyRofo", new { qtyRofo });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyRofoFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"a.[QtyRofo] {match.Groups[1].Value} @qtyRofo", new { qtyRofo = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.Yield), out var yieldFilter))
        {
            if (decimal.TryParse(yieldFilter, out var yield))
            {
                sqlBuilder.Where($"a.[Yield] = @yield", new { yield });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(yieldFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"a.[Yield] {match.Groups[1].Value} @yield", new { yield = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.RmPrice), out var rmPriceFilter))
        {
            if (decimal.TryParse(rmPriceFilter, out var rmPrice))
            {
                sqlBuilder.Where($"a.[RmPrice] = @rmPrice", new { rmPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(rmPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"a.[RmPrice] {match.Groups[1].Value} @rmPrice", new { rmPrice = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.PmPrice), out var pmPriceFilter))
        {
            if (decimal.TryParse(pmPriceFilter, out var pmPrice))
            {
                sqlBuilder.Where($"a.[PmPrice] = @pmPrice", new { pmPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(pmPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"a.[PmPrice] {match.Groups[1].Value} @pmPrice", new { pmPrice = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.StdCostPrice), out var stdCostPriceFilter))
        {
            if (decimal.TryParse(stdCostPriceFilter, out var stdCostPrice))
            {
                sqlBuilder.Where($"a.[StdCostPrice] = @stdCostPrice", new { stdCostPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(stdCostPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"a.[StdCostPrice] {match.Groups[1].Value} @stdCostPrice", new { stdCostPrice = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostSp.CostPrice), out var costPriceFilter))
        {
            if (decimal.TryParse(costPriceFilter, out var costPrice))
            {
                sqlBuilder.Where($"a.[CostPrice] = @costPrice", new { costPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(costPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"a.[CostPrice] {match.Groups[1].Value} @costPrice", new { costPrice = match.Groups[2].Value });
                }
            }
        }
        
        var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
        sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
            ? "DATEFROMPARTS(YEAR([RofoDate]),MONTH([RofoDate]),1) asc,  a.[QtyRofo] DESC"
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

    public async Task<IEnumerable<int>> GetFgCostRoomIdsAsync(GetFgCostRoomIdsCommand command)
    {
        const string query = """
                             SELECT DISTINCT Room FROM FgCost
                             UNION
                             SELECT DISTINCT Room FROM Mup
                             UNION
                             SELECT DISTINCT Room FROM BomStd
                             UNION
                             SELECT DISTINCT Room FROM Rofo
                             ORDER BY Room;
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }

    public async Task<IEnumerable<FgCostDetailSp>> GetFgCostDetailsAsync(GetFgCostDetailCommand command)
    {
        const string sql = "EXEC FgCostDetail_Select @Room";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<FgCostDetailSp>(sql, new { Room = command.Room },
            transaction: command.DbTransaction);
    }

    public async Task<PagedList<FgCostDetailSp>> GetFgCostDetailsPagedListAsync(GetFgCostDetailPagedListCommand command)
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
            if (productIdFilter.Contains('*') || productIdFilter.Contains('%'))
            {
                productIdFilter = productIdFilter.Replace('*', '%');
                sqlBuilder.Where($"a.ItemId LIKE @productId", new { productId = productIdFilter });
            }
            else
            {
                sqlBuilder.Where($"a.ItemId = @productId", new { productId = productIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ProductName), out var productNameFilter))
        {
            if (productNameFilter.Contains('*') || productNameFilter.Contains('%'))
            {
                productNameFilter = productNameFilter.Replace('*', '%');
                sqlBuilder.Where($"i.SEARCHNAME LIKE @productName", new { productName = productNameFilter });
            }
            else
            {
                sqlBuilder.Where($"i.SEARCHNAME = @productName", new { productName = productNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RofoDate), out var rofoDateFilter) && DateTime.TryParse(rofoDateFilter, out var rofoDate))
        {
            sqlBuilder.Where($"a.RofoDate = @rofoDate", new { rofoDate });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RofoQty), out var qtyRofoFilter))
        {
            if (decimal.TryParse(qtyRofoFilter, out var qtyRofo))
            {
                sqlBuilder.Where($"a.Qty = @qtyRofo", new { qtyRofo });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyRofoFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"a.Qty {match.Groups[1].Value} @qtyRofo", new { qtyRofo = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemId), out var itemIdFilter))
        {
            if (itemIdFilter.Contains('*') || itemIdFilter.Contains('%'))
            {
                itemIdFilter = itemIdFilter.Replace('*', '%');
                sqlBuilder.Where($"b.ItemId LIKE @itemId", new { itemId = itemIdFilter });
            }
            else
            {
                sqlBuilder.Where($"b.ItemId = @itemId", new { itemId = itemIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemName), out var itemNameFilter))
        {
            if (itemNameFilter.Contains('*') || itemNameFilter.Contains('%'))
            {
                itemNameFilter = itemNameFilter.Replace('*', '%');
                sqlBuilder.Where($"ib.SEARCHNAME LIKE @itemName", new { itemName = itemNameFilter });
            }
            else
            {
                sqlBuilder.Where($"ib.SEARCHNAME = @itemName", new { itemName = itemNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.GroupSubstitusi), out var groupSubstitusiFilter))
        {
            if (groupSubstitusiFilter.Contains('*') || groupSubstitusiFilter.Contains('%'))
            {
                groupSubstitusiFilter = groupSubstitusiFilter.Replace('*', '%');
                sqlBuilder.Where($"isnull(s.VtaMpSubstitusiGroupId,'') LIKE @groupSubstitusi", new { groupSubstitusi = groupSubstitusiFilter });
            }
            else
            {
                sqlBuilder.Where($"isnull(s.VtaMpSubstitusiGroupId,'') = @groupSubstitusi", new { groupSubstitusi = groupSubstitusiFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemAllocatedId), out var itemAllocatedIdFilter))
        {
            if (itemAllocatedIdFilter.Contains('*') || itemAllocatedIdFilter.Contains('%'))
            {
                itemAllocatedIdFilter = itemAllocatedIdFilter.Replace('*', '%');
                sqlBuilder.Where($"d.ItemId LIKE @itemAllocatedId", new { itemAllocatedId = itemAllocatedIdFilter });
            }
            else
            {
                sqlBuilder.Where($"d.ItemId = @itemAllocatedId", new { itemAllocatedId = itemAllocatedIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemAllocatedName), out var itemAllocatedNameFilter))
        {
            if (itemAllocatedNameFilter.Contains('*') || itemAllocatedNameFilter.Contains('%'))
            {
                itemAllocatedNameFilter = itemAllocatedNameFilter.Replace('*', '%');
                sqlBuilder.Where($"id.SEARCHNAME LIKE @itemAllocatedName", new { itemAllocatedName = itemAllocatedNameFilter });
            }
            else
            {
                sqlBuilder.Where($"id.SEARCHNAME = @itemAllocatedName", new { itemAllocatedName = itemAllocatedNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.InventBatch), out var inventBatchFilter))
        {
            if (inventBatchFilter.Contains('*') || inventBatchFilter.Contains('%'))
            {
                inventBatchFilter = inventBatchFilter.Replace('*', '%');
                sqlBuilder.Where($"d.InventBatch LIKE @inventBatch", new { inventBatch = inventBatchFilter });
            }
            else
            {
                sqlBuilder.Where($"d.InventBatch = @inventBatch", new { inventBatch = inventBatchFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Qty), out var qtyFilter))
        {
            if (decimal.TryParse(qtyFilter, out var qty))
            {
                sqlBuilder.Where($"d.Qty = @qty", new { qty });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.Qty {match.Groups[1].Value} @qty", new { qty = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Price), out var priceFilter))
        {
            if (decimal.TryParse(priceFilter, out var price))
            {
                sqlBuilder.Where($"d.Price = @price", new { price });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(priceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.Price {match.Groups[1].Value} @price", new { price = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RmPrice), out var rmPriceFilter))
        {
            if (decimal.TryParse(rmPriceFilter, out var rmPrice))
            {
                sqlBuilder.Where($"d.RmPrice = @rmPrice", new { rmPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(rmPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.RmPrice {match.Groups[1].Value} @rmPrice", new { rmPrice = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.PmPrice), out var pmPriceFilter))
        {
            if (decimal.TryParse(pmPriceFilter, out var pmPrice))
            {
                sqlBuilder.Where($"d.PmPrice = @pmPrice", new { pmPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(pmPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.PmPrice {match.Groups[1].Value} @pmPrice", new { pmPrice = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.StdCostPrice), out var stdCostPriceFilter))
        {
            if (decimal.TryParse(stdCostPriceFilter, out var stdCostPrice))
            {
                sqlBuilder.Where($"d.StdCostPrice = @stdCostPrice", new { stdCostPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(stdCostPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.StdCostPrice {match.Groups[1].Value} @stdCostPrice", new { stdCostPrice = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Source), out var sourceFilter))
        {
            if (sourceFilter.Contains('*') || sourceFilter.Contains('%'))
            {
                sourceFilter = sourceFilter.Replace('*', '%');
                sqlBuilder.Where($"d.[Source] LIKE @source", new { source = sourceFilter });
            }
            else
            {
                sqlBuilder.Where($"d.[Source] = @source", new { source = sourceFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RefId), out var refIdFilter))
        {
            if (refIdFilter.Contains('*') || refIdFilter.Contains('%'))
            {
                refIdFilter = refIdFilter.Replace('*', '%');
                sqlBuilder.Where($"d.RefId LIKE @refId", new { refId = refIdFilter });
            }
            else
            {
                sqlBuilder.Where($"d.RefId = @refId", new { refId = refIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.LatestPurchasePrice), out var latestPurchasePriceFilter))
        {
            if (decimal.TryParse(latestPurchasePriceFilter, out var latestPurchasePrice))
            {
                sqlBuilder.Where($"id.LATESTPRICE = @latestPurchasePrice", new { latestPurchasePrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(latestPurchasePriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"id.LATESTPRICE {match.Groups[1].Value} @latestPurchasePrice", new { latestPurchasePrice = match.Groups[2].Value });
                }
            }
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

    public async Task<IEnumerable<FgCostDetailSp>> GetFgCostDetailsByRofoIdFromSpAsync(GetFgCostDetailsByRofoIdFromSpCommand command)
    {
        const string sql = "EXEC FgCostDetail_Select_ByRofoId @RofoId";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<FgCostDetailSp>(sql, new { command.RofoId },
            transaction: command.DbTransaction);
    }

    public async Task<IEnumerable<FgCostDetailSp>> GetFgCostDetailsByRofoIdAsync(GetFgCostDetailsByRofoIdCommand command)
    {
        const string query = """
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
                             where a.RecId = @RofoId
                             ORDER BY a.RecId asc, b.ItemId asc
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _sqlConnection.QueryAsync<FgCostDetailSp>(query, new { command.RofoId },
            transaction: command.DbTransaction);
    }

    public async Task<PagedList<FgCostDetailSp>> GetFgCostDetailsByRofoIdPagedListAsync(GetFgCostDetailsByRofoIdPagedListCommand command)
    {
        const string query = """
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
        
        const string countQuery = """
                                   select count(*)
                                   from Rofo a 
                                   join Mup b ON b.RofoId = a.RecId
                                   join MupTrans c ON c.MupId = b.RecId
                                   join ItemTrans d ON d.RecId = c.ItemTransId
                                   join AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
                                   join AXGMKDW.dbo.DimItem ib ON ib.ITEMID = b.ItemId
                                   join AXGMKDW.dbo.DimItem id ON id.ITEMID = d.ItemId
                                   left join AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = b.ItemId
                                   where a.RecId = @RofoId
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
            if (productIdFilter.Contains('*') || productIdFilter.Contains('%'))
            {
                productIdFilter = productIdFilter.Replace('*', '%');
                sqlBuilder.Where($"a.ItemId LIKE @productId", new { productId = productIdFilter });
            }
            else
            {
                sqlBuilder.Where($"a.ItemId = @productId", new { productId = productIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ProductName), out var productNameFilter))
        {
            if (productNameFilter.Contains('*') || productNameFilter.Contains('%'))
            {
                productNameFilter = productNameFilter.Replace('*', '%');
                sqlBuilder.Where($"i.SEARCHNAME LIKE @productName", new { productName = productNameFilter });
            }
            else
            {
                sqlBuilder.Where($"i.SEARCHNAME = @productName", new { productName = productNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RofoDate), out var rofoDateFilter) && DateTime.TryParse(rofoDateFilter, out var rofoDate))
        {
            sqlBuilder.Where($"a.RofoDate = @rofoDate", new { rofoDate });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RofoQty), out var qtyRofoFilter))
        {
            if (decimal.TryParse(qtyRofoFilter, out var qtyRofo))
            {
                sqlBuilder.Where($"a.Qty = @qtyRofo", new { qtyRofo });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyRofoFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"a.Qty {match.Groups[1].Value} @qtyRofo", new { qtyRofo = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemId), out var itemIdFilter))
        {
            if (itemIdFilter.Contains('*') || itemIdFilter.Contains('%'))
            {
                itemIdFilter = itemIdFilter.Replace('*', '%');
                sqlBuilder.Where($"b.ItemId LIKE @itemId", new { itemId = itemIdFilter });
            }
            else
            {
                sqlBuilder.Where($"b.ItemId = @itemId", new { itemId = itemIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemName), out var itemNameFilter))
        {
            if (itemNameFilter.Contains('*') || itemNameFilter.Contains('%'))
            {
                itemNameFilter = itemNameFilter.Replace('*', '%');
                sqlBuilder.Where($"ib.SEARCHNAME LIKE @itemName", new { itemName = itemNameFilter });
            }
            else
            {
                sqlBuilder.Where($"ib.SEARCHNAME = @itemName", new { itemName = itemNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.GroupSubstitusi), out var groupSubstitusiFilter))
        {
            if (groupSubstitusiFilter.Contains('*') || groupSubstitusiFilter.Contains('%'))
            {
                groupSubstitusiFilter = groupSubstitusiFilter.Replace('*', '%');
                sqlBuilder.Where($"isnull(s.VtaMpSubstitusiGroupId,'') LIKE @groupSubstitusi", new { groupSubstitusi = groupSubstitusiFilter });
            }
            else
            {
                sqlBuilder.Where($"isnull(s.VtaMpSubstitusiGroupId,'') = @groupSubstitusi", new { groupSubstitusi = groupSubstitusiFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemAllocatedId), out var itemAllocatedIdFilter))
        {
            if (itemAllocatedIdFilter.Contains('*') || itemAllocatedIdFilter.Contains('%'))
            {
                itemAllocatedIdFilter = itemAllocatedIdFilter.Replace('*', '%');
                sqlBuilder.Where($"d.ItemId LIKE @itemAllocatedId", new { itemAllocatedId = itemAllocatedIdFilter });
            }
            else
            {
                sqlBuilder.Where($"d.ItemId = @itemAllocatedId", new { itemAllocatedId = itemAllocatedIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.ItemAllocatedName), out var itemAllocatedNameFilter))
        {
            if (itemAllocatedNameFilter.Contains('*') || itemAllocatedNameFilter.Contains('%'))
            {
                itemAllocatedNameFilter = itemAllocatedNameFilter.Replace('*', '%');
                sqlBuilder.Where($"id.SEARCHNAME LIKE @itemAllocatedName", new { itemAllocatedName = itemAllocatedNameFilter });
            }
            else
            {
                sqlBuilder.Where($"id.SEARCHNAME = @itemAllocatedName", new { itemAllocatedName = itemAllocatedNameFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.InventBatch), out var inventBatchFilter))
        {
            if (inventBatchFilter.Contains('*') || inventBatchFilter.Contains('%'))
            {
                inventBatchFilter = inventBatchFilter.Replace('*', '%');
                sqlBuilder.Where($"d.InventBatch LIKE @inventBatch", new { inventBatch = inventBatchFilter });
            }
            else
            {
                sqlBuilder.Where($"d.InventBatch = @inventBatch", new { inventBatch = inventBatchFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Qty), out var qtyFilter))
        {
            if (decimal.TryParse(qtyFilter, out var qty))
            {
                sqlBuilder.Where($"d.Qty = @qty", new { qty });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.Qty {match.Groups[1].Value} @qty", new { qty = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Price), out var priceFilter))
        {
            if (decimal.TryParse(priceFilter, out var price))
            {
                sqlBuilder.Where($"d.Price = @price", new { price });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(priceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.Price {match.Groups[1].Value} @price", new { price = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RmPrice), out var rmPriceFilter))
        {
            if (decimal.TryParse(rmPriceFilter, out var rmPrice))
            {
                sqlBuilder.Where($"d.RmPrice = @rmPrice", new { rmPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(rmPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.RmPrice {match.Groups[1].Value} @rmPrice", new { rmPrice = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.PmPrice), out var pmPriceFilter))
        {
            if (decimal.TryParse(pmPriceFilter, out var pmPrice))
            {
                sqlBuilder.Where($"d.PmPrice = @pmPrice", new { pmPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(pmPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.PmPrice {match.Groups[1].Value} @pmPrice", new { pmPrice = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.StdCostPrice), out var stdCostPriceFilter))
        {
            if (decimal.TryParse(stdCostPriceFilter, out var stdCostPrice))
            {
                sqlBuilder.Where($"d.StdCostPrice = @stdCostPrice", new { stdCostPrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(stdCostPriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"d.StdCostPrice {match.Groups[1].Value} @stdCostPrice", new { stdCostPrice = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.Source), out var sourceFilter))
        {
            if (sourceFilter.Contains('*') || sourceFilter.Contains('%'))
            {
                sourceFilter = sourceFilter.Replace('*', '%');
                sqlBuilder.Where($"d.[Source] LIKE @source", new { source = sourceFilter });
            }
            else
            {
                sqlBuilder.Where($"d.[Source] = @source", new { source = sourceFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.RefId), out var refIdFilter))
        {
            if (refIdFilter.Contains('*') || refIdFilter.Contains('%'))
            {
                refIdFilter = refIdFilter.Replace('*', '%');
                sqlBuilder.Where($"d.RefId LIKE @refId", new { refId = refIdFilter });
            }
            else
            {
                sqlBuilder.Where($"d.RefId = @refId", new { refId = refIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(FgCostDetailSp.LatestPurchasePrice), out var latestPurchasePriceFilter))
        {

            if (decimal.TryParse(latestPurchasePriceFilter, out var latestPurchasePrice))
            {
                sqlBuilder.Where($"id.LATESTPRICE = @latestPurchasePrice", new { latestPurchasePrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(latestPurchasePriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"id.LATESTPRICE {match.Groups[1].Value} @latestPurchasePrice", new { latestPurchasePrice = match.Groups[2].Value });
                }
            }
        }
        
        var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
        sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
            ? $"a.RecId {sort}"
            : $"{pagedListRequest.SortBy} {sort}");
        
        sqlBuilder.AddParameters(new
            { Offset = (pagedListRequest.PageNumber - 1) * pagedListRequest.PageSize, pagedListRequest.PageSize });
        
        var queryTemplate = sqlBuilder.AddTemplate(query);
        var fgCostDetailList = await _sqlConnection.QueryAsync<FgCostDetailSp>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        queryTemplate = sqlBuilder.AddTemplate(countQuery);
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var fgCostDetailCount = await _sqlConnection.ExecuteScalarAsync<int>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
        return new PagedList<FgCostDetailSp>(fgCostDetailList, pagedListRequest.PageNumber, pagedListRequest.PageSize, fgCostDetailCount);
    }
    
    private static void CustomMapping()
    {
        Dictionary<string, string> columnMappings = new()
        {
            { "Ratio RMPM/S", nameof(FgCostSp.RatioRmPmToSalesPrice) },
            { "Ratio CP/S", nameof(FgCostSp.RatioCostPriceToSalesPrice) }
        };

        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));

        SqlMapper.SetTypeMap(typeof(FgCostSp), new CustomPropertyTypeMap(
            typeof(FgCostSp),
            (type, columnName) => mapper(type, columnName)!));
    }
}