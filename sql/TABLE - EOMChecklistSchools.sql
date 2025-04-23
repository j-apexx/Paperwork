USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[EOMChecklistSchools]    Script Date: 12/9/2021 11:34:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EOMChecklistSchools](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[YearMonth] [nvarchar](7) NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
	[SnapshotDate] [date] NOT NULL,
	[CBUpdatedPricing] [bit] NOT NULL,
 CONSTRAINT [PK_EOMChecklistSchools] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[YearMonth] ASC,
	[SchoolNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EOMChecklistSchools] ADD  CONSTRAINT [DF_EOMChecklistSchools_SnapshotDate]  DEFAULT ('1/1/1900') FOR [SnapshotDate]
GO

ALTER TABLE [dbo].[EOMChecklistSchools] ADD  CONSTRAINT [DF_EOMChecklistSchools_UpdatedPricing]  DEFAULT ((0)) FOR [CBUpdatedPricing]
GO

