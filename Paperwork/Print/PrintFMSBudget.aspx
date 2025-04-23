<%@ Page Title="FMS Budget Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintFMSBudget.aspx.cs" Inherits="Paperwork.PrintFMSBudget" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
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
            <div class="col-sm-3">
                <strong>Budget Year:</strong>
                <asp:DropDownList ID="DDLBudgetYears" runat="server" DataSourceID="SqlDataSourceBudgetYears" DataTextField="BudgetYear" 
                DataValueField="BUdgetYear" AutoPostBack="true" OnSelectedIndexChanged="DDLBudgetYears_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-sm-3">
                <strong>District:</strong>
                <asp:DropDownList ID="DDLDistricts" runat="server" DataTextField="Desc" DataValueField="DistrictID" 
                    OnSelectedIndexChanged="DDLDistricts_SelectedIndexChanged" AutoPostBack="true" Width="175" >
                    
                </asp:DropDownList>
            </div>
            <div class="col-sm-3">
                <strong>Version:</strong>
                <asp:TextBox ID="TextBoxBudgetVersion" runat="server" OnTextChanged="TextBoxVersion_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10" ></asp:TextBox>
            </div>
        </div>

    </div>
    <div id="report">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
            AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous" >
            <ServerReport ReportPath="FMS Budget" />
        </rsweb:ReportViewer>
        <br />
        <rsweb:ReportViewer ID="ReportViewer2" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
            AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous" >
            <ServerReport ReportPath="FMS Budget Hourly Labor" />
        </rsweb:ReportViewer>
        <br />
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceBudgetYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(BudgetYear) BudgetYear FROM [FMSBudgetYear_Setup] ORDER BY [BudgetYear] DESC">
        </asp:SqlDataSource>
    </div>
</asp:Content>
