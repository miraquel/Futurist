CREATE TABLE [dbo].[SalesPrice] (
    [RecId]           INT              IDENTITY (1, 1) NOT NULL,
    [ItemId]          NVARCHAR (20)    NOT NULL,
    [PeriodDate]      DATETIME         NOT NULL,
    [Qty]             NUMERIC (32, 16) NOT NULL,
    [GrossSales]      NUMERIC (32, 16) NOT NULL,
    [DiscPercent]     NUMERIC (32, 16) NOT NULL,
    [DiscValue]       NUMERIC (32, 16) NOT NULL,
    [NetSales]        NUMERIC (32, 16) NOT NULL,
    [SalesPriceIndex] NUMERIC (32, 16) NOT NULL,
    CONSTRAINT [PK_SalesPrice] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

