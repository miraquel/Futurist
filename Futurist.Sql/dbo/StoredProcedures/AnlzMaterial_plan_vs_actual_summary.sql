-- =============================================
-- Author:		<Putri>
-- Create date: <Des 2025>
-- Description:	<Summary AnlzMaterial_plan_vs_actual_detail>
-- =============================================
CREATE PROCEDURE [dbo].[AnlzMaterial_plan_vs_actual_summary]  

@Room int = 1
, @ProductID nvarchar(20) = 4200031
, @Year int = 2025
, @Month int = 07
, @VerId int = 68

AS
BEGIN
--DECLARE @Plan Table (VerId int
--						, Room int
--						, Tahun int
--						, Bulan int
--						, ProductId nvarchar(20)
--						, ProductName nvarchar(1000)
--						, ItemPlan nvarchar(20)
--						, ItemName nvarchar(1000)
--						, UnitIdPlan nvarchar(5)
--						, GroupPlan nvarchar(20)
--						, QtyPlan numeric(32,16)
--						, QtyInKgPlan numeric(32,16)
--						, PricePlan numeric(32,16)
--						, Yield numeric(32,16)
--						)

--INSERT INTO @Plan 
--SELECT a.VerId
--	, a.Room
--	, a.Tahun
--	, a.Bulan
--	, a.ProductIdPlan as ProductId
--	, a.ProductNamePlan as ProductName
--	, a.ItemPlan
--	, a.ItemNamePlan
--	, a.UnitIdPlan
--	, a.GroupPlan
--	, a.QtyPlan
--	, a.QtyInKgPlan
--	, a.PricePlan
--	, a.YieldPlan
--FROM CogsProjection.dbo.BomStdVsAct a
--GROUP BY a.VerId
--	, a.Room
--	, a.Tahun
--	, a.Bulan
--	, a.ProductIdPlan 
--	, a.ProductNamePlan 
--	, a.ItemPlan
--	, a.ItemNamePlan
--	, a.UnitIdPlan
--	, a.GroupPlan
--	, a.QtyPlan
--	, a.QtyInKgPlan
--	, a.PricePlan
--	, a.YieldPlan

--SELECT * FROM @Plan



--SELECT a.VerId
--	, a.Room
--	, a.Tahun
--	, a.Bulan
--	, a.ProductId
--	, a.ProductName
--	, a.ItemPlan
--	, a.ItemName
--	, a.UnitIdPlan
--	, a.GroupPlan
--	, a.QtyPlan
--	, a.QtyInKgPlan
--	, a.PricePlan
--	, a.Yield
--	, b.ItemIdAct
--	, b.ItemNameAct
--	, b.UnitIdAct
--	, b.GroupSubstitusiAct
--	, b.QtyAct
--	, b.QtyInKgAct
--	, b.PriceAct
--	, b.SalesQtyAct

--FROM @Plan a
--LEFT JOIN (SELECT b.ProductIdPlan
--				, b.VerId
--				, b.Room
--				, b.Tahun
--				, b.Bulan
--				, b.ItemIdAct
--				, b.ItemNameAct
--				, b.UnitIdAct
--				, b.GroupSubstitusiAct
--				, SUM(QtyAct) as QtyAct
--				, SUM(QtyInKgAct) as QtyInKgAct
--				, SUM(b.QtyAct * b.PriceAct) / SUM(b.QtyAct) as PriceAct
--				, SUM(b.SalesQtyAct) as SalesQtyAct

--FROM CogsProjection.dbo.BomStdVsAct b
--WHERE b.ItemIdAct IS NOT NULL
--GROUP BY b.ItemIdAct
--				, b.ItemNameAct
--				, b.UnitIdAct
--				--, b.PriceAct
--				, b.ProductIdPlan
--				, b.VerId
--				, b.Room
--				, b.Tahun
--				, b.Bulan
--				, b.GroupSubstitusiAct
--) b 
--    ON  a.ProductId = b.ProductIdPlan
--    AND a.VerId        = b.VerId
--    AND a.Room         = b.Room
--    AND a.Tahun        = b.Tahun
--    AND a.Bulan        = b.Bulan
--    AND (
--            (a.GroupPlan IS NOT NULL AND a.GroupPlan <> '' 
--             AND b.GroupSubstitusiAct = a.GroupPlan AND b.ItemIdAct = a.ItemPlan)

--        OR  (a.GroupPlan IS NOT NULL AND a.GroupPlan <> '' 
--             AND b.GroupSubstitusiAct = a.GroupPlan)

--        OR  ((a.GroupPlan IS NULL OR a.GroupPlan = '')
--             AND b.ItemIdAct = a.ItemPlan)
--        )
--ORDER BY b.ItemIdAct asc

