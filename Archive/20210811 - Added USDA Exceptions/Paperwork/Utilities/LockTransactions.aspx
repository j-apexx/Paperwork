<%@ Page Title="Lock and Unlock Transactions" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LockTransactions.aspx.cs" Inherits="Paperwork.LockTransactions" %>
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

        function BindEvents() {
            $(function () {
                $("[id*=TextboxStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id*=TextboxEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            };

        function ShowMessage(message, messagetype) {
            var cssclass;
            var timeoutCount;
            switch (messagetype) {
                case 'Success':
                    cssclass = 'alert-success';
                    timeoutCount = 3000;
                    break;
                case 'Error':
                    cssclass = 'alert-danger';
                    timeoutCount = 30000;
                    break;
                case 'Warning':
                    cssclass = 'alert-warning';
                    timeoutCount = 30000;
                    break;
                default:
                    cssclass = 'alert-info';
                    timeoutCount = 3000;
            }
            $('#alert_container').append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert alert-dismissible fade show '
                + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><a href="#" data-dismiss="alert" ><strong>'
                + messagetype + '!</strong> <span>' + message + '</span></a></div>');
            window.setTimeout(function () { $('#alert_div').alert('close'); }, timeoutCount);

        }

        function ConfirmationCheck(taskname) {
            if ($("[id*=DDLDistricts]").val() === "%") {
                var flag = confirm("All districts are selected, continue " + taskname + "?");
                $("[id*=HFConfirmed]").val( flag? '1':'0');
            }
            else {
                $("[id*=HFConfirmed]").val( '1');
            }
        }
    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="hdrGroup">
        <h2><%: Title %></h2>
        <p>
            This page will allow you to lock/unlock existing transactions. To prevent new transactions - use the Program Year setup page to close periods.
        </p>
    </div>
    <div id="utilityGroup">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="row">
                    <div class="col-sm-2">
                        Program Year:
                    </div>
                    <div class="col-sm-10">
                        <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                            DataValueField="ProgramYear" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        <asp:HiddenField ID="HFProgramYearStartDate" runat="server" />
                        <asp:HiddenField ID="HFProgramYearEndDate" runat="server" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        District:
                    </div>
                    <div class="col-sm-10">
                        <asp:DropDownList ID="DDLDistricts" runat="server" AppendDataBoundItems="true" DataSourceID="SqlDataSourceDistricts" DataTextField="Desc" DataValueField="DistrictID">
                            <asp:ListItem Selected="True" Text="*All Districts*" Value="%"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        Start Date:
                    </div>
                    <div class="col-sm-10">
                        <asp:TextBox ID="TextboxStartDate" runat="server" CssClass="CBLarge"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        End Date:
                    </div>
                    <div class="col-sm-10">
                        <asp:TextBox ID="TextboxEndDate" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:CheckBox ID="CheckBoxPurchases" runat="server" Text="Purchases" Checked="true"></asp:CheckBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:CheckBox ID="CheckBoxRecaps" runat="server" Text="Sales" Checked="true"></asp:CheckBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:CheckBox ID="CheckBoxDistPayroll" runat="server" Text="District Payroll" Checked="true"></asp:CheckBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:CheckBox ID="CheckBoxMMB" runat="server" Text="MMB Sheets" Checked="true"></asp:CheckBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:CheckBox ID="CheckBoxInv" runat="server" Text="Inventory Entries" Checked="true"></asp:CheckBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:CheckBox ID="CheckBoxOtherFood" runat="server" Text="Other Food Fund Expenses" Checked="true"></asp:CheckBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:CheckBox ID="CheckBoxSpFunc" runat="server" Text="Special Functions" Checked="true"></asp:CheckBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:CheckBox ID="CheckBoxDays" runat="server" Text="Days" Checked="true"></asp:CheckBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:CheckBox ID="CheckBoxNotifications" runat="server" Text="Notifications" Checked="true"></asp:CheckBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <asp:LinkButton ID="ILock" Width="200" runat="server" 
                            class="fas fa-lock mGridLinkButton" 
                            ToolTip="Print" Text=" Lock" OnClick="ILock_Click" OnClientClick="ConfirmationCheck('locking')"></asp:LinkButton>
                        <asp:LinkButton ID="IUnlock" Width="200" runat="server" 
                            class="fas fa-unlock mGridLinkButton" 
                            ToolTip="Print" Text=" Unlock" OnClick="IUnlock_Click" OnClientClick="ConfirmationCheck('unlocking')"></asp:LinkButton>
                        <asp:HiddenField ID="HFConfirmed" runat="server" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        &nbsp;
                    </div>
                    <div class="col-sm-10">
                        <div class="messagealert" id="alert_container">
                                        </div>
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc'
            FROM [District] ORDER BY [DistrictName]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
    </div>
</asp:Content>
