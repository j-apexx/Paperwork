USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[SPFNInvoiceLines]    Script Date: 11/26/2019 1:43:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SPFNInvoiceLines](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[InvoiceNumber] [nvarchar](15) NOT NULL,
	[LineNumber] [int] NOT NULL,
	[MiscDescription] [nvarchar](75) NOT NULL,
	[MiscAmount] [numeric](11, 2) NOT NULL,
 CONSTRAINT [PK_SPFNInvoiceLines] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[InvoiceNumber] ASC,
	[LineNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

