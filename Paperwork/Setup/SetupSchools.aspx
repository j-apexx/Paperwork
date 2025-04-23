<%@ Page Title="Setup Schools" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupSchools.aspx.cs" Inherits="Paperwork.SetupSchools" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            }
        }

        $(function () {
            BindEvents();
        });

        var dialogDel;

        function BindEvents() {
            $("[id*=GridViewSchools]").sortable({
                containment: $("[id*=GridViewSchools]"),
                items: 'tr:not(tr:first-child)',
                cursor: 'move',
                tolerance: 'pointer',
                opacity: 0.7,
                dropOnEmpty: false,
                start: function (e, ui) {
                    ui.item.addClass("selected");
                },
                stop: function (e, ui) {
                    ui.item.removeClass("selected");
                },
                update: function (e, ui) {
                    $("[id*=HFOrdChangePending]").val("1");
                }
            });

           
        };//BindEvents
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
                        <h2><asp:Label ID="LabelTitle" runat="server"></asp:Label></h2>
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
            <span style="color:darkred;">
            * Do not rename a school to another building name. This will cause the linked Budget and USDA buildings to be incorrect.
            </span>
            <br />
            <asp:HiddenField ID="HFOrdChangePending" runat="server" Value="0" />
            Click and drag to change sort order - then click the button to save ->
            <asp:LinkButton ID="ISaveOrder" runat="server" 
                    ToolTip="Save Order" Text=" Save Order" ForeColor="Green"
                    CssClass="fas fa-save mGridLinkButton" Width="130" OnClick="ISaveOrder_Click" ></asp:LinkButton>
            <div id="SchoolSection">
                <asp:GridView ID="GridViewSchools" runat="server" DataSourceID="SqlDataSourceSchools" CssClass="mGrid" AutoGenerateColumns="False" 
                    DataKeyNames="DistrictID, SchoolNum" Width="700" EmptyDataText="No Schools have been setup for this district." AllowSorting="false" 
                    OnRowEditing="GridViewSchools_RowEditing" OnRowDeleting="GridViewSchools_RowDeleting" OnPreRender="GridViewSchools_PreRender">
                    <Columns>
                        <asp:BoundField DataField="DistrictID" HeaderText="DistrictID" ReadOnly="True" SortExpression="DistrictID" Visible="False" />
                        <asp:BoundField DataField="SchoolNum" HeaderText="ID" ReadOnly="True" SortExpression="SchoolNum" Visible="True" />
                        <asp:TemplateField HeaderText="School Name">
                            <ItemTemplate>
                                <asp:Label ID="LabelSchoolName" runat="server" Text='<%# Eval("SchoolName") %>' Width="170px" ></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TBESchoolName" runat="server" Text='<%# Bind("SchoolName") %>' Width="170px" 
                                    Enabled='<%# Session["AccessLevel"].ToString().ToUpper()=="ADMIN" || Session["AccessLevel"].ToString().ToUpper()=="OFFICE" || Session["AccessLevel"].ToString().ToUpper()=="RM"?true:false %>' ></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="School Type">
                            <ItemTemplate>
                                <asp:DropDownList ID="DDLSchoolType" runat="server" SelectedValue='<%# Eval("SchoolType") %>' Enabled="false" OnDataBinding="DDLSchoolType_DataBinding" >
                                </asp:DropDownList>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLESchoolType" runat="server" SelectedValue='<%# Bind("SchoolType") %>' Enabled="true" OnDataBinding="DDLSchoolType_DataBinding" >
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Inactive">
                            <ItemTemplate>
                                <div style="text-align:center;">
                                    <asp:CheckBox ID="CB_Inactive" runat="server" Checked='<%# Eval("Inactive").ToString()=="0"?false:true %>' OnCheckedChanged="CB_Inactive_CheckedChanged" AutoPostBack="true" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="RFSCustNum" HeaderText="RFS Cust Num" ReadOnly="True" SortExpression="RFSCustNum" Visible="True" />
                        <asp:TemplateField HeaderStyle-Width="165">
                            <ItemTemplate>
                                <div style="min-width:165px;text-align:center;">
                                    <asp:LinkButton runat="server" CommandName="Edit"
                                        ID="IEdit" ToolTip="Edit" Text=" Edit"
                                        CssClass="fas fa-pencil-alt mGridLinkButton" ForeColor="#336699"     />
                                    <asp:LinkButton runat="server" CommandName="Delete"
                                        ID="IDelete" ToolTip="Delete" Text="Delete"
                                        CssClass="fas fa-trash mGridLinkButton" ForeColor="Crimson"
                                        OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                                        
                                    <input type="hidden" name="HFSchoolNum" value='<%# Eval("SchoolNum") %>' />
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
                                    <input type="hidden" name="HFSchoolNum" value='<%# Eval("SchoolNum") %>' />
                                </div>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <div id="NewSchool" >
                <br />
                <asp:Label ID="LabelNewSection" runat="server" Text="New schools will also be created in budgets & in the USDA Web App."></asp:Label>
                <br />
                <asp:DetailsView ID="DetailsViewSchoolEntry" runat="server" AutoGenerateRows="False" DataSourceID="SqlDataSourceSchools" Height="50px" Width="300px" 
                    DefaultMode="Insert" FieldHeaderStyle-Font-Bold="true" CssClass="mGrid" 
                    HeaderText="Add a New School" HeaderStyle-BackColor="#6c93be" HeaderStyle-HorizontalAlign="Center" OnPreRender="DetailsViewSchoolEntry_PreRender"  >
                    
                    <HeaderStyle Font-Bold="true" />
                    <FieldHeaderStyle Font-Bold="True"></FieldHeaderStyle>
                    <Fields>
                        <asp:TemplateField HeaderText="School Name">
                            <InsertItemTemplate>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatorSchoolName" runat="server" Display="Dynamic" ErrorMessage="School Name is required." ForeColor="#FF3300" ControlToValidate="textboxSchoolName" ValidationGroup="SchoolInsert">*</asp:RequiredFieldValidator>
                                <asp:TextBox ID="textboxSchoolName" runat="server" Text='<%# Bind("SchoolName") %>' CausesValidation="True" ValidationGroup="SchoolInsert" Width="150px" Wrap="False"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="School Type">
                            <InsertItemTemplate>
                                <asp:DropDownList ID="DDLISchoolType" runat="server" SelectedValue='<%# Bind("SchoolType") %>' Enabled="true"  OnDataBinding="DDLSchoolType_DataBinding" >
                                </asp:DropDownList>
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
                <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SELECTCOMMAND="SELECT DS.*, ISNULL(R.RFSCustNum,'') RFSCustNum FROM DistrictSchools DS
                    LEFT OUTER JOIN (
                        SELECT SUBSTRING((SELECT ', '+RTRIM(RFSCustNum) AS [text()] 
                        FROM (SELECT Distinct RFSCustNum, DistrictID, SchoolNum FROM RFSCustomerNumbers) RFSCustomerNumbers WHERE DistrictID=R.DistrictID AND SchoolNum=R.SchoolNum
                        For XML Path('') ),2,2000) RFSCustNum, [DistrictID],[SchoolNum] FROM [RFSCustomerNumbers] R GROUP BY DistrictID, SchoolNum
                    )R ON DS.DistrictID=R.DistrictID AND DS.SchoolNum=R.SchoolNum
                    WHERE DS.DistrictID=@DistrictID ORDER BY DS.SortOrder, DS.SchoolName"
                    UPDATECOMMAND="UPDATE DistrictSchools SET SchoolName=@SchoolName, SchoolType=@SchoolType WHERE DistrictID=@DistrictID AND SchoolNum=@SchoolNum "
                    INSERTCOMMAND="DECLARE @table table (id int)
                        INSERT INTO DistrictSchools([DistrictID],[SchoolNum],[SchoolName],[Inactive],[SchoolType]) 
                        OUTPUT inserted.[SchoolNum] INTO @table
                        VALUES(@DistrictID,(SELECT ISNULL(MAX(SchoolNum),0)+1 FROM DistrictSchools WHERE DistrictID=@DistrictID),@SchoolName,0, @SchoolType )
                        SELECT @SchoolNum=id from @table;"
                    DELETECOMMAND="DELETE [DistrictSchools] WHERE DistrictID=@DistrictID AND SchoolNum=@SchoolNum; 
                        DELETE [DistrictMilkDeliveryInfo] WHERE DistrictID=@DistrictID AND SchoolNum=@SchoolNum; 
                        DELETE [DistrictSchoolYearSetup] WHERE DistrictID=@DistrictID AND SchoolNum=@SchoolNum; 
                        DELETE [RFSCustomerNumbers] WHERE DistrictID=@DistrictID AND SchoolNum=@SchoolNum; "
                    
                    OnInserting="SqlDataSourceSchools_Inserting" OnDeleted="SqlDataSourceSchools_Deleted"
                    OnInserted="SqlDataSourceSchools_Inserted" OnUpdated="SqlDataSourceSchools_Updated">
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="SchoolNum" Type="Int16" />
                        <asp:Parameter Name="SchoolName" Type="String" DefaultValue="Unnamed School" />
                        <asp:Parameter Name="SchoolType" Type="Int16" DefaultValue="1" />
                    </UpdateParameters>
                    <InsertParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="SchoolName" Type="String" DefaultValue="Unnamed School" />
                        <asp:Parameter Name="SchoolType" Type="Int16" DefaultValue="1" />
                        <asp:Parameter Name="SchoolNum" Type="Int16" Direction="Output"  />
                    </InsertParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="SchoolNum" Type="Int16" />
                    </DeleteParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
