-- =============================================
-- Author:		<Putri>
-- Create date: <Des 2025>
-- Description:	<Summary AnlzMaterial_plan_vs_actual_detail>
-- =============================================
CREATE PROCEDURE [dbo].[BomStdVsAct_Sum_Calc]  

@Room int = 1
--, @ProductID nvarchar(20) = 4200031
, @Year int = 2025
, @Month int = 07
, @VerId int = 68

AS
BEGIN

--DELETE FROM CogsProjection.dbo.BomStdVsAct_Sum
--WHERE Room = @Room
--AND VerId = @VerId
--AND Tahun = @Year
--AND Bulan = @Month

--DECLARE @Result TABLE (RowId INT IDENTITY(1,1) PRIMARY KEY
--						, VerId int
--						, Room Int
--						, Tahun int
--						, Bulan int
--						, ProductIDPlan nvarchar(20)
--						, ProductNamePlan nvarchar(60)
--						, ItemIdPlan nvarchar(20)
--						, ItemNamePlan nvarchar(60)
--						, UnitIdPlan nvarchar(20)
--						, GroupSubstitusiPlan nvarchar(50)
--						, QtyPlan numeric(32,16)
--						, QtyInKgPlan numeric(32,16)
--						, PricePlan numeric(32,16)
--						, [SourcePlan] nvarchar(20)
--						, SalesQtyPlan numeric(32,16)
--						--, InventBatchIdPlan nvarchar(50)
--						, YieldPlan numeric(32,16)

--						-- Kolom untuk aktual
--						, AssignedActRowId int NULL
--						, ItemIdAct int NULL
--						, ItemNameAct nvarchar(60) NULL
--						, UnitIdAct nvarchar(20) NULL
--						, GroupSubstitusiAct nvarchar(50) NULL
--						, QtyAct numeric(32,16) NULL
--						, QtyInKgAct numeric(32,16) NULL
--						, PriceAct numeric(32,16) NULL
--						, SourceAct nvarchar(20) NULL
--						, SalesQtyAct numeric(32,16) NULL
--						--, InventBatchIdAct nvarchar(50) NULL
--						, YieldAct numeric(32,16)
--)

--DECLARE @BomAct TABLE (ActRowid int PRIMARY KEY
--						, ProductIdAct nvarchar(20)
--						, ProductNameAct nvarchar(60)
--						, ItemIdAct nvarchar(20)
--						, ItemNameAct nvarchar(60)
--						, UnitIdAct nvarchar(20)
--						, GroupSubstitusiAct nvarchar(50)
--						, QtyAct numeric(32,16)
--						, QtyInKgAct numeric(32,16)
--						, PriceAct numeric(32,16)
--						, [SourceAct] nvarchar(20)
--						, SalesQtyAct numeric(32,16)
--						--, InventBatchIdAct nvarchar(50)
--						, YieldAct numeric(32,16)
--						, IsUsed bit DEFAULT 0
--)

----Masukkan hasil untuk yang plan--
--INSERT INTO @Result (
--    VerId, Room, Tahun, Bulan,
--    ProductIDPlan, ProductNamePlan,
--    ItemIdPlan, ItemNamePlan, UnitIdPlan,
--    GroupSubstitusiPlan,
--    QtyPlan, QtyInKgPlan, PricePlan,
--    SourcePlan, SalesQtyPlan, YieldPlan
--)
--SELECT
--    a.VerId
--	, a.Room
--	, a.Tahun
--	, a.Bulan
--    , a.ProductIDPlan
--	, a.ProductNamePlan
--    , a.ItemPlan
--	, a.ItemNamePlan
--	, a.UnitIdPlan
--    , a.GroupPlan
--    , MAX(a.QtyPlan)
--    , MAX(a.QtyInKgPlan)
--    , MAX(a.PricePlan)
--    , 'Plan'
--    , 0
--    , a.YieldPlan
--FROM CogsProjection.dbo.BomStdVsAct_Det a
--WHERE a.VerId = @VerId
--  AND a.Room = @Room
--  AND a.Tahun = @Year
--  AND a.Bulan = @Month
--  --AND a.ProductIDPlan = @ProductID
--GROUP BY
--    a.VerId, a.Room, a.Tahun, a.Bulan,
--    a.ProductIDPlan, a.ProductNamePlan,
--    a.ItemPlan, a.ItemNamePlan, a.UnitIdPlan,
--    a.GroupPlan, a.YieldPlan;


