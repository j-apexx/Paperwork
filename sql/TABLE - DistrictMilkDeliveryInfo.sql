USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[DistrictMilkDeliveryInfo]    Script Date: 3/18/2020 12:42:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DistrictMilkDeliveryInfo](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
	[MMBArea] [nvarchar](50) NOT NULL,
	[Monday] [tinyint] NOT NULL,
	[Tuesday] [tinyint] NOT NULL,
	[Wednesday] [tinyint] NOT NULL,
	[Thursday] [tinyint] NOT NULL,
	[Friday] [tinyint] NOT NULL,
	[JuiceCases] [tinyint] NOT NULL,
	[TeaCases] [tinyint] NOT NULL,
	[WalkInCooler] [bit] NOT NULL,
	[EightCaseCoolers] [tinyint] NOT NULL,
	[TwelveCaseCoolers] [tinyint] NOT NULL,
	[SixteenCaseCoolers] [tinyint] NOT NULL,
	[PALicense] [nvarchar](25) NOT NULL,
	[VendorID] [nvarchar](15) NOT NULL,
	[MMBSheet] [bit] NOT NULL,
	[AverageCaseCount] [numeric](11, 2) NOT NULL,
	[MilkDiscounts] [bit] NOT NULL,
	[OtherDiscounts] [bit] NOT NULL,
	[EquipLeases] [bit] NOT NULL,
	[EquipOwner] [nvarchar](50) NOT NULL,
	[LeasePayments] [numeric](11, 2) NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_DistrictMilkDeliveryInfo] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_MMBArea]  DEFAULT ('') FOR [MMBArea]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_Monday]  DEFAULT ((0)) FOR [Monday]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_Tuesday]  DEFAULT ((0)) FOR [Tuesday]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_Wednesday]  DEFAULT ((0)) FOR [Wednesday]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_Thursday]  DEFAULT ((0)) FOR [Thursday]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_Friday]  DEFAULT ((0)) FOR [Friday]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_JuiceCases]  DEFAULT ((0)) FOR [JuiceCases]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_TeaCases]  DEFAULT ((0)) FOR [TeaCases]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_WalkInCooler]  DEFAULT ((0)) FOR [WalkInCooler]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_EightCaseCoolers]  DEFAULT ((0)) FOR [EightCaseCoolers]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_TwelveCaseCoolers]  DEFAULT ((0)) FOR [TwelveCaseCoolers]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_SixteenCaseCoolers]  DEFAULT ((0)) FOR [SixteenCaseCoolers]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_PALicense]  DEFAULT ('') FOR [PALicense]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_VendorID]  DEFAULT ('') FOR [VendorID]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_MMBSheet]  DEFAULT ((0)) FOR [MMBSheet]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_AverageCaseCount]  DEFAULT ((0)) FOR [AverageCaseCount]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_MilkDiscounts]  DEFAULT ((0)) FOR [MilkDiscounts]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_OtherDiscounts]  DEFAULT ((0)) FOR [OtherDiscounts]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_EquipLeases]  DEFAULT ((0)) FOR [EquipLeases]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_EquipOwner]  DEFAULT ('') FOR [EquipOwner]
GO

ALTER TABLE [dbo].[DistrictMilkDeliveryInfo] ADD  CONSTRAINT [DF_DistrictMilkDeliveryInfo_LeasePayments]  DEFAULT ((0)) FOR [LeasePayments]
GO

