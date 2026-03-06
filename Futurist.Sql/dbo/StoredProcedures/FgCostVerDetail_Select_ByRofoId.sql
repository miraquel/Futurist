-- =============================================
-- Author:		Andi
-- Create date: 09 Jun 2025
-- Description:	FgCostVer Detail Select untuk FG calc yang sudah dilakukan version
-- EXEC [FgCostVerDetail_Select_ByRofoId] 287335, 84
-- =============================================
CREATE PROCEDURE [dbo].[FgCostVerDetail_Select_ByRofoId]
	@RofoId int = 115939
	,@VerId int = 66
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT a.VerId
		,a.RecId as RofoId
		,a.Room
		,a.ItemId as ProductId
		,i.SEARCHNAME as ProductName
		,a.RofoDate
		,a.Qty as QtyRofo
		,b.ItemId
		,ib.SEARCHNAME as ItemName
		,ISNULL(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
		,ISNULL(g.[GroupName],'') as [GroupProcurement]
		,d.ItemId as ItemAllocatedId
		,id.SEARCHNAME as ItemAllocatedName
		,id.[UNITID]
		,d.[InventBatch]
		,d.Qty
		,d.Price
		,d.RmPrice
		,d.PmPrice
		,d.StdCostPrice
		,d.[Source]
		,d.RefId
		,id.LatestPrice
	FROM [RofoVer] a WITH (NOLOCK) 
	JOIN [MupVer] b WITH (NOLOCK) ON b.RofoId = a.RecId AND b.VerId = a.VerId
	JOIN [MupTransVer] c WITH (NOLOCK) ON c.MupId = b.RecId AND c.VerId = b.VerId
	JOIN [ItemTransVer] d WITH (NOLOCK) ON d.RecId = c.ItemTransId AND d.VerId = c.VerId
	JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
	JOIN AXGMKDW.dbo.DimItem ib WITH (NOLOCK) ON ib.ITEMID = b.ItemId
	JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	LEFT JOIN [DimItemSubstituteVer] s WITH (NOLOCK) ON s.ItemId = b.ItemId AND s.VerId = b.VerId
	LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = b.ItemId
	WHERE a.RecId = @RofoId
		AND a.VerId = @VerId
	--ORDER BY d.ItemId
END

GO

