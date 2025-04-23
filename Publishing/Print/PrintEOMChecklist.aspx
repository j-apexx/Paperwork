<%@ Page Title="EOM Checklist" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintEOMChecklist.aspx.cs" Inherits="Paperwork.PrintEOMChecklist" %>
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

        $(function () {
            BindEvents();
        });

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
                <strong>Program Year:</strong>
                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-sm-3">
                <strong>District:</strong>
                <asp:DropDownList ID="DDLDistricts" runat="server" DataTextField="Desc" DataValueField="DistrictID" 
                    OnSelectedIndexChanged="DDLDistricts_SelectedIndexChanged" AutoPostBack="true" Width="175" >
                    
                </asp:DropDownList>
            </div>
            <div class="col-sm-5">
                <strong>Inventory Year-Month:</strong>
                <asp:DropDownList ID="DDLInvYearMonths" runat="server" DataSourceID="SqlDataSourceInvMonths" AutoPostBack="true" 
                    DataTextField="YearMOnth" DataValueField="YearMonth" OnSelectedIndexChanged="DDLInvYearMonths_SelectedIndexChanged" AppendDataBoundItems="false">

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
            <ServerReport ReportPath="EOM Checklist" />
        </rsweb:ReportViewer>
        <br />
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceInvMonths" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="DECLARE @MinDate DATE ,@MaxDate DATE ;
            SELECT @MinDate=StartDate, @MaxDate=EndDate FROM [dbo].[ProgramYear] WHERE ProgramYear=@ProgramYear

            SELECT  TOP (DATEDIFF(MONTH, @MinDate, @MaxDate) + 1)
                    MonthStart = DATEADD(MONTH, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MinDate),
		            CAST(YEAR(DATEADD(MONTH, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MinDate)) AS VARCHAR(4)) + '-' + RIGHT('0' + CAST(MONTH(DATEADD(MONTH, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MinDate)) AS VARCHAR(2)),2) YEARMONTH
            FROM    sys.all_objects a
            CROSS JOIN sys.all_objects b;">
            <SelectParameters>
                <asp:ControlParameter Name="ProgramYear" ControlID="DDLProgramYears" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
    </div>
</asp:Content>
