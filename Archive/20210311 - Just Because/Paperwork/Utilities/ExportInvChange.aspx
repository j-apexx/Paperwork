<%@ Page Title="Inventory Change Export" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExportInvChange.aspx.cs" Inherits="Paperwork.ExportInvChange" %>
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
                    <h4 class="card-title text-uppercase" style="color:white;">Export District Inventory Changes</h4>  
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Select the Inventory to use for Open Values:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:DropDownList ID="DDLStartInventory" runat="server" DataSourceID="SqlDataSourceInventoryYearMonths" DataTextField="YearMonth" 
                            DataValueField="YearMonth" AutoPostBack="true" ></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Select the Inventory Close:
                        </div>
                        <div class="col-sm-4">
                            <asp:DropDownList ID="DDLEndInventory" runat="server" DataSourceID="SqlDataSourceInventoryYearMonths" DataTextField="YearMonth" 
                            DataValueField="YearMonth" AutoPostBack="true" ></asp:DropDownList>

                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8">
                            Export Path:
                            <asp:Label ID="LabelExportPath" runat="server" ></asp:Label>
                            <asp:Label ID="LabelOK" runat="server" ForeColor="Green" ToolTip="Access granted"><i class="fas fa-check-circle"></i></asp:Label>
                            <asp:Label ID="LabelBad" runat="server" ForeColor="Red" ToolTip="Access denied"><i class="fas fa-exclamation-circle"></i></asp:Label>
                        </div>
                    </div>
                    <asp:LinkButton ID="BtnExportToFile" runat="server" CssClass="btn btn-primary" CausesValidation="true" ValidationGroup="Import" OnClick="BtnExportToFile_Click" >
                        <i class="fas fa-upload"></i>
                        Export to Text File 
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
        <asp:SqlDataSource ID="SqlDataSourceInventoryYearMonths" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="GenerateInventorySelectors" SelectCommandType="StoredProcedure" >
            <SelectParameters>
                <asp:SessionParameter SessionField="ProgramYear" Name="ProgramYear" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
