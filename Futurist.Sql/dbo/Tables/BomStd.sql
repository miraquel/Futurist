CREATE TABLE [dbo].[BomStd] (
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
    [RecId]       INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_BomStd] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_BomStd_ProductId]
    ON [dbo].[BomStd]([ProductId] ASC)
    INCLUDE([Room], [ItemId], [SubBomId], [Level]);


GO

