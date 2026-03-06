CREATE TABLE [dbo].[FgCostVer] (
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
    [RecId]        INT              NOT NULL,
    [VerId]        INT              NOT NULL,
    [IdCol]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_FgCostVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_FgCostVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

CREATE NONCLUSTERED INDEX [NCI_FgCostVer_VerId]
    ON [dbo].[FgCostVer]([VerId] ASC)
    INCLUDE([ProductId], [RofoDate], [QtyRofo], [Yield], [RmPrice], [PmPrice], [StdCostPrice], [CostPrice]);


GO

CREATE NONCLUSTERED INDEX [NCI_FgCostVer_ProductId_RofoDate_VerId]
    ON [dbo].[FgCostVer]([ProductId] ASC, [RofoDate] ASC, [VerId] ASC)
    INCLUDE([QtyRofo], [RmPrice], [PmPrice], [StdCostPrice], [CostPrice]);


GO

