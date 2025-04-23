USE [NUTR1]
GO

/****** Object:  View [dbo].[vwBDGT_ScheduleDays]    Script Date: 1/2/2020 9:56:30 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[vwBDGT_ScheduleDays]
AS
SELECT     BidNumber, BudgetYear, SUM(SERVICEDAYS1) AS SERVICEDAYS1, SUM(SERVICEDAYS2) AS SERVICEDAYS2, SUM(SERVICEDAYS3) AS SERVICEDAYS3, 
                      SUM(SERVICEDAYS4) AS SERVICEDAYS4, SUM(SERVICEDAYS5) AS SERVICEDAYS5, SUM(SERVICEDAYS6) AS SERVICEDAYS6, SUM(SERVICEDAYS7) 
                      AS SERVICEDAYS7, SUM(SERVICEDAYS8) AS SERVICEDAYS8, SUM(SERVICEDAYS9) AS SERVICEDAYS9, SUM(SERVICEDAYS10) AS SERVICEDAYS10, 
                      SUM(SERVICEDAYS11) AS SERVICEDAYS11, SUM(SERVICEDAYS12) AS SERVICEDAYS12, SUM(FSDDAYS1) AS FSDDAYS1, SUM(FSDDAYS2) AS FSDDAYS2, 
                      SUM(FSDDAYS3) AS FSDDAYS3, SUM(FSDDAYS4) AS FSDDAYS4, SUM(FSDDAYS5) AS FSDDAYS5, SUM(FSDDAYS6) AS FSDDAYS6, SUM(FSDDAYS7) AS FSDDAYS7, 
                      SUM(FSDDAYS8) AS FSDDAYS8, SUM(FSDDAYS9) AS FSDDAYS9, SUM(FSDDAYS10) AS FSDDAYS10, SUM(FSDDAYS11) AS FSDDAYS11, SUM(FSDDAYS12) 
                      AS FSDDAYS12, SUM(ELEVENTHDAYS1) AS ELEVENTHDAYS1, SUM(ELEVENTHDAYS2) AS ELEVENTHDAYS2, SUM(ELEVENTHDAYS3) AS ELEVENTHDAYS3, 
                      SUM(ELEVENTHDAYS4) AS ELEVENTHDAYS4, SUM(ELEVENTHDAYS5) AS ELEVENTHDAYS5, SUM(ELEVENTHDAYS6) AS ELEVENTHDAYS6, SUM(ELEVENTHDAYS7) 
                      AS ELEVENTHDAYS7, SUM(ELEVENTHDAYS8) AS ELEVENTHDAYS8, SUM(ELEVENTHDAYS9) AS ELEVENTHDAYS9, SUM(ELEVENTHDAYS10) AS ELEVENTHDAYS10, 
                      SUM(ELEVENTHDAYS11) AS ELEVENTHDAYS11, SUM(ELEVENTHDAYS12) AS ELEVENTHDAYS12
FROM         (SELECT     BidNumber, BudgetYear, Schedule_Days_1 AS SERVICEDAYS1, Schedule_Days_2 AS SERVICEDAYS2, Schedule_Days_3 AS SERVICEDAYS3, 
                                              Schedule_Days_4 AS SERVICEDAYS4, Schedule_Days_5 AS SERVICEDAYS5, Schedule_Days_6 AS SERVICEDAYS6, 
                                              Schedule_Days_7 AS SERVICEDAYS7, Schedule_Days_8 AS SERVICEDAYS8, Schedule_Days_9 AS SERVICEDAYS9, 
                                              Schedule_Days_10 AS SERVICEDAYS10, Schedule_Days_11 AS SERVICEDAYS11, Schedule_Days_12 AS SERVICEDAYS12, 0 AS FSDDAYS1, 
                                              0 AS FSDDAYS2, 0 AS FSDDAYS3, 0 AS FSDDAYS4, 0 AS FSDDAYS5, 0 AS FSDDAYS6, 0 AS FSDDAYS7, 0 AS FSDDAYS8, 0 AS FSDDAYS9, 
                                              0 AS FSDDAYS10, 0 AS FSDDAYS11, 0 AS FSDDAYS12, 0 AS ELEVENTHDAYS1, 0 AS ELEVENTHDAYS2, 0 AS ELEVENTHDAYS3, 0 AS ELEVENTHDAYS4, 
                                              0 AS ELEVENTHDAYS5, 0 AS ELEVENTHDAYS6, 0 AS ELEVENTHDAYS7, 0 AS ELEVENTHDAYS8, 0 AS ELEVENTHDAYS9, 0 AS ELEVENTHDAYS10, 
                                              0 AS ELEVENTHDAYS11, 0 AS ELEVENTHDAYS12
                       FROM          dbo.BDGT_Day_Schedule
                       WHERE      (Schedule_Type = 1)
                       UNION ALL
                       SELECT     BidNumber, BudgetYear, 0 AS SERVICEDAYS1, 0 AS SERVICEDAYS2, 0 AS SERVICEDAYS3, 0 AS SERVICEDAYS4, 0 AS SERVICEDAYS5, 
                                             0 AS SERVICEDAYS6, 0 AS SERVICEDAYS7, 0 AS SERVICEDAYS8, 0 AS SERVICEDAYS9, 0 AS SERVICEDAYS10, 0 AS SERVICEDAYS11, 
                                             0 AS SERVICEDAYS12, Schedule_Days_1 AS FSDDAYS1, Schedule_Days_2 AS FSDDAYS2, Schedule_Days_3 AS FSDDAYS3, 
                                             Schedule_Days_4 AS FSDDAYS4, Schedule_Days_5 AS FSDDAYS5, Schedule_Days_6 AS FSDDAYS6, Schedule_Days_7 AS FSDDAYS7, 
                                             Schedule_Days_8 AS FSDDAYS8, Schedule_Days_9 AS FSDDAYS9, Schedule_Days_10 AS FSDDAYS10, Schedule_Days_11 AS FSDDAYS11, 
                                             Schedule_Days_12 AS FSDDAYS12, 0 AS ELEVENTHDAYS1, 0 AS ELEVENTHDAYS2, 0 AS ELEVENTHDAYS3, 0 AS ELEVENTHDAYS4, 
                                             0 AS ELEVENTHDAYS5, 0 AS ELEVENTHDAYS6, 0 AS ELEVENTHDAYS7, 0 AS ELEVENTHDAYS8, 0 AS ELEVENTHDAYS9, 0 AS ELEVENTHDAYS10, 
                                             0 AS ELEVENTHDAYS11, 0 AS ELEVENTHDAYS12
                       FROM         dbo.BDGT_Day_Schedule AS BDGT_Day_Schedule_2
                       WHERE     (Schedule_Type = 2)
                       UNION ALL
                       SELECT     BidNumber, BudgetYear, 0 AS SERVICEDAYS1, 0 AS SERVICEDAYS2, 0 AS SERVICEDAYS3, 0 AS SERVICEDAYS4, 0 AS SERVICEDAYS5, 
                                             0 AS SERVICEDAYS6, 0 AS SERVICEDAYS7, 0 AS SERVICEDAYS8, 0 AS SERVICEDAYS9, 0 AS SERVICEDAYS10, 0 AS SERVICEDAYS11, 
                                             0 AS SERVICEDAYS12, 0 AS FSDDAYS1, 0 AS FSDDAYS2, 0 AS FSDDAYS3, 0 AS FSDDAYS4, 0 AS FSDDAYS5, 0 AS FSDDAYS6, 0 AS FSDDAYS7, 
                                             0 AS FSDDAYS8, 0 AS FSDDAYS9, 0 AS FSDDAYS10, 0 AS FSDDAYS11, 0 AS FSDDAYS12, Schedule_Days_1 AS ELEVENTHDAYS1, 
                                             Schedule_Days_2 AS ELEVENTHDAYS2, Schedule_Days_3 AS ELEVENTHDAYS3, Schedule_Days_4 AS ELEVENTHDAYS4, 
                                             Schedule_Days_5 AS ELEVENTHDAYS5, Schedule_Days_6 AS ELEVENTHDAYS6, Schedule_Days_7 AS ELEVENTHDAYS7, 
                                             Schedule_Days_8 AS ELEVENTHDAYS8, Schedule_Days_9 AS ELEVENTHDAYS9, Schedule_Days_10 AS ELEVENTHDAYS10, 
                                             Schedule_Days_11 AS ELEVENTHDAYS11, Schedule_Days_12 AS ELEVENTHDAYS12
                       FROM         dbo.BDGT_Day_Schedule AS BDGT_Day_Schedule_1
                       WHERE     (Schedule_Type = 3)) AS ScheduleDays
GROUP BY BidNumber, BudgetYear

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
         Begin Table = "ScheduleDays"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 221
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_ScheduleDays'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_ScheduleDays'
GO

