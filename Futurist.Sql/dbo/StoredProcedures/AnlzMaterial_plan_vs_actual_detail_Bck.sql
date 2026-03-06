-- =============================================
-- Author:		<Putri>
-- Create date: <08 Des 2025>
-- Description:	<Analisa RM Plan vs Act>
-- =============================================
CREATE PROCEDURE [dbo].[AnlzMaterial_plan_vs_actual_detail_Bck] 
    @Room int = 1,
    @ItemId nvarchar(20) = '4200031',
    @Year int = 2025,
    @Month int = 07
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------------------
    -- Temp tables: tambahkan kolom pembantu
    ----------------------------------------------------------------
    DECLARE @BomPlan TABLE (
        RowId INT IDENTITY(1,1) PRIMARY KEY,   -- kunci lokal untuk loop
        VerId Int,
        Room int,
        Tahun int,
        Bulan int,
        ProductIDPlan nvarchar(20),
        ProductNamePlan nvarchar(60),
        ItemIdPlan nvarchar(20),
        ItemNamePlan nvarchar(60),
        UnitIdPlan nvarchar(20),
        GroupSubstitusiPlan nvarchar(50),
        QtyPlan numeric(32,16),
        QtyInKgPlan numeric(32,16),
        PricePlan numeric(32,16),
        [SourcePlan] nvarchar(20),
        SalesQtyPlan numeric(32,16),
        InventBatchIdPlan nvarchar(50),
        YieldPlan numeric(32,16),

        -- columns to store assigned ACT
        AssignedActRowId int NULL,
        ItemIdAct nvarchar(20) NULL,
        ItemNameAct nvarchar(60) NULL,
        UnitIdAct nvarchar(20) NULL,
        GroupSubstitusiAct nvarchar(50) NULL,
        QtyAct numeric(32,16) NULL,
        QtyInKgAct numeric(32,16) NULL,
        PriceAct numeric(32,16) NULL,
        SourceAct nvarchar(20) NULL,
        SalesQtyAct numeric(32,16) NULL,
        InventBatchIdAct nvarchar(50) NULL,
        YieldAct numeric(32,16) NULL
    );

    DECLARE @BomAct TABLE (
        ActRowId int PRIMARY KEY, -- we will populate using ROW_NUMBER in insert
        ProductIDAct nvarchar(20),
        ProductNameAct nvarchar(60),
        ItemIdAct nvarchar(20),
        ItemNameAct nvarchar(60),
        UnitIdAct nvarchar(20),
        GroupSubstitusiAct nvarchar(50),
        QtyAct numeric(32,16),
        QtyInKgAct numeric(32,16),
        PriceAct numeric(32,16),
        [SourceAct] nvarchar(20),
        SalesQtyAct numeric(32,16),
        InventBatchIdAct nvarchar(50),
        YieldAct numeric(32,16),
        IsUsed bit DEFAULT 0
    );

    ----------------------------------------------------------------
    -- Insert Plan 
    ----------------------------------------------------------------
    INSERT INTO @BomPlan (
        VerId, Room, Tahun, Bulan, ProductIDPlan, ProductNamePlan,
        ItemIdPlan, ItemNamePlan, UnitIdPlan, GroupSubstitusiPlan,
        QtyPlan, QtyInKgPlan, PricePlan, [SourcePlan], SalesQtyPlan,
        InventBatchIdPlan, YieldPlan
    )
    SELECT 
        a.VerId,
        a.Room,
        @Year,
        @Month,
        a.ItemId,
        a.ItemName,
        b.ItemId,
        c.SEARCHNAME,
        c.UNITID,
        ISNULL(d.VtaMpSubstitusiGroupId,''),
        b.BomQty,
        CASE WHEN c.UNITID = 'g' THEN b.BomQty * c.NETWEIGHT / 1000 ELSE b.BomQty END,
        e.PlanPrice,
        'Plan',
        0,
        '',
        f.Yield
    FROM CogsProjection.dbo.RofoVer a
    JOIN CogsProjection.dbo.BomStdRoomVer b ON b.ProductId = a.ItemId
    LEFT JOIN AXGMKDW.dbo.DimItem di ON di.ITEMID = a.ItemId
    JOIN AXGMKDW.dbo.DimItem c ON c.ITEMID = b.ItemId
    LEFT JOIN AXGMKDW.dbo.DimItemSubstitute d ON d.ItemId = b.ItemId
    JOIN (
        SELECT Room, VerId, [Year], [Month], [ItemId], UnitId,
               SUM(PlanQty) as PlanQty,
               SUM([PlanQty]*[PlanPrice]) / NULLIF(SUM([PlanQty]),0)  as [PlanPrice]
        FROM CogsProjection.dbo.MaterialPlan
        WHERE Room = @Room
          AND VerId = 68
          AND Year = @Year
          AND Month = @Month
        GROUP BY Room, VerId, Year, Month, ItemId, UnitId
    ) e ON e.Room = a.Room AND e.VerId = a.VerId AND e.ItemId = b.ItemId AND e.Year = YEAR(a.RofoDate) AND e.Month = MONTH(a.RofoDate)
    LEFT JOIN CogsProjection.dbo.FgCostVer f ON f.RofoId = a.RecId AND f.Room = a.Room AND f.VerId = a.VerId AND f.RofoDate = a.RofoDate
    WHERE a.Room = @Room
      AND a.VerId = 68
      AND a.ItemId = @ItemId
      AND YEAR(a.RofoDate) = @Year
      AND MONTH(a.RofoDate) = @Month
      AND b.Room = @Room
      AND b.VerId = 68
      AND b.ProductId = @ItemId
    ORDER BY b.ItemId ASC;

    ----------------------------------------------------------------
    -- Insert Act with ActRowId (use ROW_NUMBER for stable unique id)
    ----------------------------------------------------------------
    ;WITH ActSrc AS (
        SELECT 
            a.ProductId,
            b.SEARCHNAME AS ProductName,
            a.ItemId,
            c.SEARCHNAME AS ItemName,
            c.UNITID,
            d.VtaMpSubstitusiGroupId AS GroupSubstitusi,
            SUM(a.ActQty) AS ActQty,
            SUM(a.ActQty * c.NETWEIGHT / 1000) AS QtyInKg,
            SUM(a.ActQty * a.ActPrice) / NULLIF(SUM(a.ActQty),0) AS ActPrice,
            'Act' as [Source],
            a.SalesQty,
            a.InventBatchId,
            0 AS YieldVal
        FROM CogsProjection.dbo.MaterialAct a
        LEFT JOIN AXGMKDW.dbo.DimItem b On b.ITEMID = a.ProductId
        LEFT JOIN AXGMKDW.dbo.DimItem c On c.ITEMID = a.ItemId
        LEFT JOIN AXGMKDW.dbo.DimItemSubstitute d On d.ItemId = a.ItemId
        WHERE [Year] = @Year
          AND [Month] = @Month
          AND a.ProductId = @ItemId
           AND a.InventBatchId = '92525372' 
        GROUP BY a.ProductId, b.SEARCHNAME, a.ItemId, c.SEARCHNAME, c.UNITID, d.VtaMpSubstitusiGroupId, a.SalesQty, a.InventBatchId
    )
    INSERT INTO @BomAct (
        ActRowId, ProductIDAct, ProductNameAct, ItemIdAct, ItemNameAct, UnitIdAct, GroupSubstitusiAct,
        QtyAct, QtyInKgAct, PriceAct, [SourceAct], SalesQtyAct, InventBatchIdAct, YieldAct, IsUsed
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY ProductId, ItemId, InventBatchId) AS ActRowId,
        ProductId, ProductName, ItemId, ItemName, UnitId, GroupSubstitusi,
        ActQty, QtyInKg, ActPrice, [Source], SalesQty, InventBatchId, YieldVal, 0
    FROM ActSrc
    ORDER BY ProductId, ItemId, InventBatchId;

    ----------------------------------------------------------------
    -- Matching loop: for each plan row, find an ACT (priority 1->2->3),
    -- ensure ACT is still unused, assign and mark IsUsed=1 immediately.
    ----------------------------------------------------------------
    DECLARE @PlanRowId int;
    DECLARE @p_ProductIDPlan nvarchar(20);
    DECLARE @p_ItemIdPlan nvarchar(20);
    DECLARE @p_GroupSub nvarchar(50);
    DECLARE @p_InventBatchIdPlan nvarchar(50);

    -- cursor over plans (only plan rows that still have no AssignedActRowId)
    DECLARE plan_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT RowId, ProductIDPlan, ItemIdPlan, GroupSubstitusiPlan, InventBatchIdPlan
    FROM @BomPlan
    ORDER BY RowId;

    OPEN plan_cursor;
    FETCH NEXT FROM plan_cursor INTO @PlanRowId, @p_ProductIDPlan, @p_ItemIdPlan, @p_GroupSub, @p_InventBatchIdPlan;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        ----------------------------------------------------------------
        -- Try priority 1: GroupSubstitusi + ItemId exact
        ----------------------------------------------------------------
        DECLARE @ChosenActRowId int = NULL;

        SELECT TOP 1 @ChosenActRowId = ActRowId
        FROM @BomAct a
        WHERE ISNULL(a.IsUsed,0) = 0
          AND a.GroupSubstitusiAct = @p_GroupSub
          AND a.ItemIdAct = @p_ItemIdPlan
        ORDER BY a.ActRowId;  

        IF @ChosenActRowId IS NULL
        BEGIN
            ----------------------------------------------------------------
            -- Priority 2: Group only (but ACT must be unused)
            ----------------------------------------------------------------
            IF @p_GroupSub IS NOT NULL AND LTRIM(RTRIM(@p_GroupSub)) <> ''
            BEGIN
                SELECT TOP 1 @ChosenActRowId = ActRowId
                FROM @BomAct a
                WHERE ISNULL(a.IsUsed,0) = 0
                  AND a.GroupSubstitusiAct = @p_GroupSub
                ORDER BY a.ActRowId;
            END
        END

        IF @ChosenActRowId IS NULL
        BEGIN
            ----------------------------------------------------------------
            -- Priority 3: ItemId only (for plans without group OR fallback)
            ----------------------------------------------------------------
            SELECT TOP 1 @ChosenActRowId = ActRowId
            FROM @BomAct a
            WHERE ISNULL(a.IsUsed,0) = 0
              AND a.ItemIdAct = @p_ItemIdPlan
            ORDER BY a.ActRowId;
        END

        ----------------------------------------------------------------
        -- If we found an ACT, update plan row (AssignedActRowId + ACT fields) and mark ACT IsUsed=1.
        ----------------------------------------------------------------
        IF @ChosenActRowId IS NOT NULL
        BEGIN
            UPDATE p
            SET AssignedActRowId = a.ActRowId,
                ItemIdAct = a.ItemIdAct,
                ItemNameAct = a.ItemNameAct,
                UnitIdAct = a.UnitIdAct,
                GroupSubstitusiAct = a.GroupSubstitusiAct,
                QtyAct = a.QtyAct,
                QtyInKgAct = a.QtyInKgAct,
                PriceAct = a.PriceAct,
                SourceAct = a.SourceAct,
                SalesQtyAct = a.SalesQtyAct,
                InventBatchIdAct = a.InventBatchIdAct,
                YieldAct = a.YieldAct
            FROM @BomPlan p
            JOIN @BomAct a ON a.ActRowId = @ChosenActRowId
            WHERE p.RowId = @PlanRowId;

            -- mark ACT as used
            UPDATE @BomAct
            SET IsUsed = 1
            WHERE ActRowId = @ChosenActRowId;
        END
        -- otherwise leave plan with null assigned ACT

        FETCH NEXT FROM plan_cursor INTO @PlanRowId, @p_ProductIDPlan, @p_ItemIdPlan, @p_GroupSub, @p_InventBatchIdPlan;
    END

    CLOSE plan_cursor;
    DEALLOCATE plan_cursor;

    ----------------------------------------------------------------
    -- Final select: semua PLAN ditampilkan; ACT kolom NULL jika tidak ada match
    ----------------------------------------------------------------
    SELECT 
        VerId, Room, Tahun, Bulan,
        ProductIDPlan, ProductNamePlan,
        ItemIdPlan AS ItemPlan, ItemNamePlan AS ItemNamePlan,
        UnitIdPlan, GroupSubstitusiPlan AS GroupPlan,
        QtyPlan, QtyInKgPlan, PricePlan, YieldPlan,
        ItemIdAct, ItemNameAct, UnitIdAct, GroupSubstitusiAct,
        QtyAct, QtyInKgAct, PriceAct, SalesQtyAct, InventBatchIdAct,
        CASE WHEN QtyPlan IS NULL OR QtyPlan = 0 THEN NULL ELSE (QtyAct / NULLIF(QtyPlan,0)) END AS ContrQty,
        CASE WHEN PricePlan IS NULL OR PricePlan = 0 THEN NULL ELSE (PriceAct / NULLIF(PricePlan,0)) END AS ContrPrice
    FROM @BomPlan
    ORDER BY ItemIdAct, InventBatchIdAct asc






END

GO

