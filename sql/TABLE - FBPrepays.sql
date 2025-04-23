USE [PaperworkWeb]
GO

/****** Object:  Table [dbo].[InvoiceCredits]    Script Date: 5/20/2020 9:46:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FBPrepays](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[PrepayDate] [date] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[PrepayAmount] [numeric](11, 2) NOT NULL,
	[Counter] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_FBPrepay] PRIMARY KEY CLUSTERED 
(
	[Counter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FBPrepays] ADD  CONSTRAINT [DF_FBPrepays_Description]  DEFAULT ('') FOR [Description]
GO

ALTER TABLE [dbo].[FBPrepays] ADD  CONSTRAINT [DF_FBPrepays_PrepayAmount]  DEFAULT ((0)) FOR [PrepayAmount]
GO