--SELECT * FROM CogsProjection.dbo.BomStdVsAct

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

----Insert Result Untuk yang plan--
--INSERT INTO @Result (
--        VerId, Room, Tahun, Bulan, ProductIDPlan, ProductNamePlan,
--        ItemIdPlan, ItemNamePlan, UnitIdPlan, GroupSubstitusiPlan,
--        QtyPlan, QtyInKgPlan, PricePlan, [SourcePlan], SalesQtyPlan,
--         YieldPlan
--    )
--SELECT a.VerId
--		, a.Room
--		, a.Tahun
--		, a.Bulan
--		, a.ProductIDPlan
--		, a.ProductNamePlan
--		, a.ItemPlan
--		, a.ItemNamePlan
--		, a.UnitIdPlan
--		, a.GroupPlan
--		, MAX(a.QtyPlan) as QtyPlan
--		, MAX(a.QtyInKgPlan) as QtyInKgPlan
--		, MAX(a.PricePlan) as PricePlan
--		, 'Plan'
--		, 0
--		--, ''
--		, a.YieldPlan
--FROM CogsProjection.dbo.BomStdVsAct a
--WHERE a.Room = @Room
--	AND a.VerId = @VerId
--	AND a.Tahun = @Year
--	AND a.Bulan = @Month
--	AND a.ProductIDPlan = '4200031'
--	--AND a.InventBatchId
--GROUP BY a.VerId
--		, a.Room
--		, a.Tahun
--		, a.Bulan
--		, a.ProductIDPlan
--		, a.ProductNamePlan
--		, a.ItemPlan
--		, a.ItemNamePlan
--		, a.UnitIdPlan
--		, a.GroupPlan
--		, a.YieldPlan

--SELECT * FROM @Result

----Siapkan temp tabel act untuk insert act ke Result--
--;WITH Act2 AS (
--				SELECT a.ProductIDPlan as ProductID
--						, a.ProductNamePlan as ProductNamePlan
--						, a.ItemIdAct
--						, a.ItemNameAct
--						, a.UnitIdAct
--						, a.GroupSubstitusiAct
--						, SUM(a.QtyAct) as QtyAct
--						, SUM(a.QtyInKgAct) as QtyInKgAct
--						, SUM(a.QtyAct * a.PriceAct) / NULLIF(SUM(a.QtyAct),0) AS ActPrice
--						, 'Act' as [Source]
--						--, SUM(a.SalesQtyAct) as SalesQtyAct
--						--, a.InventBatchIdAct
--						, 0 as YieldVal
--				FROM CogsProjection.dbo.BomStdVsAct a
--				WHERE a.Tahun = @Year
--					AND a.Bulan = @Month
--					AND a.VerId = @VerId
--					AND a.Room = @Room
--					AND a.ProductIDPlan = '4200031'
--					--AND a.InventBatchIdAct = '92527497'

--				GROUP BY a.ProductIDPlan
--						, a.ProductNamePlan
--						, a.ItemIdAct
--						, a.ItemNameAct
--						, a.UnitIdAct
--						, a.GroupSubstitusiAct
--				--ORDER BY a.ItemIdAct asc
--						--, a.SalesQtyAct
--						--, a.InventBatchIdAct
--)

