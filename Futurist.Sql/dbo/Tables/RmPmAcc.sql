CREATE TABLE [dbo].[RmPmAcc] (
    [Year]              INT              NOT NULL,
    [Month]             INT              NOT NULL,
    [RmPmOpening]       NUMERIC (32, 16) NOT NULL,
    [RmPmPurchase]      NUMERIC (32, 16) NOT NULL,
    [RmPmEnding]        NUMERIC (32, 16) NOT NULL,
    [RmPmUsed]          NUMERIC (32, 16) NOT NULL,
    [WipOpening]        NUMERIC (32, 16) NOT NULL,
    [WipOpeningRmPm]    NUMERIC (32, 16) NOT NULL,
    [WipOpeningStdCost] NUMERIC (32, 16) NOT NULL,
    [WipEnding]         NUMERIC (32, 16) NOT NULL,
    [WipEndingRmPm]     NUMERIC (32, 16) NOT NULL,
    [WipEndingStdCost]  NUMERIC (32, 16) NOT NULL,
    [FgOpening]         NUMERIC (32, 16) NOT NULL,
    [FgOpeningRmPm]     NUMERIC (32, 16) NOT NULL,
    [FgOpeningStdCost]  NUMERIC (32, 16) NOT NULL,
    [FgEnding]          NUMERIC (32, 16) NOT NULL,
    [FgEndingRmPm]      NUMERIC (32, 16) NOT NULL,
    [FgEndingStdCost]   NUMERIC (32, 16) NOT NULL,
    [StdCostAct]        NUMERIC (32, 16) NOT NULL,
    [RmPmGross]         NUMERIC (32, 16) NOT NULL,
    [RmPmNet]           NUMERIC (32, 16) NOT NULL,
    [SalesNet]          NUMERIC (32, 16) NOT NULL
);


GO

