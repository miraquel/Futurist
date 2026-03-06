-- =============================================
-- Author:		<Andi>
-- Create date: <11 Aug 2025>
-- Description:	<Untuk data analisa material plan vs actual>
-- EXEC MaterialPlan_Calc 1,76,2025,10
-- =============================================
CREATE PROCEDURE [dbo].[MaterialPlan_Calc]
@Room int = 1,
@VerId int = 83,	
@Year int = 2025,
@Month int = 12		
AS
BEGIN
	-- hapus dulu FgPlanNotBreakDown
	DELETE FgPlanNotBreakDown WHERE [Year] = @Year AND [Month] = @Month

	--untuk tabel hasil Plan
	DECLARE @Result table (
		Room int
		,VerId int
		,[Year] int
		,[Month] int
		,ItemId nvarchar(20)
		,ItemName nvarchar(60)
		,UnitId nvarchar(20)
		,[Source] nvarchar(50)
		,InventBatch nvarchar(20)
		,PlanQty numeric(32,16)
		,PlanPrice numeric(32,16)
	) 

	--untuk varibel cursor
	DECLARE @ItemId nvarchar(20), @InventBatch nvarchar(20)
		,@PlanQty numeric(32,16), @PlanPrice numeric(32,16)
		,@ProdId nvarchar(20)

	INSERT INTO @Result(
		Room,VerId,[Year],[Month],ItemId,ItemName,UnitId
		,[Source],InventBatch,PlanQty,PlanPrice)
	SELECT 
		a.Room
		,a.VerId
		,YEAR(a.RofoDate)
		,MONTH(a.RofoDate)
		,d.ItemId as ItemAllocatedId
		,REPLACE(REPLACE(REPLACE(id.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemAllocatedName
		,id.UnitId
		,d.[Source]
		,ISNULL(d.InventBatch,'')
		,d.Qty
		,d.Price
	FROM RofoVer a  WITH (NOLOCK)
	JOIN MupVer b WITH (NOLOCK) ON b.RofoId = a.RecId AND b.VerId = a.VerId
	JOIN MupTransVer c WITH (NOLOCK) ON c.MupId = b.RecId AND c.VerId = b.VerId
	JOIN ItemTransVer d WITH (NOLOCK) ON d.RecId = c.ItemTransId AND d.VerId = c.VerId
	LEFT JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
	WHERE a.Room = @Room 
		AND a.VerId = @VerId
		AND YEAR(a.RofoDate) = @Year
		AND MONTH(a.RofoDate) = @Month
		--AND d.ItemId = '4100013'

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT ItemId,InventBatch,PlanQty,PlanPrice
		FROM @Result
		WHERE ItemId LIKE '4%'

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @ItemId,@InventBatch,@PlanQty,@PlanPrice
 
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
				INSERT INTO @Result(
						Room,VerId,[Year],[Month],ItemId,ItemName,UnitId
						,[Source],InventBatch,PlanQty,PlanPrice)
					SELECT 
						@Room
						,@VerId
						,@Year
						,@Month
						,a.ItemId 
						,REPLACE(REPLACE(REPLACE(a.ItemName,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemName
						,i.UnitId
						,'FG-'+@ItemId+'-'+@InventBatch as [Source]
						,'' as [InventBatch]
						,a.[QTY] * @PlanQty * -1 as [Qty]
						,a.[COSTAMOUNT] / NULLIF(a.[QTY],0) as [Price]				
					FROM [AXGMKDW].[dbo].[FactGmk_RmpmCalcTable] a
					LEFT JOIN [AXGMKDW].[dbo].[DimItem] i ON i.ITEMID = a.ItemId
					LEFT JOIN [ItemGroup] g ON g.ITEMID = a.ItemId
					WHERE a.PRODID = @ProdId	--'POR25-026966'
						AND a.InventItemType in (0,1)

				--Hapus FG nya jika breakdown materialnya sudah berhasil dimasukkan
				DELETE FROM @Result 
					WHERE ItemId = @ItemId AND InventBatch = @InventBatch	
			END
			ELSE --catat ItemId dan BN yang tidak ditemukan
			BEGIN
				INSERT INTO FgPlanNotBreakDown([Year], [Month], [ItemId], [InventBatch], [ProdId], [Qty])
				VALUES (@Year, @Month, @ItemId, @InventBatch, @ProdId, @PlanQty)
			END
		END
		ELSE --catat ItemId dan BN yang tidak ditemukan
		BEGIN
			INSERT INTO FgPlanNotBreakDown([Year], [Month], [ItemId], [InventBatch], [ProdId], [Qty])
			VALUES (@Year, @Month, @ItemId, @InventBatch, '', @PlanQty)
		END

	FETCH NEXT FROM Cur01 INTO @ItemId,@InventBatch,@PlanQty,@PlanPrice
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01

	DELETE FROM MaterialPlan 
		WHERE Room = @Room AND VerId = @VerId AND [Year] = @Year AND [Month] = @Month 

	INSERT INTO [MaterialPlan] ([Room],[VerId],[Year],[Month],[ItemId],[ItemName],[UnitId]
      ,[Source],[InventBatch],[PlanQty],[PlanPrice])	
		SELECT [Room]
		  ,[VerId]
		  ,[Year]
		  ,[Month]
		  ,[ItemId]
		  ,[ItemName]
		  ,[UnitId]
		  ,[Source]
		  ,[InventBatch]
		  ,[PlanQty]
		  ,ISNULL([PlanPrice],0)
	  FROM @Result
	  ORDER BY ItemId

	SELECT * FROM [MaterialPlan]
	WHERE Room = @Room AND VerId = @VerId AND [Year] = @Year AND [Month] = @Month 

END

GO

