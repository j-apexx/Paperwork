<%@ Page Title="Setup Purchasing Other Categories" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupPurchOtherCats.aspx.cs" Inherits="Paperwork.SetupPurchOtherCats" %>
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
    The categories below will appear in the dropdown selection for "Other" amounts on the purchases page.<br />
    *Note that a district will only see a category if they have budgeted amounts for that category.<br />
    If districts are not seeing the categories they need, click this button to synch against the budgets. <br />
    <asp:LinkButton runat="server"  
        ID="ISync" ToolTip="Synchronize Other Categories" Text=" Synchronize"
        CssClass="fas fa-sync mGridLinkButton" ForeColor="#666699" Width="180" OnClick="ISync_Click"   />
        <div class="messagealert" id="alert_container"></div>
    <br />
    If the problem persists, double-check that they are budgeted to use the Other Category.
    <hr />
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                Program Year: <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                <asp:HiddenField ID="HFBudgetYear" runat="server" />
                <asp:GridView ID="GridViewOtherCats" runat="server" AutoGenerateColumns="False" DataKeyNames="OtherCategoryID,ProgramYear"
                    DataSourceID="SqlDataSourcePurchaseCats" CssClass="mGrid" Width="600" ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                    OnRowCommand="GridViewOtherCats_RowCommand" OnRowDataBound="GridViewOtherCats_RowDataBound" AllowSorting="true" >
                    <Columns>
                        <asp:TemplateField HeaderText="Category ID" SortExpression="OtherCategoryID">
                            <ItemTemplate>
                                <asp:Label ID="LabelCatID" runat="server" Text='<%# Eval("OtherCategoryID") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="LabelECatID" runat="server" Text='<%# Eval("OtherCategoryID") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFCatID" runat="server" MaxLength="5" Width="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReqFCatID" runat="server" ControlToValidate="TextBoxFCatID" Display="Dynamic" ErrorMessage="Category ID is required." 
                                    ForeColor="Red" ValidationGroup="Insert" EnableClientScript="true">*</asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Category Description" SortExpression="OtherCategoryDesc">
                            <ItemTemplate>
                                <asp:Label ID="LabelCategoryDescription" runat="server" Text='<%# Eval("OtherCategoryDesc") %>' ></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxECatDesc" runat="server" Text='<%# Bind("OtherCategoryDesc") %>' MaxLength="25" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReqECatDesc" runat="server" ControlToValidate="TextBoxECatDesc" Display="Dynamic" ErrorMessage="Category Description is required." 
                                    ForeColor="Red" ValidationGroup="Edit" EnableClientScript="true">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFCatDesc" runat="server" MaxLength="25" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReqFCatDesc" runat="server" ControlToValidate="TextBoxFCatDesc" Display="Dynamic" ErrorMessage="Category Description is required." 
                                    ForeColor="Red" ValidationGroup="Insert" EnableClientScript="true">*</asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Expense Code" SortExpression="OtherCatExpCode">
                            <ItemTemplate>
                                <asp:Label ID="LabelExpCode" runat="server" Text='<%# Eval("OtherCatExpCode") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEExpCode" runat="server" MaxLength="4" Width="50" Text='<%# Bind("OtherCatExpCode") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFExpCode" runat="server" MaxLength="4" Width="50" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <div style="min-width:165px;text-align:center;" >
                                    <asp:LinkButton runat="server" CommandName="Edit"
                                        ID="IEdit" ToolTip="Edit" Text=" Edit"
                                        CssClass="fas fa-pencil-alt mGridLinkButton" ForeColor="#336699"    />
                                    <asp:LinkButton runat="server" CommandName="Delete"
                                        ID="IDelete" ToolTip="Delete" Text=" Delete"
                                        CssClass="fa fa-trash-alt mGridLinkButton" ForeColor="Crimson"
                                        OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                                </div>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <div style="min-width:165px;text-align:center;">
                                    <asp:LinkButton runat="server" CommandName="Update"
                                        ID="ISave" ToolTip="Save" Text=" Save" TabIndex="21"
                                        CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                    <asp:LinkButton runat="server" CommandName="Cancel"
                                        ID="ICancel" ToolTip="Cancel" Text=" Cancel" TabIndex="21"
                                        CssClass="fas fa-undo-alt mGridLinkButton" ForeColor="Red" CausesValidation="false" />
                                </div>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <div style="min-width:165px;text-align:center;" >
                                    <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" Text="Add"
                                        CssClass="fas fa-plus mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                    <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" Text="Cancel"
                                        CssClass="fas fa-undo-alt mGridLinkButton" ForeColor="Red" CausesValidation="false" />
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel> 
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [ProgramYear] FROM [ProgramYear] ORDER BY [StartDate] DESC" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourcePurchaseCats" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [OtherPurchaseCategories] WHERE [OtherCategoryID] = @OtherCategoryID AND [ProgramYear] = @ProgramYear" 
            InsertCommand="INSERT INTO [OtherPurchaseCategories] ([OtherCategoryID], [OtherCategoryDesc], [ProgramYear], [OtherCatExpCode]) VALUES (@OtherCategoryID, @OtherCategoryDesc, @ProgramYear, @OtherCatExpCode)" 
            SelectCommand="SELECT OtherCategoryID, OtherCategoryDesc, ProgramYear, OtherCatExpCode FROM [OtherPurchaseCategories] WHERE ([ProgramYear] = @ProgramYear) 
            UNION ALL
            SELECT '' OtherCategoryID, '' OtherCategoryDesc, @ProgramYear , '' OtherCatExpCode
            ORDER BY [OtherCategoryDesc]" 
            UpdateCommand="UPDATE [OtherPurchaseCategories] SET [OtherCategoryDesc] = @OtherCategoryDesc, [OtherCatExpCode]=@OtherCatExpCode WHERE [OtherCategoryID] = @OtherCategoryID AND [ProgramYear] = @ProgramYear"
            OnDeleting="SqlDataSourcePurchaseCats_Deleting" OnInserting="SqlDataSourcePurchaseCats_Inserting">
            <DeleteParameters>
                <asp:Parameter Name="OtherCategoryID" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" Type="String" PropertyName="SelectedValue" Name="ProgramYear" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="OtherCategoryID" Type="String" />
                <asp:Parameter Name="OtherCategoryDesc" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" Type="String" PropertyName="SelectedValue" Name="ProgramYear" />
                <asp:Parameter Name="OtherCatExpCode" Type="String" />
            </InsertParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Type="String" PropertyName="SelectedValue" Name="ProgramYear" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="OtherCategoryDesc" Type="String" />
                <asp:Parameter Name="OtherCategoryID" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" Type="String" PropertyName="SelectedValue" Name="ProgramYear" />
                <asp:Parameter Name="OtherCatExpCode" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>

