-- =============================================
-- Author:		Andi
-- Create date: 22 jan 2025
-- Description:	Select harga Forecast utk di upload
-- EXEC [ItemForecastRoom_UploadSelect] 99
-- Durasi: 2 detik
-- =============================================
CREATE PROCEDURE [dbo].[ItemForecastRoom_UploadSelect_bck]
	@Room int=5
AS
BEGIN
	--Menampilkan data forecast yg pembeliannya tidak aktif (kurang dari 2 bulan dari today)
	SELECT a.Room
		,d.ItemId as ItemId
		,REPLACE(REPLACE(REPLACE(id.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemName
		,id.UnitId
		,ISNULL(s.[VtaMpSubstitusiGroupId],'') as [VtaMpSubstitusiGroupId]
		,ISNULL(g.[GroupName],'') as [GroupProcurement]
		,d.Price
		,ISNULL(f.LatestPurchaseDate,'1900-01-01') as [LatestPurchaseDate]
		,a.RofoDate as [ForecastDate]
		,ISNULL(f.[ForecastPrice],0) as [ForecastPrice]
		,ISNULL(f.[ForcedPrice],0) as [ForcedPrice]	
	FROM Rofo a  WITH (NOLOCK)
	JOIN Mup b WITH (NOLOCK) ON b.RofoId = a.RecId
	JOIN MupTrans c WITH (NOLOCK) ON c.MupId = b.RecId
	JOIN ItemTrans d WITH (NOLOCK) ON d.RecId = c.ItemTransId
	JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
	JOIN AXGMKDW.dbo.DimItem ib WITH (NOLOCK) ON ib.ITEMID = b.ItemId
	JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s WITH (NOLOCK) ON s.ItemId = d.ItemId
	LEFT JOIN [ItemForecastRoom] f WITH (NOLOCK) ON f.RecId = d.RefId AND f.Room = a.Room
	LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = d.ItemId
	WHERE a.Room = @Room	--5
		--AND (d.[Source] = 'Forecast' or d.[Source] = 'Forecasting LP')
		AND (d.[Source] LIKE 'Forecast%'  OR d.[Source] = 'NA' OR d.[Source] = 'AdjPrice by user')
		--AND f.LatestPurchaseDate < DATEADD(MONTH,-2,getdate())
		--AND (f.LatestPurchaseDate < DATEADD(MONTH,-4,getdate()) or d.Price = 0)
	UNION

	--Menampilkan data yang sudah di upload sebelumnya
	SELECT a.Room
		,d.ItemId as ItemId
		,REPLACE(REPLACE(REPLACE(id.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemName
		,id.UnitId
		,ISNULL(s.[VtaMpSubstitusiGroupId],'') as [VtaMpSubstitusiGroupId]
		,ISNULL(g.[GroupName],'') as [GroupProcurement]
		,d.Price
		,f.LatestPurchaseDate
		,a.RofoDate as [ForecastDate]
		,f.[ForecastPrice] as [ForecastPrice]
		,f.[ForcedPrice]		
	FROM Rofo a  WITH (NOLOCK)
	JOIN Mup b WITH (NOLOCK) ON b.RofoId = a.RecId
	JOIN MupTrans c WITH (NOLOCK) ON c.MupId = b.RecId
	JOIN ItemTrans d WITH (NOLOCK) ON d.RecId = c.ItemTransId
	JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
	JOIN AXGMKDW.dbo.DimItem ib WITH (NOLOCK) ON ib.ITEMID = b.ItemId
	JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	LEFT JOIN AXGMKDW.dbo.[DimItemSubstitute] s WITH (NOLOCK) ON s.ItemId = d.ItemId
	LEFT JOIN [ItemForecastRoom] f WITH (NOLOCK) ON f.RecId = d.RefId AND f.Room = a.Room
	LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = d.ItemId
	WHERE a.Room = @Room	--5
		--AND (d.[Source] LIKE 'Forecast' or d.[Source] = 'Forecasting LP' or d.[Source] = 'Forecast by user')
		AND (d.[Source] LIKE 'Forecast%' OR d.[Source] = 'NA' OR d.[Source] = 'AdjPrice by user')
		AND f.[ForcedPrice] > 0

	ORDER BY ItemId, ForecastDate

END

GO

