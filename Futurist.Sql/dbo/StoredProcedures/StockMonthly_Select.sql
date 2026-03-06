-- =============================================
-- Author:		<Andi>
-- Create date: <27 May 2025>
-- Description:	<Stockonhand Monthly>
-- EXEC [StockMonthly_Select] '1 Jan 2025'
-- =============================================
CREATE PROCEDURE [dbo].[StockMonthly_Select]
	@StockDate datetime = '1 Jan 2025'
AS
BEGIN
	SELECT a.[ITEMID]
		,REPLACE(REPLACE(REPLACE(b.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as [ITEMNAME]
		,a.[STOCKDATE]
		,a.[INVENTBATCHID]
		,a.[QTY]
		,(a.[QTY] * b.NETWEIGHT / 1000) as [QTYINKG]
		,a.[COSTPRICE]
		,a.[RMPRICE]
		,a.[PMPRICE]
		,a.[STDCOSTPRICE]
		,(a.[QTY] * a.[COSTPRICE]) as [COSTVALUE]
		,(a.[QTY] * a.[STDCOSTPRICE]) as [STDCOSTVALUE]
	FROM [AXGMKDW].[dbo].[FactStockMonthly] a
	LEFT JOIN [AXGMKDW].[dbo].[DimItem] b ON b.[ITEMID] = a.[ITEMID]
	WHERE a.[STOCKDATE] = @StockDate
	ORDER BY a.[ITEMID]
END

GO

