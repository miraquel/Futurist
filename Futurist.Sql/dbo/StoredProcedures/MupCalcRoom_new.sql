-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Mup Calculation
-- EXEC [MupCalcRoom] 99
-- Durasi: 1 jam 25 menit, after index 2 menit 49 detik
-- =============================================
CREATE PROCEDURE [dbo].[MupCalcRoom_new]
@Room int = 5
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @MessageStatusId int
	DECLARE @MessageStatusName NVARCHAR(MAX)

	IF EXISTS(
		-- 1. Cek ItemId ROFO terhadap BomStd
		SELECT a.Room, a.ItemId as ProducId, i.SEARCHNAME as ProductName
			,ISNULL(b.BomId,'') as BomId
			,ISNULL(b.ItemId,'') as ItemId
			,ISNULL(b.ItemName,'') as ItemName
		FROM Rofo a with(nolock)
		LEFT JOIN BomStd b with(nolock) ON b.ProductId = a.ItemId
			AND b.FromDate <= a.RofoDate AND (b.ToDate >= a.RofoDate OR b.ToDate = '1900-01-01')
		JOIN AXGMKDW.dbo.DimItem i with(nolock) ON i.ITEMID = a.ItemId
		WHERE a.Room = @Room
			AND b.BomId is null
		UNION 
		--2. Cek BomStd belum breakdown
		SELECT a.[Room]
			,a.[ProductId], i.SEARCHNAME as ProductName
			,a.[BomId]
			,a.[ItemId]
			,a.[ItemName]
		FROM [BomStd] a with(nolock) 
			JOIN AXGMKDW.dbo.DimItem i with(nolock) ON i.ITEMID = a.[ProductId]
		WHERE a.ItemId LIKE '4%' OR a.ItemId LIKE '6%' OR a.ItemId LIKE '2%'
			AND a.Room = @Room
	)
	BEGIN
		SET @MessageStatusId = 0
		SET @MessageStatusName = 'Bom Std belum lengkap, silahkan cek BOM.' 
		SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
		RETURN
	END
	
	--Samakan Qty Remainder dengan Qty aktualnya
	UPDATE [Rofo] SET QtyRem = Qty WHERE Room = @Room  

	--Rofo 
	DELETE FROM [RofoRoom] WHERE Room = @Room
	INSERT INTO [RofoRoom] ([Room],[RofoDate],[ItemId],[ItemName],[Qty],[QtyRem],[CreatedBy],[CreatedDate],[RecId])
		SELECT [Room],[RofoDate],[ItemId],[ItemName],[Qty],[QtyRem],[CreatedBy],[CreatedDate],[RecId]
			FROM [Rofo] with(nolock) 
			WHERE Room = @Room
	
	--BomStd 
	DELETE FROM [BomStdRoom] WHERE Room = @Room
	INSERT INTO [BomStdRoom] ([Room],[BomId],[ProductId],[ItemId],[ItemName],[BomQty],[BomQtySerie]
		,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],[CreatedBy],[CreatedDate],[RecId])
		SELECT [Room],[BomId],[ProductId],[ItemId],[ItemName],[BomQty],[BomQtySerie]
			,[LineType],[SubBomId],[Ref],[Level],[FromDate],[ToDate],[CreatedBy],[CreatedDate],[RecId]
			FROM [BomStd] with(nolock) 
			WHERE Room = @Room

	--ItemStdCost 
	DELETE FROM [ItemStdcostRoom] WHERE Room = @Room
	INSERT INTO [ItemStdcostRoom] ([RecId],[ItemId],[Price],[Room])
	SELECT [RecId],[ItemId],[Price],@Room
		FROM [ItemStdcost] with(nolock) 
	
	DELETE FROM [ItemOnhandRoom] WHERE Room = @Room
	INSERT INTO [ItemOnhandRoom] ([RecId],[ItemId],[InventBatch],[ExpDate],[PdsDispositionCode],[Qty],[QtyRem],[Price],[RmPrice],[PmPrice],[StdcostPrice],[Room])
	SELECT [RecId],[ItemId],[InventBatch],[ExpDate],[PdsDispositionCode],[Qty],[Qty],[Price],[RmPrice],[PmPrice],[StdcostPrice],@Room
		FROM [ItemOnhand] with(nolock) 

	DELETE FROM [ItemPoIntransitRoom] WHERE Room = @Room
	INSERT INTO [ItemPoIntransitRoom] ([RecId],[ItemId],[Po],[DeliveryDate],[Qty],[QtyRem],[Unit],[CurrencyCode],[Price],[Room])
	SELECT [RecId],[ItemId],[Po],[DeliveryDate],[Qty],[Qty],[Unit],[CurrencyCode],[Price],@Room
		FROM [ItemPoIntransit] with(nolock) 

	DELETE FROM [ItemPagRoom] WHERE [Room] = @Room
	INSERT INTO [ItemPagRoom] ([RecId],[ItemId],[Pag],[VendorId],[EffectiveDate],[ExpirationDate],[Qty],[QtyRem],[Unit],[CurrencyCode],[Price],[Room])
		SELECT [RecId],[ItemId],[Pag],[VendorId],[EffectiveDate],[ExpirationDate],[Qty],[QtyRem],[Unit],[CurrencyCode],[Price],@Room
		FROM [ItemPag] with(nolock) 

	DELETE FROM [ItemForecastRoom] WHERE [Room] = @Room
	INSERT INTO [ItemForecastRoom] ([RecId],[ItemId],[Unit],[ForecastDate],[ForecastPrice],[ForcedPrice],[Room],[LatestPurchaseDate])
		SELECT [RecId],[ItemId],[Unit],[ForecastDate],[ForecastPrice],[ForcedPrice],@Room,[LatestPurchaseDate]
		FROM [ItemForecast] with(nolock) 

	DELETE FROM [ExchangeRateRoom] WHERE [Room] = @Room
	INSERT INTO [ExchangeRateRoom] ([RecId],[CurrencyCode],[ValidFrom],[ValidTo],[ExchangeRate],[CreatedDate],[CreatedBy],[Room])
		SELECT [RecId],[CurrencyCode],[ValidFrom],[ValidTo],[ExchangeRate],[CreatedDate],[CreatedBy],@Room
		FROM [ExchangeRate] with(nolock) 
		
	DELETE FROM [ItemAdj] WHERE Room = @Room
	
	
	--Hapus dulu semua table yang akan digenerate
	DELETE FROM [MupTrans] WHERE Room = @Room
	DELETE FROM [ItemTrans] WHERE Room = @Room
	DELETE FROM [Mup] WHERE Room = @Room
	
	--Variable untuk menghandle table [Rofo]
	DECLARE @RofoId INT
		,@RofoDate DATETIME
        ,@ItemId NVARCHAR(20)
		,@QtyRofo NUMERIC(32,16)
		,@QtyRemRofo NUMERIC(32,16)
		
	--Variable untuk menghandle table [ItemStdcost]
	DECLARE @StdCostId INT
		,@Price NUMERIC(32,16)
	
	--Variable untuk menghandle table [ItemOnhandRoom]
	DECLARE @OnHandId INT
		,@OnHandInventBatch NVARCHAR(20)
		,@QtyOnHand NUMERIC(32,16)
		,@QtyRemOnHand NUMERIC(32,16)
		,@RmPrice NUMERIC(32,16)
		,@PmPrice NUMERIC(32,16)
		,@StdCostPrice NUMERIC(32,16)
		,@ItemIdSubstitute NVARCHAR(20)

	--Variable untuk menghandle table [Mup]
	DECLARE @MupDate datetime
		,@ItemGroup NVARCHAR(20)
		,@QtyMup NUMERIC(32,16)
		,@QtyRemMup NUMERIC(32,16)
		,@MupId INT

	--Variable untuk menghandle table [ItemTrans]
	DECLARE @ItemTransId INT

	--Variable untuk menghandle table [ItemPoIntransitRoom]
	DECLARE @PoIntransitId int
		,@QtyPoIntransit NUMERIC(32,16)
		,@QtyRemPoIntransit NUMERIC(32,16)

	--Variable untuk menghandle table [ItemPagRoom]
	DECLARE @PagId INT
		, @PagNo NVARCHAR(20)
		, @EffectiveDate datetime
		, @ExpirationDate datetime
		, @QtyPag NUMERIC(32,16)
		, @QtyRemPag NUMERIC(32,16)
		, @CurrencyCode NVARCHAR(20)
		, @PriceOri NUMERIC(32,16)
		, @ExchangeRate NUMERIC(32,16)

	--Variable untuk menghandle table [ItemForecast]
	DECLARE @ForecastId INT
		, @ForecastDate datetime
		, @ForecastPrice NUMERIC(32,16)
		, @ForcedPrice NUMERIC(32,16)

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT RecId, RofoDate, ItemId, Qty, QtyRem
		FROM [RofoRoom] with(nolock) 
		WHERE Room = @Room
		--ORDER BY RofoDate, ItemId
		ORDER BY RofoDate ASC, Qty DESC

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @RofoId, @RofoDate, @ItemId, @QtyRofo, @QtyRemRofo
 
    WHILE @@FETCH_STATUS = 0
    BEGIN

		--1. Cari Onhand FG yang bisa digunakan
		SET @OnHandId = 0
		SET @QtyOnHand = 0 
		SET @QtyRemOnHand = 0
		SET @OnHandInventBatch = ''
		SET @Price = 0
		SET @RmPrice = 0
		SET @PmPrice = 0
		SET @StdCostPrice = 0
		DECLARE Cur01A CURSOR READ_ONLY
		FOR
		SELECT RecId,Qty,QtyRem,InventBatch,Price,RmPrice,PmPrice,StdCostPrice
		FROM [ItemOnhandRoom] with(nolock) 
		WHERE ItemId = @ItemId
			AND ExpDate > @RofoDate
			AND QtyRem > 0
			AND (RmPrice > 0 or PmPrice > 0 or StdCostPrice > 0)
			AND Room = @Room
		ORDER BY ExpDate ASC

		OPEN Cur01A
		FETCH NEXT FROM Cur01A INTO @OnHandId,@QtyOnHand,@QtyRemOnHand,@OnHandInventBatch,@Price,@RmPrice,@PmPrice,@StdCostPrice
 
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @QtyRemOnHand >= @QtyRemRofo --Onhand FG cukup utk fulfillment rofo
			BEGIN						
				INSERT [Mup] ([Room],[MupDate],[ItemId],[ItemGroup],[Qty],[QtyRem],[RofoId])
					VALUES (@Room, @RofoDate, @ItemId, @ItemId,@QtyRemRofo,@QtyRemRofo,@RofoId)
				SET @MupId = SCOPE_IDENTITY() 

				UPDATE [RofoRoom]
				SET QtyRem = QtyRem-@QtyRemRofo
				WHERE RecId = @RofoId

				UPDATE [ItemOnhandRoom] 
					SET QtyRem = QtyRem-@QtyRemRofo
					WHERE RecId = @OnHandId AND Room = @Room

				INSERT INTO [ItemTrans] ([Room], [ItemId],[InventBatch],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
					VALUES (@Room, @ItemId,@OnHandInventBatch, @QtyRemRofo, @Price, @RmPrice, @PmPrice, @StdCostPrice, 'OnHand', @OnHandId)
				SET @ItemTransId = SCOPE_IDENTITY() 

				INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
				VALUES (@Room, @MupId, @ItemTransId)	

				UPDATE [Mup]
					SET [QtyRem] = [QtyRem]-@QtyRemRofo
					WHERE RecId = @MupId

				SET @QtyRemRofo = @QtyRemRofo-@QtyRemRofo
			END
			ELSE
			BEGIN --onhand hanya dapat fulfillment sebagian dari rofo
				IF @QtyRemOnHand > 0
				BEGIN
					UPDATE [RofoRoom]
					SET QtyRem = QtyRem-@QtyRemOnHand
					WHERE RecId = @RofoId

					INSERT [Mup] ([Room],[MupDate],[ItemId],[ItemGroup],[Qty],[QtyRem],[RofoId])
						VALUES (@Room, @RofoDate, @ItemId, @ItemId,@QtyRemOnHand,@QtyRemOnhand,@RofoId) 
					SET @MupId = SCOPE_IDENTITY() 
				
					UPDATE [ItemOnhandRoom] 
					SET QtyRem = QtyRem-@QtyRemOnHand
					WHERE RecId = @OnHandId AND Room = @Room

					INSERT INTO [ItemTrans] ([Room], [ItemId],[InventBatch],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
						VALUES (@Room, @ItemId,@OnHandInventBatch, @QtyRemOnHand, @Price, @RmPrice, @PmPrice, @StdCostPrice, 'OnHand', @OnHandId)
					SET @ItemTransId = SCOPE_IDENTITY() 

					INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
					VALUES (@Room, @MupId, @ItemTransId)	
			
					UPDATE [Mup]
						SET [QtyRem] = [QtyRem]-@QtyRemOnhand
						WHERE RecId = @MupId

					SET @QtyRemRofo = @QtyRemRofo-@QtyRemOnHand
				END
			END

			IF @QtyRemRofo = 0
				BREAK

			FETCH NEXT FROM Cur01A INTO @OnHandId,@QtyOnHand,@QtyRemOnHand,@OnHandInventBatch,@Price,@RmPrice,@PmPrice,@StdCostPrice
		END
 
		CLOSE Cur01A
		DEALLOCATE Cur01A

	FETCH NEXT FROM Cur01 INTO @RofoId, @RofoDate, @ItemId, @QtyRofo, @QtyRemRofo
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01

		
	--2. Generate MUP terhadap qty Rofo sisa setelah dipotong onhand FG
	INSERT [Mup] ([Room],[MupDate],[ItemId],[ItemGroup],[Qty],[QtyRem],[RofoId])
		SELECT @Room
			,CAST( CAST(YEAR(a.[RofoDate]) as nvarchar(10)) + '-' + CAST(MONTH(a.[RofoDate]) as nvarchar(10)) + '-' + '1' as datetime) as [RofoDate]
      		,b.ItemId 
			,b.ItemId 
			,b.BOMQTY / b.BOMQTYSERIE * a.[QtyRem] as Qty
			,b.BOMQTY / b.BOMQTYSERIE * a.[QtyRem] as QtyRem
			,a.RecId
		FROM [RofoRoom] a
			JOIN BomStdRoom b ON b.ProductId = a.ITEMID AND b.Room = a.Room
				AND b.FromDate <= a.RofoDate AND (b.ToDate >= a.RofoDate OR b.ToDate = '1900-01-01')
		WHERE a.Room = @Room AND a.[QtyRem] > 0
		ORDER BY a.[RofoDate] ASC, a.QTY DESC

	UPDATE [RofoRoom]
		SET QtyRem = 0 -- karena semua Rofo sudah diturunkan menjadi MUP RMPM
		WHERE Room = @Room


	--3. Generate MUP fulfillment
	DECLARE Cur02 CURSOR READ_ONLY
	FOR
	SELECT MupDate, ItemId, ItemGroup, Qty,QtyRem, RofoId, RecId
		FROM Mup with(nolock) 
		WHERE Room = @Room AND QtyRem > 0 
		ORDER BY RecId

    OPEN Cur02
    FETCH NEXT FROM Cur02 INTO @MupDate,@ItemId,@ItemGroup,@QtyMup,@QtyRemMup,@RofoId,@MupId
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		--0. jika content ID 9 maka cari di ItemStdcost
		IF LEFT(@ItemId,1) = '9' 
		BEGIN
			SET @StdCostId = 0
			SET @Price = 0

			SELECT @StdCostId = RecId
				, @Price = Price
			FROM [ItemStdcostRoom] with(nolock) 
			WHERE ItemId = @ItemId
				AND Room = @Room

			INSERT INTO [ItemTrans] ([Room], [ItemId],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
				VALUES (@Room, @ItemId, @QtyRemMup, @Price, 0, 0, @Price, 'StdCost', @StdCostId)
			SET @ItemTransId = SCOPE_IDENTITY() 

			INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
			VALUES (@Room, @MupId, @ItemTransId)
						
			UPDATE [Mup]
				SET [QtyRem] = [QtyRem]-[QtyRem]
				WHERE RecId = @MupId
		END
		ELSE
		BEGIN -- Selain ID 9 baca ke Onhand, PoIntransit, ContractPAG, Forecast
			--1. Cari dari ONHAND
			--setting default value
			SET @OnHandId = 0
			SET @QtyOnHand = 0
			SET @QtyRemOnHand = 0
			SET @OnHandInventBatch = 0
			SET @Price = 0
			SET @RmPrice = 0
			SET @PmPrice = 0
			SET @StdCostPrice = 0
			SET @ItemIdSubstitute = ''

			DECLARE Cur02A CURSOR READ_ONLY
			FOR
			SELECT RecId, Qty, QtyRem, InventBatch, Price, RmPrice, PmPrice, StdCostPrice, ItemId
			FROM [ItemOnhandRoom] with(nolock) 
			WHERE ExpDate > @MupDate
				AND QtyRem > 0
				AND ItemId IN(			--Baca juga ItemSubstitusi
					SELECT @ItemId
					UNION
					SELECT b.ItemId as ItemIdSubstitute
					FROM AXGMKDW.dbo.[DimItemSubstitute] a
					JOIN AXGMKDW.dbo.[DimItemSubstitute] b on b.VtaMpSubstitusiGroupId = a.VtaMpSubstitusiGroupId
					WHERE a.ItemId = @ItemId
				)
				AND Room = @Room
			ORDER BY ExpDate ASC

			OPEN Cur02A
			FETCH NEXT FROM Cur02A INTO @OnHandId,@QtyOnHand,@QtyRemOnHand,@OnHandInventBatch,@Price,@RmPrice,@PmPrice,@StdCostPrice,@ItemIdSubstitute
 
			WHILE @@FETCH_STATUS = 0
			BEGIN
				IF @QtyRemOnHand >= @QtyRemMup --Onhand cukup utk fulfillment MUP
				BEGIN					
					UPDATE [ItemOnhandRoom] 
					SET QtyRem = QtyRem-@QtyRemMup
					WHERE RecId = @OnHandId AND Room = @Room

					INSERT INTO [ItemTrans] ([Room], [ItemId],[InventBatch],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
						VALUES (@Room, @ItemIdSubstitute, @OnHandInventBatch, @QtyRemMup, @Price, @RmPrice, @PmPrice, @StdCostPrice, 'OnHand', @OnHandId)
					SET @ItemTransId = SCOPE_IDENTITY() 

					INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
					VALUES (@Room, @MupId, @ItemTransId)
						
					UPDATE [Mup]
						SET [QtyRem] = [QtyRem]-@QtyRemMup
						WHERE RecId = @MupId

					SET @QtyRemMup = @QtyRemMup - @QtyRemMup 
				END
				ELSE
				BEGIN --Onhand tidak cukup, hanya bisa fulfillment sebagian MUP saja
					IF @QtyRemOnHand > 0
					BEGIN									
						UPDATE [ItemOnhandRoom] 
						SET QtyRem = QtyRem-@QtyRemOnHand
						WHERE RecId = @OnHandId AND Room = @Room

						INSERT INTO [ItemTrans] ([Room], [ItemId],[InventBatch],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
							VALUES (@Room, @ItemIdSubstitute,@OnHandInventBatch, @QtyRemOnHand, @Price, @RmPrice, @PmPrice, @StdCostPrice, 'OnHand', @OnHandId)
						SET @ItemTransId = SCOPE_IDENTITY() 

						INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
						VALUES (@Room, @MupId, @ItemTransId)	
			
						UPDATE [Mup]
							SET [QtyRem] = [QtyRem]-@QtyRemOnHand
							WHERE RecId = @MupId

						SET @QtyRemMup = @QtyRemMup - @QtyRemOnHand
					END
				END

				IF @QtyRemMup = 0
					BREAK

			FETCH NEXT FROM Cur02A INTO @OnHandId,@QtyOnHand,@QtyRemOnHand,@OnHandInventBatch,@Price,@RmPrice,@PmPrice,@StdCostPrice,@ItemIdSubstitute
			END
 
			CLOSE Cur02A
			DEALLOCATE Cur02A
			
			--***** 2. Cari dari PO Intransit *****
			IF @QtyRemMup > 0 --jika masih ada Remainder maka lanjut ke pencarian PO Intransit
			BEGIN
				SET @PoIntransitId = 0
				SET @QtyPoIntransit = 0 
				SET @QtyRemPoIntransit = 0
				SET @Price = 0
				SET @CurrencyCode = ''
				SET @PriceOri = 0
				SET @ExchangeRate = 0

				DECLARE Cur02A CURSOR READ_ONLY
				FOR
				SELECT a.RecId, a.Qty, a.QtyRem, a.Price, a.ItemId, a.CurrencyCode, b.ExchangeRate
					FROM [ItemPoIntransitRoom] a with(nolock) 
					LEFT JOIN [ExchangeRateRoom] b with(nolock) ON b.CurrencyCode = a.CurrencyCode AND @MupDate BETWEEN b.ValidFrom AND b.ValidTo AND b.Room = a.Room
					WHERE QtyRem > 0
						AND ItemId = @ItemId
						--AND ItemId IN(			
						--	--Baca juga ItemSubstitusi
						--	SELECT @ItemId
						--	UNION
						--	SELECT b.ItemId as ItemIdSubstitute
						--	FROM AXGMKDW.dbo.[DimItemSubstitute] a
						--	JOIN AXGMKDW.dbo.[DimItemSubstitute] b on b.VtaMpSubstitusiGroupId = a.VtaMpSubstitusiGroupId
						--	WHERE a.ItemId = @ItemId
						--)
						AND a.[DeliveryDate] < DATEADD(MONTH,1,@MupDate)	--agar bisa mengambil deliverydate dibulan yg sama
						AND a.Room = @Room
					ORDER BY [DeliveryDate] ASC

				OPEN Cur02A
				FETCH NEXT FROM Cur02A INTO @PoIntransitId, @QtyPoIntransit, @QtyRemPoIntransit, @Price, @ItemIdSubstitute, @CurrencyCode, @ExchangeRate
 
				WHILE @@FETCH_STATUS = 0
				BEGIN
					IF @QtyRemPoIntransit >= @QtyRemMup --PO Intransit cukup utk fullfill MUP
					BEGIN					
						UPDATE [ItemPoIntransitRoom] 
						SET QtyRem = QtyRem-@QtyRemMup
						WHERE RecId = @PoIntransitId AND Room = @Room

						INSERT INTO [ItemTrans] ([Room],[ItemId],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
							--VALUES (@Room,@ItemIdSubstitute,@QtyRemMup,@Price,0,0,0,'PoIntransit',@PoIntransitId)
							VALUES (@Room,@ItemIdSubstitute,@QtyRemMup,@Price*@ExchangeRate,0,0,0,'PoIntransit',@PoIntransitId)

						SET @ItemTransId = SCOPE_IDENTITY() 

						INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
						VALUES (@Room, @MupId, @ItemTransId)	

						UPDATE [Mup]
							SET [QtyRem] = [QtyRem]-@QtyRemMup
							WHERE RecId = @MupId

						SET @QtyRemMup = @QtyRemMup - @QtyRemMup
					END
					ELSE
					BEGIN --PO Intransit tidak cukup, hanya bisa fill MUP sebagian saja									
						IF @QtyRemPoIntransit > 0
						BEGIN	
							UPDATE [ItemPoIntransitRoom] 
							SET QtyRem = QtyRem-@QtyRemPoIntransit
							WHERE RecId = @PoIntransitId AND Room = @Room

							INSERT INTO [ItemTrans] ([Room],[ItemId],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
								--VALUES (@Room,@ItemIdSubstitute,@QtyRemPoIntransit,@Price,0,0,0,'PoIntransit', @PoIntransitId)
								VALUES (@Room,@ItemIdSubstitute,@QtyRemPoIntransit,@Price*@ExchangeRate,0,0,0,'PoIntransit', @PoIntransitId)
							SET @ItemTransId = SCOPE_IDENTITY() 

							INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
							VALUES (@Room, @MupId, @ItemTransId)	
			
							UPDATE [Mup]
								SET [QtyRem] = [QtyRem]-@QtyRemPoIntransit
								WHERE RecId = @MupId

							SET @QtyRemMup = @QtyRemMup - @QtyRemPoIntransit				
						END
					END

					IF @QtyRemMup = 0
						BREAK
					
				FETCH NEXT FROM Cur02A INTO @PoIntransitId, @QtyPoIntransit, @QtyRemPoIntransit, @Price, @ItemIdSubstitute, @CurrencyCode, @ExchangeRate
				END
 
				CLOSE Cur02A
				DEALLOCATE Cur02A

				--***** 3. Cari dari PAG *****
				IF @QtyRemMup > 0 --jika masih ada Remainder maka lanjut ke pencarian PAG
				BEGIN
					SET @PagId = 0
					SET @PagNo = ''
					SET @EffectiveDate = '1 Jan 1900'
					SET @ExpirationDate = '1 Jan 1900'
					SET @QtyPag = 0 
					SET @QtyRemPag = 0
					SET @CurrencyCode = ''
					SET @PriceOri = 0
					SET @ExchangeRate = 0
					SET @Price = 0
										
					DECLARE Cur02A CURSOR READ_ONLY
					FOR
					SELECT a.RecId, a.Pag, a.EffectiveDate, a.ExpirationDate, a.Qty, a.QtyRem, a.CurrencyCode, a.Price, b.ExchangeRate, a.Price * b.ExchangeRate, a.ItemId
						FROM [ItemPagRoom] a with(nolock) 
						LEFT JOIN [ExchangeRateRoom] b with(nolock) ON b.CurrencyCode = a.CurrencyCode AND @MupDate BETWEEN b.ValidFrom AND b.ValidTo AND b.Room = @Room
						WHERE QtyRem > 0
							AND a.ItemId IN(			
								--Baca juga ItemSubstitusi
								SELECT @ItemId
								UNION
								SELECT b.ItemId as ItemIdSubstitute
								FROM AXGMKDW.dbo.[DimItemSubstitute] a
								JOIN AXGMKDW.dbo.[DimItemSubstitute] b on b.VtaMpSubstitusiGroupId = a.VtaMpSubstitusiGroupId
								WHERE a.ItemId = @ItemId
							)
							AND [EffectiveDate] <= @MupDate
							AND [ExpirationDate] >= @MupDate
							AND a.Room = @Room
						ORDER BY EffectiveDate ASC

					OPEN Cur02A
					FETCH NEXT FROM Cur02A INTO @PagId,@PagNo,@EffectiveDate,@ExpirationDate,@QtyPag,@QtyRemPag,@CurrencyCode,@PriceOri,@ExchangeRate,@Price,@ItemIdSubstitute
 
					WHILE @@FETCH_STATUS = 0
					BEGIN

						IF @QtyRemPag >= @QtyRemMup --PAG cukup utk fulfillment MUP
						BEGIN					
							UPDATE [ItemPagRoom] 
							SET QtyRem = QtyRem-@QtyRemMup
							WHERE RecId = @PagId AND Room = @Room

							INSERT INTO [ItemTrans] ([Room],[ItemId],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId],[CurrencyCode],[CurrencyRate])
								VALUES (@Room,@ItemIdSubstitute,@QtyRemMup,@Price,0,0,0,'Contract',@PagId,@CurrencyCode,isnull(@ExchangeRate,0))
							SET @ItemTransId = SCOPE_IDENTITY() 

							INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
							VALUES (@Room, @MupId, @ItemTransId)	

							UPDATE [Mup]
								SET [QtyRem] = [QtyRem]-@QtyRemMup
								WHERE RecId = @MupId

							SET @QtyRemMup = @QtyRemMup - @QtyRemMup	
						END
						ELSE
						BEGIN --PAG tidak cukup, hanya bisa fulfillment sebagian MUP saja									
							IF @QtyRemPag > 0
							BEGIN
								UPDATE [ItemPagRoom] 
								SET QtyRem = QtyRem-@QtyRemPag
								WHERE RecId = @PagId AND Room = @Room

								INSERT INTO [ItemTrans] ([Room],[ItemId],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId],[CurrencyCode],[CurrencyRate])
									VALUES (@Room,@ItemIdSubstitute,@QtyRemPag,@Price,0,0,0,'Contract', @PagId,@CurrencyCode,isnull(@ExchangeRate,0))
								SET @ItemTransId = SCOPE_IDENTITY() 

								INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
								VALUES (@Room, @MupId, @ItemTransId)	
			
								UPDATE [Mup]
									SET [QtyRem] = [QtyRem]-@QtyRemPag
									WHERE RecId = @MupId

								SET @QtyRemMup = @QtyRemMup - @QtyRemPag
							END
						END

						IF @QtyRemMup = 0
							BREAK
					
					FETCH NEXT FROM Cur02A INTO @PagId,@PagNo,@EffectiveDate,@ExpirationDate,@QtyPag,@QtyRemPag,@CurrencyCode,@PriceOri,@ExchangeRate,@Price,@ItemIdSubstitute
					END
 
					CLOSE Cur02A
					DEALLOCATE Cur02A


					-- ***** 4. Cari dari ItemForecast *****
					IF @QtyRemMup > 0 --jika masih ada Remainder maka lanjut ke pencarian Forecast
					BEGIN
						--Jika ada di table Forecast baca data Forecast
						IF EXISTS (
							SELECT TOP 1 RecId
							FROM [ItemForecastRoom] with(nolock) 
							WHERE ItemId = @ItemId
								AND Room = @Room		
								AND ForecastDate = @MupDate	
								AND ForecastPrice > 0
						)
						BEGIN
							SET @ForecastId = 0
							SET @ForecastDate = '1 Jan 1900'
							SET @ForecastPrice = 0
							SET @ForcedPrice = 0

							SELECT @ForecastId = RecId
								, @ForecastDate = ForecastDate
								--, @ForecastPrice = IIF(LatestPurchaseDate >= DATEADD(MONTH,-4,GETDATE()),ForecastPrice,0)
								, @ForecastPrice = ForecastPrice
								, @ForcedPrice = ForcedPrice
							FROM [ItemForecastRoom] with(nolock) 
							WHERE ItemId = @ItemId
								AND Room = @Room		
								AND ForecastDate = @MupDate	
								AND ForecastPrice > 0

							INSERT INTO [ItemTrans] ([Room],[ItemId],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
								VALUES (@Room,@ItemId,@QtyRemMup
								--,IIF(@ForcedPrice=0,@ForecastPrice,@ForcedPrice)
								,@ForecastPrice
								,0,0,0,'Forecast',@ForecastId)
							SET @ItemTransId = SCOPE_IDENTITY() 

							INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
							VALUES (@Room, @MupId, @ItemTransId)	

							UPDATE [Mup]
								SET [QtyRem] = [QtyRem]-@QtyRemMup
								WHERE RecId = @MupId		
						END
						ELSE IF EXISTS (
							SELECT TOP 1 RecId
							FROM [ItemForecastRoom] with(nolock) 
							WHERE Room = @Room		
								AND ForecastDate = @MupDate	
								AND ItemId IN(			
										--Baca juga ItemSubstitusi
										SELECT @ItemId
										UNION
										SELECT b.ItemId as ItemIdSubstitute
										FROM AXGMKDW.dbo.[DimItemSubstitute] a
										JOIN AXGMKDW.dbo.[DimItemSubstitute] b on b.VtaMpSubstitusiGroupId = a.VtaMpSubstitusiGroupId
										WHERE a.ItemId = @ItemId
									)		
								AND ForecastPrice > 0					
							)
						--mengisi harga tertinggi dari group subtitusi teman2nya
						BEGIN
							SET @ForecastId = 0
							SET @ForecastDate = '1 Jan 1900'
							SET @ForecastPrice = 0
							SET @ForcedPrice = 0

							SELECT TOP 1 @ForecastId = RecId
								, @ForecastDate = ForecastDate
								--, @ForecastPrice = IIF(LatestPurchaseDate >= DATEADD(MONTH,-4,GETDATE()),ForecastPrice,0)
								, @ForecastPrice = ForecastPrice
								, @ForcedPrice = ForcedPrice
								, @ItemIdSubstitute = ItemId
							FROM [ItemForecastRoom] with(nolock) 
							WHERE Room = @Room		
								AND ForecastDate = @MupDate	
								AND ItemId IN(			
										--Baca juga ItemSubstitusi
										SELECT b.ItemId as ItemIdSubstitute
										FROM AXGMKDW.dbo.[DimItemSubstitute] a
										JOIN AXGMKDW.dbo.[DimItemSubstitute] b on b.VtaMpSubstitusiGroupId = a.VtaMpSubstitusiGroupId
										WHERE a.ItemId = @ItemId
									)	
								AND ForecastPrice > 0	
							ORDER BY ForecastPrice DESC

							INSERT INTO [ItemTrans] ([Room],[ItemId],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
								VALUES (@Room,@ItemIdSubstitute,@QtyRemMup
								--,IIF(@ForcedPrice=0,@ForecastPrice,@ForcedPrice)
								,@ForecastPrice
								,0,0,0,'Forecast Group',@ForecastId)
							SET @ItemTransId = SCOPE_IDENTITY() 

							INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
							VALUES (@Room, @MupId, @ItemTransId)	

							UPDATE [Mup]
								SET [QtyRem] = [QtyRem]-@QtyRemMup
								WHERE RecId = @MupId		
						END
						ELSE
						--Jika tidak ada di table Forecast maka akan membaca data ItemAdj
						BEGIN
							INSERT INTO [ItemTrans] ([Room],[ItemId],[Qty],[Price],[RmPrice],[PmPrice],[StdCostPrice],[Source],[RefId])
								VALUES (@Room,@ItemId,@QtyRemMup,0,0,0,0,'NA',0)
							SET @ItemTransId = SCOPE_IDENTITY() 

							INSERT INTO MupTrans([Room],[MupId],[ItemTransId])
							VALUES (@Room, @MupId, @ItemTransId)	

							UPDATE [Mup]
								SET [QtyRem] = [QtyRem]-@QtyRemMup
								WHERE RecId = @MupId	
						END
					END
				END
			END		
		END

	FETCH NEXT FROM Cur02 INTO @MupDate,@ItemId,@ItemGroup,@QtyMup,@QtyRemMup,@RofoId,@MupId
    END
 
    CLOSE Cur02
    DEALLOCATE Cur02

	-- update harga forecast yg masih nol, samakan dengan forecast subtitusinya
	--SELECT a.ItemId, c.MupDate, a.Price, d.VtaMpSubstitusiGroupId, p.ForecastPrice
	--FROM [ItemTrans] a
	--JOIN MupTrans b ON b.ItemTransId = a.RecId
	--JOIN Mup c ON c.RecId = b.MupId
	--JOIN AXGMKDW.dbo.[DimItemSubstitute] d ON d.ItemId = a.ItemId
	--JOIN (
	--	SELECT b.VtaMpSubstitusiGroupId, a.ForecastDate, MAX(a.ForecastPrice) as ForecastPrice
	--	FROM [ItemForecastRoom] a
	--	JOIN AXGMKDW.dbo.[DimItemSubstitute] b ON b.ItemId = a.ItemId
	--	WHERE a.Room = 5 
	--	GROUP BY b.VtaMpSubstitusiGroupId, a.ForecastDate
	--) p ON p.VtaMpSubstitusiGroupId = d.VtaMpSubstitusiGroupId AND p.ForecastDate = c.MupDate 
	--WHERE a.Room = 5
	--	AND a.[Source] = 'Forecast'
	--	AND a.[Price] = 0

	UPDATE [ItemTrans]
	SET Price = p.ForecastPrice
		,[Source] = 'Forecast Group'
	FROM [ItemTrans] a
	JOIN MupTrans b ON b.ItemTransId = a.RecId
	JOIN Mup c ON c.RecId = b.MupId
	JOIN AXGMKDW.dbo.[DimItemSubstitute] d ON d.ItemId = a.ItemId
	JOIN (
		SELECT b.VtaMpSubstitusiGroupId, a.ForecastDate, MAX(a.ForecastPrice) as ForecastPrice
		FROM [ItemForecastRoom] a
		JOIN AXGMKDW.dbo.[DimItemSubstitute] b ON b.ItemId = a.ItemId
		WHERE a.Room = @Room AND ForecastPrice > 0		
		GROUP BY b.VtaMpSubstitusiGroupId, a.ForecastDate
	) p ON p.VtaMpSubstitusiGroupId = d.VtaMpSubstitusiGroupId AND p.ForecastDate = c.MupDate 
	WHERE a.Room = @Room
		AND a.[Source] = 'Forecast'
		AND a.[Price] = 0
		
	-- update komposisi harga sesuai dengan kelompoknya
	UPDATE [ItemTrans] 
		SET RmPrice = Price
		WHERE Room = @Room AND LEFT(ItemId,1) = '1'

	UPDATE [ItemTrans] 
		SET PmPrice = Price
		WHERE Room = @Room AND LEFT(ItemId,1) = '3'

	UPDATE [ItemTrans] 
		SET StdCostPrice = Price
		WHERE Room = @Room AND LEFT(ItemId,1) = '9'

	UPDATE [MUP]
		SET ItemGroup = b.VtaMpSubstitusiGroupId
		FROM [MUP] a 
		JOIN AXGMKDW.dbo.[DimItemSubstitute]b ON b.ItemId = a.ItemId
		WHERE Room = @Room
	
	DECLARE @MessageStatus nvarchar(max)
		
	SET @MessageStatusId = 1
	SET @MessageStatusName = 'Proses MUP Calculation sukses.' 
	SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
END

GO

