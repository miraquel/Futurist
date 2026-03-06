-- =============================================
-- Author:		<Andi>
-- Create date: <29 April 2025>
-- Description:	<memproses perhitungan RmPmAcc>
-- Notes: untuk mengkalkulasi bulan 4 perlu memproses [AnStockMonthly_calc] '1 May 2025' sebagai data ending stock
-- EXEC [RmPmAcc_Calc] 2025,9
-- Durasi:  0 detik utk 1 row
-- =============================================
CREATE PROCEDURE [dbo].[RmPmAcc_Calc]
	@Year int = 2026
	,@Month int = 1
AS
BEGIN
	--SET NOCOUNT ON;
	DECLARE @BeginingDate datetime, @EndingDate datetime

	DECLARE @FgOpeningStdCostPercent numeric(32,16), @FgEndingStdCostPercent numeric(32,16)
		,@WipOpeningStdCostPercent numeric(32,16), @WipEndingStdCostPercent numeric(32,16) 

	DECLARE @RmPmOpening numeric(32,16), @RmPmPurchase numeric(32,16), @RmPmEnding numeric(32,16), @RmPmUsed numeric(32,16)
		,@WipOpening numeric(32,16), @WipOpeningRmPm numeric(32,16), @WipOpeningStdCost numeric(32,16)
		,@WipEnding numeric(32,16), @WipEndingRmPm numeric(32,16), @WipEndingStdCost numeric(32,16)
		,@FgOpening numeric(32,16), @FgOpeningRmPm numeric(32,16), @FgOpeningStdCost numeric(32,16)
		,@FgEnding numeric(32,16), @FgEndingRmPm numeric(32,16), @FgEndingStdCost numeric(32,16)
		,@StdCostAct numeric(32,16)
		,@RmPmGross numeric(32,16), @RmPmNet numeric(32,16)
		,@SalesNet numeric(32,16)

	SET @BeginingDate = DATEFROMPARTS(@Year,@Month,1)
	SET @EndingDate = DATEADD(MONTH,1,@BeginingDate) 
	
	DELETE FROM [RmPmAcc]
	WHERE [Year] = @Year AND [Month] = @Month

	--[RmPmOpening]
	SELECT @RmPmOpening = sum([Opening])
	--FROM [AXGMKDW].[dbo].[FactTrialBalance]
	FROM [SellOut].[dbo].[TrialBalances]
	WHERE [Year] = @Year
		AND [Month] = @Month
		AND MainAccountId IN (10501, 10502, 10511, 10512)

	--[RmPmPurchase]
	SELECT @RmPmPurchase = SUM(ACCOUNTINGCURRENCYAMOUNT)
	FROM [AXGMKDW].[dbo].[FactGjPurchase] 
	WHERE YEAR(ACCOUNTINGDATE) = @Year AND MONTH(ACCOUNTINGDATE) = @Month

	--[RmPmEnding]
	SELECT @RmPmEnding = sum([Closing])
	--FROM [AXGMKDW].[dbo].[FactTrialBalance]
	FROM [SellOut].[dbo].[TrialBalances]
	WHERE [Year] = @Year
		AND [Month] = @Month
		AND MainAccountId IN (10501, 10502, 10511, 10512)

	--[RmPmUsed]
	SET @RmPmUsed = @RmPmOpening+@RmPmPurchase-@RmPmEnding

	--[WipOpening]
	SELECT @WipOpening = sum([Opening])
	--FROM [AXGMKDW].[dbo].[FactTrialBalance]
	FROM [SellOut].[dbo].[TrialBalances]
	WHERE [Year] = @Year
		AND [Month] = @Month
		AND MainAccountId IN (10503, 10506, 10513, 10516, 10522)
			
	--[WipOpeningStdCost]
	SELECT @WipOpeningStdCostPercent = SUM(QTY*STDCOSTPRICE) / SUM(QTY*COSTPRICE)  
	FROM [AXGMKDW].[dbo].[FactStockMonthly]
	WHERE (ITEMID LIKE '6%' OR ITEMID LIKE '2%')
		AND QTY > 0 AND RmPrice > 0 AND StdCostPrice > 0
		AND STOCKDATE = @BeginingDate

	SET @WipOpeningStdCost = @WipOpening * @WipOpeningStdCostPercent

	
	--[WipOpeningRmPm]
	SET @WipOpeningRmPm = @WipOpening - @WipOpeningStdCost
		
	--[WipEnding]
	SELECT @WipEnding = sum([Closing])
	--FROM [AXGMKDW].[dbo].[FactTrialBalance]
	FROM [SellOut].[dbo].[TrialBalances]
	WHERE [Year] = @Year
		AND [Month] = @Month
		AND MainAccountId IN (10503, 10506, 10513, 10516, 10522)
				
	--[WipEndingStdCost]
	SELECT @WipEndingStdCostPercent = SUM(QTY*STDCOSTPRICE) / SUM(QTY*COSTPRICE)  
	FROM [AXGMKDW].[dbo].[FactStockMonthly]
	WHERE (ITEMID LIKE '6%' OR ITEMID LIKE '2%')
		AND QTY > 0 AND RmPrice > 0 AND StdCostPrice > 0
		AND STOCKDATE = @EndingDate
		
	SET @WipEndingStdCost = @WipEnding * @WipEndingStdCostPercent

	--[WipEndingRmPm]
	SET @WipEndingRmPm = @WipEnding - @WipEndingStdCost

	--[FgOpening]
	SELECT @FgOpening = sum([Opening])
	FROM [SellOut].[dbo].[TrialBalances]
	WHERE [Year] = @Year
		AND [Month] = @Month
		AND MainAccountId IN (10504, 10514, 10507)
			
	--[FgOpeningStdCost]
	SELECT @FgOpeningStdCostPercent = SUM(QTY*STDCOSTPRICE) / SUM(QTY*COSTPRICE)  
	FROM [AXGMKDW].[dbo].[FactStockMonthly]
	WHERE ITEMID LIKE '4%' 
		AND QTY > 0 AND RmPrice > 0 AND StdCostPrice > 0
		AND STOCKDATE = @BeginingDate
		
	SET @FgOpeningStdCost = @FgOpening * @FgOpeningStdCostPercent
	
	--[FgOpeningRmPm]
	SET @FgOpeningRmPm = @FgOpening - @FgOpeningStdCost
			
		
	--[FgEnding]
	SELECT @FgEnding = sum([Closing])
	FROM [SellOut].[dbo].[TrialBalances]
	WHERE [Year] = @Year
		AND [Month] = @Month
		AND MainAccountId IN (10504, 10514, 10507)
		
	--[FgEndingStdCost]
	SELECT @FgEndingStdCostPercent = SUM(QTY*STDCOSTPRICE) / SUM(QTY*COSTPRICE)  
	FROM [AXGMKDW].[dbo].[FactStockMonthly]
	WHERE ITEMID LIKE '4%' 
		AND QTY > 0 AND RmPrice > 0 AND StdCostPrice > 0
		AND STOCKDATE = @EndingDate
		
	SET @FgEndingStdCost = @FgEnding * @FgEndingStdCostPercent

	--[FgEndingRmPm]
	SET @FgEndingRmPm = @FgEnding - @FgEndingStdCost
	

	--[StdCostAct]
	SELECT @StdCostAct = sum([Debit]-[Credit])
	FROM [SellOut].[dbo].[TrialBalances]
	WHERE [Year] = @Year
		AND [Month] = @Month
		AND MainAccountId LIKE '6%'
		AND CostCenterId IN ('PROD','OPEX')

	--[RmPmGross]
	SET @RmPmGross = @RmPmUsed + (@WipOpening-@WipEnding) + (@FgOpening-@FgEnding)

	--[RmPmNet]
	SET @RmPmNet =  @RmPmUsed + (@WipOpeningRmpm-@WipEndingRmpm) + (@FgOpeningRmpm-@FgEndingRmpm)

	--[SalesNet]
	SELECT @SalesNet = -1* sum([Debit]-[Credit])
	FROM [SellOut].[dbo].[TrialBalances]
	WHERE [Year] = @Year
		AND [Month] = @Month
		AND MainAccountId LIKE '4%'
	
	--SELECT @Year as [Year], @Month as [Month], @RmPmOpening as [RmPmOpening], @RmPmPurchase as [RmPmPurchase]
	--	,@RmPmEnding as [RmPmEnding], @RmPmUsed as [RmPmUsed] 
	--	,@WipOpening as [WipOpening], @WipOpeningRmPm as [WipOpeningRmPm], @WipOpeningStdCost as [WipOpeningStdCost]
	--	,@WipEnding as [WipEnding], @WipEndingRmPm as [WipEndingRmPm], @WipEndingStdCost as [WipEndingStdCost]
	--	,@FgOpening as [FgOpening], @FgOpeningRmPm as [FgOpeningRmPm], @FgOpeningStdCost as [FgOpeningStdCost]
	--	,@FgEnding as [FgEnding], @FgEndingRmPm as [FgEndingRmPm], @FgEndingStdCost as [FgEndingStdCost]
	--	,@StdCostAct as [StdCostAct], @RmPmGross as [RmPmGross], @RmPmNet as [RmPmNet], @SalesNet as [SalesNet]

	--RETURN


	INSERT INTO [RmPmAcc] ([Year],[Month],[RmPmOpening],[RmPmPurchase],[RmPmEnding],[RmPmUsed]
		,[WipOpening],[WipOpeningRmPm],[WipOpeningStdCost]
		,[WipEnding],[WipEndingRmPm],[WipEndingStdCost]
		,[FgOpening],[FgOpeningRmPm],[FgOpeningStdCost]
		,[FgEnding],[FgEndingRmPm],[FgEndingStdCost]
		,[StdCostAct],[RmPmGross],[RmPmNet],[SalesNet])
		SELECT @Year, @Month, @RmPmOpening, @RmPmPurchase, @RmPmEnding, @RmPmUsed
			,@WipOpening, @WipOpeningRmPm, @WipOpeningStdCost
			,@WipEnding, @WipEndingRmPm, @WipEndingStdCost
			,@FgOpening, @FgOpeningRmPm, @FgOpeningStdCost
			,@FgEnding, @FgEndingRmPm, @FgEndingStdCost
			,@StdCostAct,@RmPmGross,@RmPmNet,@SalesNet
			
	
	SELECT [Year]
		,[Month]
		,[RmPmOpening]
		,[RmPmPurchase]
		,[RmPmEnding]
		,[RmPmUsed]
		,[WipOpening]
		,[WipOpeningRmPm]
		,[WipOpeningStdCost]
		,[WipEnding]
		,[WipEndingRmPm]
		,[WipEndingStdCost]
		,[FgOpening]
		,[FgOpeningRmPm]
		,[FgOpeningStdCost]
		,[FgEnding]
		,[FgEndingRmPm]
		,[FgEndingStdCost]
		,[StdCostAct]
		,[RmPmGross]
		,[RmPmNet]
		,[SalesNet]
	FROM [RmPmAcc]
	WHERE [Year] = @Year AND [Month] = @Month
END

GO