----Tampung data actual ke BomAct
--INSERT INTO @BomAct
--SELECT
--    ROW_NUMBER() OVER (ORDER BY a.ProductIDPlan, a.ItemIdAct) AS ActRowId
--    , a.ProductIDPlan
--    , a.ProductNamePlan
--    , a.ItemIdAct
--    , a.ItemNameAct
--    , a.UnitIdAct
--    , a.GroupSubstitusiAct
--    , SUM(a.QtyAct) / COUNT (DISTINCT a.ProdId) 
--    , SUM(a.QtyInKgAct) / COUNT (DISTINCT a.ProdId) 
--    , SUM(a.QtyAct * a.PriceAct) / NULLIF(SUM(a.QtyAct),0)
--    , 'Act'
--    , 0
--    , 0
--    , 0
--FROM CogsProjection.dbo.BomStdVsAct_Det a
--WHERE a.VerId = @VerId
--  AND a.Room = @Room
--  AND a.Tahun = @Year
--  AND a.Bulan = @Month
--  --AND a.ProductIDPlan = @ProductID
--GROUP BY
--    a.ProductIDPlan,
--    a.ProductNamePlan,
--    a.ItemIdAct,
--    a.ItemNameAct,
--    a.UnitIdAct,
--    a.GroupSubstitusiAct;

----Cari Pasangan ItemIdAct dan ItemIdPlan
--DECLARE @PlanRowId INT, @p_ItemId NVARCHAR(20), @p_Group NVARCHAR(50), @ChosenAct INT;

--DECLARE plan_cursor CURSOR LOCAL FAST_FORWARD FOR
--SELECT RowId, ItemIdPlan, GroupSubstitusiPlan
--FROM @Result
--ORDER BY RowId;

--OPEN plan_cursor;
--FETCH NEXT FROM plan_cursor INTO @PlanRowId, @p_ItemId, @p_Group;

--WHILE @@FETCH_STATUS = 0
--BEGIN
--    SET @ChosenAct = NULL;

--    -- Priority 1: Group + Item
--    SELECT TOP 1 @ChosenAct = ActRowId
--    FROM @BomAct
--    WHERE IsUsed = 0
--      AND GroupSubstitusiAct = @p_Group
--      AND ItemIdAct = @p_ItemId;

--    -- Priority 2: Group only
--    IF @ChosenAct IS NULL AND @p_Group IS NOT NULL AND LTRIM(RTRIM(@p_Group)) <> ''
--        SELECT TOP 1 @ChosenAct = ActRowId
--        FROM @BomAct
--        WHERE IsUsed = 0 AND GroupSubstitusiAct = @p_Group;

--    -- Priority 3: Item only
--    IF @ChosenAct IS NULL
--        SELECT TOP 1 @ChosenAct = ActRowId
--        FROM @BomAct
--        WHERE IsUsed = 0 AND ItemIdAct = @p_ItemId;

--    IF @ChosenAct IS NOT NULL
--    BEGIN
--        UPDATE r
--        SET AssignedActRowId = a.ActRowId
--           , ItemIdAct = a.ItemIdAct
--           , ItemNameAct = a.ItemNameAct
--           , UnitIdAct = a.UnitIdAct
--           , GroupSubstitusiAct = a.GroupSubstitusiAct
--           , QtyAct = a.QtyAct
--           , QtyInKgAct = a.QtyInKgAct
--           , PriceAct = a.PriceAct
--           , SourceAct = a.SourceAct
--           , SalesQtyAct = a.SalesQtyAct
--           , YieldAct = a.YieldAct
--        FROM @Result r
--        JOIN @BomAct a ON a.ActRowId = @ChosenAct
--        WHERE r.RowId = @PlanRowId;

--        UPDATE @BomAct SET IsUsed = 1 WHERE ActRowId = @ChosenAct;
--    END

