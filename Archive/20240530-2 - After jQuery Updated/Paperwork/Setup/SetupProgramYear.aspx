<%@ Page Title="Setup Program Years" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupProgramYear.aspx.cs" Inherits="Paperwork.SetupProgramYear" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        $( function() {
            BindEvents();
        } );

        function BindEvents() {
            $(function () {
                $( "[id*=TextBoxStartDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true
                });
                $( "[id*=TextBoxEndDate]" ).datepicker({
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
        };
    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <asp:Image CssClass="loadingImage" ID="ImageLoading" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <h2><%: Title %></h2>
    <p>
        Check a month box to close that month - no new transactions will be allowed in closed months or years.
    </p>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <p>
                <asp:GridView ID="GridViewProgramYears" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear" DataSourceID="SqlDataSourceProgramYears" CssClass="mGrid" >
                    <Columns>
                        <asp:BoundField DataField="ProgramYear" HeaderText="Program Year" ReadOnly="True" SortExpression="ProgramYear" />
                        <asp:BoundField DataField="StartDate" DataFormatString="{0:d}" HeaderText="Start Date" SortExpression="StartDate" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="EndDate" DataFormatString="{0:d}" HeaderText="End Date" SortExpression="EndDate" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                        <asp:TemplateField HeaderText="Active Year" SortExpression="AllowDataEntry">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBoxEntryAllowed" runat="server" Checked='<%# (Eval("AllowDataEntry").ToString())=="0"?false:true %>' OnCheckedChanged="CheckBoxEntryAllowed_CheckedChanged" AutoPostBack="true" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="JUL">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close07" runat="server" Checked='<%# (Eval("Close07").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="AUG">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close08" runat="server" Checked='<%# (Eval("Close08").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="SEP">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close09" runat="server" Checked='<%# (Eval("Close09").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="OCT">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close10" runat="server" Checked='<%# (Eval("Close10").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="NOV">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close11" runat="server" Checked='<%# (Eval("Close11").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="DEC">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close12" runat="server" Checked='<%# (Eval("Close12").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="JAN">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close01" runat="server" Checked='<%# (Eval("Close01").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="FEB">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close02" runat="server" Checked='<%# (Eval("Close02").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="MAR">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close03" runat="server" Checked='<%# (Eval("Close03").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="APR">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close04" runat="server" Checked='<%# (Eval("Close04").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="MAY">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close05" runat="server" Checked='<%# (Eval("Close05").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="JUN">
                            <ItemTemplate>
                                <asp:CheckBox ID="Close06" runat="server" Checked='<%# (Eval("Close06").ToString())=="0"?false:true %>' AutoPostBack="true" OnCheckedChanged="CheckBoxEntryMonthClosed_CheckedChanged" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Budget Year">
                            <ItemTemplate>
                                <asp:DropDownList ID="DDLEditBudgetYears" runat="server" DataSourceID="SqlDataSourceBudgets" DataTextField="DSCRIPTN" DataValueField="BudgetYear" AutoPostBack="true" OnSelectedIndexChanged="DDLEditBudgetYears_SelectedIndexChanged" SelectedValue='<%# Eval("BYEARTrimmed") %>' >
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                
            </p>
                <h4>New Program Year:</h4>
            <p>
                <asp:DetailsView ID="DetailsViewInsertProgramYear" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSourceProgramYears" Height="50px" Width="350px" DefaultMode="Insert" >
                    <Fields>
                        <asp:TemplateField HeaderText="Program Year" SortExpression="ProgramYear">
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBoxProgramYear" runat="server" MaxLength="15" Text='<%# Bind("ProgramYear") %>' ValidationGroup="DetailsView"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorIY" runat="server" ControlToValidate="TextBoxProgramYear" Display="Dynamic" ErrorMessage="Program Year is required." ForeColor="Red" ValidationGroup="DetailsView">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBoxStartDate" runat="server" MaxLength="10" Text='<%# Bind("StartDate", "{0:d}") %>' ValidationGroup="DetailsView" Width="85px" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorSDate" runat="server" ControlToValidate="TextBoxStartDate" Display="Dynamic" ErrorMessage="Start Date is Required" ForeColor="Red" ValidationGroup="DetailsView">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidatorSD" runat="server" ControlToValidate="TextBoxStartDate" Display="Dynamic" ErrorMessage="Invalid Start Date" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="DetailsView">*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBoxEndDate" runat="server" MaxLength="10" Text='<%# Bind("EndDate", "{0:d}") %>' ValidationGroup="DetailsView" Width="85px" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorEDate" runat="server" ControlToValidate="TextBoxEndDate" Display="Dynamic" ErrorMessage="End Date is Required" ForeColor="Red" ValidationGroup="DetailsView">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidatorED" runat="server" ControlToValidate="TextBoxEndDate" Display="Dynamic" ErrorMessage="End Date format invalid" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="DetailsView">*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                <asp:CompareValidator ID="CompareDates" runat="server" ControlToValidate="TextBoxEndDate" ControlToCompare="TextBoxStartDate" Display="Dynamic" ErrorMessage="Ending date needs to be after the Starting date" ForeColor="Red" ValidationGroup="DetailsView" Operator="GreaterThan" Type="Date">*Must be greater</asp:CompareValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Matching Budget Year" >
                            <InsertItemTemplate>
                                <asp:DropDownList ID="DDLBudgetYears" runat="server" DataSourceID="SqlDataSourceBudgets" DataTextField="DSCRIPTN" DataValueField="BudgetYear" SelectedValue='<%# Bind("BudgetYear") %>' >

                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <InsertItemTemplate>
                                <div style="text-align:center">
                                    <asp:LinkButton runat="server" CommandName="Insert"
                                        ID="ISave" ToolTip="Add" Text=" Add"
                                        CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="DetailsView" Width="100%" />
                                </div>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </p>
            <p>
                <asp:ValidationSummary ID="DetailsViewValidation" runat="server" ForeColor="Red" ValidationGroup="DetailsView" />
                <p>
                    <asp:Label ID="LabelInsertErrors" runat="server" ForeColor="Red" Height="40px"></asp:Label>
                </p>
                <div id="Datasources">
                    <asp:SqlDataSource ID="SqlDataSourceBudgets" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" DataSourceMode="DataReader" 
                        SelectCommand="SELECT '' BudgetYear, '-- Select Budget Year --' DSCRIPTN, 'ZZZZZZZZ' SORT
                        UNION ALL                        
                        SELECT RTRIM([BudgetYear]) BudgetYear, RTRIM([DSCRIPTN]) DSCRIPTN, [DSCRIPTN] SORT FROM [BDGT_BudgetYear_SETP] 
                        ORDER BY [SORT] DESC"></asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                        InsertCommand="INSERT INTO [ProgramYear] ([ProgramYear],[StartDate],[EndDate],[AllowDataEntry],[BudgetYear]) 
                            VALUES (@ProgramYear, @StartDate, @EndDate, 0, @BudgetYear) " 
                        SelectCommand="SELECT *, RTRIM([BudgetYear]) BYEARTrimmed FROM [ProgramYear] ORDER BY [StartDate] DESC" 
                        OnInserted="SqlDataSourceProgramYears_Inserted">
                        <InsertParameters>
                            <asp:Parameter Name="BudgetYear" DbType="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        </InsertParameters>
                    </asp:SqlDataSource>
                </div>
            </p>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
