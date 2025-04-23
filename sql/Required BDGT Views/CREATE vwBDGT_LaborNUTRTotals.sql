USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_LaborNUTRTotals]    Script Date: 11/5/2019 11:23:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_LaborNUTRTotals]
AS
SELECT     HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, ROUND(SUM(ISNULL(LaborNUTR.AnnualWages, 0.00)), 2) AS AnnualWages, 
                      SUM(ISNULL(LaborNUTR.Fringes, 0.00)) AS Fringes, SUM(ISNULL(LaborNUTR.Retirement, 0.00)) AS Retirement, ROUND(SUM(ISNULL(LaborNUTR.FICA, 0.00)), 2) 
                      AS FICA, ROUND(SUM(ISNULL(LaborNUTR.FUTA, 0.00)), 2) AS FUTA, ROUND(SUM(ISNULL(LaborNUTR.PASUI, 0.00)), 2) AS PASUI, 
                      ROUND(SUM(ISNULL(LaborNUTR.WCOMP, 0.00)), 2) AS WCOMP, ROUND(SUM(ISNULL(LaborNUTR.NUTRPayrollRelated, 0)), 2) AS NUTRPayrollRelated, 
                      MAX(ISNULL(LaborNUTR.OtherLaborDLR, 0)) AS OtherLaborDLR, SUM(ISNULL(LaborNUTR.NUTRHoursDayTotal, 0.00)) AS NUTRHoursDayTotal, 
                      ROUND(SUM(ISNULL(LaborNUTR.NUTRSOCSEC, 0)), 2) AS NUTRSOCSEC, ROUND(SUM(ISNULL(LaborNUTR.NUTRMEDICARE, 0)), 2) AS NUTRMEDICARE, 
                      ROUND(SUM(ISNULL(LaborNUTR.NUTRSTATEOTHER, 0)), 2) AS NUTRSTATEOTHER
FROM         dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                          (SELECT     N.BidNumber, N.BudgetYear, N.BudgetVersion, ISNULL(E.HoursDay * E.DaysPaid * E.HRLYPYRT, 0) AS AnnualWages, 
                                                   ISNULL(E.Health + E.Dental + E.Disability + E.Vision + E.Annuity + E.Life + E.OtherBenefit, 0) AS Fringes, ISNULL(E.Retirement, 0) AS Retirement, 
                                                   ISNULL(E.HoursDay * E.DaysPaid * E.HRLYPYRT * N.FICAPCT * 0.0001 * 0.01, 0) AS FICA, 
                                                   ISNULL(E.HoursDay * E.DaysPaid * E.HRLYPYRT * N.SOCSECPCT * 0.0001 * 0.01, 0) AS NUTRSOCSEC, 
                                                   ISNULL(E.HoursDay * E.DaysPaid * E.HRLYPYRT * N.MEDICAREPCT * 0.0001 * 0.01, 0) AS NUTRMEDICARE, 
                                                   CASE WHEN ((HoursDay * DaysPaid * HRLYPYRT) < (N .OBLIGATIONDLR + N .WORKFORCEDLR) OR
                                                   (N .OBLIGATIONDLR + N .WORKFORCEDLR) = 0) 
                                                   THEN ISNULL(HoursDay * DaysPaid * HRLYPYRT * (N .RETIREPCT + N .OBLIGATIONSPCT + N .WORKFORCEPCT) * 0.0001 * 0.01, 0) 
                                                   ELSE ISNULL((N .OBLIGATIONDLR + N .WORKFORCEDLR) * (N .RETIREPCT + N .OBLIGATIONSPCT + N .WORKFORCEPCT) * 0.0001 * 0.01, 0) 
                                                   END AS NUTRSTATEOTHER, CASE WHEN (OverrideFUTA = 1 AND ISNULL((HoursDay * DaysPaid * HRLYPYRT), 0) < OFUTAThreshold) OR
                                                   (OverrideFUTA = 0 AND ISNULL((HoursDay * DaysPaid * HRLYPYRT), 0) < FUTADLR) 
                                                   THEN ISNULL(HoursDay * DaysPaid * HRLYPYRT * FUTAPCT * 0.0001 * 0.01, 0) ELSE ISNULL(FUTADLR * FUTAPCT * 0.0001 * 0.01, 0) END AS FUTA, 
                                                   CASE WHEN (OverridePASUI = 1 AND ISNULL((HoursDay * DaysPaid * HRLYPYRT), 0) < OPASUIThreshold) OR
                                                   (OverridePASUI = 0 AND ISNULL((HoursDay * DaysPaid * HRLYPYRT), 0) < PASUIDLR) 
                                                   THEN ISNULL(HoursDay * DaysPaid * HRLYPYRT * PASUIPCT * 0.0001 * 0.01, 0) ELSE ISNULL(PASUIDLR * PASUIPCT * 0.0001 * 0.01, 0) END AS PASUI, 
                                                   ISNULL(E.HoursDay * E.DaysPaid * E.HRLYPYRT * N.WCompPCT * 0.0001 * 0.01, 0) AS WCOMP, 
                                                   ISNULL(E.HoursDay * E.DaysPaid * E.HRLYPYRT * N.RelatedPayrollPCT * 0.0001 * 0.01, 0) AS NUTRPayrollRelated, ISNULL(N.OtherLaborDLR, 0) 
                                                   AS OtherLaborDLR, ISNULL(E.HoursDay, 0) AS NUTRHoursDayTotal
                            FROM          dbo.BDGT_LaborNUTR AS N LEFT OUTER JOIN
                                                   dbo.BDGT_LaborNUTREmp AS E ON N.BidNumber = E.BidNumber AND N.BudgetYear = E.BudgetYear AND N.BudgetVersion = E.BudgetVersion) 
                      AS LaborNUTR ON HDR.BidNumber = LaborNUTR.BidNumber AND HDR.BudgetYear = LaborNUTR.BudgetYear AND 
                      HDR.BudgetVersion = LaborNUTR.BudgetVersion
GROUP BY HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[25] 4[5] 2[42] 3) )"
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
               Left = 270
               Bottom = 114
               Right = 463
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "LaborNUTR"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 232
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
      Begin ColumnWidths = 17
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
         Width = 1500
      End
   End
   Begin CriteriaPane = 
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_LaborNUTRTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_LaborNUTRTotals'
GO

