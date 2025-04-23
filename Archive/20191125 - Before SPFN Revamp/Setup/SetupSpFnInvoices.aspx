<%@ Page Title="Setup Special Functions Invoices" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupSpFnInvoices.aspx.cs" Inherits="Paperwork.SetupSpFnInvoices" %>
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
    <h5>The following settings will be used for generating Special Functions Invoices for all districts.</h5>
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:DetailsView ID="DetailsViewInvoiceSetup" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSourceInvoiceSetup" 
                DefaultMode="Edit" Height="50px" width="400" CssClass="mGrid">
                <HeaderStyle Width="200" />
                <Fields>
                    <asp:BoundField DataField="InvoicePrefix" HeaderText="Invoice Prefix" SortExpression="InvoicePrefix" />
                    <asp:BoundField DataField="NextInvoiceNumber" HeaderText="Next Invoice Number" SortExpression="NextInvoiceNumber" />
                    <asp:TemplateField>
                        <EditItemTemplate>
                            <div style="min-width:165px;">
                                <asp:LinkButton runat="server" CommandName="Update"
                                    ID="ISave" ToolTip="Save" Text=" Save" TabIndex="21"
                                    CssClass="fas fa-save glyphButton mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="GridView" />
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateField>

                </Fields>
            </asp:DetailsView>
            <div class="messagealert" id="alert_container"></div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceInvoiceSetup" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [InvoicePrefix], [NextInvoiceNumber] FROM [SPFNInvoiceSetup] WHERE ([RecordKey] = @RecordKey)"
            UpdateCommand="UPDATE [SPFNInvoiceSetup] SET [InvoicePrefix]=@InvoicePrefix, [NextInvoiceNumber]=@NextInvoiceNumber WHERE ([RecordKey] = @RecordKey)"
            OnUpdated="SqlDataSourceInvoiceSetup_Updated">
            <SelectParameters>
                <asp:Parameter DefaultValue="0" Name="RecordKey" Type="Int32" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="InvoicePrefix" Type="String" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="NextInvoiceNumber" Type="Int32" />
                <asp:Parameter DefaultValue="0" Name="RecordKey" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
