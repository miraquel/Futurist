CREATE TABLE [dbo].[FgPlanCostPrice] (
    [VerId]       INT              NULL,
    [Room]        INT              NULL,
    [Tahun]       INT              NULL,
    [Bulan]       INT              NULL,
    [ProductId]   NVARCHAR (20)    NULL,
    [ProductName] NVARCHAR (60)    NULL,
    [ValuePlan]   NUMERIC (32, 16) NULL,
    [ValueAct]    NUMERIC (32, 16) NULL
);


GO

