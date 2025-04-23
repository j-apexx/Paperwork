USE [PaperworkWeb]
GO

/****** Object:  StoredProcedure [dbo].[GenerateInventorySelectors]    Script Date: 8/24/2020 4:29:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Chris Marvin
-- Create date: 8/24/2020
-- Description:	For a specified program year, generates all Inventory YearMonth selectors
-- =============================================
CREATE PROCEDURE [dbo].[GenerateInventorySelectors]
	-- Add the parameters for the stored procedure here
	@ProgramYear Varchar(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @MinDate DATE = (SELECT StartDate FROM ProgramYear WHERE ProgramYear=@ProgramYear)
	DECLARE @MaxDate DATE = (SELECT EndDate FROM ProgramYear WHERE ProgramYear=@ProgramYear)

	SELECT '*OPEN*' AS YearMonth 

	UNION ALL

	SELECT CAST(YEAR(Date1) AS VARCHAR(4)) + '-' + RIGHT('0' + CAST(MONTH(Date1) AS VARCHAR(4)),2) YearMonth  FROM(
	SELECT TOP (DATEDIFF(MONTH, @MinDate, @MaxDate) +1)
		Date1 = DATEADD(MONTH,ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MinDate)
		FROM sys.all_objects a
		CROSS JOIN sys.all_objects b
		) DateTable
END
GO

