SELECT HDR.ProgramYear, HDR.DistrictID
,Hdrb.InvNumberOnly
,STUFF((
    SELECT ', ' + [OrgBilled]  
    FROM [SPFNInvoiceHDR] 
    --WHERE (ID = Results.ID) 
    FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)')
  ,1,2,'') AS BilledOrgs
,MAX(FunctionDate) FunctionDate
,MAX(BillingDate)BillingDate
FROM
[dbo].[SPFNInvoiceHDR] HDR
CROSS APPLY (SELECT  LEFT( SUBSTRING(InvoiceNumber, PATINDEX('%[0-9]%', InvoiceNumber), LEN(InvoiceNumber)-PATINDEX('%[0-9]%', InvoiceNumber))
	,PATINDEX( '%[^0-9]%', SUBSTRING(InvoiceNumber, PATINDEX('%[0-9]%', InvoiceNumber), LEN(InvoiceNumber)-PATINDEX('%[0-9]%', InvoiceNumber))+'t'  )-1 ) InvNumberOnly ) HdrB
GROUP BY HDR.ProgramYear, HDR.DistrictID, Hdrb.InvNumberOnly