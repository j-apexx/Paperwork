USE [PaperworkWeb]
GO

/****** Object:  Table [dbo].[TaxRates]    Script Date: 1/17/2020 11:41:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TaxRates](
	[ProgramYear] nvarchar(15) NOT NULL,
	[TaxYear] int NOT NULL,
	[PayProc] numeric(11,5) NOT NULL DEFAULT 0,
	[WComp] numeric(11,5) NOT NULL DEFAULT 0,
	[OtherPayRelated] numeric(11,5) NOT NULL DEFAULT 0
CONSTRAINT [PK_TaxRates] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


