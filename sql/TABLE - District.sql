USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[District]    Script Date: 5/21/2019 12:52:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[District](
	[DistrictID] [nvarchar](3) NOT NULL,
	[DistrictName] [nvarchar](40) NOT NULL,
	[DistrictAddress1] [nvarchar](50) NOT NULL,
	[DistrictAddress2] [nvarchar](50) NOT NULL,
	[DistrictCity] [nvarchar](35) NOT NULL,
	[DistrictState] [nvarchar](2) NOT NULL,
	[DistrictZip] [nvarchar](10) NOT NULL,
	[DistrictContact] [nvarchar](50) NOT NULL,
	[DistrictFullName] [nvarchar](100) NOT NULL,
	[Inactive] [tinyint] NOT NULL,
	[EnforcePeriodClosures] [tinyint] NOT NULL,
 CONSTRAINT [PK_District] PRIMARY KEY CLUSTERED 
(
	[DistrictID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[District] ADD  CONSTRAINT [DF_District_DistrictState]  DEFAULT ('') FOR [DistrictState]
GO

ALTER TABLE [dbo].[District] ADD  CONSTRAINT [DF_District_Inactive]  DEFAULT ((0)) FOR [Inactive]
GO

ALTER TABLE [dbo].[District] ADD  CONSTRAINT [DF_District_EnforcePeriodClosures]  DEFAULT ((1)) FOR [EnforcePeriodClosures]
GO

