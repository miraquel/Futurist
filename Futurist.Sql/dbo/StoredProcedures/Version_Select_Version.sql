-- =============================================
-- Author:		Andi
-- Create date: 03 Jun 2025
-- Description:	Version Select
-- EXEC [Version_Select_Version] 3
-- =============================================
CREATE PROCEDURE [dbo].[Version_Select_Version]
@Room int = 2
AS
BEGIN
	SELECT [VerId]
		,[VerDate]
		,[Room]
		,[Notes]
	FROM [Version]
	WHERE [Room] = @Room
		AND [Cancel] = 0
	ORDER BY [VerId] DESC
END

GO

