-- =============================================
-- Author:		Andi
-- Create date: 22 Sep 2025
-- Description:	Bom Standart Insert untuk keperluan Production Simulation (PS)
-- EXEC [Ps_BomStdInsert] 1
-- Durasi : 2 menit, after index 3 detik
-- =============================================
CREATE PROCEDURE [dbo].[Ps_BomStdInsert]
	@Room int = 1
	,@CreatedBy nvarchar(20) = 'Unknown'
AS
BEGIN
	--SET NOCOUNT ON;		
	DECLARE @Level int

	DELETE FROM Ps_BomStd WHERE Room = @Room
	
----**PROSES LEVEL PERTAMA (1)
	SET @Level = 1

	INSERT INTO Ps_BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName],[BomQty],[BomQtySerie]
      ,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)
	SELECT @Room,a.[BOMID],a.[PRODUCTID],a.[ITEMID],a.[ITEMNAME],a.[BOMQTY],a.[BOMQTYSERIE]
		,a.[LINETYPE],a.[SUBBOMID],a.[BomId],@Level,a.[FromDate],a.[ToDate],@CreatedBy,GETDATE()
	  FROM AXGMKDW.dbo.[FactBom] a
	  JOIN PsPlan b ON b.ItemId = a.[ProductId]
		AND b.Room = @Room
		AND a.Active = 1
		AND a.FROMQTY = 1
		AND (b.PlanDate BETWEEN a.FROMDATE AND a.TODATE
				OR (a.FromDate <= b.PlanDate AND a.ToDate = '1900-01-01')
			)	
				 
----**PROSES LEVEL KEDUA DST (2)
    --**BREAKDOWN UTK DATA YG ADA SUBBOM NYA.
    DECLARE @RecId BIGINT
			,@BomId NVARCHAR(20)
            ,@ProductId NVARCHAR(20)
            ,@ItemId NVARCHAR(20)
			,@ItemName NVARCHAR(60)
			,@BomQty NUMERIC(32,16)
			,@BomQtySerie NUMERIC(32,16)
			,@LineType INT
            ,@SubBomId NVARCHAR(20)
			,@FromDate datetime
			,@ToDate datetime

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE SubBomId <> ''
			AND [Level] = 1

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId )
		BEGIN		

			INSERT INTO Ps_BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level]
				,[FromDate],[ToDate],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 2
					,@FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM Ps_BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 1

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 
				AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE()) )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO Ps_BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 2, @FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 
						AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE()) )
				
			DELETE FROM Ps_BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02
	
	
	----**PROSES LEVEL KETIGA DST (3)
    --**BREAKDOWN UTK DATA YG ADA SUBBOM NYA.

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE SubBomId <> ''
			AND [Level] = 2

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId)
		BEGIN		
			--SET @Level = @Level+1

			INSERT INTO Ps_BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 3, @FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM Ps_BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 2

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 
					AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE()) )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO Ps_BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 3, @FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 
						AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE()) )
				
			DELETE FROM Ps_BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02



	

	----**PROSES LEVEL KEEMPAT DST (4)
    --**BREAKDOWN UTK DATA YG ADA SUBBOM NYA.

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE SubBomId <> ''
			AND [Level] = 3

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId)
		BEGIN		
			--SET @Level = @Level+1

			INSERT INTO Ps_BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 4, @FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM Ps_BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 3

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 
					AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE()) )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO Ps_BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 4, @FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 
						AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE()) )
				
			DELETE FROM Ps_BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02


	----**PROSES LEVEL KELIMA DST (5)
    --**BREAKDOWN UTK DATA YG ADA SUBBOM NYA.

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE SubBomId <> ''
			AND [Level] = 4

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId)
		BEGIN		
			--SET @Level = @Level+1

			INSERT INTO Ps_BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 5, @FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM Ps_BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 4

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 
					AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE())  )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO Ps_BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 5, @FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 
						AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE()) )
				
			DELETE FROM Ps_BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02




	

	----**PROSES LEVEL KEENAM DST (6)
    --**BREAKDOWN UTK DATA YG ADA SUBBOM NYA.

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE SubBomId <> ''
			AND [Level] = 5

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId)
		BEGIN		
			--SET @Level = @Level+1

			INSERT INTO Ps_BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 6, @FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM Ps_BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId, FromDate, ToDate
		FROM Ps_BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 5

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 
					AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE()) )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO Ps_BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 6, @FromDate, @ToDate, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 
						AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE] >= CONVERT(DATE, GETDATE()) )
				
			DELETE FROM Ps_BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId, @FromDate, @ToDate
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02

	
	-- Cek kelengkapan BOM STD
	DECLARE @MessageStatusId int
	DECLARE @MessageStatusName NVARCHAR(MAX)

	IF EXISTS(
		-- 1. Cek ItemId PsPlan terhadap Ps_BomStd

		SELECT a.Room, a.ItemId as ProducId, i.SEARCHNAME as ProductName
			,a.[PlanDate]
			,ISNULL(b.BomId,'') as BomId
			,ISNULL(b.ItemId,'') as ItemId
			,ISNULL(b.ItemName,'') as ItemName
		FROM PsPlan a
		LEFT JOIN Ps_BomStd b ON b.ProductId = a.ItemId
			AND a.[PlanDate] BETWEEN b.FromDate AND b.ToDate
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
		WHERE a.Room = @Room
			AND b.BomId is null
		UNION 
		--2. Cek Ps_BomStd belum breakdown
		SELECT a.[Room]
			,a.[ProductId], i.SEARCHNAME as ProductName
			,b.[PlanDate]
			,a.[BomId]
			,a.[ItemId]
			,a.[ItemName]
		FROM [Ps_BomStd] a
		JOIN PsPlan b ON b.ItemId= a.ProductId 
			AND b.[PlanDate] BETWEEN a.FromDate AND a.ToDate
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.[ProductId]
		WHERE a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '2%'
			AND a.Room = @Room
	)
	BEGIN
		--SET @MessageStatusId = 0
		--SET @MessageStatusName = 'Bom Std belum lengkap, silahkan cek BOM.' 
		--SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
		RETURN
	END
	
	--SET @MessageStatusId = 1
	--SET @MessageStatusName = 'Proses Bom Std Insert sukses.' 
	--SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
END

GO

