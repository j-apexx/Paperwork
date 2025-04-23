<%@ Page Title="View/Edit Operating Days" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Days.aspx.cs" Inherits="Paperwork.Days" %>
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
            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });
        }

    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="topSection">
                <div  id="hdrgroup">

                    <div id="hdrDiv" style="float:left;width:40%;">
                        <h2><%: Title %></h2>
                    </div>
                    <div id="menuDiv" >
                        <div class="row">
                            <strong>Program Year:</strong>
                            <asp:Label ID="LabelProgramYear" runat="server"></asp:Label>

                        </div>
                        <div class="row">
                            <strong>District:</strong>
                            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> &nbsp;
                        </div>
                        <br />
                    </div>
                </div>
            </div>

            <div id="entryDiv" style="position:relative;"  >
                <div class="col-sm-12">
                    <div class="row" style="padding-bottom:5px;">
                        <div class="col-sm-1" style="border:solid;border-width:2px; border-color:pink;">
                            &nbsp;
                        </div>
                        <div class="col-sm-2">
                            =Overridden
                        </div>
                        
                        <div class="col-sm-6">
                            <div style="text-align:center;" >                   
                                <asp:LinkButton ID="ISave" runat="server" 
                                    ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                    CssClass="fas fa-save mGridLinkButton" Width="170" OnClick="ISave_Click"></asp:LinkButton>
                            </div>
                        </div>
                        
                        <div class="col-sm-3">
                            <div class="messagealert" id="alert_container"></div>
                        </div>
                    </div>
                    
                    <asp:Calendar ID="Calendar1" runat="server" BackColor="White" BorderColor="Black" BorderStyle="Solid" CellSpacing="2" CellPadding="2" Font-Names="Verdana" 
                        Font-Size="9pt" ForeColor="#354d66" Height="550px" NextPrevFormat="ShortMonth" Width="100%" OnDayRender="Calendar1_DayRender" OnVisibleMonthChanged="Calendar1_VisibleMonthChanged"
                          >
                        <WeekendDayStyle BackColor="DarkGray" />
                        <DayHeaderStyle Font-Bold="True" Font-Size="10pt" ForeColor="#333333" Height="8pt" />
                        <DayStyle BackColor="#e9e9e9" Font-Bold="True" Font-Size="10pt" />
                        <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="White" />
                        <OtherMonthDayStyle ForeColor="#999999" />
                        <SelectedDayStyle BackColor="#333399" ForeColor="White" />
                        <TitleStyle BackColor="#4e7094" BorderStyle="Solid" Font-Bold="True" Font-Size="12pt" ForeColor="White" Height="12pt" />
                        <TodayDayStyle BackColor="#ffffcc" ForeColor="Black" />
                    </asp:Calendar>
                    <asp:HiddenField ID="HFBDates" runat="server" />
                    <asp:HiddenField ID="HFLDates" runat="server" />
                    <asp:HiddenField ID="HFMinDate" runat="server" />
                    <asp:HiddenField ID="HFMaxDate" runat="server" />
                    <asp:Label ID="LabelMessage" runat="server" ></asp:Label>
                </div>
                <br />
                
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="DataSources">

    </div>
</asp:Content>
