CREATE TABLE [dbo].[FgActNotBreakDown] (
    [Year]        INT              NOT NULL,
    [Month]       INT              NOT NULL,
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [InventBatch] NVARCHAR (20)    NOT NULL,
    [ProdId]      NVARCHAR (20)    NOT NULL,
    [Qty]         NUMERIC (32, 16) NOT NULL,
    [RecId]       INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_FgActNotBreakDown] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

