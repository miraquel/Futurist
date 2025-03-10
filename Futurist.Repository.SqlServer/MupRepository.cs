using System.Data;
using System.Data.SqlTypes;
using Dapper;
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

        if (!string.IsNullOrEmpty(pagedListRequest.Search))
        {
	        sqlBuilder.OrWhere("CAST(a.Room AS NVARCHAR) LIKE @RoomSearch", new { RoomSearch = $"{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("CAST(a.RecId AS NVARCHAR) LIKE @RofoIdSearch", new { RofoIdSearch = $"{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("a.ItemId LIKE @ProductIdSearch", new { ProductIdSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("i.SEARCHNAME LIKE @ProductNameSearch", new { ProductNameSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("CAST(a.RofoDate AS NVARCHAR) LIKE @RofoDateSearch", new { RofoDateSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("CAST(a.Qty AS NVARCHAR) LIKE @QtyRofoSearch", new { QtyRofoSearch = $"{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("b.ItemId LIKE @ItemIdSearch", new { ItemIdSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("ib.SEARCHNAME LIKE @ItemNameSearch", new { ItemNameSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("isnull(s.VtaMpSubstitusiGroupId,'') LIKE @GroupSubstitusiSearch", new { GroupSubstitusiSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("d.ItemId LIKE @ItemAllocatedIdSearch", new { ItemAllocatedIdSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("id.SEARCHNAME LIKE @ItemAllocatedNameSearch", new { ItemAllocatedNameSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("id.UnitId LIKE @UnitIdSearch", new { UnitIdSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("d.InventBatch LIKE @InventBatchSearch", new { InventBatchSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("CAST(d.Qty AS NVARCHAR) LIKE @QtySearch", new { QtySearch = $"{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("CAST(d.Price AS NVARCHAR) LIKE @PriceSearch", new { PriceSearch = $"{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("CAST(d.RmPrice AS NVARCHAR) LIKE @RmPriceSearch", new { RmPriceSearch = $"{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("CAST(d.PmPrice AS NVARCHAR) LIKE @PmPriceSearch", new { PmPriceSearch = $"{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("CAST(d.StdCostPrice AS NVARCHAR) LIKE @StdCostPriceSearch", new { StdCostPriceSearch = $"{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("d.[Source] LIKE @SourceSearch", new { SourceSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("d.RefId LIKE @RefIdSearch", new { RefIdSearch = $"%{pagedListRequest.Search}%" });
	        sqlBuilder.OrWhere("CAST(id.LATESTPRICE AS NVARCHAR) LIKE @LatestPurchasePriceSearch", new { LatestPurchasePriceSearch = $"{pagedListRequest.Search}%" });
        }
        
        if (pagedListRequest.Filter.Room != 0)
		{
			sqlBuilder.Where("a.Room = @Room", new { pagedListRequest.Filter.Room });
		}

		if (pagedListRequest.Filter.RofoDate != SqlDateTime.MinValue.Value)
		{
			sqlBuilder.Where("a.RofoDate = @RofoDate", new { pagedListRequest.Filter.RofoDate });
		}
		
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.ProductId))
		{
			sqlBuilder.Where("a.ItemId LIKE @ProductId", new { ProductId = $"%{pagedListRequest.Filter.ProductId}%" });
		}

		if (!string.IsNullOrEmpty(pagedListRequest.Filter.ProductName))
		{
			sqlBuilder.Where("i.SEARCHNAME LIKE @ProductName", new { ProductName = $"%{pagedListRequest.Filter.ProductName}%" });
		}
		
		if (pagedListRequest.Filter.QtyRofo != 0)
		{
			sqlBuilder.Where("CAST(a.Qty AS NVARCHAR) LIKE @QtyRofo", new { QtyRofo = $"{pagedListRequest.Filter.QtyRofo}%" });
		}
		if (pagedListRequest.Filter.QtyRofo != 0)
		{
			sqlBuilder.Where("CAST(a.Qty AS NVARCHAR) LIKE @QtyRofo", new { QtyRofo = $"{pagedListRequest.Filter.QtyRofo}%" });
		}
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.ItemId))
		{
			sqlBuilder.Where("b.ItemId LIKE @ItemId", new { ItemId = $"%{pagedListRequest.Filter.ItemId}%" });
		}
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.ItemName))
		{
			sqlBuilder.Where("ib.SEARCHNAME LIKE @ItemName", new { ItemName = $"%{pagedListRequest.Filter.ItemName}%" });
		}
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.GroupSubstitusi))
		{
			sqlBuilder.Where("isnull(s.VtaMpSubstitusiGroupId,'') LIKE @GroupSubstitusi", new { GroupSubstitusi = $"%{pagedListRequest.Filter.GroupSubstitusi}%" });
		}
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.ItemAllocatedId))
		{
			sqlBuilder.Where("d.ItemId LIKE @ItemAllocatedId", new { ItemAllocatedId = $"%{pagedListRequest.Filter.ItemAllocatedId}%" });
		}
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.ItemAllocatedName))
		{
			sqlBuilder.Where("id.SEARCHNAME LIKE @ItemAllocatedName", new { ItemAllocatedName = $"%{pagedListRequest.Filter.ItemAllocatedName}%" });
		}
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.UnitId))
		{
			sqlBuilder.Where("id.UnitId LIKE @UnitId", new { UnitId = $"%{pagedListRequest.Filter.UnitId}%" });
		}
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.InventBatch))
		{
			sqlBuilder.Where("d.InventBatch LIKE @InventBatch", new { InventBatch = $"%{pagedListRequest.Filter.InventBatch}%" });
		}
		if (pagedListRequest.Filter.Qty != 0)
		{
			sqlBuilder.Where("CAST(d.Qty AS NVARCHAR) LIKE @Qty", new { Qty = $"{pagedListRequest.Filter.Qty}%" });
		}
		if (pagedListRequest.Filter.Price != 0)
		{
			sqlBuilder.Where("CAST(d.Price AS NVARCHAR) LIKE @Price", new { Price = $"{pagedListRequest.Filter.Price}%" });
		}
		if (pagedListRequest.Filter.RmPrice != 0)
		{
			sqlBuilder.Where("CAST(d.RmPrice AS NVARCHAR) LIKE @RmPrice", new { RmPrice = $"{pagedListRequest.Filter.RmPrice}%" });
		}
		if (pagedListRequest.Filter.PmPrice != 0)
		{
			sqlBuilder.Where("CAST(d.PmPrice AS NVARCHAR) LIKE @PmPrice", new { PmPrice = $"{pagedListRequest.Filter.PmPrice}%" });
		}
		if (pagedListRequest.Filter.StdCostPrice != 0)
		{
			sqlBuilder.Where("CAST(d.StdCostPrice AS NVARCHAR) LIKE @StdCostPrice", new { StdCostPrice = $"{pagedListRequest.Filter.StdCostPrice}%" });
		}
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.Source))
		{
			sqlBuilder.Where("d.[Source] LIKE @Source", new { Source = $"%{pagedListRequest.Filter.Source}%" });
		}
		if (!string.IsNullOrEmpty(pagedListRequest.Filter.RefId))
		{
			sqlBuilder.Where("d.RefId LIKE @RefId", new { RefId = $"%{pagedListRequest.Filter.RefId}%" });
		}
		if (pagedListRequest.Filter.LatestPurchasePrice != 0)
		{
			sqlBuilder.Where("CAST(id.LATESTPRICE AS NVARCHAR) LIKE @LatestPurchasePrice", new { LatestPurchasePrice = $"{pagedListRequest.Filter.LatestPurchasePrice}%" });
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