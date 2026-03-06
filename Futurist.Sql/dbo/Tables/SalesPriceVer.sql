CREATE TABLE [dbo].[SalesPriceVer] (
    [RecId]           INT              NOT NULL,
    [Room]            INT              NOT NULL,
    [ItemId]          NVARCHAR (20)    NOT NULL,
    [PeriodDate]      DATETIME         NOT NULL,
    [SalesPriceIndex] NUMERIC (32, 16) NOT NULL,
    [VerId]           INT              NOT NULL,
    [IdCol]           INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_SalesPriceVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_SalesPriceVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

