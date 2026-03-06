-- =============================================
-- Author:		Andi
-- Create date: 22 jan 2025
-- Description:	Upload harga Forecast
-- =============================================
CREATE PROCEDURE [dbo].[ItemForecastRoom_Upload_bck]
	@Room int=5
	,@ItemId nvarchar(20) = '1000007'
	,@ForecastDate datetime = '1-Apr-2025'
	,@ForcedPrice numeric(32,16) =  12130
AS
BEGIN
	--SET NOCOUNT ON;
	DECLARE @RecId int

	--Jika Item sudah ada sebelumnya di ItemForecastRoom
	IF EXISTS(SELECT RecId FROM [ItemForecastRoom] WHERE Room = @Room AND [ItemId] = @ItemId AND ForecastDate = @ForecastDate)
	BEGIN
		UPDATE [ItemForecastRoom]
		SET [ForcedPrice] = 0
		WHERE Room = @Room AND [ItemId] = @ItemId AND ForecastDate = @ForecastDate

		UPDATE [ItemForecastRoom] 
		SET [ForcedPrice] =	@ForcedPrice	
		WHERE Room = @Room AND [ItemId] = @ItemId AND ForecastDate = @ForecastDate
	END
	ELSE
	
	--Jika Item tidak ada maka insert di ItemForecast terlebih dahulu, baru di copy ke ItemForecastRoom
	BEGIN
		IF EXISTS(SELECT RecId FROM [ItemForecast] WHERE [ItemId] = @ItemId AND ForecastDate = @ForecastDate)
		BEGIN	
			DELETE FROM [ItemForecast] WHERE [ItemId] = @ItemId AND ForecastDate = @ForecastDate
						
			INSERT INTO [ItemForecast] ([ItemId],[Unit],[ForecastDate],[ForecastPrice],[ForcedPrice],[LatestPurchaseDate])
			VALUES (@ItemId, '', @ForecastDate, 0, @ForcedPrice, 0)
						
			SET @RecId = SCOPE_IDENTITY()
			INSERT INTO [ItemForecastRoom] ([RecId], [Room], [ItemId],[Unit],[ForecastDate],[ForecastPrice],[ForcedPrice],[LatestPurchaseDate])
			VALUES (@RecId, @Room, @ItemId, '', @ForecastDate, 0, @ForcedPrice, 0)

			UPDATE ItemTrans
			SET [Source] = 'Forecast by user'
				,RefId = @RecId
			FROM ItemTrans a
			JOIN MupTrans b ON b.ItemTransId = a.RecId
			JOIN Mup c ON c.RecId = b.MupId
			WHERE a.Room = @Room AND a.[Source] = 'NA'
				AND a.[ItemId] = @ItemId
				AND c.MupDate = @ForecastDate
		END
		ELSE
		BEGIN
			INSERT INTO [ItemForecast] ([ItemId],[Unit],[ForecastDate],[ForecastPrice],[ForcedPrice],[LatestPurchaseDate])
			VALUES (@ItemId, '', @ForecastDate, 0, @ForcedPrice, 0)
			
			SET @RecId = SCOPE_IDENTITY()
			INSERT INTO [ItemForecastRoom] ([RecId], [Room], [ItemId],[Unit],[ForecastDate],[ForecastPrice],[ForcedPrice],[LatestPurchaseDate])
			VALUES (@RecId, @Room, @ItemId, '', @ForecastDate, 0, @ForcedPrice, 0)

			UPDATE ItemTrans
			SET [Source] = 'Forecast by user'
				,RefId = @RecId
			FROM ItemTrans a
			JOIN MupTrans b ON b.ItemTransId = a.RecId
			JOIN Mup c ON c.RecId = b.MupId
			WHERE a.Room = @Room AND a.[Source] = 'NA'
				AND a.[ItemId] = @ItemId
				AND c.MupDate = @ForecastDate
		END		
	END


END

GO

