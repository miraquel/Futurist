CREATE TABLE [dbo].[ItemOnhandRoom] (
    [RecId]              INT              NOT NULL,
    [Room]               INT              NOT NULL,
    [ItemId]             NVARCHAR (20)    NOT NULL,
    [InventBatch]        NVARCHAR (20)    NOT NULL,
    [ExpDate]            DATETIME         NOT NULL,
    [PdsDispositionCode] NVARCHAR (20)    NULL,
    [Qty]                NUMERIC (32, 16) NOT NULL,
    [QtyRem]             NUMERIC (32, 16) NULL,
    [Price]              NUMERIC (32, 16) NOT NULL,
    [RmPrice]            NUMERIC (32, 16) NOT NULL,
    [PmPrice]            NUMERIC (32, 16) NOT NULL,
    [StdcostPrice]       NUMERIC (32, 16) NOT NULL,
    CONSTRAINT [PK_ItemOnhandRoom] PRIMARY KEY CLUSTERED ([RecId] ASC, [Room] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_ItemOnhandRoom_ItemId]
    ON [dbo].[ItemOnhandRoom]([ItemId] ASC)
    INCLUDE([ExpDate], [QtyRem], [RmPrice], [PmPrice], [StdcostPrice]);


GO

