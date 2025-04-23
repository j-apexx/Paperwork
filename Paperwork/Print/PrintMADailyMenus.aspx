<%@ Page Title="Menu Analysis Daily Menus" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintMADailyMenus.aspx.cs" Inherits="Paperwork.PrintMADailyMenus" %>
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
        });

        // For formview resizing
        var h = $(window).height();
        var topAndBottom = 200;
        var timer,
            $win = $(window);

        //when window resizes
        $win.on('resize', function () {
            //update the height variable
            h = $(window).height();
            clearTimeout(timer);
            timer = setTimeout(function () {
                $("#ReportViewer1").height(h - topAndBottom);
            }, 200);

        });

        function BindEvents() {
            $('#ReportViewer1').css("height", h - topAndBottom);
            $('#ReportViewer1').css("overflow", "auto");

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
            <div class="col-sm-4">
                <strong>Menu Analysis Year:</strong>
                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="MenuAnalysisYear" 
                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"></asp:DropDownList>
            </div>
            <div class="col-sm-4">
                <strong>District:</strong>
                <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                    DataTextField="Desc" DataValueField="DistrictID" OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                </asp:DropDownList> 
            </div>
            <div class="col-sm-4">
                <strong>School:</strong>
                <asp:DropDownList ID="DropDownListSchools" runat="server" AutoPostBack="True" 
                    DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" DataValueField="SchoolNum"
                    OnSelectedIndexChanged="DropDownListSchools_SelectedIndexChanged">
                </asp:DropDownList>
            </div> 
        </div>
        <div class="row"> 
            <div class="col-sm-4">
                <strong>Menu Category</strong>
                <asp:DropDownList ID="DDLCategories" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DDLCategories_SelectedIndexChanged" >
                    <asp:ListItem Text="Breakfast Entree" Value="1" ></asp:ListItem>
                    <asp:ListItem Text="Breakfast Fruit" Value="2" ></asp:ListItem>
                    <asp:ListItem Text="Lunch Entree" Value="4" ></asp:ListItem>
                    <asp:ListItem Text="Lunch Fruit" Value="5" ></asp:ListItem>
                    <asp:ListItem Text="Lunch Vegetable" Value="6" ></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-sm-4">
                <strong>Start Day:</strong>
                <asp:TextBox ID="TextBoxDayStart" runat="server" OnTextChanged="TextBoxDay_TextChanged" Width="50" ></asp:TextBox>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <strong>End Day:</strong>
                <asp:TextBox ID="TextBoxDayEnd" runat="server" OnTextChanged="TextBoxDay_TextChanged" Width="50" ></asp:TextBox>
            </div>

            <div class="col-sm-4">
                <asp:LinkButton runat="server"  
                    ID="ISync" ToolTip="Refresh Report" Text=" Refresh"
                    CssClass="fas fa-sync mGridLinkButton" ForeColor="#666699" Width="100" OnClick="ISync_Click"  />
            </div>
        </div>
    </div>
    <div id="report">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
            AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous"  >
            <ServerReport ReportPath="Menu Analysis Menus" />
        </rsweb:ReportViewer>
        <br />
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT YEAR(EndDate) MenuAnalysisYear, RTRIM(ProgramYear) ProgramYear, StartDate, EndDate FROM [ProgramYear] WHERE EndDate>='1/1/2024' 
                AND YEAR(GETDATE())*100 + MONTH(GETDATE()) >= YEAR(EndDate)*100 + MONTH(EndDate)
                ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT DS.SchoolNum, DS.SchoolName, DS.SchoolType FROM DistrictSchools DS 
            WHERE DS.DistrictID=@DistrictID AND INACTIVE=0 ORDER BY DS.SortOrder, DS.SchoolName">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
