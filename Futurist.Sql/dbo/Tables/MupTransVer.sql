CREATE TABLE [dbo].[MupTransVer] (
    [RecId]       INT NOT NULL,
    [Room]        INT NOT NULL,
    [MupId]       INT NOT NULL,
    [ItemTransId] INT NOT NULL,
    [VerId]       INT NOT NULL,
    [IdCol]       INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_MupTransVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_MupTransVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [NCI_MupTransVer_VerId]
    ON [dbo].[MupTransVer]([VerId] ASC)
    INCLUDE([MupId], [ItemTransId]);


GO

CREATE NONCLUSTERED INDEX [NCI_MupTransVer_MupId]
    ON [dbo].[MupTransVer]([MupId] ASC)
    INCLUDE([ItemTransId]);


GO

