-- =============================================
-- Author:		<andi>
-- Create date: <15 Aug 2025>
-- Description:	<Delte data Jisdor 1 bulan>
-- =============================================
CREATE PROCEDURE Jisdor_Delete 
	@Year int,
	@Month int
AS
BEGIN
	DELETE [Jisdor]
	WHERE YEAR([JisdorDate]) = @Year
		AND MONTH([JisdorDate]) = @Month
END

GO

