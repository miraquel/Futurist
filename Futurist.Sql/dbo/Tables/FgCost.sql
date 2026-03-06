CREATE TABLE [dbo].[FgCost] (
    [Room]         INT              NOT NULL,
    [RofoId]       INT              NOT NULL,
    [ProductId]    NVARCHAR (20)    NOT NULL,
    [ProductName]  NVARCHAR (60)    NOT NULL,
    [RofoDate]     DATETIME         NOT NULL,
    [QtyRofo]      NUMERIC (32, 16) NOT NULL,
    [Yield]        NUMERIC (32, 16) NOT NULL,
    [RmPrice]      NUMERIC (32, 16) NOT NULL,
    [PmPrice]      NUMERIC (32, 16) NOT NULL,
    [StdCostPrice] NUMERIC (32, 16) NOT NULL,
    [CostPrice]    NUMERIC (32, 16) NOT NULL,
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_FgCost] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_FgCost_Room]
    ON [dbo].[FgCost]([Room] ASC);


GO

