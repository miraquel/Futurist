-- =============================================
-- Author:		<Putri>
-- Create date: <Jan 2026>
-- Description:	<Summary FG Plan vs Act Calc>
-- =============================================
CREATE PROCEDURE [dbo].[FgPlanCostPrice_Insert]

@Room int = 1
--, @ProductID nvarchar(20) = 4200031
, @Year int = 2025
, @Month int = 11
, @VerId int = 81

AS
BEGIN

--1. Proses untuk yang detail nya terlebih dahulu--

--Delete table detail--
DELETE FROM CogsProjection.dbo.BomStdVsAct_Det 
WHERE Room = @Room 
AND VerId = @VerId 
AND Tahun = @Year
AND Bulan = @Month

DECLARE @BomPlan as Table (		VerId Int
								, Room int
								, Tahun int
								, Bulan int
								, ProductIDPlan nvarchar(20)
								, ProductNamePlan nvarchar(60)
								, ItemIdPlan nvarchar(20)
								, ItemNamePlan nvarchar(60)
								, UnitIdPlan nvarchar(20)
								, GroupSubstitusiPlan nvarchar(20)
								, QtyPlan numeric(32,16)
								, QtyInKgPlan numeric(32,16)
								, PricePlan numeric(32,16)
								,[SourcePlan] nvarchar(20)
								, SalesQtyPlan numeric(32,16)
								, InventBatchIdPlan nvarchar(20)
								, Yield numeric(32,16)
							)

DECLARE @BomAct as Table (
								ProductIDAct nvarchar(20)
								, ProductNameAct nvarchar(60)
								, ItemIdAct nvarchar(20)
								, ItemNameAct nvarchar(60)
								, UnitIdAct nvarchar(20)
								, GroupSubstitusiAct nvarchar(20)
								, QtyAct numeric(32,16)
								, QtyInKgAct numeric(32,16)
								, PriceAct numeric(32,16)
								,[SourceAct] nvarchar(20)
								, SalesQtyAct numeric(32,16)
								, InventBatchIdAct nvarchar(20)
								, Yield numeric(32,16)
								, ProdId nvarchar(20)
							)

