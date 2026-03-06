-- =============================================
-- Author:		<Andi>
-- Create date: <22 Jan 2025>
-- Description:	<Untuk Proses Calculation>
-- EXEC [ProjectionCalc] 99
-- durasi: 9 menit, setelah index menjadi 13 detik
-- =============================================
CREATE PROCEDURE [dbo].[ProjectionCalc_bck]
@Room int = 1
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @MessageStatusId int
	DECLARE @MessageStatusName NVARCHAR(MAX)

	IF EXISTS(
		SELECT a.RecId 
		FROM ItemTrans a
		LEFT JOIN ItemForecastRoom b ON b.ItemId = a.ItemId AND b.Room = a.Room and b.RecId = a.RefId
		WHERE a.Room = @Room AND a.Price = 0 AND (b.ForcedPrice = 0 OR b.ForcedPrice is null)
			--AND (a.[Source] = 'Forecast' OR a.[Source] = 'Forecast by user')
			AND a.[Source] LIKE 'Forecast%'
 	)
	BEGIN
		SET @MessageStatusId = 0
		SET @MessageStatusName = 'Proses tidak bisa dilanjutkan karena terdapat harga forecast yang masih nol. Silahkan update harga pada "Forecast by user"' 
		SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
		RETURN
	END
		
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

	--Update harga [ForcedPrice] dari table [ItemForecastRoom] ke harga pada table [ItemTrans] 
	UPDATE [ItemTrans]
		SET [Price] = d.[ForcedPrice]
			,[Source] = 'Forecast by user'
		FROM [ItemTrans] a
		JOIN [MupTrans] b ON b.ItemTransId = a.RecId
		JOIN [Mup] c ON c.RecId = b.MupId
		JOIN [ItemForecastRoom] d ON d.Room = a.Room AND [ForcedPrice] > 0
			AND d.ItemId = a.ItemId AND d.[ForecastDate] = c.[MupDate]
		WHERE a.Room = @Room 
			AND (a.[Source] LIKE 'Forecast%' OR a.[Source] LIKE 'NA')

	--Updet komposisi RmPrice dan PmPrice harga
	UPDATE [ItemTrans]
	SET RmPrice = Price
	WHERE Room = @Room 
		AND ItemId LIKE '1%'
		AND [Source] = 'Forecast by user'
		--AND ([Source] = 'Forecast by user' OR [Source] = 'AdjPrice by user')

	UPDATE [ItemTrans]
	SET PmPrice = Price
	WHERE Room = @Room 
		AND ItemId LIKE '3%'
		AND [Source] = 'Forecast by user'
		--AND ([Source] = 'Forecast by user' OR [Source] = 'AdjPrice by user')

	--Generate Result mulai dari table ROFO
	INSERT INTO @Result (Room, RofoDate, ProductId, RofoQty, RofoId, Yield)
		SELECT a.Room, a.RofoDate, a.ItemId, a.Qty, a.RecId, isnull(y.Yield,0.95) as Yield
		FROM Rofo a
		LEFT JOIN [ItemYield] y ON y.ItemId = a.ItemId
		WHERE a.Room = @Room
		
	--RM
	UPDATE @Result
	SET RmPrice = b.RmPrice 
	FROM @Result a
	JOIN (
		select a.RecId as RofoId
			--,sum(d.Qty*d.RmPrice) / max(a.qty) as RmPrice
			,sum(d.Qty*d.RmPrice / iif(LEFT(d.ItemId,1)='4',1, isnull(y.Yield,0.95))  ) / max(a.qty) as RmPrice
		from Rofo a 
		JOIN Mup b ON b.RofoId = a.RecId
		JOIN MupTrans c ON c.MupId = b.RecId
		JOIN ItemTrans d ON d.RecId = c.ItemTransId
		LEFT JOIN [ItemYield] y ON y.ItemId = a.ItemId
		where a.Room = @Room 
			AND (d.RmPrice > 0 OR d.ItemId like '1%')
		group by a.RecId
	) b ON b.RofoId = a.RofoId

	--PM
	UPDATE @Result
	SET PmPrice = isnull(b.PmPrice,0)
	FROM @Result a
	LEFT JOIN (
		select a.RecId as RofoId
			,sum(d.Qty*d.PmPrice) / max(a.qty) as PmPrice
		from Rofo a 
		join Mup b ON b.RofoId = a.RecId
		join MupTrans c ON c.MupId = b.RecId
		join ItemTrans d ON d.RecId = c.ItemTransId
		where a.Room = @Room 
			AND (d.PmPrice > 0 OR d.ItemId like '3%')
		group by a.RecId
	) b ON b.RofoId = a.RofoId

	--StdCost
	UPDATE @Result
	SET StdCostPrice = isnull(b.StdCostPrice,0) 
	FROM @Result a
	LEFT JOIN (
		select a.RecId as RofoId
			,sum(d.Qty*d.StdCostPrice) / max(a.qty) as StdCostPrice
		from Rofo a 
		join Mup b ON b.RofoId = a.RecId
		join MupTrans c ON c.MupId = b.RecId
		join ItemTrans d ON d.RecId = c.ItemTransId
		where a.Room = @Room 
			AND (d.StdCostPrice > 0	OR d.ItemId like '9%')
		group by a.RecId
	) b ON b.RofoId = a.RofoId

	DELETE FROM FgCost WHERE Room = @Room
	INSERT INTO FgCost ([Room],[RofoId],[ProductId],[ProductName],[RofoDate],[QtyRofo],[Yield],[RmPrice],[PmPrice],[StdCostPrice],[CostPrice])
		SELECT a.[Room],a.[RofoId],a.[ProductId],i.SEARCHNAME as [ProductName],a.[RofoDate],a.[RofoQty],a.[Yield],isnull(a.[RmPrice],0),isnull(a.[PmPrice],0),isnull(a.[StdCostPrice],0)
		--,((isnull(RmPrice,0)/Yield)+isnull(PmPrice,0)+isnull(StdCostPrice,0)) as [CostPrice]
		,(isnull(RmPrice,0)+isnull(PmPrice,0)+isnull(StdCostPrice,0)) as [CostPrice]
		FROM @Result a
		JOIN AXGMKDW.dbo.DimItem i ON i.ITEMID = a.ProductId

	SET @MessageStatusId = 1
	SET @MessageStatusName = 'Proses Calculation success.' 
	SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
	
END

GO

