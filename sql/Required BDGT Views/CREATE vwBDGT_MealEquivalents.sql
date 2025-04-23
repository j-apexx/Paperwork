USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_MealEquivalents]    Script Date: 11/5/2019 11:20:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_MealEquivalents]
AS
SELECT     BidNumber, BudgetYear, BudgetVersion, ME_Breakfast, ME_Lunch, ISNULL(MealEquivalentFactor, 0) AS MealEquivalentFactor, ME_BreakfastAdult, ME_LunchAdult, 
                      ME_BreakfastAlaCarte, ME_LunchAlaCarte, ME_Headstart, ME_OtherSponsors, ME_OtherRev1, ME_OtherRevvending, ME_OtherSnack, ME_OtherMilk, 
                      ME_AlaCarteSaleRevInc, ME_VendingCommissions, 
                      ME_Breakfast + ME_BreakfastAdult + ME_BreakfastAlaCarte + ME_Lunch + ME_LunchAlaCarte + ME_LunchAdult + ME_OtherRevvending + ME_AlaCarteSaleRevInc AS
                       Total1, 
                      ROUND(ME_Breakfast + ME_BreakfastAdult + ME_BreakfastAlaCarte + ME_Lunch + ME_LunchAlaCarte + ME_LunchAdult + ME_OtherRevvending + ME_AlaCarteSaleRevInc
                       + ME_Headstart + ME_OtherSponsors + ME_OtherRev1 + ME_OtherSnack + ME_OtherMilk + ME_VendingCommissions, 0) AS GrandTotal, 
                      ME_VendingMachSalesCommissions AS ME_VendSalesCommission
FROM         (SELECT     Hdr.BidNumber, Hdr.BudgetYear, Hdr.BudgetVersion, ISNULL(P.ParticBreakfast, 0) AS ME_Breakfast, ISNULL(P.ParticLunch, 0) AS ME_Lunch, 
                                              MEF.MealEquivalentFactor, CASE WHEN MealEquivalentFactor > 0 THEN P.ParticBreakfastAdult / MealEquivalentFactor ELSE 0 END AS ME_BreakfastAdult,
                                               CASE WHEN MealEquivalentFactor > 0 THEN P.ParticLunchAdult / MealEquivalentFactor ELSE 0 END AS ME_LunchAdult, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN P.ParticBreakfastAlaCarte / MealEquivalentFactor ELSE 0 END AS ME_BreakfastAlaCarte, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN P.ParticLunchAlaCarte / MealEquivalentFactor ELSE 0 END AS ME_LunchAlaCarte, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN P.ParticHeadStart / MealEquivalentFactor ELSE 0 END AS ME_Headstart, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN P.ParticOtherSponsors / MealEquivalentFactor ELSE 0 END AS ME_OtherSponsors, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN P.ParticOtherRev1 / MealEquivalentFactor ELSE 0 END AS ME_OtherRev1, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN P.ParticOtherRevVending / MealEquivalentFactor ELSE 0 END AS ME_OtherRevvending, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN P.ParticOtherSnack / MealEquivalentFactor ELSE 0 END AS ME_OtherSnack, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN P.ParticOtherMilk / MealEquivalentFactor ELSE 0 END AS ME_OtherMilk, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN IST.L_AlaCarteSalesRevIncrease / MealEquivalentFactor ELSE 0 END AS ME_AlaCarteSaleRevInc, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN IST.L_VendingCommissions / MealEquivalentFactor ELSE 0 END AS ME_VendingCommissions, 
                                              CASE WHEN MealEquivalentFactor > 0 THEN IST.O_VendingMachineSalesConc / MealEquivalentFactor ELSE 0 END AS ME_VendingMachSalesCommissions
                       FROM          dbo.BDGT_BudgetHDR AS Hdr LEFT OUTER JOIN
                                              dbo.BDGT_MealEquivalents AS ME ON Hdr.BidNumber = ME.BidNumber AND Hdr.BudgetYear = ME.BudgetYear AND 
                                              Hdr.BudgetVersion = ME.BudgetVersion LEFT OUTER JOIN
                                                  (SELECT     ME.BidNumber, ME.BudgetYear, ME.BudgetVersion, CASE WHEN RTRIM(HDR.BudgetYear) 
                                                                           = '2015-2016PA' THEN ISNULL(F.FedFreeLowLunch, 0) + ISNULL(S.StateLunch, 0) + ISNULL(A.LessCommodityUseAmt, 0) 
                                                                           + ISNULL(ME.MealEquivAdj, 0) ELSE ISNULL(ME.MealEquivFactor, 0) END AS MealEquivalentFactor
                                                    FROM          dbo.BDGT_MealEquivalents AS ME INNER JOIN
                                                                           dbo.BDGT_BudgetHDR AS Hdr ON ME.BidNumber = Hdr.BidNumber AND ME.BudgetYear = Hdr.BudgetYear AND 
                                                                           ME.BudgetVersion = Hdr.BudgetVersion LEFT OUTER JOIN
                                                                           dbo.BDGT_FederalReimb_SETP AS F ON Hdr.BudgetYear = F.BudgetYear LEFT OUTER JOIN
                                                                           dbo.BDGT_StateReimb_SETP AS S ON Hdr.BudgetYear = S.BudgetYear AND Hdr.STATECD = S.STATECD LEFT OUTER JOIN
                                                                           dbo.BDGT_CBS_Additional AS A ON ME.BidNumber = A.BidNumber AND ME.BudgetYear = A.BudgetYear AND 
                                                                           ME.BudgetVersion = A.BudgetVersion) AS MEF ON Hdr.BidNumber = MEF.BidNumber AND Hdr.BudgetYear = MEF.BudgetYear AND 
                                              Hdr.BudgetVersion = MEF.BudgetVersion LEFT OUTER JOIN
                                              dbo.vwBDGT_Participation AS P ON Hdr.BidNumber = P.BidNumber AND Hdr.BudgetYear = P.BudgetYear AND 
                                              Hdr.BudgetVersion = P.BudgetVersion LEFT OUTER JOIN
                                              dbo.BDGT_FederalReimb_SETP AS F ON Hdr.BudgetYear = F.BudgetYear LEFT OUTER JOIN
                                              dbo.BDGT_StateReimb_SETP AS S ON Hdr.BudgetYear = S.BudgetYear AND Hdr.STATECD = S.STATECD LEFT OUTER JOIN
                                              dbo.BDGT_CBS_Additional AS A ON Hdr.BidNumber = A.BidNumber AND Hdr.BudgetYear = A.BudgetYear AND 
                                              Hdr.BudgetVersion = A.BudgetVersion LEFT OUTER JOIN
                                              dbo.vwBDGT_CBS_InSchoolTotals AS IST ON Hdr.BidNumber = IST.BidNumber AND Hdr.BudgetYear = IST.BudgetYear AND 
                                              Hdr.BudgetVersion = IST.BudgetVersion) AS A_1
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[19] 4[20] 2[26] 3) )"
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
         Begin Table = "A_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 286
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
      Begin ColumnWidths = 21
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_MealEquivalents'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_MealEquivalents'
GO

