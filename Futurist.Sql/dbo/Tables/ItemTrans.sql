CREATE TABLE [dbo].[ItemTrans] (
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    [Room]         INT              NOT NULL,
    [ItemId]       NVARCHAR (20)    NOT NULL,
    [InventBatch]  NVARCHAR (20)    NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [RmPrice]      NUMERIC (32, 16) NOT NULL,
    [PmPrice]      NUMERIC (32, 16) NOT NULL,
    [StdCostPrice] NUMERIC (32, 16) NOT NULL,
    [Source]       NVARCHAR (20)    NOT NULL,
    [RefId]        INT              NOT NULL,
    [CurrencyCode] NVARCHAR (20)    CONSTRAINT [DF_ItemTrans_CurrencyCode] DEFAULT ('') NOT NULL,
    [CurrencyRate] NUMERIC (32, 16) CONSTRAINT [DF_ItemTrans_CurrencyRate] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ItemTrans] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemTrans_Source]
    ON [dbo].[ItemTrans]([Source] ASC)
    INCLUDE([RecId], [ItemId], [Price], [RefId]);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemTrans_ItemId]
    ON [dbo].[ItemTrans]([ItemId] ASC)
    INCLUDE([RefId], [Source], [Room], [Price]);


GO

