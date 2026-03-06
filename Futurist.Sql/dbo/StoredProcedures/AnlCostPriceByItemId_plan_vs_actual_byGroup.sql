-- =============================================
-- Author:		<Andi>
-- Create date: <10 Oct 2025>
-- Description:	<Analisa Cost price plan vs actual>
-- EXEC [AnlCostPriceByItemId_plan_vs_actual_byGroup] 1, 72, 2025, 8, '4200031'
-- =============================================
CREATE PROCEDURE [dbo].[AnlCostPriceByItemId_plan_vs_actual_byGroup]
@Room int = 1,
@VerId int = 72,	
@Year int = 2025,
@Month int = 8,
@ItemId nvarchar(20) = '4200031'
AS
BEGIN		
	
	--Menampung material yang digunakan
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
	,GroupProc nvarchar(20)
	,GroupComm nvarchar(20)
	,ItemBatch nvarchar(20)
	,Qty numeric(32,16)
	,Price numeric(32,16)
	,[Source] nvarchar(50)
	)

	--Menampung variabel cursor
	DECLARE @ProductId nvarchar(20)
		,@ProductName nvarchar(60)
		,@RofoDate datetime
		,@QtyRofo numeric(32,16)
		,@ItemIdFg nvarchar(20)
		,@ItemName nvarchar(60)
		,@Unit nvarchar(20)
		,@GroupSubstitusi nvarchar(20)
		,@GroupProc nvarchar(20)
		,@GroupComm nvarchar(20)
		,@ItemBatch nvarchar(20)
		,@Qty numeric(32,16)
		,@Price numeric(32,16)
		,@Source nvarchar(50)
		,@ProdId nvarchar(20)
	
	--Menampung hasil (detail)
	DECLARE @Result as table (
		ItemId nvarchar(20)
		,ItemName nvarchar(60)
		,Unit nvarchar(20)
		,GroupSubstitusi nvarchar(20)
		,GroupProc nvarchar(20)
		,GroupComm nvarchar(20)
		,Qty numeric(32,16) 
		,PlanPrice numeric(32,16)
		,Contr numeric(32,16)
	)

	--Menampung hasil (summary per group)
	DECLARE @Result2 as table (		
		ItemId nvarchar(20)
		,ItemName nvarchar(60)
		,Unit nvarchar(20)
		,GroupSubstitusi nvarchar(20)
		,GroupProc nvarchar(20)
		,GroupComm nvarchar(20)
		,PlanQty numeric(32,16) 
		,PlanPrice numeric(32,16)
		,ActQty numeric(32,16) 
		,ActPrice numeric(32,16)
		,Contr numeric(32,16)
	)
	
	INSERT INTO @Material (Room,VerId,ProductId,ProductName,RofoDate,QtyRofo,ItemId,ItemName,Unit
		,GroupSubstitusi, GroupProc, GroupComm
		,ItemBatch,Qty,Price,[Source])

		SELECT a.Room ,a.VerId ,a.ItemId as ProductId
			,REPLACE(REPLACE(REPLACE(i.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ProductName
			,a.RofoDate,a.Qty as QtyRofo
			,d.ItemId as ItemAllocatedId
			,REPLACE(REPLACE(REPLACE(id.SEARCHNAME,CHAR(9),''),CHAR(10),''),CHAR(13),'') as ItemAllocatedName
			,id.UnitId
			,isnull(s.VtaMpSubstitusiGroupId,'') as [GroupSubstitusi]
			,isnull(g.Group01,'') as [GroupProc]
			,isnull(g.Group02,'') as [GroupComm]
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
		LEFT JOIN [ItemGroup] g ON g.ItemId = b.ItemId
		WHERE a.Room = @Room	--@Room	--1	 
			AND a.VerId = @VerId	--@VerId	--66	
			AND YEAR(a.RofoDate) = @Year		--@Year	--2025
			AND MONTH(a.RofoDate) = @Month	--@Month	--6
			AND a.ItemId = @ItemId	--'4200031'	--

	DECLARE Cur01 CURSOR READ_ONLY
	FOR
	SELECT ProductId,ProductName,RofoDate,QtyRofo,ItemId,ItemName,Unit
	,GroupSubstitusi, GroupProc, GroupComm
	,ItemBatch,Qty,Price,[Source]
		FROM @Material
		WHERE ItemId LIKE '4%'

    OPEN Cur01
    FETCH NEXT FROM Cur01 INTO @ProductId,@ProductName,@RofoDate,@QtyRofo,@ItemIdFg,@ItemName,@Unit,@GroupSubstitusi,@GroupProc,@GroupComm,@ItemBatch,@Qty,@Price,@Source
 
    WHILE @@FETCH_STATUS = 0
    BEGIN		
		SELECT @ProdId = [PRODID]
		  FROM [AXGMKDW].[dbo].[FactPor]
		  WHERE ITEMID = @ItemIdFg	-- '4200031'
			AND INVENTBATCHID = @ItemBatch	-- '92530717'

		INSERT INTO @Material (Room,VerId,ProductId,ProductName,RofoDate,QtyRofo,ItemId,ItemName,Unit
		,GroupSubstitusi,GroupProc,GroupComm
		,ItemBatch,Qty,Price,[Source])
			
			SELECT @Room,@VerId,@ProductId,@ProductName
				,@RofoDate,@QtyRofo,a.[ITEMID],a.[ITEMNAME]
				,b.UNITID
				,ISNULL(c.VtaMpSubstitusiGroupId,'')
				,ISNULL(d.Group01,'') as GroupProc
				,ISNULL(d.Group02,'') as GroupComm
				,''
				,a.[QTY] * @Qty * -1
				,a.[COSTAMOUNT] / NULLIF(a.[QTY],0)
				,'FG;' + @ProductId + ';' + @ItemBatch + ';' + CAST(CAST(@Qty AS INT) AS VARCHAR(100))
			FROM [AXGMKDW].[dbo].[FactGmk_RmpmCalcTable] a
				LEFT JOIN [AXGMKDW].[dbo].[DimItem] b ON b.ITEMID = a.ITEMID
				LEFT JOIN [AXGMKDW].[dbo].[DimItemSubstitute] c ON c.ITEMID = a.ITEMID
				LEFT JOIN [ItemGroup] d ON d.ITEMID = a.ITEMID
			WHERE [PRODID] = @ProdId	--'POR25-036814'
				AND InventItemType in (0,1)
			ORDER BY [ITEMID]
			
			DELETE FROM @Material WHERE ItemId = @ItemIdFg AND ItemBatch = @ItemBatch 

		FETCH NEXT FROM Cur01 INTO @ProductId,@ProductName,@RofoDate,@QtyRofo,@ItemIdFg,@ItemName,@Unit,@GroupSubstitusi,@GroupProc,@GroupComm,@ItemBatch,@Qty,@Price,@Source
    END
 
    CLOSE Cur01
    DEALLOCATE Cur01

	--SELECT * FROM @Material
	--RETURN

	--
	INSERT INTO @Result (ItemId,ItemName,Unit,GroupSubstitusi,GroupProc,GroupComm,Qty,PlanPrice)
		SELECT ItemId
			,ItemName
			,Unit
			,GroupSubstitusi,GroupProc,GroupComm
			,SUM(Qty) as Qty
			,SUM(Qty*Price) / NULLIF(SUM(Qty),0) as [Price]
		FROM @Material
		GROUP BY ItemId, ItemName, Unit,GroupSubstitusi,GroupProc,GroupComm

	DECLARE @Tot numeric(32,16)
	SELECT @Tot = SUM(Qty * PlanPrice) FROM @Result

	UPDATE @Result
		SET Contr = (Qty * PlanPrice) / NULLIF(@Tot ,0)	

	--

	DECLARE @ActMaterial table (
	ItemId nvarchar(20)
	,ItemName nvarchar(60)
	,Qty numeric(32,16)
	,Price numeric(32,16)
	)

	INSERT INTO @ActMaterial (ItemId,ItemName,Qty,Price)
		SELECT c.ITEMID, c.ITEMNAME
			,c.[QTY] * a.Qty * -1 as [QTY]
			,c.[COSTAMOUNT] / NULLIF(c.[QTY],0) as [Price]
		FROM [SCMBI].[dbo].[FactSalesInvoice] a
		LEFT JOIN [AXGMKDW].[dbo].[FactPor] b ON b.ITEMID = a.ITEMID AND b.INVENTBATCHID = a.INVENTBATCHID
		LEFT JOIN [AXGMKDW].[dbo].[FactGmk_RmpmCalcTable] c ON c.PRODID = b.PRODID
		WHERE YEAR([INVOICEDATE]) = @Year AND MONTH([INVOICEDATE]) = @Month
			AND InventItemType in (0,1)
			AND a.ItemId = @ItemId	--'4200031'	--
			ORDER BY c.[ITEMID]

	INSERT INTO @Result2 (ItemId,ItemName,Unit,GroupComm,PlanQty,PlanPrice,ActQty,ActPrice)
	SELECT a.ItemId, a.ItemName, a.Unit
		,a.GroupComm as GroupSubstitusi
		,a.Qty as PlanQty
		,a.PlanPrice
		,b.Qty as ActQty
		,b.Price as ActPrice 
	FROM @Result a
	LEFT JOIN (
		SELECT ItemId, SUM(Qty) as Qty, SUM (Qty*Price) / NULLIF(SUM(Qty),0) as [Price]
		FROM @ActMaterial  
		GROUP BY ItemId
	) b ON b.ItemId = a.ItemId
	WHERE LEFT(a.ItemId,1) <> '9' 
	ORDER BY Contr DESC

	SELECT @Tot = SUM(ActQty * ActPrice) FROM @Result2

	UPDATE @Result2
		SET Contr = (ActQty * ActPrice) / NULLIF(@Tot ,0)	

	SELECT * FROM @Result2


		
	--SELECT @Room as Room
	--	,@VerId as VerId
	--	,@Year as [Year]
	--	,@Month as [Month]
	--	,a.ItemId, a.ItemName, a.Unit
	--	,a.GroupComm as GroupSubstitusi
	--	,a.Qty as PlanQty
	--	,a.PlanPrice
	--	,b.Qty as ActQty
	--	,b.Price as ActPrice 
	--	,[Contr]
	--	,b.Price/NULLIF(a.PlanPrice,0) [A/P]
	--FROM @Result a
	--LEFT JOIN (
	--	SELECT ItemId, SUM(Qty) as Qty, SUM (Qty*Price) / NULLIF(SUM(Qty),0) as [Price]
	--	FROM @ActMaterial  
	--	GROUP BY ItemId
	--) b ON b.ItemId = a.ItemId
	--WHERE LEFT(a.ItemId,1) <> '9' 
	--ORDER BY Contr DESC


	


	
END

GO