--    FETCH NEXT FROM plan_cursor INTO @PlanRowId, @p_ItemId, @p_Group;
--END

--CLOSE plan_cursor;
--DEALLOCATE plan_cursor;

--INSERT INTO @Result (
--    VerId, Room, Tahun, Bulan,
--    AssignedActRowId,
--    ItemIdAct, ItemNameAct, UnitIdAct,
--    GroupSubstitusiAct,
--    QtyAct, QtyInKgAct, PriceAct,
--    SourceAct, SalesQtyAct, YieldAct
--)
--SELECT
--    @VerId, @Room, @Year, @Month
--    , ActRowId
--    , ItemIdAct
--	, ItemNameAct
--	, UnitIdAct
--    , GroupSubstitusiAct
--    , QtyAct
--	, QtyInKgAct
--	, PriceAct
--    , SourceAct
--	, SalesQtyAct
--	, YieldAct
--FROM @BomAct
--WHERE IsUsed = 0;

--INSERT INTO CogsProjection.dbo.BomStdVsAct_Sum
--SELECT a.VerId
--		, a.Room
--		, a.Tahun
--		, a.Bulan
--		, a.ProductIDPlan
--		, a.ProductNamePlan as ProductName
--		, a.ItemIdPlan
--		, a.ItemNamePlan
--		, a.UnitIdPlan
--		, a.GroupSubstitusiPlan
--		, a.QtyPlan
--		, a.QtyInKgPlan
--		, a.PricePlan
--		, a.YieldPlan
--		, a.ItemIdAct
--		, a.ItemNameAct
--		, a.UnitIdAct
--		, a.GroupSubstitusiAct
--		, a.QtyAct
--		, a.QtyInKgAct
--		, a.PriceAct
--		--, SalesQtyAct
--		, SUM(b.SalesQtyAct) as SalesQtyAct
--		, a.YieldAct

--FROM @Result a
--LEFT JOIN (SELECT ProductIdPlan
--				--, ItemIdAct
--				, InventBatchIdAct
--				, MAX(SalesQtyAct) as SalesQtyAct
--		   FROM CogsProjection.dbo.BomStdVsAct_Det
--		   GROUP BY ProductIdPlan
--				--, ItemIdAct
--				, InventBatchIdAct
--				) b ON b.ProductIDPlan = a.ProductIDPlan 
--WHERE (a.ItemIdAct IS NOT NULL OR a.ItemIdPlan IS NOT NULL)
--GROUP BY a.VerId
--		, a.Room
--		, a.Tahun
--		, a.Bulan
--		, a.ProductIDPlan
--		, a.ProductNamePlan
--		, a.ItemIdPlan
--		, a.ItemNamePlan
--		, a.UnitIdPlan
--		, a.GroupSubstitusiPlan
--		, a.QtyPlan
--		, a.QtyInKgPlan
--		, a.PricePlan
--		, a.YieldPlan
--		, a.ItemIdAct
--		, a.ItemNameAct
--		, a.UnitIdAct
--		, a.GroupSubstitusiAct
--		, a.QtyAct
--		, a.QtyInKgAct
--		, a.PriceAct
--		, a.YieldAct
--ORDER BY
--    COALESCE(a.ItemIdPlan, a.ItemIdAct);

--SELECT * FROM CogsProjection.dbo.BomStdVsAct_Sum

-- 1. Bersihkan Data Lama
DELETE FROM CogsProjection.dbo.BomStdVsAct_Sum
WHERE Room = @Room
AND VerId = @VerId
AND Tahun = @Year 
AND Bulan = @Month

