<%@ Page Title="Setup Transmittal Options" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupTransmittalOptions.aspx.cs" Inherits="Paperwork.SetupTransmittalOptions" %>
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
            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            var PYearStart = $("[id*=HFProgYearStartDate]").val();
            var PYearEnd = $("[id*=HFProgYearEndDate]").val();

            $(function () {
                $("[id*=TextBoxEOpenPostDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });
            $(function () {
                $("[id*=TextBoxFOpenPostDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });
            $(function () {
                $("[id*=TextBoxEStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });
            $(function () {
                $("[id*=TextBoxFStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });
            $(function () {
                $("[id*=TextBoxEEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });
            $(function () {
                $("[id*=TextBoxFEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });



        }
    </script>
    <h2><%: Title %></h2>
    <p>
        Setup W/E dates allowed for transmittals. Create messages to display on the transmittals page.
    </p>
        
    
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5">
        <ProgressTemplate>
            <div class="loadingDiv" >
                <asp:Image CssClass="loadingImage" ID="ImageLoading" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="DataHdr" class="row">
                <div class="col">
                    <div class="messagealert" id="alert_container"></div>
                    <strong>Program Year:</strong>
                    <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                            DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                    <asp:HiddenField ID="HFProgYearStartDate" runat="server" />
                    <asp:HiddenField ID="HFProgYearEndDate" runat="server" />
                </div>
            </div>

            <div id="DataEntry" class="row">
                <div class="col-3">

                    <asp:GridView ID="GridViewPostDates" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,OpenPostDate" DataSourceID="SqlDataSourceWEDates"
                        CssClass="mGrid" Width="200px" OnRowCommand="GridViewRowCommand" OnRowDataBound="GridViewPostDates_RowDataBound"
                        ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:TemplateField HeaderText="W/E Dates" SortExpression="OpenPostDate" HeaderStyle-Width="150" ItemStyle-HorizontalAlign="Center" >
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBoxEOpenPostDate" runat="server" Text='<%# Bind("OpenPostDate","{0:d}") %>' Width="100" MaxLength="10" OnTextChanged="TextBoxEOpenPostDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ReqEOpenPostDate" runat="server" ControlToValidate="TextBoxEOpenPostDate" Display="Dynamic" ErrorMessage="W/E Date is required." 
                                        ForeColor="Red" ValidationGroup="Update" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="text-align:center">
                                        <asp:TextBox ID="TextBoxFOpenPostDate" runat="server" Width="100" MaxLength="10"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ReqFOpenPostDate" runat="server" ControlToValidate="TextBoxFOpenPostDate" Display="Dynamic" ErrorMessage="W/E Date is required." 
                                            ForeColor="Red" ValidationGroup="Insert" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <div style="min-width:60px;text-align:center">
                                        <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                CssClass="fa fa-trash-alt mGridLinkIconButton " ForeColor="Crimson"
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="min-width:60px;text-align:center;" >
                                        <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>


                </div>
                <div class="col-9">
                    Start Date: Date the message will begin to be displayed on the Transmittal Page. <br />
                    &nbsp;&nbsp;Start Date will be displayed on transmittal page.<br />
                    End Date: Last date the message will be displayed.<br />
                    <asp:GridView ID="GridViewBOMessages" runat="server" DataKeyNames="Counter,ProgramYear" DataSourceID="SqlDataSourceBOMsgs" AutoGenerateColumns="false" AllowSorting="true"
                        CssClass="mGrid" OnRowCommand="GridViewRowCommand" OnRowDataBound="GridViewBOMessages_RowDataBound" ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" >
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBoxEStartDate" runat="server" Text='<%# Bind("StartDate","{0:d}") %>' Width="80" MaxLength="10" AutoPostBack="true" OnTextChanged="TextBoxBOMessageTextChanged" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ReqEStartDate" runat="server" ControlToValidate="TextBoxEStartDate" Display="Dynamic" ErrorMessage="Start Date is required." 
                                        ForeColor="Red" ValidationGroup="UpdateMsg" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="text-align:center">
                                        <asp:TextBox ID="TextBoxFStartDate" runat="server" Width="80" MaxLength="10"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ReqFStartDate" runat="server" ControlToValidate="TextBoxFStartDate" Display="Dynamic" ErrorMessage="Start Date is required." 
                                            ForeColor="Red" ValidationGroup="InsertMsg" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="End Date" SortExpression="StartDate" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center" >
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBoxEEndDate" runat="server" Text='<%# Bind("EndDate","{0:d}") %>' Width="80" MaxLength="10" AutoPostBack="true" OnTextChanged="TextBoxBOMessageTextChanged" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ReqEEndDate" runat="server" ControlToValidate="TextBoxEEndDate" Display="Dynamic" ErrorMessage="End Date is required." 
                                        ForeColor="Red" ValidationGroup="UpdateMsg" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="text-align:center">
                                        <asp:TextBox ID="TextBoxFEndDate" runat="server" Width="80" MaxLength="10"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ReqFEndDate" runat="server" ControlToValidate="TextBoxFEndDate" Display="Dynamic" ErrorMessage="End Date is required." 
                                            ForeColor="Red" ValidationGroup="InsertMsg" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Dashboard Message"  ItemStyle-HorizontalAlign="Center" >
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBoxEBOMessage" runat="server" Text='<%# Bind("DashboardMessage") %>' CssClass="longTextBox" AutoPostBack="true" OnTextChanged="TextBoxBOMessageTextChanged"  ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ReqEBOMessage" runat="server" ControlToValidate="TextBoxEBOMessage" Display="Dynamic" ErrorMessage="Dashboard Message is required." 
                                        ForeColor="Red" ValidationGroup="UpdateMsg" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="text-align:center">
                                        <asp:TextBox ID="TextBoxFBOMessage" runat="server" CssClass="longTextBox"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ReqFBOMessage" runat="server" ControlToValidate="TextBoxFBOMessage" Display="Dynamic" ErrorMessage="Dashboard Message is required." 
                                            ForeColor="Red" ValidationGroup="InsertMsg" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Width="50">
                                <ItemTemplate>
                                    <div style="min-width:50px;text-align:center">
                                        <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                CssClass="fa fa-trash-alt mGridLinkIconButton " ForeColor="Crimson"
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="min-width:50px;text-align:center;" >
                                        <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="InsertMsg" />
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
                
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceWEDates" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_{0}"
                    OnInserting="SqlDataSourceWEDates_Inserting" OnInserted="SqlDataSourceGenericInserted" OnDeleted="SqlDataSourceGenericDeleted" OnUpdated="SqlDataSourceGenericUpdated"
                    DeleteCommand="DELETE FROM [PurchasesPostDates] WHERE [ProgramYear] = @old_ProgramYear AND [OpenPostDate]=@old_OpenPostDate" 
                    InsertCommand="INSERT INTO [PurchasesPostDates] ([ProgramYear],[OpenPostDate]) VALUES (@ProgramYear, @OpenPostDate)" 
                    SelectCommand="SELECT @ProgramYear ProgramYear, '1/1/1900' OpenPostDate UNION ALL SELECT [ProgramYear], [OpenPostDate] FROM [PurchasesPostDates] WHERE ProgramYear=@ProgramYear "
                    UpdateCommand="UPDATE [PurchasesPostDates] SET [OpenPostDate] = @OpenPostDate WHERE [ProgramYear] = @old_ProgramYear AND [OpenPostDate]=@old_OpenPostDate">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                    <InsertParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:Parameter Name="OpenPostDate" Type="DateTime" />
                    </InsertParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceBOMsgs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                    OnInserting="SqlDataSourceBOMsgs_Inserting" OnInserted="SqlDataSourceGenericInserted" OnDeleted="SqlDataSourceGenericDeleted" OnUpdated="SqlDataSourceGenericUpdated"
                    DeleteCommand="DELETE FROM [PurchasesBOMessages] WHERE [Counter] = @Counter" 
                    InsertCommand="INSERT INTO [PurchasesBOMessages] ([ProgramYear],[StartDate],[EndDate],[DashboardMessage]) VALUES (@ProgramYear, @StartDate, @EndDate, @DashboardMessage)" 
                    SelectCommand="SELECT @ProgramYear ProgramYear, '1/1/1900' StartDate, '1/1/1900' EndDate, '' DashboardMessage, 0 Counter UNION ALL 
                    SELECT [ProgramYear], [StartDate], [EndDate], [DashboardMessage], [Counter] FROM [PurchasesBOMessages] WHERE ProgramYear=@ProgramYear "
                    UpdateCommand="UPDATE [PurchasesBOMessages] SET [StartDate] = @StartDate, [EndDate]=@EndDate, [DashboardMessage]=@DashboardMessage WHERE [Counter] = @Counter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                    <InsertParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:Parameter Name="StartDate" Type="DateTime" />
                        <asp:Parameter Name="EndDate" Type="DateTime" />
                        <asp:Parameter Name="DashboardMessage" Type="String" />
                    </InsertParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
