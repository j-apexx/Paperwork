<%@ Page Title="Setup Milk Delivery Info" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupMilkDelivery.aspx.cs" Inherits="Paperwork.SetupMilkDelivery" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
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
        };

    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <asp:Image CssClass="loadingImage" ID="ImageLoading1" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div id="topSection" style="height:100px">
                <div id="hdrgroup">
                    <div id="hdrDiv" style="float:left;width:33%">
                        <h2><%: Title %></h2>
                    </div>
                    <div id="menuDiv" style="float:left;width:67%">
                        <br />
                        <p>
                            <strong>District:</strong>
                            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> &nbsp;
                        </p>
                    </div>
                </div>
            </div>
            <div id="MilkDelivSection">
                <asp:GridView ID="GridViewMilkDeliv" runat="server" CssClass="mGrid" AutoGenerateColumns="False" ShowFooter="True" ShowHeaderWhenEmpty="True" AllowSorting="True"
                    OnRowCommand="GridViewMilkDeliv_RowCommand" OnRowDataBound="GridViewMilkDeliv_RowDataBound" DataSourceID="SqlDataSourceMilkDeliv"
                    DataKeyNames="ProgramYear, DistrictID, ID">
                    <Columns>
                        <asp:TemplateField  HeaderText="School" SortExpression="SchoolNum" >
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLESchools" runat="server" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" 
                                    DataValueField="SchoolNum" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true" > 
                                        <asp:ListItem Selected="False" Text="- Select -" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredSchool" runat="server" ControlToValidate="DDLESchools" CssClass="validators"
                                    Display="Dynamic" ErrorMessage="School is required" Text="*" ValidationGroup="EditMilkDeliv"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:DropDownList ID="DDLSchools" runat="server" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" 
                                    DataValueField="SchoolNum" SelectedValue='<%# Eval("SchoolNum") %>' AppendDataBoundItems="true" Enabled="false" > 
                                        <asp:ListItem Selected="False" Text="- Select -" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="DDLFSchools" runat="server" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" 
                                    DataValueField="SchoolNum" AppendDataBoundItems="true" > 
                                        <asp:ListItem Selected="False" Text="- Select -" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareSchool" runat="server" ControlToValidate="DDLFSchools" Display="Dynamic" Text="*" ErrorMessage="School is required"
                                     Operator="GreaterThan" ValueToCompare="0" Type="Integer" ValidationGroup="InsertMilkDeliv" CssClass="validators"></asp:CompareValidator>
                            </FooterTemplate>                        
                        </asp:TemplateField>
                        <asp:TemplateField  HeaderText="Type" SortExpression="SchoolType" >
                            <ItemTemplate>
                                <asp:DropDownList ID="DDLSchoolType" runat="server" SelectedValue='<%# Eval("SchoolType") %>' Enabled="false" >
                                    <asp:ListItem Text="-SELECT-" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Elementary" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Middle" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Secondary" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="MMB Area" SortExpression="MMBArea" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEMMBArea" runat="server" Text='<%# Bind("MMBArea") %>' Width="100"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelMMBArea" runat="server" Text='<%# Eval("MMBArea") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFMMBArea" runat="server"  Width="100"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Mon Cases" SortExpression="Monday" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEMonday" runat="server" Text='<%# Bind("Monday") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelMonday" runat="server" Text='<%# Eval("Monday") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFMonday" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tues Cases" SortExpression="Tuesday" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxETuesday" runat="server" Text='<%# Bind("Tuesday") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelTuesday" runat="server" Text='<%# Eval("Tuesday") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFTuesday" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Weds Cases" SortExpression="Wednesday" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEWednesday" runat="server" Text='<%# Bind("Wednesday") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelWednesday" runat="server" Text='<%# Eval("Wednesday") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFWednesday" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Thurs Cases" SortExpression="Thursday" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEThursday" runat="server" Text='<%# Bind("Thursday") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelThursday" runat="server" Text='<%# Eval("Thursday") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFThursday" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fri Cases" SortExpression="Friday" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEFriday" runat="server" Text='<%# Bind("Friday") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelFriday" runat="server" Text='<%# Eval("Friday") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFFriday" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Juice Cases" SortExpression="JuiceCases" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEJuiceCases" runat="server" Text='<%# Bind("JuiceCases") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelJuiceCases" runat="server" Text='<%# Eval("JuiceCases") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFJuiceCases" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tea Cases" SortExpression="TeaCases" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxETeaCases" runat="server" Text='<%# Bind("TeaCases") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelTeaCases" runat="server" Text='<%# Eval("TeaCases") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFTeaCases" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Walk-in Cooler" SortExpression="WalkInCooler" >
                            <EditItemTemplate>
                                <div style="padding-left:35%;">
                                <asp:CheckBox ID="CBEWalkinCooler" runat="server" Checked='<%# Bind("WalkInCooler") %>' /></div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <div style="padding-left:35%;">
                                <asp:CheckBox ID="CBWalkinCooler" runat="server" Checked='<%# Bind("WalkInCooler") %>' Enabled="false" /></div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="padding-left:35%;">
                                    <asp:CheckBox ID="CBFWalkinCooler" runat="server"  />
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="# 8 Cs Coolers" SortExpression="EightCaseCoolers" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEEightCaseCoolers" runat="server" Text='<%# Bind("EightCaseCoolers") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelEightCaseCoolers" runat="server" Text='<%# Eval("EightCaseCoolers") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFEightCaseCoolers" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="# 12 Cs Coolers" SortExpression="TwelveCaseCoolers" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxETwelveCaseCoolers" runat="server" Text='<%# Bind("TwelveCaseCoolers") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelTwelveCaseCoolers" runat="server" Text='<%# Eval("TwelveCaseCoolers") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFTwelveCaseCoolers" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="# 16 Cs Coolers" SortExpression="SixteenCaseCoolers" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxESixteenCaseCoolers" runat="server" Text='<%# Bind("SixteenCaseCoolers") %>' Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelSixteenCaseCoolers" runat="server" Text='<%# Eval("SixteenCaseCoolers") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFSixteenCaseCoolers" runat="server"  Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="License" SortExpression="PALicense" >
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEPALicense" runat="server" Text='<%# Bind("PALicense") %>' Width="70"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelPALicense" runat="server" Text='<%# Eval("PALicense") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFPALicense" runat="server"  Width="70"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField  HeaderText="Dairy" SortExpression="VendorID" >
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLEVendors" runat="server" DataSourceID="SqlDataSourceDistVendors" DataTextField="VENDNAME" DataValueField="VENDORID"   
                                    AppendDataBoundItems="true" SelectedValue='<%# Bind("VendorID") %>' > 
                                    <asp:ListItem Selected="False" Text="- Select -" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredVendor" runat="server" ControlToValidate="DDLEVendors" CssClass="validators"
                                    Display="Dynamic" ErrorMessage="Dairy is required" Text="*" ValidationGroup="EditMilkDeliv"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:DropDownList ID="DDLVendors" runat="server" DataSourceID="SqlDataSourceDistVendors" DataTextField="VENDNAME" DataValueField="VENDORID" 
                                    Enabled="false"  AppendDataBoundItems="true" SelectedValue='<%# Eval("VendorID") %>' > 
                                    <asp:ListItem Selected="False" Text="- Select -" Value=""></asp:ListItem> 
                                </asp:DropDownList>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="DDLFVendors" runat="server" DataSourceID="SqlDataSourceDistVendors" DataTextField="VENDNAME" 
                                    DataValueField="VendorID" AppendDataBoundItems="true" > 
                                    <asp:ListItem Selected="False" Text="- Select -" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareVendor" runat="server" ControlToValidate="DDLFVendors" Display="Dynamic" Text="*" ErrorMessage="Dairy is required"
                                     Operator="NotEqual" ValueToCompare="" Type="String" ValidationGroup="InsertMilkDeliv" CssClass="validators"></asp:CompareValidator>
                            </FooterTemplate>                        
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="MMB Sheet?" SortExpression="MMBSheet" >
                            <EditItemTemplate>
                                <div style="padding-left:35%;">
                                <asp:CheckBox ID="CBEMMBSheet" runat="server" Checked='<%# Bind("MMBSheet") %>' /></div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <div style="padding-left:35%;">
                                <asp:CheckBox ID="CBMMBSheet" runat="server" Checked='<%# Bind("MMBSheet") %>' Enabled="false" /></div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="padding-left:35%;">
                                    <asp:CheckBox ID="CBFMMBSheet" runat="server"  />
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <div style="width:60px;text-align:center;" >
                                    <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                        CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="EditMilkDeliv" />
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
                                        OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="width:60px;text-align:center;" >
                                    <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" 
                                        CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="InsertMilkDeliv" />
                                    <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Reset" 
                                        CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                </div>
                            </FooterTemplate>
                        
                            <ItemStyle Width="60" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ValidationSummary ID="SummaryEdit" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                    ValidationGroup="EditMilkDeliv" HeaderText="Editing Errors" BackColor="WhiteSmoke" Width="500" />
                <asp:ValidationSummary ID="SummaryInsert" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                    ValidationGroup="InsertMilkDeliv" HeaderText="Insert Errors" BackColor="WhiteSmoke" Width="500" />
                <div class="messagealert" id="alert_container"></div>
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SELECTCOMMAND="SELECT * FROM DistrictSchools WHERE DistrictID=@DistrictID ORDER BY SortOrder, SchoolName"   >
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceMilkDeliv" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    DeleteCommand="DELETE FROM [DistrictMilkDeliveryInfo] WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND [ID] = @ID" 
                    InsertCommand="INSERT INTO [DistrictMilkDeliveryInfo] ([ProgramYear], [DistrictID], [SchoolNum], [MMBArea], [Monday], [Tuesday], [Wednesday], 
                    [Thursday], [VendorID], [PALicense], [SixteenCaseCoolers], [TwelveCaseCoolers], [EightCaseCoolers], [WalkInCooler], [TeaCases], [JuiceCases], 
                    [Friday], [MMBSheet], [AverageCaseCount], [MilkDiscounts], [OtherDiscounts], [EquipLeases], [EquipOwner], [LeasePayments]) 
                    VALUES (@ProgramYear, @DistrictID, @SchoolNum, @MMBArea, @Monday, @Tuesday, @Wednesday, @Thursday, @VendorID, @PALicense, @SixteenCaseCoolers, @TwelveCaseCoolers, 
                    @EightCaseCoolers, @WalkInCooler, @TeaCases, @JuiceCases, @Friday, @MMBSheet, @AverageCaseCount, @MilkDiscounts, @OtherDiscounts, @EquipLeases, @EquipOwner, @LeasePayments)" 
        
                    SelectCommand="SELECT [ProgramYear], DMD.[DistrictID], DMD.[SchoolNum], [MMBArea], [Monday], [Tuesday], [Wednesday], [Thursday], [VendorID], [PALicense], [SixteenCaseCoolers], 
                    [TwelveCaseCoolers], [EightCaseCoolers], [WalkInCooler], [TeaCases], [JuiceCases], [Friday], [MMBSheet], [ID], [SchoolType] 
                    FROM [DistrictMilkDeliveryInfo] DMD
                    INNER JOIN [DistrictSchools] DS ON DMD.DistrictID=DS.DistrictID AND DMD.SchoolNum=DS.SchoolNum
                    WHERE ((DMD.[ProgramYear] = @ProgramYear) AND (DMD.[DistrictID] = @DistrictID))
                    UNION ALL
                    SELECT @ProgramYear, '' [DistrictID], 0, ''[MMBArea], 0[Monday], 0[Tuesday], 0[Wednesday], 0[Thursday], ''[VendorID], ''[PALicense], 0[SixteenCaseCoolers], 
                    0[TwelveCaseCoolers], 0[EightCaseCoolers], CAST(0 as bit) [WalkInCooler], 0[TeaCases], 0[JuiceCases], 0[Friday], CAST(0 as bit)[MMBSheet], 0[ID], 0[SchoolType]" 
                    
                    UpdateCommand="UPDATE [DistrictMilkDeliveryInfo] SET [SchoolNum] = @SchoolNum, [MMBArea] = @MMBArea, [Monday] = @Monday, [Tuesday] = @Tuesday, [Wednesday] = @Wednesday, 
                    [Thursday] = @Thursday, [VendorID] = @VendorID, [PALicense] = @PALicense, [SixteenCaseCoolers] = @SixteenCaseCoolers, [TwelveCaseCoolers] = @TwelveCaseCoolers, 
                    [EightCaseCoolers] = @EightCaseCoolers, [WalkInCooler] = CAST(@WalkInCooler AS Bit), [TeaCases] = @TeaCases, [JuiceCases] = @JuiceCases, [Friday] = @Friday, 
                    [MMBSheet] = CAST(@MMBSheet AS BIT)
                    WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND [ID] = @ID"
                    
                    OnInserted="SqlDataSourceMilkDeliv_Inserted" OnDeleted="SqlDataSourceMilkDeliv_Deleted" OnInserting="SqlDataSourceMilkDeliv_Inserting" >
                    <DeleteParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="ID" Type="Int32" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="SchoolNum" Type="Int16" />
                        <asp:Parameter Name="MMBArea" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Monday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="Tuesday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="Wednesday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="Thursday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="VendorID" Type="String" />
                        <asp:Parameter Name="PALicense" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="SixteenCaseCoolers" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="TwelveCaseCoolers" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="EightCaseCoolers" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="WalkInCooler" Type="Boolean" DefaultValue="false" />
                        <asp:Parameter Name="TeaCases" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="JuiceCases" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="Friday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="MMBSheet" Type="Boolean" DefaultValue="false" />
                        <asp:Parameter Name="AverageCaseCount" Type="Decimal" DefaultValue="0" />
                        <asp:Parameter Name="MilkDiscounts" Type="Byte" DefaultValue="0"/>
                        <asp:Parameter Name="OtherDiscounts" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="EquipLeases" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="EquipOwner" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"/>
                        <asp:Parameter Name="LeasePayments" Type="Decimal" DefaultValue="0" />
                    </InsertParameters>
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="SchoolNum" Type="Int16" />
                        <asp:Parameter Name="MMBArea" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Monday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="Tuesday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="Wednesday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="Thursday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="VendorID" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                        <asp:Parameter Name="PALicense" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="SixteenCaseCoolers" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="TwelveCaseCoolers" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="EightCaseCoolers" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="WalkInCooler" Type="Boolean" DefaultValue="false"/>
                        <asp:Parameter Name="TeaCases" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="JuiceCases" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="Friday" Type="Byte" DefaultValue="0" />
                        <asp:Parameter Name="MMBSheet" Type="Boolean" DefaultValue="false" />
                        <asp:Parameter Name="ProgramYear" Type="String" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="DistrictID" Type="String"  ConvertEmptyStringToNull="false"/>
                        <asp:Parameter Name="ID" Type="Int32" />
                    </UpdateParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceDistVendors" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT D.VENDORID, V.VENDNAME FROM [dbo].[DistrictVendors] D 
                        INNER JOIN [dbo].[Vendors] V ON D.VENDORID=V.VENDORID 
                        WHERE VENDSTTS=1 AND [DistrictID]=@DistrictID 
                        ORDER BY VENDNAME" >
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
