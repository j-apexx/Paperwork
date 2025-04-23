<%@ Page Title="Setup Employees" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupEmployees.aspx.cs" Inherits="Paperwork.SetupEmployees" %>
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
        var topAndBottom = 200;
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

            $(function () {
                $("[id*=EditLifeAddDt]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $("[id*=InsertLifeAddDt]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $("[id*=EditLifeDeleteDt]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $("[id*=InsertLifeDeleteDt]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
        };

        function isMaxLen(o) {
            var nMaxLen = o.getAttribute ? parseInt(o.getAttribute("maxlength")) : "";
            if (o.getAttribute && o.value.length > nMaxLen) {
                o.value = o.value.substring(0, nMaxLen)
            }
        }

        function setMaxLength(e) {
            $(this).maxLength = e;
            
        };

        $('body').on('focus', 'input', function (e) {
            $(this)
                .one('mouseup', function () {
                    $(this).select();
                    return false;
                })
                .select();

            var fieldTitle = $(this).attr('title');

            switch (fieldTitle) {
                case "LastName":
                case "FirstName":
                case "MiddleName":
                    $(this).attr('maxlength', '30');
                    break;
            }

        });

    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <h2><%: Title %></h2>
    <div id="DataEntry">
        <div class="panel-group" id="accordion">
            
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:CheckBox ID="CBShowInactives" runat="server" Text="Show Inactives" Checked="false" AutoPostBack="true" />
                    <div id="GridViewHeightController" style="overflow-y:auto;" class="row">
                        <div class="col-sm-12">
                            <asp:GridView ID="GridViewEmployees" runat="server" DataSourceID="SqlDataSourceEmployees" CssClass="mGrid" 
                                EmptyDataText="No employees have been setup for this district." AutoGenerateColumns="False"
                                DataKeyNames="DistrictID,EmployeeNum" OnRowDataBound="GridViewEmployees_RowDataBound"
                                AllowSorting="True" ShowFooter="True" ShowHeaderWhenEmpty="True"
                                OnRowCommand="GridViewEmployees_RowCommand">

                                <Columns>
                                    <asp:TemplateField HeaderText="Employee Num" SortExpression="EmployeeNum">
                                        <EditItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("EmployeeNum") %>'></asp:Label>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("EmployeeNum") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="LabelNew" runat="server" ForeColor="Green" Text="*"></asp:Label>
                                            <asp:TextBox ID="TextBoxFEmployeeNum" runat="server" Width="50"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredEmpNum" runat="server" ControlToValidate="TextBoxFEmployeeNum" CssClass="validators"
                                                Display="Dynamic" ErrorMessage="Employee ID is required" Text="*" ValidationGroup="Insert"></asp:RequiredFieldValidator>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Last Name" SortExpression="LastName">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxELastName" runat="server" Text='<%# Bind("LastName") %>' MaxLength="30" Width="100"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredELastName" runat="server" ControlToValidate="TextBoxELastName" CssClass="validators"
                                                Display="Dynamic" ErrorMessage="Employee Last Name is required" Text="*" ValidationGroup="Edit"></asp:RequiredFieldValidator>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("LastName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFLastName" runat="server" MaxLength="30" Width="100"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFLastName" runat="server" ControlToValidate="TextBoxFLastName" CssClass="validators"
                                                Display="Dynamic" ErrorMessage="Employee Last Name is required" Text="*" ValidationGroup="Insert"></asp:RequiredFieldValidator>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="First Name" SortExpression="FirstName">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxEFirstName" runat="server" Text='<%# Bind("FirstName") %>' MaxLength="30" Width="100"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFFirstName" runat="server" MaxLength="30" Width="100"></asp:TextBox>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Middle Name" SortExpression="MiddleName">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxEMiddleName" runat="server" Text='<%# Bind("MiddleName") %>' MaxLength="30" Width="100"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("MiddleName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFMiddleName" runat="server" MaxLength="30" Width="100"></asp:TextBox>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="School" SortExpression="SchoolName">
                                        <ItemTemplate>
                                            <asp:Label ID="LabelSchoolName" runat="server" Text='<%# Eval("SchoolName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList ID="DropDownListESchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" 
                                                DataTextField="SchoolName" DataValueField="SchoolNum" TabIndex="2" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                                <asp:ListItem Text="--None Selected--" Value="0" Selected="True"  ></asp:ListItem>
                                            </asp:DropDownList>
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:DropDownList ID="DropDownListFSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" 
                                                DataTextField="SchoolName" DataValueField="SchoolNum" TabIndex="2" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                                <asp:ListItem Text="--None Selected--" Value="0" Selected="True"  ></asp:ListItem>
                                            </asp:DropDownList>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate1" SortExpression="Rate1">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxERate1" runat="server" Text='<%# Bind("Rate1") %>' Width="50"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("Rate1") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFRate1" runat="server" Width="50"></asp:TextBox>
                                        </FooterTemplate>
                                        <ControlStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate2" SortExpression="Rate2">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxERate2" runat="server" Text='<%# Bind("Rate2") %>' Width="50"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("Rate2") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFRate2" runat="server" Width="50"></asp:TextBox>
                                        </FooterTemplate>
                                        <ControlStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate3" SortExpression="Rate3">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxERate3" runat="server" Text='<%# Bind("Rate3") %>' Width="50"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("Rate3") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFRate3" runat="server" Width="50"></asp:TextBox>
                                        </FooterTemplate>
                                        <ControlStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate4" SortExpression="Rate4">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxERate4" runat="server" Text='<%# Bind("Rate4") %>' Width="50"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label8" runat="server" Text='<%# Bind("Rate4") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFRate4" runat="server" Width="50"></asp:TextBox>
                                        </FooterTemplate>
                                        <ControlStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate5" SortExpression="Rate5">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxERate5" runat="server" Text='<%# Bind("Rate5") %>' Width="50"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label9" runat="server" Text='<%# Bind("Rate5") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFRate5" runat="server" Width="50"></asp:TextBox>
                                        </FooterTemplate>
                                        <ControlStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Health" SortExpression="Health">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBoxEHealth" runat="server" Text='<%# Bind("Health") %>' Width="50"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label10" runat="server" Text='<%# Bind("Health") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="TextBoxFHealth" runat="server" Width="50"></asp:TextBox>
                                        </FooterTemplate>
                                        <ControlStyle Width="50px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Inactive" SortExpression="Inactive">
                                        <EditItemTemplate>
                                            <div style="text-align:center;">
                                                <asp:CheckBox ID="CBEInactive" runat="server" Checked='<%# Bind("Inactive") %>' />
                                            </div>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <div style="text-align:center;">
                                                <asp:CheckBox ID="CBInactive" runat="server" Checked='<%# Eval("Inactive") %>' Enabled="false" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                            <div style="width:60px;text-align:center;">
                                            <asp:LinkButton runat="server" CommandName="Update" ID="ISave" ToolTip="Save" 
                                                CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                            <asp:LinkButton runat="server" CommandName="Cancel" ID="ICancel" ToolTip="Cancel" 
                                                CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                            </div>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <div style="width:60px;text-align:center;">
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
                            <asp:ValidationSummary ID="SummaryEdit" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                                ValidationGroup="Edit" HeaderText="Edit Errors" BackColor="WhiteSmoke" Width="500" />
                            <asp:ValidationSummary ID="SummaryInsert" CssClass="validators" runat="server" DisplayMode="BulletList" ShowValidationErrors="true" 
                                ValidationGroup="Insert" HeaderText="Insert Errors" BackColor="WhiteSmoke" Width="500" />
                            
                        </div>
                                
                    </div>
                    <div class="messagealert" id="alert_container"></div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceEmployees" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            OldValuesParameterFormatString="old_{0}" OnUpdating="SqlDataSourceEmployees_Updating"
            OnInserted="SqlDataSourceEmployees_Inserted" OnInserting="SqlDataSourceEmployees_Inserting"
            OnDeleted="SqlDataSourceEmployees_Deleted" OnDeleting="SqlDataSourceEmployees_Deleting"
            DeleteCommand="DELETE FROM [Employees] WHERE [EmployeeNum] = @old_EmployeeNum AND [DistrictID] = @old_DistrictID" 
            InsertCommand="INSERT INTO [Employees] ([EmployeeNum], [DistrictID], [LastName], [FirstName], [MiddleName], [SchoolNum], 
            [Rate1], [Rate2], [Rate3], [Rate4], [Rate5], [Health], [LifeAmount], [LifeTotal], [LifeAddDate], [LifeDeleteDate], [ADDAmount], 
            [ADDTotal]) VALUES (@EmployeeNum, @DistrictID, @LastName, @FirstName, @MiddleName, @SchoolNum, @Rate1, @Rate2, 
            @Rate3, @Rate4, @Rate5, @Health, @LifeAmount, @LifeTotal, @LifeAddDate, @LifeDeleteDate, @ADDAmount, @ADDTotal)" 
            SelectCommand="SELECT E.[DistrictID],[EmployeeNum], [LastName], [FirstName], [MiddleName], 
            ISNULL(DS.[SchoolNum],0) SchoolNum, ISNULL(SchoolName, '') SchoolName,
            [Rate1], [Rate2], [Rate3], [Rate4], [Rate5], [Health], [LifeAmount], [LifeTotal], [ADDAmount], 
            CASE [LifeAddDate] WHEN '1/1/1900' THEN '' ELSE [LifeAddDate] END 'LifeAddDate', 
            CASE [LifeDeleteDate] WHEN '1/1/1900' THEN '' ELSE [LifeDeleteDate] END 'LifeDeleteDate', 
            [ADDTotal], E.[Inactive] 
            FROM [Employees] E 
            LEFT OUTER JOIN [DistrictSchools] DS ON E.DistrictID=DS.DistrictID AND E.SchoolNum=DS.SchoolNum
            WHERE (E.[DistrictID] = @DistrictID)
            UNION ALL
            SELECT '' DistrictID,0 EmployeeNum, '' LastName ,'' FirstName,'' MiddleName,0 SchoolNum, 
            '' SchoolName,0 [Rate1],0 [Rate2],0 [Rate3],0 [Rate4],0 [Rate5],0 [Health],0 [LifeAmount],0 [LifeTotal],0 [ADDAmount], 
            '1/1/1900' 'LifeAddDate', '1/1/1900' 'LifeDeleteDate', 0 [ADDTotal],CAST(0 AS bit) Inactive " 
            UpdateCommand="UPDATE [Employees] SET [LastName] = @LastName, [FirstName] = @FirstName, [MiddleName] = @MiddleName, 
            [SchoolNum] = @SchoolNum, [Rate1] = @Rate1, [Rate2] = @Rate2, [Rate3] = @Rate3, [Rate4] = @Rate4, [Rate5] = @Rate5, 
            [Health] = @Health, [LifeAmount] = @LifeAmount, [LifeTotal] = @LifeTotal, [LifeAddDate] = @LifeAddDate, 
            [LifeDeleteDate] = @LifeDeleteDate, [ADDAmount] = @ADDAmount, [ADDTotal] = @ADDTotal, [Inactive] = @Inactive 
            WHERE [EmployeeNum] = @old_EmployeeNum AND [DistrictID] = @old_DistrictID"
            FilterExpression="[Inactive]<={0}">
            <FilterParameters>
                <asp:ControlParameter ControlID="CBShowInactives" PropertyName="Checked" Type="Byte" />
            </FilterParameters>
            <DeleteParameters>
                <asp:Parameter Name="EmployeeNum" Type="Int32" />
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="EmployeeNum" Type="Int32" />
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:Parameter Name="LastName" Type="String" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="FirstName" Type="String" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="MiddleName" Type="String" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="SchoolNum" Type="Int16" DefaultValue="0" />
                <asp:Parameter Name="Rate1" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate2" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate3" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate4" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate5" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Health" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="LifeAmount" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="LifeTotal" Type="Decimal" DefaultValue="0" />
                <asp:Parameter DbType="Date" Name="LifeAddDate" DefaultValue="1/1/1900" ConvertEmptyStringToNull="false" />
                <asp:Parameter DbType="Date" Name="LifeDeleteDate" DefaultValue="1/1/1900" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="ADDAmount" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="ADDTotal" Type="Decimal" DefaultValue="0" />
            </InsertParameters>
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="LastName" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="FirstName" Type="String" ConvertEmptyStringToNull="false" DefaultValue=""/>
                <asp:Parameter Name="MiddleName" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="Rate1" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate2" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate3" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate4" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate5" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Health" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="LifeAmount" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="LifeTotal" Type="Decimal" DefaultValue="0" />
                <asp:Parameter DbType="Date" Name="LifeAddDate" DefaultValue="1/1/1900" ConvertEmptyStringToNull="false" />
                <asp:Parameter DbType="Date" Name="LifeDeleteDate" DefaultValue="1/1/1900" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="ADDAmount" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="ADDTotal" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Inactive" Type="Boolean" />
                <asp:Parameter Name="EmployeeNum" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID ORDER BY SortOrder, SchoolName">
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
