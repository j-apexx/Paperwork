USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_LaborFSDTotals]    Script Date: 11/5/2019 11:23:33 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_LaborFSDTotals]
AS
SELECT     BidNumber, BudgetYear, BudgetVersion, ROUND(SUM(FSDSalary), 2) AS FSDSalary, SUM(FSDBONUS) AS FSDBONUS, SUM(FSDHlthBuyout) AS FSDHlthBuyout, 
                      SUM(FSDCellBenefit) AS FSDCellBenefit, ROUND(SUM(FSDSubtotalWages), 2) AS FSDSubtotalWages, MAX(FSDVP_GMAdj) AS FSDVP_GMAdj, 
                      MAX(FSDOtherLaborDLR) AS FSDOtherLaborDLR, SUM(FSDTotalBenefits) AS FSDTotalBenefits, SUM(FSDTotalExpenses) AS FSDTotalExpenses, 
                      ROUND(SUM(FSDFICA), 2) AS FSDFICA, ROUND(SUM(FSDFUTA), 2) AS FSDFUTA, ROUND(SUM(FSDPASUI), 2) AS FSDPASUI, ROUND(SUM(FSDWCOMP), 2) 
                      AS FSDWCOMP, ROUND(SUM(FSDRelPayroll), 2) AS FSDRelPayroll, SUM(FSD401K) AS FSD401K, SUM(FSDHealth) AS FSDHealth, SUM(FSDLife) AS FSDLife, 
                      SUM(FSDDental) AS FSDDental, SUM(FSDVision) AS FSDVision, SUM(FSDOtherBenefit) AS FSDOtherBenefit, MAX(FSDWorkDays) AS FSDWorkDays, 
                      SUM(FSDExpTravel) AS FSDExpTravel, SUM(FSDExpDues) AS FSDExpDues, SUM(FSDExpEdu) AS FSDExpEdu, SUM(FSDExpSubs) AS FSDExpSubs, 
                      ROUND(SUM(FSDMEDICARE), 2) AS FSDMEDICARE, ROUND(SUM(FSDOBLIGATIONS), 2) AS FSDOBLIGATIONS, ROUND(SUM(FSDSOCSEC), 2) AS FSDSOCSEC, 
                      ROUND(SUM(FSDRETIRE), 2) AS FSDRETIRE, ROUND(SUM(FSDWORKFORCE), 2) AS FSDWORKFORCE, MAX(WorkWeeks) AS WorkWeeks
