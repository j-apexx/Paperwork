USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_LaborSFATotals]    Script Date: 11/5/2019 11:18:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_LaborSFATotals]
AS
SELECT     BidNumber, BudgetYear, BudgetVersion, SUM(ISNULL(SFAWages, 0.00)) AS SFAWages, SUM(ISNULL(SFAWagesWOLong, 0.00)) AS SFAWagesWOLong, 
                      SUM(ISNULL(SFEBensIns, 0.00)) AS SFEBensIns, SUM(ISNULL(SFAFICA, 0.00)) AS SFAFICA, SUM(ISNULL(SFAPSERS, 0.00)) AS SFAPSERS, 
                      SUM(ISNULL(SFAUnemployment, 0.00)) AS SFAUnemployment, SUM(ISNULL(SFAWComp, 0.00)) AS SFAWComp, SUM(ISNULL(SFAMedicare, 0.00)) AS SFAMedicare, 
                      SUM(ISNULL(SFAHoursDayTotal, 0.00)) AS SFAHoursDayTotal, MAX(OtherLaborDLR) AS SFAOther1, MAX(OtherLaborDLR2) AS SFAOther2
FROM         (SELECT     S.BidNumber, S.BudgetYear, S.BudgetVersion, E.HRLYPYRT * E.HoursDay * E.DaysPaid + E.Longevity AS SFAWages, 
                                              E.HRLYPYRT * E.HoursDay * E.DaysPaid AS SFAWagesWOLong, E.Health + E.Dental + E.Vision + E.OtherBenefit + E.Life + E.Disability AS SFEBensIns, 
                                              ROUND((E.HRLYPYRT * E.HoursDay * E.DaysPaid + E.Longevity) * S.FICAPCT * 0.0001 * 0.01, 2) AS SFAFICA, 
                                              ROUND((E.HRLYPYRT * E.HoursDay * E.DaysPaid + E.Longevity) * S.PSERSPCT * 0.0001 * 0.01, 2) AS SFAPSERS, 
                                              ROUND((E.HRLYPYRT * E.HoursDay * E.DaysPaid + E.Longevity) * S.UNEMPLOYPCT * 0.0001 * 0.01, 2) AS SFAUnemployment, 
                                              ROUND((E.HRLYPYRT * E.HoursDay * E.DaysPaid + E.Longevity) * S.WCompPCT * 0.0001 * 0.01, 2) AS SFAWComp, E.HoursDay AS SFAHoursDayTotal, 
                                              S.OtherLaborDLR, S.OtherLaborDLR2, ROUND((E.HRLYPYRT * E.HoursDay * E.DaysPaid + E.Longevity) * S.MEDICAREPCT * 0.0001 * 0.01, 2) 
                                              AS SFAMedicare
                       FROM          dbo.BDGT_LaborSFA AS S LEFT OUTER JOIN
                                              dbo.BDGT_LaborSFAEmp AS E ON S.BidNumber = E.BidNumber AND S.BudgetYear = E.BudgetYear AND S.BudgetVersion = E.BudgetVersion) 
                      AS SFA
GROUP BY BidNumber, BudgetYear, BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[26] 4[29] 2[25] 3) )"
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
         Begin Table = "SFA"
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_LaborSFATotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_LaborSFATotals'
GO

