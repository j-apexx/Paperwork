USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_FOODCOST]    Script Date: 11/5/2019 11:19:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_FOODCOST]
AS
SELECT     BidNumber, BudgetYear, BudgetVersion, TotalCostBreakfast, TotalCostLunch, TotalCostAlaCarte, TotalCostAdult, TotalCostHeadstart, TotalCostOther, 
                      TotalCostOtherRev, TotalCostOtherRevVending, TotalCostSnack, TotalCostMilk, 
                      TotalCostBreakfast + TotalCostLunch + TotalCostAlaCarte + TotalCostAdult + TotalCostHeadstart + TotalCostOther + TotalCostOtherRev + TotalCostOtherRevVending + TotalCostSnack
                       + TotalCostMilk AS SubtotalCostFoodOnly, 
                      TotalCostBreakfast + TotalCostLunch + TotalCostAlaCarte + TotalCostAdult + TotalCostHeadstart + TotalCostOther + TotalCostOtherRev + TotalCostOtherRevVending + TotalCostSnack
                       + TotalCostMilk - USDA + VP_GMAdj AS M33, USDA, VP_GMAdj, PctPurchases, PctMilk, TotalParticipation, ParticipationBLAC, 
                      RebateDiscountFactor * TotalParticipation AS RebatesDiscounts, InvPrepProcCPM
FROM         (SELECT     HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, P.ParticBreakfast * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) 
                                              * ISNULL(FC.BreakfastCurrCost, 0) AS TotalCostBreakfast, P.ParticLunch * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) * ISNULL(FC.LunchCurrCost, 0) 
                                              AS TotalCostLunch, (P.ParticBreakfastAlaCarte + P.ParticLunchAlaCarte) * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) * ISNULL(FC.AlaCurrCost, 0) 
                                              AS TotalCostAlaCarte, (P.ParticBreakfastAdult + P.ParticLunchAdult) * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) * ISNULL(FC.AdultAlaCurrCost, 0) 
                                              AS TotalCostAdult, P.ParticHeadStart * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) * ISNULL(FC.HeadstartCurrCost, 0) AS TotalCostHeadstart, 
                                              P.ParticOtherSponsors * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) * ISNULL(FC.OtherSpnsrCurrCost, 0) AS TotalCostOther, 
                                              P.ParticOtherRev1 * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) * ISNULL(FC.OtherRev1CurrCost, 0) AS TotalCostOtherRev, 
                                              P.ParticOtherRevVending * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) * ISNULL(FC.OtherRevVendCurrCost, 0) AS TotalCostOtherRevVending, 
                                              P.ParticOtherSnack * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) * ISNULL(FC.OtherSnackCurrCost, 0) AS TotalCostSnack, 
                                              P.ParticOtherMilk * (ISNULL(FC.PctBudgetIncrease, 0) * 0.0001 + 1) * ISNULL(FC.OtherMilkCurrCost, 0) AS TotalCostMilk, 
                                              (FED.HRL_MealCount + FED.LRL_MealCount) * A.LessCommodityUseAmt AS USDA, ISNULL(FC.VP_GMAdj, 0) AS VP_GMAdj, ISNULL(FC.PctPurchases, 0) 
                                              * 0.0001 AS PctPurchases, ISNULL(FC.PctMilk, 0) * 0.0001 AS PctMilk, P.GrandTotalPartic AS TotalParticipation, P.ParticBLAC AS ParticipationBLAC, 
                                              ISNULL(FC.RebatesDiscounts, 0) AS RebatesDiscounts, ISNULL(FC.InvPrepProcCPM, 0) AS InvPrepProcCPM, ISNULL(FC.RebateDiscountFactor, 0) 
                                              AS RebateDiscountFactor
                       FROM          dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                                              dbo.BDGT_FoodCost AS FC ON HDR.BidNumber = FC.BidNumber AND HDR.BudgetYear = FC.BudgetYear AND 
                                              HDR.BudgetVersion = FC.BudgetVersion LEFT OUTER JOIN
                                              dbo.vwBDGT_FederalReimbursements AS FED ON HDR.BidNumber = FED.BidNumber AND HDR.BudgetYear = FED.BudgetYear AND 
                                              HDR.BudgetVersion = FED.BudgetVersion LEFT OUTER JOIN
                                              dbo.BDGT_CBS_Additional AS A ON HDR.BidNumber = A.BidNumber AND HDR.BudgetYear = A.BudgetYear AND 
                                              HDR.BudgetVersion = A.BudgetVersion LEFT OUTER JOIN
                                              dbo.vwBDGT_Participation AS P ON HDR.BidNumber = P.BidNumber AND HDR.BudgetYear = P.BudgetYear AND HDR.BudgetVersion = P.BudgetVersion) 
                      AS FOODCOST
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[13] 4[17] 2[28] 3) )"
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
         Begin Table = "FOODCOST"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 249
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
      Begin ColumnWidths = 24
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 2190
         Width = 1350
         Width = 1515
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 2070
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_FOODCOST'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_FOODCOST'
GO

