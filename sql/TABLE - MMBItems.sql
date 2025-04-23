USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[MMBItems]    Script Date: 10/24/2019 1:49:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MMBItems](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[ProductCode] [smallint] NOT NULL,
	[ProductName] [nvarchar](25) NOT NULL,
	[Price] [numeric](11, 4) NOT NULL,
	[MilkItem] [bit] NOT NULL,
 CONSTRAINT [PK_MMBItems] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[ProductCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MMBItems] ADD  CONSTRAINT [DF_MMBItems_MilkItem]  DEFAULT ((0)) FOR [MilkItem]
GO

