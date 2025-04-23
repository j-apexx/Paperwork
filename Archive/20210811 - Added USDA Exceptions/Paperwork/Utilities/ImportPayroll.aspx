<%@ Page Title="Import - Payroll" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportPayroll.aspx.cs" Inherits="Paperwork.ImportPayroll" %>
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

            /* Code required to update the file name in the upload box */
            $(".custom-file-input").on("change", function () {
                var fileName = $(this).val().split("\\").pop();
                $(this).siblings(".custom-file-label").html(fileName);
            });

            $(function () {
                $("[id*=TextBoxBeginningDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $("[id*=TextBoxEndingDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id*=PayrollDateFilter]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
        };
    </script>
    <div id="dataUpload">
        <div class="container py-3">  
            <div class="card">  
                <div class="card-header bg-primary text-uppercase text-white">  
                    <h5>Import Paycom Payroll File</h5>  
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Select Program Year:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" AutoPostBack="true"  ></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Payroll Beginning Date:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:TextBox ID="TextBoxBeginningDate" runat="server" AutoCompleteType="Disabled" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqBeginningDate" runat="server" ControlToValidate="TextBoxBeginningDate" CssClass="validators" Display="Dynamic" 
                                ErrorMessage="Beginning Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Payroll Ending Date:
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="TextBoxEndingDate" runat="server" AutoCompleteType="Disabled" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqEndingDate" runat="server" ControlToValidate="TextBoxEndingDate" CssClass="validators" Display="Dynamic" 
                                ErrorMessage="Ending Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <asp:LinkButton ID="BtnImportFile" runat="server" CssClass="btn btn-primary" data-target="#myModal" CausesValidation="true" ValidationGroup="Import" OnClick="BtnImportFile_Click" >
                        <i class="fas fa-plus"></i>
                        Select Paycom CSV File  
                    </asp:LinkButton>
                    <br />
                    <asp:ValidationSummary ID="ValidImportSummary" runat="server" DisplayMode="BulletList" HeaderText="Before Importing:" ValidationGroup="Import" CssClass="validators"/>

                    <asp:UpdatePanel ID="UpdatePanelOpexWrapper" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            Status:<br />
                            <asp:TextBox ID="TextBoxStatus" runat="server" TextMode="MultiLine" Rows="1" ReadOnly="true" Width="100%" ></asp:TextBox>

                            <div id="recordsImported">
                                <br />
                                Select Payroll Ending Date to view those records: <asp:TextBox ID="PayrollDateFilter" runat="server" AutoPostBack="true" Width="100" ></asp:TextBox>
                                <asp:GridView ID="GridViewPayTax" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,EndDate" DataSourceID="SqlDataSourcePayIns"
                                    CssClass="mGrid"  AllowSorting="true" ShowHeaderWhenEmpty="true" ShowHeader="true" EmptyDataText="There are no records on this date." >
                                    <Columns>
                                        <asp:BoundField DataField="ProgramYear" HeaderText="Program Year" ReadOnly="True" SortExpression="ProgramYear"  />
                                        <asp:BoundField DataField="DistrictID" HeaderText="District ID" ReadOnly="True" SortExpression="DistrictID" />
                                        <asp:BoundField DataField="EndDate" HeaderText="EndDate" ReadOnly="True" SortExpression="EndDate" DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="StartDate" HeaderText="StartDate" SortExpression="StartDate" DataFormatString="{0:d}" ReadOnly="true" />
                                        <asp:BoundField DataField="Gross" HeaderText="Gross" SortExpression="Gross" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="Hours" HeaderText="Hours" SortExpression="Hours" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="ProdHrs" HeaderText="ProdHrs" SortExpression="ProdHrs" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="NonProdHrs" HeaderText="NonProdHrs" SortExpression="NonProdHrs" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="SpecFunctionHrs" HeaderText="SpecFunctionHrs" SortExpression="SpecFunctionHrs" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="FUTA" HeaderText="FUTA" SortExpression="FUTA" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="Medicare" HeaderText="Medicare" SortExpression="Medicare" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="PASUI" HeaderText="PASUI" SortExpression="PASUI" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="OHSUI" HeaderText="OHSUI" SortExpression="OHSUI" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="SocSec" HeaderText="SocSec" SortExpression="SocSec" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="Uniforms" HeaderText="Uniforms" SortExpression="Uniforms" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="PAPitC" HeaderText="PAPitC" SortExpression="PAPitC" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="PayAccrual" HeaderText="PayAccrual" SortExpression="PayAccrual" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="TaxAccrual" HeaderText="TaxAccrual" SortExpression="TaxAccrual" ControlStyle-Width="80" />
                                        <asp:TemplateField HeaderStyle-Width="165">
                                            <ItemTemplate>
                                                <div style="min-width:165px;" >
                                                    <asp:LinkButton runat="server" CommandName="Edit"
                                                        ID="IEdit" ToolTip="Edit" Text=" Edit"
                                                        CssClass="fas fa-pencil-alt mGridLinkButton" ForeColor="#336699"    />
                                                    <asp:LinkButton runat="server" CommandName="Delete"
                                                        ID="IDelete" ToolTip="Delete" Text=" Delete"
                                                        CssClass="fa fa-trash-alt mGridLinkButton" ForeColor="Crimson"/>
                                                </div>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <div style="min-width:165px;">
                                                    <asp:LinkButton runat="server" CommandName="Update"
                                                        ID="ISave" ToolTip="Save" Text=" Save" TabIndex="21"
                                                        CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="GridView" />
                                                    <asp:LinkButton runat="server" CommandName="Cancel"
                                                        ID="ICancel" ToolTip="Cancel" Text=" Cancel" TabIndex="21"
                                                        CssClass="fas fa-undo-alt mGridLinkButton" ForeColor="Red" CausesValidation="false" />
                                                </div>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <div class="modal fade" id="myModal">  
                        <div class="modal-dialog" >  
                            <div class="modal-content" style="width:800px;" > 
                                <div class="modal-header">  
                                    <h4 class="modal-title">Import Paycom Excel File</h4>  
                                    <button type="button" class="close" data-dismiss="modal">×</button>   
                                </div>  
                                <div class="modal-body">  
                                    <div class="row">  
                                        <div class="col-md-12">  
                                            <div class="form-group">  
                                                <label>Choose excel file</label>  
                                                <div class="input-group">  
                                                    <div class="custom-file">  
                                                        <asp:FileUpload ID="FileUpload1" CssClass="custom-file-input" runat="server"  />  
                                                        <label class="custom-file-label"></label>  
                                                    </div>  
                                                    <label id="filename"></label>  
                                                    <%--<div class="input-group-append">  
                                                        <asp:Button ID="btnUpload" runat="server" CssClass="btn btn-outline-primary" Text="Upload" OnClick="btnUpload_Click" />  
                                                    </div>  --%>
                                                    <div style="padding-top:10px;padding-left:3px;">
                                                        <asp:CheckBox ID="CB_VerboseErrors" runat="server" Text="Show detailed errors" />
                                                    </div>
                                                </div>  
                                                 
                                            </div>  
                                        </div>  
                                    </div>  
                                </div>  
                                <div class="modal-footer">  
                                    <asp:Button ID="ButtonUpld" runat="server" CssClass="btn btn-outline-primary" Text="Upload" OnClick="btnUpload_ClickCSV" /> 
                                    <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Close</button>  
                                </div>  
                            </div>  
                        </div>  
                    </div>  
                </div>  
            </div>  
        </div>  

    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourcePayIns" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [PayTax] WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND [EndDate] = @EndDate" 
            InsertCommand="INSERT INTO [PayTax] ([ProgramYear], [DistrictID], [EndDate], [StartDate], [Gross], [Hours], [ProdHrs], [NonProdHrs], 
            [SpecFunctionHrs], [FUTA], [Medicare], [PASUI], [OHSUI], [SocSec], [Uniforms], [PAPitC], [PayAccrual], [TaxAccrual]) 
            VALUES (@ProgramYear, @DistrictID, @EndDate, @StartDate, @Gross, @Hours, @ProdHrs, @NonProdHrs, @SpecFunctionHrs, @FUTA, @Medicare, @PASUI, 
            @OHSUI, @SocSec, @Uniforms, @PAPitC, @PayAccrual, @TaxAccrual)" 
            SelectCommand="SELECT * FROM [PayTax] WHERE (([ProgramYear] = @ProgramYear) AND ([EndDate] = @EndDate))" 
            UpdateCommand="UPDATE [PayTax] SET  [Gross] = @Gross, [Hours] = @Hours, [ProdHrs] = @ProdHrs, [NonProdHrs] = @NonProdHrs, 
            [SpecFunctionHrs] = @SpecFunctionHrs, [FUTA] = @FUTA, [Medicare] = @Medicare, [PASUI] = @PASUI, [OHSUI] = @OHSUI, [SocSec] = @SocSec, [Uniforms] = @Uniforms, 
            [PAPitC] = @PAPitC, [PayAccrual] = @PayAccrual, [TaxAccrual] = @TaxAccrual WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND [EndDate] = @EndDate">
            <DeleteParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter DbType="Date" Name="EndDate" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter DbType="Date" Name="EndDate" />
                <asp:Parameter DbType="Date" Name="StartDate" />
                <asp:Parameter Name="Gross" Type="Decimal" />
                <asp:Parameter Name="Hours" Type="Decimal" />
                <asp:Parameter Name="ProdHrs" Type="Decimal" />
                <asp:Parameter Name="NonProdHrs" Type="Decimal" />
                <asp:Parameter Name="SpecFunctionHrs" Type="Decimal" />
                <asp:Parameter Name="FUTA" Type="Decimal" />
                <asp:Parameter Name="Medicare" Type="Decimal" />
                <asp:Parameter Name="PASUI" Type="Decimal" />
                <asp:Parameter Name="OHSUI" Type="Decimal" />
                <asp:Parameter Name="SocSec" Type="Decimal" />
                <asp:Parameter Name="Uniforms" Type="Decimal" />
                <asp:Parameter Name="PAPitC" Type="Decimal" />
                <asp:Parameter Name="PayAccrual" Type="Decimal" />
                <asp:Parameter Name="TaxAccrual" Type="Decimal" />
            </InsertParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="PayrollDateFilter" DbType="Date" Name="EndDate" PropertyName="Text" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="Gross" Type="Decimal" />
                <asp:Parameter Name="Hours" Type="Decimal" />
                <asp:Parameter Name="ProdHrs" Type="Decimal" />
                <asp:Parameter Name="NonProdHrs" Type="Decimal" />
                <asp:Parameter Name="SpecFunctionHrs" Type="Decimal" />
                <asp:Parameter Name="FUTA" Type="Decimal" />
                <asp:Parameter Name="Medicare" Type="Decimal" />
                <asp:Parameter Name="PASUI" Type="Decimal" />
                <asp:Parameter Name="OHSUI" Type="Decimal" />
                <asp:Parameter Name="SocSec" Type="Decimal" />
                <asp:Parameter Name="Uniforms" Type="Decimal" />
                <asp:Parameter Name="PAPitC" Type="Decimal" />
                <asp:Parameter Name="PayAccrual" Type="Decimal" />
                <asp:Parameter Name="TaxAccrual" Type="Decimal" />
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter DbType="Date" Name="EndDate" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [ProgramYear] FROM [ProgramYear] ORDER BY [StartDate] DESC" >
        </asp:SqlDataSource>    
    </div>
    


</asp:Content>
