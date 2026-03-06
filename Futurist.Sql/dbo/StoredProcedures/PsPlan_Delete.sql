-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	Delte PsPlan sebelum upload
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[PsPlan_Delete]
(
	@Room int
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DELETE FROM [PsPlan] 
	WHERE [Room]=@Room
END

GO

