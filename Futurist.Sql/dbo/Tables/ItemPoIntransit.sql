CREATE TABLE [dbo].[ItemPoIntransit] (
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    [ItemId]       NVARCHAR (20)    NOT NULL,
    [Po]           NVARCHAR (20)    NOT NULL,
    [DeliveryDate] DATETIME         NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [QtyRem]       NUMERIC (32, 16) NOT NULL,
    [Unit]         NVARCHAR (20)    NOT NULL,
    [CurrencyCode] NVARCHAR (20)    NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    CONSTRAINT [PK_ItemPoIntransit] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

