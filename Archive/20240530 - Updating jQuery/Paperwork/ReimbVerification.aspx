<%@ Page Title="Reimbursement Verification" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReimbVerification.aspx.cs" Inherits="Paperwork.ReimbVerification" %>
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

            //Specialized Date Picker for YearMonth field
            $(function () {
                $('[id*=TextBoxYearmonth]').datepicker(
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
                                month = datestr.substring(datestr.length - 2, datestr.length);
                                $(this).datepicker('option', 'defaultDate', new Date(year, month - 1, 1));
                                $(this).datepicker('setDate', new Date(year, month - 1, 1));
                            }
                        }
                    }
                );

                $('[id*=TextBoxPostToYearMonth]').datepicker(
                    {
                        dateFormat: "yy-mm",
                        changeMonth: true,
                        changeYear: true,
                        showButtonPanel: true,
                        minDate: new Date($('[id*=HFMonthStart]').val()),
                        maxDate: new Date($('[id*=HFEndDate]').val()),
                        onClose: function (dateText, inst) {

                            function isDonePressed() {
                                return ($('#ui-datepicker-div').html().indexOf('ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all ui-state-hover') > -1);
                            }

                            if (isDonePressed()) {
                                var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                                var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                                $(this).datepicker('setDate', new Date(year, month, 1)).trigger('change');

                                $('.date-picker').focusout();//Added to remove focus from datepicker input box on selecting date
                                //__doPostBack('this.name', '');//need uniqueID to call the postback
                            }
                        },
                        beforeShow: function (input, inst) {

                            inst.dpDiv.addClass('month_year_datepicker');

                            if ((datestr = $(this).val()).length > 0) {
                                year = datestr.substring(0, 4);
                                month = datestr.substring(datestr.length - 2, datestr.length);
                                $(this).datepicker('option', 'defaultDate', new Date(year, month - 1, 1));
                                $(this).datepicker('setDate', new Date(year, month - 1, 1));
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



        };



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
            <div id="topSection" style="height:151px;">
                <div id="hdrDiv" style="float:left;width:30%;height:100px;">
                    <h2><%: Title %></h2>
                    <asp:LinkButton ID="ISave" runat="server" 
                        ToolTip="Save Changes" Text=" Save" ForeColor="Green"
                        CssClass="fas fa-save mGridLinkButton" Width="80" ></asp:LinkButton>* Autosave is on
                    <div class="messagealert" id="alert_container" style="float:right;width:130px;text-align:center;" ></div>
                </div>
                
                <div id="menuDiv" >
                    <div class="row">
                        <div class="col-sm-8">
                            <strong>Program Year:</strong>
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                            <asp:HiddenField ID="HFStartDate" runat="server" />
                            <asp:HiddenField ID="HFEndDate" runat="server" />
                            <asp:HiddenField ID="HFMonthStart" runat="server" />
                            <asp:HiddenField ID="HFMonthEnd" runat="server" />
                            <asp:HiddenField ID="HFBudgetYear" runat="server"  />
                            <br />
                            <strong>District:</strong>
                            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="False" AutoPostBack="true" 
                                DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> 
                            <br />
                            <strong>Year Month:</strong>
                            <asp:TextBox ID="TextBoxYearmonth" runat="server" Width="80" OnTextChanged="TextBoxYearmonth_TextChanged" AutoPostBack="true" ></asp:TextBox>
                        </div>
                        
                        <div id="hdrPrintButton" class="col-sm-4" style="padding-top:10px;text-align:right;">
                            <asp:LinkButton ID="IPrint" Width="200" runat="server" 
                            class="fas fa-print mGridLinkButton" 
                            ToolTip="Print" Text=" Verification Report" OnClick="IPrint_Click"></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
            <asp:HiddenField id="hdnScrollPos" runat="server"/>
            <div id="DataEntry">
                <div id="GridViewHeightController" style="overflow-y:auto;background-color:white;" class="row">
                    <div class="col-sm-12">
                        <asp:FormView ID="FormViewReimbVerification" runat="server" CssClass="mGrid" DefaultMode="Edit" DataSourceID="SqlDataSourceReimbVerif"
                            DataKeyNames="ProgramYear, DistrictID, YearMonth" OnPreRender="FormViewReimbVerification_PreRender" OnDataBound="FormViewReimbVerification_DataBound" >
                            <EditItemTemplate>
                                <div class="container">
                                    <div class="row">
                                        <div class="col-sm-6" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            ACTUAL REIMBURSEMENTS RECEIVED
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            Federal
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            State
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            CACFP
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            <asp:TextBox ID="TextBoxReimbFederal" runat="server" Text='<%# Bind("ActualFederalReimb", "{0:F2}") %>' OnTextChanged="TextBoxReimbursements_TextChanged"
                                                 Width="100" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            <asp:TextBox ID="TextBoxReimbState" runat="server" Text='<%# Bind("ActualStateReimb", "{0:F2}") %>' OnTextChanged="TextBoxReimbursements_TextChanged"
                                                 Width="100" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            <asp:TextBox ID="TextBoxReimbCACFP" runat="server" Text='<%# Bind("ActualCACFPReimb", "{0:F2}") %>' OnTextChanged="TextBoxReimbursements_TextChanged"
                                                 Width="100" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            Total Actual Reimbursements:
                                            <asp:Label ID="LabelActualReimbursements" runat="server" Text='<%# Eval("TotalActualReimbursements", "{0:F2}")  %>' ></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            Total Paperwork Reimbursments:
                                            <asp:Label ID="LabelPaperworkReimbursements" runat="server" Text="0.00" ></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6" style="text-align:center;border-width:1px;border-style:solid;border-color:slategrey;">
                                            Difference:
                                            <asp:Label ID="LabelDifference" runat="server" Text="0.00" ></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">&nbsp;</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-3">
                                            REIMBURSEMENT ADJUSTMENT:
                                        </div>
                                        <div class="col-sm-2">
                                            <asp:TextBox ID="TextBoxReimbAdjustment" runat="server" Text='<%# Bind("ReimbursementAdjustment", "{0:F2}") %>' OnTextChanged="TextBoxReimbursements_TextChanged"
                                                 Width="100" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2">
                                            Post To:
                                        </div>
                                        <div class="col-sm-2">
                                            <asp:TextBox ID="TextBoxPostToYearMonth" runat="server" Text='<%# Bind("PostToYearMonth") %>' OnTextChanged="TextBoxPostToYearMonth_TextChanged"
                                                 Width="100" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            Comments:<br />
                                            <asp:TextBox ID="TextBoxComments" runat="server" Rows="3" AutoCompleteType="Disabled" AutoPostBack="true" OnTextChanged="TextBoxReimbursements_TextChanged"
                                                Width="50%" TextMode="MultiLine" ></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:HiddenField ID="HFReimbursementSaved" runat="server" Value="0" />
                                            &nbsp;
                                        </div>
                                    </div>
                                </div>
                            </EditItemTemplate>

                        </asp:FormView>

                        

                    </div>
                </div>

                
            </div>


            
        </ContentTemplate>
    </asp:UpdatePanel>

                <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceReimbVerif" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                 SelectCommand="SELECT DYI.[ProgramYear],DYI.[DistrictID],ISNULL([YearMonth],@YearMonth) YearMonth,ISNULL([ActualFederalReimb],0) ActualFederalReimb,ISNULL([ActualStateReimb],0) ActualStateReimb
                        ,ISNULL([ActualCACFPReimb],0) ActualCACFPReimb, ISNULL([ActualFederalReimb] + [ActualStateReimb] + [ActualCACFPReimb],0) TotalActualReimbursements
                        ,ISNULL([ReimbursementAdjustment],0) ReimbursementAdjustment,ISNULL([PostToYearMonth],@YearMonth) PostToYearMonth, ISNULL(ReimbAdjustComments,'') ReimbAdjustComments
                        FROM [DistrictYearInfo] DYI
                        LEFT OUTER JOIN [ReimbursementVerification] RV ON DYI.DistrictID=RV.DistrictID AND DYI.ProgramYear=RV.ProgramYear AND @YearMonth=RV.YearMonth
                        WHERE DYI.[ProgramYear]=@ProgramYear AND DYI.[DistrictID]=@DistrictID">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="TextBoxYearmonth" Name="YearMonth" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceRecapTotals" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                        SELECTCommand="SELECT ISNULL(SUM(TotalReim),0) TotalReim FROM [dbo].[vw_RecapTotals]
                            WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND RecapDate>=@StartDate AND RecapDate<=@EndDate" DataSourceMode="DataReader">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                            <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                            <asp:ControlParameter ControlID="HFMonthStart" Name="StartDate" PropertyName="Value" Type="DateTime" />
                            <asp:ControlParameter ControlID="HFMonthEnd" Name="EndDate" PropertyName="Value" Type="DateTime" />
                        </SelectParameters>
                    </asp:SqlDataSource>
            </div>
</asp:Content>
