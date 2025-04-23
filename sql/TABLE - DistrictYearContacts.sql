USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[DistrictYearContacts]    Script Date: 3/19/2020 3:32:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DistrictYearContacts](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](10) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Position] [nvarchar](50) NOT NULL,
	[Note] [nvarchar](100) NOT NULL,
	[Building] [nvarchar](50) NOT NULL,
	[Address] [nvarchar](50) NOT NULL,
	[City] [nvarchar](50) NOT NULL,
	[State] [nvarchar](2) NOT NULL,
	[Zip] [nvarchar](10) NOT NULL,
	[WorkPhone] [nvarchar](50) NOT NULL,
	[CellPhone] [nvarchar](50) NOT NULL,
	[DateAdded] [date] NOT NULL,
	[DateModified] [date] NOT NULL,
 CONSTRAINT [PK_DistrictYearContacts] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_Title]  DEFAULT ('') FOR [Title]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_FirstName]  DEFAULT ('') FOR [FirstName]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_LastName]  DEFAULT ('') FOR [LastName]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_Email]  DEFAULT ('') FOR [Email]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_Position]  DEFAULT ('') FOR [Position]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_Note]  DEFAULT ('') FOR [Note]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_Building]  DEFAULT ('') FOR [Building]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_Address]  DEFAULT ('') FOR [Address]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_City]  DEFAULT ('') FOR [City]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_State]  DEFAULT ('') FOR [State]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_Zip]  DEFAULT ('') FOR [Zip]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_WorkPhone]  DEFAULT ('') FOR [WorkPhone]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_CellPhone]  DEFAULT ('') FOR [CellPhone]
GO

ALTER TABLE [dbo].[DistrictYearContacts] ADD  CONSTRAINT [DF_DistrictYearContacts_DateModified]  DEFAULT (CONVERT([date],getdate())) FOR [DateModified]
GO

