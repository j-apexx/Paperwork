<%@ Page Title="Setup RFS Customer Numbers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupRFSCustNums.aspx.cs" Inherits="Paperwork.SetupRFSCustNums" %>
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
            <asp:DropDownList ID="DDLDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true"  DataTextField="Desc" DataValueField="DistrictID" OnSelectedIndexChanged="DDLDistricts_SelectedIndexChanged" >
                <asp:ListItem Text="-All Districts-" Value="%" Selected="true"></asp:ListItem>
            </asp:DropDownList> 
            <br />
            <div id="DataEntry">
                <asp:GridView ID="GridViewRFSCustNums" runat="server" AutoGenerateColumns="False" DataKeyNames="RFSCustNum,Division" 
                    DataSourceID="SqlDataSourceRFSCusts" CssClass="mGrid" AllowSorting="True" 
                    PageSize="100" AllowPaging="True" PagerSettings-Mode="NumericFirstLast" PagerStyle-Width="100"  
                    PagerSettings-Position="TopAndBottom" PagerStyle-CssClass="mGridPager" Width="700px"
                    ShowFooter="True" OnRowCommand="GridViewRFSCustNums_RowCommand" OnRowDataBound="GridViewRFSCustNums_RowDataBound"
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
                        <asp:TemplateField HeaderText="RFS Customer Num" SortExpression="RFSCustNum">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxERFSCustNum" runat="server" Text='<%# Bind("RFSCustNum") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("RFSCustNum") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFRFSCustNum" runat="server" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Division" SortExpression="Division">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEDivision" runat="server" Text='<%# Bind("Division") %>' MaxLength="50" Width="75"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Division") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFDivision" runat="server" MaxLength="50" Width="75"></asp:TextBox>
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
                <div class="messagealert" id="alert_container"></div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceRFSCusts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_{0}"
            DeleteCommand="DELETE FROM [RFSCustomerNumbers] WHERE [RFSCustNum] = @old_RFSCustNum AND [Division] = @old_Division" 
            InsertCommand="INSERT INTO [RFSCustomerNumbers] ([DistrictID], [SchoolNum], [RFSCustNum], [Division]) VALUES (@DistrictID, @SchoolNum, @RFSCustNum, UPPER(@Division))" 
            SelectCommand="SELECT RFS.DistrictID, RFS.SchoolNum, RFS.RFSCustNum, RFS.Division, ISNULL(SchoolName, RFS.SchoolNum) SchoolName FROM [RFSCustomerNumbers] RFS
            LEFT OUTER JOIN DistrictSchools DS ON RFS.DistrictID=DS.DistrictID AND RFS.SchoolNum=DS.SchoolNum 
            WHERE RFS.DistrictID LIKE @DistrictID
            UNION ALL SELECT '' DistrictID, 0 SchoolNum, 0 RFSCustNum, '' Division, '' SchoolName " 
            UpdateCommand="UPDATE [RFSCustomerNumbers] SET [DistrictID]=@DistrictID, [SchoolNum]=@SchoolNum, [RFSCustNum] = @RFSCustNum, [Division] = UPPER(@Division)
            WHERE [RFSCustNum] = @old_RFSCustNum AND [Division] = @old_Division"
            
            OnInserting="SqlDataSourceRFSCusts_Inserting" OnInserted="SqlDataSourceRFSCusts_Inserted">
            <DeleteParameters>
                <asp:Parameter Name="RFSCustNum" Type="Int32" />
                <asp:Parameter Name="Division" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="RFSCustNum" Type="Int32" />
                <asp:Parameter Name="Division" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="RFSCustNum" Type="Int32" />
                <asp:Parameter Name="Division" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
            </UpdateParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLDistricts" DbType="StringFixedLength" PropertyName="SelectedValue" DefaultValue="%" Name="DistrictID" />
            </SelectParameters>

        </asp:SqlDataSource>
    </div>
</asp:Content>
