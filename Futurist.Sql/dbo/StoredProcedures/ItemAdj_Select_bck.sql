-- =============================================
-- Author:		Andi
-- Create date: 26 Mar 2025
-- Description:	Select Item yang perlu dilakukan Adj Price
-- EXEC [ItemAdj_Select] 5
-- Duration: 0 detik
-- =============================================
CREATE PROCEDURE [dbo].[ItemAdj_Select_bck]
	@Room int = 1
AS
BEGIN

    SELECT DISTINCT t.[Room] AS [Room]
                  , t.[ItemId] AS [ItemId]
				  , REPLACE(REPLACE(REPLACE(d.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as [ItemName]
                  , d.UNITID --AS [Unit]
                  , ISNULL(s.VtaMpSubstitusiGroupId, '') AS [ItemGroup]
				  , ISNULL(g.[GroupName],'') as [GroupProcurement]
                  , t.[Price]
    FROM [CogsProjection].[dbo].[ItemTrans] t with(nolock)
             JOIN [AXGMKDW].[dbo].[DimItem] d with(nolock) ON d.ItemId = t.ItemId
             LEFT JOIN [AXGMKDW].[dbo].[DimItemSubstitute] s with(nolock) ON s.ItemId = t.ItemId
			 LEFT JOIN [ItemGroupProcurement] g with(nolock) ON g.ItemId = t.ItemId
	
    WHERE t.Room = @Room
      AND t.Price = 0
      AND (t.ItemId LIKE '1%' OR t.ItemId LIKE '3%')	  
      AND t.ItemId NOT IN (
        SELECT ItemId FROM [CogsProjection].[dbo].[ItemAdj] ia with(nolock)
        WHERE ia.Room = t.Room
          AND ia.ItemId = t.ItemId
      --AND NOT EXISTS (
      --  SELECT 1 FROM [CogsProjection].[dbo].[ItemAdj] ia with(nolock)
      --  WHERE ia.Room = t.Room
      --    AND ia.ItemId = t.ItemId
    )
    
    UNION
    
    SELECT a.[Room] AS [Room]
         , a.[ItemId] AS [ItemId]
         , b.SEARCHNAME AS [ItemName]
         , b.UNITID AS [Unit]
         , ISNULL(c.VtaMpSubstitusiGroupId, '') AS [ItemGroup]
		 , ISNULL(g.[GroupName],'') as [GroupProcurement]
         , a.[AdjPrice] AS [Price]
    FROM [CogsProjection].[dbo].[ItemAdj] a with(nolock)
             JOIN [AXGMKDW].[dbo].[DimItem] b with(nolock) ON b.ItemId = a.ItemId
             LEFT JOIN [AXGMKDW].[dbo].[DimItemSubstitute] c with(nolock) ON c.ItemId = a.ItemId
			 LEFT JOIN [ItemGroupProcurement] g with(nolock) ON g.ItemId = a.ItemId
    WHERE a.[Room] = @Room
END

GO

