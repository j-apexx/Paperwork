<%@ Page Title="Setup Districts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupDistricts.aspx.cs" Inherits="Paperwork.SetupDistricts" %>
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
            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });
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
    <h2><%: Title %></h2>
    <div id="DataEntry">
        <asp:HiddenField ID="hfAccordionIndex" runat="server" />
        <div class="panel-group" id="accordion">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="card">
                        <div class="card-header h4">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse1">
                                Create New District
                            </a>
                        </div>
                        <div id="cardCollapse1" class="collapse show" >
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
                                <asp:UpdatePanel ID="UpdatePanelInvoices" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
                                        <div class="messagealert" id="alert_container"></div><!-- Feedback for new invoice creation -->


                                        <asp:GridView ID="GridViewDistricts" runat="server" AutoGenerateColumns="False" CssClass="mGrid" DataSourceID="SqlDataSourceDistricts" DataKeyNames="DistrictID"
                                            AllowSorting="True">
                                            <Columns>
                                                <asp:BoundField DataField="DistrictID" HeaderText="District ID" SortExpression="DistrictID" ReadOnly="true" />
                                                <asp:BoundField DataField="DistrictName" HeaderText="Name" SortExpression="DistrictName" />
                                                <asp:TemplateField HeaderText="Address" SortExpression="Address">
                                                    <EditItemTemplate>
                                                        <asp:Label ID="LabelAddr" runat="server" Text="Address:" Width="50"></asp:Label>
                                                        <asp:TextBox ID="TextBoxAddress1" runat="server" Text='<%# Bind("DistrictAddress1") %>'></asp:TextBox><br />

                                                        <asp:Label ID="Label2" runat="server" Text="" Width="50"></asp:Label>
                                                        <asp:TextBox ID="TextBoxAddress2" runat="server" Text='<%# Bind("DistrictAddress2") %>'></asp:TextBox><br />

                                                        <asp:Label ID="Label1" runat="server" Text="City:" Width="50"></asp:Label>
                                                        <asp:TextBox ID="TextBoxCity" runat="server" Text='<%# Bind("DistrictCity") %>'></asp:TextBox>,<br />

                                                        <asp:Label ID="Label3" runat="server" Text="State:" Width="50"></asp:Label>
                                                        <asp:TextBox ID="TextBoxState" runat="server" Text='<%# Bind("DistrictState") %>' Width="35" MaxLength="2"></asp:TextBox><br />

                                                        <asp:Label ID="Label4" runat="server" Text="Zip:" Width="50"></asp:Label>
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
                                                <asp:BoundField DataField="DistrictContact" HeaderText="Contact" SortExpression="DistrictContact" />
                                                <asp:BoundField DataField="DistrictFullName" HeaderText="Full Name" SortExpression="DistrictFullName" />
                                                <asp:TemplateField HeaderText="Enforce Period Closes" SortExpression="EnforcePeriodClosures">
                                                    <EditItemTemplate>
                                                        <asp:CheckBox ID="cbEnforcePeriodsEdit" runat="server" Checked='<%# (Eval("EnforcePeriodClosures").ToString())=="0"?false:true %>' OnCheckedChanged="CbEnforcePeriodsEdit_CheckedChanged" />
                                                        <asp:HiddenField ID="hfEnforcePeriods" runat="server" Value='<%# Bind("EnforcePeriodClosures") %>' />
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbEnforcePeriods" runat="server" Checked='<%# (Eval("EnforcePeriodClosures").ToString())=="0"?false:true %>' Enabled="false" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Inactive" SortExpression="Inactive">
                                                    <EditItemTemplate>
                                                        <asp:CheckBox ID="cbInactiveEdit" runat="server" Checked='<%# (Eval("Inactive").ToString())=="0"?false:true %>' OnCheckedChanged="cbInactiveEdit_CheckedChanged" />
                                                        <asp:HiddenField ID="hfInactive" runat="server" Value='<%# Bind("Inactive") %>' />
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbInactive" runat="server" Checked='<%# (Eval("Inactive").ToString())=="0"?false:true %>' Enabled="false" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
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
        <asp:SqlDataSource ID="SqlDataSourceDistricts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE FROM [District] WHERE [DistrictID] = @DistrictID" 
            InsertCommand="INSERT INTO [District] ([DistrictID], [DistrictName], [DistrictAddress1], [DistrictAddress2], [DistrictCity],[DistrictState], [DistrictZip], [DistrictContact], [DistrictFullName], [Inactive], [EnforcePeriodClosures]) 
              VALUES (@DistrictID, @DistrictName, @DistrictAddress1, @DistrictAddress2, @DistrictCity, @DistrictState, @DistrictZip, @DistrictContact, @DistrictFullName, @Inactive, @EnforcePeriodClosures)" 
            SelectCommand="SELECT [DistrictID], [DistrictName], [DistrictAddress1], [DistrictAddress2], [DistrictCity], [DistrictState], [DistrictZip], [DistrictContact], [DistrictFullName], [Inactive], [EnforcePeriodClosures] 
              FROM [District]" 
            UpdateCommand="UPDATE [District] SET [DistrictName] = @DistrictName, [DistrictAddress1] = @DistrictAddress1, [DistrictAddress2] = @DistrictAddress2, [DistrictCity] = @DistrictCity, 
              [DistrictZip] = @DistrictZip, [DistrictContact] = @DistrictContact, [DistrictFullName] = @DistrictFullName, [Inactive] = @Inactive, [EnforcePeriodClosures] = @EnforcePeriodClosures, [DistrictState]=@DistrictState
              WHERE [DistrictID] = @DistrictID"
             OnInserting="SqlDataSourceDistricts_Inserting" OnInserted="SqlDataSourceDistricts_Inserted">
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
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
