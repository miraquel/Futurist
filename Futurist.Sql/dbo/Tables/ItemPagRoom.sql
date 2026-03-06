CREATE TABLE [dbo].[ItemPagRoom] (
    [RecId]          INT              NOT NULL,
    [Room]           INT              NOT NULL,
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
    CONSTRAINT [PK_ItemPagRoom] PRIMARY KEY CLUSTERED ([RecId] ASC, [Room] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemPagRoom_ItemId]
    ON [dbo].[ItemPagRoom]([ItemId] ASC);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemPagRoom_RecId]
    ON [dbo].[ItemPagRoom]([RecId] ASC)
    INCLUDE([Room]);


GO

