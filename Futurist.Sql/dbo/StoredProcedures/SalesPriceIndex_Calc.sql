-- =============================================
-- Author:		<Andi>
-- Create date: <17 Mar 2025>
-- Description:	<Generate Index Price>
-- EXEC SalesPriceIndex_Calc 
-- =============================================
CREATE PROCEDURE [dbo].[SalesPriceIndex_Calc]
--@PeriodDate datetime = '1 Mar 2025'
AS
BEGIN
	DECLARE @PeriodDate datetime
	SET @PeriodDate = DATEFROMPARTS(YEAR(DATEADD(MONTH,-1,GETDATE())), MONTH(DATEADD(MONTH,-1,GETDATE())),1)
	--SELECT @PeriodDate

	DECLARE @FirstDateInCurrentMonth datetime
	SET @FirstDateInCurrentMonth = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()),1)
	--SELECT @FirstDateInCurrentMonth

	DELETE FROM [SalesPrice] WHERE PeriodDate = @PeriodDate

	INSERT INTO [SalesPrice] ([ItemId],[PeriodDate],[Qty],[GrossSales],[DiscPercent],[DiscValue],[NetSales],[SalesPriceIndex])
		SELECT a.ITEMID as [ItemId]
			,@PeriodDate as PeriodDate
			,SUM(a.QTY) as [Qty]
			,SUM(a.QTY*a.EXCHRATE/100*a.SALESPRICE) as [GrossSales]
			,SUM(a.QTY*a.EXCHRATE/100*a.SALESPRICE*a.DISCPERCENT/100) /  NULLIF( (SUM(a.QTY*a.SALESPRICE)),0) as [Disc%]
			,SUM(a.QTY*a.EXCHRATE/100*a.SALESPRICE*a.DISCPERCENT/100) as [DiscValue]
			--,SUM(a.QTY*(a.SALESPRICE*(1-a.DISCPERCENT/100))) as [NetSales]
			,SUM(a.LINEAMOUNTMST) as [NetSales]
			--,ISNULL(SUM(a.QTY*(a.SALESPRICE*(1-a.DISCPERCENT/100))) / NULLIF(SUM(a.QTY),0),0) as [SalesPriceIndex]
			,ISNULL(SUM(a.LINEAMOUNTMST) / NULLIF(SUM(a.QTY),0),0) as [SalesPriceIndex]
		FROM [AXGMKDW].[dbo].[FactSalesInvoice] a
		JOIN (
			SELECT [Bu], MAX([IncreaseDate]) as [IncreaseDate]
			FROM [SalesPriceIncreaseDate]
			GROUP BY [Bu]
		) b ON b.[Bu] = a.[BUSINESSUNIT] AND a.INVOICEDATE >= b.[IncreaseDate] 
		WHERE a.INVOICEDATE < @FirstDateInCurrentMonth
			--AND a.ITEMID = '4201565'
		GROUP BY a.ITEMID

	--SELECT * FROM [SalesPrice] WHERE PeriodDate = @PeriodDate
END

GO

