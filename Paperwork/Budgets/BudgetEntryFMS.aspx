<%@ Page Title="FMS Budget Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BudgetEntryFMS.aspx.cs" Inherits="Paperwork.BudgetEntryFMS" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <script type="text/javascript">
         Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
         Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequest);

         //Store focused element so that we can restore focus after postback
         var focusedElementId = "";

         function EndRequest(sender, args) {
             if (args.get_error() == undefined) {
                 BindEvents();
             };

             if (focusedElementId != "") {
                 $("#" + focusedElementId).focus();
             }
         };

         function BeginRequest(sender, args) {
             var fe = document.activeElement;
             if (fe != null) {
                 focusedElementId = fe.id;
             } else {
                 focusedElementId = "";
             }
         };



         $(function () {
             BindEvents();
         });

         // For card resizing
         var timer,
             $win = $(window);
         var w = $(window).width();//$("#headingThree").width();
         var xReduction = 250;

         //when window resizes
         $win.on('resize', function () {
             //update the width variable
             w = $(window).width();//$("#headingThree").width();

             clearTimeout(timer);
             timer = setTimeout(function () {
                 $("#collapseThree").width(w - xReduction);
             }, 100);
         });

         //apply highlight 
         $('body').on('focus', 'input,textarea,select', function (e) {
             $(this).addClass("highlight");
         });
         // remove highlight
         $('body').on('focusout', 'input,textarea,select', function (e) {
             $(this).removeClass("highlight");
         });


         function BindEvents() {
             $('#collapseThree').css("width", w - xReduction);
             $('#collapseThree').css("overflow", "auto");

             //$("[id$=TextBoxEmployeeF]").on("focus", function () {
             //    var x = window.scrollX;
             //    var y = window.scrollY;
             //    window.scrollTo(x, y + 50);
             //    //window.scrollY(0);
             //});


             $(function () {
                 $("[id$=TextBoxStartingDate]").datepicker({
                     changeMonth: true,
                     changeYear: true
                 });
             });

             $(function () {
                 $("[id$=TextBoxEndingDate]").datepicker({
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

             //Recalcs
             //NUTR fields that don't have subtotals
             $("[id$=TextBoxBackgroundNUTR],[id$=TextBoxSoftwareUpgradeNUTR],[id$=TextBoxCourierNUTR],[id$=TextBoxDFNDues],[id$=TextBoxEmployeeMealsNUTR],[id$=TextBoxFreightDelivNUTR],[id$=TextBoxVehicleInsuranceNUTR],[id$=TextBoxLicenseFeeNUTR],[id$=TextBoxPerformanceBondNUTR],[id$=TextBoxPostageNUTR],[id$=TextBoxCommNUTR]").change(function () {
                 if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                     $(this).val('0');
                     $(this).select();
                 }
                 else {
                     $(this).val(parseFloat($(this).val()).toFixed(2));
                 }
                 RecalcTotalNUTR();
             })
             //DIST fields that don't have subtotals
             $("[id$=TextBoxTechnologyFeeDIST],[id$=TextBoxPostageDIST],[id$=TextBoxTrainingDIST]").change(function () {
                 if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                     $(this).val('0');
                     $(this).select();
                 }
                 else {
                     $(this).val(parseFloat($(this).val()).toFixed(2));
                 }
                 RecalcTotalDIST();
             })
             //CarTruck NUTR
             $("[id$=TextBoxCarTruckNUTR],[id$=TextBoxCarTruckMaintNUTR]").change(function () {
                 if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                     $(this).val('0');
                     $(this).select();
                 }
                 else {
                     $(this).val(parseFloat($(this).val()).toFixed(2));
                 }
                 RecalcCarTruckNUTR();
             })
             //CarTruck DIST
             $("[id$=TextBoxCarTruckDIST],[id$=TextBoxCarTruckMaintDIST],[id$=TextBoxCarTruckMileageDIST]").change(function () {
                 if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                     $(this).val('0');
                     $(this).select();
                 }
                 else {
                     $(this).val(parseFloat($(this).val()).toFixed(2));
                 }
                 RecalcCarTruckDIST();
             })
             //EmpRecruitRepl NUTR
             $("[id$=TextBoxEmpRecruitmentNUTR],[id$=TextBoxEmpReplacementNUTR],[id$=TextBoxIndirectLaborNUTR]").change(function () {
                 if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                     $(this).val('0');
                     $(this).select();
                 }
                 else {
                     $(this).val(parseFloat($(this).val()).toFixed(2));
                 }
                 RecalcEmpRecruitReplNUTR();
             })
             //EmpRecruitReplDIST
             $("[id$=TextBoxEmpRecruitmentDIST],[id$=TextBoxEmpReplacementDIST],[id$=TextBoxIndirectLaborDIST]").change(function () {
                 if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                     $(this).val('0');
                     $(this).select();
                 }
                 else {
                     $(this).val(parseFloat($(this).val()).toFixed(2));
                 }
                 RecalcEmpRecruitReplDIST();
             });
             //Total insurance NUTR
             $("[id$=TextBoxLiabilityEquipNUTR],[id$=TextBoxLiabilityInsurNUTR]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                else {
                    $(this).val(parseFloat($(this).val()).toFixed(2));
                }
                RecalcInsLiabNUTR();
             });
             //Total Office Supplies NUTR
             $("[id$=TextBoxOfficeExpenseNUTR],[id$=TextBoxPrintingNUTR]").change(function () {
                    if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                        $(this).val('0');
                        $(this).select();
                    }
                    else {
                        $(this).val(parseFloat($(this).val()).toFixed(2));
                    }
                    RecalcOfficeSuppliesNUTR();
             });
             //Total Office Supplies DIST
            $("[id$=TextBoxOfficeExpenseDIST],[id$=TextBoxPrintingDIST]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                else {
                    $(this).val(parseFloat($(this).val()).toFixed(2));
                }
                RecalcOfficeSuppliesDIST();
            });
             //Total Training NUTR
             $("[id$=TextBoxDFNTraining],[id$=TextBoxStaffTrainingNUTR],[id$=TextBoxTrainingNUTR]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                else {
                    $(this).val(parseFloat($(this).val()).toFixed(2));
                }
                RecalcTotalTrainingNUTR();
             });
             //Total Uniform Linen NUTR
             $("[id$=TextBoxLaundryNUTR],[id$=TextBoxUniformsNUTR]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                else {
                    $(this).val(parseFloat($(this).val()).toFixed(2));
                }
                RecalcTotalLinenUniNUTR();
             });
             //Total Uniform Linen DIST
            $("[id$=TextBoxLaundryDIST],[id$=TextBoxUniformsDIST]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0');
                    $(this).select();
                }
                else {
                    $(this).val(parseFloat($(this).val()).toFixed(2));
                }
                RecalcTotalLinenUniDIST();
            });
             //Fees
             $("[id$=TextBoxManagementFee],[id$=TextBoxAdministrativeFee]").change(function () {
                 if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                     $(this).val('0');
                     $(this).select();
                 }
                 else {
                     $(this).val(parseFloat($(this).val()).toFixed(2));
                 }
                 RecalcTotalFees();
             });

             //Panel States
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

             var last1 = $('#<%= HFcollapse1State.ClientID %>').val();
             var last2 = $('#<%= HFcollapse2State.ClientID %>').val();

             if (last1 != null && last1 != "") {
                 $("#cardCollapse1").addClass('show');
             }
             else {
                 $("#cardCollapse1").removeClass('show');
             }

             if (last2 != null && last2 != "") {
                 $("#cardCollapse2").addClass('show');
             }
             else {
                 $("#cardCollapse2").removeClass('show');
             }

             //Accordian States
             $("#headingOne").click(function () {
                 $('#<%= hfAccordionIndex.ClientID %>').val("collapseOne");
             });
             $("#headingTwo").click(function () {
                 $('#<%= hfAccordionIndex.ClientID %>').val("collapseTwo");
             });
             $("#headingThree").click(function () {
                 $('#<%= hfAccordionIndex.ClientID %>').val("collapseThree");
             });
             $("#headingFour").click(function () {
                 $('#<%= hfAccordionIndex.ClientID %>').val("collapseFour");
             });
             $("#headingFive").click(function () {
                 $('#<%= hfAccordionIndex.ClientID %>').val("collapseFive");
             });

             var accordianIndex = $('#<%= hfAccordionIndex.ClientID %>').val();

             if (accordianIndex != null && accordianIndex != "" && accordianIndex !="collapseOne") {
                 $("#" + accordianIndex).addClass('show');
                 $("#collapseOne").removeClass('show');
             }
             else {
                 $("#collapseOne").addClass('show');
             }



         };

         function RecalcCarTruckNUTR() {
             var CarTruckTotal =
                 Math.round(((
                     parseFloat($("[id$=TextBoxCarTruckNUTR]").val())
                     + parseFloat($("[id$=TextBoxDFNMileage]").val())
                     + parseFloat($("[id$=TextBoxCarTruckMaintNUTR]").val())
                 ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
             if (CarTruckTotal == 0)
                 $("[id$=TextBoxTotalCarTruckNUTR]").val('0.00');
             else
                 $("[id$=TextBoxTotalCarTruckNUTR]").val(CarTruckTotal.toFixed(2));
             RecalcTotalNUTR()
         };
         function RecalcCarTruckDIST() {
             var CarTruckTotal =
                 Math.round(((
                     parseFloat($("[id$=TextBoxCarTruckDIST]").val())
                     + parseFloat($("[id$=TextBoxCarTruckMileageDIST]").val())
                     + parseFloat($("[id$=TextBoxCarTruckMaintDIST]").val())
                 ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
             if (CarTruckTotal == 0)
                 $("[id$=TextBoxTotalCarTruckDIST]").val('0.00');
             else
                 $("[id$=TextBoxTotalCarTruckDIST]").val(CarTruckTotal.toFixed(2));
             RecalcTotalDIST();
         };
         function RecalcEmpRecruitReplNUTR() {
             var EmpRecruitReplTotalNUTR =
                 Math.round(((
                     parseFloat($("[id$=TextBoxEmpRecruitmentNUTR]").val())
                     + parseFloat($("[id$=TextBoxEmpReplacementNUTR]").val())
                     + parseFloat($("[id$=TextBoxIndirectLaborNUTR]").val())
                 ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
             if (EmpRecruitReplTotalNUTR == 0)
                 $("[id$=TextBoxTotalEmpRecruitReplNUTR]").val('0.00');
             else
                 $("[id$=TextBoxTotalEmpRecruitReplNUTR]").val(EmpRecruitReplTotalNUTR.toFixed(2));
             RecalcTotalNUTR();
         };
         function RecalcEmpRecruitReplDIST() {
                var EmpRecruitReplTotalDIST =
                    Math.round(((
                        parseFloat($("[id$=TextBoxEmpRecruitmentDIST]").val())
                        + parseFloat($("[id$=TextBoxEmpReplacementDIST]").val())
                        + parseFloat($("[id$=TextBoxIndirectLaborDIST]").val())
                    ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
             if (EmpRecruitReplTotalDIST == 0)
                    $("[id$=TextBoxTotalEmpRecruitReplDIST]").val('0.00');
                else
                 $("[id$=TextBoxTotalEmpRecruitReplDIST]").val(EmpRecruitReplTotalDIST.toFixed(2));
                RecalcTotalDIST();
         };
         function RecalcInsLiabNUTR() {
            var InsLiabTotalNUTR =
                Math.round(((
                    parseFloat($("[id$=TextBoxLiabilityEquipNUTR]").val())
                    + parseFloat($("[id$=TextBoxLiabilityInsurNUTR]").val())
                ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
            if (InsLiabTotalNUTR == 0)
                $("[id$=TextBoxTotalInsLiabNUTR]").val('0.00');
            else
                $("[id$=TextBoxTotalInsLiabNUTR]").val(InsLiabTotalNUTR.toFixed(2));
            RecalcTotalNUTR();
         };
         function RecalcOfficeSuppliesNUTR() {
             var OfficeSuppliesTotalNUTR =
                 Math.round(((
                     parseFloat($("[id$=TextBoxOfficeExpenseNUTR]").val())
                     + parseFloat($("[id$=TextBoxPrintingNUTR]").val())
                 ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
             if (OfficeSuppliesTotalNUTR == 0)
                 $("[id$=TextBoxTotalOfficeExpNUTR]").val('0.00');
             else
                 $("[id$=TextBoxTotalOfficeExpNUTR]").val(OfficeSuppliesTotalNUTR.toFixed(2));
             RecalcTotalNUTR();
         };
         function RecalcOfficeSuppliesDIST() {
                var OfficeSuppliesTotalDIST =
                    Math.round(((
                        parseFloat($("[id$=TextBoxOfficeExpenseDIST]").val())
                        + parseFloat($("[id$=TextBoxPrintingDIST]").val())
                    ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
                if (OfficeSuppliesTotalDIST == 0)
                    $("[id$=TextBoxTotalOfficeExpDIST]").val('0.00');
                else
                    $("[id$=TextBoxTotalOfficeExpDIST]").val(OfficeSuppliesTotalDIST.toFixed(2));
                RecalcTotalDIST();
         };
         function RecalcTotalTrainingNUTR() {
             var TotalTrainingNUTR =
                 Math.round(((
                     parseFloat($("[id$=TextBoxDFNTraining]").val())
                     + parseFloat($("[id$=TextBoxStaffTrainingNUTR]").val())
                     + parseFloat($("[id$=TextBoxTrainingNUTR]").val())
                 ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
             if (TotalTrainingNUTR == 0)
                 $("[id$=TextBoxTotalTrainingNUTR]").val('0.00');
             else
                 $("[id$=TextBoxTotalTrainingNUTR]").val(TotalTrainingNUTR.toFixed(2));
             RecalcTotalNUTR();
         };
         function RecalcTotalLinenUniNUTR() {
             var TotalLinenUniNUTR =
                 Math.round(((
                     parseFloat($("[id$=TextBoxLaundryNUTR]").val())
                     + parseFloat($("[id$=TextBoxUniformsNUTR]").val())
                 ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
             if (TotalLinenUniNUTR == 0)
                 $("[id$=TextBoxTotalUniLaundryNUTR]").val('0.00');
             else
                 $("[id$=TextBoxTotalUniLaundryNUTR]").val(TotalLinenUniNUTR.toFixed(2));
             RecalcTotalNUTR();
         };
         function RecalcTotalLinenUniDIST() {
             var TotalLinenUniDIST =
                 Math.round(((
                     parseFloat($("[id$=TextBoxLaundryDIST]").val())
                     + parseFloat($("[id$=TextBoxUniformsDIST]").val())
                 ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
             if (TotalLinenUniDIST == 0)
                 $("[id$=TextBoxTotalUniLaundryDIST]").val('0.00');
             else
                 $("[id$=TextBoxTotalUniLaundryDIST]").val(TotalLinenUniDIST.toFixed(2));
             RecalcTotalDIST();
         };
         function RecalcTotalFees() {
             var TotalFeesNUTR =
                 Math.round(((
                     parseFloat($("[id$=TextBoxManagementFee]").val().replace(/[\$,]/g, ""))
                     + parseFloat($("[id$=TextBoxAdministrativeFee]").val().replace(/[\$,]/g, ""))
                 ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100
             if (TotalFeesNUTR == 0)
                 $("[id$=TextBoxSummaryTotalFees]").val('0.00');
             else
                 $("[id$=TextBoxSummaryTotalFees]").val('$' + TotalFeesNUTR.toLocaleString("en"));//toFixed(2)
             RecalcTotalNUTR();
         };


         function RecalcTotalNUTR() {
             var TotalNUTR =
                 Math.round(((
                     parseFloat($("[id$=TextBoxBackgroundNUTR]").val())
                     + parseFloat($("[id$=TextBoxTotalCarTruckNUTR]").val())
                     + parseFloat($("[id$=TextBoxSoftwareUpgradeNUTR]").val())
                     + parseFloat($("[id$=TextBoxCourierNUTR]").val())
                     + parseFloat($("[id$=TextBoxDFNDues]").val())
                     + parseFloat($("[id$=TextBoxEmployeeMealsNUTR]").val())
                     + parseFloat($("[id$=TextBoxTotalEmpRecruitReplNUTR]").val())
                     + parseFloat($("[id$=TextBoxFreightDelivNUTR]").val())
                     + parseFloat($("[id$=TextBoxTotalInsLiabNUTR]").val())
                     + parseFloat($("[id$=TextBoxVehicleInsuranceNUTR]").val())
                     + parseFloat($("[id$=TextBoxLicenseFeeNUTR]").val())
                     + parseFloat($("[id$=TextBoxTotalOfficeExpNUTR]").val())
                     + parseFloat($("[id$=TextBoxPerformanceBondNUTR]").val())
                     + parseFloat($("[id$=TextBoxPostageNUTR]").val())
                     + parseFloat($("[id$=TextBoxTotalTrainingNUTR]").val())
                     + parseFloat($("[id$=TextBoxCommNUTR]").val())
                     + parseFloat($("[id$=TextBoxTotalUniLaundryNUTR]").val())
                 ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

             if (TotalNUTR == 0)
                 $("[id$=TextBoxGrandTotalNUTR]").val('0.00');
             else
                 $("[id$=TextBoxGrandTotalNUTR]").val(TotalNUTR.toFixed(2));
             RecalcSummaryTotals();
         }
         function RecalcTotalDIST() {
             var TotalDIST =
                    Math.round(((
                        parseFloat($("[id$=TextBoxTotalCarTruckDIST]").val())
                        + parseFloat($("[id$=TextBoxTechnologyFeeDIST]").val())
                        + parseFloat($("[id$=TextBoxTotalEmpRecruitReplDIST]").val())
                        + parseFloat($("[id$=TextBoxTotalOfficeExpDIST]").val())
                        + parseFloat($("[id$=TextBoxPostageDIST]").val())
                        + parseFloat($("[id$=TextBoxTrainingDIST]").val())
                        + parseFloat($("[id$=TextBoxTotalUniLaundryDIST]").val())
                    ) * 100)) / 100;//rounding works with integers, so I need to mult by 100, round, then divide by 100

             if (TotalDIST == 0)
                 $("[id$=TextBoxGrandTotalDIST]").val('0.00');
             else
                 $("[id$=TextBoxGrandTotalDIST]").val(TotalDIST.toFixed(2));
         }

         function RecalcSummaryTotals() {
             var TotalLabor = parseFloat($("[id$=TextBoxSummaryTotalLabor]").val().replace(/[\$,]/g, ""));
             var DirectCostTotal = parseFloat($("[id$=TextBoxGrandTotalNUTR]").val().replace(/[\$,]/g, ""));
             var TotalFees = parseFloat($("[id$=TextBoxSummaryTotalFees]").val().replace(/[\$,]/g, ""));
             var ContractTotal = 0;

             if (isNaN(TotalLabor)) {
                 TotalLabor = 0;
             }
             if (isNaN(DirectCostTotal)) {
                 DirectCostTotal = 0;
             }
             if (isNaN(TotalFees)) {
                 TotalFees = 0;
             }

             ContractTotal = TotalLabor + DirectCostTotal + TotalFees;

             $("[id$=TextBoxSummaryTotalDirectCosts]").val('$' + DirectCostTotal.toLocaleString("en"));
             $("[id$=TextBoxSummaryCost]").val('$' + ContractTotal.toLocaleString("en"));
             
         };

         function RecalcAll() {
             RecalcTotalNUTR();
             RecalcTotalDIST();
             RecalcTotalFees();

             RecalcSummaryTotals();
         };

         function higlightSelectedRow() {
             var gvSelectedIdx = $('#<%= hfSelectedIndex.ClientID %>').val();
             if (gvSelectedIdx != null) {
                 $("[id*=GridViewContracts] tr:eq(" + gvSelectedIdx + ")").children("td").addClass("highlight");
             };
         };
     </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="100" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <asp:Image CssClass="loadingImage" ID="ImageLoading" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
        
    </asp:UpdateProgress>
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="topSection">
                <div id="hdrgroup">
                    <div id="menuDiv">
                        <div class="row">
                            <div class="col-sm-4">
                                <h2><%: Title %></h2>
                            </div>
                            <div class="col-sm-8">

                                <strong>Budget Year:</strong>
                                <asp:DropDownList ID="DDLBudgetYears" runat="server" DataSourceID="SqlDataSourceBudgetYears" DataTextField="BudgetDescription" 
                                    DataValueField="BudgetYear" AutoPostBack="true" OnSelectedIndexChanged="DDLBudgetYears_SelectedIndexChanged" ></asp:DropDownList>
                                &nbsp;&nbsp;&nbsp;&nbsp; 
                                <strong>District:</strong>
                                <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="False" AutoPostBack="true" 
                                    DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                    OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:HiddenField ID="HFStartDate" runat="server" />
                                <asp:HiddenField ID="HFEndDate" runat="server"  />
                                &nbsp;&nbsp; 
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="BudgetEntry">
                <asp:HiddenField ID="HFcollapse1State" runat="server" Value="showcollapse1" />
                <asp:HiddenField ID="HFcollapse2State" runat="server" Value="showcollapse2" />
                <asp:HiddenField ID="hfAccordionIndex" runat="server" Value="" />
                <asp:HiddenField ID="HFControlFocus" runat="server" />

                <div class="accordion" id="accordian" style="overflow-y:auto;background-color:white; position:absolute; width:95%; left:20px;">
                    <div class="card">
                        <div class="card-header h5" id="BudgetList">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse1" aria-expanded="true" aria-controls="cardCollapse1" >
                                Select/Create Budgets
                            </a>
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse1" style="position:absolute;right:5px;font-size:smaller;color:darkslateblue;">
                                show/hide
                            </a>
                        </div>
                        <div id="cardCollapse1" class='collapse show' >
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:GridView ID="GridViewBudgetHdrs" runat="server" AutoGenerateColumns="False" DataKeyNames="BudgetYear,DistrictID,BudgetVersion" 
                                            DataSourceID="SqlDataSourceBudgetHdrs" CssClass="table table-striped table-bordered table-hover" OnDataBound="GridViewBudgetHdrs_DataBound"
                                            EmptyDataText="No budgets found" OnRowDataBound="GridviewClickableRowDatabound" OnPageIndexChanged="GridViewBudgetHdrs_PageIndexChanged" OnSelectedIndexChanged="GridViewBudgetHdrs_SelectedIndexChanged"
                                            >
                                            <Columns>
                                                <asp:BoundField DataField="BudgetYear" HeaderText="Budget Year" SortExpression="BudgetYear" ReadOnly="true" />
                                                <asp:BoundField DataField="BudgetDescription" HeaderText="Budget Description" SortExpression="BudgetDescription" />
                                                <asp:BoundField DataField="BudgetVersion" HeaderText="Version" ReadOnly="True" SortExpression="BudgetVersion" />
                                                <asp:BoundField DataField="StartingDate" HeaderText="Start Date" SortExpression="StartingDate" DataFormatString="{0:d}" />
                                                <asp:BoundField DataField="EndingDate" HeaderText="End Date" SortExpression="EndingDate" DataFormatString="{0:d}" />
                                                <asp:BoundField DataField="Active" HeaderText="Active" SortExpression="Active" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <div style="top:2px;position:relative;text-align:left;">
                                            <asp:DropDownList ID="DDLBudgetYearsAdd" runat="server" DataSourceID="SqlDataSourceBudgetYearsAdd" DataTextField="BudgetDescription" 
                                                DataValueField="BudgetYear"  ></asp:DropDownList>
                                            <asp:LinkButton ID="LinkButtonNewBudget" runat="server" 
                                                ToolTip="Create New Budget" ForeColor="Green"
                                                CssClass="mGridLinkButton" Width="200" OnClick="LinkButtonNewBudget_Click" >
                                                <i class="fas fa-plus"></i> New Budget
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            
                            </div>
                        </div>
                    </div>
                    <div class="card">
                        <div class="card-header h5" id="BudgetDetails">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse2">
                                Budget Details
                            </a>
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse2" style="position:absolute;right:5px;font-size:smaller;color:darkslateblue;">
                                show/hide
                            </a>
                        </div>
                        <div id="cardCollapse2" class='collapse show' >
                            <div class="card-body">
                                <div class="row" style="text-align:center;position:relative;top:-10px;">
                                    <div class="col-xs-0 col-sm-1 col-md-2">
                                        <div class="messagealert" id="alert_container" style="z-index:100;margin:auto auto auto auto;position:absolute;text-align:center"></div>
                                    </div>
                                    <div class="col-xs-12 col-sm-10 col-md-8" style="top:10px;position:relative;text-align:center;">
                                        <asp:HiddenField ID="HFBudgetYear" runat="server" Value="2024-2025" />
                                        <asp:HiddenField ID="HFDistrictID" runat="server" Value="ICE" />
                                        <asp:HiddenField ID="HFBudgetVersion" runat="server" Value="1" />
                                        
<%--                                        <asp:LinkButton ID="ISave" runat="server" OnClick="ISave_Click" 
                                            ToolTip="Save Changes" ForeColor="Green"
                                            CssClass="mGridLinkButton" Width="100" >
                                            <i class="fas fa-save"></i> Save
                                        </asp:LinkButton>--%>
                                        <asp:LinkButton ID="IClone" runat="server" data-toggle="modal" data-target="#myModal" OnClick="IClone_Click"
                                            ToolTip="Copy Budget" 
                                            CssClass="mGridLinkButton" Width="100" >
                                            <i class="fas fa-clone"></i> Copy
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="ILock" runat="server" OnClick="ILockUnlock_Click"
                                            ToolTip="Lock Budget" ForeColor="DarkGoldenrod"
                                            CssClass="mGridLinkButton" Width="100" Visible="true" 
                                            OnClientClick="return confirm('Locked budgets can only be unlocked by admins. Continue?');">
                                            <i class="fas fa-unlock"></i> Lock
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="IUnlock" runat="server" OnClick="ILockUnlock_Click"
                                            ToolTip="Unlock Budget" ForeColor="DarkGoldenrod"
                                            CssClass="mGridLinkButton" Width="100" Visible="false">
                                            <i class="fas fa-lock"></i> Unlock
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="IDelete" runat="server" OnClick="IDelete_Click" 
                                            ToolTip="Delete Budget" ForeColor="Red"
                                            CssClass="mGridLinkButton" Width="100"
                                            OnClientClick="return confirm('Are you sure you want to delete this budget?');">
                                            <i class="fas fa-trash-alt"></i> Delete
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="IPrint" Width="100" runat="server" OnClick="IPrint_Click"
                                            class="mGridLinkButton" 
                                            ToolTip="Opens Print Preview" >
                                            <i class="fas fa-print"></i> Print
                                        </asp:LinkButton>

                                    </div>
                                </div>
                                <div class="row" >
                                    <asp:HiddenField ID="HFHourlyEmpName" runat="server" />
                                    <asp:HiddenField ID="HFHourlySchoolNum" runat="server" />
                                    <asp:HiddenField ID="HFHourlyJobClass" runat="server" />
                                    <asp:HiddenField ID="HFHourlySchedule" runat="server" />
                                    <asp:HiddenField ID="HFHourlyRate" runat="server" />
                                    <asp:HiddenField ID="HFHourlyHours" runat="server" />
                                    <asp:HiddenField ID="HFHourlyDays" runat="server" />
                                    <asp:HiddenField ID="HFHourlyPlanType" runat="server" />
                                    <asp:HiddenField ID="HFHourlyHealth" runat="server" />
                                    <asp:HiddenField ID="HFHourlyDental" runat="server" />
                                    <asp:HiddenField ID="HFHourlyDisability" runat="server" />
                                    <asp:HiddenField ID="HFHourlyVision" runat="server" />
                                    <asp:HiddenField ID="HFHourlyAnnuity" runat="server" />
                                    <asp:HiddenField ID="HFHourlyRetire" runat="server" />
                                    <asp:HiddenField ID="HFHourlyLife" runat="server" />
                                    <asp:HiddenField ID="HFHourlyOther" runat="server" />
                                    <div class="col-sm-12" >
                                        <asp:FormView ID="FormViewBudget" runat="server" DataSourceID="SqlDataSourceSelectedBudget" DefaultMode="Edit" DataKeyNames="BudgetYear, DistrictID, BudgetVersion" Width="100%"
                                             OnDataBound="FormViewBudget_DataBound" OnPreRender="FormViewBudget_PreRender">
                                            <EditItemTemplate>
                                                <div >
                                                    <div class="accordion" id="accordionBudgets" style="background-color:white;width:95%; left:20px;">
                                                      <div class="card">
                                                        <div class="card-header" id="headingOne">
                                                            <h5 class="m-0">
                                                            <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#collapseOne" aria-expanded="true" aria-controls="collapseOne">
                                                                Budget Header
                                                            </button>
                                                            </h5>
                                                        </div>

                                                        <div id="collapseOne" class="collapse show" aria-labelledby="headingOne" data-parent="#accordionBudgets">
                                                                <div class="card-body" style="width:100%;">
                                                                    <div class="row">
                                                                        <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                            <h5>Facilities Management Systems (FMS) Budget</h5>
                                                                        </div>
                                                                    </div>
                                                                    <div class="row">
                                                                        <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                            <strong>Budget Year:</strong>
                                                                            <asp:Label ID="LabelBudgetYear" runat="server" Text='<%# Bind("BudgetYear") %>'></asp:Label>
                                                                            &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                            <strong>District:</strong>
                                                                            <asp:Label ID="LabelDistrictID" runat="server" Text='<%# Bind("DistrictID") %>'></asp:Label>
                                                                            &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                            <strong>Version:</strong>
                                                                            <asp:Label ID="LabelBudgetVersion" runat="server" Text='<%# Bind("BudgetVersion") %>'></asp:Label>
                                                                        </div>
                                                                    </div>
                                                                    <div class="row">
                                                                        <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                            <strong>Budget Year Starting Date:</strong>
                                                                            <asp:TextBox ID="TextBoxStartingDate" runat="server" Text='<%# Bind("StartingDate","{0:d}") %>' Width="100" OnTextChanged="TextBoxHdr_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                                                            &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                            <strong>Budget Year Ending Date:</strong>
                                                                            <asp:TextBox ID="TextBoxEndingDate" runat="server" Text='<%# Bind("EndingDate","{0:d}") %>' Width="100" OnTextChanged="TextBoxHdr_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                        <div class="row">
                                                                            <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                                <strong>Budget Description:</strong>
                                                                                <asp:TextBox ID="TextBoxBudgetDescription" runat="server" Text='<%# Bind("BudgetDescription") %>' Width="400" MaxLength="50" OnTextChanged="TextBoxHdr_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    <div class="row">
                                                                        <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                            <strong>Contract:</strong>
                                                                            <asp:TextBox ID="TextBoxContractID" runat="server" Text='<%# Bind("ContractID") %>' Width="150" MaxLength="15" ReadOnly="true" ></asp:TextBox>
                                                                            <asp:LinkButton ID="buttonLookupContract" runat="server" ToolTip="Lookup" CssClass="btn btn-light btn-sm fas fa-search" OnClick="buttonLookupContract_Click" Width="30" />
                                                                            <asp:LinkButton runat="server" ID="IClearContract" ToolTip="Clear" CssClass="btn btn-light btn-sm fas fa-undo-alt" ForeColor="Red" OnClick="IClearContract_Click" Width="30" />
                                                                            <asp:TextBox ID="TextBoxContractDesc" runat="server" Text='<%# Eval("ContractDescription") %>' Width="400" MaxLength="50" Enabled="false"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                    <div class="row">
                                                                        <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                                <strong>Active:</strong>
                                                                                <asp:CheckBox ID="CheckBoxActive" runat="server" Checked='<%# Bind("Active") %>' OnCheckedChanged="CheckBoxActive_CheckedChanged" AutoPostBack="true" />
                                                                                &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                                <strong>Locked:</strong>
                                                                                <asp:CheckBox ID="CheckBoxLockedBudget" runat="server" Checked='<%# Bind("LockedBudget") %>' Enabled="false" />
                                                                        </div>
                                                                    </div>
                                                                     <div class="row">
                                                                         <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                             <table class="table table-hover table-sm " style="max-width:1000px;margin-left:auto;margin-right:auto;line-height:15px;text-align:right;">
                                                                                 <tbody>
                                                                                     <tr>
                                                                                         <td colspan="4" style="background-color:gray;font-weight:bold;color:black;text-align:center;">Labor Costs</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td style="font-weight:bold;color:black;">FMS Labor Hourly</td>
                                                                                         <td colspan="3"></td>
                                                                                     <tr>
                                                                                         <td>&nbsp;</td>
                                                                                         <td style="font-weight:bold;">Direct Wages</td>
                                                                                         <td>
                                                                                             <asp:TextBox ID="TextBoxSummaryTotalDirectWages" runat="server" Text='<%# Bind("TotalWages","{0:C2}") %>' Font-Bold="true" CssClass="text-sm-right" Enabled="false"></asp:TextBox>
                                                                                         </td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td>&nbsp;</td>
                                                                                         <td style="font-weight:bold;">Direct Fringe</td>
                                                                                         <td>
                                                                                             <asp:TextBox ID="TextBoxSummaryTotalDirectFringe" runat="server" Text='<%# Bind("GrandTotalFringes","{0:C2}") %>' Font-Bold="true" CssClass="text-sm-right" Enabled="false"></asp:TextBox>
                                                                                         </td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td>&nbsp;</td>
                                                                                         <td style="font-weight:bold;">Direct Benefits</td>
                                                                                         <td>
                                                                                             <asp:TextBox ID="TextBoxSummaryTotalDirectBenefits" runat="server" Text='<%# Bind("GrandTotalBenefits","{0:C2}") %>' Font-Bold="true" CssClass="text-sm-right" Enabled="false"></asp:TextBox>
                                                                                         </td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td>&nbsp;</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td>&nbsp;</td>
                                                                                         <td style="font-weight:bold;">Subtotal Labor</td>
                                                                                         <td>
                                                                                             <asp:TextBox ID="TextBoxSummaryTotalHourly" runat="server" Text='<%# ((decimal)Eval("TotalWages")+(decimal)Eval("GrandTotalFringes")+(decimal)Eval("GrandTotalBenefits")).ToString("C2") %>' Font-Bold="true" CssClass="text-sm-right" Enabled="false"></asp:TextBox>
                                                                                         </td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td colspan="4">&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td style="font-weight:bold;color:black;">FMS Labor - Management</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td>
                                                                                             <asp:TextBox ID="TextBoxSummaryTotalManagement" runat="server" Text='<%# Eval("GrandTotalMgmtCost","{0:C2}") %>' Font-Bold="true" CssClass="text-sm-right" Enabled="false"></asp:TextBox>
                                                                                         </td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td colspan="4">&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td>&nbsp;</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td style="font-weight:bold;color:black;">Total Labor</td>
                                                                                         <td>
                                                                                            <asp:TextBox CssClass="text-sm-right" ID="TextBoxSummaryTotalLabor" runat="server" Text='<%# ((decimal)Eval("TotalWages")+(decimal)Eval("GrandTotalFringes")+(decimal)Eval("GrandTotalBenefits")+(decimal)Eval("GrandTotalMgmtCost")).ToString("C2") %>' BackColor="#ffcc00" Font-Bold="true" Enabled="false" ></asp:TextBox>
                                                                                        </td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td colspan="4">&nbsp;</td>
                                                                                     </tr>

                                                                                     <tr>
                                                                                         <td colspan="4" style="background-color:gray;font-weight:bold;color:black;text-align:center;">Direct Costs</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td colspan="3">These are costs are associated  with Communication, Uniforms, Staff Training & Certification, Computer & Technology, Employee Recruitment, Liability Insurance, License & Permit Fees, Office Supplies, Employees Meals, Performance Bonds, Background Checks, etc.  If Applicable)</td>
                                                                                         <td>
                                                                                             <asp:TextBox CssClass="text-sm-right" ID="TextBoxSummaryTotalDirectCosts" runat="server" Text="$0.00" BackColor="#ffcc00" Font-Bold="true" Enabled="false" ></asp:TextBox>
                                                                                         </td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td colspan="4" style="background-color:gray;font-weight:bold;color:black;text-align:center;">FMS Fees</td>
                                                                                     </tr>
                                                                                     <tr >
                                                                                         <td colspan="4" style="line-height:1px;padding: 0 0 0 0;">&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td style="font-weight:bold;color:black;">Management Fee</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td><asp:TextBox ID="TextBoxManagementFee" runat="server" Text='<%# Bind("ManagementFee","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" OnTextChanged="TextBoxHdr_TextChanged" AutoPostBack="true"></asp:TextBox></td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td colspan="2" style="text-align:center;">Fee that will be charged to manage the operation</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td style="font-weight:bold;color:black;">Administrative Fee</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td><asp:TextBox ID="TextBoxAdministrativeFee" runat="server" Text='<%# Bind("AdminFee","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" OnTextChanged="TextBoxHdr_TextChanged" AutoPostBack="true" ></asp:TextBox></td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td colspan="2" style="text-align:center;">Fee that will be charged to administer the operation</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td>&nbsp;</td>
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td>&nbsp;</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td style="font-weight:bold;color:black;">Total Fees</td>
                                                                                         <td><asp:TextBox CssClass="text-sm-right" ID="TextBoxSummaryTotalFees" runat="server" Text="$0.00" BackColor="#ffcc00" Font-Bold="true" Enabled="false" ></asp:TextBox></td>
                                                                                         
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td>&nbsp;</td>
                                                                                         <td>&nbsp;</td>
                                                                                         <td style="font-weight:bold;color:black;">Total Contract Cost</td>
                                                                                         <td><asp:TextBox CssClass="text-sm-right" ID="TextBoxSummaryCost" runat="server" Text="$0.00" BackColor="#ffcc00" Font-Bold="true" Enabled="false" ></asp:TextBox></td>
                                                                                         
                                                                                     </tr>
                                                                                     <tr>
                                                                                         <td colspan="4" style="text-align:center;">
                                                                                             <strong>Notes:</strong><br />
                                                                                             <asp:TextBox ID="TextBoxHdrNotes" runat="server" Text='<%# Bind("CoversheetNotes") %>' OnTextChanged="TextBoxHdr_TextChanged" AutoPostBack="true" TextMode="MultiLine" Rows="4" Width="50%" ></asp:TextBox>
                                                                                         </td>
                                                                                     </tr>
                                                                                 </tbody>
                                                                             </table>
                                                                         </div>
                                                                     </div>
                                                            </div>
                                                      </div>
                                                      <div class="card">
                                                        <div class="card-header" id="headingTwo">
                                                          <h2 class="mb-0">
                                                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseTwo" aria-expanded="false" aria-controls="collapseTwo">
                                                              Direct Costs
                                                            </button>
                                                          </h2>
                                                        </div>
                                                        <div id="collapseTwo" class="collapse" aria-labelledby="headingTwo" data-parent="#accordionBudgets">
                                                            <div class="card-body">
                                                                <div class="row">
                                                                    <div class="col-sm-12" style="text-align:center;">
                                                                        <strong>Budget Year:</strong>
                                                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("BudgetYear") %>'></asp:Label>
                                                                        &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                        <strong>District:</strong>
                                                                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("DistrictID") %>'></asp:Label>
                                                                        &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                        <strong>Version:</strong>
                                                                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("BudgetVersion") %>'></asp:Label>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-sm-12">
                                                                        <table class="table table-hover table-bordered table-sm ">
                                                                            <thead>
                                                                                <tr>
                                                                                    <th scope="colgroup" colspan="3" width="80%">Direct Costs</th>
                                                                                    <th scope="colgroup" colspan="2" width="10%" style="background-color:lightblue;">FMS Costs</th>
                                                                                    <th scope="colgroup" colspan="2" width="10%" style="background-color:lightyellow;">District Costs</th>
                                                                                </tr>
                                                                            </thead>
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td>Background checks, fingerprinting and/or drug testing</td>
                                                                                    <td>Background checks, fingerprinting and/or drug testing</td>
                                                                                    <td>5060 Employee Testing</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxBackgroundNUTR" runat="server" Text='<%# Bind("BackgroundCheckNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td style="border-bottom:none;">Car/Truck Rental or Mileage</td>
                                                                                    <td>Car/Truck Rental</td>
                                                                                    <td>6220 Truck Rent</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxCarTruckNUTR" runat="server" Text='<%# Bind("CarTruckRentNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxCarTruckDIST" runat="server" Text='<%# Bind("CarTruckRentDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border-bottom:none;border-top:none;">&nbsp;</td>
                                                                                    <td>Mileage (linked to DFN Travel)</td>
                                                                                    <td>6120 Travel</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxDFNMileage" runat="server" Text='<%# Bind("DFNTravel","{0:F2}") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxCarTruckMileageDIST" runat="server" Text='<%# Bind("CarTruckMileageDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border-top:none;">&nbsp;</td>
                                                                                    <td>Vehicle Maintenance</td>
                                                                                    <td>6210 Truck Repair</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxCarTruckMaintNUTR" runat="server" Text='<%# Bind("CarTruckMaintNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxTotalCarTruckNUTR" runat="server" Text='<%# ((decimal)Eval("CarTruckMaintNUTR") + (decimal)Eval("DFNTravel") + (decimal)Eval("CarTruckRentNUTR")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxCarTruckMaintDIST" runat="server" Text='<%# Bind("CarTruckMaintDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxTotalCarTruckDIST" runat="server" Text='<%# ((decimal)Eval("CarTruckRentDIST") + (decimal)Eval("CarTruckMileageDIST") + (decimal)Eval("CarTruckMaintDIST")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td>Computer and Technology</td>
                                                                                    <td>Software Annual/Technology Fee</td>
                                                                                    <td>&nbsp;</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxSoftwareUpgradeNUTR" runat="server" Text='<%# Bind("SoftwareUpgradeNUTR","{0:F2}") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"></td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxTechnologyFeeDIST" runat="server" Text='<%# Bind("TechnologyFeeDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Courier Services</td>
                                                                                    <td>Courier Services</td>
                                                                                    <td>6240 Delivery Expense</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxCourierNUTR" runat="server" Text='<%# Bind("CourierNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Dues or Subscriptions</td>
                                                                                    <td>Dues/Subscriptions (Linked to DFN Dues)</td>
                                                                                    <td>6104 Membership Dues</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxDFNDues" runat="server" Text='<%# Bind("DFNDuesSubs","{0:F2}") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Employee Meals</td>
                                                                                    <td>Employee Meals</td>
                                                                                    <td>6180 Meals</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxEmployeeMealsNUTR" runat="server" Text='<%# Bind("EmployeeMealsNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td style="border-bottom:none;">Employee Recruitment and Advertising</td>
                                                                                    <td>Employee Recruitment - Initial</td>
                                                                                    <td>&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxEmpRecruitmentNUTR" runat="server" Text='<%# Bind("EmpRecruitmentNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxEmpRecruitmentDIST" runat="server" Text='<%# Bind("EmpRecruitmentDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border-bottom:none;border-top:none;">&nbsp;</td>
                                                                                    <td>Employee Recruitment - Replacement</td>
                                                                                    <td>&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxEmpReplacementNUTR" runat="server" Text='<%# Bind("EmpReplacementNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxEmpReplacementDIST" runat="server" Text='<%# Bind("EmpReplacementDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border-top:none;">&nbsp;</td>
                                                                                    <td>Indirect Labor or Temporary Labor (Casual Labor)</td>
                                                                                    <td>5020 Casual Labor</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxIndirectLaborNUTR" runat="server" Text='<%# Bind("IndirectLaborNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxTotalEmpRecruitReplNUTR" runat="server" Text='<%# ((decimal)Eval("EmpRecruitmentNUTR") + (decimal)Eval("EmpReplacementNUTR") + (decimal)Eval("IndirectLaborNUTR")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxIndirectLaborDIST" runat="server" Text='<%# Bind("IndirectLaborDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxTotalEmpRecruitReplDIST" runat="server" Text='<%# ((decimal)Eval("EmpRecruitmentDIST") + (decimal)Eval("EmpReplacementDIST") + (decimal)Eval("IndirectLaborDIST")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                </tr>
                                                                                
                                                                                <tr>
                                                                                    <td>Freight and Delivery</td>
                                                                                    <td>Freight and Delivery Charges</td>
                                                                                    <td>6100 Office Expenses</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxFreightDelivNUTR" runat="server" Text='<%# Bind("FreightDelivNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td style="border-bottom:none;">Insurance Liability</td>
                                                                                    <td>Product & Public Liability - Equipment</td>
                                                                                    <td>&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxLiabilityEquipNUTR" runat="server" Text='<%# Bind("LiabilityEquipNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border-top:none;">&nbsp;</td>
                                                                                    <td>Product & Public Liability - Insurance</td>
                                                                                    <td>&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxLiabilityInsurNUTR" runat="server" Text='<%# Bind("LiabilityInsurNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxTotalInsLiabNUTR" runat="server" Text='<%# ((decimal)Eval("LiabilityEquipNUTR") + (decimal)Eval("LiabilityInsurNUTR")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;">&nbsp;</td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td>Insurance Vehicle</td>
                                                                                    <td>Insurance - Vehicle</td>
                                                                                    <td>6300 Insurance</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxVehicleInsuranceNUTR" runat="server" Text='<%# Bind("VehicleInsuranceNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>License and Permits</td>
                                                                                    <td>License Fees</td>
                                                                                    <td>6310 Tax & License</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxLicenseFeeNUTR" runat="server" Text='<%# Bind("LicenseFeeNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td style="border-bottom:none;">Office Supplies and Printing</td>
                                                                                    <td>Office Expenses (supplies, warranties, misc.)</td>
                                                                                    <td>6100 Office Expenses</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxOfficeExpenseNUTR" runat="server" Text='<%# Bind("OfficeExpenseNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxOfficeExpenseDIST" runat="server" Text='<%# Bind("OfficeExpenseDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;">&nbsp;</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border-top:none;">&nbsp;</td>
                                                                                    <td>Printing</td>
                                                                                    <td>6100 Office Expenses</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxPrintingNUTR" runat="server" Text='<%# Bind("PrintingNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxTotalOfficeExpNUTR" runat="server" Text='<%# ((decimal)Eval("OfficeExpenseNUTR") + (decimal)Eval("PrintingNUTR")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxPrintingDIST" runat="server" Text='<%# Bind("PrintingDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxTotalOfficeExpDIST" runat="server" Text='<%# ((decimal)Eval("OfficeExpenseDIST") + (decimal)Eval("PrintingDIST")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td>Performance Bond</td>
                                                                                    <td>Performance Bond</td>
                                                                                    <td>6290 Interest Expenses</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxPerformanceBondNUTR" runat="server" Text='<%# Bind("PerformanceBondNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Postage</td>
                                                                                    <td>Postage</td>
                                                                                    <td>6100 Office Expenses</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxPostageNUTR" runat="server" Text='<%# Bind("PostageNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxPostageDIST" runat="server" Text='<%# Bind("PostageDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td style="border-bottom:none;">Staff Training and Certification</td>
                                                                                    <td>Continuing Education Manager (Linked to DFN Education)</td>
                                                                                    <td>6101 Education</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxDFNTraining" runat="server" Text='<%# Bind("DFNEducation","{0:F2}") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border-bottom:none;border-top:none;">&nbsp;</td>
                                                                                    <td>Staff Training and Certification</td>
                                                                                    <td>6101 Education</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxStaffTrainingNUTR" runat="server" Text='<%# Bind("StaffTrainingNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td colspan="2" style="background-color:lightyellow;"></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border-top:none;">&nbsp;</td>
                                                                                    <td>Training Fee</td>
                                                                                    <td>6101 Education</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxTrainingNUTR" runat="server" Text='<%# Bind("TrainingNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxTotalTrainingNUTR" runat="server" Text='<%# ((decimal)Eval("DFNEducation") + (decimal)Eval("StaffTrainingNUTR") + (decimal)Eval("TrainingNUTR")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxTrainingDIST" runat="server" Text='<%# Bind("TrainingDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                </tr>
                                                                                
                                                                                <tr>
                                                                                    <td>Telephone</td>
                                                                                    <td>Communications (Telephone, Cell, etc.)</td>
                                                                                    <td>6050 Telephone</td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxCommNUTR" runat="server" Text='<%# Bind("CommNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightyellow;">&nbsp;</td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td style="border-bottom:none;">Uniforms, Linens and Laundry</td>
                                                                                    <td>Laundry</td>
                                                                                    <td>6080 Sanitation</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxLaundryNUTR" runat="server" Text='<%# Bind("LaundryNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxLaundryDIST" runat="server" Text='<%# Bind("LaundryDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;">&nbsp;</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border-top:none;">&nbsp;</td>
                                                                                    <td>Uniforms (including safety wear)</td>
                                                                                    <td>5050 Uniform Allow Expense</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxUniformsNUTR" runat="server" Text='<%# Bind("UniformsNUTR","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxTotalUniLaundryNUTR" runat="server" Text='<%# ((decimal)Eval("LaundryNUTR") + (decimal)Eval("UniformsNUTR")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxUniformsDIST" runat="server" Text='<%# Bind("UniformsDIST","{0:F2}") %>' Width="100%"></asp:TextBox></td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxTotalUniLaundryDIST" runat="server" Text='<%# ((decimal)Eval("LaundryDIST") + (decimal)Eval("UniformsDIST")).ToString("F2") %>' Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                </tr>

                                                                                <tr style="border-top:solid;border-top-width:5px;border-top-color:black;">
                                                                                    <td style="background-color:lightyellow;border-right:none;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;border-right:none;border-left:none;font-weight:bold;color:black;">TOTAL DISTRICT COSTS RESPONSIBILITY</td>
                                                                                    <td colspan="4" style="background-color:lightyellow;border-right:none;border-left:none;">&nbsp;</td>
                                                                                    <td style="background-color:lightyellow;"><asp:TextBox ID="TextBoxGrandTotalDIST" runat="server" Text="0.00" Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td style="background-color:lightblue;border-right:none;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;border-right:none;border-left:none;font-weight:bold;color:black;">TOTAL TNG COSTS RESPONSIBILITY</td>
                                                                                    <td colspan="2" style="background-color:lightblue;border-right:none;border-left:none;">&nbsp;</td>
                                                                                    <td style="background-color:lightblue;"><asp:TextBox ID="TextBoxGrandTotalNUTR" runat="server" Text="0.00" Width="100%" Enabled="false"></asp:TextBox></td>
                                                                                    <td colspan="2" style="background-color:lightblue;border-right:none;border-left:none;">&nbsp;</td>
                                                                                </tr>

                                                                            </tbody>
                                                                        </table>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                      </div>
                                                      <div class="card" >
                                                        <div class="card-header" id="headingThree">
                                                          <h2 class="mb-0">
                                                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseThree" aria-expanded="false" aria-controls="collapseThree">
                                                              Labor - Hourly
                                                            </button>
                                                          </h2>
                                                        </div>
                                                        <div id="collapseThree" class="collapse" aria-labelledby="headingThree" data-parent="#accordionBudgets"  >
                                                          <div class="card-body" >
                                                            <div class="row">
                                                                <div class="col-sm-12" style="text-align:center;">
                                                                    <strong>Budget Year:</strong>
                                                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("BudgetYear") %>'></asp:Label>
                                                                    &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                    <strong>District:</strong>
                                                                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("DistrictID") %>'></asp:Label>
                                                                    &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                    <strong>Version:</strong>
                                                                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("BudgetVersion") %>'></asp:Label>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div id="divHourlyEmps" class="col-sm-12 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                    <asp:GridView ID="gvHourlyEmps" runat="server" DataSourceID="SqlDataSourceHourlyEmps" CssClass="mGrid" AutoGenerateColumns="false"
                                                                        DataKeyNames="BudgetYear,DistrictID,BudgetVersion,ORD" ShowFooter="true" OnRowCommand="gvHourlyEmps_RowCommand" OnPreRender="gvHourlyEmps_PreRender" OnDataBound="gvHourlyEmps_DataBound" 
                                                                        ShowHeaderWhenEmpty="true" ShowFooterWhenEmpty="true" OnRowDataBound="gvHourlyEmps_RowDataBound" >
                                                                        <Columns>
                                                                            <asp:TemplateField>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">&nbsp;</div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <strong>*</strong>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Building">
                                                                                <ItemTemplate>
                                                                                    <asp:DropDownList ID="DDLSchools" runat="server" DataSourceID="SqlDataSourceSchools" DataValueField="SchoolNum" DataTextField="SchoolName" OnDataBound="DDLSchools_DataBound" OnSelectedIndexChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true"  ></asp:DropDownList>
                                                                                    <asp:HiddenField ID="HFSchoolNum" runat="server" Value='<%# Eval("SchoolNum") %>' /> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">&nbsp;</div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:DropDownList ID="DDLSchoolsF" runat="server" DataSourceID="SqlDataSourceSchools" DataValueField="SchoolNum" DataTextField="SchoolName"  ></asp:DropDownList>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Employee">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxEmployee" runat="server" Text='<%# Bind("EmpName") %>' MaxLength="50" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:TextBox ID="TextBoxEmployeeF" runat="server" MaxLength="50"></asp:TextBox>
                                                                                    <asp:RequiredFieldValidator ID="ValRequiredEmp" runat="server" ControlToValidate="TextBoxEmployeeF"  CssClass="validators" 
                                                                                        Display="Dynamic" ErrorMessage="Employee name is required." ValidationGroup="InsertEmp" >*</asp:RequiredFieldValidator>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Job/Class">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxJob" runat="server" Text='<%# Bind("Job") %>' MaxLength="50" Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:TextBox ID="TextBoxJobF" runat="server" MaxLength="50" Width="75"></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Schedule">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxSchedule" runat="server" Text='<%# Bind("EmpSchedule") %>' MaxLength="50" Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:TextBox ID="TextBoxScheduleF" runat="server" MaxLength="50" Width="75"></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Hourly Rate" >
                                                                                <ItemTemplate>
                                                                                   $<asp:TextBox ID="TextBoxRate" runat="server" Text='<%# Bind("HourlyRate" , "{0:F2}") %>' Width="50" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxRateF" runat="server" Width="50"  ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Hours/Day">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxHoursDay" runat="server" Text='<%# Bind("HoursDay" , "{0:F2}") %>' Width="50" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:lightskyblue;height:50px;border-top:3px solid black;">
                                                                                        Hrs/Day<br />
                                                                                        <asp:Label ID="LabelHoursDayTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:TextBox ID="TextBoxHoursDayF" runat="server" Width="50" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Days Paid">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxDaysPaid" runat="server" Text='<%# Bind("DaysPaid") %>' Width="50" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:TextBox ID="TextBoxDaysPaidF" runat="server" Width="50" ></asp:TextBox>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Annual<br />Wages">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxWages" runat="server" Text='<%# Eval("Wages","{0:F2}") %>' Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:lightskyblue;height:50px;border-top:3px solid black;">
                                                                                        Wages<br />
                                                                                        <asp:Label ID="LabelWagesTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxWagesF" runat="server" Text="0.00" Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="FICA">
                                                                                <HeaderTemplate>
                                                                                    FICA<br />
                                                                                    <asp:Label ID="Label10" runat="server" Width="10" Text=""></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrFICAPct" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxHourlyHDR_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label9" runat="server" Width="10" Text="%"></asp:Label><br />&nbsp;
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxFICA" runat="server" Text='<%# Eval("FICA","{0:F2}") %>' Enabled="false" Width="55" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        FICA<br />
                                                                                        <asp:Label ID="LabelFICATotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxFICAF" runat="server" Text="0.00" Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="FUTA">
                                                                                <HeaderTemplate>
                                                                                    FUTA<br />
                                                                                    <asp:Label ID="LabelSpace1" runat="server" Width="10"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrFUTAPct" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxHourlyHDR_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label7" runat="server" Width="10" Text="%"></asp:Label><br />

                                                                                    <asp:Label ID="Label8" runat="server" Width="10" Text="$"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrFUTADlr" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxHourlyHDR_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="LabelSpace2" runat="server" Width="10"></asp:Label>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxFUTA" runat="server" Text='<%# Eval("FUTA","{0:F2}") %>' Enabled="false" Width="55" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        FUTA<br />
                                                                                        <asp:Label ID="LabelFUTATotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxFUTAF" runat="server" Text="0.00" Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="PA SUI">
                                                                                <HeaderTemplate>
                                                                                    PA SUI<br />
                                                                                    <asp:Label ID="LabelSpace3" runat="server" Width="10"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrSUIPCT" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxHourlyHDR_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label11" runat="server" Width="10" Text="%"></asp:Label><br />

                                                                                    <asp:Label ID="Label12" runat="server" Width="10" Text="$"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrSUIDLR" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxHourlyHDR_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="LabelSpace4" runat="server" Width="10"></asp:Label>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxSUI" runat="server" Text='<%# Eval("SUI","{0:F2}") %>' Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        PA SUI<br />
                                                                                        <asp:Label ID="LabelSUITotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxSUIF" runat="server" Text="0.00" Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Payroll Related">
                                                                                <HeaderTemplate>
                                                                                    Payroll Related<br />
                                                                                    <asp:Label ID="Label13" runat="server" Width="10" Text=""></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrRelatedPayrollPct" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxHourlyHDR_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label14" runat="server" Width="10" Text="%"></asp:Label><br />&nbsp;
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxPayRel" runat="server" Text='<%# Eval("PayrollRelated","{0:F2}") %>' Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        Payroll Related<br />
                                                                                        <asp:Label ID="LabelPayRelTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxPayRelF" runat="server" Text="0.00" Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="WComp">
                                                                                <HeaderTemplate>
                                                                                    WComp<br />
                                                                                    <asp:Label ID="Label15" runat="server" Width="10" Text=""></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrWCompPct" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxHourlyHDR_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label16" runat="server" Width="10" Text="%"></asp:Label><br />&nbsp;
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxWCOMP" runat="server" Text='<%# Eval("WComp","{0:F2}") %>' Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        WComp<br />
                                                                                        <asp:Label ID="LabelWCOMPTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxWCOMPF" runat="server" Text="0.00" Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Mandatory<br />Fringes">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxFringes" runat="server" Text='<%# Eval("Fringes","{0:F2}") %>' Enabled="false" Width="75" ></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:lightskyblue;height:50px;border-top:3px solid black;">
                                                                                        Mand. Fringes<br />
                                                                                        <asp:Label ID="LabelFringesTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxFringesF" runat="server" Text="0.00" Enabled="false" Width="75" ></asp:TextBox>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="Plan<br />Type">
                                                                                <ItemTemplate>
                                                                                    <asp:DropDownList ID="DDLPlanType" runat="server" SelectedValue='<%# Bind("PlanType") %>' OnSelectedIndexChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true">
                                                                                        <asp:ListItem Text="None" Value="0" />
                                                                                        <asp:ListItem Text="Single" Value="1" />
                                                                                        <asp:ListItem Text="Single +1" Value="2" />
                                                                                        <asp:ListItem Text="Family" Value="3" />
                                                                                    </asp:DropDownList>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:DropDownList ID="DDLPlanTypeF" runat="server" >
                                                                                        <asp:ListItem Text="None" Value="0" Selected="true" />
                                                                                        <asp:ListItem Text="Single" Value="1" />
                                                                                        <asp:ListItem Text="Single +1" Value="2" />
                                                                                        <asp:ListItem Text="Family" Value="3" />
                                                                                    </asp:DropDownList>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Health">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxHealth" runat="server" Text='<%# Eval("Health","{0:F2}") %>' Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        Health<br />
                                                                                        <asp:Label ID="LabelHealthTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxHealthF" runat="server" Text="0.00" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Dental">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxDental" runat="server" Text='<%# Eval("Dental","{0:F2}") %>' Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        Dental<br />
                                                                                        <asp:Label ID="LabelDentalTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxDentalF" runat="server" Text="0.00" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Disability">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxDisability" runat="server" Text='<%# Eval("Disability","{0:F2}") %>' Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        Disability<br />
                                                                                        <asp:Label ID="LabelDisabilityTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxDisabilityF" runat="server" Text="0.00" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Vision">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxVision" runat="server" Text='<%# Eval("Vision","{0:F2}") %>' Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        Vision<br />
                                                                                        <asp:Label ID="LabelVisionTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxVisionF" runat="server" Text="0.00" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Annuity">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxAnnuity" runat="server" Text='<%# Eval("Annuity","{0:F2}") %>' Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        Annuity<br />
                                                                                        <asp:Label ID="LabelAnnuityTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxAnnuityF" runat="server" Text="0.00" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Retire">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxRetire" runat="server" Text='<%# Eval("Retire","{0:F2}") %>' Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        Retire<br />
                                                                                        <asp:Label ID="LabelRetireTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxRetireF" runat="server" Text="0.00" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Life">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxLife" runat="server" Text='<%# Eval("Life","{0:F2}") %>' Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        Life<br />
                                                                                        <asp:Label ID="LabelLifeTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxLifeF" runat="server" Text="0.00" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Other">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxOther" runat="server" Text='<%# Eval("OtherBenefit","{0:F2}") %>' Width="75" OnTextChanged="gvHourlyEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        Other<br />
                                                                                        <asp:Label ID="LabelOtherTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxOtherF" runat="server" Text="0.00" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Total Bens">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxTtlBens" runat="server" Text='<%# Eval("TotalBenefits","{0:F2}") %>' Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:lightskyblue;height:50px;border-top:3px solid black;">
                                                                                        Total Bens<br />
                                                                                        <asp:Label ID="LabelTtlBensTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxTtlBensF" runat="server" Text="0.00" Enabled="false" Width="75" ></asp:TextBox>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Grand<br />Total">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxGrandTtl" runat="server" Text='<%# Eval("GrandTotal","{0:F2}") %>' Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:lightskyblue;height:50px;border-top:3px solid black;">
                                                                                        Grand Total<br />
                                                                                        <asp:Label ID="LabelGrandTtlTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    $<asp:TextBox ID="TextBoxGrandTtlF" runat="server" Text="0.00" Enabled="false" Width="75" ></asp:TextBox> 
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField>
                                                                                <ItemTemplate>
                                                                                     <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                                                         CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                                                         OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                                                                        CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="InsertEmp" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                    <asp:CheckBox ID="CBRetainFooterEntries" runat="server" Text="Retain Footer values for quicker record entry" Checked="true" />
                                                                </div>
                                                            </div>
                                                              <div class="row">
                                                                  <div class="col-sm-4 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                      Notes:<br />
                                                                      <asp:TextBox ID="TextBoxHourlyLaborNotes" runat="server" Rows="5" TextMode="MultiLine" AutoPostBack="true" OnTextChanged="TextBoxHourlyHDR_TextChanged" Width="100%"></asp:TextBox>
                                                                  </div>
                                                                  <div class="col-sm-8 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                      <table class="table table-hover table-sm" style="width:400px;text-align:right;font-weight:bold;">
                                                                          <tbody>
                                                                              <tr>
                                                                                  <td>WAGES</td>
                                                                                  <td>
                                                                                      <asp:TextBox ID="TextBoxTotalHourlyWages" runat="server" Enabled="false" Text='<%# Eval("TotalWages","{0:C2}") %>'></asp:TextBox>
                                                                                  </td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>MANDATORY</td>
                                                                                  <td>
                                                                                      <asp:TextBox ID="TextBoxTotalHourlyMandatory" runat="server" Enabled="false" Text='<%# Eval("GrandTotalFringes","{0:C2}") %>'></asp:TextBox>
                                                                                  </td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>BENEFITS</td>
                                                                                  <td>
                                                                                      <asp:TextBox ID="TotalHourlyBenefits" runat="server" Enabled="false" Text='<%# Eval("GrandTotalBenefits","{0:C2}") %>'></asp:TextBox>
                                                                                  </td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>OTHER</td>
                                                                                  <td>
                                                                                      <asp:TextBox ID="TextBoxOtherLaborDlr" runat="server" AutoPostBack="true" OnTextChanged="TextBoxHourlyHDR_TextChanged"></asp:TextBox>
                                                                                  </td>
                                                                              </tr>
                                                                              <tr style="background-color:lightskyblue;">
                                                                                  <td>LABOR COST</td>
                                                                                  <td>
                                                                                      <asp:TextBox ID="TotalHourlyLaborCost" runat="server" Enabled="false" Text='<%# ((decimal)Eval("TotalWages")+(decimal)Eval("GrandTotalFringes")+(decimal)Eval("GrandTotalBenefits")+(decimal)Eval("TotalOtherLaborDlr")).ToString("C2") %>'></asp:TextBox>
                                                                                  </td>
                                                                              </tr>
                                                                          </tbody>
                                                                      </table>
                                                                  </div>

                                                              </div>
                                                          </div>
                                                        </div>
                                                      </div>
                                                      <div class="card">
                                                        <div class="card-header" id="headingFour">
                                                          <h2 class="mb-0">
                                                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseFour" aria-expanded="false" aria-controls="collapseFour">
                                                              Labor - Management
                                                            </button>
                                                          </h2>
                                                        </div>
                                                        <div id="collapseFour" class="collapse" aria-labelledby="headingFour" data-parent="#accordionBudgets">
                                                          <div class="card-body">
                                                            <div class="row">
                                                                <div class="col-sm-12" style="text-align:center;">
                                                                    <strong>Budget Year:</strong>
                                                                    <asp:Label ID="Label17" runat="server" Text='<%# Bind("BudgetYear") %>'></asp:Label>
                                                                    &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                    <strong>District:</strong>
                                                                    <asp:Label ID="Label18" runat="server" Text='<%# Bind("DistrictID") %>'></asp:Label>
                                                                    &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                    <strong>Version:</strong>
                                                                    <asp:Label ID="Label19" runat="server" Text='<%# Bind("BudgetVersion") %>'></asp:Label>
                                                                </div>
                                                            </div>
                                                              <div class="row">
                                                                  <div class="col-sm-12" style="text-align:center;">
                                                                      Enter % of Increase:
                                                                      <asp:TextBox ID="TextBoxMgmtIncrease" runat="server" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" Width="50" BackColor="LightYellow"></asp:TextBox>%
                                                                  </div>
                                                              </div>
                                                            <div class="row">
                                                                <div class="col-sm-12 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                    <asp:GridView ID="gvMgmtEmps" runat="server" DataSourceID="SqlDataSourceMgmtEmps" CssClass="mGrid" AutoGenerateColumns="false"
                                                                        DataKeyNames="BudgetYear,DistrictID,BudgetVersion,ORD" ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" 
                                                                        OnRowDataBound="gvMgmtEmps_RowDataBound" OnRowCommand="gvMgmtEmps_RowCommand" OnPreRender="gvMgmtEmps_PreRender"  >
                                                                        <Columns>
                                                                            <asp:TemplateField>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <strong>*</strong><br />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Manager">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxManager" runat="server" Text='<%# Bind("ManagerName") %>' MaxLength="50" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:TextBox ID="TextBoxManagerF" runat="server" MaxLength="50"></asp:TextBox>
                                                                                    <asp:RequiredFieldValidator ID="ValRequiredMgr" runat="server" ControlToValidate="TextBoxManagerF"  CssClass="validators" 
                                                                                        Display="Dynamic" ErrorMessage="Manager name is required." ValidationGroup="InsertMgr" >*</asp:RequiredFieldValidator>
                                                                                    <br />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Job Title">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="TextBoxJob" runat="server" Text='<%# Bind("Job") %>' MaxLength="50" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox>  <br />
                                                                                    Work Weeks:
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:TextBox ID="TextBoxJobF" runat="server" MaxLength="50" ></asp:TextBox> <br />
                                                                                    Work Weeks:
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Current Salary" >
                                                                                <ItemTemplate>
                                                                                   <asp:Label ID="LabelDlr1" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxCurrSalary" runat="server" Text='<%# Bind("CurrentSalary" , "{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> <br />

                                                                                    <asp:Label ID="LabelSpace" runat="server" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxWorkWeeks" runat="server" Text='<%# Bind("WorkWeeks") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox>
                                                                                    <asp:CompareValidator ID="CompareValidatorWorkWeeks" runat="server" ControlToValidate="TextBoxWorkWeeks" CssClass="validators" Display="Dynamic" Operator="LessThanEqual" 
                                                                                        ValueToCompare="52" ValidationGroup="UpdateMgr" ErrorMessage="Work Weeks must be less than or equal to 52">*Must be less than or equal to 52</asp:CompareValidator>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:Label ID="LabelDlr2" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxCurrSalaryF" runat="server" Width="75"  ></asp:TextBox> <br />

                                                                                    <asp:Label ID="LabelSpace1" runat="server" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxWorkWeeksF" runat="server" Width="75"></asp:TextBox>
                                                                                    <asp:CompareValidator ID="CompareValidatorWorkWeeksF" runat="server" ControlToValidate="TextBoxWorkWeeksF" CssClass="validators" Display="Dynamic" Operator="LessThanEqual" 
                                                                                        ValueToCompare="52" ValidationGroup="InsertMgr" ErrorMessage="Work Weeks must be less than or equal to 52">*Must be less than or equal to 52</asp:CompareValidator>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Current Bonus" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelDlr3" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxCurrBonus" runat="server" Text='<%# Bind("CurrentBonus" , "{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:Label ID="LabelDlr4" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxCurrBonusF" runat="server" Width="75"  ></asp:TextBox> <br />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Health Buyout" >
                                                                                <ItemTemplate>
                                                                                   <asp:Label ID="LabelDlr5" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxCurrHealthBuyout" runat="server" Text='<%# Bind("CurrentHealthBuyout" , "{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        Sub-Total Wages
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:Label ID="LabelDlr6" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxCurrHealthBuyoutF" runat="server" Width="75"  ></asp:TextBox> <br />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Cell Phone" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelDlrCell" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:Textbox ID="TextBoxCellPhone" runat="server" Text='<%# Bind("CellBenefit" , "{0:F2}") %>' Width="75" Enabled="true" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelSubtotalCellPhone" runat="server" Width="75" Enabled="false"  ></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:Label ID="LabelDlrCellF" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxCellPhoneF" runat="server" Width="75" ></asp:TextBox> <br />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Fitness Credit" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelDlrFit" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:Textbox ID="TextBoxFitness" runat="server" Text='<%# Bind("FitnessCredit" , "{0:F2}") %>' Width="75" Enabled="true" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelSubtotalFitness" runat="server" Width="75" Enabled="false"  ></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:Label ID="LabelDlrFitF" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxFitnessF" runat="server" Width="75" ></asp:TextBox> <br />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="New Salary" >
                                                                                <ItemTemplate>
                                                                                   <asp:Label ID="LabelDlr7" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxNewSalary" runat="server" Width="75" Enabled="false" Text='<%# Eval("NewSalary","{0:N2}") %>' ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelSubtotalSalary" runat="server" Width="75" Enabled="false"  ></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:Label ID="LabelDlr8" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxNewSalaryF" runat="server" Width="75" Enabled="false"  ></asp:TextBox> <br />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            
                                                                            <asp:TemplateField HeaderText="Bonus" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelDlr9" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxBonus" runat="server" Text='<%# Bind("CurrentBonus" , "{0:N2}") %>' Width="75" Enabled="false"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelSubtotalBonus" runat="server" Width="75" Enabled="false"  ></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Health Buyout" >
                                                                                <ItemTemplate>
                                                                                   <asp:Label ID="LabelDlr11" runat="server" Text="$" Width="15"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHealthBuyout" runat="server" Text='<%# Bind("CurrentHealthBuyout" , "{0:N2}") %>' Width="75" Enabled="false" ></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelSubtotalHealthBuyout" runat="server" Width="75" Enabled="false"  ></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="TTL Wages" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelTotalWages" runat="server" Width="75" Enabled="false" Text='<%# Eval("Wages" , "{0:N2}") %>' ></asp:Label> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelGrandTotalWages" runat="server" Width="75" Enabled="false"  ></asp:Label>
                                                                                    </div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField>
                                                                                <ItemTemplate>
                                                                                     <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                                                         CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                                                         OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                    <div style="border-top:3px solid lightskyblue;"></div>
                                                                                    <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                                                                        CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="InsertMgr" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-sm-12 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                    <asp:GridView ID="gvMgmtEmpsFringe" runat="server" DataSourceID="SqlDataSourceMgmtEmps" CssClass="mGrid" AutoGenerateColumns="false"
                                                                        DataKeyNames="BudgetYear,DistrictID,BudgetVersion,ORD" ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" 
                                                                        OnRowDataBound="gvMgmtEmps_RowDataBound" OnRowCommand="gvMgmtEmps_RowCommand" >
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Manager">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelManager" runat="server" Text='<%# Eval("ManagerName") %>' MaxLength="50" ></asp:Label> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <strong>TOTAL MANDATORY</strong>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Annual Wages" >
                                                                                <ItemTemplate>
                                                                                   <asp:Label ID="LabelTotalWages" runat="server" Text='<%# Eval("Wages" , "{0:C2}") %>' ></asp:Label> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="FICA" >
                                                                                <HeaderTemplate>
                                                                                    FICA<br />
                                                                                    <asp:Label ID="Label10" runat="server" Width="10" Text=""></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrFICAPct" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label9" runat="server" Width="10" Text="%"></asp:Label><br />&nbsp;
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelFICA" runat="server" Text='<%# Eval("FICA","{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelTotalFica" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="FUTA" >
                                                                                <HeaderTemplate>
                                                                                    FUTA<br />
                                                                                    <asp:Label ID="LabelSpace1" runat="server" Width="10"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrFUTAPct" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label7" runat="server" Width="10" Text="%"></asp:Label><br />

                                                                                    <asp:Label ID="Label8" runat="server" Width="10" Text="$"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrFUTADlr" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="LabelSpace2" runat="server" Width="10"></asp:Label>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelFUTA" runat="server" Text='<%# Eval("FUTA","{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelTotalFuta" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Payroll Related" >
                                                                                <HeaderTemplate>
                                                                                    Payroll Related<br />
                                                                                    <asp:Label ID="Label12" runat="server" Width="10" Text=""></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrPayrollRelatedPct" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label13" runat="server" Width="10" Text="%"></asp:Label><br />&nbsp;
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelPayRel" runat="server" Text='<%# Eval("PayrollRelated","{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelTotalPayrollRelated" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="PA SUI" >
                                                                                <HeaderTemplate>
                                                                                    PA SUI<br />
                                                                                    <asp:Label ID="LabelSpace3" runat="server" Width="10"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrSUIPCT" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label11" runat="server" Width="10" Text="%"></asp:Label><br />

                                                                                    <asp:Label ID="Label22" runat="server" Width="10" Text="$"></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrSUIDLR" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="LabelSpace4" runat="server" Width="10"></asp:Label>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelSUI" runat="server" Text='<%# Eval("SUI","{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelTotalSUI" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="WComp" >
                                                                                <HeaderTemplate>
                                                                                    WComp<br />
                                                                                    <asp:Label ID="Label14" runat="server" Width="10" Text=""></asp:Label>
                                                                                    <asp:TextBox ID="TextBoxHdrWCompPct" runat="server" Width="55" BackColor="LightYellow" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" ></asp:TextBox>
                                                                                    <asp:Label ID="Label15" runat="server" Width="10" Text="%"></asp:Label><br />&nbsp;
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelWCOMP" runat="server" Text='<%# Eval("WCOMP","{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelTotalWComp" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Total Mandatory Fringe" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelTotalFringe" runat="server" Text='<%# Eval("Fringes","{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelTotalMandatory" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>

                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-sm-12 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                    <asp:GridView ID="gvMgmtEmpsBenefits" runat="server" DataSourceID="SqlDataSourceMgmtEmps" CssClass="mGrid" AutoGenerateColumns="false"
                                                                        DataKeyNames="BudgetYear,DistrictID,BudgetVersion,ORD" ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" 
                                                                        OnRowDataBound="gvMgmtEmps_RowDataBound" OnRowCommand="gvMgmtEmps_RowCommand">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Manager">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelManager" runat="server" Text='<%# Eval("ManagerName") %>' ></asp:Label> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <strong>TOTAL BENEFITS</strong>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="Plan Type">
                                                                                <ItemTemplate>
                                                                                    <asp:DropDownList ID="DDLPlanType" runat="server" SelectedValue='<%# Bind("PlanType") %>' OnSelectedIndexChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true">
                                                                                        <asp:ListItem Text="None" Value="0" />
                                                                                        <asp:ListItem Text="Single" Value="1" />
                                                                                        <asp:ListItem Text="Single +1" Value="2" />
                                                                                        <asp:ListItem Text="Family" Value="3" />
                                                                                    </asp:DropDownList>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;"></div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="Health">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxHealth" runat="server" Text='<%# Eval("Health","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelHealthTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Life">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxLife" runat="server" Text='<%# Eval("Life","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelLifeTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Dental">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxDental" runat="server" Text='<%# Eval("Dental","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelDentalTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                                </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Vision">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxVision" runat="server" Text='<%# Eval("Vision","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelVisionTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                                </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Other">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxOther" runat="server" Text='<%# Eval("OtherBenefit","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelOtherBenefitTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Total Insurance" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelTotalBenefits" runat="server" Text='<%# Eval("TotalBenefits" , "{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelTotalBenefits" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>

                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-sm-12 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                    <asp:GridView ID="gvMgmtEmpsTotals" runat="server" DataSourceID="SqlDataSourceMgmtEmps" CssClass="mGrid" AutoGenerateColumns="false"
                                                                        DataKeyNames="BudgetYear,DistrictID,BudgetVersion,ORD" ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" 
                                                                        OnRowDataBound="gvMgmtEmps_RowDataBound" OnRowCommand="gvMgmtEmps_RowCommand">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Manager">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelManager" runat="server" Text='<%# Eval("ManagerName") %>' ></asp:Label> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <strong>TOTAL LABOR COSTS</strong>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Annual Wages" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelTotalWages" runat="server" Text='<%# Eval("Wages" , "{0:C2}") %>' ></asp:Label> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelGrandTotalWages" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Mandatory" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="TextBoxTotalFringe" runat="server" Text='<%# Eval("Fringes","{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelGrandTotalMandatory" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Insurances" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelTotalBenefits" runat="server" Text='<%# Eval("TotalBenefits" , "{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelGrandTotalBenefits" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="401K">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBox401K" runat="server" Text='<%# Eval("Ben401k","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="Label401kTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Grand Total Fringes" >
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelTotalFringeBens" runat="server" Text='<%# Eval("GrandTotalFringeBens" , "{0:C2}") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelGrandTotalFringeBens" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-sm-6 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                    <asp:GridView ID="gvMgmtExpenses" runat="server" DataSourceID="SqlDataSourceMgmtEmps" CssClass="mGrid" AutoGenerateColumns="false"
                                                                        DataKeyNames="BudgetYear,DistrictID,BudgetVersion,ORD" ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" 
                                                                        OnRowDataBound="gvMgmtEmps_RowDataBound" OnRowCommand="gvMgmtEmps_RowCommand">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Manager">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelManager" runat="server" Text='<%# Eval("ManagerName") %>' ></asp:Label> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="height:50px;border-top:3px solid black;">
                                                                                        <strong>TOTAL EXPENSES</strong>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Travel">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxTravel" runat="server" Text='<%# Eval("ExpnTravel","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelTravelTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Membership Dues">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxDues" runat="server" Text='<%# Eval("ExpnDues","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelDuesTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Education">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxEducation" runat="server" Text='<%# Eval("ExpnEducation","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelEducationTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Subscriptions">
                                                                                <ItemTemplate>
                                                                                    $<asp:TextBox ID="TextBoxSubs" runat="server" Text='<%# Eval("ExpnSubscriptions","{0:F2}") %>' Width="75" OnTextChanged="gvMgmtEmps_ControlChanged" AutoPostBack="true"></asp:TextBox> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelSubsTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Subtotals">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="LabelExpensesLine" runat="server" Text='<%# Eval("TotalExpenses","{0:C2}") %>' ></asp:Label> 
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <div style="background-color:none;height:50px;border-top:3px solid black;">
                                                                                        <asp:Label ID="LabelExpensesTotal" runat="server"></asp:Label>
                                                                                    </div>
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                                <div class="col-sm-4 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                    <table class="table table-hover table-sm" style="width:400px;text-align:right;font-weight:bold;">
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>Total Management Salary/Benefits Costs</td>
                                                                                <td>
                                                                                    <asp:Label ID="LabelMgrSalBens" runat="server" ></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Expenses</td>
                                                                                <td>
                                                                                    <asp:Label ID="LabelExpenses" runat="server" ></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Other</td>
                                                                                <td style="text-align:right !important;">
                                                                                    <asp:TextBox ID="TextBoxOtherMgrDlr" runat="server" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" CssClass="text-right"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr style="background-color:lightskyblue;">
                                                                                <td>Budget Management Labor Costs</td>
                                                                                <td>
                                                                                    <asp:Label ID="LabelMgmtGrandTotalCosts" runat="server"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>

                                                                </div>
                                                                <div class="col-sm-2 overflow-auto" style="text-align:center;white-space: nowrap;">
                                                                    Notes:<br />
                                                                    <asp:TextBox ID="TextBoxMgmtNotes" runat="server" Width="100%" Rows="5" TextMode="MultiLine" AutoPostBack="true" OnTextChanged="TextBoxMgmtHdr_TextChanged" ></asp:TextBox>
                                                                </div>
                                                            </div>
                                                          </div>
                                                        </div>
                                                      </div>
                                                      <div class="card">
                                                        <div class="card-header" id="headingFive">
                                                          <h2 class="mb-0">
                                                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseFive" aria-expanded="false" aria-controls="collapseFive">
                                                              FMS NI Budget
                                                            </button>
                                                          </h2>
                                                        </div>
                                                        <div id="collapseFive" class="collapse" aria-labelledby="headingThree" data-parent="#accordionBudgets">
                                                          <div class="card-body">
                                                            <div class="row">
                                                                <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                    <h5>Facilities Management Systems (FMS) Budget</h5>
                                                                </div>
                                                            </div>
                                                              <div class="row">
                                                                <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                    <h5>NI Side</h5>
                                                                </div>
                                                              </div>
                                                            <div class="row">
                                                                <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                    <strong>Budget Year:</strong>
                                                                    <asp:Label ID="LabelBudgetYearNI" runat="server" Text='<%# Bind("BudgetYear") %>'></asp:Label>
                                                                    &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                    <strong>District:</strong>
                                                                    <asp:Label ID="LabelDistrictIDNI" runat="server" Text='<%# Bind("DistrictID") %>'></asp:Label>
                                                                    &nbsp;&nbsp;&nbsp;&nbsp; 
                                                                    <strong>Version:</strong>
                                                                    <asp:Label ID="LabelBudgetVersionNI1" runat="server" Text='<%# Bind("BudgetVersion") %>'></asp:Label>
                                                                </div>
                                                            </div>
                                                              <div class="row">
                                                                  <div class="col-sm-12" style="top:10px;position:relative;text-align:center;">
                                                                      <table class="table table-hover table-sm " style="max-width:500px;margin-left:auto;margin-right:auto;line-height:15px;text-align:right;">
                                                                          <tbody>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>Management Fee</td>
                                                                                  <td><asp:TextBox ID="TextBoxManagementFeeNI" runat="server" Text='<%# Eval("ManagementFee","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" ReadOnly="true"></asp:TextBox></td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>Administrative Fee</td>
                                                                                  <td><asp:TextBox ID="TextBoxAdministrativeFeeNI" runat="server" Text='<%# Eval("AdminFee","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" ReadOnly="true"></asp:TextBox></td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>Hourly P/P</td>
                                                                                  <td><asp:TextBox ID="TextBoxHourlyPPNI" runat="server" Text='<%#Eval("TotalHourlyPP","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" ReadOnly="true"></asp:TextBox></td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>Management P/P</td>
                                                                                  <td><asp:TextBox ID="TextBoxManagementPPNI" runat="server" Text='<%# Eval("TotalManagementPP","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" ReadOnly="true"></asp:TextBox></td></td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>Training</td>
                                                                                  <td><asp:TextBox ID="TextBoxTrainingNI" runat="server" Text='<%# Eval("TotalTrainingNI","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" ReadOnly="true"></asp:TextBox></td></td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>Technology</td>
                                                                                  <td><asp:TextBox ID="TextBoxTechnologyNI" runat="server" Text='<%# Eval("SoftwareUpgradeNUTR","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" ReadOnly="true"></asp:TextBox></td></td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>Recruitment</td>
                                                                                  <td><asp:TextBox ID="TextBoxRecruitmentNI" runat="server" Text='<%# Eval("TotalEmpRecruitNI","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" ReadOnly="true"></asp:TextBox></td></td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>Liability</td>
                                                                                  <td><asp:TextBox ID="TextBoxLiabilityNI" runat="server" Text='<%# Eval("TotalLiabilityNI","{0:C2}") %>' CssClass="text-sm-right" Font-Bold="true" ReadOnly="true"></asp:TextBox></td></td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>&nbsp;</td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                              <tr>
                                                                                  <td>&nbsp;</td>
                                                                                  <td><strong>TOTAL</strong></td>
                                                                                  <td><asp:TextBox ID="TextBoxTotalNI" runat="server" Text='<%# ((decimal)Eval("ManagementFee") + (decimal)Eval("AdminFee") + (decimal)Eval("TotalHourlyPP") + (decimal)Eval("TotalManagementPP") + (decimal)Eval("TotalTrainingNI") + (decimal)Eval("SoftwareUpgradeNUTR") + (decimal)Eval("TotalEmpRecruitNI") + (decimal)Eval("TotalLiabilityNI")).ToString("C2")  %>' CssClass="text-sm-right" Font-Bold="true" ReadOnly="true"></asp:TextBox></td></td>
                                                                                  <td>&nbsp;</td>
                                                                              </tr>
                                                                          </tbody>
                                                                      </table>
                                                                  </div>
                                                              </div>
                                                              <div class="row">
                                                                  <div class="col">&nbsp;</div>
                                                              </div>

                                                          </div>
                                                        </div>
                                                      </div>
                                                    </div>

                                                </div>
                                            </EditItemTemplate>
                                        </asp:FormView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceBudgetYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader" 
                    SelectCommand="SELECT '' BudgetYear, '-- Select Budget Year --' BudgetDescription, 'ZZZZZZZZZZZZ' SortOrder
                    UNION ALL                        
                    SELECT [BudgetYear], [BudgetDescription], [BudgetYear] SortOrder FROM [FMSBudgetYear_Setup] 
                    ORDER BY SortOrder DESC"></asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceBudgetYearsAdd" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader" 
                    SelectCommand="SELECT '' BudgetYear, '-- Select Budget Year --' BudgetDescription, 'ZZZZZZZZZZZZ' SortOrder
                    UNION ALL                        
                    SELECT [BudgetYear], [BudgetDescription], [BudgetYear] SortOrder FROM [FMSBudgetYear_Setup]
                    WHERE CB_AllowNewBudgets=1
                    ORDER BY SortOrder DESC"></asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceBudgetHdrs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader" 
                    SelectCommand="SELECT * FROM FMSBudgetHeader WHERE DistrictID=@DistrictID">
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceContracts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataSet" 
                    SelectCommand="SELECT [DistrictID], [ContractID], [ContractDescription] FROM [DistrictContracts] WHERE [DistrictID]=@DistrictID ORDER BY ContractID" 
                    FilterExpression="ContractID LIKE '%{0}%' OR ContractDescription LIKE '%{0}%' ">
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                    </SelectParameters>
                    <FilterParameters>
                        <asp:ControlParameter Name="TextFilter" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" ControlID="TextBoxSearchContracts" PropertyName="Text"  />
                    </FilterParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceHourlyEmps" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataSet" OnInserting="SqlDataSourceHourlyEmps_Inserting"
                    OnDeleted="SqlDataSourceHourlyEmps_Deleted" OnUpdating="SqlDataSourceHourlyEmps_Updating" OnInserted="SqlDataSourceHourlyEmps_Inserted"
                    UpdateCommand="UPDATE [FMSBudgetLaborHourlyEmp] SET [SchoolNum]=@SchoolNum,[EmpName]=@EmpName,[Job]=@Job,[EmpSchedule]=@EmpSchedule,[HourlyRate]=@HourlyRate,[HoursDay]=@HoursDay,[DaysPaid]=@DaysPaid
                    ,[PlanType]=@PlanType,[Health]=@Health,[Dental]=@Dental,[Disability]=@Disability,[Vision]=@Vision,[Annuity]=@Annuity,[Retire]=@Retire,[Life]=@Life,[OtherBenefit]=@OtherBenefit 
                    WHERE [BudgetYear]=@BudgetYear AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersion AND ORD=@ORD"
                    SelectCommand="SELECT E.[BudgetYear],E.[DistrictID],E.[BudgetVersion],E.[ORD],[SchoolNum],[EmpName],[Job],[EmpSchedule],[HourlyRate],[HoursDay],[DaysPaid],[Wages]
                    ,[FICA],[FUTA],[SUI],[PayrollRelated],[WComp],[Fringes],[TotalBenefits],[GrandTotal]
                    ,[PlanType],[Health],[Dental],[Disability],[Vision],[Annuity],[Retire],[Life],[OtherBenefit]
                    FROM [FMSBudgetLaborHourly] H
                    INNER JOIN [FMSBudgetLaborHourlyEmp] E  ON E.[BudgetYear]=H.[BudgetYear] AND E.[DistrictID]=H.[DistrictID] AND E.[BudgetVersion]=H.[BudgetVersion]
                    CROSS APPLY (SELECT [HourlyRate]*[HoursDay]*[DaysPaid] Wages) CALC1
                    CROSS APPLY (SELECT [Wages]*[FICAPCT] FICA, IIF(Wages>[FUTADLR],[FUTADLR]*[FUTAPCT], [Wages]*[FUTAPCT]) FUTA, IIF([Wages]>[SUIDLR],[SUIDLR]*[SUIPCT], [Wages]*[SUIPCT]) SUI,
                    [Wages]*[relatedPayrollPCT] PayrollRelated, [Wages]*[WCompPct] WComp) CALC2
                    CROSS APPLY (SELECT FICA+FUTA+SUI+PayrollRelated+WComp Fringes, [Health]+[Dental]+[Disability]+[Vision]+[Annuity]+[Retire]+[Life]+[OtherBenefit] TotalBenefits) CALC3
                    CROSS APPLY (SELECT [Wages]+[Fringes]+[TotalBenefits] GrandTotal) CALC4
                    WHERE H.[BudgetYear]=@BudgetYear AND H.[DistrictID]=@DistrictID AND H.[BudgetVersion]=@BudgetVersion
                    UNION ALL
                    SELECT @BudgetYear BudgetYear, @DistrictID DistrictID, @BudgetVersion BudgetVersion,0 ORD,0,'','','',0,0,0,0
                    ,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0"

                    DELETECommand="DELETE E FROM [FMSBudgetLaborHourlyEmp] E WHERE E.[BudgetYear]=@BudgetYear AND E.[DistrictID]=@DistrictID AND E.[BudgetVersion]=@BudgetVersion AND E.[ORD]=@ORD"
                    InsertCommand="INSERT INTO [FMSBudgetLaborHourlyEmp]([BudgetYear],[DistrictID],[BudgetVersion],[ORD],[SchoolNum],[EmpName],[Job],[EmpSchedule],[HourlyRate],[HoursDay],[DaysPaid],[PlanType],[Health],[Dental],[Disability],[Vision],[Annuity],[Retire],[Life],[OtherBenefit]) 
                    VALUES(@BudgetYear,@DistrictID,@BudgetVersion,(SELECT ISNULL(MAX(ORD),0)+1 FROM [FMSBudgetLaborHourlyEmp] E WHERE E.[BudgetYear]=@BudgetYear AND E.[DistrictID]=@DistrictID AND E.[BudgetVersion]=@BudgetVersion),@SchoolNum,@EmpName,@Job,@EmpSchedule,@HourlyRate,@HoursDay,@DaysPaid,@PlanType,@Health,@Dental,@Disability,@Vision,@Annuity,@Retire,@Life,@OtherBenefit)"
                    >
                    <SelectParameters>
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="BudgetYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="BudgetVersion" Type="Int32" />
                        <asp:Parameter Name="ORD" Type="Int32" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                        <asp:Parameter Name="SchoolNum" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="EmpName" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Job" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="EmpSchedule" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="HourlyRate" Type="Decimal" />
                        <asp:Parameter Name="HoursDay" Type="Decimal" />
                        <asp:Parameter Name="DaysPaid" Type="Int32" />
                        <asp:Parameter Name="PlanType" Type="Int32" />
                        <asp:Parameter Name="Health" Type="Decimal" />
                        <asp:Parameter Name="Dental" Type="Decimal" />
                        <asp:Parameter Name="Disability" Type="Decimal" />
                        <asp:Parameter Name="Vision" Type="Decimal" />
                        <asp:Parameter Name="Annuity" Type="Decimal" />
                        <asp:Parameter Name="Retire" Type="Decimal" />
                        <asp:Parameter Name="Life" Type="Decimal" />
                        <asp:Parameter Name="OtherBenefit" Type="Decimal" />
                    </InsertParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="BudgetYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="BudgetVersion" Type="Int32" />
                        <asp:Parameter Name="ORD" Type="Int32" />
                        <asp:Parameter Name="SchoolNum" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="EmpName" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Job" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="EmpSchedule" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="HourlyRate" Type="Decimal" />
                        <asp:Parameter Name="HoursDay" Type="Decimal" />
                        <asp:Parameter Name="DaysPaid" Type="Int32" />
                        <asp:Parameter Name="PlanType" Type="Int32" />
                        <asp:Parameter Name="Health" Type="Decimal" />
                        <asp:Parameter Name="Dental" Type="Decimal" />
                        <asp:Parameter Name="Disability" Type="Decimal" />
                        <asp:Parameter Name="Vision" Type="Decimal" />
                        <asp:Parameter Name="Annuity" Type="Decimal" />
                        <asp:Parameter Name="Retire" Type="Decimal" />
                        <asp:Parameter Name="Life" Type="Decimal" />
                        <asp:Parameter Name="OtherBenefit" Type="Decimal" />
                    </UpdateParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
                    SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID AND INACTIVE=0 ORDER BY SortOrder, SchoolName">
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceMgmtHdr" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
                    OnUpdating="SqlDataSourceMgmtHdr_Updating"
                    SelectCommand="SELECT [FICAPCT],[FUTAPCT],[FUTADLR],[SUIPCT],[SUIDLR],[RelatedPayrollPCT],[WCompPCT],Mgmt.[OtherLaborDLR],[MgmtLaborNotes],[PctBudgetIncrease]
                    ,ISNULL([TotalNewSalary],0)TotalNewSalary,ISNULL([TotalBonus],0)TotalBonus,ISNULL([TotalHealthBuyout],0)TotalHealthBuyout,ISNULL([TotalWages],0)TotalWages,ISNULL([TotalFica],0)TotalFica,ISNULL([TotalFuta],0)TotalFuta,ISNULL([TotalSui],0)TotalSui,ISNULL([TotalPayrel],0)TotalPayrel,ISNULL([TotalWComp],0)TotalWComp,ISNULL([GrandTotalMandatory],0)GrandTotalMandatory,ISNULL([TotalHealth],0) TotalHealth
                    ,ISNULL([TotalLife],0)TotalLife,ISNULL([TotalDental],0)TotalDental,ISNULL([TotalVision],0)TotalVision,ISNULL([TotalOtherBenefit],0)TotalOtherBenefit,ISNULL([GrandTotalBenefits],0)GrandTotalBenefits,ISNULL([Total401k],0)Total401k,ISNULL([GrandTotalExpenses],0)GrandTotalExpenses,ISNULL([SubtotalWageMandBens],0)SubtotalWageMandBens,ISNULL([GrandTotalMgmtCost],0)GrandTotalMgmtCost, ISNULL([GrandTotalMandatory]+[GrandTotalBenefits]+[Total401k],0) GrandTotalFringeBens
                    ,ISNULL([TotalTravel],0) TotalTravel,ISNULL([TotalDues],0) TotalDues,ISNULL([TotalEducation],0) TotalEducation,ISNULL([TotalSubscriptions],0) TotalSubscriptions,ISNULL([TotalCellBenefit],0) TotalCellBenefit,ISNULL([TotalFitnessCredit],0) TotalFitnessCredit 
                    FROM [FMSBudgetLaborMgmt] Mgmt
                    LEFT OUTER JOIN [vw_FMSBudgetLaborMgmtTotals] TTL ON Mgmt.[BudgetYear]=TTL.[BudgetYear] AND Mgmt.[DistrictID]=TTL.[DistrictID] AND Mgmt.[BudgetVersion]=TTL.[BudgetVersion]
                    WHERE Mgmt.[BudgetYear]=@BudgetYear AND Mgmt.[DistrictID]=@DistrictID AND Mgmt.[BudgetVersion]=@BudgetVersion"
                    UpdateCommand="UPDATE [FMSBudgetLaborMgmt] SET [PctBudgetIncrease]=@PctBudgetIncrease,[FICAPCT]=@FICAPCT,[FUTAPCT]=@FUTAPCT,[FUTADLR]=@FUTADLR,[SUIPCT]=@SUIPCT,[SUIDLR]=@SUIDLR,[RelatedPayrollPCT]=@RelatedPayrollPCT,[WCompPCT]=@WCompPCT
                    ,[OtherLaborDLR]=@OtherLaborDLR,[MgmtLaborNotes]=@MgmtLaborNotes
                    WHERE [BudgetYear]=@BudgetYear AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersion"
                    >
                    <SelectParameters>
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="FICAPCT" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="FUTAPCT" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="FUTADLR" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="SUIPCT" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="SUIDLR" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="RelatedPayrollPCT" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="WCompPct" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="OtherLaborDlr" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="MgmtLaborNotes" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                        <asp:Parameter Name="PctBudgetIncrease" Type="Decimal" DefaultValue="0" />
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                    </UpdateParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceHourlyHdr" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
                 OnUpdating="SqlDataSourceHourlyHdr_Updating"
                    SelectCommand="SELECT [FICAPCT],[FUTAPCT],[FUTADLR],[SUIPCT],[SUIDLR],[RelatedPayrollPCT],[WCompPct],[OtherLaborDlr],[HourlyLaborNotes]
                      ,ISNULL([TotalWages],0) TotalWages,ISNULL([TotalFica],0) TotalFica,ISNULL([TotalFuta],0) TotalFuta,ISNULL([TotalSUI],0) TotalSUI,ISNULL([TotalPayRel],0) TotalPayRel
	                  ,ISNULL([TotalWComp],0) TotalWComp,ISNULL([TotalHealth],0) TotalHealth,ISNULL([TotalDental],0) TotalDental,ISNULL([TotalDisability],0) TotalDisability,ISNULL([TotalVision],0) TotalVision
                      ,ISNULL([TotalAnnuity],0) TotalAnnuity,ISNULL([TotalRetire],0) TotalRetire,ISNULL([TotalLife],0) TotalLife,ISNULL([TotalOtherBenefit],0) TotalOtherBenefit
	                  ,ISNULL([GrandTotalBenefits],0) GrandTotalBenefits,ISNULL([GrandTotalFringes],0) GrandTotalFringes,ISNULL([TotalOtherLaborDlr],0) TotalOtherLaborDlr,ISNULL([TotalHoursDay],0) TotalHoursDay
                    FROM [FMSBudgetLaborHourly] HRLY
                    LEFT OUTER JOIN vw_FMSBudgetLaborHourlyTotals TTL ON HRLY.[BudgetYear]=TTL.[BudgetYear] AND HRLY.[DistrictID]=TTL.[DistrictID] AND HRLY.[BudgetVersion]=TTL.[BudgetVersion]
                    WHERE HRLY.[BudgetYear]=@BudgetYear AND HRLY.[DistrictID]=@DistrictID AND HRLY.[BudgetVersion]=@BudgetVersion"
                    UpdateCommand="UPDATE [FMSBudgetLaborHourly] SET [FICAPCT]=@FICAPCT,[FUTAPCT]=@FUTAPCT,[FUTADLR]=@FUTADLR,[SUIPCT]=@SUIPCT,[SUIDLR]=@SUIDLR,[RelatedPayrollPCT]=@RelatedPayrollPCT
                    ,[WCompPct]=@WCompPct,[OtherLaborDlr]=@OtherLaborDlr,[HourlyLaborNotes]=@HourlyLaborNotes
                    WHERE [BudgetYear]=@BudgetYear AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersion">
                    <SelectParameters>
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="FICAPCT" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="FUTAPCT" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="FUTADLR" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="SUIPCT" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="SUIDLR" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="RelatedPayrollPCT" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="WCompPct" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="OtherLaborDlr" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="HourlyLaborNotes" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                    </UpdateParameters>

                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSelectedBudget" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataSet"
                    SelectCommand="SELECT HDR.[BudgetYear],HDR.[DistrictID],HDR.[BudgetVersion],HDR.[Active],HDR.[BudgetDescription],HDR.[StartingDate],HDR.[EndingDate],HDR.[LockedBudget],HDR.[ContractID]
                      ,D.[BackgroundCheckNUTR],D.[CarTruckRentNUTR],D.[CarTruckRentDIST],D.[CarTruckMileageDIST],D.[CarTruckMaintNUTR],D.[CarTruckMaintDIST],D.[SoftwareUpgradeNUTR],D.[TechnologyFeeDIST]
                      ,D.[CourierNUTR],D.[EmployeeMealsNUTR],D.[EmpRecruitmentNUTR],D.[EmpRecruitmentDIST],D.[EmpReplacementNUTR],D.[EmpReplacementDIST],D.[FreightDelivNUTR],D.[IndirectLaborNUTR],D.[IndirectLaborDIST]
                      ,D.[LiabilityEquipNUTR],D.[LiabilityInsurNUTR],D.[VehicleInsuranceNUTR],D.[LicenseFeeNUTR],D.[OfficeExpenseNUTR],D.[OfficeExpenseDIST],D.[PrintingNUTR],D.[PrintingDIST]
                      ,D.[PerformanceBondNUTR],D.[PostageNUTR],D.[PostageDIST],D.[StaffTrainingNUTR],D.[TrainingNUTR],D.[TrainingDIST],D.[CommNUTR],D.[LaundryNUTR],D.[LaundryDIST],D.[UniformsNUTR],D.[UniformsDIST]
                      ,ISNULL(DFN.DFNTravel,0) DFNTravel,ISNULL(DFN.DFNDues,0)+ISNULL(DFN.DFNSubscriptions,0) DFNDuesSubs,ISNULL(DFN.DFNEducation,0) DFNEducation
                      ,E.ManagementFee, E.AdminFee, ISNULL(VWH.TotalWages,0) TotalWages,ISNULL(VWH.GrandTotalFringes,0) GrandTotalFringes,ISNULL(VWH.GrandTotalBenefits,0) GrandTotalBenefits, ISNULL(VWH.TotalOtherLaborDlr,0) TotalOtherLaborDlr
                      ,ISNULL([GrandTotalMgmtCost],0) [GrandTotalMgmtCost], ISNULL(VWH.[TotalPayRel],0) TotalHourlyPP, ISNULL(VWM.[TotalPayRel],0) TotalManagementPP, (D.[StaffTrainingNUTR]+D.[TrainingNUTR]+ISNULL(DFN.DFNEducation,0)) TotalTrainingNI
                      ,ISNULL(D.[EmpRecruitmentNUTR],0)+ISNULL(D.[EmpReplacementNUTR],0)+ISNULL(D.[IndirectLaborNUTR],0) TotalEmpRecruitNI, ISNULL(D.[LiabilityEquipNUTR],0)+ISNULL(D.[LiabilityInsurNUTR],0) TotalLiabilityNI
                      ,ISNULL(DC.ContractDescription,'') ContractDescription,ISNULL(HDR.CoversheetNotes,'') CoversheetNotes
                    FROM [FMSBudgetHeader] HDR
                    LEFT OUTER JOIN [FMSBudgetDirectCosts] D ON HDR.BudgetYear=D.BudgetYear AND HDR.DistrictID=D.DistrictID AND HDR.BudgetVersion=D.BudgetVersion
                    LEFT OUTER JOIN (SELECT [BudgetYear],[DistrictID],[BudgetVersion],SUM([ExpnTravel]) DFNTravel,SUM([ExpnDues]) DFNDues,SUM([ExpnEducation]) DFNEducation,SUM([ExpnSubscriptions]) DFNSubscriptions
                    FROM [FMSBudgetLaborMgmtEmp] GROUP BY [BudgetYear],[DistrictID],[BudgetVersion]) DFN ON HDR.BudgetYear=DFN.BudgetYear AND HDR.DistrictID=DFN.DistrictID AND HDR.BudgetVersion=DFN.BudgetVersion
                    LEFT OUTER JOIN [FMSBudgetExpenses] E ON HDR.BudgetYear=E.BudgetYear AND HDR.DistrictID=E.DistrictID AND HDR.BudgetVersion=E.BudgetVersion
                    LEFT OUTER JOIN [vw_FMSBudgetLaborHourlyTotals] VWH ON HDR.BudgetYear=VWH.BudgetYear AND HDR.DistrictID=VWH.DistrictID AND HDR.BudgetVersion=VWH.BudgetVersion
                    LEFT OUTER JOIN [vw_FMSBudgetLaborMgmtTotals] VWM ON HDR.BudgetYear=VWM.BudgetYear AND HDR.DistrictID=VWM.DistrictID AND HDR.BudgetVersion=VWM.BudgetVersion
                    LEFT OUTER JOIN [DistrictContracts] DC ON HDR.ContractID=DC.ContractID AND HDR.DistrictID=DC.DistrictID
                    WHERE HDR.BudgetYear=@BudgetYear AND HDR.DistrictID=@DistrictID AND HDR.BudgetVersion=@BudgetVersion"
                    UpdateCommand="UPDATE [FMSBudgetHeader] SET [StartingDate]=@StartingDate,[EndingDate]=@EndingDate,[BudgetDescription]=@BudgetDescription,Active=@Active,LockedBudget=@LockedBudget,[ContractID]=@ContractID,[CoversheetNotes]=@CoversheetNotes
                    WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@budgetVersion;
                    UPDATE [FMSBudgetExpenses] SET [ManagementFee]=@ManagementFee,[AdminFee]=@AdminFee WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@budgetVersion;
                    UPDATE [FMSBudgetDirectCosts] SET BackgroundCheckNUTR=@BackgroundCheckNUTR,[CarTruckRentNUTR]=@CarTruckRentNUTR,[CarTruckRentDIST]=@CarTruckRentDIST,[CarTruckMileageDIST]=@CarTruckMileageDIST
                    ,[CarTruckMaintNUTR]=@CarTruckMaintNUTR,[CarTruckMaintDIST]=@CarTruckMaintDIST,[SoftwareUpgradeNUTR]=@SoftwareUpgradeNUTR,[TechnologyFeeDIST]=@TechnologyFeeDIST
                    ,[CourierNUTR]=@CourierNUTR,[EmployeeMealsNUTR]=@EmployeeMealsNUTR,[EmpRecruitmentNUTR]=@EmpRecruitmentNUTR,[EmpRecruitmentDIST]=@EmpRecruitmentDIST,[EmpReplacementNUTR]=@EmpReplacementNUTR
                    ,[EmpReplacementDIST]=@EmpReplacementDIST,[IndirectLaborNUTR]=@IndirectLaborNUTR,[IndirectLaborDIST]=@IndirectLaborDIST
                    ,[LiabilityEquipNUTR]=@LiabilityEquipNUTR,[LiabilityInsurNUTR]=@LiabilityInsurNUTR,[VehicleInsuranceNUTR]=@VehicleInsuranceNUTR,[LicenseFeeNUTR]=@LicenseFeeNUTR,[OfficeExpenseNUTR]=@OfficeExpenseNUTR
                    ,[OfficeExpenseDIST]=@OfficeExpenseDIST,[PrintingNUTR]=@PrintingNUTR,[PrintingDIST]=@PrintingDIST
                    ,[PerformanceBondNUTR]=@PerformanceBondNUTR,[PostageNUTR]=@PostageNUTR,[PostageDIST]=@PostageDIST,[StaffTrainingNUTR]=@StaffTrainingNUTR,[TrainingNUTR]=@TrainingNUTR,[TrainingDIST]=@TrainingDIST
                    ,[CommNUTR]=@CommNUTR,[LaundryNUTR]=@LaundryNUTR,[LaundryDIST]=@LaundryDIST,[UniformsNUTR]=@UniformsNUTR,[UniformsDIST]=@UniformsDIST,[FreightDelivNUTR]=@FreightDelivNUTR
                    WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@BudgetVersion"
                    DeleteCommand="DELETE [FMSBudgetHeader] WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@BudgetVersion;
                    DELETE [FMSBudgetDirectCosts] WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@BudgetVersion;
                    DELETE [FMSBudgetExpenses] WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@BudgetVersion;
                    DELETE [FMSBudgetLaborHourly] WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@BudgetVersion;
                    DELETE [FMSBudgetLaborHourlyEmp] WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@BudgetVersion;
                    DELETE [FMSBudgetLaborMgmt] WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@BudgetVersion;
                    DELETE [FMSBudgetLaborMgmtEmp] WHERE BudgetYear=@BudgetYear AND DistrictID=@DistrictID AND BudgetVersion=@BudgetVersion;" 
                    OnUpdated="SqlDataSourceSelectedBudget_Updated" OnDeleted="SqlDataSourceSelectedBudget_Deleted" OnUpdating="SqlDataSourceSelectedBudget_Updating">
                    <SelectParameters>
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                    </DeleteParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceMgmtEmps" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataSet" OnInserting="SqlDataSourceMgmtEmps_Inserting"
                    OnDeleted="SqlDataSourceMgmtEmps_Deleted" OnUpdating="SqlDataSourceMgmtEmps_Updating"
                    UpdateCommand="UPDATE [FMSBudgetLaborMgmtEmp] SET [ManagerName]=@ManagerName,[Job]=@Job,[CurrentSalary]=@CurrentSalary,[CurrentBonus]=@CurrentBonus,[CurrentHealthBuyout]=@CurrentHealthBuyout
                    ,[WorkWeeks]=@WorkWeeks,[Health]=@Health,[PlanType]=@PlanType,[Life]=@Life,[Dental]=@Dental,[Vision]=@Vision,[OtherBenefit]=@OtherBenefit,[Ben401k]=@Ben401k
                    ,[ExpnTravel]=@ExpnTravel,[ExpnDues]=@ExpnDues,[ExpnEducation]=@ExpnEducation,[ExpnSubscriptions]=@ExpnSubscriptions,[CellBenefit]=@CellBenefit,[FitnessCredit]=@FitnessCredit
                    WHERE [BudgetYear]=@BudgetYear AND [DistrictID]=@DistrictID AND [BudgetVersion]=@BudgetVersion AND ORD=@ORD"
                    DeleteCommand="DELETE E FROM [FMSBudgetLaborMgmtEmp] E WHERE E.[BudgetYear]=@BudgetYear AND E.[DistrictID]=@DistrictID AND E.[BudgetVersion]=@BudgetVersion AND E.[ORD]=@ORD"
                    InsertCommand="INSERT INTO [FMSBudgetLaborMgmtEmp]([BudgetYear],[DistrictID],[BudgetVersion],[ORD],[ManagerName],[Job],[CurrentSalary],[CurrentBonus],[CurrentHealthBuyout],[WorkWeeks],[PlanType],[Health],[Life]
                    ,[Dental],[Vision],[OtherBenefit],[Ben401k],[ExpnTravel],[ExpnDues],[ExpnEducation],[ExpnSubscriptions],[CellBenefit],[FitnessCredit]) 
                    VALUES(@BudgetYear,@DistrictID,@BudgetVersion,(SELECT ISNULL(MAX(ORD),0)+1 FROM [FMSBudgetLaborMgmtEmp] E WHERE E.[BudgetYear]=@BudgetYear AND E.[DistrictID]=@DistrictID AND E.[BudgetVersion]=@BudgetVersion)
                    ,@ManagerName,@Job,@CurrentSalary,@CurrentBonus,@CurrentHealthBuyout,@WorkWeeks,@PlanType,@Health,@Life,@Dental,@Vision,@OtherBenefit,@Ben401k,@ExpnTravel,@ExpnDues,@ExpnEducation,@ExpnSubscriptions)"
                    
                    SelectCommand="SELECT E.[BudgetYear],E.[DistrictID],E.[BudgetVersion],E.[ORD],[ManagerName],[Job],[CurrentSalary],[CurrentBonus],[CurrentHealthBuyout],[WorkWeeks],[PlanType],[Health],[Life]
                      ,[Dental],[Vision],[OtherBenefit],[Ben401k],[ExpnTravel],[ExpnDues],[ExpnEducation],[ExpnSubscriptions],[OtherLaborDLR],[MgmtLaborNotes],[PctBudgetIncrease]
	                  ,[FICA],[FUTA],[SUI],[PayrollRelated],[WComp],[Fringes],[TotalBenefits],[GrandTotal],[TotalExpenses],[NewSalary],[Wages],[GrandTotalFringeBens],[CellBenefit],[FitnessCredit]
                    FROM [FMSBudgetLaborMgmtEmp] E 
                    INNER JOIN [FMSBudgetLaborMgmt] M ON E.[BudgetYear]=M.[BudgetYear] AND E.[DistrictID]=M.[DistrictID] AND E.[BudgetVersion]=M.[BudgetVersion]
                    CROSS APPLY (SELECT ([CurrentSalary]+([CurrentSalary]*[PctBudgetIncrease])) NewSalary) CALC1
                    CROSS APPLY (SELECT (CALC1.[NewSalary]/52*[WorkWeeks]) + [CurrentBonus] + [CurrentHealthBuyout] + [CellBenefit] + [FitnessCredit] Wages) CALC2
                    CROSS APPLY (SELECT [Wages]*[FICAPCT] FICA, IIF(Wages>[FUTADLR],[FUTADLR]*[FUTAPCT], [Wages]*[FUTAPCT]) FUTA, IIF([Wages]>[SUIDLR],[SUIDLR]*[SUIPCT], [Wages]*[SUIPCT]) SUI, [Wages]*[relatedPayrollPCT] PayrollRelated, [Wages]*[WCompPct] WComp) CALC3
                    CROSS APPLY (SELECT FICA+FUTA+SUI+PayrollRelated+WComp Fringes, [Health]+[Dental]+[Vision]+[Life]+[OtherBenefit] TotalBenefits, [ExpnTravel]+[ExpnDues]+[ExpnEducation]+[ExpnSubscriptions] TotalExpenses) CALC4
                    CROSS APPLY (SELECT [Wages]+[Fringes]+[TotalBenefits]+[Ben401k]+[TotalExpenses]+[OtherLaborDLR] GrandTotal, [Fringes]+[TotalBenefits]+[Ben401k] GrandTotalFringeBens) CALC5
                    WHERE (E.[BudgetYear]=@BudgetYear AND E.[DistrictID]=@DistrictID AND E.[BudgetVersion]=@BudgetVersion)
                    UNION ALL 
                    SELECT @BudgetYear, @DistrictID, @BudgetVersion, 0 ORD,'','',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,'',0,0,0,0,0,0,0,0,0,0,0[NewSalary],0[Wages],0[GrandTotalFringeBens],0[CellBenefit],0[FitnessCredit]"
                    >
                    <SelectParameters>
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="BudgetYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="BudgetVersion" Type="Int32" />
                        <asp:Parameter Name="ORD" Type="Int32" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:ControlParameter Name="BudgetYear" Type="String" ControlID="HFBudgetYear" PropertyName="Value" />
                        <asp:ControlParameter Name="DistrictID" Type="String" ControlID="HFDistrictID" PropertyName="Value" />
                        <asp:ControlParameter Name="BudgetVersion" Type="Int32" ControlID="HFBudgetVersion" PropertyName="Value" />
                        <asp:Parameter Name="ManagerName" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Job" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                        <asp:Parameter Name="CurrentSalary" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="CurrentBonus" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="CurrentHealthBuyout" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="WorkWeeks" Type="Int32" DefaultValue="0" />
                        <asp:Parameter Name="PlanType" Type="Int32" DefaultValue="0" />
                        <asp:Parameter Name="Health" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="Life" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="Dental" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="Vision" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="OtherBenefit" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="Ben401k" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="ExpnTravel" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="ExpnDues" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="ExpnEducation" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="ExpnSubscriptions" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="CellBenefit" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="FitnessCredit" Type="Decimal" DefaultValue="0" />
                    </InsertParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="BudgetYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="BudgetVersion" Type="Int32" />
                        <asp:Parameter Name="ORD" Type="Int32" />
                        <asp:Parameter Name="ManagerName" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Job" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="CurrentSalary" Type="Decimal" />
                        <asp:Parameter Name="CurrentBonus" Type="Decimal" />
                        <asp:Parameter Name="CurrentHealthBuyout" Type="Int32" />
                        <asp:Parameter Name="WorkWeeks" Type="Int32" />
                        <asp:Parameter Name="PlanType" Type="Int32" />
                        <asp:Parameter Name="Health" Type="Decimal" />
                        <asp:Parameter Name="Life" Type="Decimal" />
                        <asp:Parameter Name="Dental" Type="Decimal" />
                        <asp:Parameter Name="Vision" Type="Decimal" />
                        <asp:Parameter Name="OtherBenefit" Type="Decimal" />
                        <asp:Parameter Name="Ben401k" Type="Decimal" />
                        <asp:Parameter Name="ExpnTravel" Type="Decimal" />
                        <asp:Parameter Name="ExpnDues" Type="Decimal" />
                        <asp:Parameter Name="ExpnEducation" Type="Decimal"  />
                        <asp:Parameter Name="ExpnSubscriptions" Type="Decimal" />
                        <asp:Parameter Name="CellBenefit" Type="Decimal" />
                        <asp:Parameter Name="FitnessCredit" Type="Decimal" />
                    </UpdateParameters>
                </asp:SqlDataSource>
            </div>

            <div class="modal fade" id="myModal">  
                <div class="modal-dialog" >  
                    <div class="modal-content" style="width:800px;" > 
                        <div class="modal-header">  
                            <h4 class="modal-title">Copy Budget</h4>  
                            <button type="button" class="close" data-dismiss="modal">×</button>  
                        </div>  
                        <div class="modal-body">  
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <label>Budget you are copying:</label> 
                                        <br />
                                            Budget Year: <asp:Label ID="LabelBudgetYearSrc" runat="server" ></asp:Label><br />
                                            Budget Version: <asp:Label ID="LabelBudgetVersionSrc" runat="server" ></asp:Label><br />
                                    </div>
                                </div>
                            </div>
                            <div class="row">  
                                <div class="col-md-12">  
                                    <div class="form-group">  
                                        <label>Select budget year you want to create the copy in.</label>  
                                        <div class="input-group">  
                                            <asp:DropDownList ID="DDLDestBudgetYear" runat="server" DataSourceID="SqlDataSourceBudgetYearsAdd" DataTextField="BudgetDescription" 
                                                DataValueField="BudgetYear" ></asp:DropDownList>

                                        </div>  
                                    </div>  
                                </div>  
                            </div>  
                        </div>  
                        <div class="modal-footer">  
                            <asp:Button ID="ButtonCreateCopy" runat="server" CssClass="btn btn-outline-primary" Text="Create" OnClick="ButtonCreateCopy_Click" data-toggle="modal" data-target="#myModal" UseSubmitBehavior="true"  /> 
                            <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Cancel</button>  
                        </div>  
                    </div>  
                </div>  
            </div>  
            <div id="myModalContracts" class="modal" role="dialog">
                <div class="modal-dialog">
                    <!-- Modal content - This is the popup window to select an existing contract-->
                    <div class="modal-content" style="width:800px;">
                        <div class="modal-header">
                            <h4 class="modal-title">
                                District Contracts &nbsp;
                            </h4>
                            <div style="float:right;">
                                <asp:Button runat="server" ID="buttonCloseModal"  class="btn btn-outline-primary" OnClick="btnCloseContractModal_Click" Text="OK" UseSubmitBehavior="false" data-dismiss="modal" />
                                <button type="button" class="btn btn-outline-primary" data-dismiss="modal">Cancel</button>
                            </div>
                        </div>
                        <div class="modal-body">
                            <asp:UpdatePanel ID="UpdatePanelModal" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            Search:
                                            <asp:TextBox ID="TextBoxSearchContracts" runat="server" AutoPostBack="true"  ></asp:TextBox>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hfSelectedIndex" runat="server" />
                                    <asp:GridView ID="GridViewContracts" runat="server" DataSourceID="SqlDataSourceContracts" CssClass="mGrid" AutoGenerateColumns="false" EnableViewState="true"
                                        DataKeyNames="DistrictID,ContractID" AllowSorting="true" OnRowDataBound="GridViewContracts_RowDataBound" OnSelectedIndexChanged="GridViewContracts_SelectedIndexChanged"
                                        OnSorting="GridViewContracts_Sorting" >
                                        <Columns>
                                            <asp:BoundField DataField="ContractID" HeaderText="Contract ID" ReadOnly="True" SortExpression="ContractID"  />
                                            <asp:BoundField DataField="ContractDescription" HeaderText="Description" ReadOnly="True" SortExpression="ContractDescription"  />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnCloseContractModal"  class="btn btn-outline-primary" OnClick="btnCloseContractModal_Click" Text="OK" UseSubmitBehavior="true" data-dismiss="modal" />
                            <button type="button" class="btn btn-outline-primary" data-dismiss="modal">Cancel</button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
