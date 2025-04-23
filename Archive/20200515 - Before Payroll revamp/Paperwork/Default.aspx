<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Paperwork._Default" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotronsmall">
        <h1><i class="far fa-file-alt"></i>Paperwork</h1>
        <p class="lead">Welcome to the new Paperwork website.</p>
        <p class="lead">Current Program Year:
            <asp:Label ID="LabelInventoryYear" runat="server" Text="**Not Open**"></asp:Label>

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
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Purchases.aspx" >Transmittal Entry &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="SalesEntry.aspx" >Sales &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="DistrictPayrollEntry.aspx" >District Payroll &raquo;</a>
                <%--<a class="btn btn-primary btn-sm" style="width:100%;" href="MMBEntry.aspx" >MMB Sheet &raquo;</a>--%>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="InventoryEntry.aspx" >Inventory Totals &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="OtherSales.aspx" >Enter Indirect Costs &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="SpecialFunctionsEntry.aspx" >Special Functions &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Days.aspx" >View/Modify Days &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Notifications.aspx" >Notifications &raquo;</a>
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
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintBudgetReport" >Budget Report &raquo;</a>

                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseClosingRpts" aria-expanded="true">Closing Reports</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseClosingRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseClosingRpts" style="width:100%;max-width:220px;">    
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintOperatingReport" >Operating Report &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintOperatingStatistics" id="LinkOpStatsReport" runat="server"  >Operating Statistics &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintUSDACover" >USDA Cover Sheet &raquo;</a>
                </div>

                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseEditChksRpts" aria-expanded="true">Edit Checks Rpts</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseEditChksRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseEditChksRpts" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintEditCheckBySchool" >Edit Check by School &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintEditCheckSummary" >Edit Check Summary &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintCN6" >CN-6 (OHIO)&raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintCN7" >CN-7 (OHIO)&raquo;</a>
                </div>


                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintLunchClaim" >Lunch Claim Worksheet &raquo;</a>
                <%--<a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintMMB" >MMB &raquo;</a>--%>
                <a id="linkPrintNotifications" runat="server" class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintNotifications" >Notifications &raquo;</a>
                
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintPayroll" >Payroll Report &raquo;</a>

                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseSalesRpts" aria-expanded="true">Sales Rpts</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseSalesRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseSalesRpts" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSales" >Sales - School &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSalesRevisions" >Sales - School Revisions &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSalesRevisionsDetail" >Sales - School Rev. Detail &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSalesDistrict" >Sales - District &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSnackSales" >Snacks &raquo;</a>
                </div>

                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseSpFnRpts" aria-expanded="true">Special Functions Rpts</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseSpFnRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseSpFnRpts" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSpecialFuncInvoices" >Special Functions Invoices &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintSpFnLedger" >Special Functions Ledger &raquo;</a>
                </div>

                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseTransRpts" aria-expanded="true">Transmittal Rpts</a>
                    <a class="btn btn-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseTransRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseTransRpts" style="width:100%;max-width:220px;">
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintTransmittals" >Transmittals &raquo;</a>
                    <a class="btn btn-primary btn-sm" style="width:95%;margin-left:10px;" href="Print/PrintTransmittalsOther" >Transmittals Other Costs &raquo;</a>
                </div>
                

                

            </div>
            <div id="DivMgmtReports" runat="server"  >
                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-outline-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseAdminRpts" aria-expanded="true">School Invoicing</a>
                    <a class="btn btn-outline-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseAdminRpts" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseAdminRpts" style="width:100%;max-width:220px;" >
                    <div class="card-block"><!--MGMT-->
                        <div id="mgmtrpts" class="btn-group-vertical" style="width: 100%;max-width: 220px;">
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceCostPerMeal" >Cost Per Meal Invoice &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceFullBillPurch.aspx" >Purch Invoice &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceFullBillUsage.aspx" >Usage Invoice &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceFullBillPurch.aspx" >Purch Invoice w/CPM Fees &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Print/PrintInvoiceFullBillUsage.aspx" >Usage Invoice w/CPM Fees &raquo;</a>
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
                <a class="btn btn-primary btn-sm" style="                        width: 100%;
                " href="Setup/SetupDistrictInfo.aspx" >District Info &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width: 100%;" href="Setup/SetupSchools.aspx" >Setup Schools &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupSchoolInfo.aspx" >Setup School Info &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupDistrictVendors.aspx" >Select Vendors &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupPaycodes.aspx" >Setup Paycodes &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupEmployees.aspx" >Setup Employees &raquo;</a>
                <%--<a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupMMBItems.aspx" >Setup MMB Items &raquo;</a>--%>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupMilkDelivery.aspx" >Setup Milk Delivery &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupOtherSales.aspx" >Setup Indirect Costs &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupSpFnInvoices.aspx" >Setup Sp. Function Invoices &raquo;</a>
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
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupDistricts.aspx" >Setup Districts &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupUsers.aspx" >Setup Users &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupPurchOtherCats.aspx" >Setup Purchasing Other Cats. &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupRegionalMgrs.aspx" >Setup Regional Mgrs &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupRFSCustNums.aspx" >Setup Reinhart Cust Nums &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/GlobalSettings.aspx" >Global Settings &raquo;</a>
                        
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
            <div id="DivAdminOfficeUtilsSub" runat="server" class="btn-group-vertical" style="                    width: 100%;
                    max-width: 220px;
            ">
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/ImportIndirectCosts" >Import - Indirect Costs &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/ImportInsurance" >Import - Insurance &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/ImportPayroll" >Import - Payroll &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width: 100%;" href="Utilities/ImportReinhartCredits" >Import - Reinhart Credits &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/ImportReinhart" >Import - Reinhart Invoices &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/PayablesExport" >Export - Payables &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/PayrollAccruals" >Payroll Accruals &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/LockTransactions" >Lock / Unlock Transactions &raquo;</a>
            </div>
        </div>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceInvYear" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT TOP 1 [ProgramYear], [StartDate], [EndDate], [BudgetYear] FROM [ProgramYear] WHERE ([AllowDataEntry] = @AllowDataEntry)">
            <SelectParameters>
                <asp:Parameter DefaultValue="1" Name="AllowDataEntry" Type="Byte" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
