-- =============================================
-- Author:		Andi
-- Create date: 8 Apr 2025
-- Description:	FgCostVer_Select
-- EXEC [FgCostVer_Select] 1,76
-- durasi 0 detik utk 3328 rows
-- =============================================
CREATE PROCEDURE [dbo].[FgCostVer_Select]
	@Room int = 1,
	@VerId int = 63
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @VerIdPrev int

	SELECT @VerIdPrev = MAX(VerId) 
	FROM [Version] 
	WHERE [Room] = @Room  
		AND [VerId] <> @VerId
		AND [Cancel] = 0	--43

	SELECT a.[Room]
		,a.VerId
		,a.[RecId] as [RofoId]
		  ,a.[ItemId] as [ProductId]
		  ,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,char(9),''),char(10),''),char(13),'') as [ProductName]
		  ,i.UNITID as [Unit]
		  ,i.NETWEIGHT/1000 as [UnitInKg]	
		  ,ISNULL(g.[Group03],'NA') as [GroupFg]
		  ,a.[RofoDate]	 
		  ,b.[QtyRofo] as [RofoQty]
		  ,b.[Yield]
		  ,b.[RmPrice]
		  ,b.[PmPrice]
		  ,b.[StdCostPrice]
		  ,b.[CostPrice] as [CostPrice]
		  ,a.[SalesPrice] as [SalesPrice]
		  ,(b.[RmPrice] + b.[PmPrice]) / NULLIF(a.[SalesPrice],0) as [Ratio RMPM/S]
		  ,b.[CostPrice] / NULLIF(a.[SalesPrice],0) as [Ratio CP/S]
		  ,'=>' as [Previous Calc]
		  ,c.[QtyRofo] as [RofoQty Prev]
		  ,c.[Yield] as [Yield Prev]
		  ,c.[RmPrice] as [Rm Prev]
		  ,c.[PmPrice] as [Pm Prev]
		  ,c.[StdCostPrice] as [StdCost Prev]
		  ,c.[CostPrice] as [CostPrice Prev]
		  ,ABS((b.[CostPrice]-c.[CostPrice]) / NULLIF(c.[CostPrice],0) ) as [Delta Absolute]
	  FROM [RofoVer] a with(nolock)
	  JOIN AXGMKDW.dbo.DimItem i  with(nolock) ON i.ITEMID = a.ItemId
	  LEFT JOIN [FgCostVer] b ON b.ProductId = a.[ItemId] AND b.[RofoDate] = a.[RofoDate] AND b.VerId = @VerId	--63	--
	  LEFT JOIN [SalesPriceVer] p ON p.ItemId = a.ItemId AND p.VerId = @VerId	--63	--
	  LEFT JOIN [FgCostVer] c ON c.ProductId = a.[ItemId] AND c.[RofoDate] = a.[RofoDate] AND c.VerId = @VerIdPrev	--50	--
	  LEFT JOIN [ItemGroup] g ON g.ItemId =  a.[ItemId]
	  WHERE a.[VerId] = @VerId	--63
	  ORDER BY a.RofoDate ASC,(b.[QtyRofo]*b.COSTPRICE) DESC
END

GO

