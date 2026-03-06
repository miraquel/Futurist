CREATE TABLE [dbo].[BomStdVsAct_Det] (
    [VerId]              INT              NULL,
    [Room]               INT              NULL,
    [Tahun]              INT              NULL,
    [Bulan]              INT              NULL,
    [ProductIdPlan]      NVARCHAR (20)    NULL,
    [ProductNamePlan]    NVARCHAR (60)    NULL,
    [ItemPlan]           NVARCHAR (20)    NULL,
    [ItemNamePlan]       NVARCHAR (60)    NULL,
    [UnitIdPlan]         NVARCHAR (20)    NULL,
    [GroupPlan]          NVARCHAR (20)    NULL,
    [QtyPlan]            NUMERIC (32, 16) NULL,
    [QtyInKgPlan]        NUMERIC (32, 16) NULL,
    [PricePlan]          NUMERIC (32, 16) NULL,
    [ValuePlan]          NUMERIC (32, 16) NULL,
    [YieldPlan]          NUMERIC (32, 16) NULL,
    [ItemIdAct]          NVARCHAR (20)    NULL,
    [ItemNameAct]        NVARCHAR (60)    NULL,
    [UnitIdAct]          NVARCHAR (20)    NULL,
    [GroupSubstitusiAct] NVARCHAR (20)    NULL,
    [QtyAct]             NUMERIC (32, 16) NULL,
    [QtyInKgAct]         NUMERIC (32, 16) NULL,
    [PriceAct]           NUMERIC (32, 16) NULL,
    [ValueAct]           NUMERIC (32, 16) NULL
);


GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-20260204-133407]
    ON [dbo].[BomStdVsAct_Det]([VerId] ASC, [Room] ASC, [Tahun] ASC, [Bulan] ASC);


GO

