-- =============================================
-- Author:		<Andi>
-- Create date: <20 jan 2025>
-- Description:	<Melihat hasil MUP by Item dan BN>
-- EXEC [MupSelect_SumByBn] 1 
-- durasi: 0 detik utk 17331 rows
-- =============================================
CREATE PROCEDURE [dbo].[MupSelect_SumByBn_bck]
	@Room int = 1
AS
BEGIN
	SELECT a.Room
		,c.MupDate
		,IIF(a.[Source]='OnHand','1.OnHand',IIF(a.[Source]='PoIntransit','2.PoIntransit',IIF(a.[Source]='Contract','3.Contract',IIF(a.[Source]='Forecast','4.Forecast','5.StdCost')))) as [Source]
		,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
		,isnull(g.[GroupName],'') as [GroupProcurement]
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
	LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s WITH (NOLOCK) ON s.ItemId = a.ItemId
	LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = a.ItemId
	JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
	WHERE a.Room = @Room 
		--AND (LEFT(a.ItemId,1) = 1 OR LEFT(a.ItemId,1) = 3)
	GROUP BY a.Room,c.MupDate,a.[Source],s.VtaMpSubstitusiGroupId,g.[GroupName],a.ItemId,i.SEARCHNAME,i.UnitId,a.InventBatch
	ORDER BY s.VtaMpSubstitusiGroupId, a.[Source], a.ItemId
END

GO

