<%@ Page Title="Setup Vendor Customer Numbers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupVendorCustNums.aspx.cs" Inherits="Paperwork.SetupVendorCustNums" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5">
        <ProgressTemplate>
            <div class="loadingDiv" >
                <asp:Image CssClass="loadingImage" ID="ImageLoading" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div>
                <div class="row">
                    <div class="col-sm-2">
                        Vendor:
                    </div>
                    <div class="col-sm-10">
                        <asp:DropDownList ID="ddlVendors" runat="server" DataSourceID="SqlDataSourceVendors" DataTextField="VENDNAME" DataValueField="VENDORID" 
                            TabIndex="1" AutoPostBack="true" >

                        </asp:DropDownList>
                        <asp:CompareValidator ID="ValidatorDDLVendors" runat="server" ValueToCompare="_" Display="Dynamic" Operator="NotEqual" ControlToValidate="ddlVendors" 
                            ValidationGroup="Insert" ForeColor="Red" Type="String" ErrorMessage="Select a Vendor" ToolTip="Select a Vendor">*</asp:CompareValidator>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-2">
                        District:
                    </div>
                    <div class="col-sm-10">
                        <asp:DropDownList ID="DDLDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true"  DataTextField="Desc" DataValueField="DistrictID" OnSelectedIndexChanged="DDLDistricts_SelectedIndexChanged" >
                            <asp:ListItem Text="-All Districts-" Value="%" Selected="true"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            
                <div class="row">
                    <div class="col-sm-12">
                        <div id="DataEntry">
                            <asp:GridView ID="GridViewCustomerNumbers" runat="server" AutoGenerateColumns="False" DataKeyNames="CustomerNumber,VendorID" 
                                DataSourceID="SqlDataSourceVendorCusts" CssClass="mGrid" AllowSorting="True" 
                                PageSize="100" AllowPaging="True" PagerSettings-Mode="NumericFirstLast" PagerStyle-Width="100"  
                                PagerSettings-Position="TopAndBottom" PagerStyle-CssClass="mGridPager" Width="700px"
                                ShowFooter="True" OnRowCommand="GridViewCustomerNumbers_RowCommand" OnRowDataBound="GridViewCustomerNumbers_RowDataBound"
                                ShowHeader="true" ShowHeaderWhenEmpty="true">

                                <Columns>
                                    <asp:TemplateField HeaderText="District" SortExpression="DistrictID">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxEDistrictID" runat="server" Text='<%# Bind("DistrictID") %>' MaxLength="3" Width="75" Enabled="false"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("DistrictID") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFDistrictID" runat="server" AutoPostBack="true" OnTextChanged="TextBoxFDistrictID_TextChanged" MaxLength="3" Width="75" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFDist" runat="server" CssClass="validators" ControlToValidate="TextBoxFDistrictID" ValidationGroup="Insert" ErrorMessage="Enter the District ID" Display="Dynamic" >*</asp:RequiredFieldValidator>
                                        </FooterTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="School" ItemStyle-HorizontalAlign="Center" SortExpression="SchoolName">
                                        <EditItemTemplate>
                                            <asp:HiddenField ID="HFDistrictID" runat="server" Value='<%# Eval("DistrictID") %>' />
                                            <asp:DropDownList ID="DropDownListSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" DataValueField="SchoolNum" SelectedValue='<%# Bind("SchoolNum") %>'>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader" SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID AND INACTIVE=0 ORDER BY SortOrder, SchoolName">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="HFDistrictID" Name="DistrictID" PropertyName="Value" Type="String" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="LabelSchoolNum" runat="server" Text='<%# Eval("SchoolName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <FooterTemplate>
                                            <asp:HiddenField ID="HFFDistrictID" runat="server"  />
                                            <asp:DropDownList ID="DropDownListFSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" DataValueField="SchoolNum" >
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader" SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID AND INACTIVE=0 ORDER BY SortOrder, SchoolName">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="HFFDistrictID" Name="DistrictID" PropertyName="Value" Type="String" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Number" SortExpression="CustomerNumber">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxECustomerNumber" runat="server" Text='<%# Bind("CustomerNumber") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("CustomerNumber") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFCustomerNumber" runat="server" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFCustNum" runat="server" CssClass="validators" ControlToValidate="TextBoxFCustomerNumber" ValidationGroup="Insert" ErrorMessage="Enter the Customer Number" Display="Dynamic" >*</asp:RequiredFieldValidator>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="VendorID" SortExpression="VendorID">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxEVendorID" runat="server" Text='<%# Bind("VendorID") %>' MaxLength="50" Width="75"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("VendorID") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <FooterTemplate>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Use Default School">
                                        <EditItemTemplate>
                                            <div style="text-align:center;">
                                                <asp:CheckBox ID="CB_EUseDefaultSchool" runat="server" Checked='<%# Bind("CB_UseDefaultSchool") %>' />
                                           </div>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <div style="text-align:center;">
                                                <asp:CheckBox ID="CB_UseDefaultSchool" runat="server" Checked='<%# Bind("CB_UseDefaultSchool") %>' Enabled="false" />
                                           </div>
                                        </ItemTemplate>
                                       <FooterTemplate>
                                           <div style="text-align:center;">
                                                <asp:CheckBox ID="CB_FUseDefaultSchool" runat="server" Checked="false" />
                                           </div>
                                       </FooterTemplate>
                                    </asp:TemplateField>
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
                                        <HeaderStyle Width="165px" />
                                        <FooterTemplate>
                                            <div style="min-width:165px;">
                                                <asp:LinkButton runat="server" CommandName="Insert" ID="FSave" ToolTip="Add" Text="Add"
                                                        CssClass="fas fa-plus mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                                <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" Text="Cancel" 
                                                        CssClass="fas fa-undo-alt mGridLinkButton" ForeColor="Red" CausesValidation="false" />
                                            </div>
                                
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                </Columns>

                                <PagerSettings Mode="NumericFirstLast" Position="TopAndBottom" />
                                <PagerStyle CssClass="mGridPager" Width="100px" />

                            </asp:GridView>
                            <asp:ValidationSummary ID="ValidationSummaryInsert" runat="server"  CssClass="validators" DisplayMode="BulletList"  ValidationGroup="Insert"
                                 ShowSummary="true" ShowValidationErrors="true" />
                            <div class="messagealert" id="alert_container"></div>
                        </div>

                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceVendorCusts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_{0}"
            DeleteCommand="DELETE FROM [VendorCustomerMappings] WHERE [CustomerNumber] = @old_CustomerNumber AND [VendorID] = @old_VendorID" 
            InsertCommand="INSERT INTO [VendorCustomerMappings] ([DistrictID], [SchoolNum], [CustomerNumber], [VendorID], CB_UseDefaultSchool) VALUES (@DistrictID, @SchoolNum, @CustomerNumber, UPPER(@VendorID), @CB_UseDefaultSchool)" 
            SelectCommand="SELECT VCM.DistrictID, IIF(VCM.CB_UseDefaultSchool=1, dbo.GetMinSchool(VCM.DistrictID),VCM.SchoolNum) SchoolNum, VCM.CustomerNumber, VCM.VendorID, 
            IIF(VCM.CB_UseDefaultSchool=1, '*Default*',ISNULL(SchoolName, VCM.SchoolNum)) SchoolName, VCM.CB_UseDefaultSchool FROM [VendorCustomerMappings] VCM
            LEFT OUTER JOIN DistrictSchools DS ON VCM.DistrictID=DS.DistrictID AND VCM.SchoolNum=DS.SchoolNum 
            WHERE VCM.DistrictID LIKE @DistrictID AND VCM.VendorID=@VendorID
            UNION ALL SELECT '' DistrictID, 0 SchoolNum, '' CustomerNumber, '' VendorID, '' SchoolName, CAST(0 AS BIT) CB_UseDefaultSchool " 
            UpdateCommand="UPDATE [VendorCustomerMappings] SET [DistrictID]=@DistrictID, [SchoolNum]=IIF(@CB_UseDefaultSchool=1, 0, @SchoolNum), [CustomerNumber] = @CustomerNumber, [CB_UseDefaultSchool]=@CB_UseDefaultSchool
            WHERE [CustomerNumber] = @old_CustomerNumber AND [VendorID] = @old_VendorID"
            
            OnInserting="SqlDataSourceVendorCusts_Inserting" OnInserted="SqlDataSourceVendorCusts_Inserted">
            <DeleteParameters>
                <asp:Parameter Name="CustomerNumber" Type="String" />
                <asp:Parameter Name="VendorID" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="CustomerNumber" Type="String" />
                <asp:ControlParameter ControlID="ddlVendors" DbType="String" PropertyName="SelectedValue" Name="VendorID" />
                <asp:Parameter Name="CB_UseDefaultSchool" Type="Byte" DefaultValue="0" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="CustomerNumber" Type="String" />
                <asp:Parameter Name="CB_UseDefaultSchool" Type="Byte" DefaultValue="0" />
            </UpdateParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLDistricts" DbType="StringFixedLength" PropertyName="SelectedValue" DefaultValue="%" Name="DistrictID" />
                <asp:ControlParameter ControlID="ddlVendors" DbType="String" PropertyName="SelectedValue" Name="VendorID" />
            </SelectParameters>

        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceVendors" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT '_' VENDORID, ' -SELECT- ' VENDNAME
                UNION ALL
                SELECT V.VENDORID, RTRIM(V.VENDNAME) + ' - ('+ RTRIM(V.VENDORID) + ')' VENDNAME
                FROM [Vendors] V 
                WHERE VENDSTTS=1 
                ORDER BY VENDNAME">
        </asp:SqlDataSource>

    </div>
</asp:Content>
