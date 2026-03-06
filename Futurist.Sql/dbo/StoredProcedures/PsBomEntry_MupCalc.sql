-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	Mup Calculation
-- EXEC [PsBomEntry_MupCalc] 1
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[PsBomEntry_MupCalc]
@Room int = 1
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @MessageStatus NVARCHAR(MAX)

	
	DELETE FROM [PsBomEntry_Mup] WHERE [Room] = @Room

	EXEC [PsBomEntry_BomStdInsert] @Room

	IF EXISTS(
		-- 1. Cek ItemId PsPlanBomEntry terhadap PsBomEntry_BomStd
		SELECT a.Room, a.ItemId as ProducId, i.SEARCHNAME as ProductName
			,a.PlanDate
			,ISNULL(b.BomId,'') as BomId
			,ISNULL(b.ItemId,'') as ItemId
			,ISNULL(b.ItemName,'') as ItemName
		FROM PsPlanBomEntry a with(nolock)
		LEFT JOIN PsBomEntry_BomStd b with(nolock) ON b.ProductId = a.ItemId
			AND b.Room = a.Room
			AND b.BOMID = a.BomIdEntry 
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
				AND b.BomIdEntry = a.BOMID 
			JOIN AXGMKDW.dbo.DimItem i with(nolock) ON i.ITEMID = a.[ProductId]
		--WHERE a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '2%'
		WHERE (a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '2%')
			AND a.Room = @Room
	)
	BEGIN
		SET @MessageStatus = 'Bom Std belum lengkap, silahkan cek BOM.' 
		SELECT @MessageStatus as [Message Status]	

		RETURN
	END

	
	DECLARE @Result as TABLE( 
		Room int
		,ProductId NVARCHAR(20)
		,PlanDate datetime
		,PlanQty NUMERIC(32,16)
		,ItemId NVARCHAR(20)
		,Qty NUMERIC(32,16)
		)

	INSERT INTO @Result (Room, ProductId,PlanDate,PlanQty,ItemId,Qty)
	SELECT @Room
		,a.ItemId as ProductId
		,a.PlanDate
		,a.Qty as PlanQty
		,b.ItemId 
		,b.BOMQTY / b.BOMQTYSERIE * a.Qty
	FROM PsPlanBomEntry a
	JOIN PsBomEntry_BomStd b ON b.ProductId = a.ITEMID 
		AND b.Room = a.Room
		AND b.BOMID = a.BomIdEntry 
	WHERE a.[Room] = @Room
		
	INSERT INTO [PsBomEntry_Mup] ([Room],[PlanDate],[ItemId],[Qty])
		SELECT @Room,[PlanDate],[ItemId], SUM([Qty])  
		FROM @Result
		GROUP BY [PlanDate],[ItemId]
	
	--Mengupdate BOMID4
	UPDATE PsPlanBomEntry
	SET BOMID = b.BOMID
	FROM PsPlanBomEntry a
	JOIN PsBomEntry_BomStd b ON b.ProductId = a.ITEMID 
		AND b.Room = a.Room
		AND b.BomId = a.BomIdEntry
	WHERE a.[Room] = @Room

	
	--Mengupdate BOMID6
	UPDATE PsPlanBomEntry
	SET BOMID6 = b.REF
	FROM PsPlanBomEntry a
	JOIN PsBomEntry_BomStd b ON b.ProductId = a.ITEMID 
		AND b.Room = a.Room
		AND b.BomId = a.BomIdEntry
	WHERE a.[Room] = @Room
		AND LEFT(b.REF,1) = '6'

	SET @MessageStatus = 'Proses Calculation success.' 
	SELECT @MessageStatus as [Message Status]	

	select * from PsPlanBomEntry where Room = @Room
	select * from [PsBomEntry_Mup] where Room = @Room
	
END

GO

