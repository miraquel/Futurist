-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	FgCost Detail Select
-- EXEC [FgCostDetail_Select] 1
-- durasi : 6 detik utk 94,605 rows
-- =============================================
CREATE PROCEDURE [dbo].[FgCostDetail_Select]
	@Room int = 1
AS
BEGIN
	SET NOCOUNT ON;

	select a.Room
		,a.RecId as RofoId
		,a.ItemId as ProductId
		,i.SEARCHNAME as ProductName
		,a.RofoDate
		,a.Qty as RofoQty
		,b.ItemId
		,ib.SEARCHNAME as ItemName
		,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
		,isnull(g.[GroupName],'') as [GroupProcurement]
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
	FROM Rofo a WITH (NOLOCK) 
	JOIN Mup b WITH (NOLOCK) ON b.RofoId = a.RecId
	JOIN MupTrans c WITH (NOLOCK) ON c.MupId = b.RecId
	JOIN ItemTrans d WITH (NOLOCK) ON d.RecId = c.ItemTransId
	JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
	JOIN AXGMKDW.dbo.DimItem ib WITH (NOLOCK) ON ib.ITEMID = b.ItemId
	JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s WITH (NOLOCK) ON s.ItemId = d.ItemId
	LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = d.ItemId
	WHERE a.Room = @Room
	ORDER BY a.RecId asc, b.ItemId asc
END

GO

