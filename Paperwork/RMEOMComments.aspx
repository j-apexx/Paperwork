<%@ Page Title="RM EOM Comments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RMEOMComments.aspx.cs" Inherits="Paperwork.RMEOMComments" %>
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
        var topAndBottom = 100;
        var timer,
            $win = $(window);

        //when window resizes
        //$win.on('resize', function () {
        //    //update the height variable
        //    h = $(window).height();
        //    clearTimeout(timer);
        //    timer = setTimeout(function () {
        //        $("#GridViewHeightController").height(h - topAndBottom);
        //    }, 200);
        //});

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
            //$('#GridViewHeightController').css("height", h - topAndBottom);
            //$('#GridViewHeightController').css("overflow", "auto");

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
        };

    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

        <div id="topSection" style="height:101px;">
            <div id="hdrDiv" style="float:left;width:30%;height:100px;">
                <h2><%: Title %></h2>
                <asp:LinkButton ID="ISave" runat="server" 
                    ToolTip="Save Changes" Text=" Save" ForeColor="Green"
                    CssClass="fas fa-save mGridLinkButton" Width="80" OnClick="iSave_Click" ></asp:LinkButton>* Autosave is on
                <div class="messagealert" id="alert_container" style="float:right;width:130px;text-align:center;" ></div>
            </div>
                
            <div id="menuDiv" >
                <div class="row">
                    <div class="col">
                        <strong>Program Year:</strong>
                        <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                            DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                        <asp:HiddenField ID="HFStartDate" runat="server" />
                        <asp:HiddenField ID="HFEndDate" runat="server" />
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
        </div>
        <asp:HiddenField id="hdnScrollPos" runat="server"/>
        <div id="report">
            <asp:HiddenField ID="HFSSRSBaseFolder" runat="server" />
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
                CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
                ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
                ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
                AsyncRendering="True" InteractivityPostBackMode="AlwaysSynchronous" >
                <ServerReport ReportPath="RMEOM(New)" />
            </rsweb:ReportViewer>
            <br />
        </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="DataEntry">
                <div id="GridViewHeightController" style="overflow-y:auto;background-color:white;" class="row">
                    <div class="col-sm-12">
                        
                        <asp:FormView ID="FormViewRMEOMComments" runat="server" CssClass="mGrid" DefaultMode="Edit" DataSourceID="SqlDataSourceRMEOM"
                            DataKeyNames="ProgramYear, DistrictID, YearMonth" OnPreRender="FormViewRMEOMComments_PreRender" OnDataBound="FormViewRMEOMComments_DataBound" >
                            <EditItemTemplate>
                                <div class="container">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <strong>Enter comments and explanations below:</strong>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4">
                                            NI Side YTD Budget (Prior Month P&L):
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxNISideB" runat="server" Text='<%# Bind("NI_SideYTD_Budget" ) %>' Width="150"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4">
                                            NI Side YTD Actual (Prior Month P&L):
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxNISideA" runat="server" Text='<%# Bind("NI_SideYTD_Actual" ) %>'  Width="150"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4">
                                            Participations:
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxParticipations" runat="server" Text='<%# Bind("CommentsParticipation" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4">
                                            Food Cost (Include USDA & Inventory):
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxFoodCost" runat="server" Text='<%# Bind("CommentsFoodCost" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4">
                                            Labor:
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxLabor" runat="server" Text='<%# Bind("CommentsLabor" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4">
                                            Client P/L (Include guarantee concerns):
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxClientPL" runat="server" Text='<%# Bind("CommentsClientPL" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4">
                                            NI Side (Prior Month P&L):
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxNISide" runat="server" Text='<%# Bind("CommentsNISide" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4">
                                            Action Plan:
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox ID="TextBoxActionPlan" runat="server" Text='<%# Bind("CommentsActionPlan" ) %>' TextMode="MultiLine" Rows="2" Width="100%"  OnTextChanged="ChecklistItemChanged" AutoCompleteType="Disabled" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:HiddenField ID="HFCommentsSaved" runat="server" Value="0" />
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
                            CssClass="fas fa-save mGridLinkButton" Width="80" OnClick="iSave_Click" ></asp:LinkButton>
                        <div class="messagealert" id="alert_container2" style="float:right;width:130px;text-align:center;" ></div>
                        <br />


                    </div>
                </div>
                
            </div>

            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceRMEOM" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT DYI.ProgramYear, DYI.DistrictID, ISNULL(YearMonth, @YearMonth) YearMonth, ISNULL([CommentsParticipation],'') [CommentsParticipation], ISNULL([CommentsFoodCost],'') [CommentsFoodCost],
                    ISNULL([CommentsLabor],'') [CommentsLabor], ISNULL([CommentsClientPL],'') [CommentsClientPL],
                    ISNULL([CommentsNISide],'') [CommentsNISide], ISNULL([CommentsActionPlan],'') [CommentsActionPlan],
                    ISNULL([NI_SideYTD_Budget],0) [NI_SideYTD_Budget], ISNULL([NI_SideYTD_Actual],0) [NI_SideYTD_Actual]
                    FROM DistrictYearInfo DYI
					LEFT OUTER JOIN RMEOMComments ON DYI.ProgramYear=RMEOMComments.ProgramYear AND DYI.DistrictID=RMEOMComments.DistrictID AND @YearMonth=YearMonth
                    WHERE DYI.ProgramYear=@ProgramYear AND DYI.DistrictID=@DistrictID ">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="TextBoxFYearmonth" Name="YearMonth" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
            
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
