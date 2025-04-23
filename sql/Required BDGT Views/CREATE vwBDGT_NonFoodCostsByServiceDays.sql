
/****** Object:  View [dbo].[vwBDGT_NonFoodCostsBySvcDays]    Script Date: 3/12/2020 12:07:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwBDGT_NonFoodCostsBySvcDays]
AS
SELECT FC.[BidNumber]
      ,FC.[BudgetYear]
      ,[DistrictTotal]
	  ,TotalServiceDays
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS1],2) DistJul
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS2],2) DistAug
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS3],2) DistSep
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS4],2) DistOct
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS5],2) DistNov
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS6],2) DistDec
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS7],2) DistJan
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS8],2) DistFeb
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS9],2) DistMar
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS10],2) DistApr
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS11],2) DistMay
	  ,ROUND((DistrictTotal / TotalServiceDays) * [SERVICEDAYS12],2) DistJun
  FROM [vwBDGT_NonFoodCost] FC
  INNER JOIN BDGT_BudgetHDR HDR ON HDR.BidNumber=FC.BidNUmber AND HDR.BudgetYear=FC.BudgetYear AND HDR.BudgetVersion=FC.BudgetVersion
  INNER JOIN [vwBDGT_ScheduleDays] Sch ON FC.BidNumber=SCH.BidNumber AND FC.BudgetYear=Sch.BudgetYear
  CROSS APPLY (SELECT [SERVICEDAYS1]+[SERVICEDAYS2]+[SERVICEDAYS3]+[SERVICEDAYS4]+[SERVICEDAYS5]+[SERVICEDAYS6]+
  [SERVICEDAYS7]+[SERVICEDAYS8]+[SERVICEDAYS9]+[SERVICEDAYS10]+[SERVICEDAYS11]+[SERVICEDAYS12] TotalServiceDays ) CalcDays
  WHERE DistrictTotal<>0 AND ACTIVE=1
GO


