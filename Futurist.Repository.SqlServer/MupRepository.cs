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

    public async Task<SpTask?> ProcessMupAsync(ProcessMupCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON;");
        const string query = "EXEC MupCalcRoom @Room";
        return await _sqlConnection.QuerySingleOrDefaultAsync<SpTask>(
            query, 
            new { Room = command.RoomId }, 
            command.DbTransaction,
            commandTimeout: command.Timeout);
    }

    public async Task<IEnumerable<MupSp>> MupResultAsync(MupResultCommand command)
    {
	    await _sqlConnection.ExecuteAsync("SET ARITHABORT ON");
        const string query = "EXEC MupSelect_Det @Room";
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
                             	,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ProductName
                             	,a.RofoDate
                             	,a.Qty as QtyRofo
                             	,b.ItemId
                             	,REPLACE(REPLACE(REPLACE(ib.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemName
                             	,isnull(s.VtaMpSubstitusiGroupId,'') as [Group Substitusi]
                             	,d.ItemId as ItemAllocatedId
                             	,REPLACE(REPLACE(REPLACE(id.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemAllocatedName
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
                             	,isnull(d.Price/nullif(id.LATESTPRICE,0),0) as [Gap]
                             FROM Rofo a  WITH (NOLOCK)
                             JOIN Mup b WITH (NOLOCK) ON b.RofoId = a.RecId
                             JOIN MupTrans c WITH (NOLOCK) ON c.MupId = b.RecId
                             JOIN ItemTrans d WITH (NOLOCK) ON d.RecId = c.ItemTransId
                             JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
                             JOIN AXGMKDW.dbo.DimItem ib WITH (NOLOCK) ON ib.ITEMID = b.ItemId
                             JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
                             LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s WITH (NOLOCK) ON s.ItemId = b.ItemId
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

    public async Task<IEnumerable<MupSp>> MupSummaryByItemIdFromSpAsync(MupSummaryByItemIdFromSpCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON");
        const string query = "EXEC MupSelect_SumByItemId @Room";
        return await _sqlConnection.QueryAsync<MupSp>(
            query,
            new { Room = command.RoomId },
            command.DbTransaction);
    }

    public async Task<IEnumerable<MupSp>> MupSummaryByItemIdAsync(MupSummaryByItemIdCommand command)
    {
        const string query = """
                             SELECT c.MupDate as [MupDate]
                             	,isnull(s.VtaMpSubstitusiGroupId,'') as [Group Substitusi]
                             	,a.ItemId as [ItemAllocatedId]
                             	,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemName
                             	,i.UnitId as [UnitId]
                             	,sum(a.Qty) as Qty
                             	,sum(a.Qty * a.Price) / sum(a.Qty) as [Price] 
                             	,MAX(i.LATESTPRICE) as [LatestPurchasePrice]
                             	,( sum(a.Qty * a.Price) / sum(a.Qty)) / NULLIF(MAX(i.LATESTPRICE),0) as [Gap %]			
                             FROM ItemTrans a WITH (NOLOCK) 
                             JOIN MupTrans b WITH (NOLOCK) ON b.ItemTransId = a.RecId
                             JOIN Mup c WITH (NOLOCK) ON c.RecId = b.MupId
                             JOIN Rofo d WITH (NOLOCK) ON d.RecId = c.RofoId
                             LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s WITH (NOLOCK) ON s.ItemId = c.ItemId
                             JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
                             /**where**/
                             GROUP BY c.MupDate,s.VtaMpSubstitusiGroupId,a.ItemId,i.SEARCHNAME,i.UnitId
                             /**having**/
                             /**orderby**/
                             """;

        var sqlBuilder = new SqlBuilder();
        
        var listRequest = command.ListRequest;
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.Room), out var roomFilter) && int.TryParse(roomFilter, out var room))
        {
            sqlBuilder.Where("a.Room = @Room", new { Room = room });
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.MupDate), out var mupDateFilter) && DateTime.TryParse(mupDateFilter, out var mupDate))
        {
            sqlBuilder.Where("DATEFROMPARTS(YEAR(m.MupDate),MONTH(m.MupDate),1) = @MupDate", new { MupDate = mupDate });
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.ItemId), out var itemIdFilter))
        {
            if (itemIdFilter.Contains('*') || itemIdFilter.Contains('%'))
            {
                itemIdFilter = itemIdFilter.Replace("*", "%");
                sqlBuilder.Where("a.ItemId LIKE @ItemId", new { ItemId = itemIdFilter });
            }
            else
            {
                sqlBuilder.Where("a.ItemId = @ItemId", new { ItemId = itemIdFilter });
            }
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.ItemName), out var itemNameFilter))
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
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.GroupSubstitusi), out var groupSubstitusiFilter))
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
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.Price), out var priceFilter))
        {
            if (decimal.TryParse(priceFilter, out var price))
            {
                sqlBuilder.Having("sum(a.Qty * a.Price) / sum(a.Qty) = @Price", new { Price = price });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(priceFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"sum(a.Qty * a.Price) / sum(a.Qty) {match.Groups[1].Value} @Price", new { Price = match.Groups[2].Value });
                }
            }
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.Qty), out var qtyFilter))
        {
            if (decimal.TryParse(qtyFilter, out var qty))
            {
                sqlBuilder.Having("sum(a.Qty) = @Qty", new { Qty = qty });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"sum(a.Qty) {match.Groups[1].Value} @Qty", new { Gap = match.Groups[2].Value });
                }
            }
        }
        
		var sort = listRequest.IsSortAscending ? "ASC" : "DESC";
		sqlBuilder.OrderBy(string.IsNullOrEmpty(listRequest.SortBy)
			? "s.VtaMpSubstitusiGroupId, a.ItemId"
			: $"{listRequest.SortBy} {sort}");

		var queryTemplate = sqlBuilder.AddTemplate(query);
		await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
		return await _sqlConnection.QueryAsync<MupSp>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
    }

    public async Task<PagedList<MupSp>> MupSummaryByItemIdPagedListAsync(MupSummaryByItemIdPagedListCommand command)
    {
        const string query = """
                             SELECT 
                                 DATEFROMPARTS(YEAR(m.MupDate),MONTH(m.MupDate),1) as [MupDate]
                                ,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
                                ,a.ItemId, 
                                i.SEARCHNAME as ItemName
                                ,sum(a.Qty) as Qty
                                ,sum(a.Qty * a.Price) / sum(a.Qty) as [Price] 
                             FROM [ItemTrans] a
                             JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
                             JOIN [MupTrans] mt ON mt.ItemTransId = a.RecId
                             JOIN [Mup] m ON m.RecId = mt.MupId
                             LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = m.ItemId
                             /**where**/
                             GROUP BY DATEFROMPARTS(YEAR(m.MupDate),MONTH(m.MupDate),1), a.ItemId, i.SEARCHNAME, s.VtaMpSubstitusiGroupId
                             /**having**/
                             /**orderby**/
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                             """;

        const string countQuery = """
                                  SELECT COUNT(*)
                                  FROM [ItemTrans] a
                                  JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
                                  JOIN [MupTrans] mt ON mt.ItemTransId = a.RecId
                                  JOIN [Mup] m ON m.RecId = mt.MupId
                                  LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = m.ItemId
                                  /**where**/
                                  GROUP BY DATEFROMPARTS(YEAR(m.MupDate),MONTH(m.MupDate),1), a.ItemId, i.SEARCHNAME, s.VtaMpSubstitusiGroupId
                                  /**having**/
                                  """;

        var sqlBuilder = new SqlBuilder();

        var pagedListRequest = command.PagedListRequest;
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.Room), out var roomFilter) && int.TryParse(roomFilter, out var room))
        {
            sqlBuilder.Where("a.Room = @Room", new { Room = room });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.MupDate), out var mupDateFilter) && DateTime.TryParse(mupDateFilter, out var mupDate))
        {
            sqlBuilder.Where("DATEFROMPARTS(YEAR(m.MupDate),MONTH(m.MupDate),1) = @MupDate", new { MupDate = mupDate });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.ItemId), out var itemIdFilter))
        {
            if (itemIdFilter.Contains('*') || itemIdFilter.Contains('%'))
            {
                itemIdFilter = itemIdFilter.Replace("*", "%");
                sqlBuilder.Where("a.ItemId LIKE @ItemId", new { ItemId = itemIdFilter });
            }
            else
            {
                sqlBuilder.Where("a.ItemId = @ItemId", new { ItemId = itemIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.ItemName), out var itemNameFilter))
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
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.GroupSubstitusi), out var groupSubstitusiFilter))
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
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.Price), out var priceFilter))
        {
            if (decimal.TryParse(priceFilter, out var price))
            {
                sqlBuilder.Having("sum(a.Qty * a.Price) / sum(a.Qty) = @Price", new { Price = price });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(priceFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"sum(a.Qty * a.Price) / sum(a.Qty) {match.Groups[1].Value} @Price", new { Price = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.Qty), out var qtyFilter))
        {
            if (decimal.TryParse(qtyFilter, out var qty))
            {
                sqlBuilder.Having("sum(a.Qty) = @Qty", new { Qty = qty });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"sum(a.Qty) {match.Groups[1].Value} @Qty", new { Gap = match.Groups[2].Value });
                }
            }
        }
        
		var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
		sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
			? "[MupDate] ASC, VtaMpSubstitusiGroupId, a.ItemId"
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

    public async Task<IEnumerable<MupSp>> MupSummaryByBatchNumberFromSpAsync(MupSummaryByBatchNumberFromSpCommand command)
    {
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON");
        const string query = "EXEC MupSelect_SumByBn @Room";
        return await _sqlConnection.QueryAsync<MupSp>(
            query,
            new { Room = command.RoomId },
            command.DbTransaction);
    }

    public async Task<IEnumerable<MupSp>> MupSummaryByBatchNumberAsync(MupSummaryByBatchNumberCommand command)
    {
        const string query = """
                             SELECT c.MupDate
                             	,IIF(a.[Source]='OnHand','1.OnHand',IIF(a.[Source]='PoIntransit','2.PoIntransit',IIF(a.[Source]='Contract','3.Contract',IIF(a.[Source]='Forecast','4.Forecast','5.StdCost')))) as [Source]
                             	,isnull(s.VtaMpSubstitusiGroupId,'') as [Group Substitusi]
                             	,a.ItemId as ItemAllocatedId
                             	,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemAllocatedName
                             	,i.UnitId
                             	,ISNULL(a.InventBatch,'') as [InventBatch]
                             	,SUM(a.Qty)	as [Qty]	
                             	,MAX(a.Price) as [Price]
                             	,MAX(i.LATESTPRICE) as [LatestPurchasePrice]
                             	,isnull(MAX(a.Price)/nullif(MAX(i.LATESTPRICE),0),0) as [Gap]				
                             FROM ItemTrans a WITH (NOLOCK) 
                             JOIN MupTrans b WITH (NOLOCK) ON b.ItemTransId = a.RecId
                             JOIN Mup c WITH (NOLOCK) ON c.RecId = b.MupId
                             JOIN Rofo d WITH (NOLOCK) ON d.RecId = c.RofoId
                             LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s WITH (NOLOCK) ON s.ItemId = c.ItemId
                             JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
                             /**where**/
                             GROUP BY c.MupDate,a.[Source],s.VtaMpSubstitusiGroupId,a.ItemId,i.SEARCHNAME,i.UnitId,a.InventBatch
                             /**orderby**/
                             """;

        var sqlBuilder = new SqlBuilder();

        var listRequest = command.ListRequest;
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.Room), out var roomFilter) && int.TryParse(roomFilter, out var room))
        {
            sqlBuilder.Where("a.Room = @Room", new { Room = room });
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.MupDate), out var mupDateFilter) && DateTime.TryParse(mupDateFilter, out var mupDate))
        {
            sqlBuilder.Having("MIN(b.MupDate) = @MupDate", new { MupDate = mupDate });
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.Source), out var sourceFilter))
        {
            if (sourceFilter.Contains('*') || sourceFilter.Contains('%'))
            {
                sourceFilter = sourceFilter.Replace("*", "%");
                sqlBuilder.Where("IIF([Source]='OnHand','1.OnHand',IIF([Source]='PoIntransit','2.PoIntransit',IIF([Source]='Contract','3.Contract',IIF([Source]='Forecast','4.Forecast','5.StdCost')))) LIKE @Source", new { Source = sourceFilter });
            }
            else
            {
                sqlBuilder.Where("IIF([Source]='OnHand','1.OnHand',IIF([Source]='PoIntransit','2.PoIntransit',IIF([Source]='Contract','3.Contract',IIF([Source]='Forecast','4.Forecast','5.StdCost')))) = @Source", new { Source = sourceFilter });
            }
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.GroupSubstitusi), out var groupSubstitusiFilter))
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
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.ItemAllocatedId), out var itemAllocatedIdFilter))
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
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.ItemAllocatedName), out var itemAllocatedNameFilter))
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
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.UnitId), out var unitIdFilter))
        {
            if (unitIdFilter.Contains('*') || unitIdFilter.Contains('%'))
            {
                unitIdFilter = unitIdFilter.Replace("*", "%");
                sqlBuilder.Where("id.UnitId LIKE @UnitId", new { UnitId = unitIdFilter });
            }
            else
            {
                sqlBuilder.Where("id.UnitId = @UnitId", new { UnitId = unitIdFilter });
            }
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.InventBatch), out var inventBatchFilter))
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
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.Qty), out var qtyFilter))
        {
            if (decimal.TryParse(qtyFilter, out var qty))
            {
                sqlBuilder.Having("SUM(d.Qty) = @Qty", new { Qty = qty });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"SUM(d.Qty) {match.Groups[1].Value} @Qty", new { Gap = match.Groups[2].Value });
                }
            }
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.Price), out var priceFilter))
        {
            if (decimal.TryParse(priceFilter, out var price))
            {
                sqlBuilder.Having("MAX(d.Price) = @Price", new { Price = price });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(priceFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"MAX(d.Price) {match.Groups[1].Value} @Price", new { Price = match.Groups[2].Value });
                }
            }
        }
        
        if (listRequest.Filters.TryGetValue(nameof(MupSp.LatestPurchasePrice), out var latestPurchasePriceFilter))
        {
            if (decimal.TryParse(latestPurchasePriceFilter, out var latestPurchasePrice))
            {
                sqlBuilder.Having("MAX(id.LATESTPRICE) = @LatestPurchasePrice", new { LatestPurchasePrice = latestPurchasePrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(latestPurchasePriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"MAX(id.LATESTPRICE) {match.Groups[1].Value} @LatestPurchasePrice", new { LatestPurchasePrice = match.Groups[2].Value });
                }
            }
        }
        
		var sort = listRequest.IsSortAscending ? "ASC" : "DESC";
		sqlBuilder.OrderBy(string.IsNullOrEmpty(listRequest.SortBy)
			? "s.VtaMpSubstitusiGroupId, a.[Source], a.ItemId"
			: $"{listRequest.SortBy} {sort}");
        
		var queryTemplate = sqlBuilder.AddTemplate(query);
		await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
		return await _sqlConnection.QueryAsync<MupSp>(queryTemplate.RawSql, queryTemplate.Parameters, command.DbTransaction);
    }

    public async Task<PagedList<MupSp>> MupSummaryByBatchNumberPagedListAsync(MupSummaryByBatchNumberPagedListCommand command)
    {
        const string query = """
                             SELECT MIN(b.MupDate) as [MupDate]
                             	,IIF([Source]='OnHand','1.OnHand',IIF([Source]='PoIntransit','2.PoIntransit',IIF([Source]='Contract','3.Contract',IIF([Source]='Forecast','4.Forecast','5.StdCost')))) as [Source]
                             	,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
                             	,d.ItemId as ItemAllocatedId
                             	,id.SEARCHNAME as ItemAllocatedName
                             	,id.UnitId
                             	,ISNULL(d.InventBatch,'') as [InventBatch]
                             	,SUM(d.Qty) as [Qty]
                             	,MAX(d.Price) as [Price]
                             	,MAX(id.LATESTPRICE) as [LatestPurchasePrice]
                             	,isnull(MAX(d.Price)/nullif(MAX(id.LATESTPRICE),0),0) as [Gap]
                             FROM Rofo a 
                             join Mup b ON b.RofoId = a.RecId
                             join MupTrans c ON c.MupId = b.RecId
                             join ItemTrans d ON d.RecId = c.ItemTransId
                             join AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
                             join AXGMKDW.dbo.DimItem ib ON ib.ITEMID = b.ItemId
                             join AXGMKDW.dbo.DimItem id ON id.ITEMID = d.ItemId
                             left join AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = b.ItemId
                             /**where**/
                             GROUP BY d.ItemId, id.SEARCHNAME, s.VtaMpSubstitusiGroupId, id.UnitId, d.InventBatch, d.[Source]
                             /**orderby**/
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                             """;

        const string countQuery = """
                                  SELECT COUNT(*)
                                  FROM Rofo a 
                                  join Mup b ON b.RofoId = a.RecId
                                  join MupTrans c ON c.MupId = b.RecId
                                  join ItemTrans d ON d.RecId = c.ItemTransId
                                  join AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
                                  join AXGMKDW.dbo.DimItem ib ON ib.ITEMID = b.ItemId
                                  join AXGMKDW.dbo.DimItem id ON id.ITEMID = d.ItemId
                                  left join AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = b.ItemId
                                  /**where**/
                                  GROUP BY d.ItemId, id.SEARCHNAME, s.VtaMpSubstitusiGroupId, id.UnitId, d.InventBatch, d.[Source]
                                  /**having**/
                                  /**orderby**/
                                  """;

        var sqlBuilder = new SqlBuilder();

        var pagedListRequest = command.PagedListRequest;
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.Room), out var roomFilter) && int.TryParse(roomFilter, out var room))
        {
            sqlBuilder.Where("a.Room = @Room", new { Room = room });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.MupDate), out var mupDateFilter) && DateTime.TryParse(mupDateFilter, out var mupDate))
        {
            sqlBuilder.Having("MIN(b.MupDate) = @MupDate", new { MupDate = mupDate });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.Source), out var sourceFilter))
        {
            if (sourceFilter.Contains('*') || sourceFilter.Contains('%'))
            {
                sourceFilter = sourceFilter.Replace("*", "%");
                sqlBuilder.Where("IIF([Source]='OnHand','1.OnHand',IIF([Source]='PoIntransit','2.PoIntransit',IIF([Source]='Contract','3.Contract',IIF([Source]='Forecast','4.Forecast','5.StdCost')))) LIKE @Source", new { Source = sourceFilter });
            }
            else
            {
                sqlBuilder.Where("IIF([Source]='OnHand','1.OnHand',IIF([Source]='PoIntransit','2.PoIntransit',IIF([Source]='Contract','3.Contract',IIF([Source]='Forecast','4.Forecast','5.StdCost')))) = @Source", new { Source = sourceFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.GroupSubstitusi), out var groupSubstitusiFilter))
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
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.ItemAllocatedId), out var itemAllocatedIdFilter))
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
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.ItemAllocatedName), out var itemAllocatedNameFilter))
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
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.UnitId), out var unitIdFilter))
        {
            if (unitIdFilter.Contains('*') || unitIdFilter.Contains('%'))
            {
                unitIdFilter = unitIdFilter.Replace("*", "%");
                sqlBuilder.Where("id.UnitId LIKE @UnitId", new { UnitId = unitIdFilter });
            }
            else
            {
                sqlBuilder.Where("id.UnitId = @UnitId", new { UnitId = unitIdFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.InventBatch), out var inventBatchFilter))
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
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.Qty), out var qtyFilter))
        {
            if (decimal.TryParse(qtyFilter, out var qty))
            {
                sqlBuilder.Having("SUM(d.Qty) = @Qty", new { Qty = qty });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(qtyFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"SUM(d.Qty) {match.Groups[1].Value} @Qty", new { Gap = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.Price), out var priceFilter))
        {
            if (decimal.TryParse(priceFilter, out var price))
            {
                sqlBuilder.Having("MAX(d.Price) = @Price", new { Price = price });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(priceFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"MAX(d.Price) {match.Groups[1].Value} @Price", new { Price = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(MupSp.LatestPurchasePrice), out var latestPurchasePriceFilter))
        {
            if (decimal.TryParse(latestPurchasePriceFilter, out var latestPurchasePrice))
            {
                sqlBuilder.Having("MAX(id.LATESTPRICE) = @LatestPurchasePrice", new { LatestPurchasePrice = latestPurchasePrice });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(latestPurchasePriceFilter);
                if (match.Success)
                {
                    sqlBuilder.Having($"MAX(id.LATESTPRICE) {match.Groups[1].Value} @LatestPurchasePrice", new { LatestPurchasePrice = match.Groups[2].Value });
                }
            }
        }
        
        sqlBuilder.Where("LEFT(d.ItemId,1) = 1 OR LEFT(d.ItemId,1) = 3");
        
		var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
		sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
			? "VtaMpSubstitusiGroupId, [Source], d.ItemId"
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