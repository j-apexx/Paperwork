<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Paperwork._Default" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotronsmall">
        <h1><i class="far fa-file-alt"></i>Paperwork</h1>
        <p class="lead">Welcome to the new Paperwork website.</p>
        <p class="lead">Current Program Year:
            <asp:Label ID="LabelInventoryYear" runat="server" Text="**Not Open**"></asp:Label>
            <asp:SqlDataSource ID="SqlDataSourceInvYear" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                SelectCommand="SELECT TOP 1 [ProgramYear], [StartDate], [EndDate], [BudgetYear] FROM [ProgramYear] WHERE ([AllowDataEntry] = @AllowDataEntry)">
                <SelectParameters>
                    <asp:Parameter DefaultValue="1" Name="AllowDataEntry" Type="Byte" />
                </SelectParameters>
            </asp:SqlDataSource>
        </p>
        <p>
            District:
            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                DataSourceID="SqlDataSourceMyDistricts" DataTextField="Desc" DataValueField="DistrictID" OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
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
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Purchases.aspx" >Purchases &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="SalesEntry.aspx" >Sales &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="DistrictPayrollEntry.aspx" >District Payroll &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="MMBEntry.aspx" >MMB Sheet &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="InventoryEntry.aspx" >Inventory Totals &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="OtherSales.aspx" >Other Food Fund Expenses &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="SpecialFunctionsEntry.aspx" >Special Functions &raquo;</a>
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
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintDistrictInvoice" >District Invoice &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintMMB" >MMB &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintSales" >Sales - School &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintSalesDistrict" >Sales - District &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintTransmittals" >Transmittals &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Print/PrintUSDACover" >USDA Cover Sheet &raquo;</a>
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
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupDistrictInfo.aspx" >District Info &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupSchools.aspx" >Setup Schools &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupSchoolInfo.aspx" >Setup School Info &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupDistrictVendors.aspx" >Select Vendors &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupPaycodes.aspx" >Setup Paycodes &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupEmployees.aspx" >Setup Employees &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupMMBItems.aspx" >Setup MMB Items &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Setup/SetupOtherSales.aspx" >Setup Other Sales &raquo;</a>
            </div>
            <div id="DivAdminSetup" runat="server"  >
                <div class="btn-group" style="width:100%;max-width:220px;">
                    <a class="btn btn-outline-primary btn-sm" style="width:100%;" data-toggle="collapse" href="#collapseExample" aria-expanded="true">Admin Setups</a>
                    <a class="btn btn-outline-primary btn-sm dropdown-toggle" data-toggle="collapse" href="#collapseExample" aria-expanded="true" ></a>
                </div>
                <div class="collapse" id="collapseExample" >
                    <div class="card-block"><!--ADMIN-->
                        <div id="admin" class="btn-group-vertical" style="width:100%;max-width:220px;">
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupProgramYear.aspx" >Setup Program Years &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupDistricts.aspx" >Setup Districts &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupUsers.aspx" >Setup Users &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupPurchOtherCats.aspx" >Setup Purchasing Other Cats. &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/SetupSpFnInvoices.aspx" >Setup Sp. Function Invoices &raquo;</a>
                            <a class="btn btn-outline-primary btn-sm" style="width:100%;" href="Setup/GlobalSettings.aspx" >Global Settings &raquo;</a>
                        
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card" style="max-width:220px;">
                <div class="card-header">
                    <h5>Utilities</h5>
                    Imports, Exports and other utils.
                </div>
            </div>
            <div id="DivAdminOfficeUtils" runat="server" class="btn-group-vertical" style="width:100%;max-width:220px;">
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/ImportReinhart" >Import - Reinhart &raquo;</a>
                <a class="btn btn-primary btn-sm" style="width:100%;" href="Utilities/PayablesExport" >Export - Payables &raquo;</a>
            </div>
        </div>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceMyDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc'
            FROM [District] WHERE DistrictID IN (SELECT DistrictID FROM [UserDistrictAccess] WHERE Userid=@UserID) OR (@AccessLevel<>'User')
            ORDER BY [DistrictName]">
            <SelectParameters>
                <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                <asp:SessionParameter Name="AccessLevel" SessionField="AccessLevel" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
