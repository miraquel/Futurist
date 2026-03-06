-- =============================================
-- Author:		<Andi>
-- Create date: <22 Jan 2025>
-- Description:	<Hapus rofo utk keperluan upload Rofo>
-- =============================================
CREATE PROCEDURE Rofo_Delete 
	@Room int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE [Rofo]
	WHERE [Room] = @Room
END

GO