--Insert ke table temp plan--
INSERT INTO @BomPlan
SELECT a.VerId
		, a.Room
		, @Year
		, @Month
		, a.ItemId as ProductID
		, a.ItemName as ProductName
		--, a.Qty as PlanQtyFG
		, b.ItemId
		, c.SEARCHNAME as ItemName
		, c.UNITID as UnitIdPlan
		, ISNULL(d.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
		, b.BomQty 
		, CASE 
			WHEN c.UNITID = 'g' THEN b.BomQty * c.NETWEIGHT / 1000
			ELSE b.BomQty
		 END AS BomQtyInKg
		, e.PlanPrice as PricePlan
		,'Plan'
		, 0
		, ''
		, f.Yield
FROM CogsProjection.dbo.RofoVer a
JOIN CogsProjection.dbo.BomStdRoomVer b ON b.ProductId = a.ItemId AND a.RofoDate BETWEEN b.FromDate AND b.ToDate
LEFT JOIN AXGMKDW.dbo.DimItem di ON di.ITEMID = a.ItemId
JOIN AXGMKDW.dbo.DimItem c ON c.ITEMID = b.ItemId
LEFT JOIN AXGMKDW.dbo.DimItemSubstitute d ON d.ItemId = b.ItemId
JOIN ( SELECT Room
			, VerId
			, [Year]
			, [Month]
			, [ItemId]
			, UnitId
			, SUM(PlanQty) as PlanQty
			, SUM([PlanQty]*[PlanPrice]) / NULLIF(SUM([PlanQty]),0)  as [PlanPrice]
	   FROM CogsProjection.dbo.MaterialPlan
	   WHERE Room = @Room
	   and VerId = @VerId
	   and Year = @Year
	   and Month = @Month
	   GROUP BY Room
				, VerId
				, Year
				, Month
				, ItemId
				, UnitId
) e On e.Room = a.Room AND e.VerId = a.VerId AND e.ItemId = b.ItemId AND e.Year = YEAR(a.RofoDate) AND e.Month = MONTH(a.RofoDate)
LEFT JOIN CogsProjection.dbo.FgCostVer f ON f.RofoId = a.RecId AND f.Room = a.Room AND f.VerId = a.VerId AND f.RofoDate = a.RofoDate
WHERE a.Room = @Room
AND a.VerId = @VerId
--AND a.ItemId = '4200031'
AND YEAR(a.RofoDate) = @Year
AND MONTH(a.RofoDate) = @Month
AND b.Room = @Room
AND b.VerId = @VerId
--AND b.ProductId in ( '4200031', '4200032', '4200482')
--AND b.ProductId = '4200031'
GROUP BY a.VerId
		, a.Room
		, c.NETWEIGHT
		, a.ItemId
		, a.ItemName
		--, a.Qty as PlanQtyFG
		, b.ItemId
		, c.SEARCHNAME
		, c.UNITID
		, d.VtaMpSubstitusiGroupId
		, b.BomQty 
		, e.PlanPrice
		, f.Yield
ORDER BY b.ItemId asc

--Insert ke table temp act--
INSERT INTO @BomAct (ProductIDAct
								, ProductNameAct 
								, ItemIdAct 
								, ItemNameAct 
								, UnitIdAct 
								, GroupSubstitusiAct
								, QtyAct 
								, QtyInKgAct 
								, PriceAct 
								,[SourceAct] 
								, SalesQtyAct
								, InventBatchIdAct
								, Yield
								, ProdId
								)
SELECT a.ProductId
		, b.SEARCHNAME as ProductName
		, a.ItemId
		, c.SEARCHNAME as ItemName
		, c.UNITID
		, d.VtaMpSubstitusiGroupId as GroupSubstitusi
		, a.ActQty as ActQty
		, a.ActQty * c.NETWEIGHT / 1000 as QtyInKg
		, a.ActPrice
		, 'Act' as [Source]
		, a.SalesQty
		, a.InventBatchId
		, 0
		, a.ProdId
FROM CogsProjection.dbo.MaterialAct a
LEFT JOIN AXGMKDW.dbo.DimItem b On b.ITEMID = a.ProductId
LEFT JOIN AXGMKDW.dbo.DimItem c On c.ITEMID = a.ItemId
LEFT JOIN AXGMKDW.dbo.DimItemSubstitute d On d.ItemId = a.ItemId

WHERE [Year] = @Year
AND [Month] = @Month
--AND a.ProductId in ( '4200031', '4200032', '4200482')
--AND a.ProductId = '4200031'

--Update tabel temp act dengan menghitung jumlah por nya--
DECLARE @CountPor table (ProductId nvarchar(20)
						, TotalPor numeric(32,16)
						)
INSERT INTO @CountPor 
SELECT a.ProductIDAct
		--, a.ProdId 
		, COUNT(DISTINCT(a.ProdId)) as TotalPor
FROM @BomAct a
GROUP BY a.ProductIDAct

DECLARE @ActPor table ( ProductIDAct nvarchar(20)
						, ProductNameAct nvarchar(60)
						, ItemIdAct nvarchar(20)
						, ItemNameAct nvarchar(60)
						, UnitIdAct nvarchar(20)
						, GroupsubstituteAct nvarchar(20)
						, QtyAct numeric(32,16)
						, QtyInKgAct numeric(32,16)
						, PriceAct numeric(32,16)
						, ValueAct numeric(32,16)
						, SourceAct nvarchar(20)
					
						)
INSERT INTO @ActPor
SELECT a.ProductIDAct
		, a.ProductNameAct
		, a.ItemIdAct
		, a.ItemNameAct
		, a.UnitIdAct
		, a.GroupSubstitusiAct
		, SUM(a.QtyAct)  as QtyAct
		, SUM(a.QtyInKgAct)  as QtyInKgAct
		, SUM(a.QtyAct * a.PriceAct) / NULLIF(SUM(a.QtyAct),0) as ActPrice
		, 0
		, 'Act'
		--, a.SalesQtyAct
FROM @BomAct a
GROUP BY a.ProductIDAct
		, a.ProductNameAct
		, a.ItemIdAct
		, a.ItemNameAct
		, a.UnitIdAct
		, a.GroupSubstitusiAct
		--, a.SalesQtyAct

UPDATE @ActPor
SET QtyAct = a.QtyAct / b.TotalPor
	, QtyInKgAct = a.QtyInKgAct / b.TotalPor
	, ValueAct = (a.QtyInKgAct / b.TotalPor) * a.PriceAct
FROM @ActPor a 
LEFT JOIN @CountPor b ON b.ProductId = a.ProductIDAct

--Insert ke table detail--
INSERT INTO CogsProjection.dbo.BomStdVsAct_Det
SELECT ISNULL(a.VerId, @VerId) as VerId
		, ISNULL(a.Room, @Room) as Room
		, ISNULL(a.Tahun, @Year) as Tahun
		, ISNULL(a.Bulan, @Month) as Bulan
		, ISNULL(a.ProductIDPlan,b.ProductIDAct) as ProductId
		, ISNULL(a.ProductNamePlan,b.ProductNameAct) as ProductName
		, a.ItemIdPlan
		, a.ItemNamePlan
		, a.UnitIdPlan
		, a.GroupSubstitusiPlan
		, a.QtyPlan
		, a.QtyInKgPlan
		, a.PricePlan
		, (a.QtyInKgPlan * a.PricePlan) / a.Yield as ValuePlan
		, a.Yield as YieldPlan
		, b.ItemIdAct
		, b.ItemNameAct
		, b.UnitIdAct
		, b.GroupsubstituteAct
		, b.QtyAct
		, b.QtyInKgAct
		, b.PriceAct
		, b.ValueAct
FROM @BomPlan a
FULL OUTER JOIN @ActPor b ON b.ProductIDAct = a.ProductIDPlan AND b.ItemIdAct = a.ItemIdPlan


--2. Proses untuk summary--
DELETE FROM CogsProjection.dbo.FgPlanCostPrice 
WHERE Room = @Room
AND Tahun = @Year
AND Bulan = @Month
AND VerId = @VerId

INSERT INTO CogsProjection.dbo.FgPlanCostPrice
SELECT a.VerId
		, a.Room
		, a.Tahun
		, a.Bulan
		, a.ProductIdPlan as ProductID
		, a.ProductNamePlan as ProductName
		, c.ValuePlan
		, b.ValueAct
		--, SUM(a.ValuePlan) as ValuePlan
		--, b.ValueAct
FROM CogsProjection.dbo.BomStdVsAct_Det a
LEFT JOIN ( SELECT a.VerId
				, a.Room
				, a.Tahun
				, a.Bulan
				, a.ProductIdPlan as ProductIdAct
				, SUM(a.ValueAct) as ValueAct
			FROM CogsProjection.dbo.BomStdVsAct_Det a
			WHERE LEFT(a.ItemIdAct, 1) IN (1,6)
			AND a.Room = @Room
			AND a.Tahun = @Year
			AND a.Bulan = @Month
			AND a.VerId = @VerId
			GROUP BY a.VerId
					, a.Room
					, a.Tahun
					, a.Bulan
					, a.ProductIdPlan
) b ON b.ProductIdAct = a.ProductIdPlan
LEFT JOIN ( SELECT a.VerId
				, a.Room
				, a.Tahun
				, a.Bulan
				, a.ProductIdPlan
				, SUM(a.ValuePlan) as ValuePlan
			FROM CogsProjection.dbo.BomStdVsAct_Det a
			WHERE LEFT(a.ItemIdAct, 1) IN (1,6)
			AND a.Room = @Room
			AND a.Tahun = @Year
			AND a.Bulan = @Month
			AND a.VerId = @VerId
			GROUP BY a.VerId
					, a.Room
					, a.Tahun
					, a.Bulan
					, a.ProductIdPlan
) c On c.ProductIdPlan = a.ProductIdPlan
--WHERE a.ItemPlan LIKE '1%'
WHERE a.Room = @Room
AND a.Tahun = @Year
AND a.Bulan = @Month
AND a.VerId = @VerId
AND c.ValuePlan IS NOT NULL
AND b.ValueAct IS NOT NULL
--AND a.ProductIdPlan = '4200031'
GROUP BY a.VerId
		, a.Room
		, a.Tahun
		, a.Bulan
		, a.ProductIdPlan 
		, a.ProductNamePlan
		, c.ValuePlan
		, b.ValueAct
		--, b.ValueAct

END

--SELECT * FROM CogsProjection.dbo.FgPlanCostPrice
--WHERE ProductId = '4200031'
--and verid = 68
--WHERE VerId = 81
----ORDER BY ProductId asc


--SELECT * FROM CogsProjection.dbo.FgPlanCostPrice
--WHERE VerId = 76
--AND ROOM = 1
--AND Bulan = 10


--173

GO

