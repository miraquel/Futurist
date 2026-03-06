CREATE TABLE [dbo].[ItemYield] (
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [ItemName]    NVARCHAR (60)    NULL,
    [Yield]       NUMERIC (32, 16) NOT NULL,
    [CreatedBy]   NVARCHAR (20)    NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    [RecId]       INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ItemYield] PRIMARY KEY CLUSTERED ([ItemId] ASC)
);


GO

