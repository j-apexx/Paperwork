
/****** Object:  View [dbo].[vwBDGT_Totals]    Script Date: 1/2/2020 9:54:45 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_Totals]
AS
SELECT        dbo.BDGT_BudgetHDR.BidNumber, dbo.BDGT_Bids.DSCRIPTN, dbo.BDGT_BudgetHDR.BudgetYear, dbo.BDGT_BudgetHDR.BudgetVersion, dbo.BDGT_BudgetHDR.ACTIVE, dbo.BDGT_BudgetHDR.BudgetType, 
                         CASE WHEN Total1 > 0 AND dbo.BDGT_BudgetHDR.BudgetType = 1 THEN (ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_FoodCostIncComm / Total1, 4) 
                         + ROUND(dbo.vwBDGT_CBS_SummaryTotals.CommodityDeliveryCharge / Total1, 4) + ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_LaborCost / Total1, 4) 
                         + ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_FringeBenefits / Total1, 4) + ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_OnsiteMgrSalBens / Total1, 4) 
                         + ROUND(dbo.vwBDGT_CBS_SummaryTotals.OtherDirectCosts / Total1, 4) + ROUND(dbo.vwBDGT_CBS_SummaryTotals.GarbageRepairsEtc / Total1, 4) 
                         + ROUND(dbo.vwBDGT_CBS_SummaryTotals.DirectCosts / Total1, 4) + ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_NonFood / Total1, 4) 
                         + ROUND(dbo.vwBDGT_CBS_SummaryTotals.PaperCleaningOnly / Total1, 4) + ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin / Total1, 4) 
                         + ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCMgmt / Total1, 4) + ROUND(dbo.vwBDGT_CBS_SummaryTotals.FSMCOtherCosts / Total1, 4) 
                         + ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_LessRebatesDiscs / Total1, 4)) ELSE 0 END AS CostPerMeal, dbo.vwBDGT_MealEquivalents.MealEquivalentFactor, 
                         ISNULL(dbo.BDGT_LaborSFA.FICAPCT + dbo.BDGT_LaborSFA.PSERSPCT + dbo.BDGT_LaborSFA.UNEMPLOYPCT + dbo.BDGT_LaborSFA.WCompPCT + dbo.BDGT_LaborSFA.MEDICAREPCT, 0) 
                         * 0.01 * 0.0001 AS MaxofFringePercent, 
                         ISNULL(dbo.BDGT_LaborNUTR.FICAPCT + dbo.BDGT_LaborNUTR.FUTAPCT + dbo.BDGT_LaborNUTR.PASUIPCT + dbo.BDGT_LaborNUTR.WCompPCT + dbo.BDGT_LaborNUTR.RelatedPayrollPCT + dbo.BDGT_LaborNUTR.SOCSECPCT
                          + dbo.BDGT_LaborSFA.MEDICAREPCT + dbo.BDGT_LaborNUTR.RETIREPCT + dbo.BDGT_LaborNUTR.WORKFORCEPCT, 0) * 0.01 * 0.0001 AS TNGOtherPayRel, 
                         dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin, dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCMgmt, 
                         dbo.vwBDGT_NonFoodCost.AccountingNUTR + dbo.vwBDGT_NonFoodCost.InvoiceFeeNUTR AS AccountingNUTR, ISNULL(dbo.vwBDGT_LaborFSDTotals.FSDRelPayroll, 0.00) AS FSDRelPayroll, 
                         dbo.vwBDGT_LaborNUTRTotals.NUTRPayrollRelated, dbo.vwBDGT_NonFoodCost.TechnologyFeeNUTR + dbo.vwBDGT_NonFoodCost.SoftwareUpgradeNUTR AS TechFeePlusSoftware, 
                         dbo.vwBDGT_NonFoodCost.EmpRecruitmentNUTR + dbo.vwBDGT_NonFoodCost.EmpReplacementNUTR AS RecruitReplace, 
                         dbo.vwBDGT_NonFoodCost.LiabilityEquipNUTR + dbo.vwBDGT_NonFoodCost.LiabilityInsurNUTR AS Liability, dbo.vwBDGT_NonFoodCost.TrainingNUTR AS Training, 
                         dbo.vwBDGT_NonFoodCost.WellnessFeeNUTR, ISNULL(dbo.vwBDGT_CACFP.CACFPAdminFeeTotal, 0) AS CACFPAdminFeeTotal, 
                         ISNULL(dbo.vwBDGT_LaborFSDTotals.FSDSubtotalWages + dbo.vwBDGT_LaborFSDTotals.FSDTotalBenefits + dbo.vwBDGT_LaborFSDTotals.FSDFICA + dbo.vwBDGT_LaborFSDTotals.FSDFUTA + dbo.vwBDGT_LaborFSDTotals.FSDPASUI
                          + dbo.vwBDGT_LaborFSDTotals.FSDWCOMP + dbo.vwBDGT_LaborFSDTotals.FSDRelPayroll + dbo.vwBDGT_LaborFSDTotals.FSD401K + dbo.vwBDGT_LaborFSDTotals.FSDMEDICARE + dbo.vwBDGT_LaborFSDTotals.FSDOBLIGATIONS
                          + dbo.vwBDGT_LaborFSDTotals.FSDSOCSEC + dbo.vwBDGT_LaborFSDTotals.FSDRETIRE + dbo.vwBDGT_LaborFSDTotals.FSDWORKFORCE, 0) AS FSDCosts, dbo.BDGT_BudgetHDR.RegionalMgr, 
                         dbo.BDGT_DaysOfService.DaysOfSvcBreakfast, dbo.BDGT_DaysOfService.DaysOfSvcLunch, CASE WHEN DaysOfSvcLunch > 0 THEN (TotalInSchoolRevenue) 
                         / dbo.BDGT_DaysOfService.DaysOfSvcLunch ELSE 0 END AS DailyCashBudget, 
                         CASE WHEN DaysOfSvcLunch > 0 THEN ROUND(([dbo].[vwBDGT_CBS_ReimbursementsSummary].[TotalFederalReimbursements] + [dbo].[vwBDGT_CBS_ReimbursementsSummary].[TotalStateReimbursements] +
                          [dbo].[vwBDGT_CBS_ReimbursementsSummary].[TotalOtherReimbursement]) / dbo.BDGT_DaysOfService.DaysOfSvcLunch, 2) ELSE 0 END AS DailyReimBudget, 
                         CASE WHEN DaysOfSvcLunch > 0 THEN (dbo.vwBDGT_CBS_SummaryTotals.CBS_FoodCostIncComm + dbo.vwBDGT_CBS_SummaryTotals.CommodityDeliveryCharge + dbo.vwBDGT_CBS_SummaryTotals.CBS_LaborCost
                          + dbo.vwBDGT_CBS_SummaryTotals.CBS_FringeBenefits + dbo.vwBDGT_CBS_SummaryTotals.CBS_OnsiteMgrSalBens + dbo.vwBDGT_CBS_SummaryTotals.OtherDirectCosts + dbo.vwBDGT_CBS_SummaryTotals.GarbageRepairsEtc
                          + dbo.vwBDGT_CBS_SummaryTotals.DirectCosts + dbo.vwBDGT_CBS_SummaryTotals.CBS_NonFood + dbo.vwBDGT_CBS_SummaryTotals.PaperCleaningOnly + dbo.vwBDGT_CBS_SummaryTotals.CBS_FoodLaborSuppl
                          + dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin + dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCMgmt + dbo.vwBDGT_CBS_SummaryTotals.FSMCOtherCosts + dbo.vwBDGT_CBS_SummaryTotals.SFAWages
                          + dbo.vwBDGT_CBS_SummaryTotals.CBS_SFABensRelCosts + dbo.vwBDGT_CBS_SummaryTotals.DistrictCosts + dbo.vwBDGT_CBS_SummaryTotals.CBS_LessRebatesDiscs + CBS_LessCommodityCredit + dbo.vwBDGT_CBS_SummaryTotals.PDE3086TOTAL
                          + dbo.vwBDGT_CBS_SummaryTotals.ExternalCateringExpense) / dbo.BDGT_DaysOfService.DaysOfSvcLunch ELSE 0 END AS DailyExpenses, 
                         CASE WHEN DaysOfSvcLunch > 0 THEN (dbo.vwBDGT_CBS_SummaryTotals.CBS_FoodCostIncComm + dbo.vwBDGT_CBS_SummaryTotals.CommodityDeliveryCharge + dbo.vwBDGT_CBS_SummaryTotals.CBS_LaborCost
                          + dbo.vwBDGT_CBS_SummaryTotals.CBS_FringeBenefits + dbo.vwBDGT_CBS_SummaryTotals.CBS_OnsiteMgrSalBens + dbo.vwBDGT_CBS_SummaryTotals.OtherDirectCosts + dbo.vwBDGT_CBS_SummaryTotals.GarbageRepairsEtc
                          + dbo.vwBDGT_CBS_SummaryTotals.DirectCosts + dbo.vwBDGT_CBS_SummaryTotals.CBS_NonFood + dbo.vwBDGT_CBS_SummaryTotals.PaperCleaningOnly + dbo.vwBDGT_CBS_SummaryTotals.CBS_FoodLaborSuppl
                          + dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin + dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCMgmt + dbo.vwBDGT_CBS_SummaryTotals.FSMCOtherCosts + dbo.vwBDGT_CBS_SummaryTotals.SFAWages
                          + dbo.vwBDGT_CBS_SummaryTotals.CBS_SFABensRelCosts + dbo.vwBDGT_CBS_SummaryTotals.DistrictCosts + dbo.vwBDGT_CBS_SummaryTotals.CBS_LessRebatesDiscs) 
                         / dbo.BDGT_DaysOfService.DaysOfSvcLunch ELSE 0 END AS DailyCosts, 
                         ISNULL(dbo.vwBDGT_LaborSFATotals.SFAWages + dbo.vwBDGT_LaborSFATotals.SFEBensIns + dbo.vwBDGT_LaborSFATotals.SFAFICA + dbo.vwBDGT_LaborSFATotals.SFAPSERS + dbo.vwBDGT_LaborSFATotals.SFAUnemployment
                          + dbo.vwBDGT_LaborSFATotals.SFAWComp + dbo.vwBDGT_LaborSFATotals.SFAMedicare, 0) AS SFALabor, CASE WHEN (ISNULL(ParticSpecialFunctions, 0) + ISNULL(TotalParticipation, 0)) 
                         > 0 THEN ROUND((ISNULL(SubtotalCostFoodOnly, 0) + ISNULL(SFTotalCostFoodOnly, 0)) / (ISNULL(ParticSpecialFunctions, 0) + ISNULL(TotalParticipation, 0)), 4) ELSE 0 END AS FoodCPM, 
                         ISNULL(DTL.YTDActualPaper + ROUND(DTL.YTDActualPaper * DTL.PctIncreasePaper * 0.0001, 4), 0) AS PaperCPM, 
                         ISNULL(DTL.YtdActualCleaning + ROUND(DTL.YtdActualCleaning * DTL.PctIncreaseCleaning * 0.0001, 4), 0) AS CleaningCPM, dbo.BDGT_CBS_Additional.LessCommodityUseAmt, 
                         CASE WHEN DaysOfSvcLunch > 0 THEN (dbo.vwBDGT_CBS_InSchoolTotals.L_OtherSponsors + dbo.vwBDGT_CBS_InSchoolTotals.L_OtherRev1 + dbo.vwBDGT_CBS_InSchoolTotals.L_OtherRevVending + dbo.vwBDGT_CBS_InSchoolTotals.L_VendingCommissions
                          + dbo.vwBDGT_CBS_InSchoolTotals.S_SnackPaid + dbo.vwBDGT_CBS_InSchoolTotals.S_SnackReduced + dbo.vwBDGT_CBS_InSchoolTotals.S_SnackAdult + dbo.vwBDGT_CBS_InSchoolTotals.S_SnackAlaCarte
                          + dbo.vwBDGT_CBS_InSchoolTotals.O_SpecialMilk + dbo.vwBDGT_CBS_InSchoolTotals.O_VendingMachineSalesConc + PDE3086TOTAL + O_SpecialFunctions) 
                         / dbo.BDGT_DaysOfService.DaysOfSvcLunch ELSE 0 END AS DailyOtherSales, 
                         dbo.vwBDGT_NonFoodCost.AccountingDIST + dbo.vwBDGT_NonFoodCost.CarTruckRentDIST + dbo.vwBDGT_NonFoodCost.CarTruckMileageDIST + dbo.vwBDGT_NonFoodCost.CarTruckMaintDIST + dbo.vwBDGT_NonFoodCost.ChinaSilvGlassDIST
                          + dbo.vwBDGT_NonFoodCost.CleaningJanitorDIST + dbo.vwBDGT_NonFoodCost.SoftwareUpgradeDIST + dbo.vwBDGT_NonFoodCost.TechnologyFeeDIST + dbo.vwBDGT_NonFoodCost.EmpRecruitmentDIST + dbo.vwBDGT_NonFoodCost.EmpReplacementDIST
                          + dbo.vwBDGT_NonFoodCost.IndirectLaborDIST + dbo.vwBDGT_NonFoodCost.EquipMaintDIST + dbo.vwBDGT_NonFoodCost.EquipRepairDIST + dbo.vwBDGT_NonFoodCost.EquipReplaceDIST + dbo.vwBDGT_NonFoodCost.OfficeExpenseDIST
                          + dbo.vwBDGT_NonFoodCost.PrintingDIST + dbo.vwBDGT_NonFoodCost.POSDIST + dbo.vwBDGT_NonFoodCost.PostageDIST + dbo.vwBDGT_NonFoodCost.SmallwareDIST + dbo.vwBDGT_NonFoodCost.PestControlDIST
                          + dbo.vwBDGT_NonFoodCost.TrashDIST + dbo.vwBDGT_NonFoodCost.LaundryDIST + dbo.vwBDGT_NonFoodCost.UniformsDIST + dbo.vwBDGT_NonFoodCost.IndirectLaborSFADIST + dbo.vwBDGT_NonFoodCost.UtilsSFADIST
                          + dbo.vwBDGT_NonFoodCost.SFARFPDIST_1 + dbo.vwBDGT_NonFoodCost.SFARFPDIST_2 + dbo.vwBDGT_NonFoodCost.SFARFPDIST_3 + dbo.vwBDGT_NonFoodCost.LinensDIST + dbo.vwBDGT_NonFoodCost.DepreciationClient
                          AS DistrictCosts, CASE WHEN DaysOfSvcLunch > 0 THEN PSERS_RetireAmt / dbo.BDGT_DaysOfService.DaysOfSvcLunch ELSE 0 END AS DailyPSERSCredit, ISNULL(dbo.BDGT_LaborNUTR.WCompPCT, 0) 
                         * 0.01 * 0.0001 AS TNGWComp, CASE WHEN DaysOfSvcLunch > 0 THEN InterestIncAmount / dbo.BDGT_DaysOfService.DaysOfSvcLunch ELSE 0 END AS DailyInterest, 
                         (dbo.vwBDGT_CBS_ReimbursementsSummary.TotalInSchoolRevenue + dbo.vwBDGT_CBS_ReimbursementsSummary.TotalFederalReimbursements + dbo.vwBDGT_CBS_ReimbursementsSummary.TotalStateReimbursements
                          + dbo.vwBDGT_CBS_ReimbursementsSummary.PSERS_RetireAmt + dbo.vwBDGT_CBS_ReimbursementsSummary.AdditionalAmount + dbo.vwBDGT_CBS_ReimbursementsSummary.InterestIncAmount + dbo.vwBDGT_CBS_ReimbursementsSummary.ToEqualBidRevenue
                          + dbo.vwBDGT_CBS_ReimbursementsSummary.TotalOtherReimbursement + dbo.vwBDGT_CBS_ReimbursementsSummary.CateringRevenue + dbo.vwBDGT_CBS_ReimbursementsSummary.PDE3086Revenue) 
                         - (dbo.vwBDGT_CBS_SummaryTotals.CBS_FoodCostIncComm + dbo.vwBDGT_CBS_SummaryTotals.CommodityDeliveryCharge + dbo.vwBDGT_CBS_SummaryTotals.CBS_LaborCost + dbo.vwBDGT_CBS_SummaryTotals.CBS_FringeBenefits
                          + dbo.vwBDGT_CBS_SummaryTotals.CBS_OnsiteMgrSalBens + dbo.vwBDGT_CBS_SummaryTotals.OtherDirectCosts + dbo.vwBDGT_CBS_SummaryTotals.GarbageRepairsEtc + dbo.vwBDGT_CBS_SummaryTotals.DirectCosts
                          + dbo.vwBDGT_CBS_SummaryTotals.CBS_NonFood + dbo.vwBDGT_CBS_SummaryTotals.PaperCleaningOnly + dbo.vwBDGT_CBS_SummaryTotals.CBS_FoodLaborSuppl + dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin
                          + dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCMgmt + dbo.vwBDGT_CBS_SummaryTotals.FSMCOtherCosts + dbo.vwBDGT_CBS_SummaryTotals.SFAWages + dbo.vwBDGT_CBS_SummaryTotals.CBS_SFABensRelCosts
                          + dbo.vwBDGT_CBS_SummaryTotals.DistrictCosts + dbo.vwBDGT_CBS_SummaryTotals.CBS_LessRebatesDiscs + dbo.vwBDGT_CBS_SummaryTotals.CBS_LessCommodityCredit + dbo.vwBDGT_CBS_SummaryTotals.PDE3086TOTAL
                          + dbo.vwBDGT_CBS_SummaryTotals.ExternalCateringExpense) AS ProfitBudget, CASE WHEN DaysOfSvcLunch > 0 THEN (dbo.BDGT_CBS_Additional.GuaranteeToSFA / DaysOfSvcLunch) 
                         ELSE 0 END AS GuaranteePerDay, ISNULL(dbo.BDGT_TNG_FactsStats.ExtentOfGuarantee, '') AS ExtentOfGuarantee, ISNULL(dbo.BDGT_TNG_FactsStats.CURRProjectedPybl, 0.00000) AS CURRProjectedPybl, 
                         dbo.vwBDGT_CBS_ReimbursementsSummary.AdditionalAmount, 
                         dbo.vwBDGT_CBS_ReimbursementsSummary.TotalInSchoolRevenue + dbo.vwBDGT_CBS_ReimbursementsSummary.TotalFederalReimbursements + dbo.vwBDGT_CBS_ReimbursementsSummary.TotalStateReimbursements
                          + dbo.vwBDGT_CBS_ReimbursementsSummary.PSERS_RetireAmt + dbo.vwBDGT_CBS_ReimbursementsSummary.AdditionalAmount + dbo.vwBDGT_CBS_ReimbursementsSummary.InterestIncAmount + dbo.vwBDGT_CBS_ReimbursementsSummary.ToEqualBidRevenue
                          + dbo.vwBDGT_CBS_ReimbursementsSummary.TotalOtherReimbursement + dbo.vwBDGT_CBS_ReimbursementsSummary.CateringRevenue + dbo.vwBDGT_CBS_ReimbursementsSummary.PDE3086Revenue AS
                          TotalRevenue, CASE WHEN DaysOfSvcBreakfast > 0 THEN (0.0 + dbo.vwBDGT_FederalReimbursements.B_MealCount / DaysOfSvcBreakfast) ELSE 0 END AS DailyBreakfast, 
                         CASE WHEN DaysOfSvcLunch > 0 THEN ((dbo.vwBDGT_FederalReimbursements.HRL_MealCount + LRL_MealCount) / DaysOfSvcLunch) ELSE 0 END AS DailyLunch, 
                         CASE WHEN DaysOfSvcLunch > 0 THEN ((B_Adult + B_Ala + L_AdultPaidL + L_AlaCarteL + L_AlaCarteSalesRevIncrease) / DaysOfSvcLunch) ELSE 0 END AS DailyAla, 
                         dbo.vwBDGT_DailyTotals.DailyBPaidCount + dbo.vwBDGT_DailyTotals.DailyBPaidTierCount AS DailyBPaidCount, dbo.vwBDGT_DailyTotals.DailyBReducedCount, dbo.vwBDGT_DailyTotals.DailyBFreeCount, 
                         dbo.vwBDGT_DailyTotals.DailyLPaidCount + dbo.vwBDGT_DailyTotals.DailyLPaidTierCount AS DailyLPaidCount, dbo.vwBDGT_DailyTotals.DailyLReducedCount, dbo.vwBDGT_DailyTotals.DailyLFreeCount, 
                         dbo.vwBDGT_CBS_SummaryTotals.PDE3086TOTAL AS PDE3086Expenses, dbo.vwBDGT_CBS_ReimbursementsSummary.PDE3086Revenue, dbo.BDGT_BudgetHDR.CB_CEP AS CEP, 
                         CASE Total1 WHEN 0 THEN 0 ELSE ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin / dbo.vwBDGT_MealEquivalents.Total1, 4) END AS FSMCAdminCPM, 
                         CASE Total1 WHEN 0 THEN 0 ELSE ROUND(dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCMgmt / dbo.vwBDGT_MealEquivalents.Total1, 4) END AS FSMCMgmtCPM, 
                         ISNULL(dbo.BDGT_LaborFSD.RelatedPayrollPCT, 0) * 0.0001 * 0.01 AS PayrollRelatedFSD, ISNULL(dbo.BDGT_LaborNUTR.RelatedPayrollPCT, 0) * 0.0001 * 0.01 AS PayrollRelatedHourly, 
                         ISNULL(dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin, 0) * 0.56 AS BusinessOffExp, ISNULL(dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin, 0) * 0.12 AS UpperMgtExp, 
                         ISNULL(dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin, 0) - ISNULL(dbo.vwBDGT_CBS_SummaryTotals.CBS_FSMCAdmin, 0) * 0.68 AS SupportSvcExp, 
                         ISNULL(dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS1 + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS2 + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS3 + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS4 + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS5
                          + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS6 + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS7 + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS8 + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS9 + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS10
                          + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS11 + dbo.vwBDGT_ScheduleDays.ELEVENTHDAYS12, 0) AS BillingPeriods, 
                         (dbo.BDGT_LaborSFA.FICAPCT + dbo.BDGT_LaborSFA.PSERSPCT + dbo.BDGT_LaborSFA.UNEMPLOYPCT + dbo.BDGT_LaborSFA.WCompPCT + dbo.BDGT_LaborSFA.MEDICAREPCT) 
                         * 0.01 * 0.0001 AS SFA_PR
FROM            dbo.BDGT_BudgetHDR LEFT OUTER JOIN
                         dbo.BDGT_FoodCost ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_FoodCost.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_FoodCost.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_FoodCost.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_FOODCOST ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_FOODCOST.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_FOODCOST.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_FOODCOST.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_Participation ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_Participation.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_Participation.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_Participation.BudgetVersion LEFT OUTER JOIN
                         dbo.vw_BDGT_SpecialFunctionsCostTotals ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vw_BDGT_SpecialFunctionsCostTotals.BidNumber AND 
                         dbo.BDGT_BudgetHDR.BudgetYear = dbo.vw_BDGT_SpecialFunctionsCostTotals.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vw_BDGT_SpecialFunctionsCostTotals.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_FederalReimbursements ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_FederalReimbursements.BidNumber AND 
                         dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_FederalReimbursements.BudgetYear AND dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_FederalReimbursements.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_CBS_Additional ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_CBS_Additional.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_CBS_Additional.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_CBS_Additional.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_CBS_ReimbursementsSummary ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_CBS_ReimbursementsSummary.BidNumber AND 
                         dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_CBS_ReimbursementsSummary.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_CBS_ReimbursementsSummary.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_NonFoodCost ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_NonFoodCost.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_NonFoodCost.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_NonFoodCost.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_CACFP ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_CACFP.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_CACFP.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_CACFP.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_LaborFSDTotals ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_LaborFSDTotals.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_LaborFSDTotals.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_LaborFSDTotals.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_DaysOfService ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_DaysOfService.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_DaysOfService.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_DaysOfService.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_CBS_InSchoolTotals ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_CBS_InSchoolTotals.BidNumber AND 
                         dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_CBS_InSchoolTotals.BudgetYear AND dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_CBS_InSchoolTotals.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_LaborSFA ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_LaborSFA.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_LaborSFA.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_LaborSFA.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_LaborNUTR ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_LaborNUTR.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_LaborNUTR.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_LaborNUTR.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_LaborFSD ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_LaborFSD.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_LaborFSD.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_LaborFSD.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_TNG_FactsStats ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_TNG_FactsStats.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.BDGT_TNG_FactsStats.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.BDGT_TNG_FactsStats.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_CBS_SummaryTotals ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_CBS_SummaryTotals.BidNumber AND 
                         dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_CBS_SummaryTotals.BudgetYear AND dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_CBS_SummaryTotals.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_MealEquivalents ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_MealEquivalents.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_MealEquivalents.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_MealEquivalents.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_LaborSFATotals ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_LaborSFATotals.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_LaborSFATotals.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_LaborSFATotals.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_Bids ON dbo.BDGT_BudgetHDR.BidNumber = dbo.BDGT_Bids.BidNumber LEFT OUTER JOIN
                         dbo.BDGT_NonFoodCostDtl AS DTL ON dbo.BDGT_BudgetHDR.BidNumber = DTL.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = DTL.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = DTL.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_LaborNUTRTotals ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_LaborNUTRTotals.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_LaborNUTRTotals.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_LaborNUTRTotals.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_DailyTotals ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_DailyTotals.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_DailyTotals.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = dbo.vwBDGT_DailyTotals.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_ScheduleDays ON dbo.BDGT_BudgetHDR.BidNumber = dbo.vwBDGT_ScheduleDays.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = dbo.vwBDGT_ScheduleDays.BudgetYear
WHERE        (dbo.BDGT_BudgetHDR.ACTIVE = 1)
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[29] 4[17] 2[30] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[43] 4[33] 3) )"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[33] 2[32] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1[56] 3) )"
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
         Configuration = "(H (1[62] 4) )"
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
      ActivePaneConfig = 2
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = -192
         Left = 0
      End
      Begin Tables = 
         Begin Table = "BDGT_BudgetHDR"
            Begin Extent = 
               Top = 0
               Left = 6
               Bottom = 187
               Right = 173
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "BDGT_FoodCost"
            Begin Extent = 
               Top = 197
               Left = 724
               Bottom = 384
               Right = 918
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "vwBDGT_FOODCOST"
            Begin Extent = 
               Top = 115
               Left = 750
               Bottom = 223
               Right = 961
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "vwBDGT_Participation"
            Begin Extent = 
               Top = 65
               Left = 735
               Bottom = 173
               Right = 943
            End
            DisplayFlags = 344
            TopColumn = 17
         End
         Begin Table = "vw_BDGT_SpecialFunctionsCostTotals"
            Begin Extent = 
               Top = 95
               Left = 421
               Bottom = 203
               Right = 622
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "vwBDGT_FederalReimbursements"
            Begin Extent = 
               Top = 54
               Left = 450
               Bottom = 162
               Right = 711
            End
            DisplayFlags = 344
            TopColumn = 19
         End
         Begin Table = "BDGT_CBS_Additional"
            Begin Extent = 
               Top = 10
               Left = 459
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_Totals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'      Bottom = 195
               Right = 669
            End
            DisplayFlags = 344
            TopColumn = 2
         End
         Begin Table = "vwBDGT_CBS_ReimbursementsSummary"
            Begin Extent = 
               Top = 7
               Left = 218
               Bottom = 166
               Right = 437
            End
            DisplayFlags = 344
            TopColumn = 5
         End
         Begin Table = "vwBDGT_NonFoodCost"
            Begin Extent = 
               Top = 173
               Left = 485
               Bottom = 359
               Right = 680
            End
            DisplayFlags = 344
            TopColumn = 65
         End
         Begin Table = "vwBDGT_CACFP"
            Begin Extent = 
               Top = 225
               Left = 461
               Bottom = 434
               Right = 645
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "vwBDGT_LaborFSDTotals"
            Begin Extent = 
               Top = 274
               Left = 438
               Bottom = 478
               Right = 611
            End
            DisplayFlags = 344
            TopColumn = 10
         End
         Begin Table = "BDGT_DaysOfService"
            Begin Extent = 
               Top = 329
               Left = 403
               Bottom = 466
               Right = 582
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "vwBDGT_CBS_InSchoolTotals"
            Begin Extent = 
               Top = 376
               Left = 360
               Bottom = 611
               Right = 580
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "BDGT_LaborSFA"
            Begin Extent = 
               Top = 428
               Left = 344
               Bottom = 698
               Right = 504
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "BDGT_LaborNUTR"
            Begin Extent = 
               Top = 460
               Left = 279
               Bottom = 640
               Right = 447
            End
            DisplayFlags = 344
            TopColumn = 5
         End
         Begin Table = "BDGT_LaborFSD"
            Begin Extent = 
               Top = 330
               Left = 956
               Bottom = 460
               Right = 1143
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "BDGT_TNG_FactsStats"
            Begin Extent = 
               Top = 278
               Left = 27
               Bottom = 499
               Right = 219
            End
            DisplayFlags = 344
            TopColumn = 46
         End
         Begin Table = "vwBDGT_CBS_SummaryTotals"
            Begin Extent = 
               Top = 314
               Left = 17
               Bottom = 534
               Right = 224
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "vwBDGT_MealEquivalents"
            Begin Extent = 
               Top = 351
               Left = 20
               Bottom = 536
               Right = 224
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "vwBDGT_LaborSFATotals"
            Begin Extent = 
               Top = 391
               Left = 35
               Bottom = 579
               Right = 243
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "BDGT_Bids' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_Totals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane3', @value=N'"
            Begin Extent = 
               Top = 29
               Left = 749
               Bottom = 137
               Right = 903
            End
            DisplayFlags = 344
            TopColumn = 0
         End
         Begin Table = "DTL"
            Begin Extent = 
               Top = 160
               Left = 740
               Bottom = 268
               Right = 935
            End
            DisplayFlags = 344
            TopColumn = 91
         End
         Begin Table = "vwBDGT_LaborNUTRTotals"
            Begin Extent = 
               Top = 221
               Left = 257
               Bottom = 329
               Right = 435
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "vwBDGT_DailyTotals"
            Begin Extent = 
               Top = 198
               Left = 999
               Bottom = 328
               Right = 1200
            End
            DisplayFlags = 280
            TopColumn = 7
         End
         Begin Table = "vwBDGT_ScheduleDays"
            Begin Extent = 
               Top = 198
               Left = 1238
               Bottom = 328
               Right = 1424
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
      Begin ColumnWidths = 67
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
      PaneHidden = 
      Begin ColumnWidths = 11
         Column = 9390
         Alias = 2115
         Table = 2070
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_Totals'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=3 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vwBDGT_Totals'
GO

