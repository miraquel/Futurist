-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Analisa FG price plan vs actual>
-- EXEC [AnlFgPrice_plan_vs_actual] 1,72,2025,8
-- =============================================
CREATE PROCEDURE [dbo].[AnlFgPrice_plan_vs_actual_bck]
@Room int = 1,
@VerId int = 66,	--68
@Year int = 2025,
@Month int = 6		--7
AS
BEGIN
	DECLARE @Result table (
		Room int
		,VerId int
		,[Year] int
		,[Month] int
		,ItemId nvarchar(20)
		,ItemName nvarchar(60)
		,UnitId nvarchar(20)
		,PlanQty numeric(32,16)
		,PlanValue numeric(32,16)
		,PlanPrice numeric(32,16)
		,PlanRmPrice numeric(32,16)
		,PlanPmPrice numeric(32,16)
		,PlanStdCostPrice numeric(32,16)
		,ActQty numeric(32,16)
		,ActValue numeric(32,16)
		,ActPrice numeric(32,16)
		,ActRmPrice numeric(32,16)
		,ActPmPrice numeric(32,16)
		,ActStdCostPrice numeric(32,16)
		,AvgValue numeric(32,16)
		,PlanSalesPriceIndex numeric(32,16)
		,ActSalesPrice numeric(32,16)
	) 

	INSERT INTO @Result (
	Room,VerId,[Year],[Month],ItemId,ItemName,UnitId
	,PlanQty,PlanValue,PlanPrice	
	,PlanRmPrice,PlanPmPrice,PlanStdCostPrice	
	,ActQty,ActValue,ActPrice
	,ActRmPrice,ActPmPrice,ActStdCostPrice
	,PlanSalesPriceIndex, ActSalesPrice
	)
	SELECT 
		a.Room
		,a.VerId
		,YEAR(a.RofoDate) as [Year]
		,MONTH(a.RofoDate) as [Month]
		,a.ItemId as ItemId
		,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemName
		,i.UnitId
		,SUM(b.[QtyRofo]) as [PlanQty]
		,SUM(b.[QtyRofo]*b.[CostPrice]) as [PlanValue]
		,SUM(b.[QtyRofo]*b.[CostPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanPrice]

		,SUM(b.[QtyRofo]*b.[RmPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanRmPrice]
		,SUM(b.[QtyRofo]*b.[PmPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanPmPrice]
		,SUM(b.[QtyRofo]*b.[StdCostPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanStdCostPrice]

		,MAX(ac.QTY) as [ActQty]
		,MAX(ac.[COSTVALUE]) as [ActValue]
		,MAX(ac.[COSTVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActCostPrice]
		
		,MAX(ac.[RMVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActRmPrice]
		,MAX(ac.[PMVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActPmPrice]
		,MAX(ac.[STDCOSTVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActStdCostPrice]

		,MAX(p.SalesPriceIndex) as [SalesPriceIndex]
		,MAX(ac.[SALESVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActSalesPrice]
	FROM RofoVer a  WITH (NOLOCK)
	LEFT JOIN AXGMKDW.dbo.DimItem i  with(nolock) ON i.ITEMID = a.ItemId
	LEFT JOIN [FgCostVer] b ON b.ProductId = a.[ItemId] AND b.[RofoDate] = a.[RofoDate] AND b.VerId = @VerId	
	LEFT JOIN (
			SELECT [ITEMID]
				,SUM([QTY]) as [QTY]
				,SUM([RMPM]) as [RMPMVALUE]
				,SUM([STDCOST]) as [STDCOSTVALUE]
				,SUM([RMPM]+[STDCOST]) as [COSTVALUE]
				,SUM([RM]) as [RMVALUE]
				,SUM([PM]) as [PMVALUE]
				,SUM([LineAmountMst]) as [SALESVALUE]
			FROM [AXGMKDW].[dbo].[FactSalesInvoice]
			WHERE YEAR(INVOICEDATE) = @Year	--2025
				AND MONTH(INVOICEDATE) = @Month	--6
			GROUP BY ITEMID
		) ac ON ac.ITEMID = a.ItemId
	LEFT JOIN [SalesPriceVer] p ON p.VerId = @VerId AND p.ItemId = a.ItemId
	WHERE a.Room = @Room	--1	 
		AND a.VerId = @VerId	--66	
		AND YEAR(a.RofoDate) = @Year	--2025
		AND MONTH(a.RofoDate) = @Month	--6
		AND a.ItemId LIKE '4%'
	GROUP BY a.Room
		,a.VerId
		,YEAR(a.RofoDate) 
		,MONTH(a.RofoDate) 
		,a.ItemId
		,i.SEARCHNAME
		,i.UnitId


	--UNION

	--SELECT 
	--	@Room as Room
	--	,@VerId as VerId
	--	,@Year as [Year]
	--	,@Month as [Month]
	--	,ac.ItemId as ItemId
	--	,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemName
	--	,i.UnitId		
	--	,p.[PlanQty]
	--	,p.[PlanValue]
	--	,p.[PlanPrice]

	--	,p.[PlanRmPrice]
	--	,p.[PlanPmPrice]
	--	,p.[PlanStdCostPrice]

	--	,ac.QTY as [ActQty]
	--	,ac.[COSTVALUE] as [ActValue]
	--	,ac.[COSTVALUE] / NULLIF(ac.QTY,0) as [ActPrice]
				
	--	,ac.[RMVALUE] / NULLIF(ac.QTY,0) as [ActRmPrice]
	--	,ac.[PMVALUE] / NULLIF(ac.QTY,0) as [ActPmPrice]
	--	,ac.[STDCOSTVALUE] / NULLIF(ac.QTY,0) as [ActStdCostPrice]

	--FROM (
	--		SELECT [ITEMID]
	--			,SUM([QTY]) as [QTY]
	--			,SUM([RMPM]) as [RMPMVALUE]
	--			,SUM([STDCOST]) as [STDCOSTVALUE]
	--			,SUM([RMPM]+[STDCOST]) as [COSTVALUE]
	--			,SUM([RM]) as [RMVALUE]
	--			,SUM([PM]) as [PMVALUE]
	--		FROM [AXGMKDW].[dbo].[FactSalesInvoice]
	--		WHERE YEAR(INVOICEDATE) = @Year
	--			AND MONTH(INVOICEDATE) = @Month
	--		GROUP BY ITEMID
	--	) ac 
		
	--LEFT JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = ac.ItemId
	--LEFT JOIN
	--(
	--	SELECT
	--		a.ItemId as ItemId			
	--		,SUM(b.[QtyRofo]) as [PlanQty]
	--		,SUM(b.[QtyRofo]*b.[CostPrice]) as [PlanValue]
	--		,SUM(b.[QtyRofo]*b.[CostPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanPrice]
	--		,SUM(b.[QtyRofo]*b.[RmPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanRmPrice]
	--		,SUM(b.[QtyRofo]*b.[PmPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanPmPrice]
	--		,SUM(b.[QtyRofo]*b.[StdCostPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanStdCostPrice]
	--	FROM RofoVer a  WITH (NOLOCK)
	--		LEFT JOIN AXGMKDW.dbo.DimItem i  with(nolock) ON i.ITEMID = a.ItemId
	--		LEFT JOIN [FgCostVer] b ON b.ProductId = a.[ItemId] AND b.[RofoDate] = a.[RofoDate] AND b.VerId = @VerId
	--	GROUP BY a.ItemId
		
	--) p ON p.ItemId = ac.ITEMID
	
	DECLARE @ActValueTot numeric(32,16)
	SELECT @ActValueTot = SUM(ActValue)	FROM @Result 

	SELECT Room,VerId,[Year],[Month],ItemId,ItemName,UnitId
		,PlanQty,PlanValue,PlanPrice
		,PlanRmPrice,PlanPmPrice,PlanStdCostPrice		
		,ActQty,ActValue,ActPrice	
		,ActRmPrice,ActPmPrice,ActStdCostPrice
		,ActValue/@ActValueTot as [Cont]
		,ActPrice/PlanPrice as [A/P]
		,PlanSalesPriceIndex
		,ActSalesPrice
	FROM @Result
	ORDER BY [ActValue] DESC  
END

GO

