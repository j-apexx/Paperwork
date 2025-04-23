<%@ Page Title="Invoice Credits" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InvoiceCredits.aspx.cs" Inherits="Paperwork.InvoiceCredits" %>
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
            $("[id*=TextBoxFCreditAmount],[id*=TextBoxECreditAmount]").change(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
            });

            $(function () {
                $("[id*=TextBoxECreditDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxFCreditDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
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
                    Use this page to create or modify Invoice Credits. 
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
                    <asp:GridView ID="GridViewInvoiceCredits" runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataKeyNames="Counter" 
                        DataSourceID="SqlDataSourceInvoiceCredits" Width="600px" ShowFooter="True" ShowHeaderWhenEmpty="True"
                        AllowSorting="True" OnRowDataBound="GridViewInvoiceCredits_RowDataBound" OnRowCommand="GridViewInvoiceCredits_RowCommand" >
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
                            <asp:TemplateField HeaderText="Date" SortExpression="CreditDate">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxECreditDate" runat="server" Text='<%# Bind("CreditDate", "{0:d}") %>' OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelCreditDate" runat="server" Text='<%# Eval("CreditDate", "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:TextBox ID="TextBoxFCreditDate" runat="server" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                        </div>
                                    </div>
                                    
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxECreditAmount" runat="server" Text='<%# Bind("CreditAmount", "{0:F2}") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelCreditAmount" runat="server" Text='<%# Bind("CreditAmount", "{0:C2}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div class="row">
                                        <div class="col" style="background-color:white;border-bottom:solid;border-bottom-color:black;border-bottom-width:1px;">
                                            &nbsp;
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <asp:TextBox ID="TextBoxFCreditAmount" runat="server" Text="0.00" ></asp:TextBox>
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

        <asp:SqlDataSource ID="SqlDataSourceInvoiceCredits" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT IC.CreditDate, IC.CreditAmount, IC.Counter, IC.ProgramYear, IC.DistrictID, IC.Description, D.DistrictName
            FROM [InvoiceCredits] IC
            INNER JOIN [District] D ON IC.DistrictID=D.DistrictID
            WHERE IC.DistrictID LIKE @DistrictID AND ProgramYear=@ProgramYear 
            UNION ALL
            SELECT '1/1/1900', 0.00, 0, '', '', '', ''
            ORDER BY IC.DistrictID, IC.CreditDate " 
            DeleteCommand="DELETE [InvoiceCredits] WHERE Counter=@Counter" 
            UpdateCommand="UPDATE [InvoiceCredits] SET CreditDate=@CreditDate, CreditAmount=@CreditAmount, Description=@Description WHERE Counter=@Counter"
            InsertCommand="INSERT INTO [InvoiceCredits] (ProgramYear, DistrictID, CreditDate, CreditAmount, Description) VALUES (@ProgramYear, @DistrictID, @CreditDate, @CreditAmount, @Description)"
            OnInserting="SqlDataSourceInvoiceCredits_Inserting">
            <DeleteParameters>
                <asp:Parameter Name="Counter" />
            </DeleteParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="CreditDate" />
                <asp:Parameter Name="CreditAmount" />
                <asp:Parameter Name="Description" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="Counter" />
            </UpdateParameters>
            <InsertParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="CreditDate" Type="DateTime" />
                <asp:Parameter Name="CreditAmount" Type="Decimal" />
                <asp:Parameter Name="Description" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>

    
</asp:Content>
