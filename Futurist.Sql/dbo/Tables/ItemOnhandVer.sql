CREATE TABLE [dbo].[ItemOnhandVer] (
    [RecId]              INT              NOT NULL,
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
    [Room]               INT              NOT NULL,
    [VerId]              INT              NOT NULL,
    [IdCol]              INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ItemOnhandVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_ItemOnhandVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

