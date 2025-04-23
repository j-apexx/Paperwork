USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[Employees]    Script Date: 9/30/2019 3:02:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Employees](
	[EmployeeNum] [int] NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[LastName] [nvarchar](30) NOT NULL,
	[FirstName] [nvarchar](30) NOT NULL,
	[MiddleName] [nvarchar](30) NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
	[Rate1] [numeric](11, 2) NOT NULL,
	[Rate2] [numeric](11, 2) NOT NULL,
	[Rate3] [numeric](11, 2) NOT NULL,
	[Rate4] [numeric](11, 2) NOT NULL,
	[Rate5] [numeric](11, 2) NOT NULL,
	[Health] [numeric](11, 2) NOT NULL,
	[LifeAmount] [numeric](11, 2) NOT NULL,
	[LifeTotal] [numeric](11, 2) NOT NULL,
	[LifeAddDate] [date] NOT NULL,
	[LifeDeleteDate] [date] NOT NULL,
	[ADDAmount] [numeric](11, 2) NOT NULL,
	[ADDTotal] [numeric](11, 2) NOT NULL,
	[Inactive] [bit] NOT NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[DistrictID] ASC,
	[EmployeeNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_LastName]  DEFAULT ('') FOR [LastName]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_FirstName]  DEFAULT ('') FOR [FirstName]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_MiddleName]  DEFAULT ('') FOR [MiddleName]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_SchoolNum]  DEFAULT ((0)) FOR [SchoolNum]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_Rate1]  DEFAULT ((0)) FOR [Rate1]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_Rate2]  DEFAULT ((0)) FOR [Rate2]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_Rate3]  DEFAULT ((0)) FOR [Rate3]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_Rate4]  DEFAULT ((0)) FOR [Rate4]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_Rate5]  DEFAULT ((0)) FOR [Rate5]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_Health]  DEFAULT ((0)) FOR [Health]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_LifeAmount]  DEFAULT ((0)) FOR [LifeAmount]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_LifeTotal]  DEFAULT ((0)) FOR [LifeTotal]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_LifeAddDate]  DEFAULT ('1/1/1900') FOR [LifeAddDate]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_LifeDeleteDate]  DEFAULT ('1/1/1900') FOR [LifeDeleteDate]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_ADDAmount]  DEFAULT ((0)) FOR [ADDAmount]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_ADDTotal]  DEFAULT ((0)) FOR [ADDTotal]
GO

ALTER TABLE [dbo].[Employees] ADD  CONSTRAINT [DF_Employees_Inactive]  DEFAULT ((0)) FOR [Inactive]
GO

