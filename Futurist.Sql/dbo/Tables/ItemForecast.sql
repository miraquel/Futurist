CREATE TABLE [dbo].[ItemForecast] (
    [RecId]              INT              IDENTITY (1, 1) NOT NULL,
    [ItemId]             NVARCHAR (20)    NOT NULL,
    [Unit]               NVARCHAR (20)    NOT NULL,
    [ForecastDate]       DATETIME         NOT NULL,
    [ForecastPrice]      NUMERIC (32, 16) NOT NULL,
    [ForcedPrice]        NUMERIC (32, 16) NOT NULL,
    [LatestPurchaseDate] DATETIME         NULL,
    CONSTRAINT [PK_ItemForecast] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

