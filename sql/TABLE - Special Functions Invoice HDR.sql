USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[SPFNInvoiceHDR]    Script Date: 11/26/2019 1:43:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SPFNInvoiceHDR](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[InvoiceNumber] [nvarchar](15) NOT NULL,
	[CreatedDate] [date] NOT NULL,
	[CreatedUser] [nvarchar](30) NOT NULL,
	[Locked] [bit] NOT NULL,
	[OrgBilled] [nvarchar](50) NOT NULL,
	[OrgContact] [nvarchar](50) NOT NULL,
	[FunctionDate] [date] NOT NULL,
	[BillingDate] [date] NOT NULL,
	[OrgRemitTo] [nvarchar](50) NOT NULL,
	[SpecialFunctionDesc] [nvarchar](75) NOT NULL,
	[FoodCost] [numeric](11, 2) NOT NULL,
	[PaperCleanSuppl] [numeric](11, 2) NOT NULL,
	[Mileage] [numeric](11, 2) NOT NULL,
	[FSDCosts] [numeric](11, 2) NOT NULL,
	[TNGLabor] [numeric](11, 2) NOT NULL,
	[TNGFringePct] [numeric](11, 4) NOT NULL,
	[DistrictLabor] [numeric](11, 2) NOT NULL,
	[DistrictFringePct] [numeric](11, 4) NOT NULL,
	[MarkupType] [tinyint] NOT NULL,
	[MarkupDlr] [numeric](11, 2) NOT NULL,
	[MarkupPct] [numeric](11, 4) NOT NULL,
	[InvoiceNotes] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_SPFNInvoiceHDR] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_Locked]  DEFAULT ((0)) FOR [Locked]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_OrgBilled]  DEFAULT ('') FOR [OrgBilled]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_OrgContact]  DEFAULT ('') FOR [OrgContact]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_FunctionDate]  DEFAULT ('1/1/1900') FOR [FunctionDate]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_BillingDate]  DEFAULT ('1/1/1900') FOR [BillingDate]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_OrgRemitTo]  DEFAULT ('') FOR [OrgRemitTo]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_SpecialFunctionDesc]  DEFAULT ('') FOR [SpecialFunctionDesc]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_FoodCost]  DEFAULT ((0)) FOR [FoodCost]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_PaperCleanSuppl]  DEFAULT ((0)) FOR [PaperCleanSuppl]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_Mileage]  DEFAULT ((0)) FOR [Mileage]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_FSDCosts]  DEFAULT ((0)) FOR [FSDCosts]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_TNGLabor]  DEFAULT ((0)) FOR [TNGLabor]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_TNGFringePct]  DEFAULT ((0)) FOR [TNGFringePct]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_DistrictLabor]  DEFAULT ((0)) FOR [DistrictLabor]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_DistrictFringePct]  DEFAULT ((0)) FOR [DistrictFringePct]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_MarkupType]  DEFAULT ((0)) FOR [MarkupType]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_MarkupDlr]  DEFAULT ((0)) FOR [MarkupDlr]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_MarkupPct]  DEFAULT ((0)) FOR [MarkupPct]
GO

ALTER TABLE [dbo].[SPFNInvoiceHDR] ADD  CONSTRAINT [DF_SPFNInvoiceHDR_InvoiceNotes]  DEFAULT ('') FOR [InvoiceNotes]
GO

