-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	Delete PsPlanBomEntry sebelum upload
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[PsPlanBomEntry_Delete]
(
	@Room int
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DELETE FROM [PsPlanBomEntry] 
	WHERE [Room]=@Room
END

GO

