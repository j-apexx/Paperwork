<%@ Page Title="Special Functions Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SpecialFunctionsEntry.aspx.cs" Inherits="Paperwork.SpecialFunctionsEntry" EnableEventValidation="false" MaintainScrollPositionOnPostBack="true" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
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

            $("[id$=TextBoxInvoiceNotes]").attr('maxlength', '500');

            $(function () {
                $("[id$=TextBoxFunctionDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id$=TextBoxBillingDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: new Date($('[id$=HFStartDate]').val()),
                    maxDate: new Date($('[id$=HFEndDate]').val())
                });
            });

            
            $(function () {
                $("[id$=TextBoxDatePaid]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    defaultDate: 0
                });
                //$("[id$=TextBoxDatePaid]").datepicker('setDate', new Date());
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

            $("[id$=TextBoxTNGLabor]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val())) ) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(2).toString());
                }
                RecalcTNGFringe();
                RecalcInvoiceTotal();
            });

            $("[id$=TextBoxTNGFringePct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.0000');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(4).toString());
                }
                RecalcTNGFringe();
                RecalcInvoiceTotal();
            });

            $("[id$=TextBoxDistLabor]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(2).toString());
                }
                RecalcDistFringe();
                RecalcInvoiceTotal();
            });

            $("[id$=TextBoxDistFringePct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.0000');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(4).toString());
                }
                RecalcDistFringe();
                RecalcInvoiceTotal();
            });

            $("[id$=TextBoxFoodCost],[id$=TextBoxPaperCleanSup],[id$=TextBoxPaperCost],[id$=TextBoxCleaningCost],[id$=TextBoxSuppliesCost],[id$=TextBoxMileage],[id$=TextBoxFSDCosts],[id$=TextBoxTextBoxMarkupDlr],[id$=TextBoxMarkupDlr]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(2).toString());
                }

                RecalcInvoiceTotal();
            });

            $("[id$=TextBoxMarkupPct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.0000');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(4).toString());
                }
                RecalcInvoiceTotal();
            });

            $("[id$=DDLMarkupType]").change(function () {
                MarkupShowHide();
                RecalcInvoiceTotal();
            });

            $("[id$=DDLEMarkupType]").change(function () {
                MarkupShowHideEdit();
                RecalcLineEditTotal();
                RecalcInvoiceTotal();
            });

            $("[id$=DDLFMarkupType]").change(function () {
                MarkupShowHideFooter();
                RecalcLineFooterTotal
                RecalcInvoiceTotal();
            });

            //Miscellaneous Amounts
            //Markup Pct and Dollars
            $("[id$=TextBoxFMarkupPct],[id$=TextBoxFMarkupDlr],[id$=TextBoxFMiscAmount]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(2).toString());
                }
                RecalcLineFooterTotal();
            });

            $("[id$=TextBoxEMiscAmount],[id$=TextBoxEMarkupPct],[id$=TextBoxEMarkupDlr]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(2).toString());
                }

                RecalcLineEditTotal();
            });

            $("[id$=TextBoxSalesTaxPct]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(2).toString());
                }

                RecalcInvoiceTotal();
            });

            $("[id$=CBFoodCostTaxable],[id$=CBPaperCleanSupplTaxable],[id$=CBPaperTaxable],[id$=CBCleaningTaxable],[id$=CBSuppliesTaxable]").change(function () {
                RecalcInvoiceTotal();
            });

            $("[id$=TextBoxFSDHourlyRate],[id$=TextBoxHours]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(2).toString());
                }

                RecalcPayAuthForm();
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

            //Enable Enter Key tabbing - exclude the notes box so that crlf can be entered intientionally
            $('input,select,textarea').not("[id$=TextBoxInvoiceNotes]").on("keypress", function (e) {
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
                    if ($canfocus.eq(index).attr('id') === $("[id$=TextBoxBPaidTtl]").attr('id')) {
                        $("[id$=CBBreakfast]").focus();
                    }

                    return false;//Important - marks enter as handled. Otherwise dropdown lists will recover focus and open/close the dropdown.
                }
            });

            MarkupShowHideFooter();

            //Autocomplete for Sp Fn organizations
            //SearchText();

            $(".autosuggest").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        //url: "PaperworkService.asmx/getSPFNOrgNames",
                        url: "SpecialFunctionsEntry.aspx/getSPFNOrgNames",
                        data: "{'prefixText':'" + $("[id$=TextBoxOrgBilled]").val() + "','distID':'" + $("[id$=HFDistID]").val() +"'}",
                        dataType: "json",
                        success: function (data) {
                            response(data.d);
                            //alert(data.d);
                        },
                        error: function (result) {
                            alert('Error');
                        }
                    });
                }
            });

            //new actual cost fields
            $("[id$=TextBoxFoodCostActual],[id$=TextBoxPaperCostActual],[id$=TextBoxCleaningCostActual],[id$=TextBoxSuppliesCostActual]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    var thisVal = parseFloat($(this).val());
                    $(this).val(thisVal.toFixed(2).toString());
                }
            });

            blink('#msg');
        }

        function blink(selector) {
            $(selector).fadeOut(400, function () {
                $(this).fadeIn(2000, function () {
                    blink(this);
                });
            });
        }
        

        function closeModal(modalid) {
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#' + modalid).modal('hide');
        }

        function SearchText() {
            //alert($("[id$=TextBoxOrgBilled]").val());

            $(".autosuggest").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        url: "PaperworkService.asmx/getSPFNOrgNames",
                        //data: "{'prefixText':'" + $("[id$=TextBoxOrgBilled]").val() + "'}",
                        data: "{'prefixText':'A'}",
                        dataType: "json",
                        success: function (data) {
                            response(data.d);
                        },
                        error: function (result) {
                            alert("Error");
                        }
                    });
                }
            });
        };

        function MarkupShowHide() {
            var markupType = $("[id$=DDLMarkupType]").val();

            if (markupType == "1")//%
            {
                $("[id$=TextBoxMarkupPct]").show();
                $("[id$=TextBoxMarkupDlr]").val('0.00');
                $("[id$=TextBoxMarkupDlr]").hide();
            }
            else if (markupType == "2")//%
            {
                $("[id$=TextBoxMarkupDlr]").show();
                $("[id$=TextBoxMarkupPct]").val('0.0000');
                $("[id$=TextBoxMarkupPct]").hide();
            }
            else {
                $("[id$=TextBoxMarkupDlr]").val('0.00');
                $("[id$=TextBoxMarkupPct]").val('0.0000');
                $("[id$=TextBoxMarkupDlr]").hide();
                $("[id$=TextBoxMarkupPct]").hide();
            }
        };

        function MarkupShowHideEdit() {
            var markupTypeE = $("[id$=DDLEMarkupType]").val();

            if (markupTypeE == "1")//Pct
            {
                $("[id$=TextBoxEMarkupPct]").show();
                $("[id$=TextBoxEMarkupDlr]").val('0.00');
                $("[id$=TextBoxEMarkupDlr]").hide();
                $("[id$=TextBoxENoMarkup]").hide();
            }
            else if (markupTypeE == "2")//Dlr
            {
                $("[id$=TextBoxEMarkupDlr]").show();
                $("[id$=TextBoxEMarkupPct]").val('0.0000');
                $("[id$=TextBoxEMarkupPct]").hide();
                $("[id$=TextBoxENoMarkup]").hide();
            }
            else {
                $("[id$=TextBoxENoMarkup]").show();
                $("[id$=TextBoxEMarkupDlr]").val('0.00');
                $("[id$=TextBoxEMarkupPct]").val('0.0000');
                $("[id$=TextBoxEMarkupDlr]").hide();
                $("[id$=TextBoxEMarkupPct]").hide();
            }
        };

        function MarkupShowHideFooter() {
            var markupTypeF = $("[id$=DDLFMarkupType]").val();

            if (markupTypeF == "1")//Pct
            {
                $("[id$=TextBoxFMarkupPct]").show();
                $("[id$=TextBoxFMarkupDlr]").val('0.00');
                $("[id$=TextBoxFMarkupDlr]").hide();
                $("[id$=TextBoxFNoMarkup]").hide();
            }
            else if (markupTypeF == "2")//Dlr
            {
                $("[id$=TextBoxFMarkupDlr]").show();
                $("[id$=TextBoxFMarkupPct]").val('0.0000');
                $("[id$=TextBoxFMarkupPct]").hide();
                $("[id$=TextBoxFNoMarkup]").hide();
            }
            else {
                $("[id$=TextBoxFMarkupDlr]").val('0.00');
                $("[id$=TextBoxFMarkupPct]").val('0.0000');
                $("[id$=TextBoxFMarkupDlr]").hide();
                $("[id$=TextBoxFMarkupPct]").hide();
                $("[id$=TextBoxFNoMarkup]").show();
            }
        };


        function RecalcTNGFringe() {
            var TNGFringe =
                Math.round((
                    (parseFloat($("[id$=TextBoxTNGLabor]").val()) * parseFloat($("[id$=TextBoxTNGFringePct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;

            if (TNGFringe == 0)
                $("[id$=TextBoxTNGFringe]").val('0.00');
            else
                $("[id$=TextBoxTNGFringe]").val(TNGFringe.toFixed(2).toString());
        };

        function RecalcDistFringe() {
            var TNGFringe =
                Math.round((
                    (parseFloat($("[id$=TextBoxDistLabor]").val()) * parseFloat($("[id$=TextBoxDistFringePct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;

            if (TNGFringe == 0)
                $("[id$=TextBoxDistFringe]").val('0.00');
            else
                $("[id$=TextBoxDistFringe]").val(TNGFringe.toFixed(2).toString());
        };


        function RecalcLineEditTotal() {
            var MarkedUpLineTotalE = parseFloat(0.00);
            var markupTypeE = $("[id$=DDLEMarkupType]").val();
            var MarkupAmountE = parseFloat(0.00);
            var MiscAmtE = parseFloat($("[id$=TextBoxEMiscAmount]").val());

            if (markupTypeE == "1")//%
            {
                MarkupAmountE = Math.round((
                    (MiscAmtE * parseFloat($("[id$=TextBoxEMarkupPct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;
            }
            else if (markupTypeE == "2")//$
            {
                MarkupAmountE = parseFloat($("[id$=TextBoxEMarkupDlr]").val());
            }

            MarkedUpLineTotalE = (MarkupAmountE + MiscAmtE) ;

            if (MarkedUpLineTotalE == 0)
                $("[id$=TextBoxEMiscAmtTotal]").val('0.00');
            else
                $("[id$=TextBoxEMiscAmtTotal]").val(MarkedUpLineTotalE.toFixed(2).toString());
        };

        function RecalcLineFooterTotal() {
            var MarkedUpLineTotalF = parseFloat(0.00);
            var markupTypeF = $("[id$=DDLFMarkupType]").val();
            var MarkupAmountF = parseFloat(0.00);
            var MiscAmtF = parseFloat($("[id$=TextBoxFMiscAmount]").val());

            if (markupTypeF == "1")//%
            {
                MarkupAmountF = Math.round((
                    (MiscAmtF * parseFloat($("[id$=TextBoxFMarkupPct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;
            }
            else if (markupTypeF == "2")//$
            {
                MarkupAmountF = parseFloat($("[id$=TextBoxFMarkupDlr]").val());
            }

            MarkedUpLineTotalF = (MarkupAmountF + MiscAmtF) ;

            if (MarkedUpLineTotalF == 0)
                $("[id$=TextBoxFMiscAmtTotal]").val('0.00');
            else
                $("[id$=TextBoxFMiscAmtTotal]").val(MarkedUpLineTotalF.toFixed(2).toString());
        };

        function RecalcInvoiceTotal() {
            //Original PaperCleanSuppl was a text box, now it can be a hidden field - which breaks the parseFloat
            var PaperCleanSuppl = parseFloat($("[id$=TextBoxPaperCleanSup]").val());
            if (isNaN(PaperCleanSuppl)) PaperCleanSuppl = parseFloat(0.00);
            var PaperCost = parseFloat($("[id$=TextBoxPaperCost]").val());
            if (isNaN(PaperCost)) PaperCost = parseFloat(0.00);
            var CleaningCost = parseFloat($("[id$=TextBoxCleaningCost]").val());
            if (isNaN(CleaningCost)) CleaningCost = parseFloat(0.00);
            var SuppliesCost = parseFloat($("[id$=TextBoxSuppliesCost]").val());
            if (isNaN(SuppliesCost)) SuppliesCost = parseFloat(0.00);


            var InvTotal =
                Math.round((
                    (parseFloat($("[id$=TextBoxFoodCost]").val()) + PaperCleanSuppl +
                    PaperCost + CleaningCost + SuppliesCost + //new Aug2024
                    parseFloat($("[id$=TextBoxMileage]").val()) + parseFloat($("[id$=TextBoxFSDCosts]").val()) +
                    parseFloat($("[id$=TextBoxTNGLabor]").val()) + parseFloat($("[id$=TextBoxTNGFringe]").val()) +
                    parseFloat($("[id$=TextBoxDistLabor]").val()) + parseFloat($("[id$=TextBoxDistFringe]").val()) +
                    parseFloat($("[id$=HFMiscAmountTotal]").val()))
                    * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100;

            var TaxTotal = parseFloat(0.00);
            var TaxPct = parseFloat($("[id$=TextBoxSalesTaxPct]").val());
            if (isNaN(parseFloat(TaxPct))   )
                TaxPct = parseFloat(0.00);
            var FoodCostTax = parseFloat(0.00);
            var PaperCleanSupTax = parseFloat(0.00);
            var PaperTax = parseFloat(0.00);
            var CleaningTax = parseFloat(0.00);
            var SuppliesTax = parseFloat(0.00);
            var MiscAmountTax = parseFloat(0.00);

            if ($("[id$=CBFoodCostTaxable]").is(":checked")) {
                FoodCostTax = Math.round(((parseFloat($("[id$=TextBoxFoodCost]").val()) * TaxPct) * 100)) / 10000;
            }
            if ($("[id$=CBPaperCleanSupplTaxable]").is(":checked")) {
                PaperCleanSupTax = Math.round(((PaperCleanSuppl * TaxPct) * 100)) / 10000;
            }
            if ($("[id$=CBPaperTaxable]").is(":checked")) {
                PaperTax = Math.round(((PaperCost * TaxPct) * 100)) / 10000;
            }
            if ($("[id$=CBCleaningTaxable]").is(":checked")) {
                CleaningTax = Math.round(((CleaningCost * TaxPct) * 100)) / 10000;
            }
            if ($("[id$=CBSuppliesTaxable]").is(":checked")) {
                SuppliesTax = Math.round(((SuppliesCost * TaxPct) * 100)) / 10000;
            }

            MiscAmountTax = Math.round(((parseFloat($("[id$=HFMiscTaxableAmount]").val()) * TaxPct) * 100)) / 10000;

            TaxTotal = FoodCostTax + PaperCleanSupTax + MiscAmountTax + PaperTax + CleaningTax + SuppliesTax;

            if (TaxTotal == 0)
                $("[id$=TextBoxSalesTaxTotal]").val('0.00');
            else
                $("[id$=TextBoxSalesTaxTotal]").val(TaxTotal.toFixed(2).toString());

            InvTotal = InvTotal + TaxTotal;

            var MarkedUpInvTotal = parseFloat(0.00);
            var markupType = $("[id$=DDLMarkupType]").val();
            var MarkupAmount = parseFloat(0.00);

            if (markupType == "1")//%
            {
                $("[id$=TextBoxMarkupPct]").show();
                MarkupAmount = Math.round((
                    (InvTotal * parseFloat($("[id$=TextBoxMarkupPct]").val()))
                    * 100)) / 10000;//rounding works with integers, so I need to mult by 100, round, then divide by 100;
                $("[id$=TextBoxMarkupDlr]").val('0.00');
                $("[id$=TextBoxMarkupDlr]").hide();
            }
            else if (markupType == "2")//$
            {
                $("[id$=TextBoxMarkupDlr]").show();
                MarkupAmount = parseFloat($("[id$=TextBoxMarkupDlr]").val());
                $("[id$=TextBoxMarkupPct]").val('0.0000');
                $("[id$=TextBoxMarkupPct]").hide();
            }
            else
            {
                $("[id$=TextBoxMarkupDlr]").val('0.00');
                $("[id$=TextBoxMarkupPct]").val('0.0000');
                $("[id$=TextBoxMarkupDlr]").hide();
                $("[id$=TextBoxMarkupPct]").hide();
            }

            MarkedUpInvTotal = InvTotal + MarkupAmount;

            if (MarkedUpInvTotal == 0)
                $("[id$=TextBoxInvoiceTotal]").val('0.00');
            else
                $("[id$=TextBoxInvoiceTotal]").val(MarkedUpInvTotal.toFixed(2).toString());
        };

        //apply highlight 
        $('body').on('focus', 'input,textarea,select', function (e) {
            $(this).addClass("highlight");
        });
        // remove highlight
        $('body').on('focusout', 'input,textarea,select', function (e) {
            $(this).removeClass("highlight");
        });

        function PrintElem(imgSrc) {
            var mywindow = window.open('', 'PRINT', 'height=1100,width=850');

            mywindow.document.write('<html><head><title></title>');
            //Handling to hide browser headers and footers with url, dates etc
            mywindow.document.write('<style type="text/css" media="print">');
            mywindow.document.write('@page {size: auto;   /* auto is the initial value */margin: 0mm;  /* this affects the margin in the printer settings */}');
            mywindow.document.write('html{background- color: #FFFFFF;margin: 0px;  /* this affects the margin on the html before sending to printer */}');
            mywindow.document.write('body{border: solid 1px white;margin: 0 0 0 0; /* margin you want for the content */}');//change color of border to troubleshoot
            mywindow.document.write('</style>');
            //Body includes triggers to auto print, then close
            //mywindow.document.write('</head><body onload="window.print();window.close();">');
            mywindow.document.write('</head><body>');
            mywindow.document.write('<img id="InvImage" src="' + imgSrc + '" Width="850" Height="1088"  >');
            mywindow.document.write('</body></html>');
            mywindow.document.close(); // necessary for IE >= 10
            mywindow.focus(); // necessary for IE >= 10*/

            return true;
        }

        /* New Aug 2024 - Pay Authorization calcs */
        function RecalcPayAuthForm() {
            var FSDRate = parseFloat($("[id$=TextBoxFSDHourlyRate]").val());
            var FSDHours = parseFloat($("[id$=TextBoxHours]").val());
            var TotalWages = FSDRate * FSDHours;

            if (TotalWages == 0)
                $("[id$=TextBoxTotalWages]").val('0.00');
            else
                $("[id$=TextBoxTotalWages]").val(TotalWages.toFixed(2).toString());

            var TotalFringePct = parseFloat($("[id$=HFTNGFringePct]").val());
            var TotalFringeAmt = TotalWages * TotalFringePct;

            if(TotalFringeAmt == 0)
                $("[id$=TextBoxFringeTotal]").val('0.00');
            else
                $("[id$=TextBoxFringeTotal]").val(TotalFringeAmt.toFixed(2).toString());

            var TotalLabor = TotalWages + TotalFringeAmt;

            if (TotalLabor == 0)
                $("[id$=TextBoxTotalFSDLabor]").val('0.00');
            else
                $("[id$=TextBoxTotalFSDLabor]").val(TotalLabor.toFixed(2).toString());
        };

    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanelOverall" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgressModal" runat="server" AssociatedUpdatePanelID="UpdatePanelModal" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgressModalPrintRange" runat="server" AssociatedUpdatePanelID="UpdatePanelModalPrintRange" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgressModalPayAuthorization" runat="server" AssociatedUpdatePanelID="UpdatePanelModalPayAuthorization" DisplayAfter="5" Visible="true">
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
    <asp:UpdatePanel ID="UpdatePanelOverall" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ButtonSend" EventName="Click" />
        </Triggers>
        <ContentTemplate>
<div class="container">
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
                                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                    DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                                <asp:HiddenField ID="HFStartDate" runat="server" />
                                <asp:HiddenField ID="HFEndDate" runat="server"  />
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
                                    <asp:LinkButton ID="LinkButtonPrintInvoices" Width="150" runat="server" 
                                        class="fas fa-print mGridLinkButton" 
                                        ToolTip="Opens Print Range Dialog" Text=" Print Invoices" OnClick="IPrintRangeOpen_Click" ></asp:LinkButton>
                                </div>
                                <asp:GridView ID="GridViewInvoices" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber" 
                                    DataSourceID="SqlDataSourceInvHdrsList" CssClass="mGrid" AllowPaging="true" PageSize="25"  PagerSettings-Mode="NumericFirstLast"  
                                    PagerStyle-HorizontalAlign="Center" PagerStyle-Font-Size="Large" PagerSettings-PageButtonCount="10" PagerStyle-Width="100" PagerStyle-CssClass="mGridPager"
                                    OnSelectedIndexChanged="GridViewInvoices_SelectedIndexChanged" OnDataBound="GridViewInvoices_DataBound"
                                    OnRowDataBound="GridviewClickableRowDatabound" OnPageIndexChanged="GridViewInvoices_PageIndexChanged"
                                    AllowSorting="true" >
                                    <Columns>
                                        <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" SortExpression="InvoiceNumberSort" ReadOnly="True" HeaderStyle-Width="150" />
                                        <asp:BoundField DataField="OrgBilled" HeaderText="Organization" SortExpression="OrgBilled" />
                                        <asp:BoundField DataField="FunctionDate" HeaderText="Function Date" SortExpression="FunctionDate" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center"  />
                                        <asp:BoundField DataField="BillingDate" HeaderText="Posting Date" SortExpression="BillingDate" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center"  />
                                        <asp:BoundField DataField="SpecialFunctionDesc" SortExpression="SpecialFunctionDesc" HeaderText="Special Function Description" />
                                        <asp:CheckBoxField DataField="InvoicePaid" SortExpression="InvoicePaid" HeaderText="Paid" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
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
                                            <div class="col-sm-2">
                                                <div class="messagealert" id="alert_container_InvoiceHdr"></div>
                                            </div>
                                            <div class="col-sm-10">
                                                    <asp:HiddenField ID="HFInvoiceNumber" runat="server" />
                                                    <asp:HiddenField ID="HFInvoiceSaved" runat="server" />
                                                    <asp:HiddenField ID="HFInvoiceLocked" runat="server" />
                                                    <asp:LinkButton ID="ISave" runat="server" 
                                                        ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                                        CssClass="fas fa-save mGridLinkButton" Width="150" OnClick="ISave_Click" ></asp:LinkButton>
                                                    <asp:LinkButton ID="IPrint" Width="150" runat="server" 
                                                        class="fas fa-print mGridLinkButton" 
                                                        ToolTip="Opens Print Preview" Text=" Invoice Report" OnClick="IPrint_Click" ></asp:LinkButton>
                                                    <asp:LinkButton ID="BtnOpenEmailDialog" runat="server" OnClick="BtnOpenEmailDialog_Click" 
                                                        class="fas fa-envelope mGridLinkButton" Width="150">
                                                        Open Email Dialog  
                                                    </asp:LinkButton>
                                                    <asp:HiddenField ID="HFSSRSBaseFolder" runat="server" />
                                                <span id="quickPrintSpan" runat="server">
                                                    <asp:LinkButton ID="IPrintDirect" Width="150" runat="server" 
                                                        class="fas fa-print mGridLinkButton" 
                                                        ToolTip="Prints Direct to Default Printer" OnClick="IPrintDirect_Click" >
                                                        <i class="fas fa-bolt" style="color:gold;left:-25px;position:relative;padding: 0 0 0 0;" ></i> 
                                                        Quick Print
                                                    </asp:LinkButton>
                                                    <asp:CheckBox ID="CBPrintOnlyMisc" runat="server" Text="Only Print Misc. Detail" Checked="false" />
                                                </span>
                                            </div>

                                        </div>
                                        <div class="row">
                                            <div class="col-sm-10 " style="padding-top:5px;border-style:solid;border-width:1px;border-color:dimgray;">
                                                <asp:FormView ID="FormViewSpFnOrg" runat="server" CssClass="mGridRecaps" DefaultMode="Edit" DataSourceID="SqlDataSourceInvHdr" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber"
                                                    OnPreRender="FormViewSpFnOrg_PreRender" Width="100%" OnDataBound="FormViewSpFnOrg_DataBound" >
                                                    <EditItemTemplate>
                                                        <div class="container">
                                                            <div class="row">
                                                                <div class="col-sm-5">
                                                                    <asp:TextBox ID="TextBoxInvoice" runat="server" Text='<%# Bind("InvoiceNumber") %>' Enabled='<%# !(bool)Eval("Locked")%>' Width="150" OnTextChanged="TextBox_TextChanged" MaxLength="15"></asp:TextBox>&nbsp;Invoice No.
                                                                    <asp:HiddenField ID="HFShowSalesTax" runat="server" Value='<%# Eval("ShowSalesTax") %>' />
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
                                                                    <asp:HiddenField ID="HFDistID" runat="server" Value='<%# Eval("DistrictID") %>' />
                                                                    <asp:TextBox ID="TextBoxOrgBilled" runat="server" Text='<%# Bind("OrgBilled") %>' Width="100%" CssClass="longTextBox autosuggest" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' MaxLength="50" AutoCompleteType="Search" AutoPostBack="true" ></asp:TextBox>
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
                                                                    <asp:TextBox ID="TextBoxContactPerson" runat="server" Text='<%# Bind("OrgContact") %>' Width="100%" CssClass="longTextBox" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' MaxLength="50" AutoCompleteType="Disabled" ></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="row" style="padding-top:5px;padding-bottom:5px;">
                                                                <div class="col-sm-5">
                                                                    <asp:Label ID="Label5" runat="server" Text="SPECIAL FUNCTION DESCRIPTION:" ></asp:Label>
                                                                </div>
                                                                <div class="col-sm-7" style="text-align:right;">
                                                                    <asp:TextBox ID="TextBoxSpecFuncDesc" runat="server" Text='<%# Bind("SpecialFunctionDesc") %>' CssClass="longTextBox" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' MaxLength="75" AutoCompleteType="Disabled"></asp:TextBox>
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
                                                                    <asp:TextBox ID="TextBoxPONumber" runat="server" Text='<%# Bind("PONumber") %>' Width="90" OnTextChanged="TextBox_TextChanged" MaxLength="25"></asp:TextBox>
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
                                                                            <div class="col-sm-12" style="text-align:right;">
                                                                                <asp:Label ID="LabelTaxable" runat="server" Text="Taxable" Width="80" Visible='<%# Eval("ShowSalesTax") %>' ></asp:Label>

                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                Food Cost
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxFoodCost" runat="server" Text='<%# Bind("FoodCost") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>                                                                         
                                                                            </div>
                                                                            <div class="col-sm-1" style="text-align:center;">
                                                                                <asp:CheckBox ID="CBFoodCostTaxable" runat="server" Checked='<%# Bind("FoodCostTaxable") %>' Enabled='<%# !(bool)Eval("Locked")%>' Visible='<%# Eval("ShowSalesTax") %>' OnCheckedChanged="CheckBox_CheckedChanged" />
                                                                            </div>
                                                                        </div>
                                                                        <div class="row" id="divLegacyPaperCleanSup" runat="server">
                                                                            <div class="col-sm-5">
                                                                                Paper, Cleaning & Supplies
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxPaperCleanSup" runat="server" Text='<%# Bind("PaperCleanSuppl") %>' Width="80" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>'  ></asp:TextBox>                                                                         
                                                                            </div>
                                                                            <div class="col-sm-1" style="text-align:center;">
                                                                                <asp:CheckBox ID="CBPaperCleanSupplTaxable" runat="server" Checked='<%# Bind("PaperCleanSupplTaxable") %>' Enabled='<%# !(bool)Eval("Locked")%>' Visible='<%# Eval("ShowSalesTax") %>' OnCheckedChanged="CheckBox_CheckedChanged" />
                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                Paper
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxPaperCost" runat="server" Text='<%# Bind("PaperCost") %>' Width="80" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>'  ></asp:TextBox>                                                                         
                                                                            </div>
                                                                            <div class="col-sm-1" style="text-align:center;">
                                                                                <asp:CheckBox ID="CBPaperTaxable" runat="server" Checked='<%# Bind("PaperCostTaxable") %>' Enabled='<%# !(bool)Eval("Locked")%>' Visible='<%# Eval("ShowSalesTax") %>' OnCheckedChanged="CheckBox_CheckedChanged" />
                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                Cleaning
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxCleaningCost" runat="server" Text='<%# Bind("CleaningCost") %>' Width="80" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>'  ></asp:TextBox>                                                                         
                                                                            </div>
                                                                            <div class="col-sm-1" style="text-align:center;">
                                                                                <asp:CheckBox ID="CBCleaningTaxable" runat="server" Checked='<%# Bind("CleaningCostTaxable") %>' Enabled='<%# !(bool)Eval("Locked")%>' Visible='<%# Eval("ShowSalesTax") %>' OnCheckedChanged="CheckBox_CheckedChanged" />
                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                Supplies
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxSuppliesCost" runat="server" Text='<%# Bind("SuppliesCost") %>' Width="80" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>'  ></asp:TextBox>                                                                         
                                                                            </div>
                                                                            <div class="col-sm-1" style="text-align:center;">
                                                                                <asp:CheckBox ID="CBSuppliesTaxable" runat="server" Checked='<%# Bind("SuppliesCostTaxable") %>' Enabled='<%# !(bool)Eval("Locked")%>' Visible='<%# Eval("ShowSalesTax") %>' OnCheckedChanged="CheckBox_CheckedChanged" />
                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                Mileage
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxMileage" runat="server" Text='<%# Bind("Mileage") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>                                                                         

                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                DFN Costs
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxFSDCosts" runat="server" Text='<%# Bind("FSDCosts") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled="false" ></asp:TextBox>                                                                         

                                                                            </div>
                                                                            <div class="col-sm-1" style="text-align:left;margin-left:0px;padding-left:0px;">
                                                                                <asp:LinkButton ID="IApproval" runat="server" style="font-size:smaller;overflow:clip;line-height:unset;"
                                                                                    class="fas fa-list-alt mGridLinkButton" 
                                                                                    ToolTip="OUTSIDE ORGANIZATION SPECIAL FUNCTION PAY AUTHORIZATION" Text=" Approval" OnClick="IApproval_Click" Visible="true" ></asp:LinkButton>
                                                                                
                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                TNG Labor excluding fringe
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxTNGLabor" runat="server" Text='<%# Bind("TNGLabor") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>                                                                         

                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-3">
                                                                                TNG Fringe
                                                                            </div>
                                                                            <div class="col-sm-2">
                                                                                <asp:TextBox ID="TextBoxTNGFringePct" runat="server" Text='<%# Bind("TNGFringePct") %>' Width="75"  OnTextChanged="TextBox_TextChanged" Enabled='<%# (bool)Eval("Locked")==false && (Session["AccessLevel"].ToString().ToUpper()=="OFFICE" || Session["AccessLevel"].ToString().ToUpper()=="ADMIN")?true:false %>'></asp:TextBox>% <%-- changed enabled to always locked 8-2024 '<%# !(bool)Eval("Locked")%>'--%>
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxTNGFringe" runat="server" Text='<%# Bind("TNGFringe", "{0:F2}") %>' Enabled="false" Width="80" ></asp:TextBox>                                                                         

                                                                            </div>
                                                                            
                                                                            
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-5">
                                                                                District Labor excluding fringe
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxDistLabor" runat="server" Text='<%# Bind("DistrictLabor") %>' Width="80"  OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>                                                                         

                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-3">
                                                                                District Fringe
                                                                            </div>
                                                                            <div class="col-sm-2">
                                                                                <asp:TextBox ID="TextBoxDistFringePct" runat="server" Text='<%# Bind("DistrictFringePct") %>' Width="75" OnTextChanged="TextBox_TextChanged" Enabled='<%# !(bool)Eval("Locked")%>' ></asp:TextBox>%
                                                                            </div>
                                                                            <div class="col-sm-6" style="text-align:right;">
                                                                                <asp:TextBox ID="TextBoxDistFringe" runat="server" Text='<%# Bind("DistrictFringe", "{0:F2}") %>' Enabled="false" Width="80" ></asp:TextBox>                                                                         

                                                                            </div>
                                                                        </div>
                                                                    <div class="row">
                                                                        <div class="col-sm-3" style="font-weight:bold;">
                                                                            Miscellaneous:
                                                                        </div>
                                                                        <div class="col-sm-2" style="text-align:right;font-weight:bold;">
                                                                            Cost
                                                                        </div>
                                                                        <div class="col-sm-3" style="text-align:right;font-weight:bold;">
                                                                            Dist Markup
                                                                        </div>
                                                                        <div class="col-sm-2" style="text-align:right;font-weight:bold;" id="divTotalColumnHdr" runat="server">
                                                                            Total
                                                                        </div>
                                                                        <div class="col-sm-1" style="text-align:right;font-weight:bold;" id="divTaxColumnHdr" runat="server">
                                                                            Tax
                                                                        </div>
                                                                    </div>
                                                                    <div class="row">
                                                                        <div class="col-sm-12">
                                                                            <asp:GridView ID="GridViewMiscLines" runat="server" DataSourceID="SqlDataSourceInvLines"  ShowHeader="false" CssClass="SpGrid"
                                                                                ShowFooter="True" AutoGenerateColumns="False" 
                                                                                OnRowCommand="GridViewLines_RowCommand" OnRowDataBound="GridViewLines_RowDataBound" 
                                                                                OnRowEditing="GridViewMiscLines_RowEditing" OnDataBound="GridViewMiscLines_DataBound"
                                                                                DataKeyNames="ProgramYear, DistrictID, InvoiceNumber, LineNumber" Width="100%">
                                                                                <Columns>
                                                                                    <asp:TemplateField HeaderText="MiscDescription" SortExpression="MiscDescription">
                                                                                        <EditItemTemplate>
                                                                                            <div class="row" style="text-align:left;">
                                                                                                <div class="col-sm-3">
                                                                                                    <asp:TextBox ID="TextBoxEMiscDesc" runat="server" Text='<%# Bind("MiscDescription") %>' MaxLength="75" Width="200"></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-2" style="text-align:right;">
                                                                                                    <asp:TextBox ID="TextBoxEMiscAmount" runat="server" Text='<%# Bind("MiscAmount") %>' Width="80"></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-3" style="text-align:right;">
                                                                                                    <asp:DropDownList ID="DDLEMarkupType" runat="server" Enabled="true" SelectedValue='<%# Bind("MarkupType") %>' AutoPostBack="true" OnSelectedIndexChanged="DDLEMarkupType_SelectedIndexChanged" >
                                                                                                        <asp:ListItem Text="None" Value="0"></asp:ListItem>
                                                                                                        <asp:ListItem Text="%" Value="1"></asp:ListItem>
                                                                                                        <asp:ListItem Text="$" Value="2"></asp:ListItem>
                                                                                                    </asp:DropDownList>
                                                                                                    <asp:TextBox ID="TextBoxEMarkupPct" runat="server" Text='<%# Bind("MarkupPct", "{0:F4}") %>' Width="80" Enabled="true" Visible='<%# Eval("MarkupType").ToString()=="1" %>' ></asp:TextBox>
                                                                                                    <asp:TextBox ID="TextBoxEMarkupDlr" runat="server" Text='<%# Bind("MarkupDlr", "{0:F2}") %>' Width="80" Enabled="true" Visible='<%# Eval("MarkupType").ToString()=="2" %>'></asp:TextBox>
                                                                                                    <asp:TextBox ID="TextBoxENoMarkup"  runat="server" Text=""                                   Width="80" Enabled="false" Visible='<%# Eval("MarkupType").ToString()=="0" %>'></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-2" style="text-align:right;" id="divTotalColumnEdit" runat="server">
                                                                                                    <asp:TextBox ID="TextBoxEMiscAmtTotal" runat="server" Width="80" Text='<%# Eval("LineTotal", "{0:F2}") %>' Enabled="false"  ></asp:TextBox>
                                                                                                    
                                                                                                </div>
                                                                                                <div class="col-sm-1" style="text-align:right;" id="divTaxColumnEdit" runat="server">
                                                                                                    <asp:CheckBox ID="CBMiscAmountTaxable" runat="server" Checked='<%# Bind("MiscAmountTaxable") %>'  />
                                                                                                </div>
                                                                                                <div class="col-sm-1" style="margin-left:0;margin-right:0;padding-left:0;padding-right:0;">
                                                                                                <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                                                                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                                                                                <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                                                                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                                                                            </div>
                                                                                            </div>
                                                                                            
                                                                                        </EditItemTemplate>
                                                                                        <ItemTemplate>
                                                                                            <div class="row">
                                                                                                <div class="col-sm-3">
                                                                                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("MiscDescription") %>' Width="200"></asp:Label>
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
                                                                                                    <asp:TextBox ID="TextBoxINoMarkup"  runat="server" Text=""                                   Width="80" Visible='<%# Eval("MarkupType").ToString()=="0"?true:false %>' Enabled="false" ></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-2" style="text-align:right;" id="divTotalColumnItem" runat="server">
                                                                                                    <asp:TextBox ID="TextBoxIMiscAmtTotal" runat="server" Width="80" Text='<%# Eval("LineTotal", "{0:F2}") %>' Enabled="false"  ></asp:TextBox>
                                                                                                </div>
                                                                                                <div class="col-sm-1" style="text-align:right;" id="divTaxColumnItem" runat="server">
                                                                                                    <asp:CheckBox ID="CBIMiscAmountTaxable" runat="server" Checked='<%# Bind("MiscAmountTaxable") %>' Enabled="false" />
                                                                                                </div>
                                                                                                <div class="col-sm-1" style="margin-left:0;margin-right:0;padding-left:0;padding-right:0;">
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
                                                                                                <div class="col-sm-3">
                                                                                                    Add New Misc. Amount<br />
                                                                                                    
                                                                                                    <asp:TextBox ID="TextBoxFMiscDesc" runat="server" MaxLength="75" Width="200" ></asp:TextBox>
                                                                                                    <asp:RequiredFieldValidator ID="ReqMiscDesc" runat="server" ControlToValidate="TextBoxFMiscDesc" Display="Dynamic" CssClass="validators" 
                                                                                                        ValidationGroup="Insert" ErrorMessage="Description is required" Text="*Required"></asp:RequiredFieldValidator>
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
                                                                                                <div class="col-sm-2" style="text-align:right;" id="divTotalColumnFooter" runat="server">
                                                                                                    Total
                                                                                                    <br />
                                                                                                    <asp:TextBox ID="TextBoxFMiscAmtTotal" runat="server" Width="80" Text="0.00" Enabled="false" ></asp:TextBox>
                                                                                                    
                                                                                                </div>
                                                                                                <div class="col-sm-1" style="text-align:right;" id="divTaxColumnFooter" runat="server">
                                                                                                    Tax
                                                                                                    <br />
                                                                                                    <asp:CheckBox ID="CBFMiscAmountTaxable" runat="server" />
                                                                                                </div>
                                                                                                <div class="col-sm-1"  style="margin-left:0;margin-right:0;padding-left:0;padding-right:0;">
                                                                                                    <br />
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
                                                                            <asp:CheckBox ID="CBPrintNotes" runat="server" Text="Display on Invoice" Checked='<%# Bind("PrintNotes") %>' Enabled='<%# (bool)Eval("Locked")?false:true %>' OnCheckedChanged="CheckBox_CheckedChanged" />
                                                                        </div>
                                                                    </div>
                                                                    <asp:TextBox ID="TextBoxInvoiceNotes" runat="server" Rows="4" Text='<%# Bind("InvoiceNotes") %>' TextMode="MultiLine" Width="100%" OnTextChanged="TextBox_TextChanged" ReadOnly='<%# (bool)Eval("Locked")%>' ></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-6">
                                                                    <div class="row" id="SalesTax" runat="server">
                                                                        <div class="col-sm-5" style="text-align:right;">
                                                                            <asp:TextBox ID="TextBoxSalesTaxPct" runat="server" Text='<%# Bind("SalesTaxPct","{0:F2}") %>' OnTextChanged="TextBox_TextChanged" ReadOnly='<%# (bool)Eval("Locked")%>' Width="100" Visible='<%# Eval("ShowSalesTax") %>'></asp:TextBox>
                                                                            <asp:Label ID="LabelPct" runat="server"  Visible='<%# Eval("ShowSalesTax") %>' Text="%"></asp:Label>
                                                                        </div>
                                                                        <div class="col-sm-7" style="text-align:right;">
                                                                            <asp:Label ID="Label9" runat="server" Text="SALES TAX" Visible='<%# Eval("ShowSalesTax") %>'></asp:Label>
                                                                            <asp:TextBox ID="TextBoxSalesTaxTotal" runat="server" Text="0" Enabled="false" Width="130" Visible='<%# Eval("ShowSalesTax") %>'></asp:TextBox>
                                                                        </div>
                                                                    </div>
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
                                                                    <div class="row">
                                                                        <asp:HiddenField ID="HFMiscAmountTotal" runat="server" Value='<%# Eval("MiscAmountTotal") %>' />
                                                                        <asp:HiddenField ID="HFMiscTaxableAmount" runat="server" Value='<%# Eval("MiscAmountTaxableTotal") %>' />
                                                                    </div>
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
                                            <div class="col-sm-2 " style="padding-top:300px;border-style:solid;border-width:1px;border-color:dimgray;">
                                                <asp:FormView ID="FormViewSpFnCosts" runat="server" CssClass="mGridRecaps" DefaultMode="Edit" DataSourceID="SqlDataSourceInvHdr" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber"
                                                    OnPreRender="FormViewSpFnOrg_PreRender" Width="100%" >
                                                    <EditItemTemplate>
                                                        <div class="row">
                                                            <div class="col-sm-12" style="text-align:center;">
                                                                <h5>Actual Costs</h5>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-sm-5">
                                                                <asp:Label ID="LabelFood" runat="server" Text="Food" Width="150"></asp:Label>
                                                            </div>
                                                            <div class="col-sm-7" style="text-align:right;">
                                                                <asp:TextBox ID="TextBoxFoodCostActual" runat="server" Text='<%# Bind("FoodCostActual") %>' Width="75"  ></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-sm-5">
                                                                <asp:Label ID="LabelPaper" runat="server" Text="Paper" Width="150"></asp:Label>
                                                            </div>
                                                            <div class="col-sm-7" style="text-align:right;">
                                                                <asp:TextBox ID="TextBoxPaperCostActual" runat="server" Text='<%# Bind("PaperCostActual") %>' Width="75"  ></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-sm-5">
                                                                <asp:Label ID="LabelCleaning" runat="server" Text="Cleaning" Width="150"></asp:Label>
                                                            </div>
                                                            <div class="col-sm-7" style="text-align:right;">
                                                                <asp:TextBox ID="TextBoxCleaningCostActual" runat="server" Text='<%# Bind("CleaningCostActual") %>' Width="75"  ></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-sm-5">
                                                                <asp:Label ID="LabelSupplies" runat="server" Text="Supplies" Width="150"></asp:Label>
                                                            </div>
                                                            <div class="col-sm-7" style="text-align:right;">
                                                                <asp:TextBox ID="TextBoxSuppliesCostActual" runat="server" Text='<%# Bind("SuppliesCostActual") %>' Width="75" ></asp:TextBox>
                                                            </div>
                                                        </div>



                                                    </EditItemTemplate>
                                                </asp:FormView>
                                                
                                            </div>
                                        </div>



                                    </ContentTemplate>
                                </asp:UpdatePanel>


                                <div class="modal fade" id="ModalPayAuthorization" >
                                    <div class="modal-dialog modal-xl modal-dialog-centered" >
                                        <div class="modal-content" style="width:1400px;" >
                                            <div class="modal-header">
                                                <h4 class="modal-title">OUTSIDE ORGANIZATION SPECIAL FUNCTION PAY AUTHORIZATION</h4>
                                                <button type="button" class="close" data-dismiss="modal">×</button>
                                            </div>
                                            <div class="modal-body">
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <asp:UpdatePanel ID="UpdatePanelModalPayAuthorization" UpdateMode="Conditional" runat="server">
                                                                    <ContentTemplate>

                                                                        <asp:FormView ID="FormViewPayAuth" runat="server" CssClass="mGridRecaps" DefaultMode="Edit" DataSourceID="SqlDataSourcePayAuth" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber,PaymentAuthNumber">
                                                                            <EditItemTemplate>
                                                                                <div class="container">
                                                                                    <div class="row">
                                                                                         <div class="col-sm-4">
                                                                                             DFN HOURLY RATE
                                                                                         </div>
                                                                                        <div class="col-sm-2">
                                                                                            <asp:TextBox ID="TextBoxFSDHourlyRate" runat="server" Text='<%# Bind("FSDHourlyRate") %>' Width="100" Enabled="False" ></asp:TextBox>
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col-sm-4" style="text-align:right;">
                                                                                            x
                                                                                        </div>
                                                                                        
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col-sm-4">
                                                                                            NUMBER OF HOURS FOR THE FUNCTION
                                                                                        </div>
                                                                                        <div class="col-sm-2">
                                                                                            <asp:TextBox ID="TextBoxHours" runat="server" Text='<%# Bind("HOURS") %>' Width="100" Enabled='<%# !(bool)Eval("Locked") && (byte)Eval("PaymentAuthStatus")==0 %>' ></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="col-sm-4">
                                                                                            
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col-sm-4">
                                                                                            TOTAL WAGES
                                                                                        </div>
                                                                                        <div class="col-sm-2">
                                                                                            <asp:TextBox ID="TextBoxTotalWages" runat="server" Text='<%# Bind("TotalWages", "{0:C2}") %>' Width="100" Enabled="false" ></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="col-sm-6">
                                                                                            This amount is the DFN compensation for the special function.<br />
                                                                                            **RM, Enter this Total Wages amount into ADP** 
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col-sm-4" style="text-align:right;">
                                                                                            x
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col-sm-4">
                                                                                            Employer Payroll %
                                                                                        </div>
                                                                                        <div class="col-sm-2">
                                                                                            <asp:TextBox ID="TextBoxTNGFringePct" runat="server" Text='<%# Bind("TNGFringe", "{0:P2}" ) %>' Width="100" Enabled="false" ></asp:TextBox>
                                                                                            <asp:HiddenField ID="HFTNGFringePct" runat="server" Value='<%# Bind("TNGFringe") %>' /> 
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col-sm-4">
                                                                                            Employer PR % Amt.
                                                                                        </div>
                                                                                        <div class="col-sm-2">
                                                                                            <asp:TextBox ID="TextBoxFringeTotal" runat="server" Text='<%# Bind("TNGFringeTotal", "{0:C2}" ) %>' Width="100" Enabled="false" ></asp:TextBox>
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col"><hr /></div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col-sm-4">
                                                                                            TOTAL DFN LABOR
                                                                                        </div>
                                                                                        <div class="col-sm-2">
                                                                                            <asp:TextBox ID="TextBoxTotalFSDLabor" runat="server" Text='<%# Bind("TotalFSDLabor", "{0:C2}" ) %>' Width="100" Enabled="false" ></asp:TextBox>
                                                                                        </div>
                                                                                        <div class="col-sm-6">
                                                                                            This amount is to be billed for the special function to the organization.
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col"><hr /></div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col" style="text-align:center; vertical-align:middle; font-size:larger;">
                                                                                            <asp:LinkButton ID="ISubmit" Width="100" runat="server"
                                                                                                class="far fa-arrow-alt-circle-right mGridLinkButton" 
                                                                                                ToolTip="Submit" Text=" Submit" Visible='<%# !(bool)Eval("Locked") && (byte)Eval("PaymentAuthStatus")==0 %>' OnClick="ISubmit_Click" ></asp:LinkButton>

                                                                                            <asp:LinkButton ID="IApprove" Width="100" runat="server" ForeColor="ForestGreen"
                                                                                                class="far fa-check-circle mGridLinkButton" OnClick="UpdateApprovalStatus_Click"
                                                                                                ToolTip="Approve" Text=" Approve" Visible='<%# !(bool)Eval("Locked") && (byte)Eval("PaymentAuthStatus")==1 && (Session["AccessLevel"].ToString().ToUpper()=="ADMIN" || Session["AccessLevel"].ToString().ToUpper()=="OFFICE" || Session["AccessLevel"].ToString().ToUpper()=="RM" ) %>' ></asp:LinkButton>

                                                                                            <asp:LinkButton ID="IDeny" Width="100" runat="server" ForeColor="Red"
                                                                                                class="far fa-times-circle mGridLinkButton" OnClick="UpdateApprovalStatus_Click"
                                                                                                ToolTip="Deny" Text=" Deny"       Visible='<%# (byte)Eval("PaymentAuthStatus")==1 && (Session["AccessLevel"].ToString().ToUpper()=="ADMIN" || Session["AccessLevel"].ToString().ToUpper()=="OFFICE" || Session["AccessLevel"].ToString().ToUpper()=="RM" ) %>'></asp:LinkButton>

                                                                                            <asp:Label ID="LabelSubmitted" runat="server" Text="SUBMITTED" Width="100" BackColor="DarkGray" ForeColor="White" Visible='<%# (byte)Eval("PaymentAuthStatus")==1 && Session["AccessLevel"].ToString().ToUpper()=="USER" %>'></asp:Label>
                                                                                            <asp:Label ID="LabelApproved" runat="server" Text="APPROVED" Width="100" BackColor="DarkGreen" ForeColor="White" Visible='<%# (byte)Eval("PaymentAuthStatus")==2 %>'></asp:Label>
                                                                                            <asp:Label ID="LabelDenied" runat="server" Text="DENIED" Width="100" BackColor="DarkRed" ForeColor="White" Visible='<%# (byte)Eval("PaymentAuthStatus")==3 %>'></asp:Label>
                                                                                            <asp:HiddenField ID="HFRMNumber" runat="server" Value='<%# Bind("RMNumber") %>' />
                                                                                            <asp:HiddenField ID="HFRMEmail" runat="server" Value='<%# Bind("RMEmail") %>' /> 
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row" runat="server" Visible='<%# !(bool)Eval("Locked") && (byte)Eval("PaymentAuthStatus")==2 && (Session["AccessLevel"].ToString().ToUpper()=="ADMIN" || Session["AccessLevel"].ToString().ToUpper()=="OFFICE" || Session["AccessLevel"].ToString().ToUpper()=="RM" ) %>'>
                                                                                        <div class="col" style="text-align:center; vertical-align:middle; font-size:larger;">
                                                                                            <div id="msg"> <strong><font color="red">
                                                                                                ** RM, Please complete in ADP to submit this Special Function Pay for the DFN, selecting the Special Function Pay - Supplemental Pay Code.
                                                                                            </font></strong></div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </EditItemTemplate>
                                                                        </asp:FormView>

                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="modal fade" id="ModalRangePrint" >  
                                    <div class="modal-dialog" >  
                                        <div class="modal-content" style="width:1000px;" > 
                                            <div class="modal-header">  
                                                <h4 class="modal-title">Select the Special Functions invoices to print</h4>  
                                                <button type="button" class="close" data-dismiss="modal">×</button>  
                                            </div>  
                                            <div class="modal-body">  
                                                <div class="row">  
                                                    <div class="col-md-12">  
                                                        <div class="form-group">  
                                                            <div class="input-group">  
                                                                <asp:UpdatePanel ID="UpdatePanelModalPrintRange" UpdateMode="Conditional" runat="server">
                                                                    <Triggers>
                                                                        <asp:PostBackTrigger ControlID="ButtonPrintRange" />
                                                                    </Triggers>
                                                                    <ContentTemplate>
                                                                        <div style="width:800px;"  > 
                                                                            <asp:CheckBox ID="CBPrintOnlyMiscModal" runat="server" Text="Only Print Misc. Detail" Checked="false" />
                                                                            <asp:GridView ID="GridViewPrintInvoices" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,InvoiceNumber" 
                                                                                DataSourceID="SqlDataSourceInvHdrsList" CssClass="mGrid" AllowPaging="true" PageSize="25"  PagerSettings-Mode="NumericFirstLast"  
                                                                                PagerStyle-HorizontalAlign="Center" PagerStyle-Font-Size="Large" PagerSettings-PageButtonCount="10" PagerStyle-Width="100" PagerStyle-CssClass="mGridPager"
                                                                                OnRowDataBound="GridviewClickableRowDatabound" OnPageIndexChanged="GridViewInvoices_PageIndexChanged"
                                                                                AllowSorting="true" >
                                                                                <Columns>
                                                                                    <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" SortExpression="InvoiceNumberSort" ReadOnly="True" HeaderStyle-Width="150" />
                                                                                    <asp:BoundField DataField="OrgBilled" HeaderText="Organization" SortExpression="OrgBilled" />
                                                                                    <asp:BoundField DataField="FunctionDate" HeaderText="Function Date" SortExpression="FunctionDate" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center"  />
                                                                                    <asp:BoundField DataField="BillingDate" HeaderText="Posting Date" SortExpression="BillingDate" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center"  />
                                                                                    <asp:BoundField DataField="SpecialFunctionDesc" SortExpression="SpecialFunctionDesc" HeaderText="Special Function Description" />
                                                                                    <asp:TemplateField HeaderText="Print?">
                                                                                        <ItemTemplate>
                                                                                            <div style="width:60px;text-align:center;">
                                                                                                <asp:CheckBox ID="CBPrint" runat="server" />
                                                                                            </div>
                                                                                        </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                                </Columns>
                                                                            </asp:GridView>
                                                                            <rsweb:ReportViewer ID="ReportViewer2" runat="server" ProcessingMode="Remote" 
                                                                                CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
                                                                                ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
                                                                                ShowPageNavigationControls="False" ZoomPercent="50" ShowParameterPrompts="False" ZoomMode="Percent" 
                                                                                AsyncRendering="false" ShowRefreshButton="false" ShowZoomControl="false" ShowReportBody="false" ShowFindControls="false" ShowToolBar="false"  >
                                                                                <ServerReport ReportPath="Special Functions Invoice" />
                                                                            </rsweb:ReportViewer>

                                                                        </div>  
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>  
                                                        </div>  
                                                    </div>  
                                                </div>  
                                            </div>  
                                            <div class="modal-footer">  
                                                <asp:Button ID="ButtonPrintRange" runat="server" CssClass="btn btn-outline-primary" Text="Print Invoices" OnClick="ButtonPrintRange_Click" /> 
                                                <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Close</button>  
                                            </div>  
                                        </div>  
                                    </div>  
                                </div>  
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
                CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
                ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
                ShowPageNavigationControls="False" ZoomPercent="50" ShowParameterPrompts="False" ZoomMode="Percent" 
                AsyncRendering="true" ShowRefreshButton="false" ShowZoomControl="false" ShowReportBody="false" ShowFindControls="false" ShowToolBar="true"  >
                <ServerReport ReportPath="Special Functions Invoice" />
            </rsweb:ReportViewer>
            <br />
</div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvHdrsList" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
            SelectCommand="SELECT [ProgramYear],[DistrictID],[InvoiceNumber],[OrgBilled],[OrgContact],[FunctionDate],
            CASE WHEN [BillingDate]='1/1/1900' THEN '' ELSE CONVERT(VARCHAR(10),BillingDate,101) END BillingDate,[SpecialFunctionDesc], [Locked],
            dbo.GetNumeric([InvoiceNumber]) InvoiceNumberSort,
            CASE WHEN [DatePaid]='1/1/1900' THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END InvoicePaid
            FROM [SPFNInvoiceHDR] Hdr
            WHERE ((Hdr.[ProgramYear] = @ProgramYear) AND (Hdr.[DistrictID] = @DistrictID)) ORDER BY Hdr.FunctionDate DESC, [InvoiceNumber] DESC"
            DeleteCommand="DELETE [SPFNInvoiceHDR] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber]=@InvoiceNumber));
            DELETE [SPFNInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber]=@InvoiceNumber))"
                OnDeleted="SqlDataSourceInvHdrsList_Deleted">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvHdr" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_"
            SelectCommand="SELECT [ProgramYear],HDR.[DistrictID],D.[DistrictState], [InvoiceNumber], [PONumber], CASE WHEN [DatePaid]='1/1/1900' THEN '' ELSE CONVERT(VARCHAR(10),DatePaid,101) END DatePaid
            ,[OrgBilled],[OrgContact],[FunctionDate],CASE WHEN [BillingDate]='1/1/1900' THEN '' ELSE CONVERT(VARCHAR(10),BillingDate,101) END BillingDate
            ,[OrgRemitTo],[SpecialFunctionDesc]
            ,[FoodCost],[PaperCleanSuppl],[Mileage],[FSDCosts],[TNGLabor],[TNGFringePct],[DistrictLabor],[DistrictFringePct],[MarkupType],[MarkupDlr],[MarkupPct],[InvoiceNotes],[PrintNotes]
            ,0 InvoiceTotal, ROUND([TNGLabor]*[TNGFringePct]*0.01,2) TNGFringe, ROUND([DistrictLabor]*[DistrictFringePct]*0.01,2) DistrictFringe
            ,(SELECT ISNULL(SUM(  MiscAmount + IIF(MarkupType=0, 0, IIF(MarkupType=1, MiscAmount*MarkupPct*0.01, MarkupDlr))   ),0) FROM [SPFNInvoiceLines] WHERE  (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber))) MiscAmountTotal
            , [Locked], [SchoolNum],[Org3086],[OrgCACFP],[PaymentNumber]
			,[FoodCostTaxable], [PaperCleanSupplTaxable], ([SalesTaxPct] * 100) SalesTaxPct
			,(SELECT ISNULL(SUM(IIF(MiscAmountTaxable=0, 0.00, MiscAmount + IIF(MarkupType=0, 0, IIF(MarkupType=1, MiscAmount*MarkupPct*0.01, MarkupDlr))) )
				,0) FROM [SPFNInvoiceLines] WHERE  (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber))) MiscAmountTaxableTotal
            ,CAST(IIF(D.[DistrictState]='SC', 1, 0) AS BIT) ShowSalesTax
            ,FoodCostActual, PaperCostActual, CleaningCostActual, SuppliesCostActual
            ,PaperCost ,CleaningCost , SuppliesCost , PaperCostTaxable, CleaningCostTaxable, SuppliesCostTaxable
			FROM [SPFNInvoiceHDR] HDR
			INNER JOIN [District] D ON HDR.DistrictID=D.DistrictID
			CROSS APPLY (SELECT IIF([FoodCostTaxable]=1, [FoodCostTaxable]*[SalesTaxPct],0.00) FoodCostTax) TaxCalcs
			WHERE (([ProgramYear] = @ProgramYear) AND (HDR.[DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber))">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:ControlParameter Name="InvoiceNumber" ControlID="HFInvoicenumber" PropertyName="Value" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvLines" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT LINE.[ProgramYear],LINE.[DistrictID],LINE.[InvoiceNumber],LINE.[LineNumber],LINE.[MiscDescription],LINE.[MiscAmount]
            ,LINE.[MarkupType], LINE.[MarkupDlr], LINE.[MarkupPct], HDR.[Locked]
            ,LINE.[MiscAmount] + IIF(LINE.MarkupType=0, 0, IIF(LINE.MarkupType=1, LINE.MarkupPct*LINE.MiscAmount*0.01, LINE.MarkupDlr)) LineTotal
            ,LINE.[MiscAmountTaxable]
            FROM [SPFNInvoiceLines] LINE
            INNER JOIN [SPFNInvoiceHDR] HDR ON HDR.[ProgramYear]=LINE.[ProgramYear] AND HDR.[DistrictID]=LINE.[DistrictID] AND HDR.[InvoiceNumber]=LINE.[InvoiceNumber]
            WHERE ((LINE.[ProgramYear] = @ProgramYear) AND (LINE.[DistrictID] = @DistrictID) AND (LINE.[InvoiceNumber] = @InvoiceNumber))
            UNION ALL
            SELECT '' [ProgramYear],'' [DistrictID],'' [InvoiceNumber],0 [LineNumber],'' [MiscDescription],0.00 [MiscAmount]
            ,0 MarkupType, 0 MarkupDlr, 0 MarkupPct, HDR.Locked, 0 LineTotal, CAST(0 AS BIT) MiscAmountTaxable
            FROM [SPFNInvoiceHDR] HDR
            WHERE ((HDR.[ProgramYear] = @ProgramYear) AND (HDR.[DistrictID] = @DistrictID) AND (HDR.[InvoiceNumber] = @InvoiceNumber))"
                    
            DeleteCommand="DELETE [SPFNInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) 
            AND ([LineNumber] = @LineNumber))"
                    
            InsertCommand="INSERT INTO [SPFNInvoiceLines] ([ProgramYear],[DistrictID],[InvoiceNumber],[LineNumber],[MiscDescription],[MiscAmount],[MarkupType],[MarkupPct],[MarkupDlr],[MiscAmountTaxable]) 
            VALUES(@ProgramYear,@DistrictID,@InvoiceNumber,
            (SELECT ISNULL(MAX([LineNumber]),0)+100 FROM [SPFNInvoiceLines] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber)))
            ,@MiscDescription,@MiscAmount,@MarkupType,@MarkupPct,@MarkupDlr,@MiscAmountTaxable)"
                    
            UpdateCommand="UPDATE [SPFNInvoiceLines] SET [MiscDescription]=@MiscDescription, [MiscAmount]=@MiscAmount, [MarkupType]=@MarkupType, [MarkupPct]=@MarkupPct, [MarkupDlr]=@MarkupDlr, [MiscAmountTaxable]=@MiscAmountTaxable
            WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([InvoiceNumber] = @InvoiceNumber) 
            AND ([LineNumber] = @LineNumber))"
                    
            OnInserting="SqlDataSourceInvLines_Inserting" OnInserted="SqlDataSourceInvLines_Inserted" OnDeleted="SqlDataSourceInvLines_Deleted" OnUpdated="SqlDataSourceInvLines_Updated" >
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
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
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:Parameter Name="InvoiceNumber" Type="String" />
                <asp:Parameter Name="MiscDescription" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="MiscAmount" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="MarkupType" Type="Single" DefaultValue="0" />
                <asp:Parameter Name="MarkupPct" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="MarkupDlr" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="MiscAmountTaxable" Type="Byte" DefaultValue="0" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:Parameter Name="InvoiceNumber" Type="String" />
                <asp:Parameter Name="MiscDescription" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="MiscAmount" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="MarkupType" Type="Int16" DefaultValue="0" />
                <asp:Parameter Name="MarkupPct" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="MarkupDlr" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="MiscAmountTaxable" Type="Byte" DefaultValue="0" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourcePayAuth" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
            SelectCommand="SELECT ISNULL(Auth.[ProgramYear], @ProgramYear) ProgramYear,ISNULL(Auth.[DistrictID], @DistrictID) DistrictID,ISNULL(Auth.[InvoiceNumber], @InvoiceNumber) InvoiceNumber,
            ISNULL(Auth.[PaymentAuthNumber],@PaymentAuthNumber) PaymentAuthNumber,ISNULL(Auth.[RMNumber], DYI.RMNumber) RMNumber,ISNULL(Auth.[PaymentAuthStatus],0) PaymentAuthStatus,
            ISNULL(Auth.[FSDHourlyRate],TR.SpFnDFNRate) FSDHourlyRate,ISNULL(Auth.[Hours],0) hours,ISNULL(Auth.[TNGFringe],TR.TNGFringe) TNGFringe, ISNULL(HDR.Locked, CAST(0 AS BIT)) Locked,
            TotalWages, TNGFringeTotal, TotalWages + TNGFringeTotal TotalFSDLabor, RMEmail
              FROM District D
              LEFT OUTER JOIN [SPFNPaymentAuth] Auth ON @ProgramYear=Auth.ProgramYear AND @DistrictID=Auth.DistrictID AND @InvoiceNumber=Auth.InvoiceNumber AND @PaymentAuthNumber=Auth.PaymentAuthNumber
              LEFT OUTER JOIN SPFNInvoiceHDR HDR ON @ProgramYear=HDR.ProgramYear AND @DistrictID=HDR.DistrictID AND @InvoiceNumber=HDR.InvoiceNumber
              LEFT OUTER JOIN [DistrictYearInfo] DYI ON @ProgramYear=DYI.ProgramYear AND @DistrictID=DYI.DistrictID
              LEFT OUTER JOIN [RMSetup] RM ON DYI.RMNumber=RM.RMNumber
              LEFT OUTER JOIN [TaxRates] TR ON @ProgramYear=TR.ProgramYear
              CROSS APPLY (SELECT ISNULL(Auth.[FSDHourlyRate],0)*ISNULL(Auth.[Hours],0) TotalWages,  ISNULL(Auth.[FSDHourlyRate],0)*ISNULL(Auth.[Hours],0)*ISNULL(Auth.[TNGFringe],TR.TNGFringe) TNGFringeTotal) CALC1
              WHERE D.DistrictID=@DistrictID">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:ControlParameter Name="InvoiceNumber" ControlID="HFInvoicenumber" PropertyName="Value" Type="String" />
                <asp:Parameter Name="PaymentAuthNumber" Type="Int32" DefaultValue="1" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceEmailSettings" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT * FROM [PaperworkWebEmailSettings] WHERE ([Index] = @Index)">
            <SelectParameters>
                <asp:Parameter DefaultValue="0" Name="Index" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>

    <div class="modal fade" id="myModal" >  
        <div class="modal-dialog" >  
            <div class="modal-content" style="width:1000px;" > 
                <div class="modal-header">  
                    <h4 class="modal-title">Email Special Functions Invoice</h4>  
                    <button type="button" class="close" data-dismiss="modal">×</button>  
                </div>  
                <div class="modal-body">  
                    <div class="row">  
                        <div class="col-md-12">  
                            <div class="form-group">  
                                <label id="LabelEmailHeader">Enter a Nutrition Group email below and click send</label>  
                                <div class="input-group">  
                                    <asp:UpdatePanel ID="UpdatePanelModal" UpdateMode="Conditional" runat="server">
                                        <ContentTemplate>
                                            <div class="EmailRecipient" style="width:800px;"  > 
                                                <asp:TextBox ID="TextBoxEmailTo" runat="server" CssClass="textboxlong" AutoCompleteType="Email" ValidationGroup="email" AutoPostBack="true" OnTextChanged="TextBoxEmailTo_TextChanged" ></asp:TextBox>
                                                <br />
                                                <asp:RegularExpressionValidator ID="regexEmailValid" CssClass="validators" runat="server"
                                                    ValidationExpression="^(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))*$" ControlToValidate="TextBoxEmailTo" ErrorMessage="Invalid Email Format" ValidationGroup="Email"></asp:RegularExpressionValidator>
                                                <br />
                                                <asp:CustomValidator ID="customEmailValidator" runat="server" CssClass="validators" ControlToValidate="TextBoxEmailTo" ValidationGroup="Email"></asp:CustomValidator>
                                            </div>  
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>  
                            </div>  
                        </div>  
                    </div>  
                </div>  
                <div class="modal-footer">  
                    <asp:Button ID="ButtonSend" runat="server" CssClass="btn btn-outline-primary" Text="Send Email" OnClick="ButtonSend_Click" CausesValidation="true" 
                        ValidationGroup="Email"  /> <!-- OnClientClick="closeModal('myModal');" -->
                    <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Close</button>  
                </div>  
            </div>  
        </div>  
    </div>  


</asp:Content>
