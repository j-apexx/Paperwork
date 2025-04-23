<%@ Page Title="Full Bill Purchase Invoice" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintInvoiceFullBillPurch.aspx.cs" Inherits="Paperwork.PrintInvoiceFullBillPurch" %>
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
                <strong>Start Date:</strong>
                <asp:TextBox ID="TextBoxStartDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10" ></asp:TextBox>
                <asp:RegularExpressionValidator ID="validDate" ControlToValidate="TextBoxStartDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-3">
                <strong>End Date:</strong>
                <asp:TextBox ID="TextBoxEndDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="100" AutoCompleteType="Disabled" TextMode="DateTime" MaxLength="10"  ></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TextBoxEndDate" ErrorMessage="Invalid Date" ForeColor="Red" runat="server" ValidationExpression="^[0-9m]{1,2}/[0-9d]{1,2}/[0-9y]{4}$" Display="Dynamic">*</asp:RegularExpressionValidator>
            </div>
            <div class="col-sm-5">
                <strong>District:</strong>
                <asp:DropDownList ID="DDLDistricts" runat="server" DataSourceID="SqlDataSourceMyDistricts" DataTextField="Desc" DataValueField="DistrictID" AppendDataBoundItems="true" OnSelectedIndexChanged="DDLDistricts_SelectedIndexChanged" AutoPostBack="true">
                    <asp:ListItem Selected="False" Text="-SELECT-" Value=""></asp:ListItem>
                    <asp:ListItem Selected="False" Text="-All Districts-" Value="%"></asp:ListItem>
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
            <ServerReport ReportServerUrl="http://NU-SQL-IRW-1/reportserver_DYNAMICSGP" ReportPath="/PAPERWORKWEB/Invoice Full Bill Purch All" />
        </rsweb:ReportViewer>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceMyDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT D.DistrictID, RTRIM(D.DistrictName) + '-' + RTRIM(D.DistrictID) 'Desc'
            FROM [District] D
            INNER JOIN [DistrictYearInfo] DYI ON D.DistrictID=DYI.DistrictID
            WHERE D.DistrictID IN (SELECT DistrictID FROM [UserDistrictAccess] WHERE Userid=@UserID) OR (@AccessLevel NOT IN ('User'))
            AND DYI.ProgramYear=@ProgramYear AND DYI.AcctType=2
            ORDER BY [DistrictName]">
            <SelectParameters>
                <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                <asp:SessionParameter Name="AccessLevel" SessionField="AccessLevel" Type="String" />
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
