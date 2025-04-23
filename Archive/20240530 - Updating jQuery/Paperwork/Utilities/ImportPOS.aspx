<%@ Page Title="Import - POS" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportPOS.aspx.cs" Inherits="Paperwork.ImportPOS" %>
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
                    <h5>Import POS</h5>  
                </div>  
                <div class="card-body">  
                    <strong>Program Year:</strong>
                        <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                            DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                        <br />
                        <asp:LinkButton ID="BtnImportFile" runat="server" CssClass="btn btn-primary" data-target="#myModal" OnClick="BtnImportFile_Click">
                            
                        <i class="fas fa-plus"></i>
                        Select POS Export File  
                    </asp:LinkButton>
                    <br />
                    <asp:UpdatePanel ID="UpdatePanelOpexWrapper" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            Status:<br />
                            
                            <asp:TextBox ID="TextBoxStatus" runat="server" TextMode="MultiLine" Rows="1" ReadOnly="true" Width="100%" ></asp:TextBox>

                            <div id="recordsImported">
                                <br />

                                
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="modal fade" id="myModal">  
                        <div class="modal-dialog" >  
                            <div class="modal-content" style="width:800px;" > 
                                <div class="modal-header">  
                                    <h4 class="modal-title">Import POS XLSX File</h4>  
                                    <button type="button" class="close" data-dismiss="modal">×</button>  
                                </div>  
                                <div class="modal-body">  
                                    <div class="row">  
                                        <div class="col-md-12">  
                                            <div class="form-group">  
                                                <label>Choose import file</label>  
                                                <div class="input-group">  
                                                    <div class="custom-file">  
                                                        <asp:FileUpload ID="FileUpload1" accept=".csv,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" CssClass="custom-file-input" runat="server"  />  
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
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
    </div>

</asp:Content>
