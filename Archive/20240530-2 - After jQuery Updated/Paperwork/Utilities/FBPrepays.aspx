<%@ Page Title="Full Bill Prepays" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FBPrepays.aspx.cs" Inherits="Paperwork.FBPrepays" %>
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
            $("[id*=TextBoxFPrepayAmount],[id*=TextBoxEPrepayAmount]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
            });

            $(function () {
                $("[id*=TextBoxEPrepayDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxFPrepayDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
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
                    Use this page to create or modify Full Bill Prepayments. 
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
                <div class="col-sm-12" >
                    <asp:GridView ID="GridViewFBPrepays" runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataKeyNames="Counter" 
                        DataSourceID="SqlDataSourceFBPrepays" Width="600px" ShowFooter="True" ShowHeaderWhenEmpty="True"
                        AllowSorting="True" OnRowDataBound="GridViewFBPrepays_RowDataBound" OnRowCommand="GridViewFBPrepays_RowCommand" >
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
                            <asp:TemplateField HeaderText="Date" SortExpression="PrepayDate">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEPrepayDate" runat="server" Text='<%# Bind("PrepayDate", "{0:d}") %>' OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelPrepayDate" runat="server" Text='<%# Eval("PrepayDate", "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:TextBox ID="TextBoxFPrepayDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                        </div>
                                    </div>
                                    
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Amount" SortExpression="PrepayAmount">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEPrepayAmount" runat="server" Text='<%# Bind("PrepayAmount", "{0:F2}") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelPrepayAmount" runat="server" Text='<%# Bind("PrepayAmount", "{0:C2}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:TextBox ID="TextBoxFPrepayAmount" runat="server" Text="0.00" ></asp:TextBox>
                                        </div>
                                    </div>
                                    
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description" SortExpression="Description">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEDescription" runat="server" Text='<%# Bind("Description") %>' MaxLength="50"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelDescription" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:TextBox ID="TextBoxFDescription" runat="server" Text="" MaxLength="50" ></asp:TextBox>
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
        <asp:SqlDataSource ID="SqlDataSourceFBPrepays" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT FB.PrepayDate, FB.PrepayAmount, FB.Counter, FB.ProgramYear, FB.DistrictID, FB.Description, D.DistrictName
            FROM [FBPrepays] FB
            INNER JOIN [District] D ON FB.DistrictID=D.DistrictID
            WHERE FB.DistrictID LIKE @DistrictID AND ProgramYear=@ProgramYear 
            UNION ALL
            SELECT '1/1/1900', 0.00, 0, '', '', '', ''
            ORDER BY FB.DistrictID, FB.PrepayDate " 
            DeleteCommand="DELETE [FBPrepays] WHERE Counter=@Counter" 
            UpdateCommand="UPDATE [FBPrepays] SET PrepayDate=@PrepayDate, PrepayAmount=@PrepayAmount, Description=@Description WHERE Counter=@Counter"
            InsertCommand="INSERT INTO [FBPrepays] (ProgramYear, DistrictID, PrepayDate, PrepayAmount, Description) VALUES (@ProgramYear, @DistrictID, @PrepayDate, @PrepayAmount, @Description)"
            OnInserting="SqlDataSourceFBPrepays_Inserting" OnInserted="SqlDataSourceFBPrepays_Inserted">
            <DeleteParameters>
                <asp:Parameter Name="Counter" />
            </DeleteParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="PrepayDate" />
                <asp:Parameter Name="PrepayAmount" />
                <asp:Parameter Name="Description" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="Counter" />
            </UpdateParameters>
            <InsertParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="PrepayDate" Type="DateTime" />
                <asp:Parameter Name="PrepayAmount" Type="Decimal" />
                <asp:Parameter Name="Description" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>

    
</asp:Content>
