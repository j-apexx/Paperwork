USE TWO
GO

/****** Object:  View [dbo].[vwBDGT_Participation]    Script Date: 11/5/2019 11:24:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_Participation]
AS
SELECT     BidNumber, BudgetYear, BudgetVersion, ISNULL(ParticBreakfast, 0) AS ParticBreakfast, ISNULL(ParticLunch, 0) AS ParticLunch, ISNULL(ParticBreakfastAlaCarte, 0) 
                      AS ParticBreakfastAlaCarte, ISNULL(ParticLunchAlaCarte, 0) AS ParticLunchAlaCarte, ISNULL(ParticBreakfastAdult, 0) AS ParticBreakfastAdult, ISNULL(ParticLunchAdult,
                       0) AS ParticLunchAdult, ISNULL(ParticHeadStart, 0) AS ParticHeadStart, ISNULL(ParticOtherSponsors, 0) AS ParticOtherSponsors, ISNULL(ParticOtherRev1, 0) 
                      AS ParticOtherRev1, ISNULL(ParticOtherRevVending, 0) AS ParticOtherRevVending, ISNULL(ParticOtherSnack, 0) AS ParticOtherSnack, ISNULL(ParticOtherMilk, 0) 
                      AS ParticOtherMilk, ISNULL(ParticAlaCarteSalesRevInc, 0) AS ParticAlaCarteSalesRevInc, ISNULL(ParticVendingCommissions, 0) AS ParticVendingCommissions, 
                      ISNULL(ParticSpecialFunctions, 0) AS ParticSpecialFunctions, ISNULL(ParticOtherVendingSales, 0) AS ParticOtherVendingSales, 
                      ISNULL(ParticBreakfast + ParticLunch + ParticBreakfastAlaCarte + ParticLunchAlaCarte + ParticBreakfastAdult + ParticLunchAdult + ParticAlaCarteSalesRevInc, 
                      0.00000) AS ParticBLAC, 
                      ISNULL(ParticBreakfast + ParticLunch + ParticBreakfastAlaCarte + ParticLunchAlaCarte + ParticBreakfastAdult + ParticLunchAdult + ParticHeadStart + ParticOtherSponsors
                       + ParticOtherRev1 + ParticOtherRevVending + ParticOtherSnack + ParticOtherMilk, 0.00000) AS GrandTotalPartic, ParticPDEFood, ParticPDEPaprCln
FROM         (SELECT     Hdr.BidNumber, Hdr.BudgetYear, Hdr.BudgetVersion, ROUND(B.ElemPaid * D.DaysOfSvcBreakfast, 0) 
                                              + ROUND(B.HighSchoolPaid * D.DaysOfSvcBreakfast, 0) + ROUND(B.MiddleSchoolPaid * D.DaysOfSvcBreakfast, 0) 
                                              + ROUND(B.ElemPaidTierB * D.DaysOfSvcBreakfast, 0) + ROUND(B.HighSchoolPaidTierB * D.DaysOfSvcBreakfast, 0) 
                                              + ROUND(B.MiddlePaidTierB * D.DaysOfSvcBreakfast, 0) + ROUND(B.ElemReduced * D.DaysOfSvcBreakfast, 0) 
                                              + ROUND(B.HighSchoolReduced * D.DaysOfSvcBreakfast, 0) + ROUND(B.MiddleSchoolReduced * D.DaysOfSvcBreakfast, 0) 
                                              - ROUND(F.ReducedSevere * D.DaysOfSvcBreakfast, 0) + ROUND(F.BreakfastFree * D.DaysOfSvcBreakfast, 0) 
                                              + ROUND(F.BreakfastFreeSevere * D.DaysOfSvcBreakfast, 0) + ROUND(F.ReducedSevere * D.DaysOfSvcBreakfast, 0) AS ParticBreakfast, 
                                              ROUND(F.HRLunchPaid * D.DaysOfSvcLunch, 0) + ROUND(F.HRLunchFree * D.DaysOfSvcLunch, 0) + ROUND(F.HRLunchReduced * D.DaysOfSvcLunch, 0) 
                                              + ROUND(F.LRLunchFree * D.DaysOfSvcLunch, 0) + ROUND(L.ElemPaidL * D.DaysOfSvcLunch, 0) + ROUND(L.HighSchoolPaidL * D.DaysOfSvcLunch, 0) 
                                              + ROUND(L.MiddleSchoolPaidL * D.DaysOfSvcLunch, 0) + ROUND(L.ElemPaidTier * D.DaysOfSvcLunch, 0) 
                                              + ROUND(L.HighSchoolPaidTier * D.DaysOfSvcLunch, 0) + ROUND(L.MiddleSchoolPaidTier * D.DaysOfSvcLunch, 0) 
                                              - ROUND(F.HRLunchPaid * D.DaysOfSvcLunch, 0) + ROUND(L.ElemReducedL * D.DaysOfSvcLunch, 0) 
                                              + ROUND(L.HighSchoolReducedL * D.DaysOfSvcLunch, 0) + ROUND(L.MiddleSchoolReducedL * D.DaysOfSvcLunch, 0) 
                                              - ROUND(F.HRLunchReduced * D.DaysOfSvcLunch, 0) AS ParticLunch, ROUND(B.AlaCarte * D.DaysOfSvcBreakfast, 0) 
                                              * B.AlaCarteRate AS ParticBreakfastAlaCarte, ROUND(L.AlaCarteL * D.DaysOfSvcLunch, 0) * L.AlaCarteRateL AS ParticLunchAlaCarte, 
                                              ROUND(B.AdultPaid * D.DaysOfSvcBreakfast, 0) * B.AdultPaidRate AS ParticBreakfastAdult, ROUND(L.AdultPaidL * D.DaysOfSvcLunch, 0) 
                                              * L.AdultPaidRateL AS ParticLunchAdult, CASE WHEN Hdr.BudgetYear = '2015-2016PA' OR
                                              Hdr.BudgetYear = '2016-2017' THEN ROUND(L.HeadStartSales * D .DaysOfSvcLunch, 0) * L.HeadStartSalesRate ELSE 0 END AS ParticHeadStart, 
                                              CASE WHEN Hdr.BudgetYear = '2015-2016PA' OR
                                              Hdr.BudgetYear = '2016-2017' THEN ROUND(L.OtherSponsors * D .DaysOfSvcLunch, 0) * L.OtherSponsorsRate ELSE 0 END AS ParticOtherSponsors, 
                                              CASE WHEN Hdr.BudgetYear = '2015-2016PA' OR
                                              Hdr.BudgetYear = '2016-2017' THEN ROUND(L.OtherRev1 * D .DaysOfSvcLunch, 0) * L.OtherRev1Rate ELSE 0 END AS ParticOtherRev1, 
                                              ROUND(L.OtherRevVending * D.DaysOfSvcLunch, 0) * L.OtherRevVendingRate AS ParticOtherRevVending, ROUND(S.SnackPaid * D.DaysOfSvcSnack, 0) 
                                              + ROUND(S.SnackReduced * D.DaysOfSvcSnack, 0) + ROUND(F.SnackFree * D.DaysOfSvcSnack, 0) AS ParticOtherSnack, 
                                              ROUND(F.SpecialMilkFree * D.DaysOfSvcSnack, 0) + ROUND(O.SpecialMilk * D.DaysOfSvcLunch, 0) AS ParticOtherMilk, 
                                              ROUND(L.AlaCarteSalesRevIncrease * D.DaysOfSvcLunch, 0) * L.AlaCarteSalesRevIncRate AS ParticAlaCarteSalesRevInc, 
                                              ROUND(L.VendingCommissions * D.DaysOfSvcLunch, 0) * L.VendingCommissionsRate AS ParticVendingCommissions, 
                                              CASE WHEN Hdr.BudgetYear = '2015-2016PA' OR
                                              Hdr.BudgetYear = '2016-2017' THEN ROUND(O.SpecialFunctions * D .DaysOfSvcLunch, 0) 
                                              * O.SpecialFunctionsRate ELSE ROUND((L.HeadStartSales + L.OtherSponsors + L.OtherRev1 + O.Catering) * D .DaysOfSvcLunch, 0) 
                                              * O.SpecialFunctionsRate END AS ParticSpecialFunctions, ROUND(O.VendingMachineSalesConc * D.DaysOfSvcLunch, 0) 
                                              * O.VendingMachineSalesRate AS ParticOtherVendingSales, ISNULL(PDE.FP_FoodPartic, 0.00000) AS ParticPDEFood, 
                                              ISNULL(PDE.FP_CleanPartic + PDE.FP_PaperPartic, 0.00000) AS ParticPDEPaprCln
                       FROM          dbo.BDGT_BudgetHDR AS Hdr LEFT OUTER JOIN
                                              dbo.BDGT_CBS_Breakfast AS B ON Hdr.BidNumber = B.BidNumber AND Hdr.BudgetYear = B.BudgetYear AND 
                                              Hdr.BudgetVersion = B.BudgetVersion LEFT OUTER JOIN
                                              dbo.BDGT_CBS_FedReimbCounts AS F ON Hdr.BidNumber = F.BidNumber AND Hdr.BudgetYear = F.BudgetYear AND 
                                              Hdr.BudgetVersion = F.BudgetVersion LEFT OUTER JOIN
                                              dbo.BDGT_DaysOfService AS D ON Hdr.BidNumber = D.BidNumber AND Hdr.BudgetYear = D.BudgetYear AND 
                                              Hdr.BudgetVersion = D.BudgetVersion LEFT OUTER JOIN
                                              dbo.BDGT_CBS_Lunches AS L ON Hdr.BidNumber = L.BidNumber AND Hdr.BudgetYear = L.BudgetYear AND 
                                              Hdr.BudgetVersion = L.BudgetVersion LEFT OUTER JOIN
                                              dbo.BDGT_CBS_SnacksSuppl AS S ON Hdr.BidNumber = S.BidNumber AND Hdr.BudgetYear = S.BudgetYear AND 
                                              Hdr.BudgetVersion = S.BudgetVersion LEFT OUTER JOIN
                                              dbo.BDGT_CBS_Additional AS A ON Hdr.BidNumber = A.BidNumber AND Hdr.BudgetYear = A.BudgetYear AND 
                                              Hdr.BudgetVersion = A.BudgetVersion LEFT OUTER JOIN
                                              dbo.BDGT_CBS_Other AS O ON Hdr.BidNumber = O.BidNumber AND Hdr.BudgetYear = O.BudgetYear AND 
                                              Hdr.BudgetVersion = O.BudgetVersion LEFT OUTER JOIN
                                              dbo.BDGT_SponsorToSponsor AS PDE ON Hdr.BidNumber = PDE.BidNumber AND Hdr.BudgetYear = PDE.BudgetYear AND 
                                              Hdr.BudgetVersion = PDE.BudgetVersion) AS PARTICIPATION
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[23] 4[18] 2[38] 3) )"
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
         Top = -192
         Left = 0
      End
      Begin Tables = 
         Begin Table = "PARTICIPATION"
            Begin Extent = 
               Top = 198
               Left = 38
               Bottom = 306
               Right = 246
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
         Column = 9825
         Alias = 1950
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_Participation'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_Participation'
GO

