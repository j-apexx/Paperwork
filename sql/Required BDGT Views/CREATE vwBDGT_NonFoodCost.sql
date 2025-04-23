

/****** Object:  View [dbo].[vwBDGT_NonFoodCost]    Script Date: 3/12/2020 12:10:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_NonFoodCost]
AS
SELECT        dbo.BDGT_BudgetHDR.BidNumber, dbo.BDGT_BudgetHDR.BudgetYear, dbo.BDGT_BudgetHDR.BudgetVersion, ISNULL(P.ParticBLAC * FC.InvPrepProcCPM, 0) AS AccountingNUTR, ISNULL(DTL.AccountingDIST, 
                         0) AS AccountingDIST, ISNULL(DTL.ElectronicInvFeeNUTR, 0) AS InvoiceFeeNUTR, ISNULL(DTL.BackgroundCheckNUTR, 0) AS BackgroundCheckNUTR, ISNULL(DTL.CarTruckRentNUTR, 0) AS CarTruckRentNUTR, 
                         ISNULL(DTL.CarTruckRentDIST, 0) AS CarTruckRentDIST, ISNULL(FSD.FSDExpTravel, 0) AS FSDExpTravel, ISNULL(DTL.CarTruckMileageDIST, 0) AS CarTruckMileageDIST, ISNULL(DTL.CarTruckMaintNUTR, 0) 
                         AS CarTruckMaintNUTR, ISNULL(DTL.CarTruckMaintDIST, 0) AS CarTruckMaintDIST, ISNULL(DTL.ChinaSilvGlassNUTR, 0) AS ChinaSilvGlassNUTR, ISNULL(DTL.ChinaSilvGlassDIST, 0) AS ChinaSilvGlassDIST, 
                         ISNULL(DTL.CleaningJanitorDIST, 0) AS CleaningJanitorDIST, ISNULL(ROUND(P.GrandTotalPartic * (DTL.YtdActualCleaning + ROUND(DTL.YtdActualCleaning * DTL.PctIncreaseCleaning * 0.0001, 4)), 2), 0) 
                         AS CleaningJanitorNUTR, ISNULL(DTL.SoftwareUpgradeDIST, 0) AS SoftwareUpgradeDIST, ISNULL(DTL.SoftwareUpgradeNUTR, 0) AS SoftwareUpgradeNUTR, 
                         ISNULL(ROUND(DTL.TechnologyFeeCPM * P.ParticBLAC, 2), 0) AS TechnologyFeeNUTR, ISNULL(DTL.TechnologyFeeDIST, 0) AS TechnologyFeeDIST, ISNULL(DTL.CourierNUTR, 0) AS CourierNUTR, 
                         ISNULL(FSD.FSDExpDues + FSD.FSDExpSubs, 0) AS DuesSubsNUTR, ISNULL(DTL.EmployeeMealsNUTR, 0) AS EmployeeMealsNUTR, ISNULL(ROUND(DTL.EmpRecruitmentCPM * P.ParticBLAC, 2), 0) 
                         AS EmpRecruitmentNUTR, ISNULL(DTL.EmpRecruitmentDIST, 0) AS EmpRecruitmentDIST, ISNULL(ROUND(DTL.EmpReplacementCPM * P.ParticBLAC, 2), 0) AS EmpReplacementNUTR, 
                         ISNULL(DTL.EmpReplacementDIST, 0) AS EmpReplacementDIST, ISNULL(DTL.IndirectLaborNUTR, 0) AS IndirectLaborNUTR, ISNULL(DTL.IndirectLaborDIST, 0) AS IndirectLaborDIST, 
                         ISNULL(DTL.BuybackInvestNUTR, 0) AS BuybackInvestNUTR, ISNULL(DTL.RentalsNUTR, 0) AS RentalsNUTR, ISNULL(DTL.EquipMaintNUTR, 0) AS EquipMaintNUTR, ISNULL(DTL.EquipMaintDIST, 0) 
                         AS EquipMaintDIST, ISNULL(DTL.EquipRepairNUTR, 0) AS EquipRepairNUTR, ISNULL(DTL.EquipRepairDIST, 0) AS EquipRepairDIST, ISNULL(DTL.EquipReplaceNUTR, 0) AS EquipReplaceNUTR, 
                         ISNULL(DTL.EquipReplaceDIST, 0) AS EquipReplaceDIST, ISNULL(DTL.FreightDelivNUTR, 0) AS FreightDelivNUTR, ISNULL(ROUND(DTL.LiabilityEquipCPM * P.ParticBLAC, 2), 0) AS LiabilityEquipNUTR, 
                         ISNULL(ROUND(DTL.LiabilityInsurCPM * P.ParticBLAC, 2), 0) AS LiabilityInsurNUTR, ISNULL(DTL.VehicleInsuranceNUTR, 0) AS VehicleInsuranceNUTR, ISNULL(DTL.LicenseFeeNUTR, 0) AS LicenseFeeNUTR, 
                         ISNULL(DTL.OfficeExpenseNUTR, 0) AS OfficeExpenseNUTR, ISNULL(DTL.OfficeExpenseDIST, 0) AS OfficeExpenseDIST, ISNULL(DTL.PrintingNUTR, 0) AS PrintingNUTR, ISNULL(DTL.PrintingDIST, 0) 
                         AS PrintingDIST, ISNULL(P.GrandTotalPartic * (DTL.YTDActualPaper + ROUND(DTL.YTDActualPaper * DTL.PctIncreasePaper * 0.0001, 4)), 0) AS PaperNUTR, ISNULL(DTL.PerformanceBondNUTR, 0) 
                         AS PerformanceBondNUTR, ISNULL(DTL.POSNUTR, 0) AS POSNUTR, ISNULL(DTL.POSDIST, 0) AS POSDIST, ISNULL(DTL.PostageNUTR, 0) AS PostageNUTR, ISNULL(DTL.PostageDIST, 0) AS PostageDIST, 
                         ISNULL(DTL.AreaEnhancementNUTR, 0) AS AreaEnhancementNUTR, ISNULL(DTL.NutrisliceNUTR, 0) AS NutrisliceNUTR, ISNULL(DTL.SmallwareNUTR, 0) AS SmallwareNUTR, ISNULL(DTL.SmallwareDIST, 0) 
                         AS SmallwareDIST, ISNULL(DTL.StaffTrainingNUTR, 0) AS StaffTrainingNUTR, ISNULL(FSD.FSDExpEdu, 0) AS ContinuingEdNUTR, ISNULL(ROUND(DTL.TrainingCPM * P.ParticBLAC, 2), 0) AS TrainingNUTR, 
                         ISNULL(DTL.StorageFoodNUTR, 0) AS StorageFoodNUTR, ISNULL(DTL.TaxesNUTR, 0) AS TaxesNUTR, ISNULL(DTL.CommNUTR, 0) AS CommNUTR, ISNULL(DTL.TicketsNUTR, 0) AS TicketsNUTR, 
                         ISNULL(DTL.PestControlNUTR, 0) AS PestControlNUTR, ISNULL(DTL.PestControlDIST, 0) AS PestControlDIST, ISNULL(DTL.TrashNUTR, 0) AS TrashNUTR, ISNULL(DTL.TrashDIST, 0) AS TrashDIST, 
                         ISNULL(DTL.LaundryNUTR, 0) AS LaundryNUTR, ISNULL(DTL.LaundryDIST, 0) AS LaundryDIST, ISNULL(DTL.UniformsNUTR, 0) AS UniformsNUTR, ISNULL(DTL.UniformsDIST, 0) AS UniformsDIST, 
                         ISNULL(DTL.VendingRentalNUTR, 0) AS VendingRentalNUTR, ISNULL(ROUND(DTL.WellnessFee * P.ParticBLAC, 2), 0) AS WellnessFeeNUTR, ISNULL(DTL.IndirectLaborSFADIST, 0) AS IndirectLaborSFADIST, 
                         ISNULL(DTL.UtilsSFADIST, 0) AS UtilsSFADIST, ISNULL(DTL.CateringNUTR, 0) AS CateringNUTR, ISNULL(CACFP.CACFPAdminFeeTotal, 0) AS CACFPAdmin, ISNULL(DTL.FSMCRFPNUTR_1, 0) AS FSMCRFPNUTR_1,
                          ISNULL(DTL.FSMCRFPNUTR_2, 0) AS FSMCRFPNUTR_2, ISNULL(DTL.FSMCRFPNUTR_3, 0) AS FSMCRFPNUTR_3, ISNULL(DTL.FSMCRFPNUTR_4, 0) AS FSMCRFPNUTR_4, ISNULL(DTL.FSMCRFPNUTR_5, 0) 
                         AS FSMCRFPNUTR_5, ISNULL(DTL.SFARFPDIST_1, 0) AS SFARFPDIST_1, ISNULL(DTL.SFARFPDIST_2, 0) AS SFARFPDIST_2, ISNULL(DTL.SFARFPDIST_3, 0) AS SFARFPDIST_3, ISNULL(DTL.LinensNUTR, 0) 
                         AS LinensNUTR, ISNULL(DTL.LinensDIST, 0) AS LinensDIST, ISNULL(HDR.DepreciationClient, 0) AS DepreciationClient, ISNULL(HDR.DepreciationTNGCurr, 0) AS DepreciationTNGCurr, 
                         0.00 AS NUTRPayrollRelated, ISNULL(DTL.SFARFPDesc_1, '') AS SFARFPDesc_1, ISNULL(DTL.SFARFPDesc_2, '') AS SFARFPDesc_2, ISNULL(DTL.SFARFPDesc_3, '') AS SFARFPDesc_3, 
                         ISNULL(DTL.FSMCRFPDesc_1, '') AS FSMCRFPDesc_1, ISNULL(DTL.FSMCRFPDesc_2, '') AS FSMCRFPDesc_2, ISNULL(DTL.FSMCRFPDesc_3, '') AS FSMCRFPDesc_3, ISNULL(DTL.FSMCRFPDesc_4, '') 
                         AS FSMCRFPDesc_4, ISNULL(DTL.FSMCRFPDesc_5, '') AS FSMCRFPDesc_5, ISNULL(DTL.ElectronicInvFeeNUTR, 0) AS ElectronicInvFeeNUTR, ISNULL(DTL.AccountingDIST, 0) + ISNULL(DTL.CarTruckRentDIST, 
                         0) + ISNULL(DTL.CarTruckMileageDIST, 0) + ISNULL(DTL.CarTruckMaintDIST, 0) + ISNULL(DTL.ChinaSilvGlassDIST, 0) + ISNULL(DTL.CleaningJanitorDIST, 0) + ISNULL(DTL.SoftwareUpgradeDIST, 0) 
                         + ISNULL(DTL.TechnologyFeeDIST, 0) + ISNULL(DTL.EmpRecruitmentDIST, 0) + ISNULL(DTL.EmpReplacementDIST, 0) + ISNULL(DTL.IndirectLaborDIST, 0) + ISNULL(DTL.EquipMaintDIST, 0) 
                         + ISNULL(DTL.EquipRepairDIST, 0) + ISNULL(DTL.EquipReplaceDIST, 0) + ISNULL(DTL.OfficeExpenseDIST, 0) + ISNULL(DTL.PrintingDIST, 0) + ISNULL(DTL.POSDIST, 0) + ISNULL(DTL.PostageDIST, 0) 
                         + ISNULL(DTL.SmallwareDIST, 0) + ISNULL(DTL.PestControlDIST, 0) + ISNULL(DTL.TrashDIST, 0) + ISNULL(DTL.LaundryDIST, 0) + ISNULL(DTL.UniformsDIST, 0) + ISNULL(DTL.IndirectLaborSFADIST, 0) 
                         + ISNULL(DTL.UtilsSFADIST, 0) + ISNULL(DTL.SFARFPDIST_1, 0) + ISNULL(DTL.SFARFPDIST_2, 0) + ISNULL(DTL.SFARFPDIST_3, 0) + ISNULL(DTL.LinensDIST, 0) + ISNULL(HDR.DepreciationClient, 0) 
                         AS DistrictTotal, ISNULL(P.ParticBLAC * FC.InvPrepProcCPM, 0) + ISNULL(DTL.ElectronicInvFeeNUTR, 0) + ISNULL(DTL.BackgroundCheckNUTR, 0) + ISNULL(DTL.CarTruckRentNUTR, 0) 
                         + ISNULL(FSD.FSDExpTravel, 0) + ISNULL(DTL.CarTruckMaintNUTR, 0) + ISNULL(DTL.ChinaSilvGlassNUTR, 0) 
                         + ISNULL(ROUND(P.GrandTotalPartic * (DTL.YtdActualCleaning + ROUND(DTL.YtdActualCleaning * DTL.PctIncreaseCleaning * 0.0001, 4)), 2), 0) + ISNULL(DTL.SoftwareUpgradeNUTR, 0) 
                         + ISNULL(ROUND(DTL.TechnologyFeeCPM * P.ParticBLAC, 2), 0) + ISNULL(DTL.CourierNUTR, 0) + ISNULL(FSD.FSDExpDues + FSD.FSDExpSubs, 0) + ISNULL(DTL.EmployeeMealsNUTR, 0) 
                         + ISNULL(ROUND(DTL.EmpRecruitmentCPM * P.ParticBLAC, 2), 0) + ISNULL(ROUND(DTL.EmpReplacementCPM * P.ParticBLAC, 2), 0) + ISNULL(DTL.IndirectLaborNUTR, 0) + ISNULL(DTL.BuybackInvestNUTR, 0) 
                         + ISNULL(DTL.RentalsNUTR, 0) + ISNULL(DTL.EquipMaintNUTR, 0) + ISNULL(DTL.EquipRepairNUTR, 0) + ISNULL(DTL.EquipReplaceNUTR, 0) + ISNULL(DTL.FreightDelivNUTR, 0) 
                         + ISNULL(ROUND(DTL.LiabilityEquipCPM * P.ParticBLAC, 2), 0) + ISNULL(ROUND(DTL.LiabilityInsurCPM * P.ParticBLAC, 2), 0) + ISNULL(DTL.VehicleInsuranceNUTR, 0) + ISNULL(DTL.LicenseFeeNUTR, 0) 
                         + ISNULL(DTL.OfficeExpenseNUTR, 0) + ISNULL(DTL.PrintingNUTR, 0) + ISNULL(P.GrandTotalPartic * (DTL.YTDActualPaper + ROUND(DTL.YTDActualPaper * DTL.PctIncreasePaper * 0.0001, 4)), 0) 
                         + ISNULL(DTL.PerformanceBondNUTR, 0) + ISNULL(DTL.POSNUTR, 0) + ISNULL(DTL.PostageNUTR, 0) + ISNULL(DTL.AreaEnhancementNUTR, 0) + ISNULL(DTL.NutrisliceNUTR, 0) + ISNULL(DTL.SmallwareNUTR, 
                         0) + ISNULL(DTL.StaffTrainingNUTR, 0) + ISNULL(FSD.FSDExpEdu, 0) + ISNULL(ROUND(DTL.TrainingCPM * P.ParticBLAC, 2), 0) + ISNULL(DTL.StorageFoodNUTR, 0) + ISNULL(DTL.TaxesNUTR, 0) 
                         + ISNULL(DTL.CommNUTR, 0) + ISNULL(DTL.TicketsNUTR, 0) + ISNULL(DTL.PestControlNUTR, 0) + ISNULL(DTL.TrashNUTR, 0) + ISNULL(DTL.LaundryNUTR, 0) + ISNULL(DTL.UniformsNUTR, 0) 
                         + ISNULL(DTL.VendingRentalNUTR, 0) + ISNULL(ROUND(DTL.WellnessFee * P.ParticBLAC, 2), 0) + ISNULL(DTL.CateringNUTR, 0) + ISNULL(CACFP.CACFPAdminFeeTotal, 0) + ISNULL(DTL.FSMCRFPNUTR_1, 0) 
                         + ISNULL(DTL.FSMCRFPNUTR_2, 0) + ISNULL(DTL.FSMCRFPNUTR_3, 0) + ISNULL(DTL.FSMCRFPNUTR_4, 0) + ISNULL(DTL.FSMCRFPNUTR_5, 0) + ISNULL(DTL.LinensNUTR, 0) 
                         + ISNULL(HDR.DepreciationTNGCurr, 0) + ISNULL(DTL.ElectronicInvFeeNUTR, 0) AS NUTRTotal
FROM            dbo.BDGT_BudgetHDR LEFT OUTER JOIN
                         dbo.BDGT_NonFoodCostDtl AS DTL ON dbo.BDGT_BudgetHDR.BidNumber = DTL.BidNumber AND dbo.BDGT_BudgetHDR.BudgetYear = DTL.BudgetYear AND 
                         dbo.BDGT_BudgetHDR.BudgetVersion = DTL.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_NonFoodCostHDR AS HDR ON DTL.BidNumber = HDR.BidNumber AND DTL.BudgetYear = HDR.BudgetYear AND DTL.BudgetVersion = HDR.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_Participation AS P ON DTL.BidNumber = P.BidNumber AND DTL.BudgetYear = P.BudgetYear AND DTL.BudgetVersion = P.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_LaborFSDTotals AS FSD ON DTL.BidNumber = FSD.BidNumber AND DTL.BudgetYear = FSD.BudgetYear AND DTL.BudgetVersion = FSD.BudgetVersion LEFT OUTER JOIN
                         dbo.BDGT_FoodCost AS FC ON DTL.BidNumber = FC.BidNumber AND DTL.BudgetYear = FC.BudgetYear AND DTL.BudgetVersion = FC.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_LaborNUTRTotals AS NUTR ON DTL.BidNumber = NUTR.BidNumber AND DTL.BudgetYear = NUTR.BudgetYear AND DTL.BudgetVersion = NUTR.BudgetVersion LEFT OUTER JOIN
                         dbo.vwBDGT_CACFP AS CACFP ON DTL.BidNumber = CACFP.BidNumber AND DTL.BudgetYear = CACFP.BudgetYear AND DTL.BudgetVersion = CACFP.BudgetVersion
GO

