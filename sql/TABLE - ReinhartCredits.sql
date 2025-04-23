USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[ReinhartCredits]    Script Date: 12/6/2019 10:33:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReinhartCredits](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[InvoiceDate] [date] NOT NULL,
	[Type] [nvarchar](1) NOT NULL,
	[Month] [smallint] NOT NULL,
	[Total] [numeric](11, 2) NOT NULL,
	[Promotional] [numeric](11, 2) NOT NULL,
	[Competitive] [numeric](11, 2) NOT NULL,
	[NetOffInvoice] [numeric](11, 2) NOT NULL,
	[Counter] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_ReinhartCredits] PRIMARY KEY CLUSTERED 
(
	[Counter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UniqueRecordIndex] ON [dbo].[ReinhartCredits]
(
	[ProgramYear] ASC,
	[DistrictID] ASC,
	[InvoiceDate] ASC,
	[Type] ASC,
	[Month] ASC,
	[Total] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

