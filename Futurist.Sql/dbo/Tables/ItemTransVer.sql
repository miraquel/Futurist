CREATE TABLE [dbo].[ItemTransVer] (
    [RecId]        INT              NOT NULL,
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
    [CurrencyCode] NVARCHAR (20)    NOT NULL,
    [CurrencyRate] NUMERIC (32, 16) NOT NULL,
    [VerId]        INT              NOT NULL,
    [IdCol]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ItemTransVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_ItemTransVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemTransVer_VerId]
    ON [dbo].[ItemTransVer]([VerId] ASC, [ItemId] ASC)
    INCLUDE([RecId], [Qty], [Price]);


GO

