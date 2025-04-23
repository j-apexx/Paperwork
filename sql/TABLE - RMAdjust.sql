USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[RMAdjust]    Script Date: 12/6/2019 2:03:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RMAdjust](
	[Counter] [bigint] IDENTITY(1,1) NOT NULL,
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[AdjustmentDate] [date] NOT NULL,
	[Amount] [numeric](18, 2) NOT NULL,
 CONSTRAINT [PK_RMAdjust] PRIMARY KEY CLUSTERED 
(
	[Counter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

