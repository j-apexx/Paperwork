USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[DistrictSchoolYearSetup]    Script Date: 6/19/2019 7:46:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DistrictSchoolYearSetup](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
	[BfastElPrice] [numeric](11, 2) NOT NULL,
	[BfastJrPrice] [numeric](11, 2) NOT NULL,
	[BfastSrPrice] [numeric](11, 2) NOT NULL,
	[BfastRedPrice] [numeric](11, 2) NOT NULL,
	[LunchElPrice] [numeric](11, 2) NOT NULL,
	[LunchJrPrice] [numeric](11, 2) NOT NULL,
	[LunchSrPrice] [numeric](11, 2) NOT NULL,
	[LunchTier1Price] [numeric](11, 2) NOT NULL,
	[LunchTier2Price] [numeric](11, 2) NOT NULL,
	[LunchRedPrice] [numeric](11, 2) NOT NULL,
	[BfastPaidRate] [numeric](11, 2) NOT NULL,
	[BfastRedRate] [numeric](11, 2) NOT NULL,
	[BfastFreeRate] [numeric](11, 2) NOT NULL,
	[LunchPaidRate] [numeric](11, 2) NOT NULL,
	[LunchRedRate] [numeric](11, 2) NOT NULL,
	[LunchFreeRate] [numeric](11, 2) NOT NULL,
	[BfastIncentive] [numeric](11, 2) NOT NULL,
 CONSTRAINT [PK_DistrictSchoolYearSetup] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[SchoolNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

