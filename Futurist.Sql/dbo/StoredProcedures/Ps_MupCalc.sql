-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	Mup Calculation
-- EXEC [Ps_MupCalc] 99
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[Ps_MupCalc]
@Room int = 1
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @MessageStatus NVARCHAR(MAX)

	
	DELETE FROM [Ps_Mup] WHERE [Room] = @Room

	EXEC [Ps_BomStdInsert] @Room

	IF EXISTS(
		-- 1. Cek ItemId PsPlan terhadap Ps_BomStd
		SELECT a.Room, a.ItemId as ProducId, i.SEARCHNAME as ProductName
			,a.PlanDate
			,ISNULL(b.BomId,'') as BomId
			,ISNULL(b.ItemId,'') as ItemId
			,ISNULL(b.ItemName,'') as ItemName
		FROM PsPlan a with(nolock)
		LEFT JOIN Ps_BomStd b with(nolock) ON b.ProductId = a.ItemId
			AND b.Room = a.Room
			--AND a.PlanDate BETWEEN b.FromDate AND b.ToDate
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
	FROM PsPlan a
	JOIN Ps_BomStd b ON b.ProductId = a.ITEMID 
		AND b.Room = a.Room
		AND a.PlanDate BETWEEN b.FromDate AND b.ToDate
	WHERE a.[Room] = @Room
		
	INSERT INTO [Ps_Mup] ([Room],[PlanDate],[ItemId],[Qty])
		SELECT @Room,[PlanDate],[ItemId], SUM([Qty])  
		FROM @Result
		GROUP BY [PlanDate],[ItemId]
	
	--Mengupdate BOMID4
	UPDATE PsPlan
	SET BOMID = b.BOMID
	FROM PsPlan a
	JOIN Ps_BomStd b ON b.ProductId = a.ITEMID 
		AND b.Room = a.Room
		AND a.PlanDate BETWEEN b.FromDate AND b.ToDate
	WHERE a.[Room] = @Room

	
	--Mengupdate BOMID6
	UPDATE PsPlan
	SET BOMID6 = b.REF
	FROM PsPlan a
	JOIN Ps_BomStd b ON b.ProductId = a.ITEMID 
		AND b.Room = a.Room
		AND a.PlanDate BETWEEN b.FromDate AND b.ToDate
	WHERE a.[Room] = @Room
		AND LEFT(b.REF,1) = '6'

	SET @MessageStatus = 'Proses Calculation success.' 
	SELECT @MessageStatus as [Message Status]	

	--select * from PsPlan where Room = @Room
	--select * from [Ps_Mup] where Room = @Room
	
END

GO

