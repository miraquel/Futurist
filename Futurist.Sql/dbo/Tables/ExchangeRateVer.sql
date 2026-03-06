CREATE TABLE [dbo].[ExchangeRateVer] (
    [RecId]        INT              NOT NULL,
    [CurrencyCode] NVARCHAR (20)    NOT NULL,
    [ValidFrom]    DATETIME         NOT NULL,
    [ValidTo]      DATETIME         NOT NULL,
    [ExchangeRate] NUMERIC (32, 16) NOT NULL,
    [VerId]        INT              NOT NULL,
    [IdCol]        INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ExchangeRateVer] PRIMARY KEY CLUSTERED ([IdCol] ASC),
    CONSTRAINT [FK_ExchangeRateVer.Version_VerId] FOREIGN KEY ([VerId]) REFERENCES [dbo].[Version] ([VerId]) ON DELETE CASCADE
);


GO

