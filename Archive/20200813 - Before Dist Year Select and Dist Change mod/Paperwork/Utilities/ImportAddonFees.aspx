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
                            OnRowDataBound="GridViewDistBDGTTotals_RowDataBound"></asp:GridView>
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
            SelectCommand="SELECT BidNumber, BudgetYear, AccountingNUTR, TechFeePlusSoftware, RecruitReplace, Liability, Training, WellnessFeeNUTR, CACFPAdminFeeTotal FROM [vwBDGT_Totals] 
                 WHERE BudgetYear=@BudgetYear AND BidNumber LIKE @BidNumber AND BudgetType=2 AND ACTIVE=1 AND (AccountingNUTR+TechFeePlusSoftware+RecruitReplace+Liability+Training+WellnessFeeNUTR+CACFPAdminFeeTotal)<>0 ">
            <SelectParameters>
                <asp:ControlParameter ControlID="LabelBudgetYear" Name="BudgetYear" PropertyName="Text" Type="String" />
                <asp:ControlParameter ControlID="DDLDistricts" Name="BidNumber" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
