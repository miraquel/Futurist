CREATE TABLE [dbo].[MaterialPlan] (
    [Room]        INT              NOT NULL,
    [VerId]       INT              NOT NULL,
    [Year]        INT              NOT NULL,
    [Month]       INT              NOT NULL,
    [ItemId]      NVARCHAR (20)    NOT NULL,
    [ItemName]    NVARCHAR (60)    NOT NULL,
    [UnitId]      NVARCHAR (20)    NOT NULL,
    [Source]      NVARCHAR (50)    NOT NULL,
    [InventBatch] NVARCHAR (20)    NOT NULL,
    [PlanQty]     NUMERIC (32, 16) NOT NULL,
    [PlanPrice]   NUMERIC (32, 16) NOT NULL,
    [RecId]       INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_MaterialPlan] PRIMARY KEY CLUSTERED ([RecId] ASC)
);


GO

