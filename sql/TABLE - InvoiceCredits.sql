USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[InvoiceCredits]    Script Date: 12/5/2019 2:17:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvoiceCredits](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[CreditDate] [date] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[CreditAmount] [numeric](11, 2) NOT NULL,
	[Counter] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_InvoiceCredits] PRIMARY KEY CLUSTERED 
(
	[Counter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[InvoiceCredits] ADD  CONSTRAINT [DF_InvoiceCredits_Description]  DEFAULT ('') FOR [Description]
GO

ALTER TABLE [dbo].[InvoiceCredits] ADD  CONSTRAINT [DF_InvoiceCredits_CreditAmount]  DEFAULT ((0)) FOR [CreditAmount]
GO