--SELECT * FROM Act2


--INSERT INTO @BomAct (
--        ActRowId, ProductIDAct, ProductNameAct, ItemIdAct, ItemNameAct, UnitIdAct, GroupSubstitusiAct,
--        QtyAct, QtyInKgAct, PriceAct, [SourceAct], SalesQtyAct, YieldAct, IsUsed
--    )

--SELECT ROW_NUMBER() OVER (ORDER BY ProductId, ItemIdAct) AS ActRowId
--		, ProductID
--		, ProductNamePlan
--		, ItemIdAct
--		, ItemNameAct
--		, UnitIdAct
--		, GroupSubstitusiAct
--		, SUM(QtyAct) as QtyAct
--		, SUM(QtyInKgAct) as QtyInKgAct
--		, SUM(QtyAct * PriceAct) / NULLIF(SUM(QtyAct),0) as PriceAct
--		, 'Act'
--		, 0
--		--, InventBatchIdAct
--		, 0
--		, 0

--FROM Act2
--ORDER By ProductID, ItemIdAct
--INSERT INTO @BomAct (
--        ActRowId, ProductIDAct, ProductNameAct, ItemIdAct, ItemNameAct, UnitIdAct, GroupSubstitusiAct,
--        QtyAct, QtyInKgAct, PriceAct, [SourceAct], SalesQtyAct, YieldAct, IsUsed
--    )

--SELECT ROW_NUMBER() OVER (ORDER BY ProductIdPlan, ItemIdAct) AS ActRowId
--		, ProductIDPlan
--		, ProductNamePlan
--		, ItemIdAct
--		, ItemNameAct
--		, UnitIdAct
--		, GroupSubstitusiAct
--		, SUM(QtyAct) as QtyAct
--		, SUM(QtyInKgAct) as QtyInKgAct
--		, SUM(QtyAct * PriceAct) / NULLIF(SUM(QtyAct),0) as PriceAct
--		, 'Act'
--		, 0
--		--, InventBatchIdAct
--		, 0
--		, 0

--FROM CogsProjection.dbo.BomStdVsAct 
--WHERE VerId = @VerId
--  AND Room = @Room
--  AND Tahun = @Year
--  AND Bulan = @Month
--  AND ProductIDPlan = @ProductID
--GROUP BY
--    ProductIDPlan,
--    ProductNamePlan,
--    ItemIdAct,
--    ItemNameAct,
--    UnitIdAct,
--    GroupSubstitusiAct;

--SELECT * FROM @BomAct

--Looping untuk matching data : untuk setiap baris , temukan itemid act dan plan nya prioritas 1 > 2 > 3)
--Pastikan Act masih belum digunakan untuk di act
--DECLARE @PlanRowId int
--DECLARE @p_ProductIDPlan nvarchar(20)
--DECLARE @p_ItemIdPlan nvarchar(20)
--DECLARE @p_GroupSub nvarchar(50)
--DECLARE @PlanRowId INT, @p_ItemId NVARCHAR(20), @p_Group NVARCHAR(50), @ChosenAct INT;

----Cursor untuk ngambil data itemidAct
--DECLARE plan_cursor CURSOR LOCAL FAST_FORWARD FOR
--SELECT RowId
--	--, ProductIDPlan
--	, ItemIdPlan
--	, GroupSubstitusiPlan

--FROM @Result
--ORDER BY RowId

--OPEN plan_cursor;
--FETCH NEXT FROM plan_cursor INTO @PlanRowId, @p_ItemId, @p_Group
--WHILE @@FETCH_STATUS = 0 
--BEGIN
--	--Prioritas 1: GroupSubstitusi + ItemId exact --
--	SET @ChosenAct = NULL

--	SELECT TOP 1 @ChosenAct = ActRowId
--	FROM @BomAct a
--	WHERE ISNULL(a.IsUsed, 0) = 0
--		AND a.GroupSubstitusiAct = @p_Group
--		AND a.ItemIdAct = @p_ItemId
	

	--IF @ChosenAct IS NULL
	--BEGIN
		--Prioritas 2 : Hanya Grup (tapi act yang udah dipake gak dipake lagi)
