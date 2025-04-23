<%@ Page Title="Sales Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalesEntry.aspx.cs" Inherits="Paperwork.SalesEntry" %>
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
            $(function () {
                $( "[id*=TrxDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    beforeShowDay: highlightDays
                });
            });

            updateDatesArray();

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            /*BREAKFAST SECTION*/

            $("[id*=TextBoxBPaidElem],[id*=TextBoxBPaidJr],[id*=TextBoxBPaidSr],[id*=TextBoxBPaidLrn]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcPaidB();

                if (parseFloat($("[id*=TextBoxBPaidTtl]").val()) > parseFloat($("[id*=TextBoxPaidElig]").val())) {
                    $("[id*=TextBoxBPaidTtl]").addClass("invalidCount");
                    //alert("The Paid Breakfast count is more than the eligible#. Please correct.");
                }
                else 
                    $("[id*=TextBoxBPaidTtl]").removeClass("invalidCount");
            });

            $("[id*=TextBoxBPaidElem],[id*=TextBoxBPaidJr],[id*=TextBoxBPaidSr],[id*=TextBoxBPaidLrn]").change(function () {

            })

            $("[id*=TextBoxBReduced],[id*=TextBoxBReducedLrn]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcReducedB();
                RecalcMealSaleB();

                if (parseFloat($("[id*=TextBoxBReducedTtl]").val()) > parseFloat($("[id*=TextBoxReducedElig]").val())) {
                    $("[id*=TextBoxBReducedTtl]").addClass("invalidCount");
                    //alert("The Reduced Breakfast count is more than the eligible#. Please correct.");
                }
                else 
                    $("[id*=TextBoxBReducedTtl]").removeClass("invalidCount");
            });

            $("[id*=TextBoxBReduced],[id*=TextBoxBReducedLrn]").change(function () {

            })

            $("[id*=TextBoxBFree],[id*=TextBoxBFreeLrn]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcFreeB();

                if (parseFloat($("[id*=TextBoxBFreeTtl]").val()) > parseFloat($("[id*=TextBoxFreeElig]").val())) {
                    $("[id*=TextBoxBFreeTtl]").addClass("invalidCount");
                    //alert("The Free Breakfast count is more than the eligible#. Please correct.");
                }
                else 
                    $("[id*=TextBoxBFreeTtl]").removeClass("invalidCount");
            });

            $("[id*=TextBoxBFree],[id*=TextBoxBFreeLrn]").change(function () {

            });

            $("[id*=TextBoxBStuAla],[id*=TextBoxBAdltAla]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcAlaB();
            });

            /*LUNCH SECTION*/
            $("[id*=TextBoxLPaidElem],[id*=TextBoxLPaidJr],[id*=TextBoxLPaidSr],[id*=TextBoxLPaidLrn],[id*=TextBoxLTier1],[id*=TextBoxLTier2]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcPaidL();

                if (parseFloat($("[id*=TextBoxLPaidTtl]").val()) > parseFloat($("[id*=TextBoxPaidElig]").val())) {
                    $("[id*=TextBoxLPaidTtl]").addClass("invalidCount");
                    //alert("The Paid Lunch count is more than the eligible#. Please correct.");
                }
                else 
                    $("[id*=TextBoxLPaidTtl]").removeClass("invalidCount");
            });

            $("[id*=TextBoxLPaidElem],[id*=TextBoxLPaidJr],[id*=TextBoxLPaidSr],[id*=TextBoxLPaidLrn],[id*=TextBoxLTier1],[id*=TextBoxLTier2]").change(function () {

            });

            $("[id*=TextBoxLReduced],[id*=TextBoxLReducedLrn],[id*=TextBoxLReducedT1]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcReducedL();

                if (parseFloat($("[id*=TextBoxLReducedTtl]").val()) > parseFloat($("[id*=TextBoxReducedElig]").val())) {
                    $("[id*=TextBoxLReducedTtl]").addClass("invalidCount");
                    //alert("The Reduced Lunch count is more than the eligible#. Please correct.");
                }
                else 
                    $("[id*=TextBoxLReducedTtl]").removeClass("invalidCount");
            });

            $("[id*=TextBoxLReduced],[id*=TextBoxLReducedLrn],[id*=TextBoxLReducedT1]").change(function () {

            });

            $("[id*=TextBoxLFree],[id*=TextBoxLFreeLrn],[id*=TextBoxLFreeT1],[id*=TextBoxLFreeT2]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                RecalcFreeL();

                if (parseFloat($("[id*=TextBoxLFreeTtl]").val()) > parseFloat($("[id*=TextBoxFreeElig]").val())) {
                    $("[id*=TextBoxLFreeTtl]").addClass("invalidCount");
                    //alert("The Free Lunch count is more than the eligible#. Please correct.");
                }
                else 
                    $("[id*=TextBoxLFreeTtl]").removeClass("invalidCount");
            });

            $("[id*=TextBoxLFree],[id*=TextBoxLFreeLrn],[id*=TextBoxLFreeT1],[id*=TextBoxLFreeT2]").change(function () {
                
            });

            $("[id*=TextBoxLStuAla],[id*=TextBoxLAdltAla]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcAlaL();

            });

            $("[id*=TextBoxLOtherAla1],[id*=TextBoxLVendAla]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcOtherAlaL();
            });

            /*OTHER Section*/
            $("[id*=TextBoxOSnack],[id*=TextBoxOReducedSnack],[id*=TextBoxOFreeSnack]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }

                //recalc snack hidden field
                var SnackTTL=
                    Math.round(((
                        ( parseFloat($("[id*=TextBoxOSnack]").val()) * parseFloat($("[id*=HFSnackPaidPrice]").val()))
                        + ( parseFloat($("[id*=TextBoxOReducedSnack]").val()) * parseFloat($("[id*=HFSnackReducedPrice]").val()))
                    ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

                if (SnackTTL == 0)
                    $("[id*=HFTotalSnackSale]").val('0');
                else
                    $("[id*=HFTotalSnackSale]").val(SnackTTL.toFixed(2).toString());

                RecalcTotalSales();
            });

            $("[id*=TextBoxOSFSale],[id*=TextBoxOVendComm],[id*=TextBoxUSDASale],[id*=TextBoxOSale1],[id*=TextBoxOSale2]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcTotalSales();
            });

            /*Misc*/
            $("[id*=TextBoxBDeposit],[id*=TextBoxLDeposit]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcDeposit();
            });

            $("[id*=TextBoxBAdvanced],[id*=TextBoxLAdvanced]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcAdvances();
            });

            //Tom had thes fields so that you could just enter a value and it's flipped negative
            $("[id*=TextBoxBRedeemed],[id*=TextBoxLRedeemed],[id*=TextBoxBCharges],[id*=TextBoxLCharges]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    var fieldVal = parseFloat($(this).val())

                    if (fieldVal > 0) {
                        $(this).val( fieldVal * -1);
                    }
                }

                RecalcRedemptions();
                RecalcCharges();
            });

            $("[id*=TextBoxBRedeemed],[id*=TextBoxLRedeemed]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcRedemptions();
            });

            $("[id*=TextBoxBCharges],[id*=TextBoxLCharges]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcCharges();
            });

            $("[id*=TextBoxBRepaid],[id*=TextBoxLRepaid]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcRepayments();
            });

            $("[id*=TextBoxOSFCollect],[id*=TextBoxUSDAColl],[id*=TextBoxOColl1],[id*=TextBoxOColl2]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcCollections();
            });

            $("[id*=TextBoxAdjustment],[id*=TextBoxOnlineAdv]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcOverUnder();
            });

        }

        /* Breakfast Functions */
        function RecalcPaidB() {
            var BPaidTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBPaidElem]").val())
                    + parseFloat($("[id*=TextBoxBPaidJr]").val())
                    + parseFloat($("[id*=TextBoxBPaidSr]").val())
                    + parseFloat($("[id*=TextBoxBPaidLrn]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (BPaidTTL == 0)
                $("[id*=TextBoxBPaidTtl]").val('0');
            else
                $("[id*=TextBoxBPaidTtl]").val(BPaidTTL.toString());
            RecalcTotalB();
            RecalcMealSaleB();
        };

        function RecalcReducedB() {
            var BRedTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBReduced]").val())
                    + parseFloat($("[id*=TextBoxBReducedLrn]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100


            if (BRedTTL == 0)
                $("[id*=TextBoxBReducedTtl]").val('0');
            else
                $("[id*=TextBoxBReducedTtl]").val(BRedTTL.toString());
            RecalcTotalB();
        };

        function RecalcFreeB() {
            var BFreeTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBFree]").val())
                    + parseFloat($("[id*=TextBoxBFreeLrn]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100


            if (BFreeTTL == 0)
                $("[id*=TextBoxBFreeTtl]").val('0');
            else
                $("[id*=TextBoxBFreeTtl]").val(BFreeTTL.toString());
            RecalcTotalB();

        };

        function RecalcTotalB() {
            var BTotal=
                Math.round(((
                    parseFloat($("[id*=TextBoxBPaidTtl]").val())
                    + parseFloat($("[id*=TextBoxBReducedTtl]").val())
                    + parseFloat($("[id*=TextBoxBFreeTtl]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100


            if (BTotal == 0)
                $("[id*=TextBoxBTotal]").val('0');
            else
                $("[id*=TextBoxBTotal]").val(BTotal.toString());
        };

        function RecalcAlaB() {
            var BAlaTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBStuAla]").val())
                    + parseFloat($("[id*=TextBoxBAdltAla]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100


            if (BAlaTTL == 0)
                $("[id*=TextBoxBAlaTtl]").val('0.00');
            else
                $("[id*=TextBoxBAlaTtl]").val(BAlaTTL.toFixed(2).toString());

            //Ala is part of total Sales
            RecalcTotalSales();
        };

        function RecalcMealSaleB() {
            //alert($("[id*=HFBPaidElemRate]").text());
            var BMealSaleTTL=
                Math.round(((
                    (parseFloat($("[id*=TextBoxBPaidElem]").val()) * parseFloat($("[id*=HFBPaidElemPrice]").val()))
                    + ( parseFloat($("[id*=TextBoxBPaidJr]").val()) * parseFloat($("[id*=HFBPaidJrPrice]").val()))
                    + ( parseFloat($("[id*=TextBoxBPaidSr]").val()) * parseFloat($("[id*=HFBPaidSrPrice]").val()))
                    + ( parseFloat($("[id*=TextBoxBReduced]").val()) * parseFloat($("[id*=HFBRedPrice]").val()))
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (BMealSaleTTL == 0)
                $("[id*=TextBoxBMealSaleTtl]").val('0.00');
            else
                $("[id*=TextBoxBMealSaleTtl]").val(BMealSaleTTL.toFixed(2).toString());

            RecalcTotalSales();
        };

        /* Lunch Functions */
        function RecalcPaidL() {
            var LPaidTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxLPaidElem]").val())
                    + parseFloat($("[id*=TextBoxLPaidJr]").val())
                    + parseFloat($("[id*=TextBoxLPaidSr]").val())
                    + parseFloat($("[id*=TextBoxLPaidLrn]").val())
                    + parseFloat($("[id*=TextBoxLTier1]").val())
                    + parseFloat($("[id*=TextBoxLTier2]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100


            if (LPaidTTL == 0)
                $("[id*=TextBoxLPaidTtl]").val('0');
            else
                $("[id*=TextBoxLPaidTtl]").val(LPaidTTL.toString());
            RecalcTotalL();
            RecalcMealSaleL();

        };

        function RecalcReducedL() {
            var LRedTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxLReduced]").val())
                    + parseFloat($("[id*=TextBoxLReducedLrn]").val())
                    + parseFloat($("[id*=TextBoxLReducedT1]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100


            if (LRedTTL == 0)
                $("[id*=TextBoxLReducedTtl]").val('0');
            else
                $("[id*=TextBoxLReducedTtl]").val(LRedTTL.toString());
            RecalcTotalL();
            RecalcMealSaleL();
        };

        function RecalcFreeL() {
            var LFreeTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxLFree]").val())
                    + parseFloat($("[id*=TextBoxLFreeLrn]").val())
                    + parseFloat($("[id*=TextBoxLFreeT1]").val())
                    + parseFloat($("[id*=TextBoxLFreeT2]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100


            if (LFreeTTL == 0)
                $("[id*=TextBoxLFreeTtl]").val('0');
            else
                $("[id*=TextBoxLFreeTtl]").val(LFreeTTL.toString());
            RecalcTotalL();
        };

        function RecalcTotalL() {
            var LTotal=
                Math.round(((
                    parseFloat($("[id*=TextBoxLPaidTtl]").val())
                    + parseFloat($("[id*=TextBoxLReducedTtl]").val())
                    + parseFloat($("[id*=TextBoxLFreeTtl]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100


            if (LTotal == 0)
                $("[id*=TextBoxLTotal]").val('0');
            else
                $("[id*=TextBoxLTotal]").val(LTotal.toString());
        };

        function RecalcAlaL() {
            var LAlaTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxLStuAla]").val())
                    + parseFloat($("[id*=TextBoxLAdltAla]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (LAlaTTL == 0)
                $("[id*=TextBoxLAlaTtl]").val('0.00');
            else
                $("[id*=TextBoxLAlaTtl]").val(LAlaTTL.toFixed(2).toString());

            //Ala is part of total Sales
            RecalcTotalSales();
        };

        function RecalcOtherAlaL() {
            var LOthAlaTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxLOtherAla1]").val())
                    + parseFloat($("[id*=TextBoxLVendAla]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (LOthAlaTTL == 0)
                $("[id*=TextBoxLOtherAlaTtl]").val('0.00');
            else
                $("[id*=TextBoxLOtherAlaTtl]").val(LOthAlaTTL.toFixed(2).toString());

            //Ala is part of total Sales
            RecalcTotalSales();
        };

        function RecalcMealSaleL() {
                var LMealSaleTTL=
                    Math.round(((
                        (parseFloat($("[id*=TextBoxLPaidElem]").val()) * parseFloat($("[id*=HFLPaidElemPrice]").val()))
                        + ( parseFloat($("[id*=TextBoxLPaidJr]").val()) * parseFloat($("[id*=HFLPaidJrPrice]").val()))
                        + ( parseFloat($("[id*=TextBoxLPaidSr]").val()) * parseFloat($("[id*=HFLPaidSrPrice]").val()))
                        + ( parseFloat($("[id*=TextBoxLTier1]").val()) * parseFloat($("[id*=HFLPaidT1Price]").val()))
                        + ( parseFloat($("[id*=TextBoxLTier2]").val()) * parseFloat($("[id*=HFLPaidT2Price]").val()))
                        + (( parseFloat($("[id*=TextBoxLReduced]").val()) + parseFloat($("[id*=TextBoxLReducedT1]").val()) ) * parseFloat($("[id*=HFLRedPrice]").val()))
                    ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

                if (LMealSaleTTL == 0)
                    $("[id*=TextBoxLMealSale]").val('0.00');
                else
                    $("[id*=TextBoxLMealSale]").val(LMealSaleTTL.toFixed(2).toString());

                RecalcTotalSales();
            };

        /* Grand Totals */
        function RecalcDeposit() {
            var DepositTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBDeposit]").val())
                    + parseFloat($("[id*=TextBoxLDeposit]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (DepositTTL == 0)
                $("[id*=TextBoxTotalDeposit]").val('0.00');
            else
                $("[id*=TextBoxTotalDeposit]").val(DepositTTL.toFixed(2).toString());
            RecalcOverUnder();
        };

        function RecalcAdvances() {
            var AdvancesTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBAdvanced]").val())
                    + parseFloat($("[id*=TextBoxLAdvanced]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (AdvancesTTL == 0)
                $("[id*=HFAdvances]").val('0.00');
            else
                $("[id*=HFAdvances]").val(AdvancesTTL.toString());

            RecalcOverUnder();
        };

        function RecalcRedemptions() {
            var RedemptionsTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBRedeemed]").val())
                    + parseFloat($("[id*=TextBoxLRedeemed]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (RedemptionsTTL == 0)
                $("[id*=HFRedemptions]").val('0.00');
            else
                $("[id*=HFRedemptions]").val(RedemptionsTTL.toString());

            RecalcOverUnder();
        };

        function RecalcCharges() {
            var ChargesTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBCharges]").val())
                    + parseFloat($("[id*=TextBoxLCharges]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (ChargesTTL == 0)
                $("[id*=HFCharges]").val('0.00');
            else
                $("[id*=HFCharges]").val(ChargesTTL.toString());

            RecalcOverUnder();
        };

        function RecalcRepayments() {
            var RepaymentsTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBRepaid]").val())
                    + parseFloat($("[id*=TextBoxLRepaid]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (RepaymentsTTL == 0)
                $("[id*=HFRepayments]").val('0.00');
            else
                $("[id*=HFRepayments]").val(RepaymentsTTL.toString());

            RecalcOverUnder();
        };

        function RecalcCollections() {
            var CollectionsTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxOSFCollect]").val())
                    + parseFloat($("[id*=TextBoxUSDAColl]").val())
                    + parseFloat($("[id*=TextBoxOColl1]").val())
                    + parseFloat($("[id*=TextBoxOColl2]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (CollectionsTTL == 0)
                $("[id*=HFCollections]").val('0.00');
            else
                $("[id*=HFCollections]").val(CollectionsTTL.toString());

            RecalcOverUnder();
        };

        function RecalcTotalSales() {
            var SalesTTL=
                Math.round(((
                    parseFloat($("[id*=TextBoxBMealSale]").val())
                    + parseFloat($("[id*=TextBoxLMealSale]").val())
                    + parseFloat($("[id*=TextBoxBAlaTtl]").val())
                    + parseFloat($("[id*=TextBoxLAlaTtl]").val())
                    + parseFloat($("[id*=TextBoxLOtherAlaTtl]").val())
                    + parseFloat($("[id*=TextBoxOSFSale]").val()) 
                    + parseFloat($("[id*=TextBoxOVendComm]").val())
                    + parseFloat($("[id*=TextBoxUSDASale]").val())
                    + parseFloat($("[id*=TextBoxOSale1]").val())
                    + parseFloat($("[id*=TextBoxOSale2]").val())
                    + parseFloat($("[id*=HFTotalSnackSale]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (SalesTTL == 0)
                $("[id*=TextBoxTotalSales]").val('0.00');
            else
                $("[id*=TextBoxTotalSales]").val(SalesTTL.toFixed(2).toString());
            RecalcOverUnder();
        };

        function RecalcOverUnder() {
            var OverUnderTTL=Math.round(((
                    parseFloat($("[id*=TextBoxTotalDeposit]").val())
                    - parseFloat($("[id*=TextBoxTotalSales]").val())
                    - parseFloat($("[id*=HFAdvances]").val())
                    - parseFloat($("[id*=HFRedemptions]").val())
                    - parseFloat($("[id*=HFCharges]").val())
                    - parseFloat($("[id*=HFRepayments]").val())
                    - parseFloat($("[id*=TextBoxOSFCollect]").val())
                    - parseFloat($("[id*=TextBoxUSDAColl]").val())
                    - parseFloat($("[id*=TextBoxOColl1]").val())
                    - parseFloat($("[id*=TextBoxOColl2]").val())
                    + parseFloat($("[id*=TextBoxOSFSale]").val())
                    + parseFloat($("[id*=TextBoxUSDASale]").val())
                    + parseFloat($("[id*=TextBoxOSale1]").val())
                    + parseFloat($("[id*=TextBoxOSale2]").val())
                    - parseFloat($("[id*=TextBoxAdjustment]").val())
                    + parseFloat($("[id*=TextBoxOnlineAdv]").val())
            ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (OverUnderTTL == 0)
                $("[id*=TextBoxTotalCash]").val('0.00');
            else
                $("[id*=TextBoxTotalCash]").val(OverUnderTTL.toFixed(2).toString());
        };

        //For initial load - recalc all totals
        function RecalcAll() {
            RecalcPaidB();
            RecalcReducedB();
            RecalcFreeB();
            RecalcAlaB();
            RecalcMealSaleB();

            RecalcPaidL();
            RecalcReducedL();
            RecalcFreeL();
            RecalcAlaL();
            RecalcOtherAlaL();

            RecalcDeposit();
            RecalcAdvances();
            RecalcRedemptions();
            RecalcCharges();
            RecalcRepayments();
            RecalcCollections();
            RecalcOverUnder();
        };

        //Date Functions for colorized calendar
        var dateArray;
        var dateArrayLocked;

        function highlightDays(date) {
            //if no dates, return immediately
            if (dateArray[0] != "") {
                for (var i = 0; i < dateArray.length; i++) {
                    //convert string element to date
                    var d = $.datepicker.parseDate("mm/dd/yy", dateArray[i]);

                    if (
                        (d.getDate() == date.getDate()) &&
                        (d.getMonth() == date.getMonth()) &&
                        (d.getFullYear() == date.getFullYear())
                    ) {
                        return [true, 'calendarHighlighted', 'Sales Entered'];
                    }
                }
            }

            if (dateArrayLocked[0] != "") {
                for (var i = 0; i < dateArrayLocked.length; i++) {
                    //convert string element to date
                    var d = $.datepicker.parseDate("mm/dd/yy", dateArrayLocked[i]);

                    if (
                        (d.getDate() == date.getDate()) &&
                        (d.getMonth() == date.getMonth()) &&
                        (d.getFullYear() == date.getFullYear())
                    ) {
                        return [true, 'calendarLocked', 'Sales Entered (Locked)'];
                    }
                }
            }
                

            
            return [true, '', ''];
        };

        function updateDatesArray() {
            dateArray = $('[id*=HFEnteredSchoolDaysArray]').val().split(',');
            dateArrayLocked = $('[id*=HFLockedSchoolDaysArray]').val().split(',');
        };


    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"  >
        <ContentTemplate>
            <div id="topSection">
                <div id="hdrgroup">
                    <div id="hdrDiv" style="float:left;width:25%">
                        <h2><%: Title %></h2>
                        <asp:LinkButton ID="ISave" runat="server" 
                                    ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                    CssClass="fas fa-save mGridLinkButton" Width="170"></asp:LinkButton>
                    </div>
                    <div id="menuDiv" >
                        <!-- Using the following hidden fields to act as a source to colorize the calendar days which are entered already-->
                        <asp:HiddenField ID="HFEnteredSchoolDaysArray" runat="server" />
                        <asp:HiddenField ID="HFLockedSchoolDaysArray" runat="server" />
                        <p>
                            <strong>Program Year:</strong>
                            <asp:Label ID="LabelProgramYear" runat="server"></asp:Label>
                            &nbsp;&nbsp; 
                            <strong>Date:</strong>
                            <asp:TextBox ID="TrxDate" runat="server" Width="100" AutoPostBack="true" OnTextChanged="TrxDate_TextChanged"></asp:TextBox>
                        </p>
                        <p>
                            <strong>District:</strong>
                            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                DataSourceID="SqlDataSourceMyDistricts" DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> &nbsp;

                            <strong>School:</strong><asp:DropDownList ID="DropDownListSchools" runat="server" AutoPostBack="True" 
                                DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" DataValueField="SchoolNum" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListSchools_SelectedIndexChanged">
                            </asp:DropDownList> &nbsp;
                        </p>
                    </div>
                </div>
            </div>

            <div id="entryDiv" style="position:relative;"  >
                <div style="float:left;width:75%;">



                    <asp:FormView ID="FormViewSales" runat="server" CssClass="mGridRecaps" DataSourceID="SqlDataSourceSales" DefaultMode="Edit" OnPreRender="FormViewSales_PreRender" OnDataBound="FormViewSales_DataBound" >
                        <EditItemTemplate>
                            <div class="container">
                                <div class="row">
                                    <div class="col-sm-3">
                                        <asp:CheckBox ID="CBBreakfast" runat="server" Text="Breakfast Day?" Checked='<%# Eval("BreakfastDay").ToString()=="1"?true:false %>' OnCheckedChanged="SalesEntryDataChanged" Enabled='<%# Eval("Breakfasts") %>' />
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:CheckBox ID="CheckBoxLunch" runat="server" Text="Lunch Day?" Checked='<%# Eval("LunchDay").ToString()=="1"?true:false %>' OnCheckedChanged="SalesEntryDataChanged" />
                                    </div>
                                    <div class="col-sm-3" style="color:red;font-weight:bold;">
                                        <asp:Label ID="LabelLocked" runat="server" Text="** LOCKED **" Visible='<%# (bool)Eval("Locked") %>'></asp:Label>
                                    </div>
                                    <asp:FormView ID="FormViewRates" runat="server" DataSourceID="SqlDataSourceSchoolYearSetup" DefaultMode="ReadOnly" >
                                        <ItemTemplate>
                                            <asp:HiddenField ID="HFSchoolNum" runat="server" Value='<%# Eval("SchoolNum") %>' />
                                            <!-- Breakfast Prices -->
                                            <asp:HiddenField id="HFBPaidElemPrice" runat="server" Value='<%# Eval("BfastElPrice") %>' ></asp:HiddenField>
                                            <asp:HiddenField id="HFBPaidJrPrice" runat="server" Value='<%# Eval("BfastJrPrice") %>' ></asp:HiddenField>
                                            <asp:HiddenField id="HFBPaidSrPrice" runat="server" Value='<%# Eval("BfastSrPrice") %>' ></asp:HiddenField>
                                            <asp:HiddenField id="HFBRedPrice" runat="server" Value='<%# Eval("BfastRedPrice") %>' ></asp:HiddenField>
                                            <!-- LUNCH Prices -->
                                            <asp:HiddenField id="HFLPaidElemPrice" runat="server" Value='<%# Eval("LunchElPrice") %>' ></asp:HiddenField>
                                            <asp:HiddenField id="HFLPaidJrPrice" runat="server" Value='<%# Eval("LunchJrPrice") %>' ></asp:HiddenField>
                                            <asp:HiddenField id="HFLPaidSrPrice" runat="server" Value='<%# Eval("LunchSrPrice") %>' ></asp:HiddenField>
                                            <asp:HiddenField id="HFLPaidT1Price" runat="server" Value='<%# Eval("LunchTier1Price") %>' ></asp:HiddenField>
                                            <asp:HiddenField id="HFLPaidT2Price" runat="server" Value='<%# Eval("LunchTier2Price") %>' ></asp:HiddenField>
                                            <asp:HiddenField id="HFLRedPrice" runat="server" Value='<%# Eval("LunchRedPrice") %>' ></asp:HiddenField>
                                            <!-- OTHER Prices -->
                                            <asp:HiddenField id="HFSnackPaidPrice" runat="server" Value='<%# Eval("OSnackPaidPrice") %>' ></asp:HiddenField>
                                            <asp:HiddenField id="HFSnackReducedPrice" runat="server" Value='<%# Eval("OSnackRedPrice") %>' ></asp:HiddenField>
                                            <!-- Save Indicators -->
                                            <asp:HiddenField ID="HFDistSchoolHdrSaved" runat="server" Value="0" />
                                            <asp:HiddenField ID="HFDistSchoolDaySaved" runat="server" Value="0" />
                                        </ItemTemplate>
                                    </asp:FormView>
                                </div>
                                <div class="row">
                                    <div class="col-sm-3">
                                        Enrollment:
                                    </div>
                                    <div class="col-sm-3">
                                        Attendance Factor:
                                    </div>
                                    <div class="col-sm-3">
                                        Breakfast Incentive:
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxEnrollment" runat="server" Text='<%# Bind("R_Enrollment") %>' Width="50" OnTextChanged="TextBoxSchoolHeader_TextChanged" />
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxAttFactor" runat="server" Text='<%# Bind("R_AF", "{0:P1}") %>' Width="75" OnTextChanged="TextBoxSchoolHeader_TextChanged" />
                                    </div>
                                    <div class="col-sm-3">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxBfastIncentive" runat="server" Text='<%# Bind("R_BreakfastIncent") %>' Width="50" OnTextChanged="TextBoxSchoolHeader_TextChanged" />
                                    </div>
                                </div>
                                <div class="row"  >
                                    <div class="col-sm-3" >
                                        Paid Eligible:
                                    </div>
                                    <div class="col-sm-3">
                                        Reduced Eligible:
                                    </div>
                                    <div class="col-sm-3">
                                        Free Eligible:
                                    </div>
                                </div>
                                <div class="row" >
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxPaidElig" runat="server" Text='<%# Bind("R_EligiblePaid") %>' Width="50" OnTextChanged="TextBoxSchoolHeader_TextChanged" />
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxReducedElig" runat="server" Text='<%# Bind("R_EligibleReduced") %>' Width="50" OnTextChanged="TextBoxSchoolHeader_TextChanged" />
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="TextBoxFreeElig" runat="server" Text='<%# Bind("R_EligibleFree") %>' Width="50" OnTextChanged="TextBoxSchoolHeader_TextChanged" />
                                    </div>
                                </div>
                                <div class="row" style="border-bottom-color:deepskyblue;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;" <%# (bool)Eval("Breakfasts")==true?" ":"hidden='hidden'" %>>
                                    <div class="col-sm-2" style="background-color:deepskyblue;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Breakfast
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("Breakfasts")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-2">
                                        Paid Elem:
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Jr:
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Sr:
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced:
                                    </div>
                                    <div class="col-sm-2">
                                        Free:
                                    </div>
                                    <div class="col-sm-2">
                                        
                                        
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("Breakfasts")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxBPaidElem" runat="server" Text='<%# Eval("BPaidElem") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>'  />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxBPaidJr" runat="server" Text='<%# Eval("BPaidJr") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxBPaidSr" runat="server" Text='<%# Eval("BPaidSr") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxBReduced" runat="server" Text='<%# Eval("BReduced") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxBFree" runat="server" Text='<%# Eval("BFree") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        
                                        
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("Breakfasts")==true?" ":"hidden='hidden'" %>>
                                    <div class="col-sm-2" <%# (bool)Eval("Learners")==true?" ":"hidden='hidden'" %> >
                                        Paid Learn:
                                    </div>
                                    <div class="col-sm-2" style="text-wrap:avoid;" <%# (bool)Eval("Learners")==true?" ":"hidden='hidden'" %> >
                                        Reduced Learn:
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Learners")==true?" ":"hidden='hidden'" %> >
                                        Free Learn:
                                    </div>
                                    <div class="col-sm-2">
                                        Student Ala:
                                    </div>
                                    <div class="col-sm-2">
                                        Adult Ala:
                                    </div>
                                    <div class="col-sm-2">
                                        
                                        
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("Breakfasts")==true?" ":"hidden='hidden'" %>>
                                    <div class="col-sm-2" <%# (bool)Eval("Learners")==true?" ":"hidden='hidden'" %> >
                                        <asp:TextBox ID="TextBoxBPaidLrn" runat="server" Text='<%# Eval("BPaidLrn") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && (bool)Eval("Learners") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2" style="text-wrap:avoid;" <%# (bool)Eval("Learners")==true?" ":"hidden='hidden'" %> >
                                        <asp:TextBox ID="TextBoxBReducedLrn" runat="server" Text='<%# Eval("BReducedLrn") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && (bool)Eval("Learners") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Learners")==true?" ":"hidden='hidden'" %> >
                                        <asp:TextBox ID="TextBoxBFreeLrn" runat="server" Text='<%# Eval("BFreeLrn") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && (bool)Eval("Learners") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxBStuAla" runat="server"  Text='<%# Eval("BStuAla", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxBAdltAla" runat="server"  Text='<%# Eval("BAdultAla", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        
                                        
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("Breakfasts")==true?" ":"hidden='hidden'" %>>
                                    <div class="col-sm-2">
                                        Advanced:
                                    </div>
                                    <div class="col-sm-2">
                                        Redeemed:
                                    </div>
                                    <div class="col-sm-2">
                                        Repaid:
                                    </div>
                                    <div class="col-sm-2">
                                        Charges:
                                    </div>
                                    <div class="col-sm-2">
                                        Deposit:
                                    </div>
                                    <div class="col-sm-2">
                                        
                                        
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("Breakfasts")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxBAdvanced" runat="server"  Text='<%# Eval("BAdvanced", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# Eval("Breakfasts") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxBRedeemed" runat="server"  Text='<%# Eval("BRedeemed", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# Eval("Breakfasts") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxBRepaid" runat="server"  Text='<%# Eval("BRepaid", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# Eval("Breakfasts") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxBCharges" runat="server"  Text='<%# Eval("BCharges", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# Eval("Breakfasts") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div>
                                            <div style="position:absolute;left:5px;">$</div>
                                            <div style="position:absolute;">
                                                <asp:TextBox ID="TextBoxBDeposit" runat="server" Text='<%# Eval("BDeposit", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# Eval("Breakfasts") %>'></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        
                                        
                                    </div>
                                </div>

                                <!-- LUNCH -->
                                <div class="row" style="border-bottom-color:turquoise;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                    <div class="col-sm-2" style="background-color:turquoise;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Lunch
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Paid Elem:
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Jr:
                                    </div>
                                    <div class="col-sm-2">
                                        Paid Sr:
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        Tier1:
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        Tier2:
                                    </div>
                                    <div class="col-sm-2">

                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxLPaidElem" runat="server" Text='<%# Eval("LPaidElem") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxLPaidJr" runat="server" Text='<%# Eval("LPaidJr") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxLPaidSr" runat="server" Text='<%# Eval("LPaidSr") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        <asp:TextBox ID="TextBoxLTier1" runat="server" Text='<%# Eval("LTier1") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Tiers") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        <asp:TextBox ID="TextBoxLTier2" runat="server" Text='<%# Eval("LTier2") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Tiers") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">

                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Reduced:
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        Tier1 Reduced:
                                    </div>
                                    <div class="col-sm-2">
                                        Free:
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        Tier1 Free:
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        Tier2 Free:
                                    </div>
                                    <div class="col-sm-2">

                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxLReduced" runat="server" Text='<%# Eval("LReduced") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        <asp:TextBox ID="TextBoxLReducedT1" runat="server" Text='<%# Eval("LReducedTier1") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Tiers") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxLFree" runat="server" Text='<%# Eval("LFree") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        <asp:TextBox ID="TextBoxLFreeT1" runat="server" Text='<%# Eval("LFreeTier1") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Tiers") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2" <%# (bool)Eval("Tiers")==true?" ":"hidden='hidden'" %> >
                                        <asp:TextBox ID="TextBoxLFreeT2" runat="server" Text='<%# Eval("LFreeTier2") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Tiers") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">

                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("Learners")==true?" ":"hidden='hidden'" %>>
                                    <div class="col-sm-2">
                                        Paid Learn:
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Learn:
                                    </div>
                                    <div class="col-sm-2">
                                        Free Learn:
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">

                                    </div>

                                </div>
                                <div class="row" <%# (bool)Eval("Learners")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxLPaidLrn" runat="server" Text='<%# Eval("LPaidLrn") %>' Width="50" OnTextChanged="SalesEntryDataChanged"  Enabled='<%# (bool)Eval("Learners") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxLReducedLrn" runat="server" Text='<%# Eval("LReducedLrn") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Learners") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxLFreeLrn" runat="server" Text='<%# Eval("LFreeLrn") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Learners") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">

                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Student Ala:
                                    </div>
                                    <div class="col-sm-2">
                                        Adult Ala:
                                    </div>
                                    <div class="col-sm-2">
                                        Other Ala 1:
                                    </div>
                                    <div class="col-sm-2">
                                        Vend Ala:
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>

                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxLStuAla" runat="server" Text='<%# Eval("LStuAla", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxLAdltAla" runat="server" Text='<%# Eval("LAdultAla", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxLOtherAla1" runat="server" Text='<%# Eval("LOtherAla1", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxLVendAla" runat="server" Text='<%# Eval("LVendAla", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Advanced:
                                    </div>
                                    <div class="col-sm-2">
                                        Redeemed:
                                    </div>
                                    <div class="col-sm-2">
                                        Repaid:
                                    </div>
                                    <div class="col-sm-2">
                                        Charges:
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxLAdvanced" runat="server" Text='<%# Eval("LAdvanced", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxLRedeemed" runat="server" Text='<%# Eval("LRedeemed", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxLRepaid" runat="server" Text='<%# Eval("LRepaid", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxLCharges" runat="server" Text='<%# Eval("LCharges", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                    <div class="col-sm-2">
                                        
                                    </div>
                                </div>
                                <!-- Other -->
                                <div class="row" style="border-bottom-color:greenyellow;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                    <div class="col-sm-2" style="background-color:greenyellow;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Other
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("Snacks")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-2">
                                        Paid Snack:
                                    </div>
                                    <div class="col-sm-2">
                                        Reduced Snack:
                                    </div>
                                    <div class="col-sm-2">
                                        Free Snack:
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("Snacks")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxOSnack" runat="server" Text='<%# Eval("OPaidSnack") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Snacks") && !(bool)Eval("Locked") %>' />
                                        <asp:HiddenField ID="HFTotalSnackSale" runat="server" Value="0" />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxOReducedSnack" runat="server" Text='<%# Eval("OReducedSnack") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Snacks") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="TextBoxOFreeSnack" runat="server" Text='<%# Eval("OFreeSnack") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Snacks") && !(bool)Eval("Locked") %>' />
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("OtherSales")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-2">
                                        Special Function Sale:
                                    </div>
                                    <div class="col-sm-2">
                                        Special Function Collect:
                                    </div>
                                    <div class="col-sm-2">
                                        Vending Comm:
                                    </div>
                                    <div class="col-sm-2">
                                        USDA Sale:
                                    </div>
                                    <div class="col-sm-2">
                                        USDA Coll:
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("OtherSales")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxOSFSale" runat="server" Text='<%# Eval("SpecialFunctionSale", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxOSFCollect" runat="server" Text='<%# Eval("SpecialFunctionColl", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxOVendComm" runat="server" Text='<%# Eval("SpecialFunctionComm", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxUSDASale" runat="server" Text='<%# Eval("USDASale", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxUSDAColl" runat="server" Text='<%# Eval("USDAColl", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("OtherSales")==true?" ":"hidden='hidden'" %>>
                                    <div class="col-sm-6 col-lg-5 ">
                                        Other1 Sale:
                                    </div>
                                    <div class="col-sm-6 col-lg-5 ">
                                        Other1 Coll:
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("OtherSales")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-6 col-lg-5 ">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxOSale1" runat="server" Text='<%# Eval("OtherSale1", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                        <asp:DropDownList ID="DDLSale1Cats" runat="server" DataSourceID="SqlDataSourceOther1Cats" DataTextField="Description" DataValueField="O1SLNum"  Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>'
                                            AppendDataBoundItems="true" OnSelectedIndexChanged="SalesEntryDataChanged" SelectedValue='<%# Eval("OtherSale1Cat") %>' >
                                            <asp:ListItem Text="-- Select One --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-6 col-lg-5 ">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxOColl1" runat="server" Text='<%# Eval("OtherColl1", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                        <asp:DropDownList ID="DDLColl1Cats" runat="server" DataSourceID="SqlDataSourceOther1Cats" DataTextField="Description" DataValueField="O1SLNum"  Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>'
                                            AppendDataBoundItems="true" OnSelectedIndexChanged="SalesEntryDataChanged" SelectedValue='<%# Eval("OtherColl1Cat") %>' >
                                            <asp:ListItem Text="-- Select One --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                        
                                </div>
                                <div class="row" <%# (bool)Eval("OtherSales")==true?" ":"hidden='hidden'" %>>
                                    <div class="col-sm-6 col-lg-5">
                                        Other2 Sale:
                                    </div>
                                    <div class="col-sm-6 col-lg-5">
                                        Other2 Coll:
                                    </div>
                                </div>
                                <div class="row" <%# (bool)Eval("OtherSales")==true?" ":"hidden='hidden'" %> >
                                    <div class="col-sm-6 col-lg-5">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxOSale2" runat="server" Text='<%# Eval("OtherSale2", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                        <asp:DropDownList ID="DDLSale2Cats" runat="server" DataSourceID="SqlDataSourceOther2Cats" DataTextField="Description" DataValueField="O2SLNum"  Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>'
                                            AppendDataBoundItems="true" OnSelectedIndexChanged="SalesEntryDataChanged" SelectedValue='<%# Eval("OtherSale2Cat") %>' >
                                            <asp:ListItem Text="-- Select One --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-6 col-lg-5">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxOColl2" runat="server" Text='<%# Eval("OtherColl2", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                        <asp:DropDownList ID="DDLColl2Cats" runat="server" DataSourceID="SqlDataSourceOther2Cats" DataTextField="Description" DataValueField="O2SLNum"  Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>'
                                            AppendDataBoundItems="true" OnSelectedIndexChanged="SalesEntryDataChanged" SelectedValue='<%# Eval("OtherColl2Cat") %>'>
                                            <asp:ListItem Text="-- Select One --" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        Online Advance:
                                    </div>
                                    <div class="col-sm-2">
                                        Adjustment:
                                    </div>
                                    <div class="col-sm-2">
                                        Deposit:
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxOnlineAdv" runat="server" Text='<%# Eval("OnlineAdvance", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxAdjustment" runat="server" Text='<%# Eval("Adjustment", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" />
                                    </div>
                                    <div class="col-sm-2">
                                        <div style="position:absolute;left:5px;">$</div>
                                        <asp:TextBox ID="TextBoxLDeposit" runat="server" Text='<%# Eval("Deposit", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged"></asp:TextBox>
                                    </div>
                                </div>
                                <!-- Comments -->
                                <div class="row" style="border-bottom-color:yellow;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                    <div class="col-sm-2" style="background-color:yellow;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                        Comments
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-4">
                                        Edit Check Comments:<br />
                                        <asp:TextBox ID="TextBoxEditCheckCmt" runat="server" Text='<%# Eval("EditCheckComments") %>' Width="100%" Rows="3" Wrap="true" TextMode="MultiLine" OnTextChanged="SalesEntryDataChanged" />
                                    </div>
                                    <div class="col-sm-4">
                                        Sales Comments:<br />
                                        <asp:TextBox ID="TextBoxRecapCmt" runat="server" Text='<%# Eval("RecapComments") %>' Width="100%" Rows="3" Wrap="true" TextMode="MultiLine"  OnTextChanged="SalesEntryDataChanged" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-6">
                                        &nbsp;
                                    </div>
                                </div>
                            </div>
                        </EditItemTemplate>
                    </asp:FormView>
                </div>
                
                <div id="SalesTotals" style="float:left;width:200px;">
                    <div class="container">
                        <table class="table-condensed" >
                            <tr>
                                <td colspan="2" style="text-align:center;background-color:lightgray;"><strong>Breakfast Totals:</strong> </td>
                                <tr>
                                    <td>Paid</td>
                                    <td>
                                        <asp:TextBox ID="TextBoxBPaidTtl" runat="server" ReadOnly="true" Text="0" TabStop="false" ></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Reduced</td>
                                    <td>
                                        <asp:TextBox ID="TextBoxBReducedTtl" runat="server" ReadOnly="true" Text="0"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Free</td>
                                    <td>
                                        <asp:TextBox ID="TextBoxBFreeTtl" runat="server" ReadOnly="true" Text="0"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Total</td>
                                    <td>
                                        <asp:TextBox ID="TextBoxBTotal" runat="server" ReadOnly="true" Text="0"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Ala</td>
                                    <td>
                                        $<asp:TextBox ID="TextBoxBAlaTtl" runat="server" ReadOnly="true" Text="0.00" Width="60"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Meal Sale</td>
                                    <td>
                                        $<asp:TextBox ID="TextBoxBMealSaleTtl" runat="server" ReadOnly="true" Text="0.00" Width="60"></asp:TextBox>
                                    </td>
                                </tr>
                                <!-- Lunch-->
                                <tr>
                                    <td colspan="2" style="text-align:center;background-color:lightgray;"><strong>Lunch Totals:</strong> </td>
                                </tr>
                                <tr>
                                    <td>Paid</td>
                                    <td>
                                        <asp:TextBox ID="TextBoxLPaidTtl" runat="server" ReadOnly="true" Text="0"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Reduced</td>
                                    <td>
                                        <asp:TextBox ID="TextBoxLReducedTtl" runat="server" ReadOnly="true" Text="0"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Free</td>
                                    <td>
                                        <asp:TextBox ID="TextBoxLFreeTtl" runat="server" ReadOnly="true" Text="0"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Total</td>
                                    <td>
                                        <asp:TextBox ID="TextBoxLTotal" runat="server" ReadOnly="true" Text="0"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Ala</td>
                                    <td>
                                        $<asp:TextBox ID="TextBoxLAlaTtl" runat="server" ReadOnly="true" Text="0.00" Width="60"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Other Ala</td>
                                    <td>
                                        $<asp:TextBox ID="TextBoxLOtherAlaTtl" runat="server" ReadOnly="true" Text="0.00" Width="60"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Meal Sale</td>
                                    <td>
                                        $<asp:TextBox ID="TextBoxLMealSale" runat="server" ReadOnly="true" Text="0.00" Width="60"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>Total Sales</td>
                                    <td>
                                        $<asp:TextBox ID="TextBoxTotalSales" runat="server" ReadOnly="true" Text="0.00" Width="60"></asp:TextBox>
                                        <asp:HiddenField ID="HFAdvances" runat="server" Value="0" />
                                        <asp:HiddenField ID="HFRedemptions" runat="server" Value="0" />
                                        <asp:HiddenField ID="HFCharges" runat="server" Value="0" />
                                        <asp:HiddenField ID="HFRepayments" runat="server" Value="0" />
                                        <asp:HiddenField ID="HFCollections" runat="server" Value="0" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Cash +/-</td>
                                    <td>$<asp:TextBox ID="TextBoxTotalCash" runat="server" ReadOnly="true" Text="0.00" Width="60"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Deposit</td>
                                    <td>$<asp:TextBox ID="TextBoxTotalDeposit" runat="server" ReadOnly="true" Text="0.00" Width="60"></asp:TextBox>
                                    </td>
                                </tr>
                            </tr>
                        </table>
                    </div>
                </div>
                    
                
            </div>

            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceMyDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc'
                    FROM [District] WHERE DistrictID IN (SELECT DistrictID FROM [UserDistrictAccess] WHERE Userid=@UserID) OR (@AccessLevel<>'User')
                    ORDER BY [DistrictName]">
                    <SelectParameters>
                        <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                        <asp:SessionParameter Name="AccessLevel" SessionField="AccessLevel" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader" 
                    SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID AND INACTIVE=0 ORDER BY SchoolName">
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSales" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                    SELECTCOMMAND="SELECT [BreakfastDay],[LunchDay],ISNULL([BPaidElem],0) BPaidElem,ISNULL([BPaidJr],0) BPaidJr,ISNULL([BPaidSr],0) BPaidSr,ISNULL([BReduced],0) BReduced
                    ,ISNULL([BFree],0) BFree,ISNULL([BPaidLrn],0) BPaidLrn,ISNULL([BReducedLrn],0) BReducedLrn,ISNULL([BFreeLrn],0) BFreeLrn,ISNULL([BStuAla],0) BStuAla
                    ,ISNULL([BAdultAla],0) BAdultAla,ISNULL([BAdvanced],0) BAdvanced,ISNULL([BRedeemed],0) BRedeemed,ISNULL([BRepaid],0) BRepaid,ISNULL([BCharges],0) BCharges,ISNULL([BDeposit],0) BDeposit
                    ,ISNULL([LPaidElem],0) LPaidElem,ISNULL([LPaidJr],0) LPaidJr,ISNULL([LPaidSr],0) LPaidSr,ISNULL([LTier1],0) LTier1,ISNULL([LTier2],0) LTier2,ISNULL([LReduced],0) LReduced,ISNULL([LReducedTier1],0) LReducedTier1
                    ,ISNULL([LFree],0) LFree,ISNULL([LFreeTier1],0) LFreeTier1,ISNULL([LFreeTier2],0) LFreeTier2,ISNULL([LPaidLrn],0) LPaidLrn,ISNULL([LReducedLrn],0) LReducedLrn,ISNULL([LFreeLrn],0) LFreeLrn,ISNULL([LStuAla],0) LStuAla
                    ,ISNULL([LAdultAla],0) LAdultAla,ISNULL([LOtherAla1],0) LOtherAla1,ISNULL([LVendAla],0) LVendAla,ISNULL([LAdvanced],0) LAdvanced,ISNULL([LRedeemed],0) LRedeemed,ISNULL([LRepaid],0) LRepaid,ISNULL([LCharges],0) LCharges
                    ,ISNULL([OPaidSnack],0) OPaidSnack,ISNULL([OReducedSnack],0) OReducedSnack,ISNULL([OFreeSnack],0) OFreeSnack,ISNULL([SpecialFunctionSale],0) SpecialFunctionSale,ISNULL([SpecialFunctionColl],0) SpecialFunctionColl,ISNULL([SpecialFunctionComm],0) SpecialFunctionComm
                    ,ISNULL([USDASale],0) USDASale,ISNULL([USDAColl],0) USDAColl,ISNULL([OtherSale1],0) OtherSale1,ISNULL([OtherColl1],0) OtherColl1,ISNULL([OtherSale2],0) OtherSale2,ISNULL([OtherColl2],0) OtherColl2
                    ,ISNULL([OnlineAdvance],0) OnlineAdvance,ISNULL([Adjustment],0) Adjustment,ISNULL([Deposit],0) Deposit,ISNULL([OtherSale1Cat],0) OtherSale1Cat,ISNULL([OtherColl1Cat],0) OtherColl1Cat
                    ,ISNULL([EditCheckComments],'') EditCheckComments,ISNULL([RecapComments],'') RecapComments,ISNULL([OtherSale2Cat],0) OtherSale2Cat,ISNULL([OtherColl2Cat],0) OtherColl2Cat
                    ,ISNULL([R_AF], SETUP.[AF]) R_AF, ISNULL([R_Enrollment],SETUP.[Enrollment]) R_Enrollment, ISNULL([R_EligiblePaid], SETUP.[EligiblePaid]) R_EligiblePaid
                    ,ISNULL([R_EligibleFree], SETUP.[EligibleFree]) R_EligibleFree, ISNULL([R_EligibleReduced], SETUP.[EligibleReduced]) R_EligibleReduced
                    ,ISNULL([R_BreakfastIncent], SETUP.[BreakfastIncent]) R_BreakfastIncent, SETUP.*, ISNULL([Locked], 0) Locked
                    FROM [DistrictSchoolYearSetup] SETUP
                    LEFT OUTER JOIN RecapEntry R ON SETUP.ProgramYear=R.ProgramYear AND SETUP.DistrictID=R.DistrictID AND SETUP.SchoolNum=R.SchoolNum AND @RecapDate=R.RecapDate
                    WHERE ((SETUP.[DistrictID] = @DistrictID) AND (SETUP.[ProgramYear] = @ProgramYear) AND (SETUP.[SchoolNum] = @SchoolNum))">
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int16" />
                        <asp:ControlParameter ControlID="TrxDate" Name="RecapDate" PropertyName="Text" Type="DateTime" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceOther1Cats" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SELECTCOMMAND="SELECT * FROM Other1Categories">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceOther2Cats" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SELECTCOMMAND="SELECT * FROM Other2Categories">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchoolYearSetup" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT * FROM [DistrictSchoolYearSetup] WHERE (([DistrictID] = @DistrictID) AND ([ProgramYear] = @ProgramYear) AND ([SchoolNum] = @SchoolNum))">
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int16" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
