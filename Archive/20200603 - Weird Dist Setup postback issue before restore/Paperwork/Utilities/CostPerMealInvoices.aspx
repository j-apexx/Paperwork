<%@ Page Title="Cost Per Meal Invoices" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CostPerMealInvoices.aspx.cs" Inherits="Paperwork.CPMInvoices" EnableEventValidation="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        $(function () {
            BindEvents();
        });

        function BindEvents() {
            var PYearStart = $("[id*=HFProgYearStartDate]").val();
            var PYearEnd = $("[id*=HFProgYearEndDate]").val();

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
                $("[id*=TextBoxNewInvPerStart]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxNewInvPerEnd]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });
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
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanelTop" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanelNewInv" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanelExistingInv" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanelTop" runat="server" UpdateMode="Always">
            <ContentTemplate>        
                <div id="topSection" style="position:relative;" class="row">
                    <div id="hdrDiv"  class="col-sm-4">
                        <h2><%: Title %></h2>
                    </div>
                    <div id="menuDiv" class="col-sm-8">
                        <div class="row">
                            <div class="col-sm-4">
                                <strong>Program Year:</strong>
                            </div>
                            <div class="col-sm-6">
                                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                    DataValueField="ProgramYear" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                <asp:HiddenField ID="HFProgYearStartDate" runat="server" />
                                <asp:HiddenField ID="HFProgYearEndDate" runat="server" />
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
                    </div>
                    </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="accordian">
            <asp:UpdatePanel ID="UpdatePanelNewInv" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse1" >
                                New CPM Invoice
                            </a>
                        </div>
                        <div id="cardCollapse1" class="collapse hide">
                            <div class="card-body">
                                <p>
                                    Period Start:
                                    <asp:TextBox ID="TextBoxNewInvPerStart" runat="server" Width="100" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorPeriodStart" runat="server" ControlToValidate="TextBoxNewInvPerStart" Display="Dynamic" ErrorMessage="Start Date is Required" ForeColor="Red" ValidationGroup="NewInvoice">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidatorPerStart" runat="server" ControlToValidate="TextBoxNewInvPerStart" Display="Dynamic" ErrorMessage="Start Date format invalid" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="NewInvoice" >*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                    <br />

                                    Period End / Statement Date:
                                    <asp:TextBox ID="TextBoxNewInvPerEnd" runat="server" Width="100" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorEDate" runat="server" ControlToValidate="TextBoxNewInvPerEnd" Display="Dynamic" ErrorMessage="Statement Date is Required" ForeColor="Red" ValidationGroup="NewInvoice">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidatorED" runat="server" ControlToValidate="TextBoxNewInvPerEnd" Display="Dynamic" ErrorMessage="Statement Date format invalid" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="NewInvoice" >*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                    <br />
                                    Select District:
                                    <asp:DropDownList ID="DropDownListNewInvDist" runat="server" DataSourceID="SqlDataSourceInvDists" DataTextField="Desc" 
                                        DataValueField="DistrictID"  >
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredContract" runat="server" ControlToValidate="DropDownListNewInvDist" Display="Dynamic" ValidationGroup="NewInvoice" ErrorMessage="District is required." ForeColor="Red"></asp:RequiredFieldValidator>
                                    <br />
                                    <asp:LinkButton ID="ICreate" runat="server" CausesValidation="True" ValidationGroup="NewInvoice"
                                                        ToolTip="Create Invoice" Text=" Create Invoice"
                                                        CssClass="fas fa-cogs mGridLinkButton" Width="200" OnClick="ICreate_Click" />
                                </p>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
                    
            <asp:UpdatePanel ID="UpdatePanelExistingInv" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse2">
                                Existing CPM Invoices
                            </a>
                        </div>
                        <div id="cardCollapse2" class="collapse show">
                            <div class="card-body">
                                <asp:GridView ID="GridViewInvoiceList" runat="server" AutoGenerateColumns="False" CssClass="mGrid" 
                                    DataKeyNames="InvoiceNumber" DataSourceID="SqlDataSourceInvoiceList" Width="500" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                    AllowPaging="true" PageSize="25" PagerSettings-Mode="NumericFirstLast" PagerStyle-HorizontalAlign="Center" PagerStyle-Font-Size="Large" 
                                    PagerSettings-PageButtonCount="10" PagerStyle-Width="100" PagerStyle-CssClass="mGridPager" 
                                    OnSelectedIndexChanged="GridViewInvoiceList_SelectedIndexChanged" OnRowDataBound="GridViewInvoiceList_RowDataBound" 
                                    OnPageIndexChanged="GridViewInvoiceList_PageIndexChanged" AllowSorting="true" OnSorted="GridViewInvoiceList_Sorted"
                                    OnPreRender="GridViewInvoiceList_PreRender">
                                    <Columns>
                                        <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" ReadOnly="True" SortExpression="InvoiceNumber" />
                                        <asp:BoundField DataField="DistrictID" HeaderText="District" SortExpression="DistrictID" />
                                        <asp:BoundField DataField="PeriodEnd" HeaderText="Statement Date" SortExpression="PeriodEnd" DataFormatString="{0:d}" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>                    
            <asp:UpdatePanel ID="UpdatePanelEditInvoice" runat="server" UpdateMode="Conditional">
                <ContentTemplate>        
                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse3">
                                Invoice Details
                            </a>
                        </div>
                        <div id="cardCollapse3" class="collapse show">
                            <div class="card-body">
                                <div class="row" style="text-align:center;position:relative;top:-10px;">
                                    <div class="col-sm-3">
                                        <div class="messagealert" id="alert_container_InvoiceHdr"></div>
                                    </div>
                                    <div class="col-sm-6">
                                        <asp:HiddenField ID="HFInvoiceNumber" runat="server" />
                                        <asp:HiddenField ID="HFInvoiceSaved" runat="server" />
                                        <asp:HiddenField ID="HFInvoiceLocked" runat="server" />
                                        <asp:LinkButton ID="ISave" runat="server" 
                                            ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                                            CssClass="fas fa-save mGridLinkButton" Width="140" OnClick="ISave_Click" ></asp:LinkButton>
                                        <asp:LinkButton ID="IPrint" Width="140" runat="server" 
                                            class="fas fa-print mGridLinkButton" 
                                            ToolTip="Print" Text=" Print Invoices" OnClick="IPrint_Click" ></asp:LinkButton>
                                    </div>
                                    <div class="col-sm-3">
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-xl-1 col-sm-0 "></div>
                                    <div class="col-xl-10 col-sm-12 " style="padding-top:5px;border-style:solid;border-width:1px;border-color:dimgray;">
                                        <asp:FormView ID="FormViewCPMInvoice" runat="server" CssClass="mGridRecaps" DefaultMode="Edit" DataSourceID="SqlDataSourceInvDetails" DataKeyNames="InvoiceNumber">
                                            <EditItemTemplate>
                                                <div class="container">
                                                    <div class="row">
                                                        <div class="col-sm-2" style="text-align:right;">To:</div>
                                                        <div class="col-sm-4" ><asp:Label ID="LabelDistName" runat="server" Text='<%# Bind("DistrictName") %>'></asp:Label></div>
                                                        <div class="col-sm-3" style="text-align:right;" >Statement Date:</div>
                                                        <div class="col-sm-3" ><asp:TextBox ID="TextBoxDate" runat="server" Text='<%# Bind("PeriodEnd","{0:d}") %>' Width="100" Enabled="false"></asp:TextBox></div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-2">&nbsp;</div>
                                                        <div class="col-sm-4">
                                                            <asp:Label ID="LabelDistAddress1" runat="server" Text='<%# Bind("DistrictAddress1") %>' Visible='<%# Eval("DistrictAddress1")==""?false:true %>'></asp:Label>
                                                    
                                                    
                                                            <asp:Literal ID="break2" runat="server" Text="<br />" Visible='<%# Eval("DistrictAddress2")==""?false:true %>'></asp:Literal>
                                                            <asp:Label ID="LabelDistAddress2" runat="server" Text='<%# Bind("DistrictAddress2") %>' Visible='<%# Eval("DistrictAddress2")==""?false:true %>'></asp:Label>

                                                    
                                                            <asp:Literal ID="break3" runat="server" Text="<br />" Visible='<%# Eval("DistrictCity")=="" && Eval("DistrictState")=="" && Eval("DistrictZip")==""?false:true %>'></asp:Literal>
                                                            <asp:Label ID="LabelCity" runat="server" Text='<%# Bind("DistrictCity") %>' Visible='<%# Eval("DistrictCity")==""?false:true %>'></asp:Label>
                                                            <asp:Literal ID="LiteralComma1" runat="server" Text=", " Visible='<%# Eval("DistrictCity")=="" && Eval("DistrictState")==""?false:true %>'></asp:Literal>
                                                            <asp:Label ID="LabelState" runat="server" Text='<%# Bind("DistrictState") %>' Visible='<%# Eval("DistrictState")==""?false:true %>'></asp:Label>&nbsp;
                                                            <asp:Label ID="LabelZip" runat="server" Text='<%# Bind("DistrictZip") %>' Visible='<%# Eval("DistrictZip")==""?false:true %>'></asp:Label>
                                                        </div>
                                                        <div class="col-sm-3" style="text-align:right;" >Statement Number:</div>
                                                        <div class="col-sm-3">
                                                            <asp:TextBox ID="TextBoxInvoice" runat="server" Text='<%# Bind("InvoiceNumber") %>' Width="100" Enabled="false" MaxLength="15"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-2" style="text-align:right;">Period:</div>
                                                        <div class="col-sm-10" >
                                                            <asp:TextBox ID="TextBoxPeriodStart" runat="server" Text='<%# Bind("PeriodStart", "{0:d}") %>' Width="100"></asp:TextBox> to 
                                                            <asp:TextBox ID="TextBoxPeriodEnd" runat="server" Text='<%# Bind("PeriodEnd", "{0:d}") %>' Width="100"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            &nbsp;
                                                        </div>
                                                    </div>
                                                    <div class="row" style="border-bottom:solid;border-bottom-color:gray;border-bottom-width:2px;text-align:right;">
                                                        <div class="col-sm-2" style="text-align:left;">Detail:</div>
                                                        <div class="col-sm-2">Sales</div>
                                                        <div class="col-sm-2">Equivalent<br />Rate</div>
                                                        <div class="col-sm-2">Meal<br />Count</div>
                                                        <div class="col-sm-2">Meal<br />Rate</div>
                                                        <div class="col-sm-2">Billing<br />Amount</div>
                                                    </div>
                                                    <div class="row" style="text-align:right;">
                                                        <div class="col-sm-2" style="text-align:left;">Breakfast</div>
                                                        <div class="col-sm-4">&nbsp;</div>
                                                        <div class="col-sm-2"><asp:TextBox ID="TextBoxBfastCount" runat="server" Text='<%# Bind("BreakfastTotal") %>' Width="60" ></asp:TextBox></div>
                                                        <div class="col-sm-2"><asp:Label ID="LabelMealRateB" runat="server" Text="0.0000"></asp:Label></div>
                                                        <div class="col-sm-2"><asp:Label ID="LabelBfastAmount" runat="server" Text="0.0000"></asp:Label></div>
                                                    </div>
                                                    <div class="row" style="text-align:right;">
                                                        <div class="col-sm-2" style="text-align:left;">Lunch</div>
                                                        <div class="col-sm-4">&nbsp;</div>
                                                        <div class="col-sm-2"><asp:TextBox ID="TextBoxLunchCount" runat="server" Text='<%# Bind("LunchTotal") %>' Width="60" ></asp:TextBox></div>
                                                        <div class="col-sm-2"><asp:Label ID="LabelMealRateL" runat="server" Text="0.0000"></asp:Label></div>
                                                        <div class="col-sm-2"><asp:Label ID="LabelLunchAmount" runat="server" Text="0.0000"></asp:Label></div>
                                                    </div>
                                                    <div class="row" style="text-align:right;">
                                                        <div class="col-sm-2" style="text-align:left;">Ala Carte Sales</div>
                                                        <div class="col-sm-2"><asp:TextBox ID="TextBoxAlaSales" runat="server" Text='<%# Bind("AlaSales", "{0:F2}") %>' Width="75" ></asp:TextBox></div>
                                                        <div class="col-sm-2"><asp:Label ID="LabelEquivRate" runat="server" Text="0.0000"></asp:Label></div>
                                                        <div class="col-sm-2"><asp:Label ID="LabelEquivMealCount" runat="server" Text="0.0000"></asp:Label></div>
                                                        <div class="col-sm-2"><asp:Label ID="LabelMealRateA" runat="server" Text="0.0000"></asp:Label></div>
                                                        <div class="col-sm-2"><asp:Label ID="LabelAlaAmount" runat="server" Text="0.0000"></asp:Label></div>
                                                    </div>
                                                    <div class="row" style="text-align:right;">
                                                        <div class="col-sm-2" style="text-align:left;">Special Function Sales</div>
                                                        <div class="col-sm-8">&nbsp;</div>
                                                        <div class="col-sm-2"><asp:TextBox ID="TextBoxSpFnSales" runat="server" Text='<%# Bind("SpecialFunctionSales", "{0:F2}") %>' Width="75" ></asp:TextBox></div>
                                                    </div>
                                                    <div class="row" style="text-align:right;">
                                                        <div class="col-sm-2" style="text-align:left;">Other Sales:</div>
                                                    </div>
                                                    <div class="row" style="text-align:right;">
                                                        <div class="col-sm-7" style="text-align:left;">
                                                            USDA Commodity Usage<br />
                                                            USDA Commodity Received
                                                        </div>
                                                        <div class="col-sm-2">
                                                            <asp:Label ID="LabelUSDACommUsage" runat="server" Text="0.00"></asp:Label><br />
                                                            <asp:Label ID="LabelUSDACommRcvd" runat="server" Text="0.00"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-1">&nbsp;</div>
                                                        <div class="col-sm-2" style="vertical-align:middle;height:100%">
                                                            <br />
                                                            <asp:Label ID="Label2" runat="server" Text="0.00"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align:right;">
                                                        <div class="col-sm-7" style="text-align:left;">
                                                            Others....
                                                        </div>
                                                    </div>
                                                    <div class="row" style="text-align:right;">
                                                        <div class="col-sm-10" style="text-align:left;font-weight:bold">
                                                            Total
                                                        </div>
                                                        <div class="col-sm-2" style="font-weight:bold;border-top:solid;border-top-width:2px;border-top-color:black;">
                                                            <asp:Label ID="TextBoxTotal" runat="server" Text='<%# Bind("SpecialFunctionSales", "{0:F2}") %>' Width="75" ></asp:Label>

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
                </ContentTemplate>
            </asp:UpdatePanel>  


        </div>

    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc', Inactive
            FROM [District]
            UNION ALL
            SELECT '%' as [DistrictID], '*All Districts*' as [DistrictName], 0 as [Inactive]
            ORDER BY [Desc]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvDists" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc', Inactive
            FROM [District]
            ORDER BY [Desc]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvoiceList" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            InsertCommand="INSERT INTO [CPMInvoiceHDR] ([InvoiceNumber], [ProgramYear], [DistrictID], [PeriodStart], [PeriodEnd], [BreakfastTotal], [LunchTotal], [CPMRate], [AlaSales], 
            [ACEquivRate], [SpecialFunctionSales], [USDACommUsage], [USDACommRcvd], [DateCreated]) 
            VALUES ((SELECT ISNULL(MAX(InvoiceNumber),0)+1 InvoiceNumber FROM [dbo].[CPMInvoiceHDR]), @ProgramYear, @DistrictID, 
                @PeriodStart, @PeriodEnd, @BreakfastTotal, @LunchTotal, @CPMRate, @AlaSales, @ACEquivRate, @SpecialFunctionSales, @USDACommUsage, @USDACommRcvd, GetDate())" 
            SelectCommand="SELECT HDR.[InvoiceNumber], HDR.[DistrictID], HDR.[PeriodEnd], D.DistrictName
            FROM [CPMInvoiceHDR] HDR
            INNER JOIN [District] D ON HDR.DistrictID=D.DistrictID
            WHERE ([ProgramYear] = @ProgramYear) AND HDR.DistrictID LIKE @DistrictID" 
            >
            <DeleteParameters>
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListNewInvDist" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="TextBoxNewInvPerStart" Name="PeriodStart" PropertyName="Text" Type="DateTime" />
                <asp:ControlParameter ControlID="TextBoxNewInvPerEnd" Name="PeriodEnd" PropertyName="Text" Type="DateTime" />
                <asp:Parameter Name="BreakfastTotal" Type="Int32" DefaultValue="0" />
                <asp:Parameter Name="LunchTotal" Type="Int32" DefaultValue="0" />
                <asp:Parameter Name="CPMRate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="AlaSales" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="ACEquivRate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="SpecialFunctionSales" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="USDACommUsage" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="USDACommRcvd" Type="Decimal" DefaultValue="0" />
            </InsertParameters>
            <SelectParameters>
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter ControlID="DDLDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter DbType="Date" Name="PeriodEnd" />
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceInvDetails" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [CPMInvoiceHDR] WHERE [InvoiceNumber] = @InvoiceNumber" 
            InsertCommand="INSERT INTO [CPMInvoiceHDR] ([InvoiceNumber], [ProgramYear], [DistrictID], [PeriodStart], [PeriodEnd], [BreakfastTotal], [LunchTotal], 
            [CPMRate], [AlaSales], [ACEquivRate], [SpecialFunctionSales], [USDACommUsage], [USDACommRcvd], [DateCreated]) 
            VALUES (@InvoiceNumber, @ProgramYear, @DistrictID, @PeriodStart, @PeriodEnd, @BreakfastTotal, @LunchTotal, 
            @CPMRate, @AlaSales, @ACEquivRate, @SpecialFunctionSales, @USDACommUsage, @USDACommRcvd, @DateCreated)" 
            SelectCommand="SELECT HDR.*, D.*
            FROM [CPMInvoiceHDR] HDR 
            LEFT OUTER JOIN [District] D ON HDR.DistrictID=D.DistrictID
            WHERE (HDR.[InvoiceNumber] = @InvoiceNumber)" 
            UpdateCommand="UPDATE [CPMInvoiceHDR] SET [ProgramYear] = @ProgramYear, [DistrictID] = @DistrictID, [PeriodStart] = @PeriodStart, [PeriodEnd] = @PeriodEnd, 
            [BreakfastTotal] = @BreakfastTotal, [LunchTotal] = @LunchTotal, [CPMRate] = @CPMRate, [AlaSales] = @AlaSales, [ACEquivRate] = @ACEquivRate, 
            [SpecialFunctionSales] = @SpecialFunctionSales, [USDACommUsage] = @USDACommUsage, [USDACommRcvd] = @USDACommRcvd, [DateCreated] = @DateCreated 
            WHERE [InvoiceNumber] = @InvoiceNumber">
            <DeleteParameters>
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter DbType="Date" Name="PeriodStart" />
                <asp:Parameter DbType="Date" Name="PeriodEnd" />
                <asp:Parameter Name="BreakfastTotal" Type="Int32" />
                <asp:Parameter Name="LunchTotal" Type="Int32" />
                <asp:Parameter Name="CPMRate" Type="Decimal" />
                <asp:Parameter Name="AlaSales" Type="Decimal" />
                <asp:Parameter Name="ACEquivRate" Type="Decimal" />
                <asp:Parameter Name="SpecialFunctionSales" Type="Decimal" />
                <asp:Parameter Name="USDACommUsage" Type="Decimal" />
                <asp:Parameter Name="USDACommRcvd" Type="Decimal" />
                <asp:Parameter DbType="Date" Name="DateCreated" />
            </InsertParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="HFInvoiceNumber" Name="InvoiceNumber" PropertyName="Value" Type="Int32" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter DbType="Date" Name="PeriodStart" />
                <asp:Parameter DbType="Date" Name="PeriodEnd" />
                <asp:Parameter Name="BreakfastTotal" Type="Int32" />
                <asp:Parameter Name="LunchTotal" Type="Int32" />
                <asp:Parameter Name="CPMRate" Type="Decimal" />
                <asp:Parameter Name="AlaSales" Type="Decimal" />
                <asp:Parameter Name="ACEquivRate" Type="Decimal" />
                <asp:Parameter Name="SpecialFunctionSales" Type="Decimal" />
                <asp:Parameter Name="USDACommUsage" Type="Decimal" />
                <asp:Parameter Name="USDACommRcvd" Type="Decimal" />
                <asp:Parameter DbType="Date" Name="DateCreated" />
                <asp:Parameter Name="InvoiceNumber" Type="Int32" />
            </UpdateParameters>

        </asp:SqlDataSource>
    </div>

</asp:Content>
