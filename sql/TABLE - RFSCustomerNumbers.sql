USE PaperworkWeb

/****** Object:  Table [dbo].[RFSCustomerNumbers]    Script Date: 10/28/2019 10:59:17 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RFSCustomerNumbers](
	[DistrictID] [nvarchar](3) NOT NULL,
	[SchoolNum] [smallint] NOT NULL,
	[ProgramYear] nvarchar (15) NOT NULL,
	[RFSCustNum] [int] NOT NULL,
	[Division] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_RFSCustomerNumbers] PRIMARY KEY CLUSTERED 
(
	[RFSCustNum] ASC,
	[Division] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


