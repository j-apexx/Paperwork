<%@ Page Title="Cost Per Meal Invoices" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CostPerMealInvoices.aspx.cs" Inherits="Paperwork.CPMInvoices" EnableEventValidation="false" %>
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
            var PYearStart = $("[id$=HFProgYearStartDate]").val();
            var PYearEnd = $("[id$=HFProgYearEndDate]").val();

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            $(function () {
                $("[id$=TextBoxNewInvPerStart]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id$=TextBoxNewInvPerEnd]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id$=TextBoxPeriodStart]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id$=TextBoxPeriodEnd]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id$=TextBoxSpFnStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id$=TextBoxSpFnEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id$=TextBoxCovidStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id$=TextBoxCovidEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $("[id$=TextBoxPeriodEnd]").change(function () {
                $("[id$=TextBoxDate]").val($(this).val());
                
            })

            //Specialized Date Picker
            $(function () {
                $('[id$=TextBoxYearmonth]').datepicker(
                {
                    dateFormat: "yy-mm",
                    changeMonth: true,
                    changeYear: true,
                    showButtonPanel: true,
                    onClose: function (dateText, inst) {
                        inst.dpDiv.removeClass("month_year_datepicker");
                        function isDonePressed() {
                            return ($('#ui-datepicker-div').html().indexOf('ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all ui-state-hover') > -1);
                        }

                        if (isDonePressed()) {
                            var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                            var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                            $(this).datepicker('setDate', new Date(year, month, 1)).trigger('change');

                            $('.date-picker').focusout()//Added to remove focus from datepicker input box on selecting date
                        }
                    },
                    beforeShow: function (input, inst) {
                        inst.dpDiv.addClass("month_year_datepicker");

                        if ((datestr = $(this).val()).length > 0) {
                            year = datestr.substring(0, 4);
                            month = datestr.substring(datestr.length - 2, datestr.length);
                            $(this).datepicker('option', 'defaultDate', new Date(year, month - 1, 1));
                            $(this).datepicker('setDate', new Date(year, month - 1, 1));
                        }
                    }
                })
            });
            
            
            $("[id$=TextBoxBfastCount]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcBfast();
            })

            $("[id$=TextBoxLunchCount]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcLunch();
            })

            $("[id$=TextBoxAlaSales]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcAla();
            })

            $("[id$=TextBoxSpFnSales]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcTotal();
            })

            $("[id$=TextBoxUSDACommUsage],[id$=TextBoxUSDACommRcvd]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcUSDA();
            })

            $("[id$=TextBoxIncome], [id$=TextBoxLabor], [id$=TextBoxCosts]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
            })

            $("[id$=TextBoxSnackCount]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcSnack();
            })

            $("[id$=TextBoxDinnerCount]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcDinner();
            })
        }

        function RecalcBfast() {
            var BEquivs = 0;

            if (parseFloat($("[id$=LabelEquivRateB]").html()) > 0)
            {
                BEquivs = (
                parseFloat($("[id$=TextBoxBfastCount]").val())
                / parseFloat($("[id$=LabelEquivRateB]").html())
                ) ;
            }

            if (BEquivs == 0)
                $("[id$=LabelEquivMealCountB]").html('0.0000');
            else
                $("[id$=LabelEquivMealCountB]").html(BEquivs.toFixed(4).toString());

            var BfastTotal =
                Math.round(((
                    BEquivs
                    * parseFloat($("[id$=LabelMealRateB]").html())
                ) * 10000)) / 10000;//rounding works with integers, so I need to mult by 10000, round, then divide by 10000

            if (BfastTotal == 0)
                $("[id$=LabelBfastAmount]").html('0.0000');
            else
                $("[id$=LabelBfastAmount]").html(BfastTotal.toFixed(4).toString());

            RecalcTotal();
        };

        function RecalcLunch() {
            $("[id$=LabelEquivMealCountL]").html(parseFloat($("[id$=TextBoxLunchCount]").val()).toFixed(4).toString());

            var LunchTotal =
                Math.round(((
                    parseFloat($("[id$=TextBoxLunchCount]").val())
                    * parseFloat($("[id$=LabelMealRateL]").html())
                ) * 10000)) / 10000;//rounding works with integers, so I need to mult by 10000, round, then divide by 10000

            if (LunchTotal == 0)
                $("[id$=LabelLunchAmount]").html('0.0000');
            else
                $("[id$=LabelLunchAmount]").html(LunchTotal.toFixed(4).toString());

            RecalcTotal();
        };

        function RecalcAla() {
            
            var AlaTotal;
            if (parseFloat($("[id$=LabelEquivRate]").html()) == 0) {
                AlaTotal = 0;
                $("[id$=LabelEquivMealCount]").html('0.0000');
            }
            else {
                var MealCountAla = Math.round(((
                    (parseFloat($("[id$=TextBoxAlaSales]").val()) / parseFloat($("[id$=LabelEquivRate]").html()))
                ) * 10000)) / 10000;

                $("[id$=LabelEquivMealCount]").html(MealCountAla.toFixed(4).toString());

                AlaTotal =  Math.round(((
                    MealCountAla
                    * parseFloat($("[id$=LabelMealRateA]").html())
                ) * 10000)) / 10000;//rounding works with integers, so I need to mult by 10000, round, then divide by 10000
            }

            if (AlaTotal == 0)
                $("[id$=LabelAlaAmount]").html('0.0000');
            else
                $("[id$=LabelAlaAmount]").html(AlaTotal.toFixed(4).toString());

            RecalcTotal();
        };

        function RecalcSnack() {
            var SEquivs = 0;

            if (parseFloat($("[id$=LabelEquivRateS]").html()) > 0) {
                SEquivs = (
                    parseFloat($("[id$=TextBoxSnackCount]").val())
                    / parseFloat($("[id$=LabelEquivRateS]").html())
                );
            }

            if (SEquivs == 0)
                $("[id$=LabelEquivMealCountS]").html('0.0000');
            else
                $("[id$=LabelEquivMealCountS]").html(SEquivs.toFixed(4).toString());

            var SnackTotal =
                Math.round(((
                    SEquivs
                    * parseFloat($("[id$=LabelMealRateS]").html())
                ) * 10000)) / 10000;//rounding works with integers, so I need to mult by 10000, round, then divide by 10000

            if (SnackTotal == 0)
                $("[id$=LabelSnackAmount]").html('0.0000');
            else
                $("[id$=LabelSnackAmount]").html(SnackTotal.toFixed(4).toString());

            RecalcTotal();
        };

        function RecalcDinner() {
            var DEquivs = 0;

            if (parseFloat($("[id$=LabelEquivRateD]").html()) > 0) {
                DEquivs = (
                    parseFloat($("[id$=TextBoxDinnerCount]").val())
                    / parseFloat($("[id$=LabelEquivRateD]").html())
                );
            }

            if (DEquivs == 0)
                $("[id$=LabelEquivMealCountD]").html('0.0000');
            else
                $("[id$=LabelEquivMealCountD]").html(DEquivs.toFixed(4).toString());

            var DinnerTotal =
                Math.round(((
                    DEquivs
                    * parseFloat($("[id$=LabelMealRateD]").html())
                ) * 10000)) / 10000;//rounding works with integers, so I need to mult by 10000, round, then divide by 10000

            if (DinnerTotal == 0)
                $("[id$=LabelDinnerAmount]").html('0.0000');
            else
                $("[id$=LabelDinnerAmount]").html(DinnerTotal.toFixed(4).toString());

            RecalcTotal();
        };

        function RecalcUSDA() {
            var USDARcvd = parseFloat($("[id$=TextBoxUSDACommRcvd]").val());

            if (USDARcvd == 0)
                $("[id$=LabelUSDACommRcvd]").html('0.00');
            else
                $("[id$=LabelUSDACommRcvd]").html(USDARcvd.toFixed(2).toString());

            RecalcTotal();
        }

        function RecalcTotal() {
            //Handle hidden rows which result in NAN values
            var Snack = parseFloat($("[id$=LabelSnackAmount]").html());
            if (isNaN(Snack))
                Snack = 0.00;

            var Dinner = parseFloat($("[id$=LabelDinnerAmount]").html());
            if (isNaN(Dinner))
                Dinner = 0.00;

            var SpFn = parseFloat($("[id$=TextBoxSpFnSales]").val());
            if (isNaN(SpFn))
                SpFn = 0.00;

            var HFLines = parseFloat($("[id$=HFLinesTotal]").val());
            if (isNaN(HFLines))
                HFLines = 0.00;


            var OverallTotal = Math.round(((
                parseFloat($("[id$=LabelBfastAmount]").html())
                + parseFloat($("[id$=LabelLunchAmount]").html())
                + parseFloat($("[id$=LabelAlaAmount]").html())
                + Snack
                + Dinner
                + SpFn
                + parseFloat($("[id$=LabelUSDACommRcvd]").html())
                + HFLines
                
            ) * 100)) / 100;

            
            if (OverallTotal == 0)
                $("[id$=TextBoxTotal]").html('0.00');
            else
                $("[id$=TextBoxTotal]").html(OverallTotal.toFixed(2).toString());
        };

        function RecalcAll() {
            RecalcBfast();
            RecalcLunch();
            RecalcAla();
            RecalcUSDA();
            RecalcSnack();
            RecalcDinner();
            RecalcTotal();
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
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanelTop" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanelNewInv" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanelExistingInv" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="UpdatePanelSPFN" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress5" runat="server" AssociatedUpdatePanelID="UpdatePanelCovid" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="DataEntry">
        <asp:UpdatePanel ID="OverallPanel" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>

                <asp:UpdatePanel ID="UpdatePanelTop" runat="server" UpdateMode="Always">
                    <ContentTemplate>        
                        <div id="topSection" style="position:relative;" class="row">
                            <div id="hdrDiv"  class="col-sm-4">
                                <h2><%: Title %></h2>
                            </div>
                            <div id="menuDiv" class="col-sm-8">
                                <div class="row">
                                    <div class="col-sm-4">
                                        <strong>Program Year:</strong>
                                    </div>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                            DataValueField="ProgramYear" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                        <asp:HiddenField ID="HFProgYearStartDate" runat="server" />
                                        <asp:HiddenField ID="HFProgYearEndDate" runat="server" />
                                        <asp:HiddenField ID="HFBudgetYear" runat="server" />
                                        <asp:HiddenField id="hdnScrollPos" runat="server" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-4">
                                        District:
                                    </div>
                                    <div class="col-sm-6">
                                        <asp:DropDownList ID="DDLDistricts" runat="server" DataSourceID="SqlDataSourceDistricts" DataTextField="Desc" 
                                            DataValueField="DistrictID" AutoPostBack="true" OnSelectedIndexChanged="DDLDistricts_SelectedIndexChanged" >
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <div id="accordian">
                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse1" >
                                New CPM Invoice
                            </a>
                        </div>
                        <div id="cardCollapse1" class="collapse hide">
                            <div class="card-body">
                                <asp:UpdatePanel ID="UpdatePanelNewInv" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <p>
                                            Period Start:
                                            <asp:TextBox ID="TextBoxNewInvPerStart" runat="server" Width="100" OnTextChanged="TextBoxDateField_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorPeriodStart" runat="server" ControlToValidate="TextBoxNewInvPerStart" Display="Dynamic" ErrorMessage="Start Date is Required" ForeColor="Red" ValidationGroup="NewInvoice">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorPerStart" runat="server" ControlToValidate="TextBoxNewInvPerStart" Display="Dynamic" ErrorMessage="Start Date format invalid" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="NewInvoice" >*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                            <br />

                                            Period End / Statement Date:
                                            <asp:TextBox ID="TextBoxNewInvPerEnd" runat="server" Width="100" OnTextChanged="TextBoxDateField_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorEDate" runat="server" ControlToValidate="TextBoxNewInvPerEnd" Display="Dynamic" ErrorMessage="Statement Date is Required" ForeColor="Red" ValidationGroup="NewInvoice">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorED" runat="server" ControlToValidate="TextBoxNewInvPerEnd" Display="Dynamic" ErrorMessage="Statement Date format invalid" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="NewInvoice" >*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                            <br />
                                            Select District:
                                            <asp:DropDownList ID="DropDownListNewInvDist" runat="server" DataSourceID="SqlDataSourceInvDists" DataTextField="Desc" 
                                                DataValueField="DistrictID"  >
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredContract" runat="server" ControlToValidate="DropDownListNewInvDist" Display="Dynamic" ValidationGroup="NewInvoice" ErrorMessage="District is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                            <br />
                                            <asp:LinkButton ID="ICreate" runat="server" CausesValidation="True" ValidationGroup="NewInvoice"
                                                                ToolTip="Create Invoice" Text=" Create Invoice"
                                                                CssClass="fas fa-cogs mGridLinkButton" Width="200" OnClick="ICreate_Click" />
                                        </p>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanelExistingInv" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <div class="card">
                                <div class="card-header h5">
                                    <a class="card-link" data-toggle="collapse" href="#cardCollapse2">
                                        Existing CPM Invoices
                                    </a>
                                </div>
                                <div id="cardCollapse2" class="collapse show">
                                    <div class="card-body">
                                        <asp:GridView ID="GridViewInvoiceList" runat="server" AutoGenerateColumns="False" CssClass="mGrid" 
                                            DataKeyNames="InvoiceNumber" DataSourceID="SqlDataSourceInvoiceList" Width="600" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                            AllowPaging="true" PageSize="10" PagerSettings-Mode="NumericFirstLast" PagerStyle-HorizontalAlign="Center" PagerStyle-Font-Size="Large" 
                                            PagerSettings-PageButtonCount="10" PagerStyle-Width="100" PagerStyle-CssClass="mGridPager" 
                                            OnSelectedIndexChanged="GridViewInvoiceList_SelectedIndexChanged" OnRowDataBound="GridViewInvoiceList_RowDataBound" 
                                            OnPageIndexChanged="GridViewInvoiceList_PageIndexChanged" AllowSorting="true" OnSorted="GridViewInvoiceList_Sorted"
                                            OnPreRender="GridViewInvoiceList_PreRender" OnDataBound="GridViewInvoiceList_DataBound"
                                            EmptyDataText="No Invoices Found.">
                                            <Columns>
                                                <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" ReadOnly="True" SortExpression="InvoiceNumber" />
                                                <asp:BoundField DataField="DistDisplayName" HeaderText="District" SortExpression="DistrictID" />
                                                <asp:BoundField DataField="PeriodEnd" HeaderText="Statement Date" SortExpression="PeriodEnd" DataFormatString="{0:d}" />
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <div style="width:60px;text-align:center;" >
                                                            <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                                CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                                OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>                    
                    <asp:UpdatePanel ID="UpdatePanelEditInvoice" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>        
                            <div class="card">
                                <div class="card-header h5">
                                    <a class="card-link" data-toggle="collapse" href="#cardCollapse3">
                                        Invoice Details
                                    </a>
                                </div>
                                <div id="cardCollapse3" class="collapse show">
                                    <div class="card-body">
                                        <div class="row" style="text-align:center;position:relative;top:-10px;">
                                            <div class="col-sm-3">
                                                <div class="messagealert" id="alert_container_InvoiceHdr"></div>
                                            </div>
                                            <div class="col-sm-6">
                                                <asp:HiddenField ID="HFInvoiceNumber" runat="server" />
                                                <asp:HiddenField ID="HFInvoiceSaved" runat="server" />
                                                <asp:HiddenField ID="HFInvoiceLocked" runat="server" />
                                                <asp:LinkButton ID="ISave" runat="server" 
                                                    ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                                    CssClass="fas fa-save mGridLinkButton" Width="140" OnClick="ISave_Click" ></asp:LinkButton>
                                                <asp:LinkButton ID="IPrint" Width="140" runat="server" 
                                                    class="fas fa-print mGridLinkButton" 
                                                    ToolTip="Print" Text=" Print Invoices" OnClick="IPrint_Click" ></asp:LinkButton>
                                                <asp:LinkButton ID="IPrintPL" Width="200" runat="server" 
                                                    class="fas fa-print mGridLinkButton" 
                                                    ToolTip="Print P&L" Text=" Print P&L Statement" OnClick="IPrintPL_Click" ></asp:LinkButton>
                                            </div>
                                            <div class="col-sm-3">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-xl-1 col-sm-0 ">&nbsp;</div>
                                            <div class="col-xl-10 col-sm-12 " style="padding-top:5px;border-style:solid;border-width:1px;border-color:dimgray;">
                                                <asp:FormView ID="FormViewCPMInvoice" runat="server" CssClass="mGridRecaps" DefaultMode="Edit" DataSourceID="SqlDataSourceInvDetails" DataKeyNames="InvoiceNumber" 
                                                    OnDataBound="FormViewCPMInvoice_DataBound">
                                                    <EditItemTemplate>
                                                        <div class="container">
                                                            <div class="row">
                                                                <div class="col-sm-2" style="text-align:right;">To:</div>
                                                                <div class="col-sm-4" >
                                                                    <asp:Label ID="LabelDistName" runat="server" Text='<%# Bind("DistrictName") %>'></asp:Label>
                                                                    <asp:HiddenField ID="HFInvoiceDistrictID" runat="server" Value='<%# Bind("DistrictID") %>' />
                                                                    <asp:HiddenField ID="HFInvoiceProgYear" runat="server" Value='<%# Bind("ProgramYear") %>' />
                                                                    <asp:HiddenField ID="HFShowMEOverrides" runat="server" Value='<%# Bind("ShowOverrideMERates") %>' />
                                                                </div>
                                                                <div class="col-sm-3" style="text-align:right;" >Statement Date:</div>
                                                                <div class="col-sm-3" ><asp:TextBox ID="TextBoxDate" runat="server" Text='<%# Bind("PeriodEnd","{0:d}") %>' Width="100" Enabled="false"></asp:TextBox></div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-sm-2">&nbsp;</div>
                                                                <div class="col-sm-4">
                                                                    <asp:Label ID="LabelDistAddress1" runat="server" Text='<%# Bind("DistrictAddress1") %>' Visible='<%# Eval("DistrictAddress1")==""?false:true %>'></asp:Label>
                                                                    <asp:Literal ID="break2" runat="server" Text="<br />" Visible='<%# Eval("DistrictAddress2")==""?false:true %>'></asp:Literal>
                                                                    <asp:Label ID="LabelDistAddress2" runat="server" Text='<%# Bind("DistrictAddress2") %>' Visible='<%# Eval("DistrictAddress2")==""?false:true %>'></asp:Label>
                                                                    <asp:Literal ID="break3" runat="server" Text="<br />" Visible='<%# Eval("DistrictCity")=="" && Eval("DistrictState")=="" && Eval("DistrictZip")==""?false:true %>'></asp:Literal>
                                                                    <asp:Label ID="LabelCity" runat="server" Text='<%# Bind("DistrictCity") %>' Visible='<%# Eval("DistrictCity")==""?false:true %>'></asp:Label>
                                                                    <asp:Literal ID="LiteralComma1" runat="server" Text=", " Visible='<%# Eval("DistrictCity")=="" && Eval("DistrictState")==""?false:true %>'></asp:Literal>
                                                                    <asp:Label ID="LabelState" runat="server" Text='<%# Bind("DistrictState") %>' Visible='<%# Eval("DistrictState")==""?false:true %>'></asp:Label>&nbsp;
                                                                    <asp:Label ID="LabelZip" runat="server" Text='<%# Bind("DistrictZip") %>' Visible='<%# Eval("DistrictZip")==""?false:true %>'></asp:Label>
                                                                </div>
                                                                <div class="col-sm-3" style="text-align:right;" >Statement Number:</div>
                                                                <div class="col-sm-3">
                                                                    <asp:TextBox ID="TextBoxInvoice" runat="server" Text='<%# Bind("InvoiceNumber") %>' Width="100" Enabled="false" MaxLength="15"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-sm-2" style="text-align:right;">Period:</div>
                                                                <div class="col-sm-10" >
                                                                    <asp:TextBox ID="TextBoxPeriodStart" runat="server" Text='<%# Bind("PeriodStart", "{0:d}") %>' Width="100" OnTextChanged="TextBoxDateField_TextChanged" AutoPostBack="true"></asp:TextBox> to 
                                                                    <asp:TextBox ID="TextBoxPeriodEnd" runat="server" Text='<%# Bind("PeriodEnd", "{0:d}") %>' Width="100" OnTextChanged="TextBoxDateField_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col">
                                                                    &nbsp;
                                                                </div>
                                                            </div>
                                                            <div class="row" style="border-bottom:solid;border-bottom-color:gray;border-bottom-width:2px;text-align:right;">
                                                                <div class="col-sm-2" style="text-align:left;">Detail:</div>
                                                                <div class="col-sm-2">Sales /<br />Entered Meals</div>
                                                                <div class="col-sm-2">Equivalent<br />Rate</div>
                                                                <div class="col-sm-2">Meal<br />Count</div>
                                                                <div class="col-sm-2">Meal<br />Rate</div>
                                                                <div class="col-sm-2">Billing<br />Amount</div>
                                                            </div>
                                                            <div class="row" style="text-align:right;">
                                                                <div class="col-sm-2" style="text-align:left;">Breakfast</div>
                                                                <div class="col-sm-2"><asp:TextBox ID="TextBoxBfastCount" runat="server" Text='<%# Bind("BreakfastTotal") %>' Width="75" ></asp:TextBox></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelEquivRateB" runat="server" Text='<%# Bind("BrEquivRateCalc", "{0:F4}") %>'></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelEquivMealCountB" runat="server" Text="0.0000"></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelMealRateB" runat="server" Text='<%# Eval("CPMRate","{0:F4}") %>'></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelBfastAmount" runat="server" Text="0.0000"></asp:Label></div>
                                                            </div>

                                                            <div class="row" style="text-align:right;">
                                                                <div class="col-sm-2" style="text-align:left;">Lunch</div>
                                                                <div class="col-sm-2"><asp:TextBox ID="TextBoxLunchCount" runat="server" Text='<%# Bind("LunchTotal") %>' Width="75" ></asp:TextBox></div>
                                                                <div class="col-sm-2">&nbsp;</div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelEquivMealCountL" runat="server" Text="0.0000"></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelMealRateL" runat="server" Text='<%# Eval("CPMRate","{0:F4}") %>'></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelLunchAmount" runat="server" Text="0.0000"></asp:Label></div>
                                                            </div>

                                                            <div class="row" style="text-align:right;">
                                                                <div class="col-sm-2" style="text-align:left;">Ala Carte Sales</div>
                                                                <div class="col-sm-2"><asp:TextBox ID="TextBoxAlaSales" runat="server" Text='<%# Bind("AlaSales", "{0:F2}") %>' Width="75" ></asp:TextBox></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelEquivRate" runat="server" Text='<%# Bind("ACEquivRate", "{0:F4}") %>'></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelEquivMealCount" runat="server" Text="0.0000"></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelMealRateA" runat="server" Text='<%# Eval("CPMRate","{0:F4}") %>'></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelAlaAmount" runat="server" Text="0.0000"></asp:Label></div>
                                                            </div>

                                                            <div class="row" style="text-align:right;" runat="server" id="DivSnackRow">
                                                                <div class="col-sm-2" style="text-align:left;">Snack</div>
                                                                <div class="col-sm-2"><asp:TextBox ID="TextBoxSnackCount" runat="server" Text='<%# Bind("SnackTotal") %>' Width="75" ></asp:TextBox></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelEquivRateS" runat="server" Text='<%# Bind("SnEquivRateCalc", "{0:F4}") %>'></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelEquivMealCountS" runat="server" Text="0.0000"></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelMealRateS" runat="server" Text='<%# Eval("CPMRate","{0:F4}") %>'></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelSnackAmount" runat="server" Text="0.0000"></asp:Label></div>
                                                            </div>

                                                            <div class="row" style="text-align:right;" runat="server" id="DivDinnerRow">
                                                                <div class="col-sm-2" style="text-align:left;">Dinner</div>
                                                                <div class="col-sm-2"><asp:TextBox ID="TextBoxDinnerCount" runat="server" Text='<%# Bind("DinnerTotal") %>' Width="75" ></asp:TextBox></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelEquivRateD" runat="server" Text='<%# Bind("DnEquivRateCalc", "{0:F4}") %>'></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelEquivMealCountD" runat="server" Text="0.0000"></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelMealRateD" runat="server" Text='<%# Eval("CPMRate","{0:F4}") %>'></asp:Label></div>
                                                                <div class="col-sm-2"><asp:Label ID="LabelDinnerAmount" runat="server" Text="0.0000"></asp:Label></div>
                                                            </div>




                                                            <div class="row" style="text-align:right;" id="divSpecFuncSales" runat="server">
                                                                <div class="col-sm-3" style="text-align:left;">Special Function Sales</div>
                                                                <div class="col-sm-7">&nbsp;</div>
                                                                <div class="col-sm-2"><asp:TextBox ID="TextBoxSpFnSales" runat="server" Text='<%# Bind("SpecialFunctionSales", "{0:F2}") %>' Width="75" ></asp:TextBox></div>
                                                            </div>
                                                            <div class="row" style="text-align:right;">
                                                                <div class="col-sm-2" style="text-align:left;">Other Sales:</div>
                                                            </div>
                                                            <div class="row" style="text-align:right;">
                                                                <div class="col-sm-12" style="text-align:left;padding-left:30px;">
                                                                    <asp:GridView ID="GridViewOtherSales" runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataKeyNames="InvoiceNumber,InvoiceLineNumber" 
                                                                        DataSourceID="SqlDataSourceInvoiceLines" ShowHeader="false" OnRowEditing="GridViewOtherSales_RowEditing" OnRowCancelingEdit="GridViewOtherSales_RowCancelingEdit"
                                                                        OnRowDeleted="GridViewOtherSales_RowDeleted" OnRowUpdated="GridViewOtherSales_RowUpdated">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Description" SortExpression="OtherSalesDesc">
                                                                                <EditItemTemplate>
                                                                                    <div class="row" style="text-align:right;">
                                                                                        <div class="col-sm-8" style="text-align:left;">
                                                                                            <asp:TextBox ID="TextBoxEOtherSalesDesc" runat="server" Text='<%# Bind("OtherSalesDesc") %>' MaxLength="50" Width="300"></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="col-sm-4" >
                                                                                            <asp:TextBox ID="TextBoxEOtherSalesAmount" runat="server" Text='<%# Bind("OtherSalesAmount", "{0:F2}") %>'></asp:TextBox>
                                                                                            <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                                                                                    CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                                                                            <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                                                                                    CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                                                                        </div>
                                                                                    </div>
                                                                                </EditItemTemplate>
                                                                                <ItemTemplate>
                                                                                    <div class="row" style="text-align:right;">
                                                                                        <div class="col-sm-8" style="text-align:left;">
                                                                                            <asp:Label ID="LabelOtherSalesDesc" runat="server" Text='<%# Bind("OtherSalesDesc") %>'></asp:Label>
                                                                                        </div>
                                                                                        <div class="col-sm-4">
                                                                                            <asp:Label ID="LabelOtherSalesAmount" runat="server" Text='<%# Bind("OtherSalesAmount", "{0:F2}") %>'></asp:Label>

                                                                                            <asp:LinkButton runat="server" CommandName="Edit" ID="IEdit" ToolTip="Edit" 
                                                                                                    CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  
                                                                                                     />
                                                                                            <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                                                                    CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                                                                    OnClientClick="return confirm('Are you sure you want to delete this record?');" 
                                                                                                    />
                                                                                        </div>
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="text-align:right;">
                                                                <div class="col-sm-7" style="text-align:left;">
                                                                    USDA Commodity Usage<br />
                                                                    USDA Commodity Received
                                                                </div>
                                                                <div class="col-sm-2">
                                                                    <asp:TextBox ID="TextBoxUSDACommUsage" runat="server" Text='<%# Bind("USDACommUsage", "{0:F2}") %>' Width="100"></asp:TextBox><br />
                                                                    <asp:TextBox ID="TextBoxUSDACommRcvd" runat="server" Text='<%# Bind("USDACommRcvd", "{0:F2}") %>' Width="100" ></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-1">&nbsp;</div>
                                                                <div class="col-sm-2" style="vertical-align:middle;height:100%">
                                                                    <br />
                                                                    <asp:Label ID="LabelUSDACommRcvd" runat="server" Text='<%# Bind("USDACommRcvd", "{0:F2}") %>'></asp:Label>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="text-align:right;">

                                                            </div>
                                                            <div class="row" style="text-align:right;">
                                                                <div class="col-sm-10" style="text-align:left;font-weight:bold">
                                                                    Total
                                                                </div>
                                                                <div class="col-sm-2" style="font-weight:bold;border-top:solid;border-top-width:2px;border-top-color:black;">
                                                                    <asp:HiddenField ID="HFLinesTotal" runat="server" Value='<%# Bind("LinesTotal") %>' />
                                                                    <asp:Label ID="TextBoxTotal" runat="server" Text="0.00" Width="75" ></asp:Label>

                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col">&nbsp;</div>
                                                            </div>
                                                            <div class="row" style="border-top:solid;border-bottom-color:gray;border-bottom-width:2px;text-align:right;">
                                                                <div class="col-sm-3" style="text-align:left;font-weight:bold">
                                                                    P&L Report Amounts
                                                                </div>
                                                                <div class="col-sm-3" style="text-align:center;font-weight:bold">
                                                                    Other Income
                                                                </div>
                                                                <div class="col-sm-3" style="text-align:center;font-weight:bold">
                                                                    Dist Labor & Fringes 2
                                                                </div>
                                                                <div class="col-sm-3" style="text-align:center;font-weight:bold">
                                                                    Dist Costs 2
                                                                </div>
                                                            </div>
                                                            <div class="row" >
                                                                <div class="col-sm-3" style="text-align:left;font-weight:bold">
                                                                    &nbsp;
                                                                </div>
                                                                <div class="col-sm-3" style="text-align:center;font-weight:bold">
                                                                    <asp:TextBox ID="TextBoxIncome" runat="server" Width="100" Text='<%# Bind("PLIncome1", "{0:F2}") %>'></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-3" style="text-align:center;font-weight:bold">
                                                                    <asp:TextBox ID="TextBoxLabor" runat="server" Width="100" Text='<%# Bind("PLLabor2", "{0:F2}") %>'></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-3" style="text-align:center;font-weight:bold">
                                                                    <asp:TextBox ID="TextBoxCosts" runat="server" Width="100" Text='<%# Bind("PLCost2", "{0:F2}") %>'></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </EditItemTemplate>
                                                </asp:FormView>
                                            </div>
                                        </div>
                                        <div id="ImportData" class="row" style="padding-top:10px;">
                                            <div class="col-xl-1 col-sm-0"  ></div>
                                            <div class="col-sm-1" style="background-color:honeydew;border-color:black;border-style:solid;border-width:1px 0px 1px 1px;padding:5px 5px 5px 5px;">
                                                <br />
                                                <strong>IMPORT>></strong>
                                            </div>
                                            <div class="col-xl-9 col-sm-11" style="background-color:honeydew;border-color:black;border-style:solid;border-width:1px 1px 1px 0px;padding:5px 5px 5px 5px;">
                                                <asp:LinkButton ID="LinkButtonSpFn" Width="140" Height="70" runat="server" class="fas fa-file-invoice-dollar fa-2x mGridLinkButton" 
                                                    ToolTip="Import Special Functions" Text=" Special Functions" OnClick="LinkButtonSpFn_Click"   ></asp:LinkButton>
                                                <asp:LinkButton ID="LinkButtonInventory" Width="140" Height="70" runat="server" 
                                                    class="fas fa-boxes fa-2x mGridLinkButton" OnClick="LinkButtonInventory_Click"
                                                    ToolTip="Import USDA Amounts" Text=" USDA Amounts"  ></asp:LinkButton>
                                                <asp:LinkButton ID="LinkButtonMeals" Width="140" Height="70" runat="server" 
                                                    class="fas fa-boxes fa-2x mGridLinkButton" OnClick="LinkButtonMeals_Click"
                                                    ToolTip="Import Meal Counts" Text=" Meal Counts"  ></asp:LinkButton>
                                                <asp:LinkButton ID="LinkButtonCovidPurch" Width="140" Height="70" runat="server" class="fas fa-file-invoice-dollar fa-2x mGridLinkButton" 
                                                    ToolTip="Import Covid Purchases" Text=" Covid Purchases" OnClick="LinkButtonCovidPurch_Click"   ></asp:LinkButton>
                                            </div>
                                        </div>
                                        
                                        <div id="OtherSales" class="row" style="padding-top:10px;">
                                            <div class="col-xl-1 col-sm-0"  ></div>
                                            <div class="col-sm-3" style="background-color:#e6ffe6;border-color:black;border-style:solid;border-width:1px 0px 1px 1px;padding:5px 5px 5px 5px;">
                                                <br />
                                                <strong>Add Other Sales Amount>></strong>
                                            </div>
                                            <div class="col-xl-7 col-sm-9" style="background-color:#e6ffe6;border-color:black;border-style:solid;border-width:1px 1px 1px 0px;padding:5px 5px 5px 5px;">
                                                <asp:Label ID="LabelDesc" Width="300" runat="server" Text="Description"></asp:Label>
                                                <asp:Label ID="LabelAmount" Width="100" runat="server" Text="$Amount"></asp:Label>
                                                <br />
                                                <asp:TextBox ID="TextBoxOSalesDesc" runat="server" MaxLength="50" Width="300" ></asp:TextBox>
                                                <asp:TextBox ID="TextBoxOSalesAmount" runat="server" Width="100" Text="0.00"></asp:TextBox>
                                                <asp:LinkButton ID="LinkButtonAddOtherSales" runat="server" 
                                                    class="fas fa-plus mGridLinkButton" ToolTip="Add Other Sales Amount" ForeColor="Green" Width="40" OnClick="LinkButtonAddOtherSales_Click"  ></asp:LinkButton>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div> 
                        </ContentTemplate>
                    </asp:UpdatePanel>  

                    <div class="modal fade" id="SpecialFunctionsImportModal"><!--class="modal fade"-->
                        <div class="modal-dialog" >
                            <div class="modal-content" style="width:800px;">
                                <div class="modal-header">
                                    <h4 class="modal-title">Import Special Functions</h4>
                                    <button type="button" class="close" data-dismiss="modal">×</button>
                                </div>

                                <asp:UpdatePanel ID="UpdatePanelSPFN" runat="server" UpdateMode="Conditional"  >
                                    <ContentTemplate>
                                        <div class="modal-body">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <label style="font-weight:bold;">Set date range for Special Functions</label>
                                                    <asp:HiddenField ID="HFSPFNDistrictID" runat="server"  />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-3">
                                                    <div class="form-group">
                                                        <label>Posting Start Date:</label> 
                                                        <div class="input-group">
                                                            <asp:TextBox ID="TextBoxSpFnStartDate" runat="server" AutoPostBack="true" OnTextChanged="UpdateSpFnDateRange" ></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-md-3">
                                                    <div class="form-group">
                                                        <label>Posting End Date:</label> 
                                                        <div class="input-group">
                                                            <asp:TextBox ID="TextBoxSpFnEndDate" runat="server" AutoPostBack="true" OnTextChanged="UpdateSpFnDateRange" ></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    &nbsp;
                                                    <%--<label style="font-style:italic;">Only unposted Special Functions are included.</label>--%>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <asp:GridView ID="GridviewSpecialFunctions" runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataSourceID="SqlDataSourceSpFnInvoices"  >
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="CBSelect" runat="server" AutoPostBack="true" OnCheckedChanged="CBSelect_CheckedChanged" Checked="true" />
                                                                    <asp:HiddenField ID="HFInvoiceNumber" runat="server" Value='<%# Eval("InvoiceNumber") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:CheckBoxField   />
                                                            <asp:BoundField DataField="InvoiceNumber" HeaderText="InvoiceNumber"  />
                                                            <asp:BoundField DataField="OrgBilled" HeaderText="Org Billed"  />
                                                            <asp:BoundField DataField="FunctionDate" HeaderText="Function Date" DataFormatString="{0:d}"/>
                                                            <%--<asp:BoundField DataField="BillingDate" HeaderText="Post Date" DataFormatString="{0:d}" />--%>
                                                            <asp:BoundField DataField="SpecialFunctionDesc" HeaderText="Description"  />
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LabelInvoiceTotal" runat="server" Text='<%# Eval("InvoiceTotal" , "{0:F2}") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    Total: &nbsp;
                                                    <asp:Label ID="LabelTotal" runat="server" Text="$0.00"></asp:Label>
                                                    <asp:RadioButtonList ID="rlOptions" runat="server" RepeatDirection="Horizontal" BorderStyle="Solid" BorderColor="Silver" BorderWidth="1" 
                                                        Width="600" CellSpacing="10"  >
                                                        <asp:ListItem Text="Replace Special Function Sales Amount" Value="REPLACE" ></asp:ListItem>
                                                        <asp:ListItem Selected="True" Text="Add to Special Functions Sales Amount" Value="ADD" ></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                    <asp:LinkButton ID="BtnProcess" runat="server" CssClass="btn btn-primary" OnClick="BtnProcessSpFn_Click"  >
                                                        <i class="fas fa-cogs"></i>
                                                        Process 
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="BtnCancelSpFn" runat="server" data-dismiss="modal" CssClass="btn btn-primary" ForeColor="#cccccc">
                                                        <i class="fas fa-window-close" ></i>
                                                        Cancel 
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                    <div class="modal fade" id="InventoryImportModal" ><!--class="modal fade"-->
                        <div class="modal-dialog" >
                            <div class="modal-content" style="width:800px;">
                                <div class="modal-header">
                                    <h4 class="modal-title">Import USDA Inventory</h4>
                                    <button type="button" class="close" data-dismiss="modal">×</button>
                                </div>

                                <asp:UpdatePanel ID="UpdatePanelInventory" runat="server" UpdateMode="Conditional"  >
                                    <ContentTemplate>
                                        <div class="modal-body">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <label style="font-weight:bold;">Select Inventory Year-Month</label>
                                                    <asp:HiddenField ID="HFInvDistrictID" runat="server"  />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-3">
                                                    <div class="form-group">
                                                        <asp:TextBox ID="TextBoxYearmonth" runat="server" Width="60" OnTextChanged="TextboxYearMonth_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="RegexTrxMonth" runat="server" ControlToValidate="TextBoxYearmonth" CssClass="validators" ValidationExpression="([\d]){4}-([\d]){2}|(\*OPEN\*)"
                                                        Display="Dynamic" ErrorMessage="Inventory Month is not formatted correctly (YYYY-MM)." ValidationGroup="Insert">*</asp:RegularExpressionValidator>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    &nbsp;
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <asp:GridView ID="GridviewInventory"  runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataSourceID="SqlDataSourceUSDA" OnDataBound="GridviewInventory_DataBound"  >
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="CBSelect" runat="server" AutoPostBack="true" OnCheckedChanged="CBSelectInv_CheckedChanged" Checked="true" />
                                                                    <asp:HiddenField ID="HFInvoiceNumber" runat="server" Value='<%# Eval("YearMonth") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:CheckBoxField   />
                                                            <asp:BoundField DataField="YearMonth" HeaderText="Year-Month"  />
                                                            <asp:TemplateField HeaderText="USDA Received">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LabelRcvdTotal" runat="server" Text='<%# Eval("USDARcvd" , "{0:F2}") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="USDA Usage">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LabelUsageTotal" runat="server" Text='<%# Eval("USDAUsage" , "{0:F2}") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    Total Rcvd: &nbsp;
                                                    <asp:Label ID="LabelTotalRcvd"  runat="server" Text="0.00"></asp:Label>
                                                    Total Usage: &nbsp;
                                                    <asp:Label ID="LabelTotalUsage" runat="server" Text="0.00"></asp:Label>

                                                    <asp:RadioButtonList ID="rlInvOptions" runat="server" RepeatDirection="Horizontal" BorderStyle="Solid" BorderColor="Silver" BorderWidth="1" 
                                                        Width="600" CellSpacing="10"  >
                                                        <asp:ListItem Text="Replace Inventory Amounts" Value="REPLACE" ></asp:ListItem>
                                                        <asp:ListItem Selected="True" Text="Add to Inventory Amounts" Value="ADD" ></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                    <asp:LinkButton ID="BtnProcessInv" runat="server" CssClass="btn btn-primary" OnClick="BtnProcessInv_Click"  >
                                                        <i class="fas fa-cogs"></i>
                                                        Process 
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="BtnCancelInv" runat="server" data-dismiss="modal" CssClass="btn btn-primary" ForeColor="#cccccc">
                                                        <i class="fas fa-window-close"></i>
                                                        Cancel 
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                    <div class="modal fade" id="CovidImportModal"><!--class="modal fade"-->
                        <div class="modal-dialog" >
                            <div class="modal-content" style="width:800px;">
                                <div class="modal-header">
                                    <h4 class="modal-title">Import Covid Purchases</h4>
                                    <button type="button" class="close" data-dismiss="modal">×</button>
                                </div>

                                <asp:UpdatePanel ID="UpdatePanelCovid" runat="server" UpdateMode="Conditional"  >
                                    <ContentTemplate>
                                        <div class="modal-body">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <label style="font-weight:bold;">Set date range for Covid Transmittals</label>
                                                    <asp:HiddenField ID="HFCovidDistrictID" runat="server"  />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-3">
                                                    <div class="form-group">
                                                        <label>W/E Start Date Range:</label> 
                                                        <div class="input-group">
                                                            <asp:TextBox ID="TextBoxCovidStartDate" runat="server" AutoPostBack="true" OnTextChanged="UpdateCovidDateRange" ></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-md-3">
                                                    <div class="form-group">
                                                        <label>W/E End Date Range:</label> 
                                                        <div class="input-group">
                                                            <asp:TextBox ID="TextBoxCovidEndDate" runat="server" AutoPostBack="true" OnTextChanged="UpdateCovidDateRange" ></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    &nbsp;
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <asp:GridView ID="GridviewCovidPurch" runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataSourceID="SqlDataSourceCovidTransmittals"  >
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="CBSelect" runat="server" AutoPostBack="true" OnCheckedChanged="CBSelect_CovidCheckedChanged" Checked="true" />
                                                                    <asp:HiddenField ID="HFCounter" runat="server" Value='<%# Eval("Counter") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="InvoiceNumber" HeaderText="InvoiceNumber"  />
                                                            <asp:BoundField DataField="VendorID" HeaderText="Vendor"  />
                                                            <asp:BoundField DataField="InvoiceDate" HeaderText="Invoice Date" DataFormatString="{0:d}"/>
                                                            <asp:BoundField DataField="PostDate" HeaderText="W/E Date" DataFormatString="{0:d}" />
                                                            <asp:TemplateField HeaderText="Total">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="LabelInvoiceTotal" runat="server" Text='<%# Eval("InvoiceTotal" , "{0:F2}") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-3">
                                                    Total: &nbsp;
                                                    <asp:Label ID="LabelCovidTotal" runat="server" Text="$0.00"></asp:Label>
                                                </div>
                                                <div class="col-md-6" style="text-align:center;">
                                                    <asp:LinkButton ID="LinkButtonProcessCovid" runat="server" CssClass="btn btn-primary" OnClick="LinkButtonProcessCovid_Click"   >
                                                        <i class="fas fa-cogs"></i>
                                                        Process 
                                                    </asp:LinkButton>
                                                    &nbsp;
                                                    <asp:LinkButton ID="LinkButtonCancelCovid" runat="server" data-dismiss="modal" CssClass="btn btn-primary" ForeColor="#cccccc">
                                                        <i class="fas fa-window-close" ></i>
                                                        Cancel 
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>


                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT D.DistrictID, RTRIM(D.DistrictName) + '-' + RTRIM(D.DistrictID) 'Desc', D.Inactive
            FROM [District] D
            INNER JOIN [DistrictYearInfo] DI ON DI.DistrictID=D.DistrictID
            WHERE DI.ProgramYear=@ProgramYear AND DI.AcctType=1
            UNION ALL
            SELECT '%' as [DistrictID], '*All Districts*' as [DistrictName], 0 as [Inactive]
            ORDER BY [Desc]"> 
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvDists" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT D.DistrictID, RTRIM(D.DistrictName) + '-' + RTRIM(D.DistrictID) 'Desc', D.Inactive
            FROM [District] D
            INNER JOIN [DistrictYearInfo] DI ON DI.DistrictID=D.DistrictID
            WHERE DI.ProgramYear=@ProgramYear AND DI.AcctType=1 AND D.Inactive=0
            ORDER BY [Desc]">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvoiceList" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            OnInserting="SqlDataSourceInvoiceList_Inserting" OnInserted="SqlDataSourceInvoiceList_Inserted"

            InsertCommand="SET @InvoiceIdent=(SELECT ISNULL(MAX(InvoiceNumber),0)+1 InvoiceNumber FROM [dbo].[CPMInvoiceHDR]);
                INSERT INTO [CPMInvoiceHDR] ([InvoiceNumber], [ProgramYear], [DistrictID], [PeriodStart], [PeriodEnd], [BreakfastTotal], [LunchTotal], [CPMRate], [AlaSales], 
                [ACEquivRate], [SpecialFunctionSales], [USDACommUsage], [USDACommRcvd], [DateCreated], [SnackTotal], [DinnerTotal], [BrEquivRate], [SnEquivRate], [DnEquivRate],[ShowOverrideMERates]) 
                VALUES (@InvoiceIdent, @ProgramYear, @DistrictID, 
                @PeriodStart, @PeriodEnd, @BreakfastTotal, @LunchTotal, @CPMRate, @AlaSales, @ACEquivRate, @SpecialFunctionSales, @USDACommUsage, @USDACommRcvd, GetDate(),
                @SnackTotal, @DinnerTotal, @BrEquivRate, @SnEquivRate, @DnEquivRate, @ShowOverrideMERates);" 
            SelectCommand="SELECT HDR.[InvoiceNumber], HDR.[DistrictID], HDR.[PeriodEnd], D.DistrictName, HDR.DistrictID + ' - ' + D.DistrictName DistDisplayName
            FROM [CPMInvoiceHDR] HDR
            INNER JOIN [District] D ON HDR.DistrictID=D.DistrictID
            WHERE ([ProgramYear] = @ProgramYear) AND HDR.DistrictID LIKE @DistrictID" 
            DeleteCommand="DELETE [CPMInvoiceHDR] WHERE [InvoiceNumber]=@InvoiceNumber;DELETE [CPMInvoiceLINES] WHERE [InvoiceNumber]=@InvoiceNumber"
            FilterExpression="DistrictID LIKE '%{0}%'" OnFiltering="SqlDataSourceInvoiceList_Filtering"
            >
            <FilterParameters>
                <asp:ControlParameter ControlID="DDLDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </FilterParameters>
            <DeleteParameters>
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListNewInvDist" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="TextBoxNewInvPerStart" Name="PeriodStart" PropertyName="Text" Type="DateTime" />
                <asp:ControlParameter ControlID="TextBoxNewInvPerEnd" Name="PeriodEnd" PropertyName="Text" Type="DateTime" />
                <asp:Parameter Name="BreakfastTotal" Type="Int32" DefaultValue="0" />
                <asp:Parameter Name="LunchTotal" Type="Int32" DefaultValue="0" />
                <asp:Parameter Name="CPMRate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="AlaSales" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="ACEquivRate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="SpecialFunctionSales" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="USDACommUsage" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="USDACommRcvd" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="InvoiceIdent" Type="Int32" Direction="Output" />
                <asp:Parameter Name="SnackTotal" Type="Int32" DefaultValue="0" />
                <asp:Parameter Name="DinnerTotal" Type="Int32" DefaultValue="0" />
                <asp:Parameter Name="BrEquivRate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="SnEquivRate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="DnEquivRate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="ShowOverrideMERates" Type="Byte" DefaultValue="0" />
            </InsertParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvDetails" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [CPMInvoiceHDR] WHERE [InvoiceNumber] = @InvoiceNumber" 
            SelectCommand="SELECT HDR.*, D.*, ISNULL(LINES.LinesTotal,0.00) LinesTotal
            ,IIF(HDR.BrEquivRate=0, 1, HDR.BrEquivRate) BrEquivRateCalc
            ,IIF(HDR.SnEquivRate=0, 1, HDR.SnEquivRate) SnEquivRateCalc
            ,IIF(HDR.DnEquivRate=0, 1, HDR.DnEquivRate) DnEquivRateCalc
            FROM [CPMInvoiceHDR] HDR 
            LEFT OUTER JOIN [District] D ON HDR.DistrictID=D.DistrictID
            LEFT OUTER JOIN (SELECT InvoiceNumber, SUM(OtherSalesAmount) LinesTotal FROM CPMInvoiceLines GROUP BY InvoiceNumber) LINES ON HDR.InvoiceNumber=LINES.InvoiceNumber
            WHERE (HDR.[InvoiceNumber] = @InvoiceNumber)" 
            UpdateCommand="UPDATE [CPMInvoiceHDR] SET [ProgramYear] = @ProgramYear, [DistrictID] = @DistrictID, [PeriodStart] = @PeriodStart, [PeriodEnd] = @PeriodEnd, 
            [BreakfastTotal] = @BreakfastTotal, [LunchTotal] = @LunchTotal, [AlaSales] = @AlaSales, 
            [SpecialFunctionSales] = @SpecialFunctionSales, [USDACommUsage] = @USDACommUsage, [USDACommRcvd] = @USDACommRcvd,
            [PLIncome1]=@PLIncome1, [PLLabor2]=@PLLabor2, [PLCost2]=@PLCost2,
            [SnackTotal] = @SnackTotal, [DinnerTotal] = @DinnerTotal
            WHERE [InvoiceNumber] = @InvoiceNumber" OnUpdating="SqlDataSourceInvDetails_Updating" OnUpdated="SqlDataSourceInvDetails_Updated">
            <DeleteParameters>
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
            </DeleteParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="HFInvoiceNumber" Name="InvoiceNumber" PropertyName="Value" Type="Int32" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter DbType="Date" Name="PeriodStart" />
                <asp:Parameter DbType="Date" Name="PeriodEnd" />
                <asp:Parameter Name="BreakfastTotal" Type="Int32" />
                <asp:Parameter Name="LunchTotal" Type="Int32" />
                <asp:Parameter Name="AlaSales" Type="Decimal" />
                <asp:Parameter Name="SpecialFunctionSales" Type="Decimal" />
                <asp:Parameter Name="USDACommUsage" Type="Decimal" />
                <asp:Parameter Name="USDACommRcvd" Type="Decimal" />
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
                <asp:Parameter Name="PLIncome1" Type="Decimal" />
                <asp:Parameter Name="PLLabor2" Type="Decimal" />
                <asp:Parameter Name="PLCost2" Type="Decimal" />
                <asp:Parameter Name="SnackTotal" Type="Int32" />
                <asp:Parameter Name="DinnerTotal" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceDistYearInfo" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"   
            SELECTCOMMAND="SELECT [CPMBillingRate],[SpecifiedMEquivRate],[SpecifiedMEBfast],[SpecifiedMESnack],[SpecifiedMEDinner],[CBOverrideMERates]  FROM [DistrictYearInfo] WHERE [DistrictID]=@DistrictID AND [ProgramYear]=@ProgramYear " >
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListNewInvDist" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceBDGT" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" 
            SelectCommand="SELECT CostPerMeal [FoodCPM],[MealEquivalentFactor] FROM [vwBDGT_Totals] TOTN WHERE (TOTN.[BidNumber] = @BidNumber) AND TOTN.BudgetYear=@BudgetYear" DataSourceMode="DataSet">
            <SelectParameters>
                <asp:ControlParameter Name="BidNumber" ControlID="DropDownListNewInvDist" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter Name="BudgetYear" ControlID="HFBudgetYear" PropertyName="Value" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceBDGTSFSP" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" 
            DataSourceMode="DataSet"
            SelectCommand="SELECT TotalCPM [FoodCPM], 0.00 MealEquivalentFactor 
            FROM [vwSFSP_TotalsActiveOnly] TOTN
            WHERE (TOTN.[BidNumber] = @BidNumber) AND TOTN.BudgetYear=@BudgetYear ">
            <SelectParameters>
                <asp:ControlParameter Name="BidNumber" ControlID="DropDownListNewInvDist" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter Name="BudgetYear" ControlID="HFBudgetYear" PropertyName="Value" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceSpFnInvoices" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT SFL.[ProgramYear], SFL.[DistrictID], SFL.[InvoiceNumber], SFL.[OrgBilled], SFL.[FunctionDate], SFL.[BillingDate], SFL.[SpecialFunctionDesc]
	                ,SFL.Subtotal - SFL.DistrictLabor - SFL.DistrictFringe InvoiceTotal
                FROM [SpecialFunctionsLedger] SFL
            WHERE SFL.ProgramYear=@ProgramYear AND SFL.DistrictID=@DistrictID AND SFL.BillingDate>=@StartDate AND SFL.BillingDate<=@EndDate 
            ORDER BY dbo.GetNumeric(InvoiceNumber),  SFL.[FunctionDate]">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="HFSPFNDistrictID" Name="DistrictID" PropertyName="Value" Type="String" />
                <asp:ControlParameter ControlID="TextBoxSpFnStartDate" DbType="Date" Name="StartDate" PropertyName="Text" DefaultValue="1/1/1900" />
                <asp:ControlParameter ControlID="TextBoxSpFnEndDate" DbType="Date" Name="EndDate" PropertyName="Text" DefaultValue="1/2/1900" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceUSDA" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, DistrictID, YearMonth, (USDAProteinRcvd + USDAGroceryRcvd) *-1.00 AS USDARcvd,
                (USDAProteinOpen + USDAGroceryOpen + USDAProteinRcvd + USDAGroceryRcvd - USDAProteinInstUse - USDAGroceryInstUse + USDAProteinTrans + USDAGroceryTrans - USDAProteinEnding - USDAGroceryEnding) AS USDAUsage
                FROM [Inventory] INV
                WHERE INV.ProgramYear=@ProgramYear AND INV.DistrictID=@DistrictID AND INV.YearMonth=@YearMonth ">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="HFInvDistrictID" Name="DistrictID" PropertyName="Value" Type="String" />
                <asp:ControlParameter ControlID="TextBoxYearmonth" Type="String" Name="YearMonth" PropertyName="Text" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvoiceLines" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [CPMInvoiceLines] WHERE [InvoiceNumber] = @InvoiceNumber AND [InvoiceLineNumber] = @InvoiceLineNumber" 
            InsertCommand="INSERT INTO [CPMInvoiceLines] ([InvoiceNumber], [InvoiceLineNumber], [OtherSalesAmount], [OtherSalesDesc]) 
            VALUES (@InvoiceNumber, (SELECT ISNULL(MAX(InvoiceLineNumber),0)+1 FROM [CPMInvoiceLines] WHERE InvoiceNumber=@InvoiceNumber ), @OtherSalesAmount, @OtherSalesDesc)" 
            SelectCommand="SELECT * FROM [CPMInvoiceLines] WHERE ([InvoiceNumber] = @InvoiceNumber) ORDER BY [InvoiceLineNumber]" 
            UpdateCommand="UPDATE [CPMInvoiceLines] SET [OtherSalesAmount] = @OtherSalesAmount, [OtherSalesDesc] = @OtherSalesDesc 
            WHERE [InvoiceNumber] = @InvoiceNumber AND [InvoiceLineNumber] = @InvoiceLineNumber"
            OnInserting="SqlDataSourceInvoiceLines_Inserting" OnInserted="SqlDataSourceInvoiceLines_Inserted">
            <DeleteParameters>
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
                <asp:Parameter Name="InvoiceLineNumber" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:ControlParameter ControlID="HFInvoiceNumber" Name="InvoiceNumber" PropertyName="Value" Type="Int32" />
                <asp:Parameter Name="OtherSalesAmount" Type="Decimal" />
                <asp:Parameter Name="OtherSalesDesc" Type="String" />
            </InsertParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="HFInvoiceNumber" Name="InvoiceNumber" PropertyName="Value" Type="Int32" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="OtherSalesAmount" Type="Decimal" />
                <asp:Parameter Name="OtherSalesDesc" Type="String" />
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
                <asp:Parameter Name="InvoiceLineNumber" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceCovidTransmittals" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SELECTCOMMAND="SELECT Purchases.*, (Groceries + Paper + Cleaning + Milk + Other) InvoiceTotal 
            FROM Purchases WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND PostDate>=@StartingDate AND PostDate<=@EndingDate AND ProgramType='Covid' ORDER BY PostDate">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="HFCovidDistrictID" Name="DistrictID" PropertyName="Value" Type="String" />
                <asp:ControlParameter ControlID="TextBoxCovidStartDate" DbType="Date" Name="StartingDate" PropertyName="Text" DefaultValue="1/1/1900" />
                <asp:ControlParameter ControlID="TextBoxCovidEndDate" DbType="Date" Name="EndingDate" PropertyName="Text" DefaultValue="1/2/1900" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>

</asp:Content>
