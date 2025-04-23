
/****** Object:  Table [dbo].[DistrictYearInfo]    Script Date: 9/9/2019 2:21:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DistrictYearInfo](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[AcctType] [tinyint] NOT NULL,
	[GM] [nvarchar](50) NOT NULL,
	[RMCode] [nvarchar](2) NOT NULL,
	[RMName] [nvarchar](50) NOT NULL,
	[FSDName] [nvarchar](50) NOT NULL,
	[FSDAddress] [nvarchar](50) NOT NULL,
	[FSDCity] [nvarchar](35) NOT NULL,
	[FSDState] [nvarchar](2) NOT NULL,
	[FSDZip] [nvarchar](10) NOT NULL,
	[FSDEmail] [nvarchar](50) NOT NULL,
	[FSDPhone] [nvarchar](25) NOT NULL,
	[FSDCell] [nvarchar](25) NOT NULL,
	[BusinessMgr] [nvarchar](50) NOT NULL,
	[Superintendent] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DistrictYearInfo] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] ASC,
	[DistrictID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_AcctType]  DEFAULT ((0)) FOR [AcctType]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_GM]  DEFAULT ('') FOR [GM]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_RMCode]  DEFAULT ('') FOR [RMCode]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_RMName]  DEFAULT ('') FOR [RMName]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_FSDName]  DEFAULT ('') FOR [FSDName]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_FSDAddress]  DEFAULT ('') FOR [FSDAddress]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_FSDCity]  DEFAULT ('') FOR [FSDCity]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_FSDState]  DEFAULT ('') FOR [FSDState]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_FSDZip]  DEFAULT ('') FOR [FSDZip]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_FSDEmail]  DEFAULT ('') FOR [FSDEmail]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_FSDPhone]  DEFAULT ('') FOR [FSDPhone]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_FSDCell]  DEFAULT ('') FOR [FSDCell]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_BusinessMgr]  DEFAULT ('') FOR [BusinessMgr]
GO

ALTER TABLE [dbo].[DistrictYearInfo] ADD  CONSTRAINT [DF_DistrictYearInfo_Superintendent]  DEFAULT ('') FOR [Superintendent]
GO


