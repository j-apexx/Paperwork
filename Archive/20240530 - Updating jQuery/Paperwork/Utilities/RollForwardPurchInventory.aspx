<%@ Page Title="Roll Forward Purchased Inventory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RollForwardPurchInventory.aspx.cs" Inherits="Paperwork.RollForwardPurchInventory" %>
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
                $( "[id*=TextBoxBeginningDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $( "[id*=TextBoxEndingDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
        };
    </script>
    <div id="Utility">
        <div class="container py-3">  
            <div class="card">  
                <div class="card-header bg-primary">  
                    <h4 class="card-title text-uppercase" style="color:white;">Purchased Inventory Utility</h4>  
                    <h5 style="color:white;">Use this utility to roll forward purchased inventory from source year/month to destination year/month</h5>
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Source Program Year:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:DropDownList ID="DDLProgramYearsSrc" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear"  ></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Source Year-Month:
                        </div>
                        <div class="col-sm-4">
                            <asp:CheckBox ID="CBOpenSrc" runat="server" Text="Open" OnCheckedChanged="CBOpen_CheckedChanged" AutoPostBack="true" />
                            <asp:CheckBox ID="CBCloseSrc" runat="server" Text="Close" OnCheckedChanged="CBClose_CheckedChanged" AutoPostBack="true" />
                            <asp:TextBox ID="TextBoxYearmonthSrc" runat="server" Width="60" OnTextChanged="TextboxYearMonth_TextChanged" AutoPostBack="true" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqTrxMonthSrc" runat="server" ControlToValidate="TextBoxYearmonthSrc" CssClass="validators" 
                                Display="Dynamic" ErrorMessage="Source Inventory Month is required." ValidationGroup="Insert"  >*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegexTrxMonthSrc" runat="server" ControlToValidate="TextBoxYearmonthSrc" CssClass="validators" ValidationExpression="([\d]){4}-([\d]){2}|(\*OPEN\*)|(CLOSE)"
                                Display="Dynamic" ErrorMessage="Source Inventory Month is not formatted correctly (YYYY-MM)." ValidationGroup="Insert">*</asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Destination Program Year:
                        </div>
                        <div class="col-sm-4">
                            <asp:DropDownList ID="DDLProgramYearsDest" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear"  ></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Destination Year-Month:
                        </div>
                        <div class="col-sm-4">
                            <asp:CheckBox ID="CBOpenDest" runat="server" Text="Open" OnCheckedChanged="CBOpen_CheckedChanged" AutoPostBack="true" />
                            <asp:CheckBox ID="CBCloseDest" runat="server" Text="Close" OnCheckedChanged="CBClose_CheckedChanged" AutoPostBack="true" />
                            <asp:TextBox ID="TextBoxYearmonthDest" runat="server" Width="60" OnTextChanged="TextboxYearMonth_TextChanged" AutoPostBack="true" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqTrxMonthDest" runat="server" ControlToValidate="TextBoxYearmonthDest" CssClass="validators" 
                                Display="Dynamic" ErrorMessage="Destination Inventory Month is required." ValidationGroup="Insert"  >*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegexTrxMonthDest" runat="server" ControlToValidate="TextBoxYearmonthDest" CssClass="validators" ValidationExpression="([\d]){4}-([\d]){2}|(\*OPEN\*)|(CLOSE)"
                                Display="Dynamic" ErrorMessage="Destination Inventory Month is not formatted correctly (YYYY-MM)." ValidationGroup="Insert">*</asp:RegularExpressionValidator>
                        </div>
                    </div>

                    <asp:LinkButton ID="BtnUpdateAccruals" runat="server" CssClass="btn btn-primary" CausesValidation="true" ValidationGroup="Import" OnClick="BtnUpdateAccruals_Click" >
                        <i class="fas fa-cogs"></i>
                        Update Accrual Totals 
                    </asp:LinkButton>
                    <br />
                    <asp:ValidationSummary ID="ValidImportSummary" runat="server" DisplayMode="BulletList" HeaderText="Before Importing:" ValidationGroup="Import" CssClass="validators"/>

                    <asp:UpdatePanel ID="UpdatePanelOpexWrapper" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            Status:<br />
                            <asp:TextBox ID="TextBoxStatus" runat="server" TextMode="MultiLine" Rows="1" ReadOnly="true" Width="100%" ></asp:TextBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>  
            </div>  
        </div>  
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
    </div>
</asp:Content>
