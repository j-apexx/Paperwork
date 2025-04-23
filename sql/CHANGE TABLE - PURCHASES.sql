USE COMMISSARY

ALTER TABLE dbo.Purchases ADD
	ImpToCommissaryweb bit NOT NULL DEFAULT 0,
	ImpToCommissarywebDate datetime NOT NULL DEFAULT 1/1/1900,
	ExpToAcctng bit NOT NULL DEFAULT 0,
	ExpToAcctngDate datetime NOT NULL DEFAULT 1/1/1900,
	UserLastModified nvarchar(30) NOT NULL DEFAULT ''

	GO

USE PAPERWORKWEB

ALTER TABLE dbo.Purchases ADD
	ImpToPaperwork bit NOT NULL CONSTRAINT DF_Purchases_ImpToPaperwork DEFAULT 0,
	ImpToPaperworkDate datetime NOT NULL CONSTRAINT DF_Purchases_ImpToPaperworkDate DEFAULT 1/1/1900,
	ExpToAcctng bit NOT NULL CONSTRAINT DF_Purchases_ExpToAcctng DEFAULT 0,
	ExpToAcctngDate datetime NOT NULL CONSTRAINT DF_Purchases_ExpToAcctngDate DEFAULT 1/1/1900,
	UserLastModified nvarchar(30) NOT NULL CONSTRAINT DF_Purchases_UserLastModified DEFAULT ''

GO

USE PAPERWORKWEBTRAIN

ALTER TABLE dbo.Purchases ADD
	ImpToPaperwork bit NOT NULL CONSTRAINT DF_Purchases_ImpToPaperwork DEFAULT 0,
	ImpToPaperworkDate datetime NOT NULL CONSTRAINT DF_Purchases_ImpToPaperworkDate DEFAULT 1/1/1900,
	ExpToAcctng bit NOT NULL CONSTRAINT DF_Purchases_ExpToAcctng DEFAULT 0,
	ExpToAcctngDate datetime NOT NULL CONSTRAINT DF_Purchases_ExpToAcctngDate DEFAULT 1/1/1900,
	UserLastModified nvarchar(30) NOT NULL CONSTRAINT DF_Purchases_UserLastModified DEFAULT ''

GO