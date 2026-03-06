-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Analisa RM price plan vs actual>
-- EXEC AnlRmPrice_plan_vs_actual 1,66,2025,6
-- =============================================
CREATE PROCEDURE [dbo].[AnlRmPrice_plan_vs_actual_bck]
@Room int = 1,
@Version int = 66,	--68
@Year int = 2025,
@Month int = 6		--7
AS
BEGIN
	SELECT 
		a.Room
		,a.VerId
		,YEAR(a.RofoDate) as [Year]
		,MONTH(a.RofoDate) as [Month]
		,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
		,isnull(g.[GroupName],'') as [GroupProcurement]
		,d.ItemId as ItemAllocatedId
		,REPLACE(REPLACE(REPLACE(id.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemAllocatedName
		,id.UnitId
		,SUM(d.Qty) as [PlanQty]
		,SUM(d.Qty*d.Price) as [PlanValue]
		,SUM(d.Qty*d.Price) / NULLIF(SUM(d.Qty),0) as [PlanPrice]
		,MAX(ac.QTY) as [ActQty]
		,MAX(ac.[COSTVALUE]) as [ActValue]
		,MAX(ac.[COSTVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActPrice]

	FROM RofoVer a  WITH (NOLOCK)
	JOIN MupVer b WITH (NOLOCK) ON b.RofoId = a.RecId AND b.VerId = a.VerId
	JOIN MupTransVer c WITH (NOLOCK) ON c.MupId = b.RecId AND c.VerId = b.VerId
	JOIN ItemTransVer d WITH (NOLOCK) ON d.RecId = c.ItemTransId AND d.VerId = c.VerId
	LEFT JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
	LEFT JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	LEFT JOIN [DimItemSubstituteVer] s WITH (NOLOCK) ON s.ItemId = b.ItemId AND s.VerId = b.VerId
	LEFT JOIN ItemPagVer p WITH (NOLOCK) ON p.RecId = d.RefId 
		AND p.Room = a.Room AND d.[Source] = 'Contract'
		AND p.VerId = a.VerId
	LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = d.ItemId
	LEFT JOIN (
			SELECT ITEMID
				,SUM(-1*[QTY]) AS [QTY]
				,SUM(-1*[COSTVALUE]) AS [COSTVALUE]
				,SUM([COSTVALUE]) / SUM([QTY]) AS [PRICE]
			FROM [AXGMKDW].[dbo].[FactInventTrans]
			WHERE [REFERENCECATEGORY] = 'PRODUCTION LINE'
				AND YEAR(DATEPHYSICAL) = 2025
				AND MONTH(DATEPHYSICAL) = 6
				AND ITEMID LIKE '1%'
			GROUP BY ITEMID
		) ac ON ac.ITEMID = d.ItemId
	WHERE a.Room = 1	--1	--@Room 
		AND a.VerId = 66	--66	--@VerId
		AND YEAR(a.RofoDate) = 2025
		AND MONTH(a.RofoDate) = 6
		AND d.ItemId LIKE '1%'
	GROUP BY a.Room
		,a.VerId
		,YEAR(a.RofoDate) 
		,MONTH(a.RofoDate) 
		,isnull(s.VtaMpSubstitusiGroupId,'') 
		,isnull(g.[GroupName],'') 
		,d.ItemId
		,id.SEARCHNAME
		,id.UnitId
	ORDER BY [PlanValue] DESC
END

GO

