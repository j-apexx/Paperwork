<%@ Page Title="MMB Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MMBEntry.aspx.cs" Inherits="Paperwork.MMBEntry" %>
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
            $(function () {
                $("[id*=TextBoxEDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            

            $(function () {
                $("[id*=TextBoxFDate]").datepicker({
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

        }

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
                <div id="topSection">
                    <div id="hdrDiv" style="float:left;width:35%">
                        <h2><%: Title %></h2>
                        <%--<asp:LinkButton ID="ISave" runat="server" 
                            ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green"
                            CssClass="fas fa-save mGridLinkButton" Width="170"></asp:LinkButton>--%>
                    </div>
                    <div id="menuDiv">
                        <strong>Program Year:</strong>
                        <asp:Label ID="LabelProgramYear" runat="server"></asp:Label>
                        &nbsp;&nbsp; 
                        <strong>District:</strong>
                        <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                            DataSourceID="SqlDataSourceMyDistricts" DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                            OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                        </asp:DropDownList> &nbsp;
                        <asp:CheckBox ID="CBShowLocked" runat="server" Text="Show Locked" Checked="false" AutoPostBack="true" />
                    </div>


                </div>
                <div id="Records">
                    <asp:GridView ID="GridViewMMBEntry" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,ProductCode,VendorID,MMBDate,SchoolNum" 
                        DataSourceID="SqlDataSourceMMBEntry" CssClass="mGrid" ShowFooter="true" OnRowCommand="GridViewMMBEntry_RowCommand" ShowHeaderWhenEmpty="true" 
                        OnRowDataBound="GridViewMMBEntry_RowDataBound" AllowSorting="true" OnPreRender="GridViewMMBEntry_PreRender" 
                        >
                        <Columns>
                            <asp:TemplateField HeaderText="Date" SortExpression="MMBDate" HeaderStyle-Width="100" >
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEDate" runat="server" Text='<%# Bind("MMBDate", "{0:d}") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelDate" runat="server" Text='<%# Eval("MMBDate", "{0:d}") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFDate" runat="server"  Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Vendors" SortExpression="VendName">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlEVendors" runat="server" DataSourceID="SqlDataSourceDistVendors" DataTextField="VENDNAME" 
                                        DataValueField="VENDORID" SelectedValue='<%# Bind("VendorID") %>' ></asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelVendor" runat="server" Text='<%# Eval("VendName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlFVendors" runat="server" DataSourceID="SqlDataSourceDistVendors" DataTextField="VENDNAME" 
                                        DataValueField="VENDORID"  AppendDataBoundItems="true">
                                        <asp:ListItem Text="--Select One--" Value="" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Product" SortExpression="ProductName">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlEProducts" runat="server" DataSourceID="SqlDataSourceMMBItems" DataTextField="ProductName" 
                                        DataValueField="ProductCode" SelectedValue='<%# Bind("ProductCode") %>' OnSelectedIndexChanged="ddlProducts_SelectedIndexChanged" ></asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelProduct" runat="server" Text='<%# Eval("ProductName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlFProducts" runat="server" DataSourceID="SqlDataSourceMMBItems" DataTextField="ProductName" 
                                        DataValueField="ProductCode" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlProducts_SelectedIndexChanged">
                                        <asp:ListItem Text="--Select One--" Value="0" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="School" SortExpression="SchoolNum">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DropDownEListSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceAllSchools" DataTextField="SchoolName" 
                                        DataValueField="SchoolNum" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelSchoolName" runat="server" Text='<%# Eval("SchoolName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="DropDownListFSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceActiveSchools" DataTextField="SchoolName" 
                                        DataValueField="SchoolNum"  AppendDataBoundItems="true">
                                        <asp:ListItem Text="--Select One--" Value="0" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity" HeaderStyle-Width="100">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEQuantity" runat="server" Text='<%# Bind("Quantity") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelQuantity" runat="server" Text='<%# Eval("Quantity") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFQuantity" runat="server"  Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Price" SortExpression="Price" HeaderStyle-Width="100">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEPrice" runat="server" Text='<%# Bind("Price") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelPrice" runat="server" Text='<%# Eval("Price") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFPrice" runat="server" Width="100" ></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Milk Item" SortExpression="MilkItem" HeaderStyle-Width="100">
                                <EditItemTemplate>
                                    <div style="text-align:center;">
                                        <asp:CheckBox ID="CBEMilkItem" runat="server" Checked='<%# Bind("MilkItem") %>' />
                                    </div>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="text-align:center;">
                                        <asp:CheckBox ID="CBMilkItem" runat="server" Checked='<%# Eval("MilkItem") %>' Enabled="false"  />
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <div style="text-align:center;">
                                        <asp:CheckBox ID="CBFMilkItem" runat="server" />
                                    </div>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Locked" HeaderStyle-Width="50">
                                <ItemTemplate>
                                    <div style="width:50px;text-align:center;">
                                        <asp:CheckBox ID="cbLocked" runat="server" Checked='<%# Eval("Locked") %>' Enabled="false" />
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    
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
                        
                                <ItemStyle Width="50" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <div class="messagealert" id="alert_container"></div>
                    <br />
                    <br />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceMyDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc'
            FROM [District] WHERE DistrictID IN (SELECT DistrictID FROM [UserDistrictAccess] WHERE Userid=@UserID) OR (@AccessLevel<>'User')
            ORDER BY [DistrictName]">
            <SelectParameters>
                <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                <asp:SessionParameter Name="AccessLevel" SessionField="AccessLevel" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceAllSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID ORDER BY SchoolName">
            <SelectParameters>
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceActiveSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID AND Inactive=0 ORDER BY SchoolName">
            <SelectParameters>
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceMMBEntry" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_{0}"
            DeleteCommand="DELETE FROM [MMBTransactions] WHERE [ProgramYear]=@old_ProgramYear AND [DistrictID] = @old_DistrictID AND 
            [ProductCode] = @old_ProductCode AND [VendorID] = @old_VendorID AND [MMBDate] = @old_MMBDate AND [SchoolNum] = @old_SchoolNum" 
            InsertCommand="INSERT INTO [MMBTransactions] ([ProgramYear], [DistrictID], [ProductCode], [VendorID], [MMBDate], [SchoolNum], 
            [Quantity], [Price], [MilkItem], [Locked]) VALUES (@ProgramYear, @DistrictID, @ProductCode, @VendorID, @MMBDate, @SchoolNum, 
            @Quantity, @Price, @MilkItem, 0)" 
            SelectCommand="SELECT TRX.[ProgramYear], TRX.[DistrictID], TRX.[ProductCode], TRX.[VendorID], ISNULL(VENDNAME, '') VENDNAME, TRX.[MMBDate], TRX.[SchoolNum], TRX.[Quantity], 
            TRX.[Price], TRX.[MilkItem], [Locked] , DS.SchoolName, ISNULL(ProductName, TRX.ProductCode) ProductName
            FROM [MMBTransactions] TRX 
            LEFT OUTER JOIN VENDORS ON TRX.VendorID=VENDORS.VendorID
            LEFT OUTER JOIN [DistrictSchools] DS ON TRX.[DistrictID]=DS.[DistrictID] AND TRX.[SchoolNum]=DS.[SchoolNum]
            LEFT OUTER JOIN [MMBItems] MI ON TRX.ProgramYear=MI.ProgramYear AND TRX.DistrictID=MI.DistrictID AND TRX.ProductCode=MI.ProductCode
            WHERE ((TRX.[ProgramYear] = @ProgramYear) AND (TRX.[DistrictID] = @DistrictID)) 
            UNION ALL
            SELECT '' ProgramYear, '' DistrictID, 0 ProductCode, '' VendorID, '' VendName, '1/1/1900' MMBDate, 0 SchoolNum, 0 Quantity, 0 Price, 
            CAST(0 AS bit) MilkItem, CAST(0 AS bit) Locked, '' SchoolName, '' ProductName
            ORDER BY [MMBDate], [SchoolNum], [ProductCode]" 
            UpdateCommand="UPDATE [MMBTransactions] SET [Quantity]=@Quantity, [Price]=@Price, [MilkItem]=@MilkItem, 
            [ProductCode]=@ProductCode, [VendorID]=@VendorID, [MMBDate]=@MMBDate, [SchoolNum]=@SchoolNum
            WHERE [ProgramYear]=@old_ProgramYear AND [DistrictID]=@old_DistrictID AND [ProductCode]=@old_ProductCode AND [VendorID]=@old_VendorID 
            AND [MMBDate] = @old_MMBDate AND [SchoolNum] = @old_SchoolNum"
            OnInserting="SqlDataSourceMMBEntry_Inserting" OnInserted="SqlDataSourceMMBEntry_Inserted"
            OnDeleted="SqlDataSourceMMBEntry_Deleted" FilterExpression="[Locked]<={0}">
            <FilterParameters>
                <asp:ControlParameter ControlID="CBShowLocked" PropertyName="Checked" Type="Byte" />
            </FilterParameters>
            <DeleteParameters>
                <asp:Parameter Name="ProgramYear" Type="String" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="ProductCode" Type="Int16" />
                <asp:Parameter Name="VendorID" Type="String" />
                <asp:Parameter DbType="Date" Name="MMBDate" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
            </DeleteParameters>
            <InsertParameters>
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:Parameter Name="ProductCode" Type="Int16" />
                <asp:Parameter Name="VendorID" Type="String" />
                <asp:Parameter DbType="Date" Name="MMBDate" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="Quantity" Type="Int32" />
                <asp:Parameter Name="Price" Type="Decimal" />
                <asp:Parameter Name="MilkItem" Type="Boolean" />
                <asp:Parameter Name="Locked" Type="Boolean" />
            </InsertParameters>
            <SelectParameters>
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:Parameter Name="Quantity" Type="Int32" />
                <asp:Parameter Name="Price" Type="Decimal" />
                <asp:Parameter Name="MilkItem" Type="Boolean" />
                <asp:Parameter Name="ProductCode" Type="Int16" />
                <asp:Parameter Name="VendorID" Type="String" />
                <asp:Parameter DbType="Date" Name="MMBDate" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceDistVendors" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT D.VENDORID, V.VENDNAME FROM [dbo].[DistrictVendors] D 
                INNER JOIN [dbo].[Vendors] V ON D.VENDORID=V.VENDORID 
                WHERE VENDSTTS=1 AND [DistrictID]=@DistrictID 
                ORDER BY VENDNAME">
            <SelectParameters>
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceMMBItems" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [MilkItem], [Price], [ProductName], [ProductCode], [DistrictID], [ProgramYear] FROM [MMBItems] 
            WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID))
            ORDER BY ProductName">
            <SelectParameters>
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
