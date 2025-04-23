USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_CBS_InSchoolTotals]    Script Date: 11/5/2019 11:21:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_CBS_InSchoolTotals]
AS
SELECT     HDR.BidNumber, HDR.BudgetYear, HDR.BudgetVersion, ROUND(B.ElemPaid * DOS.DaysOfSvcBreakfast, 0) * B.ElemPaidRate AS B_ElemPaid, 
                      ROUND(B.MiddleSchoolPaid * DOS.DaysOfSvcBreakfast, 0) * B.MiddleSchoolPaidRate AS B_MiddleSchoolPaid, 
                      ROUND(B.HighSchoolPaid * DOS.DaysOfSvcBreakfast, 0) * B.HighSchoolPaidRate AS B_HighSchoolPaid, ROUND(B.ElemPaidTierB * DOS.DaysOfSvcBreakfast, 0) 
                      * B.ElemPaidTierRateB AS B_ElemPaidTier, ROUND(B.MiddlePaidTierB * DOS.DaysOfSvcBreakfast, 0) * B.MiddlePaidTierRateB AS B_MiddlePaidTier, 
                      ROUND(B.HighSchoolPaidTierB * DOS.DaysOfSvcBreakfast, 0) * B.HighSchoolPaidTierRateB AS B_HighSchoolPaidTier, 
                      ROUND(B.ElemReduced * DOS.DaysOfSvcBreakfast, 0) * B.ElemReducedRate AS B_ElemReduced, ROUND(B.MiddleSchoolReduced * DOS.DaysOfSvcBreakfast, 0) 
                      * B.MiddleSchoolReducedRate AS B_MiddleSchoolReduced, ROUND(B.HighSchoolReduced * DOS.DaysOfSvcBreakfast, 0) 
                      * B.HighSchoolReducedRate AS B_HighSchoolReduced, ROUND(B.AdultPaid * DOS.DaysOfSvcBreakfast, 0) * B.AdultPaidRate AS B_Adult, 
                      ROUND(B.AlaCarte * DOS.DaysOfSvcBreakfast, 0) * B.AlaCarteRate AS B_Ala, ROUND(L.ElemPaidL * DOS.DaysOfSvcLunch, 0) * L.ElemPaidRateL AS L_ElemPaidL, 
                      ROUND(L.MiddleSchoolPaidL * DOS.DaysOfSvcLunch, 0) * L.MiddleSchoolPaidRateL AS L_MiddleSchoolPaidL, ROUND(L.HighSchoolPaidL * DOS.DaysOfSvcLunch, 0) 
                      * L.HighSchoolPaidRateL AS L_HighSchoolPaidL, ROUND(L.ElemPaidTier * DOS.DaysOfSvcLunch, 0) * L.ElemPaidTierRate AS L_ElemPaidTier, 
                      ROUND(L.MiddleSchoolPaidTier * DOS.DaysOfSvcLunch, 0) * L.MiddleSchoolPaidTierRate AS L_MiddleSchoolPaidTier, 
                      ROUND(L.HighSchoolPaidTier * DOS.DaysOfSvcLunch, 0) * L.HighSchoolPaidTierRate AS L_HighSchoolPaidTier, ROUND(L.ElemReducedL * DOS.DaysOfSvcLunch, 0) 
                      * L.ElemReducedRateL AS L_ElemReducedL, ROUND(L.MiddleSchoolReducedL * DOS.DaysOfSvcLunch, 0) 
                      * L.MiddleSchoolReducedRateL AS L_MiddleSchoolReducedL, ROUND(L.HighSchoolReducedL * DOS.DaysOfSvcLunch, 0) 
                      * L.HighSchoolReducedRateL AS L_HighSchoolReducedL, ROUND(L.AdultPaidL * DOS.DaysOfSvcLunch, 0) * L.AdultPaidRateL AS L_AdultPaidL, 
                      ROUND(L.AlaCarteL * DOS.DaysOfSvcLunch, 0) * L.AlaCarteRateL AS L_AlaCarteL, ROUND(L.AlaCarteSalesRevIncrease * DOS.DaysOfSvcLunch, 0) 
                      * L.AlaCarteSalesRevIncRate AS L_AlaCarteSalesRevIncrease, CASE WHEN HDR.BudgetYear = '2015-2016PA' OR
                      HDR.BudgetYear = '2016-2017' THEN ROUND(L.HeadStartSales * DOS.DaysOfSvcLunch, 0) * L.HeadStartSalesRate ELSE 0.00 END AS L_HeadStartSales, 
                      CASE WHEN HDR.BudgetYear = '2015-2016PA' OR
                      HDR.BudgetYear = '2015-2017' THEN ROUND(L.OtherSponsors * DOS.DaysOfSvcLunch, 0) * L.OtherSponsorsRate ELSE 0.00 END AS L_OtherSponsors, 
                      CASE WHEN HDR.BudgetYear = '2015-2016PA' OR
                      HDR.BudgetYear = '2015-2017' THEN ROUND(L.OtherRev1 * DOS.DaysOfSvcLunch, 0) * L.OtherRev1Rate ELSE 0.00 END AS L_OtherRev1, 
                      ROUND(L.OtherRevVending * DOS.DaysOfSvcLunch, 0) * L.OtherRevVendingRate AS L_OtherRevVending, CASE WHEN HDR.BudgetYear = '2015-2016PA' OR
                      HDR.BudgetYear = '2015-2017' THEN ROUND(L.VendingCommissions * DOS.DaysOfSvcLunch, 0) 
                      * L.VendingCommissionsRate ELSE 0.00 END AS L_VendingCommissions, ROUND(S.SnackPaid * DOS.DaysOfSvcSnack, 0) * S.SnackPaidRate AS S_SnackPaid, 
                      ROUND(S.SnackReduced * DOS.DaysOfSvcSnack, 0) * S.SnackReducedRate AS S_SnackReduced, ROUND(S.SnackAdult * DOS.DaysOfSvcSnack, 0) 
                      * S.SnackAdultRate AS S_SnackAdult, ROUND(S.SnackAlaCarte * DOS.DaysOfSvcSnack, 0) * S.SnackAlaCarteRate AS S_SnackAlaCarte, 
                      ROUND(O.SpecialMilk * DOS.DaysOfSvcLunch, 0) * O.SpecialMilkRate AS O_SpecialMilk, ROUND(O.VendingMachineSalesConc * DOS.DaysOfSvcLunch, 0) 
                      * O.VendingMachineSalesRate AS O_VendingMachineSalesConc, CASE WHEN HDR.BudgetYear = '2015-2016PA' OR
                      HDR.BudgetYear = '2015-2017' THEN ROUND(O.SpecialFunctions * DOS.DaysOfSvcLunch, 0) 
                      * O.SpecialFunctionsRate ELSE (L.HeadStartSales + L.OtherSponsors + L.OtherRev1 + O.Catering) 
                      * DOS.DaysOfSvcLunch * O.SpecialFunctionsRate END AS O_SpecialFunctions, ISNULL(CACFP.CACFPTotalPrice, 0) AS CACFPTotalPrice
