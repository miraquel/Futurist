-- =============================================
-- Author:		<Andi>
-- Create date: <22 Aug 2025>
-- Description:	<Report Forecast>
--
-- =============================================
CREATE PROCEDURE [dbo].[Rpt_RmForecast]
	@Room int = 4,
	@Year int = 2025
AS
BEGIN
	SELECT @Year as [Year]
		,[Room]
		,a.ItemId
		,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as [ItemName]
		,i.[UnitId]
		,ISNULL(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
		,ISNULL(g.[GroupName],'') as [GroupProcurement]
		,ISNULL(a.LatestPurchaseDate,0) as [LatestPurchaseDate]
		,i.LATESTPRICE as [LatestPurchasePrice]
		,ISNULL([January],0) as [January]
		,ISNULL([February],0) as [February]
		,ISNULL([March],0) as [March]
		,ISNULL([April],0) as [March]
		,ISNULL([May],0) as [May]
		,ISNULL([June],0) as [June]
		,ISNULL([July],0) as [July]
		,ISNULL([August],0) as [August]
		,ISNULL([September],0) as [September]
		,ISNULL([October],0) as [October]
		,ISNULL([November],0) as [November]
		,ISNULL([December],0) as [December]
	FROM
		(
			SELECT [Room]
				,[ItemId]
				,[LatestPurchaseDate]
				,DATENAME(MONTH, [ForecastDate]) as [Month]
				,[ForecastPrice]
			FROM [ItemForecastRoom]
			WHERE YEAR([ForecastDate]) = @Year
				AND Room = @Room
				--AND [ItemId] LIKE '1%'
		) AS source
	PIVOT
	(
		MAX([ForecastPrice])
		FOR [Month] IN ([January], [February], [March], [April], [May], [June]
			,[July], [August], [September], [October], [November], [December])
	) AS a

	JOIN [AXGMKDW].dbo.[DimItem] i ON i.ItemId = a.ItemId 
	LEFT JOIN [AXGMKDW].dbo.[DimItemSubstitute] s ON s.ItemId = a.ItemId 
	LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = a.ItemId
	ORDER BY ITEMID ASC;
END

GO

