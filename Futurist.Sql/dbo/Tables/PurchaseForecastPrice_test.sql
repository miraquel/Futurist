CREATE TABLE [dbo].[PurchaseForecastPrice_test] (
    [RecId]              INT              IDENTITY (1, 1) NOT NULL,
    [N]                  INT              NULL,
    [ItemId]             NVARCHAR (20)    NULL,
    [PurchUnit]          NVARCHAR (20)    NULL,
    [InventUnit]         NVARCHAR (20)    NULL,
    [DateTrans]          DATETIME         NULL,
    [Price]              NUMERIC (32, 16) NULL,
    [PriceUnitInvent]    NUMERIC (32, 16) NULL,
    [Source]             NVARCHAR (20)    NULL,
    [Slope]              NUMERIC (32, 16) NULL,
    [Intercept]          NUMERIC (32, 16) NULL,
    [Pearson]            NUMERIC (32, 16) NULL,
    [LatestPurchaseDate] DATETIME         NULL
);


GO

