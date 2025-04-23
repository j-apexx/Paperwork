USE [PAPERWORKWEB]
GO

/****** Object:  Table [dbo].[Inventory]    Script Date: 11/13/2019 2:15:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Inventory](
	[ProgramYear] [nvarchar](15) NOT NULL,
	[DistrictID] [nvarchar](3) NOT NULL,
	[YearMonth] [nvarchar](7) NOT NULL,
	[Groceries] [numeric](11, 2) NOT NULL,
	[Paper] [numeric](11, 2) NOT NULL,
	[Cleaning] [numeric](11, 2) NOT NULL,
	[Milk] [numeric](11, 2) NOT NULL,
	[USDAProteinOpen] [numeric](11, 2) NOT NULL,
	[USDAProteinRcvd] [numeric](11, 2) NOT NULL,
	[USDAProteinInstUse] [numeric](11, 2) NOT NULL,
	[USDAProteinTrans] [numeric](11, 2) NOT NULL,
	[USDAProteinEnding] [numeric](11, 2) NOT NULL,
	[USDAGroceryOpen] [numeric](11, 2) NOT NULL,
	[USDAGroceryRcvd] [numeric](11, 2) NOT NULL,
	[USDAGroceryInstUse] [numeric](11, 2) NOT NULL,
	[USDAGroceryTrans] [numeric](11, 2) NOT NULL,
	[USDAGroceryEnding] [numeric](11, 2) NOT NULL,
	[Comments] [nvarchar](150) NOT NULL,
	[Locked] [bit] NOT NULL,
 CONSTRAINT [PK_Inventory] PRIMARY KEY CLUSTERED 
(
	[ProgramYear] DESC,
	[DistrictID] ASC,
	[YearMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_Groceries]  DEFAULT ((0)) FOR [Groceries]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_Paper]  DEFAULT ((0)) FOR [Paper]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_Cleaning]  DEFAULT ((0)) FOR [Cleaning]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_Milk]  DEFAULT ((0)) FOR [Milk]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAProteinOpen]  DEFAULT ((0)) FOR [USDAProteinOpen]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAProteinRcvd]  DEFAULT ((0)) FOR [USDAProteinRcvd]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAProteinInstUse]  DEFAULT ((0)) FOR [USDAProteinInstUse]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAProteinTrans]  DEFAULT ((0)) FOR [USDAProteinTrans]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAProteinEnding]  DEFAULT ((0)) FOR [USDAProteinEnding]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAGroceryOpen]  DEFAULT ((0)) FOR [USDAGroceryOpen]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAGroceryRcvd]  DEFAULT ((0)) FOR [USDAGroceryRcvd]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAGroceryInstUse]  DEFAULT ((0)) FOR [USDAGroceryInstUse]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAGroceryTrans]  DEFAULT ((0)) FOR [USDAGroceryTrans]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_USDAGroceryEnding]  DEFAULT ((0)) FOR [USDAGroceryEnding]
GO

ALTER TABLE [dbo].[Inventory] ADD  CONSTRAINT [DF_Inventory_Locked]  DEFAULT ((0)) FOR [Locked]
GO

