-- =============================================
-- Author:		<Andi>
-- Create date: <24 Jun 2025>
-- Description:	<Rofo vs Sales Order>
-- =============================================
CREATE PROCEDURE [RofoVsSo_Select] 
	@Room int = 1,
	@VerId int = 63,
	@PeriodDate datetime = '1 May 2025'
AS
BEGIN
	DECLARE @StartDate datetime, @EndDate datetime
	DECLARE @Kurs numeric(32,16)

	SET @StartDate = @PeriodDate
	SET @EndDate = DATEADD(MONTH,1,@StartDate) - 1

	SELECT a.[Room]
		,a.VerId
		,a.[RecId] as [RofoId]
		,a.[ItemId] as [ProductId]
		,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,char(9),''),char(10),''),char(13),'') as [ProductName]
		,i.UNITID as [Unit]
		,i.NETWEIGHT/1000 as [UnitInKg]	
		,a.[RofoDate]	 
		,a.[Qty] as [RofoQty]		 
		,a.[Qty]*i.NETWEIGHT/1000 as [RofoInKg]		 
		,ISNULL(b.SoQty,0) as [SoQty]
		,ISNULL(b.SoInKg,0)  as [SoInKg]
		,ISNULL(c.DoQty,0)  as [DoQty]
		,ISNULL(c.DoInKg,0)  as [DoInKg]
		,ABS((ISNULL(c.DoInKg,0) - (a.[Qty]*i.NETWEIGHT/1000)) / ((ISNULL(c.DoInKg,0) + (a.[Qty]*i.NETWEIGHT/1000)) / 2)) as [Mape]

	FROM [RofoVer] a with(nolock)
	JOIN AXGMKDW.dbo.DimItem i  with(nolock) ON i.ITEMID = a.ItemId
	LEFT JOIN (
		SELECT o.ItemId, SUM(o.SALESQTY) as [SoQty], SUM(o.SALESQTY*i.NETWEIGHT/1000) as [SoInKg]
		FROM AXGMKDW.dbo.FactSO o
		JOIN AXGMKDW.dbo.DimItem i with(nolock) ON i.ITEMID = o.ItemId
		WHERE o.SHIPPINGDATEREQUESTED BETWEEN @StartDate AND @EndDate 
		GROUP BY o.ItemId
	) b ON b.ItemId = a.ItemId
	LEFT JOIN (
		SELECT d.ItemId, SUM(d.QTY) as [DoQty], SUM(d.QTY*i.NETWEIGHT/1000) as [DoInKg]
		FROM AXGMKDW.dbo.FactDo d
		JOIN AXGMKDW.dbo.DimItem i with(nolock) ON i.ITEMID = d.ItemId
		WHERE d.DELIVERYDATE BETWEEN @StartDate AND @EndDate 
		GROUP BY d.ItemId
	) c ON c.ItemId = a.ItemId
	WHERE a.[VerId] = 63
		AND a.[Room] = 1
		AND a.[RofoDate] BETWEEN @StartDate AND @EndDate 
	ORDER BY a.RofoDate ASC,[RofoInKg] DESC





	-- 832	
END

GO

