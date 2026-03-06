-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	FgCost Detail Select
-- EXEC [FgCostDetail_Select_ByRofoId] 66580
-- =============================================
CREATE PROCEDURE [dbo].[FgCostDetail_Select_ByRofoId_bck]
	@RofoId int = 61139
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Result as table (
		RofoId int
		,Room int
		,ProductId nvarchar(20)
		,ProductName nvarchar(60)
		,RofoDate datetime
		,QtyRofo numeric(32,16)
		,ItemId nvarchar(20)
		,ItemName nvarchar(60)
		,[Group Substitusi] nvarchar(20)
		,ItemAllocatedId nvarchar(20)
		,ItemAllocatedName nvarchar(60)
		,UnitId nvarchar(20)
		,[InventBatch] nvarchar(20)
		,Qty numeric(32,16)
		,Price numeric(32,16)
		,RmPrice numeric(32,16)
		,PmPrice numeric(32,16)
		,StdCostPrice numeric(32,16)
		,[Source] nvarchar(20) 
		,RefId int
		,LatestPrice numeric(32,16)
	)

	INSERT INTO @Result
	SELECT a.RecId as RofoId
		,a.Room
		,a.ItemId as ProductId
		,i.SEARCHNAME as ProductName
		,a.RofoDate
		,a.Qty as QtyRofo
		,b.ItemId
		,ib.SEARCHNAME as ItemName
		,isnull(s.VtaMpSubstitusiGroupId,'') as [Group Substitusi]
		,d.ItemId as ItemAllocatedId
		,id.SEARCHNAME as ItemAllocatedName
		,id.[UNITID]
		,d.[InventBatch]
		,d.Qty
		,d.Price
		,d.RmPrice
		,d.PmPrice
		,d.StdCostPrice
		,d.[Source]
		,d.RefId
		,id.LatestPrice
	FROM RofoRoom a WITH (NOLOCK) 
	JOIN Mup b WITH (NOLOCK) ON b.RofoId = a.RecId
	JOIN MupTrans c WITH (NOLOCK) ON c.MupId = b.RecId
	JOIN ItemTrans d WITH (NOLOCK) ON d.RecId = c.ItemTransId
	JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
	JOIN AXGMKDW.dbo.DimItem ib WITH (NOLOCK) ON ib.ITEMID = b.ItemId
	JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s WITH (NOLOCK) ON s.ItemId = b.ItemId
	WHERE a.RecId = @RofoId
	ORDER BY a.RecId asc, d.ItemId asc

	----Updet harga dari ItemAdjRoom karena harga nol
	--UPDATE @Result
	--SET Price = b.ForcedPrice
	--	,[Source] = 'Forecast by user'
	--FROM @Result a
	--JOIN [ItemForecastRoom] b ON b.Room = a.Room AND b.ItemId = a.ItemAllocatedId AND b.ForcedPrice <> 0
	--WHERE a.[Source] = 'Forecast'

	----Updet harga dari ItemAdjRoom karena harga nol
	--UPDATE @Result
	--SET Price = b.AdjPrice
	--	,[Source] = 'AdjPrice by user'
	--FROM @Result a
	--JOIN ItemAdjRoom b ON b.Room = a.Room AND b.ItemId = a.ItemAllocatedId
	--WHERE a.[Price] = 0
	
	SELECT RofoId,Room,ProductId,ProductName,RofoDate,QtyRofo as RofoQty,ItemId,ItemName
		,[Group Substitusi] as [GroupSubstitusi]
		,ItemAllocatedId,ItemAllocatedName
		,[UnitId]
		,[InventBatch]
		,Qty,Price,RmPrice,PmPrice,StdCostPrice,[Source],RefId,LatestPrice as LatestPurchasePrice
		FROM @Result 
		ORDER BY ItemAllocatedId
END

GO

