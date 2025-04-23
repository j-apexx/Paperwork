USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_FederalReimbursements]    Script Date: 11/5/2019 11:21:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_FederalReimbursements]
AS
SELECT        HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, (ROUND(B.ElemPaid * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.MiddleSchoolPaid * DOS.DaysOfSvcBreakfast, 0) 
                         + ROUND(B.HighSchoolPaid * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.ElemPaidTierB * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.MiddlePaidTierB * DOS.DaysOfSvcBreakfast, 0) 
                         + ROUND(B.HighSchoolPaidTierB * DOS.DaysOfSvcBreakfast, 0)) * Fed.FedPaidBreakfast AS B_FedPaid, ROUND(F.BreakfastFree * DOS.DaysOfSvcBreakfast, 0) * Fed.FedFreeBreakfast AS B_FedFree, 
                         ROUND(F.BreakfastFreeSevere * DOS.DaysOfSvcBreakfast, 0) * Fed.FedFreeSevereBfast AS B_FedFreeSevere, (ROUND(B.ElemReduced * DOS.DaysOfSvcBreakfast, 0) 
                         + ROUND(B.MiddleSchoolReduced * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.HighSchoolReduced * DOS.DaysOfSvcBreakfast, 0) - ROUND(F.ReducedSevere * DOS.DaysOfSvcBreakfast, 0)) 
                         * Fed.FedReducedBfast AS B_FedReduced, ROUND(F.ReducedSevere * DOS.DaysOfSvcBreakfast, 0) * Fed.FedReducedSevBfast AS B_FedReducedSevere, ROUND(F.HRLunchPaid * DOS.DaysOfSvcLunch, 0) 
                         * Fed.FedPaidHighLunch AS HRL_Paid, ROUND(F.HRLunchFree * DOS.DaysOfSvcLunch, 0) * Fed.FedFreeHighLunch AS HRL_Free, ROUND(F.HRLunchReduced * DOS.DaysOfSvcLunch, 0) 
                         * Fed.FedReducedHighLunch AS HRL_Reduced, (ROUND(L.ElemPaidL * DOS.DaysOfSvcLunch, 0) + ROUND(L.MiddleSchoolPaidL * DOS.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolPaidL * DOS.DaysOfSvcLunch,
                          0) + ROUND(L.ElemPaidTier * DOS.DaysOfSvcLunch, 0) + ROUND(L.MiddleSchoolPaidTier * DOS.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolPaidTier * DOS.DaysOfSvcLunch, 0) 
                         - ROUND(F.HRLunchPaid * DOS.DaysOfSvcLunch, 0)) * Fed.FedPaidLowLunch AS LRL_Paid, ROUND(F.LRLunchFree * DOS.DaysOfSvcLunch, 0) * Fed.FedFreeLowLunch AS LRL_Free, 
                         (ROUND(L.ElemReducedL * DOS.DaysOfSvcLunch, 0) + ROUND(L.MiddleSchoolReducedL * DOS.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolReducedL * DOS.DaysOfSvcLunch, 0) 
                         - ROUND(F.HRLunchReduced * DOS.DaysOfSvcLunch, 0)) * Fed.FedReducedLowLunch AS LRL_Reduced, ROUND(S.SnackPaid * DOS.DaysOfSvcSnack, 0) * Fed.FedPaidRegularSnack AS S_Paid, 
                         ROUND(F.SnackFree * DOS.DaysOfSvcSnack, 0) * Fed.FedFreeRegularSnack AS S_Free, ROUND(S.SnackReduced * DOS.DaysOfSvcSnack, 0) * Fed.FedReducedRegSnack AS S_Reduced, 
                         ROUND(O.SpecialMilk * DOS.DaysOfSvcLunch, 0) * Fed.FedPaidSpecMilk AS SM_PaidSpecialMilk, ROUND(F.SpecialMilkFree * DOS.DaysOfSvcSnack, 0) * F.SpecialMilkFreeRate AS SM_Free, 
                         CASE WHEN F.OverrideFedPBR = 1 THEN F.OValueFedPBR ELSE (((ROUND(F.HRLunchPaid * DOS.DaysOfSvcLunch, 0) + ROUND(F.HRLunchFree * DOS.DaysOfSvcLunch, 0) 
                         + ROUND(F.HRLunchReduced * DOS.DaysOfSvcLunch, 0)) + (ROUND(L.ElemPaidL * DOS.DaysOfSvcLunch, 0) + ROUND(L.MiddleSchoolPaidL * DOS.DaysOfSvcLunch, 0) 
                         + ROUND(L.HighSchoolPaidL * DOS.DaysOfSvcLunch, 0) + ROUND(L.ElemPaidTier * DOS.DaysOfSvcLunch, 0) + ROUND(L.MiddleSchoolPaidTier * DOS.DaysOfSvcLunch, 0) 
                         + ROUND(L.HighSchoolPaidTier * DOS.DaysOfSvcLunch, 0) - ROUND(F.HRLunchPaid * DOS.DaysOfSvcLunch, 0)) + ROUND(F.LRLunchFree * DOS.DaysOfSvcLunch, 0)) 
                         + (ROUND(L.ElemReducedL * DOS.DaysOfSvcLunch, 0) + ROUND(L.MiddleSchoolReducedL * DOS.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolReducedL * DOS.DaysOfSvcLunch, 0) 
                         - ROUND(F.HRLunchReduced * DOS.DaysOfSvcLunch, 0))) * ISNULL(ST.StateSNILunchRate, 0) END AS PerfBasedReimb, ROUND(F.HRLunchPaid * DOS.DaysOfSvcLunch, 0) 
                         + ROUND(F.HRLunchFree * DOS.DaysOfSvcLunch, 0) + ROUND(F.HRLunchReduced * DOS.DaysOfSvcLunch, 0) AS HRL_MealCount, (ROUND(L.ElemPaidL * DOS.DaysOfSvcLunch, 0) 
                         + ROUND(L.MiddleSchoolPaidL * DOS.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolPaidL * DOS.DaysOfSvcLunch, 0) + ROUND(L.ElemPaidTier * DOS.DaysOfSvcLunch, 0) 
                         + ROUND(L.MiddleSchoolPaidTier * DOS.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolPaidTier * DOS.DaysOfSvcLunch, 0) - ROUND(F.HRLunchPaid * DOS.DaysOfSvcLunch, 0) 
                         + ROUND(F.LRLunchFree * DOS.DaysOfSvcLunch, 0)) + (ROUND(L.ElemReducedL * DOS.DaysOfSvcLunch, 0) + ROUND(L.MiddleSchoolReducedL * DOS.DaysOfSvcLunch, 0) 
                         + ROUND(L.HighSchoolReducedL * DOS.DaysOfSvcLunch, 0) - ROUND(F.HRLunchReduced * DOS.DaysOfSvcLunch, 0)) AS LRL_MealCount, (ROUND(B.ElemPaid * DOS.DaysOfSvcBreakfast, 0) 
                         + ROUND(B.MiddleSchoolPaid * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.HighSchoolPaid * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.ElemPaidTierB * DOS.DaysOfSvcBreakfast, 0) 
                         + ROUND(B.MiddlePaidTierB * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.HighSchoolPaidTierB * DOS.DaysOfSvcBreakfast, 0) + ROUND(F.BreakfastFree * DOS.DaysOfSvcBreakfast, 0) 
                         + ROUND(F.BreakfastFreeSevere * DOS.DaysOfSvcBreakfast, 0)) + (ROUND(B.ElemReduced * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.MiddleSchoolReduced * DOS.DaysOfSvcBreakfast, 0) 
                         + ROUND(B.HighSchoolReduced * DOS.DaysOfSvcBreakfast, 0) - ROUND(F.ReducedSevere * DOS.DaysOfSvcBreakfast, 0)) + ROUND(F.ReducedSevere * DOS.DaysOfSvcBreakfast, 0) AS B_MealCount
