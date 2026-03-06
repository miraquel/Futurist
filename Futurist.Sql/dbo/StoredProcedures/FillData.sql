-- =============================================
-- Author:		<Andi>
-- Create date: <12 Maret 2025>
-- Description:	<Untuk mengisi data pendukung proses RMPM Projection>
-- Duration   : <9 detik>
-- =============================================
CREATE PROCEDURE [dbo].[FillData]
AS
BEGIN
	-- 0. Jalankan perhitungan Regresi 
	EXEC [ForecastingRegresi]

	-- 1. On Hand
	DELETE FROM [ItemOnhand]

	INSERT INTO [ItemOnhand] ([ItemId],[InventBatch],[ExpDate],[PdsDispositionCode],[Qty]
			,[QtyRem],[Price],[RmPrice],[PmPrice],[StdcostPrice])
		SELECT a.ITEMID
			,a.[INVENTBATCHID]
			,b.EXPDATE
			--,DATEDIFF(DAY,GETDATE(),b.EXPDATE) as [ACTBESTDAYS]
			--,c.[BESTDLVDAYS]
			,b.[PDSDISPOSITIONCODE]
			,(sum(a.PostedQty) + sum(a.Received) - sum(a.Deducted) + sum(a.Registered) - sum(a.Picked)) as [ONHAND]
			,(sum(a.PostedQty) + sum(a.Received) - sum(a.Deducted) + sum(a.Registered) - sum(a.Picked))  as [QtyRem]
			--,sum(a.[POSTEDVALUE]) / sum(a.PostedQty) as [Price]
			,ISNULL(SUM(a.[POSTEDVALUE]) / NULLIF(SUM(a.PostedQty), 0), 0) AS [Price]
			,0,0,0
		FROM [AXGMKDW].[dbo].[FactInventSum_Yesterday] a
			JOIN [AXGMKDW].[dbo].FactInventBatch_Yesterday b ON ((a.itemid = b.itemid ) and (a.INVENTBATCHID = b.INVENTBATCHID))
			LEFT JOIN [AXGMKDW].[dbo].[DimBestDlvDays] c ON c.ItemId = a.ItemId
		WHERE a.[PDSNETTABLE] = 1 AND b.EXPDATE >= GETDATE()
			AND b.[PDSDISPOSITIONCODE] IN ('A','C','E')
			AND (c.[BESTDLVDAYS] is null OR (DATEDIFF(DAY,GETDATE(),b.EXPDATE)) > c.[BESTDLVDAYS])
		GROUP BY a.itemid,a.INVENTBATCHID,b.EXPDATE,b.[PDSDISPOSITIONCODE]
		HAVING SUM(a.PostedQty) <> 0
			AND SUM(a.PostedQty + a.Received - a.Deducted + a.Registered - a.Picked) > 0

		--khusus ID Als ditambahkan 5%
		UPDATE [ItemOnhand] 
		SET [Price] = [Price] *1.05
		WHERE ITEMID IN ('1500001','1500002','1500003')		

		--updet harga RM-PM-STDCOST untuk OnHand FG
		UPDATE [ItemOnhand]
		SET Price = b.COSTPRICE
			,RmPrice = b.RmPrice
			,PmPrice = b.PmPrice
			,StdCostPrice = b.StdCostPrice
		FROM [ItemOnhand] a 
		JOIN [AXGMKDW].[dbo].[FactPorRmPm] b ON b.ItemId = a.ItemId AND b.[INVENTBATCHID] = a.[InventBatch]
		WHERE a.ItemId LIKE '4%'
		
		-- jika harga breakdown masih kosong, maka pakai item yg sama pada BN sebelumnya
		DECLARE @ItemId nvarchar(20), @ExpDate datetime,  @RmPrice numeric(32,16), @PmPrice numeric(32,16), @StdCostPrice numeric(32,16)
		DECLARE Cur01 CURSOR READ_ONLY
		FOR
		SELECT ItemId, ExpDate
			FROM [ItemOnhand]
			WHERE LEFT([ItemId],1) = '4' 
			AND(
			RmPrice = 0 and PmPrice = 0 and StdCostPrice = 0 
			)

		OPEN Cur01
		FETCH NEXT FROM Cur01 INTO @ItemId, @ExpDate
 
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @RmPrice = 0
			SET @PmPrice = 0
			SET @StdCostPrice = 0

			SELECT top 1 @RmPrice=a.RmPrice, @PmPrice=a.PmPrice, @StdCostPrice=a.StdCostPrice 
			FROM [AXGMKDW].[dbo].[FactPorRmPm] a
			JOIN [AXGMKDW].[dbo].[FactInventBatch_Yesterday] b ON b.ItemId = a.ItemId AND b.InventBatchId = a.InventBatchId
			WHERE a.ItemId = @ItemId AND b.ExpDate <= @ExpDate
			ORDER BY ExpDate desc

			UPDATE [ItemOnhand] 
				SET RmPrice=@RmPrice
					,PmPrice =@PmPrice
					,StdCostPrice=@StdCostPrice
				WHERE ItemId = @ItemId AND ExpDate=@ExpDate

		FETCH NEXT FROM Cur01 INTO @ItemId, @ExpDate
		END
 
		CLOSE Cur01
		DEALLOCATE Cur01

		--updet harga stdcost dengan selisih dari costprice-rmprice-pmprice
		UPDATE [ItemOnhand]
		SET StdCostPrice = Price - RmPrice - PmPrice
		FROM [ItemOnhand] a 
		WHERE a.ItemId LIKE '4%'
				
		--updet harga RM-PM samakan dengan harga purchase pada inventtrans yg sudah diinvoice
		UPDATE [ItemOnhand]
		SET Price = ISNULL(b.CostValue / NULLIF(b.Qty,0),0)
		FROM [ItemOnhand] a 
		JOIN AXGMKDW.dbo.FactInventtrans b ON b.ItemId = a.ItemId AND b.[INVENTBATCHID] = a.[InventBatch]
			AND REFERENCECATEGORY = 'PURCHASE ORDER' AND COSTVALUE > 0
		WHERE a.ItemId LIKE '1%' OR a.ItemId LIKE '3%' 

		--Mengupdet harga RM-PM samakan dengan harga purchase pada PO, utk inventtrans yg costpricenya nol
		UPDATE [ItemOnhand]
		SET Price = c.INVENTPRICE
		FROM [ItemOnhand] a 
		JOIN AXGMKDW.dbo.FactInventtrans b ON b.ItemId = a.ItemId AND b.[INVENTBATCHID] = a.[InventBatch]
			AND REFERENCECATEGORY = 'PURCHASE ORDER' AND COSTVALUE = 0
		JOIN (
			SELECT a.PURCHID, a.ITEMID, a.PURCHPRICE, a.PURCHUNIT, b.UNITID
				,IIF(a.PURCHUNIT<>b.UNITID, a.PURCHPRICE / (c.Factor * c.Numerator / c.Denominator), a.PURCHPRICE)  as INVENTPRICE
			FROM AXGMKDW.dbo.FactPo a
			JOIN AXGMKDW.dbo.DimItem b ON b.ITEMID = a.ItemId
			LEFT JOIN UnitConversion c ON c.FromUnit = a.PURCHUNIT AND c.ToUnit = b.UnitId
		) c ON c.PURCHID = b.REFERENCEID AND c.ITEMID = b.ITEMID
		WHERE (a.ItemId LIKE '1%' OR a.ItemId LIKE '3%')
			AND Price = 0

		--Mengupdate harga untuk PO yang belum diinvoice disamakan dengan PO nya
		UPDATE [ItemOnhand]
		SET Price = ISNULL(c.[LINEAMOUNT] / NULLIF(c.[QTYORDERED],0),0)
		FROM [ItemOnhand] a
		JOIN AXGMKDW.dbo.FactInventtrans b ON b.ItemId = a.ItemId AND b.[INVENTBATCHID] = a.[InventBatch]
			AND b.REFERENCECATEGORY = 'PURCHASE ORDER' AND b.DATEFINANCIAL = '1900-01-01'
		JOIN [AXGMKDW].[dbo].[FactPo] c ON c.PURCHID = b.REFERENCEID AND c.ItemId = b.ItemId
			AND c.CURRENCYCODE = 'IDR'
		WHERE (a.ItemId LIKE '1%' OR a.ItemId LIKE '3%')

		
	-- 2. PO Intransit
	DELETE FROM [ItemPoIntransit]

	INSERT INTO [ItemPoIntransit] ([ItemId],[Po],[DeliveryDate],[Qty],[QtyRem],[Unit],[CurrencyCode],[Price])
		SELECT [ITEMID],[PURCHID],[DELIVERYDATE],[QTY],[QTY],[PURCHUNIT],[CurrencyCode],[PURCHPRICE]
		FROM [AXGMKDW].[dbo].[FactPoIntransit]

	UPDATE [ItemPoIntransit]
		SET Unit = c.ToUnit
		, Qty = a.qty * (c.Factor * c.Numerator / c.Denominator)
		, QtyRem = a.QtyRem * (c.Factor * c.Numerator / c.Denominator)
		, Price = a.Price / (c.Factor * c.Numerator / c.Denominator)
		FROM [ItemPoIntransit] a
		JOIN AXGMKDW.dbo.DimItem b ON b.ITEMID = a.ItemId
		LEFT JOIN UnitConversion c ON c.FromUnit = a.Unit AND c.ToUnit = b.UnitId
		where a.Unit <> b.UNITID
	
	--khusus ID Als ditambahkan 5%
	UPDATE [ItemPoIntransit] 
		SET [Price] = [Price] *1.05
		WHERE ITEMID IN ('1500001','1500002','1500003')		

	-- 3. PAG
	DECLARE @ItemPag as table (
	[ItemId] nvarchar(20)
	,[Pag] nvarchar(20)
	,[VendorId] nvarchar(20)
	,[EffectiveDate] datetime
	,[ExpirationDate] datetime
	,[Qty] numeric(32,16)
	,[Unit] nvarchar(20)
	,[CurrencyCode] nvarchar(20) 
	,[PRICEUNIT] numeric(32,16)
	,[QTYGR] numeric(32,16)
	,[QTYPOINTRANSIT] numeric(32,16)
	,[CURRENCYORIGINAL] nvarchar(20)
	,[PRICEUNITORIGINAL] numeric(32,16)
	,[PRICEUNITMST] numeric(32,16)
	)

	INSERT INTO @ItemPag ([ItemId],[Pag],[VendorId],[EffectiveDate],[ExpirationDate],[Qty],[Unit],[CurrencyCode],[PRICEUNIT],[QTYGR],[QTYPOINTRANSIT],[CURRENCYORIGINAL],[PRICEUNITORIGINAL])
		SELECT [ITEMID], [NOPAG], [VENDACCOUNT], [EFFECTIVEDATE], [EXPIRATIONDATE],[QTYPAG],[UNIT],[CURRENCY],[PRICEUNIT],[QTYGR],[QTYPOINTRANSIT],[CURRENCYORIGINAL],[PRICEUNITORIGINAL]
		FROM [AXGMKDW].[dbo].[FactPAG]
	
	UPDATE @ItemPag
	SET [Qty] = a.qty * (c.Factor * c.Numerator / c.Denominator)
		,[UNIT] = c.ToUnit
		,[PRICEUNIT] = a.[PRICEUNIT] / (c.Factor * c.Numerator / c.Denominator)
		,[PRICEUNITORIGINAL] = a.[PRICEUNITORIGINAL] / (c.Factor * c.Numerator / c.Denominator)
	FROM @ItemPag a
		JOIN AXGMKDW.dbo.DimItem b ON b.ITEMID = a.ItemId
		JOIN UnitConversion c ON c.FromUnit = a.Unit AND c.ToUnit = b.UnitId
		where a.Unit <> b.UNITID		

	DELETE FROM [ItemPag]

	INSERT INTO [ItemPag] ([ItemId],[Pag],[VendorId],[EffectiveDate],[ExpirationDate],[Qty],[QtyRem],[Unit],[CurrencyCode],[Price])
		SELECT [ItemId],[Pag],[VendorId],[EffectiveDate],[ExpirationDate],([Qty]-[QTYGR]-[QTYPOINTRANSIT]),([Qty]-[QTYGR]-[QTYPOINTRANSIT]),[Unit],IIF([CURRENCYORIGINAL]='',[CurrencyCode],[CURRENCYORIGINAL]),IIF([CURRENCYORIGINAL]='',[PRICEUNIT],[PRICEUNITORIGINAL])
		FROM @ItemPag

	DELETE FROM [ItemPag]
		WHERE [Qty] <=0

	--khusus ID Als ditambahkan 5%
	UPDATE [ItemPag] 
		SET [Price] = [Price] *1.05
		WHERE ITEMID IN ('1500001','1500002','1500003')	

	-- 4. Forecast
	DECLARE @ItemForecast as table (
	[ItemId] nvarchar(20)
	,[Unit] nvarchar(20)
	,[ForecastDate] datetime
	,[ForecastPrice] numeric(32,16)
	,[ForcedPrice] numeric(32,16)
	,[LatestPurchaseDate] datetime
	)

	INSERT INTO @ItemForecast ([ItemId],[Unit],[ForecastDate],[ForecastPrice],[ForcedPrice],[LatestPurchaseDate])
		SELECT [ItemId],[InventUnit],[DateTrans],[PriceUnitInvent],0,[LatestPurchaseDate]
		FROM [PurchaseForecastPrice]
		WHERE [DateTrans] BETWEEN '1 Jan 2025' AND '31 Dec 2025'

	
	UPDATE @ItemForecast
	SET [Unit] = c.ToUnit
		,[ForecastPrice] = a.[ForecastPrice] / (c.Factor * c.Numerator / c.Denominator)
	FROM @ItemForecast a
		JOIN AXGMKDW.dbo.DimItem b ON b.ITEMID = a.ItemId
		JOIN UnitConversion c ON c.FromUnit = a.Unit AND c.ToUnit = b.UnitId
		where a.Unit <> b.UNITID		

	DELETE FROM [ItemForecast]

	INSERT INTO [ItemForecast] ([ItemId],[Unit],[ForecastDate],[ForecastPrice],[ForcedPrice],[LatestPurchaseDate])
		SELECT [ItemId],[Unit],[ForecastDate],[ForecastPrice],[ForcedPrice],[LatestPurchaseDate]
		FROM @ItemForecast
		
	--khusus ID Als ditambahkan 5%
	UPDATE [ItemForecast] 
		SET [ForecastPrice] = [ForecastPrice] *1.05
		WHERE ITEMID IN ('1500001','1500002','1500003')	

	-- 5. ItemStdCost
	DELETE FROM [ItemStdCost]

	INSERT INTO [ItemStdCost] ([ItemId],[Price])
		SELECT [ItemId],[COSTPRICE]
		FROM [AXGMKDW].[dbo].[DimItem]
		WHERE [ItemId] LIKE '9%'

	-- 6. SalesindexPrice
	EXEC SalesPriceIndex_Calc

END

GO