-- 2. CREATE TABLE #Result
IF OBJECT_ID('tempdb..#Result') IS NOT NULL DROP TABLE #Result;
CREATE TABLE #Result (
    RowId INT IDENTITY(1,1) PRIMARY KEY
   , VerId int
   , Room Int
   , Tahun int
   , Bulan int
   , ProductIDPlan nvarchar(20)
   , ProductNamePlan nvarchar(60)
   , ItemIdPlan nvarchar(20)
   , ItemNamePlan nvarchar(60)
   , UnitIdPlan nvarchar(20)
   , GroupSubstitusiPlan nvarchar(50)
   , QtyPlan numeric(32,16)
   , QtyInKgPlan numeric(32,16)
   , PricePlan numeric(32,16)
   , SourcePlan nvarchar(20)
   , SalesQtyPlan numeric(32,16)
   , YieldPlan numeric(32,16)
   , AssignedActRowId int NULL 
   , ItemIdAct nvarchar(20) NULL
   , ItemNameAct nvarchar(60) NULL
   , UnitIdAct nvarchar(20) NULL
   , GroupSubstitusiAct nvarchar(50) NULL
   , QtyAct numeric(32,16) NULL
   , QtyInKgAct numeric(32,16) NULL
   , PriceAct numeric(32,16) NULL
   , SourceAct nvarchar(20) NULL
   , SalesQtyAct numeric(32,16) NULL
   , YieldAct numeric(32,16) NULL
);

-- 3. CREATE TABLE #BomAct 
IF OBJECT_ID('tempdb..#BomAct') IS NOT NULL DROP TABLE #BomAct;
CREATE TABLE #BomAct (
    ActRowId INT PRIMARY KEY
   , ProductIdAct nvarchar(20) 
   , ProductNameAct nvarchar(60) 
   , ItemIdAct nvarchar(20)
   , ItemNameAct nvarchar(60) 
   , UnitIdAct nvarchar(20) 
   , GroupSubstitusiAct nvarchar(50)
   , QtyAct numeric(32,16)
   , QtyInKgAct numeric(32,16) 
   , PriceAct numeric(32,16)
   , SourceAct nvarchar(20)
   , SalesQtyAct numeric(32,16)
   , YieldAct numeric(32,16)
   , IsUsed bit DEFAULT 0
);

-- 4. INSERT DATA PLAN
INSERT INTO #Result (VerId
					, Room
					, Tahun
					, Bulan
					, ProductIDPlan
					, ProductNamePlan
					, ItemIdPlan
					, ItemNamePlan
					, UnitIdPlan
					, GroupSubstitusiPlan
					, QtyPlan
					, QtyInKgPlan
					, PricePlan
					, SourcePlan
					, SalesQtyPlan
					, YieldPlan)
SELECT a.VerId
		, a.Room
		, a.Tahun
		, a.Bulan
		, a.ProductIDPlan
		, a.ProductNamePlan
		, a.ItemPlan
		, a.ItemNamePlan
		, a.UnitIdPlan
		, a.GroupPlan
        , MAX(a.QtyPlan)
		, MAX(a.QtyInKgPlan)
		, MAX(a.PricePlan)
		, 'Plan'
		, 0
		, a.YieldPlan
FROM CogsProjection.dbo.BomStdVsAct_Det a
WHERE a.VerId = @VerId 
AND a.Room = @Room 
AND a.Tahun = @Year 
AND a.Bulan = @Month
GROUP BY a.VerId
		, a.Room
		, a.Tahun
		, a.Bulan
		, a.ProductIDPlan
		, a.ProductNamePlan
		, a.ItemPlan
		, a.ItemNamePlan
		, a.UnitIdPlan
		, a.GroupPlan
		, a.YieldPlan;

-- 5. INSERT DATA AKTUAL
INSERT INTO #BomAct (ActRowId, ProductIdAct, ProductNameAct, ItemIdAct, ItemNameAct, UnitIdAct, GroupSubstitusiAct, QtyAct, QtyInKgAct, PriceAct, SourceAct, SalesQtyAct, YieldAct)
SELECT ROW_NUMBER() OVER (ORDER BY a.ProductIDPlan, a.ItemIdAct) AS ActRowId,
       a.ProductIDPlan, a.ProductNamePlan, a.ItemIdAct, a.ItemNameAct, a.UnitIdAct, a.GroupSubstitusiAct,
       SUM(a.QtyAct) / NULLIF(COUNT(DISTINCT a.ProdId),0), 
       SUM(a.QtyInKgAct) / NULLIF(COUNT(DISTINCT a.ProdId),0), 
       SUM(a.QtyAct * a.PriceAct) / NULLIF(SUM(a.QtyAct),0),
       'Act', 0, 0
