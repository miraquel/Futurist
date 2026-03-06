-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	harga rata material yg digunakan per bulan menggunakan version
-- =============================================
CREATE PROCEDURE [dbo].[RptMaterialPerMonthByVer]
@VerId int=13,
@ItemId nvarchar(20) = ''
AS
BEGIN
	SET NOCOUNT ON;

	IF @ItemId = ''
		SELECT a.ItemId
			,i.SEARCHNAME as ItemName 
			,ISNULL(s.VtaMpSubstitusiGroupId,'') as SubstituteGroup
			,i.UNITID
			,m.MupDate
			,SUM(a.Qty) as [Qty]
			,SUM(a.Qty*a.Price) / SUM(a.Qty) as [Avg Price]
		FROM ItemTransVer a
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
		LEFT JOIN MupTransVer mt ON mt.ItemTransId = a.RecId 
		LEFT JOIN Mup m ON m.RecId = mt.MupId
		LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = a.ItemId
		WHERE a.VerId = @VerId
		GROUP BY a.ItemId,i.SEARCHNAME,i.UNITID,m.MupDate,s.VtaMpSubstitusiGroupId
		ORDER BY a.ItemId asc
	ELSE
		
		SELECT a.ItemId
			,i.SEARCHNAME as ItemName 
			,ISNULL(s.VtaMpSubstitusiGroupId,'') as SubstituteGroup
			,i.UNITID
			,m.MupDate
			,SUM(a.Qty) as [Qty]
			,SUM(a.Qty*a.Price) / SUM(a.Qty) as [Avg Price]
		FROM ItemTransVer a
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
		LEFT JOIN MupTransVer mt ON mt.ItemTransId = a.RecId 
		LEFT JOIN Mup m ON m.RecId = mt.MupId
		LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = a.ItemId
		WHERE a.VerId = @VerId AND a.ItemId = @ItemId
		GROUP BY a.ItemId,i.SEARCHNAME,i.UNITID,m.MupDate,s.VtaMpSubstitusiGroupId
		ORDER BY a.ItemId asc
END

GO

