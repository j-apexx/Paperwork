USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[EmployeePaycodes]    Script Date: 10/8/2019 1:57:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmployeePaycodes](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[EmployeeNum] [int] NOT NULL,
	[PayCode] [smallint] NOT NULL,
	[Rate] [numeric](11, 2) NOT NULL,
 CONSTRAINT [PK_EmployeePaycodes] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] ASC,
	[DistrictID] ASC,
	[EmployeeNum] ASC,
	[PayCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

