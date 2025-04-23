USE TWO
GO

/****** Object:  View [dbo].[vw_BDGT_SpecialFunctionsCostTotals]    Script Date: 11/5/2019 10:59:57 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_BDGT_SpecialFunctionsCostTotals]
AS
SELECT     HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, 
                      ISNULL(P.ParticSpecialFunctions * (S.CurrCostFood + ROUND(S.CurrCostFood * S.CostIncreasePct * 0.0001 * 0.01, 5)), 0) AS SFTotalCostFoodOnly, 
                      ISNULL(P.ParticSpecialFunctions * (S.CurrCostPaper + ROUND(S.CurrCostPaper * S.CostIncreasePct * 0.0001 * 0.01, 5)), 0) AS SFTotalCostPaperOnly, 
                      ISNULL(P.ParticSpecialFunctions * (S.CurrCostCleaning + ROUND(S.CurrCostCleaning * S.CostIncreasePct * 0.0001 * 0.01, 5)), 0) AS SFTotalCostCleaningOnly, 
                      ISNULL(P.ParticSpecialFunctions * (S.CurrCostOther + ROUND(S.CurrCostOther * S.CostIncreasePct * 0.0001 * 0.01, 5)), 0) AS SFTotalCostOtherOnly, 
                      ISNULL(S.ClientWages, 0) AS SFDistrictLaborCost, 
                      ISNULL(S.NutritionWages * (LaborN.FICAPCT + LaborN.FUTAPCT + LaborN.MEDICAREPCT + LaborN.OBLIGATIONSPCT + LaborN.PASUIPCT + LaborN.RETIREPCT + LaborN.SOCSECPCT
                       + LaborN.WORKFORCEPCT) * 0.0001 * 0.01, 0) + ISNULL(S.NutritionWages * LaborN.WCompPCT * 0.01 * 0.0001, 0) + ISNULL(S.NutritionWages, 0) 
                      AS SFNutritionLaborCost, ISNULL(O.CACFP * D.DaysOfSvcSFSP * ROUND(CACFP.CACFPAdminFeePM, 5), 0) AS SFNutritionAdminCost, 
                      ISNULL(CACFP.CACFPTotalPrice * CACFP.CACFPFoodCost, 0) AS CACFPFoodCost, ISNULL(CACFP.CACFPTotalPrice * CACFP.CACFPPaperCost, 0) AS CACFPPaperCost, 
                      ISNULL(CACFP.CACFPTotalPrice * CACFP.CACFPCleaningCost, 0) AS CACFPCleaningCost, ISNULL(S.NutritionWages, 0) AS NutrWagesOnly
FROM         dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                      dbo.BDGT_SpecialFunctionsCst AS S ON HDR.BidNumber = S.BidNumber AND HDR.BudgetYear = S.BudgetYear AND 
                      HDR.BudgetVersion = S.BudgetVersion LEFT OUTER JOIN
                      dbo.vwBDGT_Participation AS P ON HDR.BidNumber = P.BidNumber AND HDR.BudgetYear = P.BudgetYear AND 
                      HDR.BudgetVersion = P.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_CBS_Other AS O ON HDR.BidNumber = O.BidNumber AND HDR.BudgetYear = O.BudgetYear AND HDR.BudgetVersion = O.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_DaysOfService AS D ON HDR.BidNumber = D.BidNumber AND HDR.BudgetYear = D.BudgetYear AND 
                      HDR.BudgetVersion = D.BudgetVersion LEFT OUTER JOIN
                      dbo.vwBDGT_CACFP AS CACFP ON HDR.BidNumber = CACFP.BidNumber AND HDR.BudgetYear = CACFP.BudgetYear AND 
                      HDR.BudgetVersion = CACFP.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_LaborNUTR AS LaborN ON HDR.BidNumber = LaborN.BidNumber AND HDR.BudgetYear = LaborN.BudgetYear AND 
                      HDR.BudgetVersion = LaborN.BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[39] 4[15] 2[29] 3) )"
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
               Top = 138
               Left = 38
               Bottom = 246
               Right = 215
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "S"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 266
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "P"
            Begin Extent = 
               Top = 138
               Left = 244
               Bottom = 246
               Right = 452
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "O"
            Begin Extent = 
               Top = 6
               Left = 304
               Bottom = 136
               Right = 537
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "D"
            Begin Extent = 
               Top = 6
               Left = 575
               Bottom = 136
               Right = 768
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CACFP"
            Begin Extent = 
               Top = 114
               Left = 806
               Bottom = 222
               Right = 982
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "LaborN"
            Begin Extent = 
               Top = 6
               Left = 806
               Bottom = 114
               Right = 974
            End
            DisplayFlags = 280
            TopColumn = 0
      ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_BDGT_SpecialFunctionsCostTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'   End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 14
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
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
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_BDGT_SpecialFunctionsCostTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_BDGT_SpecialFunctionsCostTotals'
GO

