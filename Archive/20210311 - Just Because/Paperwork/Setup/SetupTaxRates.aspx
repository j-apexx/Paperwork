<%@ Page Title="Setup Tax Rates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupTaxRates.aspx.cs" Inherits="Paperwork.SetupTaxRates" %>
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
    <p>Setup and maintain program year tax rates.</p>
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
                <asp:GridView ID="GridViewTaxRates" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear" DataSourceID="SqlDataSourceTaxRates" CssClass="mGrid"
                    Width="700px" OnRowUpdating="GridViewTaxRates_RowUpdating">
                    <Columns>
                        <asp:BoundField DataField="ProgramYear" HeaderText="Program Year" ReadOnly="True" SortExpression="ProgramYear" HeaderStyle-Width="130" ItemStyle-HorizontalAlign="Center" />
                        <asp:TemplateField HeaderText="Tax Year" SortExpression="TaxYear" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxTaxYear" runat="server" Text='<%# Bind("TaxYear") %>' Width="75" MaxLength="4"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("TaxYear") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Payroll Proc." SortExpression="PayProc" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxPayProc" runat="server" Text='<%# Bind("PayProc") %>' Width="75"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("PayProc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Workers Comp." SortExpression="WComp" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxWComp" runat="server" Text='<%# Bind("WComp") %>' Width="75"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("WComp") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Other Pay Related" SortExpression="OtherPayRelated" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxOtherPayRelated" runat="server" Text='<%# Bind("OtherPayRelated") %>' Width="75"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("OtherPayRelated") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ohio SERS" SortExpression="OH_SERS" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxOH_SERS" runat="server" Text='<%# Bind("OH_SERS") %>' Width="75"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelOH_SERS" runat="server" Text='<%# Eval("OH_SERS") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <div style="min-width:165px;text-align:center">
                                    <asp:LinkButton runat="server" CommandName="Edit"
                                        ID="IEdit" ToolTip="Edit" Text=" Edit"
                                        CssClass="fas fa-pencil-alt mGridLinkButton" ForeColor="#336699"    />
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
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceTaxRates" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    DeleteCommand="DELETE FROM [TaxRates] WHERE [ProgramYear] = @ProgramYear" 
                    InsertCommand="INSERT INTO [TaxRates] ([ProgramYear], [TaxYear], [PayProc], [WComp], [OtherPayRelated]) VALUES (@ProgramYear, @TaxYear, @PayProc, @WComp, @OtherPayRelated)" 
                    SelectCommand="SELECT * FROM [TaxRates] ORDER BY [ProgramYear] DESC" 
                    UpdateCommand="UPDATE [TaxRates] SET [TaxYear] = @TaxYear, [PayProc] = @PayProc, [WComp] = @WComp, [OtherPayRelated] = @OtherPayRelated, [OH_SERS]=@OH_SERS 
                    WHERE [ProgramYear] = @ProgramYear">
                    <DeleteParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="TaxYear" Type="Int32" />
                        <asp:Parameter Name="PayProc" Type="Decimal" />
                        <asp:Parameter Name="WComp" Type="Decimal" />
                        <asp:Parameter Name="OtherPayRelated" Type="Decimal" />
                    </InsertParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="TaxYear" Type="Int32" />
                        <asp:Parameter Name="PayProc" Type="Decimal" />
                        <asp:Parameter Name="WComp" Type="Decimal" />
                        <asp:Parameter Name="OtherPayRelated" Type="Decimal" />
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="OH_SERS" Type="Decimal" />
                    </UpdateParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