FROM CogsProjection.dbo.BomStdVsAct_Det a
WHERE a.VerId = @VerId AND a.Room = @Room AND a.Tahun = @Year AND a.Bulan = @Month
GROUP BY a.ProductIDPlan, a.ProductNamePlan, a.ItemIdAct, a.ItemNameAct, a.UnitIdAct, a.GroupSubstitusiAct;

-- 6. INDEXING
CREATE INDEX IX_Matching_Act ON #BomAct(ProductIdAct, ItemIdAct, GroupSubstitusiAct);

-- 7. UPDATE JOIN (DENGAN PERBAIKAN SYNTAX)
;WITH MatchMaker AS (
    SELECT 
        r.RowId as RefRowId, 
        b.ActRowId as RefActId,
        ROW_NUMBER() OVER (PARTITION BY r.RowId ORDER BY 
            CASE 
                WHEN b.ItemIdAct = r.ItemIdPlan AND b.GroupSubstitusiAct = r.GroupSubstitusiPlan THEN 1
                WHEN b.GroupSubstitusiAct = r.GroupSubstitusiPlan AND b.GroupSubstitusiAct <> '' THEN 2
                WHEN b.ItemIdAct = r.ItemIdPlan THEN 3
                ELSE 99 END ASC, b.ActRowId ASC) as PriorityRank,
        ROW_NUMBER() OVER (PARTITION BY b.ActRowId ORDER BY 
            CASE 
                WHEN b.ItemIdAct = r.ItemIdPlan AND b.GroupSubstitusiAct = r.GroupSubstitusiPlan THEN 1
                WHEN b.GroupSubstitusiAct = r.GroupSubstitusiPlan AND b.GroupSubstitusiAct <> '' THEN 2
                WHEN b.ItemIdAct = r.ItemIdPlan THEN 3
                ELSE 99 END ASC, r.RowId ASC) as AntiDoubleRank
    FROM #Result r
    INNER JOIN #BomAct b ON r.ProductIDPlan = b.ProductIdAct
    WHERE (b.ItemIdAct = r.ItemIdPlan OR (b.GroupSubstitusiAct = r.GroupSubstitusiPlan AND b.GroupSubstitusiAct <> ''))
)
UPDATE res
SET res.AssignedActRowId = m.RefActId,
    res.ItemIdAct = b.ItemIdAct, 
    res.ItemNameAct = b.ItemNameAct, 
    res.UnitIdAct = b.UnitIdAct,
    res.GroupSubstitusiAct = b.GroupSubstitusiAct, 
    res.QtyAct = b.QtyAct, 
    res.QtyInKgAct = b.QtyInKgAct,
    res.PriceAct = b.PriceAct, 
    res.SourceAct = b.SourceAct,
    res.SalesQtyAct = b.SalesQtyAct,
    res.YieldAct = b.YieldAct
FROM #Result res
INNER JOIN MatchMaker m ON res.RowId = m.RefRowId
INNER JOIN #BomAct b ON m.RefActId = b.ActRowId
WHERE m.PriorityRank = 1 AND m.AntiDoubleRank = 1;

-- 8. Tampilkan Hasil Akhir
INSERT INTO CogsProjection.dbo.BomStdVsAct_Sum
SELECT a.VerId
		, a.Room
		, a.Tahun
		, a.Bulan
		, a.ProductIDPlan 
		, a.ProductNamePlan
		, a.ItemIdPlan
		, a.ItemNamePlan
		, a.UnitIdPlan
		, a.GroupSubstitusiPlan
		, a.QtyPlan
		, a.QtyInKgPlan
		, a.PricePlan
		, a.YieldPlan
		, a.ItemIdAct
		, a.ItemNameAct
		, a.UnitIdAct
		, a.GroupSubstitusiAct
		, a.QtyAct
		, a.QtyInKgAct
		, a.PriceAct
		, a.SalesQtyAct
		, a.YieldAct
FROM #Result a 
ORDER BY ProductIDPlan, COALESCE(ItemIdPlan, ItemIdAct);


SELECT * FROM CogsProjection.dbo.BomStdVsAct_Sum



END

GO

