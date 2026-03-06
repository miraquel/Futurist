-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Analisa FG price plan vs actual>
-- EXEC [AnlFgPrice_plan_vs_actual_new] 1,72,2025,8
-- =============================================
CREATE PROCEDURE [dbo].[AnlFgPrice_plan_vs_actual]
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
		,PlanCostValue numeric(32,16)
		,PlanCostPrice numeric(32,16)
		,PlanRmPrice numeric(32,16)
		,PlanPmPrice numeric(32,16)
		,PlanStdCostPrice numeric(32,16)
		,ActQty numeric(32,16)
		,ActCostValue numeric(32,16)
		,ActCostPrice numeric(32,16)
		,ActCostPriceLm numeric(32,16)
		,ActRmPrice numeric(32,16)
		,ActPmPrice numeric(32,16)
		,ActStdCostPrice numeric(32,16)
		,ActStdCostPriceLm numeric(32,16)
		,AvgValue numeric(32,16)
		,PlanNetSalesPriceIndex numeric(32,16)
		,ActNetSalesPrice numeric(32,16)
		,ActNetSalesPriceLm numeric(32,16)
		,ActGrossSalesPrice numeric(32,16)
		,ActGrossSalesPriceLm numeric(32,16)
		,[ActDisc%] numeric(32,16)
		,[ActDisc%Lm] numeric(32,16)
	) 

	INSERT INTO @Result (
	Room,VerId,[Year],[Month],ItemId,ItemName,UnitId
	,PlanQty,PlanCostValue,PlanCostPrice	
	,PlanRmPrice,PlanPmPrice,PlanStdCostPrice	
	,ActQty,ActCostValue,ActCostPrice,ActCostPriceLm
	,ActRmPrice,ActPmPrice,ActStdCostPrice
	,ActStdCostPriceLm
	,PlanNetSalesPriceIndex, ActNetSalesPrice
	,ActNetSalesPriceLm
	,ActGrossSalesPrice,ActGrossSalesPriceLm
	,[ActDisc%],[ActDisc%Lm]
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
		,SUM(b.[QtyRofo]*b.[CostPrice]) as [PlanCostValue]
		,SUM(b.[QtyRofo]*b.[CostPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanCostPrice]

		,SUM(b.[QtyRofo]*b.[RmPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanRmPrice]
		,SUM(b.[QtyRofo]*b.[PmPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanPmPrice]
		,SUM(b.[QtyRofo]*b.[StdCostPrice]) / NULLIF(SUM(b.[QtyRofo]),0) as [PlanStdCostPrice]

		,MAX(ac.QTY) as [ActQty]
		,MAX(ac.[COSTVALUE]) as [ActCostValue]
		,MAX(ac.[COSTVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActCostPrice]
		,MAX(aclm.[COSTVALUE]) / NULLIF(MAX(aclm.QTY),0) as [ActCostPriceLm]
		
		,MAX(ac.[RMVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActRmPrice]
		,MAX(ac.[PMVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActPmPrice]
		,MAX(ac.[STDCOSTVALUE]) / NULLIF(MAX(ac.QTY),0) as [ActStdCostPrice]
		,MAX(aclm.[STDCOSTVALUE]) / NULLIF(MAX(aclm.QTY),0) as [ActStdCostPriceLm]

		,MAX(p.SalesPriceIndex) as [PlanNetSalesPriceIndex]
		,MAX(ac.[NETPRICE]) as [ActNetSalesPrice]
		,MAX(aclm.[NETPRICE]) as [ActNetSalesPriceLastMonth]
		,MAX(ac.[GROSSPRICE]) as [ActGrossSalesPrice]
		,MAX(aclm.[GROSSPRICE]) as [ActGrossSalesPriceLm]
		,MAX(ac.[DISC%]) as [ActDisc%]
		,MAX(aclm.[DISC%]) as [ActDiscLm%]
		
	FROM RofoVer a  WITH (NOLOCK)
	LEFT JOIN AXGMKDW.dbo.DimItem i  with(nolock) ON i.ITEMID = a.ItemId
	LEFT JOIN [FgCostVer] b ON b.ProductId = a.[ItemId] AND b.[RofoDate] = a.[RofoDate] AND b.VerId = @VerId	
	LEFT JOIN (
			SELECT [ITEMID]
				,SUM([QTY] * ([SALESPRICE]*[EXCHRATE]/100)) / NULLIF(SUM([QTY]),0) as [GROSSPRICE]
				,SUM([LINEAMOUNTMST]) / NULLIF(SUM([QTY]),0) as [NETPRICE]
				,1-(SUM([LINEAMOUNTMST]) / NULLIF(SUM([QTY]),0)) / ((SUM([QTY] * ([SALESPRICE]*[EXCHRATE]/100)) / NULLIF(SUM([QTY]),0))) as [DISC%]
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

	LEFT JOIN (
			SELECT [ITEMID]
				,SUM([QTY] * ([SALESPRICE]*[EXCHRATE]/100)) / NULLIF(SUM([QTY]),0) as [GROSSPRICE]
				,SUM([LINEAMOUNTMST]) / NULLIF(SUM([QTY]),0) as [NETPRICE]
				,1-(SUM([LINEAMOUNTMST]) / NULLIF(SUM([QTY]),0)) / ((SUM([QTY] * ([SALESPRICE]*[EXCHRATE]/100)) / NULLIF(SUM([QTY]),0))) as [DISC%]
				,SUM([QTY]) as [QTY]
				,SUM([RMPM]) as [RMPMVALUE]
				,SUM([STDCOST]) as [STDCOSTVALUE]
				,SUM([RMPM]+[STDCOST]) as [COSTVALUE]
				,SUM([RM]) as [RMVALUE]
				,SUM([PM]) as [PMVALUE]
				,SUM([LineAmountMst]) as [SALESVALUE]
			FROM [AXGMKDW].[dbo].[FactSalesInvoice]
			WHERE YEAR(INVOICEDATE) = YEAR(DATEADD(DAY,-1, DATEFROMPARTS(@Year,@Month,1)))
				AND MONTH(INVOICEDATE) = MONTH(DATEADD(DAY,-1, DATEFROMPARTS(@Year,@Month,1)))
			GROUP BY ITEMID
		) aclm ON aclm.ITEMID = a.ItemId	--Actual Last Month

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

	
	DECLARE @ActCostValueTot numeric(32,16)
	SELECT @ActCostValueTot = SUM(ActCostValue)	FROM @Result 

	SELECT Room,VerId,[Year],[Month],ItemId,ItemName,UnitId
		,PlanQty,PlanCostValue,PlanCostPrice
		,PlanRmPrice,PlanPmPrice,PlanStdCostPrice		
		,ActQty,ActCostValue,ActCostPrice,ActCostPriceLm	
		,ActRmPrice,ActPmPrice,ActStdCostPrice
		,ActStdCostPriceLm
		,ActCostValue/@ActCostValueTot as [Cont]
		,ActCostPrice/PlanCostPrice as [A/P]
		,PlanNetSalesPriceIndex
		,ActNetSalesPrice
		,ActNetSalesPriceLm
		,[ActGrossSalesPrice]
		,[ActGrossSalesPriceLm]
		,[ActDisc%]
		,[ActDisc%Lm]
	FROM @Result
	ORDER BY [ActCostValue] DESC  
END

GO

