USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[OtherSales]    Script Date: 11/15/2019 3:23:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OtherSales](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[OtherSalesDate] [datetime] NOT NULL,
	[OtherSalesID] [smallint] NOT NULL,
	[OtherSalesAmount] [numeric](11, 2) NOT NULL,
	[Locked] [bit] NOT NULL,
 CONSTRAINT [PK_OtherSales] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[OtherSalesDate] ASC,
	[OtherSalesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

