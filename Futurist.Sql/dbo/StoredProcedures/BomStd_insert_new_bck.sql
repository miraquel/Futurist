-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Bom Standart Insert by Rofo
-- EXEC [BomStd_insert] 5
-- Durasi : 2 menit, after index 3 detik
-- =============================================
CREATE PROCEDURE [dbo].[BomStd_insert_new_bck]
	@Room int = 99
	,@CreatedBy nvarchar(20) = 'Unknown'
AS
BEGIN
	--SET NOCOUNT ON;		
	DECLARE @Level int

	DELETE FROM BomStd WHERE Room = @Room
	
----**PROSES LEVEL PERTAMA (1)
	SET @Level = 1

	INSERT INTO BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName],[BomQty],[BomQtySerie]
      ,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)
	SELECT @Room,[BOMID],[PRODUCTID],[ITEMID],[ITEMNAME],[BOMQTY],[BOMQTYSERIE]
		,[LINETYPE],[SUBBOMID],[BomId],@Level,@CreatedBy,GETDATE()
	  FROM AXGMKDW.dbo.[FactBom]
	  WHERE ACTIVE = 1
		AND FROMQTY = 1
		AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
		AND PRODUCTID in (SELECT DISTINCT ItemId FROM Rofo WHERE  Room = @Room)	
		ORDER BY [PRODUCTID],  [ITEMID]		
		 
		 
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

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE SubBomId <> ''
			AND [Level] = 1

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId )
		BEGIN		
			--SET @Level = @Level+1

			INSERT INTO BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 2, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 1

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 2, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				
			DELETE FROM BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02


	----**PROSES LEVEL KETIGA DST (3)
    --**BREAKDOWN UTK DATA YG ADA SUBBOM NYA.

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE SubBomId <> ''
			AND [Level] = 2

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId)
		BEGIN		
			--SET @Level = @Level+1

			INSERT INTO BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 3, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 2

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 3, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				
			DELETE FROM BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02



	

	----**PROSES LEVEL KEEMPAT DST (4)
    --**BREAKDOWN UTK DATA YG ADA SUBBOM NYA.

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE SubBomId <> ''
			AND [Level] = 3

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId)
		BEGIN		
			--SET @Level = @Level+1

			INSERT INTO BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 4, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 3

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 4, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				
			DELETE FROM BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02


	----**PROSES LEVEL KELIMA DST (5)
    --**BREAKDOWN UTK DATA YG ADA SUBBOM NYA.

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE SubBomId <> ''
			AND [Level] = 4

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId)
		BEGIN		
			--SET @Level = @Level+1

			INSERT INTO BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 5, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 4

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 5, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				
			DELETE FROM BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02




	

	----**PROSES LEVEL KEENAM DST (6)
    --**BREAKDOWN UTK DATA YG ADA SUBBOM NYA.

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE SubBomId <> ''
			AND [Level] = 5

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		IF EXISTS ( SELECT 1
			FROM AXGMKDW.dbo.[FactBom]
			WHERE [BOMID] = @SubBomId
				AND ProductId = @ItemId)
		BEGIN		
			--SET @Level = @Level+1

			INSERT INTO BomStd ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)
			
				SELECT @Room,@BomId,@ProductId,[ITEMID],[ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 6, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [BOMID] = @SubBomId
						AND ProductId = @ItemId
					ORDER BY [ITEMID]	

			DELETE FROM BomStd WHERE RecId = @RecId	
		END	
 
		FETCH NEXT FROM Cur01 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01


	--**BREAKDOWN UTK DATA YG TIDAK ADA SUBBOMNYA
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT RecId, BomId, ProductId, ItemId, ItemName, BomQty, BomQtySerie, LineType, SubBomId
		FROM BomStd
		WHERE LEFT(ItemId,1) = '6' OR LEFT(ItemId,1) = '2'
			AND [Level] = 5

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		IF EXISTS ( SELECT 1
				FROM AXGMKDW.dbo.[FactBom]
				WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				)
		BEGIN
			--SET @Level = @Level+1

			INSERT INTO BomStd  ([Room],[BomId],[ProductId],[ItemId],[ItemName]
				,[BomQty],[BomQtySerie]
				,[LineType],[SubBomId],[Ref],[Level],CreatedBy,CreatedDate)

				SELECT @Room, @BomId, @ProductId, [ITEMID], [ITEMNAME]
					,[BOMQTY]/[BOMQTYSERIE] * @BomQty/@BomQtySerie, @BomQtySerie
					,[LINETYPE], [SUBBOMID], @SubBomId, 6, @CreatedBy,GETDATE()
					FROM AXGMKDW.dbo.[FactBom]
					WHERE [PRODUCTID] = @ItemId AND [Active] = 1 AND FROMQTY = 1 AND FROMDATE <= GETDATE() AND ( [TODATE] = '1900-01-01' OR [TODATE]>GETDATE() )
				
			DELETE FROM BomStd WHERE RecId = @RecId
		END
		
		FETCH NEXT FROM Cur02 INTO @RecId, @BomId, @ProductId, @ItemId, @ItemName, @BomQty, @BomQtySerie, @LineType, @SubBomId
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02

	
	-- Cek kelengkapan BOM STD
	DECLARE @MessageStatusId int
	DECLARE @MessageStatusName NVARCHAR(MAX)

	IF EXISTS(
		-- 1. Cek ItemId ROFO terhadap BomStd
		SELECT a.Room, a.ItemId as ProducId, i.SEARCHNAME as ProductName
			,ISNULL(b.BomId,'') as BomId
			,ISNULL(b.ItemId,'') as ItemId
			,ISNULL(b.ItemName,'') as ItemName
		FROM Rofo a
		LEFT JOIN BomStd b ON b.ProductId = a.ItemId
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
		WHERE a.Room = @Room
			AND b.BomId is null
		UNION 
		--2. Cek BomStd belum breakdown
		SELECT a.[Room]
			,a.[ProductId], i.SEARCHNAME as ProductName
			,a.[BomId]
			,a.[ItemId]
			,a.[ItemName]
		FROM [BomStd] a
			JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.[ProductId]
		WHERE a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '2%'
			AND a.Room = @Room
	)
	BEGIN
		SET @MessageStatusId = 0
		SET @MessageStatusName = 'Bom Std belum lengkap, silahkan cek BOM.' 
		SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
		RETURN
	END
	
	SET @MessageStatusId = 1
	SET @MessageStatusName = 'Proses Bom Std Insert sukses.' 
	SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
END

GO

