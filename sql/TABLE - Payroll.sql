USE [PaperworkWeb]
GO

/****** Object:  Table [dbo].[Payroll]    Script Date: 10/2/2019 11:26:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Payroll](
	[Counter] [int] IDENTITY(1,1) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[EmployeeNum] [int] NOT NULL,
	[ProgramYear] [nvarchar](15) NOT NULL,
	[PayDate] [date] NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
	[PayCode] [smallint] NOT NULL,
	[Hours] [numeric](11, 2) NOT NULL,
	[Rate] [numeric](11, 2) NOT NULL,
	[OtherPay] [numeric](11, 2) NOT NULL,
	[Health] [numeric](11, 2) NOT NULL,
	[Locked] [bit] NOT NULL,
 CONSTRAINT [PK_Payroll] PRIMARY KEY NONCLUSTERED 
(
	[Counter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Payroll] ADD  CONSTRAINT [DF_Payroll_SchoolNum]  DEFAULT ((0)) FOR [SchoolNum]
GO

ALTER TABLE [dbo].[Payroll] ADD  CONSTRAINT [DF_Payroll_PayCode]  DEFAULT ((0)) FOR [PayCode]
GO

ALTER TABLE [dbo].[Payroll] ADD  CONSTRAINT [DF_Table_1_Hour]  DEFAULT ((0)) FOR [Hours]
GO

ALTER TABLE [dbo].[Payroll] ADD  CONSTRAINT [DF_Payroll_Rate]  DEFAULT ((0)) FOR [Rate]
GO

ALTER TABLE [dbo].[Payroll] ADD  CONSTRAINT [DF_Payroll_OtherPay]  DEFAULT ((0)) FOR [OtherPay]
GO

ALTER TABLE [dbo].[Payroll] ADD  CONSTRAINT [DF_Payroll_Health]  DEFAULT ((0)) FOR [Health]
GO

ALTER TABLE [dbo].[Payroll] ADD  CONSTRAINT [DF_Payroll_Locked]  DEFAULT ((0)) FOR [Locked]
GO

CREATE CLUSTERED INDEX [IX_DistrictPayDate] ON [dbo].[Payroll]
(
	[DistrictID] ASC,
	[PayDate] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


