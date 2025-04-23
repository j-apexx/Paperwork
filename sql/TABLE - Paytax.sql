USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[PayTax]    Script Date: 12/5/2019 9:16:23 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PayTax](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[EndDate] [date] NOT NULL,
	[StartDate] [date] NOT NULL,
	[Gross] [numeric](11, 2) NOT NULL,
	[Hours] [numeric](11, 2) NOT NULL,
	[ProdHrs] [numeric](11, 2) NOT NULL,
	[NonProdHrs] [numeric](11, 2) NOT NULL,
	[SpecFunctionHrs] [numeric](11, 2) NOT NULL,
	[FUTA] [numeric](11, 2) NOT NULL,
	[Medicare] [numeric](11, 2) NOT NULL,
	[PASUI] [numeric](11, 2) NOT NULL,
	[OHSUI] [numeric](11, 2) NOT NULL,
	[SocSec] [numeric](11, 2) NOT NULL,
	[Uniforms] [numeric](11, 2) NOT NULL,
	[PAPitC] [numeric](11, 2) NOT NULL,
	[PayAccrual] [numeric](11, 2) NOT NULL,
	[TaxAccrual] [numeric](11, 2) NOT NULL,
 CONSTRAINT [PK_PayTax] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_Gross]  DEFAULT ((0)) FOR [Gross]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_Hours]  DEFAULT ((0)) FOR [Hours]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_ProdHrs]  DEFAULT ((0)) FOR [ProdHrs]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_NonProdHrs]  DEFAULT ((0)) FOR [NonProdHrs]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_SpecFunctionHrs]  DEFAULT ((0)) FOR [SpecFunctionHrs]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_FUTA]  DEFAULT ((0)) FOR [FUTA]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_Medicare]  DEFAULT ((0)) FOR [Medicare]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_PASUI]  DEFAULT ((0)) FOR [PASUI]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_OHSUI]  DEFAULT ((0)) FOR [OHSUI]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_SocSec]  DEFAULT ((0)) FOR [SocSec]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_Uniforms]  DEFAULT ((0)) FOR [Uniforms]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_PAPitC]  DEFAULT ((0)) FOR [PAPitC]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_PayAccrual]  DEFAULT ((0)) FOR [PayAccrual]
GO

ALTER TABLE [dbo].[PayTax] ADD  CONSTRAINT [DF_PayTax_TaxAccrual]  DEFAULT ((0)) FOR [TaxAccrual]
GO

