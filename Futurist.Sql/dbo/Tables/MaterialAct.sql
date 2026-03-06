CREATE TABLE [dbo].[MaterialAct] (
    [Year]          INT              NOT NULL,
    [Month]         INT              NOT NULL,
    [ItemId]        NVARCHAR (20)    NOT NULL,
    [Source]        NVARCHAR (50)    NOT NULL,
    [ActQty]        NUMERIC (32, 16) NOT NULL,
    [ActPrice]      NUMERIC (32, 16) NOT NULL,
    [ProductId]     NVARCHAR (20)    NULL,
    [InventBatchId] NVARCHAR (20)    NULL,
    [SalesQty]      NUMERIC (32, 16) NULL,
    [ProdId]        NVARCHAR (20)    NULL,
    [RecId]         INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_MaterialAct] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

