<%@ Page Title="Enter Indirect Costs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OtherSales.aspx.cs" Inherits="Paperwork.OtherSales" %>
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
            $(function () {
                $("[id*=TextBoxEDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: new Date($('[id*=HFStartDate]').val()),
                    maxDate: new Date($('[id*=HFEndDate]').val())
                });
            });

            $(function () {
                $("[id*=TextBoxFDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: new Date($('[id*=HFStartDate]').val()),
                    maxDate: new Date($('[id*=HFEndDate]').val())
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

            $("[id*=TextBoxEAmount],[id*=TextBoxFAmount]").change(function () {
                if (jQuery.trim($(this).val()) == '' ||  isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
            });
        }

        //apply highlight 
        $('body').on('focus', 'input,textarea,select', function (e) {
            $(this).addClass("highlight");
        });
        // remove highlight
        $('body').on('focusout', 'input,textarea,select', function (e) {
            $(this).removeClass("highlight");
        });
    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="topSection">
                    <div class="row">
                        <div id="hdrDiv" style="float:left;width:35%">
                            <h2><%: Title %></h2>
                        </div>
                        <div id="menuDiv" style="padding-left:20px;padding-top:10px;">
                            <strong>Program Year:</strong>
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"  ></asp:DropDownList>
                            <asp:HiddenField ID="HFStartDate" runat="server" />
                            <asp:HiddenField ID="HFEndDate" runat="server"  />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8">
                            Use this page to enter additional items that will appear as a cost on the district statement. <br />
                            Your RM will give these to you and you can set them up by clicking the "Setup Indirect Costs" button on the main page. They will then appear as a "pick list" choice on this page. Once a month enter the cost for each of your categories by entering it's amount as a new row in the grid. An example is Vending Labor Credit.<br />
                            <div style="text-decoration:underline;color:darkred;">Do not add special functions here.</div>
                        </div>
                    </div>
                </div>
                <div id="Records">
                    <asp:GridView ID="GridViewOtherSales" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,OtherSalesDate,OtherSalesID" DataSourceID="SqlDataSourceOtherSalesEntry"
                        ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" CssClass="mGrid" Width="600" 
                        OnRowCommand="GridViewOtherSales_RowCommand" OnRowDataBound="GridViewOtherSales_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Date" SortExpression="OtherSalesDate">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEDate" runat="server" Text='<%# Bind("OtherSalesDate" , "{0:d}") %>'></asp:TextBox>
                                    <%--<asp:Label ID="Label1" runat="server" Text='<%# Eval("OtherSalesDate") %>'></asp:Label>--%>
                                    <asp:RequiredFieldValidator ID="ReqEDate" runat="server" CssClass="validators" ControlToValidate="TextBoxEDate" Display="Dynamic" 
                                     ErrorMessage="Date is required." ValidationGroup="Edit" >*</asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("OtherSalesDate" , "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFDate" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ReqFDate" runat="server" CssClass="validators" ControlToValidate="TextBoxFDate" Display="Dynamic" 
                                     ErrorMessage="Date is required." ValidationGroup="Insert" >*</asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name" SortExpression="OtherSalesID">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DDLESalesCats" runat="server" DataSourceID="SqlDataSourceOSalesCats" 
                                        DataTextField="OtherSalesDesc" DataValueField="OtherSalesID" AppendDataBoundItems="true" SelectedValue='<%# Bind("OtherSalesID") %>'>
                                        <asp:ListItem Selected="True" Text="-- Select One --" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="ValidECat" runat="server" ControlToValidate="DDLESalesCats"  CssClass="validators" Display="Dynamic"
                                     ErrorMessage="Select an expense name." Operator="NotEqual" ValueToCompare="0" ValidationGroup="Edit" >*</asp:CompareValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:DropDownList ID="DDLSalesCats" runat="server" DataSourceID="SqlDataSourceOSalesCats" Enabled="false"
                                        DataTextField="OtherSalesDesc" DataValueField="OtherSalesID" AppendDataBoundItems="true" SelectedValue='<%# Eval("OtherSalesID") %>'>
                                        <asp:ListItem Selected="True" Text="-- Select One --" Value="0"></asp:ListItem>
                                    </asp:DropDownList>                                
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="DDLFSalesCats" runat="server" DataSourceID="SqlDataSourceOSalesCats" 
                                        DataTextField="OtherSalesDesc" DataValueField="OtherSalesID" AppendDataBoundItems="true">
                                        <asp:ListItem Selected="True" Text="-- Select One --" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="ValidFCat" runat="server" ControlToValidate="DDLFSalesCats"  CssClass="validators" Display="Dynamic"
                                     ErrorMessage="Select an expense name." Operator="NotEqual" ValueToCompare="0" ValidationGroup="Insert" >*</asp:CompareValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Amount" SortExpression="OtherSalesAmount">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEAmount" runat="server" Text='<%# Bind("OtherSalesAmount") %>'></asp:TextBox>
                                    <asp:CompareValidator ID="ValidAmount" runat="server" ControlToValidate="TextBoxEAmount" CssClass="validators" Display="Dynamic"
                                     ErrorMessage="Amount must be greater than 0." Operator="NotEqual" ValueToCompare="0" ValidationGroup="Edit" >*</asp:CompareValidator>
                                    <asp:RequiredFieldValidator ID="ReqEAmount" runat="server" CssClass="validators" ControlToValidate="TextBoxEAmount" Display="Dynamic" 
                                     ErrorMessage="Amount is required." ValidationGroup="Insert" >*</asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("OtherSalesAmount") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFAmount" runat="server"></asp:TextBox>
                                    <asp:CompareValidator ID="ValidAmount" runat="server" ControlToValidate="TextBoxFAmount" CssClass="validators" Display="Dynamic"
                                     ErrorMessage="Amount must be greater than 0." Operator="NotEqual" ValueToCompare="0" ValidationGroup="Insert" >*</asp:CompareValidator>
                                    <asp:RequiredFieldValidator ID="ReqFAmount" runat="server" CssClass="validators" ControlToValidate="TextBoxFAmount" Display="Dynamic" 
                                     ErrorMessage="Amount is required." ValidationGroup="Insert" >*</asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                        <asp:TemplateField>
                                <EditItemTemplate>
                                    <div style="width:60px;text-align:center;"  <%# (bool)Eval("Locked")?"hidden='hidden'":"" %>>
                                        <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="width:60px;text-align:center;"   <%# (bool)Eval("Locked")?"hidden='hidden'":"" %>>
                                        <asp:LinkButton runat="server" CommandName="Edit" ID="IEdit" ToolTip="Edit" 
                                            CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  />
                                        <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                            CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                            OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                    </div>
                                    <div style="text-align:center; font-size:xx-small;font-weight:bold;">
                                        <asp:Label runat="server" ID="LabelImported" Text="*LOCKED*" Visible='<%# (bool)Eval("Locked")==false ? false : true %>' ></asp:Label>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="width:60px;text-align:center;" >
                                        <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </FooterTemplate>
                        
                                <ItemStyle Width="60" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:ValidationSummary ID="ValidSummaryEdit" runat="server" CssClass="validators" DisplayMode="BulletList" HeaderText="Edit Errors" ShowSummary="true" ShowValidationErrors="true" ValidationGroup="Edit" />
                    <asp:ValidationSummary ID="ValidSummaryInsert" runat="server" CssClass="validators" DisplayMode="BulletList" HeaderText="New Record Errors" ShowSummary="true" ShowValidationErrors="true" ValidationGroup="Insert" />
                    <div class="messagealert" id="alert_container"></div>
                    <br />
                </div>
                <div id="DataSources">
                    <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                        SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceOSalesCats" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                        SelectCommand="SELECT [OtherSalesID], [OtherSalesDesc] FROM [OtherSalesCats] WHERE (([DistrictID] = @DistrictID) AND ([ProgramYear] = @ProgramYear))">
                        <SelectParameters>
                            <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                            <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceOtherSalesEntry" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_{0}" 
                        DeleteCommand="DELETE FROM [OtherSales] WHERE [ProgramYear] = @old_ProgramYear AND [DistrictID] = @old_DistrictID AND [OtherSalesDate] = @old_OtherSalesDate AND [OtherSalesID] = @old_OtherSalesID" 
                        InsertCommand="INSERT INTO [OtherSales] ([ProgramYear], [DistrictID], [OtherSalesDate], [OtherSalesID], [OtherSalesAmount], [Locked]) VALUES (@ProgramYear, @DistrictID, @OtherSalesDate, @OtherSalesID, @OtherSalesAmount, @Locked)" 
                        SelectCommand="SELECT [ProgramYear], [DistrictID], [OtherSalesDate], [OtherSalesID], [OtherSalesAmount], [Locked] 
                        FROM [OtherSales] WHERE (([DistrictID] = @DistrictID) AND ([ProgramYear] = @ProgramYear))
                        UNION ALL
                        SELECT ''[ProgramYear], ''[DistrictID], '1/1/1900'[OtherSalesDate], 0 [OtherSalesID], 0 [OtherSalesAmount], Cast(0 as bit) [Locked] " 
                        UpdateCommand="UPDATE [OtherSales] SET [OtherSalesAmount]=@OtherSalesAmount, [OtherSalesID]=@OtherSalesID, [OtherSalesDate]=@OtherSalesDate WHERE [ProgramYear]=@old_ProgramYear AND [DistrictID]=@old_DistrictID AND [OtherSalesDate]=@old_OtherSalesDate AND [OtherSalesID]=@old_OtherSalesID"
                        OnInserted="SqlDataSourceOtherSalesEntry_Inserted" OnUpdated="SqlDataSourceOtherSalesEntry_Updated" OnDeleted="SqlDataSourceOtherSalesEntry_Deleted"
                        OnInserting="SqlDataSourceOtherSalesEntry_Inserting" OnUpdating="SqlDataSourceOtherSalesEntry_Updating" OnDeleting="SqlDataSourceOtherSalesEntry_Deleting">
                        <DeleteParameters>
                            <asp:Parameter Name="ProgramYear" Type="String" />
                            <asp:Parameter Name="DistrictID" Type="String" />
                            <asp:Parameter Name="OtherSalesDate" Type="DateTime" />
                            <asp:Parameter Name="OtherSalesID" Type="Int16" />
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                            <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                            <asp:Parameter Name="OtherSalesDate" Type="DateTime" />
                            <asp:Parameter Name="OtherSalesID" Type="Int16" />
                            <asp:Parameter Name="OtherSalesAmount" Type="Decimal" />
                            <asp:Parameter Name="Locked" Type="Boolean" DefaultValue="false" />
                        </InsertParameters>
                        <SelectParameters>
                            <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                            <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="OtherSalesAmount" Type="Decimal" />
                            <asp:Parameter Name="OtherSalesDate" Type="DateTime"  />
                            <asp:Parameter Name="OtherSalesID" Type="Int16" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
