<%@ Page Title="Inventory Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InventoryEntry.aspx.cs" Inherits="Paperwork.InventoryEntry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        $(function () {
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

            //Ensure footer numeric insert textboxes have valid numbers
            $("[id*=TextBoxFFood],[id*=TextBoxFPaper],[id*=TextBoxFCleaning],[id*=TextBoxFMilk], [id *= TextBoxFUSDAProtOpen], [id *= TextBoxFUSDAProtRcvd], [id *= LabelFUSDAProtIUse], [id *= TextBoxFUSDAProtTrns], [id *= TextBoxFUSDAProtEnd], [id *= TextBoxFUSDAGrocOpen], [id *= TextBoxFUSDAGrocRcvd], [id *= TextBoxFUSDAGrocIUse], [id *= TextBoxFUSDAGrocTrns], [id *= TextBoxFUSDAGrocEnd]").keyup(function () {
                if (jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val()))) {
                    $(this).val('0.00');
                    $(this).select();
                }
            });

            //Specialized Date Picker
            $(function () {
                $('[id*=TextBoxFYearmonth]').datepicker(
                    {
                        dateFormat: "yy-mm",
                        changeMonth: true,
                        changeYear: true,
                        showButtonPanel: true,
                        minDate: new Date($('[id*=HFStartDate]').val()),
                        maxDate: new Date($('[id*=HFEndDate]').val()),
                        onClose: function (dateText, inst) {

                            function isDonePressed() {
                                return ($('#ui-datepicker-div').html().indexOf('ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all ui-state-hover') > -1);
                            }

                            if (isDonePressed()) {
                                var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                                var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                                $(this).datepicker('setDate', new Date(year, month, 1)).trigger('change');

                                $('.date-picker').focusout()//Added to remove focus from datepicker input box on selecting date
                            }
                        },
                        beforeShow: function (input, inst) {

                            inst.dpDiv.addClass('month_year_datepicker')

                            if ((datestr = $(this).val()).length > 0) {
                                year = datestr.substring(0, 4);
                                //year = datestr.substring(datestr.length - 4, datestr.length);
                                month = datestr.substring(datestr.length - 2, datestr.length);
                                //month = datestr.substring(0, 2);
                                $(this).datepicker('option', 'defaultDate', new Date(year, month - 1, 1));
                                $(this).datepicker('setDate', new Date(year, month - 1, 1));
                                //$(".ui-datepicker-calendar").css("display", "none");
                                //$('.date-picker').css("display", "none");
                            }
                        }
                    })
            });
        };

    </script>
    <style>
        .ui-datepicker-calendar {
            display: none;
        }
    </style>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"  >
        <ContentTemplate>
            <div id="topSection">
                <div id="hdrgroup">
                    <div id="hdrDiv" style="float:left;width:25%">
                        <h2><%: Title %></h2>
                        
                    </div>
                    <div id="menuDiv">
                        <div class="row">
                            <div class="col-sm-12">
                                <strong>Program Year:</strong>
                                <asp:Label ID="LabelProgramYear" runat="server"></asp:Label>
                                <asp:HiddenField ID="HFStartDate" runat="server" />
                                <asp:HiddenField ID="HFEndDate" runat="server"  />
                                &nbsp;&nbsp; 
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="DataEntry">
                <asp:GridView ID="GridViewInventory" runat="server" CssClass="mGrid" AutoGenerateColumns="False" DataSourceID="SqlDataSourceInventory" 
                    ShowHeader="true" ShowFooter="true" ShowHeaderWhenEmpty="true" DataKeyNames="ProgramYear, DistrictID, YearMonth" 
                    OnRowCommand="GridViewInventory_RowCommand" OnRowDataBound="GridViewInventory_RowDataBound" >
                    <Columns>
                        <asp:TemplateField HeaderText="Year-Month">
                            <EditItemTemplate>
                                <asp:Label ID="LabelEYearMonth" runat="server" Text='<%# Bind("YearMonth") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelYearMonth" runat="server" Text='<%# Eval("YearMonth") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div class="row">
                                    <div class="col-sm-12" style="padding-bottom:0px;margin-bottom:0px;text-wrap:none; width:200px;">

                                        <asp:CheckBox ID="CBFOpen" runat="server" Text="Open" OnCheckedChanged="CBOpen_CheckedChanged" AutoPostBack="true" />
                                        <asp:TextBox ID="TextBoxFYearmonth" runat="server" Width="60" OnTextChanged="TextboxYearMonth_TextChanged" ></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ReqTrxMonth" runat="server" ControlToValidate="TextBoxFYearmonth" CssClass="validators" 
                                            Display="Dynamic" ErrorMessage="Inventory Month is required." ValidationGroup="Insert"  >*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegexTrxMonth" runat="server" ControlToValidate="TextBoxFYearmonth" CssClass="validators" ValidationExpression="([\d]){4}-([\d]){2}|(\*OPEN\*)"
                                            Display="Dynamic" ErrorMessage="Inventory Month is not formatted correctly (YYYY-MM)." ValidationGroup="Insert">*</asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Food" SortExpression="Groceries">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEFood" runat="server" Text='<%# Bind("Groceries") %>' Width="70" ></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelFood" runat="server" Text='<%# Eval("Groceries") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFFood" runat="server" Width="70" Text="0.00"  ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Paper" SortExpression="Paper">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEPaper" runat="server" Text='<%# Bind("Paper") %>' Width="70" ></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelPaper" runat="server" Text='<%# Eval("Paper") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFPaper" runat="server"  Width="70" Text="0.00" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cleaning" SortExpression="Cleaning">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxECleaning" runat="server" Text='<%# Bind("Cleaning") %>' Width="70" ></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelCleaning" runat="server" Text='<%# Eval("Cleaning") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFCleaning" runat="server"  Width="70" Text="0.00" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Milk" SortExpression="Milk">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEMilk" runat="server" Text='<%# Bind("Milk") %>' Width="70" ></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelMilk" runat="server" Text='<%# Eval("Milk") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFMilk" runat="server"  Width="70" Text="0.00" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Protein Open" SortExpression="USDAProteinOpen">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEUSDAProtOpen" runat="server" Text='<%# Bind("USDAProteinOpen") %>' Width="70" ></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelUSDAProtOpen" runat="server" Text='<%# Eval("USDAProteinOpen") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFUSDAProtOpen" runat="server" Width="70" Text="0.00" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Protein Rcvd" SortExpression="USDAProteinRcvd">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEUSDAProtRcvd" runat="server" Text='<%# Bind("USDAProteinRcvd") %>' Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelUSDAProtRcvd" runat="server" Text='<%# Eval("USDAProteinRcvd") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFUSDAProtRcvd" runat="server" Text="0.00" Width="70"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Protein Inst Use" SortExpression="USDAProteinInstUse">
                            <EditItemTemplate>
                                <asp:Textbox ID="LabelEUSDAProtIUse" runat="server" Text='<%# Bind("USDAProteinInstUse") %>' Width="70"></asp:Textbox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelUSDAProtIUse" runat="server" Text='<%# Eval("USDAProteinInstUse") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Textbox ID="LabelFUSDAProtIUse" runat="server" Text="0.00" Width="70"></asp:Textbox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Protein Trans" SortExpression="USDAProteinTrans">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEUSDAProtTrns" runat="server" Text='<%# Bind("USDAProteinTrans") %>' Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="TextBoxUSDAProtTrns" runat="server" Text='<%# Eval("USDAProteinTrans") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                 <asp:TextBox ID="TextBoxFUSDAProtTrns" runat="server" Text="0.00"  Width="70"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Protein Ending" SortExpression="USDAProteinEnding">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEUSDAProtEnd" runat="server" Text='<%# Bind("USDAProteinEnding") %>' Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelUSDAProtEnd" runat="server" Text='<%# Eval("USDAProteinEnding") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFUSDAProtEnd" runat="server" Text="0.00" Width="70"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Grocery Open" SortExpression="USDAGroceryOpen">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEUSDAGrocOpen" runat="server" Text='<%# Bind("USDAGroceryOpen") %>' Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelUSDAGrocOpen" runat="server" Text='<%# Eval("USDAGroceryOpen") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFUSDAGrocOpen" runat="server" Text="0.00" Width="70"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Grocery Rcvd" SortExpression="USDAGroceryRcvd">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEUSDAGrocRcvd" runat="server" Text='<%# Bind("USDAGroceryRcvd") %>' Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelUSDAGrocRcvd" runat="server" Text='<%# Eval("USDAGroceryRcvd") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFUSDAGrocRcvd" runat="server" Text="0.00" Width="70"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Grocery Inst Use" SortExpression="USDAGroceryInstUse">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEUSDAGrocIUse" runat="server" Text='<%# Bind("USDAGroceryInstUse") %>' Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelUSDAGrocIUse" runat="server" Text='<%# Eval("USDAGroceryInstUse") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFUSDAGrocIUse" runat="server" Text="0.00" Width="70"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Grocery Trans" SortExpression="USDAGroceryTrans" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEUSDAGrocTrns" runat="server" Text='<%# Bind("USDAGroceryTrans") %>' Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelUSDAGrocTrns" runat="server" Text='<%# Eval("USDAGroceryTrans") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFUSDAGrocTrns" runat="server" Text="0.00" Width="70"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="USDA Grocery Ending" SortExpression="USDAGroceryEnding">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEUSDAGrocEnd" runat="server" Text='<%# Bind("USDAGroceryEnding") %>' Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelUSDAGrocEnd" runat="server" Text='<%# Eval("USDAGroceryEnding") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFUSDAGrocEnd" runat="server" Text="0.00" Width="70"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Comments" SortExpression="Comments">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEComments" runat="server" Text='<%# Bind("Comments") %>' ></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelComments" runat="server" Text='<%# Eval("Comments") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFComments" runat="server" Text="" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                    <div style="width:60px;text-align:center;" <%# (bool)Eval("Locked")==true?"hidden='hidden'":"" %> >
                                        <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="width:60px;text-align:center;"<%# (bool)Eval("Locked")==true?"hidden='hidden'":"" %> >
                                        <asp:LinkButton runat="server" CommandName="Edit" ID="IEdit" ToolTip="Edit" 
                                            CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  />
                                        <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                            CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                            OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="width:60px;text-align:center;" >
                                        <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <div class="messagealert" id="alert_container"></div>
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceInventory" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT [ProgramYear],[DistrictID],[YearMonth],[Groceries],[Paper],[Cleaning],[Milk],[USDAProteinOpen],[USDAProteinRcvd],[USDAProteinInstUse]
                    ,[USDAProteinTrans],[USDAProteinEnding],[USDAGroceryOpen],[USDAGroceryRcvd],[USDAGroceryInstUse],[USDAGroceryTrans],[USDAGroceryEnding] ,[Comments],[Locked] 
                    FROM [Inventory] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) )
                    UNION ALL
                    SELECT @ProgramYear,@DistrictID,''[YearMonth],0,0,0,0,0,0,0,0,0,0,0,0,0,0,'',CAST(0 AS bit) Locked"
                    DeleteCommand="DELETE [Inventory] WHERE ([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([YearMonth] = @YearMonth) AND [Locked]<>1 "
                    InsertCommand="INSERT INTO [Inventory] ([ProgramYear],[DistrictID],[YearMonth],[Groceries],[Paper],[Cleaning],[Milk],[USDAProteinOpen],[USDAProteinRcvd],[USDAProteinInstUse]
                    ,[USDAProteinTrans],[USDAProteinEnding],[USDAGroceryOpen],[USDAGroceryRcvd],[USDAGroceryInstUse],[USDAGroceryTrans],[USDAGroceryEnding] ,[Comments],[Locked]) 
                    VALUES (@ProgramYear,@DistrictID,@YearMonth,@Groceries,@Paper,@Cleaning,@Milk,@USDAProteinOpen,@USDAProteinRcvd,@USDAProteinInstUse
                    ,@USDAProteinTrans,@USDAProteinEnding,@USDAGroceryOpen,@USDAGroceryRcvd,@USDAGroceryInstUse,@USDAGroceryTrans,@USDAGroceryEnding,@Comments,0)"
                    UpdateCommand="UPDATE [Inventory] SET [Groceries]=@Groceries,[Paper]=@Paper,[Cleaning]=@Cleaning,[Milk]=@Milk,[USDAProteinOpen]=@USDAProteinOpen
                    ,[USDAProteinRcvd]=@USDAProteinRcvd,[USDAProteinInstUse]=@USDAProteinInstUse,[USDAProteinTrans]=@USDAProteinTrans,[USDAProteinEnding]=@USDAProteinEnding
                    ,[USDAGroceryOpen]=@USDAGroceryOpen,[USDAGroceryRcvd]=@USDAGroceryRcvd,[USDAGroceryInstUse]=@USDAGroceryInstUse,[USDAGroceryTrans]=@USDAGroceryTrans
                    ,[USDAGroceryEnding]=@USDAGroceryEnding ,[Comments]=@Comments 
                    WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID) AND ([YearMonth] = @YearMonth))" 
                    OnInserting="SqlDataSourceInventory_Inserting" OnInserted="SqlDataSourceInventory_Inserted" OnDeleted="SqlDataSourceInventory_Deleted" 
                    OnUpdated="SqlDataSourceInventory_Updated" >
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="YearMonth" Type="String" />
                    </DeleteParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="YearMonth" Type="String" />
                        <asp:Parameter Name="Groceries" Type="Decimal" />
                        <asp:Parameter Name="Milk" Type="Decimal" />
                        <asp:Parameter Name="Paper" Type="Decimal" />
                        <asp:Parameter Name="Cleaning" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinOpen" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinRcvd" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinInstUse" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinTrans" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinEnding" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryOpen" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryRcvd" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryInstUse" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryTrans" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryEnding" Type="Decimal" />
                        <asp:Parameter Name="Comments" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                    </UpdateParameters>
                    <InsertParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="YearMonth" Type="String" />
                        <asp:Parameter Name="Groceries" Type="Decimal" />
                        <asp:Parameter Name="Milk" Type="Decimal" />
                        <asp:Parameter Name="Paper" Type="Decimal" />
                        <asp:Parameter Name="Cleaning" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinOpen" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinRcvd" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinInstUse" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinTrans" Type="Decimal" />
                        <asp:Parameter Name="USDAProteinEnding" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryOpen" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryRcvd" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryInstUse" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryTrans" Type="Decimal" />
                        <asp:Parameter Name="USDAGroceryEnding" Type="Decimal" />
                        <asp:Parameter Name="Comments" Type="String" ConvertEmptyStringToNull="false" />
                    </InsertParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
