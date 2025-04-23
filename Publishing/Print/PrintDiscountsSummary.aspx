<%@ Page Title="Discounts Summary" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintDiscountsSummary.aspx.cs" Inherits="Paperwork.PrintDiscountsSummary" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        }; 

        var w = $(window).width();
        var timer,
            $win = $(window);

        //when window resizes
        $win.on('resize', function () {
            //update the width variable
            w = $(window).width();
            clearTimeout(timer);
            timer = setTimeout(function () {
                $("#SpecialFitDiv").width(w - 20);
                $("#ReportViewer1").widget(w - 20);
            }, 200);

        });

        $( function() {
            BindEvents();
        } );

        function BindEvents() {

            var PosRefY = $("#DataEntryReferenceLocation").position().top;

            $('#SpecialFitDiv').width(w - 20);
            $('#SpecialFitDiv').css("top", PosRefY + 30);
            $('#SpecialFitDiv').css("left", 10);

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
                <strong>Month:</strong>
                <asp:DropDownList ID="DDLMonth" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DDLMonth_SelectedIndexChanged">
                    <asp:ListItem Text="Jul" Value="07" ></asp:ListItem>
                    <asp:ListItem Text="Aug" Value="08" ></asp:ListItem>
                    <asp:ListItem Text="Sep" Value="09" ></asp:ListItem>
                    <asp:ListItem Text="Oct" Value="10" ></asp:ListItem>
                    <asp:ListItem Text="Nov" Value="11" ></asp:ListItem>
                    <asp:ListItem Text="Dec" Value="12" ></asp:ListItem>
                    <asp:ListItem Text="Jan" Value="01" ></asp:ListItem>
                    <asp:ListItem Text="Feb" Value="02" ></asp:ListItem>
                    <asp:ListItem Text="Mar" Value="03" ></asp:ListItem>
                    <asp:ListItem Text="Apr" Value="04" ></asp:ListItem>
                    <asp:ListItem Text="May" Value="05" ></asp:ListItem>
                    <asp:ListItem Text="Jun" Value="06" ></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div id="DataEntryReferenceLocation"></div>
    <div id="SpecialFitDiv" style="overflow:auto;background-color:white; position:absolute;" >
        
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
                CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
                ShowCredentialPrompts="False" SizeToReportContent="True"  Visible="true" ShowDocumentMapButton="False" 
                ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
                AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous">
                <ServerReport ReportPath="Summary Discounts" />
            </rsweb:ReportViewer>
            <br />
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
    </div>
</asp:Content>
