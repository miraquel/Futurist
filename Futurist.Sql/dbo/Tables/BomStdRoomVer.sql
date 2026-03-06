CREATE TABLE [dbo].[BomStdRoomVer] (
    [Room]        INT              NULL,
    [BomId]       NVARCHAR (20)    NOT NULL,
    [ProductId]   NVARCHAR (20)    NOT NULL,
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [ItemName]    NVARCHAR (60)    NOT NULL,
    [BomQty]      NUMERIC (32, 16) NOT NULL,
    [BomQtySerie] NUMERIC (32, 16) NOT NULL,
    [LineType]    INT              NOT NULL,
    [SubBomId]    NVARCHAR (20)    NOT NULL,
    [Ref]         NVARCHAR (20)    NOT NULL,
    [Level]       INT              NOT NULL,
    [FromDate]    DATETIME         NULL,
    [ToDate]      DATETIME         NULL,
    [CreatedBy]   NVARCHAR (20)    NULL,
    [CreatedDate] DATETIME         NULL,
    [RecId]       INT              NOT NULL,
    [VerId]       INT              NOT NULL,
    [IdCol]       INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_BomStdRoomVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_BomStdRoomVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

