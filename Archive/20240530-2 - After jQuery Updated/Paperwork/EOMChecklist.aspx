<%@ Page Title="EOM Checklist" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EOMChecklist.aspx.cs" Inherits="Paperwork.EOMChecklist" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        // For formview resizing
        var h = $(window).height();
        var topAndBottom = 270;
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

        $(function () {
            BindEvents();
        });

        /* Retain element focus after UpdatePanel postback
        ***************************************************/
        jQuery(function ($) {
            var focusedElementId = "";
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_beginRequest(function (source, args) {
                var fe = document.activeElement;
                if (fe != null) {
                    focusedElementId = fe.id;
                } else {
                    focusedElementId = "";
                }
            });

            prm.add_endRequest(function (source, args) {
                if (focusedElementId != "") {
                    $("#" + focusedElementId).focus();
                }
            });
        });



        function BindEvents() {
            $('#GridViewHeightController').css("height", h - topAndBottom);
            $('#GridViewHeightController').css("overflow", "auto");

            //my JQuery code to maintain scroll position
            //When GridViewHeightController div is scrolled - store the scroll bar top pixel value in hidden field
            $("#GridViewHeightController").scroll(function () {
                $("[id*=hdnScrollPos]").val($("#GridViewHeightController").scrollTop());
            });

            //After postback - restore the scroll position
            $('#GridViewHeightController').scrollTop($("[id*=hdnScrollPos]").val());

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
                $("[id*=TextBoxDateOfInventory],[id*=TextBoxSnapshotDate],[id*=TextBoxSchoolInventoryDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    beforeShow: function (input, inst) {
                        inst.dpDiv.removeClass('month_year_datepicker');
                    }
                });
            });

            //Specialized Date Picker for YearMonth field
            $(function () {
                $('[id*=TextBoxFYearmonth]').datepicker(
                    {
                        dateFormat: "yy-mm",
                        changeMonth: true,
                        changeYear: true,
                        showButtonPanel: true,
                        minDate: new Date($('[id*=HFStartDate]').val()),
                        maxDate: new Date($('[id*=HFEndDate]').val()),
                        onClose: function (dateText, inst) {

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

                            inst.dpDiv.addClass('month_year_datepicker');

                            if ((datestr = $(this).val()).length > 0) {
                                year = datestr.substring(0, 4);
                                //year = datestr.substring(datestr.length - 4, datestr.length);
                                month = datestr.substring(datestr.length - 2, datestr.length);
                                //month = datestr.substring(0, 2);
                                $(this).datepicker('option', 'defaultDate', new Date(year, month - 1, 1));
                                $(this).datepicker('setDate', new Date(year, month - 1, 1));
                                //$(".ui-datepicker-calendar").css("display", "none");
                                //$('.date-picker').css("display", "none");
                            }
                        }
                    }
                );

            });

            //Enable Enter Key tabbing
            $('input,select,textarea').on("keypress", function (e) {
                /* ENTER PRESSED*/
                if (e.keyCode == 13) {
                    /* FOCUS ELEMENT */
                    var inputs = $(this).parents("form").eq(0).find(":input,select,textarea:visible");
                    var idx = inputs.index(this);

                    if (idx == inputs.length - 1) {
                        inputs[0].select()
                    }
                    else {
                        inputs[idx + 1].focus(); //  handles submit buttons
                        inputs[idx + 1].select();
                    }
                    return false;
                }
            });

            $("[id*=TextBoxCases]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }

                RecalcCasesTotals();
            });

            $("[id*=TextBoxValue]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }

                RecalcValueTotals();
            });

            //invalid character handling for numeric fields
            $("[id*=TextBoxNOIAmount],[id*=TextBoxPurchAmount],[id*=TextBoxPOSBalance],[id*=TextBoxFoodCostActual],[id*=TextBoxPaperCleanActual]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
                else {
                    $(this).val(parseFloat($(this).val()).toFixed(2).toString());
                }

            });

            //force recalc anytime page posts back
            RecalcCasesTotals();
            RecalcValueTotals();
            RecalcCasesYTDTotals();
            RecalcValueYTDTotals();
        };

        function RecalcCasesTotals() {

            varNA = parseFloat($("[id$=TextBoxCasesNA]").val());
            varK12 = parseFloat($("[id$=TextBoxCasesK12]").val());
            //varDOD = parseFloat($("[id$=TextBoxCasesDOD]").val());
            varBB = parseFloat($("[id$=TextBoxCasesBB]").val());
            varPL = parseFloat($("[id$=TextBoxCasesPL]").val())

            var CasesTotal = varNA + varK12 + varBB + varPL;

            if (CasesTotal == 0)
                $("[id$=TextBoxTotalCases]").val('0.00');
            else
                $("[id$=TextBoxTotalCases]").val(CasesTotal.toFixed(2).toString());

            var USDANA = parseFloat($("[id$=TextBoxUSDACasesNA]").val());
            var USDAK12 = parseFloat($("[id$=TextBoxUSDACasesK12]").val());
            //var USDADOD = parseFloat($("[id$=TextBoxUSDACasesDOD]").val());
            var USDABB = parseFloat($("[id$=TextBoxUSDACasesBB]").val());
            var USDAPL = parseFloat($("[id$=TextBoxUSDACasesPL]").val());

            var USDATotal = USDANA + USDAK12 + USDABB + USDAPL;

            if (USDATotal == 0)
                $("[id$=TextBoxTotalUSDACases]").val('0.00');
            else
                $("[id$=TextBoxTotalUSDACases]").val(USDATotal.toFixed(2).toString());

            $("[id$=TextBoxDiffNA]").val((varNA - USDANA).toFixed(2).toString());
            $("[id$=TextBoxDiffK12]").val((varK12 - USDAK12).toFixed(2).toString());
            //$("[id$=TextBoxDiffDOD]").val((varDOD - USDADOD).toFixed(2).toString());
            $("[id$=TextBoxDiffBB]").val((varBB - USDABB).toFixed(2).toString());
            $("[id$=TextBoxDiffPL]").val((varPL - USDAPL).toFixed(2).toString());

            var difference = CasesTotal - USDATotal;

            $("[id$=TextBoxDifference]").val(difference.toFixed(2).toString());

            
        };

        function RecalcValueTotals() {
            varNA = parseFloat($("[id$=TextBoxValueNA]").val());
            varK12 = parseFloat($("[id$=TextBoxValueK12]").val());
            varDOD = parseFloat($("[id$=TextBoxValueDOD]").val());
            varBB = parseFloat($("[id$=TextBoxValueBB]").val());
            varPL = parseFloat($("[id$=TextBoxValuePL]").val())

            var ValueTotal = varNA + varK12 + varDOD + varBB + varPL;

            if (ValueTotal == 0)
                $("[id$=TextBoxTotalValue]").val('0.00');
            else
                $("[id$=TextBoxTotalValue]").val(ValueTotal.toFixed(2).toString());

            var USDANA = parseFloat($("[id$=TextBoxUSDAValueNA]").val());
            var USDAK12 = parseFloat($("[id$=TextBoxUSDAValueK12]").val());
            var USDADOD = parseFloat($("[id$=TextBoxUSDAValueDOD]").val());
            var USDABB = parseFloat($("[id$=TextBoxUSDAValueBB]").val());
            var USDAPL = parseFloat($("[id$=TextBoxUSDAValuePL]").val());

            var USDATotalValue = USDANA + USDAK12 + USDADOD + USDABB + USDAPL;

            if (USDATotalValue == 0)
                $("[id$=TextBoxTotalUSDAValue]").val('0.00');
            else
                $("[id$=TextBoxTotalUSDAValue]").val(USDATotalValue.toFixed(2).toString());

            $("[id$=TextBoxDiffValueNA]").val(( varNA - USDANA).toFixed(2).toString());
            $("[id$=TextBoxDiffValueK12]").val(( varK12 - USDAK12).toFixed(2).toString());
            $("[id$=TextBoxDiffValueDOD]").val(( varDOD - USDADOD).toFixed(2).toString());
            $("[id$=TextBoxDiffValueBB]").val(( varBB - USDABB).toFixed(2).toString());
            $("[id$=TextBoxDiffValuePL]").val(( varPL - USDAPL).toFixed(2).toString());

            var difference = ValueTotal - USDATotalValue;

            $("[id$=TextBoxValueDifference]").val(difference.toFixed(2).toString());

        }

        function RecalcCasesYTDTotals() {

            varNA = parseFloat($("[id$=TextBoxCasesNAYTD]").val());
            varK12 = parseFloat($("[id$=TextBoxCasesK12YTD]").val());
            //varDOD = parseFloat($("[id$=TextBoxCasesDODYTD]").val());
            varBB = parseFloat($("[id$=TextBoxCasesBBYTD]").val());
            varPL = parseFloat($("[id$=TextBoxCasesPLYTD]").val())

            var CasesTotal = varNA + varK12 + varBB + varPL;

            if (CasesTotal == 0)
                $("[id$=TextBoxTotalCasesYTD]").val('0.00');
            else
                $("[id$=TextBoxTotalCasesYTD]").val(CasesTotal.toFixed(2).toString());

            var USDANA = parseFloat($("[id$=TextBoxUSDACasesNAYTD]").val());
            var USDAK12 = parseFloat($("[id$=TextBoxUSDACasesK12YTD]").val());
            //var USDADOD = parseFloat($("[id$=TextBoxUSDACasesDODYTD]").val());
            var USDABB = parseFloat($("[id$=TextBoxUSDACasesBBYTD]").val());
            var USDAPL = parseFloat($("[id$=TextBoxUSDACasesPLYTD]").val());

            var USDATotal = USDANA + USDAK12 + USDABB + USDAPL;

            if (USDATotal == 0)
                $("[id$=TextBoxTotalUSDACasesYTD]").val('0.00');
            else
                $("[id$=TextBoxTotalUSDACasesYTD]").val(USDATotal.toFixed(2).toString());

            $("[id$=TextBoxDiffNAYTD]").val((varNA - USDANA).toFixed(2).toString());
            $("[id$=TextBoxDiffK12YTD]").val((varK12 - USDAK12).toFixed(2).toString());
            //$("[id$=TextBoxDiffDODYTD]").val((varDOD - USDADOD).toFixed(2).toString());
            $("[id$=TextBoxDiffBBYTD]").val((varBB - USDABB).toFixed(2).toString());
            $("[id$=TextBoxDiffPLYTD]").val((varPL - USDAPL).toFixed(2).toString());

            var difference = CasesTotal - USDATotal;

            $("[id$=TextBoxDifferenceYTD]").val(difference.toFixed(2).toString());


        };

        function RecalcValueYTDTotals() {
            varNA = parseFloat($("[id$=TextBoxValueNAYTD]").val());
            varK12 = parseFloat($("[id$=TextBoxValueK12YTD]").val());
            varDOD = parseFloat($("[id$=TextBoxValueDODYTD]").val());
            varBB = parseFloat($("[id$=TextBoxValueBBYTD]").val());
            varPL = parseFloat($("[id$=TextBoxValuePLYTD]").val())

            var ValueTotal = varNA + varK12 + varDOD + varBB + varPL;

            if (ValueTotal == 0)
                $("[id$=TextBoxTotalValueYTD]").val('0.00');
            else
                $("[id$=TextBoxTotalValueYTD]").val(ValueTotal.toFixed(2).toString());

            var USDANA = parseFloat($("[id$=TextBoxUSDAValueNAYTD]").val());
            var USDAK12 = parseFloat($("[id$=TextBoxUSDAValueK12YTD]").val());
            var USDADOD = parseFloat($("[id$=TextBoxUSDAValueDODYTD]").val());
            var USDABB = parseFloat($("[id$=TextBoxUSDAValueBBYTD]").val());
            var USDAPL = parseFloat($("[id$=TextBoxUSDAValuePLYTD]").val());

            var USDATotalValue = USDANA + USDAK12 + USDADOD + USDABB + USDAPL;

            if (USDATotalValue == 0)
                $("[id$=TextBoxTotalUSDAValueYTD]").val('0.00');
            else
                $("[id$=TextBoxTotalUSDAValueYTD]").val(USDATotalValue.toFixed(2).toString());

            $("[id$=TextBoxDiffValueNAYTD]").val((varNA - USDANA).toFixed(2).toString());
            $("[id$=TextBoxDiffValueK12YTD]").val((varK12 - USDAK12).toFixed(2).toString());
            $("[id$=TextBoxDiffValueDODYTD]").val((varDOD - USDADOD).toFixed(2).toString());
            $("[id$=TextBoxDiffValueBBYTD]").val((varBB - USDABB).toFixed(2).toString());
            $("[id$=TextBoxDiffValuePLYTD]").val((varPL - USDAPL).toFixed(2).toString());

            var difference = ValueTotal - USDATotalValue;

            $("[id$=TextBoxValueDifferenceYTD]").val(difference.toFixed(2).toString());

        }


    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="topSection" style="height:101px;" class="row">
                <div id="hdrDiv" class="col-3">
                    <h2><%: Title %></h2>
                    <asp:LinkButton ID="ISave" runat="server" 
                        ToolTip="Save Changes" Text=" Save" ForeColor="Green"
                        CssClass="fas fa-save mGridLinkButton" Width="80" ></asp:LinkButton>* Autosave is on
                    <div class="messagealert" id="alert_container" style="float:right;width:130px;text-align:center;" ></div>
                </div>
                
                <div id="menuDiv" class="col-6" >
                    <div class="row">
                        <div class="col">
                            <strong>Program Year:</strong>
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                            <asp:HiddenField ID="HFStartDate" runat="server" />
                            <asp:HiddenField ID="HFEndDate" runat="server" />
                            <asp:HiddenField ID="HFBudgetYear" runat="server"  />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <strong>District:</strong>
                            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="False" AutoPostBack="true" 
                                DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> 
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <strong>Year Month:</strong>
                            <asp:TextBox ID="TextBoxFYearmonth" runat="server" Width="80" OnTextChanged="TextBoxFYearmonth_TextChanged" AutoPostBack="true" ></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div id="printDiv" class="col-3" style="text-align:right;" >
                    <div class="row">
                        <div class="col">
                            <asp:LinkButton ID="IPrint" runat="server" 
                            ToolTip="Print EOM Checklist" Text=" Print"
                            CssClass="fas fa-print mGridLinkButton" Width="80" OnClick="IPrint_Click" ></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <asp:HiddenField id="hdnScrollPos" runat="server"/>
            <div id="DataEntry">
                <div id="GridViewHeightController" style="overflow-y:auto;background-color:white;" class="row">
                    <div class="col-sm-12">
                        
                        <asp:FormView ID="FormViewEOMChecklist" runat="server" CssClass="mGrid" DefaultMode="Edit" DataSourceID="SqlDataSourceEOM"
                            DataKeyNames="ProgramYear, DistrictID, YearMonth" OnPreRender="FormViewEOMChecklist_PreRender" OnDataBound="FormViewEOMChecklist_DataBound">
                            <EditItemTemplate>
                                <div class="container">
                                    <asp:HiddenField ID="HFBatchEmailed" runat="server" Value='<%# Eval("EOMReportBatchEmailed") %>' />
                                    <asp:HiddenField ID="HFBatchEmailedDateTime" runat="server" Value='<%# Eval("EOMReportBatchDateTime") %>' />
                                    <asp:HiddenField ID="HFSourceRMEmail" runat="server" Value='<%# Eval("RMEmail") %>' />
                                    <asp:HiddenField ID="HFSourceRMName" runat="server" Value='<%# Eval("RMName") %>' />
                                    <div class="row" style="border-bottom-color:#9DCBFF;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                        <div class="col-sm-2" style="background-color:#9DCBFF;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                            Inventory
                                        </div>
                                    </div>
                                    <div class="row" hidden="hidden">
                                        <div class="col-sm-12">
                                            <asp:TextBox ID="TextBoxDateOfInventory" runat="server" Text='<%# Bind("InventoryDate","{0:d}" )  %>' Width="100" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            Date of physical inventory
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                             <asp:GridView ID="GridViewSchoolRecords" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceSchoolRecords" 
                                                DataKeyNames="ProgramYear, DistrictID, YearMonth, SchoolNum" CssClass="mGrid" Width="97%" OnRowDataBound="GridViewSchoolRecords_RowDataBound"
                                                OnPreRender="GridViewSchoolRecords_PreRender" ShowFooter="true">
                                                <Columns >
                                                    <asp:BoundField DataField="SchoolName" HeaderText="Name" ReadOnly="true" ItemStyle-Width="150" />
                                                    <asp:TemplateField HeaderText="Tracs purchased inventory - Snapshot Date" >
                                                        <ItemTemplate>
                                                            <div style="text-align:center;">
                                                                <asp:TextBox ID="TextBoxSnapshotDate" runat="server" Text='<%# Bind("SnapshotDate", "{0:d}") %>' Width="100" OnTextChanged="ChecklistSchoolItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>

                                                                <asp:HiddenField ID="HFRowSaved" runat="server" Value="0" />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Snapshot Name" >
                                                        <ItemTemplate>
                                                            <div style="text-align:center;">
                                                                <asp:TextBox ID="TextBoxSnapshotName" runat="server" Text='<%# Bind("SnapshotName") %>' Width="150" OnTextChanged="ChecklistSchoolItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                            </div>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            <div style="text-align:right;font-weight:bold;">
                                                                <asp:Label ID="LabelTotal" runat="server" Text="Total:" Width="150" ></asp:Label>
                                                            </div>
                                                        </FooterTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Snapshot $ Total" >
                                                        <ItemTemplate>
                                                            <div style="text-align:center;">
                                                                <asp:TextBox ID="TextBoxSnapshotTotal" runat="server" Text='<%# Bind("SnapshotTotal") %>' Width="100" OnTextChanged="ChecklistSchoolItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                            </div>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            <div style="text-align:center;">
                                                                <asp:TextBox ID="TextBoxSnapshotTotalFooter" runat="server" Text='<%# Bind("SnapshotTotal") %>' Width="100"  AutoCompleteType="Disabled" Enabled="false" ></asp:TextBox>
                                                            </div>
                                                        </FooterTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Non-Reinhart item pricing is updated (produce/milk/etc)">
                                                        <ItemTemplate>
                                                            <div style="text-align:center;">
                                                                <asp:CheckBox ID="CBUpdatedPricing" runat="server" Checked='<%# Bind("CBUpdatedPricing") %>' OnCheckedChanged="ChecklistSchoolItemChanged" AutoPostBack="true" />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Date of Physical Inventory">
                                                        <ItemTemplate>
                                                            <div style="text-align:center;">
                                                                <asp:TextBox ID="TextBoxSchoolInventoryDate" runat="server" Text='<%# Bind("DateOfPhysicalInventory", "{0:d}") %>' Width="100" OnTextChanged="ChecklistSchoolItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                             Enter USDA received case totals from K12 / Processor Link / Brown Box, DOD Dollar Amount.
                                         </div>
                                    </div>
                                    <div class="row" style="text-align:center;">
                                        <div class="col-sm-12" style="margin-left:15px;">
                                            <div class="row">
                                                <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;" >
                                                    <span style="font-weight:bold;vertical-align:middle;">PERIOD</span>
                                                </div>
                                                <div class="col-sm-1" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    # Cases
                                                </div>
                                                <div class="col-sm-1" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    $ Value
                                                </div>
                                                <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    USDA
                                                </div>
                                                <div class="col-sm-1"  style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    # Cases
                                                </div>
                                                <div class="col-sm-1"  style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    $ Value
                                                </div>
                                                <div class="col-sm-1"  style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    Diff. Cases
                                                </div>
                                                <div class="col-sm-1"  style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    Diff. Value
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12" style="margin-left:15px;" >
                                           <div class="row" >
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        NA
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxCasesNA" runat="server" Text='<%# Bind("CasesNA", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxValueNA" runat="server" Text='<%# Bind("ValueNA", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        NA
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDACasesNA" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDAValueNA" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffNA" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffValueNA" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row" >
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        K12
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxCasesK12" runat="server" Text='<%# Bind("CasesK12", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxValueK12" runat="server" Text='<%# Bind("ValueK12", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        K12
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDACasesK12" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDAValueK12" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffK12" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffValueK12" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row">
                                                    <div class="col-sm-2" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        DOD (insert $ value)
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxCasesDOD" runat="server" Text='<%# Bind("CasesDOD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true" Visible="false"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxValueDOD" runat="server" Text='<%# Bind("ValueDOD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        DOD ($ value)
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDACasesDOD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70" Visible="false"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDAValueDOD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffDOD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70" Visible="false"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffValueDOD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row">
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Processorlink
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxCasesPL" runat="server" Text='<%# Bind("CasesPL", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxValuePL" runat="server" Text='<%# Bind("ValuePL", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Processor Link
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDACasesPL" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDAValuePL" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffPL" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffValuePL" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row">
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Brown Box/<br />Agency Summary
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <br />
                                                        <asp:TextBox ID="TextBoxCasesBB" runat="server" Text='<%# Bind("CasesBB", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <br />
                                                        <asp:TextBox ID="TextBoxValueBB" runat="server" Text='<%# Bind("ValueBB", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Brown Box/<br />Agency Summary
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <br />
                                                        <asp:TextBox ID="TextBoxUSDACasesBB" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <br />
                                                        <asp:TextBox ID="TextBoxUSDAValueBB" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <br />
                                                        <asp:TextBox ID="TextBoxDiffBB" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <br />
                                                        <asp:TextBox ID="TextBoxDiffValueBB" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row">
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Total
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxTotalCases" runat="server" Text="0.00" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxTotalValue" runat="server" Text="0.00" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2" style="background-color:slategrey" >
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxTotalUSDACases" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxTotalUSDAValue" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDifference" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxValueDifference" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>

                                        </div>
                                    </div>

                                    <br />
                                    <!-- USDA YTD AMOUNTS -->
                                    <div class="row" style="text-align:center;">
                                        <div class="col-sm-12" style="margin-left:15px;">
                                            <div class="row">
                                                <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;" >
                                                    <span style="font-weight:bold;vertical-align:middle;">YTD</span>
                                                </div>
                                                <div class="col-sm-1" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    # Cases
                                                </div>
                                                <div class="col-sm-1" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    $ Value
                                                </div>
                                                <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    USDA
                                                </div>
                                                <div class="col-sm-1"  style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    # Cases
                                                </div>
                                                <div class="col-sm-1"  style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    $ Value
                                                </div>
                                                <div class="col-sm-1"  style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    Diff. Cases
                                                </div>
                                                <div class="col-sm-1"  style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                                    Diff. Value
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12" style="margin-left:15px;" >
                                           <div class="row" >
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        NA
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxCasesNAYTD" runat="server" Text='<%# Bind("CasesNAYTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxValueNAYTD" runat="server" Text='<%# Bind("ValueNAYTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        NA
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDACasesNAYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDAValueNAYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffNAYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffValueNAYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row" >
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        K12
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxCasesK12YTD" runat="server" Text='<%# Bind("CasesK12YTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxValueK12YTD" runat="server" Text='<%# Bind("ValueK12YTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        K12
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDACasesK12YTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDAValueK12YTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffK12YTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffValueK12YTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row">
                                                    <div class="col-sm-2" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        DOD (insert $ value)
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxCasesDODYTD" runat="server" Text='<%# Bind("CasesDODYTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true" Enabled="false" Visible="false"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxValueDODYTD" runat="server" Text='<%# Bind("ValueDODYTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        DOD ($ value)
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDACasesDODYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70" Visible="false"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDAValueDODYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffDODYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70" Visible="false"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffValueDODYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row">
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Processorlink
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxCasesPLYTD" runat="server" Text='<%# Bind("CasesPLYTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true" ></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxValuePLYTD" runat="server" Text='<%# Bind("ValuePLYTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Processor Link
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDACasesPLYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxUSDAValuePLYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffPLYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDiffValuePLYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row">
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Brown Box/<br />Agency Summary
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <br />
                                                        <asp:TextBox ID="TextBoxCasesBBYTD" runat="server" Text='<%# Bind("CasesBBYTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <br />
                                                        <asp:TextBox ID="TextBoxValueBBYTD" runat="server" Text='<%# Bind("ValueBBYTD", "{0:F2}") %>' OnTextChanged="ChecklistItemChanged" Width="70" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Brown Box/<br />Agency Summary
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <br />
                                                        <asp:TextBox ID="TextBoxUSDACasesBBYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <br />
                                                        <asp:TextBox ID="TextBoxUSDAValueBBYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <br />
                                                        <asp:TextBox ID="TextBoxDiffBBYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <br />
                                                        <asp:TextBox ID="TextBoxDiffValueBBYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>
                                            <div class="row">
                                                    <div class="col-sm-2"  style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        Total
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxTotalCasesYTD" runat="server" Text="0.00" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxTotalValueYTD" runat="server" Text="0.00" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-2" style="background-color:slategrey" >
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxTotalUSDACasesYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;">
                                                        <asp:TextBox ID="TextBoxTotalUSDAValueYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxDifferenceYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-1" style="border-width:1px;border-style:solid;border-color:slategrey;" >
                                                        <asp:TextBox ID="TextBoxValueDifferenceYTD" runat="server" Text="0.00" ReadOnly="true" Enabled="false" Width="70"></asp:TextBox>
                                                    </div>
                                            </div>

                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-6">
                                            Comment on differences between USDA actual and inventory amounts.
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxUSDAComment" runat="server" Text='<%# Bind("USDADiffComment" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:TextBox ID="TextBoxNOIAmount" runat="server" Text='<%# Bind("NOIAmount","{0:F2}" ) %>' Width="100" OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                            NOI purchased total is entered in Tracs inventory from USDA cover sheet
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBPurchTotalEntered" runat="server" Checked='<%# Bind("CBPurchTotalEntered") %>' Text="Purchased inventory totals entered in Paperwork" OnCheckedChanged="ChecklistItemChanged"  AutoPostBack="true"/>
                                            <br />
                                            <asp:CheckBox ID="CBUSDAComplete" runat="server" Checked='<%# Bind("CBUSDAComplete") %>' Text="USDA inventory completed" OnCheckedChanged="ChecklistItemChanged"  AutoPostBack="true"/>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxPurchAmount" runat="server" Text='<%# Bind("PurchTotal","{0:F2}" ) %>' Width="100" OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true" Enabled="false"></asp:TextBox>
                                             Enter total purchased inventory amount.
                                         </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxTRACSBldg" runat="server" Text='<%# Bind("TRACSBldgTotals","{0:F2}" ) %>' Width="100" OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                             Enter TRACS Building Totals.
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBExtInventoryPrinted" runat="server" Checked='<%# Bind("CBExtInventoryPrinted") %>' Text="Extended inventory printed or saved for each school" OnCheckedChanged="ChecklistItemChanged"  AutoPostBack="true"/>
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            Comment on target purchased inventory.
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxTargetPurchasedComment" runat="server" Text='<%# Bind("TargetPurchasedComment" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                         <div class="col-sm-6">
                                             (Actual Meals)/(Number of Days) x (Actual Food, Paper & Cleaning Usage) x (8) <br />
                                             ( <asp:Label ID="LabelMeals" runat="server" Text="0"></asp:Label> ) / ( <asp:Label ID="LabelNumDays" runat="server" Text="0"></asp:Label> ) x ( <asp:Label ID="LabelFoodPaperClean" runat="server" Text="0.000"></asp:Label> ) x ( 8 ) = <asp:Label ID="LabelTotalTargetPurchInv" runat="server" Text="0.00"></asp:Label>
                                         </div>
                                    </div>
                                    <div class="row" style="border-bottom-color:#9DCBFF;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                        <div class="col-sm-2" style="background-color:#9DCBFF;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                            Special Functions
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBSpecFuncLedger" runat="server" Checked='<%# Bind("CBSpecFuncLedger") %>' Text="Special function for the month sales total matches operating report" OnCheckedChanged="ChecklistItemChanged"  AutoPostBack="true"/>
                                         </div>
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CheckBoxSFMatchOR" runat="server" Checked='<%# Bind("CBSFLedgerYTDMatchesOR") %>' Text="Special function YTD sales total matches operating report" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                         </div>
                                    </div>
                                    <div class="row" style="border-bottom-color:#9DCBFF;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                        <div class="col-sm-2" style="background-color:#9DCBFF;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                            Other
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBIndirectCosts" runat="server" Checked='<%# Bind("CBIndirectCosts") %>' Text="Entered indirect cost (if applicable)" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBReviewedTransOCosts" runat="server" Checked='<%# Bind("CBReviewedTransOCosts") %>' Text="Reviewed transmittal other cost report - actual vs budget amounts for each item" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:TextBox ID="TextBoxPOSBalance" runat="server" Text='<%# Bind("POSBalance","{0:F2}" ) %>' Width="100" OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                             Enter POS negative balance total.
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBSOC1" runat="server" Checked='<%# Bind("CBSOC1") %>' Text="SOC 1 - deposit reconciliation (district deposits match citrix)" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBNotifComplete" runat="server" Checked='<%# Bind("CBNotificationsComplete") %>' Text="Completed Paperwork notifications" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-3">
                                            Enter POS Over/Under
                                        </div>
                                        <div class="col-sm-3">
                                            Current Month:
                                            <asp:TextBox ID="TextBoxPOSOverUnderCurrent" runat="server" Text='<%# Bind("POSOverUnderCurrent") %>' Width="100" OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-3">
                                            YTD:
                                            <asp:TextBox ID="TextBoxPOSOverUnderYTD" runat="server" Text='<%# Bind("POSOverUnderYTD") %>' Width="100" OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBCompareTracsStatement" runat="server" Checked='<%# Bind("CBCompareTracsStatement") %>' Text="Compared/Verified Tracs AR Statement for Outstanding RFS-PFG Invoices" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <ul>
                                                <li>Board Report Completed for Month</li>
                                                <li>Safety Checklist Completed for Month</li>
                                                <li>Reimbursement Claim Summary, Received from District</li>
                                            </ul>
                                         </div>
                                    </div>

                                    <div class="row" style="border-bottom-color:#9DCBFF;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                        <div class="col-sm-2" style="background-color:#9DCBFF;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                            Sales
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBOnlinePayments" runat="server" Checked='<%# Bind("CBOnlinePayments") %>' Text="All on-line payments captured for the month" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBReviewBdgtReport" runat="server" Checked='<%# Bind("CBReviewBdgtReport") %>' Text="Reviewed Budget report in paperwork for the month" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:CheckBox ID="CBNegAdjusts" runat="server" Checked='<%# Bind("CBNegAdjusts") %>' Text="Adjustment amounts have a comment on the sales entry" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                        <div class="col-sm-6">
                                            
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            Participation Comments:<br />
                                            <asp:TextBox ID="TextBoxParticipationComments" runat="server" Text='<%# Bind("ParticipationComments") %>' TextMode="MultiLine" Rows="3" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>

                                        </div>
                                        <div class="col-sm-1" style="text-align:right;">
                                            <div style="font-size:smaller;">&nbsp;</div>
                                            Brk:<br />
                                            Lun:<br />
                                            Ala:<br />
                                        </div>
                                        <div class="col-sm-1">
                                            <div style="font-size:smaller;text-align:center;width:90px;">Month</div>
                                            <asp:TextBox ID="TextBoxBrMonth" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxLnMonth" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxAlaMonth" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1">
                                            <div style="font-size:smaller;text-align:center;width:90px;">Month Bud</div>
                                            <asp:TextBox ID="TextBoxBrMonthBud" runat="server" Text="0.00" Width="90" Enabled="false"></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxLnMonthBud" runat="server" Text="0.00" Width="90" Enabled="false"></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxAlaMonthBud" runat="server" Text="0.00" Width="90" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1">
                                            <div style="font-size:smaller;text-align:center;width:90px;">YTD</div>
                                            <asp:TextBox ID="TextBoxBrYtd" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxLnYtd" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxAlaYtd" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1">
                                            <div style="font-size:smaller;text-align:center;width:90px;">YTD Bud</div>
                                            <asp:TextBox ID="TextBoxBrYtdBud" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxLnYtdBud" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxAlaYtdBud" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            Add comment here if the adjustment total is not zero.<br />
                                            <asp:TextBox ID="TextBoxNegAdjustsComment" runat="server" Text='<%# Bind("NegAdjustsComment" ) %>' TextMode="MultiLine" Rows="3" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                            
                                        </div>
                                         <div class="col-sm-6">
                                             <br />
                                             <asp:TextBox ID="TextBoxAdjustments" runat="server" Text="0.00" Enabled="false" Width="100"></asp:TextBox> Adjustment Total<br />
                                             
                                             <div style="color:red;">
                                                 <asp:Label ID="LabelCommentRequired" runat="server" Text="*required" ></asp:Label>
                                             </div>
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBVendDepOSales" runat="server" Checked='<%# Bind("CBVendDepOSales") %>' Text="Vending deposits and other sales entered for the month" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="row" style="border-bottom-color:#9DCBFF;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                        <div class="col-sm-3" style="background-color:#9DCBFF;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                            Edit Checks/CN Reports
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBEditChecksMatch" runat="server" Checked='<%# Bind("CBEditChecksMatch") %>' Text="POS edit checks match Paperwork edit checks" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBOpDaysVerified" runat="server" Checked='<%# Bind("CBOpDaysVerified") %>' Text="Operating days correct on edit checks for each building and meal service" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBSchoolInfoUpdated" runat="server" Checked='<%# Bind("CBSchoolInfoUpdated") %>' Text="School information tab - enrollment, paid, free & reduced updated" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBCompletedClaimRec" runat="server" Checked='<%# Bind("CBCompletedClaimRec") %>' Text="Completed Monthly Claim Reconciliation, By Building" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <ul>
                                                <li>Edit Checks & Citrix Edit Checks are kept on file to verify the reconciliation was complete.</li>
                                                <li>District Edit Checks Signed & dated monthly by the district</li>
                                            </ul>
                                        </div>
                                    </div>


                                    <div class="row" style="border-bottom-color:#9DCBFF;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                        <div class="col-sm-2" style="background-color:#9DCBFF;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                            Payroll
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBDistPayrollHrsEntered" runat="server" Checked='<%# Bind("CBDistPayrollHrsEntered") %>' Text="All District payroll hours entered for the month, including productive & non-productive" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBHealthInsCaptured" runat="server" Checked='<%# Bind("CBHealthInsCaptured") %>' Text="Health / Insurance is captured for the month" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="row" style="border-bottom-color:#9DCBFF;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                        <div class="col-sm-4" style="background-color:#9DCBFF;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                            Operating Report - DO NOT GIVE TO DISTRICT
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            Food Cost.  Enter comments.
                                            <asp:Label ID="LabelFoodComment" runat="server" Text="*required" CssClass="validators" ></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxFoodCostComment" runat="server" Text='<%# Bind("FoodCostComment" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1">
                                            Month:<br />
                                            <asp:TextBox ID="TextBoxFoodCostActualM" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1">
                                            Budget<br />
                                            <asp:TextBox ID="TextBoxFoodCostBDGT" runat="server" Width="90" Enabled="false"></asp:TextBox><br />
                                        </div>
                                        <div class="col-sm-1">
                                            YTD:<br />
                                            <asp:TextBox ID="TextBoxFoodCostActualY" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox>
                                        </div>
                                        
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                             Paper / Cleaning Cost.  Enter comments.
                                            <asp:Label ID="LabelPprClnComment" runat="server" Text="*required" CssClass="validators" ></asp:Label>
                                         </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxPaperCleanComment" runat="server" Text='<%# Bind("PaperCleanComment" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1">
                                            Month:<br />
                                            <asp:TextBox ID="TextBoxPaperCleanActualM" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1">
                                            Budget:<br />
                                            <asp:TextBox ID="TextBoxPprClnBDGT" runat="server" Width="90" Enabled="false"></asp:TextBox><br />
                                        </div>
                                        <div class="col-sm-1">
                                            YTD:<br />
                                            <asp:TextBox ID="TextBoxPaperCleanActualY" runat="server" Text="0.00" Enabled="false" Width="90" ></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:CheckBox ID="CBUSDAInvMatches" runat="server" Checked='<%# Bind("CBUSDAInvMatches") %>' Text="USDA inventory Protein and Grocery usage matches Paperwork" OnCheckedChanged="ChecklistItemChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            Comments on USDA usage vs budget per meal.
                                        </div>
                                        <div class="col-sm-1" style="text-align:center;">
                                            Actual
                                        </div>
                                        <div class="col-sm-1" style="text-align:center;">
                                            Budget
                                        </div>
                                        <div class="col-sm-1" style="text-align:center;">
                                            YTD
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxReviewedUsageComment" runat="server" Text='<%# Bind("ReviewedUsageComment" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxUSDAActual" runat="server" Text="0.000" Enabled="false" Width="90"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxUSDABdgt" runat="server" Text="0.000" Enabled="false" Width="90"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-1" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxUSDAYTD" runat="server" Text="0.000" Enabled="false" Width="90"></asp:TextBox>
                                        </div>
                                    </div>
                                    
                                    <div class="row">
                                        <div class="col-sm-6">
                                            Comment on TNG hours/wages and District hours/wages against budget
                                        </div>
                                        <div class="col-sm-2"></div>
                                        <div class="col-sm-1" style="text-align:center;">Actual</div>
                                        <div class="col-sm-1" style="text-align:center;">Budget</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxHoursWagesComment" runat="server" Text='<%# Bind("HoursWagesComment" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2">
                                            TNG Hours<br />
                                            TNG Wages<br />
                                            District Hours<br />
                                            District Wages
                                        </div>
                                        <div class="col-sm-1" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxTNGHoursActual" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxTNGWagesActual" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxDistHoursActual" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxDistWagesActual" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox>
                                            <asp:HiddenField ID="HFSFA_PR" runat="server" Value="0" />
                                        </div>
                                        <div class="col-sm-1" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxTNGHoursBdgt" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxTNGWagesBdgt" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxDistHoursBdgt" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox><br />
                                            <asp:TextBox ID="TextBoxDistWagesBdgt" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row" style="border-bottom-color:#9DCBFF;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                        <div class="col-sm-4" style="background-color:#9DCBFF;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                            Profit Statement - DO NOT GIVE TO DISTRICT
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            Comment on monthly bottom line vs budget
                                        </div>
                                        <div class="col-sm-6" >
                                            <asp:Label ID="Label1" runat="server" width="150"></asp:Label>
                                            <asp:Label ID="LabelCurrent" runat="server" Width="90" Text="Current"></asp:Label>
                                            <asp:Label ID="LabelYtd" runat="server" Width="90"     Text="YTD"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxBottomLineComment" runat="server" Text='<%# Bind("BottomLineComment" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                            <br />
                                            IF CPM:  P&L Figures will be generated at completion of TNG Generated invoice.
                                        </div>
                                         <div class="col-sm-6">
                                            <asp:Label ID="Label2" runat="server" width="150" Text="Net Income/(Loss)"></asp:Label>
                                            <asp:TextBox ID="TextBoxPLCurrent" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox>
                                            <asp:TextBox ID="TextBoxPLYtd" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox>
                                             <br />
                                             <asp:Label ID="Label5" runat="server" width="100" Text=""></asp:Label>
                                             <asp:Label ID="Label6" runat="server" width="90" Text="Budget P/L"></asp:Label> / 
                                             <asp:Label ID="Label7" runat="server" width="90" Text="Budget Days"></asp:Label> x 
                                             <asp:Label ID="Label8" runat="server" width="90" Text=" Actual Days"></asp:Label> 
                                             <br />
                                             <asp:Label ID="Label3" runat="server" width="100" Text="Budget Period"></asp:Label>
                                             <asp:TextBox ID="TextBoxProfitBudget1" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox> / 
                                             <asp:TextBox ID="TextBoxServiceDays1" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox> x
                                             <asp:TextBox ID="TextBoxDaysMonth" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox> = 
                                             <asp:TextBox ID="TextBoxBudgetPeriod" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox>
                                             <br />
                                             <asp:Label ID="Label4" runat="server" width="100" Text="Budget YTD"></asp:Label>
                                             <asp:TextBox ID="TextBoxProfitBudget2" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox> / 
                                             <asp:TextBox ID="TextBoxServiceDays2" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox> x
                                             <asp:TextBox ID="TextBoxDaysYtd" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox> = 
                                             <asp:TextBox ID="TextBoxBudgetYtd" runat="server" Text="0.00" Enabled="false" Width="90"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row" style="border-bottom-color:#9DCBFF;border-bottom-style:solid;border-bottom-width:5px;padding-top:10px;">
                                        <div class="col-sm-2" style="background-color:#9DCBFF;border-top-right-radius:5px;border-top-left-radius:5px;font-weight:bold;">
                                            EOM Packet
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <strong>The below items must be provided to the School District. </strong>
                                            <ul>
                                                <li>District Recap</li>
                                                <li>Edit Checks for each building and district summary</li>
                                                <li>USDA Inventory Spreadsheet & Cover Sheet </li>
                                                <li>Lunch Claim Calculation Worksheet </li>
                                                <li>Special Function Ledger </li>
                                                <li>Snack Report </li>
                                            </ul>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:HiddenField ID="HFChecklistSaved" runat="server" Value="0" />
                                            &nbsp;
                                        </div>
                                    </div>
                                    
                                </div>
                            </EditItemTemplate>
                            
                        </asp:FormView>

                        

                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        <asp:LinkButton ID="iSave2" runat="server" 
                            ToolTip="Save Changes" Text=" Save" ForeColor="Green"
                            CssClass="fas fa-save mGridLinkButton" Width="80" ></asp:LinkButton>
                        <div class="messagealert" id="alert_container2" style="float:right;width:130px;text-align:center;" ></div>
                        <br />
                        <asp:Label ID="LabelRM" runat="server"></asp:Label>
                        <asp:Label ID="LabelReportsSent" runat="server" Visible="false" ForeColor="Crimson"></asp:Label>
                        <br />
                        Send Reports:
                        <asp:CheckBox ID="CBOperatingReport" runat="server" Text="Operating Report" Checked="true" />&nbsp;
                        <asp:CheckBox ID="CBEOMChecklist" runat="server" Text="EOM Checklist" Checked="true" />&nbsp;

                        <asp:Button ID="btnSend" runat="server" Text="Send" OnClick="btnSend_Click" />
                        <asp:HiddenField ID="HFSSRSBaseFolder" runat="server" />
                        <asp:HiddenField ID="HFRMEmail" runat="server" />
                        <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
                            CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
                            ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="false" ShowDocumentMapButton="False" 
                            ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
                            AsyncRendering="False" InteractivityPostBackMode="AlwaysSynchronous">
                            <ServerReport ReportPath="EOM Packet-Portrait"  />
                        </rsweb:ReportViewer>
                    </div>
                </div>
                
            </div>

            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceEOM" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                     SelectCommand="SELECT DYI.ProgramYear, DYI.[DistrictID], ISNULL([YearMonth],@YearMonth) YearMonth, ISNULL([InventoryDate],'') InventoryDate
                    ,ISNULL([CasesBB],0) CasesBB,ISNULL([CasesDOD],0) CasesDOD,ISNULL([CasesK12],0) CasesK12,ISNULL([CasesPL],0) CasesPL, ISNULL([CasesNA],0) CasesNA, ISNULL([NOIAmount],0) NOIAmount,ISNULL([CBPurchTotalEntered], CAST(0 AS BIT)) CBPurchTotalEntered
                    ,ISNULL([CBUSDAComplete],CAST(0 AS BIT)) CBUSDAComplete,ISNULL([PurchTotal],CAST(0 AS BIT)) PurchTotal,ISNULL([CBSpecFuncLedger],CAST(0 AS BIT)) CBSpecFuncLedger
                    ,ISNULL([CBIndirectCosts],CAST(0 AS BIT)) CBIndirectCosts,ISNULL([CBReviewedTransOCosts],CAST(0 AS BIT)) CBReviewedTransOCosts,ISNULL([POSBalance],0) POSBalance,ISNULL([CBSOC1],0) CBSOC1,ISNULL([CBOnlinePayments],CAST(0 AS BIT)) CBOnlinePayments
                    ,ISNULL([CBReviewBdgtReport],CAST(0 AS BIT)) CBReviewBdgtReport,ISNULL([CBNegAdjusts],CAST(0 AS BIT)) CBNegAdjusts,ISNULL([NegAdjustsComment],'') NegAdjustsComment,ISNULL([CBVendDepOSales],CAST(0 AS BIT)) CBVendDepOSales,ISNULL([CBEditChecksMatch],CAST(0 AS BIT)) CBEditChecksMatch
                    ,ISNULL([CBOpDaysVerified],CAST(0 AS BIT)) CBOpDaysVerified,ISNULL([CBSchoolInfoUpdated],CAST(0 AS BIT)) CBSchoolInfoUpdated,ISNULL([CBDistPayrollHrsEntered],CAST(0 AS BIT)) CBDistPayrollHrsEntered,ISNULL([CBHealthInsCaptured],CAST(0 AS BIT)) CBHealthInsCaptured
                    ,ISNULL([FoodCostComment],'') FoodCostComment,ISNULL([PaperCleanComment],'') PaperCleanComment,ISNULL([CBUSDAInvMatches],CAST(0 AS BIT)) CBUSDAInvMatches
                    ,ISNULL([ReviewedUsageComment],'') ReviewedUsageComment,ISNULL([TargetPurchasedComment],'') TargetPurchasedComment
                    ,ISNULL([EOMReportBatchEmailed],0) EOMReportBatchEmailed,ISNULL([EOMReportBatchDateTime],'') EOMReportBatchDateTime,ISNULL([LastEditedUser],'') LastEditedUser,ISNULL([LastEditedDate],'') LastEditedDate
                    ,ISNULL([CBExtInventoryPrinted], CAST(0 AS BIT)) CBExtInventoryPrinted, ISNULL(HoursWagesComment,'') HoursWagesComment, ISNULL(BottomLineComment,'') BottomLineComment
                    ,ISNULL([RMEmail],'') RMEmail, ISNULL(RM.[RMName],'') RMName, ISNULL([CBSFLedgerYTDMatchesOR],0) CBSFLedgerYTDMatchesOR
                    ,ISNULL([ValueBB],0) ValueBB,ISNULL([ValueDOD],0) ValueDOD,ISNULL([ValueK12],0) ValueK12,ISNULL([ValuePL],0) ValuePL, ISNULL([ValueNA],0) ValueNA, ISNULL(TRACSBldgTotals,0) TRACSBldgTotals
                    ,ISNULL([CBNotificationsComplete],0) CBNotificationsComplete,ISNULL([CasesBBYTD],0) CasesBBYTD,ISNULL([CasesDODYTD],0) CasesDODYTD,ISNULL([CasesK12YTD],0) CasesK12YTD,ISNULL([CasesPLYTD],0) CasesPLYTD, ISNULL([CasesNAYTD],0) CasesNAYTD
                    ,ISNULL([ValueBBYTD],0) ValueBBYTD,ISNULL([ValueDODYTD],0) ValueDODYTD,ISNULL([ValueK12YTD],0) ValueK12YTD,ISNULL([ValuePLYTD],0) ValuePLYTD, ISNULL([ValueNAYTD],0) ValueNAYTD,ISNULL(USDADiffComment,'') USDADiffComment
                    ,ISNULL([POSOverUnderCurrent],0) POSOverUnderCurrent, ISNULL([POSOverUnderYTD],0) POSOverUnderYTD, ISNULL(CBCompareTracsStatement, CAST(0 AS BIT)) CBCompareTracsStatement, ISNULL(CBCompletedClaimRec, CAST(0 AS BIT)) CBCompletedClaimRec
                    ,CBOverridePayRelDist, OverridePayRelPctDist, ISNULL([ParticipationComments],'') ParticipationComments
                    FROM  [DistrictYearInfo] DYI
                    LEFT OUTER JOIN [EOMChecklist] EOM ON DYI.DistrictID=EOM.DistrictID AND DYI.ProgramYear=EOM.ProgramYear AND @YearMonth=EOM.YearMonth
                    LEFT OUTER JOIN [RMSetup] RM ON DYI.RMNumber=RM.RMNumber
                    WHERE DYI.ProgramYear=@ProgramYear AND DYI.DistrictID=@DistrictID AND ISNULL(YearMonth,@YearMonth)=@YearMonth">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="TextBoxFYearmonth" Name="YearMonth" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceUSDAInventory" runat="server" ConnectionString="<%$ ConnectionStrings:connUSDA %>" DataSourceMode="DataReader" 
                    SelectCommand="SELECT [InventoryYear],[DistrictID],[ItemClass],SUM([TotalCasesRcvd]) TotalCasesReceived, SUM([TotalCasesRcvdValue]) TotalCasesReceivedValue
                          FROM [USDA].[dbo].[vw_PaperworkReceivedSourceByClass]
                          WHERE InventoryYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth
                          GROUP BY InventoryYear,[DistrictID],[ItemClass]">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="TextBoxFYearmonth" Name="YearMonth" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceUSDAInvYTD" runat="server" ConnectionString="<%$ ConnectionStrings:connUSDA %>" DataSourceMode="DataReader" 
                    SelectCommand="SELECT [InventoryYear],[DistrictID],[ItemClass],SUM([TotalCasesRcvd]) TotalCasesReceived, SUM([TotalCasesRcvdValue]) TotalCasesReceivedValue
                          FROM [USDA].[dbo].[vw_PaperworkReceivedSourceByClass]
                          WHERE InventoryYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth<=@YearMonth
                          GROUP BY InventoryYear,[DistrictID],[ItemClass]">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="TextBoxFYearmonth" Name="YearMonth" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchoolRecords" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                    SelectCommand="SELECT @ProgramYear ProgramYear, DS.DistrictID, @YearMonth YearMonth, ISNULL([SnapshotDate], '1/1/1900')[SnapshotDate], ISNULL([CBUpdatedPricing], CAST(0 AS BIT)) [CBUpdatedPricing], 
                        DS.SchoolName, DS.SchoolNum,ISNULL([DateOfPhysicalInventory], '1/1/1900') [DateOfPhysicalInventory], ISNULL([SnapshotName],'') SnapshotName, ISNULL([SnapshotTotal],0) SnapshotTotal
                        FROM DistrictSchools DS 
                        LEFT OUTER JOIN [EOMChecklistSchools] CS ON DS.DistrictID=CS.DistrictID AND DS.SchoolNum=CS.SchoolNum AND @YearMonth=CS.YearMonth AND @ProgramYear=CS.ProgramYear
                        WHERE DS.Inactive=0 AND DS.DistrictID=@DistrictID  
                        ORDER BY SortOrder">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="TextBoxFYearmonth" Name="YearMonth" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <!-- Note: Operating Totals contains reference to Linked Server which connects the APPS Instance to the GP instance for access to BDGT Views -->
                <asp:SqlDataSource ID="SqlDataOperating" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader" 
                 SelectCommand="OperatingTotals" SelectCommandType="StoredProcedure">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:Parameter Name="StartingDate" Type="String" />
                        <asp:Parameter Name="EndingDate" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceBDGT" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" DataSourceMode="DataReader" 
                    SelectCommand="SELECT [PaperCPM]+[CleaningCPM] PaperCleanCPM, [FoodCPM], [SFAHoursDayTotal], [NUTRHoursDayTotal],
                    IIF([DaysOfSvcLunch]=0, 0, ([SFANonProdHours]/[DaysOfSvcLunch])) SFANPHoursDayTotal, 
                    IIF([DaysOfSvcLunch]=0, 0, ([NUTRNonProdHours]/[DaysOfSvcLunch])) NUTRNPHoursDayTotal,
                    [DistLbrHrsPerDay] SPFNDistHrsPerDay, [NutLbrHrsPerDay] SPFNNutrHrsPerDay,
                    IIF([DaysOfSvcLunch]=0, 0, ([SFALabor]/[DaysOfSvcLunch])) SFAWagesDayTotal, 
                    IIF([DaysOfSvcLunch]=0, 0, ([NUTRLabor]/[DaysOfSvcLunch])) NUTRWagesDayTotal,
                    LessCommodityUseAmt, [ProfitBudget], [DaysOfSvcLunch], [DailyBPaidCount], [DailyBReducedCount],[DailyBFreeCount],[DailyLPaidCount],[DailyLReducedCount],[DailyLFreeCount],
                    [DailyAla]
                    FROM [vwBDGT_Totals] TOTN
                    LEFT OUTER JOIN [vwBDGT_LaborSFATotals] SFA ON TOTN.BidNumber=SFA.BidNumber AND TOTN.BudgetYear=SFA.BudgetYear AND TOTN.BudgetVersion=SFA.BudgetVersion
                    LEFT OUTER JOIN [vwBDGT_LaborNUTRTotals] NUTR ON TOTN.BidNumber=NUTR.BidNumber AND TOTN.BudgetYear=NUTR.BudgetYear AND TOTN.BudgetVersion=NUTR.BudgetVersion
                    LEFT OUTER JOIN [BDGT_SpecialFunctionsCst] SpFn ON TOTN.BidNumber=SpFn.BidNumber AND TOTN.BudgetYear=SpFn.BudgetYear AND TOTN.BudgetVersion=SpFn.BudgetVersion
                    WHERE (TOTN.[BidNumber] = @DistrictID) AND TOTN.BudgetYear=@BudgetYear AND [DaysOfSvcLunch]<>0">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter Name="BudgetYear" ControlID="HFBudgetYear" PropertyName="Value" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceEmailSettings" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT * FROM [PaperworkWebEmailSettings] WHERE ([Index] = @Index)">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="Index" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
            
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
