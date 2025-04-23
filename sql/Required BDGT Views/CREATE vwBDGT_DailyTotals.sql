USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_DailyTotals]    Script Date: 11/5/2019 10:58:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_DailyTotals]
AS
SELECT        dbo.BDGT_BudgetHDR.BidNumber, dbo.BDGT_BudgetHDR.BudgetYear, dbo.BDGT_BudgetHDR.BudgetVersion, 
                         dbo.BDGT_CBS_Breakfast.ElemPaid + dbo.BDGT_CBS_Breakfast.MiddleSchoolPaid + dbo.BDGT_CBS_Breakfast.HighSchoolPaid AS DailyBPaidCount, 
                         dbo.BDGT_CBS_Breakfast.ElemPaidTierB + dbo.BDGT_CBS_Breakfast.MiddlePaidTierB + dbo.BDGT_CBS_Breakfast.HighSchoolPaidTierB AS DailyBPaidTierCount, 
                         dbo.BDGT_CBS_Breakfast.ElemReduced + dbo.BDGT_CBS_Breakfast.MiddleSchoolReduced + dbo.BDGT_CBS_Breakfast.HighSchoolReduced AS DailyBReducedCount, 
                         dbo.BDGT_CBS_FedReimbCounts.BreakfastFree + dbo.BDGT_CBS_FedReimbCounts.BreakfastFreeSevere AS DailyBFreeCount, 
                         dbo.BDGT_CBS_Lunches.ElemPaidL + dbo.BDGT_CBS_Lunches.MiddleSchoolPaidL + dbo.BDGT_CBS_Lunches.HighSchoolPaidL AS DailyLPaidCount, 
                         dbo.BDGT_CBS_Lunches.ElemPaidTier + dbo.BDGT_CBS_Lunches.MiddleSchoolPaidTier + dbo.BDGT_CBS_Lunches.HighSchoolPaidTier AS DailyLPaidTierCount, 
                         dbo.BDGT_CBS_Lunches.ElemReducedL + dbo.BDGT_CBS_Lunches.MiddleSchoolReducedL + dbo.BDGT_CBS_Lunches.HighSchoolReducedL AS DailyLReducedCount, 
                         dbo.BDGT_CBS_FedReimbCounts.HRLunchFree + dbo.BDGT_CBS_FedReimbCounts.LRLunchFree AS DailyLFreeCount
FROM            dbo.BDGT_BudgetHDR LEFT OUTER JOIN
                         dbo.BDGT_CBS_Breakfast ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_CBS_Breakfast.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_CBS_Breakfast.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_CBS_Breakfast.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_CBS_Lunches ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_CBS_Lunches.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_CBS_Lunches.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_CBS_Lunches.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_CBS_FedReimbCounts ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_CBS_FedReimbCounts.BidNumber AND 
                         dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_CBS_FedReimbCounts.BudgetYear AND dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_CBS_FedReimbCounts.BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
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
         Begin Table = "BDGT_BudgetHDR"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 238
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "BDGT_CBS_Breakfast"
            Begin Extent = 
               Top = 6
               Left = 276
               Bottom = 340
               Right = 507
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "BDGT_CBS_Lunches"
            Begin Extent = 
               Top = 6
               Left = 545
               Bottom = 336
               Right = 782
            End
            DisplayFlags = 280
            TopColumn = 23
         End
         Begin Table = "BDGT_CBS_FedReimbCounts"
            Begin Extent = 
               Top = 6
               Left = 820
               Bottom = 334
               Right = 1014
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
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
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
         Or' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_DailyTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_DailyTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_DailyTotals'
GO

