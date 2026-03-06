-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Upload Exchange Rate
-- exec ExchangeRate_Upload 'USD','45658','45688','15701'
-- =============================================
CREATE PROCEDURE [dbo].[ExchangeRate_Upload]
	@CurrencyCode nvarchar(20)
	,@ValidFrom datetime
	,@ValidTo datetime
	,@ExchangeRate numeric(32,16)
	,@CreatedBy nvarchar(20) = 'Unknow'
	,@CreatedDate datetime = '1900-01-01'
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [ExchangeRate] WHERE CurrencyCode = @CurrencyCode AND [ValidFrom] = @ValidFrom AND [ValidTo] = @ValidTo 

	INSERT INTO [ExchangeRate] ([CurrencyCode],[ValidFrom],[ValidTo],[ExchangeRate],[CreatedBy],[CreatedDate])
		VALUES (@CurrencyCode,@ValidFrom,@ValidTo,@ExchangeRate,@CreatedBy,GETDATE())

END

GO

