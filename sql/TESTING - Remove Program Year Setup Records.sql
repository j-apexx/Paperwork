--Cleanup record for use after testing

DECLARE @NewProgramYear varchar(15)

SET @NewProgramYear='2020-2021'

DELETE [ProgramYear] WHERE ProgramYear=@NewProgramYear
DELETE [TaxRates] WHERE ProgramYear=@NewProgramYear
DELETE [OtherPurchaseCategories] WHERE ProgramYear=@NewProgramYear
DELETE [DistrictAllowedOtherCats] WHERE ProgramYear=@NewProgramYear
DELETE DistrictMilkDeliveryInfo WHERE ProgramYear=@NewProgramYear
DELETE DistrictPaycodes WHERE ProgramYear=@NewProgramYear
DELETE DistrictYearInfo WHERE ProgramYear=@NewProgramYear
DELETE DistrictYearContacts WHERE ProgramYear=@NewProgramYear
DELETE DistrictSchoolYearSetup WHERE ProgramYear=@NewProgramYear
DELETE [OtherSalesCats] WHERE ProgramYear=@NewProgramYear
DELETE [Purchases] WHERE ProgramYear=@NewProgramYear