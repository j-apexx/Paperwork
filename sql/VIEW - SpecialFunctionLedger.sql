USE [PaperworkWeb]
GO

/****** Object:  View [dbo].[SpecialFunctionsLedger]    Script Date: 5/5/2020 11:07:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[SpecialFunctionsLedger]
AS
SELECT        HDR.ProgramYear, HDR.DistrictID, HDR.InvoiceNumber, [OrgBilled], [BillingDate], [FunctionDate], ISNULL(Subtotal, 0) Subtotal, ISNULL(LinesMarkup, 0) Markup, (ISNULL(Subtotal, 0) + ISNULL(LinesMarkup, 0)) 
                         InvoiceTotal, [TNGLabor], ([TNGFringePct] * [TNGLabor] * 0.01) TNGFringe, [DistrictLabor], ([DistrictLabor] * [DistrictFringePct] * 0.01) DistrictFringe, [SpecialFunctionDesc]
FROM            [SPFNInvoiceHDR] HDR LEFT OUTER JOIN
                             (SELECT        SUM([MiscAmount]) LinesTotal, SUM(IIF(MarkupType = 0, 0, IIF(MarkupType = 1, MarkupPct * MiscAmount * 0.01, MarkupDlr))) LinesMarkup, [ProgramYear], [DistrictID], [InvoiceNumber]
                               FROM            [SPFNInvoiceLines]
                               GROUP BY [ProgramYear], [DistrictID], [InvoiceNumber]) LINES ON HDR.ProgramYear = LINES.ProgramYear AND HDR.DistrictID = LINES.DistrictID AND HDR.InvoiceNumber = LINES.InvoiceNumber CROSS 
                         APPLY
                             (SELECT        [FoodCost] + [PaperCleanSuppl] + [Mileage] + [FSDCosts] + [TNGLabor] + ([TNGFringePct] * [TNGLabor] * 0.01) + [DistrictLabor] + ([DistrictLabor] * [DistrictFringePct] * 0.01) + [LinesTotal] Subtotal) 
                         AS Calc1 CROSS APPLY
                             (SELECT        CASE MarkupType WHEN 0 THEN 0 WHEN 1 THEN (ISNULL(Subtotal, 0) * MarkupPct * 0.01) WHEN 2 THEN [MarkupDlr] END Markup) AS Calc2
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[17] 4[28] 2[23] 3) )"
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
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SpecialFunctionsLedger'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SpecialFunctionsLedger'
GO

