USE [NUTR1]
GO

/****** Object:  StoredProcedure [dbo].[Paperworkweb_DistBudgetedOCats]    Script Date: 8/12/2020 8:34:39 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Chris Marvin
-- Create date: 11/8/2019
-- Description:	Generates a list of allowed Other Categories with budgeted amounts for use by Districts during Purchase Entry according to budgeted amounts
-- =============================================
CREATE PROCEDURE [dbo].[Paperworkweb_DistBudgetedOCats] 
	-- Add the parameters for the stored procedure here
	@BudgetYear nvarchar(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ColumnList nvarchar(max)
	DECLARE @ColumnConvert nvarchar(max)
	DECLARE @SQL nvarchar(max)

	-- Fill ColumnList with all column names.
	-- This allows the procedure to capture new columns(Expense Categories) to be added in the future and continue to work without modification
	SET @ColumnList= 
	STUFF((SELECT ',' + COLUMN_NAME   FROM
		(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME LIKE 'vwBDGT_PurchaseCategoryAmounts' AND COLUMN_NAME NOT IN ('BidNumber','BudgetYear','DistinctKey')) COLS
	FOR XML PATH('')), 1, 1, '');

	--in order to resolve issue with the following error, needed to make the following view select statement
	--The type of column "BGCHK" conflicts with the type of other columns specified in the UNPIVOT list.
	--Need to explicitly set all coulmns as numeric values
	SET @ColumnConvert= 
	STUFF((SELECT ',CAST(' + COLUMN_NAME + ' AS Numeric(11,5)) ' + COLUMN_NAME FROM
		(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME LIKE 'vwBDGT_PurchaseCategoryAmounts' AND COLUMN_NAME NOT IN ('BidNumber','BudgetYear','DistinctKey')) COLS
	FOR XML PATH('')), 1, 1, '');

	--Build the SQL statement. Note that I do it this way since UNPIVOT does not allow a subquery to gather the column names.
	--This essentially types in each column name to be included in the UNPIVOT

	--SELECT T.Bidnumber, T.CATEGORIES, T.val AS Budget FROM 

	SET @SQL = N'

	
	
	SELECT BidNumber, BudgetYear, U.CATEGORIES, U.val AS BudgetedAmount
	FROM (SELECT BidNumber, BudgetYear, '+ @ColumnConvert +' FROM [dbo].[vwBDGT_PurchaseCategoryAmounts]) VW
	UNPIVOT
	(
		val
		for CATEGORIES IN (' + @ColumnList + N')
	) U WHERE U.val <>0
	AND BudgetYear=''' +@BudgetYear+ ''''

	EXEC sp_executesql @sql;

END
GO

