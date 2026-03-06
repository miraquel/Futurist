-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	BomStdCheck
-- EXEC [PsBomEntry_BomStdCheck] 1
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[PsBomEntry_BomStdCheck]
@Room int = 1
AS
BEGIN
	SELECT a.Room, a.ItemId as ProducId, i.SEARCHNAME as ProductName
		,a.PlanDate
		,ISNULL(b.BomId,'') as BomId
		,ISNULL(b.ItemId,'') as ItemId
		,ISNULL(b.ItemName,'') as ItemName
	FROM PsPlanBomEntry a with(nolock)
	LEFT JOIN PsBomEntry_BomStd b with(nolock) ON b.ProductId = a.ItemId
		AND b.Room = a.Room
		AND b.BomId = a.BomIdEntry
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
	FROM PsBomEntry_BomStd a with(nolock) 
		JOIN PsPlanBomEntry b ON b.ItemId = a.ProductId
			AND b.Room = a.Room
			AND b.BomIdEntry = a.BomId
		JOIN AXGMKDW.dbo.DimItem i with(nolock) ON i.ITEMID = a.[ProductId]
	--WHERE a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '2%'
	WHERE (a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '2%')
		AND a.Room = @Room

	
	
END

GO

