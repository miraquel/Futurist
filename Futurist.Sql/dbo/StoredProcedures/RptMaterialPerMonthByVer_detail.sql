-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Detail harga material yg digunakan menggunakan version
-- =============================================
CREATE PROCEDURE [dbo].[RptMaterialPerMonthByVer_detail]
@VerId int=13,
@ItemId nvarchar(20) = '1000434'
AS
BEGIN
	SET NOCOUNT ON;

	IF @ItemId = ''
		SELECT a.ItemId
			,i.SEARCHNAME as ItemName 
			,ISNULL(s.VtaMpSubstitusiGroupId,'') as SubstituteGroup
			,i.UNITID
			,m.MupDate
			,a.Qty
			,a.Price
			,a.[Source]
			,a.RefId
		FROM ItemTransVer a
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
		LEFT JOIN MupTransVer mt ON mt.ItemTransId = a.RecId 
		LEFT JOIN Mup m ON m.RecId = mt.MupId
		LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = a.ItemId
		WHERE a.VerId = @VerId
		ORDER BY a.ItemId, m.MupDate asc
	ELSE
		
		SELECT a.ItemId
			,i.SEARCHNAME as ItemName 
			,ISNULL(s.VtaMpSubstitusiGroupId,'') as SubstituteGroup
			,i.UNITID
			,m.MupDate
			,a.Qty
			,a.Price
			,a.[Source]
			,a.RefId
		FROM ItemTransVer a
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
		LEFT JOIN MupTransVer mt ON mt.ItemTransId = a.RecId 
		LEFT JOIN Mup m ON m.RecId = mt.MupId
		LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = a.ItemId
		WHERE a.VerId = @VerId AND a.ItemId = @ItemId
		ORDER BY a.ItemId, m.MupDate asc

END

GO

