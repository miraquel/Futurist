-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Analisa FG price plan vs actual>
-- EXEC AnlFg_plan_vs_actual_group03 1,72,2025,8
--1	68	2025	7
--1	72	2025	8
--1	74	2025	9
--1	76	2025	10
-- =============================================
CREATE PROCEDURE [AnlFg_plan_vs_actual_group03]
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
		,[Group03] nvarchar(20)
		,PlanQty numeric(32,16)
		,PlanPrice numeric(32,16)
	) 

	DECLARE @Actual table (
		[Group03] nvarchar(20)
		,ActQty numeric(32,16)
		,ActPrice numeric(32,16)
	) 
	
	DECLARE @Total numeric(32,16)
	
	INSERT INTO @Plan (Room,VerId,[Year],[Month],[Group03],PlanQty,PlanPrice)
		SELECT a.Room
			,a.VerId
			,YEAR(a.RofoDate)
			,MONTH(a.RofoDate)
			,IIF(ISNULL(g.[Group03],'')='','NA',g.[Group03]) as [Group03]
			,SUM(a.Qty*i.Netweight/1000) as [PlanQty]
			,MAX(a.SalesPrice/(i.Netweight/1000)) as [PlanPrice]
		FROM RofoVer a  WITH (NOLOCK)
			JOIN [AXGMKDW].[dbo].[DimItem] i ON i.ITEMID = a.ItemId
			LEFT JOIN [ItemGroup] g ON g.ITEMID = a.ItemId		
		WHERE a.Room = @Room 
			AND a.VerId = @VerId
			AND YEAR(a.RofoDate) = @Year
			AND MONTH(a.RofoDate) = @Month
		GROUP BY a.[Room]
			,a.[VerId]
			,YEAR(a.RofoDate)
			,MONTH(a.RofoDate)
			,g.[Group03]
		ORDER BY g.[Group03]


	INSERT INTO @Actual ([Group03],ActQty,ActPrice)
		SELECT IIF(ISNULL(g.[Group03],'')='','NA',g.[Group03]) as [Group03]
			,SUM(a.QTY * i.NETWEIGHT/1000) as [QtyInKg]
			,SUM(a.LINEAMOUNTMST) / SUM(a.QTY * i.NETWEIGHT/1000) as [PriceInKg]
		FROM [AXGMKDW].[dbo].[FactSalesInvoice] a
			JOIN [AXGMKDW].[dbo].[DimItem] i ON i.ITEMID = a.ItemId
			LEFT JOIN [ItemGroup] g ON g.ITEMID = a.ItemId		
		WHERE YEAR(INVOICEDATE) = @Year
			AND MONTH(INVOICEDATE) = @Month
		GROUP BY g.[Group03]
		ORDER BY g.[Group03]

	SELECT @Total = SUM(ActQty*ActPrice)
	FROM @Actual 

	SELECT a.Room, a.VerId, a.[Year], a.[Month], a.Group03, a.PlanQty, a.PlanPrice
		,b.ActQty, b.ActPrice
		,(b.ActQty * b.ActPrice) / @Total as [Contr]
	FROM @Plan a
	LEFT JOIN @Actual b ON b.Group03 = a.Group03
	ORDER BY [Contr] DESC


	

	--SELECT MAX(a.Room) as Room, MAX(a.VerId) as VerId
	--	,MAX(a.[Year]) as [Year]
	--	,MAX(a.[Month]) as [Month]
	--	,a.Group03, SUM(a.PlanQty) as PlanQty
	--	,SUM(a.PlanQty*a.PlanPrice) / NULLIF(SUM(a.PlanQty),0) as PlanPrice
	--	,MAX(b.ActQty) as ActQty
	--	,MAX(b.ActPrice) as ActPrice
	--	,MAX(b.ActPrice)/ (SUM(a.PlanQty*a.PlanPrice) / NULLIF(SUM(a.PlanQty),0)) as [Gap]
	--	,MAX(b.ActQty) * MAX(b.ActPrice) / @Total as [Contr]
	--FROM @Plan a
	--LEFT JOIN (
	--		SELECT ISNULL(g.Group01,'NA') as Group01, SUM(b.ActQty) as ActQty
	--			,SUM(b.ActQty*b.ActPrice) / NULLIF(SUM(b.ActQty),0) as ActPrice
	--		FROM [MaterialAct] b 
	--		LEFT JOIN [ItemGroup] g ON g.ITEMID = b.ItemId		
	--		WHERE b.[Year] = @Year
	--			AND b.[Month] = @Month
	--		GROUP BY g.Group01
	--	) b ON b.Group01 = a.Group01
	--GROUP BY a.Group01
	--ORDER BY [Contr] DESC
		
END

GO

