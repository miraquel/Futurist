-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Analisa RM price plan vs actual>
-- EXEC AnlMaterial_plan_vs_actual_Group01 1,68,2025,7
-- =============================================
CREATE PROCEDURE [dbo].[AnlMaterial_plan_vs_actual_group01]
@Room int = 1,
@VerId int = 68,	
@Year int = 2025,
@Month int = 7		

AS
BEGIN
	DECLARE @Result table (
		Room int
		,VerId int
		,[Year] int
		,[Month] int
		,[Group01] nvarchar(20)
		,PlanQty numeric(32,16)
		,PlanPrice numeric(32,16)
	) 
	
	DECLARE @Total numeric(32,16)
	
	INSERT INTO @Result (Room,VerId,[Year],[Month],[Group01],PlanQty,PlanPrice)
		SELECT a.[Room]
			,a.[VerId]
			,a.[Year]
			,a.[Month]
			,IIF(ISNULL(g.[Group01],'')='','NA',g.[Group01]) as [Group01]
			,SUM(a.[PlanQty]) as [PlanQty]
			,SUM(a.[PlanQty]*a.[PlanPrice]) / NULLIF(SUM(a.[PlanQty]),0)  as [PlanPrice]
		FROM [MaterialPlan] a
			LEFT JOIN [AXGMKDW].[dbo].[DimItem] i ON i.ITEMID = a.ItemId
			LEFT JOIN [ItemGroup] g ON g.ITEMID = a.ItemId		
		WHERE a.[Room] = @Room AND a.[VerId] = @VerId AND a.[Year] = @Year AND a.[Month] = @Month
		GROUP BY a.[Room]
			,a.[VerId]
			,a.[Year]
			,a.[Month]
			,g.[Group01]
		ORDER BY g.[Group01]


	SELECT @Total = SUM(b.ActQty*b.ActPrice)
	FROM [MaterialAct] b 
	WHERE b.[Year] = @Year
		AND b.[Month] = @Month

	SELECT MAX(a.Room) as Room, MAX(a.VerId) as VerId
		,MAX(a.[Year]) as [Year]
		,MAX(a.[Month]) as [Month]
		,a.Group01, SUM(a.PlanQty) as PlanQty
		,SUM(a.PlanQty*a.PlanPrice) / NULLIF(SUM(a.PlanQty),0) as PlanPrice
		,MAX(b.ActQty) as ActQty
		,MAX(b.ActPrice) as ActPrice
		,MAX(b.ActPrice)/ (SUM(a.PlanQty*a.PlanPrice) / NULLIF(SUM(a.PlanQty),0)) as [Gap]
		,MAX(b.ActQty) * MAX(b.ActPrice) / @Total as [Contr]
	FROM @Result a
	LEFT JOIN (
			SELECT ISNULL(g.Group01,'NA') as Group01, SUM(b.ActQty) as ActQty
				,SUM(b.ActQty*b.ActPrice) / NULLIF(SUM(b.ActQty),0) as ActPrice
			FROM [MaterialAct] b 
			LEFT JOIN [ItemGroup] g ON g.ITEMID = b.ItemId		
			WHERE b.[Year] = @Year
				AND b.[Month] = @Month
			GROUP BY g.Group01
		) b ON b.Group01 = a.Group01
	GROUP BY a.Group01
	ORDER BY [Contr] DESC
		
END

GO

