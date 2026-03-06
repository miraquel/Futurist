CREATE TABLE [dbo].[ItemPoIntransitVer] (
    [RecId]        INT              NOT NULL,
    [ItemId]       NVARCHAR (20)    NOT NULL,
    [Po]           NVARCHAR (20)    NOT NULL,
    [DeliveryDate] DATETIME         NOT NULL,
    [Qty]          NUMERIC (32, 16) NOT NULL,
    [QtyRem]       NUMERIC (32, 16) NOT NULL,
    [Unit]         NVARCHAR (20)    NOT NULL,
    [Price]        NUMERIC (32, 16) NOT NULL,
    [Room]         INT              NOT NULL,
    [VerId]        INT              NOT NULL,
    [IdCol]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ItemPoIntransitVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_ItemPoIntransitVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

