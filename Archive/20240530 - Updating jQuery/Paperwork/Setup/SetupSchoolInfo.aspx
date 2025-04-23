<%@ Page Title="Setup School Info" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupSchoolInfo.aspx.cs" Inherits="Paperwork.SetupSchoolInfo" %>
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

        function BindEvents() {
            var PYearStart = $("[id*=HFProgYearStartDate]").val();
            var PYearEnd = $("[id*=HFProgYearEndDate]").val();

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            $(function () {
                $("[id*=TextBoxEffectiveRatesDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxEffectivePricesDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $("[id*=TextBoxSchoolCEPPaidPct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }

                //enforce 4 decimal places
                var amount = parseFloat($("[id*=TextBoxSchoolCEPPaidPct]").val());
                $("[id*=TextBoxSchoolCEPPaidPct]").val(amount.toFixed(4));

                if (parseFloat($("[id*=TextBoxSchoolCEPPaidPct]").val()) + parseFloat($("[id*=TextBoxSchoolCEPFreePct]").val()) != 100) {
                    var diffAmount = 100 - parseFloat($("[id*=TextBoxSchoolCEPPaidPct]").val());
                    $("[id*=TextBoxSchoolCEPFreePct]").val(diffAmount.toFixed(4));
                }
            })

            $("[id*=TextBoxSchoolCEPFreePct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }

                //enforce 4 decimal places
                var amount = parseFloat($("[id*=TextBoxSchoolCEPFreePct]").val());
                $("[id*=TextBoxSchoolCEPFreePct]").val(amount.toFixed(4));

                if (parseFloat($("[id*=TextBoxSchoolCEPFreePct]").val()) + parseFloat($("[id*=TextBoxSchoolCEPPaidPct]").val()) != 100) {
                    var diffAmount = 100 - parseFloat($("[id*=TextBoxSchoolCEPFreePct]").val());
                    $("[id*=TextBoxSchoolCEPPaidPct]").val(diffAmount.toFixed(4));
                }
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
            <div id="topSection" style="height:90px">
                <div id="hdrgroup">
                    <div id="hdrDiv" style="float:left;width:30%">
                        <h2><%: Title %></h2>
                        <asp:LinkButton ID="ISave" runat="server" 
                                    ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                    CssClass="fas fa-save mGridLinkButton" Width="170" CausesValidation="true" ValidationGroup="Save"   />
                        <asp:HiddenField ID="HFProgYearStartDate" runat="server" />
                        <asp:HiddenField ID="HFProgYearEndDate" runat="server" />
                    </div>
                    <div id="menuDiv" style="float:left;width:70%;padding-top:10px;">
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
                        <br />
                        <strong>School:</strong><asp:DropDownList ID="DropDownListSchools" runat="server" AutoPostBack="True" 
                            DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" DataValueField="SchoolNum" ValidationGroup="GridView"
                            >
                        </asp:DropDownList> &nbsp;
                        
                        <div class="messagealert" id="alert_container" style="padding-top:10px;"></div>
                    </div>
                </div>
            </div>
            <div id="SchoolInfoSection">
                <asp:FormView ID="FormViewSchoolInfo" runat="server" CssClass="mGridRecaps" DataSourceID="SqlDataSourceSchoolYearSetup" DefaultMode="Edit"
                    OnPreRender="FormViewSchoolInfo_PreRender" OnDataBound="FormViewSchoolInfo_DataBound" DataKeyNames="ProgramYear, DistrictID, SchoolNum" >
                    <EditItemTemplate>
                        <div class="container">
                            <div class="row">
                                <div class="col-sm-12" style="font-size:larger;font-weight:bold;">
                                    <asp:Label ID="labelSchoolName" runat="server" Text='<%# DropDownListSchools.SelectedItem  %>'></asp:Label>
                                    <asp:HiddenField ID="HFSchoolNum" runat="server" Value='<%# DropDownListSchools.SelectedValue  %>' />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-2">
                                    <asp:CheckBox ID="CB_Breakfast" runat="server" Checked='<%# Bind("Breakfasts") %>' Text="Breakfasts" OnCheckedChanged="CB_SchInfo_CheckedChanged" />
                                </div>
                                <div class="col-sm-2">
                                    <asp:CheckBox ID="CB_Tiers" runat="server" Checked='<%# Bind("Tiers") %>' Text="Tiers" OnCheckedChanged="CB_SchInfo_CheckedChanged" />
                                </div>
                                <div class="col-sm-2">
                                    <asp:CheckBox ID="CB_OtherSales" runat="server" Checked='<%# Bind("OtherSales") %>' Text="Other Sales" OnCheckedChanged="CB_SchInfo_CheckedChanged" />
                                </div>
                                <div class="col-sm-2">
                                    <asp:Checkbox ID="CB_Learners" runat="server" Checked='<%# Bind("Learners") %>' Text="Learners" OnCheckedChanged="CB_SchInfo_CheckedChanged" />
                                </div>
                                <div class="col-sm-2">
                                    <asp:CheckBox ID="CB_Snacks" runat="server" Checked='<%# Bind("Snacks") %>' Text="Snacks" OnCheckedChanged="CB_SchInfo_CheckedChanged" />
                                </div>
                                <div class="col-sm-2">
                                    <asp:CheckBox ID="CB_Dinner" runat="server" Checked='<%# Bind("Dinners") %>' Text="Dinners" OnCheckedChanged="CB_SchInfo_CheckedChanged" />
                                </div>
                            </div>
                            <div id="SchoolCEP" runat="server"><!-- Will only appear as applicable -->
                                <div class="row" style="border-bottom-color:orchid;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;" >
                                    <div class="col-sm-2" style="background-color:orchid;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        CEP Settings
                                        <asp:HiddenField ID="HFCEPDist" runat="server" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Coverage:
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxCEPCoverage" runat="server" Enabled="false"></asp:TextBox> 
                                        <asp:HiddenField ID="HFCEPCoverageInt" runat="server" />
                                    </div>
                                    <div class="col-sm-5">
                                        <asp:CheckBox ID="CheckBoxSchoolCEP" runat="server" Text="CEP Participating School" AutoPostBack="true" Enabled="false" Checked='<%# Bind("PartialCEPPartic") %>' OnCheckedChanged="CB_SchInfo_CheckedChanged"  />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-4">
                                        <asp:CheckBox ID="CB_UseCustomCEPPcts" runat="server" Enabled="true" Text="Use Custom Percentages" AutoPostBack="true" OnCheckedChanged="CB_UseCustomCEPPcts_CheckedChanged" Checked='<%# Bind("UseCustomCEPPct") %>' />
                                    </div>
                                    <div class="col-sm-4">
                                        Paid
                                        <asp:HiddenField ID="HFDistCEPaidPct" runat="server" Value='<%# Eval("DistCEPPaidPct") %>' />
                                        <asp:TextBox ID="TextBoxSchoolCEPPaidPct" runat="server" Text='<%# Bind("SchoolCEPPaidPct","{0:F4}" ) %>' Width="100" MaxLength="8" OnTextChanged="TextBoxSchoolInfo_TextChanged" Enabled="false"></asp:TextBox>%
                                        <asp:RangeValidator ID="ValidRangePaid" runat="server" ControlToValidate="TextBoxSchoolCEPPaidPct" Display="Dynamic" CssClass="validators"
                                             MaximumValue="100" MinimumValue="0" SetFocusOnError="true" Type="Double" Text="Invalid Paid Percent" ValidationGroup="Save">*Invalid Paid Percent</asp:RangeValidator>

                                    </div>
                                    <div class="col-sm-4">
                                        Free
                                        <asp:HiddenField ID="HFDistCEFreePct" runat="server" Value='<%# Eval("DistCEPFreePct") %>' />
                                        <asp:TextBox ID="TextBoxSchoolCEPFreePct" runat="server" Text='<%# Bind("SchoolCEPFreePct","{0:F4}" ) %>' Width="100" MaxLength="8" OnTextChanged="TextBoxSchoolInfo_TextChanged" Enabled="false"></asp:TextBox>%
                                        <asp:RangeValidator ID="ValidRangeFree" runat="server" ControlToValidate="TextBoxSchoolCEPFreePct" Display="Dynamic" CssClass="validators"
                                             MaximumValue="100" MinimumValue="0" SetFocusOnError="true" Type="Double" Text="Invalid Paid Percent" ValidationGroup="Save">*Invalid Free Percent</asp:RangeValidator>

                                    </div>
                                </div>
                            </div>
                            <div id="School Info">
                                <div class="row" style="border-bottom-color:deepskyblue;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;" >
                                    <div class="col-sm-2" style="background-color:deepskyblue;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        School Info
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Eligible:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxPaidElig" runat="server" Text='<%# Bind("EligiblePaid") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Eligible:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxReducedElig" runat="server" Text='<%# Bind("EligibleReduced") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Eligible:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxFreeElig" runat="server" Text='<%# Bind("EligibleFree") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Enrollment:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxEnrollment" runat="server" Text='<%# Bind("Enrollment") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged"  />
                                    </div>
                                    <div class="col-sm-2">
                                        Attendance Factor:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxAttFactor" runat="server" Text='<%# Bind("AF", "{0:P1}") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                            </div>
                            <div id="Student Meal Prices">
                                <div class="row" style="border-bottom-color:turquoise;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                    <div class="col-sm-2" style="background-color:turquoise;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Student Meal Prices
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <strong>Prices below are effective at the start of the program year.</strong>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Elem Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastElPrice" runat="server" Text='<%# Bind("BfastElPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Jr Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastJrPrice" runat="server" Text='<%# Bind("BfastJrPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Sr Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastSrPrice" runat="server" Text='<%# Bind("BfastSrPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Reduced Breakfast:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastRedPrice" runat="server" Text='<%# Bind("BfastRedPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Elem Lunch:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchElPrice" runat="server" Text='<%# Bind("LunchElPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Jr Lunch:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchJrPrice" runat="server" Text='<%# Bind("LunchJrPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Sr Lunch:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchSrPrice" runat="server" Text='<%# Bind("LunchSrPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Tier1 Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchTier1Price" runat="server" Text='<%# Bind("LunchTier1Price") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Tier2 Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchTier2Price" runat="server" Text='<%# Bind("LunchTier2Price") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchRedPrice" runat="server" Text='<%# Bind("LunchRedPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Snack:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxOSnackPaidPrice" runat="server" Text='<%# Bind("OSnackPaidPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Snack:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxOSnackRedPrice" runat="server" Text='<%# Bind("OSnackRedPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Adult Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxAdultLunchPrice" runat="server" Text='<%# Bind("AdultLunchPrice") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
<!-- Mid Year Price Changes -->
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:CheckBox ID="CBMidYearPriceChange" runat="server" Checked='<%# Eval("Price2EffectiveDate", "{0:d}")=="12/31/9999"? false:true %>' Text="Enable mid-year breakfast price change" AutoPostBack="true" OnCheckedChanged="CBMidYearPriceChange_CheckedChanged" />
                                    </div>
                                </div>
                                <div id="DivMidYearPrices" runat="server" style="background-color:powderblue;" >
                                     <div class="row">
                                        <div class="col-sm-2">
                                            Effective Date:
                                        </div>
                                        <div class="col-sm-2">
                                            <asp:TextBox ID="TextBoxEffectivePricesDate" runat="server" Text='<%# Bind("Price2EffectiveDate", "{0:d}") %>' Width="100" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">
                                            Paid Elem Breakfast:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxBfastElPrice2" runat="server" Text='<%# Bind("BfastElPrice2") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Paid Jr Breakfast:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxBfastJrPrice2" runat="server" Text='<%# Bind("BfastJrPrice2") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Paid Sr Breakfast:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxBfastSrPrice2" runat="server" Text='<%# Bind("BfastSrPrice2") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">
                                            Reduced Breakfast:<br />
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxBfastRedPrice2" runat="server" Text='<%# Bind("BfastRedPrice2") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                </div>
                            </div>
<!-- Mid Year Price Changes (Lunch) -->
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:CheckBox ID="CBMidYearPriceChangeL" runat="server" Checked='<%# Eval("LPrice2EffectiveDate", "{0:d}")=="12/31/9999"? false:true %>' Text="Enable mid-year lunch price change" AutoPostBack="true" OnCheckedChanged="CBMidYearPriceChangeL_CheckedChanged" />
                                    </div>
                                </div>
                                <div id="DivMidYearPricesL" runat="server" style="background-color:powderblue;" >
                                     <div class="row">
                                        <div class="col-sm-2">
                                            Effective Date:
                                        </div>
                                        <div class="col-sm-2">
                                            <asp:TextBox ID="TextBoxEffectivePricesDateL" runat="server" Text='<%# Bind("LPrice2EffectiveDate", "{0:d}") %>' Width="100" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">
                                            Paid Elem Lunch:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxLunchElPrice2" runat="server" Text='<%# Bind("LunchElPrice2") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Paid Jr Lunch:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxLunchJrPrice2" runat="server" Text='<%# Bind("LunchJrPrice2") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Paid Sr Lunch:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxLunchSrPrice2" runat="server" Text='<%# Bind("LunchSrPrice2") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">
                                            Reduced Lunch:<br />
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxLunchRedPrice2" runat="server" Text='<%# Bind("LunchRedPrice2") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                </div>
                            </div>


                            <div id="Reimbursement Rates">
                                <div class="row" style="border-bottom-color:greenyellow;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                    <div class="col-sm-2" style="background-color:greenyellow;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Reimburse Rates
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <strong>Rates below are effective at the start of the program year.</strong>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastPaidRate" runat="server" Text='<%# Bind("BfastPaidRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastRedRate" runat="server" Text='<%# Bind("BfastRedRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastFreeRate" runat="server" Text='<%# Bind("BfastFreeRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchPaidRate" runat="server" Text='<%# Bind("LunchPaidRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchRedRate" runat="server" Text='<%# Bind("LunchRedRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchFreeRate" runat="server" Text='<%# Bind("LunchFreeRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Snack:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxOSnackPaidRate" runat="server" Text='<%# Bind("OSnackPaidRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Snack:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxOSnackRedRate" runat="server" Text='<%# Bind("OSnackRedRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Snack:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxOSnackFreeRate" runat="server" Text='<%# Bind("OSnackFreeRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Breakfast Incentive:
                                    </div>
                                    <div class="col-sm-1" >
                                        <asp:TextBox ID="TextBoxBreakfastIncent" runat="server" Text='<%# Bind("BreakfastIncent") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Dinner:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxDinnerRate" runat="server" Text='<%# Bind("DinnerRate") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
<!-- Mid Year Changes -->
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:CheckBox ID="CBMidYearRateChange" runat="server" Checked='<%# Eval("Reim2EffectiveDate", "{0:d}")=="12/31/9999"? false:true %>' Text="Enable mid-year rate change" AutoPostBack="true" OnCheckedChanged="CBMidYearRateChange_CheckedChanged" />
                                    </div>
                                </div>
                                <div id="DivMidYearRates" runat="server" style="background-color:powderblue;" >
                                    <div class="row">
                                        <div class="col-sm-2">
                                            Effective Date:
                                        </div>
                                        <div class="col-sm-2">
                                            <asp:TextBox ID="TextBoxEffectiveRatesDate" runat="server" Text='<%# Bind("Reim2EffectiveDate", "{0:d}") %>' Width="100" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">
                                            Paid Breakfast:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxBfastPaidRate2" runat="server" Text='<%# Bind("BfastPaidRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Reduced Breakfast:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxBfastRedRate2" runat="server" Text='<%# Bind("BfastRedRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Free Breakfast:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxBfastFreeRate2" runat="server" Text='<%# Bind("BfastFreeRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">
                                            Paid Lunch:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxLunchPaidRate2" runat="server" Text='<%# Bind("LunchPaidRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Reduced Lunch:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxLunchRedRate2" runat="server" Text='<%# Bind("LunchRedRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Free Lunch:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxLunchFreeRate2" runat="server" Text='<%# Bind("LunchFreeRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">
                                            Paid Snack:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxOSnackPaidRate2" runat="server" Text='<%# Bind("OSnackPaidRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Reduced Snack:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxOSnackRedRate2" runat="server" Text='<%# Bind("OSnackRedRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                        <div class="col-sm-2">
                                            Free Snack:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxOSnackFreeRate2" runat="server" Text='<%# Bind("OSnackFreeRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">
                                            Dinner:
                                        </div>
                                        <div class="col-sm-1">
                                            <asp:TextBox ID="TextBoxDinnerRate2" runat="server" Text='<%# Bind("DinnerRate2") %>' Width="60" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                        </div>
                                    </div>
                                </div>
                                
                            </div>
                            
                            <div id="Budgets">
                                <div class="row" style="border-bottom-color:yellow;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                    <div class="col-sm-2" style="background-color:yellow;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Daily Meal Budgets
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Elem Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtPaidElemB" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Jr Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtPaidJrB" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Sr Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtPaidSrB" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Reduced Breakfast:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtRedB" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtFreeB" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Elem Lunch:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtPaidElemL" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Jr Lunch:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtPaidJrL" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Sr Lunch:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtPaidSrL" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Tier1 Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtTier1L" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Tier2 Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtTier2L" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Reduced Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtRedL" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtFreeL" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Student AlaCarte:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtStuAla" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Adult AlaCarte:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtAdultAla" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Other AlaCarte 1:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtOther1" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Other AlaCarte 2:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBdgtOther2" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:HiddenField ID="HFDistSchoolInfoSaved" runat="server" Value="0" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </EditItemTemplate>
                </asp:FormView>
                <asp:Label ID="LabelErrors" runat="server" ForeColor="Red"></asp:Label>
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SELECTCOMMAND="SELECT * FROM DistrictSchools WHERE DistrictID=@DistrictID AND Inactive=0 ORDER BY SortOrder, SchoolName" >
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchoolYearSetup" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT 1, 
                    ISNULL(DSYS.[ProgramYear], @ProgramYear) ProgramYear, ISNULL(DSYS.DistrictID, @DistrictID) DistrictID, ISNULL(DSYS.SchoolNum, @SchoolNum) SchoolNum,
                    ISNULL([BfastElPrice], 0) BfastElPrice, ISNULL([BfastJrPrice],0) BfastJrPrice,  ISNULL( [BfastSrPrice],0) BfastSrPrice,
                    ISNULL( [BfastRedPrice],0) [BfastRedPrice],ISNULL( [LunchElPrice],0) [LunchElPrice],ISNULL( [LunchJrPrice],0) [LunchJrPrice],ISNULL( [LunchSrPrice],0) [LunchSrPrice],
                    ISNULL( [LunchTier1Price],0) [LunchTier1Price],ISNULL( [LunchTier2Price],0) [LunchTier2Price],ISNULL( [LunchRedPrice],0) [LunchRedPrice],ISNULL( [BfastPaidRate],0) [BfastPaidRate],
                    ISNULL( [BfastRedRate],0) [BfastRedRate],ISNULL( [BfastFreeRate],0) [BfastFreeRate],ISNULL( [LunchPaidRate],0) [LunchPaidRate],ISNULL( [LunchRedRate],0) [LunchRedRate],
                    ISNULL( [LunchFreeRate],0) [LunchFreeRate],ISNULL( [OSnackPaidPrice],0) [OSnackPaidPrice],ISNULL( [OSnackRedPrice],0) [OSnackRedPrice],ISNULL( [OSnackPaidRate],0) [OSnackPaidRate],
                    ISNULL( [OSnackRedRate],0) [OSnackRedRate],ISNULL( [OSnackFreeRate],0) [OSnackFreeRate],ISNULL( [AF],0) [AF],ISNULL( [Enrollment],0) [Enrollment],
                    ISNULL( [EligiblePaid],0) [EligiblePaid],ISNULL( [EligibleFree],0) [EligibleFree],ISNULL( [EligibleReduced],0) [EligibleReduced],ISNULL( [BreakfastIncent],0) [BreakfastIncent],
                    ISNULL( [Tiers],0) [Tiers],ISNULL( [OtherSales],0) [OtherSales],ISNULL( [Learners],0) [Learners],ISNULL( [Snacks],0) [Snacks],
                    ISNULL( [SchoolCEPFreePct],0) [SchoolCEPFreePct],ISNULL( [SchoolCEPPaidPct],0) [SchoolCEPPaidPct],ISNULL( [PartialCEPPartic],0) [PartialCEPPartic],ISNULL( [UseCustomCEPPct],0) [UseCustomCEPPct],
                    ISNULL([Breakfasts], 0)[Breakfasts],
                    ISNULL([DistCEPPaidPct],0) DistCEPPaidPct, ISNULL([DistCEPFreePct],0) DistCEPFreePct,
                    ISNULL([Reim2EffectiveDate],CAST('12/31/9999' AS DATE)) Reim2EffectiveDate,
                    ISNULL( [BfastPaidRate2],0) [BfastPaidRate2],ISNULL( [BfastRedRate2],0) [BfastRedRate2],ISNULL( [BfastFreeRate2],0) [BfastFreeRate2],ISNULL( [LunchPaidRate2],0) [LunchPaidRate2],
                    ISNULL( [LunchRedRate2],0) [LunchRedRate2],ISNULL( [LunchFreeRate2],0) [LunchFreeRate2],ISNULL( [OSnackPaidRate2],0) [OSnackPaidRate2],
                    ISNULL( [OSnackRedRate2],0) [OSnackRedRate2],ISNULL( [OSnackFreeRate2],0) [OSnackFreeRate2],
					ISNULL([DinnerRate],0) DinnerRate, ISNULL([DinnerRate2],0) DinnerRate2, ISNULL([Dinners],CAST(0 AS BIT)) Dinners, ISNULL([AdultLunchPrice],0) AdultLunchPrice,
                    ISNULL([Price2EffectiveDate],CAST('12/31/9999' AS DATE)) Price2EffectiveDate,
                    ISNULL( [BfastElPrice2],0) [BfastElPrice2], ISNULL( [BfastJrPrice2],0) [BfastJrPrice2], ISNULL( [BfastSrPrice2],0) [BfastSrPrice2], ISNULL( [BfastRedPrice2],0) [BfastRedPrice2],
                    ISNULL([LPrice2EffectiveDate],CAST('12/31/9999' AS DATE)) LPrice2EffectiveDate,
                    ISNULL( [LunchElPrice2],0) [LunchElPrice2], ISNULL( [LunchJrPrice2],0) [LunchJrPrice2], ISNULL( [LunchSrPrice2],0) [LunchSrPrice2], ISNULL( [LunchRedPrice2],0) [LunchRedPrice2]
                    FROM [District] D
                    LEFT OUTER JOIN [DistrictYearInfo] DYI ON D.DistrictID=DYI.DistrictID
                    LEFT OUTER JOIN [DistrictSchoolYearSetup] DSYS ON @ProgramYear=DSYS.ProgramYear AND D.DistrictID=DSYS.DistrictID AND @SchoolNum=DSYS.SchoolNum
                    WHERE D.DistrictID=@DistrictID"
                    >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int16" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceBDGT" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" 
                    DataSourceMode="DataReader" 
                    SelectCommand="SELECT [ElemPaidB_Site], [MiddlePaidB_Site], [HSPaidB_Site], [ElemReducedB_Site]+[MiddleReducedB_Site]+[HSReducedB_Site] ReducedBreakfast,
                    [FreeB_Site]+[FreeBMid_Site]+[FreeBHS_Site] FreeBreakfast, [ElemPaidL_Site], [MiddlePaidL_Site], [HSPaidL_Site], 
                    [ElemPaidTier1L_Site]+[MiddlePaidTier1L_Site]+[HSPaidTier1L_Site] PaidTier1L, [ElemPaidTier2L_Site]+[MiddlePaidTier2L_Site]+[HSPaidTier2L_Site] PaidTier2L,
                    [ElemReducedL_Site]+[MiddleReducedL_Site]+[HSReducedL_Site] ReducedLunch, [FreeL_Site]+[FreeLMid_Site]+[FreeLHS_Site] FreeLunch,
                    [AlaCarteB_Site]+[AlaCarteL_Site] StudentAla, [AdultPaidB_Site]+[AdultPaidL_Site] Adult, [AlaCarteSalesRevInc_Site], [OtherRevVending_Site],
                    [CEP], [CEPCoverage]
                    FROM [vwBDGT_SiteCounts] COUNTS
                    INNER JOIN [vwBDGT_Totals] TOTALS ON COUNTS.BidNumber=TOTALS.BidNumber AND COUNTS.BudgetYear=TOTALS.BudgetYear
                    WHERE (COUNTS.[BidNumber] = @BidNumber) AND COUNTS.BudgetYear=@BudgetYear AND [ORD]=@SchoolNum">
                    <SelectParameters>
                        <asp:SessionParameter Name="BidNumber" SessionField="CurrentDistrict" Type="String" />
                        <asp:ControlParameter Name="BudgetYear" ControlID="HFBudgetYear" PropertyName="Value" Type="String" />
                        <asp:ControlParameter Name="SchoolNum" ControlID="DropDownListSchools" PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
