CREATE TABLE [dbo].[RofoVer] (
    [Room]        INT              NOT NULL,
    [RofoDate]    DATETIME         NOT NULL,
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [ItemName]    NVARCHAR (60)    NOT NULL,
    [Qty]         NUMERIC (32, 16) NOT NULL,
    [QtyRem]      NUMERIC (32, 16) NOT NULL,
    [SalesPrice]  NUMERIC (32, 16) CONSTRAINT [DF_RofoVer_SalesPrice] DEFAULT ((0)) NOT NULL,
    [CreatedBy]   NVARCHAR (20)    NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    [RecId]       INT              NOT NULL,
    [VerId]       INT              NOT NULL,
    [IdCol]       INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_RofoVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_RofoVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [NCI_RofoVer_Room]
    ON [dbo].[RofoVer]([Room] ASC, [VerId] ASC)
    INCLUDE([RofoDate], [RecId]);


GO

