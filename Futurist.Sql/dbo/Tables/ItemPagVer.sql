CREATE TABLE [dbo].[ItemPagVer] (
    [RecId]          INT              NOT NULL,
    [ItemId]         NVARCHAR (20)    NOT NULL,
    [Pag]            NVARCHAR (20)    NOT NULL,
    [VendorId]       NVARCHAR (20)    NOT NULL,
    [EffectiveDate]  DATETIME         NOT NULL,
    [ExpirationDate] DATETIME         NOT NULL,
    [Qty]            NUMERIC (32, 16) NOT NULL,
    [QtyRem]         NUMERIC (32, 16) NOT NULL,
    [Unit]           NVARCHAR (20)    NOT NULL,
    [CurrencyCode]   NVARCHAR (20)    NOT NULL,
    [Price]          NUMERIC (32, 16) NOT NULL,
    [Room]           INT              NOT NULL,
    [VerId]          INT              NOT NULL,
    [IdCol]          INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ItemPagVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_ItemPagVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

