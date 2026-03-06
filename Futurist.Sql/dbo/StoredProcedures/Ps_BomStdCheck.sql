-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	BomStdCheck
-- EXEC [Ps_BomStdCheck] 1
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[Ps_BomStdCheck]
@Room int = 1
AS
BEGIN
	SELECT a.Room, a.ItemId as ProducId, i.SEARCHNAME as ProductName
		,a.PlanDate
		,ISNULL(b.BomId,'') as BomId
		,ISNULL(b.ItemId,'') as ItemId
		,ISNULL(b.ItemName,'') as ItemName
	FROM PsPlan a with(nolock)
	LEFT JOIN Ps_BomStd b with(nolock) ON b.ProductId = a.ItemId
		AND b.Room = a.Room
		AND (a.PlanDate BETWEEN b.FromDate AND b.ToDate
			OR (a.PlanDate >= b.FromDate AND b.ToDate = '1900-01-01')
			)
		--AND b.FromDate <= a.PlanDate AND (b.ToDate >= a.PlanDate OR b.ToDate = '1900-01-01')
	JOIN AXGMKDW.dbo.DimItem i with(nolock) ON i.ITEMID = a.ItemId
	WHERE a.Room = @Room
		AND b.BomId is null
	UNION 
	--2. Cek BomStd belum breakdown
	SELECT a.[Room]
		,a.[ProductId], i.SEARCHNAME as ProductName
		,b.PlanDate
		,a.[BomId]
		,a.[ItemId]
		,a.[ItemName]
	FROM Ps_BomStd a with(nolock) 
		JOIN PsPlan b ON b.ItemId = a.ProductId
			AND b.Room = a.Room
			--AND b.PlanDate BETWEEN a.FromDate AND a.ToDate
			AND (b.PlanDate BETWEEN a.FromDate AND a.ToDate
			OR (b.PlanDate >= a.FromDate AND a.ToDate = '1900-01-01')
			)
			--AND a.FromDate <= b.PlanDate AND (a.ToDate >= b.PlanDate OR a.ToDate = '1900-01-01')
		JOIN AXGMKDW.dbo.DimItem i with(nolock) ON i.ITEMID = a.[ProductId]
	WHERE a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '2%'
		AND a.Room = @Room

	
	
END

GO

