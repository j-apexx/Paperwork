USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[RecapEntry]    Script Date: 8/27/2019 9:48:08 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RecapEntry](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
	[RecapDate] [date] NOT NULL,
	[BreakfastDay] [tinyint] NOT NULL,
	[LunchDay] [tinyint] NOT NULL,
	[BPaidElem] [int] NOT NULL,
	[BPaidJr] [int] NOT NULL,
	[BPaidSr] [int] NOT NULL,
	[BReduced] [int] NOT NULL,
	[BFree] [int] NOT NULL,
	[BPaidLrn] [int] NOT NULL,
	[BReducedLrn] [int] NOT NULL,
	[BFreeLrn] [int] NOT NULL,
	[BStuAla] [numeric](11, 2) NOT NULL,
	[BAdultAla] [numeric](11, 2) NOT NULL,
	[BAdvanced] [numeric](11, 2) NOT NULL,
	[BRedeemed] [numeric](11, 2) NOT NULL,
	[BRepaid] [numeric](11, 2) NOT NULL,
	[BCharges] [numeric](11, 2) NOT NULL,
	[BDeposit] [numeric](11, 2) NOT NULL,
	[LPaidElem] [int] NOT NULL,
	[LPaidJr] [int] NOT NULL,
	[LPaidSr] [int] NOT NULL,
	[LTier1] [int] NOT NULL,
	[LTier2] [int] NOT NULL,
	[LReduced] [int] NOT NULL,
	[LReducedTier1] [int] NOT NULL,
	[LFree] [int] NOT NULL,
	[LFreeTier1] [int] NOT NULL,
	[LFreeTier2] [int] NOT NULL,
	[LPaidLrn] [int] NOT NULL,
	[LReducedLrn] [int] NOT NULL,
	[LFreeLrn] [int] NOT NULL,
	[LStuAla] [numeric](11, 2) NOT NULL,
	[LAdultAla] [numeric](11, 2) NOT NULL,
	[LOtherAla1] [numeric](11, 2) NOT NULL,
	[LVendAla] [numeric](11, 2) NOT NULL,
	[LAdvanced] [numeric](11, 2) NOT NULL,
	[LRedeemed] [numeric](11, 2) NOT NULL,
	[LRepaid] [numeric](11, 2) NOT NULL,
	[LCharges] [numeric](11, 2) NOT NULL,
	[OPaidSnack] [int] NOT NULL,
	[OReducedSnack] [int] NOT NULL,
	[OFreeSnack] [int] NOT NULL,
	[SpecialFunctionSale] [numeric](11, 2) NOT NULL,
	[SpecialFunctionColl] [numeric](11, 2) NOT NULL,
	[SpecialFunctionComm] [numeric](11, 2) NOT NULL,
	[USDASale] [numeric](11, 2) NOT NULL,
	[USDAColl] [numeric](11, 2) NOT NULL,
	[OtherSale1] [numeric](11, 2) NOT NULL,
	[OtherColl1] [numeric](11, 2) NOT NULL,
	[OtherSale2] [numeric](11, 2) NOT NULL,
	[OtherColl2] [numeric](11, 2) NOT NULL,
	[OnlineAdvance] [numeric](11, 2) NOT NULL,
	[Adjustment] [numeric](11, 2) NOT NULL,
	[Deposit] [numeric](11, 2) NOT NULL,
	[OtherSale1Cat] [smallint] NOT NULL,
	[OtherColl1Cat] [smallint] NOT NULL,
	[EditCheckComments] [ntext] NOT NULL,
	[RecapComments] [ntext] NOT NULL,
	[OtherSale2Cat] [smallint] NOT NULL,
	[OtherColl2Cat] [smallint] NOT NULL,
 CONSTRAINT [PK_RecapEntry] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] ASC,
	[DistrictID] ASC,
	[SchoolNum] ASC,
	[RecapDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BreakfastDay]  DEFAULT ((0)) FOR [BreakfastDay]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LunchDay]  DEFAULT ((0)) FOR [LunchDay]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BPaidElem]  DEFAULT ((0)) FOR [BPaidElem]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BPaidJr]  DEFAULT ((0)) FOR [BPaidJr]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BPaidSr]  DEFAULT ((0)) FOR [BPaidSr]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BReduced]  DEFAULT ((0)) FOR [BReduced]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BFree]  DEFAULT ((0)) FOR [BFree]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BPaidLrn]  DEFAULT ((0)) FOR [BPaidLrn]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BReducedLrn]  DEFAULT ((0)) FOR [BReducedLrn]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BFreeLrn]  DEFAULT ((0)) FOR [BFreeLrn]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BStuAla]  DEFAULT ((0)) FOR [BStuAla]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BAdultAla]  DEFAULT ((0)) FOR [BAdultAla]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BAdvanced]  DEFAULT ((0)) FOR [BAdvanced]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BRedeemed]  DEFAULT ((0)) FOR [BRedeemed]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BRepaid]  DEFAULT ((0)) FOR [BRepaid]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BCharges]  DEFAULT ((0)) FOR [BCharges]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_BDeposit]  DEFAULT ((0)) FOR [BDeposit]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LPaidElem]  DEFAULT ((0)) FOR [LPaidElem]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LPaidJr]  DEFAULT ((0)) FOR [LPaidJr]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LPaidSr]  DEFAULT ((0)) FOR [LPaidSr]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LTier1]  DEFAULT ((0)) FOR [LTier1]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LTier2]  DEFAULT ((0)) FOR [LTier2]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LReduced]  DEFAULT ((0)) FOR [LReduced]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LReducedTier1]  DEFAULT ((0)) FOR [LReducedTier1]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LFree]  DEFAULT ((0)) FOR [LFree]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LFreeTier1]  DEFAULT ((0)) FOR [LFreeTier1]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LFreeTier2]  DEFAULT ((0)) FOR [LFreeTier2]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LPaidLrn]  DEFAULT ((0)) FOR [LPaidLrn]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LReducedLrn]  DEFAULT ((0)) FOR [LReducedLrn]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LFreeLrn]  DEFAULT ((0)) FOR [LFreeLrn]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LStuAla]  DEFAULT ((0)) FOR [LStuAla]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LAdultAla]  DEFAULT ((0)) FOR [LAdultAla]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LOtherAla1]  DEFAULT ((0)) FOR [LOtherAla1]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LVendAla]  DEFAULT ((0)) FOR [LVendAla]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LAdvanced]  DEFAULT ((0)) FOR [LAdvanced]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LRedeemed]  DEFAULT ((0)) FOR [LRedeemed]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LRepaid]  DEFAULT ((0)) FOR [LRepaid]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_LCharges]  DEFAULT ((0)) FOR [LCharges]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OPaidSnack]  DEFAULT ((0)) FOR [OPaidSnack]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OReducedSnack]  DEFAULT ((0)) FOR [OReducedSnack]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OFreeSnack]  DEFAULT ((0)) FOR [OFreeSnack]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_SpecialFunctionSale]  DEFAULT ((0)) FOR [SpecialFunctionSale]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_SecialFunctionColl]  DEFAULT ((0)) FOR [SpecialFunctionColl]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_SpecialFunctionComm]  DEFAULT ((0)) FOR [SpecialFunctionComm]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_USDASale]  DEFAULT ((0)) FOR [USDASale]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_USDAColl]  DEFAULT ((0)) FOR [USDAColl]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OtherSale1]  DEFAULT ((0)) FOR [OtherSale1]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OtherColl1]  DEFAULT ((0)) FOR [OtherColl1]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OtherSale2]  DEFAULT ((0)) FOR [OtherSale2]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OtherColl2]  DEFAULT ((0)) FOR [OtherColl2]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OnlineAdvance]  DEFAULT ((0)) FOR [OnlineAdvance]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_Adjustment]  DEFAULT ((0)) FOR [Adjustment]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_Deposit]  DEFAULT ((0)) FOR [Deposit]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_O1SLNum]  DEFAULT ((0)) FOR [OtherSale1Cat]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_O2SLNum]  DEFAULT ((0)) FOR [OtherColl1Cat]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_EditCheckComments]  DEFAULT ('') FOR [EditCheckComments]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_RecapComments]  DEFAULT ('') FOR [RecapComments]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OtherSale2Cat]  DEFAULT ((0)) FOR [OtherSale2Cat]
GO

ALTER TABLE [dbo].[RecapEntry] ADD  CONSTRAINT [DF_RecapEntry_OtherColl2Cat]  DEFAULT ((0)) FOR [OtherColl2Cat]
GO

