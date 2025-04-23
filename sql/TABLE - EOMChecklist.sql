USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[EOMChecklist]    Script Date: 12/9/2021 11:34:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EOMChecklist](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[YearMonth] [nvarchar](7) NOT NULL,
	[InventoryDate] [date] NOT NULL,
	[CasesBB] [numeric](11, 2) NOT NULL,
	[CasesDOD] [numeric](11, 2) NOT NULL,
	[CasesK12] [numeric](11, 2) NOT NULL,
	[CasesPL] [numeric](11, 2) NOT NULL,
	[NOIAmount] [numeric](11, 2) NOT NULL,
	[CBPurchTotalEntered] [bit] NOT NULL,
	[CBUSDAComplete] [bit] NOT NULL,
	[PurchTotal] [numeric](11, 2) NOT NULL,
	[CBSpecFuncLedger] [bit] NOT NULL,
	[CBIndirectCosts] [bit] NOT NULL,
	[CBReviewedTransOCosts] [bit] NOT NULL,
	[POSBalance] [numeric](11, 2) NOT NULL,
	[CBSOC1] [bit] NOT NULL,
	[CBOnlinePayments] [bit] NOT NULL,
	[CBReviewBdgtReport] [bit] NOT NULL,
	[CBNegAdjusts] [bit] NOT NULL,
	[NegAdjustsComment] [nvarchar](max) NOT NULL,
	[CBVendDepOSales] [bit] NOT NULL,
	[CBEditChecksMatch] [bit] NOT NULL,
	[CBOpDaysVerified] [bit] NOT NULL,
	[CBSchoolInfoUpdated] [bit] NOT NULL,
	[CBDistPayrollHrsEntered] [bit] NOT NULL,
	[CBHealthInsCaptured] [bit] NOT NULL,
	[FoodCostComment] [nvarchar](max) NOT NULL,
	[PaperCleanComment] [nvarchar](max) NOT NULL,
	[CBUSDAInvMatches] [bit] NOT NULL,
	[ReviewedUsageComment] [nvarchar](max) NOT NULL,
	[TargetPurchasedComment] [nvarchar](max) NOT NULL,
	[EOMReportBatchEmailed] [bit] NOT NULL,
	[EOMReportBatchDateTime] [datetime] NOT NULL,
	[LastEditedUser] [nchar](30) NOT NULL,
	[LastEditedDate] [datetime] NOT NULL,
	[CBExtInventoryPrinted] [bit] NOT NULL,
	[HoursWagesComment] [nvarchar](max) NOT NULL,
	[BottomLineComment] [nvarchar](max) NOT NULL,
	[CasesNA] [numeric](11, 2) NOT NULL,
 CONSTRAINT [PK_EOMChecklist] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[YearMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_InventoryDate]  DEFAULT ('1/1/1900') FOR [InventoryDate]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CasesBB]  DEFAULT ((0)) FOR [CasesBB]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CasesDOD]  DEFAULT ((0)) FOR [CasesDOD]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CasesK12]  DEFAULT ((0)) FOR [CasesK12]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CasesPL]  DEFAULT ((0)) FOR [CasesPL]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_NOIAmount]  DEFAULT ((0)) FOR [NOIAmount]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_Table_1_PurchTotalEntered]  DEFAULT ((0)) FOR [CBPurchTotalEntered]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBUSDAComplete]  DEFAULT ((0)) FOR [CBUSDAComplete]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_PurchTotal]  DEFAULT ((0)) FOR [PurchTotal]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBSpecFuncLedger]  DEFAULT ((0)) FOR [CBSpecFuncLedger]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBIndirectCosts]  DEFAULT ((0)) FOR [CBIndirectCosts]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBReviewedTransOCosts]  DEFAULT ((0)) FOR [CBReviewedTransOCosts]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_POSBalance]  DEFAULT ((0)) FOR [POSBalance]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBSOC1]  DEFAULT ((0)) FOR [CBSOC1]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBOnlinePayments]  DEFAULT ((0)) FOR [CBOnlinePayments]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBReviewBdgtReport]  DEFAULT ((0)) FOR [CBReviewBdgtReport]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBNegAdjusts]  DEFAULT ((0)) FOR [CBNegAdjusts]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_NegAdjustsComment]  DEFAULT ('') FOR [NegAdjustsComment]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBVendDepOSales]  DEFAULT ((0)) FOR [CBVendDepOSales]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBEditChecksMatch]  DEFAULT ((0)) FOR [CBEditChecksMatch]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBOpDaysVerified]  DEFAULT ((0)) FOR [CBOpDaysVerified]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBSchoolInfoUpdated]  DEFAULT ((0)) FOR [CBSchoolInfoUpdated]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBDistPayrollHrsEntered]  DEFAULT ((0)) FOR [CBDistPayrollHrsEntered]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBHealthInsCaptured]  DEFAULT ((0)) FOR [CBHealthInsCaptured]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_FoodCostComment]  DEFAULT ('') FOR [FoodCostComment]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_PaperCleanComment]  DEFAULT ('') FOR [PaperCleanComment]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBUSDAInvMatches]  DEFAULT ((0)) FOR [CBUSDAInvMatches]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_ReviewedUsageComment]  DEFAULT ('') FOR [ReviewedUsageComment]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_TargetPurchasedComment]  DEFAULT ('') FOR [TargetPurchasedComment]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_EOMReportBatchEmailed]  DEFAULT ((0)) FOR [EOMReportBatchEmailed]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_EOMReportBatchDateTime]  DEFAULT (((1)/(1))/(1900)) FOR [EOMReportBatchDateTime]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_LastEditedUser]  DEFAULT ('') FOR [LastEditedUser]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_LastEditedDate]  DEFAULT (((1)/(1))/(1900)) FOR [LastEditedDate]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CBExtInventoryPrinted]  DEFAULT ((0)) FOR [CBExtInventoryPrinted]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_HoursWagesComment]  DEFAULT ('') FOR [HoursWagesComment]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_BottomLineComment]  DEFAULT ('') FOR [BottomLineComment]
GO

ALTER TABLE [dbo].[EOMChecklist] ADD  CONSTRAINT [DF_EOMChecklist_CasesNA]  DEFAULT ((0)) FOR [CasesNA]
GO

