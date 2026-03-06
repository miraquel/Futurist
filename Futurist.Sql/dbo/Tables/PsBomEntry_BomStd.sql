CREATE TABLE [dbo].[PsBomEntry_BomStd] (
    [Room]        INT              NOT NULL,
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
    [FromDate]    DATETIME         NOT NULL,
    [ToDate]      DATETIME         NOT NULL,
    [CreatedBy]   NVARCHAR (20)    NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    [RecId]       INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_PsBomEntry_BomStd] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

