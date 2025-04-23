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
                $("[id*=TextBoxFunctionDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id*=TextBoxBillingDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            
            $(function () {
                $("[id*=TextBoxDatePaid]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    defaultDate: 0
                });
                //$("[id*=TextBoxDatePaid]").datepicker('setDate', new Date());
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

            $("[id*=TextBoxTNGLabor]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val())) ) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcTNGFringe();
                RecalcInvoiceTotal();
            });

            $("[id*=TextBoxTNGFringePct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.0000');
                    $(this).select();
                }
                RecalcTNGFringe();
                RecalcInvoiceTotal();
            });

            $("[id*=TextBoxDistLabor]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcDistFringe();
                RecalcInvoiceTotal();
            });

            $("[id*=TextBoxDistFringePct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.0000');
                    $(this).select();
                }
                RecalcDistFringe();
                RecalcInvoiceTotal();
            });

            $("[id*=TextBoxFoodCost],[id*=TextBoxPaperCleanSup],[id*=TextBoxMileage],[id*=TextBoxFSDCosts],[id*=TextBoxPaperCleanSup],[id*=TextBoxTextBoxMarkupDlr],[id*=TextBoxMarkupDlr]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }

                RecalcInvoiceTotal();
            });

            $("[id*=TextBoxMarkupPct]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.0000');
                    $(this).select();
                }
                RecalcInvoiceTotal();
            });

            $("[id*=DDLMarkupType]").change(function () {
                MarkupShowHide();
                RecalcInvoiceTotal();
            });

            $("[id*=DDLEMarkupType]").change(function () {
                MarkupShowHideEdit();
                RecalcLineEditTotal();
                RecalcInvoiceTotal();
            });

            $("[id*=DDLFMarkupType]").change(function () {
                MarkupShowHideFooter();
                RecalcLineFooterTotal
                RecalcInvoiceTotal();
            });

            //Miscellaneous Amounts
            //Markup Pct and Dollars
            $("[id*=TextBoxFMarkupPct],[id*=TextBoxFMarkupDlr],[id*=TextBoxFMiscAmount]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcLineFooterTotal();
            });

            $("[id*=TextBoxEMiscAmount],[id*=TextBoxEMarkupPct],[id*=TextBoxEMarkupDlr]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }

                RecalcLineEditTotal();
            });

            

            //Triggers to Store Card Collapse States
            $("#cardCollapse1").on('shown.bs.collapse', function () {
                $('#<%= HFcollapse1State.ClientID %>').val("showcollapse1");
            });
            $("#cardCollapse1").on('hidden.bs.collapse', function () {
                $('#<%= HFcollapse1State.ClientID %>').val(null);
            });

            $("#cardCollapse2").on('shown.bs.collapse', function () {
                $('#<%= HFcollapse2State.ClientID %>').val("showcollapse2");
            });
            $("#cardCollapse2").on('hidden.bs.collapse', function () {
                $('#<%= HFcollapse2State.ClientID %>').val(null);
            });

            //Set Card Collapse States
            var last1 = $('#<%= HFcollapse1State.ClientID %>').val();
            var last2 = $('#<%= HFcollapse2State.ClientID %>').val();

            if (last1 != null && last1 != "") {
                <%--$("#accordian .panel-collapse").addClass('in');--%>
                $("#cardCollapse1").addClass('show');
            }
            else {
                <%--$("#accordian .panel-collapse").removeClass('in');--%>
                $("#cardCollapse1").removeClass('show');
            }

            if (last2 != null && last2 != "") {
                <%--$("#accordian .panel-collapse").addClass('in');--%>
                $("#cardCollapse2").addClass('show');
            }
            else {
                <%--$("#accordian .panel-collapse").removeClass('in');--%>
                $("#cardCollapse2").removeClass('show');
            }

            //Enable Enter Key tabbing
            $('input,select,textarea').on("keypress", function (e) {
                /* ENTER PRESSED*/
                if (e.keyCode == 13) {

                    //This code moves between focusable elements. 
                    var $canfocus = $(':focusable');
                    //var index = $canfocus.index(document.activeElement) + 1;
                    var index = $canfocus.index(this) + 1;
                    if (index >= $canfocus.length) index = 0;
                    $canfocus.eq(index).focus();
                    $canfocus.eq(index).select();//helps with dropdowns


                    //When focus gets to the first Breakfast Totals box - redirect to Checkbox Breakfasts
                    if ($canfocus.eq(index).attr('id') === $("[id*=TextBoxBPaidTtl]").attr('id')) {
                        $("[id*=CBBreakfast]").focus();
                    }

                    return false;//Important - marks enter as handled. Otherwise dropdown lists will recover focus and open/close the dropdown.
                }
            });

            MarkupShowHideFooter();
            
        }

        function MarkupShowHide() {
            var markupType = $("[id*=DDLMarkupType]").val();

            if (markupType == "1")//%
            {
                $("[id*=TextBoxMarkupPct]").show();
                $("[id*=TextBoxMarkupDlr]").val('0.00');
                $("[id*=TextBoxMarkupDlr]").hide();
            }
            else if (markupType == "2")//%
            {
                $("[id*=TextBoxMarkupDlr]").show();
                $("[id*=TextBoxMarkupPct]").val('0.0000');
                $("[id*=TextBoxMarkupPct]").hide();
            }
            else {
                $("[id*=TextBoxMarkupDlr]").val('0.00');
                $("[id*=TextBoxMarkupPct]").val('0.0000');
                $("[id*=TextBoxMarkupDlr]").hide();
                $("[id*=TextBoxMarkupPct]").hide();
            }
        };

        function MarkupShowHideEdit() {
            var markupTypeE = $("[id*=DDLEMarkupType]").val();

            if (markupTypeE == "1")//Pct
            {
                $("[id*=TextBoxEMarkupPct]").show();
                $("[id*=TextBoxEMarkupDlr]").val('0.00');
                $("[id*=TextBoxEMarkupDlr]").hide();
                $("[id*=TextBoxENoMarkup]").hide();
            }
            else if (markupTypeE == "2")//Dlr
            {
                $("[id*=TextBoxEMarkupDlr]").show();
                $("[id*=TextBoxEMarkupPct]").val('0.0000');
                $("[id*=TextBoxEMarkupPct]").hide();
                $("[id*=TextBoxENoMarkup]").hide();
            }
            else {
                $("[id*=TextBoxENoMarkup]").show();
                $("[id*=TextBoxEMarkupDlr]").val('0.00');
                $("[id*=TextBoxEMarkupPct]").val('0.0000');
                $("[id*=TextBoxEMarkupDlr]").hide();
                $("[id*=TextBoxEMarkupPct]").hide();
            }
        };

        function MarkupShowHideFooter() {
            var markupTypeF = $("[id*=DDLFMarkupType]").val();

            if (markupTypeF == "1")//Pct
            {
                $("[id*=TextBoxFMarkupPct]").show();
                $("[id*=TextBoxFMarkupDlr]").val('0.00');
                $("[id*=TextBoxFMarkupDlr]").hide();
                $("[id*=TextBoxFNoMarkup]").hide();
            }
            else if (markupTypeF == "2")//Dlr
            {
                $("[id*=TextBoxFMarkupDlr]").show();
                $("[id*=TextBoxFMarkupPct]").val('0.0000');
                $("[id*=TextBoxFMarkupPct]").hide();
                $("[id*=TextBoxFNoMarkup]").hide();
            }
            else {
                $("[id*=TextBoxFMarkupDlr]").val('0.00');
                $("[id*=TextBoxFMarkupPct]").val('0.0000');
                $("[id*=TextBoxFMarkupDlr]").hide();
                $("[id*=TextBoxFMarkupPct]").hide();
                $("[id*=TextBoxFNoMarkup]").show();
            }
        };


        function RecalcTNGFringe() {
            var TNGFringe =
                Math.round((
                    (parseFloat($("[id*=TextBoxTNGLabor]").val()) * parseFloat($("[id*=TextBoxTNGFringePct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;

            if (TNGFringe == 0)
                $("[id$=TextBoxTNGFringe]").val('0.0000');
            else
                $("[id$=TextBoxTNGFringe]").val(TNGFringe.toFixed(2).toString());
        };

        function RecalcDistFringe() {
            var TNGFringe =
                Math.round((
                    (parseFloat($("[id*=TextBoxDistLabor]").val()) * parseFloat($("[id*=TextBoxDistFringePct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;

            if (TNGFringe == 0)
                $("[id$=TextBoxDistFringe]").val('0.0000');
            else
                $("[id$=TextBoxDistFringe]").val(TNGFringe.toFixed(2).toString());
        };


        function RecalcLineEditTotal() {
            var MarkedUpLineTotalE = parseFloat(0.00);
            var markupTypeE = $("[id*=DDLEMarkupType]").val();
            var MarkupAmountE = parseFloat(0.00);
            var MiscAmtE = parseFloat($("[id*=TextBoxEMiscAmount]").val());

            if (markupTypeE == "1")//%
            {
                MarkupAmountE = Math.round((
                    (MiscAmtE * parseFloat($("[id*=TextBoxEMarkupPct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;
            }
            else if (markupTypeE == "2")//$
            {
                MarkupAmountE = parseFloat($("[id*=TextBoxEMarkupDlr]").val());
            }

            MarkedUpLineTotalE = (MarkupAmountE + MiscAmtE) ;

            if (MarkedUpLineTotalE == 0)
                $("[id*=TextBoxEMiscAmtTotal]").val('0.00');
            else
                $("[id*=TextBoxEMiscAmtTotal]").val(MarkedUpLineTotalE.toFixed(2).toString());
        };

        function RecalcLineFooterTotal() {
            var MarkedUpLineTotalF = parseFloat(0.00);
            var markupTypeF = $("[id*=DDLFMarkupType]").val();
            var MarkupAmountF = parseFloat(0.00);
            var MiscAmtF = parseFloat($("[id*=TextBoxFMiscAmount]").val());

            if (markupTypeF == "1")//%
            {
                MarkupAmountF = Math.round((
                    (MiscAmtF * parseFloat($("[id*=TextBoxFMarkupPct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;
            }
            else if (markupTypeF == "2")//$
            {
                MarkupAmountF = parseFloat($("[id*=TextBoxFMarkupDlr]").val());
            }

            MarkedUpLineTotalF = (MarkupAmountF + MiscAmtF) ;

            if (MarkedUpLineTotalF == 0)
                $("[id*=TextBoxFMiscAmtTotal]").val('0.00');
            else
                $("[id*=TextBoxFMiscAmtTotal]").val(MarkedUpLineTotalF.toFixed(2).toString());
        };

        function RecalcInvoiceTotal() {
            var InvTotal =
                Math.round((
                    (parseFloat($("[id*=TextBoxFoodCost]").val()) + parseFloat($("[id*=TextBoxPaperCleanSup]").val()) +
                    parseFloat($("[id*=TextBoxMileage]").val()) + parseFloat($("[id*=TextBoxFSDCosts]").val()) +
                    parseFloat($("[id*=TextBoxTNGLabor]").val()) + parseFloat($("[id$=TextBoxTNGFringe]").val()) +
                    parseFloat($("[id*=TextBoxDistLabor]").val()) + parseFloat($("[id$=TextBoxDistFringe]").val()) +
                    parseFloat($("[id*=HFMiscAmountTotal]").val()))
                    * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100;

            var MarkedUpInvTotal = parseFloat(0.00);
            var markupType = $("[id*=DDLMarkupType]").val();
            var MarkupAmount = parseFloat(0.00);

            if (markupType == "1")//%
            {
                $("[id*=TextBoxMarkupPct]").show();
                MarkupAmount = Math.round((
                    (InvTotal * parseFloat($("[id*=TextBoxMarkupPct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;
                $("[id*=TextBoxMarkupDlr]").val('0.00');
                $("[id*=TextBoxMarkupDlr]").hide();
            }
            else if (markupType == "2")//$
            {
                $("[id*=TextBoxMarkupDlr]").show();
                MarkupAmount = parseFloat($("[id*=TextBoxMarkupDlr]").val());
                $("[id*=TextBoxMarkupPct]").val('0.0000');
                $("[id*=TextBoxMarkupPct]").hide();
            }
            else
            {
                $("[id*=TextBoxMarkupDlr]").val('0.00');
                $("[id*=TextBoxMarkupPct]").val('0.0000');
                $("[id*=TextBoxMarkupDlr]").hide();
                $("[id*=TextBoxMarkupPct]").hide();
            }

            MarkedUpInvTotal = InvTotal + MarkupAmount;


            if (MarkedUpInvTotal == 0)
                $("[id*=TextBoxInvoiceTotal]").val('0.00');
            else
                $("[id*=TextBoxInvoiceTotal]").val(MarkedUpInvTotal.toFixed(2).toString());
        };

        //apply highlight 
        $('body').on('focus', 'input,textarea,select', function (e) {
            $(this).addClass("highlight");
        });
        // remove highlight
        $('body').on('focusout', 'input,textarea,select', function (e) {
            $(this).removeClass("highlight");
        });
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
                <asp:HiddenField ID="HFcollapse1State" runat="server" Value="showcollapse1" />
                <asp:HiddenField ID="HFcollapse2State" runat="server" Value="showcollapse2" />

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
                            <div class="messagealert" id="alert_container"></div>
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
                                <div style="top:-10px;position:relative;text-align:center;">
                                    <asp:LinkButton ID="LinkButtonNewInvoice" runat="server" 
                                        ToolTip="Create New Invoice" ForeColor="Green"
                                        CssClass=" mGridLinkButton" Width="200" OnClick="ICreateClick" >
                                        <i class="fas fa-asterisk"></i> Create New Invoice
                                    </asp:LinkButton>
                                </div>
                                <asp:GridView ID="GridViewInvoices" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber" 
                                    DataSourceID="SqlDataSourceInvHdrsList" CssClass="mGrid" AllowPaging="true" PageSize="25"  PagerSettings-Mode="NumericFirstLast"  
                                    PagerStyle-HorizontalAlign="Center" PagerStyle-Font-Size="Large" PagerSettings-PageButtonCount="10" PagerStyle-Width="100" PagerStyle-CssClass="mGridPager"
                                    OnSelectedIndexChanged="GridViewInvoices_SelectedIndexChanged" 
                                    OnRowDataBound="GridviewClickableRowDatabound" OnPageIndexChanged="GridViewInvoices_PageIndexChanged"
                                    AllowSorting="true" >
                                    <Columns>
                                        <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" SortExpression="InvoiceNumberSort" ReadOnly="True" HeaderStyle-Width="150" />
                                        <asp:BoundField DataField="OrgBilled" HeaderText="Organization" SortExpression="OrgBilled" />
                                        <asp:BoundField DataField="FunctionDate" HeaderText="Function Date" SortExpression="FunctionDate" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="BillingDate" HeaderText="Posting Date" SortExpression="BillingDate" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="SpecialFunctionDesc" SortExpression="SpecialFunctionDesc" HeaderText="Special Function Description" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:HiddenField ID="HFInvoiceRowLocked" runat="server" Value='<%# Eval("Locked") %>' />
                                                <div style="width:60px;text-align:center;font-size:xx-small;font-weight:bold;">
                                                    <asp:Label runat="server" ID="LabelImported" Text="*LOCKED*" Visible='<%# (bool)Eval("Locked")==false ? false : true %>' ></asp:Label>
                                                </div>
                                                <div style="width:60px;text-align:center;">
                                                    <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete" Visible='<%# (bool)Eval("Locked")==false ? true : false %>'
                                                        CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                        OnClientClick="return confirm('Are you sure you want to delete this invoice?');" />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
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
                                            <div class="col-sm-4">
                                                <div class="messagealert" id="alert_container_InvoiceHdr"></div>
                                            </div>
                                            <div class="col-sm-4">
                                                <asp:HiddenField ID="HFInvoiceNumber" runat="server" />
                                                <asp:HiddenField ID="HFInvoiceSaved" runat="server" />
                                                <asp:HiddenField ID="HFInvoiceLocked" runat="server" />
                                                <asp:LinkButton ID="ISave" runat="server" 
                                                    ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                                    CssClass="fas fa-save mGridLinkButton" Width="150" OnClick="ISave_Click" ></asp:LinkButton>
                                                <asp:LinkButton ID="IPrint" Width="150" runat="server" 
                                                    class="fas fa-print mGridLinkButton" 
                                                    ToolTip="Print" Text=" Print Invoices" OnClick="IPrint_Click" ></asp:LinkButton>
                                            </div>
                                            <div class="col-sm-4">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-xl-1 col-sm-0 "></div>
                                            <div class="col-xl-10 col-sm-12 " style="padding-top:5px;border-style:solid;border-width:1px;border-color:dimgray;">
                                                <asp:FormView ID="FormViewSpFnOrg" runat="server" CssClass="mGridRecaps" DefaultMode="Edit" DataSourceID="SqlDataSourceInvHdr" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber"
                                                    OnPreRender="FormViewSpFnOrg_PreRender" OnDataBound="FormViewSpFnOrg_DataBound">
                                                    <EditItemTemplate>
                                                        <div class="container">
                                                            <div class="row">
                                                                <div class="col-sm-5">
                                                                    <asp:TextBox ID="TextBoxInvoice" runat="server" Text='<%# Bind("InvoiceNumber") %>' Enabled='<%# !(bool)Eval("Locked")%>' Width="150" OnTextChanged="TextBox_TextChanged" MaxLength="15"></asp:TextBox>&nbsp;Invoice No.
                                                                </div>
                                                                <div class="col-sm-2" style="text-align:center;color:red;">
                                                                    <asp:Label ID="LabelLocked" runat="server" Text="**Locked**" Visible='<%# (bool)Eval("Locked")%>' />
                                                                </div>
                                                                <div style="float:right;text-align:right;position:absolute;right:20px;">
                                                                    <asp:Image ID="ImageLogo" runat="server" ImageUrl="~/Images/InvoiceLogoSM.png" ImageAlign="Right" />
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-sm-12" style="text-align:center;">
                                                                    <h5>SPECIAL FUNCTION BILLING</h5>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-5">
                                                                    <asp:Label ID="LabelOrgBilled" runat="server" Text="ORGANIZATION BILLED:" Width="200"></asp:Label>
                                                                </div>
                                                                <div class="col-sm-7" style="text-align:right;">
                                                                    <asp:TextBox ID="TextBoxOrgBilled" runat="server" Text='<%# Bind("OrgBilled") %>' Width="100%" CssClass="longTextBox"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' MaxLength="50"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-5">
                                                                    &nbsp
                                                                </div>
                                                                <div class="col-sm-3">
                                                                    <asp:CheckBox ID="CB3086" runat="server" Checked='<%# Bind("Org3086") %>' OnCheckedChanged="SPFN_CheckedChanged" Enabled='<%# !(bool)Eval("Locked")%>' Text="3086" AutoPostBack="true" />
                                                                </div>
                                                                <div class="col-sm-3" >
                                                                    <asp:CheckBox ID="CBCACFP" runat="server" Checked='<%# Bind("OrgCACFP") %>' OnCheckedChanged="SPFN_CheckedChanged" Enabled='<%# !(bool)Eval("Locked")%>' Text="CACFP" AutoPostBack="true" />
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-5">
                                                                    <asp:Label ID="Label1" runat="server" Text="CONTACT PERSON:" Width="200"></asp:Label>
                                                                </div>
                                                                <div class="col-sm-7" style="text-align:right;">
                                                                    <asp:TextBox ID="TextBoxContactPerson" runat="server" Text='<%# Bind("OrgContact") %>' Width="100%" CssClass="longTextBox" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' MaxLength="50" ></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-5">
                                                                    <asp:Label ID="Label5" runat="server" Text="SPECIAL FUNCTION DESCRIPTION:" ></asp:Label>
                                                                </div>
                                                                <div class="col-sm-7" style="text-align:right;">
                                                                    <asp:TextBox ID="TextBoxSpecFuncDesc" runat="server" Text='<%# Bind("SpecialFunctionDesc") %>' CssClass="longTextBox" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' MaxLength="75"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-5">
                                                                    <asp:Label ID="Label2" runat="server" Text="FUNCTION DATE" Width="130"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxFunctionDate" runat="server" Text='<%# Bind("FunctionDate", "{0:d}") %>' Width="90" OnTextChanged="TextBox_TextChanged" Enabled='<%# (bool)Eval("Locked")?false:true %>' ></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-7" style="text-align:right;">
                                                                    <asp:Label ID="Label3" runat="server" Text="POSTING DATE" Width="110"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxBillingDate" runat="server" Text='<%# Bind("BillingDate", "{0:d}") %>' Width="90" OnTextChanged="TextBox_TextChanged" Enabled='<%# (bool)Eval("Locked")?false:true %>' ToolTip="Date invoice was posted to sales/recaps."  ></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-4">
                                                                    <asp:Label ID="Label6" runat="server" Text="PO Number" Width="130"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxPONumber" runat="server" Text='<%# Bind("PONumber") %>' Width="90" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' MaxLength="25"></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-4" style="text-align:right;">
                                                                    <asp:Label ID="Label8" runat="server" Text="PAYMENT NUMBER" Width="150"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxPaymentNumber" runat="server" Text='<%# Bind("PaymentNumber") %>' Width="90" OnTextChanged="TextBox_TextChanged" ToolTip="Check/Payment Number."  ></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-4" style="text-align:right;">
                                                                    <asp:Label ID="Label7" runat="server" Text="DATE PAID" Width="110"></asp:Label>
                                                                    <asp:TextBox ID="TextBoxDatePaid" runat="server" Text='<%# Bind("DatePaid", "{0:d}") %>' Width="90" OnTextChanged="TextBox_TextChanged" ToolTip="Date invoice amount was paid to cafeteria fund."  ></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;border-style:solid;border-width:2px 1px 1px 1px;border-color:darkslategrey;text-align:center;">
                                                                <div class="col-sm-12" >
                                                                    ITEMIZED COSTS
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-bottom:5px;border-style:solid;border-width:1px 1px 2px 1px;border-color:darkslategrey;">
                                                                <div class="col-sm-12" style="padding-top:5px;">
                                                                    <!-- Invoice Lines Here -->
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                Food Cost
                                                                            </div>
                                                                            <div class="col-sm-7" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxFoodCost" runat="server" Text='<%# Bind("FoodCost") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>                                                                         </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                Paper, Cleaning & Supplies
                                                                            </div>
                                                                            <div class="col-sm-7" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxPaperCleanSup" runat="server" Text='<%# Bind("PaperCleanSuppl") %>' Width="80" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>'  ></asp:TextBox>                                                                         </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                Mileage
                                                                            </div>
                                                                            <div class="col-sm-7" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxMileage" runat="server" Text='<%# Bind("Mileage") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>                                                                         </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                FSD Costs
                                                                            </div>
                                                                            <div class="col-sm-7" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxFSDCosts" runat="server" Text='<%# Bind("FSDCosts") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>                                                                         </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                TNG Labor excluding fringe
                                                                            </div>
                                                                            <div class="col-sm-7" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxTNGLabor" runat="server" Text='<%# Bind("TNGLabor") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>                                                                         </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-3">
                                                                                TNG Fringe
                                                                            </div>
                                                                            <div class="col-sm-2">
                                                                                <asp:TextBox ID="TextBoxTNGFringePct" runat="server" Text='<%# Bind("TNGFringePct") %>' Width="75"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>'></asp:TextBox>%
                                                                            </div>
                                                                            <div class="col-sm-7" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxTNGFringe" runat="server" Text='<%# Bind("TNGFringe", "{0:F2}") %>' Enabled="false" Width="80" ></asp:TextBox>                                                                         

                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                District Labor excluding fringe
                                                                            </div>
                                                                            <div class="col-sm-7" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxDistLabor" runat="server" Text='<%# Bind("DistrictLabor") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>                                                                         </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-3">
                                                                                District Fringe
                                                                            </div>
                                                                            <div class="col-sm-2">
                                                                                <asp:TextBox ID="TextBoxDistFringePct" runat="server" Text='<%# Bind("DistrictFringePct") %>' Width="75" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>%
                                                                            </div>
                                                                            <div class="col-sm-7" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxDistFringe" runat="server" Text='<%# Bind("DistrictFringe", "{0:F2}") %>' Enabled="false" Width="80" ></asp:TextBox>                                                                         </div>
                                                                        </div>
                                                                    <div class="row">
                                                                        <div class="col-sm-4" style="font-weight:bold;">
                                                                            Miscellaneous:
                                                                        </div>
                                                                        <div class="col-sm-2" style="text-align:right;font-weight:bold;">
                                                                            Cost
                                                                        </div>
                                                                        <div class="col-sm-3" style="text-align:right;font-weight:bold;">
                                                                            Dist Markup
                                                                        </div>
                                                                        <div class="col-sm-3" style="text-align:right;font-weight:bold;">
                                                                            Total
                                                                        </div>
                                                                    </div>
                                                                    <div class="row">
                                                                        <div class="col-sm-12">
                                                                            <asp:GridView ID="GridViewMiscLines" runat="server" DataSourceID="SqlDataSourceInvLines"  ShowHeader="false" CssClass="SpGrid"
                                                                                ShowFooter="True" AutoGenerateColumns="False" OnRowCommand="GridViewLines_RowCommand" OnRowDataBound="GridViewLines_RowDataBound" OnRowEditing="GridViewMiscLines_RowEditing"
                                                                                OnDataBound="GridViewMiscLines_DataBound"
                                                                                DataKeyNames="ProgramYear, DistrictID, InvoiceNumber, LineNumber">
                                                                                <Columns>
                                                                                    <asp:TemplateField HeaderText="MiscDescription" SortExpression="MiscDescription">
                                                                                        <EditItemTemplate>
                                                                                            <div class="row">
                                                                                                <div class="col-sm-4">
                                                                                                    
                                                                                                    <asp:TextBox ID="TextBoxEMiscDesc" runat="server" Text='<%# Bind("MiscDescription") %>' MaxLength="75" Width="300"></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-2" style="text-align:right;">
                                                                                                    <asp:TextBox ID="TextBoxEMiscAmount" runat="server" Text='<%# Bind("MiscAmount") %>' Width="80"></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-3" style="text-align:right;">
                                                                                                    <asp:DropDownList ID="DDLEMarkupType" runat="server" Enabled="true" SelectedValue='<%# Bind("MarkupType") %>' >
                                                                                                        <asp:ListItem Text="None" Value="0"></asp:ListItem>
                                                                                                        <asp:ListItem Text="%" Value="1"></asp:ListItem>
                                                                                                        <asp:ListItem Text="$" Value="2"></asp:ListItem>
                                                                                                    </asp:DropDownList>
                                                                                                    <asp:TextBox ID="TextBoxEMarkupPct" runat="server" Text='<%# Bind("MarkupPct", "{0:F4}") %>' Width="80" Enabled="true" ></asp:TextBox>
                                                                                                    <asp:TextBox ID="TextBoxEMarkupDlr" runat="server" Text='<%# Bind("MarkupDlr", "{0:F2}") %>' Width="80" Enabled="true" ></asp:TextBox>
                                                                                                    <asp:TextBox ID="TextBoxENoMarkup"  runat="server" Text=""                                   Width="80" Enabled="false" ></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-3" style="text-align:right;">
                                                                                                    <asp:TextBox ID="TextBoxEMiscAmtTotal" runat="server" Width="80" Text='<%# Eval("LineTotal", "{0:F2}") %>' Enabled="false"  ></asp:TextBox>
                                                                                                    <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                                                                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                                                                                    <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                                                                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                                                                                </div>
                                                                                            </div>
                                                                                            
                                                                                        </EditItemTemplate>
                                                                                        <ItemTemplate>
                                                                                            <div class="row">
                                                                                                <div class="col-sm-4">
                                                                                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("MiscDescription") %>'></asp:Label>
                                                                                                </div>
                                                                                                <div class="col-sm-2" style="text-align:right;">
                                                                                                    <asp:TextBox ID="TextBoxMiscAmount" runat="server" Text='<%# Eval("MiscAmount") %>' Width="80" Enabled="false"  ></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-3" style="text-align:right;">
                                                                                                    <asp:DropDownList ID="DDLIMarkupType" runat="server" Enabled="false" SelectedValue='<%# Eval("MarkupType") %>' >
                                                                                                        <asp:ListItem Text="None" Value="0"></asp:ListItem>
                                                                                                        <asp:ListItem Text="%" Value="1"></asp:ListItem>
                                                                                                        <asp:ListItem Text="$" Value="2"></asp:ListItem>
                                                                                                    </asp:DropDownList>
                                                                                                    <asp:TextBox ID="TextBoxIMarkupPct" runat="server" Text='<%# Eval("MarkupPct", "{0:F4}") %>' Width="80" Visible='<%# Eval("MarkupType").ToString()=="1"?true:false %>' Enabled="false" ></asp:TextBox>
                                                                                                    <asp:TextBox ID="TextBoxIMarkupDlr" runat="server" Text='<%# Eval("MarkupDlr", "{0:F2}") %>' Width="80" Visible='<%# Eval("MarkupType").ToString()=="2"?true:false %>' Enabled="false" ></asp:TextBox>
                                                                                                    <asp:TextBox ID="TextBoxINoMarkup"  runat="server" Text=""                         Width="80" Visible='<%# Eval("MarkupType").ToString()=="0"?true:false %>' Enabled="false" ></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-3" style="text-align:right;">
                                                                                                    <asp:TextBox ID="TextBoxIMiscAmtTotal" runat="server" Width="80" Text='<%# Eval("LineTotal", "{0:F2}") %>' Enabled="false"  ></asp:TextBox>
                                                                                                    <asp:LinkButton runat="server" CommandName="Edit" ID="IEdit" ToolTip="Edit" 
                                                                                                            CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  
                                                                                                            Visible='<%# (bool)Eval("Locked")?false:true %>' />
                                                                                                    <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                                                                            CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                                                                            OnClientClick="return confirm('Are you sure you want to delete this record?');" 
                                                                                                            Visible='<%# (bool)Eval("Locked")?false:true %>' />
                                                                                                </div>
                                                                                            </div>
                                                                                        </ItemTemplate>
                                                                                        <FooterTemplate>
                                                                                            <div class="row">
                                                                                                <div class="col-sm-4">
                                                                                                    Add New Misc. Amount<br />
                                                                                                    
                                                                                                    <asp:TextBox ID="TextBoxFMiscDesc" runat="server" MaxLength="75" ></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-2" style="text-align:right;">
                                                                                                    Cost
                                                                                                    <br />
                                                                                                    <asp:TextBox ID="TextBoxFMiscAmount" runat="server" Width="80" Text="0.00"  ></asp:TextBox>
                                                                                                    
                                                                                                </div>
                                                                                                <div class="col-sm-3" style="text-align:right;">
                                                                                                    Dist Markup
                                                                                                    <br />
                                                                                                    <asp:DropDownList ID="DDLFMarkupType" runat="server" >
                                                                                                        <asp:ListItem Text="None" Value="0" Selected="true"></asp:ListItem>
                                                                                                        <asp:ListItem Text="%" Value="1"></asp:ListItem>
                                                                                                        <asp:ListItem Text="$" Value="2"></asp:ListItem>
                                                                                                    </asp:DropDownList>
                                                                                                    <asp:TextBox ID="TextBoxFMarkupPct" runat="server" Text="0.0000" Width="80" ></asp:TextBox>
                                                                                                    <asp:TextBox ID="TextBoxFMarkupDlr" runat="server" Text="0.00" Width="80" ></asp:TextBox>
                                                                                                    <asp:TextBox ID="TextBoxFNoMarkup" runat="server" Width="80" Text="" Enabled="false" ></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-3" style="text-align:right;">
                                                                                                    Total
                                                                                                    <br />
                                                                                                    <asp:TextBox ID="TextBoxFMiscAmtTotal" runat="server" Width="80" Text="0.00" Enabled="false" ></asp:TextBox>
                                                                                                    <asp:LinkButton runat="server" CommandName="Insert" ID="LinkButton1" ToolTip="Add" 
                                                                                                         CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                                                                                    <asp:LinkButton runat="server" CommandName="Cancel" ID="LinkButton2" ToolTip="Cancel" 
                                                                                                         CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                                                                                </div>
                                                                                            </div>
                                                                                        </FooterTemplate>
                                                                                    </asp:TemplateField>
                                                                                </Columns>
                                                                            </asp:GridView>
                                                                        </div>
                                                                    </div>
                                                                    <!-- End of invoice Lines -->
                                                                </div>
                                                            </div>
                                                            <div class="row" >
                                                                <div class="col-sm-6">
                                                                    <div class="row">
                                                                        <div class="col-sm-3">
                                                                            Notes:
                                                                        </div>
                                                                        <div class="col-sm-9" >
                                                                            <asp:CheckBox ID="CBPrintNotes" runat="server" Text="Display on Invoice" Checked='<%# Bind("PrintNotes") %>' Enabled='<%# (bool)Eval("Locked")?false:true %>' />
                                                                        </div>
                                                                    </div>
                                                                    <asp:TextBox ID="TextBoxInvoiceNotes" runat="server" Rows="4" Text='<%# Bind("InvoiceNotes") %>' TextMode="MultiLine" MaxLength="250" Width="100%" OnTextChanged="TextBox_TextChanged" ReadOnly='<%# (bool)Eval("Locked")%>' ></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-6">
                                                                    <div class="row">&nbsp;</div>
                                                                    <div class="row">
                                                                        <div class="col-sm-6" style="padding-right:0px;" hidden="hidden">
                                                                            District Profit:&nbsp;
                                                                            <asp:DropDownList ID="DDLMarkupType" runat="server" SelectedValue='<%# Bind("MarkupType") %>' Enabled='<%# (bool)Eval("Locked")?false:true %>'>
                                                                                <asp:ListItem Text="None" Value="0"></asp:ListItem>
                                                                                <asp:ListItem Text="%" Value="1"></asp:ListItem>
                                                                                <asp:ListItem Text="$" Value="2"></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                        <div class="col-sm-6" style="text-align:right;" hidden="hidden">
                                                                            <asp:TextBox ID="TextBoxMarkupPct" runat="server" Text='<%# Eval("MarkupPct") %>' Width="130" OnTextChanged="TextBox_TextChanged" ReadOnly='<%# (bool)Eval("Locked")%>' ></asp:TextBox>
                                                                            
                                                                            <asp:TextBox ID="TextBoxMarkupDlr" runat="server" Text='<%# Eval("MarkupDlr") %>' Width="130" OnTextChanged="TextBox_TextChanged" ReadOnly='<%# (bool)Eval("Locked")%>' ></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                    <div class="row"><asp:HiddenField ID="HFMiscAmountTotal" runat="server" Value='<%# Eval("MiscAmountTotal") %>' /></div>
                                                                    <div class="row">
                                                                        <div class="col-sm-12" style="text-align:right;">
                                                                            <asp:Label ID="LabelAmtDue" runat="server" Text="AMOUNT DUE"></asp:Label>
                                                                            <asp:TextBox ID="TextBoxInvoiceTotal" runat="server" Text='<%# Eval("InvoiceTotal", "{0:C2}") %>' Enabled="false" Width="130"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                    
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-12" style="text-align:center;">
                                                                    <asp:Label ID="Label4" runat="server" Text="REMIT PAYMENT TO:"></asp:Label><br />
                                                                    <asp:TextBox ID="TextBoxDueTo" runat="server" Text='<%# Eval("OrgRemitTo") %>' Width="300" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' MaxLength="50" ></asp:TextBox>
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
                    SelectCommand="SELECT [ProgramYear],[DistrictID],[InvoiceNumber],[OrgBilled],[OrgContact],[FunctionDate],
                    CASE WHEN [BillingDate]='1/1/1900' THEN '' ELSE CONVERT(VARCHAR(10),BillingDate,101) END BillingDate,[SpecialFunctionDesc], [Locked],
                    CONVERT(INT, SUBSTRING(InvoiceNumber, PATINDEX('%[0-9]%', InvoiceNumber), LEN(InvoiceNumber))) InvoiceNumberSort
                    FROM [SPFNInvoiceHDR] Hdr
                    WHERE ((Hdr.[ProgramYear] = @ProgramYear) AND (Hdr.[DistrictID] = @DistrictID)) ORDER BY Hdr.FunctionDate DESC, [InvoiceNumber] DESC"
                 DeleteCommand="DELETE [SPFNInvoiceHDR] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber]=@InvoiceNumber));
                    DELETE [SPFNInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber]=@InvoiceNumber))"
                     OnDeleted="SqlDataSourceInvHdrsList_Deleted">
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>

                <asp:SqlDataSource ID="SqlDataSourceInvHdr" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_"
                    SelectCommand="SELECT [ProgramYear],[DistrictID],[InvoiceNumber], [PONumber], CASE WHEN [DatePaid]='1/1/1900' THEN '' ELSE CONVERT(VARCHAR(10),DatePaid,101) END DatePaid
                    ,[OrgBilled],[OrgContact],[FunctionDate],CASE WHEN [BillingDate]='1/1/1900' THEN '' ELSE CONVERT(VARCHAR(10),BillingDate,101) END BillingDate
                    ,[OrgRemitTo],[SpecialFunctionDesc]
                    ,[FoodCost],[PaperCleanSuppl],[Mileage],[FSDCosts],[TNGLabor],[TNGFringePct],[DistrictLabor],[DistrictFringePct],[MarkupType],[MarkupDlr],[MarkupPct],[InvoiceNotes],[PrintNotes]
                    ,0 InvoiceTotal, ROUND([TNGLabor]*[TNGFringePct]*0.01,2) TNGFringe, ROUND([DistrictLabor]*[DistrictFringePct]*0.01,2) DistrictFringe
                    ,(SELECT ISNULL(SUM(  MiscAmount + IIF(MarkupType=0, 0, IIF(MarkupType=1, MiscAmount*MarkupPct*0.01, MarkupDlr))   ),0) FROM [SPFNInvoiceLines] WHERE  (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber))) MiscAmountTotal
                    , [Locked], [SchoolNum],[Org3086],[OrgCACFP],[PaymentNumber]
                    FROM [SPFNInvoiceHDR] 
                    WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber)) ">
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:ControlParameter Name="InvoiceNumber" ControlID="HFInvoicenumber" PropertyName="Value" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceInvLines" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT LINE.[ProgramYear],LINE.[DistrictID],LINE.[InvoiceNumber],LINE.[LineNumber],LINE.[MiscDescription],LINE.[MiscAmount]
                    ,LINE.[MarkupType], LINE.[MarkupDlr], LINE.[MarkupPct], HDR.[Locked]
                    ,LINE.[MiscAmount] + IIF(LINE.MarkupType=0, 0, IIF(LINE.MarkupType=1, LINE.MarkupPct*LINE.MiscAmount*0.01, LINE.MarkupDlr)) LineTotal
                    FROM [SPFNInvoiceLines] LINE
                    INNER JOIN [SPFNInvoiceHDR] HDR ON HDR.[ProgramYear]=LINE.[ProgramYear] AND HDR.[DistrictID]=LINE.[DistrictID] AND HDR.[InvoiceNumber]=LINE.[InvoiceNumber]
                    WHERE ((LINE.[ProgramYear] = @ProgramYear) AND (LINE.[DistrictID] = @DistrictID) AND (LINE.[InvoiceNumber] = @InvoiceNumber))
                    UNION ALL
                    SELECT '' [ProgramYear],'' [DistrictID],'' [InvoiceNumber],0 [LineNumber],'' [MiscDescription],0.00 [MiscAmount]
                    ,0 MarkupType, 0 MarkupDlr, 0 MarkupPct, HDR.Locked, 0 LineTotal
                    FROM [SPFNInvoiceHDR] HDR
                    WHERE ((HDR.[ProgramYear] = @ProgramYear) AND (HDR.[DistrictID] = @DistrictID) AND (HDR.[InvoiceNumber] = @InvoiceNumber))"
                    
                    DeleteCommand="DELETE [SPFNInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) 
                    AND ([LineNumber] = @LineNumber))"
                    
                    InsertCommand="INSERT INTO [SPFNInvoiceLines] ([ProgramYear],[DistrictID],[InvoiceNumber],[LineNumber],[MiscDescription],[MiscAmount],[MarkupType],[MarkupPct],[MarkupDlr]) 
                    VALUES(@ProgramYear,@DistrictID,@InvoiceNumber,
                    (SELECT ISNULL(MAX([LineNumber]),0)+100 FROM [SPFNInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber)))
                    ,@MiscDescription,@MiscAmount,@MarkupType,@MarkupPct,@MarkupDlr)"
                    
                    UpdateCommand="UPDATE [SPFNInvoiceLines] SET [MiscDescription]=@MiscDescription, [MiscAmount]=@MiscAmount, [MarkupType]=@MarkupType, [MarkupPct]=@MarkupPct, [MarkupDlr]=@MarkupDlr
                    WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) 
                    AND ([LineNumber] = @LineNumber))"
                    
                    OnInserting="SqlDataSourceInvLines_Inserting" OnInserted="SqlDataSourceInvLines_Inserted" OnDeleted="SqlDataSourceInvLines_Deleted" OnUpdated="SqlDataSourceInvLines_Updated" >
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:ControlParameter Name="InvoiceNumber" ControlID="HFInvoicenumber" PropertyName="Value" Type="String" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="InvoiceNumber" Type="String" />
                        <asp:Parameter Name="LineNumber" Type="Int32" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="InvoiceNumber" Type="String" />
                        <asp:Parameter Name="MiscDescription" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="MiscAmount" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="MarkupType" Type="Single" DefaultValue="0" />
                        <asp:Parameter Name="MarkupPct" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="MarkupDlr" Type="Decimal" DefaultValue="0" />
                    </InsertParameters>
                    <UpdateParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="InvoiceNumber" Type="String" />
                        <asp:Parameter Name="MiscDescription" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="MiscAmount" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="MarkupType" Type="Int16" DefaultValue="0" />
                        <asp:Parameter Name="MarkupPct" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="MarkupDlr" Type="Decimal" DefaultValue="0" />
                    </UpdateParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
