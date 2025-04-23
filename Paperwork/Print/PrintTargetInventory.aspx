<%@ Page Title="Target Inventory Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintTargetInventory.aspx.cs" Inherits="Paperwork.PrintTargetInventory" %>
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
            <div class="col-sm-11">
                <div class="row">
                    <div class="col-sm-6">
                        <strong>Program Year:</strong>
                        <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                        DataValueField="ProgramYear" AutoPostBack="false" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="col-sm-6">
                        RM:
                        <asp:DropDownList ID="DropDownListRMs" runat="server" AppendDataBoundItems="True" AutoPostBack="true" DataSourceID="SqlDataSourceRMs"
                            DataTextField="RMName" DataValueField="RMNumber" OnSelectedIndexChanged="DropDownListRMs_SelectedIndexChanged"  >
                            <asp:ListItem Text="- Select -" Value="-1" Selected="True"></asp:ListItem>
                        </asp:DropDownList> 
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-1">
                        <strong>VP:</strong>
                    </div>
                    <div class="col-sm-11">
                        <asp:CheckBoxList ID="CheckBoxListVPs" runat="server" AutoPostBack="false" TextAlign="Right" 
                            DataSourceID="SqlDataSourceVPs" DataTextField="GMDESC" DataValueField="GM" RepeatColumns="8" 
                            RepeatDirection="Horizontal" RepeatLayout="Table" CssClass="cblistPad"
                             OnSelectedIndexChanged="CheckBoxListVPs_SelectedIndexChanged"  >
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>
            <div class="col-sm-1">
                <asp:LinkButton runat="server"  
                    ID="ISync" ToolTip="Refresh Report" Text=" Refresh"
                    CssClass="fas fa-sync mGridLinkButton" ForeColor="#666699" Width="100" OnClick="ISync_Click"  />
                    <div class="messagealert" id="alert_container"></div>
            </div>
        </div>

    </div>
    <div id="report">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent"
            AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous">
            <ServerReport ReportPath="Summary Target Inventory" />
        </rsweb:ReportViewer>
        <br />
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceVPs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT DISTINCT GM, IIF(GM='', ' -BLANK- ', GM) GMDESC FROM DistrictYearInfo
            INNER JOIN DISTRICT ON DistrictYearInfo.DistrictID=District.DistrictID WHERE ProgramYear=@ProgramYear AND Inactive=0 ORDER BY GMDESC">
            <SelectParameters>
                <asp:ControlParameter Name="ProgramYear" ControlID="DDLProgramYears" Type="String" PropertyName="SelectedValue" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceDists" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [DistrictID], RTRIM(DistrictName) + '-' + RTRIM(DistrictID) AS [DistrictName] 
                            FROM [District] 
                            ORDER BY DistrictName " 
            DataSourceMode="DataReader">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceRMs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT IIF(Active=0,'* ','') + RMSetup.RMName RMName ,RMSetup.RMNumber,IIF(Active=0, 'Z','')+ RMSetup.RMName RMSort FROM RMSetup
            UNION ALL
            SELECT '** All RMs **' RMName, 0 RMNumber,'** All RMs **'  RMSort
            ORDER BY RMSort">
        </asp:SqlDataSource>
    </div>
</asp:Content>
