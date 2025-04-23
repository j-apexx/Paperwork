USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_CBS_ReimbursementsSummary]    Script Date: 11/5/2019 10:46:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_CBS_ReimbursementsSummary]
AS
SELECT     dbo.BDGT_BudgetHDR.BidNumber, dbo.BDGT_BudgetHDR.BudgetYear, dbo.BDGT_BudgetHDR.BudgetVersion, dbo.BDGT_BudgetHDR.ACTIVE, 
                      dbo.BDGT_BudgetHDR.BudgetType, 
                      dbo.vwBDGT_CBS_InSchoolTotals.B_ElemPaid + dbo.vwBDGT_CBS_InSchoolTotals.B_MiddleSchoolPaid + dbo.vwBDGT_CBS_InSchoolTotals.B_HighSchoolPaid + dbo.vwBDGT_CBS_InSchoolTotals.B_ElemPaidTier
                       + dbo.vwBDGT_CBS_InSchoolTotals.B_MiddlePaidTier + dbo.vwBDGT_CBS_InSchoolTotals.B_HighSchoolPaidTier + dbo.vwBDGT_CBS_InSchoolTotals.B_ElemReduced
                       + dbo.vwBDGT_CBS_InSchoolTotals.B_MiddleSchoolReduced + dbo.vwBDGT_CBS_InSchoolTotals.B_HighSchoolReduced + dbo.vwBDGT_CBS_InSchoolTotals.B_Adult
                       + dbo.vwBDGT_CBS_InSchoolTotals.B_Ala + dbo.vwBDGT_CBS_InSchoolTotals.L_ElemPaidL + dbo.vwBDGT_CBS_InSchoolTotals.L_MiddleSchoolPaidL + dbo.vwBDGT_CBS_InSchoolTotals.L_HighSchoolPaidL
                       + dbo.vwBDGT_CBS_InSchoolTotals.L_ElemPaidTier + dbo.vwBDGT_CBS_InSchoolTotals.L_MiddleSchoolPaidTier + dbo.vwBDGT_CBS_InSchoolTotals.L_HighSchoolPaidTier
                       + dbo.vwBDGT_CBS_InSchoolTotals.L_ElemReducedL + dbo.vwBDGT_CBS_InSchoolTotals.L_MiddleSchoolReducedL + dbo.vwBDGT_CBS_InSchoolTotals.L_HighSchoolReducedL
                       + dbo.vwBDGT_CBS_InSchoolTotals.L_AdultPaidL + dbo.vwBDGT_CBS_InSchoolTotals.L_AlaCarteL + dbo.vwBDGT_CBS_InSchoolTotals.L_AlaCarteSalesRevIncrease
                       + dbo.vwBDGT_CBS_InSchoolTotals.L_HeadStartSales + dbo.vwBDGT_CBS_InSchoolTotals.L_OtherSponsors + dbo.vwBDGT_CBS_InSchoolTotals.L_OtherRev1 + dbo.vwBDGT_CBS_InSchoolTotals.L_OtherRevVending
                       + dbo.vwBDGT_CBS_InSchoolTotals.L_VendingCommissions + dbo.vwBDGT_CBS_InSchoolTotals.S_SnackPaid + dbo.vwBDGT_CBS_InSchoolTotals.S_SnackReduced
                       + dbo.vwBDGT_CBS_InSchoolTotals.S_SnackAdult + dbo.vwBDGT_CBS_InSchoolTotals.S_SnackAlaCarte + dbo.vwBDGT_CBS_InSchoolTotals.O_SpecialMilk + dbo.vwBDGT_CBS_InSchoolTotals.O_VendingMachineSalesConc
                       + dbo.vwBDGT_CBS_InSchoolTotals.O_SpecialFunctions + dbo.vwBDGT_CBS_InSchoolTotals.CACFPTotalPrice AS TotalInSchoolRevenue, 
                      dbo.vwBDGT_FederalReimbursements.B_FedPaid + dbo.vwBDGT_FederalReimbursements.B_FedFree + dbo.vwBDGT_FederalReimbursements.B_FedFreeSevere + dbo.vwBDGT_FederalReimbursements.B_FedReduced
                       + dbo.vwBDGT_FederalReimbursements.B_FedReducedSevere + dbo.vwBDGT_FederalReimbursements.HRL_Paid + dbo.vwBDGT_FederalReimbursements.HRL_Free
                       + dbo.vwBDGT_FederalReimbursements.HRL_Reduced + dbo.vwBDGT_FederalReimbursements.LRL_Paid + dbo.vwBDGT_FederalReimbursements.LRL_Free + dbo.vwBDGT_FederalReimbursements.LRL_Reduced
                       + dbo.vwBDGT_FederalReimbursements.S_Paid + dbo.vwBDGT_FederalReimbursements.S_Free + dbo.vwBDGT_FederalReimbursements.S_Reduced + dbo.vwBDGT_FederalReimbursements.SM_PaidSpecialMilk
                       + dbo.vwBDGT_FederalReimbursements.SM_Free + dbo.vwBDGT_FederalReimbursements.PerfBasedReimb AS TotalFederalReimbursements, 
                      dbo.vwBDGT_CBS_StateReimbursements.B_StatePaid + dbo.vwBDGT_CBS_StateReimbursements.B_StateFree + dbo.vwBDGT_CBS_StateReimbursements.B_StateFreeSevere
                       + dbo.vwBDGT_CBS_StateReimbursements.B_StateReduced + dbo.vwBDGT_CBS_StateReimbursements.B_StateReducedSevere + dbo.vwBDGT_CBS_StateReimbursements.L_StatePaid
                       + dbo.vwBDGT_CBS_StateReimbursements.L_StateFree + dbo.vwBDGT_CBS_StateReimbursements.L_StateReduced + dbo.vwBDGT_CBS_StateReimbursements.L_BI02
                       + dbo.vwBDGT_CBS_StateReimbursements.L_BI04 + dbo.vwBDGT_CBS_StateReimbursements.StateLumpSum AS TotalStateReimbursements, 
                      dbo.BDGT_CBS_SummaryInfo.PSERS_RetireAmt, dbo.BDGT_CBS_SummaryInfo.AdditionalAmount, dbo.BDGT_CBS_SummaryInfo.InterestIncAmount, 
                      dbo.BDGT_CBS_SummaryInfo.ToEqualBidRevenue, dbo.BDGT_CBS_SummaryInfo.CateringRevenue, dbo.BDGT_CBS_SummaryInfo.PDE3086Revenue, 
                      ROUND((dbo.vwBDGT_FederalReimbursements.B_FedFree + dbo.vwBDGT_FederalReimbursements.B_FedFreeSevere + dbo.vwBDGT_FederalReimbursements.B_FedReduced
                       + dbo.vwBDGT_FederalReimbursements.B_FedReducedSevere + dbo.vwBDGT_FederalReimbursements.HRL_Free + dbo.vwBDGT_FederalReimbursements.HRL_Reduced
                       + dbo.vwBDGT_FederalReimbursements.LRL_Free + dbo.vwBDGT_FederalReimbursements.LRL_Reduced + dbo.vwBDGT_FederalReimbursements.S_Free + dbo.vwBDGT_FederalReimbursements.S_Reduced
                       + dbo.vwBDGT_FederalReimbursements.SM_Free) * dbo.BDGT_CBS_SummaryInfo.ReimbursementPct * 0.0001, 2) AS TotalOtherReimbursement
