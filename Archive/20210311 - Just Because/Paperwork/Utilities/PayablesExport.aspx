<%@ Page Title="Payables Export" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PayablesExport.aspx.cs" Inherits="Paperwork.PayablesExport" %>
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
                    <h4 class="card-title text-uppercase" style="color:white;">Export Payables to Dynamics GP</h4>  
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Beginning Date:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:TextBox ID="TextBoxBeginningDate" runat="server" AutoCompleteType="Disabled" AutoPostBack="true" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqBeginningDate" runat="server" ControlToValidate="TextBoxBeginningDate" CssClass="validators" Display="Dynamic" 
                                ErrorMessage="Beginning Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Ending Date:
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="TextBoxEndingDate" runat="server" AutoCompleteType="Disabled" AutoPostBack="true"  ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqEndingDate" runat="server" ControlToValidate="TextBoxEndingDate" CssClass="validators" Display="Dynamic" 
                                ErrorMessage="Ending Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-3">
                            Included Program Types:
                        </div>
                        <div class="col-sm-9">
                            <asp:CheckBoxList ID="CheckBoxListProgTypes" runat="server" AutoPostBack="false" TextAlign="Right" 
                                DataSourceID="SqlDataSourceProgramTypes" DataTextField="ProgramType" DataValueField="ProgramType" RepeatColumns="6" 
                                RepeatDirection="Vertical" RepeatLayout="Table" CssClass="cblistPad" OnDataBound="CheckBoxListProgTypes_DataBound" >
                            </asp:CheckBoxList>
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
<%--                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Dynamics GP Batch to which the transactions will be added. The Batch will be created if necessary.
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="TextBoxBatchID" runat="server" AutoCompleteType="Disabled" MaxLength="15" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqBatchID" runat="server" ControlToValidate="TextBoxBatchID" CssClass="validators" Display="Dynamic" ErrorMessage="Batch ID is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    
                    <br />
                    <asp:LinkButton ID="BtnExportToGP" runat="server" CssClass="btn btn-primary"  CausesValidation="true" ValidationGroup="Import" >
                        <i class="fas fa-upload"></i>
                        Export to GP 
                    </asp:LinkButton>--%>
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
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceProgramTypes" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
         SelectCommand="SELECT DISTINCT ProgramType FROM (SELECT ProgramType FROM [ProgramTypeSetup]
            UNION ALL
            SELECT 'School Year' AS ProgramType
            UNION ALL
            SELECT ProgramType FROM [PURCHASES] WHERE ProgramYear=@ProgramYear AND
            DistrictID=@DistrictID AND PostDate>=@PostDateBeginning AND PostDate<=@PostDateEnding GROUP BY ProgramType) Types ORDER BY ProgramType">
            <SelectParameters>
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:SessionParameter Name="DistrictID"  SessionField="CurrentDistrict" Type="String" />
                <asp:ControlParameter Name="PostDateBeginning" ControlID="TextBoxBeginningDate" Type="DateTime" />
                <asp:ControlParameter Name="PostDateEnding" ControlID="TextBoxEndingDate" Type="DateTime" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
