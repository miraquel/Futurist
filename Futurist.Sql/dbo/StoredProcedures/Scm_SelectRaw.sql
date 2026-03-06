-- =============================================
-- Author:		<Andi>
-- Create date: <28 May 2025>
-- Description:	<Scm_Select>
-- EXEC [Scm_SelectRaw] '1 Jan 2025'
-- =============================================
CREATE PROCEDURE [dbo].[Scm_SelectRaw]
	@PeriodDate datetime = '1 Jan 2025'
AS
BEGIN
	DECLARE	@StartDate datetime, @EndDate datetime, @Divisi nvarchar(20)
	
	SET @StartDate = @PeriodDate
	SET @EndDate = DATEADD(MONTH,1,@PeriodDate) - 1
	SET	@Divisi = 'Domestik'	--'Export'

	SELECT 
		@PeriodDate as [PeriodDate]
		,a.[INVOICEDATE]
		,a.[INVOICEID]
		,a.[INVOICEACCOUNT] as [CUSTID]
		,c.[NAME] as [CUSTNAME]
		,a.[DIVISI]
		,a.[BUSINESSUNIT]
		,a.[ITEMID]
		,b.[SEARCHNAME] as [ITEMNAME]
		,b.[BRANDID] as [BRAND]
		,a.[INVENTBATCHID] 
		,a.[QTY] as [QTY]
		,a.[QTYINKG] as [QTYINKG]
		,a.SALESPRICE
		,a.LINEPERCENT
		,a.SUMLINEDISCMST
		,a.[LINEAMOUNTMST] as [SALESAMOUNT]
		,a.[RMPM] as [RMPMAMOUNT]
		,a.[STDCOST] as [STDCOST]
		,a.[RMPM] / NULLIF(a.[LINEAMOUNTMST],0) as [%RMPM]
	FROM [SCMBI].[dbo].[FactSalesInvoice] a
	JOIN [AXGMKDW].[dbo].[DimItem] b ON b.ITEMID = a.ITEMID
	JOIN [AXGMKDW].[dbo].[DimCustomer] c ON c.ACCOUNTNUM = a.[INVOICEACCOUNT]
	WHERE [INVOICEDATE] BETWEEN @StartDate AND @EndDate
	ORDER BY a.[INVOICEDATE],a.[INVOICEID]
END

GO

