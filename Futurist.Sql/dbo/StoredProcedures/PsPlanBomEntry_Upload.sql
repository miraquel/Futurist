
-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	Upload PsPlanBomEntry
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[PsPlanBomEntry_Upload]
(
	@Room int
	, @PlanDate datetime
	, @ItemId nvarchar(20)
	, @ItemName nvarchar(60)
	, @Qty numeric(32,16)
	, @BomIdEntry nvarchar(20)
)
AS
BEGIN
	INSERT INTO [PsPlanBomEntry] (Room,PlanDate,ItemId,ItemName,Qty,BomIdEntry)
	VALUES (@Room,@PlanDate,@ItemId,@ItemName,@Qty,@BomIdEntry)
END

GO

