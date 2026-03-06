-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Exchange Rate Select
-- EXEC ExchangeRate
-- =============================================
CREATE PROCEDURE [dbo].[ExchangeRate_Select]
	@StartDate datetime = '1 Jan 2025',
	@EndDate datetime = '1 Dec 2025'
AS
BEGIN
	SET NOCOUNT ON;

	SET @StartDate = DATEFROMPARTS(YEAR(GETDATE()),1,1)
	SET @EndDate = DATEFROMPARTS(YEAR(GETDATE()),12,31)

	SELECT [CurrencyCode]
		,[ValidFrom]
		,[ValidTo]
		,[ExchangeRate]
	FROM [ExchangeRate]
	WHERE [ValidFrom] BETWEEN @StartDate AND @EndDate
		OR [ValidTo] BETWEEN @StartDate AND @EndDate
	ORDER BY [CurrencyCode], [ValidFrom]
END

GO

