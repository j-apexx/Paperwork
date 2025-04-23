<%@ Page Title="Setup Menu Analysis Items" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupMenuAnalysisItems.aspx.cs" Inherits="Paperwork.SetupMenuAnalysisItems" %>
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

            //Enable Enter Key tabbing
            $('input,select,textarea').on("keypress", function (e) {
                /* ENTER PRESSED*/
                if (e.keyCode == 13) {

                    //This code moves between focusable elements. 
                    var $canfocus = $(':focusable').not("a[title|='Show All Items']");//"title", "Show All Items"
                    //var index = $canfocus.index(document.activeElement) + 1;
                    var index = $canfocus.index(this) + 1;

                    if (index >= $canfocus.length) index = 0;
                    $canfocus.eq(index).focus();//Handles Buttons
                    $canfocus.eq(index).select();//helps with dropdowns

                }
            });

            //Ensure footer numeric insert textboxes have valid numbers
            $("[id*=TextBoxECost],[id*=TextBoxFCost]").keyup(function () {
                if (jQuery.trim($(this).val()) != '.' && ( jQuery.trim($(this).val()) == '' || isNaN(parseFloat($(this).val())))) {
                    $(this).val('0.00');
                    $(this).select();
                }
            });

            $("[id*=TextBoxEDesc]").attr('maxlength', '50');
            $("[id*=TextBoxFDesc]").attr('maxlength', '50');

        };

        //apply highlight 
        $('body').on('focus', 'input,textarea,select', function (e) {
            $(this).addClass("highlight");
        });
        // remove highlight
        $('body').on('focusout', 'input,textarea,select', function (e) {
            $(this).removeClass("highlight");
        });

    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="row">
                    <div class="col-6">
                        <div class="row">
                            <div class="col">
                                <h2><%: Title %></h2>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-3">
                                <strong>Program Year:</strong>
                            </div>
                            <div class="col-9">
                                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-3">
                                <strong>District:</strong>
                            </div>
                            <div class="col-9">
                                    <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                        DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                        OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                                    </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-3">
                                <strong>Category:</strong>
                            </div>
                            <div class="col-9">
                                <asp:DropDownList ID="DDLCategories" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DDLCategories_SelectedIndexChanged" >
                                    <asp:ListItem Text="Breakfast Entree" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Lunch Entree" Value="2" ></asp:ListItem>
                                    <asp:ListItem Text="Vegetable Choices" Value="3" ></asp:ListItem>
                                    <asp:ListItem Text="Fruit Choices" Value="4" ></asp:ListItem>
                                    <asp:ListItem Text="Other Items" Value="5" ></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="row">
                            <div class="col">&nbsp;</div>
                        </div>
                        <div class="row">
                            <div class="col">
                                Enter all breakfast entrees, lunch entrees, fruits, vegetables offered throughout the 6 week cycle menu under each category
                                <br />
                                Then Enter the Cost of each item in the column labeled "Cost" to the right of each item name
                                <br />
                                Milk and Condiments are already entered under OTHER category
                            </div>
                        </div>
                    </div>
                </div>

                <asp:HiddenField ID="HFCatMidLower" runat="server" />
                <asp:HiddenField ID="HFCatMidUpper" runat="server" />

                <div class="row">
                    <div class="col">
                        <asp:GridView ID="GridViewItemCostSetup" CssClass="mGrid" runat="server" AutoGenerateColumns="False" 
                            DataKeyNames="ProgramYear,DistrictID,ItemCategoryID,ItemID" DataSourceID="SqlDataSourceMenuCostItemsSetup"
                            ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" Width="600" AllowSorting="true"
                            OnRowCommand="GridViewItemCostSetup_RowCommand" OnRowDataBound="GridViewItemCostSetup_RowDataBound" >
                            <Columns>
                                <asp:TemplateField HeaderText="Menu Item Description" SortExpression="ItemDescription">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBoxEDesc" runat="server" Text='<%# Bind("ItemDescription") %>' MaxLength="50" CssClass="longTextBox"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredDesc" runat="server" ControlToValidate="TextBoxEDesc" CssClass="validators"
                                         Display="Dynamic" ErrorMessage="Description is required" Text="*" ValidationGroup="EditItem"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="LabelDesc" runat="server" Text='<%# Eval("ItemDescription") %>' CssClass="longTextBox"></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="TextBoxFDesc" runat="server" MaxLength="50" CssClass="longTextBox"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFDesc" runat="server" ControlToValidate="TextBoxFDesc" CssClass="validators"
                                         Display="Dynamic" ErrorMessage="Description is required" Text="*" ValidationGroup="InsertItem"></asp:RequiredFieldValidator>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cost" SortExpression="ItemDescription">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBoxECost" runat="server" Text='<%# Bind("ItemCost", "{0:F2}") %>' Width="100"></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <div style="text-align:center;">
                                            <asp:Label ID="LabelCost" runat="server" Text='<%# Eval("ItemCost", "{0:F2}") %>' Width="100" ></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="TextBoxFCost" runat="server" Width="100"></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <EditItemTemplate>
                                        <div style="width:60px;text-align:center;" >
                                            <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                                CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="EditItem" />
                                            <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                                CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                        </div>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <div style="width:60px;text-align:center;"  >
                                            <asp:LinkButton runat="server" CommandName="Edit" ID="IEdit" ToolTip="Edit" 
                                                CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  />
                                            <asp:LinkButton runat="server" CommandName="Delete" ID="IDelete" ToolTip="Delete"
                                                CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                Visible='<%# (bool)Eval("InUse")==false %>'/>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <div style="width:60px;text-align:center;" >
                                            <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                                CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="InsertItem" />
                                            <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" 
                                                CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                        </div>
                                    </FooterTemplate>

                                    <ItemStyle Width="60" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        *click the enter key twice after cost to save the line and begin a new row
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        Use the box below to paste from excel and quickly build items. Copy the Menu item  and cost cells and paste below. Resize box as needed.
                    </div>

                </div>
                <div class="row">
                    <div class="col">
                        <asp:TextBox ID="TextBoxExcelPaste" runat="server" TextMode="MultiLine" Width="500" Height="250" ></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <asp:Button ID="btnSubmitPastedRecords" runat="server" OnClick="btnSubmitPastedRecords_Click" Text="Import Menu Records" />
                    </div>
                </div>



                <asp:ValidationSummary ID="SummaryEdit" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                    ValidationGroup="EditItem" HeaderText="Editing Errors" BackColor="WhiteSmoke" Width="500" />
                <asp:ValidationSummary ID="SummaryInsert" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                    ValidationGroup="InsertItem" HeaderText="Insert Errors" BackColor="WhiteSmoke" Width="500" />
                <div class="messagealert" id="alert_container"></div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceMenuCostItemsSetup" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
            DeleteCommand="DELETE FROM [MenuCostItemsSetup] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [ItemCategoryID]=@ItemCategoryID AND [ItemID]=@ItemID"
            InsertCommand="INSERT INTO [MenuCostItemsSetup] ([ProgramYear],[DistrictID],[ItemCategoryID],[ItemID],[ItemDescription],[ItemCost]) 
            VALUES (@ProgramYear,@DistrictID,@ItemCategoryID,
            (SELECT ISNULL(MAX(ItemID), 0)+1 FROM [MenuCostItemsSetup] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [ItemCategoryID]=@ItemCategoryID)
            ,dbo.InitCap(@ItemDescription),@ItemCost)"
            UpdateCommand="UPDATE [MenuCostItemsSetup] SET [ItemDescription]=dbo.InitCap(@ItemDescription), [ItemCost]=@ItemCost WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [ItemCategoryID]=@ItemCategoryID AND [ItemID]=@ItemID"
            OnInserted="SqlDataSourceMenuCostItemsSetup_Inserted" OnInserting="SqlDataSourceMenuCostItemsSetup_Inserting" OnDeleted="SqlDataSourceMenuCostItemsSetup_Deleted"

            SelectCommand="SELECT @ProgramYear [ProgramYear],@DistrictID [DistrictID],@ItemCategoryID [ItemCategoryID],0 [ItemID],'' [ItemDescription],0 [ItemCost], CAST(0 AS BIT) InUse
            UNION ALL
            SELECT [ProgramYear],[DistrictID],[ItemCategoryID],[ItemID],[ItemDescription],[ItemCost],
            CAST(IIF((SELECT COUNT(*) FROM [MenuAnalysisEntry] WHERE [ProgramYear]=MCIS.ProgramYear AND [DistrictID]=MCIS.DistrictID AND ItemID=MCIS.ItemID and ItemCategoryID=MCIS.ItemCategoryID)>0, 1, 0) AS BIT) InUse
            FROM [MenuCostItemsSetup] MCIS
            WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND [ItemCategoryID]=@ItemCategoryID
            ORDER BY [ItemDescription]"
            >
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLCategories" Name="ItemCategoryID" PropertyName="SelectedValue" Type="Int16" />
            </SelectParameters>
            <DeleteParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="ItemCategoryID" Type="Int16" />
                <asp:Parameter Name="ItemID" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLCategories" Name="ItemCategoryID" PropertyName="SelectedValue" Type="Int16" />
                <asp:Parameter Name="ItemID" Type="Int32" />
                <asp:Parameter Name="ItemDescription" Type="String" />
                <asp:Parameter Name="ItemCost" Type="Decimal" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="ItemCategoryID" Type="Int16" />
                <asp:Parameter Name="ItemID" Type="Int32" />
                <asp:Parameter Name="ItemDescription" Type="String" />
                <asp:Parameter Name="ItemCost" Type="Decimal" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate FROM [ProgramYear] WHERE EndDate>='1/1/2024' ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceRanges" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
             SelectCommand="SELECT MidrangeLower, MidrangeUpper FROM MenuCostRangeSetup WHERE ProgramYear=@ProgramYear AND ItemCategoryID=@ItemCategoryID">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DDLCategories" Name="ItemCategoryID" PropertyName="SelectedValue" Type="Int16" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
