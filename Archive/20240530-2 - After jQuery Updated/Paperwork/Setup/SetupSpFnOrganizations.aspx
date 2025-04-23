<%@ Page Title="Setup Sp Fn Organizations" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupSpFnOrganizations.aspx.cs" Inherits="Paperwork.SetupSpFnOrganizations" %>
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
                    <div class="row">
                        <div class="col-sm-12">
                            Use this page to create and maintain organizations which will be used to populate the Special Function invoices page.<br />
                            Values will be used as defaults, but can be changed as needed once applied to an invoice.
                        </div>
                        <div class="col-sm-12">
                            <asp:CheckBox ID="CBShowInactives" runat="server" Text="Show Inactives" Checked="false" AutoPostBack="true" />
                        </div>
                        <div class="col-sm-12">
                            <div id="GridViewHeightController" style="overflow-y:auto;" class="row">
                                <div class="col-sm-12">
                                    <asp:GridView ID="GridViewSpFnOrgs" runat="server" DataSourceID="SqlDataSourceSpFnOrgs" CssClass="mGrid"
                                        EmptyDataText="No Special Function Organizations have been setup for this district." AutoGenerateColumns="False"
                                        DataKeyNames="SpFnOrgID" OnRowDataBound="GridViewSpFnOrgs_RowDataBound" OnRowCommand="GridViewSpFnOrgs_RowCommand" 
                                        AllowSorting="True" ShowFooter="True" ShowHeaderWhenEmpty="True" >
                                        <Columns>
                                            <asp:BoundField DataField="DistrictID" Visible="false" />
                                            <asp:TemplateField HeaderText="Name" SortExpression="SpFnOrgName" ItemStyle-Width="300">
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="TextBoxESpFnOrgName" runat="server" Text='<%# Bind("SpFnOrgName") %>' Width="300"></asp:TextBox>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("SpFnOrgName") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label ID="LabelNew" runat="server" ForeColor="Green" Text="*"></asp:Label>
                                                    <asp:TextBox ID="TextBoxFSpFnOrgName" runat="server" Width="99%"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredSpFnOrgName" runat="server" ControlToValidate="TextBoxFSpFnOrgName" CssClass="validators"
                                                        Display="Dynamic" ErrorMessage="Name is required" Text="*" ValidationGroup="Insert"></asp:RequiredFieldValidator>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="3086" SortExpression="3086">
                                                <EditItemTemplate>
                                                    <div style="text-align:center;">
                                                        <asp:CheckBox ID="CBE3086" runat="server" Checked='<%# Bind("3086") %>' />
                                                    </div>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <div style="text-align:center;">
                                                        <asp:CheckBox ID="CB3086" runat="server" Checked='<%# Eval("3086") %>' Enabled="false" />
                                                    </div>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="text-align:center;">
                                                        <asp:CheckBox ID="CBF3086" runat="server"  />
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="CACFP" SortExpression="CACFP">
                                                <EditItemTemplate>
                                                    <div style="text-align:center;">
                                                        <asp:CheckBox ID="CBECACFP" runat="server" Checked='<%# Bind("CACFP") %>' />
                                                    </div>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <div style="text-align:center;">
                                                        <asp:CheckBox ID="CBCACFP" runat="server" Checked='<%# Eval("CACFP") %>' Enabled="false" />
                                                    </div>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="text-align:center;">
                                                        <asp:CheckBox ID="CBFCACFP" runat="server" />
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Contact Name" SortExpression="SpFnOrgContact">
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="TextBoxESpFnOrgContact" runat="server" Text='<%# Bind("SpFnOrgContact") %>' MaxLength="50" Width="100%"></asp:TextBox>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("SpFnOrgContact") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:TextBox ID="TextBoxFSpFnOrgContact" runat="server" MaxLength="50" Width="100%"></asp:TextBox>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Default Remit To" SortExpression="SpFnOrgContact">
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="TextBoxESpFnOrgRemitTo" runat="server" Text='<%# Bind("SpFnOrgRemitTo") %>' MaxLength="50" Width="100%"></asp:TextBox>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("SpFnOrgRemitTo") %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:TextBox ID="TextBoxFSpFnOrgRemitTo" runat="server" MaxLength="50" Width="100%"></asp:TextBox>
                                                </FooterTemplate>
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
                        </div>
                    </div>
                    
                    <div class="messagealert" id="alert_container"></div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceSpFnOrgs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
            DeleteCommand="DELETE FROM [SPFNOrganizations] WHERE SpFnOrgID=@SpFnOrgID AND DistrictID=@DistrictID"
            InsertCommand="INSERT INTO [SPFNOrganizations] ([SpFnOrgName],[Inactive],[3086],[CACFP],[SpFnOrgContact],[SpFnOrgRemitTo],[DistrictID]) 
            VALUES (@SpFnOrgName,0,@3086,@CACFP,@SpFnOrgContact,@SpFnOrgRemitTo,@DistrictID)"
            SelectCommand="SELECT [SpFnOrgID],[SpFnOrgName],[Inactive],[3086],[CACFP],[SpFnOrgContact],[SpFnOrgRemitTo],[DistrictID]
                  FROM [SPFNOrganizations] WHERE DistrictID=@DistrictID
                  UNION ALL 
                  SELECT 0 [SpFnOrgID],'' [SpFnOrgName], CAST(0 AS BIT) [Inactive],CAST(0 AS BIT) [3086],CAST(0 AS BIT) [CACFP],
                  '' [SpFnOrgContact],'' [SpFnOrgRemitTo],'' [DistrictID]"
            UpdateCommand="UPDATE [SPFNOrganizations] SET [SpFnOrgName]=@SpFnOrgName, SpFnOrgContact=@SpFnOrgContact, SpFnOrgRemitTo=@SpFnOrgRemitTo, Inactive=@Inactive, [3086]=@3086, CACFP=@CACFP WHERE [SpFnOrgID]=@SpFnOrgID" 
            FilterExpression="[Inactive]<={0}" OnInserted="SqlDataSourceSpFnOrgs_Inserted" OnUpdating="SqlDataSourceSpFnOrgs_Updating"
            OnInserting="SqlDataSourceSpFnOrgs_Inserting" OnUpdated="SqlDataSourceSpFnOrgs_Updated">
            <FilterParameters>
                <asp:ControlParameter ControlID="CBShowInactives" PropertyName="Checked" Type="Byte" />
            </FilterParameters>
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
            </SelectParameters>
            <DeleteParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:Parameter Name="SpFnOrgID" DbType="Int32" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="SpFnOrgName" Type="String" />
                <asp:Parameter Name="SpFnOrgContact" Type="String" />
                <asp:Parameter Name="SpFnOrgRemitTo" Type="String" />
                <asp:Parameter Name="3086" Type="Byte" />
                <asp:Parameter Name="CACFP" Type="Byte" />
                <asp:Parameter Name="Inactive" Type="Byte" />
            </UpdateParameters>
            <InsertParameters>
                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                <asp:Parameter Name="SpFnOrgName" Type="String" />
                <asp:Parameter Name="SpFnOrgContact" Type="String" />
                <asp:Parameter Name="SpFnOrgRemitTo" Type="String" />
                <asp:Parameter Name="3086" Type="Byte" />
                <asp:Parameter Name="CACFP" Type="Byte" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
