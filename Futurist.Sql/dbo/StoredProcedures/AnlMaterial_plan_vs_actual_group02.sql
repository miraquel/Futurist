-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Analisa RM price plan vs actual>
-- EXEC AnlMaterial_plan_vs_actual_group02 1,68,2025,7
-- =============================================
CREATE PROCEDURE [dbo].[AnlMaterial_plan_vs_actual_group02]
@Room int = 1,
@VerId int = 68,	
@Year int = 2025,
@Month int = 7		

AS
BEGIN
	DECLARE @Plan table (
		Room int
		,VerId int
		,[Year] int
		,[Month] int
		,[Group02] nvarchar(20)
		,PlanQty numeric(32,16)
		,PlanPrice numeric(32,16)
	) 

	DECLARE @Result table (
		Room int
		,VerId int
		,[Year] int
		,[Month] int
		,[Group02] nvarchar(20)
		,PlanQty numeric(32,16)
		,PlanPrice numeric(32,16)
	) 
	
	DECLARE @Total numeric(32,16)
	
	INSERT INTO @Plan (Room,VerId,[Year],[Month],[Group02],PlanQty,PlanPrice)
		SELECT a.[Room]
			,a.[VerId]
			,a.[Year]
			,a.[Month]
			,ISNULL(g.[Group02],'') as [Group02]
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
			,g.[Group02]
		ORDER BY g.[Group02]

	UPDATE @Plan SET [Group02] = 'NA' WHERE [Group02] = ''

	INSERT INTO @Result(Room,VerId,[Year],[Month],[Group02],PlanQty,PlanPrice)
		SELECT MAX(Room),MAX(VerId),MAX([Year]),MAX([Month]),[Group02]
			,SUM(PlanQty) as PlanQty
			,SUM(PlanQty*PlanPrice) / NULLIF(SUM(PlanQty),0) as PlanPrice
		FROM @Plan
		GROUP BY [Group02]

	SELECT @Total = SUM(b.ActQty*b.ActPrice)
	FROM [MaterialAct] b 
	WHERE b.[Year] = @Year
		AND b.[Month] = @Month

	SELECT MAX(a.Room) as Room, MAX(a.VerId) as VerId
		,MAX(a.[Year]) as [Year]
		,MAX(a.[Month]) as [Month]
		,a.Group02, SUM(a.PlanQty) as PlanQty
		,SUM(a.PlanQty*a.PlanPrice) / NULLIF(SUM(a.PlanQty),0) as PlanPrice
		,MAX(b.ActQty) as ActQty
		,MAX(b.ActPrice) as ActPrice
		,MAX(b.ActPrice)/ (SUM(a.PlanQty*a.PlanPrice) / NULLIF(SUM(a.PlanQty),0)) as [Gap]
		,MAX(b.ActQty) * MAX(b.ActPrice) / @Total as [Contr]
	FROM @Result a
	LEFT JOIN (
			SELECT ISNULL(g.Group02,'NA') as Group02, SUM(b.ActQty) as ActQty
				,SUM(b.ActQty*b.ActPrice) / NULLIF(SUM(b.ActQty),0) as ActPrice
			FROM [MaterialAct] b 
			LEFT JOIN [ItemGroup] g ON g.ITEMID = b.ItemId		
			WHERE b.[Year] = @Year
				AND b.[Month] = @Month
			GROUP BY g.Group02
		) b ON b.Group02 = a.Group02
	GROUP BY a.Group02
	ORDER BY [Contr] DESC
		
END

GO

