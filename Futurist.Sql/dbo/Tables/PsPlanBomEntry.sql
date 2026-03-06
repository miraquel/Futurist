CREATE TABLE [dbo].[PsPlanBomEntry] (
    [Room]       INT              NOT NULL,
    [PlanDate]   DATETIME         NOT NULL,
    [ItemId]     NVARCHAR (20)    NOT NULL,
    [ItemName]   NVARCHAR (60)    NOT NULL,
    [Qty]        NUMERIC (32, 16) NOT NULL,
    [BomIdEntry] NVARCHAR (20)    NOT NULL,
    [BomId]      NVARCHAR (20)    NULL,
    [BomId6]     NVARCHAR (20)    NULL,
    [RecId]      INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_PsPlanBomEntry] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

