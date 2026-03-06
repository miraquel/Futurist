CREATE TABLE [dbo].[ItemAdjVer] (
    [RecId]       INT              NOT NULL,
    [Room]        INT              NULL,
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [AdjPrice]    NUMERIC (32, 16) NOT NULL,
    [RmPrice]     NUMERIC (32, 16) NULL,
    [PmPrice]     NUMERIC (32, 16) NULL,
    [CreatedBy]   NVARCHAR (20)    NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    [VerId]       INT              NOT NULL,
    [IdCol]       INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ItemAdjVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_ItemAdjVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

