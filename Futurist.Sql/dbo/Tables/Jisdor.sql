CREATE TABLE [dbo].[Jisdor] (
    [RecId]        INT              IDENTITY (1, 1) NOT NULL,
    [CurrencyCode] NVARCHAR (20)    NOT NULL,
    [JisdorDate]   DATETIME         NOT NULL,
    [Kurs]         NUMERIC (32, 16) NOT NULL,
    CONSTRAINT [PK_Jisdor] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

