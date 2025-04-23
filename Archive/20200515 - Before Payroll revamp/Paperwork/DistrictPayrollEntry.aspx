<%@ Page Title="District Payroll Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DistrictPayrollEntry.aspx.cs" Inherits="Paperwork.DistrictPayrollEntry" %>
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

        $(function () {
            BindEvents();
        });

        function BindEvents() {
            $('#GridViewHeightController').css("height", h - topAndBottom);
            $('#GridViewHeightController').css("overflow", "auto");

            $(function () {
                $("[id*=TextBoxPayrollDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    beforeShowDay: highlightDays
                });
            });

            updateDatesArray();

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            //Enable Enter Key tabbing
            $('input,select,textarea').on("keypress", function (e) {
                /* ENTER PRESSED*/
                if (e.keyCode == 13) {
                    /* FOCUS ELEMENT */
                    var inputs = $(this).parents("form").eq(0).find(":input,select,textarea:visible");
                    var idx = inputs.index(this);

                    if (idx == inputs.length - 1) {
                        inputs[0].select()
                    }
                    else {
                        inputs[idx + 1].focus(); //  handles submit buttons
                        inputs[idx + 1].select();
                    }
                    return false;
                }
            });

        }

        //Date Functions for colorized calendar
        var dateArray;
        var dateArrayLocked;

        function highlightDays(date) {
            //if no dates, return immediately
            //if (dateArray.length === 0 && dateArrayLocked.length===0)
            //    return [true, '', ''];
            if (dateArray[0] != "") {
                for (var i = 0; i < dateArray.length; i++) {
                    //convert string element to date
                    var d = $.datepicker.parseDate("mm/dd/yy", dateArray[i]);

                    if (
                        (d.getDate() == date.getDate()) &&
                        (d.getMonth() == date.getMonth()) &&
                        (d.getFullYear() == date.getFullYear())
                    ) {
                        return [true, 'calendarHighlighted', 'Payroll Entered'];
                    }
                }
            }

            if (dateArrayLocked[0] != "") {
                for (var i = 0; i < dateArrayLocked.length; i++) {
                    //convert string element to date
                    var d = $.datepicker.parseDate("mm/dd/yy", dateArrayLocked[i]);

                    if (
                        (d.getDate() == date.getDate()) &&
                        (d.getMonth() == date.getMonth()) &&
                        (d.getFullYear() == date.getFullYear())
                    ) {
                        return [true, 'calendarLocked', 'Payroll Entered (Locked)'];
                    }
                }
            }

            return [true, '', ''];
        };

        function updateDatesArray() {
            dateArray = $('[id*=HFEnteredPayrollDates]').val().split(','); 
            dateArrayLocked = $('[id*=HFLockedPayrollDates]').val().split(',');
        };

        //apply highlight 
        $('body').on('focus', 'input,textarea,select', function (e) {
            $(this).addClass("highlight");
        });
        // remove highlight
        $('body').on('focusout', 'input,textarea,select', function (e) {
            $(this).removeClass("highlight");
        });

    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="DataEntry">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <div id="topSection" style="position:relative;" class="row">
                    <div id="hdrDiv"  class="col-sm-4">
                        <h2><%: Title %></h2>
                    </div>
                    <div id="menuDiv" class="col-sm-8">
                        <strong>Program Year:</strong>
                        <asp:Label ID="LabelProgramYear" runat="server"></asp:Label>
                        &nbsp;&nbsp; 
                        <asp:HiddenField ID="HFEnteredPayrollDates" runat="server" />
                        <asp:HiddenField ID="HFLockedPayrollDates" runat="server" />
                        <strong>Date:</strong>
                        <asp:TextBox ID="TextBoxPayrollDate" runat="server" Width="100" AutoPostBack="true" 
                            OnTextChanged="TextBoxDate_TextChanged"></asp:TextBox>
                        <br />
                        <strong>District:</strong>
                            <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                            </asp:DropDownList> &nbsp;

                    </div>


                </div>
                <div id="GridViewHeightController" style="overflow-y:auto;" class="row">
                    <div class="col-sm-12">
                        <asp:GridView ID="GridViewPayroll" runat="server" AutoGenerateColumns="False" DataKeyNames="Counter,DistrictID,PayDate" DataSourceID="SqlDataSourcePayroll" CssClass="mGrid"
                        ShowFooter="true" OnRowCommand="GridViewPayroll_RowCommand" ShowHeaderWhenEmpty="true" OnRowDataBound="GridViewPayroll_RowDataBound" AllowSorting="true" 
                        OnPreRender="GridViewPayroll_PreRender">
                        <Columns>
                            <asp:TemplateField HeaderText="Employee" SortExpression="EmpName">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DropDownListEmps" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceAllEmps" DataTextField="EmpName" 
                                        DataValueField="EmployeeNum" SelectedValue='<%# Bind("EmployeeNum") %>' AppendDataBoundItems="true" OnSelectedIndexChanged="DropDownListEmps_SelectedIndexChanged"
                                        >
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareDDLEEmps" runat="server" ValueToCompare="0" ControlToValidate="DropDownListEmps" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="Employee is required." ValidationGroup="Update">*</asp:CompareValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelEmpName" runat="server" Text='<%# Eval("EmpName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="DropDownListFEmps" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceActiveEmps" DataTextField="EmpName" 
                                        DataValueField="EmployeeNum" SelectedValue='<%# Bind("EmployeeNum") %>' AppendDataBoundItems="true" OnSelectedIndexChanged="DropDownListEmps_SelectedIndexChanged"
                                        >
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>

                                    <asp:CompareValidator ID="CompareDDLFEmps" runat="server" ValueToCompare="0" ControlToValidate="DropDownListFEmps" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="Employee is required." ValidationGroup="Insert">*</asp:CompareValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="School" SortExpression="SchoolNum">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DropDownListSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceAllSchools" DataTextField="SchoolName" 
                                        DataValueField="SchoolNum" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareDDLESchool" runat="server" ValueToCompare="0" ControlToValidate="DropDownListSchools" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="School is required." ValidationGroup="Update">*</asp:CompareValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelSchoolName" runat="server" Text='<%# Eval("SchoolName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="DropDownListFSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceActiveSchools" DataTextField="SchoolName" 
                                        DataValueField="SchoolNum" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareDDLFSchool" runat="server" ValueToCompare="0" ControlToValidate="DropDownListFSchools" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="School is required." ValidationGroup="Insert">*</asp:CompareValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Hours" SortExpression="Hours">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEditHours" runat="server" Text='<%# Bind("Hours") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelEditHours" runat="server" Text='<%# Eval("Hours") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFHours" runat="server" Text="" Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Category">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="DropDownListECodes" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourcePaycodes" DataTextField="PayCodeDesc" 
                                        DataValueField="PayCode" SelectedValue='<%# Bind("PayCode") %>' AppendDataBoundItems="true" OnSelectedIndexChanged="DropDownListPayCodes_SelectedIndexChanged" >
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareDDLECodes" runat="server" ValueToCompare="0" ControlToValidate="DropDownListECodes" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="Category is required." ValidationGroup="Update">*</asp:CompareValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <div style="text-align:center;">
                                        <asp:Label ID="TextBoxCategory" runat="server" Text='<%# Eval("PayCodeDesc") %>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="DropDownListFCodes" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourcePaycodes" DataTextField="PayCodeDesc" 
                                        DataValueField="PayCode" SelectedValue='<%# Bind("PayCode") %>' AppendDataBoundItems="true" OnSelectedIndexChanged="DropDownListPayCodes_SelectedIndexChanged" >
                                        <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareDDLFCodes" runat="server" ValueToCompare="0" ControlToValidate="DropDownListFCodes" Operator="GreaterThan" Display="Dynamic" 
                                        CssClass="validators" ErrorMessage="Category is required." ValidationGroup="Insert">*</asp:CompareValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Rate" SortExpression="Rate">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEditRate" runat="server" Text='<%# Bind("Rate") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelRate" runat="server" Text='<%# Eval("Rate") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFRate" runat="server" Text="" Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Other $" SortExpression="OtherPay">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEditOther" runat="server" Text='<%# Bind("OtherPay") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelOther" runat="server" Text='<%# Eval("OtherPay") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFOther" runat="server" Text="" Width="100"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Health" SortExpression="Health">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBoxEditHealth" runat="server" Text='<%# Bind("Health") %>' Width="100"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="LabelHealth" runat="server" Text='<%# Eval("Health") %>' Width="100"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TextBoxFHealth" runat="server" Text="" Width="100"></asp:TextBox>
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
                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Update" />
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
                    </div>
                    
                </div>
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceAllSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID ORDER BY SortOrder, SchoolName">
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceActiveSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID AND Inactive=0 ORDER BY SortOrder, SchoolName">
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourcePayroll" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
         SelectCommand="SELECT PAY.[Counter],PAY.[DistrictID],PAY.[EmployeeNum],PAY.[ProgramYear],PAY.[PayDate],PAY.[SchoolNum],PAY.[PayCode]
            ,PAY.[Hours],PAY.[Rate],PAY.[OtherPay],PAY.[Health],PAY.[Locked]
            , [SchoolName], CASE WHEN EMP.[Inactive]=1 THEN 'X-' ELSE '' END + [LastName] + ', ' + [FirstName] EmpName 
            ,ISNULL(PayCodeDesc,  RTRIM(CAST(PAY.PayCode AS Varchar(3))) ) PayCodeDesc
            FROM [Payroll] PAY
            LEFT OUTER JOIN [DistrictSchools] DS ON PAY.[DistrictID]=DS.[DistrictID] AND PAY.[SchoolNum]=DS.[SchoolNum]
            LEFT OUTER JOIN [Employees] EMP ON PAY.[DistrictID]=EMP.[DistrictID] AND PAY.[EmployeeNum]=EMP.[EmployeeNum]
            LEFT OUTER JOIN [DistrictPaycodes] DPC ON PAY.[DistrictID]=DPC.[DistrictID] AND PAY.[ProgramYear]=DPC.[ProgramYear] AND PAY.[PayCode]=DPC.[PayCode]
            WHERE PAY.DistrictID=@DistrictID AND PAY.PayDate=@PayDate
            UNION ALL --Needed for Insert Row
            SELECT 0,'',0,'','1/1/1900',0,0,0,0,0,0,CAST(0 AS bit) Locked
            ,'' SchoolName, '' EmpName, '' PayCodeDesc"
            UpdateCommand="UPDATE [Payroll] SET [EmployeeNum]=@EmployeeNum, SchoolNum=@SchoolNum, PayCode=@PayCode, Hours=@Hours, Rate=@Rate, OtherPay=@OtherPay, Health=@Health
            WHERE DistrictID=@DistrictID AND PayDate=@PayDate AND Counter=@Counter"
            InsertCommand="INSERT INTO [Payroll] ([DistrictID],[EmployeeNum],[ProgramYear],[PayDate],[SchoolNum],[PayCode],[Hours],[Rate],[OtherPay],[Health],[Locked]) 
            VALUES(@DistrictID,@EmployeeNum,@ProgramYear,@PayDate,@SchoolNum,@PayCode,@Hours,@Rate,@OtherPay,@Health,0)"
            OnInserting="SqlDataSourcePayroll_Inserting" OnInserted="SqlDataSourcePayroll_Inserted"
            DeleteCommand="DELETE [Payroll] WHERE DistrictID=@DistrictID AND PayDate=@PayDate AND Counter=@Counter AND Locked=0">
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:ControlParameter Name="PayDate" ControlID="TextBoxPayrollDate" PropertyName="Text" Type="DateTime" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="PayDate" Type="DateTime" />
                <asp:Parameter Name="Counter" Type="Int32" />
                <asp:Parameter Name="EmployeeNum" Type="Int32" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="PayCode" Type="Int16" />
                <asp:Parameter Name="Hours" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="OtherPay" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Health" Type="Decimal" DefaultValue="0" />
            </UpdateParameters>
            <InsertParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter Name="PayDate" ControlID="TextBoxPayrollDate" PropertyName="Text" Type="DateTime" />
                <asp:Parameter Name="EmployeeNum" Type="Int32" />
                <asp:Parameter Name="SchoolNum" Type="Int16" />
                <asp:Parameter Name="PayCode" Type="Int16" />
                <asp:Parameter Name="Hours" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Rate" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="OtherPay" Type="Decimal" DefaultValue="0" />
                <asp:Parameter Name="Health" Type="Decimal" DefaultValue="0" />
            </InsertParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceAllEmps" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT EmployeeNum, CASE WHEN Inactive=1 THEN 'X-' ELSE '' END + LastName + ', ' + FirstName EmpName FROM [Employees] WHERE DistrictID=@DistrictID ORDER BY LastName,FirstName ">
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceActiveEmps" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT EmployeeNum, LastName + ', ' + FirstName EmpName FROM [Employees] WHERE DistrictID=@DistrictID AND Inactive=0 ORDER BY LastName,FirstName ">
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourcePaycodes" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT [PayCode], [PayCodeDesc] FROM [DistrictPaycodes] WHERE DistrictID=@DistrictID AND ProgramYear=@ProgramYear ORDER BY PayCodeDesc ">
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
