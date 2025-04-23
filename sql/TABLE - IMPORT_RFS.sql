USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[IMPORT_RFS]    Script Date: 10/30/2019 10:52:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[IMPORT_RFS](
	[InvoiceDate] [datetime] NOT NULL,
	[PostDate] [datetime] NOT NULL,
	[InvNum] [nvarchar](50) NOT NULL,
	[RFSCustNum] [int] NOT NULL,
	[USDAFlag] [smallint] NOT NULL,
	[InvTotal] [numeric](11, 2) NOT NULL,
	[Food] [numeric](11, 2) NOT NULL,
	[Paper] [numeric](11, 2) NOT NULL,
	[Clean] [numeric](11, 2) NOT NULL,
	[Other] [numeric](11, 2) NOT NULL,
	[Promo] [numeric](11, 2) NOT NULL,
	[NOI] [numeric](11, 2) NOT NULL,
	[RFSDivision] [nvarchar](50) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[SalesTax] [numeric](11, 2) NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
 CONSTRAINT [PK_IMPORT_RFS] PRIMARY KEY CLUSTERED 
(
	[InvNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[IMPORT_RFS] ADD  CONSTRAINT [DF_IMPORT_RFS_SchoolNum]  DEFAULT ((0)) FOR [SchoolNum]
GO

