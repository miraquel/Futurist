-- =============================================
-- Author:		<Andi>
-- Create date: <2 Sep 2025>
-- Description:	<Melihat data stock on hand>
-- =============================================
CREATE PROCEDURE [dbo].[ItemOnhand_Select] 
AS
BEGIN
	SELECT a.[ItemId]
		,i.SEARCHNAME as [ItemName]
		,a.[InventBatch]
		,a.[ExpDate]
		,a.[PdsDispositionCode]
		,a.[Qty]
		,i.UNITID
		,a.[Price]
	FROM [ItemOnhand] a
	JOIN AXGMKDW.dbo.DimItem i ON i.ItemId = a.ItemId 
	ORDER BY a.[ItemId]
END

GO

