-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	Select hasil Mup 
-- EXEC [Ps_Mup_Select] 1
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[PsBomEntry_Mup_Select]
@Room int = 1
AS
BEGIN
	SELECT a.[Room] as [ROOM]
		  ,a.[PlanDate] as [PLANDATE]
		  ,a.[ItemId] as [ITEMID]
		  ,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as [ITEMNAME]
		  ,ISNULL(c.GroupName,'') as [GROUPNAME]
		  ,a.Qty as [QTY]
		  ,REPLACE(REPLACE(REPLACE(i.UNITID,CHAR(9),''),CHAR(10),''),CHAR(13),'') as [UNIT] 
	  FROM [PsBomEntry_Mup] a
	  JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
	  LEFT JOIN RMPMPredict.dbo.[ItemGroup] c ON c.ITEMID = a.ItemId
	  WHERE a.[Room] = @Room
	  ORDER BY a.[Room]
		  ,a.[PlanDate]
		  ,a.[ItemId]
END

GO

