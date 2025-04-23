USE [PAPERWORKWEB]
GO

/****** Object:  View [dbo].[PaycomSumPayPeriod]    Script Date: 1/10/2020 12:41:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[PaycomSumPayPeriod]
AS
SELECT        RIGHT(Distribution, 4) AS DistrictID, PayDate, SUM(Gross) AS SumOfGross, SUM(Hours) AS SumOfHours, 
                         SUM(BereavHrs + BkftHrs + FFVHrs + HolHrs + JuryHrs + PTOHrs + MeetHrs + Meet1Hrs + OTNonbudHrs + OtherHrs + OTHrs + Rate4Hrs + Rate5Hrs + RegHrs + RetroHrs + ShiftDifHrs + SpfnHrs + SummerWNHrs + SummerHrs +
                          TrainHrs + UnpaidHrs + BonusHrs + IncentHrs + InsBuyHrs + IraHrs + OtherDollarsHrs + RegDollarsHrs + RetrocativeHrs + ShiftDiffDollarHrs + SpfnDollarsHrs + ThirdPartySickHrs + WNOTHrs + WNVacHrs + PdTimeOffHrs + DNUHrs
                          + HSOAHrs) AS AllHrs, 
                         SUM(BkftHrs + FFVHrs + RegHrs + OTNonbudHrs + OTHrs + Rate4Hrs + Rate5Hrs + RetroHrs + ShiftDifHrs + SummerHrs + BonusHrs + IncentHrs + OtherDollarsHrs + RegDollarsHrs + WNOTHrs + DNUHrs + HSOAHrs) 
                         AS ProdHrs, SUM(BereavHrs + HolHrs + JuryHrs + PTOHrs + MeetHrs + Meet1Hrs + OtherHrs + TrainHrs + UnpaidHrs + InsBuyHrs + IraHrs + RetrocativeHrs + ShiftDiffDollarHrs + ThirdPartySickHrs + WNVacHrs + PdTimeOffHrs)
                          AS NonProdHrs, SUM(SpfnHrs) + SUM(SpfnDollarsHrs) AS SpFnHr, SUM(FUTA) AS SumOfFUTA, SUM(MEDR) AS SumOfMEDR, SUM(PASUI) AS SumOfPASUI, SUM(OHSUI) AS SumOfOHSUI, SUM(MISUI) AS SumOfMISUI, 
                         SUM(FLSUI) AS SumOfFLSUI, SUM(NJSUI) AS SumOfNJSUI, SUM(PittTax) AS SumOfPittTax, SUM(SSR) AS SumOfSSR, SUM(Uniform) AS SumOfUniform, SUM(BereavHrs) AS SumOfBereavHrs, SUM(BkftHrs) AS SumOfBkftHrs, 
                         SUM(FFVHrs) AS SumOfFFVHrs, SUM(HolHrs) AS SumOfHolHrs, SUM(JuryHrs) AS SumOfJuryHrs, SUM(PTOHrs) AS SumOfPTOHrs, SUM(MeetHrs) AS SumOfMeetHrs, SUM(Meet1Hrs) AS SumOfMeet1Hrs, SUM(OTNonbudHrs) 
                         AS SumOfOTNonbudHrs, SUM(OtherHrs) AS SumOfOtherHrs, SUM(OTHrs) AS SumOfOTHrs, SUM(Rate4Hrs) AS SumOfRate4Hrs, SUM(Rate5Hrs) AS SumOfRate5Hrs, SUM(RegHrs) AS SumOfRegHrs, SUM(RetroHrs) 
                         AS SumOfRetroHrs, SUM(ShiftDifHrs) AS SumOfShiftDifHrs, SUM(SpfnHrs) AS SumOfSpfnHrs, SUM(SummerWNHrs) AS SumOfSummerWNHrs, SUM(SummerHrs) AS SumOfSummerHrs, SUM(TrainHrs) AS SumOfTrainHrs, 
                         SUM(UnpaidHrs) AS SumOfUnpaidHrs, SUM(BonusHrs) AS SumOfBonusHrs, SUM(IncentHrs) AS SumOfIncentHrs, SUM(InsBuyHrs) AS SumOfInsBuyHrs, SUM(IraHrs) AS SumOfIraHrs, SUM(OtherDollarsHrs) 
                         AS SumOfOtherDollarsHrs, SUM(RegDollarsHrs) AS SumOfRegDollarsHrs, SUM(RetrocativeHrs) AS SumOfRetrocativeHrs, SUM(ShiftDiffDollarHrs) AS SumOfShiftDiffDollarHrs, SUM(SpfnDollarsHrs) AS SumOfSpfnDollarsHrs, 
                         SUM(ThirdPartySickHrs) AS SumOfThirdPartySickHrs, SUM(WNOTHrs) AS SumOfWNOTHrs, SUM(WNVacHrs) AS SumOfWNVacHrs, SUM(PdTimeOffDollar) AS SumOfPdTimeOffDollar, SUM(PdTimeOffHrs) AS SumOfPdTimeOffHrs, 
                         SUM(DNUDollars) AS SumOfDNUDollars, SUM(DNUHrs) AS SumOfDNUHrs, SUM(HSOA) AS SumOfHSOA, SUM(HSOAHrs) AS SumOfHSOAHrs
FROM            dbo.PaycomTemp
GROUP BY RIGHT(Distribution, 4), PayDate
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
         Begin Table = "PaycomTemp"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 236
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PaycomSumPayPeriod'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PaycomSumPayPeriod'
GO

