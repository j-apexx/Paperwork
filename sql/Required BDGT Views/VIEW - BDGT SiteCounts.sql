USE [NUTR1]
GO

/****** Object:  View [dbo].[vwBDGT_SiteCounts]    Script Date: 3/4/2020 4:02:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[vwBDGT_SiteCounts]
AS
SELECT        HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, Counts.ORD, Sites.BDGTSiteName, Counts.FreeB_Site, Counts.ElemPaidB_Site, Counts.MiddlePaidB_Site, Counts.HSPaidB_Site, 
                         Counts.ElemReducedB_Site, Counts.MiddleReducedB_Site, Counts.HSReducedB_Site, Counts.AdultPaidB_Site, Counts.AlaCarteB_Site, Counts.ElemPaidTier1B_Site, Counts.ElemPaidTier2B_Site, 
                         Counts.MiddlePaidTier1B_Site, Counts.MiddlePaidTier2B_Site, Counts.HSPaidTier1B_Site, Counts.HSPaidTier2B_Site, Counts.FreeL_Site, Counts.ElemPaidL_Site, Counts.MiddlePaidL_Site, 
                         Counts.HSPaidL_Site, Counts.ElemPaidTier1L_Site, Counts.ElemPaidTier2L_Site, Counts.MiddlePaidTier1L_Site, Counts.MiddlePaidTier2L_Site, Counts.HSPaidTier1L_Site, Counts.HSPaidTier2L_Site, 
                         Counts.ElemReducedL_Site, Counts.MiddleReducedL_Site, Counts.HSReducedL_Site, Counts.AdultPaidL_Site, Counts.AlaCarteL_Site, Counts.AlaCarteSalesRevInc_Site, Counts.SnackPaid_Site, 
                         Counts.SnackReduced_Site, Counts.SnackAdult_Site, Counts.SnackAlaCarte_Site, Counts.SnackFree_Site, Counts.OtherRevVending_Site, Counts.FreeBMid_Site, Counts.FreeBHS_Site, Counts.FreeLMid_Site, 
                         Counts.FreeLHS_Site
FROM            dbo.BDGT_CBS_SiteCounts AS Counts INNER JOIN
                         dbo.BDGT_BudgetHDR AS HDR ON HDR.BudgetYear = Counts.BudgetYear AND HDR.BidNumber = Counts.BidNumber AND HDR.BudgetVersion = Counts.BudgetVersion INNER JOIN
                         dbo.BDGT_Sites AS Sites ON Counts.BudgetYear = Sites.BudgetYear AND Counts.BidNumber = Sites.BidNumber AND Counts.ORD = Sites.ORD
WHERE        (HDR.ACTIVE = 1)
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[26] 4[35] 2[20] 3) )"
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
         Begin Table = "Sites"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 235
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Counts"
            Begin Extent = 
               Top = 6
               Left = 273
               Bottom = 136
               Right = 492
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "HDR"
            Begin Extent = 
               Top = 6
               Left = 530
               Bottom = 136
               Right = 730
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_SiteCounts'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_SiteCounts'
GO

