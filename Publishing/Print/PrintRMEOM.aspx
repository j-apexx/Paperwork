<%@ Page Title="RM EOM Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintRMEOM.aspx.cs" Inherits="Paperwork.PrintRMEOM" %>
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
            <div class="col-sm-2">
                <strong>Program Year:</strong>
                <br />
                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-sm-2">
                <strong>Start Date:</strong>
                <br />
                <asp:TextBox ID="TextBoxStartDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10" ></asp:TextBox>
                <asp:RegularExpressionValidator ID="validDate" ControlToValidate="TextBoxStartDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-2">
                <strong>End Date:</strong>
                <br />
                <asp:TextBox ID="TextBoxEndDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10"  ></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TextBoxEndDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-2">
                <strong>RM:</strong>
                <br />
                <asp:DropDownList ID="DDLRMs" runat="server" DataTextField="RMName" DataValueField="RMNumber" DataSourceID="SqlDataSourceRMs"
                    OnSelectedIndexChanged="DDLRMs_SelectedIndexChanged" AutoPostBack="true" Width="175" >
                </asp:DropDownList>
            </div>
            <div class="col-sm-2">
                <strong>VP:</strong>
                <br />
                <asp:DropDownList ID="DDLVPs" runat="server" DataSourceID="SqlDataSourceGMs" DataTextField="GMName" DataValueField="GMValue" OnSelectedIndexChanged="DDLVPs_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
            </div>
        </div>

    </div>
    <div id="email">
        Email report to VP: 
        <asp:Button ID="btnSend" runat="server" Text="Send" OnClick="btnSend_Click" Enabled="true" />
        <asp:HiddenField ID="HFSSRSBaseFolder2" runat="server" />
        <asp:HiddenField ID="HFRMEmail2" runat="server" />
        <rsweb:ReportViewer ID="ReportViewer2" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="false" ShowDocumentMapButton="False" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
            AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous">
            <ServerReport ReportPath="RMEOM(New)"  />
        </rsweb:ReportViewer>
    </div>
    <div id="report">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
            AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous" >
            <ServerReport ReportPath="RMEOM(New)" />
        </rsweb:ReportViewer>
        <br />
    </div>

    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceRMs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RMSetup.RMName ,RMSetup.RMNumber FROM RMSetup
            UNION ALL
            SELECT '** All RMs **' RMName, 0 RMNumber
            UNION ALL
            SELECT '-- SELECT --' RMName, -1 RMNumber
            ORDER BY RMSetup.RMName">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceEmailSettings" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT * FROM [PaperworkWebEmailSettings] WHERE ([Index] = @Index)">
            <SelectParameters>
                <asp:Parameter DefaultValue="0" Name="Index" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceGMs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT '** All VPs **' GMName, '*' GMValue, '' GMEmail
            UNION ALL            
            SELECT DISTINCT GM.[GM] GMName,GM.[GM] GMValue, GM.[GMEmail] GMEmail
                FROM [GMSetup] GM
                INNER JOIN DistrictYearInfo DYI ON GM.GM=DYI.GM AND DYI.ProgramYear=@ProgramYear">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

    </div>
</asp:Content>
