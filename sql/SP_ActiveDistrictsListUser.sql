USE [PAPERWORKWEB]
GO

/****** Object:  StoredProcedure [dbo].[ActiveDistrictsListUser]    Script Date: 8/13/2020 3:15:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Chris Marvin
-- Create date: 8/12/2020
-- Description:	Lists all active Districts and any inactive districts which have transactions for a given program year
-- =============================================
CREATE PROCEDURE [dbo].[ActiveDistrictsListUser] 
	-- Add the parameters for the stored procedure here
	@ProgramYear Varchar(15),
	@UserID Varchar(30),
	@AccessLevel Varchar(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT ActiveDists.DistrictID, ActiveDists.DistrictName + '-' + ActiveDists.DistrictID 'Desc', AcctType
	FROM (
		SELECT @ProgramYear ProgramYear, DistrictID, DistrictName FROM District WHERE Inactive=0

		UNION ALL

		SELECT P.ProgramYear, P.DistrictID, D.DistrictName 
		FROM Purchases P
		INNER JOIN District D ON P.DistrictID=D.DistrictID
		WHERE D.Inactive=1 AND P.ProgramYear=@ProgramYear
		GROUP BY P.ProgramYear, P.DistrictID, D.DistrictName

		UNION ALL

		SELECT S.[ProgramYear],S.[DistrictID],D.DistrictName
		  FROM [RecapEntry] S
		INNER JOIN District D ON S.DistrictID=D.DistrictID
		WHERE D.Inactive=1 AND S.ProgramYear=@ProgramYear
		GROUP BY S.ProgramYear, S.DistrictID, D.DistrictName

		UNION ALL

		SELECT P.[ProgramYear],P.[DistrictID], D.DistrictName
		FROM [Payroll] P
		INNER JOIN District D ON P.DistrictID=D.DistrictID
		WHERE D.Inactive=1 AND P.ProgramYear=@ProgramYear
		GROUP BY P.ProgramYear, P.DistrictID, D.DistrictName

		UNION ALL

		SELECT I.[ProgramYear],I.[DistrictID], D.DistrictName
		FROM [Inventory] I
		INNER JOIN District D ON I.DistrictID=D.DistrictID
		WHERE D.Inactive=1 AND I.ProgramYear=@ProgramYear
		GROUP BY I.ProgramYear, I.DistrictID, D.DistrictName

	) ActiveDists
	LEFT OUTER JOIN DistrictYearInfo DYI ON ActiveDists.DistrictID=DYI.DistrictID AND @ProgramYear=DYI.ProgramYear
	WHERE ActiveDists.DistrictID IN (SELECT DistrictID FROM [UserDistrictAccess] WHERE Userid=@UserID) OR (UPPER(@AccessLevel) NOT IN ('USER'))
	ORDER BY ActiveDists.DistrictName + '-' + ActiveDists.DistrictID
END
GO

