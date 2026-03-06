CREATE TABLE [dbo].[BomStdVsAct_Sum] (
    [VerId]              INT              NULL,
    [Room]               INT              NULL,
    [Tahun]              INT              NULL,
    [Bulan]              INT              NULL,
    [ProductId]          NVARCHAR (20)    NULL,
    [ProductName]        NVARCHAR (60)    NULL,
    [ItemPlan]           NVARCHAR (20)    NULL,
    [ItemNamePlan]       NVARCHAR (60)    NULL,
    [UnitIdPlan]         NVARCHAR (20)    NULL,
    [GroupPlan]          NVARCHAR (20)    NULL,
    [QtyPlan]            NUMERIC (32, 16) NULL,
    [QtyInKgPlan]        NUMERIC (32, 16) NULL,
    [PricePlan]          NUMERIC (32, 16) NULL,
    [YieldPlan]          NUMERIC (32, 16) NULL,
    [ItemIdAct]          NVARCHAR (20)    NULL,
    [ItemNameAct]        NVARCHAR (60)    NULL,
    [UnitIdAct]          NVARCHAR (20)    NULL,
    [GroupSubstitusiAct] NVARCHAR (20)    NULL,
    [QtyAct]             NUMERIC (32, 16) NULL,
    [QtyInKgAct]         NUMERIC (32, 16) NULL,
    [PriceAct]           NUMERIC (32, 16) NULL,
    [SalesQtyAct]        NUMERIC (32, 16) NULL,
    [YieldAct]           NUMERIC (32, 16) NULL
);


GO

