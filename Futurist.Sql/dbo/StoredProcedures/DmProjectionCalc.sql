CREATE PROCEDURE [dbo].[DmProjectionCalc]
@Room int = 1
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @MessageStatus NVARCHAR(MAX)
	
	DECLARE @Result as TABLE( 
		Room INT
		,[RofoDate] DATETIME
		,ProductId NVARCHAR(20)
		,RofoQty NUMERIC(32,16)
		,RofoId INT
		,Yield NUMERIC(32,16)
		,RmPrice NUMERIC(32,16)
		,PmPrice NUMERIC(32,16)
		,StdCostPrice NUMERIC(32,16)
		)

	--Generate dari ROFO
	INSERT INTO @Result (Room, RofoDate, ProductId, RofoQty, RofoId, Yield)
		SELECT a.Room, a.RofoDate, a.ItemId, a.Qty, a.RecId, y.Yield
		FROM DmRofo a
		LEFT JOIN [ItemYield] y ON y.ItemId = a.ItemId
		WHERE a.Room = @Room
		
	--RM
	UPDATE @Result
	SET RmPrice = b.RmPrice
	FROM @Result a
	JOIN (
		select a.RecId as RofoId
			, sum(d.Qty*d.RmPrice) / sum(d.Qty)	as RmPrice
		from DmRofo a 
		join DmMup b ON b.RofoId = a.RecId
		join DmMupTrans c ON c.MupId = b.RecId
		join DmItemTrans d ON d.RecId = c.ItemTransId
		where a.Room = @Room AND d.RmPrice > 0
		group by a.RecId
	) b ON b.RofoId = a.RofoId

	--PM
	UPDATE @Result
	SET PmPrice = isnull(b.PmPrice,0)
	FROM @Result a
	LEFT JOIN (
		select a.RecId as RofoId
			, sum(d.Qty*d.PmPrice) / sum(d.Qty)	as PmPrice
		from DmRofo a 
		join DmMup b ON b.RofoId = a.RecId
		join DmMupTrans c ON c.MupId = b.RecId
		join DmItemTrans d ON d.RecId = c.ItemTransId
		where a.Room = @Room AND d.PmPrice > 0
		group by a.RecId
	) b ON b.RofoId = a.RofoId

	--StdCost
	UPDATE @Result
	SET StdCostPrice = isnull(b.StdCostPrice,0)
	FROM @Result a
	LEFT JOIN (
		select a.RecId as RofoId
			, sum(d.Qty*d.StdCostPrice) / sum(d.Qty) as StdCostPrice
		from DmRofo a 
		join DmMup b ON b.RofoId = a.RecId
		join DmMupTrans c ON c.MupId = b.RecId
		join DmItemTrans d ON d.RecId = c.ItemTransId
		where a.Room = @Room AND d.StdCostPrice > 0
		group by a.RecId
	) b ON b.RofoId = a.RofoId

select * from @Result

	--select * from DmRofo
	--select * from DmMup
	--select * from DmMupTrans
	--select * from DmItemTrans
		
	--select a.RecId, sum(d.Qty*d.RmPrice) / sum(d.Qty)	--d.*	--a.*, b.*, 
	--from DmRofo a 
	--join DmMup b ON b.RofoId = a.RecId
	--join DmMupTrans c ON c.MupId = b.RecId
	--join DmItemTrans d ON d.RecId = c.ItemTransId
	--where a.RecId = 1
	--group by a.RecId

	
 --   DELETE FROM [ProjectionPrice] WHERE [Year] = @Year AND [Month] = @Month

	--INSERT INTO [ProjectionPrice] ([Year],[Month],[ItemId],[Rm],[Pm],[StdCost],[Yield],[CostRmPm],[CostPrice])
	--SELECT [Year],[Month],PRODUCTID,[Rm],[Pm],[StdCost], [Yield]
	--	,([Rm]/[Yield])+[Pm]
	--	,([Rm]/[Yield])+[Pm]+[StdCost]  FROM @Result		
	
	
	
	--EXEC [Version_insert] @Year, @Month, ''


	--SET @MessageStatus = 'Proses Calculation success.' 
	--SELECT @MessageStatus as [Message Status]
	




	
END

GO

