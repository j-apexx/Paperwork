USE [NUTR1]
GO

/****** Object:  View [dbo].[vwBDGT_PurchaseCategoryPermissions]    Script Date: 11/8/2019 4:25:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_PurchaseCategoryPermissions]
AS
SELECT        HDR.BidNumber, HDR.BudgetYear, CASE WHEN ([AccountingNUTR] + [InvoiceFeeNUTR] + [ElectronicInvFeeNUTR]) <> 0 THEN 1 ELSE 0 END AS ACCT, CASE WHEN ([BackgroundCheckNUTR]) 
                         <> 0 THEN 1 ELSE 0 END AS BGCHK, CASE WHEN ([CarTruckRentNUTR] + [FSDExpTravel] + [CarTruckMaintNUTR]) <> 0 THEN 1 ELSE 0 END AS CARTK, CASE WHEN ([CateringNUTR] + [CACFPAdmin]) 
                         <> 0 THEN 1 ELSE 0 END AS CATER, CASE WHEN ([ChinaSilvGlassNUTR]) <> 0 THEN 1 ELSE 0 END AS CHINA, CASE WHEN ([SoftwareUpgradeNUTR] + [TechnologyFeeNUTR]) 
                         <> 0 THEN 1 ELSE 0 END AS COMTK, CASE WHEN [CourierNUTR] <> 0 THEN 1 ELSE 0 END AS COUR, CASE WHEN [DuesSubsNUTR] <> 0 THEN 1 ELSE 0 END AS DUES, 
                         CASE WHEN ([DepreciationTNGCurr] + [BuybackInvestNUTR] + [RentalsNUTR]) <> 0 THEN 1 ELSE 0 END AS EDEPR, CASE WHEN ([EquipMaintNUTR]) <> 0 THEN 1 ELSE 0 END AS EMAIN, 
                         CASE WHEN ([EmpRecruitmentNUTR] + [EmpReplacementNUTR] + [IndirectLaborNUTR]) <> 0 THEN 1 ELSE 0 END AS EMPRC, CASE WHEN ([EquipReplaceNUTR]) <> 0 THEN 1 ELSE 0 END AS EREPL, 
                         CASE WHEN ([EquipRepairNUTR]) <> 0 THEN 1 ELSE 0 END AS EREPR, CASE WHEN ([FreightDelivNUTR]) <> 0 THEN 1 ELSE 0 END AS FRGHT, 
                         CASE WHEN ([FSMCRFPNUTR_1] + [FSMCRFPNUTR_2] + [FSMCRFPNUTR_3] + [FSMCRFPNUTR_4] + [FSMCRFPNUTR_5]) <> 0 THEN 1 ELSE 0 END AS FSMCQ, 
                         CASE WHEN ([LiabilityEquipNUTR] + [LiabilityInsurNUTR]) <> 0 THEN 1 ELSE 0 END AS INSLI, CASE WHEN ([LicenseFeeNUTR]) <> 0 THEN 1 ELSE 0 END AS LICPR, CASE WHEN ([EmployeeMealsNUTR]) 
                         <> 0 THEN 1 ELSE 0 END AS MEALS, CASE WHEN ([OfficeExpenseNUTR] + [PrintingNUTR]) <> 0 THEN 1 ELSE 0 END AS OSUPP, CASE WHEN ([PerformanceBondNUTR]) <> 0 THEN 1 ELSE 0 END AS PERFB, 
                         CASE WHEN ([POSNUTR]) <> 0 THEN 1 ELSE 0 END AS POSSS, CASE WHEN ([PostageNUTR]) <> 0 THEN 1 ELSE 0 END AS POSTG, CASE WHEN ([AreaEnhancementNUTR] + [NutrisliceNUTR]) 
                         <> 0 THEN 1 ELSE 0 END AS PROMO, CASE WHEN ([SFARFPDIST_1] + [SFARFPDIST_2] + [SFARFPDIST_3]) <> 0 THEN 1 ELSE 0 END AS SFAQ, CASE WHEN ([SmallwareNUTR]) 
                         <> 0 THEN 1 ELSE 0 END AS SMALL, CASE WHEN ([StorageFoodNUTR]) <> 0 THEN 1 ELSE 0 END AS STORE, CASE WHEN ([TaxesNUTR]) <> 0 THEN 1 ELSE 0 END AS TAXES, CASE WHEN ([CommNUTR]) 
                         <> 0 THEN 1 ELSE 0 END AS TEL, CASE WHEN ([TicketsNUTR]) <> 0 THEN 1 ELSE 0 END AS TIXTK, CASE WHEN ([PestControlNUTR] + [TrashNUTR]) <> 0 THEN 1 ELSE 0 END AS TRASH, 
                         CASE WHEN ([StaffTrainingNUTR] + [ContinuingEdNUTR] + [TrainingNUTR]) <> 0 THEN 1 ELSE 0 END AS TRNG, CASE WHEN ([LinensNUTR] + [LaundryNUTR] + [UniformsNUTR]) <> 0 THEN 1 ELSE 0 END AS UNI, 
                         1 AS USDAD, 1 AS USDAP, CASE WHEN ([VehicleInsuranceNUTR]) <> 0 THEN 1 ELSE 0 END AS VINS, CASE WHEN ([VendingRentalNUTR]) <> 0 THEN 1 ELSE 0 END AS VRENT, CASE WHEN ([WellnessFeeNUTR]) 
                         <> 0 THEN 1 ELSE 0 END AS WELNS
FROM            dbo.vwBDGT_NonFoodCost AS NF INNER JOIN
                         dbo.BDGT_BudgetHDR AS HDR ON HDR.BidNumber = NF.BidNumber AND HDR.BudgetYear = NF.BudgetYear AND HDR.BudgetVersion = NF.BudgetVersion
WHERE        (HDR.ACTIVE = 1)
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
         Begin Table = "NF"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 255
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "HDR"
            Begin Extent = 
               Top = 6
               Left = 293
               Bottom = 136
               Right = 493
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_PurchaseCategoryPermissions'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_PurchaseCategoryPermissions'
GO

