USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[TrainingAttendees]    Script Date: 3/7/2025 4:56:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrainingAttendees](
	[DistrictID] [nvarchar](3) NOT NULL,
	[ProgramYear] [nvarchar](15) NOT NULL,
	[TrainingID] [int] NOT NULL,
	[AttendeeID] [int] IDENTITY(1,1) NOT NULL,
	[AttendeeName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TrainingAttendees_1] PRIMARY KEY CLUSTERED 
(
	[DistrictID] ASC,
	[ProgramYear] ASC,
	[TrainingID] ASC,
	[AttendeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

