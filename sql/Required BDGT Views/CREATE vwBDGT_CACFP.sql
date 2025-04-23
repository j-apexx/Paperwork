USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_CACFP]    Script Date: 11/5/2019 11:23:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_CACFP]
AS
SELECT     HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, ISNULL((((C.ServingsPerDay_1 * CAST(C.ServingDays_1 AS INT) 
                      * C.CACFPPrice_1 + C.ServingsPerDay_2 * CAST(C.ServingDays_2 AS INT) * C.CACFPPrice_2) + C.ServingsPerDay_3 * CAST(C.ServingDays_3 AS INT) 
                      * C.CACFPPrice_3) + C.ServingsPerDay_4 * CAST(C.ServingDays_4 AS INT) * C.CACFPPrice_4) + C.ServingsPerDay_5 * CAST(C.ServingDays_5 AS INT) 
                      * C.CACFPPrice_5, 0) AS CACFPTotalPrice, 
					  ISNULL(
						  (SELECT MAX(v) FROM (VALUES
						  (CASE MealType_1 WHEN 1 THEN C.CACFPPrice_1 ELSE 0 END), 
						  (CASE MealType_2 WHEN 1 THEN C.CACFPPrice_2 ELSE 0 END), 
						  (CASE MealType_3 WHEN 1 THEN C.CACFPPrice_3 ELSE 0 END),
						  (CASE MealType_4 WHEN 1 THEN C.CACFPPrice_4 ELSE 0 END), 
						  (CASE MealType_5 WHEN 1 THEN C.CACFPPrice_5 ELSE 0 END)) AS value(v))
					  , 0) AS CACFPBreakfast, 
					  ISNULL(
						  (SELECT MAX(v) FROM (VALUES
						  (CASE MealType_1 WHEN 2 THEN C.CACFPPrice_1 ELSE 0 END), 
						  (CASE MealType_2 WHEN 2 THEN C.CACFPPrice_2 ELSE 0 END), 
						  (CASE MealType_3 WHEN 2 THEN C.CACFPPrice_3 ELSE 0 END),
						  (CASE MealType_4 WHEN 2 THEN C.CACFPPrice_4 ELSE 0 END), 
						  (CASE MealType_5 WHEN 2 THEN C.CACFPPrice_5 ELSE 0 END)) AS value(v))
					  , 0) AS CACFPLunch, 
					  ISNULL(
						  (SELECT MAX(v) FROM (VALUES
						  (CASE MealType_1 WHEN 3 THEN C.CACFPPrice_1 ELSE 0 END), 
						  (CASE MealType_2 WHEN 3 THEN C.CACFPPrice_2 ELSE 0 END), 
						  (CASE MealType_3 WHEN 3 THEN C.CACFPPrice_3 ELSE 0 END),
						  (CASE MealType_4 WHEN 3 THEN C.CACFPPrice_4 ELSE 0 END), 
						  (CASE MealType_5 WHEN 3 THEN C.CACFPPrice_5 ELSE 0 END)) AS value(v))
					  , 0) AS CACFPSupper, 
					  ISNULL(
						  (SELECT MAX(v) FROM (VALUES
						  (CASE MealType_1 WHEN 4 THEN C.CACFPPrice_1 ELSE 0 END), 
						  (CASE MealType_2 WHEN 4 THEN C.CACFPPrice_2 ELSE 0 END), 
						  (CASE MealType_3 WHEN 4 THEN C.CACFPPrice_3 ELSE 0 END),
						  (CASE MealType_4 WHEN 4 THEN C.CACFPPrice_4 ELSE 0 END), 
						  (CASE MealType_5 WHEN 4 THEN C.CACFPPrice_5 ELSE 0 END)) AS value(v))
					  , 0) AS CACFPPMSnack, 
					  ISNULL(
						  (SELECT MAX(v) FROM (VALUES
						  (CASE MealType_1 WHEN 5 THEN C.CACFPPrice_1 ELSE 0 END), 
						  (CASE MealType_2 WHEN 5 THEN C.CACFPPrice_2 ELSE 0 END), 
						  (CASE MealType_3 WHEN 5 THEN C.CACFPPrice_3 ELSE 0 END),
						  (CASE MealType_4 WHEN 5 THEN C.CACFPPrice_4 ELSE 0 END), 
						  (CASE MealType_5 WHEN 5 THEN C.CACFPPrice_5 ELSE 0 END)) AS value(v))
					  , 0) AS CACFPAMSnack, 
                      ISNULL(CASE CACFPPrice_1 WHEN 0 THEN 0 ELSE ROUND(CACFPAvgFoodCost / CACFPPrice_1, 3) END, 0) AS CACFPFoodCost, 
                      ISNULL(CASE CACFPPrice_1 WHEN 0 THEN 0 ELSE ROUND(CACFPAvgPaperCost / CACFPPrice_1, 3) END, 0) AS CACFPPaperCost, 
                      ISNULL(CASE CACFPPrice_1 WHEN 0 THEN 0 ELSE ROUND(CACFPAvgCleaningCost / CACFPPrice_1, 3) END, 0) AS CACFPCleaningCost, 
                      ISNULL(ROUND(C.CACFPPrice_1 * C.CACFPPctAdmin * 0.0001, 3), 0) AS CACFPAdminFeePM, ISNULL(CAST(C.ServingsPerDay_1 AS INT) 
                      * CAST(C.ServingDays_1 AS INT), 0) AS CACFPTotalDays, ISNULL(ROUND(C.CACFPPrice_1 * C.CACFPPctAdmin * 0.0001, 3) * O.CACFP * D.DaysOfSvcSFSP, 0) 
                      AS CACFPAdminFeeTotal
FROM         dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                      dbo.BDGT_CACFP AS C ON HDR.BidNumber = C.BidNumber AND HDR.BudgetVersion = C.BudgetVersion AND HDR.BudgetYear = C.BudgetYear LEFT OUTER JOIN
                      dbo.BDGT_CBS_Other AS O ON HDR.BidNumber = O.BidNumber AND HDR.BudgetVersion = O.BudgetVersion AND HDR.BudgetYear = O.BudgetYear LEFT OUTER JOIN
                      dbo.BDGT_DaysOfService AS D ON HDR.BidNumber = D.BidNumber AND HDR.BudgetVersion = D.BudgetVersion AND HDR.BudgetYear = D.BudgetYear
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[24] 4[25] 2[19] 3) )"
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
               Left = 732
               Bottom = 114
               Right = 909
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "C"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 233
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "O"
            Begin Extent = 
               Top = 6
               Left = 271
               Bottom = 114
               Right = 477
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "D"
            Begin Extent = 
               Top = 6
               Left = 515
               Bottom = 114
               Right = 694
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
      Begin ColumnWidths = 32
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
   ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CACFP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'      Width = 1500
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CACFP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CACFP'
GO

