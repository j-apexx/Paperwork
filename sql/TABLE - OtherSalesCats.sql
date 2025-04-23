USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[OtherSalesCats]    Script Date: 11/15/2019 3:24:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OtherSalesCats](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[OtherSalesID] [smallint] NOT NULL,
	[OtherSalesDesc] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_OtherSalesCats] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[OtherSalesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

