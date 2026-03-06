-- =============================================
-- Author:		Andi
-- Create date: 26 Mar 2025
-- Description:	<Hapus ItemAdj utk keperluan upload>
-- EXEC [ItemAdj_Upload]
-- =============================================
CREATE PROCEDURE [dbo].[ItemAdj_Upload]
	@Room int
	,@ItemId nvarchar(20)
	,@AdjPrice numeric(32,16)
	,@CreatedBy nvarchar(20) = 'Unknow'
	,@CreatedDate datetime = '1900-01-01'
AS
BEGIN
	SET NOCOUNT ON;
	    
	INSERT INTO [ItemAdj] ([Room],[ItemId],[AdjPrice],[RmPrice],[PmPrice],[CreatedBy],[CreatedDate])
	VALUES (@Room,@ItemId,@AdjPrice
	,IIF(LEFT(@ItemId,1) = '1',@AdjPrice,0)
	,IIF(LEFT(@ItemId,1) = '3',@AdjPrice,0)
	,@CreatedBy,GETDATE())
	
END

GO

