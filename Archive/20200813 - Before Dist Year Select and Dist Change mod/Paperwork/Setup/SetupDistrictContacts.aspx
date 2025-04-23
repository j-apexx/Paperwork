<%@ Page Title="District Contacts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupDistrictContacts.aspx.cs" Inherits="Paperwork.SetupDistrictContacts" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        // For gridview resizing
        var h = $(window).height();
        var topAndBottom = 180;
        var timer,
            $win = $(window);

        //when window resizes
        $win.on('resize', function () {
            //update the height variable
            h = $(window).height();
            clearTimeout(timer);
            timer = setTimeout(function () {
                $("#GridViewHeightController").height(h - topAndBottom);
            }, 200);

        });


        $( function() {
            BindEvents();
        });

        function BindEvents() {
            $('#GridViewHeightController').css("height", h - topAndBottom);
            $('#GridViewHeightController').css("overflow", "auto");

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            $("[id*=DDLFTitles]").change(function (e) {
                if ($(this).val() === "Other") {
                    $("[id*=TextBoxFTitle]").val("");
                    $("#TitleHiderF").css('visibility', 'visible');
                    $("[id*=TextBoxFTitle]").focus();
                }
                else {
                    $("#TitleHiderF").css('visibility', 'hidden');
                    $("[id*=TextBoxFTitle]").val($(this).val());
                }
            });

            $("[id*=DDLETitles]").change(function (e) {
                if ($(this).val() === "Other") {
                    $("[id*=TextBoxETitle]").val("");
                    $("#TitleHiderE").css('visibility', 'visible');
                    $("[id*=TextBoxETitle]").focus();
                }
                else {
                    $("#TitleHiderE").css('visibility', 'hidden');
                    $("[id*=TextBoxETitle]").val($(this).val());
                }
            });

            $("[id*=DDLEPositions]").change(function (e) {
                if ($(this).val() === "99") {
                    $("[id*=TextBoxEPosition]").val("");
                    $("#PositionHiderE").css('visibility', 'visible');
                    $("[id*=TextBoxEPosition]").focus();
                }
                else {
                    $("#PositionHiderE").css('visibility', 'hidden');
                    $("[id*=TextBoxEPosition]").val($(this).children(":selected").text());
                }
            });

            $("[id*=DDLFPositions]").change(function (e) {
                if ($(this).val() === "99") {
                    $("[id*=TextBoxFPosition]").val("");
                    $("#PositionHiderF").css('visibility', 'visible');
                    $("[id*=TextBoxFPosition]").focus();
                }
                else {
                    $("#PositionHiderF").css('visibility', 'hidden');
                    $("[id*=TextBoxFPosition]").val($(this).children(":selected").text());
                }
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
            <div id="topSection" style="height:50px" class="row">
                <div id="hdrgroup" class="col-sm-12">
                    <div class="row">
                        <div id="hdrDiv" class="col-lg-4 col-sm-5 col-xs-6" >
                            <h2><%: Title %></h2>
                        </div>
                        <div id="menuDiv" style="padding-top:10px;" class="col-lg-8 col-sm-7 col-xs-6">
                                <strong>Program Year:</strong>
                                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                    DataValueField="ProgramYear" AutoPostBack="true" ></asp:DropDownList>
                                &nbsp;&nbsp;
                                <strong>District:</strong>
                                <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                    DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                    OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                                </asp:DropDownList> 
                                &nbsp;&nbsp;
                        
                        </div>
                    </div>
                    
                </div>
            </div>
<%--            <div id="DistrictContactsSection" >--%>
            <div id="GridViewHeightController" style="overflow-y:auto;" class="row">
                <asp:GridView ID="GridViewContacts" runat="server" CssClass="mGrid" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,ID" 
                    DataSourceID="SqlDataSourceContacts" ShowHeader="true" ShowHeaderWhenEmpty="true" ShowFooter="true"
                    OnRowCommand="GridViewContacts_RowCommand" OnRowDataBound="GridViewContacts_RowDataBound"
                    OnPreRender="GridViewContacts_PreRender">
                    <Columns>
                        <asp:TemplateField HeaderText="Title" SortExpression="Title">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLETitles" runat="server" >
                                    <asp:ListItem Text="-Select-" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Atty." Value="Atty."></asp:ListItem>
                                    <asp:ListItem Text="Dr." Value="Dr."></asp:ListItem>
                                    <asp:ListItem Text="Fr." Value="Fr."></asp:ListItem>
                                    <asp:ListItem Text="Mr." Value="Mr."></asp:ListItem>
                                    <asp:ListItem Text="Mrs." Value="Mrs."></asp:ListItem>
                                    <asp:ListItem Text="Ms." Value="Ms."></asp:ListItem>
                                    <asp:ListItem Text="Msgr." Value="Msgr."></asp:ListItem>
                                    <asp:ListItem Text="Rev." Value="Rev."></asp:ListItem>
                                    <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
                                </asp:DropDownList>
                                <div id="TitleHiderE" style="visibility:hidden;">
                                    <asp:TextBox ID="TextBoxETitle" runat="server"  MaxLength="10" Width="50" Text='<%# Bind("Title") %>'></asp:TextBox>
                                </div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelTitle" runat="server" Text='<%# Bind("Title") %>' Width="30"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="DDLFTitles" runat="server" >
                                    <asp:ListItem Text="-Select-" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Atty." Value="Atty."></asp:ListItem>
                                    <asp:ListItem Text="Dr." Value="Dr."></asp:ListItem>
                                    <asp:ListItem Text="Fr." Value="Fr."></asp:ListItem>
                                    <asp:ListItem Text="Mr." Value="Mr."></asp:ListItem>
                                    <asp:ListItem Text="Mrs." Value="Mrs."></asp:ListItem>
                                    <asp:ListItem Text="Ms." Value="Ms."></asp:ListItem>
                                    <asp:ListItem Text="Msgr." Value="Msgr."></asp:ListItem>
                                    <asp:ListItem Text="Rev." Value="Rev."></asp:ListItem>
                                    <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
                                </asp:DropDownList>
                                <div id="TitleHiderF" style="visibility:hidden;">
                                    <asp:TextBox ID="TextBoxFTitle" runat="server"  MaxLength="10" Width="50" ></asp:TextBox>
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="First Name" SortExpression="FirstName">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEFName" runat="server" Text='<%# Bind("FirstName") %>' MaxLength="50" Width="100"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelFName" runat="server" Text='<%# Bind("FirstName") %>' Width="100"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFFName" runat="server" MaxLength="50" Width="100"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Last Name" SortExpression="LastName">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxELName" runat="server" Text='<%# Bind("LastName") %>' MaxLength="50" Width="100"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelLName" runat="server" Text='<%# Bind("LastName") %>' Width="100"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFLName" runat="server" MaxLength="50" Width="100"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email" SortExpression="Email">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEEmail" runat="server" Text='<%# Bind("Email") %>' MaxLength="50"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="regexEmailValid" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="TextBoxEEmail" ErrorMessage="Invalid Email Format" 
                                    ValidationGroup="EditContact" CssClass="validators" Display="Dynamic" Text="*"></asp:RegularExpressionValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelEmail" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFEmail" runat="server" MaxLength="50" ></asp:TextBox>
                                <asp:RegularExpressionValidator ID="regexEmailValid" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="TextBoxFEmail" ErrorMessage="Invalid Email Format" 
                                    ValidationGroup="InsertContact" CssClass="validators" Display="Dynamic" Text="*"></asp:RegularExpressionValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Position" SortExpression="Position">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DDLEPositions" runat="server" DataSourceID="SqlDataSourcePositions" DataTextField="Position" DataValueField="ID"
                                    AppendDataBoundItems="true">
                                    <asp:ListItem Text="-Select-" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <div id="PositionHiderE" style="visibility:hidden;">
                                    <asp:TextBox ID="TextBoxEPosition" runat="server" Text='<%# Bind("Position") %>' MaxLength="50"></asp:TextBox>
                                </div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelPosition" runat="server" Text='<%# Bind("Position") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="DDLFPositions" runat="server" DataSourceID="SqlDataSourcePositions" DataTextField="Position" DataValueField="ID"
                                    AppendDataBoundItems="true">
                                    <asp:ListItem Text="-Select-" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <div id="PositionHiderF" style="visibility:hidden;">
                                    <asp:TextBox ID="TextBoxFPosition" runat="server" MaxLength="50"></asp:TextBox>
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Building" SortExpression="Building">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEBuilding" runat="server" Text='<%# Bind("Building") %>' MaxLength="50" Width="100"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelBuilding" runat="server" Text='<%# Bind("Building") %>' Width="100"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFBuilding" runat="server" MaxLength="50" Width="100"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Note" SortExpression="Note">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxENote" runat="server" Text='<%# Bind("Note") %>' MaxLength="100" Width="100"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelNote" runat="server" Text='<%# Bind("Note") %>' Width="100"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFNote" runat="server" MaxLength="100" Width="100"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Work Phone" SortExpression="WorkPhone">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEWorkPhone" runat="server" Text='<%# Bind("WorkPhone") %>' MaxLength="50" Width="100"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelWPhone" runat="server" Text='<%# Bind("WorkPhone") %>' Width="100"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFWorkPhone" runat="server" MaxLength="50" Width="100"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cell Phone" SortExpression="CellPhone">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxECellPhone" runat="server" Text='<%# Bind("CellPhone") %>' MaxLength="50" Width="100"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelCellPhone" runat="server" Text='<%# Bind("CellPhone") %>' Width="100"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFCellPhone" runat="server" MaxLength="50" Width="100"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Address" SortExpression="Address">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEAddress" runat="server" Text='<%# Bind("Address") %>' MaxLength="50" Width="200"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelAddress" runat="server" Text='<%# Bind("Address") %>' Width="200"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFAddress" runat="server" MaxLength="50" Width="200"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="City" SortExpression="City">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxECity" runat="server" Text='<%# Bind("City") %>' MaxLength="50" Width="200"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelCity" runat="server" Text='<%# Bind("City") %>' Width="100"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFCity" runat="server" MaxLength="50" Width="100"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="State" SortExpression="State">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEState" runat="server" Text='<%# Bind("State") %>' MaxLength="2" Width="30"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelState" runat="server" Text='<%# Bind("State") %>' Width="30"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFState" runat="server" MaxLength="2" Width="30"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Zip" SortExpression="Zip">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBoxEZip" runat="server" Text='<%# Bind("Zip") %>' MaxLength="10" Width="50"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelZip" runat="server" Text='<%# Bind("Zip") %>' Width="50"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TextBoxFZip" runat="server" MaxLength="10" Width="50"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                    <div style="width:60px;text-align:center;" >
                                        <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="EditContact" />
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
                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="InsertContact" />
                                        <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancel" ToolTip="Cancel" 
                                            CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                    </div>
                                </FooterTemplate>
                        
                                <ItemStyle Width="60" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:ValidationSummary ID="SummaryEdit" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                    ValidationGroup="EditContact" HeaderText="Editing Errors" BackColor="WhiteSmoke" Width="500" />
                <asp:ValidationSummary ID="SummaryInsert" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                    ValidationGroup="InsertContact" HeaderText="Insert Errors" BackColor="WhiteSmoke" Width="500" />
                <div class="messagealert" id="alert_container"></div>
            </div>
            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC">
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceContacts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    DeleteCommand="DELETE FROM [DistrictYearContacts] WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND [ID] = @ID" 
                    InsertCommand="INSERT INTO [DistrictYearContacts] ([ProgramYear], [DistrictID], [Title], [FirstName], [LastName], [Email], [Position], [DateAdded], [CellPhone], 
                    [WorkPhone], [Zip], [City], [Address], [State], [Building], [Note]) 
                    VALUES (@ProgramYear, @DistrictID, @Title, @FirstName, @LastName, @Email, @Position, CAST(GetDate() AS Date), @CellPhone, 
                    @WorkPhone, @Zip, @City, @Address, @State, @Building, @Note)" 
                    
                    SelectCommand="SELECT [ProgramYear], [DistrictID], [ID], [Title], [FirstName], [LastName], [Email], [Position], [DateAdded], [CellPhone], [WorkPhone], 
                    [Zip], [City], [Address], [State], [Building], [Note] 
                    FROM [DistrictYearContacts] 
                    WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID))
                    UNION ALL
                    SELECT ''[ProgramYear], ''[DistrictID], 0[ID], ''[Title], ''[FirstName], ''[LastName], ''[Email], ''[Position], '1/1/1900'[DateAdded], ''[CellPhone], ''[WorkPhone], 
                    ''[Zip], ''[City], ''[Address], ''[State], ''[Building], ''[Note] " 
                    
                    UpdateCommand="UPDATE [DistrictYearContacts] SET [Title] = @Title, [FirstName] = @FirstName, [LastName] = @LastName, [Email] = @Email, [Position] = @Position, 
                    [CellPhone] = @CellPhone, [WorkPhone] = @WorkPhone, [Zip] = @Zip, [City] = @City, [Address] = @Address, [State] = @State, 
                    [Building] = @Building, [Note] = @Note, [DateModified]=CAST(Getdate() AS Date)
                    WHERE [ProgramYear] = @ProgramYear AND [DistrictID] = @DistrictID AND [ID] = @ID"
                    OnInserted="SqlDataSourceContacts_Inserted" OnDeleted="SqlDataSourceContacts_Deleted" OnInserting="SqlDataSourceContacts_Inserting">
                    <DeleteParameters>
                        <asp:Parameter Name="ProgramYear" Type="String" />
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="ID" Type="Int32" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                        <asp:Parameter Name="Title" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="FirstName" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="LastName" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Email" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Position" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="CellPhone" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="WorkPhone" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Zip" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="City" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Address" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="State" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Building" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        <asp:Parameter Name="Note" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                    </InsertParameters>
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="Title" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="FirstName" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="LastName" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="Email" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="Position" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="CellPhone" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="WorkPhone" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="Zip" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="City" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="Address" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="State" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="Building" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="Note" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"  />
                        <asp:Parameter Name="ProgramYear" Type="String"   />
                        <asp:Parameter Name="DistrictID" Type="String"  />
                        <asp:Parameter Name="ID" Type="Int32" />
                    </UpdateParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourcePositions" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    DataSourceMode="DataReader" SelectCommand="SELECT * FROM [ContactPositions] ORDER BY [ID]">
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
