USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[UserAccess]    Script Date: 5/10/2019 11:32:12 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserAccess](
	[UserID] [nchar](30) NOT NULL,
	[AccessLevel] [int] NOT NULL,
	[LastActivity] [datetime] NOT NULL,
	[LastDistrictID] [nvarchar](3) NOT NULL,
 CONSTRAINT [PK_UserAccess] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserAccess] ADD  DEFAULT ('1/1/1900') FOR [LastActivity]
GO

ALTER TABLE [dbo].[UserAccess] ADD  DEFAULT ('') FOR [LastDistrictID]
GO

