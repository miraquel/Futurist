-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Upload Rofo
-- =============================================
CREATE PROCEDURE [dbo].[Rofo_Upload_bck]
	@Room int
	,@RofoDate int
	,@ItemId nvarchar(20)
	,@ItemName nvarchar(60)
	,@Qty numeric(32,16)
	,@CreatedBy nvarchar(20) = 'Unknow'
	,@CreatedDate datetime = '1900-01-01'
AS
BEGIN
	SET NOCOUNT ON;

    IF @Qty >= 1
	BEGIN
		INSERT INTO [Rofo] ([Room],[RofoDate],[ItemId],[ItemName],[Qty],[QtyRem],[CreatedBy],[CreatedDate])
		VALUES (@Room,@RofoDate,@ItemId,@ItemName,@Qty,@Qty,@CreatedBy,GETDATE())
	END
END

GO

