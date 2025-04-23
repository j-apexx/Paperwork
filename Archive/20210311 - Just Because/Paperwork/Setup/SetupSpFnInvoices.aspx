<%@ Page Title="Setup Special Functions Invoices" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupSpFnInvoices.aspx.cs" Inherits="Paperwork.SetupSpFnInvoices" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
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
    <h5>The following settings will be used for generating Special Functions Invoices for the selected district.</h5>
    <div id="menuDiv" style="float:left;width:70%">
        <p>
            <strong>Program Year:</strong>
            <asp:Label ID="LabelProgramYear" runat="server" ></asp:Label>
            &nbsp;&nbsp;
            <strong>District:</strong>
            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
            </asp:DropDownList> 
        </p>
    </div>
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:DetailsView ID="DetailsViewInvoiceSetup" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSourceInvoiceSetup" 
                DefaultMode="Edit" Height="50px" width="400" CssClass="mGrid" OnDataBound="DetailsViewInvoiceSetup_DataBound">
                <HeaderStyle Width="200" />
                <Fields>
                    <asp:TemplateField HeaderText="Invoice Prefix">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBoxInvoicePrefix" runat="server" Text='<%# Bind("InvoicePrefix") %>' MaxLength="5" AutoCompleteType="Disabled" Width="75" ToolTip="Optional string which will appear before the invoice number."></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
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
            SelectCommand="SELECT [InvoicePrefix], [NextInvoiceNumber] FROM [SPFNInvoiceSetup] WHERE (([DistrictID] = @DistrictID) AND ([ProgramYear] = @ProgramYear))"
            UpdateCommand="UPDATE [SPFNInvoiceSetup] SET [InvoicePrefix]=@InvoicePrefix, [NextInvoiceNumber]=@NextInvoiceNumber WHERE (([DistrictID] = @DistrictID) AND ([ProgramYear] = @ProgramYear))"
            OnUpdated="SqlDataSourceInvoiceSetup_Updated" OnSelecting="SqlDataSourceInvoiceSetup_Selecting" OnSelected="SqlDataSourceInvoiceSetup_Selected">
            <SelectParameters>
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="InvoicePrefix" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="NextInvoiceNumber" Type="Int32" />
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
