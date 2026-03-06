-- =============================================
-- Author:		<Putri>
-- Create date: <Jan 2026>
-- Description:	<Summary FG Plan vs Act Calc>
-- =============================================
CREATE PROCEDURE [dbo].[FgPlanCostPrice_Insert_bck]

@Room int = 1
--, @ProductID nvarchar(20) = 4200031
, @Year int = 2025
, @Month int = 11
, @VerId int = 81

AS
BEGIN

DELETE FROM CogsProjection.dbo.FgPlanCostPrice 
WHERE Room = @Room
AND Tahun = @Year
AND Bulan = @Month
AND VerId = @VerId

INSERT INTO CogsProjection.dbo.FgPlanCostPrice
SELECT a.VerId
		, a.Room
		, a.Tahun
		, a.Bulan
		, a.ProductIdPlan as ProductID
		, a.ProductNamePlan as ProductName
		, SUM(a.ValuePlan) as ValuePlan
		, b.ValueAct
FROM CogsProjection.dbo.BomStdVsAct_Det a
LEFT JOIN ( SELECT a.VerId
				, a.Room
				, a.Tahun
				, a.Bulan
				, a.ProductIdPlan as ProductIdAct
				, SUM(a.ValueAct) as ValueAct
			FROM CogsProjection.dbo.BomStdVsAct_Det a
			WHERE LEFT(a.ItemIdAct, 1) IN (1,6)
			AND a.Room = @Room
			AND a.Tahun = @Year
			AND a.Bulan = @Month
			AND a.VerId = @VerId
			GROUP BY a.VerId
					, a.Room
					, a.Tahun
					, a.Bulan
					, a.ProductIdPlan
			--ORDER BY a.ProductIdPlan
) b ON b.ProductIdAct = a.ProductIdPlan
WHERE a.ItemPlan LIKE '1%'
AND a.Room = @Room
AND a.Tahun = @Year
AND a.Bulan = @Month
AND a.VerId = @VerId
AND b.ValueAct IS NOT NULL
--AND a.ProductIdPlan = '4200031'
GROUP BY a.VerId
		, a.Room
		, a.Tahun
		, a.Bulan
		, a.ProductIdPlan 
		, a.ProductNamePlan
		, b.ValueAct

SELECT * FROM CogsProjection.dbo.FgPlanCostPrice
ORDER BY ProductId asc

END

SELECT * FROM CogsProjection.dbo.FgPlanCostPrice
WHERE ProductId = '4200031'
--and verid = 68
--WHERE VerId = 81
----ORDER BY ProductId asc

--DELETE FROM CogsProjection.dbo.FgPlanCostPrice

GO

