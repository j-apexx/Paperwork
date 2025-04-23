<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Paperwork._Default" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotronsmall">
        <h1>
            <asp:Literal ID="LiteralHDRGeneralPaperwork" runat="server" Text="<i class='fas fa-file-alt'></i>Paperwork" Visible="true"></asp:Literal>
            <asp:Literal ID="LiteralHDRGeneralToolbox" runat="server" Text="<i class='fas fa-toolbox'></i>Toolbox" Visible="false"></asp:Literal>
            &nbsp;
            <asp:Label ID="LabelTrainIndicator" runat="server"></asp:Label>

        </h1>
        
        <asp:Literal ID="LiteralGeneralPaperwork" runat="server" Text="<p class='lead'>Welcome to the Paperwork website.</p>" ></asp:Literal>

        <asp:Literal ID="LiteralTrainingMessage" runat="server" Text="<p class='lead'>**Do not use this site to enter live data!**</p>" Visible="false"></asp:Literal>
        <asp:Literal ID="LiteralSFSPMessage" runat="server" Text="<p class='lead'>Summer Food Service Program</p>" Visible="false"></asp:Literal>
        <asp:Literal ID="LiteralToolboxMessage" runat="server" Text="<p class='lead'>Welcome to Toolbox.</p>" Visible="false"></asp:Literal>
        
        <p class="lead">Latest Active Program Year:
            <asp:Label ID="LabelInventoryYear" runat="server" Text="**Not Open**"></asp:Label>

        </p>
        <p>
            Selected Program Year:
            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
              DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
        </p>
        <p>
            District:
            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                DataTextField="Desc" DataValueField="DistrictID" OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
            </asp:DropDownList>
        </p>
    </div>

    <div class="row">
        <div class="col-md-3">
            <div class="card" style="max-width:220px;">
                <div class="card-header">
                    <h5>Transactions</h5>
                    Enter transmittals, sales and other transactions.
                </div>
            </div>
            <div class="btn-group-vertical" style="width:100%;max-width:220px;">
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Purchases.aspx" id="link_Purchases" runat="server" >Transmittal Entry &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="SalesEntry.aspx" id="link_SalesEntry" runat="server" >Sales &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="DistrictPayrollEntry.aspx" id="link_DistrictPayrollEntry" runat="server" >District Payroll &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="InventoryEntry.aspx"  id="link_InventoryEntry" runat="server">Inventory Totals &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="InventoryVerificationSFSP.aspx"  id="link_InventoryVerificationSFSP" runat="server">Inventory Verification SFSP &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="OtherSales.aspx"  id="link_OtherSales" runat="server">Enter Indirect Costs &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="SpecialFunctionsEntry.aspx"  id="link_SpecialFunctionsEntry" runat="server">Special Functions &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Days.aspx"  id="link_Days" runat="server">View/Modify Days &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Notifications.aspx"  id="link_Notifications" runat="server">Notifications &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="EOMChecklist.aspx"  id="link_EOMChecklist" runat="server">EOM Checklist &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="MenuAnalysisEntry.aspx"  id="link_MenuAnalysisEntry" runat="server">Menu Analysis &raquo;</a>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card" style="max-width:220px;">
                <div class="card-header">
                    <h5>Reports</h5>
                    Run reports for your district.
                </div>
            </div>
            <div class="btn-group-vertical" style="width:100%;max-width:220px;">
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintBudgetReport" runat="server" id="link_PrintBudgetReport" >Budget Report &raquo;</a>

                <div class="btn-group" style="width:100%;max-width:220px;" runat="server" id="DivClosingReports">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseClosingRpts" aria-expanded="true">Closing Reports</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseClosingRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseClosingRpts" style="width:100%;max-width:220px;">    
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintOperatingReportAll" runat="server" id="link_PrintOperatingReport" >Operating Report &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintOperatingStatistics" id="link_PrintOperatingStatistics" runat="server"  >Operating Statistics &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintCompanyFoodCost" id="link_PrintCompanyFoodCost" runat="server"  >Companywide Food Cost &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintCompanyConsolMenu" id="link_PrintCompConsolMenu" runat="server"  >Menu Consolidation &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintUSDACover" runat="server" id="link_PrintUSDACover" >USDA Cover Sheet &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintDetailedProfitCPM" id="link_PrintDetailedProfitCPM" runat="server" >Detailed Profit Rpt CPM &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintDetailedProfitFB" id="link_PrintDetailedProfitFB" runat="server" >Detailed Profit Rpt FB &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintEOMChecklist" id="link_PrintEOMChecklist" runat="server" >EOM Checklist &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintInventoryDistTotals" id="link_PrintInventoryDistTotals" runat="server" >Inventory Change Dists &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintInventoryVerification" id="link_PrintInventoryVerification" runat="server" >Inventory Verification SFSP &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintPurchasesSummary" id="link_PrintPurchasesSummary" runat="server" >Purchases Summary &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintReinhartCredits" id="link_PrintReinhartCredits" runat="server" >Reinhart Credits &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintUsageSummary" id="link_PrintUsageSummary" runat="server" >Usage Summary &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintUSDACPM" id="link_PrintUSDACPM" runat="server" >USDA CPM Consolidated &raquo;</a>
                </div>

                <div class="btn-group" style="width:100%;max-width:220px;" runat="server" id="DivEditCheckReports">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseEditChksRpts" aria-expanded="true">Edit Checks Rpts</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseEditChksRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseEditChksRpts" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintEditCheckBySchool" runat="server" id="link_PrintEditCheckBySchool" >Edit Check by School &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintEditCheckSummary" runat="server" id="link_PrintEditCheckSummary"  >Edit Check Summary &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintCN6" runat="server" id="link_PrintCN6"  >CN-6 (OHIO)&raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintCN7" runat="server" id="link_PrintCN7"  >CN-7 (OHIO)&raquo;</a>
                </div>

                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintLunchClaim" runat="server" id="link_PrintLunchClaim">Lunch Claim Worksheet &raquo;</a>
                <a id="link_PrintNotifications" runat="server" class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintNotifications" >Notifications &raquo;</a>

                <div class="btn-group" style="width:100%;max-width:220px;" runat="server" id="DivPayrollReports">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapsePayrollRpts" aria-expanded="true">Payroll Rpts</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapsePayrollRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapsePayrollRpts" style="width:100%;max-width:220px;" >
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintPayroll" runat="server" id="link_PrintPayroll" >Payroll Report &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintPayrollHealth" runat="server" id="link_PrintPayrollHealth" >Payroll Health Report &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintPayrollDetail" id="link_PrintPayrollDetail" runat="server" >TNG Payroll Report &raquo;</a>
                </div>

                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintRMEOM" id="link_PrintRMEOM" runat="server"  >RM EOM Report &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="RMEOMComments" id="link_RMEOMComments" runat="server"  >RM EOM Explanations &raquo;</a>

                <div class="btn-group" style="width:100%;max-width:220px;" runat="server" id="DivSalesReports">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseSalesRpts" aria-expanded="true">Sales Rpts</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseSalesRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseSalesRpts" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSales" id="link_PrintSales" runat="server" >Sales - School &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSalesRevisions" id="link_PrintSalesRevisions" runat="server" >Sales - School Revisions &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSalesRevisionsDetail" id="link_PrintSalesRevisionsDetail" runat="server" >Sales - School Rev. Detail &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSalesDistrict" id="link_PrintSalesDistrict" runat="server" >Sales - District &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSnackSales" id="link_PrintSnackSales" runat="server" >Snacks &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSalesAdultLunch" id="link_PrintSalesAdultLunch" runat="server" >Sales - Adult Lunch &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSalesDinner" id="link_PrintSalesDinner" runat="server" >Sales - Dinner &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSalesNutrForLife" id="link_PrintSalesNutrForLife" runat="server" >Sales - Nutr For life &raquo;</a>
                </div>

                <div class="btn-group" style="width:100%;max-width:220px;" runat="server" id="DivSpecialFunctionReports">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseSpFnRpts" aria-expanded="true">Special Functions Rpts</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseSpFnRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseSpFnRpts" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSpecialFuncInvoices" id="link_PrintSpecialFuncInvoices" runat="server" >Special Functions Invoices &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSpFnLedger" id="link_PrintSpFnLedger" runat="server" >Special Functions Ledger &raquo;</a>
                </div>

                <div class="btn-group" style="width:100%;max-width:220px;" runat="server" id="DivTransmittalReports">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseTransRpts" aria-expanded="true">Transmittal Rpts</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseTransRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseTransRpts" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintAddOnFeesInvoice" id="link_PrintAddOnFeesInvoice" runat="server" >Add-On Fees Invoice &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintTransmittals" id="link_PrintTransmittals" runat="server" >Transmittals &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintTransmittalsOther" id="link_PrintTransmittalsOther" runat="server" >Transmittals Other Costs &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintOHCatTaxInv" id="link_PrintOHCatTaxInv" runat="server" >Ohio CAT Tax Invoices &raquo;</a>
                </div>
                <a id="link_PrintSFSPDistsSummary" runat="server" class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintSummarySFSPDists" >Summary SFSP Districts &raquo;</a>


            </div>
            <div id="DivMgmtReports" runat="server"  >
                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-outline-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseAdminRpts" aria-expanded="true">School Invoicing</a>
                    <a class="btn btn-outline-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseAdminRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseAdminRpts" style="width:100%;max-width:220px;" >
                    <div class="card-block"><!--MGMT-->
                        <div id="mgmtrpts" class="btn-group-vertical" style="width:100%;max-width: 220px;">
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Utilities/CostPerMealInvoices" >Create CPM Invoices &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceCostPerMeal" >Print CPM Invoices &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintCPMInvoiceSummary" >CPM Invoice Summary &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintCPMPLReport" >Print CPM PL Report &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceFullBillPurch.aspx" >Purch Invoice &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceFullBillUsage.aspx" >Usage Invoice &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoicePacket.aspx" >Invoice Packet &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceFullBillPurchwCPMFees.aspx" >Purch Invoice w/CPM Fees &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceFullBillUsagewCPMFees.aspx" >Usage Invoice w/CPM Fees &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintFullBillSummary.aspx" >Full Bill Summary &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintSchoolInvAndFees.aspx" >School Invoices and Fees &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintCOVIDCPMSales.aspx" >COVID CPM Sales &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintDiscountsSummary.aspx" >Discounts Summary &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintFFVDistInvoiceTotals.aspx" >FFV Dists Summary &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintSummaryReimVerif.aspx" >Reimb. Verification Summ. &raquo;</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card" style="max-width:220px;">
                <div class="card-header">
                    <h5>Settings</h5>
                    Application and district settings.
                </div>
            </div>
            <div class="btn-group-vertical" style="width:100%;max-width:220px;">
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupDistrictInfo.aspx" runat="server" id="link_SetupDistrictInfo" >District Info &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupSchools.aspx" runat="server" id="link_SetupSchools" >Setup Schools &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupSchoolInfo.aspx" runat="server" id="link_SetupSchoolInfo" >Setup School Info &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupDistrictVendors.aspx" runat="server" id="link_SetupDistrictVendors" >Select Vendors &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupPaycodes.aspx" runat="server" id="link_SetupPaycodes" >Setup Paycodes &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupEmployees.aspx" runat="server" id="link_SetupEmployees" >Setup Employees &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupMenuAnalysisItems.aspx" runat="server" id="link_SetupMenuAnalysisItems" >Setup Menu Analysis Items&raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupMilkDelivery.aspx" runat="server" id="link_SetupMilkDelivery" >Setup Milk Delivery &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupOtherSales.aspx" runat="server" id="link_SetupOtherSales" >Setup Indirect Costs &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupSpFnInvoices.aspx" runat="server" id="link_SetupSpFnInvoices" >Setup Sp. Function Invoices &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupSpFnOrganizations.aspx" runat="server" id="link_SetupSpFnOrgs" >Setup Sp. Fn. Organizations &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupTransmittalOptions.aspx" runat="server" id="link_SetupTransOpts" >Setup Transmittal Options &raquo;</a>
            </div>
            <div id="DivAdminSetup" runat="server"  >
                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-outline-primary btn-sm" style="width: 100%;" data-toggle="collapse" href="#collapseExample" aria-expanded="true">Admin Setups</a>
                    <a class="btn btn-outline-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseExample" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseExample" >
                    <div class="card-block"><!--ADMIN-->
                        <div id="admin" class="btn-group-vertical" style="width:100%;max-width:220px;">
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupProgramYear.aspx" >Setup Program Years &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupTaxRates.aspx" >Setup Tax Rates &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupDistricts.aspx" >Setup Districts &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupDistrictsInvTerms.aspx" >Setup Dists Inv Terms &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupUsers.aspx" >Setup Users &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupMenuAnalysisLevels.aspx" >Setup Menu Analysis Lvls &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupProgramTypes.aspx" >Setup Program Types &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupPurchOtherCats.aspx" >Setup Purch. Other Cats. &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupRegionalMgrs.aspx" >Setup Regional Mgrs &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupGallikerCustNums.aspx" >Setup Galliker Cust Nums &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupMorabitoCustNums.aspx" >Setup Morabito Cust Nums &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupRFSCustNums.aspx" >Setup Reinhart Cust Nums &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupVendorCustNums.aspx" >Setup Vendor Cust Nums &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupVPs.aspx" >Setup VPs &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/GlobalSettings.aspx" >Global Settings &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupEmail.aspx" >Email Settings &raquo;</a>
                        
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-3" id="DivAdminOfficeUtils" runat="server">
            <div class="card" style="max-width:220px;">
                <div class="card-header">
                    <h5>Utilities</h5>
                    Imports, Exports and other utils.
                </div>
            </div>
            <div id="DivUserOnlyUtils" runat="server" class="btn-group-vertical" style="width: 100%; max-width: 220px;">
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/ImportPOS" runat="server" id="link_ImportPOSUsr" >Import - POS &raquo;</a>
            </div>
            <div id="DivAdminOfficeUtilsSub" runat="server" class="btn-group-vertical" style="width: 100%; max-width: 220px;">
                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseExports" aria-expanded="true">Exports</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseExports" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseExports" style="width:100%;max-width:220px;">
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ExportFBPassThru" runat="server" id="link_ExportFBPassThru" >Export - FB Pass Thru &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ExportInvChange" runat="server" id="link_ExportInvChange" >Export - Inventory Change &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/PayablesExport" runat="server" id="link_PayablesExport" >Export - Payables &raquo;</a>
                </div>
                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseImports" aria-expanded="true">Imports</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseImports" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseImports" style="width:100%;max-width:220px;">
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportAddonFees" runat="server" id="link_ImportAddonFees" >Import - Add-on Fees &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportExtraChef" runat="server" id="link_ImportExtraChef" >Import - ExtraChef Invs. &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportGalliker" runat="server" id="link_ImportGalliker" >Import - Galliker Milk &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportIndirectCosts" runat="server" id="link_ImportIndirectCosts" >Import - Indirect Costs &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportInsurance" runat="server" id="link_ImportInsurance" >Import - Insurance &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportMorabito" runat="server" id="link_ImportMorabito" >Import - Morabito &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportPayroll" runat="server" id="link_ImportPayroll" >Import - Payroll &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportPOS" runat="server" id="link_ImportPOSMgt" >Import - POS &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportReinhartCredits" runat="server" id="link_ImportReinhartCredits" >Import - Reinhart Credits &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportReinhart" runat="server" id="link_ImportReinhart" >Import - Reinhart Invoices &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/ImportTransmittalsGeneric" runat="server" id="link_ImportTransmittalsGeneric" >Import - Transmittals &raquo;</a>
                </div>
                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseInvoiceUpdates" aria-expanded="true">Invoice Updates</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseInvoiceUpdates" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseInvoiceUpdates" style="width:100%;max-width:220px;">
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/InvoiceCredits" runat="server" id="link_InvoiceCredits" >FB Invoice Credits &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/RMAdjustments" runat="server" id="link_RMAdjustments" >FB Reim. Adjustments &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/FBPrepays" runat="server" id="link_FBPrepays" >FB Prepayments &raquo;</a>
                    <a class="btn btn-outline-primary btn-sm" style="width:95%;margin-left:10px;" href="Utilities/InvAdjustments" runat="server" id="link_InvAdj" >Inventory Adjustments &raquo;</a>
                </div>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/LockTransactions" runat="server" id="link_LockTransactions" >Lock / Unlock Transactions &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/PayrollAccruals" runat="server" id="link_PayrollAccruals" >Payroll Accruals &raquo;</a>
            </div>
        </div>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvYear" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT TOP 1 [ProgramYear], [StartDate], [EndDate], [BudgetYear] FROM [ProgramYear] WHERE ([AllowDataEntry] = @AllowDataEntry) ORDER BY StartDate DESC">
            <SelectParameters>
                <asp:Parameter DefaultValue="1" Name="AllowDataEntry" Type="Byte" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
