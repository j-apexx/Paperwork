<%@ Page Title="Setup VPs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupVPs.aspx.cs" Inherits="Paperwork.SetupVPs" %>
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

    <div class="messagealert" id="alert_container"></div>

    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="GridViewVPs" runat="server" DataSourceID="SqlDataSourceVPs" CssClass="mGrid" Width="500px" AutoGenerateColumns="False"
                    DataKeyNames="GM" ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                    OnRowCommand="GridViewVPs_RowCommand" OnRowDataBound="GridViewVPs_RowDataBound" AllowSorting="true">
                    <Columns>
                        <asp:TemplateField HeaderText="VP" SortExpression="GM" HeaderStyle-Width="50">
                            <ItemTemplate>
                                <asp:Label ID="LabelGM" runat="server" Text='<%# Eval("GM") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEGM" runat="server" Text='<%# Bind("GM") %>' MaxLength="50"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFGM" runat="server" Text="" MaxLength="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="VP Email" SortExpression="GMEmail">
                            <ItemTemplate>
                                <asp:Label ID="LabelGMEmail" runat="server" Text='<%# Eval("GMEmail") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEGMEmail" runat="server" Text='<%# Bind("GMEmail") %>' MaxLength="200"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReqEGMEmail" runat="server" ControlToValidate="TextBoxEGMEmail" Display="Dynamic" ErrorMessage="VP Email is required." 
                                    ForeColor="Red" ValidationGroup="Insert" EnableClientScript="true">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFGMEmail" runat="server" MaxLength="200"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReqFGMEmail" runat="server" ControlToValidate="TextBoxFGMEmail" Display="Dynamic" ErrorMessage="VP Email is required." 
                                    ForeColor="Red" ValidationGroup="Insert" EnableClientScript="true">*</asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField  HeaderStyle-Width="175">
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
        <asp:SqlDataSource ID="SqlDataSourceVPs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_{0}" 
            DeleteCommand="DELETE FROM [GMSetup] WHERE [GM] = @old_GM" 
            InsertCommand="INSERT INTO [GMSetup] ([GM],[GMEmail]) VALUES (@GM, @GMEmail)" 
            SelectCommand="SELECT [GM], [GMEmail] FROM [GMSetup]  UNION ALL SELECT '', ''  ORDER BY [GM]" 
            UpdateCommand="UPDATE [GMSetup] SET [GM]=@GM, [GMEmail]=@GMEmail WHERE [GM] = @old_GM"
            OnDeleting="SqlDataSourceVPs_Deleting" OnInserting="SqlDataSourceVPs_Inserting">
            <DeleteParameters>
                <asp:Parameter Name="GM" Type="String" ConvertEmptyStringToNull="false" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="GM" Type="String" />
                <asp:Parameter Name="GMEmail" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="GM" Type="String" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="GMEmail" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>