FROM         dbo.BDGT_BudgetHDR LEFT OUTER JOIN
                      dbo.BDGT_CBS_SummaryInfo ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_CBS_SummaryInfo.BidNumber AND 
                      dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_CBS_SummaryInfo.BudgetYear AND 
                      dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_CBS_SummaryInfo.BudgetVersion LEFT OUTER JOIN
                      dbo.vwBDGT_CBS_InSchoolTotals ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_CBS_InSchoolTotals.BidNumber AND 
                      dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_CBS_InSchoolTotals.BudgetYear AND 
                      dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_CBS_InSchoolTotals.BudgetVersion LEFT OUTER JOIN
                      dbo.vwBDGT_FederalReimbursements ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_FederalReimbursements.BidNumber AND 
                      dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_FederalReimbursements.BudgetYear AND 
                      dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_FederalReimbursements.BudgetVersion LEFT OUTER JOIN
                      dbo.vwBDGT_CBS_StateReimbursements ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_CBS_StateReimbursements.BidNumber AND 
                      dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_CBS_StateReimbursements.BudgetYear AND 
                      dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_CBS_StateReimbursements.BudgetVersion
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[32] 4[11] 2[21] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[39] 2[37] 3) )"
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
         Configuration = "(H (1[56] 4[12] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[36] 2) )"
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
         Begin Table = "BDGT_BudgetHDR"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 205
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "BDGT_CBS_SummaryInfo"
            Begin Extent = 
               Top = 47
               Left = 552
               Bottom = 155
               Right = 727
            End
            DisplayFlags = 280
            TopColumn = 6
         End
         Begin Table = "vwBDGT_CBS_InSchoolTotals"
            Begin Extent = 
               Top = 6
               Left = 243
               Bottom = 114
               Right = 463
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "vwBDGT_FederalReimbursements"
            Begin Extent = 
               Top = 6
               Left = 501
               Bottom = 114
               Right = 687
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "vwBDGT_CBS_StateReimbursements"
            Begin Extent = 
               Top = 6
               Left = 725
               Bottom = 114
               Right = 919
            End
            DisplayFlags = 344
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 16
         Width = 284
         Width = 1815
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1815
         Width = 1740
         Width = 1500
         Width = 1500
         Width = 1500
      ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_ReimbursementsSummary'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'   Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      PaneHidden = 
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_ReimbursementsSummary'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_ReimbursementsSummary'
GO

