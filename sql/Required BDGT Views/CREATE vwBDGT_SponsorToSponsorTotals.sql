USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_SponsorToSponsorTotals]    Script Date: 11/5/2019 11:18:50 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[vwBDGT_SponsorToSponsorTotals]
AS
SELECT     HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, HDR.BudgetType, 
                      CASE WHEN HDR.BudgetYear = '2017-2018' THEN ISNULL(CASE HDR.BudgetType WHEN 1 THEN ISNULL(SP.FP_FoodPartic * CASE WHEN TOTAL1 > 0 THEN ROUND(ISNULL(FC.SubtotalCostFoodOnly
                       + FC.RebatesDiscounts + FC.VP_GMAdj, 0) / TOTAL1, 4) ELSE 0 END, 0) + ISNULL(SP.FP_PaperPartic * CASE WHEN TOTAL1 > 0 THEN ROUND(ISNULL(NF.PaperOnly, 
                      0) / TOTAL1, 4) ELSE 0 END, 0) + ISNULL(SP.FP_CleanPartic * CASE WHEN TOTAL1 > 0 THEN ROUND(ISNULL(NF.CleaningOnly, 0) / TOTAL1, 4) ELSE 0 END, 0) 
                      ELSE ((([CR_CurrCostFood] * [CostIncreasePct] * 0.0001 * 0.01) + [CR_CurrCostFood]) * [CR_FoodPartic]) + ((([CR_CurrCostPaper] * [CostIncreasePct] * 0.0001 * 0.01) 
                      + [CR_CurrCostPaper]) * [CR_PaperPartic]) + ((([CR_CurrCostCleaning] * [CostIncreasePct] * 0.0001 * 0.01) + [CR_CurrCostCleaning]) * [CR_CleaningPartic]) 
                      + ((([CR_CurrCostOther] * [CostIncreasePct] * 0.0001 * 0.01) + [CR_CurrCostOther]) * [CR_OtherPartic]) END, 0) 
                      ELSE ISNULL((SP2.CR_CurrCostFoodArray_1 * SP2.CR_FoodParticArray_1) + (SP2.CR_CurrCostFoodArray_2 * SP2.CR_FoodParticArray_2) 
                      + (SP2.CR_CurrCostFoodArray_3 * SP2.CR_FoodParticArray_3), 0) + ISNULL((SP2.CR_CurrCostPaperArray_1 * SP2.CR_PaperParticArray_1) 
                      + (SP2.CR_CurrCostPaperArray_2 * SP2.CR_PaperParticArray_2) + (SP2.CR_CurrCostPaperArray_3 * SP2.CR_PaperParticArray_3), 0) 
                      + ISNULL((SP2.CR_CurrCostCleanArray_1 * SP2.CR_CleaningParticArray_1) + (SP2.CR_CurrCostCleanArray_2 * SP2.CR_CleaningParticArray_2) 
                      + (SP2.CR_CurrCostCleanArray_3 * SP2.CR_CleaningParticArray_3), 0) + ISNULL((SP2.CR_CurrCostOtherArray_1 * SP2.CR_OtherParticArray_1) 
                      + (SP2.CR_CurrCostOtherArray_2 * SP2.CR_OtherParticArray_2) + (SP2.CR_CurrCostOtherArray_3 * SP2.CR_OtherParticArray_3), 0) END AS PDE3086TOTAL, 
                      CASE WHEN HDR.BudgetYear = '2017-2018' THEN ISNULL(SP.FP_FoodPartic * CASE WHEN TOTAL1 > 0 THEN ROUND(ISNULL(FC.SubtotalCostFoodOnly + FC.RebatesDiscounts
                       + FC.VP_GMAdj, 0) / TOTAL1, 4) ELSE 0 END, 0) WHEN HDR.BudgetType = 1 AND 
                      HDR.BudgetYear != '2017-2018' THEN ISNULL((SP2.CR_CurrCostFoodArray_1 * SP2.CR_FoodParticArray_1) 
                      + (SP2.CR_CurrCostFoodArray_2 * SP2.CR_FoodParticArray_2) + (SP2.CR_CurrCostFoodArray_3 * SP2.CR_FoodParticArray_3), 0) ELSE 0 END AS FP_FoodTotal, 
                      CASE WHEN HDR.BudgetYear = '2017-2018' THEN ISNULL(SP.FP_PaperPartic * CASE WHEN TOTAL1 > 0 THEN ROUND(ISNULL(NF.PaperOnly, 0) / TOTAL1, 4) 
                      ELSE 0 END, 0) WHEN HDR.BudgetType = 1 AND HDR.BudgetYear != '2017-2018' THEN ISNULL((SP2.CR_CurrCostPaperArray_1 * SP2.CR_PaperParticArray_1) 
                      + (SP2.CR_CurrCostPaperArray_2 * SP2.CR_PaperParticArray_2) + (SP2.CR_CurrCostPaperArray_3 * SP2.CR_PaperParticArray_3), 0) ELSE 0 END AS FP_PaperTotal, 
                      CASE WHEN HDR.BudgetYear = '2017-2018' THEN ISNULL(SP.FP_CleanPartic * CASE WHEN TOTAL1 > 0 THEN ROUND(ISNULL(NF.CleaningOnly, 0) / TOTAL1, 4) 
                      ELSE 0 END, 0) WHEN HDR.BudgetType = 1 AND HDR.BudgetYear != '2017-2018' THEN ISNULL((SP2.CR_CurrCostCleanArray_1 * SP2.CR_CleaningParticArray_1) 
                      + (SP2.CR_CurrCostCleanArray_2 * SP2.CR_CleaningParticArray_2) + (SP2.CR_CurrCostCleanArray_3 * SP2.CR_CleaningParticArray_3), 0) 
                      ELSE 0 END AS FP_CleaningTotal, CASE WHEN HDR.BudgetType = 1 AND 
                      HDR.BudgetYear != '2017-2018' THEN ISNULL((SP2.CR_CurrCostOtherArray_1 * SP2.CR_OtherParticArray_1) 
                      + (SP2.CR_CurrCostOtherArray_2 * SP2.CR_OtherParticArray_2) + (SP2.CR_CurrCostOtherArray_3 * SP2.CR_OtherParticArray_3), 0) ELSE 0 END AS FP_OtherTotal, 
                      CASE WHEN HDR.BudgetType = 2 AND HDR.BudgetYear != '2017-2018' THEN ISNULL((SP2.CR_CurrCostFoodArray_1 * SP2.CR_FoodParticArray_1) 
                      + (SP2.CR_CurrCostFoodArray_2 * SP2.CR_FoodParticArray_2) + (SP2.CR_CurrCostFoodArray_3 * SP2.CR_FoodParticArray_3), 0) ELSE 0 END AS CR_FoodTotal, 
                      CASE WHEN HDR.BudgetType = 2 AND HDR.BudgetYear != '2017-2018' THEN ISNULL((SP2.CR_CurrCostPaperArray_1 * SP2.CR_PaperParticArray_1) 
                      + (SP2.CR_CurrCostPaperArray_2 * SP2.CR_PaperParticArray_2) + (SP2.CR_CurrCostPaperArray_3 * SP2.CR_PaperParticArray_3), 0) ELSE 0 END AS CR_PaperTotal, 
                      CASE WHEN HDR.BudgetType = 2 AND HDR.BudgetYear != '2017-2018' THEN ISNULL((SP2.CR_CurrCostCleanArray_1 * SP2.CR_CleaningParticArray_1) 
                      + (SP2.CR_CurrCostCleanArray_2 * SP2.CR_CleaningParticArray_2) + (SP2.CR_CurrCostCleanArray_3 * SP2.CR_CleaningParticArray_3), 0) 
                      ELSE 0 END AS CR_CleaningTotal, CASE WHEN HDR.BudgetType = 2 AND 
                      HDR.BudgetYear != '2017-2018' THEN ISNULL((SP2.CR_CurrCostOtherArray_1 * SP2.CR_OtherParticArray_1) 
                      + (SP2.CR_CurrCostOtherArray_2 * SP2.CR_OtherParticArray_2) + (SP2.CR_CurrCostOtherArray_3 * SP2.CR_OtherParticArray_3), 0) 
                      ELSE 0 END AS CR_OtherTotal
