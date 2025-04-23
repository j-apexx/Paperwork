USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_CBS_SummaryTotals]    Script Date: 11/5/2019 10:58:14 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_CBS_SummaryTotals]
AS
SELECT        HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, ISNULL(FC.SubtotalCostFoodOnly + FC.RebatesDiscounts + FC.VP_GMAdj, 0) AS CBS_FoodCostIncComm, ISNULL(CBSA.CommodityDeliveryCharge, 0) 
                         AS CommodityDeliveryCharge, ISNULL(NUTR.AnnualWages, 0) AS CBS_LaborCost, 
                         ISNULL(NUTR.Fringes + NUTR.Retirement + NUTR.FICA + NUTR.FUTA + NUTR.PASUI + NUTR.NUTRPayrollRelated + NUTR.WCOMP + NUTR.OtherLaborDLR + NUTR.NUTRSOCSEC + NUTR.NUTRMEDICARE + NUTR.NUTRSTATEOTHER,
                          0) AS CBS_FringeBenefits, 
                         ISNULL(FSD.FSDSubtotalWages + FSD.FSDTotalBenefits + FSD.FSDFICA + FSD.FSDFUTA + FSD.FSDPASUI + FSD.FSDWCOMP + FSD.FSDRelPayroll + FSD.FSD401K + FSD.FSDSOCSEC + FSD.FSDMEDICARE + FSD.FSDRETIRE
                          + FSD.FSDOBLIGATIONS + FSD.FSDWORKFORCE + FSD.FSDVP_GMAdj, 0) AS CBS_OnsiteMgrSalBens, ISNULL(NF.OtherDirectCosts, 0) AS OtherDirectCosts, ISNULL(NF.TrashPestsOnly, 0) 
                         AS GarbageRepairsEtc, 0 AS DirectCosts, 0 AS CBS_NonFood, ISNULL(NF.PaperCleaningOnly, 0) AS PaperCleaningOnly, 
                         ISNULL(SF.SFTotalCostFoodOnly + SF.SFTotalCostPaperOnly + SF.SFTotalCostCleaningOnly + SF.SFTotalCostOtherOnly + SF.SFDistrictLaborCost + SF.SFNutritionLaborCost + SF.CACFPFoodCost + SF.CACFPPaperCost
                          + SF.CACFPCleaningCost + CASE WHEN RTRIM(HDR.BudgetYear) = '2015-2016PA' THEN SF.SFNutritionAdminCost ELSE 0 END, 0) AS CBS_FoodLaborSuppl, CASE WHEN RTRIM(HDR.BudgetYear) 
                         = '2015-2016PA' OR
                         HDR.BudgetType = 2 THEN ROUND(CBSA.FeesCurrAdminCost * CBSA.FeesCurrAdminCostsPctInc / 10000000 + CBSA.FeesCurrAdminCost, 2) 
                         ELSE ROUND(ROUND((CBSA.FeesCurrAdminCost * CBSA.FeesCurrAdminCostsPctInc * 0.0000001) + CBSA.FeesCurrAdminCost, 4) * ROUND(ME.Total1, 0), 2) END AS CBS_FSMCAdmin, 
                         CASE WHEN RTRIM(HDR.BudgetYear) = '2015-2016PA' OR
                         HDR.BudgetType = 2 THEN CBSA.FeesCurrAdminCost ELSE ROUND(CBSA.FeesCurrAdminCost * ROUND(ME.Total1, 0), 2) END AS CBS_FSMCAdminCurrent, CASE WHEN RTRIM(HDR.BudgetYear) 
                         = '2015-2016PA' OR
                         HDR.BudgetType = 2 THEN ROUND((CASE WHEN ME.Total1 = 0 THEN 0 ELSE (CBSA.FeesCurrAdminCost * CBSA.FeesCurrAdminCostsPctInc / 10000000 + CBSA.FeesCurrAdminCost) / ROUND(ME.Total1, 0) END), 
                         4) ELSE ROUND((FeesCurrAdminCost * FeesCurrAdminCostsPctInc * 0.0000001) + FeesCurrAdminCost, 4) END AS CBS_FSMCAdminCC, CASE WHEN RTRIM(HDR.BudgetYear) = '2015-2016PA' OR
                         HDR.BudgetType = 2 THEN ROUND(CBSA.FeesCurrentMgmt * CBSA.FeesCurrMgmtPercentInc / 10000000 + CBSA.FeesCurrentMgmt, 2) 
                         ELSE ROUND(ROUND((CBSA.FeesCurrentMgmt * CBSA.FeesCurrMgmtPercentInc / 10000000 + CBSA.FeesCurrentMgmt), 4) * ROUND(ME.Total1, 0), 2) END AS CBS_FSMCMgmt, 
                         CASE WHEN RTRIM(HDR.BudgetYear) = '2015-2016PA' OR
                         HDR.BudgetType = 2 THEN CBSA.FeesCurrentMgmt ELSE ROUND(CBSA.FeesCurrentMgmt * ROUND(ME.Total1, 0), 2) END AS CBS_FSMCMgmtCurrent, CASE WHEN RTRIM(HDR.BudgetYear) = '2015-2016PA' OR
                         HDR.BudgetType = 2 THEN ROUND((CASE WHEN ME.Total1 = 0 THEN 0 ELSE (CBSA.FeesCurrentMgmt * CBSA.FeesCurrMgmtPercentInc / 10000000 + CBSA.FeesCurrentMgmt) / ROUND(ME.Total1, 0) END), 4) 
                         ELSE ROUND((FeesCurrentMgmt * FeesCurrMgmtPercentInc * 0.0000001) + FeesCurrentMgmt, 4) END AS CBS_FSMCMgmtCC, NF.FSMCOtherCosts, ISNULL(SFA.SFAWages, 0) AS SFAWages, 
                         ISNULL(SFA.SFEBensIns + SFA.SFAFICA + SFA.SFAPSERS + SFA.SFAUnemployment + SFA.SFAWComp + SFA.SFAOther1 + SFA.SFAOther2 + SFA.SFAMedicare, 0) AS CBS_SFABensRelCosts, 
                         NF.DistrictTotal AS DistrictCosts, ISNULL(- FC.RebatesDiscounts, 0) AS CBS_LessRebatesDiscs, ROUND(- (CBSA.LessCommodityUseAmt * (FR.HRL_MealCount + FR.LRL_MealCount)), 2) 
                         AS CBS_LessCommodityCredit, SP.PDE3086TOTAL, CBSA.GuaranteeToSFA, CBSA.ExternalCateringExpense
