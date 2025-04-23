<%@ Page Title="Inventory Adjustments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InvAdjustments.aspx.cs" Inherits="Paperwork.InvAdjustments" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        // For gridview resizing
        var h = $(window).height();
        var topAndBottom = 180;
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

            var PYearStart = $("[id*=HFProgramYearStartDate]").val();
            var PYearEnd = $("[id*=HFProgramYearEndDate]").val();

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
            $("[id*=TextBoxFAmount],[id*=TextBoxEAmount]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
            });

            //Specialized Date Picker
            $('[id*=TextBoxFYearmonth]').datepicker(
                {
                    dateFormat: "yy-mm",
                    changeMonth: true,
                    changeYear: true,
                    showButtonPanel: true,
                    minDate: new Date($('[id*=HFProgramYearStartDate]').val()),
                    maxDate: new Date($('[id*=HFProgramYearEndDate]').val()),
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

                        inst.dpDiv.addClass('month_year_datepicker')

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
                });

        }
    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="hdrGroup" class="row">
                <div id="hdrDiv" class="col-sm-6">
                    <h2><%: Title %></h2>
                    Use this page to create adjustments to USDA Commodity Received including NOI. Inventory adjustments are only included on invoice inventory totals.
                </div>
                <div id="SelectionDiv" class="col-sm-6">
                    <div class="row">
                        <div class="col-sm-4">
                            Program Year:
                        </div>
                        <div class="col-sm-6">
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <asp:HiddenField ID="HFProgramYearStartDate" runat="server" />
                            <asp:HiddenField ID="HFProgramYearEndDate" runat="server" />
                            <asp:HiddenField id="hdnScrollPos" runat="server"/>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4">
                            District:
                        </div>
                        <div class="col-sm-6">
                            <asp:DropDownList ID="DDLDistricts" runat="server" DataSourceID="SqlDataSourceDistricts" DataTextField="Desc" 
                                DataValueField="DistrictID" AutoPostBack="true" >
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4">
                            &nbsp;
                        </div>
                        <div class="col-sm-6" style="padding-top:5px;">
                            <asp:CheckBox ID="CBActiveOnly" runat="server" Text=" Display Inactive Districts" Checked="false" AutoPostBack="true" OnCheckedChanged="CBActiveOnly_CheckedChanged"/>
                        </div>
                    </div>
                </div>
            </div>

            <div id="GridViewHeightController" style="overflow-y:auto;" class="row">
                <div class="col-sm-6 offset-sm-2 offset-xs-0">
                    <asp:GridView ID="GridViewInvAdjustments" runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataKeyNames="ProgramYear, DistrictID, YearMonth" 
                        DataSourceID="SqlDataSourceInvAdjusts" Width="600px" ShowFooter="True" ShowHeaderWhenEmpty="True"
                        AllowSorting="True" OnRowDataBound="GridViewInvAdjustments_RowDataBound" OnRowCommand="GridViewInvAdjustments_RowCommand" >
                        <Columns>
                            <asp:TemplateField HeaderText="District" SortExpression="DistrictName">
                                <EditItemTemplate>
                                    <asp:Label ID="LabelEDistrict" runat="server" Text='<%# Eval("DistrictName") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelDistrict" runat="server" Text='<%# Eval("DistrictName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col-sm-12" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            **NEW**
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:DropDownList ID="DDLFDistricts" runat="server" DataSourceID="SqlDataSourceFDistricts" DataTextField="Desc" 
                                                DataValueField="DistrictID" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="YearMonth" SortExpression="YearMonth">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEYearmonth" runat="server" Text='<%# Bind("YearMonth") %>' ReadOnly="true" ></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelYearmonth" runat="server" Text='<%# Eval("YearMonth") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:TextBox ID="TextBoxFYearmonth" runat="server"  ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="ReqTrxMonth" runat="server" ControlToValidate="TextBoxFYearmonth" CssClass="validators" 
                                                Display="Dynamic" ErrorMessage="Inventory Month is required." ValidationGroup="Insert"  >*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="RegexTrxMonth" runat="server" ControlToValidate="TextBoxFYearmonth" CssClass="validators" ValidationExpression="([\d]){4}-([\d]){2}|(\*OPEN\*)|(CLOSE)"
                                                Display="Dynamic" ErrorMessage="Inventory Month is not formatted correctly (YYYY-MM)." ValidationGroup="Insert">*</asp:RegularExpressionValidator>
                                        </div>
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Amount" SortExpression="CommodRcvdInclNOI_Adjust">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEAmount" runat="server" Text='<%# Bind("CommodRcvdInclNOI_Adjust", "{0:F2}") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelAmount" runat="server" Text='<%# Bind("CommodRcvdInclNOI_Adjust", "{0:C2}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:TextBox ID="TextBoxFAmount" runat="server" Text="0.00" ></asp:TextBox>
                                        </div>
                                    </div>
                                    
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Comments" SortExpression="USDAComments">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEComments" runat="server" Text='<%# Bind("USDAComments") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelComments" runat="server" Text='<%# Eval("USDAComments") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:TextBox ID="TextBoxFComments" runat="server"  ></asp:TextBox>
                                        </div>
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <EditItemTemplate>
                                    <div style="width:120px;text-align:center;" class="col"  >
                                        <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="width:120px;text-align:center;" class="col" >
                                        <asp:LinkButton runat="server" CommandName="Edit" ID="IEdit" ToolTip="Edit" 
                                            CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  />
                                        <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                            CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                            OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    
                                    <div class="row">
                                        <div style="width:120px;text-align:center;" class="col" >
                                            <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                                CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                            <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" 
                                                CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                        </div>
                                    </div>
                                    
                                </FooterTemplate>
                        
                                <ItemStyle Width="50" />
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
            
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OnFiltering="SqlDataSourceDistricts_Filtering"
            SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc', Inactive
            FROM [District]
            UNION ALL
            SELECT '%' as [DistrictID], '*All Districts*' as [DistrictName], 0 as [Inactive]
            ORDER BY [Desc]"
            FilterExpression="Inactive<={0}">
            <FilterParameters>
                <asp:ControlParameter ControlID="CBActiveOnly" PropertyName="Checked" Type="SByte" Name="Inactive" />
            </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceFDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc', Inactive
            FROM [District]
            ORDER BY [Desc]"
            FilterExpression="Inactive<={0}">
            <FilterParameters>
                <asp:ControlParameter ControlID="CBActiveOnly" PropertyName="Checked" Type="SByte" Name="Inactive" />
            </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="SqlDataSourceInvAdjusts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT IA.ProgramYear, IA.DistrictID, IA.YearMonth, IA.CommodRcvdInclNOI_Adjust, IA.USDAComments, IA.CreatedDate, IA.LastModifiedUser, IA.LastModifiedDate, D.DistrictName
            FROM [InventoryAdjustment] IA
            INNER JOIN [District] D ON IA.DistrictID=D.DistrictID
            WHERE IA.DistrictID LIKE @DistrictID AND IA.ProgramYear=@ProgramYear 
            UNION ALL
            SELECT '','', '', 0, '', '1/1/1900', '', '1/1/1900',''
            ORDER BY IA.DistrictID, IA.YearMonth " 
            DeleteCommand="DELETE [InventoryAdjustment] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth" 
            UpdateCommand="UPDATE [InventoryAdjustment] SET CommodRcvdInclNOI_Adjust=@CommodRcvdInclNOI_Adjust, USDAComments=@USDAComments, LastModifiedDate=GETDATE(), LastModifiedUser=@LastModifiedUser WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND YearMonth=@YearMonth"
            InsertCommand="INSERT INTO [InventoryAdjustment] (ProgramYear, DistrictID, YearMonth, [CommodRcvdInclNOI_Adjust],[USDAComments],[CreatedDate],[LastModifiedUser],[LastModifiedDate]) VALUES (@ProgramYear, @DistrictID, @YearMonth, @CommodRcvdInclNOI_Adjust, @USDAComments, GetDate(), @LastModifiedUser, GetDate())"
            OnInserting="SqlDataSourceInvAdjusts_Inserting">
            <DeleteParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="YearMonth" Type="String" />
            </DeleteParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="YearMonth" Type="String" />
                <asp:Parameter Name="CommodRcvdInclNOI_Adjust" Type="Decimal" />
                <asp:Parameter Name="USDAComments" Type="String" />
                <asp:SessionParameter Name="LastModifiedUser" SessionField="CurrentUser" Type="String" />
            </UpdateParameters>
            <InsertParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="YearMonth" Type="String" />
                <asp:Parameter Name="CommodRcvdInclNOI_Adjust" Type="Decimal" />
                <asp:Parameter Name="USDAComments" Type="String" />
                <asp:SessionParameter Name="LastModifiedUser" SessionField="CurrentUser" Type="String" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>

    
</asp:Content>