FROM         dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                      dbo.BDGT_SponsorToSponsor AS SP ON HDR.BidNumber = SP.BidNumber AND HDR.BudgetYear = SP.BudgetYear AND 
                      HDR.BudgetVersion = SP.BudgetVersion LEFT OUTER JOIN
                      dbo.vwBDGT_MealEquivalents AS ME ON HDR.BidNumber = ME.BidNumber AND HDR.BudgetYear = ME.BudgetYear AND 
                      HDR.BudgetVersion = ME.BudgetVersion LEFT OUTER JOIN
                      dbo.vwBDGT_FOODCOST AS FC ON HDR.BidNumber = FC.BidNumber AND HDR.BudgetYear = FC.BudgetYear AND 
                      HDR.BudgetVersion = FC.BudgetVersion LEFT OUTER JOIN
                      dbo.vwBDGT_NonFoodCostsTotals1516 AS NF ON HDR.BidNumber = NF.BidNumber AND HDR.BudgetYear = NF.BudgetYear AND 
                      HDR.BudgetVersion = NF.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_SponsorToSpons2 AS SP2 ON HDR.BidNumber = SP2.BidNumber AND HDR.BudgetYear = SP2.BudgetYear AND HDR.BudgetVersion = SP2.BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[34] 4[27] 2[23] 3) )"
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
         Begin Table = "HDR"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 215
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SP"
            Begin Extent = 
               Top = 6
               Left = 253
               Bottom = 114
               Right = 445
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ME"
            Begin Extent = 
               Top = 6
               Left = 483
               Bottom = 114
               Right = 687
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "FC"
            Begin Extent = 
               Top = 6
               Left = 725
               Bottom = 114
               Right = 936
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "NF"
            Begin Extent = 
               Top = 114
               Left = 38
               Bottom = 222
               Right = 209
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SP2"
            Begin Extent = 
               Top = 6
               Left = 974
               Bottom = 114
               Right = 1184
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
         Width ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_SponsorToSponsorTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'= 1500
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
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_SponsorToSponsorTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_SponsorToSponsorTotals'
GO

