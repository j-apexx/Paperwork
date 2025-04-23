USE [PaperworkWeb]
GO

/****** Object:  View [dbo].[vw_RecapTotals]    Script Date: 1/2/2020 9:51:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_RecapTotals]
AS
SELECT        ProgramYear, DistrictID, SchoolNum, RecapDate, BPaidSaleElem, BPaidSaleJr, BPaidSaleSr, BReducedSaleTotal, LPaidSaleElem, LPaidSaleJr, LPaidSaleSr, LPaidSaleTier1, LPaidSaleTier2, LReducedSaleTotal,
                          LStuAla, LAdultAla, LOtherAla1, SpecialFunctionSale, SpecialFunctionComm, USDASale, OtherSale1, OtherSale2, PaidSnackSale, ReducedSnackSale, Cash, 
                         BPaidSaleElem + BPaidSaleJr + BPaidSaleSr AS BPaidSaleTotal, BPaidSaleElem + BPaidSaleJr + BPaidSaleSr + BReducedSaleTotal AS BSaleTotal, 
                         LPaidSaleElem + LPaidSaleJr + LPaidSaleSr + LPaidSaleTier1 + LPaidSaleTier2 AS LPaidSaleTotal, 
                         LPaidSaleElem + LPaidSaleJr + LPaidSaleSr + LPaidSaleTier1 + LPaidSaleTier2 + LReducedSaleTotal AS LSaleTotal, (BPaidSaleElem + BPaidSaleJr + BPaidSaleSr + BReducedSaleTotal) 
                         + (LPaidSaleElem + LPaidSaleJr + LPaidSaleSr + LPaidSaleTier1 + LPaidSaleTier2 + LReducedSaleTotal) 
                         + TotalAla + SpecialFunctionSale + SpecialFunctionComm + USDASale + OtherSale1 + OtherSale2 + PaidSnackSale + ReducedSnackSale AS TotalSale, AdvancedTotal, RedeemedTotal, ChargeRepaidTotal, 
                         ChargeMadeTotal, BPaidReim, BReducedReim, BFreeReim, LPaidReim, LReducedReim, LFreeReim, BLunchIncentive, SnackReim
FROM            (SELECT        dbo.RecapEntry.ProgramYear, dbo.RecapEntry.DistrictID, dbo.RecapEntry.SchoolNum, dbo.RecapEntry.RecapDate, dbo.RecapEntry.BPaidElem * SchoolSetup.BfastElPrice AS BPaidSaleElem, 
                                                    dbo.RecapEntry.BPaidJr * SchoolSetup.BfastJrPrice AS BPaidSaleJr, dbo.RecapEntry.BPaidSr * SchoolSetup.BfastSrPrice AS BPaidSaleSr, 
                                                    dbo.RecapEntry.BReduced * SchoolSetup.BfastRedPrice AS BReducedSaleTotal, dbo.RecapEntry.LPaidElem * SchoolSetup.LunchElPrice AS LPaidSaleElem, 
                                                    dbo.RecapEntry.LPaidJr * SchoolSetup.LunchJrPrice AS LPaidSaleJr, dbo.RecapEntry.LPaidSr * SchoolSetup.LunchSrPrice AS LPaidSaleSr, 
                                                    dbo.RecapEntry.LTier1 * SchoolSetup.LunchTier1Price AS LPaidSaleTier1, dbo.RecapEntry.LTier2 * SchoolSetup.LunchTier2Price AS LPaidSaleTier2, dbo.RecapEntry.BStuAla, 
                                                    dbo.RecapEntry.BAdultAla, (dbo.RecapEntry.LReduced + dbo.RecapEntry.LReducedTier1) * SchoolSetup.LunchRedPrice AS LReducedSaleTotal, dbo.RecapEntry.R_AF, dbo.RecapEntry.R_Enrollment, 
                                                    dbo.RecapEntry.LStuAla, dbo.RecapEntry.LAdultAla, dbo.RecapEntry.LOtherAla1, dbo.RecapEntry.LVendAla, dbo.RecapEntry.SpecialFunctionSale, dbo.RecapEntry.SpecialFunctionComm, 
                                                    dbo.RecapEntry.USDASale, dbo.RecapEntry.OtherSale1, dbo.RecapEntry.OtherSale2, dbo.RecapEntry.OPaidSnack * SchoolSetup.OSnackPaidPrice AS PaidSnackSale, 
                                                    dbo.RecapEntry.OReducedSnack * SchoolSetup.OSnackRedPrice AS ReducedSnackSale, dbo.RecapEntry.BDeposit + dbo.RecapEntry.Deposit AS Cash, 
                                                    dbo.RecapEntry.BAdvanced + dbo.RecapEntry.LAdvanced AS AdvancedTotal, dbo.RecapEntry.BRedeemed + dbo.RecapEntry.LRedeemed AS RedeemedTotal, 
                                                    dbo.RecapEntry.BRepaid + dbo.RecapEntry.LRepaid AS ChargeRepaidTotal, dbo.RecapEntry.BCharges + dbo.RecapEntry.LCharges AS ChargeMadeTotal, 
                                                    dbo.RecapEntry.LStuAla + dbo.RecapEntry.LAdultAla + dbo.RecapEntry.LOtherAla1 + dbo.RecapEntry.LVendAla + dbo.RecapEntry.BAdultAla + dbo.RecapEntry.BStuAla AS TotalAla, 
                                                    (dbo.RecapEntry.BPaidElem + dbo.RecapEntry.BPaidJr + dbo.RecapEntry.BPaidSr + dbo.RecapEntry.BPaidLrn) * SchoolSetup.BfastPaidRate AS BPaidReim, 
                                                    (dbo.RecapEntry.BReduced + dbo.RecapEntry.BReducedLrn) * SchoolSetup.BfastRedRate AS BReducedReim, (dbo.RecapEntry.BFree + dbo.RecapEntry.BFreeLrn) 
                                                    * SchoolSetup.BfastFreeRate AS BFreeReim, 
                                                    (dbo.RecapEntry.LPaidElem + dbo.RecapEntry.LPaidJr + dbo.RecapEntry.LPaidSr + dbo.RecapEntry.LPaidLrn + dbo.RecapEntry.LTier1 + dbo.RecapEntry.LTier2) 
                                                    * SchoolSetup.LunchPaidRate AS LPaidReim, (dbo.RecapEntry.LReduced + dbo.RecapEntry.LReducedLrn + dbo.RecapEntry.LReducedTier1) * SchoolSetup.LunchRedRate AS LReducedReim, 
                                                    (dbo.RecapEntry.LFree + dbo.RecapEntry.LFreeLrn + dbo.RecapEntry.LFreeTier1 + dbo.RecapEntry.LFreeTier2) * SchoolSetup.LunchFreeRate AS LFreeReim, 
                                                    (dbo.RecapEntry.LPaidElem + dbo.RecapEntry.LPaidJr + dbo.RecapEntry.LPaidSr + dbo.RecapEntry.LPaidLrn + dbo.RecapEntry.LTier1 + dbo.RecapEntry.LTier2 + dbo.RecapEntry.LReduced + dbo.RecapEntry.LReducedLrn
                                                     + dbo.RecapEntry.LReducedTier1 + dbo.RecapEntry.LFree + dbo.RecapEntry.LFreeLrn + dbo.RecapEntry.LFreeTier1 + dbo.RecapEntry.LFreeTier2) 
                                                    * dbo.RecapEntry.R_BreakfastIncent AS BLunchIncentive, (dbo.RecapEntry.OPaidSnack * SchoolSetup.OSnackPaidRate + dbo.RecapEntry.OReducedSnack * SchoolSetup.OSnackRedRate) 
                                                    + dbo.RecapEntry.OFreeSnack * SchoolSetup.OSnackFreeRate AS SnackReim
                          FROM            dbo.RecapEntry INNER JOIN
                                                    dbo.DistrictSchoolYearSetup AS SchoolSetup ON dbo.RecapEntry.DistrictID = SchoolSetup.DistrictID AND dbo.RecapEntry.SchoolNum = SchoolSetup.SchoolNum AND 
                                                    dbo.RecapEntry.ProgramYear = SchoolSetup.ProgramYear) AS Recaps
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[21] 4[26] 2[35] 3) )"
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
         Begin Table = "Recaps"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 248
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
      Begin ColumnWidths = 16
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_RecapTotals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_RecapTotals'
GO

