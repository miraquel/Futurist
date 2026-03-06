-- =============================================
-- Author:		<Andi>
-- Create date: <27 May 2025>
-- Description:	<RMPM(Cleansing) versi Accounting>
-- EXEC [RmPmAcc_Select] 2025
-- =============================================
CREATE PROCEDURE [dbo].[RmPmAcc_Select]
	@Year int = 2025
AS
BEGIN
	SELECT [Year]
		,[Month]
		,[RmPmOpening]
		,[RmPmPurchase]
		,[RmPmEnding]
		,[RmPmUsed]
		,[WipOpening]
		,[WipOpeningRmPm]
		,[WipOpeningStdCost]
		,[WipEnding]
		,[WipEndingRmPm]
		,[WipEndingStdCost]
		,[FgOpening]
		,[FgOpeningRmPm]
		,[FgOpeningStdCost]
		,[FgEnding]
		,[FgEndingRmPm]
		,[FgEndingStdCost]
		,[StdCostAct]
		,[RmPmGross]
		,[RmPmNet]
		,[SalesNet]
		,[RmPmNet]/[SalesNet] as [%RmPm]
	FROM [RmPmAcc]
	WHERE [Year] = @Year
	ORDER BY [Month]
END

GO