--		IF @p_GroupSub IS NOT NULL AND LTRIM(RTRIM(@p_GroupSub)) <> ''
--		BEGIN
--			SELECT TOP 1 @ChosenActRowId = ActRowId
--			FROM @BomAct a
--			WHERE ISNULL(a.IsUsed, 0) = 0
--				AND a.GroupSubstitusiAct = @p_GroupSub
--		END
--	END

--	IF @ChosenActRowId IS NULL
--	BEGIN
--		--Prioritas 3: hanya itemId yang sama
--		SELECT TOP 1 @ChosenActRowId = ActRowId
--		FROM @BomAct a 
--		WHERE ISNULL (a.IsUsed, 0) = 0
--			AND a.ItemIdAct = @p_ItemIdPlan
--		ORDER BY a.ActRowid
--	END

--	--Jika menemukan ACT , perbarui baris (AssignedActRowId + Kolom ACT) dan tandai ACT IsUsed =1

--	IF @ChosenActRowId IS NOT NULL
--	BEGIN 
--		UPDATE p 
--		SET AssignedActRowId = a.ActRowId
--			, ItemIdAct = a.ItemIdAct
--               , ItemNameAct = a.ItemNameAct
--               , UnitIdAct = a.UnitIdAct
--               , GroupSubstitusiAct = a.GroupSubstitusiAct
--               , QtyAct = a.QtyAct
--               , QtyInKgAct = a.QtyInKgAct
--               , PriceAct = a.PriceAct
--               , SourceAct = a.SourceAct
--               , SalesQtyAct = a.SalesQtyAct
--               --, InventBatchIdAct = a.InventBatchIdAct
--               , YieldAct = a.YieldAct
--		FROM @Result p
--		JOIN @BomAct a ON a.ActRowid = @ChosenActRowId
--		WHERE p.RowId = @PlanRowId

--		--Tandai Act digunakan
--		UPDATE @BomAct
--		SET IsUsed = 1
--		WHERE ActRowid = @ChosenActRowId
--	END
--	--jika gak ada, act null

--	FETCH NEXT FROM plan_cursor INTO @PlanRowId, @p_ProductIDPlan, @p_ItemIdPlan, @p_GroupSub
--END

--CLOSE plan_cursor
--DEALLOCATE plan_cursor

----Final Select : semua PLAN ditampilkan : ACT kolom NULL jika tidak ada match

----Step akhir insert plan yang tidak ada (act nya aja yang ada)


--SELECT 
--        a.VerId
--		, a.Room
--		, a.Tahun
--		, a.Bulan
--		, a.ProductIDPlan
--		, a.ProductNamePlan
--		, a.ItemIdPlan AS ItemPlan
--		, a.ItemNamePlan AS ItemNamePlan
--		, a.UnitIdPlan
--		, a.GroupSubstitusiPlan AS GroupPlan
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
--		, SUM(b.SalesQtyAct) as SalesQtyAct
--		--, InventBatchIdAct
--        --, CASE WHEN a.QtyPlan IS NULL OR a.QtyPlan = 0 THEN NULL ELSE (a.QtyAct / NULLIF(a.QtyPlan,0)) END AS ContrQty
--        --, CASE WHEN PricePlan IS NULL OR a.PricePlan = 0 THEN NULL ELSE (a.PriceAct / NULLIF(a.PricePlan,0)) END AS ContrPrice
--FROM @Result a
--LEFT JOIN (SELECT ProductIdPlan
--				--, ItemIdAct
--				, InventBatchIdAct
--				, MAX(SalesQtyAct) as SalesQtyAct
--		   FROM CogsProjection.dbo.BomStdVsAct
--		   GROUP BY ProductIdPlan
--				--, ItemIdAct
--				, InventBatchIdAct
--				) b ON b.ProductIDPlan = a.ProductIDPlan 

