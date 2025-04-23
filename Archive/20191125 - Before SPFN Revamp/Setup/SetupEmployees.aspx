<%@ Page Title="Setup Employees" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupEmployees.aspx.cs" Inherits="Paperwork.SetupEmployees" %>
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
            
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse2">
                                Employees
                            </a>
                        </div>
                        <div id="cardCollapse2" class="collapse show">
                            <div class="card-body" >
                                <asp:CheckBox ID="CBShowInactives" runat="server" Text="Show Inactives" Checked="false" AutoPostBack="true" />
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
                                        <asp:GridView ID="GridViewEmployees" runat="server" DataSourceID="SqlDataSourceEmployees" CssClass="mGrid" 
                                            EmptyDataText="No employees have been setup for this district." AutoGenerateColumns="False"
                                            DataKeyNames="DistrictID, EmployeeNum" OnRowDataBound="GridViewEmployees_RowDataBound"
                                            AllowSorting="true">

                                            <Columns>
                                                <asp:BoundField DataField="EmployeeNum" SortExpression="EmployeeNum" HeaderText="Employee Num" ReadOnly="true"/>
                                                <asp:BoundField DataField="LastName" SortExpression="LastName" HeaderText="Last Name" />
                                                <asp:BoundField DataField="FirstName" SortExpression="FirstName" HeaderText="First Name" />
                                                <asp:BoundField DataField="MiddleName" SortExpression="MiddleName" HeaderText="Middle Name" />
                                                <asp:TemplateField HeaderText="School" SortExpression="SchoolName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="LabelSchoolName" runat="server" Text='<%# Eval("SchoolName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList ID="DropDownListEditSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" 
                                                            DataTextField="SchoolName" DataValueField="SchoolNum" TabIndex="2" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                                            <asp:ListItem Text="--None Selected--" Value="0" Selected="True"  ></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Rate1" SortExpression="Rate1" HeaderText="Rate1" ControlStyle-Width="50" />
                                                <asp:BoundField DataField="Rate2" SortExpression="Rate2" HeaderText="Rate2" ControlStyle-Width="50" />
                                                <asp:BoundField DataField="Rate3" SortExpression="Rate3" HeaderText="Rate3" ControlStyle-Width="50" />
                                                <asp:BoundField DataField="Rate4" SortExpression="Rate4" HeaderText="Rate4" ControlStyle-Width="50" />
                                                <asp:BoundField DataField="Rate5" SortExpression="Rate5" HeaderText="Rate5" ControlStyle-Width="50" />
                                                <asp:BoundField DataField="Health" SortExpression="Health" HeaderText="Health" ControlStyle-Width="50" />
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
                                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="EditComm" />
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
                                                    <ItemStyle Width="50" />
                                                </asp:TemplateField>
                                            </Columns>

                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse1">
                                Create New Employee
                            </a>
                        </div>
                        <div id="cardCollapse1" class="collapse" >
                            <div class="card-body">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DetailsView ID="DetailsViewNewEmployee" runat="server" DataSourceID="SqlDataSourceEmployees" 
                                            AutoGenerateRows="False" DataKeyNames="EmployeeNum,DistrictID" CssClass="mGrid" Width="450px"
                                            DefaultMode="Insert" HeaderText="New Employee" >
                                            <HeaderStyle Font-Bold="true" />
                                            <Fields>
                                                <asp:BoundField DataField="EmployeeNum" HeaderText="EmployeeNum" ReadOnly="True" SortExpression="EmployeeNum" />
                                                <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName"  />
                                                <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                                                <asp:BoundField DataField="MiddleName" HeaderText="MiddleName" SortExpression="MiddleName" />
                                                <asp:TemplateField HeaderText="School">
                                                    <InsertItemTemplate>
                                                        <asp:DropDownList ID="DropDownListSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" 
                                                            DataTextField="SchoolName" DataValueField="SchoolNum" TabIndex="2" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                                            <asp:ListItem Text="--None Selected--" Value="0" Selected="True"  ></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </InsertItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Rate1" HeaderText="Rate1" SortExpression="Rate1" />
                                                <asp:BoundField DataField="Rate2" HeaderText="Rate2" SortExpression="Rate2" />
                                                <asp:BoundField DataField="Rate3" HeaderText="Rate3" SortExpression="Rate3" />
                                                <asp:BoundField DataField="Rate4" HeaderText="Rate4" SortExpression="Rate4" />
                                                <asp:BoundField DataField="Rate5" HeaderText="Rate5" SortExpression="Rate5" />
                                                <asp:BoundField DataField="Health" HeaderText="Health" SortExpression="Health" />
                                                <asp:TemplateField ShowHeader="false">
                                                    <InsertItemTemplate>
                                                        <asp:LinkButton runat="server" CommandName="Insert" ID="ISave" ToolTip="Add" Text=" Add"
                                                            CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="false" ValidationGroup="DistrictInsert" Width="100%" />
                                                    </InsertItemTemplate>
                                                </asp:TemplateField>
                                            </Fields>
                                        </asp:DetailsView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceEmployees" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            OldValuesParameterFormatString="old_{0}" OnUpdating="SqlDataSourceEmployees_Updating"
            DeleteCommand="DELETE FROM [Employees] WHERE [EmployeeNum] = @EmployeeNum AND [DistrictID] = @DistrictID" 
            InsertCommand="INSERT INTO [Employees] ([EmployeeNum], [DistrictID], [LastName], [FirstName], [MiddleName], [SchoolNum], 
            [Rate1], [Rate2], [Rate3], [Rate4], [Rate5], [Health], [LifeAmount], [LifeTotal], [LifeAddDate], [LifeDeleteDate], [ADDAmount], 
            [ADDTotal]) VALUES (@EmployeeNum, @DistrictID, @LastName, @FirstName, @MiddleName, @SchoolNum, @Rate1, @Rate2, 
            @Rate3, @Rate4, @Rate5, @Health, @LifeAmount, @LifeTotal, @LifeAddDate, @LifeDeleteDate, @ADDAmount, @ADDTotal)" 
            SelectCommand="SELECT E.[DistrictID],[EmployeeNum], [LastName], [FirstName], [MiddleName], E.[SchoolNum], 
            ISNULL(SchoolName, '') SchoolName,
            [Rate1], [Rate2], [Rate3], [Rate4], [Rate5], [Health], [LifeAmount], [LifeTotal], [ADDAmount], 
            CASE [LifeAddDate] WHEN '1/1/1900' THEN '' ELSE [LifeAddDate] END 'LifeAddDate', 
            CASE [LifeDeleteDate] WHEN '1/1/1900' THEN '' ELSE [LifeDeleteDate] END 'LifeDeleteDate', 
            [ADDTotal], E.[Inactive] 
            FROM [Employees] E 
            LEFT OUTER JOIN [DistrictSchools] DS ON E.DistrictID=DS.DistrictID AND E.SchoolNum=DS.SchoolNum
            WHERE (E.[DistrictID] = @DistrictID)" 
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
                <asp:Parameter Name="LastName" Type="String" />
                <asp:Parameter Name="FirstName" Type="String" />
                <asp:Parameter Name="MiddleName" Type="String" />
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
            SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID ORDER BY SchoolName">
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
