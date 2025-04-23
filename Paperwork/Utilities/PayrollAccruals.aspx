<%@ Page Title="Payroll Accruals" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PayrollAccruals.aspx.cs" Inherits="Paperwork.PayrollAccruals" %>
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
    <div id="dataUpload">
        <div class="container py-3">  
            <div class="card">  
                <div class="card-header bg-primary">  
                    <h4 class="card-title text-uppercase" style="color:white;">Update PayTax Accruals</h4>  
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Beginning Date:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:TextBox ID="TextBoxBeginningDate" runat="server" AutoCompleteType="Disabled" AutoPostBack="true" OnTextChanged="TextBoxDate_TextChanged" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqBeginningDate" runat="server" ControlToValidate="TextBoxBeginningDate" CssClass="validators" Display="Dynamic" 
                                ErrorMessage="Beginning Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Ending Date:
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="TextBoxEndingDate" runat="server" AutoCompleteType="Disabled" AutoPostBack="true" OnTextChanged="TextBoxDate_TextChanged" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqEndingDate" runat="server" ControlToValidate="TextBoxEndingDate" CssClass="validators" Display="Dynamic" 
                                ErrorMessage="Ending Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Calculated Accrual Days:
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="TextBoxAccrualDaysCount" runat="server" TextMode="Number"></asp:TextBox>
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
</asp:Content>
