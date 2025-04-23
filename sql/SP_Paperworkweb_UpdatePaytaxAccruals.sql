USE [PAPERWORKWEB]
GO

/****** Object:  StoredProcedure [dbo].[UpdatePaytaxAccruals]    Script Date: 1/29/2020 3:53:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePaytaxAccruals] 
	-- Add the parameters for the stored procedure here
	@BDt DATETIME, 
	@EDt DATETIME, 
	@AccrualDaysOverride INT=NULL,
	@RowsUpdated INT OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE PAYTAX SET PAYTAX.PayAccrual=AccrualTotals.PayAccrual , PayTax.TaxAccrual=AccrualTotals.TaxAccrual
	FROM PAYTAX
	INNER JOIN 

	(SELECT DistrictID,
		Max(Accrual.EndDate) AS MaxOfEndDate, 
		Min(Accrual.AccruDays) AS MinOfAccruDays, 
		Min(Accrual.AccruDays)*Sum(Accrual.Gross)/20 AS PayAccrual,
		--CCur(FormatNumber(([MinofAccruDays]*[SumofGross]/20),2)) AS PayAccrual, 
		(Sum(Accrual.FUTA)+Sum(Accrual.Medicare)+Sum(Accrual.PASUI)+Sum(Accrual.SocSec)+(Sum([WComp])) )/ 20 * Min(Accrual.AccruDays) AS TaxAccrual,
		--FormatCurrency(([SumofFUTA]+[SumofMEDR]+[SumofPASUI]+[SumofSSR]+[WComp])/20*[minofAccruDays],2) AS TaxAccrual, 
		Sum(Accrual.Gross) AS SumOfGross, 
		Sum(Accrual.FUTA) AS SumOfFUTA, 
		Sum(Accrual.Medicare) AS SumOfMEDR, 
		Sum(Accrual.PASUI) AS SumOfPASUI, 
		Sum(Accrual.OHSUI) AS SumOfOHSUI, 
		Sum(Accrual.SocSec) AS SumOfSSR, 
		Sum(Accrual.Uniforms) AS SumOfUnifm, 
		Sum(Accrual.PAPitC) AS SumOfPAPitC,
		Min(Accrual.[WCompRate]) WCompRate,
		Sum(Calcs.[WComp]) AS WComp,
		Min(Accrual.PayProcRate) AS HourlyProcRate,
		Sum(Accrual.Gross) + (Min(Accrual.AccruDays)*Sum(Accrual.Gross)/20) AS GrossPlusAccrual,
		Min(Accrual.PayProcRate) * (Sum(Accrual.Gross) + (Min(Accrual.AccruDays)*Sum(Accrual.Gross)/20)) AS HourlyProc
	FROM
		(SELECT PayTax.DistrictID, 
		PayTax.ProgramYear, 
		PayTax.EndDate,
		EOMONTH(@EDt) AS LastDay, 
		IIF(@AccrualDaysOverride IS NULL,
			DateDiff("d",PayTax.EndDate,EOMONTH(@EDt))-(2*(DatePart("ww",EOMONTH(@EDt))-DatePart("ww",[EndDate])))
			,@AccrualDaysOverride) AS AccruDays, 
		PayTax.Gross, PayTax.FUTA, 
		PayTax.Medicare, PayTax.PASUI, 
		PayTax.OHSUI, PayTax.SocSec, 
		PayTax.Uniforms, 
		PayTax.PAPitC,
		ISNULL(TaxRates.WComp, 0.00) WCompRate,
		ISNULL(TaxRates.PayProc, 0.00) PayProcRate
		FROM PayTax
		LEFT OUTER JOIN TaxRates ON PayTax.ProgramYear=TaxRates.ProgramYear
		WHERE PayTax.EndDate Between @BDt And @EDt
		) Accrual
	CROSS APPLY (SELECT (WCompRate * Gross) AS WComp ) Calcs
	GROUP BY DistrictID) AccrualTotals ON PayTax.DistrictID=AccrualTotals.DistrictID AND PayTax.EndDate=AccrualTotals.MaxOfEndDate;

	Set @RowsUpdated=@@ROWCOUNT;
END
GO

