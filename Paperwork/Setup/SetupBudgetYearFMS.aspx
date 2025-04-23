<%@ Page Title="Setup Budget Years (FMS)" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupBudgetYearFMS.aspx.cs" Inherits="Paperwork.SetupBudgetYearFMS" %>
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
                $( "[id*=TextBoxStartingDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true
                });
                $( "[id*=TextBoxEndingDate]" ).datepicker({
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <p>
                <asp:GridView ID="GridViewBudgetYears" runat="server" AutoGenerateColumns="False" DataKeyNames="BudgetYear" DataSourceID="SqlDataSourceBudgetYears" CssClass="mGrid" >
                    <Columns>
                        <asp:BoundField DataField="BudgetYear" HeaderText="Budget Year" ReadOnly="True" SortExpression="BudgetYear" />
                        <asp:BoundField DataField="BudgetDescription" HeaderText="Description" ReadOnly="True" SortExpression="BudgetDescription" />
                        <asp:BoundField DataField="StartingDate" DataFormatString="{0:d}" HeaderText="Start Date" SortExpression="StartingDate" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="EndingDate" DataFormatString="{0:d}" HeaderText="End Date" SortExpression="EndingDate" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                        <asp:TemplateField HeaderText="Allow New Budgets" SortExpression="AllowDataEntry">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBoxEntryAllowed" runat="server" Checked='<%# Eval("CB_AllowNewBudgets") %>' OnCheckedChanged="CBBudgets_CheckedChanged" AutoPostBack="true" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Allow Editing" SortExpression="AllowDataEntry">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBoxEditingAllowed" runat="server" Checked='<%# Eval("CB_AllowEditExisting") %>' OnCheckedChanged="CBBudgets_CheckedChanged" AutoPostBack="true" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                
            </p>
                <h4>New Budget Year:</h4>
            <p>
                <asp:DetailsView ID="DetailsViewInsertBudgetYear" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSourceBudgetYears" Height="50px" Width="350px" DefaultMode="Insert" >
                    <Fields>
                        <asp:TemplateField HeaderText="Budget Year" SortExpression="BudgetYear">
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBoxBudgetYear" runat="server" MaxLength="15" Text='<%# Bind("BudgetYear") %>' ValidationGroup="DetailsView"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorIY" runat="server" ControlToValidate="TextBoxBudgetYear" Display="Dynamic" ErrorMessage="Budget Year is required." ForeColor="Red" ValidationGroup="DetailsView">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Description" SortExpression="BudgetDescription">
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBoxBudgetDescription" runat="server" MaxLength="50" Text='<%# Bind("BudgetDescription") %>' ValidationGroup="DetailsView"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorDsc" runat="server" ControlToValidate="TextBoxBudgetDescription" Display="Dynamic" ErrorMessage="Budget Year Description is required." ForeColor="Red" ValidationGroup="DetailsView">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Start Date" SortExpression="StartingDate">
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBoxStartingDate" runat="server" MaxLength="10" Text='<%# Bind("StartingDate", "{0:d}") %>' ValidationGroup="DetailsView" Width="85px" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorSDate" runat="server" ControlToValidate="TextBoxStartingDate" Display="Dynamic" ErrorMessage="Start Date is Required" ForeColor="Red" ValidationGroup="DetailsView">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidatorSD" runat="server" ControlToValidate="TextBoxStartingDate" Display="Dynamic" ErrorMessage="Invalid Start Date" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="DetailsView">*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="End Date" SortExpression="EndingDate">
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBoxEndingDate" runat="server" MaxLength="10" Text='<%# Bind("EndingDate", "{0:d}") %>' ValidationGroup="DetailsView" Width="85px" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorEDate" runat="server" ControlToValidate="TextBoxEndingDate" Display="Dynamic" ErrorMessage="End Date is Required" ForeColor="Red" ValidationGroup="DetailsView">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidatorED" runat="server" ControlToValidate="TextBoxEndingDate" Display="Dynamic" ErrorMessage="End Date format invalid" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="DetailsView">*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                <asp:CompareValidator ID="CompareDates" runat="server" ControlToValidate="TextBoxEndingDate" ControlToCompare="TextBoxStartingDate" Display="Dynamic" ErrorMessage="Ending date needs to be after the Starting date" ForeColor="Red" ValidationGroup="DetailsView" Operator="GreaterThan" Type="Date">*Must be greater</asp:CompareValidator>
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
                    <asp:SqlDataSource ID="SqlDataSourceBudgetYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                        InsertCommand="INSERT INTO [FMSBudgetYear_Setup] ([BudgetYear],[BudgetDescription],[StartingDate],[EndingDate],[CB_AllowNewBudgets],[CB_AllowEditExisting]) 
                            VALUES (@BudgetYear, @BudgetDescription, @StartingDate, @EndingDate, 0, 0) " 
                        SelectCommand="SELECT * FROM [FMSBudgetYear_Setup] ORDER BY [StartingDate] DESC" 
                        >
                        <InsertParameters>
                            <asp:Parameter Name="BudgetYear" DbType="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        </InsertParameters>
                    </asp:SqlDataSource>
                </div>
            </p>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
