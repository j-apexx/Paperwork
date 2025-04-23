<%@ Page Title="Setup Regional Managers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupRegionalMgrs.aspx.cs" Inherits="Paperwork.SetupRegionalMgrs" %>
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
                Search:
                <asp:TextBox ID="TextBoxSearch" runat="server" MaxLength="50" Width="200px" AutoPostBack="true"></asp:TextBox>
                <asp:GridView ID="GridViewRMs" runat="server" DataSourceID="SqlDataSourceRMs" CssClass="mGrid" Width="600px" AutoGenerateColumns="False"
                    DataKeyNames="RMNumber" ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" AllowSorting="true"
                    OnRowCommand="GridViewRMs_RowCommand" OnRowDataBound="GridViewRMs_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="ID" SortExpression="RMNumber" HeaderStyle-Width="50">
                            <ItemTemplate>
                                <asp:Label ID="LabelRMNumber" runat="server" Text='<%# Eval("RMNumber") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="LabelERMNumber" runat="server" Text='<%# Bind("RMNumber") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="LabelFRMNumber" runat="server" Text="**"></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="RM Name" SortExpression="RMName">
                            <ItemTemplate>
                                <asp:Label ID="LabelRMName" runat="server" Text='<%# Eval("RMName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxERMName" runat="server" Text='<%# Bind("RMName") %>' MaxLength="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReqERMName" runat="server" ControlToValidate="TextBoxERMName" Display="Dynamic" ErrorMessage="RM Name is required." 
                                    ForeColor="Red" ValidationGroup="Insert" EnableClientScript="true">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFRMName" runat="server" MaxLength="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReqFRMName" runat="server" ControlToValidate="TextBoxFRMName" Display="Dynamic" ErrorMessage="RM Name is required." 
                                    ForeColor="Red" ValidationGroup="Insert" EnableClientScript="true">*</asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Active" SortExpression="Active" HeaderStyle-Width="65">
                            <ItemTemplate>
                                <div style="text-align:center;">
                                <asp:CheckBox ID="CBActive" runat="server" Checked='<%# (bool)Eval("Active") %>' Enabled="false" /></div>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <div style="text-align:center;">
                                <asp:CheckBox ID="CBEActive" runat="server" Checked='<%# Bind("Active") %>' Enabled="true" /></div>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <div style="text-align:center;">
                                <asp:CheckBox ID="CBFActive" runat="server" Checked="true" Enabled="true" /></div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="RM Email" SortExpression="RMEmail">
                            <ItemTemplate>
                                <asp:Label ID="LabelRMEmail" runat="server" Text='<%# Eval("RMEmail") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxERMEmail" runat="server" Text='<%# Bind("RMEmail") %>' MaxLength="50" Width="300"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFRMEmail" runat="server" MaxLength="50" Width="300"></asp:TextBox>
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
        <asp:SqlDataSource ID="SqlDataSourceRMs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
            DeleteCommand="DELETE FROM [RMSetup] WHERE [RMNumber] = @RMNumber"
            InsertCommand="INSERT INTO [RMSetup] ([RMName], [Active],[RMNumber],[RMEmail]) VALUES (@RMName, @Active,(SELECT MAX([RMNumber])+1 FROM [RMSetup]), @RMEmail)"
            SelectCommand="SELECT [RMName], [Active], [RMNumber], [RMEmail] FROM [RMSetup]  UNION ALL SELECT '',CAST(1 AS Bit), 0, ''  ORDER BY [RMName]"
            UpdateCommand="UPDATE [RMSetup] SET [RMName]=@RMName, [Active]=@Active, [RMEmail]=@RMEmail WHERE [RMNumber] = @RMNumber"
            FilterExpression="RMName LIKE '%{0}%' OR RMEmail LIKE '%{0}%'"
            OnDeleting="SqlDataSourceRMs_Deleting" OnInserting="SqlDataSourceRMs_Inserting">
            <DeleteParameters>
                <asp:Parameter Name="RMNumber" Type="Int16" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="RMName" Type="String" />
                <asp:Parameter Name="Active" Type="Boolean" />
                <asp:Parameter Name="RMEmail" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="RMName" Type="String" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="Active" Type="Boolean" />
                <asp:Parameter Name="RMNumber" Type="Int16" />
                <asp:Parameter Name="RMEmail" Type="String" />
            </UpdateParameters>
            <FilterParameters>
                <asp:ControlParameter ControlID="TextBoxSearch" Name="RMName" PropertyName="Text" Type="String" DefaultValue="" />
            </FilterParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>