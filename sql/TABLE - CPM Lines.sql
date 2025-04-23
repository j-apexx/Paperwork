USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[CPMInvoiceLines]    Script Date: 6/5/2020 8:07:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CPMInvoiceLines](
	[InvoiceNumber] [int] NOT NULL,
	[InvoiceLineNumber] [int] NOT NULL,
	[OtherSalesAmount] [numeric](11, 2) NOT NULL,
	[OtherSalesDesc] [nvarchar](50) NOT NULL,
	[OtherSalesDate] [date] NOT NULL,
 CONSTRAINT [PK_CPMInvoiceLines] PRIMARY KEY CLUSTERED 
(
	[InvoiceNumber] DESC,
	[InvoiceLineNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CPMInvoiceLines] ADD  CONSTRAINT [DF_CPMInvoiceLines_OtherSalesAmount]  DEFAULT ((0)) FOR [OtherSalesAmount]
GO

ALTER TABLE [dbo].[CPMInvoiceLines] ADD  CONSTRAINT [DF_CPMInvoiceLines_OtherSalesDesc]  DEFAULT ('') FOR [OtherSalesDesc]
GO

