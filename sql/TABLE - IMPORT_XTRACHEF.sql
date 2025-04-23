USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[IMPORT_RFS]    Script Date: 5/21/2021 11:56:02 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[IMPORT_XTRACHEF](
	[InvoiceDate] [datetime] NOT NULL,
	[PostDate] [datetime] NOT NULL,
	[InvNum] [nvarchar](50) NOT NULL,
	[InvTotal] [numeric](11, 2) NOT NULL,
	[Food] [numeric](11, 2) NOT NULL,
	[Paper] [numeric](11, 2) NOT NULL,
	[Clean] [numeric](11, 2) NOT NULL,
	[Other] [numeric](11, 2) NOT NULL,
	[Milk] [numeric](11, 2) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
	[VendorID] [nvarchar](15) NOT NULL,
 CONSTRAINT [PK_IMPORT_XTRACHEF] PRIMARY KEY CLUSTERED 
(
	[InvNum] ASC, [DistrictID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[IMPORT_XTRACHEF] ADD  CONSTRAINT [DF_IMPORT_XTRACHEF_SchoolNum]  DEFAULT ((0)) FOR [SchoolNum]
GO


