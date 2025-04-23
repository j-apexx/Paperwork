<%@ Page Title="Special Functions Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SpecialFunctionsEntry.aspx.cs" Inherits="Paperwork.SpecialFunctionsEntry" EnableEventValidation="false" %>
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
            $(function () {
                $("[id*=TextBoxFunctionDateA]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id*=TextBoxBillingDateA]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id*=TextBoxFunctionDateB]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id*=TextBoxBillingDateB]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            $("[id*=TextBoxEAmountA],[id*=TextBoxFAmountA],[id*=TextBoxEAmountB],[id*=TextBoxFAmountB]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
            });
        }

    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgressInvoices" runat="server" AssociatedUpdatePanelID="UpdatePanelInvoices" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="topSection" class="row">
                <div id="hdrgroup" class="col-sm-12">
                     <div id="hdrDiv" style="float:left;width:40%">
                        <h2><%: Title %></h2>
                    </div>
                    <div id="menuDiv">
                        <div class="row">
                            <div class="col-sm-12">
                                <strong>Program Year:</strong>
                                <asp:Label ID="LabelProgramYear" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="dataentry">
                <div id="accordian">
                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse1">
                                Select/Create Invoices
                            </a>
                        </div>
                        <div id="cardCollapse1" class="collapse show">
                            <div class="card-body">
                                <div style="top:-10px;position:relative;">
                                    <asp:LinkButton ID="LinkButtonNewInvoice" runat="server" 
                                        ToolTip="Create New Invoice" ForeColor="Green"
                                        CssClass=" mGridLinkButton" Width="200" OnClick="ICreateClick" >
                                        <i class="fas fa-asterisk"></i> Create New Invoice
                                    </asp:LinkButton>
                                </div>
                                <asp:GridView ID="GridViewInvoices" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber" 
                                    DataSourceID="SqlDataSourceInvHdrsList" CssClass="mGrid" AllowPaging="true" PageSize="25"  PagerSettings-Mode="NumericFirstLast"  
                                    PagerStyle-HorizontalAlign="Center" PagerStyle-Font-Size="Large" PagerSettings-PageButtonCount="10" PagerStyle-Width="100" PagerStyle-CssClass="mGridPager"
                                    OnSelectedIndexChanged="GridViewInvoices_SelectedIndexChanged" OnRowDeleted="GridViewInvoices_RowDeleted"
                                    OnRowDataBound="GridviewClickableRowDatabound">
                                    <Columns>
                                        <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" SortExpression="InvoiceNumber" ReadOnly="True" HeaderStyle-Width="150" />
                                        <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" SortExpression="CreatedDate" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="CreatedUser" HeaderText="Created User" SortExpression="CreatedUser" />
                                        <asp:BoundField DataField="OrganizationBilled" HeaderText="Organization" SortExpression="OrganizationBilled" />
                                        <asp:BoundField DataField="FunctionDate" HeaderText="Function Date" SortExpression="FunctionDate" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="BillingDate" HeaderText="Billing Date" SortExpression="BillingDate" DataFormatString="{0:d}" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse2">
                                Invoice Details
                            </a>
                        </div>
                        <div id="cardCollapse2" class="collapse show">
                            <div class="card-body">
                                <asp:UpdatePanel ID="UpdatePanelInvoices" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row" style="text-align:center;position:relative;top:-10px;">
                                            <div class="col-sm-12">
                                                <asp:HiddenField ID="HFInvoiceNumber" runat="server" />
                                                <asp:LinkButton ID="ISave" runat="server" 
                                                    ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                                    CssClass="fas fa-save mGridLinkButton" Width="200"></asp:LinkButton>
                                                <asp:LinkButton ID="IPrint" Width="200" runat="server" 
                                                    class="fas fa-print mGridLinkButton" 
                                                    ToolTip="Print" Text=" Print Invoices" ></asp:LinkButton>
                                            </div>
                                            
                                        </div>
                                        
                                        <div class="row">
                                            <div class="col-sm-6" style="border-style:solid;border-width:1px;border-color:dimgray;">
                                                For Organization
                                                <asp:FormView ID="FormViewSpFnOrg" runat="server" CssClass="mGridRecaps" DefaultMode="Edit" DataSourceID="SqlDataSourceInvSubHdrA" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber,InvoiceSuffix" >
                                                    <EditItemTemplate>
                                                        <div class="container">
                                                            <div class="row">
                                                                    <div class="col-sm-6">
                                                                        <asp:TextBox ID="TextBoxInvoiceA" runat="server" Text='<%# Bind("DisplayInvoice") %>' Enabled="false" Width="100"></asp:TextBox>&nbsp;Invoice No.
                                                                    </div>
                                                                    <div style="float:right;text-align:right;z-index:100;position:absolute;right:20px;">
                                                                        <asp:Image ID="ImageLogo" runat="server" ImageUrl="~/Images/InvoiceLogoSM.png" ImageAlign="Right" />
                                                                    </div>
                                                                </div>
                                                            <div class="row">
                                                                <div class="col-sm-12" style="text-align:center;">
                                                                    <h5>SPECIAL FUNCTION BILLING</h5>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-4">
                                                                    <asp:Label ID="LabelOrgBilled" runat="server" Text="ORGANIZATION BILLED:" Width="200"></asp:Label>
                                                                </div>
                                                                <div class="col-sm-8" style="text-align:right;">
                                                                    <asp:TextBox ID="TextBoxOrgBilled" runat="server" Text='<%# Bind("OrganizationBilled") %>' Width="100%"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-4">
                                                                    <asp:Label ID="Label1" runat="server" Text="CONTACT PERSON:" Width="200"></asp:Label>
                                                                </div>
                                                                <div class="col-sm-8" style="text-align:right;">
                                                                    <asp:TextBox ID="TextBoxContactPerson" runat="server" Text='<%# Bind("ContactPerson") %>' Width="100%"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-6">
                                                                    <asp:Label ID="Label2" runat="server" Text="FUNCTION DATE" Width="130"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxFunctionDateA" runat="server" Text='<%# Bind("FunctionDate", "{0:d}") %>' Width="90"></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-6" style="text-align:right;">
                                                                    <asp:Label ID="Label3" runat="server" Text="BILLING DATE" Width="110"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxBillingDateA" runat="server" Text='<%# Bind("BillingDate", "{0:d}") %>' Width="90"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-8">
                                                                    <asp:Label ID="LabelDistrict" runat="server" Text="DISTRICT" ></asp:Label>
                                                                    <asp:TextBox ID="TextBoxDistName" runat="server" Text='<%# Session["CurrentDistrictName"].ToString() %>' Width="250" Enabled="false"></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-4" style="text-align:right;">
                                                                    <asp:Label ID="LabelUnit" runat="server" Text="Unit #"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxDistID" runat="server" Text='<%# Session["CurrentDistrict"].ToString() %>' Enabled="false" Width="50"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-12">
                                                                    <asp:GridView ID="GridViewLinesA" runat="server" DataSourceID="SqlDataSourceInvSubLinesA" AutoGenerateColumns="False" 
                                                                        ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber,InvoiceSuffix,LineNumber"
                                                                        OnRowCommand="GridViewLinesA_RowCommand" OnRowDataBound="GridViewLines_RowDataBound" Width="100%"  >
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Description of Services" >
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxEDescription" runat="server" Text='<%# Bind("ServiceDescription") %>' Width="100%" ></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    *<asp:TextBox ID="TextBoxFDescription" runat="server" Width="100%" ></asp:TextBox>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Amount" HeaderStyle-Width="100" >
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxEAmountA" runat="server" Text='<%# Bind("ServiceAmount") %>' Width="100" ></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <asp:TextBox ID="TextBoxFAmountA" runat="server" Width="100" ></asp:TextBox>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderStyle-Width="30" >
                                                                                <ItemTemplate>
                                                                                    <div style="width:30px;text-align:center;" >
                                                                                        <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                                                            CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                                                            OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="width:30px;text-align:center;" >
                                                                                        <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                                                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-12" style="text-align:right;">
                                                                    <asp:Label ID="LabelAmtDue" runat="server" Text="AMOUNT DUE"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxInvoiceTotalA" runat="server" Text='<%# Eval("InvoiceTotal", "{0:C2}") %>' Enabled="false" Width="130"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-12" style="text-align:center;">
                                                                    <asp:Label ID="Label4" runat="server" Text="PLEASE MAKE CHECK PAYABLE TO:"></asp:Label><br />
                                                                    <asp:TextBox ID="TextBoxDueToA" runat="server" Text='<%# Eval("DueTo") %>' Enabled="false" Width="300"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </EditItemTemplate>
                                                </asp:FormView>
                                            </div>
                                            <div class="col-sm-6" style="border-style:solid;border-width:1px;border-color:dimgray;">
                                                For District
                                                <asp:FormView ID="FormViewSpFnDist" runat="server" CssClass="mGridRecaps" DefaultMode="Edit" DataSourceID="SqlDataSourceInvSubHdrB" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber,InvoiceSuffix" >
                                                    <EditItemTemplate>
                                                        <div class="container">
                                                            <div class="row">
                                                                    <div class="col-sm-6">
                                                                        <asp:TextBox ID="TextBoxInvoiceB" runat="server" Text='<%# Bind("DisplayInvoice") %>' Enabled="false" Width="100"></asp:TextBox>&nbsp;Invoice No.
                                                                    </div>
                                                                    <div style="float:right;text-align:right;z-index:100;position:absolute;right:20px;">
                                                                        <asp:Image ID="ImageLogo" runat="server" ImageUrl="~/Images/InvoiceLogoSM.png" ImageAlign="Right" />
                                                                    </div>
                                                                </div>
                                                            <div class="row">
                                                                <div class="col-sm-12" style="text-align:center;">
                                                                    <h5>SPECIAL FUNCTION BILLING</h5>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-4">
                                                                    <asp:Label ID="LabelOrgBilled" runat="server" Text="ORGANIZATION BILLED:" Width="200"></asp:Label>
                                                                </div>
                                                                <div class="col-sm-8" style="text-align:right;">
                                                                    <asp:TextBox ID="TextBoxOrgBilled" runat="server" Text='<%# Bind("OrganizationBilled") %>' Width="100%"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-4">
                                                                    <asp:Label ID="Label1" runat="server" Text="CONTACT PERSON:" Width="200"></asp:Label>
                                                                </div>
                                                                <div class="col-sm-8" style="text-align:right;">
                                                                    <asp:TextBox ID="TextBoxContactPerson" runat="server" Text='<%# Bind("ContactPerson") %>' Width="100%"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-6">
                                                                    <asp:Label ID="Label2" runat="server" Text="FUNCTION DATE" Width="130"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxFunctionDateB" runat="server" Text='<%# Bind("FunctionDate", "{0:d}") %>' Width="90"></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-6" style="text-align:right;">
                                                                    <asp:Label ID="Label3" runat="server" Text="BILLING DATE" Width="110"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxBillingDateB" runat="server" Text='<%# Bind("BillingDate", "{0:d}") %>' Width="90"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-8">
                                                                    <asp:Label ID="LabelDistrict" runat="server" Text="DISTRICT" ></asp:Label>
                                                                    <asp:TextBox ID="TextBoxDistName" runat="server" Text='<%# Session["CurrentDistrictName"].ToString() %>' Width="250" Enabled="false"></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-4" style="text-align:right;">
                                                                    <asp:Label ID="LabelUnit" runat="server" Text="UNIT #"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxDistID" runat="server" Text='<%# Session["CurrentDistrict"].ToString() %>' Enabled="false" Width="50"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-12">
                                                                    <asp:GridView ID="GridViewLinesB" runat="server" DataSourceID="SqlDataSourceInvSubLinesB" AutoGenerateColumns="False" 
                                                                        ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber,InvoiceSuffix,LineNumber"
                                                                        OnRowCommand="GridViewLinesB_RowCommand" OnRowDataBound="GridViewLines_RowDataBound" Width="100%"  >
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Description of Services" >
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxEDescription" runat="server" Text='<%# Bind("ServiceDescription") %>' Width="100%" ></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    *<asp:TextBox ID="TextBoxFDescription" runat="server" Width="100%" ></asp:TextBox>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Amount" HeaderStyle-Width="100" >
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxEAmountB" runat="server" Text='<%# Bind("ServiceAmount") %>' Width="100" ></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <asp:TextBox ID="TextBoxFAmountB" runat="server" Width="100" ></asp:TextBox>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderStyle-Width="30" >
                                                                                <ItemTemplate>
                                                                                    <div style="width:30px;text-align:center;" >
                                                                                        <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                                                            CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                                                            OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="width:30px;text-align:center;" >
                                                                                        <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                                                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-12" style="text-align:right;">
                                                                    <asp:Label ID="LabelAmtDue" runat="server" Text="AMOUNT DUE"></asp:Label>
                                                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Eval("InvoiceTotal", "{0:C2}") %>' Enabled="false" Width="130"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-12" style="text-align:center;">
                                                                    <asp:Label ID="Label4" runat="server" Text="PLEASE MAKE CHECK PAYABLE TO:"></asp:Label><br />
                                                                    <asp:TextBox ID="TextBoxDueToB" runat="server" Text='<%# Eval("DueTo") %>' Enabled="false" Width="300"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </EditItemTemplate>
                                                </asp:FormView>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
        
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="datasources">
                <asp:SqlDataSource ID="SqlDataSourceInvHdrsList" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT Hdr.*, Sub.OrganizationBilled, Sub.BillingDate, Sub.FunctionDate
                    FROM [SPFNInvoiceHDR] Hdr
                    LEFT OUTER JOIN [dbo].[SPFNSubInvoiceHdr] Sub ON HDR.ProgramYear=Sub.ProgramYear AND Hdr.DistrictID=Sub.DistrictID AND Hdr.InvoiceNumber=Sub.InvoiceNumber AND Sub.InvoiceSuffix='A'
                    WHERE ((Hdr.[ProgramYear] = @ProgramYear) AND (Hdr.[DistrictID] = @DistrictID)) ORDER BY Hdr.InvoiceNumber DESC"
                     >
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceInvSubHdrA" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT [ProgramYear],[DistrictID],[InvoiceNumber],[InvoiceSuffix],[InvoiceNumber]+[InvoiceSuffix] DisplayInvoice
                    ,[OrganizationBilled],[ContactPerson],[FunctionDate],[BillingDate],[DueTo] 
                    ,(SELECT SUM([ServiceAmount]) FROM [SPFNSubInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) AND ([InvoiceSuffix] = @InvoiceSuffix))) InvoiceTotal
                    FROM [SPFNSubInvoiceHDR] 
                    WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) AND ([InvoiceSuffix] = @InvoiceSuffix)) ">
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:ControlParameter Name="InvoiceNumber" ControlID="HFInvoicenumber" PropertyName="Value" Type="String" />
                        <asp:Parameter Name="InvoiceSuffix" Type="String" DefaultValue="A" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceInvSubHdrB" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT [ProgramYear],[DistrictID],[InvoiceNumber],[InvoiceSuffix],[InvoiceNumber]+[InvoiceSuffix] DisplayInvoice
                    ,[OrganizationBilled],[ContactPerson],[FunctionDate],[BillingDate],[DueTo] 
                    ,(SELECT SUM([ServiceAmount]) FROM [SPFNSubInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) AND ([InvoiceSuffix] = @InvoiceSuffix))) InvoiceTotal
                    FROM [SPFNSubInvoiceHDR] 
                    WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) AND ([InvoiceSuffix] = @InvoiceSuffix)) ">
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:ControlParameter Name="InvoiceNumber" ControlID="HFInvoicenumber" PropertyName="Value" Type="String" />
                        <asp:Parameter Name="InvoiceSuffix" Type="String" DefaultValue="B" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceInvSubLinesA" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT [ProgramYear],[DistrictID],[InvoiceNumber],[InvoiceSuffix],[LineNumber],[ServiceDescription],[ServiceAmount] 
                    FROM [SPFNSubInvoiceLines] 
                    WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) AND ([InvoiceSuffix] = @InvoiceSuffix))
                    UNION ALL
                    SELECT '' [ProgramYear],'' [DistrictID],'' [InvoiceNumber],'A' [InvoiceSuffix],0 [LineNumber],'' [ServiceDescription],0.00 [ServiceAmount] "
                    DeleteCommand="DELETE [SPFNSubInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) 
                    AND ([InvoiceSuffix] = @InvoiceSuffix) AND ([LineNumber] = @LineNumber))"
                    InsertCommand="INSERT INTO [SPFNSubInvoiceLines] ([ProgramYear],[DistrictID],[InvoiceNumber],[InvoiceSuffix],[LineNumber],[ServiceDescription],[ServiceAmount]) 
                    VALUES(@ProgramYear,@DistrictID,@InvoiceNumber,@InvoiceSuffix,
                    (SELECT ISNULL(MAX([LineNumber]),0)+100 FROM [SPFNSubInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) AND ([InvoiceSuffix] = @InvoiceSuffix)))
                    ,@ServiceDescription,@ServiceAmount)"
                    OnInserting="SqlDataSourceInvSubLinesA_Inserting" OnInserted="SqlDataSourceInvSubLinesA_Inserted" >
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:ControlParameter Name="InvoiceNumber" ControlID="HFInvoicenumber" PropertyName="Value" Type="String" />
                        <asp:Parameter Name="InvoiceSuffix" Type="String" DefaultValue="A" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="InvoiceNumber" Type="String" />
                        <asp:Parameter Name="InvoiceSuffix" Type="String" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="InvoiceNumber" Type="String" />
                        <asp:Parameter Name="InvoiceSuffix" Type="String" />
                        <asp:Parameter Name="ServiceDescription" Type="String" />
                        <asp:Parameter Name="ServiceAmount" Type="Decimal" />
                    </InsertParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceInvSubLinesB" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT [ProgramYear],[DistrictID],[InvoiceNumber],[InvoiceSuffix],[LineNumber],[ServiceDescription],[ServiceAmount] 
                    FROM [SPFNSubInvoiceLines] 
                    WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) AND ([InvoiceSuffix] = @InvoiceSuffix))
                    UNION ALL
                    SELECT '' [ProgramYear],'' [DistrictID],'' [InvoiceNumber],'A' [InvoiceSuffix],0 [LineNumber],'' [ServiceDescription],0.00 [ServiceAmount] "
                    DeleteCommand="DELETE [SPFNSubInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) 
                    AND ([InvoiceSuffix] = @InvoiceSuffix) AND ([LineNumber] = @LineNumber))"
                    InsertCommand="INSERT INTO [SPFNSubInvoiceLines] ([ProgramYear],[DistrictID],[InvoiceNumber],[InvoiceSuffix],[LineNumber],[ServiceDescription],[ServiceAmount]) 
                    VALUES(@ProgramYear,@DistrictID,@InvoiceNumber,@InvoiceSuffix,
                    (SELECT ISNULL(MAX([LineNumber]),0)+100 FROM [SPFNSubInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) AND ([InvoiceSuffix] = @InvoiceSuffix)))
                    ,@ServiceDescription,@ServiceAmount)"
                    OnInserting="SqlDataSourceInvSubLinesB_Inserting" OnInserted="SqlDataSourceInvSubLinesB_Inserted" >
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:ControlParameter Name="InvoiceNumber" ControlID="HFInvoicenumber" PropertyName="Value" Type="String" />
                        <asp:Parameter Name="InvoiceSuffix" Type="String" DefaultValue="B" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="InvoiceNumber" Type="String" />
                        <asp:Parameter Name="InvoiceSuffix" Type="String" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="InvoiceNumber" Type="String" />
                        <asp:Parameter Name="InvoiceSuffix" Type="String" />
                        <asp:Parameter Name="ServiceDescription" Type="String" />
                        <asp:Parameter Name="ServiceAmount" Type="Decimal" />
                    </InsertParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
