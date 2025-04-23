<%@ Page Title="Import - Insurance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportInsurance.aspx.cs" Inherits="Paperwork.ImportInsurance" %>
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
                $("[id*=InsDateFilter]").datepicker({
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
                    <h5>Import Insurance</h5>  
                </div>  
                <div class="card-body">  
                    <asp:LinkButton ID="BtnImportFile" runat="server" CssClass="btn btn-primary" data-target="#myModal" OnClick="BtnImportFile_Click">
                        <i class="fas fa-plus"></i>
                        Select Insurance File  
                    </asp:LinkButton>
                    <br />
                    <asp:UpdatePanel ID="UpdatePanelOpexWrapper" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            Status:<br />
                            
                            <asp:TextBox ID="TextBoxStatus" runat="server" TextMode="MultiLine" Rows="1" ReadOnly="true" Width="100%" ></asp:TextBox>

                            <div id="recordsImported">
                                <br />
                                Select Insurance Date to view those records: 
                                
                                <asp:TextBox ID="InsDateFilter" runat="server" AutoPostBack="true" Width="100" ></asp:TextBox>
                                <asp:GridView ID="GridViewPayInsRecords" runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,InsuranceDate" DataSourceID="SqlDataSourcePayIns"
                                     AllowSorting="true" ShowHeaderWhenEmpty="true" ShowHeader="true" Width="500" EmptyDataText="There are no records on this date.">
                                    <Columns>
                                        <asp:BoundField DataField="DistrictID" HeaderText="District ID" ReadOnly="True" SortExpression="DistrictID" />
                                        <asp:BoundField DataField="InsuranceDate" DataFormatString="{0:d}" HeaderText="Insurance Date" ReadOnly="True" SortExpression="InsuranceDate" />
                                        <asp:BoundField DataField="Amount" DataFormatString="{0:F2}" HeaderText="Amount" SortExpression="Amount" />
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
                                    <h4 class="modal-title">Import Insurance Delimited File</h4>  
                                    <button type="button" class="close" data-dismiss="modal">×</button>  
                                </div>  
                                <div class="modal-body">  
                                    <div class="row">  
                                        <div class="col-md-12">  
                                            <div class="form-group">  
                                                <label>Choose import file</label>  
                                                <div class="input-group">  
                                                    <div class="custom-file">  
                                                        <asp:FileUpload ID="FileUpload1" CssClass="custom-file-input" runat="server"  />  
                                                        <label class="custom-file-label"></label>  
                                                    </div>  
                                                    &nbsp;
                                                    <div>
                                                        <asp:CheckBox ID="CB_VerboseErrors" runat="server" Text="Show detailed errors" />
                                                    </div>
                                                </div>  
                                            </div>  
                                        </div>  
                                    </div>  
                                </div>  
                                <div class="modal-footer">  
                                    <asp:Button ID="ButtonUpld" runat="server" CssClass="btn btn-outline-primary" Text="Upload" OnClick="btnUpload_Click" /> 
                                    <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Close</button>  
                                </div>  
                            </div>  
                        </div>  
                    </div>  

                    
                </div>  
            </div>  
        </div>  

    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourcePayIns" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT * FROM [PayIns] WHERE ProgramYear=@ProgramYear AND InsuranceDate=@InsuranceDate" 
            DeleteCommand="DELETE [PayIns] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InsuranceDate=@InsuranceDate"
            UPDATECOMMAND="UPDATE [PAYINS] Set [Amount]=@Amount WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND InsuranceDate=@InsuranceDate"
            >
            <SelectParameters>
               <asp:SessionParameter SessionField="ProgramYear" DbType="String" Name="ProgramYear" />
               <asp:ControlParameter ControlID="InsDateFilter" DbType="Date" Name="InsuranceDate" PropertyName="Text" />
            </SelectParameters>

        </asp:SqlDataSource>
    </div>

</asp:Content>
