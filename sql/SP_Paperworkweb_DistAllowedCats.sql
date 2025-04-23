USE [NUTR1]
GO

/****** Object:  StoredProcedure [dbo].[Papwerworkweb_DistAllowedOCats]    Script Date: 11/8/2019 4:27:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Chris Marvin
-- Create date: 11/8/2019
-- Description:	Generates a list of allowed Other Categories for use by Districts during Purchase Entry according to budgeted amounts
-- =============================================
CREATE PROCEDURE [dbo].[Papwerworkweb_DistAllowedOCats] 
	-- Add the parameters for the stored procedure here
	@BudgetYear nvarchar(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ColumnList nvarchar(max)
	DECLARE @SQL nvarchar(max)

	-- Fill ColumnList with all column names.
	-- This allows the procedure to capture new columns(Expense Categories) to be added in the future and continue to work without modification
	SET @ColumnList= 
	STUFF((SELECT ',' + COLUMN_NAME FROM
		(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME LIKE 'vwBDGT_PurchaseCategoryPermissions' AND COLUMN_NAME NOT IN ('BidNumber','BudgetYear')) COLS
	FOR XML PATH('')), 1, 1, '');

	--Build the SQL statement. Note that I do it this way since UNPIVOT does not allow a subquery to gather the column names.
	--This essentially types in each column name to be included in the UNPIVOT
	SET @SQL = N'

	SELECT T.Bidnumber, T.CATEGORIES FROM (SELECT BidNumber, BudgetYear, U.CATEGORIES, U.val
	FROM [dbo].[vwBDGT_PurchaseCategoryPermissions]
	UNPIVOT
	(
		val
		for CATEGORIES IN (' + @ColumnList + N')
	) U WHERE U.val=1) T
	WHERE T.BudgetYear=''' +@BudgetYear+ ''''

	EXEC sp_executesql @sql;

END
GO

