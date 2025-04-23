<%@ Page Title="Special Functions Ledger" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintSpFnLedger.aspx.cs" Inherits="Paperwork.PrintSpFnLedger" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        $(function () {
            BindEvents();
        });

        function BindEvents() {
            $(function () {
                $("[id*=TextBoxStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
                $("[id*=TextBoxEndDate]").datepicker({
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
            <div class="col-sm-2">
                <strong>Program Year:</strong>
                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-sm-2">
                <strong>Start Date:</strong>
                <asp:TextBox ID="TextBoxStartDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10" ></asp:TextBox>
                <asp:RegularExpressionValidator ID="validDate" ControlToValidate="TextBoxStartDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-2">
                <strong>End Date:</strong>
                <asp:TextBox ID="TextBoxEndDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10"  ></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TextBoxEndDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-2">
                <strong>Record Selection:</strong>
                <asp:DropDownList ID="DDLRecords" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DDLRecords_SelectedIndexChanged">
                    <asp:ListItem Selected="True" Text="All Invoices" Value="All"></asp:ListItem>
                    <asp:ListItem Text="3086 Invoices" Value="3086"></asp:ListItem>
                    <asp:ListItem Text="CACFP Invoices" Value="CACFP"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-sm-2">
                <strong>Paid/Unpaid:</strong>
                <asp:DropDownList ID="DDLPaidUnpaid" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DDLRecords_SelectedIndexChanged">
                    <asp:ListItem Selected="True" Text="ALL" Value="ALL"></asp:ListItem>
                    <asp:ListItem Text="PAID" Value="PAID"></asp:ListItem>
                    <asp:ListItem Text="UNPAID" Value="UNPAID"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div id="report">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
            AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous">
            <ServerReport ReportPath="Special Functions Ledger" />
        </rsweb:ReportViewer>
        <br />
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
    </div>
</asp:Content>
