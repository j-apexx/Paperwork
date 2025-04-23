<%@ Page Title="Participation Changes Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintParticipationChanges.aspx.cs" Inherits="Paperwork.PrintParticipationChanges" %>
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
            $(function () {
                $( "[id*=TextBoxStartDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true
                });
                $( "[id*=TextBoxEndDate]" ).datepicker({
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
            <div class="col-sm-11">
                <div class="row">
                    <div class="col-sm-3">
                        <strong>Program Year:</strong>
                        <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                        DataValueField="ProgramYear" AutoPostBack="false" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="col-sm-6">
                        <strong>Show CEP Change Records:</strong>
                        <asp:DropDownList ID="DDLRecordType" runat="server" AutoPostBack="false" Width="150">
                            <asp:ListItem Text="All Records" Value="0"></asp:ListItem>
                            <asp:ListItem Text="When Changing to CEP" Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Non CEP" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <strong>Districts</strong>
                        <asp:DropDownList ID="DDLIncludeDists" runat="server" AutoPostBack="false" Width="150">
                            <asp:ListItem Text="All Districts" Value="0"></asp:ListItem>
                            <asp:ListItem Selected="true" Text="My Districts" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
<%--                <div class="row">
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
                </div>--%>
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
            <ServerReport ReportPath="Summary Participation Changes" />
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

        <asp:SqlDataSource ID="SqlDataSourceMyRMDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT D.DistrictID, RTRIM(DistrictName) + '-' + RTRIM(D.DistrictID) 'Desc'
                FROM [District] D
                INNER JOIN DistrictYearInfo DYI ON DYI.DistrictID=D.DistrictID
                INNER JOIN vw_RMUserIDLink RMID ON DYI.RMNumber=RMID.RMNumber
                WHERE DYI.ProgramYear=@ProgramYear AND (RMID.UserID=@UserID OR (UPPER(@AccessLevel) IN ('ADMIN','OFFICE')))" DataSourceMode="DataReader">
            <SelectParameters>
                <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                <asp:SessionParameter Name="AccessLevel" SessionField="AccessLevel" Type="String" />
                <asp:ControlParameter Name="ProgramYear" ControlID="DDLProgramYears" Type="String" PropertyName="SelectedValue" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
