-- =============================================
-- Author:		<Andi>
-- Create date: <03 Jun 2025>
-- Description:	<Melihat hasil MUP Version>
-- EXEC [MupVerSelect_Det] 1,63		
-- Durasi : 1 menit, setelah index menjadi 4 detik utk 79,120 rows
-- =============================================
CREATE
 PROCEDURE [dbo].[MupVerSelect_Det]
	@Room int = 1,
	@VerId int = 63
AS
BEGIN
	SELECT 
		a.Room
		,a.VerId
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
	FROM RofoVer a  WITH (NOLOCK)
	JOIN MupVer b WITH (NOLOCK) ON b.RofoId = a.RecId AND b.VerId = a.VerId
	JOIN MupTransVer c WITH (NOLOCK) ON c.MupId = b.RecId AND c.VerId = b.VerId
	JOIN ItemTransVer d WITH (NOLOCK) ON d.RecId = c.ItemTransId AND d.VerId = c.VerId
	LEFT JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
	LEFT JOIN AXGMKDW.dbo.DimItem ib WITH (NOLOCK) ON ib.ITEMID = b.ItemId
	LEFT JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	LEFT JOIN [DimItemSubstituteVer] s WITH (NOLOCK) ON s.ItemId = b.ItemId AND s.VerId = b.VerId
	LEFT JOIN ItemPagVer p WITH (NOLOCK) ON p.RecId = d.RefId 
		AND p.Room = a.Room AND d.[Source] = 'Contract'
		AND p.VerId = a.VerId
	LEFT JOIN [ItemGroup] g ON g.ItemId = d.ItemId
	WHERE a.Room = @Room	--1	--@Room 
		AND a.VerId = @VerId	--63	--@VerId
	ORDER BY a.RofoDate ASC, a.Qty DESC, a.ItemId ASC, d.ItemId ASC
END

GO

