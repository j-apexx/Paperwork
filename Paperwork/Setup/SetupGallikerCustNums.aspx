<%@ Page Title="Setup Galliker Customer Numbers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupGallikerCustNums.aspx.cs" Inherits="Paperwork.SetupGallikerCustNums" %>
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
                <asp:GridView ID="GridViewGLKRCustNums" runat="server" AutoGenerateColumns="False" DataKeyNames="GLKRCustNum" 
                    DataSourceID="SqlDataSourceGLKRCusts" CssClass="mGrid" AllowSorting="True" 
                    PageSize="100" AllowPaging="True" PagerSettings-Mode="NumericFirstLast" PagerStyle-Width="100"  
                    PagerSettings-Position="TopAndBottom" PagerStyle-CssClass="mGridPager" Width="700px"
                    ShowFooter="True" OnRowCommand="GridViewGLKRCustNums_RowCommand" OnRowDataBound="GridViewGLKRCustNums_RowDataBound"
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
                        <asp:TemplateField HeaderText="GLKR Customer Num" SortExpression="GLKRCustNum">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEGLKRCustNum" runat="server" Text='<%# Bind("GLKRCustNum") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("GLKRCustNum") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFGLKRCustNum" runat="server" ></asp:TextBox>
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
        <asp:SqlDataSource ID="SqlDataSourceGLKRCusts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OldValuesParameterFormatString="old_{0}"
            DeleteCommand="DELETE FROM [GLKRCustomerNumbers] WHERE [GLKRCustNum] = @old_GLKRCustNum " 
            InsertCommand="INSERT INTO [GLKRCustomerNumbers] ([DistrictID], [SchoolNum], [GLKRCustNum]) VALUES (@DistrictID, @SchoolNum, @GLKRCustNum)" 
            SelectCommand="SELECT GLKR.DistrictID, GLKR.SchoolNum, GLKR.GLKRCustNum, ISNULL(SchoolName, GLKR.SchoolNum) SchoolName FROM [GLKRCustomerNumbers] GLKR
            LEFT OUTER JOIN DistrictSchools DS ON GLKR.DistrictID=DS.DistrictID AND GLKR.SchoolNum=DS.SchoolNum 
            WHERE GLKR.DistrictID LIKE @DistrictID
            UNION ALL SELECT '' DistrictID, 0 SchoolNum, 0 GLKRCustNum, '' SchoolName " 
            UpdateCommand="UPDATE [GLKRCustomerNumbers] SET [DistrictID]=@DistrictID, [SchoolNum]=@SchoolNum, [GLKRCustNum] = @GLKRCustNum
            WHERE [GLKRCustNum] = @old_GLKRCustNum "
            
            OnInserting="SqlDataSourceGLKRCusts_Inserting" OnInserted="SqlDataSourceGLKRCusts_Inserted">
            <DeleteParameters>
                <asp:Parameter Name="GLKRCustNum" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="GLKRCustNum" Type="Int32" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="GLKRCustNum" Type="Int32" />
            </UpdateParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLDistricts" DbType="String" PropertyName="SelectedValue" DefaultValue="%" Name="DistrictID" />
            </SelectParameters>

        </asp:SqlDataSource>
    </div>
</asp:Content>
