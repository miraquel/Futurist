-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Analisa RM price plan vs actual>
-- EXEC AnlMaterial_plan_vs_actual 1,68,2025,7
-- =============================================
CREATE PROCEDURE [dbo].[AnlMaterial_plan_vs_actual]
@Room int = 1,
@VerId int = 68,	
@Year int = 2025,
@Month int = 7		

AS
BEGIN
	SELECT a.[Room]
		,a.[VerId]
		,a.[Year]
		,a.[Month]
		,MAX(ISNULL(g.[Group01],'NA')) as [GroupProcurement]
		,MAX(ISNULL(g.[Group02],'NA')) as [GroupCommercial]
		,a.[ItemId]
		,MAX(i.[SearchName]) as [ItemName]
		,MAX(i.[UnitId]) as [UnitId]
		,SUM(a.[PlanQty]) as [PlanQty]
		,SUM(a.[PlanQty]*a.[PlanPrice]) / NULLIF(SUM(a.[PlanQty]),0)  as [PlanPrice]
		,MAX(act.ActQty) as [ActQty]
		,MAX(act.ActPrice) as [ActPrice]
	FROM [MaterialPlan] a
		LEFT JOIN [AXGMKDW].[dbo].[DimItem] i ON i.ITEMID = a.ItemId
		LEFT JOIN [ItemGroup] g ON g.ITEMID = a.ItemId
		LEFT JOIN (
			SELECT ItemId, SUM(ActQty) as [ActQty]
				,SUM(ActQty*ActPrice) / NULLIF(SUM(ActQty),0) as [ActPrice]
			FROM [MaterialAct] 
			WHERE [Year] = @Year
				AND [Month] = @Month
			GROUP BY ItemId
		) act ON act.ITEMID = a.ItemId
	WHERE a.[Room] = @Room AND a.[VerId] = @VerId AND a.[Year] = @Year AND a.[Month] = @Month
	GROUP BY a.[Room]
		,a.[VerId]
		,a.[Year]
		,a.[Month]
		,a.[ItemId]
	ORDER BY a.[ItemId]

	
END

GO

