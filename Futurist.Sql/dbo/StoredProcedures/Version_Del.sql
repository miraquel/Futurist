-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Version delete (utk keperluan development)
-- =============================================
CREATE PROCEDURE [dbo].[Version_Del]
@VerId int
AS
BEGIN
	--01
	delete from [Version] WHERE VerId = @VerId
	--02
	delete from [FgCostVer] WHERE VerId = @VerId
	--03
	delete from [RofoVer] WHERE VerId = @VerId
	--04
	delete from [ItemStdCostVer] WHERE VerId = @VerId
	--05
	delete from [ItemOnhandVer] WHERE VerId = @VerId
	--06
	delete from [ItemPoIntransitVer] WHERE VerId = @VerId
	--07
	delete from [ItemPagVer] WHERE VerId = @VerId
	--08
	delete from [ItemForecastVer] WHERE VerId = @VerId
	--09
	delete from [MupVer] WHERE VerId = @VerId
	--10
	delete from [ItemTransVer] WHERE VerId = @VerId
	--11
	delete from [MupTransVer] WHERE VerId = @VerId
	--12
	delete from [SalesPriceVer]  WHERE VerId = @VerId
	--13
	delete from [ExchangeRateVer]  WHERE VerId = @VerId
	--14
	delete from [ItemAdjVer]  WHERE VerId = @VerId
	--15
	delete from [BomStdRoomVer]  WHERE VerId = @VerId

	select * from [Version]
	ORDER BY VerId DESC

END

GO

