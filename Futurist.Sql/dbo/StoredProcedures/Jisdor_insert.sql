-- =============================================
-- Author:		<andi>
-- Create date: <15 Aug 2025>
-- Description:	<Upload data Jisdor>
-- =============================================
CREATE PROCEDURE [dbo].[Jisdor_insert] 
	@CurrencyCode nvarchar(20),
	@JisdorDate datetime,
	@Kurs numeric(32,16)
AS
BEGIN
	INSERT INTO [Jisdor] ([CurrencyCode],[JisdorDate],[Kurs]) VALUES (	@CurrencyCode,@JisdorDate, @Kurs);
END

GO

