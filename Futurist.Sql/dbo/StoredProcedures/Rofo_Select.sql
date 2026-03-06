-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Rofo Select
-- =============================================
CREATE PROCEDURE Rofo_Select
	@Room int = 1
AS
BEGIN
	SET NOCOUNT ON;

    SELECT a.[Room] as [ROOM]
		  , a.[RofoDate] as [ROFO DATE]
		  , a.[ItemId] as [ITEM ID]
		  , i.SEARCHNAME as [ITEM NAME]
		  , a.[QTY] as [ROFO QTY]
	FROM Rofo a
	JOIN AXGMKDW.dbo.DimItem i on i.ITEMID = a.ITEMID
	WHERE a.[Room] = @Room
END

GO

