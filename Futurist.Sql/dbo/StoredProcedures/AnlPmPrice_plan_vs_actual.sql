-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Analisa PM price plan vs actual>
-- EXEC AnlRmPrice_plan_vs_actual 1,66,2025,6
-- =============================================
CREATE PROCEDURE [dbo].[AnlPmPrice_plan_vs_actual]
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
		,GroupProcurement nvarchar(20)
		,ItemId nvarchar(20)
		,ItemName nvarchar(60)
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
	Room,VerId,[Year],[Month],GroupSubstitusi,GroupProcurement,ItemId,ItemName
		,UnitId,PlanQty,PlanValue,PlanPrice,ActQty,ActValue,ActPrice
	)
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
	LEFT JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	LEFT JOIN [DimItemSubstituteVer] s WITH (NOLOCK) ON s.ItemId = b.ItemId AND s.VerId = b.VerId
	LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = d.ItemId
	LEFT JOIN (
			SELECT ITEMID
				,SUM(-1*[QTY]) AS [QTY]
				,SUM(-1*[COSTVALUE]) AS [COSTVALUE]
				,SUM([COSTVALUE]) / NULLIF(SUM([QTY]),0) AS [PRICE]
			FROM [AXGMKDW].[dbo].[FactInventTrans]
			WHERE [REFERENCECATEGORY] = 'PRODUCTION LINE'
				AND YEAR(DATEPHYSICAL) = @Year
				AND MONTH(DATEPHYSICAL) = @Month
				AND ITEMID LIKE '3%'
			GROUP BY ITEMID
		) ac ON ac.ITEMID = d.ItemId
	WHERE a.Room = @Room 
		AND a.VerId = @VerId
		AND YEAR(a.RofoDate) = @Year
		AND MONTH(a.RofoDate) = @Month
		AND d.ItemId LIKE '3%'
	GROUP BY a.Room
		,a.VerId
		,YEAR(a.RofoDate) 
		,MONTH(a.RofoDate) 
		,isnull(s.VtaMpSubstitusiGroupId,'') 
		,isnull(g.[GroupName],'') 
		,d.ItemId
		,id.SEARCHNAME
		,id.UnitId

	UNION

	SELECT 
		@Room as Room
		,@VerId as VerId
		,@Year as [Year]
		,@Month as [Month]
		,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
		,isnull(g.[GroupName],'') as [GroupProcurement]
		,ac.ItemId as ItemAllocatedId
		,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemAllocatedName
		,i.UnitId
		,p.[PlanQty]
		,p.[PlanValue]
		,p.[PlanPrice]
		,ac.QTY as [ActQty]
		,ac.[COSTVALUE] as [ActValue]
		,ac.[COSTVALUE] / NULLIF(ac.QTY,0) as [ActPrice]

	FROM (
			SELECT ITEMID
				,SUM(-1*[QTY]) AS [QTY]
				,SUM(-1*[COSTVALUE]) AS [COSTVALUE]
				,SUM([COSTVALUE]) / NULLIF(SUM([QTY]),0) AS [PRICE]
			FROM [AXGMKDW].[dbo].[FactInventTrans]
			WHERE [REFERENCECATEGORY] = 'PRODUCTION LINE'
				AND YEAR(DATEPHYSICAL) = @Year
				AND MONTH(DATEPHYSICAL) = @Month
				AND ITEMID LIKE '3%'
			GROUP BY ITEMID
		) ac 
		
	LEFT JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = ac.ItemId
	LEFT JOIN [DimItemSubstituteVer] s WITH (NOLOCK) ON s.ItemId = ac.ItemId AND s.VerId = @VerId
	LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = ac.ItemId
	LEFT JOIN
	(
		SELECT 
			d.ItemId as ItemAllocatedId
			,SUM(d.Qty) as [PlanQty]
			,SUM(d.Qty*d.Price) as [PlanValue]
			,SUM(d.Qty*d.Price) / NULLIF(SUM(d.Qty),0) as [PlanPrice]			
		FROM RofoVer a  WITH (NOLOCK)
			JOIN MupVer b WITH (NOLOCK) ON b.RofoId = a.RecId AND b.VerId = a.VerId
			JOIN MupTransVer c WITH (NOLOCK) ON c.MupId = b.RecId AND c.VerId = b.VerId
			JOIN ItemTransVer d WITH (NOLOCK) ON d.RecId = c.ItemTransId AND d.VerId = c.VerId
		WHERE a.Room = @Room 
			AND a.VerId = @VerId
			AND YEAR(a.RofoDate) = @Year
			AND MONTH(a.RofoDate) = @Month
			AND d.ItemId LIKE '3%'
		GROUP BY d.ItemId
	) p ON p.ItemAllocatedId = ac.ITEMID
	
	DECLARE @ActValueTot numeric(32,16)
	SELECT @ActValueTot = SUM(ActValue)	FROM @Result 

	SELECT Room,VerId,[Year],[Month],GroupSubstitusi,GroupProcurement,ItemId,ItemName
		,UnitId,PlanQty,PlanValue,PlanPrice,ActQty,ActValue,ActPrice
		,ActValue/NULLIF(@ActValueTot,0) as [Cont]
		,ActPrice/NULLIF(PlanPrice,0) as [A/P]
	FROM @Result
	ORDER BY [ActValue] DESC  
END

GO

