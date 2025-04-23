<%@ Page Title="Global Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GlobalSettings.aspx.cs" Inherits="Paperwork.GlobalSettings" %>
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
                <asp:LinkButton ID="ISave" runat="server" 
                    ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                    CssClass="fas fa-save mGridLinkButton" Width="170"   />
                <div style="float:right;">
                    <div class="messagealert" id="alert_container"></div>
                </div>
                <asp:HiddenField ID="HFSettingsSaved" runat="server" Value="0" />
                <asp:FormView ID="FormViewGlobalSettings" runat="server" CssClass="mGrid" DataSourceID="SqlDataSourceGlobalSettings" DefaultMode="Edit"
                    OnPreRender="FormViewGlobalSettings_PreRender">
                    <EditItemTemplate>
                        <div class="container">
                            <div class="row">
                                <div class="col-sm-12">
                                    <div style="font-weight:bold;">Reinhart Invoice Suffix: </div>
                                    The string entered into the following box will append all Reinhart invoices when they are exported to accounting.
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="TextBoxReinhartInvSuffix" runat="server" Text='<%# Bind("ReinhartInvSuffix") %>' MaxLength="5" OnTextChanged="TextBoxSettings_TextChanged" Width="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <hr />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <div style="font-weight:bold;">SQL Server Reporting Services URL: </div>
                                    Enter the base url for SSRS where the Paperwork reports are located. Example: http://localhost/reportserver
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12" >
                                    <asp:TextBox ID="TextBoxSSRSUrl" runat="server" Text='<%# Bind("SSRS_URL") %>' OnTextChanged="TextBoxSettings_TextChanged" CssClass="longTextBox" ></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <div style="font-weight:bold;">SQL Server Reporting Services Folder: </div>
                                    Enter the base folder on the SSRS website where the reports are located: /PaperworkWeb/ or /PaperworkWebTraining/<br />
                                    *Note: this setting is stored in the web.config file in the web application root folder and must be changed there by administrators. This is necessary to allow the training company to be refreshed, while not overwriting the reports folder.
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12" >
                                    <asp:TextBox ID="TextBoxSSRSFolder" runat="server" Text='<%$ AppSettings:SSRS_BaseFolder %>' CssClass="longTextBox" Enabled="false" ></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <hr />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <div style="font-weight:bold;">Payables Export Path: </div>
                                    Enter the UNC path for the default export location. Example: \\SERVER1\DataShare\DestinationFolder\
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="TextBoxPayablesExportPath" runat="server" Text='<%# Bind("PayablesExportFolder") %>' OnTextChanged="TextBoxSettings_TextChanged" CssClass="longTextBox" ></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <hr />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <div style="font-weight:bold;">Export File Suffix: </div>
                                    Optionally enter a suffix to add to exported file names (currently used in Payables and Inventory Change exports). 
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <asp:TextBox ID="TextBoxExportFileSuffix" runat="server" Text='<%# Bind("ExportFileSuffix") %>' OnTextChanged="TextBoxSettings_TextChanged" CssClass="longTextBox" MaxLength="15" ></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <hr />
                                </div>
                            </div>
                        </div>
                    </EditItemTemplate>
                </asp:FormView>
                <br />
                <asp:GridView ID="GridViewOtherSettings" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear" DataSourceID="SqlDataSourcePaperworkOtherSettings" CssClass="mGrid"
                    Width="400px" OnRowUpdating="GridViewOtherSettings_RowUpdating">
                    <Columns>
                        <asp:BoundField DataField="ProgramYear" HeaderText="Program Year" ReadOnly="True" SortExpression="ProgramYear" HeaderStyle-Width="130" ItemStyle-HorizontalAlign="Center" />
                        <asp:TemplateField HeaderText="Sales Report NonProgram Rate" SortExpression="SalesReportNonProgramRate" HeaderStyle-Width="200" ItemStyle-HorizontalAlign="Center" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxNonProgRate" runat="server" Text='<%# Bind("SalesReportNonProgramRate") %>' ></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelNonProgRate" runat="server" Text='<%# Eval("SalesReportNonProgramRate") %>'></asp:Label>
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

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceGlobalSettings" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT * FROM [PaperworkGlobalSettings] WHERE ([Index] = @Index)" 
            UpdateCommand="UPDATE [PaperworkGlobalSettings] SET [ReinhartInvSuffix] = @ReinhartInvSuffix, [SSRS_URL] = @SSRS_URL, [PayablesExportFolder] = @PayablesExportFolder, [ExportFileSuffix]=@ExportFileSuffix  WHERE [Index] = 0">
            <SelectParameters>
                <asp:Parameter DefaultValue="0" Name="Index" Type="Int32" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="ReinhartInvSuffix" Type="String" />
                <asp:Parameter Name="SSRS_URL" Type="String" />
                <asp:Parameter Name="PayablesExportFolder" Type="String" />
                <asp:Parameter Name="ExportFileSuffix" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourcePaperworkOtherSettings" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT * FROM PaperworkOtherSettings ORDER BY ProgramYear DESC" UpdateCommand="UPDATE PaperworkOtherSettings SET [SalesReportNonProgramRate] = @SalesReportNonProgramRate WHERE [ProgramYear] = @ProgramYear">
            <UpdateParameters>
                <asp:Parameter Name="SalesReportNonProgramRate" Type="Decimal" />
                <asp:Parameter Name="ProgramYear" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
    
</asp:Content>