--GROUP BY  a.VerId
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
--ORDER BY a.ItemIdAct asc


DECLARE @Result TABLE (RowId INT IDENTITY(1,1) PRIMARY KEY
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
						, [SourcePlan] nvarchar(20)
						, SalesQtyPlan numeric(32,16)
						--, InventBatchIdPlan nvarchar(50)
						, YieldPlan numeric(32,16)

						-- Kolom untuk aktual
						, AssignedActRowId int NULL
						, ItemIdAct int NULL
						, ItemNameAct nvarchar(60) NULL
						, UnitIdAct nvarchar(20) NULL
						, GroupSubstitusiAct nvarchar(50) NULL
						, QtyAct numeric(32,16) NULL
						, QtyInKgAct numeric(32,16) NULL
						, PriceAct numeric(32,16) NULL
						, SourceAct nvarchar(20) NULL
						, SalesQtyAct numeric(32,16) NULL
						--, InventBatchIdAct nvarchar(50) NULL
						, YieldAct numeric(32,16)
)

DECLARE @BomAct TABLE (ActRowid int PRIMARY KEY
						, ProductIdAct nvarchar(20)
						, ProductNameAct nvarchar(60)
						, ItemIdAct nvarchar(20)
						, ItemNameAct nvarchar(60)
						, UnitIdAct nvarchar(20)
						, GroupSubstitusiAct nvarchar(50)
						, QtyAct numeric(32,16)
						, QtyInKgAct numeric(32,16)
						, PriceAct numeric(32,16)
						, [SourceAct] nvarchar(20)
						, SalesQtyAct numeric(32,16)
						--, InventBatchIdAct nvarchar(50)
						, YieldAct numeric(32,16)
						, IsUsed bit DEFAULT 0
)

--Masukkan hasil untuk yang plan--
INSERT INTO @Result (
    VerId, Room, Tahun, Bulan,
    ProductIDPlan, ProductNamePlan,
    ItemIdPlan, ItemNamePlan, UnitIdPlan,
    GroupSubstitusiPlan,
    QtyPlan, QtyInKgPlan, PricePlan,
    SourcePlan, SalesQtyPlan, YieldPlan
)
SELECT
    a.VerId
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
  AND a.ProductIDPlan = @ProductID
GROUP BY
    a.VerId, a.Room, a.Tahun, a.Bulan,
    a.ProductIDPlan, a.ProductNamePlan,
    a.ItemPlan, a.ItemNamePlan, a.UnitIdPlan,
    a.GroupPlan, a.YieldPlan;


--Tampung data actual ke BomAct
INSERT INTO @BomAct
SELECT
    ROW_NUMBER() OVER (ORDER BY a.ProductIDPlan, a.ItemIdAct) AS ActRowId
    , a.ProductIDPlan
    , a.ProductNamePlan
    , a.ItemIdAct
    , a.ItemNameAct
    , a.UnitIdAct
    , a.GroupSubstitusiAct
    , SUM(a.QtyAct) / COUNT (DISTINCT a.ProdId) 
    , SUM(a.QtyInKgAct) / COUNT (DISTINCT a.ProdId) 
    , SUM(a.QtyAct * a.PriceAct) / NULLIF(SUM(a.QtyAct),0)
    , 'Act'
    , 0
    , 0
    , 0
FROM CogsProjection.dbo.BomStdVsAct_Det a
WHERE a.VerId = @VerId
  AND a.Room = @Room
  AND a.Tahun = @Year
  AND a.Bulan = @Month
  AND a.ProductIDPlan = @ProductID
GROUP BY
    a.ProductIDPlan,
    a.ProductNamePlan,
    a.ItemIdAct,
    a.ItemNameAct,
    a.UnitIdAct,
    a.GroupSubstitusiAct;

--Cari Pasangan ItemIdAct dan ItemIdPlan
DECLARE @PlanRowId INT, @p_ItemId NVARCHAR(20), @p_Group NVARCHAR(50), @ChosenAct INT;

