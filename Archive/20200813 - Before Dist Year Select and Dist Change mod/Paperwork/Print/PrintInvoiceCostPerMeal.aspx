<%@ Page Title="Cost Per Meal Invoice" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintInvoiceCostPerMeal.aspx.cs" Inherits="Paperwork.PrintCostPerMealInvoice" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        }; 

        $( function() {
            BindEvents();
        } );

        function BindEvents() {
            $(function () {
                $( "[id*=TextBoxStatementDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
        };

        //When an input is focused - select the entire text
        //also if focused field is not one of the date fields - hide the calendar
        $('body').on('focus', 'input', function (e) {
            $(this)
                .one('mouseup', function () {
                    $(this).select();
                    return false;
                })
                .select();

        });
    </script>
    <div id="header">
        <div class="row">
            <div class="col-sm-12">
                <strong>Enter Invoice Number to display a specific invoice:</strong>
                <asp:TextBox ID="TextBoxInvoiceNumber" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10" ></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-12">
                <strong>Enter Statement date to display all invoices with the same statement date:</strong>
                <asp:TextBox ID="TextBoxStatementDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10"  ></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TextBoxStatementDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
                &nbsp;
                <strong>District</strong>
                <asp:DropDownList ID="DDLDistrict" runat="server" DataSourceID="SqlDataSourceMyDistricts" DataTextField="Desc" 
                    DataValueField="DistrictID" AutoPostBack="true" OnSelectedIndexChanged="DDLDistrict_SelectedIndexChanged" ></asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-12">
                <strong>Show Prepay Discount?</strong>
                <asp:DropDownList ID="DDLPrepay" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DDLPrepay_SelectedIndexChanged">
                    <asp:ListItem Text="No" Value="No" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Yes" Value="Yes" ></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div id="report">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="True" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
            AsyncRendering="False" InteractivityPostBackMode="AlwaysSynchronous">
            <ServerReport ReportPath="Invoice Cost Per Meal All" />
        </rsweb:ReportViewer>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceMyDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT '' as DistrictID, '-All Districts-' as 'Desc', '' as DistrictName
            UNION ALL
            SELECT D.DistrictID, RTRIM(D.DistrictName) + '-' + RTRIM(D.DistrictID) 'Desc', DistrictName
            FROM [District] D
            INNER JOIN [DistrictYearInfo] DYI ON D.DistrictID=DYI.DistrictID AND DYI.ProgramYear=@ProgramYear
            WHERE D.DistrictID IN (SELECT DistrictID FROM [UserDistrictAccess] WHERE Userid=@UserID) OR (@AccessLevel NOT IN ('User'))
            AND DYI.ProgramYear=@ProgramYear AND DYI.AcctType=1
            ORDER BY [DistrictName]">
            <SelectParameters>
                <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                <asp:SessionParameter Name="AccessLevel" SessionField="AccessLevel" Type="String" />
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
