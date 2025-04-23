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
                        <div>
                        <asp:LinkButton ID="ISave" runat="server" 
                                    ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                    CssClass="fas fa-save mGridLinkButton" Width="170"   />
                            
                        </div>
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
                        </p>
                        <div class="messagealert" id="alert_container"></div>
                    </div>
                </div>
            </div>
            <div id="DistrictInfoSection">
                <asp:FormView ID="FormViewDistInfo" runat="server" CssClass="mGridRecaps" DataSourceID="SqlDataSourceDistYearInfo" DefaultMode="Edit"
                    OnPreRender="FormViewDistInfo_PreRender" OnDataBound="FormViewDistInfo_DataBound">
                    <EditItemTemplate>
                        <div class="container">
                            <div id="District Info">
                                <div class="row" style="border-bottom-color:deepskyblue;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;" >
                                    <div class="col-sm-2" style="background-color:deepskyblue;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        District Info
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Regional Manager:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxRM" runat="server" Text='<%# Bind("DistrictName") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="50" Width="100%" />
                                    </div>
                                    <div class="col-sm-2">
                                        FSD Name:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxFSD" runat="server" Text='<%# Bind("FSDName") %>'  OnTextChanged="DistInfoControl_Changed" MaxLength="50" Width="100%" />
                                    </div>
                                </div>
                                <div class="row">
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
                                        <asp:DropDownList ID="DDL_AccountType" runat="server" SelectedValue='<%# Bind("AcctType") %>' Enabled='<%# Session["AccessLevel"]=="User"?false:true %>' OnSelectedIndexChanged="DistInfoControl_Changed" 
                                            BackColor='<%# Session["AccessLevel"]=="User"? System.Drawing.Color.LightGray : System.Drawing.Color.White %>' Width="100%" >
                                            <asp:ListItem Text="Unknown" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Cost Per Meal" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Full Bill Purchase" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Full Bill Usage" Value="3"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Cost Per Meal Bill Rate
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxCPM" runat="server" Enabled="false" Width="100%"></asp:TextBox>
                                    </div>
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
                                    <div class="col-sm-3">
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
                                    <div class="col-sm-3">
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
                                    <div class="col-sm-3">
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
                                    <div class="col-sm-3">
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
                                    <div class="col-sm-3">
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Food Cost / Meal:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxFoodCPM" runat="server" Enabled="false" Width="100%" />
                                    </div>
                                    <div class="col-sm-3">
                                        Paper/Clean Cost / Meal:
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxPaperCleanCPM" runat="server" Enabled="false" Width="100%" />
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
                <asp:SqlDataSource ID="SqlDataSourceDistYearInfo" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"  
                    SELECTCOMMAND="SELECT D.*, ISNULL([AcctType],0) AcctType,ISNULL([GM],'') GM,ISNULL([RMCode],'') RMCode,ISNULL([RMName],'') RMName,ISNULL([FSDName], '') FSDName
                    ,ISNULL([FSDAddress],'') FSDAddress ,ISNULL([FSDCity],'') FSDCity,ISNULL([FSDState],'') FSDState
                    ,ISNULL([FSDZip],'') FSDZip,ISNULL([FSDEmail],'') FSDEmail,ISNULL([FSDPhone],'') FSDPhone,ISNULL([FSDCell],'') FSDCell
	                ,ISNULL([BusinessMgr],'') BusinessMgr,ISNULL([Superintendent],'') Superintendent
                    FROM [District] D
                    LEFT OUTER JOIN [DistrictYearInfo] DYI ON D.DistrictID=DYI.DistrictID AND @ProgramYear=DYI.[ProgramYear]
                    WHERE (D.[DistrictID] = @DistrictID) " >
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>

                <asp:SqlDataSource ID="SqlDataSourceBDGT" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" 
                    DataSourceMode="DataReader" 
                    SelectCommand="SELECT [DaysOfSvcLunch], [DailyCashBudget] + ([PDE3086Revenue]/[DaysOfSvcLunch]) DailyCashCalc, [DailyReimBudget], [DailyCosts], [DistrictCosts], 
                    [PaperCPM]+[CleaningCPM] PaperCleanCPM, [FoodCPM], [DailyInterest], [DailyExpenses], [DailyOtherSales], [DailyPSERSCredit], ([SFALabor]/[DaysOfSvcLunch]) DailySFALabor, [MaxofFringePercent],
                    [CostPerMeal], [MealEquivalentFactor], [CBS_FSMCAdmin], [CBS_FSMCMgmt], [FSDCosts], ROUND([AccountingNUTR] + [TechFeePlusSoftware] + [RecruitReplace] + [Liability] + [Training] + [WellnessFeeNUTR] + [CACFPAdminFeeTotal],2) AddOnFees
                    FROM [vwBDGT_Totals] 
                    WHERE ([BidNumber] = @BidNumber) AND BudgetYear=@BudgetYear AND [DaysOfSvcLunch]<>0">
                    <SelectParameters>
                        <asp:SessionParameter Name="BidNumber" SessionField="CurrentDistrict" Type="String" />
                        <asp:SessionParameter Name="BudgetYear" SessionField="BudgetYear" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
