<%@ Page Title="Setup Other Sales" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupOtherSales.aspx.cs" Inherits="Paperwork.SetupOtherSales" %>
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
                        Setup Other Sales Categories that Nutrition Invoices the district for.
                    </div>
                    <div id="menuDiv" style="float:left;width:70%">
                        <br />
                        <div class="row">
                            <div class="col-sm-12">
                                <strong>Program Year:</strong>
                                <asp:Label ID="LabelProgramYear" runat="server" ></asp:Label>
                            </div>
                        </div>
                        <div class="messagealert" id="alert_container"></div>
                    </div>
                </div>
            </div>
            <div id="SetupSection">
                <asp:GridView ID="GridViewOtherSales" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,OtherSalesID" 
                    DataSourceID="SqlDataSourceOtherSalesSetup" CssClass="mGrid" Width="500px" ShowFooter="True" ShowHeaderWhenEmpty="True"
                    OnRowCommand="GridViewOtherSales_RowCommand" OnRowDataBound="GridViewOtherSales_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Cost Number" SortExpression="OtherSalesID">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("OtherSalesID") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("OtherSalesID") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFOtherSalesID" runat="server" TextMode="Number"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Description" SortExpression="OtherSalesDesc">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEDescription" runat="server" Text='<%# Bind("OtherSalesDesc") %>' MaxLength="30"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("OtherSalesDesc") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFDescription" runat="server" MaxLength="30"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                                <EditItemTemplate>
                                    <div style="width:60px;text-align:center;" >
                                        <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
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
                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </FooterTemplate>
                        
                                <ItemStyle Width="60" />
                            </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceOtherSalesSetup" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    DeleteCommand="DELETE FROM [OtherSalesCats] WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND [OtherSalesID] = @OtherSalesID" 
                    InsertCommand="INSERT INTO [OtherSalesCats] ([ProgramYear], [DistrictID], [OtherSalesID], [OtherSalesDesc]) VALUES (@ProgramYear, @DistrictID, @OtherSalesID, @OtherSalesDesc)" 
                    SelectCommand="SELECT [ProgramYear], [DistrictID], [OtherSalesID], [OtherSalesDesc] FROM [OtherSalesCats] 
                    WHERE (([DistrictID] = @DistrictID) AND ([ProgramYear] = @ProgramYear))
                    UNION ALL SELECT '', '', 0, '' " 
                    UpdateCommand="UPDATE [OtherSalesCats] SET [OtherSalesDesc] = @OtherSalesDesc WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND [OtherSalesID] = @OtherSalesID"
                    OnInserted="SqlDataSourceOtherSalesSetup_Inserted" OnDeleted="SqlDataSourceOtherSalesSetup_Deleted" OnUpdated="SqlDataSourceOtherSalesSetup_Updated"
                    OnInserting="SqlDataSourceOtherSalesSetup_Inserting" OnDeleting="SqlDataSourceOtherSalesSetup_Deleting" >
                    <DeleteParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="OtherSalesID" Type="Int16" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:Parameter Name="OtherSalesID" Type="Int16" />
                        <asp:Parameter Name="OtherSalesDesc" Type="String" ConvertEmptyStringToNull="false" />
                    </InsertParameters>
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="OtherSalesDesc" Type="String" />
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="OtherSalesID" Type="Int16" />
                    </UpdateParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
