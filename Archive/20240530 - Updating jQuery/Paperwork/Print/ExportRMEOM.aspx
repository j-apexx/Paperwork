<%@ Page Title="RM EOM Export" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExportRMEOM.aspx.cs" Inherits="Paperwork.ExportRMEOM" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
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
                    <h4 class="card-title text-uppercase" style="color:white;">Regional Manager End of Month Report</h4>  
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-6 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Select Program Year:
                        </div>
                        <div class="col-sm-6 " >
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" AutoPostBack="true"  ></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Beginning Date:
                        </div>
                        <div class="col-sm-6 " >
                            <asp:TextBox ID="TextBoxBeginningDate" runat="server" AutoCompleteType="Disabled" AutoPostBack="True" OnTextChanged="TextBoxDate_TextChanged" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqBeginningDate" runat="server" ControlToValidate="TextBoxBeginningDate" CssClass="validators" Display="Dynamic" 
                                ErrorMessage="Beginning Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Ending Date:
                        </div>
                        <div class="col-sm-6">
                            <asp:TextBox ID="TextBoxEndingDate" runat="server" AutoCompleteType="Disabled" AutoPostBack="True" OnTextChanged="TextBoxDate_TextChanged" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqEndingDate" runat="server" ControlToValidate="TextBoxEndingDate" CssClass="validators" Display="Dynamic" 
                                ErrorMessage="Ending Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <strong>RM's:</strong>
                        </div>
                        <div class="col-sm-6">
                            <asp:DropDownList ID="DropDownListRMs" runat="server" AppendDataBoundItems="True" AutoPostBack="true" DataSourceID="SqlDataSourceRMs"
                                DataTextField="RMName" DataValueField="RMNumber"  >
                                <asp:ListItem Text="- Select -" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList> 
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <hr />
                        </div>
                    </div>
                    <asp:LinkButton ID="BtnExportToFile" runat="server" CssClass="btn btn-primary" CausesValidation="true" ValidationGroup="Import" OnClick="BtnExportToFile_Click" >
                        <i class="fas fa-download"></i>
                        Generate XLS File 
                    </asp:LinkButton>
                    <rsweb:ReportViewer ID="ReportViewer1" runat="server" ProcessingMode="Remote" 
                        CssClass="ReportViewerClass" Height="50px" ShowBackButton="False" 
                        ShowCredentialPrompts="False" SizeToReportContent="True" Width="100%" Visible="true" ShowDocumentMapButton="False" 
                        ShowPageNavigationControls="True" ZoomPercent="100" ShowParameterPrompts="False" ZoomMode="Percent" 
                        AsyncRendering="False" InteractivityPostBackMode="AlwaysSynchronous">
                        <ServerReport ReportPath="RMEOM(New)"  />
                    </rsweb:ReportViewer>
                    <br />

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
        <asp:SqlDataSource ID="SqlDataSourceRMs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RMSetup.RMName ,RMSetup.RMNumber FROM RMSetup
            UNION ALL
            SELECT '** All RMs **' RMName, 0 RMNumber
            ORDER BY RMSetup.RMName">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [ProgramYear] FROM [ProgramYear] ORDER BY [StartDate] DESC" >
        </asp:SqlDataSource>
    </div>
</asp:Content>
