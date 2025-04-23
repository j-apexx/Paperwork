<%@ Page Title="Import Indirect Costs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportIndirectCosts.aspx.cs" Inherits="Paperwork.ImportIndirectCosts" %>
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
            Use this page to import indirect costs from the Budgets. 
            <br />
            Select the districts and the available Non-Food expense totals will populate. Clicking import will bring in the totals distributed over the 12 months for the Program Year.
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
            <div class="row">
                <div class="col-sm-4">
                    Indirect Cost Label:
                </div>
                <div class="col-sm-6">
                    <asp:TextBox ID="TextBoxIndirectCosts" runat="server" ToolTip="This will be the label for each monthly Indirect Cost." >District Cost</asp:TextBox>
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
                    <div class="col-sm-6">
            
                        <asp:GridView ID="GridViewDistBDGTTotals" runat="server" DataSourceID="SqlDataSourceDistTotals"
                            ShowHeaderWhenEmpty="true" CssClass="mGrid" Width="100%" DataKeyNames="DistrictID, DistrictTotal"
                            OnRowDataBound="GridViewDistBDGTTotals_RowDataBound"></asp:GridView>
                    </div>       
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
            UNION ALL
            SELECT '%' as [DistrictID], '*All Districts*' as [DistrictName], 0 as [Inactive]
            ORDER BY [Desc]"
            FilterExpression="Inactive<={0}">
         <FilterParameters>
             <asp:ControlParameter ControlID="CBActiveOnly" PropertyName="Checked" Type="SByte" Name="Active" />
         </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceDistTotals" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTs %>" 
            SelectCommand="SELECT RTRIM(VW.Bidnumber) DistrictID, RTRIM(DSCRIPTN) AS DistrictName, DistrictTotal FROM [vwBDGT_NonFoodCostsBySvcDays] VW
            INNER JOIN BDGT_Bids B ON VW.BidNumber=B.BidNumber
            WHERE VW.BidNumber LIKE @BidNumber AND VW.BudgetYear=@BudgetYear 
            ORDER BY VW.BidNumber">
            <SelectParameters>
                <asp:ControlParameter ControlID="LabelBudgetYear" Name="BudgetYear" PropertyName="Text" Type="String" />
                <asp:ControlParameter ControlID="DDLDistricts" Name="BidNumber" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
