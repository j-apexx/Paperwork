USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_NonFoodCostsTotals1516]    Script Date: 11/5/2019 11:20:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_NonFoodCostsTotals1516]
AS
SELECT     HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, 
                      NF.AccountingNUTR + NF.InvoiceFeeNUTR + NF.BackgroundCheckNUTR + NF.CarTruckRentNUTR + NF.FSDExpTravel + NF.CarTruckMaintNUTR + NF.ChinaSilvGlassNUTR
                       + NF.SoftwareUpgradeNUTR + NF.TechnologyFeeNUTR + NF.CourierNUTR + NF.DuesSubsNUTR + NF.EmployeeMealsNUTR + NF.EmpRecruitmentNUTR + NF.EmpReplacementNUTR
                       + NF.IndirectLaborNUTR + NF.BuybackInvestNUTR + NF.RentalsNUTR + NF.DepreciationTNGCurr + NF.EquipMaintNUTR + NF.EquipRepairNUTR + NF.EquipReplaceNUTR
                       + NF.FreightDelivNUTR + NF.LiabilityEquipNUTR + NF.LiabilityInsurNUTR + NF.VehicleInsuranceNUTR + NF.LicenseFeeNUTR + NF.OfficeExpenseNUTR + NF.PrintingNUTR
                       + NF.PerformanceBondNUTR + NF.POSNUTR + NF.PostageNUTR + NF.AreaEnhancementNUTR + NF.NutrisliceNUTR + NF.SmallwareNUTR + NF.StaffTrainingNUTR + NF.ContinuingEdNUTR
                       + NF.TrainingNUTR + NF.StorageFoodNUTR + NF.TaxesNUTR + NF.CommNUTR + NF.TicketsNUTR + NF.LaundryNUTR + NF.UniformsNUTR + NF.LinensNUTR + NF.VendingRentalNUTR
                       + NF.WellnessFeeNUTR + CASE WHEN RTRIM(HDR.BudgetYear) <> '2015-2016PA' THEN CateringNUTR + CACFPAdmin ELSE 0.00 END AS OtherDirectCosts, 
                      NF.PestControlNUTR + NF.TrashNUTR AS TrashPestsOnly, NF.PaperNUTR + NF.CleaningJanitorNUTR AS PaperCleaningOnly, NF.PaperNUTR AS PaperOnly, 
                      NF.CleaningJanitorNUTR AS CleaningOnly, 
                      NF.FSMCRFPNUTR_1 + NF.FSMCRFPNUTR_2 + NF.FSMCRFPNUTR_3 + NF.FSMCRFPNUTR_4 + NF.FSMCRFPNUTR_5 AS FSMCOtherCosts, 
                      NF.AccountingDIST + NF.CarTruckRentDIST + NF.CarTruckMileageDIST + NF.CarTruckMaintDIST + NF.ChinaSilvGlassDIST + NF.CleaningJanitorDIST + NF.SoftwareUpgradeDIST
                       + NF.TechnologyFeeDIST + NF.EmpRecruitmentDIST + NF.EmpReplacementDIST + NF.IndirectLaborDIST + NF.EquipMaintDIST + NF.EquipRepairDIST + NF.EquipReplaceDIST
                       + NF.OfficeExpenseDIST + NF.PrintingDIST + NF.POSDIST + NF.PostageDIST + NF.SmallwareDIST + NF.PestControlDIST + NF.TrashDIST + NF.LaundryDIST + NF.UniformsDIST
                       + NF.IndirectLaborSFADIST + NF.UtilsSFADIST + NF.SFARFPDIST_1 + NF.SFARFPDIST_2 + NF.SFARFPDIST_3 + NF.LinensDIST + NF.DepreciationClient AS DistrictTotal
FROM         dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                      dbo.vwBDGT_NonFoodCost AS NF ON HDR.BidNumber = NF.BidNumber AND HDR.BudgetYear = NF.BudgetYear AND HDR.BudgetVersion = NF.BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[20] 4[24] 2[32] 3) )"
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
               Left = 271
               Bottom = 114
               Right = 448
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "NF"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 233
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
      Begin ColumnWidths = 11
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
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1890
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_NonFoodCostsTotals1516'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_NonFoodCostsTotals1516'
GO

