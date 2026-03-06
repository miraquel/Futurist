-- =============================================
-- Author:		Andi
-- Create date: 23 Sep 2025
-- Description:	Upload PsPlan
-- Durasi:
-- =============================================
CREATE PROCEDURE [dbo].[PsPlan_Upload]
(
	@Room int
	, @PlanDate datetime
	, @ItemId nvarchar(20)
	, @ItemName nvarchar(60)
	, @Qty numeric(32,16)
)
AS
BEGIN
	INSERT INTO [PsPlan] (Room,PlanDate,ItemId,ItemName,Qty)
	VALUES (@Room,@PlanDate,@ItemId,@ItemName,@Qty)
END

GO

