<%@ Page Title="Setup Districts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupDistricts.aspx.cs" Inherits="Paperwork.SetupDistricts" EnableEventValidation="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        }; 

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, args) {
            saveDivPosition();
        });

        function saveDivPosition() {
            var intY = $("[id*=divDistsScroll]").scrollTop();
            $("[id*=HFScrollStore]").val(intY);
        }

        function restoreDivPosition() {
            var intY = $("[id*=HFScrollStore]").val();
            $("[id*=divDistsScroll]").scrollTop(intY);
        }


        $(function () {
            BindEvents();
        });

        function BindEvents() {
            // Reset the scrollbar to where it belongs
            restoreDivPosition();

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            //highlight the clients selected row in the lookup modal
            $("[id*=GridViewCustomers] tr").focusin(function () {
                $(this).siblings().children("td").removeClass("highlight");
                $(this).children("td").addClass("highlight");
            });
        };

        function higlightSelectedRow() {
            var gvSelectedIdx = $('#<%= hfSelectedIndex.ClientID %>').val();
            if (gvSelectedIdx != null) {
                $("[id*=GridViewCustomers] tr:eq(" + gvSelectedIdx + ")").children("td").addClass("highlight");
            };
        };
    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="UpdatePanel2" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-primary loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="UpdatePanelModal" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-primary loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdateProgress ID="UpdateProgress4" runat="server" AssociatedUpdatePanelID="UpdatePanelDistList" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-primary loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="hdrGroup" class="row">
        <div id="hdrDiv" class="col-sm-4">
            <h2><%: Title %></h2>
        </div>
        <div id="menuDiv" class="col-sm-4" style="padding-top:10px;">
            &nbsp;
        </div>
        <div id="hdrPrintButton" class="col-sm-4" style="padding-top:10px;text-align:right;">
            <asp:LinkButton ID="IPrint" Width="200" runat="server" 
            class="fas fa-print mGridLinkButton" 
            ToolTip="Print" Text=" District List" OnClick="IPrint_Click"></asp:LinkButton>
        </div>
    </div>
    <div id="DataEntry">
        <asp:HiddenField ID="hfAccordionIndex" runat="server" />
        <asp:HiddenField ID="HFScrollStore" runat="server" />
        <div class="panel-group" id="accordion" style="overflow-y:auto;background-color:white; position:absolute; width:93%; left:20px;">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="card">
                        <div class="card-header h4">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse1">
                                Create New District
                            </a>
                        </div>
                        <div id="cardCollapse1" class="collapse " >
                            <div class="card-body">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <p>
                                            New District:
                                        </p>
                                            <asp:DetailsView ID="DetailsViewNewDistrict" runat="server" DataSourceID="SqlDataSourceDistricts" AutoGenerateRows="False" DataKeyNames="DistrictID" CssClass="mGrid" Width="450"
                                                DefaultMode="Insert" >
                                                <HeaderStyle Font-Bold="true" />
                                                <Fields>
                                                    <asp:TemplateField HeaderText="District ID">
                                                        <InsertItemTemplate >
                                                            <asp:TextBox ID="TextboxDistID" runat="server" MaxLength="3" Text='<%# Bind("DistrictID") %>' ></asp:TextBox>
                                                        </InsertItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="DistrictName" HeaderText="District Name" SortExpression="DistrictName" />
                                                    <asp:BoundField DataField="DistrictAddress1" HeaderText="Address1" SortExpression="DistrictAddress1" />
                                                    <asp:BoundField DataField="DistrictAddress2" HeaderText="Address2" SortExpression="DistrictAddress2" />
                                                    <asp:BoundField DataField="DistrictCity" HeaderText="City" SortExpression="DistrictCity" />
                                                    <asp:BoundField DataField="DistrictState" HeaderText="State" SortExpression="DistrictState" />
                                                    <asp:BoundField DataField="DistrictZip" HeaderText="Zip" SortExpression="DistrictZip" />
                                                    <asp:BoundField DataField="DistrictContact" HeaderText="Contact" SortExpression="DistrictContact" />
                                                    <asp:BoundField DataField="DistrictFullName" HeaderText="Full Name" SortExpression="DistrictFullName" />
                                                    <asp:CheckBoxField DataField="AllowInvUSDAEdits" HeaderText="Allow USDA Edits" />
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
                    <div class="card">
                        <div class="card-header h4">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse2">
                                District List
                            </a>
                        </div>
                        <div id="cardCollapse2" class="collapse show" >
                            <div class="card-body">
                                <asp:UpdatePanel ID="UpdatePanelDistList" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row" id="divDistsScroll" runat="server" style="height:600px;overflow:auto;">
                                            
                                            <div class="col-sm-12" >
                                                <div class="messagealert" id="alert_container"></div><!-- Feedback for new invoice creation -->
                                                <div class="row">
                                                    <div class="col-2">
                                                        Filters:
                                                    </div>
                                                    <div class="col-3">
                                                        State:
                                                        <asp:TextBox ID="TextBoxState" runat="server" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-3">
                                                        Text:
                                                        <asp:TextBox runat="server" ID="TextBoxTextSearch" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <asp:GridView ID="GridViewDistricts" runat="server" AutoGenerateColumns="False" CssClass="mGrid" DataSourceID="SqlDataSourceDistricts" DataKeyNames="DistrictID"
                                                AllowSorting="True" Width="1500">
                                                <Columns>
                                                    <asp:BoundField DataField="DistrictID" HeaderText="District ID" SortExpression="DistrictID" ReadOnly="true" HeaderStyle-Width="75" />
                                                    <asp:BoundField DataField="DistrictName" HeaderText="Name" SortExpression="DistrictName" HeaderStyle-Width="100" ItemStyle-Width="100" ControlStyle-Width="100" />
                                                    <asp:TemplateField HeaderText="Address" SortExpression="Address" HeaderStyle-Width="225">
                                                        <EditItemTemplate>
                                                            <asp:Label ID="LabelAddr" runat="server" Text="Addr." Width="45"></asp:Label>
                                                            <asp:TextBox ID="TextBoxAddress1" runat="server" Text='<%# Bind("DistrictAddress1") %>'></asp:TextBox><br />
                                                            <asp:Label ID="Label2" runat="server" Text=" " Width="45"></asp:Label>
                                                            <asp:TextBox ID="TextBoxAddress2" runat="server" Text='<%# Bind("DistrictAddress2") %>'></asp:TextBox><br />

                                                            <asp:Label ID="Label1" runat="server" Text="City:" Width="45"></asp:Label>
                                                            <asp:TextBox ID="TextBoxCity" runat="server" Text='<%# Bind("DistrictCity") %>'></asp:TextBox><br />

                                                            <asp:Label ID="Label3" runat="server" Text="State:" Width="45"></asp:Label>
                                                            <asp:TextBox ID="TextBoxState" runat="server" Text='<%# Bind("DistrictState") %>' Width="35" MaxLength="2"></asp:TextBox>

                                                            <asp:Label ID="Label4" runat="server" Text="Zip:" Width="45"></asp:Label>
                                                            <asp:TextBox ID="TextBoxZip" runat="server" Text='<%# Bind("DistrictZip") %>' Width="50"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="LabelAddress1" runat="server" Text='<%# Eval("DistrictAddress1") %>' Visible='<%# Eval("DistrictAddress1").ToString().Trim()==string.Empty?false:true %>' ></asp:Label><br />
                                                            <asp:Label ID="LabelAddress2" runat="server" Text='<%# Eval("DistrictAddress2") %>' Visible='<%# Eval("DistrictAddress2").ToString().Trim()==string.Empty?false:true %>'></asp:Label><br />
                                                            <asp:Label ID="LabelCity" runat="server" Text='<%# Eval("DistrictCity") %>' Visible='<%# Eval("DistrictCity").ToString().Trim()==string.Empty?false:true %>'></asp:Label>
                                                            <asp:Label ID="LabelComma" runat="server" Text='<%# Eval("DistrictCity").ToString().Trim()==string.Empty? "":", " %>'></asp:Label>
                                                            <asp:Label ID="LabelState" runat="server" Text='<%# Eval("DistrictState") %>' Visible='<%# Eval("DistrictState").ToString().Trim()==string.Empty?false:true %>'></asp:Label>&nbsp;
                                                            <asp:Label ID="LabelZip" runat="server" Text='<%# Eval("DistrictZip") %>' Visible='<%# Eval("DistrictZip").ToString().Trim()==string.Empty?false:true %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="DistrictContact" HeaderText="Contact" SortExpression="DistrictContact" HeaderStyle-Width="150" ItemStyle-Width="150" ControlStyle-Width="150" />
                                                    <asp:BoundField DataField="DistrictFullName" HeaderText="Full Name" SortExpression="DistrictFullName" HeaderStyle-Width="150" ItemStyle-Width="150" ControlStyle-Width="150" />
                                                    <asp:TemplateField HeaderText="Enforce Period Closes" SortExpression="EnforcePeriodClosures">
                                                        <EditItemTemplate>
                                                            <asp:CheckBox ID="cbEnforcePeriodsEdit" runat="server" Checked='<%# (Eval("EnforcePeriodClosures").ToString())=="0"?false:true %>' OnCheckedChanged="CbEnforcePeriodsEdit_CheckedChanged" ToolTip="Enforce Period Closes" />
                                                            <asp:HiddenField ID="hfEnforcePeriods" runat="server" Value='<%# Bind("EnforcePeriodClosures") %>' />
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbEnforcePeriods" runat="server" Checked='<%# (Eval("EnforcePeriodClosures").ToString())=="0"?false:true %>' Enabled="false" ToolTip="Enforce Period Closes" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Inactive" SortExpression="Inactive">
                                                        <EditItemTemplate>
                                                            <asp:CheckBox ID="cbInactiveEdit" runat="server" Checked='<%# (Eval("Inactive").ToString())=="0"?false:true %>' OnCheckedChanged="cbInactiveEdit_CheckedChanged" ToolTip="Inactive" />
                                                            <asp:HiddenField ID="hfInactive" runat="server" Value='<%# Bind("Inactive") %>' />
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbInactive" runat="server" Checked='<%# (Eval("Inactive").ToString())=="0"?false:true %>' Enabled="false" ToolTip="Inactive" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Allow USDA Edits" SortExpression="AllowInvUSDAEdits">
                                                        <EditItemTemplate>
                                                            <asp:CheckBox ID="cbUSDAEdit" runat="server" Checked='<%# Bind("AllowInvUSDAEdits") %>' ToolTip="Allow USDA Edits" />
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="cbUSDA" runat="server" Checked='<%# Eval("AllowInvUSDAEdits") %>' Enabled="false" ToolTip="Allow USDA Edits" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="ERP Customer ID" SortExpression="CUSTNMBR" ItemStyle-Width="300">
                                                        <EditItemTemplate>
                                                            Company:
                                                            <asp:DropDownList ID="DDLCompanies" runat="server" SelectedValue='<%# Bind("INTERID") %>' DataSourceID="SqlDataSourceGPCompanies" DataTextField="CMPNYNAM" DataValueField="INTERID" Width="80" ></asp:DropDownList>
                                                            <br />
                                                            Customer:
                                                            <asp:TextBox ID="TextBoxCustomerNumber" runat="server" Text='<%# Bind("CUSTNMBR") %>' Enabled="false" Width="130"></asp:TextBox> 
                                                            <asp:LinkButton ID="buttonLookup" runat="server" CssClass="btn btn-info btn-sm fas fa-search" OnClick="buttonLookup_Click" />
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            Company:<asp:Label ID="LabelCompany" runat="server" Text='<%# Eval("INTERID") %>' Width="80"></asp:Label>
                                                            <br />
                                                            Customer:<asp:Label ID="LabelCustomer" runat="server" Text='<%# Eval("CUSTNMBR") %>' Width="130"></asp:Label>
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
                                                                OnClientClick="return confirm('Are you sure you want to delete this district?');" />
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="50" />
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div id="myModal" class="modal" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content - This is the popup window to select an existing customer number for the specified company-->
            <div class="modal-content" style="width:1200px;">
                <div class="modal-header">
                    <h4 class="modal-title">
                        ERP Customers &nbsp;
                    </h4>
                    <div style="float:right;">
                        <asp:Button runat="server" ID="buttonCloseModal"  class="btn btn-outline-primary" OnClick="buttonCloseModal_Click" Text="OK" UseSubmitBehavior="false" data-dismiss="modal" />
                        <button type="button" class="btn btn-outline-primary" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanelModal" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-sm-12">
                                    Search:
                                    <asp:TextBox ID="TextBoxSearch" runat="server" AutoPostBack="true" OnTextChanged="TextBoxSearch_TextChanged" ></asp:TextBox>
                                </div>
                            </div>
                            <asp:HiddenField ID="hfINTERID" runat="server" />
                            <asp:HiddenField ID="hfSelectedIndex" runat="server" />
                            <asp:GridView ID="GridViewCustomers" runat="server" DataSourceID="SqlDataSourceGPCustomers" CssClass="mGrid" AutoGenerateColumns="false" EnableViewState="true"
                                DataKeyNames="Custnmbr" AllowSorting="true" OnRowDataBound="GridViewCustomers_RowDataBound" OnSelectedIndexChanged="GridViewCustomers_SelectedIndexChanged"
                                OnSorting="GridViewCustomers_Sorting"
                                        >
                                <Columns>
                                    <asp:BoundField DataField="Custnmbr" HeaderText="Customer Number" ReadOnly="True" SortExpression="Custnmbr"  />
                                    <asp:BoundField DataField="Custname" HeaderText="Customer Name" ReadOnly="True" SortExpression="Custname"  />
                                    <asp:BoundField DataField="City" HeaderText="City" ReadOnly="True" SortExpression="City"  />
                                    <asp:BoundField DataField="State" HeaderText="State" ReadOnly="True" SortExpression="State"  />
                                </Columns>
                            </asp:GridView>
                            <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>



                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCloseModal"  class="btn btn-outline-primary" OnClick="buttonCloseModal_Click" Text="OK" UseSubmitBehavior="false" data-dismiss="modal" />
                    <button type="button" class="btn btn-outline-primary" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>



    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [District] WHERE [DistrictID] = @DistrictID" 
            InsertCommand="INSERT INTO [District] ([DistrictID], [DistrictName], [DistrictAddress1], [DistrictAddress2], [DistrictCity],[DistrictState], [DistrictZip], [DistrictContact], [DistrictFullName], [Inactive], [EnforcePeriodClosures],[AllowInvUSDAEdits],[INTERID],[CUSTNMBR]) 
              VALUES (@DistrictID, @DistrictName, @DistrictAddress1, @DistrictAddress2, @DistrictCity, @DistrictState, @DistrictZip, @DistrictContact, @DistrictFullName, @Inactive, @EnforcePeriodClosures, @AllowInvUSDAEdits, @INTERID, @CUSTNMBR)" 
            SelectCommand="SELECT [DistrictID], [DistrictName], [DistrictAddress1], [DistrictAddress2], [DistrictCity], [DistrictState], [DistrictZip], [DistrictContact], [DistrictFullName], [Inactive], [EnforcePeriodClosures] ,[AllowInvUSDAEdits], [INTERID], [CUSTNMBR]
              FROM [District]" 
            UpdateCommand="UPDATE [District] SET [DistrictName] = @DistrictName, [DistrictAddress1] = @DistrictAddress1, [DistrictAddress2] = @DistrictAddress2, [DistrictCity] = @DistrictCity, 
              [DistrictZip] = @DistrictZip, [DistrictContact] = @DistrictContact, [DistrictFullName] = @DistrictFullName, [Inactive] = @Inactive, [EnforcePeriodClosures] = @EnforcePeriodClosures, 
              [DistrictState]=@DistrictState, [AllowInvUSDAEdits]=@AllowInvUSDAEdits, [INTERID]=@INTERID, [CUSTNMBR]=@CUSTNMBR
              WHERE [DistrictID] = @DistrictID"
             OnInserting="SqlDataSourceDistricts_Inserting" OnInserted="SqlDataSourceDistricts_Inserted" OnUpdated="SqlDataSourceDistricts_Updated"
             FilterExpression="[DistrictState] LIKE '%{0}%' AND ([DistrictID] LIKE '%{1}%' OR [DistrictName] LIKE '%{1}%' OR [DistrictAddress1] LIKE '%{1}%' OR [DistrictAddress2] LIKE '%{1}%' OR [DistrictCity] LIKE '%{1}%' OR [DistrictContact] LIKE '%{1}%' OR [DistrictFullName] LIKE '%{1}%')">
            <DeleteParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="DistrictID" Type="String" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="DistrictName" Type="String" ConvertEmptyStringToNull="false" />
                <asp:Parameter Name="DistrictAddress1" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="DistrictAddress2" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="DistrictCity" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="DistrictState" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="DistrictZip" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="DistrictContact" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="DistrictFullName" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="Inactive" Type="Int16" DefaultValue="0" />
                <asp:Parameter Name="EnforcePeriodClosures" Type="Int16" DefaultValue="1" />
                <asp:Parameter Name="AllowInvUSDAEdits" Type="Byte" DefaultValue="0" />
                <asp:Parameter Name="INTERID" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="CUSTNMBR" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="DistrictName" Type="String" ConvertEmptyStringToNull="false" DefaultValue=""  />
                <asp:Parameter Name="DistrictAddress1" Type="String" ConvertEmptyStringToNull="false" DefaultValue=""  />
                <asp:Parameter Name="DistrictAddress2" Type="String" ConvertEmptyStringToNull="false" DefaultValue=""  />
                <asp:Parameter Name="DistrictCity" Type="String" ConvertEmptyStringToNull="false" DefaultValue=""  />
                <asp:Parameter Name="DistrictState" Type="String" ConvertEmptyStringToNull="false" DefaultValue=""  />
                <asp:Parameter Name="DistrictZip" Type="String" ConvertEmptyStringToNull="false" DefaultValue=""  />
                <asp:Parameter Name="DistrictContact" Type="String" ConvertEmptyStringToNull="false" DefaultValue=""  />
                <asp:Parameter Name="DistrictFullName" Type="String" ConvertEmptyStringToNull="false" DefaultValue=""  />
                <asp:Parameter Name="Inactive" Type="Int16" />
                <asp:Parameter Name="EnforcePeriodClosures" Type="Int16" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="AllowInvUSDAEdits" Type="Byte" DefaultValue="0" />
                <asp:Parameter Name="INTERID" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="CUSTNMBR" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
            </UpdateParameters>
            <FilterParameters>
                <asp:ControlParameter Name="StateFilter" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" ControlID="TextBoxState" PropertyName="Text"  />
                <asp:ControlParameter Name="TextFilter" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" ControlID="TextBoxTextSearch" PropertyName="Text"  />
            </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceGPCompanies" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" 
            DataSourceMode="DataReader" 
            SelectCommand="SELECT '' INTERID, '- SELECT -' CMPNYNAM 
            UNION ALL
            SELECT INTERID, CMPNYNAM FROM DYNAMICS..SY01500 WHERE CMPNYNAM NOT LIKE '%TEST%'
            ORDER BY CMPNYNAM">
        </asp:SqlDataSource>                            
        <asp:SqlDataSource ID="SqlDataSourceGPCustomers" runat="server" ConnectionString="<%$ ConnectionStrings:connBDGTS %>" 
            DataSourceMode="DataSet" OnFiltering="SqlDataSourceGPCustomers_Filtering" 
            FilterExpression="Custnmbr LIKE '%{0}%' OR Custname LIKE '%{0}%' OR [City] LIKE '%{0}%' OR [State] LIKE '%{0}%' ">
            <FilterParameters>
                <asp:ControlParameter Name="TextFilter" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" ControlID="TextBoxSearch" PropertyName="Text"  />
            </FilterParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
