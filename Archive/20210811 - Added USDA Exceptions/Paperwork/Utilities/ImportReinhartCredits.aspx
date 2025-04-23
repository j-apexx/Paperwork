<%@ Page Title="Import - Reinhart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportReinhartCredits.aspx.cs" Inherits="Paperwork.ImportReinhartCredits" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
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
                $("[id*=TBSearchDateStart]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            $(function () {
                $("[id*=TBSearchDateEnd]").datepicker({
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
                    <h5>Import Reinhart Credits</h5>  
                </div>  
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-3 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Select Program Year:
                        </div>
                        <div class="col-sm-9 " >
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" AutoPostBack="true"  ></asp:DropDownList>
                        </div>
                    </div>
                    <asp:LinkButton ID="BtnImportFile" runat="server" CssClass="btn btn-primary" data-target="#myModal" OnClick="BtnImportFile_Click">
                        <i class="fas fa-plus"></i>
                        Import Reinhart Credit File  
                    </asp:LinkButton>
                    <br />
                    <asp:UpdatePanel ID="UpdatePanelOpexWrapper" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            Status:<br />
                            <asp:TextBox ID="TextBoxStatus" runat="server" TextMode="MultiLine" Rows="1" ReadOnly="true" Width="100%" ></asp:TextBox>
                            <br /><br />
                            <div id="recordsImported">
                                Existing Records<br />
                                <asp:DropDownList ID="DDLDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true"  DataTextField="Desc" DataValueField="DistrictID" >
                                    <asp:ListItem Text="-All Districts-" Value="%" Selected="true"></asp:ListItem>
                                </asp:DropDownList> 
                                &nbsp;
                                Starting Date:<asp:TextBox ID="TBSearchDateStart" runat="server" AutoPostBack="true"></asp:TextBox>
                                &nbsp;
                                Ending Date:<asp:TextBox ID="TBSearchDateEnd" runat="server" AutoPostBack="true"></asp:TextBox>
                                <asp:HiddenField ID="HFCounterFilterStart" runat="server" />
                                <asp:HiddenField ID="HFCounterFilterEnd" runat="server" />
                                <asp:GridView ID="GridViewReinhartCreds" runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataKeyNames="Counter" 
                                    DataSourceID="SqlDataSourceReinCreds" AllowSorting="true" ShowHeaderWhenEmpty="true" ShowHeader="true" 
                                    PageSize="100" AllowPaging="true" PagerSettings-Mode="NumericFirstLast" PagerStyle-Width="100" PagerStyle-Font-Size="14" 
                                    PagerSettings-Position="TopAndBottom" PagerStyle-CssClass="mGridPager" >
                                    <Columns>
                                        <asp:BoundField DataField="DistrictID" HeaderText="District ID" ReadOnly="True" SortExpression="DistIDInt" />
                                        <asp:BoundField DataField="InvoiceDate" DataFormatString="{0:d}" HeaderText="Invoice Date" ReadOnly="True" SortExpression="InvoiceDate" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="Type" HeaderText="Type"  SortExpression="Type" ControlStyle-Width="30" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="Month" HeaderText="Month"  SortExpression="Month" ControlStyle-Width="50" ItemStyle-HorizontalAlign="Center"  />
                                        <asp:BoundField DataField="Total" DataFormatString="{0:F2}" HeaderText="Total" SortExpression="Total" ControlStyle-Width="100" ReadOnly="true" />
                                        <asp:BoundField DataField="Promotional" DataFormatString="{0:F2}" HeaderText="Promotional" SortExpression="Promotional" ControlStyle-Width="100" />
                                        <asp:BoundField DataField="Competitive" DataFormatString="{0:F2}" HeaderText="Competitive" SortExpression="Competitive" ControlStyle-Width="100" />
                                        <asp:BoundField DataField="NetOffInvoice" DataFormatString="{0:F2}" HeaderText="NetOffInvoice" SortExpression="NetOffInvoice"  ControlStyle-Width="100"/>
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
                                    <h4 class="modal-title">Import Reinhart Credits CSV File</h4>  
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
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceReinCreds" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [ReinhartCredits] WHERE [Counter] = @Counter" 
            SelectCommand="SELECT *, CAST(DistrictID AS INT) DistIDInt FROM [ReinhartCredits] WHERE Counter>@CounterStart AND Counter <=@CounterEnd AND ProgramYear=@ProgramYear " 
            UpdateCommand="UPDATE [ReinhartCredits] SET [Type] = @Type, [Month] = @Month, [Total] = (@Promotional + @Competitive + @NetOffInvoice), [Promotional] = @Promotional, [Competitive] = @Competitive, 
            [NetOffInvoice] = @NetOffInvoice WHERE [Counter] = @Counter"
            FilterExpression="DistrictID LIKE '{0}' AND InvoiceDate>='{1}' AND InvoiceDate<='{2}'">
            <DeleteParameters>
                <asp:Parameter Name="Counter" Type="Int64" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="Type" Type="String" />
                <asp:Parameter Name="Month" Type="Int16" />
                <asp:Parameter Name="Total" Type="Decimal" />
                <asp:Parameter Name="Promotional" Type="Decimal" />
                <asp:Parameter Name="Competitive" Type="Decimal" />
                <asp:Parameter Name="NetOffInvoice" Type="Decimal" />
                <asp:Parameter Name="Counter" Type="Int64" />
            </UpdateParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="HFCounterFilterStart" DbType="Int64" Name="CounterStart" PropertyName="value" DefaultValue="0" />
                <asp:ControlParameter ControlID="HFCounterFilterEnd" DbType="Int64" Name="CounterEnd" PropertyName="value" DefaultValue="0"  />
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <FilterParameters>
                <asp:ControlParameter ControlID="DDLDistricts" DbType="StringFixedLength" PropertyName="SelectedValue" DefaultValue="%" />
                <asp:ControlParameter ControlID="TBSearchDateStart" DbType="Date" PropertyName="Text" DefaultValue="1/1/1900" />
                <asp:ControlParameter ControlID="TBSearchDateEnd" DbType="Date" PropertyName="Text" DefaultValue="1/1/9999" />
            </FilterParameters>
        </asp:SqlDataSource>
    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [ProgramYear] FROM [ProgramYear] ORDER BY [StartDate] DESC" >
        </asp:SqlDataSource>    
    </div>


</asp:Content>
