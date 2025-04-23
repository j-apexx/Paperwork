USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[CPMInvoiceHDR]    Script Date: 6/5/2020 8:07:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CPMInvoiceHDR](
	[InvoiceNumber] [int] NOT NULL,
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[PeriodStart] [date] NOT NULL,
	[PeriodEnd] [date] NOT NULL,
	[BreakfastTotal] [int] NOT NULL,
	[LunchTotal] [int] NOT NULL,
	[CPMRate] [numeric](11, 4) NOT NULL,
	[AlaSales] [numeric](11, 2) NOT NULL,
	[ACEquivRate] [numeric](11, 4) NOT NULL,
	[SpecialFunctionSales] [numeric](11, 2) NOT NULL,
	[USDACommUsage] [numeric](11, 2) NOT NULL,
	[USDACommRcvd] [numeric](11, 2) NOT NULL,
	[DateCreated] [date] NOT NULL,
 CONSTRAINT [PK_CPMInvoicesHDR] PRIMARY KEY CLUSTERED 
(
	[InvoiceNumber] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CPMInvoiceHDR] ADD  CONSTRAINT [DF_CPMInvoicesHDR_BreakfastTotal]  DEFAULT ((0)) FOR [BreakfastTotal]
GO

ALTER TABLE [dbo].[CPMInvoiceHDR] ADD  CONSTRAINT [DF_CPMInvoicesHDR_LunchTotal]  DEFAULT ((0)) FOR [LunchTotal]
GO

ALTER TABLE [dbo].[CPMInvoiceHDR] ADD  CONSTRAINT [DF_CPMInvoicesHDR_CPMRate]  DEFAULT ((0)) FOR [CPMRate]
GO

ALTER TABLE [dbo].[CPMInvoiceHDR] ADD  CONSTRAINT [DF_CPMInvoicesHDR_AlaSales]  DEFAULT ((0)) FOR [AlaSales]
GO

ALTER TABLE [dbo].[CPMInvoiceHDR] ADD  CONSTRAINT [DF_CPMInvoicesHDR_ACEquivRate]  DEFAULT ((0)) FOR [ACEquivRate]
GO

ALTER TABLE [dbo].[CPMInvoiceHDR] ADD  CONSTRAINT [DF_CPMInvoicesHDR_SpecialFunctionSales]  DEFAULT ((0)) FOR [SpecialFunctionSales]
GO

ALTER TABLE [dbo].[CPMInvoiceHDR] ADD  CONSTRAINT [DF_CPMInvoicesHDR_USDACommUsage]  DEFAULT ((0)) FOR [USDACommUsage]
GO

ALTER TABLE [dbo].[CPMInvoiceHDR] ADD  CONSTRAINT [DF_CPMInvoicesHDR_USDACommRcvd]  DEFAULT ((0)) FOR [USDACommRcvd]
GO

