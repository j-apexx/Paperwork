USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[TrainingHDR]    Script Date: 3/7/2025 4:57:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrainingHDR](
	[DistrictID] [nvarchar](3) NOT NULL,
	[ProgramYear] [nvarchar](15) NOT NULL,
	[TrainingID] [int] IDENTITY(1,1) NOT NULL,
	[TrainingTitle] [varbinary](50) NULL,
	[TrainingDescription] [nvarchar](max) NULL,
	[TrainingDate] [date] NULL,
	[TrainingTime] [time](7) NULL,
	[TrainerName] [nvarchar](50) NULL,
	[Location] [nvarchar](50) NULL,
 CONSTRAINT [PK_TrainingHDR] PRIMARY KEY CLUSTERED 
(
	[DistrictID] ASC,
	[ProgramYear] ASC,
	[TrainingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

