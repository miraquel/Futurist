-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Untuk data analisa material plan vs actual>
-- EXEC MaterialAct_Calc 2025,7
-- =============================================
CREATE PROCEDURE [dbo].[MaterialAct_Calc]
@Year int = 2025,
@Month int = 12		
AS
BEGIN
	-- hapus dulu FgActNotBreakDown
	DELETE FgActNotBreakDown WHERE [Year] = @Year AND [Month] = @Month

	--untuk tabel hasil Plan
	DECLARE @Result table (
		[Year] int
		,[Month] int
		,ItemId nvarchar(20)
		,[Source] nvarchar(50)
		,ActQty numeric(32,16)
		,ActPrice numeric(32,16)
		,ProductId nvarchar(20)
		,InventbatchId nvarchar(20)
		,SalesQty numeric(32,16)
		,ProdId nvarchar(20)
	) 

	--untuk varibel cursor
	DECLARE @ItemId nvarchar(20), @InventBatch nvarchar(20)
		,@SalesQty numeric(32,16), @ProdId nvarchar(20)
			
	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT ITEMID,INVENTBATCHID,SUM(QTY)
	FROM [AXGMKDW].[dbo].[FactSalesInvoice]
	WHERE YEAR(INVOICEDATE) = @Year		--2025
		AND MONTH(INVOICEDATE) = @Month	--6
	GROUP BY ITEMID,INVENTBATCHID

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @ItemId,@InventBatch,@SalesQty
 
    WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @ProdId = ''
		SELECT @ProdId = ProdId
			FROM [AXGMKDW].[dbo].[FactPor]
			WHERE ITEMID = @ItemId	--'4100013'
				AND INVENTBATCHID = @InventBatch	--'82521018'

		IF @ProdId <> ''
		BEGIN
			IF EXISTS ( 
				SELECT PRODID FROM [AXGMKDW].[dbo].[FactGmk_RmpmCalcTable] WHERE PRODID = @ProdId
			)
			BEGIN
				INSERT INTO @Result([Year],[Month],ItemId,[Source],ActQty,ActPrice, ProductId, InventbatchId, SalesQty, ProdId)
					SELECT @Year
						,@Month
						,a.ItemId 
						,'FG-'+@ItemId+'-'+@InventBatch as [Source]
						--,a.[QTY] * @SalesQty * -1 as [Qty]
						,a.[QTY] * -1 as [Qty]
						,a.[COSTAMOUNT] / NULLIF(a.[QTY],0) as [Price]	
						,@ItemId
						,@InventBatch
						,@SalesQty
						,@ProdId
					FROM [AXGMKDW].[dbo].[FactGmk_RmpmCalcTable] a
					LEFT JOIN [AXGMKDW].[dbo].[DimItem] i ON i.ITEMID = a.ItemId
					WHERE a.PRODID = @ProdId	--'POR25-026966'
						AND a.InventItemType in (0,1)		
			END
			ELSE --catat ItemId dan BN yang tidak ditemukan
			BEGIN
				INSERT INTO FgActNotBreakDown([Year], [Month], [ItemId], [InventBatch], [ProdId], [Qty])
				VALUES (@Year, @Month, @ItemId, @InventBatch, @ProdId, @SalesQty)
			END

		END
		ELSE --catat ItemId dan BN yang tidak ditemukan
		BEGIN
			INSERT INTO FgActNotBreakDown([Year], [Month], [ItemId], [InventBatch], [ProdId], [Qty])
			VALUES (@Year, @Month, @ItemId, @InventBatch, '', @SalesQty)
		END

	FETCH NEXT FROM Cur01 INTO @ItemId,@InventBatch,@SalesQty
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01

	DELETE FROM MaterialAct 
		WHERE [Year] = @Year AND [Month] = @Month 

	INSERT INTO [MaterialAct] ([Year],[Month],[ItemId],[Source],[ActQty],[ActPrice], ProductId, InventBatchId, SalesQty, ProdId)	
		SELECT [Year]
		  ,[Month]
		  ,[ItemId]
		  ,[Source]
		  ,[ActQty]
		  ,ISNULL([ActPrice],0)
		  ,ProductId
		  ,InventbatchId
		  ,SalesQty
		  ,ProdId
	  FROM @Result
	  ORDER BY ItemId

	SELECT * FROM [MaterialAct]
	WHERE [Year] = @Year AND [Month] = @Month 

END

	--SELECT * FROM [MaterialAct]
	--WHERE [Year] = 2025 AND [Month] = 8 

GO

