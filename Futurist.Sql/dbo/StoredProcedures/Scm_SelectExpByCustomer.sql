-- =============================================
-- Author:		<Andi>
-- Create date: <28 May 2025>
-- Description:	<Scm_Select>
-- EXEC [Scm_SelectExpByCustomer] '1 Jan 2025'
-- =============================================
CREATE PROCEDURE [dbo].[Scm_SelectExpByCustomer]
	@PeriodDate datetime = '1 Jan 2025'
AS
BEGIN
	DECLARE	@StartDate datetime, @EndDate datetime, @Divisi nvarchar(20)
	
	SET @StartDate = @PeriodDate
	SET @EndDate = DATEADD(MONTH,1,@PeriodDate) - 1
	SET	@Divisi = 'Export'

	SELECT 
		@PeriodDate as [PeriodDate]
		,a.[INVOICEACCOUNT] as [CUSTID]
		,c.[NAME] as [CUSTNAME]
		,a.[DIVISI]
		,SUM(a.[QTY]) as [QTY]
		,SUM(a.[QTYINKG]) as [QTYINKG]
		
		,SUM(a.[QTY] * a.[SALESPRICE]) / NULLIF(SUM(a.[QTYINKG]),0) as [SALESPRICEINKG]
		,SUM(a.SUMLINEDISCMST) / NULLIF(SUM(a.[QTY] * a.[SALESPRICE]),0) as [LINEPERCENT]
		,SUM(a.SUMLINEDISCMST) as [SUMLINEDISCMST]

		,SUM(a.[LINEAMOUNTMST]) as [SALESAMOUNT]
		,SUM(a.[RMPM]) as [RMPMAMOUNT]
		,SUM(a.[STDCOST]) as [STDCOST]
		
		,SUM(a.[RMPM] + a.[STDCOST]) as [COGS]

		,SUM(a.[RMPM]) / NULLIF(SUM(a.[LINEAMOUNTMST]),0) as [%RMPM]
		
		,SUM(a.[RMPM]+a.[STDCOST]) / NULLIF(SUM(a.[LINEAMOUNTMST]),0) as [%COGS]
		,SUM(a.[LINEAMOUNTMST] - (a.[RMPM] + a.[STDCOST])) as [MARGIN]
	FROM [SCMBI].[dbo].[FactSalesInvoice] a
	JOIN [AXGMKDW].[dbo].[DimItem] b ON b.ITEMID = a.ITEMID
	JOIN [AXGMKDW].[dbo].[DimCustomer] c ON c.ACCOUNTNUM = a.[INVOICEACCOUNT]
	WHERE [INVOICEDATE] BETWEEN @StartDate AND @EndDate
		AND [DIVISI] = @Divisi
	GROUP BY  a.[INVOICEACCOUNT] 
		,c.[NAME] 
		,a.[DIVISI]
	ORDER BY a.[INVOICEACCOUNT]
END

GO

