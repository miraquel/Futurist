-- =============================================
-- Author:		<Putri>
-- Create date: <08 Des 2025>
-- Description:	<Analisa RM Plan vs Act>
-- =============================================
CREATE PROCEDURE [dbo].[BomStdVsAct_CalcBckup] 

@Room int = 1
--, @ItemId nvarchar(20) 
, @VerId int = 68
, @Year int = 2025
, @Month int = 07

AS
BEGIN

--DELETE FROM CogsProjection.dbo.BomStdVsAct_Det 
--WHERE Room = @Room 
--AND VerId = @VerId 
--AND Tahun = @Year
--AND Bulan = @Month

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
JOIN CogsProjection.dbo.BomStdRoomVer b ON b.ProductId = a.ItemId
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
--AND a.ItemId in ( '4200031', '4200032')
AND a.ItemId = '4200031'
AND YEAR(a.RofoDate) = @Year
AND MONTH(a.RofoDate) = @Month
AND b.Room = @Room
AND b.VerId = @VerId
--AND b.ProductId in ( '4200031', '4200032')
--AND b.ProductId = '4200032'
ORDER BY b.ItemId asc

--SELECT * FROM @BomPlan
--RETURN


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
		--, SUM(a.ActQty) / COUNT(DISTINCT a.ProdId) as ActQty
		, SUM(a.ActQty) as ActQty
		--, SUM(a.ActQty * c.NETWEIGHT / 1000) / COUNT(DISTINCT a.ProdId) as QtyInKg
		, SUM(a.ActQty * c.NETWEIGHT / 1000)  as QtyInKg
		, SUM(a.ActQty * a.ActPrice) / NULLIF(Sum(a.ActQty),0) as ActPrice
		--, a.ActPrice 
		, 'Act' as [Source]
		, a.SalesQty
		--,0
		, a.InventBatchId
		, 0
		--, ''
		, a.ProdId
FROM CogsProjection.dbo.MaterialAct a
LEFT JOIN AXGMKDW.dbo.DimItem b On b.ITEMID = a.ProductId
LEFT JOIN AXGMKDW.dbo.DimItem c On c.ITEMID = a.ItemId
LEFT JOIN AXGMKDW.dbo.DimItemSubstitute d On d.ItemId = a.ItemId

WHERE [Year] = @Year
AND [Month] = @Month
--AND a.ProductId in ( '4200031', '4200032')
AND a.ProductId = '4200031'
--AND a.InventBatchId = '92525372'

GROUP BY a.ProductId
		, b.SEARCHNAME 
		, a.ItemId
		, c.SEARCHNAME
		, c.UNITID
		, d.VtaMpSubstitusiGroupId
		, a.SalesQty
		, a.InventBatchId
		, a.ProdId

SELECT * FROM @BomAct
--WHERE ItemIdAct = ''

RETURN

--Final Prosess--
DECLARE @MatchItem as Table (VerId Int
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
								, YieldPlan numeric(32,16)
								, ProductIDAct nvarchar(20)
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
								, YieldAct numeric(32,16)
								, ProdId nvarchar(20)
								, MatchRank int
								
								)

INSERT INTO @MatchItem
SELECT a.*
		, b.*
		, CASE 
			WHEN a.GroupSubstitusiPlan IS NOT NULL AND a.GroupSubstitusiPlan <> '' 
				AND b.GroupSubstitusiAct = a.GroupSubstitusiPlan
                AND b.ItemIdAct = a.ItemIdPlan
				THEN 1 -- group substitute & itemid
			WHEN a.GroupSubstitusiPlan IS NOT NULL AND a.GroupSubstitusiPlan <> '' 
                AND b.GroupSubstitusiAct = a.GroupSubstitusiPlan
                THEN 2   -- hanya groupsubstitute
			WHEN (a.GroupSubstitusiPlan IS NULL OR a.GroupSubstitusiPlan = '')
                 AND b.ItemIdAct = a.ItemIdPlan
                 THEN 3   -- hanya itemid
                ELSE 99
            END AS MatchRank


FROM @BomPlan a
LEFT JOIN @BomAct b ON a.ProductIDPlan = b.ProductIDAct
AND (
		(
            (a.GroupSubstitusiPlan IS NOT NULL AND a.GroupSubstitusiPlan <> '' 
             AND b.GroupSubstitusiAct = a.GroupSubstitusiPlan AND b.ItemIdAct = a.ItemIdPlan)
        )
        OR
        (
            (a.GroupSubstitusiPlan IS NOT NULL AND a.GroupSubstitusiPlan <> '' 
             AND b.GroupSubstitusiAct = a.GroupSubstitusiPlan)
        )
        OR
        (
            (a.GroupSubstitusiPlan IS NULL OR a.GroupSubstitusiPlan = '')
            AND b.ItemIdAct = a.ItemIdPlan
        )
)


DECLARE @BestMatch as Table (VerId Int
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
								, YieldPlan numeric(32,16)
								, ProductIDAct nvarchar(20)
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
								, YieldAct numeric(32,16)
								, ProdId nvarchar(20)
								, MatchRank int
								, Rowmatch int

								)
INSERT INTO @BestMatch 
SELECT *
      , ROW_NUMBER() OVER (
                PARTITION BY ProductIDPlan ,ItemIdPlan, InventBatchIdAct   -- per PLAN ambil act kalau ada
                ORDER BY MatchRank ASC           -- hanya 1 yang match
           ) AS Rowmatch
FROM @MatchItem
		


--Result Detail--
INSERT INTO CogsProjection.dbo.BomStdVsAct_Det
SELECT VerId,
	Room,
	Tahun,
	Bulan,
    ProductIdPlan,
    ProductNamePlan,
    ItemIdPlan AS ItemPlan,
    ItemNamePlan AS ItemNamePlan,
    UnitIdPlan AS UnitIdPlan,
    GroupSubstitusiPlan AS GroupPlan,
    QtyPlan AS QtyPlan,
    QtyInKgPlan AS QtyInKgPlan,
    PricePlan AS PricePlan,
	YieldPlan,
    ItemIdAct,
    ItemNameAct,
    UnitIdAct,
    GroupSubstitusiAct,
    QtyAct,
    QtyInKgAct,
    PriceAct,
    SalesQtyAct,
    InventBatchIdAct,
    (QtyAct / NULLIF(QtyPlan,0)) as ContrQty,
    (PriceAct / NULLIF(PricePlan,0)) as ContrPrice,
	ProdId
	
    --CASE WHEN (QtyAct / QtyPlan) >= 120 THEN 'YES' ELSE 'NO' END AS [ContrQty>120%]
--INTO CogsProjection.dbo.BomStdVsAct
FROM @BestMatch
WHERE Rowmatch = 1      
ORDER BY ItemIdAct asc



END

--SELECT * FROM CogsProjection.dbo.BomStdVsAct_Det
--SELECT * FROM CogsProjection.dbo.BomStdVsAct_Sum
--1285 

--SELECT * FROM CogsProjection.dbo.BomStdVsAct_Det 

GO

