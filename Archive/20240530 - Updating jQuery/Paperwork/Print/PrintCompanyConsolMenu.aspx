<%@ Page Title="Companywide Menu Consolidation" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintCompanyConsolMenu.aspx.cs" Inherits="Paperwork.PrintCompanyConsolMenu" %>
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
            <div class="col-sm-3">
                <strong>Program Year:</strong>
                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                DataValueField="ProgramYear" AutoPostBack="false" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-sm-3">
                <strong>End Date:</strong>
                <asp:TextBox ID="TextBoxEndDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10"  ></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TextBoxEndDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-2">
                <asp:CheckBox ID="CB_CPMOnly" runat="server" OnCheckedChanged="CB_CPMOnly_CheckedChanged" Text="CPM Only" />
            </div>
            <div class="col-sm-2">
                <asp:Label ID="labelState" runat="server" Text="State:" Font-Bold="true" Width="50"></asp:Label>
                <asp:DropDownList ID="DDLStateFilter" runat="server" DataSourceID="SqlDataSourceStates" DataTextField="DistrictState" DataValueField="StateFilter"></asp:DropDownList>
            </div>
            <div class="col-sm-2">
                <asp:LinkButton runat="server"  
                    ID="ISync" ToolTip="Refresh Report" Text=" Refresh"
                    CssClass="fas fa-sync mGridLinkButton" ForeColor="#666699" Width="120" OnClick="ISync_Click"  />
                    <div class="messagealert" id="alert_container"></div>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-2">
                <strong>VP:</strong>

            </div>
            <div class="col-sm-10">
                <asp:CheckBoxList ID="CheckBoxListVPs" runat="server" AutoPostBack="false" TextAlign="Right" 
                    DataSourceID="SqlDataSourceVPs" DataTextField="GMDESC" DataValueField="GM" RepeatColumns="8" 
                    RepeatDirection="Horizontal" RepeatLayout="Table" CssClass="cblistPad"
                    >
                </asp:CheckBoxList>
            </div>
        </div>
            
    </div>
    <div id="report">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
            AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous">
            <ServerReport ReportPath="Summary Companywide Menu Consol" />
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
        <asp:SqlDataSource ID="SqlDataSourceStates" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT '-ALL-' DistrictState, '*' StateFilter
                UNION ALL
                SELECT DISTINCT DistrictState, DistrictState StateFilter FROM District WHERE DistrictState<>'' ORDER BY DistrictState" 
            DataSourceMode="DataReader">
        </asp:SqlDataSource>
    </div>
</asp:Content>
