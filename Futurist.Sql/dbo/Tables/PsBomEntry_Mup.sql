CREATE TABLE [dbo].[PsBomEntry_Mup] (
    [Room]     INT              NOT NULL,
    [PlanDate] DATETIME         NOT NULL,
    [ItemId]   NVARCHAR (20)    NOT NULL,
    [Qty]      NUMERIC (32, 16) NOT NULL,
    [RecId]    INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_PsBomEntry_Mup] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