FROM         dbo.BDGT_BudgetHDR AS HDR LEFT OUTER JOIN
                      dbo.BDGT_CBS_Breakfast AS B ON HDR.BidNumber = B.BidNumber AND HDR.BudgetVersion = B.BudgetVersion AND 
                      HDR.BudgetYear = B.BudgetYear LEFT OUTER JOIN
                      dbo.BDGT_DaysOfService AS DOS ON HDR.BidNumber = DOS.BidNumber AND HDR.BudgetVersion = DOS.BudgetVersion AND 
                      HDR.BudgetYear = DOS.BudgetYear LEFT OUTER JOIN
                      dbo.BDGT_CBS_Lunches AS L ON HDR.BidNumber = L.BidNumber AND HDR.BudgetVersion = L.BudgetVersion AND 
                      HDR.BudgetYear = L.BudgetYear LEFT OUTER JOIN
                      dbo.BDGT_CBS_SnacksSuppl AS S ON HDR.BidNumber = S.BidNumber AND HDR.BudgetVersion = S.BudgetVersion AND 
                      HDR.BudgetYear = S.BudgetYear LEFT OUTER JOIN
                      dbo.BDGT_CBS_Other AS O ON HDR.BidNumber = O.BidNumber AND HDR.BudgetVersion = O.BudgetVersion AND HDR.BudgetYear = O.BudgetYear LEFT OUTER JOIN
                      dbo.vwBDGT_CACFP AS CACFP ON HDR.BidNumber = CACFP.BidNumber AND HDR.BudgetVersion = CACFP.BudgetVersion AND HDR.BudgetYear = CACFP.BudgetYear
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[31] 4[16] 2[30] 3) )"
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
         Begin Table = "B"
            Begin Extent = 
               Top = 6
               Left = 243
               Bottom = 114
               Right = 449
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DOS"
            Begin Extent = 
               Top = 6
               Left = 487
               Bottom = 114
               Right = 666
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "L"
            Begin Extent = 
               Top = 6
               Left = 704
               Bottom = 114
               Right = 915
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "S"
            Begin Extent = 
               Top = 114
               Left = 38
               Bottom = 222
               Right = 211
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "O"
            Begin Extent = 
               Top = 114
               Left = 249
               Bottom = 222
               Right = 455
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CACFP"
            Begin Extent = 
               Top = 114
               Left = 493
               Bottom = 222
               Right = 669
            End
            DisplayFlags = 280
            TopColumn = 0
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_InSchoolTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 37
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_InSchoolTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_CBS_InSchoolTotals'
GO

