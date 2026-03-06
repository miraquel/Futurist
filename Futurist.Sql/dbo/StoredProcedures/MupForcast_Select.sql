-- =============================================
-- Author:		Andi
-- Create date: 22 jan 2025
-- Description:	untuk melihat harga forecast yang digunakan
-- =============================================
CREATE PROCEDURE [dbo].[MupForcast_Select]
	@Room int=1
AS
BEGIN
	--SET NOCOUNT ON;
		
	SELECT a.RefId
	, a.ItemId, i.SEARCHNAME as ItemName, a.Qty, a.[Source], m.MupDate, f.ForecastPrice as [ForecastPrice], f.ForcedPrice as [ForcedPrice]
	FROM [ItemTrans] a
	JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ItemId
	JOIN [MupTrans] mt ON mt.ItemTransId = a.RecId
	JOIN [Mup] m ON m.RecId = mt.MupId
	JOIN [ItemForecast] f ON f.ItemId = a.ItemId AND f.ForecastDate = m.MupDate
	WHERE a.Room = @Room
		AND a.[Source] = 'Forecast'
END

GO

