USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[UserAccessCustomSecurity]    Script Date: 8/12/2020 2:37:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserAccessCustomSecurity](
	[UserID] [nchar](30) NOT NULL,
	[SecurityObject] [nvarchar](50) NOT NULL,
	[AccessAllowed] [bit] NOT NULL,
	[AccessDenied] [bit] NOT NULL,
 CONSTRAINT [PK_UserAccessCustomSecurity] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[SecurityObject] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


