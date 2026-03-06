-- =============================================
-- Author:		Andi
-- Create date: 22 jan 2025
-- Description:	untuk monitoring Material per bulan
-- exec [MupItemSummary_Select] 5
-- =============================================
CREATE PROCEDURE [dbo].[MupItemSummary_Select]
	@Room int=5
AS
BEGIN
	--SET NOCOUNT ON;
		
	SELECT DATEFROMPARTS(YEAR(m.MupDate),MONTH(m.MupDate),1) as [MupDate]
		,isnull(s.VtaMpSubstitusiGroupId,'') as [Group Substitusi]
		,a.ItemId, i.SEARCHNAME as ItemName, sum(a.Qty) as Qty, sum(a.Qty * a.Price) / sum(a.Qty) as [Price] 
		FROM [ItemTrans] a
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
		JOIN [MupTrans] mt ON mt.ItemTransId = a.RecId
		JOIN [Mup] m ON m.RecId = mt.MupId
		LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s ON s.ItemId = m.ItemId
		WHERE a.Room = @Room	--1
		GROUP BY DATEFROMPARTS(YEAR(m.MupDate),MONTH(m.MupDate),1), a.ItemId, i.SEARCHNAME, s.VtaMpSubstitusiGroupId
		ORDER BY [MupDate] ASC, VtaMpSubstitusiGroupId, a.ItemId
END

GO

