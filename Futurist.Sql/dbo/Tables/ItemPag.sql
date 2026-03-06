CREATE TABLE [dbo].[ItemPag] (
    [RecId]          INT              IDENTITY (1, 1) NOT NULL,
    [ItemId]         NVARCHAR (20)    NOT NULL,
    [Pag]            NVARCHAR (20)    NOT NULL,
    [VendorId]       NVARCHAR (20)    NOT NULL,
    [EffectiveDate]  DATETIME         NOT NULL,
    [ExpirationDate] DATETIME         NOT NULL,
    [Qty]            NUMERIC (32, 16) NOT NULL,
    [QtyRem]         NUMERIC (32, 16) NOT NULL,
    [Unit]           NVARCHAR (20)    NOT NULL,
    [CurrencyCode]   NVARCHAR (20)    NOT NULL,
    [Price]          NUMERIC (32, 16) NOT NULL,
    CONSTRAINT [PK_ItemPag] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

