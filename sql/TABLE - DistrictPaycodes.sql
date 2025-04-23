USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[DistrictPaycodes]    Script Date: 10/8/2019 1:57:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DistrictPaycodes](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[PayCode] [smallint] NOT NULL,
	[PayCodeDesc] [nvarchar](30) NOT NULL,
	[HoursType] [smallint] NOT NULL,
 CONSTRAINT [PK_DistrictPaycodes] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] ASC,
	[DistrictID] ASC,
	[PayCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

