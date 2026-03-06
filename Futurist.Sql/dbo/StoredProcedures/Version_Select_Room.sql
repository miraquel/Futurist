-- =============================================
-- Author:		Andi
-- Create date: 03 Jun 2025
-- Description:	Version Select
-- EXEC [Version_Select_Room]
-- =============================================
CREATE PROCEDURE [dbo].[Version_Select_Room]
AS
BEGIN
	SELECT DISTINCT [Room]
	FROM [Version]
	WHERE  [Cancel] = 0
	ORDER BY [Room] ASC
END

GO

