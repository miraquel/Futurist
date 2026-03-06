CREATE TABLE [dbo].[MupVer] (
    [Room]      INT              NOT NULL,
    [MupDate]   DATETIME         NOT NULL,
    [ItemId]    NVARCHAR (20)    NOT NULL,
    [ItemGroup] NVARCHAR (20)    NOT NULL,
    [Qty]       NUMERIC (32, 16) NOT NULL,
    [QtyRem]    NUMERIC (32, 16) NOT NULL,
    [RofoId]    INT              NOT NULL,
    [RecId]     INT              NOT NULL,
    [VerId]     INT              NOT NULL,
    CONSTRAINT [FK_MupVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [NCI_MupVer_[RofoId]
    ON [dbo].[MupVer]([RofoId] ASC, [VerId] ASC)
    INCLUDE([ItemId], [RecId]);


GO

CREATE NONCLUSTERED INDEX [NCI_MupVer_VerId]
    ON [dbo].[MupVer]([VerId] ASC)
    INCLUDE([ItemId], [RofoId], [RecId]);


GO