DECLARE plan_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT RowId, ItemIdPlan, GroupSubstitusiPlan
FROM @Result
ORDER BY RowId;

OPEN plan_cursor;
FETCH NEXT FROM plan_cursor INTO @PlanRowId, @p_ItemId, @p_Group;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @ChosenAct = NULL;

    -- Priority 1: Group + Item
    SELECT TOP 1 @ChosenAct = ActRowId
    FROM @BomAct
    WHERE IsUsed = 0
      AND GroupSubstitusiAct = @p_Group
      AND ItemIdAct = @p_ItemId;

    -- Priority 2: Group only
    IF @ChosenAct IS NULL AND @p_Group IS NOT NULL AND LTRIM(RTRIM(@p_Group)) <> ''
        SELECT TOP 1 @ChosenAct = ActRowId
        FROM @BomAct
        WHERE IsUsed = 0 AND GroupSubstitusiAct = @p_Group;

    -- Priority 3: Item only
    IF @ChosenAct IS NULL
        SELECT TOP 1 @ChosenAct = ActRowId
        FROM @BomAct
        WHERE IsUsed = 0 AND ItemIdAct = @p_ItemId;

    IF @ChosenAct IS NOT NULL
    BEGIN
        UPDATE r
        SET AssignedActRowId = a.ActRowId
           , ItemIdAct = a.ItemIdAct
           , ItemNameAct = a.ItemNameAct
           , UnitIdAct = a.UnitIdAct
           , GroupSubstitusiAct = a.GroupSubstitusiAct
           , QtyAct = a.QtyAct
           , QtyInKgAct = a.QtyInKgAct
           , PriceAct = a.PriceAct
           , SourceAct = a.SourceAct
           , SalesQtyAct = a.SalesQtyAct
           , YieldAct = a.YieldAct
        FROM @Result r
        JOIN @BomAct a ON a.ActRowId = @ChosenAct
        WHERE r.RowId = @PlanRowId;

        UPDATE @BomAct SET IsUsed = 1 WHERE ActRowId = @ChosenAct;
    END

    FETCH NEXT FROM plan_cursor INTO @PlanRowId, @p_ItemId, @p_Group;
END

CLOSE plan_cursor;
DEALLOCATE plan_cursor;

INSERT INTO @Result (
    VerId, Room, Tahun, Bulan,
    AssignedActRowId,
    ItemIdAct, ItemNameAct, UnitIdAct,
    GroupSubstitusiAct,
    QtyAct, QtyInKgAct, PriceAct,
    SourceAct, SalesQtyAct, YieldAct
)
SELECT
    @VerId, @Room, @Year, @Month
    , ActRowId
    , ItemIdAct
	, ItemNameAct
	, UnitIdAct
    , GroupSubstitusiAct
    , QtyAct
	, QtyInKgAct
	, PriceAct
    , SourceAct
	, SalesQtyAct
	, YieldAct
FROM @BomAct
WHERE IsUsed = 0;


SELECT a.VerId
		, a.Room
		, a.Tahun
		, a.Bulan
		, a.ProductIDPlan as ProductID
		, a.ProductNamePlan as ProductName
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
		--, SalesQtyAct
		, SUM(b.SalesQtyAct) as SalesQtyAct
		, a.YieldAct

FROM @Result a
LEFT JOIN (SELECT ProductIdPlan
				--, ItemIdAct
				, InventBatchIdAct
				, MAX(SalesQtyAct) as SalesQtyAct
		   FROM CogsProjection.dbo.BomStdVsAct_Det
		   GROUP BY ProductIdPlan
				--, ItemIdAct
				, InventBatchIdAct
				) b ON b.ProductIDPlan = a.ProductIDPlan 
WHERE (a.ItemIdAct IS NOT NULL OR a.ItemIdPlan IS NOT NULL)
GROUP BY a.VerId
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
		, a.YieldAct
ORDER BY
    COALESCE(a.ItemIdPlan, a.ItemIdAct);



END

GO

