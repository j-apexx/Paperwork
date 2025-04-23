<%@ Page Title="Setup Schools" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupSchools.aspx.cs" Inherits="Paperwork.SetupSchools" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

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
                                DataSourceID="SqlDataSourceMyDistricts" DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> &nbsp;
                        </p>
                    </div>
                </div>
            </div>
            <div id="SchoolSection">
                <asp:GridView ID="GridViewSchools" runat="server" DataSourceID="SqlDataSourceSchools" CssClass="mGrid" AutoGenerateColumns="False" 
                    DataKeyNames="DistrictID, SchoolNum" Width="500" EmptyDataText="No Schools have been setup for this district." >
                    <Columns>
                        <asp:BoundField DataField="DistrictID" HeaderText="DistrictID" ReadOnly="True" SortExpression="DistrictID" Visible="False" />
                        <asp:BoundField DataField="SchoolNum" HeaderText="SchoolNum" ReadOnly="True" SortExpression="SchoolNum" Visible="False" />
                        <asp:BoundField DataField="SchoolName" HeaderText="School Name" SortExpression="SchoolName" />
                        <asp:TemplateField HeaderText="Inactive">
                            <ItemTemplate>
                                <div style="text-align:center;">
                                    <asp:CheckBox ID="CB_Inactive" runat="server" Checked='<%# Eval("Inactive").ToString()=="0"?false:true %>' OnCheckedChanged="CB_Inactive_CheckedChanged" AutoPostBack="true" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-Width="165">
                            <ItemTemplate>
                                <div style="min-width:165px;text-align:center;">
                                    <asp:LinkButton runat="server" CommandName="Edit"
                                        ID="IEdit" ToolTip="Edit" Text=" Edit"
                                        CssClass="fas fa-pencil-alt mGridLinkButton" ForeColor="#336699"    />
                                    <!--<asp:LinkButton runat="server" CommandName="Delete"
                                        ID="IDelete" ToolTip="Delete" Text="Delete"
                                        CssClass="fas fa-trash mGridLinkButton" ForeColor="Crimson"
                                        OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                                        -->
                                </div>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <div style="min-width:165px;text-align:center;">
                                    <asp:LinkButton runat="server" CommandName="Update"
                                        ID="ISave" ToolTip="Save" Text=" Save" TabIndex="21"
                                        CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="GridView" />
                                    <asp:LinkButton runat="server" CommandName="Cancel"
                                        ID="ICancel" ToolTip="Cancel" Text=" Cancel" TabIndex="21"
                                        CssClass="fas fa-undo glyphButton mGridLinkButton" ForeColor="Red" CausesValidation="false" />

                                </div>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <div id="NewSchool">
                <asp:DetailsView ID="DetailsViewSchoolEntry" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSourceSchools" Height="50px" Width="300px" Caption="Add a New School" 
                    DefaultMode="Insert" FieldHeaderStyle-Font-Bold="true" CssClass="mGrid" >
                    <HeaderStyle Font-Bold="true" />
                    <FieldHeaderStyle Font-Bold="True"></FieldHeaderStyle>
                    <Fields>
                        <asp:TemplateField HeaderText="School Name">
                            <InsertItemTemplate>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorSchoolName" runat="server" Display="Dynamic" ErrorMessage="School Name is required." ForeColor="#FF3300" ControlToValidate="textboxSchoolName" ValidationGroup="SchoolInsert">*</asp:RequiredFieldValidator>
                                <asp:TextBox ID="textboxSchoolName" runat="server" Text='<%# Bind("SchoolName") %>' CausesValidation="True" ValidationGroup="SchoolInsert" Width="150px" Wrap="False"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <InsertItemTemplate>
                                <asp:LinkButton runat="server" CommandName="Insert"
                                    ID="ISave" ToolTip="Add" Text="Add"
                                    CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="SchoolInsert" Width="100%" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceMyDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT DistrictID, RTRIM(DistrictName) + '-' + RTRIM(DistrictID) 'Desc'
                    FROM [District] WHERE DistrictID IN (SELECT DistrictID FROM [UserDistrictAccess] WHERE Userid=@UserID) OR (@AccessLevel<>'User')
                    ORDER BY [DistrictName]" >
                    <SelectParameters>
                        <asp:SessionParameter Name="UserID" SessionField="CurrentUser" Type="String" />
                        <asp:SessionParameter Name="AccessLevel" SessionField="AccessLevel" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SELECTCOMMAND="SELECT * FROM DistrictSchools WHERE DistrictID=@DistrictID ORDER BY SchoolName"
                    UPDATECOMMAND="UPDATE DistrictSchools SET SchoolName=@SchoolName WHERE DistrictID=@DistrictID AND SchoolNum=@SchoolNum "
                    INSERTCOMMAND="INSERT INTO DistrictSchools([DistrictID],[SchoolNum],[SchoolName],[Inactive]) 
                    VALUES(@DistrictID,(SELECT ISNULL(MAX(SchoolNum),0)+1 FROM DistrictSchools WHERE DistrictID=@DistrictID),@SchoolName,0 )" >
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="SchoolNum" Type="Int16" />
                        <asp:Parameter Name="SchoolName" Type="String" DefaultValue="Unnamed School" />
                    </UpdateParameters>
                    <InsertParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="SchoolName" Type="String" DefaultValue="Unnamed School" />
                    </InsertParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
