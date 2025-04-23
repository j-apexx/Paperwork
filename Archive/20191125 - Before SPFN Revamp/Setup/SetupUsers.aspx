<%@ Page Title="Setup Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupUsers.aspx.cs" Inherits="Paperwork.SetupUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <p>Setup Paperwork Web users.</p>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5">
        <ProgressTemplate>
            <div class="loadingDiv" >
                <asp:Image CssClass="loadingImage" ID="ImageLoading" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <p>
                <asp:DetailsView ID="DetailsViewUserEntry" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSourceUsers" Height="50px" Width="300px" 
                    DefaultMode="Insert" FieldHeaderStyle-Font-Bold="true" OnItemInserted="DetailsViewUserEntry_ItemInserted" CssClass="mGrid" >
                    <HeaderStyle Font-Bold="true" />
                    <FieldHeaderStyle Font-Bold="True"></FieldHeaderStyle>
                    <Fields>
                        <asp:TemplateField HeaderText="Windows User ID">
                            <InsertItemTemplate>
                                <asp:Label ID="labelInstructions" runat="server">Enter as Domain\User</asp:Label><br />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorUserID" runat="server" Display="Dynamic" ErrorMessage="User ID is required." ForeColor="#FF3300" ControlToValidate="textboxUserID" ValidationGroup="UserInsert">*</asp:RequiredFieldValidator>
                                <asp:TextBox ID="textboxUserID" runat="server" Text='<%# Bind("UserID") %>' CausesValidation="True" ValidationGroup="UserInsert" Width="150px" Wrap="False"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Access Level">
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlAccessLevel" runat="server" SelectedValue='<%# Bind("AccessLevel") %>'>
                                    <asp:ListItem Value="0">User</asp:ListItem>
                                    <asp:ListItem Value="10">Office</asp:ListItem>
                                    <asp:ListItem Value="99">Admin</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <InsertItemTemplate>
                                <asp:LinkButton runat="server" CommandName="Insert"
                                    ID="ISave" ToolTip="Add" Text=" Add"
                                    CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="UserInsert" Width="100%" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </p>
            <p>
                <asp:Label ID="LabelInsertErrors" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label>
            </p>
            <p>
                &nbsp;</p>
            <p>Modify or delete existing users:</p>
            <p>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="UserID" DataSourceID="SqlDataSourceUsers" 
                    OnRowDeleted="GridView1_RowDeleted" CssClass="mGrid" OnRowDeleting="GridView1_RowDeleting" AllowSorting="true">
        
                    <Columns>
                        <asp:BoundField DataField="UserID" HeaderText="Domain\UserID" ReadOnly="True" SortExpression="UserID" >
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Last Activity" SortExpression="LastActivity">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="LabelLastActivity" Text='<%# Eval("LastActivity") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="AccessLevel" SortExpression="AccessLevel">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlAccessLevel" runat="server" SelectedValue='<%# Bind("AccessLevel") %>'>
                                    <asp:ListItem Value="0">User</asp:ListItem>
                                    <asp:ListItem Value="10">Office</asp:ListItem>
                                    <asp:ListItem Value="99">Admin</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelAccessLevel" runat="server" Text='<%# Eval("AccessLevelString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Districts" >
                            <ItemTemplate>
                                <asp:Label ID="CurrUser" runat="server" Text='<%# Eval("UserID").ToString() %>' Visible="false" ></asp:Label>
                                <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataKeyNames="UserID,DistrictID" DataSourceID="SqlDataSourceDistrictAccess" ShowHeader="False" CssClass="mGrid"
                                     >
                                    <Columns>
                                        <asp:BoundField DataField="DistrictID" SortExpression="DistrictID" ShowHeader="False" Visible="false" />
                                        <asp:BoundField DataField="DistrictName" SortExpression="DistrictName" ShowHeader="False" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" CommandName="Delete"
                                                    ID="IDelete" ToolTip="Delete" CssClass="fas fa-trash " ForeColor="Crimson"
                                                    />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:SqlDataSource ID="SqlDataSourceDistrictAccess" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                                    SelectCommand="SELECT DA.[UserID], RTRIM(ISNULL(D.DistrictName,'*Unknown*')) + '-' + RTRIM(DA.DistrictID) AS DistrictName, DA.DistrictID 
                                        FROM [UserDistrictAccess] DA 
                                        LEFT OUTER JOIN [District] D ON DA.[DistrictID]=D.[DistrictID]
                                        WHERE ([UserID] = @UserID) ORDER BY D.DistrictName"  
                                    DeleteCommand="DELETE FROM [UserDistrictAccess] WHERE ([UserID] = @UserID AND [DistrictID]=@DistrictID)">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="CurrUser" Name="UserID" PropertyName="Text" Type="String" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:HiddenField ID="HFCurrentUser" runat="server" Value='<%# Eval("UserID") %>' />
                                <asp:GridView ID="GridViewDistrictAccess" runat="server" AutoGenerateColumns="False" DataKeyNames="UserID,DistrictID" DataSourceID="SqlDataSourceDistAccess" ShowHeader="False" CssClass="mGrid"  >
                                    <Columns>
                                        <asp:BoundField DataField="UserID" SortExpression="UserID" ShowHeader="False" Visible="false" />
                                        <asp:BoundField DataField="DistrictID" SortExpression="DistrictID" ShowHeader="False" Visible="false" />
                                        <asp:BoundField DataField="DistrictName" SortExpression="DistrictName" ShowHeader="False" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" CommandName="Delete"
                                                    ID="IDelete" ToolTip="Delete" CssClass="fas fa-trash " ForeColor="Crimson"
                                                    />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>

                                <asp:DetailsView ID="DetailsViewAvailDists" runat="server" DataSourceID="SqlDataSourceDistAccess" Height="50px" Width="150px" DefaultMode="Insert" AutoGenerateRows="False" 
                                    OnItemInserted="DetailsView1_ItemInserted" CssClass="mGrid">
                                    <Fields>
                                        <asp:TemplateField ShowHeader="False">
                                            <InsertItemTemplate>
                                                <asp:ListBox ID="lbDistricts" runat="server" AppendDataBoundItems="True" DataSourceID="SqlDataSourceAvailDists" DataTextField="DistrictName" 
                                                    DataValueField="DistrictID" SelectedValue='<%# Bind("DistrictID") %>' ValidationGroup="NewDistrict" SelectionMode="Multiple" Height="150">
                                                </asp:ListBox>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ShowHeader="false">
                                            <InsertItemTemplate>
                                                <asp:LinkButton runat="server" CommandName="Insert" Text=" Add" Width="100%"
                                                    ID="IInsert" ToolTip="Add" CssClass="fas fa-plus mGridLinkButton" ForeColor="Green"  />
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                    </Fields>
                                </asp:DetailsView>
                                <asp:Label ID="LabelErrors" runat="server" ForeColor="Red"></asp:Label>
                                

                                <asp:SqlDataSource ID="SqlDataSourceDistAccess" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                                    SelectCommand="SELECT DA.[UserID], RTRIM(ISNULL(D.DistrictName,'*Unknown*')) + '-' + RTRIM(DA.DistrictID) AS DistrictName, DA.DistrictID 
                                                    FROM [UserDistrictAccess] DA 
                                                    LEFT OUTER JOIN [District] D ON DA.[DistrictID]=D.[DistrictID]
                                                    WHERE ([UserID] = @UserID) Order BY D.DistrictName" 
                                    InsertCommand="INSERT INTO [UserDistrictAccess] ([UserID], [DistrictID]) VALUES (UPPER(@UserID), @DistrictID)" 
                                    DeleteCommand="DELETE FROM [UserDistrictAccess] WHERE ([UserID] = @UserID AND [DistrictID]=@DistrictID)" OnInserting="SqlDataSource2_Inserting">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="HFCurrentUser" Name="UserID" PropertyName="Value" Type="String" />
                                    </SelectParameters>
                                    <InsertParameters>
                                        <asp:ControlParameter ControlID="HFCurrentUser" Name="UserID" PropertyName="Value" Type="String" />
                                        <asp:Parameter Name="DistrictID" Type="String" />
                                    </InsertParameters>
                                    <DeleteParameters>
                                        <asp:ControlParameter ControlID="HFCurrentUser" Name="UserID" PropertyName="Value" Type="String" />
                                        <asp:Parameter Name="DistrictID" Type="String" />
                                    </DeleteParameters>
                                </asp:SqlDataSource>
                                <asp:SqlDataSource ID="SqlDataSourceAvailDists" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                                    SelectCommand="SELECT [DistrictID], RTRIM(DistrictName) + '-' + RTRIM(DistrictID) AS [DistrictName] 
                                                    FROM [District] 
                                                    WHERE [DistrictID] NOT IN (SELECT [DistrictID] FROM [UserDistrictAccess] WHERE [UserID] = @UserID) 
                                                    ORDER BY DistrictName " 
                                    DataSourceMode="DataReader">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="HFCurrentUser" Name="UserID" PropertyName="Value" Type="String" />
                                    </SelectParameters>
                                </asp:SqlDataSource>

                            </EditItemTemplate>





                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <div style="min-width:165px;">
                                    <asp:LinkButton runat="server" CommandName="Edit"
                                        ID="IEdit" ToolTip="Edit" Text=" Edit"
                                        CssClass="fas fa-pencil-alt mGridLinkButton" ForeColor="#336699"    />
                                    <asp:LinkButton runat="server" CommandName="Delete"
                                        ID="IDelete" ToolTip="Delete" Text=" Delete"
                                        CssClass="fas fa-trash mGridLinkButton" ForeColor="Crimson"
                                        OnClientClick="return confirm('Are you sure you want to delete this record?');"/>

                                </div>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <div style="min-width:165px;">
                                    <asp:LinkButton runat="server" CommandName="Update"
                                        ID="ISave" ToolTip="Save" Text=" Save" TabIndex="21"
                                        CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="GridView" />
                                    <asp:LinkButton runat="server" CommandName="Cancel"
                                        ID="ICancel" ToolTip="Cancel" Text=" Cancel" TabIndex="21"
                                        CssClass="fas fa-undo glyphButton mGridLinkButton" ForeColor="Red" CausesValidation="false" />

                                </div>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSourceUsers" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    DeleteCommand="DELETE FROM [UserAccess] WHERE [UserID] = @UserID; DELETE FROM [UserDistrictAccess] WHERE [UserID] = @UserID" 
                    InsertCommand="INSERT INTO [UserAccess] ([UserID], [AccessLevel]) VALUES (UPPER(@UserID), @AccessLevel)" 
                    SelectCommand="SELECT UPPER([UserID]) AS UserID, [AccessLevel], [LastActivity], CASE [AccessLevel] WHEN 99 THEN 'ADMIN' WHEN 10 THEN 'OFFICE' ELSE 'USER' END AS AccessLevelString FROM [UserAccess]" 
                    UpdateCommand="UPDATE [UserAccess] SET [AccessLevel] = @AccessLevel WHERE [UserID] = @UserID">
                    <DeleteParameters>
                        <asp:Parameter Name="UserID" Type="String" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:Parameter Name="UserID" Type="String" />
                        <asp:Parameter Name="AccessLevel" Type="Int32" />
                    </InsertParameters>
                    <SelectParameters>
                        <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="AccessLevel" Type="Int32" />
                        <asp:Parameter Name="UserID" Type="String" />
                    </UpdateParameters>
                </asp:SqlDataSource>
            </p>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
