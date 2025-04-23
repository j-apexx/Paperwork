<%@ Page Title="Menu Averages Overall" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrintMenuAveragesOverall.aspx.cs" Inherits="Paperwork.PrintMenuAveragesOverall" %>
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
    <div >
        <div id="header">
            <div class="row">
                <div class="col-sm-3">
                    <strong>Menu Analysis Year:</strong>
                    <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="MenuAnalysisYear" 
                    DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-sm-9">
                    <asp:LinkButton ID="btnSelectDists" runat="server" CssClass="fas fa-tasks btn btn-outline-secondary" ForeColor="#666699" Width="100%" OnClick="BtnSelectDists_Click" Text="Select Districts" />
                </div>
            </div>
        </div>
        <div id="report">
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
                CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
                ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
                ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
                AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous" ShowRefreshButton="true"  >
                <ServerReport ReportPath="Menu Averages Overall" />
            </rsweb:ReportViewer>
        </div>

        <div class="modal fade" id="myModal">  
            <div class="modal-dialog  modal-xl" >  
                <div class="modal-content" > 
                    <div class="modal-header">  
                        <h4 class="modal-title">Select Districts</h4>  
                        <button type="button" class="close" data-dismiss="modal">×</button>  
                    </div>  
                    <div class="modal-body">  
                        <div class="row">  
                            <div class="col-md-12">  
                                <asp:CheckBoxList ID="CheckBoxListDistricts" runat="server" AutoPostBack="false" TextAlign="Right"  CssClass="cblistPad" 
                                    DataTextField="Desc" DataValueField="DistrictID" RepeatColumns="1000" RepeatDirection="Horizontal" RepeatLayout="Table"  
                                    Width="100%"></asp:CheckBoxList>
                            </div>  
                        </div>  
                    </div>  
                    <div class="modal-footer">  
                        <asp:LinkButton runat="server" ID="btnRefresh" ToolTip="Refresh Report" Text=" Refresh"
                            CssClass="fas fa-sync btn btn-outline-secondary" ForeColor="#666699" Width="100%" OnClick="btnRefresh_Click" Font-Size="Larger"  />
                        <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Cancel</button>  
                    </div>  
                </div>  
            </div>  
        </div> 




    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT YEAR(EndDate) MenuAnalysisYear, RTRIM(ProgramYear) ProgramYear, StartDate, EndDate FROM [ProgramYear] WHERE EndDate>='1/1/2024' 
                AND YEAR(GETDATE())*100 + MONTH(GETDATE()) >= YEAR(EndDate)*100 + MONTH(EndDate)
                ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
    </div>
</asp:Content>
