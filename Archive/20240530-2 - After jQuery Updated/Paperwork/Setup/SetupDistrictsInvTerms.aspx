<%@ Page Title="Setup District Invoice Terms" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupDistrictsInvTerms.aspx.cs" Inherits="Paperwork.SetupDistrictsInvTerms" %>
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
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <h5><%: Title %></h5>
    Districts marked as New or Rebid, or having prepayments in the budget, affect terms verbiage on the bottom of invoices.<br />
    -Use this page to change the New or Rebid flag.<br />
    -Prepayments are set on the active budgets, and are for informational purposes only.<br />
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="messagealert" id="alert_container"></div><!-- Feedback for new invoice creation -->
                Program Year: <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" DataValueField="ProgramYear" 
                    AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                <asp:HiddenField ID="HFBudgetYear" runat="server" />
                <asp:GridView ID="GridViewDistricts" runat="server" AutoGenerateColumns="False" CssClass="mGrid" DataSourceID="SqlDataSourceDistricts" DataKeyNames="DistrictID"
                    AllowSorting="True" OnRowDataBound="GridViewDistricts_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="DistrictID" HeaderText="District ID" SortExpression="DistrictID" ReadOnly="true" HeaderStyle-Width="75" />
                        <asp:BoundField DataField="DistrictName" HeaderText="Name" SortExpression="DistrictName" HeaderStyle-Width="100" ItemStyle-Width="200" ControlStyle-Width="200" />
                        <asp:TemplateField HeaderText="New or Rebid" SortExpression="NewOrRebid">
                            <EditItemTemplate>
                                <asp:CheckBox ID="cbNORBEdit" runat="server" Checked='<%# Bind("NewOrRebid") %>' ToolTip="New Or Rebid" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="cbNORB" runat="server" Checked='<%# Eval("NewOrrebid") %>' Enabled="false" ToolTip="New Or Rebid" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Prepayment" >
                            <EditItemTemplate>
                                <asp:CheckBox ID="cbPrePayEdit" runat="server" Enabled="false"  ToolTip="Prepayment" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="cbPrePay" runat="server" Enabled="false" ToolTip="Prepayment" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <div style="width:60px;text-align:center;">
                                <asp:LinkButton runat="server" CommandName="Update" ID="ISave" ToolTip="Save" 
                                    CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="EditComm" />
                                <asp:LinkButton runat="server" CommandName="Cancel" ID="ICancel" ToolTip="Cancel" 
                                    CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                </div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <div style="width:60px;text-align:center;">
                                <asp:LinkButton runat="server" CommandName="Edit" ID="IEdit" ToolTip="Edit" 
                                    CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  />
                                </div>
                            </ItemTemplate>
                            <ItemStyle Width="50" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [ProgramYear], BudgetYear FROM [ProgramYear] ORDER BY [StartDate] DESC" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT DYI.DistrictID, D.DistrictName,  NewOrRebid 
                FROM DistrictYearInfo DYI
                INNER JOIN District D ON DYI.DistrictID=D.DistrictID
                WHERE ProgramYear=@ProgramYear" 
            UpdateCommand="UPDATE [DistrictYearInfo] SET [NewOrRebid]=@NewOrRebid
              WHERE [DistrictID] = @DistrictID AND ProgramYear=@ProgramYear"
             >
            <UpdateParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Type="String" PropertyName="SelectedValue" Name="ProgramYear" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="NewOrRebid" Type="Byte" DefaultValue="0" />
            </UpdateParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Type="String" PropertyName="SelectedValue" Name="ProgramYear" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceBDGT" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" 
            
            SelectCommand="SELECT CAST(IIF(ISNULL(TNGAdvancePayment,0)=2, 1, 0) as BIT) TNGAdvancePaymentBit
                FROM BDGT_BudgetHDR HDR
                INNER JOIN BDGT_TNG_FactsStats FS ON HDR.BidNumber=FS.BidNumber AND HDR.BudgetYear=FS.BudgetYear AND HDR.BudgetVersion=FS.BudgetVersion
                WHERE HDR.BidNumber=@BidNumber AND HDR.BudgetYear=@BudgetYear AND ACTIVE=1">
            <SelectParameters>
                <asp:Parameter Name="BidNumber" Type="String" />
                <asp:ControlParameter Name="BudgetYear" ControlID="HFBudgetYear" PropertyName="Value" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

    </div>
</asp:Content>
