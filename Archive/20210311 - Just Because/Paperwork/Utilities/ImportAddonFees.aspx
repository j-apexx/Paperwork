<%@ Page Title="Import Add-On Fees" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportAddonFees.aspx.cs" Inherits="Paperwork.ImportAddonFees" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="hdrGroup" class="row">
        <div id="hdrDiv" class="col-sm-6">
            <h2><%: Title %></h2>
            Use this page to import Add-on Fees from the Budgets. Note that Add-on Fees are automatically imported from active budgets during creation of the Program Year. Use this page to import new districts, or districts which were missed. 
            Amounts will be distributed according to the billing periods setup from the Nutrition Budgets window, displayed for reference below.
        </div>
        <div id="SelectionDiv" class="col-sm-6">
            <div class="row">
                <div class="col-sm-4">
                    Program Year:
                </div>
                <div class="col-sm-6">
                    <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                        DataValueField="ProgramYear" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    <asp:HiddenField ID="HFProgramYearStartDate" runat="server" />
                    <asp:HiddenField ID="HFProgramYearEndDate" runat="server" />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-4">
                    District:
                </div>
                <div class="col-sm-6">
                    <asp:DropDownList ID="DDLDistricts" runat="server" DataSourceID="SqlDataSourceDistricts" DataTextField="Desc" 
                        DataValueField="DistrictID" AutoPostBack="true">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-4">
                    &nbsp;
                </div>
                <div class="col-sm-6" style="padding-top:5px;">
                    <asp:CheckBox ID="CBActiveOnly" runat="server" Text=" Display Inactive Districts" Checked="false" AutoPostBack="true" OnCheckedChanged="CBActiveOnly_CheckedChanged"/>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-4">
                    Budget Year:
                </div>
                <div class="col-sm-6">
                    <asp:Label ID="LabelBudgetYear" runat="server" ToolTip="Budget Year id from the Budget program. This value is configured in Setup Program Years." ></asp:Label>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-12">&nbsp;</div>
    </div>
    
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="UtilityPanel" class="row">
                    <div class="col-sm">
            
                        <asp:GridView ID="GridViewDistBDGTTotals" runat="server" DataSourceID="SqlDataSourceAddonFees"
                            ShowHeaderWhenEmpty="true" CssClass="mGrid" Width="100%" DataKeyNames="BidNumber, BudgetYear"
                            OnRowDataBound="GridViewDistBDGTTotals_RowDataBound" AutoGenerateColumns="false" >
                            <Columns>
                                <asp:BoundField DataField="BidNumber" HeaderText="District ID"  />
                                <asp:BoundField DataField="BudgetYear" HeaderText="Budget Year"  />
                                <asp:BoundField DataField="AccountingNUTR" DataFormatString="{0:F2}" HeaderText="Accounting"  />
                                <asp:BoundField DataField="TechFeePlusSoftware" DataFormatString="{0:F2}" HeaderText="Tech Fee & SW"  />
                                <asp:BoundField DataField="RecruitReplace" DataFormatString="{0:F2}" HeaderText="Recruit/Replace"  />
                                <asp:BoundField DataField="Liability" DataFormatString="{0:F2}" HeaderText="Liability"  />
                                <asp:BoundField DataField="Training" DataFormatString="{0:F2}" HeaderText="Training"  />
                                <asp:BoundField DataField="WellnessFeeNUTR" DataFormatString="{0:F2}" HeaderText="Wellness"  />
                                <asp:BoundField DataField="BillingPeriods" DataFormatString="{0:F0}" HeaderText="Billing Periods"  />
                                <asp:BoundField DataField="BillingJul" DataFormatString="{0:F0}" HeaderText="Jul"  />
                                <asp:BoundField DataField="BillingAug" DataFormatString="{0:F0}" HeaderText="Aug"  />
                                <asp:BoundField DataField="BillingSep" DataFormatString="{0:F0}" HeaderText="Sep"  />
                                <asp:BoundField DataField="BillingOct" DataFormatString="{0:F0}" HeaderText="Oct"  />
                                <asp:BoundField DataField="BillingNov" DataFormatString="{0:F0}" HeaderText="Nov"  />
                                <asp:BoundField DataField="BillingDec" DataFormatString="{0:F0}" HeaderText="Dec"  />
                                <asp:BoundField DataField="BillingJan" DataFormatString="{0:F0}" HeaderText="Jan"  />
                                <asp:BoundField DataField="BillingFeb" DataFormatString="{0:F0}" HeaderText="Feb"  />
                                <asp:BoundField DataField="BillingMar" DataFormatString="{0:F0}" HeaderText="Mar"  />
                                <asp:BoundField DataField="BillingApr" DataFormatString="{0:F0}" HeaderText="Apr"  />
                                <asp:BoundField DataField="BillingMay" DataFormatString="{0:F0}" HeaderText="May"  />
                                <asp:BoundField DataField="BillingJun" DataFormatString="{0:F0}" HeaderText="Jun"  />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div id="UtilityPanel" class="row">
                    <div class="col-sm-6">
                        <asp:LinkButton ID="BtnProcess" runat="server" CssClass="btn btn-primary" OnClick="BtnProcess_Click" >
                            <i class="fas fa-cogs"></i>
                            Process 
                        </asp:LinkButton>
                        <br />
                        Results:<br />
                        <asp:TextBox runat="server" ID="TextBoxResults" TextMode="MultiLine" ReadOnly="true" Width="100%"></asp:TextBox>
                    </div>

                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OnFiltering="SqlDataSourceDistricts_Filtering"
            SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc', Inactive
            FROM [District] 
            ORDER BY [Desc]"
            FilterExpression="Inactive<={0}">
         <FilterParameters>
             <asp:ControlParameter ControlID="CBActiveOnly" PropertyName="Checked" Type="SByte" Name="Active" />
         </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceAddonFees" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTs %>" 
            SelectCommand="SELECT T.BidNumber, T.BudgetYear, T.AccountingNUTR, T.TechFeePlusSoftware, T.RecruitReplace, T.Liability, T.Training, T.WellnessFeeNUTR, T.CACFPAdminFeeTotal, T.BillingPeriods,
            ISNULL(D.ELEVENTHDAYS1,0) BillingJul,ISNULL(D.ELEVENTHDAYS2,0) BillingAug,ISNULL(D.ELEVENTHDAYS3,0) BillingSep,ISNULL(D.ELEVENTHDAYS4,0) BillingOct,ISNULL(D.ELEVENTHDAYS5,0) BillingNov,ISNULL(D.ELEVENTHDAYS6,0) BillingDec,
            ISNULL(D.ELEVENTHDAYS7,0) BillingJan,ISNULL(D.ELEVENTHDAYS8,0) BillingFeb,ISNULL(D.ELEVENTHDAYS9,0) BillingMar,ISNULL(D.ELEVENTHDAYS10,0) BillingApr,ISNULL(D.ELEVENTHDAYS11,0) BillingMay,ISNULL(D.ELEVENTHDAYS12,0) BillingJun
            FROM [vwBDGT_Totals] T
            LEFT OUTER JOIN [vwBDGT_ScheduleDays] D ON T.BidNumber=D.BidNumber AND T.BudgetYear=D.BudgetYear
                 WHERE T.BudgetYear=@BudgetYear AND T.BidNumber LIKE @BidNumber AND BudgetType=2 AND ACTIVE=1 AND (AccountingNUTR+TechFeePlusSoftware+RecruitReplace+Liability+Training+WellnessFeeNUTR+CACFPAdminFeeTotal)<>0 ">
            <SelectParameters>
                <asp:ControlParameter ControlID="LabelBudgetYear" Name="BudgetYear" PropertyName="Text" Type="String" />
                <asp:ControlParameter ControlID="DDLDistricts" Name="BidNumber" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
