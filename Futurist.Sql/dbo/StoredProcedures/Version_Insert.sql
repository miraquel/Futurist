-- =============================================
-- Author:		Andi
-- Create date: 22 Jan 2025
-- Description:	Rofo Select
-- EXEC [Version_Insert] 3
-- =============================================
CREATE PROCEDURE [dbo].[Version_Insert]
@Room int = 5,
@Notes nvarchar(50)=''

AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @MessageStatusId int
	DECLARE @MessageStatusName NVARCHAR(MAX)

	IF NOT EXISTS(SELECT TOP 1 Room FROM FgCost WHERE Room = @Room)
	BEGIN
		SET @MessageStatusId = 0
		SET @MessageStatusName = 'FG Cost tidak ada data' 
		SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
		RETURN
	END

	DECLARE @VerDate date, @VerId int

	SET @VerDate = GETDATE()

	--01 [Version]
	INSERT INTO [Version]
	([VerDate]
      ,[Room]
      ,[Notes]
    )
	SELECT @VerDate, @Room, @Notes
		
	SET @VerId = SCOPE_IDENTITY() 

	--02 [FgCostVer]
	INSERT INTO [FgCostVer]
		([Room]
		,[RofoId]
		,[ProductId]
		,[ProductName]
		,[RofoDate]
		,[QtyRofo]
		,[Yield]
		,[RmPrice]
		,[PmPrice]
		,[StdCostPrice]
		,[CostPrice]
		,[RecId]
		,[VerId]
		)
		SELECT [Room]
			,[RofoId]
			,[ProductId]
			,[ProductName]
			,[RofoDate]
			,[QtyRofo]
			,[Yield]
			,[RmPrice]
			,[PmPrice]
			,[StdCostPrice]
			,[CostPrice]
			,[RecId]
			,@VerId
		FROM [FgCost] WITH(NOLOCK)
		WHERE [Room] = @Room

	--03 [RofoVer]
	INSERT INTO [RofoVer]
		(
		 [Room]
		,[RofoDate]
		,[ItemId]
		,[ItemName]
		,[Qty]
		,[QtyRem]
		,[SalesPrice]
		,[CreatedBy]
		,[CreatedDate]
		,[RecId]
		,[VerId]
		)
		SELECT  [Room]
		,[RofoDate]
		,[ItemId]
		,[ItemName]
		,[Qty]
		,[QtyRem]
		,[SalesPrice]
		,[CreatedBy]
		,[CreatedDate]
		,[RecId]
		,@VerId
		FROM [RofoRoom] WITH(NOLOCK)
		WHERE [Room] = @Room

	--04 [ItemStdCostVer]
	INSERT INTO [ItemStdCostVer]
		( [RecId]
		,[ItemId]
		,[Price]
		,[Room]
		,[VerId]
		)
		SELECT  [RecId]
			,[ItemId]
			,[Price]
			,@Room
			,@VerId
		FROM [ItemStdCostRoom] WITH(NOLOCK)
		WHERE RecId in (select distinct RefId from ItemTrans where Room = @Room and [Source] = 'StdCost')

	--05 [ItemOnhandVer]
	INSERT INTO [ItemOnhandVer]
		([RecId]
		,[ItemId]
		,[InventBatch]
		,[ExpDate]
		,[PdsDispositionCode]
		,[Qty]
		,[QtyRem]
		,[Price]
		,[RmPrice]
		,[PmPrice]
		,[StdcostPrice]
		,[Room]
		,[VerId]
		)
		SELECT [RecId]
			,[ItemId]
			,[InventBatch]
			,[ExpDate]
			,[PdsDispositionCode]
			,[Qty]
			,[QtyRem]
			,[Price]
			,[RmPrice]
			,[PmPrice]
			,[StdcostPrice]
			,[Room]
			,@VerId
		FROM [ItemOnhandRoom] WITH(NOLOCK)
		WHERE RecId in (select distinct RefId from ItemTrans where Room = @Room and [Source] = 'OnHand')

	--06 [ItemPoIntransitVer]
	INSERT INTO [ItemPoIntransitVer]
		(
		[RecId]
		,[ItemId]
		,[Po]
		,[DeliveryDate]
		,[Qty]
		,[QtyRem]
		,[Unit]
		,[Price]
		,[Room]
		,[VerId]
		)
		SELECT [RecId]
		,[ItemId]
		,[Po]
		,[DeliveryDate]
		,[Qty]
		,[QtyRem]
		,[Unit]
		,[Price]
		,[Room]
		,@VerId
		FROM [ItemPoIntransitRoom] WITH(NOLOCK)
		WHERE RecId in (select distinct RefId from ItemTrans where Room = @Room and [Source] = 'PoIntransit')
	

	--07 [ItemPagVer]
	INSERT INTO [ItemPagVer]
		(
		[RecId]
		,[ItemId]
		,[Pag]
		,[VendorId]
		,[EffectiveDate]
		,[ExpirationDate]
		,[Qty]
		,[QtyRem]
		,[Unit]
		,[CurrencyCode]
		,[Price]
		,[Room]
		,[VERID]
		)
		SELECT [RecId]
		,[ItemId]
		,[Pag]
		,[VendorId]
		,[EffectiveDate]
		,[ExpirationDate]
		,[Qty]
		,[QtyRem]
		,[Unit]
		,[CurrencyCode]
		,[Price]
		,[Room]
		,@VerId
		FROM [ItemPagRoom] WITH(NOLOCK)
		WHERE RecId in (select distinct RefId from ItemTrans WITH(NOLOCK) where Room = @Room and [Source] = 'Contract')
		
	--08 [ItemForecastVer]
	INSERT INTO [ItemForecastVer]
		(
		[RecId]
		,[ItemId]
		,[Unit]
		,[ForecastDate]
		,[ForecastPrice]
		,[ForcedPrice]
		,[Room]
		,[VerId]
		)
		SELECT [RecId]
		,[ItemId]
		,[Unit]
		,[ForecastDate]
		,[ForecastPrice]
		,[ForcedPrice]
		,@Room
		,@VerId
		FROM [ItemForecastRoom] WITH(NOLOCK)
		WHERE RecId in (select distinct RefId from ItemTrans WITH(NOLOCK) where Room = @Room and [Source] = 'Forecast')
		
	--09 [MupVer]
	INSERT INTO [MupVer]
		(
		[Room]
		,[MupDate]
		,[ItemId]
		,[ItemGroup]
		,[Qty]
		,[QtyRem]
		,[RofoId]
		,[RecId]
		,[VerId]
		)
		SELECT [Room]
		,[MupDate]
		,[ItemId]
		,[ItemGroup]
		,[Qty]
		,[QtyRem]
		,[RofoId]
		,[RecId]
		,@VerId
		FROM [Mup] WITH(NOLOCK)
		WHERE Room = @Room

	--10 [ItemTransVer]
	INSERT INTO [ItemTransVer]
		(
		[RecId]
      ,[Room]
      ,[ItemId]
	  ,[InventBatch]
      ,[Qty]
      ,[Price]
      ,[RmPrice]
      ,[PmPrice]
      ,[StdCostPrice]
      ,[Source]
      ,[RefId]
      ,[CurrencyCode]
      ,[CurrencyRate]
		,[VerId]
		)
		SELECT [RecId]
      ,[Room]
      ,[ItemId]
      ,[InventBatch]
      ,[Qty]
      ,[Price]
      ,[RmPrice]
      ,[PmPrice]
      ,[StdCostPrice]
      ,[Source]
      ,[RefId]
      ,[CurrencyCode]
      ,[CurrencyRate]
		,@VerId
		FROM [ItemTrans] WITH(NOLOCK)
		WHERE Room = @Room


	--11 [MupTransVer]
	INSERT INTO [MupTransVer]
		(
		[RecId]
		,[Room]
		,[MupId]
		,[ItemTransId]
		,[VerId]
		)
		SELECT [RecId]
		,[Room]
		,[MupId]
		,[ItemTransId]
		,@VerId
		FROM [MupTrans] WITH(NOLOCK)
		WHERE Room = @Room
	
	--12 [SalesPriceVer]
	INSERT INTO [SalesPriceVer]
		(
			[Room]
			,[RecId]
			,[ItemId]
			,[PeriodDate]
			,[SalesPriceIndex]
			,[VerId]
		)
		SELECT @Room, x.[RecId], x.[ItemId], x.[MaxPeriodDate], x.[SalesPriceIndex], @VerId
		FROM ( 
			SELECT a.RecId, a.ItemId, a.[SalesPriceIndex], b.[MaxPeriodDate]
			FROM [SalesPrice] a WITH(NOLOCK)
			JOIN (
				SELECT [ItemId], MAX([PeriodDate]) [MaxPeriodDate]
				FROM [SalesPrice] WITH(NOLOCK)
				GROUP BY [ItemId]
			) b ON b.ItemId = a.ItemId AND b.[MaxPeriodDate] = a.[PeriodDate]
		) x			
		where x.ItemId IN (SELECT DISTINCT [ProductId] FROM [FgCost] WHERE [Room] = @Room) 
		
	--13 [ExchangeRateVer]
	INSERT INTO [ExchangeRateVer] ([RecId],[CurrencyCode],[ValidFrom],[ValidTo],[ExchangeRate],[VerId])
	SELECT [RecId],[CurrencyCode],[ValidFrom],[ValidTo],[ExchangeRate],@VerId
		FROM [ExchangeRate] WITH(NOLOCK)

	--14 [ItemAdjVer]
	INSERT INTO [ItemAdjVer] ([RecId],[Room],[ItemId],[AdjPrice],[RmPrice],[PmPrice],[CreatedBy],[CreatedDate],[VerId])
	SELECT [RecId],[Room],[ItemId],[AdjPrice],[RmPrice],[PmPrice],[CreatedBy],[CreatedDate],@VerId
		FROM [ItemAdjRoom] WITH(NOLOCK)
		WHERE Room = @Room

	--15 [BomStdRoomVer]
	--INSERT INTO [BomStdRoomVer] ([Room],[BomId],[ProductId],[ItemId],[ItemName],[BomQty],[BomQtySerie],[LineType],[SubBomId],[Ref],[Level],[CreatedBy],[CreatedDate],[RecId],[VerId])
	--SELECT [Room],[BomId],[ProductId],[ItemId],[ItemName],[BomQty],[BomQtySerie],[LineType],[SubBomId],[Ref],[Level],[CreatedBy],[CreatedDate],[RecId],@VerId
	--	FROM [BomStdRoom] WITH(NOLOCK)
	--	WHERE Room = @Room

	INSERT INTO [BomStdRoomVer] ([Room],[BomId],[ProductId],[ItemId],[ItemName],[BomQty],[BomQtySerie],[LineType],[SubBomId],[Ref],[Level],[FromDate], [ToDate],[CreatedBy],[CreatedDate],[RecId],[VerId])
	SELECT [Room],[BomId],[ProductId],[ItemId],[ItemName],[BomQty],[BomQtySerie],[LineType],[SubBomId],[Ref],[Level],[FromDate], [ToDate],[CreatedBy],[CreatedDate],[RecId],@VerId
		FROM [BomStdRoom] WITH(NOLOCK)
		WHERE Room = @Room

	--16 [DimItemSubstituteVer]
	INSERT INTO [DimItemSubstituteVer] ([ItemId], [VtaMpSubstitusiGroupId], [Description],[VerId])
	SELECT [ItemId], [VtaMpSubstitusiGroupId], [Description],@VerId
	FROM [AXGMKDW].[dbo].[DimItemSubstitute]

	--SELECT @VerId as VersionId
	SET @MessageStatusId = 1
	SET @MessageStatusName = 'Proses berhsail dengan nomor Versi ' + CAST(@VerId as nvarchar(max))
	SELECT @MessageStatusId as [StatusId], @MessageStatusName as [StatusName]
END

GO

