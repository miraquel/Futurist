-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Upload Rofo
-- =============================================
CREATE PROCEDURE [dbo].[Rofo_Upload]
	@Room int
	,@RofoDate int
	,@ItemId nvarchar(20)
	,@ItemName nvarchar(60)
	,@Qty numeric(32,16)
	,@SalesPrice numeric(32,16)
	,@CreatedBy nvarchar(20) = 'Unknow'
	,@CreatedDate datetime = '1900-01-01'
AS
BEGIN
	SET NOCOUNT ON;

    IF @Qty > 0
	BEGIN
		INSERT INTO [Rofo] ([Room],[RofoDate],[ItemId],[ItemName],[Qty],[QtyRem],[SalesPrice],[CreatedBy],[CreatedDate])
		VALUES (@Room,@RofoDate,@ItemId,@ItemName,CEILING(@Qty),CEILING(@Qty),@SalesPrice,@CreatedBy,GETDATE())
	END
END

GO

