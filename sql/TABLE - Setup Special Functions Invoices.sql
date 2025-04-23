USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[SPFNInvoiceSetup]    Script Date: 12/23/2019 10:08:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SPFNInvoiceSetup](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[InvoicePrefix] [nvarchar](5) NOT NULL,
	[NextInvoiceNumber] [int] NOT NULL,
 CONSTRAINT [PK_SPFNInvoiceSetup] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

