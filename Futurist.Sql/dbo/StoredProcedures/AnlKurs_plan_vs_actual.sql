-- =============================================
-- Author:		<Andi>
-- Create date: <15 Aug 2025>
-- Description:	<Analisa kurs plan vs actual>
-- EXEC AnlKurs_plan_vs_actual 66
-- =============================================
CREATE PROCEDURE [dbo].[AnlKurs_plan_vs_actual]
	@Version int = 66	
AS
BEGIN
	SELECT a.[VerId]
		,v.[Room]
		,a.[CurrencyCode]
		,a.[ValidFrom]
		,a.[ExchangeRate] as [KursPlan]
		,b.[Kurs] as [KursAct]
		,b.[Kurs]/a.[ExchangeRate] as [A/P]
  FROM [ExchangeRateVer] a
  JOIN [Version] v ON v.VerId = a.VerId
  JOIN (
	SELECT [CurrencyCode]
		,DATEFROMPARTS(YEAR(JisdorDate),MONTH(JisdorDate),1) as [JisdorDate]
		,AVG([Kurs]) as [Kurs]
	FROM [Jisdor]
	GROUP BY [CurrencyCode]
		,DATEFROMPARTS(YEAR(JisdorDate),MONTH(JisdorDate),1)
	) b ON b.[CurrencyCode] = a.[CurrencyCode]	AND b.[JisdorDate] = a.[ValidFrom]
  WHERE a.[VerId] = @Version
  ORDER BY [CurrencyCode] DESC, [ValidFrom] ASC
END

GO

