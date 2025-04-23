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
                        <asp:LinkButton ID="ISave" runat="server" 
                                    ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                    CssClass="fas fa-save mGridLinkButton" Width="170"   />
                    </div>
                    <div id="menuDiv" style="float:left;width:70%">
                        <br />
                        <p>
                            <strong>Program Year:</strong>
                            <asp:Label ID="LabelProgramYear" runat="server" ></asp:Label>
                            &nbsp;&nbsp;
                            <strong>District:</strong>
                            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                DataSourceID="SqlDataSourceMyDistricts" DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> 
                            &nbsp;&nbsp;
                            <strong>School:</strong><asp:DropDownList ID="DropDownListSchools" runat="server" AutoPostBack="True" 
                                DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" DataValueField="SchoolNum" ValidationGroup="GridView">
                            </asp:DropDownList> &nbsp;
                        </p>
                    </div>
                </div>
            </div>
            <div id="SchoolInfoSection">
                <asp:FormView ID="FormViewSchoolInfo" runat="server" CssClass="mGridRecaps" DataSourceID="SqlDataSourceSchoolYearSetup" DefaultMode="Edit"
                    OnPreRender="FormViewSchoolInfo_PreRender" OnDataBound="FormViewSchoolInfo_DataBound" >
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
                                        <asp:TextBox ID="TextBoxAttFactor" runat="server" Text='<%# Bind("AF", "{0:P1}") %>' Width="75" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
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
                                </div>
                            </div>
                            <div id="Reimbursement Rates">
                                <div class="row" style="border-bottom-color:greenyellow;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                    <div class="col-sm-2" style="background-color:greenyellow;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Reimburse Rates
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastPaidRate" runat="server" Text='<%# Bind("BfastPaidRate") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastRedRate" runat="server" Text='<%# Bind("BfastRedRate") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBfastFreeRate" runat="server" Text='<%# Bind("BfastFreeRate") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchPaidRate" runat="server" Text='<%# Bind("LunchPaidRate") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchRedRate" runat="server" Text='<%# Bind("LunchRedRate") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxLunchFreeRate" runat="server" Text='<%# Bind("LunchFreeRate") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Breakfast Incentive:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxBreakfastIncent" runat="server" Text='<%# Bind("BreakfastIncent") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Snack:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxOSnackPaidRate" runat="server" Text='<%# Bind("OSnackPaidRate") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Snack:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxOSnackRedRate" runat="server" Text='<%# Bind("OSnackRedRate") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Snack:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBoxOSnackFreeRate" runat="server" Text='<%# Bind("OSnackFreeRate") %>' Width="50" OnTextChanged="TextBoxSchoolInfo_TextChanged" />
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
                                        <asp:TextBox ID="TextBox1" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Jr Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox4" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Sr Breakfast:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox5" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Reduced Breakfast:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox6" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Elem Lunch:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox7" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Jr Lunch:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox8" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Sr Lunch:<br />
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox9" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Tier1 Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox10" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Tier2 Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox11" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Reduced Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox12" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Free Lunch:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox15" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Student AlaCarte:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox13" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Adult AlaCarte:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox14" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Other AlaCarte 1:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox16" runat="server" Text="" Width="50" ReadOnly="true" />
                                    </div>
                                    <div class="col-sm-2">
                                        Other AlaCarte 2:
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:TextBox ID="TextBox17" runat="server" Text="" Width="50" ReadOnly="true" />
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
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceMyDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc'
                    FROM [District] WHERE DistrictID IN (SELECT DistrictID FROM [UserDistrictAccess] WHERE Userid=@UserID) OR (@AccessLevel<>'User')
                    ORDER BY [DistrictName]" >
                    <SelectParameters>
                        <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                        <asp:SessionParameter Name="AccessLevel" SessionField="AccessLevel" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SELECTCOMMAND="SELECT * FROM DistrictSchools WHERE DistrictID=@DistrictID AND Inactive=0 ORDER BY SchoolName" >
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchoolYearSetup" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT 1,* FROM [DistrictSchoolYearSetup] WHERE (([DistrictID] = @DistrictID) AND ([ProgramYear] = @ProgramYear) AND ([SchoolNum] = @SchoolNum))"
                    OnSelected="SqlDataSourceSchoolYearSetup_Selected">
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int16" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
