<%@ Page Title="District Info" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupDistrictInfo.aspx.cs" Inherits="Paperwork.SetupDistrictInfo" %>
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
                $("#GridViewHeightController").height(h - topAndBottom);
            }, 200);

        });

        function BindEvents() {
            $('#GridViewHeightController').css("height", h - topAndBottom);
            $('#GridViewHeightController').css("overflow", "auto");

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            $("[id*=TextBoxDistCEPPaidPct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }

                //enforce 4 decimal places
                var amount = parseFloat($("[id*=TextBoxDistCEPPaidPct]").val());
                $("[id*=TextBoxDistCEPPaidPct]").val(amount.toFixed(4));

                if (parseFloat($("[id*=TextBoxDistCEPPaidPct]").val()) + parseFloat($("[id*=TextBoxDistCEPFreePct]").val()) != 100) {
                    var diffAmount = 100 - parseFloat($("[id*=TextBoxDistCEPPaidPct]").val());
                    $("[id*=TextBoxDistCEPFreePct]").val(diffAmount.toFixed(4));
                }
            })

            $("[id*=TextBoxDistCEPFreePct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }

                //enforce 4 decimal places
                var amount = parseFloat($("[id*=TextBoxDistCEPFreePct]").val());
                $("[id*=TextBoxDistCEPFreePct]").val(amount.toFixed(4));

                if (parseFloat($("[id*=TextBoxDistCEPFreePct]").val()) + parseFloat($("[id*=TextBoxDistCEPPaidPct]").val()) != 100) {
                    var diffAmount = 100 - parseFloat($("[id*=TextBoxDistCEPFreePct]").val());
                    $("[id*=TextBoxDistCEPPaidPct]").val(diffAmount.toFixed(4));
                }
            })

            $("[id*=TextBoxCPMOverride]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }

                //enforce 4 decimal places
                var amount = parseFloat($(this).val());
                $(this).val(amount.toFixed(4));

            })
        };

    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <asp:Image CssClass="loadingImage" ID="ImageLoading1" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div id="topSection" style="height:100px">
                <div id="hdrgroup">
                    <div id="hdrDiv" style="float:left;width:30%">
                        <h2><%: Title %></h2>
                        <div>
                        <asp:LinkButton ID="ISave" runat="server" 
                                ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                CssClass="fas fa-save mGridLinkButton" Width="170"
                            ValidationGroup="Save" CausesValidation="true" />
                            
                        </div>
                    </div>
                    <div id="menuDiv" style="float:left;width:70%">
                        <br />
                        <p>
                            <strong>Program Year:</strong>
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                    DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                            <asp:HiddenField ID="HFBudgetYear" runat="server"  />
                            
                            &nbsp;&nbsp;
                            <strong>District:</strong>
                            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> 
                            &nbsp;&nbsp;
                            <br />
                            <asp:LinkButton ID="LinkButtonContacts" runat="server" 
                                ToolTip="Display Contacts" Text=" Contacts" ForeColor="SteelBlue"
                                CssClass="far fa-address-book mGridLinkButton" Width="170"
                            ValidationGroup="Save" CausesValidation="true" OnClick="LinkButtonContacts_Click" />
                        </p>
                        
                    </div>
                </div>
            </div>
            <div id="GridViewHeightController" style="overflow-y:auto;background-color:white;" class="row">
                <div class="messagealert" id="alert_container"></div>
                <div class="col-sm-12">
                    <div class="messagealert" id="alert_container"></div>
                    <asp:FormView ID="FormViewDistInfo" runat="server" CssClass="mGridRecaps" DataSourceID="SqlDataSourceDistYearInfo" DefaultMode="Edit"
                    OnPreRender="FormViewDistInfo_PreRender" OnDataBound="FormViewDistInfo_DataBound" DataKeyNames="ProgramYear, DistrictID" >
                    <EditItemTemplate>
                        <div class="container">
                            <div id="District Info">
                                <div class="row" style="border-bottom-color:deepskyblue;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;" >
                                    <div class="col-sm-2" style="background-color:deepskyblue;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        District Info
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-4 h4">
                                        <asp:Label ID="LabelDistHdr" runat="server" Text='<%# Eval("DistrictName") %>'   />
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:CheckBox ID="CBFFVDist" runat="server" Checked='<%# Bind("FFVDistrict") %>' Text="FFV District" OnCheckedChanged="DistInfoControl_Changed" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Regional Manager:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:HiddenField ID="HFRMName" runat="server" Value='<%# Bind("RMName") %>' />
                                        <asp:DropDownList ID="DropDownListRMs" runat="server" AppendDataBoundItems="True" AutoPostBack="true" DataSourceID="SqlDataSourceRMs"
                                            DataTextField="RMName" DataValueField="RMNumber" SelectedValue='<%# Bind("RMNumber") %>' OnDataBound="DropDownListRMs_DataBound" OnSelectedIndexChanged="DistInfoControl_Changed"  >
                                            <asp:ListItem Text="- Select -" Value="0"></asp:ListItem>
                                        </asp:DropDownList> 
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        RM Code:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxRMCode" runat="server" Text='<%# Bind("RMCode") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="2" Width="50" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        FSD Name:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxFSD" runat="server" Text='<%# Bind("FSDName") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="50" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        FSD Office Phone:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxPhone" runat="server" Text='<%# Bind("FSDPhone") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="25" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        FSD Cell:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxCell" runat="server" Text='<%# Bind("FSDCell") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="25" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        FSD Email:
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxFSDEmail" runat="server" Text='<%# Bind("FSDEmail") %>'  OnTextChanged="DistInfoControl_Changed" Width="100%" MaxLength="50" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        FSD Address:
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxAddress" runat="server" Text='<%# Bind("FSDAddress") %>'  OnTextChanged="DistInfoControl_Changed" Width="100%" MaxLength="50" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        City:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxFSDCity" runat="server" Text='<%# Bind("FSDCity") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="35" Width="100%" />
                                    </div>
                                    <div class="col-sm-1">
                                        State:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxFSDState" runat="server" Text='<%# Bind("FSDState") %>'  OnTextChanged="DistInfoControl_Changed" Width="100%" MaxLength="2" />
                                    </div>
                                    <div class="col-sm-1">
                                        Zip:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxFSDZip" runat="server" Text='<%# Bind("FSDZip") %>'  OnTextChanged="DistInfoControl_Changed" Width="100%" MaxLength="10" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Asst. FSD Name:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxAsstFSD" runat="server" Text='<%# Bind("AsstFSDName") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="50" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        Asst. FSD Cell:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxAsstFSDPhone" runat="server" Text='<%# Bind("AsstFSDPhone") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="25" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Business Manager:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxBusMgr" runat="server" Text='<%# Bind("BusinessMgr") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="50" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        Superintendent:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxSuper" runat="server" Text='<%# Bind("Superintendent") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="50" Width="100%" />
                                    </div>
                                </div>
                            </div>
                            <div id="Invoice Details">
                                <div class="row" style="border-bottom-color:turquoise;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                    <div class="col-sm-2" style="background-color:turquoise;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Invoice Details
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Name used on invoice:
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxInvoiceName" runat="server" Text='<%# Bind("DistrictFullName") %>'  OnTextChanged="DistFieldsChanged" Enabled='<%# Session["AccessLevel"]=="User"?false:true %>' MaxLength="40"  Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Invoice Billing Address:
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxAddress1" runat="server" Text='<%# Bind("DistrictAddress1") %>'  OnTextChanged="DistFieldsChanged" Width="100%" Enabled='<%# Session["AccessLevel"]=="User"?false:true %>' MaxLength="50" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxAddress2" runat="server" Text='<%# Bind("DistrictAddress2") %>'  OnTextChanged="DistFieldsChanged" Width="100%" Enabled='<%# Session["AccessLevel"]=="User"?false:true %>' MaxLength="50" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        City:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxInvCity" runat="server" Text='<%# Bind("DistrictCity") %>'  OnTextChanged="DistFieldsChanged" Enabled='<%# Session["AccessLevel"]=="User"?false:true %>' MaxLength="35" Width="100%" />
                                    </div>
                                    <div class="col-sm-1">
                                        State:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxInvState" runat="server" Text='<%# Bind("DistrictState") %>'  OnTextChanged="DistFieldsChanged" Width="100%" Enabled='<%# Session["AccessLevel"]=="User"?false:true %>' MaxLength="2" />
                                    </div>
                                    <div class="col-sm-1">
                                        Zip:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxInvZip" runat="server" Text='<%# Bind("DistrictZip") %>'  OnTextChanged="DistFieldsChanged"  Enabled='<%# Session["AccessLevel"]=="User"?false:true %>' MaxLength="10" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Invoice Contact:
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxInvContact" runat="server" Text='<%# Bind("DistrictContact") %>'  OnTextChanged="DistFieldsChanged"  Width="100%" MaxLength="50"  />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Account Type:
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:DropDownList ID="DDL_AccountType" runat="server" SelectedValue='<%# Bind("AcctType") %>' Enabled='<%# Session["AccessLevel"]=="User"?false:true %>' OnSelectedIndexChanged="DistInfoControl_Changed" AutoPostBack="true"
                                            BackColor='<%# Session["AccessLevel"]=="User"? System.Drawing.Color.LightGray : System.Drawing.Color.White %>' Width="100%" >
                                            <asp:ListItem Text="Unknown" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Cost Per Meal" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Full Bill Purchase" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Full Bill Usage" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Full Bill Purch w/ CPM Fees" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="Full Bill Usage w/ CPM Fees" Value="5"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2">
                                        CEP?
                                    </div>
                                    <div class="col-sm-5">
                                        <asp:CheckBox ID="CB_CEP" runat="server" Enabled="false" />
                                        <asp:TextBox ID="TextBoxCEPCoverage" runat="server" Enabled="false"></asp:TextBox> 
                                    </div>
                                </div>
                                <div class="row" id="CEPPaidRow" runat="server">
                                    <div class="col-sm-5">
                                        &nbsp;
                                    </div>
                                    <div class="col-sm-2">
                                        CEP Paid Pct
                                    </div>
                                    <div class="col-sm-5">
                                        <asp:TextBox ID="TextBoxDistCEPPaidPct" runat="server" Text='<%# Bind("DistCEPPaidPct","{0:F4}" ) %>' Width="100" MaxLength="8" OnTextChanged="DistInfoControl_Changed"></asp:TextBox>%
                                        <asp:RangeValidator ID="ValidRangePaid" runat="server" ControlToValidate="TextBoxDistCEPPaidPct" Display="Dynamic" CssClass="validators"
                                             MaximumValue="100" MinimumValue="0" SetFocusOnError="true" Type="Double" Text="Invalid Paid Percent" ValidationGroup="Save">*Invalid Paid Percent</asp:RangeValidator>
                                    </div>
                                </div>
                                <div class="row" id="CEPFreeRow" runat="server">
                                    <div class="col-sm-5">
                                        &nbsp;
                                    </div>
                                    <div class="col-sm-2">
                                        CEP Free Pct
                                    </div>
                                    <div class="col-sm-5">
                                        <asp:TextBox ID="TextBoxDistCEPFreePct" runat="server" Text='<%# Bind("DistCEPFreePct","{0:F4}" ) %>' Width="100" MaxLength="8" OnTextChanged="DistInfoControl_Changed"></asp:TextBox>%
                                        <asp:RangeValidator ID="ValidRangeFree" runat="server" ControlToValidate="TextBoxDistCEPFreePct" Display="Dynamic" CssClass="validators"
                                             MaximumValue="100" MinimumValue="0" SetFocusOnError="true" Type="Double" Text="Invalid Paid Percent" ValidationGroup="Save">*Invalid Free Percent</asp:RangeValidator>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-3">
                                        Cost Per Meal Rate from Budgets
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxCPM" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="LabelCPMBillRate" runat="server" Text="Cost Per Meal Bill Rate"></asp:Label>
                                    </div>
                                    <div class="col-sm-5">
                                        <asp:TextBox ID="TextBoxCPMOverride" runat="server" Enabled="true" Width="100" Text='<%# Bind("CPMBillingRate","{0:F4}") %>'  OnTextChanged="DistInfoControl_Changed"></asp:TextBox>
                                        <asp:HiddenField ID="HFCPMBillingRate" runat="server" Value='<%# Bind("CPMBillingRate") %>' />
                                        <asp:Label ID="LabelOverrideCPM" runat="server" Text="<br/>*amount other than 0 will override Budget CPM on invoices"></asp:Label>
                                    </div>
                                </div>
                                <div class="row" id="overrideMessageRow" runat="server">
                                    <div class="col-sm-5"></div>
                                    <div class="col-sm-7">
                                        <asp:Label ID="LabelOverrideNote" ForeColor="DarkRed" runat="server" Text='<%# Eval("BillingRateChangeNote") %>' ></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        AC Equivalent Rate
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxMealEquiv" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Annual Mgmt Fee
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxMgmtFee" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        Annual Admin Fee
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxAdminFee" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Annual Mgmt Payroll
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxMgmtPay" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        Annual Add-On Fees
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxAddonFee" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div id="Budget Details">
                                <div class="row" style="border-bottom-color:greenyellow;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                    <div class="col-sm-2" style="background-color:greenyellow;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Budget Details
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Account Name:
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxDistName" runat="server" Text='<%# Bind("DistrictName") %>'  OnTextChanged="DistInfoControl_Changed" Width="100%" Enabled="false"  />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Operating Days:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxOpDays" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Daily Cash Sales:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxCashBud" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        Daily Reimbursements:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDailyReimb" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Daily FICA Credit:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDailyFICA" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        Daily Interest:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDailyInterest" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Daily Other Sales:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDailyOSales" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        Daily Costs:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDailyCosts" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Annual Dist Cost:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxAnnualDistCost" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        Daily SFA Labor:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDailySFALabor" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Dist. Payroll Related %:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDistPayRel" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Food CPM:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxFoodCPM" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paper/Clean CPM:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxPaperCleanCPM" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        NUTR Prod Hours:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxNProdHours" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        NUTR Nonprod Hours:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxNNonProdHours" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        NUTR Sp Fn Hours:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxNSpFnHours" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        DIST Prod Hours:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDProdHours" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        DIST Nonprod Hours:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDNonProdHours" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        DIST Sp Fn Hours:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxDSpFnHours" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-2">
                                    <asp:HiddenField ID="HFDistInfoSaved" runat="server" Value="0" />
                                    <asp:HiddenField ID="HFDistSaved" runat="server" Value="0" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-2">&nbsp;</div>
                            </div>
                        </div>
                    </EditItemTemplate>
                </asp:FormView>
                </div>
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceRMs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="
                        SELECT COALESCE(RM.[RMName], DYI.[RMNAME]) RMNAME , COALESCE(RM.[RMNumber] , DYI.RMNUMBER) RMNUMBER
                        FROM [RMSetup] RM
                        FULL OUTER JOIN DistrictYearInfo DYI ON RM.RMNumber=DYI.RMNumber AND DistrictID=@DistrictID AND [ProgramYear]=@ProgramYear
                        WHERE [Active]=1 OR (RM.[RMNumber] IS NULL AND DistrictID=@DistrictID AND [ProgramYear]=@ProgramYear)
                        ORDER BY [RMName]" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceDistYearInfo" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"   
                    SELECTCOMMAND="SELECT D.*, ISNULL([AcctType],0) AcctType,ISNULL([GM],'') GM,ISNULL([RMCode],'') RMCode,ISNULL([RMName],'') RMName,ISNULL([FSDName], '') FSDName
                    ,ISNULL([FSDAddress],'') FSDAddress ,ISNULL([FSDCity],'') FSDCity,ISNULL([FSDState],'') FSDState
                    ,ISNULL([FSDZip],'') FSDZip,ISNULL([FSDEmail],'') FSDEmail,ISNULL([FSDPhone],'') FSDPhone,ISNULL([FSDCell],'') FSDCell
	                ,ISNULL([BusinessMgr],'') BusinessMgr,ISNULL([Superintendent],'') Superintendent, ISNULL([RMNumber],0) RMNumber
                    ,ISNULL([AsstFSDName],'') AsstFSDName, ISNULL([AsstFSDPhone],'') AsstFSDPhone, ISNULL([DistCEPPaidPct],0) DistCEPPaidPct, ISNULL([DistCEPFreePct],0) DistCEPFreePct
                    ,ISNULL([CPMBillingRate],0) CPMBillingRate, ISNULL(ProgramYear, @ProgramYear) ProgramYear, ISNULL([FFVDistrict], CAST(0 AS Bit)) FFVDistrict, ISNULL(BillingRateChangeNote, '') BillingRateChangeNote
                    FROM [District] D
                    LEFT OUTER JOIN [DistrictYearInfo] DYI ON D.DistrictID=DYI.DistrictID AND @ProgramYear=DYI.[ProgramYear]
                    WHERE (D.[DistrictID] = @DistrictID) " >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceBDGT" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" 
                    DataSourceMode="DataReader" 
                    SelectCommand="SELECT [DaysOfSvcLunch], [DailyCashBudget] + ([PDE3086Revenue]/[DaysOfSvcLunch]) DailyCashCalc, [DailyReimBudget], [DailyCosts], [DistrictCosts], 
                    [PaperCPM]+[CleaningCPM] PaperCleanCPM, [FoodCPM], [DailyInterest], [DailyExpenses], [DailyOtherSales], [DailyPSERSCredit], ([SFALabor]/[DaysOfSvcLunch]) DailySFALabor, [MaxofFringePercent],
                    [CostPerMeal], [MealEquivalentFactor], [CBS_FSMCAdmin], [CBS_FSMCMgmt], [FSDCosts], ROUND([AccountingNUTR] + [TechFeePlusSoftware] + [RecruitReplace] + [Liability] + [Training] + [WellnessFeeNUTR] + [CACFPAdminFeeTotal],2) AddOnFees,
                    [CEP], [CEPCoverage], ISNULL(NUTRHoursDayTotal,0.00) NUTRProdHours, ISNULL(NUTRNonProdHours/[DaysOfSvcLunch],0.00) NUTRNonProdHours, 
                    ISNULL(SFAHoursDayTotal,0.00) SFAProdHours, ISNULL(SFANonProdHours/[DaysOfSvcLunch],0.00) SFANonProdHours, (ISNULL(DistLbrHrsPerDay, 0.00) * [DaysOfSvcLunch]) 'DistSPFNLaborHours', (ISNULL(NutLbrHrsPerDay, 0.00) * [DaysOfSvcLunch]) 'NutSPFNLaborHours'
                    FROM [vwBDGT_Totals] TOTN
                    LEFT OUTER JOIN [vwBDGT_LaborNUTRTotals] LBRN ON TOTN.BudgetYear=LBRN.BudgetYear AND TOTN.BudgetVersion=LBRN.BudgetVersion AND TOTN.BidNumber=LBRN.BidNumber
                    LEFT OUTER JOIN [vwBDGT_LaborSFATotals] LBRS ON TOTN.BudgetYear=LBRS.BudgetYear AND TOTN.BudgetVersion=LBRS.BudgetVersion AND TOTN.BidNumber=LBRS.BidNumber
                    LEFT OUTER JOIN [BDGT_SpecialFunctionsCst] SPFN ON TOTN.BudgetYear=SPFN.BudgetYear AND TOTN.BudgetVersion=SPFN.BudgetVersion AND TOTN.BidNumber=SPFN.BidNumber
                    WHERE (TOTN.[BidNumber] = @BidNumber) AND TOTN.BudgetYear=@BudgetYear AND [DaysOfSvcLunch]<>0">
                    <SelectParameters>
                        <asp:SessionParameter Name="BidNumber" SessionField="CurrentDistrict" Type="String" />
                        <asp:ControlParameter Name="BudgetYear" ControlID="HFBudgetYear" PropertyName="Value" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
