CREATE TABLE [dbo].[ExchangeRate] (
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    [CurrencyCode] NVARCHAR (20)    NOT NULL,
    [ValidFrom]    DATETIME         NOT NULL,
    [ValidTo]      DATETIME         NOT NULL,
    [ExchangeRate] NUMERIC (32, 16) NOT NULL,
    [CreatedDate]  DATETIME         NULL,
    [CreatedBy]    NVARCHAR (20)    NULL,
    CONSTRAINT [PK_ExchangeRate] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

