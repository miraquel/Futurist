-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	FgCost Detail Select menggunakan version
-- EXEC [FgCostVerDetail_Select] 72
-- =============================================
CREATE PROCEDURE [dbo].[FgCostVerDetail_Select]
	@VerId int=13
AS
BEGIN
	SET NOCOUNT ON;

	SELECT a.Room, a.RofoId, a.ProductId, i.SEARCHNAME as [ProductName], a.RofoDate, a.QtyRofo, a.Yield
		,b.ItemId
		,ib.SEARCHNAME as ItemName
		,ISNULL(s.VtaMpSubstitusiGroupId,'') as SubstituteGroup
		,d.ItemId as ItemAllocatedId
		,id.SEARCHNAME as ItemAllocatedName
		,d.Qty
		,d.Price
		,d.RmPrice
		,d.PmPrice
		,d.StdCostPrice
		,d.[Source]
		,d.RefId
	FROM FgCostVer a
		JOIN MupVer b ON b.RofoId = a.RofoId
		join MupTransVer c ON c.MupId = b.RecId
		join ItemTransVer d ON d.RecId = c.ItemTransId
		join AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ProductId
		join AXGMKDW.dbo.DimItem ib ON ib.ITEMID = b.ItemId
		join AXGMKDW.dbo.DimItem id ON id.ITEMID = d.ItemId
		LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = b.ItemId
	WHERE a.VerId = @VerId
	ORDER BY a.RofoDate, a.RofoId, b.ItemId asc
END

GO

