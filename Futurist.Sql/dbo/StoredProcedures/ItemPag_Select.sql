-- =============================================
-- Author:		<Andi>
-- Create date: <2 Sep 2025>
-- Description:	<Melihat data contract PAG>
-- =============================================
CREATE PROCEDURE ItemPag_Select 
AS
BEGIN
	SELECT a.[ItemId]
		,i.SEARCHNAME as [ItemName]
		,a.[Unit]
		,a.[Pag]
		,a.[VendorId]
		,v.[NAME] as [VendorName]
		,a.[EffectiveDate]
		,a.[ExpirationDate]
		,a.[Qty]
		,a.[CurrencyCode]
		,a.[Price]
	FROM [ItemPag] a
	JOIN AXGMKDW.dbo.DimItem i ON i.ItemId = a.ItemId 
	JOIN AXGMKDW.dbo.DimVendor v ON v.ACCOUNTNUM = a.[VendorId] 
	ORDER BY a.[ItemId]
END

GO

