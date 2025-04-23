<%@ Page Title="Setup Program Types" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupProgramTypes.aspx.cs" Inherits="Paperwork.SetupProgramTypes" %>
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
    </script>
    <h2><%: Title %></h2>
    <p>
        Setup and maintain program types. Program types are used for reporting purposes to isolate specific transactions. 
        The default program type - School Year will always appear in the selection list and does not need to be added to the list below.
    </p>
    <p>
        Note: Renaming values will not update transactions already associated with that Program Type.
        <br />
        Deleting Program Types will prevent new transactions from being associated with that Program Type. You will still be able to report on deleted Program Types.
    </p>
    <p>
        
    </p>
        
    
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5">
        <ProgressTemplate>
            <div class="loadingDiv" >
                <asp:Image CssClass="loadingImage" ID="ImageLoading" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="DataEntry">
                <asp:GridView ID="GridViewProgramTypes" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramType" DataSourceID="SqlDataSourceProgramTypes" 
                    CssClass="mGrid" Width="400px" OnRowCommand="GridViewProgramTypes_RowCommand" OnRowDataBound="GridViewProgramTypes_RowDataBound" 
                    ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:TemplateField HeaderText="Program Type" SortExpression="ProgramType" HeaderStyle-Width="200" ItemStyle-HorizontalAlign="Center" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEProgramType" runat="server" Text='<%# Bind("ProgramType") %>' Width="150" MaxLength="25"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReqEProgramType" runat="server" ControlToValidate="TextBoxEProgramType" Display="Dynamic" ErrorMessage="Program Type is required." 
                                    ForeColor="Red" ValidationGroup="Update" EnableClientScript="true">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ProgramType") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="text-align:center">
                                    <asp:TextBox ID="TextBoxFProgramType" runat="server" Width="150" MaxLength="25"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ReqFProgramType" runat="server" ControlToValidate="TextBoxFProgramType" Display="Dynamic" ErrorMessage="Program Type is required." 
                                        ForeColor="Red" ValidationGroup="Insert" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            
                            <EditItemTemplate>
                                <div style="min-width:165px;text-align:center">
                                    <asp:LinkButton runat="server" CommandName="Update"
                                        ID="ISave" ToolTip="Save" Text=" Save" TabIndex="21" 
                                        CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Update" />
                                    <asp:LinkButton runat="server" CommandName="Cancel"
                                        ID="ICancel" ToolTip="Cancel" Text=" Cancel" TabIndex="21"
                                        CssClass="fas fa-undo glyphButton mGridLinkButton" ForeColor="Red" CausesValidation="false" />
                                </div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <div style="min-width:165px;text-align:center">
                                    <asp:LinkButton runat="server" CommandName="Edit"
                                        ID="IEdit" ToolTip="Edit" Text=" Edit"
                                        CssClass="fas fa-pencil-alt mGridLinkButton" ForeColor="#336699"    />
                                    <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                            CssClass="fa fa-trash-alt mGridLinkButton" ForeColor="Crimson" Text="Delete"
                                            OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                </div>
                            </ItemTemplate>
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

                <br />
                <asp
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceProgramTypes" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_{0}"
                    OnInserting="SqlDataSourceProgramTypes_Inserting"
                    DeleteCommand="DELETE FROM [ProgramTypeSetup] WHERE [ProgramType] = @old_ProgramType" 
                    InsertCommand="INSERT INTO [ProgramTypeSetup] ([ProgramType]) VALUES (@ProgramType)" 
                    SelectCommand="SELECT [ProgramType] FROM [ProgramTypeSetup] 
                    UNION ALL 
                    SELECT '' ProgramType 
                    ORDER BY [ProgramType]" 
                    UpdateCommand="UPDATE [ProgramTypeSetup] SET [ProgramType] = @ProgramType WHERE [ProgramType] = @old_ProgramType">
                    <InsertParameters>
                        <asp:Parameter Name="ProgramType" Type="String" />
                    </InsertParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
