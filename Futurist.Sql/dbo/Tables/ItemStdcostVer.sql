CREATE TABLE [dbo].[ItemStdcostVer] (
    [RecId]  INT              NOT NULL,
    [ItemId] NVARCHAR (20)    NOT NULL,
    [Price]  NUMERIC (32, 16) NOT NULL,
    [Room]   INT              NOT NULL,
    [VerId]  INT              NOT NULL,
    [IdCol]  INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ItemStdcostVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_ItemStdcostVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

