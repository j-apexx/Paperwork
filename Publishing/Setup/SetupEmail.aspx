<%@ Page Title="Email Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupEmail.aspx.cs" Inherits="Paperwork.SetupEmail" %>
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
                <div style="float:right;">
                    <div class="messagealert" id="alert_container"></div>
                </div>
                <asp:HiddenField ID="HFSettingsSaved" runat="server" Value="0" />
                <asp:DetailsView ID="DetailsViewEmailSettings" runat="server" Height="50px" Width="500px" AutoGenerateRows="False" 
                    DataSourceID="SqlDataSourceEmailSettings" DefaultMode="Edit" CssClass="mGrid" >
                    <Fields>
                        <asp:BoundField DataField="emailServer" HeaderText="Email Server" SortExpression="emailServer" ControlStyle-Width="300" />
                        <asp:TemplateField HeaderText="Exchange Web Svcs">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CB_UseEWS" runat="server" Checked='<%# (byte)Eval("emailUseEWS")==1?true:false %>' Text="Use Exchange Web Svcs" OnCheckedChanged="CB_UseEWS_CheckedChanged"  />
                                <asp:HiddenField ID="HFEWS" runat="server" Value='<%# Bind("emailUseEWS") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="emailPort" HeaderText="Port" SortExpression="emailPort" />
                        <asp:TemplateField HeaderText="SSL">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CB_UseSSL" runat="server" Checked='<%# (byte)Eval("emailUseSSL")==1?true:false %>' Text="Use SSL" OnCheckedChanged="CB_UseSSL_CheckedChanged"  />
                                <asp:HiddenField ID="HFSSL" runat="server" Value='<%# Bind("emailUseSSL") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="emailFromAddress" HeaderText="From Address" SortExpression="emailFromAddress" ControlStyle-Width="300" />
                        <asp:TemplateField HeaderText="Credentials">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxDomain" runat="server" Text='<%# Bind("emailAuthDomain") %>'></asp:TextBox><br />
                                <asp:TextBox ID="TextBoxUserName" runat="server" Text='<%# Bind("emailAuthUser") %>'></asp:TextBox><br />
                                <asp:TextBox ID="TextBoxPassword" runat="server" TextMode="Password" Text='<%# Bind("emailAuthPassword") %>' value='<%# Eval("emailAuthPassword") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <EditItemTemplate>
                                Enter email recipients below separated by commas when multiple.
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NewVendorRequests" HeaderText="New Vendor Requests Email" SortExpression="NewVendorRequests" ControlStyle-Width="300" />
                        <asp:BoundField DataField="AdminContact" HeaderText="Admin Email" SortExpression="AdminContact" ControlStyle-Width="300" />
                        <asp:TemplateField>
                        <EditItemTemplate>
                            <div style="min-width:165px;">
                                <asp:LinkButton runat="server" CommandName="Update"
                                    ID="ISave" ToolTip="Save" Text="Save" TabIndex="21"
                                    CssClass="fas fa-save glyphButton mGridLinkButton" ForeColor="Green" />
                                <asp:LinkButton runat="server" CommandName="Cancel"
                                    ID="LinkButtonCancel" ToolTip="Reload" Text="Reload" TabIndex="21"
                                    CssClass="fas fa-sync glyphButton mGridLinkButton" ForeColor="Gray" />
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceEmailSettings" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT * FROM [PaperworkWebEmailSettings] WHERE ([Index] = @Index)" 
            UpdateCommand="UPDATE [CommissaryWebEmailSettings] SET [emailServer] = @emailServer, [emailUseEWS] = @emailUseEWS, 
            [emailPort] = @emailPort, [emailUseSSL] = @emailUseSSL, [emailFromAddress] = @emailFromAddress, 
            [emailAuthDomain] = @emailAuthDomain, [emailAuthUser] = @emailAuthUser, [emailAuthPassword] = @emailAuthPassword, 
            [NewVendorRequests] = @NewVendorRequests, [AdminContact] = @AdminContact WHERE [Index]=0">
            <SelectParameters>
                <asp:Parameter DefaultValue="0" Name="Index" Type="Int32" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="emailServer" Type="String" />
                <asp:Parameter Name="emailUseEWS" Type="Byte" />
                <asp:Parameter Name="emailPort" Type="Int32" />
                <asp:Parameter Name="emailUseSSL" Type="Byte" />
                <asp:Parameter Name="emailFromAddress" Type="String" />
                <asp:Parameter Name="emailAuthDomain" Type="String" />
                <asp:Parameter Name="emailAuthUser" Type="String" />
                <asp:Parameter Name="emailAuthPassword" Type="String" />
                <asp:Parameter Name="NewVendorRequests" Type="String" />
                <asp:Parameter Name="AdminContact" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>

    </div>
    
</asp:Content>
