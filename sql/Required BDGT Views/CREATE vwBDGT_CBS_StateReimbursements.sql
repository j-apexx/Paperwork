USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_CBS_StateReimbursements]    Script Date: 11/5/2019 10:59:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_CBS_StateReimbursements]
AS
SELECT     HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, (ROUND(B.ElemPaid * DOS.DaysOfSvcBreakfast, 0) 
                      + ROUND(B.MiddleSchoolPaid * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.HighSchoolPaid * DOS.DaysOfSvcBreakfast, 0) 
                      + ROUND(B.ElemPaidTierB * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.MiddlePaidTierB * DOS.DaysOfSvcBreakfast, 0) 
                      + ROUND(B.HighSchoolPaidTierB * DOS.DaysOfSvcBreakfast, 0)) * ST.StateBreakfast AS B_StatePaid, ROUND(F.BreakfastFree * DOS.DaysOfSvcBreakfast, 0) 
                      * ST.StateBreakfast AS B_StateFree, ROUND(F.BreakfastFreeSevere * DOS.DaysOfSvcBreakfast, 0) * ST.StateBreakfast AS B_StateFreeSevere, 
                      (ROUND(B.ElemReduced * DOS.DaysOfSvcBreakfast, 0) + ROUND(B.MiddleSchoolReduced * DOS.DaysOfSvcBreakfast, 0) 
                      + ROUND(B.HighSchoolReduced * DOS.DaysOfSvcBreakfast, 0) - ROUND(F.ReducedSevere * DOS.DaysOfSvcBreakfast, 0)) * ST.StateBreakfast AS B_StateReduced, 
                      ROUND(F.ReducedSevere * DOS.DaysOfSvcBreakfast, 0) * ST.StateBreakfast AS B_StateReducedSevere, (ROUND(L.ElemPaidL * DOS.DaysOfSvcLunch, 0) 
                      + ROUND(L.MiddleSchoolPaidL * DOS.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolPaidL * DOS.DaysOfSvcLunch, 0) + ROUND(L.ElemPaidTier * DOS.DaysOfSvcLunch,
                       0) + ROUND(L.MiddleSchoolPaidTier * DOS.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolPaidTier * DOS.DaysOfSvcLunch, 0)) * ST.StateLunch AS L_StatePaid, 
                      (ROUND(F.HRLunchFree * DOS.DaysOfSvcLunch, 0) + ROUND(F.LRLunchFree * DOS.DaysOfSvcLunch, 0)) * ST.StateLunch AS L_StateFree, 
                      (ROUND(F.HRLunchReduced * DOS.DaysOfSvcLunch, 0) + ROUND(L.ElemReducedL * DOS.DaysOfSvcLunch, 0) 
                      + ROUND(L.MiddleSchoolReducedL * DOS.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolReducedL * DOS.DaysOfSvcLunch, 0) 
                      - ROUND(F.HRLunchReduced * DOS.DaysOfSvcLunch, 0)) * ST.StateLunch AS L_StateReduced, 
                      CASE StateReimb.OverrideBreakfast02 WHEN 1 THEN StateReimb.OValueBreakfast02 ELSE ROUND(StateReimb.BreakfastIncentive02 * DOS.DaysOfSvcLunch, 0) 
                      * ST.StateAddtlLunchLowPartic END AS L_BI02, 
                      CASE StateReimb.OverrideBreakfast04 WHEN 1 THEN StateReimb.OValueBreakfast04 ELSE (CASE DOS.DaysOfSvcBreakfast WHEN 0 THEN 0 ELSE ROUND((((L.ElemPaidL
                       + L.MiddleSchoolPaidL + L.HighSchoolPaidL + L.ElemPaidTier + L.MiddleSchoolPaidTier + L.HighSchoolPaidTier) + (F.HRLunchFree + F.LRLunchFree)) 
                      + (F.HRLunchReduced + L.ElemReducedL + L.MiddleSchoolReducedL + L.HighSchoolReducedL - F.HRLunchReduced) - StateReimb.BreakfastIncentive02) 
                      * DOS.DaysOfSvcLunch, 0) * ST.StateAddtlLunchHighParti END) END AS L_BI04, StateReimb.StateLumpSum
FROM         dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                      dbo.BDGT_FederalReimb_SETP AS Fed ON HDR.BudgetYear = Fed.BudgetYear LEFT OUTER JOIN
                      dbo.BDGT_StateReimb_SETP AS ST ON HDR.BudgetYear = ST.BudgetYear AND HDR.STATECD = ST.STATECD LEFT OUTER JOIN
                      dbo.BDGT_DaysOfService AS DOS ON HDR.BidNumber = DOS.BidNumber AND HDR.BudgetYear = DOS.BudgetYear AND 
                      HDR.BudgetVersion = DOS.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_CBS_Breakfast AS B ON HDR.BidNumber = B.BidNumber AND HDR.BudgetYear = B.BudgetYear AND 
                      HDR.BudgetVersion = B.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_CBS_FedReimbCounts AS F ON HDR.BidNumber = F.BidNumber AND HDR.BudgetYear = F.BudgetYear AND 
                      HDR.BudgetVersion = F.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_CBS_Lunches AS L ON HDR.BidNumber = L.BidNumber AND HDR.BudgetYear = L.BudgetYear AND 
                      HDR.BudgetVersion = L.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_CBS_SnacksSuppl AS S ON HDR.BidNumber = S.BidNumber AND HDR.BudgetYear = S.BudgetYear AND 
                      HDR.BudgetVersion = S.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_CBS_Other AS O ON HDR.BidNumber = O.BidNumber AND HDR.BudgetYear = O.BudgetYear AND HDR.BudgetVersion = O.BudgetVersion LEFT OUTER JOIN
                      dbo.BDGT_CBS_StateReimb AS StateReimb ON HDR.BidNumber = StateReimb.BidNumber AND HDR.BudgetYear = StateReimb.BudgetYear AND 
                      HDR.BudgetVersion = StateReimb.BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[13] 2[28] 3) )"
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
               Top = 6
               Left = 1397
               Bottom = 114
               Right = 1579
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "L"
            Begin Extent = 
               Top = 114
               Left = 38
               Bottom = 222
               Right = 249
            End
            DisplayFlags = 280
            TopColumn = 0
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_StateReimbursements'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'End
         Begin Table = "S"
            Begin Extent = 
               Top = 114
               Left = 287
               Bottom = 222
               Right = 460
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "O"
            Begin Extent = 
               Top = 114
               Left = 498
               Bottom = 222
               Right = 704
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "StateReimb"
            Begin Extent = 
               Top = 114
               Left = 742
               Bottom = 222
               Right = 925
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
      Begin ColumnWidths = 15
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1905
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_StateReimbursements'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_StateReimbursements'
GO