FROM            dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                         dbo.BDGT_FederalReimb_SETP AS Fed ON HDR.BudgetYear = Fed.BudgetYear LEFT OUTER JOIN
                         dbo.BDGT_BudgetYear_SETP AS Budget ON HDR.BudgetYear = Budget.BudgetYear LEFT OUTER JOIN
                         dbo.BDGT_StateReimb_SETP AS ST ON Budget.BudgetYear = ST.BudgetYear AND HDR.STATECD = ST.STATECD LEFT OUTER JOIN
                         dbo.BDGT_DaysOfService AS DOS ON HDR.BidNumber = DOS.BidNumber AND HDR.BudgetYear = DOS.BudgetYear AND HDR.BudgetVersion = DOS.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_CBS_Breakfast AS B ON HDR.BidNumber = B.BidNumber AND HDR.BudgetYear = B.BudgetYear AND HDR.BudgetVersion = B.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_CBS_FedReimbCounts AS F ON HDR.BidNumber = F.BidNumber AND HDR.BudgetYear = F.BudgetYear AND HDR.BudgetVersion = F.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_CBS_Lunches AS L ON HDR.BidNumber = L.BidNumber AND HDR.BudgetYear = L.BudgetYear AND HDR.BudgetVersion = L.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_CBS_SnacksSuppl AS S ON HDR.BidNumber = S.BidNumber AND HDR.BudgetYear = S.BudgetYear AND HDR.BudgetVersion = S.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_CBS_Other AS O ON HDR.BidNumber = O.BidNumber AND HDR.BudgetYear = O.BudgetYear AND HDR.BudgetVersion = O.BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[17] 4[11] 2[60] 3) )"
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
         Begin Table = "Fed"
            Begin Extent = 
               Top = 6
               Left = 243
               Bottom = 114
               Right = 450
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Budget"
            Begin Extent = 
               Top = 6
               Left = 488
               Bottom = 114
               Right = 655
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ST"
            Begin Extent = 
               Top = 6
               Left = 693
               Bottom = 114
               Right = 898
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DOS"
            Begin Extent = 
               Top = 6
               Left = 936
               Bottom = 114
               Right = 1115
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "B"
            Begin Extent = 
               Top = 6
               Left = 1153
               Bottom = 114
               Right = 1359
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "F"
            Begin Extent = 
               Top = 144
               Left = 37
               Bottom = 252
               Right = 219
            End
            DisplayFlags = 280
            TopColumn = 0
      ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_FederalReimbursements'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'   End
         Begin Table = "L"
            Begin Extent = 
               Top = 159
               Left = 261
               Bottom = 267
               Right = 472
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "S"
            Begin Extent = 
               Top = 149
               Left = 509
               Bottom = 257
               Right = 682
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "O"
            Begin Extent = 
               Top = 159
               Left = 731
               Bottom = 267
               Right = 937
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_FederalReimbursements'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_FederalReimbursements'
GO