FROM            dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                         dbo.vwBDGT_FOODCOST AS FC ON HDR.BidNumber = FC.BidNumber AND HDR.BudgetYear = FC.BudgetYear AND HDR.BudgetVersion = FC.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_CBS_Additional AS CBSA ON HDR.BidNumber = CBSA.BidNumber AND HDR.BudgetYear = CBSA.BudgetYear AND HDR.BudgetVersion = CBSA.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_LaborNUTRTotals AS NUTR ON HDR.BidNumber = NUTR.BidNumber AND HDR.BudgetYear = NUTR.BudgetYear AND HDR.BudgetVersion = NUTR.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_LaborFSDTotals AS FSD ON HDR.BidNumber = FSD.BidNumber AND HDR.BudgetYear = FSD.BudgetYear AND HDR.BudgetVersion = FSD.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_NonFoodCostsTotals1516 AS NF ON HDR.BidNumber = NF.BidNumber AND HDR.BudgetYear = NF.BudgetYear AND HDR.BudgetVersion = NF.BudgetVersion LEFT OUTER JOIN
                         dbo.vw_BDGT_SpecialFunctionsCostTotals AS SF ON HDR.BidNumber = SF.BidNumber AND HDR.BudgetYear = SF.BudgetYear AND HDR.BudgetVersion = SF.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_LaborSFATotals AS SFA ON HDR.BidNumber = SFA.BidNumber AND HDR.BudgetYear = SFA.BudgetYear AND HDR.BudgetVersion = SFA.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_MealEquivalents AS ME ON HDR.BidNumber = ME.BidNumber AND HDR.BudgetYear = ME.BudgetYear AND HDR.BudgetVersion = ME.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_FederalReimbursements AS FR ON HDR.BidNumber = FR.BidNumber AND HDR.BudgetYear = FR.BudgetYear AND HDR.BudgetVersion = FR.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_SponsorToSponsorTotals AS SP ON HDR.BidNumber = SP.BidNumber AND HDR.BudgetYear = SP.BudgetYear AND HDR.BudgetVersion = SP.BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[33] 4[17] 2[24] 3) )"
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
               Right = 205
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "FC"
            Begin Extent = 
               Top = 6
               Left = 243
               Bottom = 114
               Right = 431
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CBSA"
            Begin Extent = 
               Top = 6
               Left = 469
               Bottom = 114
               Right = 679
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "NUTR"
            Begin Extent = 
               Top = 6
               Left = 717
               Bottom = 114
               Right = 893
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "FSD"
            Begin Extent = 
               Top = 114
               Left = 38
               Bottom = 222
               Right = 211
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "NF"
            Begin Extent = 
               Top = 114
               Left = 249
               Bottom = 222
               Right = 420
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SF"
            Begin Extent = 
               Top = 114
               Left = 474
               Bottom = 222
               Right = 675
            End
            DisplayFlags = 280
            TopColumn = 0
    ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_SummaryTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     End
         Begin Table = "SFA"
            Begin Extent = 
               Top = 114
               Left = 713
               Bottom = 222
               Right = 883
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ME"
            Begin Extent = 
               Top = 222
               Left = 38
               Bottom = 330
               Right = 236
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "FR"
            Begin Extent = 
               Top = 222
               Left = 274
               Bottom = 330
               Right = 460
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SP"
            Begin Extent = 
               Top = 222
               Left = 498
               Bottom = 330
               Right = 653
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
      Begin ColumnWidths = 28
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
         Width = 1500
         Width = 1500
         Width = 1800
         Width = 1500
         Width = 1500
         Width = 2340
         Width = 1770
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_SummaryTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_SummaryTotals'
GO

