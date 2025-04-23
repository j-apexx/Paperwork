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
                        </div>
                    </EditItemTemplate>
                </asp:FormView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceGlobalSettings" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT * FROM [PaperworkGlobalSettings] WHERE ([Index] = @Index)" 
            UpdateCommand="UPDATE [PaperworkGlobalSettings] SET [ReinhartInvSuffix] = @ReinhartInvSuffix, [SSRS_URL] = @SSRS_URL, [PayablesExportFolder] = @PayablesExportFolder WHERE [Index] = 0">
            <SelectParameters>
                <asp:Parameter DefaultValue="0" Name="Index" Type="Int32" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="ReinhartInvSuffix" Type="String" />
                <asp:Parameter Name="SSRS_URL" Type="String" />
                <asp:Parameter Name="PayablesExportFolder" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
    
</asp:Content>
