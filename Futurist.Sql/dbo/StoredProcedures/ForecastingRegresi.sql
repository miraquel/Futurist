-- =============================================
-- Author:		<Andi>
-- Create date: <12 Maret 2025>
-- Description:	<Harga forecasting>
-- Duration   : <17 menit>, <8 detik>, <20 detik>
-- =============================================
CREATE PROCEDURE [dbo].[ForecastingRegresi]
AS
BEGIN
	DECLARE @DateProcess as datetime
	SET @DateProcess = GETDATE()

	--Buat kerangka tanggal 2 tahun kebelakang dan 1 tahun kedepan
	DECLARE @YearMax int, @YearMin int, @Year int, @i int

	DECLARE @DateTransTable as table (
		YearId int
		,MonthId int
		,DateTrans datetime
	)

	SET @YearMax = YEAR(@DateProcess)+1
	SET @YearMin = YEAR(@DateProcess)-2
	SET @Year = @YearMin

	WHILE @Year <= @YearMax
	BEGIN
		SET @i = 1
		WHILE @i <= 12
		BEGIN
			INSERT INTO @DateTransTable (YearId,MonthId,DateTrans)
				SELECT @Year, @i, DATEFROMPARTS(@Year,@i,1)
			SET @i = @i + 1
		END		
		SET @Year = @Year + 1
	END

	--Buat Item id yang dibeli dan kerangka waktu selama periode diatas
	DECLARE @Item as table (
		ItemId nvarchar(20)
		,PurchUnit nvarchar(20)
		,LatestPurchaseDate datetime
	)
	
	DELETE FROM [PurchaseForecastPrice]

	DECLARE @ItemId nvarchar(20), @DateTrans datetime

	INSERT INTO @Item (ItemId,PurchUnit,LatestPurchaseDate) 
		SELECT ItemId,PurchUnit,MAX([DELIVERYDATE]) as [LatestPurchaseDate]
		FROM [AXGMKDW].[dbo].[FactGr]
		WHERE DELIVERYDATE BETWEEN DATEADD(year,-2,@DateProcess) AND @DateProcess
			AND (LEFT(ITEMID,1) = 1 OR LEFT(ITEMID,1) = 3)
			AND QTY > 0
			AND PurchUnit <> 'g'
			--AND ITEMID = '1000030'			
		GROUP BY ItemId,PurchUnit
			
	INSERT INTO [PurchaseForecastPrice] (ItemId,PurchUnit,LatestPurchaseDate,InventUnit,DateTrans)
		SELECT a.ItemId, a.PurchUnit, a.LatestPurchaseDate, b.UNITID, c.DateTrans
		FROM @Item a
		JOIN [AXGMKDW].[dbo].[DimItem] b ON b.ITEMID = a.ItemId
		CROSS APPLY @DateTransTable c
		ORDER BY a.ItemId, c.DateTrans

	--Buat table @MinRec untuk membuat N populasi
	DECLARE @MinRec TABLE (ItemId nvarchar(20), MinRecId int)
	INSERT INTO @MinRec
		SELECT ItemId, MIN(RecId) as [MinRecId]
		FROM [PurchaseForecastPrice]
		GROUP BY ItemId 

	UPDATE [PurchaseForecastPrice]
	SET N = a.RecId - b.[MinRecId] + 1
	FROM [PurchaseForecastPrice] a
		JOIN @MinRec b ON b.ItemId = a.ItemId 

	--Buat harga dari history pembelian aktual
	UPDATE [PurchaseForecastPrice]
	SET Price = b.PriceGr
		,[Source] = 'Purchase'
	FROM [PurchaseForecastPrice] a
	JOIN (
		SELECT ITEMID as ItemId
			,DATEFROMPARTS (YEAR(DELIVERYDATE), MONTH(DELIVERYDATE), 1) as [DateGr]
			,(SUM(VALUEMST)/NULLIF(SUM(QTY),0))as [PriceGr]
		FROM [AXGMKDW].[dbo].[FactGr]
		WHERE QTY > 0
			AND PURCHID NOT IN ('PO24013247', 'PO24014486')
		GROUP BY ITEMID, YEAR(DELIVERYDATE), MONTH(DELIVERYDATE) 
	) b ON b.ItemId = a.ItemId AND b.DateGr = a.DateTrans

	
	--hapus transaksi awal yang tidak ada harga pembelian
	DELETE a
	FROM [PurchaseForecastPrice] a
	JOIN (
		SELECT ItemId, MIN(DateTrans) as MinDateTrans FROM [PurchaseForecastPrice] WHERE [Source] = 'Purchase' GROUP BY ItemId 
	) b ON b.ItemId = a.ItemId AND a.DateTrans < b.MinDateTrans 

	--Isi harga yang kosong dengan pembelian sebelumnya
	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT ItemId, DateTrans FROM [PurchaseForecastPrice] WHERE Price is null  AND DateTrans < getdate()

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @ItemId, @DateTrans
 
    WHILE @@FETCH_STATUS = 0
    BEGIN
		UPDATE [PurchaseForecastPrice] 
			SET Price = b.Price
				,[Source] = 'Previous Purchase'
			FROM [PurchaseForecastPrice] a
			JOIN (
				SELECT top 1 ItemId, Price FROM [PurchaseForecastPrice] 
				WHERE ItemId = @ItemId AND DateTrans < @DateTrans
				ORDER BY DateTrans DESC
			) b ON b.ItemId = a.ItemId
			WHERE a.ItemId = @ItemId AND a.DateTrans = @DateTrans
		

	FETCH NEXT FROM Cur01 INTO @ItemId, @DateTrans
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01
	
	--Rubah harga ke satuan invent unit
	UPDATE [PurchaseForecastPrice]
	SET [PriceUnitInvent] = IIF(a.PurchUnit <> a.InventUnit
								,a.[Price] / NULLIF((c.Factor * c.Numerator / NULLIF(c.Denominator,0)),0)
								,a.[Price])
	FROM [PurchaseForecastPrice] a
		LEFT JOIN UnitConversion c ON c.FromUnit = a.PurchUnit AND c.ToUnit = a.InventUnit	
		
	--Persiapakam table Regresi utk perhitungan harga forecast 
	DECLARE @Regresi table (
		RecId int identity
		,ItemId nvarchar(20) 
		,Cnt numeric(32,16)
		,SumX numeric(32,16)
		,SumY numeric(32,16)
		,SumXY numeric(32,16)
		,SumX2 numeric(32,16)
		,SumX#2 numeric(32,16)
		,Slope numeric(32,16)
		,Intercept numeric(32,16)
	)

	INSERT INTO @Regresi (ItemId,Cnt,SumX,SumY,SumXY,SumX2,SumX#2 )
		SELECT a.ItemId, count(a.ItemId) as Cnt
			,SUM(N) as SumX
			,SUM(PriceUnitInvent) as SumY
			,SUM(N*PriceUnitInvent) as SumXY
			,POWER(SUM(N),2)  as SumX2
			,SUM(POWER(N,2)) as SumX#2
		FROM [PurchaseForecastPrice] a
		WHERE DateTrans < getdate()
		GROUP BY ItemId
		ORDER BY a.ItemId
	
	UPDATE @Regresi
	SET Slope = ((Cnt*SumXY) - (SumX*SumY)) / NULLIF((Cnt*SumX#2 - SumX2),0)	--(Cnt*SumX2-SumX#2)

	UPDATE @Regresi
	SET Intercept = (SumY - (Slope*SumX)) / NULLIF(Cnt,0)
	
	--Update harga forecast menggunakan bantuan table regresi
	UPDATE [PurchaseForecastPrice]
	SET PriceUnitInvent = b.Slope * N + b.Intercept
		,[Source] = 'Forecasting'
		,Slope = b.Slope
		,Intercept = b.Intercept
	FROM [PurchaseForecastPrice] a
	JOIN @Regresi b ON b.ItemId = a.ItemId
	WHERE [Source] is null
		
	--Perbaiki harga forecast yang turun dari latest purchase, samakan dengan latest purch price
	UPDATE [PurchaseForecastPrice]
	SET PriceUnitInvent = b.LatestPurchasePrice
		,[Source] = 'Forecasting LP'
	FROM [PurchaseForecastPrice] a
	JOIN (
		SELECT x.ItemId, x.DateTrans, y.PriceUnitInvent as LatestPurchasePrice
		FROM (
			SELECT ItemId, MAX(DateTrans) as DateTrans
			FROM [PurchaseForecastPrice]
			WHERE [Source] = 'Purchase'
			GROUP BY ItemId
		) x 
		JOIN [PurchaseForecastPrice] y ON y.ItemId = x.ItemId AND y.DateTrans = x.DateTrans
	) b ON b.ItemId = a.ItemId
	WHERE [Source] = 'Forecasting'
		AND a.PriceUnitInvent < b.LatestPurchasePrice

	
	DELETE FROM [PurchaseForecastPrice]
	WHERE [PriceUnitInvent] IS NULL



	

END

GO

