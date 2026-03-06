-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	FgCost Select
-- EXEC FgCost_Select 99
-- durasi: 0 detik utk 4125 rows
-- =============================================
CREATE PROCEDURE [dbo].[FgCost_Select]
	@Room int = 1
AS
BEGIN
	SET NOCOUNT ON;

	SELECT a.[Room]
		,a.[RofoId]
		,a.[ProductId]
		,a.[ProductName]
		,i.UNITID as [Unit]
		,i.NETWEIGHT / 1000 as [UnitInKg]
		,ISNULL(g.[Group03],'NA') as [GroupFg]
		,a.[RofoDate]
		,a.[QtyRofo] as [RofoQty]
		,a.[Yield]
		,a.[RmPrice]
		,a.[PmPrice]
		,a.[StdCostPrice]
		,a.[CostPrice]
		,ISNULL(s.[SalesPrice],0) as [SalesPrice]
		,(a.[RmPrice] + a.[PmPrice]) / NULLIF(s.[SalesPrice],0) as [Ratio RMPM/S]
		,a.[CostPrice] / NULLIF(s.[SalesPrice],0) as [Ratio CP/S]
		FROM [FgCost] a WITH (NOLOCK) 
		JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.[ProductId]
		--LEFT JOIN (
		--	SELECT x.[ItemId], x.[SalesPriceIndex]
		--	FROM ( 
		--		SELECT a.ItemId, a.[SalesPriceIndex]
		--		FROM [SalesPrice] a WITH (NOLOCK) 
		--		JOIN (
		--			SELECT [ItemId], MAX([PeriodDate]) [MaxPeriodDate]
		--			FROM [SalesPrice] WITH (NOLOCK) 
		--			GROUP BY [ItemId]
		--		) b ON b.ItemId = a.ItemId AND b.[MaxPeriodDate] = a.[PeriodDate]
		--	) x			
		--) s ON s.ItemId = a.[ProductId]
		LEFT JOIN RofoRoom s ON s.ItemId = a.[ProductId]
			AND s.RecId = a.RofoId
		LEFT JOIN [ItemGroup] g ON g.ItemId =  a.[ProductId]
		WHERE a.[Room] = @Room
		ORDER BY DATEFROMPARTS(YEAR(a.[RofoDate]),MONTH(a.[RofoDate]),1) asc,  a.[QtyRofo] DESC
END

GO

