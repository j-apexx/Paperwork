USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[ProgramYear]    Script Date: 5/21/2019 12:51:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProgramYear](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[AllowDataEntry] [tinyint] NOT NULL,
	[Close01] [tinyint] NOT NULL,
	[Close02] [tinyint] NOT NULL,
	[Close03] [tinyint] NOT NULL,
	[Close04] [tinyint] NOT NULL,
	[Close05] [tinyint] NOT NULL,
	[Close06] [tinyint] NOT NULL,
	[Close07] [tinyint] NOT NULL,
	[Close08] [tinyint] NOT NULL,
	[Close09] [tinyint] NOT NULL,
	[Close10] [tinyint] NOT NULL,
	[Close11] [tinyint] NOT NULL,
	[Close12] [tinyint] NOT NULL,
 CONSTRAINT [PK_ProgramYear] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close01]  DEFAULT ((0)) FOR [Close01]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close02]  DEFAULT ((0)) FOR [Close02]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close03]  DEFAULT ((0)) FOR [Close03]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close04]  DEFAULT ((0)) FOR [Close04]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close05]  DEFAULT ((0)) FOR [Close05]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close06]  DEFAULT ((0)) FOR [Close06]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close07]  DEFAULT ((0)) FOR [Close07]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close08]  DEFAULT ((0)) FOR [Close08]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close09]  DEFAULT ((0)) FOR [Close09]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close10]  DEFAULT ((0)) FOR [Close10]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close11]  DEFAULT ((0)) FOR [Close11]
GO

ALTER TABLE [dbo].[ProgramYear] ADD  CONSTRAINT [DF_ProgramYear_Close12]  DEFAULT ((0)) FOR [Close12]
GO

