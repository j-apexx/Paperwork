<%@ Page Title="Setup Paycodes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupPaycodes.aspx.cs" Inherits="Paperwork.SetupPaycodes" %>
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
    <h2><%: Title %></h2>
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="row">
                    <div class="col-sm-3">
                        <strong>Program Year:</strong>
                        <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                        DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                    </div>
                </div>
                <asp:GridView ID="GridViewPaycodes" CssClass="mGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,PayCode" DataSourceID="SqlDataSourcePaycodes"
                    ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" Width="500" AllowSorting="true"
                    OnRowCommand="GridViewPaycodes_RowCommand" OnRowDataBound="GridViewPaycodes_RowDataBound" >
                    <Columns>
                        <asp:TemplateField HeaderText="Description" SortExpression="PayCodeDesc">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEDesc" runat="server" Text='<%# Bind("PayCodeDesc") %>' MaxLength="30" Width="100"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredDesc" runat="server" ControlToValidate="TextBoxEDesc" CssClass="validators"
                                 Display="Dynamic" ErrorMessage="Description is required" Text="*" ValidationGroup="EditPaycode"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelDesc" runat="server" Text='<%# Eval("PayCodeDesc") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFDesc" runat="server" MaxLength="30" Width="100"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFDesc" runat="server" ControlToValidate="TextBoxFDesc" CssClass="validators"
                                 Display="Dynamic" ErrorMessage="Description is required" Text="*" ValidationGroup="InsertPaycode"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Hours Type" SortExpression="HoursType">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLEHoursType" runat="server" SelectedValue='<%# Bind("HoursType") %>' >
                                    <asp:ListItem Text="-Select One-" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Productive" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Non Productive" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Special Function" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RangeValidator ID="ValidHoursType" runat="server" ControlToValidate="DDLEHoursType" MinimumValue="1" 
                                    MaximumValue="3" ValidationGroup="EditPaycode" Display="Dynamic" ErrorMessage="Select a valid Hours Type" Text="*" CssClass="validators"></asp:RangeValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:DropDownList ID="DDLHoursType" runat="server" SelectedValue='<%# Bind("HoursType") %>' Enabled="false" >
                                    <asp:ListItem Text="-Select One-" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Productive" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Non Productive" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Special Function" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="DDLFHoursType" runat="server" >
                                    <asp:ListItem Text="-Select One-" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Productive" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Non Productive" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Special Function" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RangeValidator ID="ValidFHoursType" runat="server" ControlToValidate="DDLFHoursType" MinimumValue="1" 
                                    MaximumValue="3" ValidationGroup="InsertPaycode" Display="Dynamic" ErrorMessage="Select a valid Hours Type" Text="*" CssClass="validators"></asp:RangeValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rate Tier" SortExpression="RateTier">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLERateTier" runat="server" SelectedValue='<%# Bind("RateTier") %>'>
                                    <asp:ListItem Text="-Select One-" Value="0"></asp:ListItem>
                                    <asp:ListItem Value="1"></asp:ListItem>
                                    <asp:ListItem Value="2"></asp:ListItem>
                                    <asp:ListItem Value="3"></asp:ListItem>
                                    <asp:ListItem Value="4"></asp:ListItem>
                                    <asp:ListItem Value="5"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RangeValidator ID="ValidRateTier" runat="server" ControlToValidate="DDLERateTier" MinimumValue="1" 
                                   MaximumValue="5" ValidationGroup="EditPaycode" Display="Dynamic" ErrorMessage="Select a valid Rate Tier" Text="*" CssClass="validators"></asp:RangeValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelRateTier" runat="server" Text='<%# Bind("RateTier") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="DDLFRateTier" runat="server" >
                                    <asp:ListItem Text="-Select One-" Value="0"></asp:ListItem>
                                    <asp:ListItem Value="1"></asp:ListItem>
                                    <asp:ListItem Value="2"></asp:ListItem>
                                    <asp:ListItem Value="3"></asp:ListItem>
                                    <asp:ListItem Value="4"></asp:ListItem>
                                    <asp:ListItem Value="5"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RangeValidator ID="ValidFRateTier" runat="server" ControlToValidate="DDLFRateTier" MinimumValue="1" 
                                   MaximumValue="5" ValidationGroup="InsertPaycode" Display="Dynamic" ErrorMessage="Select a valid Rate Tier" Text="*" CssClass="validators"></asp:RangeValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                                <EditItemTemplate>
                                    <div style="width:60px;text-align:center;" >
                                        <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="EditPaycode" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="width:60px;text-align:center;"  >
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
                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="InsertPaycode" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </FooterTemplate>
                        
                                <ItemStyle Width="60" />
                            </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ValidationSummary ID="SummaryEdit" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                    ValidationGroup="EditPaycode" HeaderText="Editing Errors" BackColor="WhiteSmoke" Width="500" />
                <asp:ValidationSummary ID="SummaryInsert" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                    ValidationGroup="InsertPaycode" HeaderText="Insert Errors" BackColor="WhiteSmoke" Width="500" />
                <div class="messagealert" id="alert_container"></div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourcePaycodes" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
            DeleteCommand="DELETE FROM [DistrictPaycodes] WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND [PayCode] = @PayCode" 
            InsertCommand="INSERT INTO [DistrictPaycodes] ([ProgramYear], [DistrictID], [PayCode], [PayCodeDesc], [HoursType], [RateTier]) 
            VALUES (@ProgramYear, @DistrictID, 
            (SELECT ISNULL(MAX(PayCode), 0)+1 FROM [DistrictPaycodes] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID)
            , dbo.InitCap(@PayCodeDesc), @HoursType, @RateTier)" 
            SelectCommand="SELECT [ProgramYear], [DistrictID], [PayCode], [PayCodeDesc], [HoursType], [RateTier] 
            FROM [DistrictPaycodes] WHERE (([DistrictID] = @DistrictID) AND ([ProgramYear] = @ProgramYear))
            UNION ALL
            SELECT '','',0,'',0,0 
            ORDER BY [PayCodeDesc]"
            UpdateCommand="UPDATE [DistrictPaycodes] SET [PayCodeDesc] = dbo.InitCap(@PayCodeDesc), [HoursType] = @HoursType, 
            [RateTier] = @RateTier WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND 
            [PayCode] = @PayCode"
            OnInserted="SqlDataSourcePaycodes_Inserted" OnInserting="SqlDataSourcePaycodes_Inserting"
            OnDeleted="SqlDataSourcePaycodes_Deleted" OnDeleting="SqlDataSourcePaycodes_Deleting" >
            <DeleteParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="PayCode" Type="Int16" />
            </DeleteParameters>
            <InsertParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:Parameter Name="PayCodeDesc" Type="String" />
                <asp:Parameter Name="HoursType" Type="Int16" />
                <asp:Parameter Name="RateTier" Type="Int16" />
            </InsertParameters>
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="PayCodeDesc" Type="String" />
                <asp:Parameter Name="HoursType" Type="Int16" />
                <asp:Parameter Name="RateTier" Type="Int16" />
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="PayCode" Type="Int16" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
    </div>
</asp:Content>
