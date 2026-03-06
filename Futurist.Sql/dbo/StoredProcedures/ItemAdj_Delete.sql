-- =============================================
-- Author:		Andi
-- Create date: 26 Mar 2025
-- Description:	<Hapus ItemAdj utk keperluan upload>
-- EXEC [ItemAdj_Delete]
-- =============================================
CREATE PROCEDURE [dbo].[ItemAdj_Delete] 
	@Room int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE [ItemAdj]
	WHERE [Room] = @Room
END

GO

