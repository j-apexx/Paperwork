<%@ Page Title="Import - Special Function Pay" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportSpecialFunctionPay.aspx.cs" Inherits="Paperwork.ImportSpecialFunctionPay" %>
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
                    <h5>Import Special Function Pay File</h5>  
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Select Program Year:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged"  ></asp:DropDownList>
                        </div>
                    </div>  
                    <asp:LinkButton ID="BtnImportFile" runat="server" CssClass="btn btn-primary" data-target="#myModal" CausesValidation="true" ValidationGroup="Import" OnClick="BtnImportFile_Click" >
                        <i class="fas fa-plus"></i>
                        Select Special Function Pay CSV File  
                    </asp:LinkButton>
                    <br />
                    <asp:ValidationSummary ID="ValidImportSummary" runat="server" DisplayMode="BulletList" HeaderText="Before Importing:" ValidationGroup="Import" CssClass="validators"/>

                    <asp:UpdatePanel ID="UpdatePanelOpexWrapper" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            Status:<br />
                            <asp:TextBox ID="TextBoxStatus" runat="server" TextMode="MultiLine" Rows="1" ReadOnly="true" Width="100%" ></asp:TextBox>

                            <div id="recordsImported">
                                <br />
                                Special Function Pay records: 
                                <br />

                                <asp:GridView ID="GridViewSpFnPay" runat="server" AutoGenerateColumns="False" DataKeyNames="Counter" DataSourceID="SqlDataSourceSpFnPay"
                                    CssClass="mGrid"  AllowSorting="true" ShowHeaderWhenEmpty="true" ShowHeader="true" EmptyDataText="There are no records." >
                                    <Columns>
                                        <asp:BoundField DataField="Counter" HeaderText="Counter" ReadOnly="True" SortExpression="Counter" Visible="false" />
                                        <asp:BoundField DataField="ProgramYear" HeaderText="Program Year" ReadOnly="True" SortExpression="ProgramYear"  />
                                        <asp:BoundField DataField="DistrictID" HeaderText="District ID" ReadOnly="True" SortExpression="DistrictID" />
                                        <asp:BoundField DataField="PayDate" HeaderText="SpFn Date" ReadOnly="False" SortExpression="PayDate" DataFormatString="{0:d}" ApplyFormatInEditMode="true" />
                                        <asp:BoundField DataField="SpFnAmount" HeaderText="SpFn Amount" SortExpression="SpFnAmount" ControlStyle-Width="80" />
                                        <asp:BoundField DataField="LastName" HeaderText="Last Name" SortExpression="LastName" ControlStyle-Width="150" HeaderStyle-Width="150" />
                                        <asp:BoundField DataField="FirstName" HeaderText="First Name" SortExpression="FirstName" ControlStyle-Width="150" HeaderStyle-Width="150" />
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
                                    <h4 class="modal-title">Import Payroll Excel File</h4>  
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
                                                        <asp:CheckBox ID="CB_VerboseErrors" runat="server" Text="Show detailed errors" Checked="true" />
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
        <asp:SqlDataSource ID="SqlDataSourceSpFnPay" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [SPFNPay] WHERE [Counter] = @Counter" 
            SelectCommand="SELECT * FROM [SPFNPay] WHERE ([ProgramYear] = @ProgramYear)" 
            UpdateCommand="UPDATE [SPFNPay] SET  [PayDate] = @PayDate, [SpFnAmount] = @SpFnAmount, [LastName] = @LastName, [FirstName] = @FirstName
            WHERE [Counter] = @Counter ">
            <DeleteParameters>
                <asp:Parameter Name="Counter" Type="Int64" />
            </DeleteParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="SpFnAmount" Type="Decimal" />
                <asp:Parameter Name="LastName" Type="String" />
                <asp:Parameter Name="FirstName" Type="String" />
                <asp:Parameter DbType="Date" Name="PayDate" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [ProgramYear] FROM [ProgramYear] ORDER BY [StartDate] DESC" >
        </asp:SqlDataSource>    
    </div>
    


</asp:Content>
