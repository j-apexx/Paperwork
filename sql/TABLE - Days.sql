USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[Days]    Script Date: 1/24/2020 2:38:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Days](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[ServingDate] [date] NOT NULL,
	[BreakfastDay] [bit] NOT NULL,
	[LunchDay] [bit] NOT NULL,
	[Overridden] [bit] NOT NULL,
	[OverriddenBy] [nchar](30) NOT NULL,
	[Locked] [bit] NOT NULL,
 CONSTRAINT [PK_Days] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[ServingDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Days] ADD  CONSTRAINT [DF_Days_BreakfastDay]  DEFAULT ((0)) FOR [BreakfastDay]
GO

ALTER TABLE [dbo].[Days] ADD  CONSTRAINT [DF_Days_LunchDay]  DEFAULT ((0)) FOR [LunchDay]
GO

ALTER TABLE [dbo].[Days] ADD  CONSTRAINT [DF_Table_1_ManuallyChanged]  DEFAULT ((0)) FOR [Overridden]
GO

ALTER TABLE [dbo].[Days] ADD  CONSTRAINT [DF_Days_OverriddenBy]  DEFAULT ('') FOR [OverriddenBy]
GO

ALTER TABLE [dbo].[Days] ADD  CONSTRAINT [DF_Days_Locked]  DEFAULT ((0)) FOR [Locked]
GO


