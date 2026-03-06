CREATE TABLE [dbo].[PurchaseForecastPrice] (
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
    [LatestPurchaseDate] DATETIME         NULL,
    CONSTRAINT [PK_PurchaseForecastPrice] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_PurchaseForecastPrice_ItemId]
    ON [dbo].[PurchaseForecastPrice]([ItemId] ASC)
    INCLUDE([Source], [DateTrans], [PriceUnitInvent]);


GO

