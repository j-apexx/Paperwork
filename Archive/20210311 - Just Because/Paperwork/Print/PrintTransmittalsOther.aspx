<%@ Page Title="Transmittals Other Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintTransmittalsOther.aspx.cs" Inherits="Paperwork.PrintTransmittalsOther" %>
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

        $(function () {
            BindEvents();
        })

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
            <div class="col-sm-3">
                <strong>Program Year:</strong>
                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-sm-3">
                <strong>Start Date:</strong>
                <asp:TextBox ID="TextBoxStartDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10" ></asp:TextBox>
                <asp:RegularExpressionValidator ID="validDate" ControlToValidate="TextBoxStartDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-3">
                <strong>End Date:</strong>
                <asp:TextBox ID="TextBoxEndDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10"  ></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TextBoxEndDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-3">
                <strong>Program Type:</strong>
                <asp:DropDownList ID="DDLProgramTypes" runat="server" DataSourceID="SqlDataSourceProgramTypes" DataTextField="ProgramTypeLabel" 
                DataValueField="ProgramType" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramTypes_SelectedIndexChanged"></asp:DropDownList>
            </div>
        </div>
    </div>
    <div id="report">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
            AsyncRendering="False" InteractivityPostBackMode="AlwaysSynchronous">
            <ServerReport ReportPath="Transmittals Other Costs" />
        </rsweb:ReportViewer>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramTypes" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
         SelectCommand="SELECT '%' AS ProgramType, '-All Program Types-' AS ProgramTypeLabel
            UNION ALL
            SELECT ProgramType, ProgramType AS ProgramTypeLabel FROM PURCHASES WHERE ProgramYear=@ProgramYear AND
            DistrictID=@DistrictID AND PostDate>=@PostDateBeginning AND PostDate<=@PostDateEnding GROUP BY ProgramType ORDER BY ProgramTypeLabel">
            <SelectParameters>
                <asp:ControlParameter Name="ProgramYear" ControlID="DDLProgramYears" Type="String" PropertyName="SelectedValue" />
                <asp:SessionParameter Name="DistrictID"  SessionField="CurrentDistrict" Type="String" />
                <asp:ControlParameter Name="PostDateBeginning" ControlID="TextBoxStartDate" Type="DateTime" />
                <asp:ControlParameter Name="PostDateEnding" ControlID="TextBoxEndDate" Type="DateTime" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>

</asp:Content>