FROM         (SELECT     E.BidNumber, E.BudgetYear, E.BudgetVersion, (E.CurrentSalary + E.CurrentSalary * F.PctBudgetIncrease * 0.0001) / 52.00 * E.WorkWeeks AS FSDSalary, 
                                              E.CurrentBonus AS FSDBONUS, E.CurrentHlthBuyout AS FSDHlthBuyout, E.CellBenefit AS FSDCellBenefit, CalculatedFSD.FSDSubtotalWages, 
                                              F.VP_GMAdj AS FSDVP_GMAdj, F.OtherLaborDLR AS FSDOtherLaborDLR, E.Health + E.Life + E.Dental + E.Vision + E.OtherBenefit AS FSDTotalBenefits, 
                                              E.ExpnTravel + E.ExpnDues + E.ExpnEducation + E.ExpnSubscriptions AS FSDTotalExpenses, 
                                              CalculatedFSD.FSDSubtotalWages * F.SOCSECPCT * 0.0001 * 0.01 AS FSDSOCSEC, 
                                              CalculatedFSD.FSDSubtotalWages * F.MEDICAREPCT * 0.0001 * 0.01 AS FSDMEDICARE, 
                                              CalculatedFSD.FSDSubtotalWages * F.RETIREPCT * 0.0001 * 0.01 AS FSDRETIRE, 
                                              CASE WHEN CalculatedFSD.FSDSubtotalWages < [WORKFORCEDLR] THEN CalculatedFSD.FSDSubtotalWages * [WORKFORCEPCT] * 0.0001 * 0.01 ELSE [WORKFORCEDLR]
                                               * [WORKFORCEPCT] * 0.0001 * 0.01 END AS FSDWORKFORCE, 
                                              CASE WHEN CalculatedFSD.FSDSubtotalWages < [OBLIGATIONDLR] THEN CalculatedFSD.FSDSubtotalWages * [OBLIGATIONSPCT] * 0.0001 * 0.01 ELSE [OBLIGATIONDLR]
                                               * [OBLIGATIONSPCT] * 0.0001 * 0.01 END AS FSDOBLIGATIONS, CalculatedFSD.FSDSubtotalWages * F.FICAPCT * 0.0001 * 0.01 AS FSDFICA, 
                                              CASE WHEN (OverrideFUTA = 1 AND CalculatedFSD.FSDSubtotalWages < OFUTAThreshold) OR
                                              (OverrideFUTA = 0 AND CalculatedFSD.FSDSubtotalWages < FUTADLR) THEN (CalculatedFSD.FSDSubtotalWages * FUTAPCT * 0.0001 * 0.01) 
                                              ELSE (FUTADLR * FUTAPCT * 0.0001 * 0.01) END AS FSDFUTA, CASE WHEN (OverridePASUI = 1 AND 
                                              CalculatedFSD.FSDSubtotalWages < OPASUIThreshold) OR
                                              (OverridePASUI = 0 AND CalculatedFSD.FSDSubtotalWages < PASUIDLR) THEN (CalculatedFSD.FSDSubtotalWages * PASUIPCT * 0.0001 * 0.01) 
                                              ELSE (PASUIDLR * PASUIPCT * 0.0001 * 0.01) END AS FSDPASUI, CalculatedFSD.FSDSubtotalWages * F.WCompPCT * 0.0001 * 0.01 AS FSDWCOMP, 
                                              CalculatedFSD.FSDSubtotalWages * F.RelatedPayrollPCT * 0.0001 * 0.01 AS FSDRelPayroll, E.Ben401k AS FSD401K, E.Health AS FSDHealth, 
                                              E.Life AS FSDLife, E.Dental AS FSDDental, E.Vision AS FSDVision, E.OtherBenefit AS FSDOtherBenefit, E.WorkWeeks * 5 AS FSDWorkDays, 
                                              E.ExpnTravel AS FSDExpTravel, E.ExpnDues AS FSDExpDues, E.ExpnEducation AS FSDExpEdu, E.ExpnSubscriptions AS FSDExpSubs, 
                                              E.WorkWeeks
                       FROM          dbo.BDGT_LaborFSDEmps AS E LEFT OUTER JOIN
                                              dbo.BDGT_LaborFSD AS F ON E.BidNumber = F.BidNumber AND E.BudgetYear = F.BudgetYear AND E.BudgetVersion = F.BudgetVersion LEFT OUTER JOIN
                                                  (SELECT     dbo.BDGT_LaborFSDEmps.BidNumber, dbo.BDGT_LaborFSDEmps.BudgetYear, dbo.BDGT_LaborFSDEmps.BudgetVersion, 
                                                                           dbo.BDGT_LaborFSDEmps.ORD, 
                                                                           (dbo.BDGT_LaborFSDEmps.CurrentSalary + dbo.BDGT_LaborFSDEmps.CurrentSalary * F.PctBudgetIncrease * 0.0001) 
                                                                           / 52 * dbo.BDGT_LaborFSDEmps.WorkWeeks + dbo.BDGT_LaborFSDEmps.CurrentBonus + dbo.BDGT_LaborFSDEmps.CurrentHlthBuyout + dbo.BDGT_LaborFSDEmps.CellBenefit
                                                                            AS FSDSubtotalWages
                                                    FROM          dbo.BDGT_LaborFSDEmps LEFT OUTER JOIN
                                                                           dbo.BDGT_LaborFSD AS F ON dbo.BDGT_LaborFSDEmps.BidNumber = F.BidNumber AND 
                                                                           dbo.BDGT_LaborFSDEmps.BudgetYear = F.BudgetYear AND dbo.BDGT_LaborFSDEmps.BudgetVersion = F.BudgetVersion) 
                                              AS CalculatedFSD ON E.BidNumber = CalculatedFSD.BidNumber AND E.BudgetYear = CalculatedFSD.BudgetYear AND 
                                              E.BudgetVersion = CalculatedFSD.BudgetVersion AND E.ORD = CalculatedFSD.ORD) AS FSD
GROUP BY BidNumber, BudgetYear, BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[32] 4[30] 2[21] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[19] 2[56] 3) )"
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
         Configuration = "(H (1[32] 2) )"
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
      ActivePaneConfig = 2
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "FSD"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 227
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
      Begin ColumnWidths = 33
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1590
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
      PaneHidden = 
      Begin ColumnWidths = 12
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_LaborFSDTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_LaborFSDTotals'
GO

