-- =============================================
-- Author:		<Andi>
-- Create date: <28 May 2025>
-- Description:	<Scm_Select>
-- EXEC [Scm_SelectDomRaw] '1 Jun 2025'
-- =============================================
CREATE PROCEDURE [dbo].[Scm_SelectDomRaw]
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
		,a.LINEPERCENT / 100 as [LINEPERCENT]
		,a.SUMLINEDISCMST
		,a.[LINEAMOUNTMST] as [SALESAMOUNT]
		,a.[RMPM] as [RMPMAMOUNT]
		,a.[STDCOST] as [STDCOST]
		,(a.[RMPM] + a.[STDCOST]) as [COGS]
		,a.[RMPM] / NULLIF(a.[LINEAMOUNTMST],0) as [%RMPM]
		,(a.[RMPM] + a.[STDCOST]) / NULLIF(a.[LINEAMOUNTMST],0) as [%COGS]
		,a.[LINEAMOUNTMST] - (a.[RMPM] + a.[STDCOST]) as [MARGIN]
	FROM [SCMBI].[dbo].[FactSalesInvoice] a
	JOIN [AXGMKDW].[dbo].[DimItem] b ON b.ITEMID = a.ITEMID
	JOIN [AXGMKDW].[dbo].[DimCustomer] c ON c.ACCOUNTNUM = a.[INVOICEACCOUNT]
	WHERE [INVOICEDATE] BETWEEN @StartDate AND @EndDate
		AND [DIVISI] = @Divisi
	ORDER BY a.[INVOICEDATE],a.[INVOICEID]
END

GO

