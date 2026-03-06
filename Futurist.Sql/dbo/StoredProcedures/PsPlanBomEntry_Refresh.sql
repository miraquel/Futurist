-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	Mup Calculation
-- EXEC [PsPlanBomEntry_Refresh] 1
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[PsPlanBomEntry_Refresh]
@Room int = 1
AS
BEGIN
	SELECT a.[Room] as [ROOM]
		  , a.[ITEMID]
		  , i.SEARCHNAME as [ITEMNAME]
		  , a.PlanDate as [PLANDATE]
		  , a.[QTY]
		  , i.UNITID
		  , a.BomIdEntry
		  , a.BOMID
		  , a.BOMID6
	FROM PsPlanBomEntry a
	JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ITEMID
	WHERE a.[Room] = @Room
	ORDER BY a.ItemId asc

END

GO

