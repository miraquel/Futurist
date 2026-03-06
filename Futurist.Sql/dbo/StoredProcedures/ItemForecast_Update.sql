-- =============================================
-- Author:		Andi
-- Create date: 22 jan 2025
-- Description:	Update harga Forecast
-- =============================================
CREATE PROCEDURE ItemForecast_Update
	@RecId int=29138
	,@ForcedPrice numeric(32,16) = 1500000
AS
BEGIN
	--SET NOCOUNT ON;
	UPDATE [ItemForecast]
	SET [ForcedPrice] = @ForcedPrice
	WHERE [RecId] = @RecId
END

GO

