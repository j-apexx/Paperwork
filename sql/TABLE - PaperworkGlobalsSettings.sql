USE PAPERWORKWEB
GO

/****** Object:  Table [dbo].[CommissaryGlobalSettings]    Script Date: 11/7/2019 1:23:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PaperworkGlobalSettings](
	[Index] [int] NOT NULL,
	[ReinhartInvSuffix] [nvarchar](5) NOT NULL,
	[ReinhartVendorIDs] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_PaperworkGlobalSettings] PRIMARY KEY CLUSTERED 
(
	[Index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PaperworkGlobalSettings] ADD  CONSTRAINT [DF_PaperworkGlobalSettings_ReinhartInvPrefix]  DEFAULT ('') FOR [ReinhartInvSuffix]
GO

ALTER TABLE [dbo].[PaperworkGlobalSettings] ADD  CONSTRAINT [DF_PaperworkGlobalSettings_ReinhartVendorIDs]  DEFAULT ('') FOR [ReinhartVendorIDs]
GO


