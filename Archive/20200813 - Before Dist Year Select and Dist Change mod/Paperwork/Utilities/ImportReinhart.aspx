<%@ Page Title="Import - Reinhart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportReinhart.aspx.cs" Inherits="Paperwork.ImportReinhart" %>
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
            $(function () {
                $("[id*=TextBoxPostingDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id*=TextBoxMonthEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

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
        };
    </script>
    <div id="dataUpload">
        <div class="container py-3">  
            <div class="card">  
                <div class="card-header bg-primary text-uppercase text-white">  
                    <h5>Import Reinhart Invoices</h5>  
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Enter the Date to post the invoices to:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:TextBox ID="TextBoxPostingDate" runat="server" AutoCompleteType="Disabled" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqPostingDate" runat="server" ControlToValidate="TextBoxPostingDate" CssClass="validators" Display="Dynamic" ErrorMessage="Posting Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-8 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            At month end in a split week enter the later date to post invoices to.  Otherwise enter the same data as above.  Invoices dated after the above date will post to this date.
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="TextBoxMonthEndDate" runat="server" AutoCompleteType="Disabled" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ReqMonthEnd" runat="server" ControlToValidate="TextBoxMonthEndDate" CssClass="validators" Display="Dynamic" ErrorMessage="Split Week Posting Date is required." ForeColor="Red" ValidationGroup="Import" EnableClientScript="false">*</asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <br />
                    <asp:LinkButton ID="BtnImportFile" runat="server" CssClass="btn btn-primary" data-target="#myModal" CausesValidation="true" ValidationGroup="Import" OnClick="BtnImportFile_Click">
                        <i class="fas fa-plus"></i>
                        Import Reinhart File  
                    </asp:LinkButton>
                    <br />
                    <asp:ValidationSummary ID="ValidImportSummary" runat="server" DisplayMode="BulletList" HeaderText="Before Importing:" ValidationGroup="Import" CssClass="validators"/>

                    <asp:UpdatePanel ID="UpdatePanelOpexWrapper" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            Status:<br />
                            <asp:TextBox ID="TextBoxStatus" runat="server" TextMode="MultiLine" Rows="1" ReadOnly="true" Width="100%" ></asp:TextBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="modal fade" id="myModal">  
                        <div class="modal-dialog" >  
                            <div class="modal-content" style="width:800px;" > 
                                <div class="modal-header">  
                                    <h4 class="modal-title">Import Reinhart CSV File</h4>  
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


</asp:Content>
