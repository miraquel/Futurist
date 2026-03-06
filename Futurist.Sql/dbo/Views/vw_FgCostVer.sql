
CREATE VIEW [dbo].[vw_FgCostVer]
AS
SELECT TOP (100) 
PERCENT a.Room, a.VerId, a.RecId AS RofoId, a.ItemId AS ProductId, REPLACE(REPLACE(REPLACE(i.SEARCHNAME, CHAR(9), ''), CHAR(10), ''), CHAR(13), '') AS ProductName, i.UNITID AS Unit, i.NETWEIGHT / 1000 AS UnitInKg, a.RofoDate, 
                       b.QtyRofo AS RofoQty, b.Yield, b.RmPrice, b.PmPrice, b.RmPrice + b.PmPrice AS [RmPm+Y], b.StdCostPrice, b.CostPrice, a.SalesPrice
FROM              dbo.RofoVer AS a WITH (NOLOCK) LEFT OUTER JOIN
                       AXGMKDW.dbo.CIDMDb_DImItem AS i WITH (NOLOCK) ON i.ITEMID = a.ItemId LEFT OUTER JOIN
                       dbo.FgCostVer AS b ON b.ProductId = a.ItemId AND b.RofoDate = a.RofoDate AND b.VerId = a.VerId LEFT OUTER JOIN
                       dbo.SalesPriceVer AS p ON p.ItemId = a.ItemId AND p.VerId = a.VerId
WHERE             EXISTS
                           (SELECT            1 AS Expr1
                              FROM              dbo.Version
                              WHERE             (Room = a.Room) AND (Cancel = 0))

GO

EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "a"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 206
               Right = 279
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "i"
            Begin Extent = 
               Top = 9
               Left = 336
               Bottom = 206
               Right = 596
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "b"
            Begin Extent = 
               Top = 9
               Left = 686
               Bottom = 206
               Right = 908
            End
            DisplayFlags = 280
            TopColumn = 10
         End
         Begin Table = "p"
            Begin Extent = 
               Top = 9
               Left = 965
               Bottom = 206
               Right = 1189
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_FgCostVer';


GO

EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_FgCostVer';


GO

