<%@ Page Title="Setup MMB Items" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupMMBItems.aspx.cs" Inherits="Paperwork.SetupMMBItems" %>
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
                            </asp:DropDownList> &nbsp;
                        </p>
                    </div>
                </div>
                <div id="SetupSection">
                    <asp:GridView ID="GridViewMMBItems" runat="server" AutoGenerateColumns="False" DataKeyNames="ProductCode,DistrictID,ProgramYear" 
                        DataSourceID="SqlDataSourceMMBItems" CssClass="mGrid" Width="650"
                        ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" AllowSorting="true"
                        OnRowCommand="GridViewMMBItems_RowCommand" OnRowDataBound="GridViewMMBItems_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Product Code" SortExpression="ProductCode">
                                <EditItemTemplate>
                                    <asp:Label ID="LabelEProdCode" runat="server" Text='<%# Eval("ProductCode") %>'></asp:Label>                                
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelProdCode" runat="server" Text='<%# Eval("ProductCode") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFProdCode" runat="server" TextMode="Number" Width="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFProdCode" runat="server" ControlToValidate="TextBoxFProdCode" CssClass="validators"
                                     Display ="Dynamic" ErrorMessage="Product Code is required" Text="*" ValidationGroup="Insert"></asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Product Name" SortExpression="ProductName">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEProdName" runat="server" Text='<%# Bind("ProductName") %>' MaxLength="25"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredProdName" runat="server" ControlToValidate="TextBoxEProdName" CssClass="validators"
                                     Display ="Dynamic" ErrorMessage="Product Name is required" Text="*" ValidationGroup="Edit"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelProdName" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFProdName" runat="server" MaxLength="25"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFProdName" runat="server" ControlToValidate="TextBoxFProdName" CssClass="validators"
                                     Display ="Dynamic" ErrorMessage="Product Name is required" Text="*" ValidationGroup="Insert"></asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Price" SortExpression="Price">
                                <EditItemTemplate>
                                    $<asp:TextBox ID="TextBoxEPrice" runat="server" Text='<%# Bind("Price") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelPrice" runat="server" Text='<%# Eval("Price", "{0:C4}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    $<asp:TextBox ID="TextBoxFPrice" runat="server" Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Milk Item" SortExpression="MilkItem">
                                <EditItemTemplate>
                                    <div style="text-align:center;">
                                    <asp:CheckBox ID="CBEMilkItem" runat="server" Checked='<%# Bind("MilkItem") %>' /></div>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="text-align:center;">
                                    <asp:CheckBox ID="CBMilkItem" runat="server" Checked='<%# Eval("MilkItem") %>' Enabled="false"  /></div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="text-align:center;">
                                    <asp:CheckBox ID="CBFMilkItem" runat="server" /></div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <EditItemTemplate>
                                    <div style="width:60px;text-align:center;">
                                    <asp:LinkButton runat="server" CommandName="Update" ID="ISave" ToolTip="Save" 
                                        CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                    <asp:LinkButton runat="server" CommandName="Cancel" ID="ICancel" ToolTip="Cancel" 
                                        CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="width:60px;text-align:center;">
                                    <asp:LinkButton runat="server" CommandName="Edit" ID="IEdit" ToolTip="Edit" 
                                        CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  />
                                    <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                        CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                        OnClientClick="return confirm('Are you sure you want to delete this record?');" />
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
                                <ItemStyle Width="50" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:ValidationSummary ID="SummaryEdit" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                        ValidationGroup="Edit" HeaderText="Edit Errors" BackColor="WhiteSmoke" Width="500" />
                    <asp:ValidationSummary ID="SummaryInsert" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                        ValidationGroup="Insert" HeaderText="Insert Errors" BackColor="WhiteSmoke" Width="500" />
                    <div class="messagealert" id="alert_container"></div>
                </div>
                <div id="DataSources">
                    <asp:SqlDataSource ID="SqlDataSourceMyDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                        SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc'
                        FROM [District] WHERE DistrictID IN (SELECT DistrictID FROM [UserDistrictAccess] WHERE Userid=@UserID) OR (@AccessLevel<>'User')
                        ORDER BY [DistrictName]">
                        <SelectParameters>
                            <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                            <asp:SessionParameter Name="AccessLevel" SessionField="AccessLevel" Type="String" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceMMBItems" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                        OldValuesParameterFormatString="old_{0}" OnInserted="SqlDataSourceMMBItems_Inserted" OnInserting="SqlDataSourceMMBItems_Inserting"
                        OnDeleting="SqlDataSourceMMBItems_Deleting" OnDeleted="SqlDataSourceMMBItems_Deleted"
                        SelectCommand="SELECT [MilkItem], [Price], [ProductName], [ProductCode], [DistrictID], [ProgramYear] FROM [MMBItems] 
                        WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID)) UNION ALL
                        SELECT CAST(0 AS bit) 'MilkItem', 0 [Price], '' [ProductName], 0 [ProductCode], '' [DistrictID], @ProgramYear [ProgramYear]
                        ORDER BY [ProductName]" 
                        DeleteCommand="DELETE FROM [MMBItems] WHERE [ProductCode] = @old_ProductCode AND [DistrictID] = @old_DistrictID AND [ProgramYear] = @old_ProgramYear" 
                        InsertCommand="INSERT INTO [MMBItems] ([MilkItem], [Price], [ProductName], [ProductCode], [DistrictID], [ProgramYear])
                        VALUES (@MilkItem, @Price, dbo.InitCap(@ProductName), @ProductCode, @DistrictID, @ProgramYear)" 
                        UpdateCommand="UPDATE [MMBItems] SET [MilkItem] = @MilkItem, [Price] = @Price, [ProductName] = dbo.InitCap(@ProductName)
                        WHERE [ProductCode] = @old_ProductCode AND [DistrictID] = @old_DistrictID AND [ProgramYear] = @old_ProgramYear" >
                        <DeleteParameters>
                            <asp:Parameter Name="ProductCode" Type="Int16" />
                            <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                            <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:Parameter Name="MilkItem" Type="Boolean" />
                            <asp:Parameter Name="Price" Type="Decimal" />
                            <asp:Parameter Name="ProductName" Type="String" />
                            <asp:Parameter Name="ProductCode" Type="Int16" />
                            <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                            <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        </InsertParameters>
                        <SelectParameters>
                            <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                            <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="MilkItem" Type="Boolean" />
                            <asp:Parameter Name="Price" Type="Decimal" />
                            <asp:Parameter Name="ProductName" Type="String" />
                            <asp:Parameter Name="ProductCode" Type="Int16" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
