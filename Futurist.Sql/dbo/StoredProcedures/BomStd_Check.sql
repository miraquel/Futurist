-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Bom Standart yang tidak lengkap
-- exec [BomStd_Check] 8
-- =============================================
CREATE PROCEDURE [dbo].[BomStd_Check]
	@Room int = 5
AS
BEGIN
	--SET NOCOUNT ON;	
	-- 1. Cek ItemId ROFO terhadap BomStd
	SELECT a.Room, a.ItemId as ProductId, i.SEARCHNAME as ProductName
			,a.RofoDate as RofoDate
			,ISNULL(b.BomId,'') as BomId
			,ISNULL(b.ItemId,'') as ItemId
			,ISNULL(b.ItemName,'') as ItemName
		FROM Rofo a
		LEFT JOIN BomStd b ON b.ProductId = a.ItemId
			AND b.Room = @Room
			AND b.FromDate <= a.RofoDate AND (b.ToDate >= a.RofoDate OR b.ToDate = '1900-01-01')
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
		WHERE a.Room = @Room
			AND b.BomId is null

	UNION 

	--2. Cek BomStd belum breakdown
	SELECT a.[Room]
		,a.[ProductId], i.SEARCHNAME as ProductName
		,'' as RofoDate
		,a.[BomId]
		,a.[ItemId]
		,a.[ItemName]
	FROM [BomStd] a
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.[ProductId]
	WHERE (a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '2%')
		AND a.Room = @Room
END

GO

