<%@ Page Title="CEP Recalculation Utility" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CEPRecalcUtil.aspx.cs" Inherits="Paperwork.CEPRecalcUtil" %>
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
                    <h4 class="card-title text-uppercase" style="color:white;">Recalculate CEP Counts</h4>  
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Program Year:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                            DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            District:
                        </div>
                        <div class="col-sm-4">
                            <asp:DropDownList ID="DDLDistricts" runat="server" DataSourceID="SqlDataSourceDistricts" DataTextField="Desc" 
                                DataValueField="DistrictID" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>
                    
                    <div class="row">
                        <div class="col-sm-8">
                            &nbsp;
                        </div>
                        <div class="col-sm-4" style="padding-top:5px;">
                            <asp:CheckBox ID="CBActiveOnly" runat="server" Text=" Display Inactive Districts" Checked="false" AutoPostBack="true" OnCheckedChanged="CBActiveOnly_CheckedChanged"/>
                        </div>
                    </div>
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


                    <asp:LinkButton ID="BtnProcess" runat="server" CssClass="btn btn-primary" CausesValidation="true" ValidationGroup="Import" OnClick="BtnProcess_Click" >
                        <i class="fas fa-cogs"></i>
                        Recalculate CEP
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
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OnFiltering="SqlDataSourceDistricts_Filtering"
            SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc', Inactive
            FROM [District] 
            ORDER BY [Desc]"
            FilterExpression="Inactive<={0}">
         <FilterParameters>
             <asp:ControlParameter ControlID="CBActiveOnly" PropertyName="Checked" Type="SByte" Name="Active" />
         </FilterParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
