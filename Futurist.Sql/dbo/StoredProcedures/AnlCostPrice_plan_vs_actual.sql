-- =============================================
-- Author:		<Andi>
-- Create date: <10 Oct 2025>
-- Description:	<Analisa Cost price plan vs actual>
-- EXEC [AnlCostPrice_plan_vs_actual] 2025, 8, '4200031'
-- 4200031, 92530717
-- 4200031, 92530721
-- =============================================
CREATE PROCEDURE [dbo].[AnlCostPrice_plan_vs_actual]
@Room int = 1,
@VerId int = 72,	
@Year int = 2025,
@Month int = 8,
@ItemId nvarchar(20) = '4200031'
AS
BEGIN
	DECLARE @ActMaterial table (
	ItemId nvarchar(20)
	,ItemName nvarchar(60)
	,Qty numeric(32,16)
	,Price numeric(32,16)
	)

	INSERT INTO @ActMaterial (ItemId,ItemName,Qty,Price)
		SELECT a.ITEMID, a.INVENTBATCHID, b.PRODID, SUM(a.QTY) as [QTY]
		FROM [SCMBI].[dbo].[FactSalesInvoice] a
		LEFT JOIN [AXGMKDW].[dbo].[FactPor] b ON b.ITEMID = a.ITEMID AND b.INVENTBATCHID = a.INVENTBATCHID
		LEFT JOIN [AXGMKDW].[dbo].[FactGmk_RmpmCalcTable] c ON c.PRODID = b.PRODID
		WHERE YEAR([INVOICEDATE]) = 2025 AND MONTH([INVOICEDATE]) = 8
			AND a.ItemId = '4200031'
		GROUP BY a.ITEMID, a.INVENTBATCHID, b.PRODID

	select * from @ActMaterial
	return

	DECLARE @Material table (
	Room int
	,VerId int
	,ProductId nvarchar(20)
	,ProductName nvarchar(60)
	,RofoDate datetime
	,QtyRofo numeric(32,16)
	,ItemId nvarchar(20)
	,ItemName nvarchar(60)
	,Unit nvarchar(20)
	,GroupSubstitusi nvarchar(20)
	,ItemBatch nvarchar(20)
	,Qty numeric(32,16)
	,Price numeric(32,16)
	,[Source] nvarchar(50)
	)

	DECLARE @ProductId nvarchar(20)
		,@ProductName nvarchar(60)
		,@RofoDate datetime
		,@QtyRofo numeric(32,16)
		,@ItemIdFg nvarchar(20)
		,@ItemName nvarchar(60)
		,@Unit nvarchar(20)
		,@GroupSubstitusi nvarchar(20)
		,@ItemBatch nvarchar(20)
		,@Qty numeric(32,16)
		,@Price numeric(32,16)
		,@Source nvarchar(50)

	DECLARE @ProdId nvarchar(20)
		,@MatItemId nvarchar(20)
		,@MatItemName nvarchar(60)
		,@MatUnit nvarchar(20)
		,@MatInventBatchId nvarchar(20)
		,@MatQty numeric(32,16)
		,@MatPrice numeric(32,16)
		,@MatGroupSubstitusi nvarchar(20)

	DECLARE @Result as table (
		ItemId nvarchar(20)
		,ItemName nvarchar(60)
		,Unit nvarchar(20)
		,GroupSubstitusi nvarchar(20)
		,Qty numeric(32,16) 
		,PlanPrice numeric(32,16)
		,Contr numeric(32,16)
	)
	
	INSERT INTO @Material (Room,VerId,ProductId,ProductName,RofoDate,QtyRofo,ItemId,ItemName
		,Unit,GroupSubstitusi,ItemBatch,Qty,Price,[Source])

		SELECT a.Room ,a.VerId ,a.ItemId as ProductId
			,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ProductName
			,a.RofoDate,a.Qty as QtyRofo
			,d.ItemId as ItemAllocatedId
			,REPLACE(REPLACE(REPLACE(id.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemAllocatedName
			,id.UnitId
			,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
			,d.InventBatch
			,d.Qty
			,d.Price
			,d.[Source]
		FROM RofoVer a  WITH (NOLOCK)
		JOIN MupVer b WITH (NOLOCK) ON b.RofoId = a.RecId AND b.VerId = a.VerId
		JOIN MupTransVer c WITH (NOLOCK) ON c.MupId = b.RecId AND c.VerId = b.VerId
		JOIN ItemTransVer d WITH (NOLOCK) ON d.RecId = c.ItemTransId AND d.VerId = c.VerId
		LEFT JOIN AXGMKDW.dbo.DimItem i WITH (NOLOCK) ON i.ITEMID = a.ItemId
		LEFT JOIN AXGMKDW.dbo.DimItem ib WITH (NOLOCK) ON ib.ITEMID = b.ItemId
		LEFT JOIN AXGMKDW.dbo.DimItem id WITH (NOLOCK) ON id.ITEMID = d.ItemId
		LEFT JOIN [DimItemSubstituteVer] s WITH (NOLOCK) ON s.ItemId = b.ItemId AND s.VerId = b.VerId
		LEFT JOIN ItemPagVer p WITH (NOLOCK) ON p.RecId = d.RefId 
			AND p.Room = a.Room AND d.[Source] = 'Contract'
			AND p.VerId = a.VerId
		LEFT JOIN [ItemGroupProcurement] g ON g.ItemId = d.ItemId
		WHERE a.Room = 1	--@Room	--1	 
			AND a.VerId = 72	--@VerId	--66	
			AND YEAR(a.RofoDate) = 2025	--@Year	--2025
			AND MONTH(a.RofoDate) = 8	--@Month	--6
			AND a.ItemId = '4200031'	--@ItemId

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT ProductId,ProductName,RofoDate,QtyRofo,ItemId,ItemName
		,Unit,GroupSubstitusi,ItemBatch,Qty,Price,[Source]
		FROM @Material
		WHERE ItemId LIKE '4%'

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @ProductId,@ProductName,@RofoDate,@QtyRofo,@ItemIdFg,@ItemName,@Unit,@GroupSubstitusi,@ItemBatch,@Qty,@Price,@Source
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		SELECT @ProdId = [PRODID]
		  FROM [AXGMKDW].[dbo].[FactPor]
		  WHERE ITEMID = @ItemIdFg	-- '4200031'
			AND INVENTBATCHID = @ItemBatch	-- '92530717'

		INSERT INTO @Material (Room,VerId,ProductId,ProductName,RofoDate,QtyRofo,ItemId,ItemName
			,Unit,GroupSubstitusi,ItemBatch,Qty,Price,[Source])
			
			SELECT @Room,@VerId,@ProductId,@ProductName
				,@RofoDate,@QtyRofo,a.[ITEMID],a.[ITEMNAME]
				,b.UNITID
				,ISNULL(c.VtaMpSubstitusiGroupId,'')
				,''
				,a.[QTY] * @Qty * -1
				,a.[COSTAMOUNT] / a.[QTY]
				,'FG;' + @ProductId + ';' + @ItemBatch + ';' + CAST(CAST(@Qty AS INT) AS VARCHAR(100))
			FROM [AXGMKDW].[dbo].[FactGmk_RmpmCalcTable] a
				LEFT JOIN [AXGMKDW].[dbo].[DimItem] b ON b.ITEMID = a.ITEMID
				LEFT JOIN [AXGMKDW].[dbo].[DimItemSubstitute] c ON c.ITEMID = a.ITEMID
			WHERE [PRODID] = @ProdId	--'POR25-036814'
				AND InventItemType in (0,1)
			ORDER BY [ITEMID]
			
			DELETE FROM @Material WHERE ItemId = @ItemIdFg AND ItemBatch = @ItemBatch 

		FETCH NEXT FROM Cur01 INTO @ProductId,@ProductName,@RofoDate,@QtyRofo,@ItemIdFg,@ItemName,@Unit,@GroupSubstitusi,@ItemBatch,@Qty,@Price,@Source
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01

	--SELECT * FROM @Material
	INSERT INTO @Result (ItemId,ItemName,Unit,GroupSubstitusi,Qty,PlanPrice)
		SELECT ItemId
			,ItemName
			,Unit
			,GroupSubstitusi
			,SUM(Qty) as Qty
			,SUM(Qty*Price) / NULLIF(SUM(Qty),0) as [Price]
		FROM @Material
		GROUP BY ItemId, ItemName, Unit, GroupSubstitusi

	DECLARE @Tot numeric(32,16)
	SELECT @Tot = SUM(Qty * PlanPrice) FROM @Result

	UPDATE @Result
		SET Contr = (Qty * PlanPrice) / @Tot 

	SELECT ItemId,ItemName,Unit,GroupSubstitusi,Qty,PlanPrice,[Contr] 
		FROM @Result 	
		ORDER BY Contr DESC




	







	


	
END

GO

