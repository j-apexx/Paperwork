USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[DistrictSchoolYearSetup]    Script Date: 10/12/2020 12:46:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[DistrictSchoolYearSetup]
ALTER COLUMN [BfastPaidRate] [numeric](11, 4) NOT NULL

ALTER TABLE [dbo].[DistrictSchoolYearSetup]
ALTER COLUMN [BfastRedRate] [numeric](11, 4) NOT NULL

ALTER TABLE [dbo].[DistrictSchoolYearSetup]
ALTER COLUMN [BfastFreeRate] [numeric](11, 4) NOT NULL

ALTER TABLE [dbo].[DistrictSchoolYearSetup]
ALTER COLUMN [LunchPaidRate] [numeric](11, 4) NOT NULL

ALTER TABLE [dbo].[DistrictSchoolYearSetup]
ALTER COLUMN [LunchRedRate] [numeric](11, 4) NOT NULL

ALTER TABLE [dbo].[DistrictSchoolYearSetup]
ALTER COLUMN [LunchFreeRate] [numeric](11, 4) NOT NULL

ALTER TABLE [dbo].[DistrictSchoolYearSetup]
ALTER COLUMN [OSnackPaidRate] [numeric](11, 4) NOT NULL

ALTER TABLE [dbo].[DistrictSchoolYearSetup]
ALTER COLUMN [OSnackRedRate] [numeric](11, 4) NOT NULL

ALTER TABLE [dbo].[DistrictSchoolYearSetup]
ALTER COLUMN [OSnackFreeRate] [numeric](11, 4) NOT NULL



