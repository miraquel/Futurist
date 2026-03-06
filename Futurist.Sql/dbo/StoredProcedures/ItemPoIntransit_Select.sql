-- =============================================
-- Author:		<Andi>
-- Create date: <2 Sep 2025>
-- Description:	<Melihat data Po Intransit>
-- =============================================
CREATE PROCEDURE ItemPoIntransit_Select 
AS
BEGIN
	SELECT a.[Po]
		,a.[DeliveryDate]
		,a.[ItemId]
		,i.[SEARCHNAME] as [ItemName]
		,a.[Qty]
		,a.[Unit]
		,a.[CurrencyCode]
		,a.[Price]
	FROM [ItemPoIntransit] a
	JOIN AXGMKDW.dbo.DimItem i ON i.ItemId = a.ItemId 
	ORDER BY a.DeliveryDate
END

GO

