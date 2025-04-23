USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[MMBTransactions]    Script Date: 10/24/2019 2:19:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MMBTransactions](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[ProductCode] [smallint] NOT NULL,
	[VendorID] [nvarchar](15) NOT NULL,
	[MMBDate] [date] NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [numeric](11, 4) NOT NULL,
	[MilkItem] [bit] NOT NULL,
	[Locked] [bit] NOT NULL,
 CONSTRAINT [PK_MMBTransactions] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[MMBDate] ASC,
	[SchoolNum] ASC,
	[ProductCode] ASC,
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MMBTransactions] ADD  CONSTRAINT [DF_MMBTransactions_MilkItem]  DEFAULT ((0)) FOR [MilkItem]
GO

ALTER TABLE [dbo].[MMBTransactions] ADD  CONSTRAINT [DF_MMBTransactions_Locked]  DEFAULT ((0)) FOR [Locked]
GO


