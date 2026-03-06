CREATE TABLE [dbo].[ExchangeRateRoom] (
    [RecId]        INT              NOT NULL,
    [Room]         INT              NOT NULL,
    [CurrencyCode] NVARCHAR (20)    NOT NULL,
    [ValidFrom]    DATETIME         NOT NULL,
    [ValidTo]      DATETIME         NOT NULL,
    [ExchangeRate] NUMERIC (32, 16) NOT NULL,
    [CreatedDate]  DATETIME         NULL,
    [CreatedBy]    NVARCHAR (20)    NULL,
    CONSTRAINT [PK_ExchangeRateRoom] PRIMARY KEY CLUSTERED ([RecId] ASC, [Room] ASC)
);


GO

CREATE NONCLUSTERED INDEX [NCI_ExchangeRateRoom_CurrencyCode]
    ON [dbo].[ExchangeRateRoom]([CurrencyCode] ASC)
    INCLUDE([ValidFrom], [ValidTo]);


GO

