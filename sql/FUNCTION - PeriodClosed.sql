-- ================================================
-- Template generated from Template Explorer using:
-- Create Scalar Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Marvin
-- Create date: 6/25/2021
-- Description:	Helper function to determine if district is allowed to Create, Update, Delete entries
-- =============================================
CREATE FUNCTION PeriodClosed
(
	-- Add the parameters for the function here
	@DistrictID VARCHAR(3),
	@ProgYear Varchar(15),
	@CheckDate DATETIME
)
RETURNS BIT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @PeriodClosed BIT

	-- Add the T-SQL statements to compute the return value here
	SET @PeriodClosed = (SELECT IIF(EnforcePeriodClosures=1 AND ([AllowDataEntry]=0 OR PeriodClosed=1), 1, 0) DataEntryAllowed
							FROM 
							(SELECT ISNULL(EnforcePeriodClosures, 1) EnforcePeriodClosures,
							IIF(EnforcePeriodClosures IS NULL, 0, [AllowDataEntry]) [AllowDataEntry],
							CASE MONTH(@CheckDate)
								WHEN 1 then Close01
								WHEN 2 then Close02
								WHEN 3 then Close03
								WHEN 4 then Close04
								WHEN 5 then Close05
								WHEN 6 then Close06
								WHEN 7 then Close07
								WHEN 8 then Close08
								WHEN 9 then Close09
								WHEN 10 then Close10
								WHEN 11 then Close11
								WHEN 12 then Close12
								END PeriodClosed
	
							FROM [dbo].[ProgramYear]
							LEFT OUTER JOIN [District] ON @DistrictID=[DistrictID]
							WHERE ProgramYear=@ProgYear) DateEntryCheck)

	-- Return the result of the function
	RETURN @PeriodClosed

END
GO

