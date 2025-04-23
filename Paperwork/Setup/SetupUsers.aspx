<%@ Page Title="Setup Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupUsers.aspx.cs" Inherits="Paperwork.SetupUsers" EnableEventValidation="false" MaintainScrollPositionOnPostBack="true" %>
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

        }

        function closeModal(modalid) {
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#' + modalid).modal('hide');
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
    <h2><%: Title %></h2>
    <h5>Setup Paperwork Web users.</h5>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5">
        <ProgressTemplate>
            <div class="loadingDiv" >
                <asp:Image CssClass="loadingImage" ID="ImageLoading" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanelModalCustomSecurity" DisplayAfter="5">
        <ProgressTemplate>
            <div class="loadingDiv" >
                <asp:Image CssClass="loadingImage" ID="ImageLoading2" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>






            <div class="row">
                <div class="col-sm-12">
                    <asp:DetailsView ID="DetailsViewUserEntry" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSourceUsers" Height="50px" Width="300px" 
                        DefaultMode="Insert" FieldHeaderStyle-Font-Bold="true" OnItemInserted="DetailsViewUserEntry_ItemInserted" OnItemInserting="DetailsViewUserEntry_ItemInserting" CssClass="mGrid" >
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
                                        <asp:ListItem Value="5">RM</asp:ListItem>
                                        <asp:ListItem Value="10">Office</asp:ListItem>
                                        <asp:ListItem Value="99">Admin</asp:ListItem>
                                    </asp:DropDownList>
                                </InsertItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="USDA Web App">
                                <InsertItemTemplate>
                                    <asp:CheckBox ID="cbUSDA" runat="server" Checked="false" Text="Also add user to USDA" />
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

                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    &nbsp;
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <asp:Label ID="LabelInsertErrors" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    Modify or delete existing users:
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    Search: <asp:TextBox ID="TextBoxSearch" runat="server" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                    <asp:LinkButton ID="ISearch" runat="server" CausesValidation="False" 
                        ToolTip="Search" Text="Search"
                        CssClass="fas fa-search" OnClick="ISearch_Click"      />
                    &nbsp;
                    <asp:LinkButton ID="IClear" runat="server" CausesValidation="False" 
                        ToolTip="Clear Search" forecolor="#333333" Text="Clear"
                        CssClass="fas fa-undo-alt" OnClick="IClear_Click"  />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="UserID" DataSourceID="SqlDataSourceUsers" 
                        OnRowDeleted="GridView1_RowDeleted" CssClass="mGrid" OnRowDeleting="GridView1_RowDeleting" AllowSorting="true"
                        OnRowCommand="GridView1_RowCommand" OnRowDataBound="GridView1_RowDataBound">
        
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
                                        <asp:ListItem Value="5">RM</asp:ListItem>
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
                                    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataKeyNames="UserID,DistrictID" DataSourceID="SqlDataSourceDistrictAccess" OnRowDeleted="GridViewDistrictAccess_RowDeleted"
                                        ShowHeader="False" CssClass="mGrid" Width="250" >
                                        <Columns>
                                            <asp:BoundField DataField="DistrictID" SortExpression="DistrictID" ShowHeader="False" Visible="false" />
                                            <asp:BoundField DataField="DistrictName" SortExpression="DistrictName" ShowHeader="False" ItemStyle-Width="225"/>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete" 
                                                        CssClass="fas fa-trash " ForeColor="Crimson" Width="25" />
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
                                    <asp:GridView ID="GridViewDistrictAccess" runat="server" AutoGenerateColumns="False" DataKeyNames="UserID,DistrictID" 
                                        DataSourceID="SqlDataSourceDistAccess" ShowHeader="False" CssClass="mGrid" Width="250" OnRowDeleted="GridViewDistrictAccess_RowDeleted"  >
                                        <Columns>
                                            <asp:BoundField DataField="UserID" SortExpression="UserID" ShowHeader="False" Visible="false" />
                                            <asp:BoundField DataField="DistrictID" SortExpression="DistrictID" ShowHeader="False" Visible="false" />
                                            <asp:BoundField DataField="DistrictName" SortExpression="DistrictName" ShowHeader="False" />
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete" 
                                                        CssClass="fas fa-trash " ForeColor="Crimson" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>

                                    <asp:DetailsView ID="DetailsViewAvailDists" runat="server" DataSourceID="SqlDataSourceDistAccess" Height="50px" Width="150px" 
                                        DefaultMode="Insert" AutoGenerateRows="False" OnItemInserting="DetailsViewAvailDists_ItemInserting" OnItemInserted="DetailsView1_ItemInserted" CssClass="mGrid">
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
                            <asp:TemplateField HeaderText="USDA">
                                <ItemTemplate>
                                    <asp:CheckBox ID="CBInUSDA" runat="server" Enabled="false" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:CheckBox ID="CBInUSDAEdit" runat="server" OnCheckedChanged="CBInUSDA_CheckedChanged" />
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
                                        <asp:LinkButton runat="server" 
                                            ID="IAdvanced" ToolTip="Advanced Security" Text="Custom" 
                                            CssClass="fas fa-tools mGridLinkButton" ForeColor='<%# ((int)Eval("AdvSecurityRecords"))>0? System.Drawing.Color.Green:System.Drawing.Color.CornflowerBlue %>' 
                                            CommandArgument='<%# Eval("UserID") %>' OnClick="IAdvanced_Click" ></asp:LinkButton>
                                        <asp:HiddenField ID="HFCurrentUserAdv" runat="server" Value='<%# Eval("UserID") %>' />
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
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="modal fade" id="ModalCustomSecurity" >  
        <div class="modal-dialog" >  
            <div class="modal-content" style="width:1000px;" > 
                <div class="modal-header">  
                    <h4 class="modal-title">Modify Custom User Security</h4>  
                    <button type="button" class="close" data-dismiss="modal">×</button>  
                </div>  
                <div class="modal-body">  
                    <div class="row">  
                        <div class="col-md-12">  
                            <div class="form-group">  
                                <div class="input-group">  
                                    <asp:UpdatePanel ID="UpdatePanelModalCustomSecurity" UpdateMode="Conditional" runat="server">
                                        <ContentTemplate>
                                            <div style="width:800px;"  > 
                                                <asp:Label ID="LabelUserID" runat="server"></asp:Label>
                                                <asp:GridView ID="GridViewCustomSecurity" runat="server" DataSourceID="SqlDataSourceCustomSecurity" DataKeyNames="UserID,SecurityObject" ShowFooter="true" 
                                                    OnRowDataBound="GridViewCustomSecurity_RowDataBound" OnRowCommand="GridViewCustomSecurity_RowCommand" AutoGenerateColumns="false" >
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Security Object">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LabelSecurityObject" runat="server" Text='<%# Eval("SecurityObject") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <asp:TextBox ID="TextBoxFSecurityObject" runat="Server"></asp:TextBox>
                                                            </FooterTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Access">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="CBAccessGranted" runat="server" Text="Granted" OnCheckedChanged="CBAccess_CheckedChanged" AutoPostBack="true" Enabled="true" Checked='<%# Eval("AccessAllowed") %>' />
                                                                <asp:CheckBox ID="CBAccessDenied" runat="server" Text="Denied"  OnCheckedChanged="CBAccess_CheckedChanged" AutoPostBack="true" Enabled="true" Checked='<%# Eval("AccessDenied") %>'/>
                                                            </ItemTemplate>
                                                            <EditItemTemplate>
                                                                <asp:CheckBox ID="CBEAccessGranted" runat="server" Text="Granted" OnCheckedChanged="CBAccess_CheckedChanged" AutoPostBack="true" Enabled="true" Checked='<%# Eval("AccessAllowed") %>' />
                                                                <asp:CheckBox ID="CBEAccessDenied" runat="server" Text="Denied"  OnCheckedChanged="CBAccess_CheckedChanged" AutoPostBack="true" Enabled="true" Checked='<%# Eval("AccessDenied") %>'/>
                                                            </EditItemTemplate>
                                                            <FooterTemplate>
                                                                <asp:CheckBox ID="CBFAccessGranted" runat="server" Text="Granted" OnCheckedChanged="CBAccess_CheckedChanged" AutoPostBack="true" Enabled="true"  />
                                                                <asp:CheckBox ID="CBFAccessDenied" runat="server" Text="Denied"  OnCheckedChanged="CBAccess_CheckedChanged" AutoPostBack="true" Enabled="true" />
                                                            </FooterTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderStyle-Width="80">
                                                            <ItemTemplate>
                                                                <div style="min-width:80px;" >
                                                                    <asp:LinkButton runat="server" CommandName="Delete"
                                                                        ID="IDelete" ToolTip="Delete" Text=" Delete"
                                                                        CssClass="fa fa-trash-alt mGridLinkButton" ForeColor="Crimson"/>
                                                                </div>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <div style="min-width:80px;" >
                                                                    <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" Text="Add"
                                                                        CssClass="fas fa-plus mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                                                </div>
                                                            </FooterTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                *Security Objects are the names of the pages without any ".aspx"
                                                <br />
                                                Use Reports/*, Transactions/*, Utilities/*, Setup/* to grant or deny access to a whole menu
                                            </div>  
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>  
                            </div>  
                        </div>  
                    </div>  
                </div>  
                <div class="modal-footer">  
                    <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Close</button>  
                </div>  





            </div>  
        </div>  
    </div> 





    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceUsers" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [UserAccess] WHERE [UserID] = @UserID; DELETE FROM [UserDistrictAccess] WHERE [UserID] = @UserID" 
            InsertCommand="INSERT INTO [UserAccess] ([UserID], [AccessLevel]) VALUES (UPPER(@UserID), @AccessLevel)" 
            SelectCommand="SELECT UPPER([UserID]) AS UserID, [AccessLevel], [LastActivity], CASE [AccessLevel] WHEN 99 THEN 'ADMIN' WHEN 10 THEN 'OFFICE' WHEN 5 THEN 'RM' ELSE 'USER' END AS AccessLevelString 
            ,(Select Count(*)  FROM UserAccessCustomSecurity WHERE UserID=[UserAccess].UserID) AdvSecurityRecords
            FROM [UserAccess]" 
            UpdateCommand="UPDATE [UserAccess] SET [AccessLevel] = @AccessLevel WHERE [UserID] = @UserID"
            FilterExpression="UserID LIKE '%{0}%' " OnFiltering="SqlDataSourceUsers_Filtering">
            <FilterParameters>
                <asp:ControlParameter ControlID="TextBoxSearch" Name="SearchString" PropertyName="Text" Type="String" DefaultValue="" />
            </FilterParameters>
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
        <asp:SqlDataSource ID="SqlDataSourceCustomSecurity" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE [UserAccessCustomSecurity] WHERE [UserID]=@UserID AND [SecurityObject]=@SecurityObject"
            InsertCommand="INSERT INTO [UserAccessCustomSecurity] ([UserID],[SecurityObject],[AccessAllowed],[AccessDenied]) VALUES(@UserID,@SecurityObject,@AccessAllowed,@AccessDenied)"
            SelectCommand="SELECT @UserID UserID,'' SecurityObject ,CAST(0 AS BIT) AccessAllowed,CAST(0 AS BIT) AccessDenied
            UNION ALL
            Select [UserID],[SecurityObject],[AccessAllowed],[AccessDenied] FROM [UserAccessCustomSecurity] WHERE [UserID]=@UserID"
            OnInserting="SqlDataSourceCustomSecurity_Inserting"
            >
            <DeleteParameters>
                <asp:Parameter Name="UserID" Type="String" />
                <asp:Parameter Name="SecurityObject" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="UserID" Type="String" />
                <asp:Parameter Name="SecurityObject" Type="String" />
                <asp:Parameter Name="AccessAllowed" Type="Byte" />
                <asp:Parameter Name="AccessDenied" Type="Byte" />
            </InsertParameters>
            <SelectParameters>
                <asp:ControlParameter Name="UserID" ControlID="LabelUserID" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="UserID" Type="String" />
                <asp:Parameter Name="SecurityObject" Type="String" />
                <asp:Parameter Name="AccessAllowed" Type="Byte" />
                <asp:Parameter Name="AccessDenied" Type="Byte" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
