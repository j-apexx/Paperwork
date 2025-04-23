<%@ Page Title="Sales Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalesEntry.aspx.cs" Inherits="Paperwork.SalesEntry" MaintainScrollPositionOnPostBack="true" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="Tools/jcalc.js" type="text/javascript"></script>
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        // For formview resizing
        var h = $(window).height();
        var topAndBottom = 200;
        var timer,
            $win = $(window);

        //when window resizes
        $win.on('resize', function () {
            //update the height variable
            h = $(window).height();
            clearTimeout(timer);
            timer = setTimeout(function () {
                $("#GridViewHeightController").height(h - topAndBottom);
            }, 200);

        });

        
        $( function() {
            BindEvents();
        });

        function BindEvents() {
            var PYearStart = $("[id*=HFProgYearStartDate]").val();
            var PYearEnd = $("[id*=HFProgYearEndDate]").val();

            $('#GridViewHeightController').css("height", h - topAndBottom);
            $('#GridViewHeightController').css("overflow", "auto");

            //my JQuery code to maintain scroll position
            //When GridViewHeightController div is scrolled - store the scroll bar top pixel value in hidden field
            $("#GridViewHeightController").scroll(function () {
                $("[id*=hdnScrollPos]").val($("#GridViewHeightController").scrollTop());
            });

            //After postback - restore the scroll position
            $('#GridViewHeightController').scrollTop( $("[id*=hdnScrollPos]").val() );

            $(function () {
                $( "[id*=TrxDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    beforeShowDay: highlightDays,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxPostingRecapDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    beforeShowDay: highlightPostingDays,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxSpFnStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id*=TextBoxSpFnEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

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
            //Recalc CEP Breakfasts
            $("[id*=TextBoxCEPTotalMealsB]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                var EnteredCEPB = parseFloat($(this).val()).toFixed(0);
                var PaidPct = parseFloat($("[id*=HFPaidPct]").val());
                var FreePct = parseFloat($("[id*=HFFreePct]").val());

                var paidB = Math.round(EnteredCEPB * (PaidPct/100) ) ;
                var freeB = Math.round(EnteredCEPB * (FreePct/100) ) ;

                var NewTotal = (paidB + freeB);

                //check for imbalance and apply extra meal to larger mealcount
                if (NewTotal != EnteredCEPB) {
                    if (paidB > freeB)
                        paidB = paidB + (EnteredCEPB - NewTotal);
                    else
                        freeB = freeB + (EnteredCEPB - NewTotal);
                }

                var schooltype = $("[id *= HFSchoolType]").val();

                //reset values prior to assigning
                $("[id *= TextBoxBPaidElem]").val('0');
                $("[id *= TextBoxBPaidJr]").val('0');
                $("[id *= TextBoxBPaidSr]").val('0');

                if (schooltype == 1)
                    $("[id *= TextBoxBPaidElem]").val(paidB.toString());
                if (schooltype==2)
                    $("[id *= TextBoxBPaidJr]").val(paidB.toString());
                if (schooltype == 3)
                    $("[id *= TextBoxBPaidSr]").val(paidB.toString());
                
                $("[id *= TextBoxBFree1]").val(freeB.toString());

                RecalcPaidB();
                RecalcFreeB();
            });

            $("[id*=TextBoxBPaidElem],[id*=TextBoxBPaidJr],[id*=TextBoxBPaidSr],[id*=TextBoxBPaidLrn]").change(function () {
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
            })

            $("[id*=TextBoxBReduced],[id*=TextBoxBReducedLrn]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val() ))) {
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

            $("[id*=TextBoxBFree1],[id*=TextBoxBFreeLrn]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val() ))) {
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

            $("[id*=TextBoxBStuAla],[id*=TextBoxBAdltAla]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val() ))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcAlaB();
            });

            /*LUNCH SECTION*/
            $("[id*=TextBoxCEPTotalMealsL]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                var EnteredCEPL = parseFloat($(this).val()).toFixed(0);
                var PaidPct = parseFloat($("[id*=HFPaidPct]").val());
                var FreePct = parseFloat($("[id*=HFFreePct]").val());

                var paidL = Math.round(EnteredCEPL * (PaidPct/100));
                var freeL = Math.round(EnteredCEPL * (FreePct/100));

                var NewTotal = (paidL + freeL);

                //check for imbalance and apply extra meal to larger mealcount
                if (NewTotal != EnteredCEPL) {
                    if (paidL > freeL)
                        paidL = paidL + (EnteredCEPL - NewTotal);
                    else
                        freeL = freeL + (EnteredCEPL - NewTotal);
                }

                var schooltype = $("[id *= HFSchoolType]").val();

                //reset values prior to assigning
                $("[id *= TextBoxLPaidElem]").val('0');
                $("[id *= TextBoxLPaidJr]").val('0');
                $("[id *= TextBoxLPaidSr]").val('0');

                if (schooltype == 1)
                    $("[id *= TextBoxLPaidElem]").val(paidL.toString());
                if (schooltype == 2)
                    $("[id *= TextBoxLPaidJr]").val(paidL.toString());
                if (schooltype == 3)
                    $("[id *= TextBoxLPaidSr]").val(paidL.toString());

                $("[id*=TextBoxLFree1]").val(freeL.toString());

                RecalcPaidL();
                RecalcFreeL();
            });

            $("[id*=TextBoxLPaidElem],[id*=TextBoxLPaidJr],[id*=TextBoxLPaidSr],[id*=TextBoxLPaidLrn],[id*=TextBoxLTier1],[id*=TextBoxLTier2]").change(function () {
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

            $("[id*=TextBoxLReduced],[id*=TextBoxLReducedLrn],[id*=TextBoxLReducedT1]").change(function () {
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

            $("[id*=TextBoxLFree1],[id*=TextBoxLFreeLrn],[id*=TextBoxLFreeT1],[id*=TextBoxLFreeT2]").change(function () {
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

            $("[id*=TextBoxLStuAla],[id*=TextBoxLAdltAla]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcAlaL();

            });

            $("[id*=TextBoxLOtherAla1],[id*=TextBoxLVendAla]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcOtherAlaL();
            });

            /*OTHER Section*/
            $("[id*=TextBoxOSnack],[id*=TextBoxOReducedSnack],[id*=TextBoxOFreeSnack]").change(function () {
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

            $("[id*=TextBoxOSFSale],[id*=TextBoxOVendComm],[id*=TextBoxUSDASale],[id*=TextBoxOSale1],[id*=TextBoxOSale2]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcTotalSales();
            });

            /*Misc*/
            $("[id*=TextBoxBDeposit],[id*=TextBoxLDeposit]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcDeposit();
            });

            $("[id*=TextBoxBAdvanced],[id*=TextBoxLAdvanced]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcAdvances();
            });

            //Tom had these fields so that you could just enter a value and it's flipped negative
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


            $("[id*=TextBoxBRepaid],[id*=TextBoxLRepaid]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcRepayments();
            });

            $("[id*=TextBoxOSFCollect],[id*=TextBoxUSDAColl],[id*=TextBoxOColl1],[id*=TextBoxOColl2]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcCollections();
            });

            $("[id*=TextBoxAdjustment],[id*=TextBoxOnlineAdv]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                RecalcOverUnder();
            });

            $("[id*=IDelete]").click(function () {
                var isDisabled = $('a[id$=IDelete]').hasClass('aspNetDisabled');

                if (isDisabled) {
                    
                }
                else {
                    if (confirm('Are you sure you want to delete this record?')) {
                        $('[id*=HFDeleting]').val('1');
                    }
                    else { return false; }
                }
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

                    //ORIG Values if Revision
                    + parseFloat($("[id*=HFOrigBPaidElem]").val())
                    + parseFloat($("[id*=HFOrigBPaidJr]").val())
                    + parseFloat($("[id*=HFOrigBPaidSr]").val())
                    + parseFloat($("[id*=HFOrigBPaidLrn]").val())

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
                    //ORIG Values if Revision
                    + parseFloat($("[id*=HFOrigBReduced]").val())
                    + parseFloat($("[id*=HFOrigBReducedLrn]").val())
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
                    parseFloat($("[id*=TextBoxBFree1]").val())
                    + parseFloat($("[id*=TextBoxBFreeLrn]").val())
                    //ORIG Values if Revision
                    + parseFloat($("[id*=HFOrigBFree]").val())
                    + parseFloat($("[id*=HFOrigBFreeLrn]").val())
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigBStuAla]").val())
                    + parseFloat($("[id*=HFOrigBAdltAla]").val())
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
                    + (parseFloat($("[id*=TextBoxBReduced]").val()) * parseFloat($("[id*=HFBRedPrice]").val()))
                    //ORIG Values if revision
                    + (parseFloat($("[id*=HFOrigBPaidElem]").val()) * parseFloat($("[id*=HFBPaidElemPrice]").val()))
                    + (parseFloat($("[id*=HFOrigBPaidJr]").val()) * parseFloat($("[id*=HFBPaidJrPrice]").val()))
                    + (parseFloat($("[id*=HFOrigBPaidSr]").val()) * parseFloat($("[id*=HFBPaidSrPrice]").val()))
                    + (parseFloat($("[id*=HFOrigBReduced]").val()) * parseFloat($("[id*=HFBRedPrice]").val()))

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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigLPaidElem]").val())
                    + parseFloat($("[id*=HFOrigLPaidJr]").val())
                    + parseFloat($("[id*=HFOrigLPaidSr]").val())
                    + parseFloat($("[id*=HFOrigLPaidLrn]").val())
                    + parseFloat($("[id*=HFOrigLTier1]").val())
                    + parseFloat($("[id*=HFOrigLTier2]").val())
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigLReduced]").val())
                    + parseFloat($("[id*=HFOrigLReducedLrn]").val())
                    + parseFloat($("[id*=HFOrigLReducedT1]").val())
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
                    parseFloat($("[id*=TextBoxLFree1]").val())
                    + parseFloat($("[id*=TextBoxLFreeLrn]").val())
                    + parseFloat($("[id*=TextBoxLFreeT1]").val())
                    + parseFloat($("[id*=TextBoxLFreeT2]").val())
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigLFree]").val())
                    + parseFloat($("[id*=HFOrigLFreeLrn]").val())
                    + parseFloat($("[id*=HFOrigLFreeT1]").val())
                    + parseFloat($("[id*=HFOrigLFreeT2]").val())
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigLStuAla]").val())
                    + parseFloat($("[id*=HFOrigLAdltAla]").val())
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigLOtherAla1]").val())
                    + parseFloat($("[id*=HFOrigLVendAla]").val())
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
                        + ((parseFloat($("[id*=TextBoxLReduced]").val()) + parseFloat($("[id*=TextBoxLReducedT1]").val())) * parseFloat($("[id*=HFLRedPrice]").val()))
                        //ORIG Values if revision
                        + (parseFloat($("[id*=HFOrigLPaidElem]").val()) * parseFloat($("[id*=HFLPaidElemPrice]").val()))
                        + (parseFloat($("[id*=HFOrigLPaidJr]").val()) * parseFloat($("[id*=HFLPaidJrPrice]").val()))
                        + (parseFloat($("[id*=HFOrigLPaidSr]").val()) * parseFloat($("[id*=HFLPaidSrPrice]").val()))
                        + (parseFloat($("[id*=HFOrigLTier1]").val()) * parseFloat($("[id*=HFLPaidT1Price]").val()))
                        + (parseFloat($("[id*=HFOrigLTier2]").val()) * parseFloat($("[id*=HFLPaidT2Price]").val()))
                        + ((parseFloat($("[id*=HFOrigLReduced]").val()) + parseFloat($("[id*=HFOrigLReducedT1]").val())) * parseFloat($("[id*=HFLRedPrice]").val()))
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigBDeposit]").val())
                    + parseFloat($("[id*=HFOrigLDeposit]").val())
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigBAdvanced]").val())
                    + parseFloat($("[id*=HFOrigLAdvanced]").val())
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigBRedeemed]").val())
                    + parseFloat($("[id*=HFOrigLRedeemed]").val())
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigBCharges]").val())
                    + parseFloat($("[id*=HFOrigLCharges]").val())
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigBRepaid]").val())
                    + parseFloat($("[id*=HFOrigLRepaid]").val())
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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigOSFCollect]").val())
                    + parseFloat($("[id*=HFOrigUSDAColl]").val())
                    + parseFloat($("[id*=HFOrigOColl1]").val())
                    + parseFloat($("[id*=HFOrigOColl2]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (CollectionsTTL == 0)
                $("[id*=HFCollections]").val('0.00');
            else
                $("[id*=HFCollections]").val(CollectionsTTL.toString());

            RecalcOverUnder();
        };

        function RecalcTotalSales() {

            //Special handling for Orig Other Snacks
            //recalc snack hidden field
            var OtherORIGSnackTTL =
                Math.round(((
                    (parseFloat($("[id*=HFOrigOSnack]").val()) * parseFloat($("[id*=HFSnackPaidPrice]").val()))
                    + (parseFloat($("[id*=HFOrigOReducedSnack]").val()) * parseFloat($("[id*=HFSnackReducedPrice]").val()))
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (OtherORIGSnackTTL == 0)
                $("[id*=HFOrigOtherTotalSnackSale]").val('0');
            else
                $("[id*=HFOrigOtherTotalSnackSale]").val(SnackTTL.toFixed(2).toString());

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
                    //ORIG Values if revision
                    + parseFloat($("[id*=HFOrigOSFSale]").val())
                    + parseFloat($("[id*=HFOrigOVendComm]").val())
                    + parseFloat($("[id*=HFOrigUSDASale]").val())
                    + parseFloat($("[id*=HFOrigOSale1]").val())
                    + parseFloat($("[id*=HFOrigOSale2]").val())
                    + parseFloat($("[id*=HFOrigOtherTotalSnackSale]").val())
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

                    //ORIG VALUES if revision
                    - parseFloat($("[id*=HFOrigOSFCollect]").val())
                    - parseFloat($("[id*=HFOrigUSDAColl]").val())
                    - parseFloat($("[id*=HFOrigOColl1]").val())
                    - parseFloat($("[id*=HFOrigOColl2]").val())
                    + parseFloat($("[id*=HFOrigOSFSale]").val())
                    + parseFloat($("[id*=HFOrigUSDASale]").val())
                    + parseFloat($("[id*=HFOrigOSale1]").val())
                    + parseFloat($("[id*=HFOrigOSale2]").val())
                    - parseFloat($("[id*=HFOrigAdjustment]").val())
                    + parseFloat($("[id*=HFOrigOnlineAdv]").val())

            ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

            if (OverUnderTTL == 0)
                $("[id*=TextBoxTotalCash]").val('0.00');
            else
                $("[id*=TextBoxTotalCash]").val(OverUnderTTL.toFixed(2).toString());
        };

        function RecalcCEPTotals() {

            //BREAKFASTS
            var TotalBreakfasts = Math.round(
                parseFloat($("[id*=TextBoxBPaidElem]").val())
                + parseFloat($("[id*=TextBoxBPaidJr]").val())
                + parseFloat($("[id*=TextBoxBPaidSr]").val())
                + parseFloat($("[id*=TextBoxBFree1]").val())
            );

            if (TotalBreakfasts == 0)
                $("[id*=TextBoxCEPTotalMealsB]").val('0');
            else
                $("[id*=TextBoxCEPTotalMealsB]").val(TotalBreakfasts.toFixed(0).toString());

            //LUNCHES
            var TotalLunches = Math.round(
                parseFloat($("[id*=TextBoxLPaidElem]").val())
                + parseFloat($("[id*=TextBoxLPaidJr]").val())
                + parseFloat($("[id*=TextBoxLPaidSr]").val())
                + parseFloat($("[id*=TextBoxLFree1]").val())
            );

            if (TotalLunches == 0)
                $("[id*=TextBoxCEPTotalMealsL]").val('0');
            else
                $("[id*=TextBoxCEPTotalMealsL]").val(TotalBreakfasts.toFixed(0).toString());
        }

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

            RecalcCEPTotals();
        };

        //Date Functions for colorized calendar
        var dateArray;
        var dateArrayLocked;
        var dateArrayPosting;
        var dateArrayRevised;

        function highlightDays(date) {
            //mark as revised before marking as locked
            if (dateArrayRevised[0] != "") {
                for (var i = 0; i < dateArrayRevised.length; i++) {
                    //convert string element to date
                    var d = $.datepicker.parseDate("mm/dd/yy", dateArrayRevised[i]);

                    if (
                        (d.getDate() == date.getDate()) &&
                        (d.getMonth() == date.getMonth()) &&
                        (d.getFullYear() == date.getFullYear())
                    ) {
                        return [true, 'calendarRevised', 'Revision Entered'];
                    }
                }
            }

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

        function highlightPostingDays(date) {
            //if no dates, return immediately
            if (dateArrayPosting[0] != "") {
                for (var i = 0; i < dateArrayPosting.length; i++) {
                    //convert string element to date
                    var d = $.datepicker.parseDate("mm/dd/yy", dateArrayPosting[i]);

                    if (
                        (d.getDate() == date.getDate()) &&
                        (d.getMonth() == date.getMonth()) &&
                        (d.getFullYear() == date.getFullYear())
                    ) {
                        return [true, 'calendarHighlighted', 'Sales Entered'];
                    }
                }
            }

            return [false, '', 'Not Available'];
        };

        function updateDatesArray() {
            dateArray = $('[id*=HFEnteredSchoolDaysArray]').val().split(',');
            dateArrayLocked = $('[id*=HFLockedSchoolDaysArray]').val().split(',');
            dateArrayPosting = $('[id*=HFPostingDaysArray]').val().split(',');
            dateArrayRevised = $('[id*=HFRevisedSchoolDaysArray]').val().split(',');
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
    
    <link type="text/css" rel="stylesheet" href="Tools/jcalcstyle.css" />
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel2" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="OverallPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"  >
                <ContentTemplate>
                    <div id="topSection">
                        <div id="hdrgroup">
                            <div id="hdrDiv" style="float:left;width:300px">
                                <h2><%: Title %></h2>
                                <asp:LinkButton ID="ISave" runat="server" 
                                            ToolTip="Save Changes" Text=" Save" ForeColor="Green"
                                            CssClass="fas fa-save mGridLinkButton" Width="80" ></asp:LinkButton>
                                <!-- NOTE: Using the OnClientClick function to set a deleting flag which is being used to prevent Saves from happening after postback if Queued -->
                                <asp:LinkButton ID="IDelete" runat="server" 
                                            ToolTip="Delete Sales Entry" Text=" Delete" ForeColor="DarkRed"
                                            CssClass="fas fa-trash mGridLinkButton" Width="80" OnClick="IDelete_Click" 
                                            OnClientClick="var isDisabled = $('[id*=IDelete]').is(':disabled');if(isDisabled){    if( confirm('Are you sure you want to delete this record?')){$('[id*=HFDeleting]').val('1');}else{return false;}};"></asp:LinkButton>
                                <asp:HiddenField ID="HFDeleting" runat="server" Value="0" />
                                
                                <div id="Next" style="position:absolute;top:50%;right:50px;">
                                    <span class="fa-stack fa-2x">
                                        <i class="fas fa-angle-right fa-stack-1x " style="color:forestgreen;padding-left:50px;"></i>
                                        <asp:LinkButton ID="LinkButtonSchoolNext" runat="server" ToolTip="Next School" CssClass="fas fa-school fa-stack-1x" OnClick="NavNextSchool"/>
                                    </span>
                                    <br />
                                    <span class="fa-stack fa-2x">
                                        <i class="fas fa-angle-right fa-stack-1x " style="color:forestgreen;padding-left:50px;"></i>
                                        <asp:LinkButton ID="LinkButtonCalNext" runat="server" ToolTip="Next Day" CssClass="fas fa-calendar-day fa-stack-1x fa-flip-horizontal" OnClick="NavNextDay" ForeColor="Highlight"/>
                                    </span>
                                </div>
                                <div id="Prev" style="position:absolute;top:50%;left:50px;min-width:100px;">
                                    <span class="fa-stack fa-2x">
                                        <i class="fas fa-angle-left fa-stack-1x " style="color:forestgreen;position:relative;left:-25px;"></i>
                                        <asp:LinkButton ID="LinkButtonSchoolPrev" runat="server" ToolTip="Prev School" CssClass="fas fa-school fa-stack-1x" OnClick="NavPrevSchool"/>
                                    </span>
                                    <br />
                                    <span class="fa-stack fa-2x">
                                        <i class="fas fa-angle-left fa-stack-1x " style="color:forestgreen;position:relative;left:-25px;"></i>
                                        <asp:LinkButton ID="LinkButtonCalPrev" runat="server" ToolTip="Prev Day" CssClass="fas fa-calendar-day fa-stack-1x" OnClick="NavPrevDay" ForeColor="Highlight"/>
                                    </span>
                                </div>
                                
                                <div class="messagealert" id="alert_container" style="float:right;width:130px;text-align:center;" ></div>
                            </div>
                    
                            <div id="menuDiv" >
                                <!-- Using the following hidden fields to act as a source to colorize the calendar days which are entered already-->
                                <asp:HiddenField ID="HFEnteredSchoolDaysArray" runat="server" />
                                <asp:HiddenField ID="HFLockedSchoolDaysArray" runat="server" />
                                <asp:HiddenField ID="HFRevisedSchoolDaysArray" runat="server" />
                                <!-- This field will hold unlocked and entered Recap Dates to which revisions can be posted -->
                                <asp:HiddenField ID="HFPostingDaysArray" runat="server" />
                                <asp:HiddenField id="hdnScrollPos" runat="server"/>
                                <p>
                                    <strong>Program Year:</strong>
                                    <asp:Label ID="LabelProgramYear" runat="server"></asp:Label>
                                    <asp:HiddenField ID="HFProgYearStartDate" runat="server" />
                                    <asp:HiddenField ID="HFProgYearEndDate" runat="server" />
                                    &nbsp;&nbsp; 
                                    <strong>Date:</strong>
                                    <asp:TextBox ID="TrxDate" runat="server" Width="100" AutoPostBack="true" OnTextChanged="TrxDate_TextChanged"></asp:TextBox>
                                </p>
                                <p>
                                    <strong>District:</strong>
                                    <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                        DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                        OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                                    </asp:DropDownList> &nbsp;

                                    <strong>School:</strong>
                                    <asp:DropDownList ID="DropDownListSchools" runat="server" AutoPostBack="True" 
                                        DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" DataValueField="SchoolNum" ValidationGroup="GridView"
                                        OnSelectedIndexChanged="DropDownListSchools_SelectedIndexChanged">
                                    </asp:DropDownList> &nbsp;

                                    <asp:HiddenField ID="HFRevision" runat="server"  />
                                    <asp:HiddenField ID="HFRevRecapDate" runat="server" />
                                    <img src="Tools/minicalc.png" alt="open calculator" id="opener" />
                                </p>
                            </div>
                            <asp:Label ID="LabelError" runat="server" ForeColor="Red" Text=""></asp:Label>
                        </div>
                    </div>

                    <div id="entryDiv" style="position:relative;"  >
                        <div id="GridViewHeightController" style="overflow-y:auto;background-color:white;" class="row">

                            <div style="float:left;width:75%;">
                                <asp:FormView ID="FormViewSales" runat="server" CssClass="mGridRecaps" DataSourceID="SqlDataSourceSales" DefaultMode="Edit" 
                                    OnPreRender="FormViewSales_PreRender" OnDataBound="FormViewSales_DataBound" OnDataBinding="FormViewSales_DataBinding"
                                    DataKeyNames="ProgramYear, DistrictID, SchoolNum, RecapDate, Revision">
                                    <EditItemTemplate>
                                        <div class="container">
                                            <div class="row">
                                                <div class="col-sm-3" >
                                                    <!-- Maintain hidden field versions of date because if date is changed before saving, the record was being saved to the new date -->
                                                    <asp:HiddenField ID="HFEditCheckDate" runat="server" Value='<%# Eval("EditCheckDate") %>' />
                                                    <asp:HiddenField ID="HFNewRecord" runat="server" Value='<%# Eval("NEWRECORD") %>' />

                                                    <asp:CheckBox ID="CBBreakfast" runat="server" Text="Breakfast Day?" Checked='<%# Eval("BreakfastDay").ToString()=="1"?true:false %>' 
                                                        OnCheckedChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' Visible='<%# (int)Eval("Revision")==0?true:false %>' />
                                                                                                                                                                                  
                                                </div>
                                                <div class="col-sm-2" >
                                                    <asp:CheckBox ID="CheckBoxLunch" runat="server" Text="Lunch Day?" Checked='<%# Eval("LunchDay").ToString()=="1"?true:false %>' 
                                                        OnCheckedChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' Visible='<%# (int)Eval("Revision")==0?true:false %>' />
                                                </div>
                                                <div class="col-sm-2" style="color:red;font-weight:bold;">
                                                    <asp:Label ID="LabelLocked" runat="server" Text="**LOCKED**" Visible='<%# (bool)Eval("Locked") %>'></asp:Label>
                                                </div>
                                                <div class="col-sm-5" style="padding-right:0px;padding-left:0px;" >
                                                    <!-- Only display the revision selector if there is more than one -->
                                        
                                                    <asp:Label ID="LabelRev" runat="server" Text="Revision:" Visible='<%# (int)Eval("RecapCount")>=0?true:false %>'></asp:Label>
                                                    <asp:DropDownList ID="DDLRevisions" runat="server" AutoPostBack="true" DataSourceID="SqlDataSourceRevisions" DataTextField="RevisionLabel" DataValueField="Revision" 
                                                        OnSelectedIndexChanged="DDLRevisions_SelectedIndexChanged" SelectedValue='<%# Bind("Revision") %>' AppendDataBoundItems="true"
                                                        Visible='<%# (int)Eval("RecapCount")>=0?true:false %>' OnDataBinding="DDLRevisions_DataBinding" OnPreRender="DDLRevisions_PreRender"
                                                        ></asp:DropDownList>

                                            
                                            

                                                    <!-- Only display the revision button if this is locked -->
                                                    <!-- New button for revisions -->
                                                    <asp:LinkButton ID="BtnRevisionDialog" runat="server" CssClass="btn btn-primary" data-toggle="modal" data-target="#myModal" OnClick="BtnRevisionDialog_Click" 
                                                        Visible='<%# (bool)Eval("Locked") && (int)Eval("Revision")==0?true:false %>'   
                                                        Enabled='<%# (bool)Eval("Locked") && (int)Eval("Revision")==0?true:false %>'>
                                                        <i class="fas fa-tools"></i>
                                                        Revise this entry  
                                                    </asp:LinkButton>
            


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
                                                <asp:FormView ID="FormViewOrigSales" runat="server" DataSourceID="SqlDataSourceORIGSales" DefaultMode="ReadOnly">
                                                    <ItemTemplate>
                                                        <!-- BREAKFAST -->
                                                        <asp:HiddenField ID="HFOrigBPaidElem" runat="server" Value='<%# Eval("BPaidElem") %>' />
                                                        <asp:HiddenField ID="HFOrigBPaidJr" runat="server" Value='<%# Eval("BPaidJr") %>' />
                                                        <asp:HiddenField ID="HFOrigBPaidSr" runat="server" Value='<%# Eval("BPaidSr") %>' />
                                                        <asp:HiddenField ID="HFOrigBReduced" runat="server" Value='<%# Eval("BReduced") %>' />
                                                        <asp:HiddenField ID="HFOrigBFree" runat="server" Value='<%# Eval("BFree") %>' />
                                                        <!-- Learners -->
                                                        <asp:HiddenField ID="HFOrigBPaidLrn" runat="server" Value='<%# Eval("BPaidLrn") %>' />
                                                        <asp:HiddenField ID="HFOrigBReducedLrn" runat="server" Value='<%# Eval("BReducedLrn") %>' />
                                                        <asp:HiddenField ID="HFOrigBFreeLrn" runat="server" Value='<%# Eval("BFreeLrn") %>' />
                                                        <!-- Ala -->
                                                        <asp:HiddenField ID="HFOrigBStuAla" runat="server" Value='<%# Eval("BStuAla") %>' />
                                                        <asp:HiddenField ID="HFOrigBAdltAla" runat="server" Value='<%# Eval("BAdultAla") %>' />
                                                        <!-- CASH -->
                                                        <asp:HiddenField ID="HFOrigBAdvanced" runat="server" Value='<%# Eval("BAdvanced") %>' />
                                                        <asp:HiddenField ID="HFOrigBRedeemed" runat="server" Value='<%# Eval("BRedeemed") %>' />
                                                        <asp:HiddenField ID="HFOrigBRepaid" runat="server" Value='<%# Eval("BRepaid") %>' />
                                                        <asp:HiddenField ID="HFOrigBCharges" runat="server" Value='<%# Eval("BCharges") %>' />
                                                        <asp:HiddenField ID="HFOrigBDeposit" runat="server" Value='<%# Eval("BDeposit") %>' />
                                                        <!-- LUNCH -->
                                                        <asp:HiddenField ID="HFOrigLPaidElem" runat="server" Value='<%# Eval("LPaidElem") %>' />
                                                        <asp:HiddenField ID="HFOrigLPaidJr" runat="server" Value='<%# Eval("LPaidJr") %>' />
                                                        <asp:HiddenField ID="HFOrigLPaidSr" runat="server" Value='<%# Eval("LPaidSr") %>' />
                                                        <asp:HiddenField ID="HFOrigLTier1" runat="server" Value='<%# Eval("LTier1") %>' />
                                                        <asp:HiddenField ID="HFOrigLTier2" runat="server" Value='<%# Eval("LTier2") %>' />
                                                        <!-- L Reduced Free -->
                                                        <asp:HiddenField ID="HFOrigLReduced" runat="server" Value='<%# Eval("LReduced") %>' />
                                                        <asp:HiddenField ID="HFOrigLReducedT1" runat="server" Value='<%# Eval("LReducedTier1") %>' />
                                                        <asp:HiddenField ID="HFOrigLFree" runat="server" Value='<%# Eval("LFree") %>' />
                                                        <asp:HiddenField ID="HFOrigLFreeT1" runat="server" Value='<%# Eval("LFreeTier1") %>' />
                                                        <asp:HiddenField ID="HFOrigLFreeT2" runat="server" Value='<%# Eval("LFreeTier2") %>' />
                                                        <!-- L Learn -->
                                                        <asp:HiddenField ID="HFOrigLPaidLrn" runat="server" Value='<%# Eval("LPaidLrn") %>' />
                                                        <asp:HiddenField ID="HFOrigLReducedLrn" runat="server" Value='<%# Eval("LReducedLrn") %>' />
                                                        <asp:HiddenField ID="HFOrigLFreeLrn" runat="server" Value='<%# Eval("LFreeLrn") %>' />
                                                        <!-- L Ala -->
                                                        <asp:HiddenField ID="HFOrigLStuAla" runat="server" Value='<%# Eval("LStuAla") %>' />
                                                        <asp:HiddenField ID="HFOrigLAdltAla" runat="server" Value='<%# Eval("LAdultAla") %>' />
                                                        <asp:HiddenField ID="HFOrigLOtherAla1" runat="server" Value='<%# Eval("LOtherAla1") %>' />
                                                        <asp:HiddenField ID="HFOrigLVendAla" runat="server" Value='<%# Eval("LVendAla") %>' />
                                                        <!-- L CASH -->
                                                        <asp:HiddenField ID="HFOrigLAdvanced" runat="server" Value='<%# Eval("LAdvanced") %>' />
                                                        <asp:HiddenField ID="HFOrigLRedeemed" runat="server" Value='<%# Eval("LRedeemed") %>' />
                                                        <asp:HiddenField ID="HFOrigLRepaid" runat="server" Value='<%# Eval("LRepaid") %>' />
                                                        <asp:HiddenField ID="HFOrigLCharges" runat="server" Value='<%# Eval("LCharges") %>' />
                                                        <!-- OTHER -->
                                                        <asp:HiddenField ID="HFOrigOSnack" runat="server" Value='<%# Eval("OPaidSnack") %>' />
                                                        <asp:HiddenField ID="HFOrigOReducedSnack" runat="server" Value='<%# Eval("OReducedSnack") %>' />
                                                        <asp:HiddenField ID="HFOrigOFreeSnack" runat="server" Value='<%# Eval("OFreeSnack") %>' />
                                                        <asp:HiddenField ID="HFOrigOtherTotalSnackSale" runat="server" Value="0" />
                                                        <!-- OTHER SpFn Vend USDA -->
                                                        <asp:HiddenField ID="HFOrigOSFSale" runat="server" Value='<%# Eval("SpecialFunctionSale") %>' />
                                                        <asp:HiddenField ID="HFOrigOSFCollect" runat="server" Value='<%# Eval("SpecialFunctionColl") %>' />
                                                        <asp:HiddenField ID="HFOrigOVendComm" runat="server" Value='<%# Eval("SpecialFunctionComm") %>' />
                                                        <asp:HiddenField ID="HFOrigUSDASale" runat="server" Value='<%# Eval("USDASale") %>' />
                                                        <asp:HiddenField ID="HFOrigUSDAColl" runat="server" Value='<%# Eval("USDAColl") %>' />
                                                        <!-- OTHER Sales OTHER COLL -->
                                                        <asp:HiddenField ID="HFOrigOSale1" runat="server" Value='<%# Eval("OtherSale1") %>' />
                                                        <asp:HiddenField ID="HFOrigOColl1" runat="server" Value='<%# Eval("OtherColl1") %>' />
                                                        <asp:HiddenField ID="HFOrigOSale2" runat="server" Value='<%# Eval("OtherSale2") %>' />
                                                        <asp:HiddenField ID="HFOrigOColl2" runat="server" Value='<%# Eval("OtherColl2") %>' />
                                                        <!-- OTHER CASH -->
                                                        <asp:HiddenField ID="HFOrigOnlineAdv" runat="server" Value='<%# Eval("OnlineAdvance") %>' />
                                                        <asp:HiddenField ID="HFOrigAdjustment" runat="server" Value='<%# Eval("Adjustment") %>' />
                                                        <asp:HiddenField ID="HFOrigLDeposit" runat="server" Value='<%# Eval("Deposit") %>' />
                                                    </ItemTemplate>
                                                </asp:FormView>
                                            </div>
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div id="CEPSchoolSalesB" class="row" runat="server">
                                                <div class="col-sm-2">Total Meals CEP:</div>
                                                <div class="col-sm-2">
                                                    <asp:TextBox ID="TextBoxCEPTotalMealsB" runat="server" Width="75" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>'></asp:TextBox>
                                                    <asp:HiddenField ID="HFSchoolCEPParticipant" runat="server" Value='<%# Eval("PartialCEPPartic") %>' />
                                                    <asp:HiddenField ID="HFUseCustomCEPPct" runat="server" Value='<%# Eval("UseCustomCEPPct") %>' />
                                                    <asp:HiddenField ID="HFPaidPct" runat="server" Value='<%# Eval("CEPPaidPct") %>' />
                                                    <asp:HiddenField ID="HFFreePct" runat="server" Value='<%# Eval("CEPFreePct") %>' />
                                                    <asp:HiddenField ID="HFSchoolType" runat="server" Value='<%# Eval("SchoolType") %>' />
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
                                                    <asp:TextBox ID="TextBoxBFree1" runat="server" Text='<%# Eval("BFree") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' />
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
                                                <div class="col-sm-2" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %>>
                                                    Student Ala:
                                                </div>
                                                <div class="col-sm-2" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %>>
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
                                                <div class="col-sm-2" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
                                                    <div style="position:absolute;left:5px;">$</div>
                                                    <asp:TextBox ID="TextBoxBStuAla" runat="server"  Text='<%# Eval("BStuAla", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' />
                                                </div>
                                                <div class="col-sm-2" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
                                                    <div style="position:absolute;left:5px;">$</div>
                                                    <asp:TextBox ID="TextBoxBAdltAla" runat="server"  Text='<%# Eval("BAdultAla", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("Breakfasts") && !(bool)Eval("Locked") %>' />
                                                </div>
                                                <div class="col-sm-2">
                                        
                                        
                                                </div>
                                            </div>
                                            <div class="row" <%# (bool)Eval("Breakfasts")==true && (int)Eval("Revision")==0?" ":"hidden='hidden'" %>>
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
                                            <div class="row" <%# (bool)Eval("Breakfasts")==true && (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div id="CEPSchoolSalesL" class="row" runat="server">
                                                <div class="col-sm-2">Total Meals CEP:</div>
                                                <div class="col-sm-2">
                                                    <asp:TextBox ID="TextBoxCEPTotalMealsL" runat="server" Width="75" Enabled='<%# !(bool)Eval("Locked") %>'></asp:TextBox>
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
                                                    <asp:TextBox ID="TextBoxLFree1" runat="server" Text='<%# Eval("LFree") %>' Width="50" OnTextChanged="SalesEntryDataChanged" Enabled='<%# !(bool)Eval("Locked") %>' />
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
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (bool)Eval("OtherSales")==true && (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >  
                                                <div class="col-sm-2" style="padding-right:0px;">
                                                    Sp. Func. Sale:
                                                </div>
                                                <div class="col-sm-2" style="padding-right:0px;">
                                                    Sp. Func. Collect:
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
                                            <div class="row" <%# (bool)Eval("OtherSales")==true && (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
                                                <div class="col-sm-2">
                                                    <div style="position:absolute;left:5px;">$</div>
                                                    <asp:TextBox ID="TextBoxOSFSale" runat="server" Text='<%# Eval("SpecialFunctionSale", "{0:F2}") %>' Width="75" OnTextChanged="SalesEntryDataChanged" Enabled='<%# (bool)Eval("OtherSales") && !(bool)Eval("Locked") %>' />
                                                    <!-- data-toggle="modal" data-target="#SpecialFunctionsImportModal" -->
                                                    <asp:LinkButton ID="IImport" runat="server" data-target="#SpecialFunctionsImportModal"
                                                        ToolTip="Import Special Functions Transactions" ForeColor="Green"
                                                        CssClass="fas fa-file-invoice-dollar mGridLinkButton" Width="30" OnClick="BtnSpFnDialog_Click"  ></asp:LinkButton>
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
                                            <div class="row" <%# (bool)Eval("OtherSales")==true && (int)Eval("Revision")==0?" ":"hidden='hidden'" %>>
                                                <div class="col-sm-6 col-lg-5 ">
                                                    Other1 Sale:
                                                </div>
                                                <div class="col-sm-6 col-lg-5 ">
                                                    Other1 Coll:
                                                </div>
                                            </div>
                                            <div class="row" <%# (bool)Eval("OtherSales")==true && (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (bool)Eval("OtherSales")==true && (int)Eval("Revision")==0?" ":"hidden='hidden'" %>>
                                                <div class="col-sm-6 col-lg-5">
                                                    Other2 Sale:
                                                </div>
                                                <div class="col-sm-6 col-lg-5">
                                                    Other2 Coll:
                                                </div>
                                            </div>
                                            <div class="row" <%# (bool)Eval("OtherSales")==true && (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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
                                            <div class="row" <%# (int)Eval("Revision")==0?" ":"hidden='hidden'" %> >
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

                                        <!-- Popup modal box for creating revisions -->
                                        <!-- **MUST REMAIN IN FormView for Data Mapped TextBoxes** -->
                                        <div class="modal fade" id="myModal">  
                                            <div class="modal-dialog" >  
                                                <div class="modal-content" style="width:800px;" > 
                                                    <div class="modal-header">  
                                                        <h4 class="modal-title">Create Sales Revision</h4>  
                                                        <button type="button" class="close" data-dismiss="modal">×</button>  
                                                    </div>  
                                                    <div class="modal-body">  
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <div class="form-group">
                                                                    <label>Date which you are revising:</label> 
                                                                    <div class="input-group">
                                                                        <asp:TextBox ID="TextBoxOrigDate" runat="server" Text='<%# Eval("EditCheckDate","{0:d}") %>' ReadOnly="true" ></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <div class="form-group">
                                                                    <label>School which you are revising:</label> 
                                                                    <div class="input-group">
                                                                        <asp:TextBox ID="TextBoxSchoolName" runat="server" Text='<%# Eval("SchoolName") %>' ReadOnly="true" ></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row">  
                                                            <div class="col-md-12">  
                                                                <div class="form-group">  
                                                                    <label>Select sales date to post the revision to: *Note the date must be in the current period and must already have a sales entry created.</label>  
                                                                    <div class="input-group">  
                                                                        <asp:TextBox ID="TextBoxPostingRecapDate" runat="server" AutoCompleteType="None" ></asp:TextBox>
                                                                    </div>  
                                                                </div>  
                                                            </div>  
                                                        </div>  
                                                    </div>  
                                                    <div class="modal-footer">  
                                                        <asp:Button ID="ButtonCreateRevision" runat="server" CssClass="btn btn-outline-primary" Text="Create" OnClick="ButtonCreateRevision_Click" data-toggle="modal" data-target="#myModal" UseSubmitBehavior="false" /> 
                                                        <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Cancel</button>  
                                                    </div>  
                                                </div>  
                                            </div>  
                                        </div>  

                                    </EditItemTemplate>
                                </asp:FormView>
                            </div>
                
                            <div id="SalesTotals" style="float:left;width:210px;">
                                <div class="container">
                                    <table class="table-condensed" >
                                        <tr>
                                            <td colspan="2" style="text-align:center;background-color:lightgray;"><strong>Breakfast Totals:</strong> </td>
                                            <tr>
                                                <td>Paid</td>
                                                <td>
                                                    <asp:TextBox ID="TextBoxBPaidTtl" runat="server" ReadOnly="true" Text="0" TabStop="false" Enabled="false" BackColor="White"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Reduced</td>
                                                <td>
                                                    <asp:TextBox ID="TextBoxBReducedTtl" runat="server" ReadOnly="true" Text="0" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Free</td>
                                                <td>
                                                    <asp:TextBox ID="TextBoxBFreeTtl" runat="server" ReadOnly="true" Text="0" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Total</td>
                                                <td>
                                                    <asp:TextBox ID="TextBoxBTotal" runat="server" ReadOnly="true" Text="0" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Ala</td>
                                                <td>
                                                    $<asp:TextBox ID="TextBoxBAlaTtl" runat="server" ReadOnly="true" Text="0.00" Width="70" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Meal Sale</td>
                                                <td>
                                                    $<asp:TextBox ID="TextBoxBMealSaleTtl" runat="server" ReadOnly="true" Text="0.00" Width="70" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <!-- Lunch-->
                                            <tr>
                                                <td colspan="2" style="text-align:center;background-color:lightgray;"><strong>Lunch Totals:</strong> </td>
                                            </tr>
                                            <tr>
                                                <td>Paid</td>
                                                <td>
                                                    <asp:TextBox ID="TextBoxLPaidTtl" runat="server" ReadOnly="true" Text="0" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Reduced</td>
                                                <td>
                                                    <asp:TextBox ID="TextBoxLReducedTtl" runat="server" ReadOnly="true" Text="0" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Free</td>
                                                <td>
                                                    <asp:TextBox ID="TextBoxLFreeTtl" runat="server" ReadOnly="true" Text="0" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Total</td>
                                                <td>
                                                    <asp:TextBox ID="TextBoxLTotal" runat="server" ReadOnly="true" Text="0" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Ala</td>
                                                <td>
                                                    $<asp:TextBox ID="TextBoxLAlaTtl" runat="server" ReadOnly="true" Text="0.00" Width="70" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Other Ala</td>
                                                <td>
                                                    $<asp:TextBox ID="TextBoxLOtherAlaTtl" runat="server" ReadOnly="true" Text="0.00" Width="70" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Meal Sale</td>
                                                <td>
                                                    $<asp:TextBox ID="TextBoxLMealSale" runat="server" ReadOnly="true" Text="0.00" Width="70" TabStop="false" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td>Total Sales</td>
                                                <td>
                                                    $<asp:TextBox ID="TextBoxTotalSales" runat="server" ReadOnly="true" Text="0.00" Width="70" Enabled="false" BackColor="White"></asp:TextBox>
                                                    <asp:HiddenField ID="HFAdvances" runat="server" Value="0" />
                                                    <asp:HiddenField ID="HFRedemptions" runat="server" Value="0" />
                                                    <asp:HiddenField ID="HFCharges" runat="server" Value="0" />
                                                    <asp:HiddenField ID="HFRepayments" runat="server" Value="0" />
                                                    <asp:HiddenField ID="HFCollections" runat="server" Value="0" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Cash +/-</td>
                                                <td>$<asp:TextBox ID="TextBoxTotalCash" runat="server" ReadOnly="true" Text="0.00" Width="70" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Deposit</td>
                                                <td>$<asp:TextBox ID="TextBoxTotalDeposit" runat="server" ReadOnly="true" Text="0.00" Width="70" Enabled="false" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </tr>
                                    </table>
                                </div>
                            </div>


                        </div>

                    </div>
                    <!-- New Modal for importing Special Functions class="modal fade" -->
                    <div id="calculator" class="CalculatorClass">
		                <div class="numpad"></div>
		
                        <input type="text" id="display">
    
    	                <a class="num-button seven" href="#">7</a>
    	                <a class="num-button eight" href="#">8</a>
    	                <a class="num-button nine" href="#">9</a>
    
    	                <a class="num-button four" href="#">4</a>
    	                <a class="num-button five" href="#">5</a>
    	                <a class="num-button six" href="#">6</a>
    
    	                <a class="num-button one" href="#">1</a>
    	                <a class="num-button two" href="#">2</a>
    	                <a class="num-button three" href="#">3</a>

		                <a class="num-button zero-button zero" href="#">0</a>
    	                <a class="num-button dot" href="#">.</a>
    	                <a class="clear-button clear" href="#">C</a>
    
    	                <a class="function-button divide" href="#">&#247;</a>
    	                <a class="function-button multiply" href="#">x</a>
    	                <a class="function-button add" href="#">+</a>
    	                <a class="function-button subtract" href="#">&ndash;</a>
    
    	                <a class="percent-button percent" href="#">%</a>
    	                <a class="plusminus-button plusminus" href="#">&#177</a>
   
    	                <a class="equals-button" href="#">=</a>
	
 		                <div id="closer">x</div>
	                </div>
                    <div id="DataSources">
                        <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader" 
                            SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID AND INACTIVE=0 ORDER BY SortOrder, SchoolName">
                            <SelectParameters>
                                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="SqlDataSourceSales" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OnSelecting="SqlDataSourceSales_Selecting"
                            SELECTCOMMAND="SELECT IIF([BreakfastDay] IS NULL, CAST(1 AS BIT), CAST(0 AS BIT)) NEWRECORD, ISNULL([BreakfastDay],1) BreakfastDay, ISNULL([LunchDay],1) LunchDay,ISNULL([BPaidElem],0) BPaidElem,ISNULL([BPaidJr],0) BPaidJr,ISNULL([BPaidSr],0) BPaidSr,ISNULL([BReduced],0) BReduced
                            ,ISNULL([BFree],0) BFree,ISNULL([BPaidLrn],0) BPaidLrn,ISNULL([BReducedLrn],0) BReducedLrn,ISNULL([BFreeLrn],0) BFreeLrn,ISNULL([BStuAla],0) BStuAla
                            ,ISNULL([BAdultAla],0) BAdultAla,ISNULL([BAdvanced],0) BAdvanced,ISNULL([BRedeemed],0) BRedeemed,ISNULL([BRepaid],0) BRepaid,ISNULL([BCharges],0) BCharges,ISNULL([BDeposit],0) BDeposit
                            ,ISNULL([LPaidElem],0) LPaidElem,ISNULL([LPaidJr],0) LPaidJr,ISNULL([LPaidSr],0) LPaidSr,ISNULL([LTier1],0) LTier1,ISNULL([LTier2],0) LTier2,ISNULL([LReduced],0) LReduced
                            ,ISNULL([LReducedTier1],0) LReducedTier1,ISNULL([LFree],0) LFree,ISNULL([LFreeTier1],0) LFreeTier1,ISNULL([LFreeTier2],0) LFreeTier2,ISNULL([LPaidLrn],0) LPaidLrn
                            ,ISNULL([LReducedLrn],0) LReducedLrn,ISNULL([LFreeLrn],0) LFreeLrn,ISNULL([LStuAla],0) LStuAla,ISNULL([LAdultAla],0) LAdultAla,ISNULL([LOtherAla1],0) LOtherAla1
                            ,ISNULL([LVendAla],0) LVendAla,ISNULL([LAdvanced],0) LAdvanced,ISNULL([LRedeemed],0) LRedeemed,ISNULL([LRepaid],0) LRepaid,ISNULL([LCharges],0) LCharges
                            ,ISNULL([OPaidSnack],0) OPaidSnack,ISNULL([OReducedSnack],0) OReducedSnack,ISNULL([OFreeSnack],0) OFreeSnack,ISNULL([SpecialFunctionSale],0) SpecialFunctionSale
                            ,ISNULL([SpecialFunctionColl],0) SpecialFunctionColl,ISNULL([SpecialFunctionComm],0) SpecialFunctionComm,ISNULL([USDASale],0) USDASale,ISNULL([USDAColl],0) USDAColl
                            ,ISNULL([OtherSale1],0) OtherSale1,ISNULL([OtherColl1],0) OtherColl1,ISNULL([OtherSale2],0) OtherSale2,ISNULL([OtherColl2],0) OtherColl2
                            ,ISNULL([OnlineAdvance],0) OnlineAdvance,ISNULL([Adjustment],0) Adjustment,ISNULL([Deposit],0) Deposit,ISNULL([OtherSale1Cat],0) OtherSale1Cat
                            ,ISNULL([OtherColl1Cat],0) OtherColl1Cat,ISNULL([EditCheckComments],'') EditCheckComments,ISNULL([RecapComments],'') RecapComments
                            ,ISNULL([OtherSale2Cat],0) OtherSale2Cat,ISNULL([OtherColl2Cat],0) OtherColl2Cat,ISNULL([R_AF], SETUP.[AF]) R_AF, ISNULL([R_Enrollment],SETUP.[Enrollment]) R_Enrollment
                            ,ISNULL([R_EligiblePaid], SETUP.[EligiblePaid]) R_EligiblePaid,ISNULL([R_EligibleFree], SETUP.[EligibleFree]) R_EligibleFree, ISNULL([R_EligibleReduced]
                            ,SETUP.[EligibleReduced]) R_EligibleReduced,ISNULL([R_BreakfastIncent], SETUP.[BreakfastIncent]) R_BreakfastIncent, SETUP.*
                            , ISNULL([Locked], 0) Locked, ISNULL([Revision], 0) Revision, ISNULL(R.[EditCheckDate], @EditCheckDate) EditCheckDate, ISNULL(RC.RecapCount,0) RecapCount, SchoolName, SchoolType
                            ,IIF([UseCustomCEPPct]='true', [SchoolCEPPaidPct], [DistCEPPaidPct]) CEPPaidPct, IIF([UseCustomCEPPct]='true', [SchoolCEPFreePct], [DistCEPFreePct]) CEPFreePct, IIF(@Revision=0, @EditCheckDate, @RevRecapDate) RecapDate
                            FROM [DistrictSchoolYearSetup] SETUP
                                LEFT OUTER JOIN [RecapEntry] R ON SETUP.ProgramYear=R.ProgramYear AND SETUP.DistrictID=R.DistrictID AND SETUP.SchoolNum=R.SchoolNum AND @EditCheckDate=R.EditCheckDate AND @Revision=R.Revision
                                    AND IIF(@Revision=0, @EditCheckDate, @RevRecapDate)=R.RecapDate
                                LEFT OUTER JOIN (SELECT COUNT(*) RecapCount, ProgramYear, DistrictID, SchoolNum, EditCheckDate FROM RecapEntry GROUP BY ProgramYear, DistrictID, SchoolNum, EditCheckDate ) RC 
                                    ON SETUP.ProgramYear=RC.ProgramYear AND SETUP.DistrictID=RC.DistrictID AND SETUP.SchoolNum=RC.SchoolNum AND @EditCheckDate=RC.EditCheckDate 
                                LEFT OUTER JOIN [DistrictSchools] DS ON SETUP.DistrictID=DS.DistrictID and SETUP.SchoolNum=DS.SchoolNum
                                LEFT OUTER JOIN [DistrictYearInfo] DYI ON SETUP.DistrictID=DYI.DistrictID and SETUP.ProgramYear=DYI.ProgramYear
                            WHERE ((SETUP.[DistrictID] = @DistrictID) AND (SETUP.[ProgramYear] = @ProgramYear) AND (SETUP.[SchoolNum] = @SchoolNum)
                            )" DeleteCommand="DELETE [RecapEntry] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [SchoolNum]=@SchoolNum AND [RecapDate]=@RecapDate AND [Revision]=@Revision "
                            OnDeleted="SqlDataSourceSales_Deleted">
                            <SelectParameters>
                                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int16" />
                                <asp:ControlParameter ControlID="TrxDate" Name="EditCheckDate" PropertyName="Text" Type="DateTime" />
                                <asp:ControlParameter ControlID="HFRevision" Name="Revision" PropertyName="Value" Type="Int32" DefaultValue="0" />
                                <asp:ControlParameter ControlID="HFRevRecapDate" Name="RevRecapDate" PropertyName="Value" Type="DateTime" DefaultValue="1/1/1900" />
                            </SelectParameters>
                            <DeleteParameters>
                                <asp:Parameter Name="ProgramYear" Type="String" />
                                <asp:Parameter Name="DistrictID" Type="String" />
                                <asp:Parameter Name="SchoolNum" Type="Int16" />
                                <asp:ControlParameter ControlID="HFRevRecapDate" Name="RecapDate" PropertyName="Value" Type="DateTime" DefaultValue="1/1/1900" />
                                <asp:Parameter Name="Revision" Type="Int32" />
                            </DeleteParameters>
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="SqlDataSourceOther1Cats" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                            SELECTCOMMAND="SELECT * FROM Other1Categories">
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="SqlDataSourceOther2Cats" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                            SELECTCOMMAND="SELECT * FROM Other2Categories">
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="SqlDataSourceSchoolYearSetup" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                            SelectCommand="SELECT DSYS.*
                            FROM [DistrictSchoolYearSetup] DSYS
                            WHERE ((DSYS.[DistrictID] = @DistrictID) AND (DSYS.[ProgramYear] = @ProgramYear) AND (DSYS.[SchoolNum] = @SchoolNum))">
                            <SelectParameters>
                                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int16" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="SqlDataSourceRevisions" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                            SelectCommand="SELECT ISNULL(Revision, 0) Revision, CASE ISNULL(Revision,0) WHEN 0 THEN 'ORIG' ELSE CAST(RecapDate AS VARCHAR(10)) END RevisionLabel FROM [RecapEntry] 
                            WHERE (([DistrictID] = @DistrictID) AND ([ProgramYear] = @ProgramYear) AND ([SchoolNum] = @SchoolNum) AND [EditCheckDate]=@EditCheckDate)">
                            <SelectParameters>
                                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int16" />
                                <asp:ControlParameter ControlID="TrxDate" Name="EditCheckDate" PropertyName="Text" Type="DateTime" />
                                <asp:ControlParameter ControlID="HFRevision" Name="Revision" PropertyName="Value" Type="Int32" DefaultValue="0" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="SqlDataSourceORIGSales" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                            SELECTCOMMAND="SELECT ISNULL([BPaidElem],0) BPaidElem,ISNULL([BPaidJr],0) BPaidJr,ISNULL([BPaidSr],0) BPaidSr,ISNULL([BReduced],0) BReduced
                            ,ISNULL([BFree],0) BFree,ISNULL([BPaidLrn],0) BPaidLrn,ISNULL([BReducedLrn],0) BReducedLrn,ISNULL([BFreeLrn],0) BFreeLrn,ISNULL([BStuAla],0) BStuAla
                            ,ISNULL([BAdultAla],0) BAdultAla,ISNULL([BAdvanced],0) BAdvanced,ISNULL([BRedeemed],0) BRedeemed,ISNULL([BRepaid],0) BRepaid,ISNULL([BCharges],0) BCharges,ISNULL([BDeposit],0) BDeposit
                            ,ISNULL([LPaidElem],0) LPaidElem,ISNULL([LPaidJr],0) LPaidJr,ISNULL([LPaidSr],0) LPaidSr,ISNULL([LTier1],0) LTier1,ISNULL([LTier2],0) LTier2,ISNULL([LReduced],0) LReduced
                            ,ISNULL([LReducedTier1],0) LReducedTier1,ISNULL([LFree],0) LFree,ISNULL([LFreeTier1],0) LFreeTier1,ISNULL([LFreeTier2],0) LFreeTier2,ISNULL([LPaidLrn],0) LPaidLrn
                            ,ISNULL([LReducedLrn],0) LReducedLrn,ISNULL([LFreeLrn],0) LFreeLrn,ISNULL([LStuAla],0) LStuAla,ISNULL([LAdultAla],0) LAdultAla,ISNULL([LOtherAla1],0) LOtherAla1
                            ,ISNULL([LVendAla],0) LVendAla,ISNULL([LAdvanced],0) LAdvanced,ISNULL([LRedeemed],0) LRedeemed,ISNULL([LRepaid],0) LRepaid,ISNULL([LCharges],0) LCharges
                            ,ISNULL([OPaidSnack],0) OPaidSnack,ISNULL([OReducedSnack],0) OReducedSnack,ISNULL([OFreeSnack],0) OFreeSnack,ISNULL([SpecialFunctionSale],0) SpecialFunctionSale
                            ,ISNULL([SpecialFunctionColl],0) SpecialFunctionColl,ISNULL([SpecialFunctionComm],0) SpecialFunctionComm,ISNULL([USDASale],0) USDASale,ISNULL([USDAColl],0) USDAColl
                            ,ISNULL([OtherSale1],0) OtherSale1,ISNULL([OtherColl1],0) OtherColl1,ISNULL([OtherSale2],0) OtherSale2,ISNULL([OtherColl2],0) OtherColl2
                            ,ISNULL([OnlineAdvance],0) OnlineAdvance,ISNULL([Adjustment],0) Adjustment,ISNULL([Deposit],0) Deposit,ISNULL([OtherSale1Cat],0) OtherSale1Cat
                            ,ISNULL([OtherColl1Cat],0) OtherColl1Cat,ISNULL([EditCheckComments],'') EditCheckComments,ISNULL([RecapComments],'') RecapComments
                            ,ISNULL([OtherSale2Cat],0) OtherSale2Cat,ISNULL([OtherColl2Cat],0) OtherColl2Cat,ISNULL([R_AF], SETUP.[AF]) R_AF, ISNULL([R_Enrollment],SETUP.[Enrollment]) R_Enrollment
                            ,ISNULL([R_EligiblePaid], SETUP.[EligiblePaid]) R_EligiblePaid,ISNULL([R_EligibleFree], SETUP.[EligibleFree]) R_EligibleFree, ISNULL([R_EligibleReduced]
                            ,SETUP.[EligibleReduced]) R_EligibleReduced,ISNULL([R_BreakfastIncent], SETUP.[BreakfastIncent]) R_BreakfastIncent, SETUP.*
                            , ISNULL([Locked], 0) Locked, ISNULL([Revision], 0) Revision, ISNULL(R.[EditCheckDate], @EditCheckDate) EditCheckDate, ISNULL(RC.RecapCount,0) RecapCount, SchoolName
                            FROM [DistrictSchoolYearSetup] SETUP
                            LEFT OUTER JOIN RecapEntry R ON SETUP.ProgramYear=R.ProgramYear AND SETUP.DistrictID=R.DistrictID AND SETUP.SchoolNum=R.SchoolNum AND @EditCheckDate=R.EditCheckDate AND IIF(@Revision=0, NULL , 0)=R.Revision
                            AND @EditCheckDate=R.RecapDate
                            LEFT OUTER JOIN (SELECT COUNT(*) RecapCount, ProgramYear, DistrictID, SchoolNum, EditCheckDate FROM RecapEntry GROUP BY ProgramYear, DistrictID, SchoolNum, EditCheckDate ) RC 
                            ON SETUP.ProgramYear=RC.ProgramYear AND SETUP.DistrictID=RC.DistrictID AND SETUP.SchoolNum=RC.SchoolNum AND @EditCheckDate=RC.EditCheckDate 
                            LEFT OUTER JOIN DistrictSchools DS ON SETUP.DistrictID=DS.DistrictID and SETUP.SchoolNum=DS.SchoolNum
                            WHERE ((SETUP.[DistrictID] = @DistrictID) AND (SETUP.[ProgramYear] = @ProgramYear) AND (SETUP.[SchoolNum] = @SchoolNum)
                            )">
                            <SelectParameters>
                                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int16" />
                                <asp:ControlParameter ControlID="TrxDate" Name="EditCheckDate" PropertyName="Text" Type="DateTime" />
                                <asp:ControlParameter ControlID="HFRevision" Name="Revision" PropertyName="Value" Type="Int32" DefaultValue="1" />
                                <asp:ControlParameter ControlID="HFRevRecapDate" Name="RevRecapDate" PropertyName="Value" Type="DateTime" DefaultValue="1/1/1900" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="SqlDataSourceBDGT" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" 
                            DataSourceMode="DataReader" 
                            SelectCommand="SELECT [CEP], [CEPCoverage]
                            FROM [vwBDGT_Totals] 
                            WHERE ([BidNumber] = @BidNumber) AND BudgetYear=@BudgetYear">
                            <SelectParameters>
                                <asp:SessionParameter Name="BidNumber" SessionField="CurrentDistrict" Type="String" />
                                <asp:SessionParameter Name="BudgetYear" SessionField="BudgetYear" Type="String" />
                                <asp:ControlParameter Name="SchoolNum" ControlID="DropDownListSchools" PropertyName="SelectedValue" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="SqlDataSourceSpFnInvoices" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                            SelectCommand="SELECT SFL.[ProgramYear], SFL.[DistrictID], SFL.[InvoiceNumber], SFL.[OrgBilled], SFL.[FunctionDate], SFL.[BillingDate], SFL.[SpecialFunctionDesc]
	                              ,SFL.InvoiceTotal
                              FROM [SpecialFunctionsLedger] SFL
                            WHERE SFL.ProgramYear=@ProgramYear AND SFL.DistrictID=@DistrictID AND SFL.FunctionDate>=@StartDate AND SFL.FunctionDate<=@EndDate AND SFL.[BillingDate]='1/1/1900' 
                            ORDER BY CONVERT(INT, SUBSTRING(InvoiceNumber, PATINDEX('%[0-9]%', InvoiceNumber), LEN(InvoiceNumber))) ">
                            <SelectParameters>
                                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                                <asp:ControlParameter ControlID="TextBoxSpFnStartDate" DbType="Date" Name="StartDate" PropertyName="Text" DefaultValue="1/1/1900" />
                                <asp:ControlParameter ControlID="TextBoxSpFnEndDate" DbType="Date" Name="EndDate" PropertyName="Text" DefaultValue="1/2/1900" />
                            </SelectParameters>
                        </asp:SqlDataSource>
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

                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional"  >
                            <ContentTemplate>
                                <div class="modal-body">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <label style="font-weight:bold;">Set date range for Special Functions</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <label>Starting Date:</label> 
                                                <div class="input-group">
                                                    <asp:TextBox ID="TextBoxSpFnStartDate" runat="server" AutoPostBack="true" OnTextChanged="UpdateSpFnDateRange" ></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <label>Ending Date:</label> 
                                                <div class="input-group">
                                                    <asp:TextBox ID="TextBoxSpFnEndDate" runat="server" AutoPostBack="true" OnTextChanged="UpdateSpFnDateRange" ></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <label style="font-style:italic;">Only unposted Special Functions are included.</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:GridView ID="GridviewSpecialFunctions" runat="server" CssClass="mGrid" AutoGenerateColumns="False" 
                                                DataSourceID="SqlDataSourceSpFnInvoices"  >
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
                                                            <asp:Label ID="LabelInvoiceTotal" runat="server" Text='<%# Eval("InvoiceTotal" , "{0:C2}") %>'></asp:Label>
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

        </ContentTemplate>
    </asp:UpdatePanel>



</asp:Content>
