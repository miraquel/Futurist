-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Analisa RM price plan vs actual>
-- EXEC AnlRmPriceGroup_plan_vs_actual 1,68,2025,7
-- =============================================
CREATE PROCEDURE [dbo].[AnlRmPriceGroup_plan_vs_actual]
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
		,GroupSubstitusi nvarchar(20)
		,UnitId nvarchar(20)
		,PlanQty numeric(32,16)
		,PlanValue numeric(32,16)
		,PlanPrice numeric(32,16)
		,ActQty numeric(32,16)
		,ActValue numeric(32,16)
		,ActPrice numeric(32,16)
		,AvgValue numeric(32,16)
	) 

	INSERT INTO @Result (
	Room,VerId,[Year],[Month],GroupSubstitusi
		,UnitId,PlanQty,PlanValue,PlanPrice,ActQty,ActValue,ActPrice
	)
	SELECT 
		a.Room
		,a.VerId
		,YEAR(a.RofoDate) as [Year]
		,MONTH(a.RofoDate) as [Month]
		,isnull(s.VtaMpSubstitusiGroupId,'N/A') as [GroupSubstitusi]
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
	LEFT JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	LEFT JOIN [DimItemSubstituteVer] s WITH (NOLOCK) ON s.ItemId = b.ItemId AND s.VerId = b.VerId
	LEFT JOIN (
			SELECT ITEMID
				,SUM(-1*[QTY]) AS [QTY]
				,SUM(-1*[COSTVALUE]) AS [COSTVALUE]
				,SUM([COSTVALUE]) / NULLIF(SUM([QTY]),0) AS [PRICE]
			FROM [AXGMKDW].[dbo].[FactInventTrans]
			WHERE [REFERENCECATEGORY] = 'PRODUCTION LINE'
				AND YEAR(DATEPHYSICAL) = @Year
				AND MONTH(DATEPHYSICAL) = @Month
				AND ITEMID LIKE '1%'
			GROUP BY ITEMID
		) ac ON ac.ITEMID = d.ItemId
	WHERE a.Room = @Room 
		AND a.VerId = @VerId
		AND YEAR(a.RofoDate) = @Year
		AND MONTH(a.RofoDate) = @Month
		AND d.ItemId LIKE '1%'
	GROUP BY a.Room
		,a.VerId
		,YEAR(a.RofoDate) 
		,MONTH(a.RofoDate) 
		,s.VtaMpSubstitusiGroupId
		,id.UnitId

	
	
	DECLARE @ActValueTot numeric(32,16)
	SELECT @ActValueTot = SUM(ActValue)	FROM @Result 

	SELECT Room,VerId,[Year],[Month],GroupSubstitusi
		,UnitId,PlanQty,PlanValue,PlanPrice,ActQty,ActValue,ActPrice
		,ActValue/NULLIF(@ActValueTot,0) as [Cont]
		,ActPrice/NULLIF(PlanPrice,0) as [A/P]
	FROM @Result
	ORDER BY [ActValue] DESC  
END

GO

