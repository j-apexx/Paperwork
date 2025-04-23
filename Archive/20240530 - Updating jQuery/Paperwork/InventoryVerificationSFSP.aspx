<%@ Page Title="Inventory Verification SFSP" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InventoryVerificationSFSP.aspx.cs" Inherits="Paperwork.InventoryVerificationSFSP" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        var w = $(window).width();
        var timer,
            $win = $(window);

        //when window resizes
        $win.on('resize', function () {
            //update the width variable
            w = $(window).width();
            clearTimeout(timer);
            timer = setTimeout(function () {
                $("#SpecialFitDiv").width(w - 20);
            }, 200);

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

        $(function () {
            BindEvents();
        });





        function BindEvents() {

            var PosRefY = $("#DataEntryReferenceLocation").position().top;

            $('#SpecialFitDiv').width(w - 20);
            $('#SpecialFitDiv').css("top", PosRefY + 30 );
            $('#SpecialFitDiv').css("left", 10);

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            //Ensure footer numeric insert textboxes have valid numbers
            $("[id*=TextBoxFood],[id*=TextBoxPaper],[id*=TextBoxCleaning],[id*=TextBoxMilk]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }

                UpdateTotals();
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

        function UpdateTotals() {
            var SYFoodEnding = parseFloat($("[id$=TextBoxFoodSYEnding]").val());
            $("[id$=TextBoxFoodSFSPOpening]").val(SYFoodEnding.toFixed(2).toString());
            var SYMilkEnding = parseFloat($("[id$=TextBoxMilkSYEnding]").val());
            $("[id$=TextBoxMilkSFSPOpening]").val(SYMilkEnding.toFixed(2).toString());
            var SYPaperEnding = parseFloat($("[id$=TextBoxPaperSYEnding]").val());
            $("[id$=TextBoxPaperSFSPOpening]").val(SYPaperEnding.toFixed(2).toString());
            var SYCleaningEnding = parseFloat($("[id$=TextBoxCleaningSYEnding]").val());
            $("[id$=TextBoxCleaningSFSPOpening]").val(SYCleaningEnding.toFixed(2).toString());

            var SFSPFoodEnding = parseFloat($("[id$=TextBoxFoodSFSPEnding]").val());
            $("[id$=TextBoxFoodSYOpening]").val(SFSPFoodEnding.toFixed(2).toString());
            var SFSPMilkEnding = parseFloat($("[id$=TextBoxMilkSFSPEnding]").val());
            $("[id$=TextBoxMilkSYOpening]").val(SFSPMilkEnding.toFixed(2).toString());
            var SFSPPaperEnding = parseFloat($("[id$=TextBoxPaperSFSPEnding]").val());
            $("[id$=TextBoxPaperSYOpening]").val(SFSPPaperEnding.toFixed(2).toString());
            var SFSPCleaningEnding = parseFloat($("[id$=TextBoxCleaningSFSPEnding]").val());
            $("[id$=TextBoxCleaningSYOpening]").val(SFSPCleaningEnding.toFixed(2).toString());

            //TOTALS
            $("[id$=TextBoxTotalsSYEnding]").val((SYFoodEnding + SYMilkEnding + SYPaperEnding + SYCleaningEnding).toFixed(2).toString());
            $("[id$=TextBoxTotalsSFSPOpening]").val((SYFoodEnding + SYMilkEnding + SYPaperEnding + SYCleaningEnding).toFixed(2).toString());
            $("[id$=TextBoxTotalsSYOpening]").val((SFSPFoodEnding + SFSPMilkEnding + SFSPPaperEnding + SFSPCleaningEnding).toFixed(2).toString());
            $("[id$=TextBoxTotalsSFSPEnding]").val((SFSPFoodEnding + SFSPMilkEnding + SFSPPaperEnding + SFSPCleaningEnding).toFixed(2).toString());
        }

        //apply highlight 
        $('body').on('focus', 'input,textarea,select', function (e) {
            $(this).addClass("highlight");
        });
        // remove highlight
        $('body').on('focusout', 'input,textarea,select', function (e) {
            $(this).removeClass("highlight");
        });
    </script>
    <style>
        .ui-datepicker-calendar {
            display: none;
        }
    </style>
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
                        <div id="hdrDiv" style="float:left;width:40%">
                            <h2><%: Title %></h2>
                        </div>
                        <div id="menuDiv">
                            <div class="row">
                                <div class="col-sm-8">
                                    <strong>Program Year:</strong>
                                    <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                        DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                                    &nbsp;&nbsp; <br />
                                    <strong>District:</strong>
                                    <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                        DataTextField="Desc" DataValueField="DistrictID" OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                                    </asp:DropDownList> 
                                </div>
                                <div id="hdrPrintButton" class="col-sm-4" style="padding-top:10px;text-align:right;">
                                    <asp:LinkButton ID="IPrint" Width="200" runat="server" 
                                    class="fas fa-print mGridLinkButton" 
                                    ToolTip="Print" Text=" Verification Report" OnClick="IPrint_Click"></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            <div id="DataEntryReferenceLocation"></div>
            <div id="SpecialFitDiv" style="overflow-y:auto;background-color:white; position:absolute;" >
                <div id="DataEntry">
                    <asp:FormView ID="FormViewInvVerification" runat="server" CssClass="mGridRecaps" DataSourceID="SqlDataSourceSFSPInvVerification" 
                        DefaultMode="Edit" DataKeyNames="ProgramYear, DistrictID" OnPreRender="FormViewInvVerification_PreRender" OnDataBound="FormViewInvVerification_DataBound">
                        <EditItemTemplate>
                            <div class="container">
                                <div id="SFSP Inventory Verification">
                                    <div class="row" style="padding-top:10px;" >
                                        <div class="col-sm-2" style="font-weight:bold;">
                                            District:
                                        </div>
                                        <div class="col-sm-4" style="font-weight:bold;border-color:black;border-width:2px;border-style:solid;">
                                            <asp:Label ID="LabelDistrictName" runat="server" Text='<%# Eval("DistrictName") %>'></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">&nbsp;</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">&nbsp;</div>
                                        <div class="col-sm-2" style="font-weight:bold;border-color:black;border-width:2px;border-style:solid;background-color:#92D050;text-align:center;">
                                            Normal School Year<br />
                                            Ending Purchased Inventory
                                        </div>
                                        <div class="col-sm-2" style="font-weight:bold;border-color:black;border-width:2px;border-style:solid;background-color:#92D050;text-align:center;">
                                            SFSP Opening<br />
                                            Purchased Inventory</div>
                                        <div class="col-sm-2" style="font-weight:bold;border-color:black;border-width:2px;border-style:solid;background-color:yellow;text-align:center;">
                                            SFSP Closing<br />
                                            Purchased Inventory</div>
                                        <div class="col-sm-2" style="font-weight:bold;border-color:black;border-width:2px;border-style:solid;background-color:yellow;text-align:center;">
                                            Normal School Year<br />
                                            Opening Purchased Inventory</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">&nbsp;</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2" style="font-weight:bold;">
                                            Food
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxFoodSYEnding" runat="server" Text='<%# Bind("GroceriesSYEnding","{0:F2}") %>' Width="100" OnTextChanged="TextBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxFoodSFSPOpening" runat="server" Text='<%# Bind("GroceriesSYEnding","{0:F2}") %>' Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxFoodSFSPEnding" runat="server" Text='<%# Bind("GroceriesSFSPEnding","{0:F2}") %>' Width="100" OnTextChanged="TextBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxFoodSYOpening" runat="server" Text='<%# Bind("GroceriesSFSPEnding","{0:F2}") %>' Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">&nbsp;</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2" style="font-weight:bold;">
                                            Milk
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxMilkSYEnding" runat="server" Text='<%# Bind("MilkSYEnding","{0:F2}") %>' Width="100" OnTextChanged="TextBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxMilkSFSPOpening" runat="server" Text='<%# Bind("MilkSYEnding","{0:F2}") %>' Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxMilkSFSPEnding" runat="server" Text='<%# Bind("MilkSFSPEnding","{0:F2}") %>' Width="100" OnTextChanged="TextBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxMilkSYOpening" runat="server" Text='<%# Bind("MilkSFSPEnding","{0:F2}") %>' Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">&nbsp;</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2" style="font-weight:bold;">
                                            Paper
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxPaperSYEnding" runat="server" Text='<%# Bind("PaperSYEnding","{0:F2}") %>' Width="100" OnTextChanged="TextBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxPaperSFSPOpening" runat="server" Text='<%# Bind("PaperSYEnding","{0:F2}") %>' Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxPaperSFSPEnding" runat="server" Text='<%# Bind("PaperSFSPEnding","{0:F2}") %>' Width="100" OnTextChanged="TextBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxPaperSYOpening" runat="server" Text='<%# Bind("PaperSFSPEnding","{0:F2}") %>' Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">&nbsp;</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2" style="font-weight:bold;">
                                            Cleaning
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxCleaningSYEnding" runat="server" Text='<%# Bind("CleaningSYEnding","{0:F2}") %>' Width="100" OnTextChanged="TextBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxCleaningSFSPOpening" runat="server" Text='<%# Bind("CleaningSYEnding","{0:F2}") %>' Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxCleaningSFSPEnding" runat="server" Text='<%# Bind("CleaningSFSPEnding","{0:F2}") %>' Width="100" OnTextChanged="TextBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="text-align:center;">
                                            <asp:TextBox ID="TextBoxCleaningSYOpening" runat="server" Text='<%# Bind("CleaningSFSPEnding","{0:F2}") %>' Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">&nbsp;</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2" style="font-weight:bold;">
                                            Total Purchased Inventory
                                        </div>
                                        <div class="col-sm-2" style="font-weight:bold;border-color:black;border-width:2px;border-style:solid;background-color:#92D050;text-align:center;">
                                            <asp:TextBox ID="TextBoxTotalsSYEnding" runat="server" Text="0.00" Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="font-weight:bold;border-color:black;border-width:2px;border-style:solid;background-color:#92D050;text-align:center;">
                                            <asp:TextBox ID="TextBoxTotalsSFSPOpening" runat="server" Text="0.00" Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="font-weight:bold;border-color:black;border-width:2px;border-style:solid;background-color:yellow;text-align:center;">
                                            <asp:TextBox ID="TextBoxTotalsSFSPEnding" runat="server" Text="0.00" Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2" style="font-weight:bold;border-color:black;border-width:2px;border-style:solid;background-color:yellow;text-align:center;">
                                            <asp:TextBox ID="TextBoxTotalsSYOpening" runat="server" Text="0.00" Width="100" Enabled="false"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:HiddenField ID="HFVerificationSaved" runat="server" Value="0" />
                                            &nbsp;
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </EditItemTemplate>
                    </asp:FormView>
                    <div class="messagealert" id="alert_container"></div>
                </div>
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, RTRIM(BudgetYear) BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSFSPInvVerification" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT PY.[ProgramYear],ISNULL(SIV.[DistrictID], @DistrictID) DistrictID ,ISNULL([GroceriesSYEnding],0) [GroceriesSYEnding],ISNULL([PaperSYEnding],0) [PaperSYEnding],ISNULL([CleaningSYEnding],0) [CleaningSYEnding],ISNULL([MilkSYEnding],0) [MilkSYEnding]
                    ,ISNULL([GroceriesSFSPEnding],0) [GroceriesSFSPEnding],ISNULL([PaperSFSPEnding],0) [PaperSFSPEnding],ISNULL([CleaningSFSPEnding],0) [CleaningSFSPEnding],ISNULL([MilkSFSPEnding],0) [MilkSFSPEnding]
                    ,D.DistrictName
                    FROM [ProgramYear] PY
                    LEFT OUTER JOIN [SFSPInventoryVerification] SIV ON PY.ProgramYear=SIV.ProgramYear AND @DistrictID=SIV.DistrictID
                    INNER JOIN [District] D ON @DistrictID=D.DistrictID
                    WHERE PY.ProgramYear=@ProgramYear">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" DbType="String" Name="ProgramYear" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" DbType="String" Name="DistrictID" PropertyName="SelectedValue" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
