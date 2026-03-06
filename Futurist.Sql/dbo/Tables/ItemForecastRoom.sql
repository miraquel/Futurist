CREATE TABLE [dbo].[ItemForecastRoom] (
    [RecId]              INT              NOT NULL,
    [Room]               INT              NOT NULL,
    [ItemId]             NVARCHAR (20)    NOT NULL,
    [Unit]               NVARCHAR (20)    NOT NULL,
    [ForecastDate]       DATETIME         NOT NULL,
    [ForecastPrice]      NUMERIC (32, 16) NOT NULL,
    [ForcedPrice]        NUMERIC (32, 16) NOT NULL,
    [LatestPurchaseDate] DATETIME         NULL,
    CONSTRAINT [PK_ItemForecastRoom] PRIMARY KEY CLUSTERED ([RecId] ASC, [Room] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemForecastRoom_ItemId]
    ON [dbo].[ItemForecastRoom]([ItemId] ASC)
    INCLUDE([ForecastDate], [LatestPurchaseDate], [ForcedPrice]);


GO

