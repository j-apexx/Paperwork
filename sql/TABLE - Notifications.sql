USE [PaperworkWeb]
GO

/****** Object:  Table [dbo].[Notifications]    Script Date: 1/27/2020 8:28:22 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Notifications](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[NotificationDate] [date] NOT NULL,
	[Transmittals] [bit] NOT NULL,
	[NoTransmittals] [bit] NOT NULL,
	[Recaps] [bit] NOT NULL,
	[MonthEnd] [bit] NOT NULL,
	[Note] [nvarchar](50) NOT NULL,
	[EntryDate] [datetime] NOT NULL,
	[ChangeDate] [datetime] NOT NULL,
	[Locked] [bit] NOT NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[NotificationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_Transmittals]  DEFAULT ((0)) FOR [Transmittals]
GO

ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_NoTransmittals]  DEFAULT ((0)) FOR [NoTransmittals]
GO

ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_Recaps]  DEFAULT ((0)) FOR [Recaps]
GO

ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_MonthEnd]  DEFAULT ((0)) FOR [MonthEnd]
GO

ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_Note]  DEFAULT ('') FOR [Note]
GO

ALTER TABLE [dbo].[Notifications] ADD  CONSTRAINT [DF_Notifications_Locked]  DEFAULT ((0)) FOR [Locked]
GO


