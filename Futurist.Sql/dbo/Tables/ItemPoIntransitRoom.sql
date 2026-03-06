CREATE TABLE [dbo].[ItemPoIntransitRoom] (
    [RecId]        INT              NOT NULL,
    [Room]         INT              NOT NULL,
    [ItemId]       NVARCHAR (20)    NOT NULL,
    [Po]           NVARCHAR (20)    NOT NULL,
    [DeliveryDate] DATETIME         NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [QtyRem]       NUMERIC (32, 16) NOT NULL,
    [Unit]         NVARCHAR (20)    NOT NULL,
    [CurrencyCode] NVARCHAR (20)    NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    CONSTRAINT [PK_ItemPoIntransitRoom] PRIMARY KEY CLUSTERED ([RecId] ASC, [Room] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemPoIntransitRoom_ItemId]
    ON [dbo].[ItemPoIntransitRoom]([ItemId] ASC)
    INCLUDE([QtyRem], [DeliveryDate]);


GO

