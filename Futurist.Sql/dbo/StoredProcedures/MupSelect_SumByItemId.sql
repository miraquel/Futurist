-- =============================================
-- Author:		Andi
-- Create date: 22 jan 2025
-- Description:	untuk monitoring Material per bulan
-- exec [MupSelect_SumByItemId] 1
-- durasi: 0 detik utk 12827 rows
-- =============================================
CREATE PROCEDURE [dbo].[MupSelect_SumByItemId]
	@Room int=1
AS
BEGIN
	--SET NOCOUNT ON;
		
	SELECT a.Room
		,c.MupDate as [MupDate]
		,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
		,isnull(g.[Group01],'') as [GroupProcurement]
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
	LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s WITH (NOLOCK) ON s.ItemId = a.ItemId
	--LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = a.ItemId
	LEFT JOIN [ItemGroup] g ON g.ItemId = c.ItemId
	JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
	WHERE a.Room = @Room	--1
		--AND (LEFT(a.ItemId,1) = 1 OR LEFT(a.ItemId,1) = 3)
	GROUP BY a.Room, c.MupDate,s.VtaMpSubstitusiGroupId,g.[Group01],a.ItemId,i.SEARCHNAME,i.UnitId
	ORDER BY s.VtaMpSubstitusiGroupId, a.ItemId
END

GO

