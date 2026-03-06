-- =============================================
-- Author:		<Andi>
-- Create date: <20 jan 2025>
-- Description:	<Melihat hasil MUP>
-- EXEC [MupSelect_Det_new] 99		
-- Durasi : 1 menit, setelah index menjadi 3 detik utk 96,874 rows
-- =============================================
CREATE PROCEDURE [dbo].[MupSelect_Det_new]
	@Room int = 99
AS
BEGIN
	SELECT 
		a.Room
		,a.RecId as RofoId
		,a.ItemId as ProductId
		,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ProductName
		,a.RofoDate
		,a.Qty as QtyRofo
		,b.ItemId
		,REPLACE(REPLACE(REPLACE(ib.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemName
		,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
		,isnull(g.[Group01],'') as [GroupProcurement]
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
		,isnull(p.CurrencyCode,'') as [OriginalCurrency]
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
	LEFT JOIN ItemPagRoom p WITH (NOLOCK) ON p.RecId = d.RefId AND p.Room = a.Room AND d.[Source] = 'Contract'
	--LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = d.ItemId
	LEFT JOIN [ItemGroup] g ON g.ItemId = d.ItemId
	WHERE a.Room = @Room
	ORDER BY a.RofoDate ASC, a.Qty DESC, a.ItemId ASC, d.ItemId ASC
END

GO

