USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[PayIns]    Script Date: 12/5/2019 1:40:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PayIns](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[InsuranceDate] [date] NOT NULL,
	[Amount] [numeric](11, 2) NOT NULL,
 CONSTRAINT [PK_PayIns] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[InsuranceDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PayIns] ADD  CONSTRAINT [DF_PayIns_Amount]  DEFAULT ((0)) FOR [Amount]
GO

