<%@ Page Title="Setup Contracts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupContracts.aspx.cs" Inherits="Paperwork.SetupContracts" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" >
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);

        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        var w = $(window).width();
        var h = $(window).height();
        var wMargin = 50;
        var hMargin = 80;
        var timer,
            $win = $(window);
        var scrollPosPanel;

        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
            $('#SpecialFitDiv').scrollTop(scrollPosPanel);
        };

        function BeginRequest(sender, args) {
            scrollPosPanel = $("#SpecialFitDiv").scrollTop();
        };

        //when window resizes
        $win.on('resize', function () {
            //update the width variable
            w = $(window).width();
            h = $(window).height();
            clearTimeout(timer);
            timer = setTimeout(function () {
                $("#SpecialFitDiv").width(w - wMargin);
                $("#SpecialFitDiv").height(h - hMargin);
            }, 200);

        });



        function BindEvents() {
            var PosRefY = $("#DataEntryReferenceLocation").position().top;
            $('#SpecialFitDiv').width(w - wMargin);
            $("#SpecialFitDiv").height(h - hMargin);
            $('#SpecialFitDiv').css("top", PosRefY + 0);
            $('#SpecialFitDiv').css("left", wMargin);

            $(function () {
                $("[id*=TextBoxEditStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $("[id*=TextBoxEditEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $("[id*=TextBoxStartDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $("[id*=TextBoxEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });

            if ($('#<%= hfAttachmentModalState.ClientID %>').val() === 'open') {
                $('#myAttachmentModal').modal('show');
            };

            // Store modal state
            $('#myAttachmentModal').on('shown.bs.modal', function () {
                $('#<%= hfAttachmentModalState.ClientID %>').val('open');
            });

            $('#myAttachmentModal').on('hidden.bs.modal', function () {
                $('#<%= hfAttachmentModalState.ClientID %>').val('');
            });





        };

        $(function () {
            BindEvents();
        });

        //When an input is focused - select the entire text
        //also if focused field is not one of the date fields - hide the calendar
        $('body').on('focus', 'input', function (e) {
            $(this)
                .one('mouseup', function () {
                    $(this).select();
                    return false;
                })
                .select();
        });





        //The following is used with file upload for contract attachments
        function displayNames(input, target) {
            const output = document.getElementById(target);
            output.innerHTML = '';
            const files = input.files
            for (let i = 0; i < files.length; i++) {
                let file = files.item(i)
                let el = document.createElement('li');
                el.innerText = file.name;
                output.appendChild(el);
            }
        }
    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <asp:Image CssClass="loadingImage" ID="ImageLoading1" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:HiddenField ID="hfAttachmentModalState" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <div id="DataEntryReferenceLocation"></div>
        <div id="SpecialFitDiv" style="background-color:white; position:absolute; left:10px;" >

            <div id="topSection" style="width:99%;position:sticky;top:0px;padding: 20px 0px;background-color:white;">
                <div class="row" >
                    <div class="col-sm-3">
                        <h2><%: Title %></h2>
                    </div>
                    <div class="col-sm-9" style="margin-left: auto; margin-right:auto;">
                        <asp:LinkButton ID="LinkButtonNewContract" runat="server" ToolTip="Add new contract" Width="150" OnClick="LinkButtonNewContract_Click" data-target="#NewContractModal" >
                            <span class="fa-stack fa-1x">
                                <i class="fas fa-file-invoice fa-stack-2x" style="color:dodgerblue;"></i>
                                <i class="fas fa-plus fa-stack-1x" style="color:black;"></i>
                            </span>
                            <span style="font-weight:bold;text-decoration: none; color:gray;left:-10px;position:relative;vertical-align:middle;">New Contract</span>
                        </asp:LinkButton>
                    </div>
                </div>
            

                <div class="row">
                    <div class="col-sm-12">
                        <asp:HiddenField ID="HFScrollStore" runat="server" />
                        <strong>District:</strong>
                        <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                            DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                            OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                        </asp:DropDownList> 
                        <asp:Panel ID="SearchPanel" runat="server" Width="100%"  >
                            <div style="text-align:left;margin-bottom:5px;">
                                Active/Inactives<asp:DropDownList ID="DDLShowInactives" runat="server" AutoPostBack="true" >
                                    <asp:ListItem Text="Display Active Only" Value='0' Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Display All" Value='1' ></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;
                                Text Search <asp:TextBox ID="TextBoxTextSearch" runat="server" AutoPostBack="true" AutoCompleteType="Disabled"></asp:TextBox>
                            </div>
                        </asp:Panel>
                    </div>

                </div>

            </div>
            <div id="DataEntry" style="width:99%;" >
                <div class="row">
                    <div class="col-sm-12">
                        <asp:GridView ID="gvContracts" runat="server" DataSourceID="SqlDataSourceContracts" AllowSorting="True" AutoGenerateColumns="false"
                            CssClass="mGrid" DataKeyNames="DistrictID,ContractID" ShowHeaderWhenEmpty="True" >
                            <Columns>
                                <asp:TemplateField HeaderText="ID" SortExpression="ContractID" ItemStyle-HorizontalAlign="Center">
                                    <EditItemTemplate>
                                            <asp:Label ID="LabelContractID" runat="server" Text='<%# Bind("ContractID") %>' Font-Bold="true"></asp:Label>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                            <asp:Label ID="LabelContractIDItem" runat="server" Text='<%# Eval("ContractID") %>' Font-Bold="true"></asp:Label>
                                            <br />
                                            <asp:LinkButton ID="IButtonAttached" runat="server" ToolTip="Documents are attached to this contract" Width="42" Height="40"  
                                                Visible='<%# Eval("HasAttachments") %>' CssClass="mGridLinkIconButton" OnClick="IButtonAttach_Click"  >
                                                <span class="fa-stack fa-1x">
                                                    <i class="fas fa-file fa-stack-2x" style="color:lightskyblue;"></i>
                                                    <i class="fas fa-paperclip fa-stack-1x" style="color:black;"></i>
                                                </span>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="IButtonAttach" runat="server" ToolTip="Attach Documents to this contract" Width="42" Height="40"  
                                                Visible='<%# (bool)Eval("HasAttachments")?false:true %>' CssClass="mGridLinkIconButton" OnClick="IButtonAttach_Click"  >
                                                <span class="fa-stack fa-1x">
                                                    <i class="fas fa-paperclip fa-stack-1x" style="color:grey;"></i>
                                                </span>
                                            </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description" SortExpression="ContractDescription" ItemStyle-HorizontalAlign="Center">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBoxDesc" runat="server" Text='<%# Bind("ContractDescription") %>' MaxLength="100" onFocus="this.select()" Width="150"></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("ContractDescription") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Dates" SortExpression="ContractStartDate">
                                    <EditItemTemplate>
                                        <asp:Label ID="LabelStartDate" runat="server" Width="75px" Text="Start Date:"></asp:Label>
                                        <asp:TextBox ID="TextBoxEditStartDate" runat="server" Text='<%# Bind("ContractStartDate", "{0:MM/dd/yyyy}") %>' Width="75px" MaxLength="10"
                                                AutoCompleteType="Disabled"></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ErrorMessage="*Enter a valid start date" id="validStartDate" ControlToValidate="TextBoxEditStartDate" ValidationExpression="^([0]?[1-9]|1[012])[- /.]([0]?[1-9]|[12][0-9]|3[01])[- /.](19|20|21|22)\d\d$" CssClass="validators" Display="Dynamic" SetFocusOnError="true">
				                        </asp:RegularExpressionValidator>
                                        <br />
                                        <asp:Label ID="LabelEndDate" runat="server" Width="75px" Text="End Date:"></asp:Label>
                                        <asp:TextBox ID="TextBoxEditEndDate" runat="server" Text='<%# Bind("ContractEndDate", "{0:MM/dd/yyyy}") %>' Width="75px" MaxLength="10"
                                                AutoCompleteType="Disabled"></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ErrorMessage="*Enter a valid start date" id="validEndDate" ControlToValidate="TextBoxEditEndDate" ValidationExpression="^([0]?[1-9]|1[012])[- /.]([0]?[1-9]|[12][0-9]|3[01])[- /.](19|20|21|22)\d\d$" CssClass="validators" Display="Dynamic" SetFocusOnError="true">
                                        </asp:RegularExpressionValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="LabelStartDate" runat="server" Width="75px" Text="Start Date:"></asp:Label>
                                        <asp:Label ID="LabelDateStartDate" runat="server" Text='<%# ((DateTime)Eval("ContractStartDate")).ToShortDateString() %>' Width="75px"></asp:Label>
                                        <br />
                                        <asp:Label ID="LabelEndDate" runat="server" Width="75px" Text="End Date:"></asp:Label>
                                        <asp:Label ID="LabelDateEndDate" runat="server" Text='<%# ((DateTime)Eval("ContractEndDate")).ToShortDateString() %>' Width="75px"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Billing Name" SortExpression="ContractBillingName">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBoxBillingName" runat="server" Text='<%# Bind("ContractBillingName") %>' MaxLength="50" onFocus="this.select()" Width="150"></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label5" runat="server" Text='<%# Bind("ContractBillingName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Address" SortExpression="ContractAddress1">
                                    <EditItemTemplate>
                                        <asp:Label ID="LabelAddress1" runat="server" Text="Address1:"></asp:Label>
                                        <asp:TextBox ID="TextBoxAddress1" runat="server" Text='<%# Bind("ContractAddress1") %>' MaxLength="50" onFocus="this.select()" Width="150"></asp:TextBox>
                                        <br />
                                        <asp:Label ID="LabelAddress2" runat="server" Text="Address2:"></asp:Label>
                                        <asp:TextBox ID="TextBoxAddress2" runat="server" Text='<%# Bind("ContractAddress2") %>' MaxLength="50" onFocus="this.select()" Width="150"></asp:TextBox>
                                        <br />
                                        <asp:Label ID="LabelCity" runat="server" Text="City:"></asp:Label>
                                        <asp:TextBox ID="TextBoxCity" runat="server" Text='<%# Bind("ContractCity") %>' MaxLength="35" onFocus="this.select()" Width="100"></asp:TextBox>
                                        &nbsp;
                                        <asp:Label ID="LabelState" runat="server" Text="State:"></asp:Label>
                                        <asp:TextBox ID="TextBoxState" runat="server" Text='<%# Bind("ContractState") %>' MaxLength="2" Width="50px" onFocus="this.select()"></asp:TextBox>
                                        &nbsp;
                                        <asp:Label ID="LabelZIP" runat="server" Text="ZIP:"></asp:Label>
                                        <asp:TextBox ID="TextBoxZip" runat="server" Text='<%# Bind("ContractZIP") %>' MaxLength="10" Width="100px" onFocus="this.select()"></asp:TextBox>

                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="LabelAddress1" runat="server" Text='<%# Bind("ContractAddress1") %>'></asp:Label><br />
                                        <asp:Label ID="LabelAddress2" runat="server" Text='<%# Bind("ContractAddress2") %>' Visible='<%# Eval("ContractAddress2").ToString().Trim()==string.Empty?false:true %>'></asp:Label>
                                        <asp:Literal ID="LiteralAddress2Break" runat="server" Visible='<%# Eval("ContractAddress2").ToString().Trim()==string.Empty?false:true %>'><br /></asp:Literal>
                                        <asp:Label ID="LabelCity" runat="server" Text='<%# Bind("ContractCity") %>'></asp:Label>
                                        <asp:Literal ID="LiteralCityComma" runat="server" Visible='<%# Eval("ContractCity").ToString().Trim()==string.Empty?false:true %>' >,&nbsp;</asp:Literal>
                                        <asp:Label ID="LabelState" runat="server" Text='<%# Bind("ContractState") %>'></asp:Label>
                                        <asp:Label ID="LabelZip" runat="server" Text='<%# Bind("ContractZIP") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Phone1" SortExpression="ContractPhone1">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBoxPhone1" runat="server" Text='<%# Bind("ContractPhone1") %>' MaxLength="20" onFocus="this.select()" Width="100px"></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label11" runat="server" Text='<%# Bind("ContractPhone1") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Phone2" SortExpression="ContractPhone2">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBoxPhone2" runat="server" Text='<%# Bind("ContractPhone2") %>' MaxLength="20" onFocus="this.select()" Width="100px"></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label12" runat="server" Text='<%# Bind("ContractPhone2") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fax" SortExpression="ContractFax">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBoxFax" runat="server" Text='<%# Bind("ContractFax") %>' MaxLength="20" onFocus="this.select()" Width="100px"></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="LabelFax" runat="server" Text='<%# Bind("ContractFax") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Contact" SortExpression="ContractContact">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBoxContact" runat="server" Text='<%# Bind("ContractContact") %>' MaxLength="50" onFocus="this.select()" Width="150px"></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label13" runat="server" Text='<%# Bind("ContractContact") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Email" SortExpression="ContractEmail">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBoxEmail" runat="server" Text='<%# Bind("ContractEmail") %>' MaxLength="50" onFocus="this.select()" Width="150px"></asp:TextBox>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:HyperLink NavigateUrl='<%# Bind( "ContractEmail", "mailto:{0}") %>' runat="server" ID="emailLink" Text='<%# Bind("ContractEmail") %>' ></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Accounting Cust. ID" SortExpression="AccountingCustomerID">
                                    <EditItemTemplate>
                                        <div style="text-align:center;">
                                            <asp:TextBox ID="TextBoxCustomerID" runat="server" Text='<%# Bind("AccountingCustomerID") %>' MaxLength="15" Width="100" ToolTip="ID must match customer ID in accounting system."></asp:TextBox> <br />
                                            <asp:LinkButton ID="lbInfo" CssClass="fas fa-info-circle " runat="server" data-toggle="modal" data-target="#myInfoModal"></asp:LinkButton>
                                        </div>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <div style="text-align:center;">
                                            <asp:Label ID="LabelCustomerID" runat="server" Text='<%# Bind("AccountingCustomerID") %>' ToolTip="ID must match customer ID in accounting system."
                                                Visible='<%# string.IsNullOrEmpty(Eval("AccountingCustomerID").ToString())?false:true %>'></asp:Label>
                                            <asp:Literal ID="LiteralCustomerID" runat="server" Visible='<%# string.IsNullOrEmpty(Eval("AccountingCustomerID").ToString())?false:true %>' Text="<br />">
                                            </asp:Literal>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Inactive" SortExpression="Inactive">
                                    <EditItemTemplate>
                                        <div style="text-align:center;">
                                            <asp:CheckBox ID="CheckBoxEInactive" runat="server" Checked='<%# Bind("Inactive") %>' />
                                        </div>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <div style="text-align:center;">
                                            <asp:CheckBox ID="CheckBoxInactive" runat="server" Checked='<%# Eval("Inactive") %>' Enabled="false" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False" ItemStyle-Width="75px">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" CommandName="edit" Width="75px"
                                            ID="IEdit" ToolTip="Edit" Text="Edit"
                                            CssClass="fas fa-pencil-alt glyphButton mGridLinkButton"/>
                                            <br />
							            <asp:LinkButton ID="IDelete" runat="server" CausesValidation="False" CommandName="Delete" Width="75px"
                                            ToolTip="Delete" Text="Delete"
                                            CssClass="fas fa-trash-alt glyphButton mGridLinkButton" ForeColor="DarkRed"
                                            OnClientClick="return confirm('Are you sure you want to delete this contract?');"/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:LinkButton runat="server" CommandName="update" Width="75px"
                                            ID="IUpdate" ToolTip="Update" Text="Update"
                                            CssClass="fas fa-save mGridLinkButton" ForeColor="Green"/>
                                            <br />
							            <asp:LinkButton ID="ICancel" runat="server" CausesValidation="False" CommandName="Cancel" Width="75px"
                                            ToolTip="Cancel" Text="Cancel" ForeColor="Red"
                                            CssClass="fas fa-window-close mGridLinkButton"/>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

    </div>


    <div id="NewContractModal" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title"><i class="fas fa-file" style="color:steelblue"></i><strong>New Contract</strong></h4>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                            
                </div>
                <div class="modal-body">

                    <div class="row justify-content-md-center">
                        <div class="col col-md-1">&nbsp;</div>
                        <div class="col col-md-11">
                            <asp:DetailsView ID="DetailsViewContractEntry" runat="server" AutoGenerateRows="False" DataKeyNames="DistrictID,ContractID" 
                                DataSourceID="SqlDataSourceContracts" DefaultMode="Insert" HeaderText="Enter New Contract:" Height="50px" Width="400px" CssClass="mGrid"
                                OnItemInserted="DetailsViewContractEntry_ItemInserted">
                                <Fields>
                                    <asp:TemplateField HeaderText="Contract ID / Short Name" SortExpression="ContractID">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBoxContractID" runat="server" Text='<%# Bind("ContractID") %>' MaxLength="15" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="requiredContractID" runat="server" ControlToValidate="TextBoxContractID" Display="Dynamic" CssClass="validators" ErrorMessage="Contract Short Name is required." ValidationGroup="validInsert" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Description" SortExpression="ContractDescription">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBoxDesc" runat="server" Text='<%# Bind("ContractDescription") %>' MaxLength="100"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Start Date" SortExpression="ContractStartDate">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBoxStartDate" runat="server" Text='<%# Bind("ContractStartDate") %>' MaxLength="10"
                                                AutoCompleteType="Disabled"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="requiredStartDate" runat="server" ControlToValidate="TextBoxStartDate" Display="Dynamic" CssClass="validators" ErrorMessage="Start Date is required." ValidationGroup="validInsert" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator runat="server" ErrorMessage="*Enter a valid start date" id="validStartDate" ControlToValidate="TextBoxStartDate" ValidationExpression="^([0]?[1-9]|1[012])[- /.]([0]?[1-9]|[12][0-9]|3[01])[- /.](19|20|21|22)\d\d$" CssClass="validators" Display="Dynamic" ValidationGroup="validInsert">*</asp:RegularExpressionValidator>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="End Date" SortExpression="ContractEndDate">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBoxEndDate" runat="server" Text='<%# Bind("ContractEndDate") %>' MaxLength="10"
                                                AutoCompleteType="Disabled"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="requiredEndDate" runat="server" ControlToValidate="TextBoxEndDate" Display="Dynamic" CssClass="validators" ErrorMessage="End Date is required." ValidationGroup="validInsert" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator runat="server" ErrorMessage="*Enter a valid end date" id="validEndDate" ControlToValidate="TextBoxEndDate" ValidationExpression="^([0]?[1-9]|1[012])[- /.]([0]?[1-9]|[12][0-9]|3[01])[- /.](19|20|21|22)\d\d$" CssClass="validators" Display="Dynamic" ValidationGroup="validInsert">*</asp:RegularExpressionValidator>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Billing Name" SortExpression="ContractBillingName">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("ContractBillingName") %>' MaxLength="50"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Address1" SortExpression="ContractAddress1">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("ContractAddress1") %>' MaxLength="50"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Address2" SortExpression="ContractAddress2">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("ContractAddress2") %>' MaxLength="50"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="City" SortExpression="ContractCity">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("ContractCity") %>' MaxLength="35"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="State" SortExpression="ContractState">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("ContractState") %>' MaxLength="2"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ZIP" SortExpression="ContractZIP">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox10" runat="server" Text='<%# Bind("ContractZIP") %>' MaxLength="10"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Phone1" SortExpression="ContractPhone1">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox11" runat="server" Text='<%# Bind("ContractPhone1") %>' MaxLength="20"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Phone2" SortExpression="ContractPhone2">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox12" runat="server" Text='<%# Bind("ContractPhone2") %>' MaxLength="20"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Fax" SortExpression="ContractFax">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBoxFax" runat="server" Text='<%# Bind("ContractFax") %>' MaxLength="20"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contact Name" SortExpression="ContractContact">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBox13" runat="server" Text='<%# Bind("ContractContact") %>' MaxLength="50"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" SortExpression="ContractEmail">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBoxEmail" runat="server" Text='<%# Bind("ContractEmail") %>' MaxLength="50"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Accounting Customer ID" >
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBoxCustomerID" runat="server" Text='<%# Bind("AccountingCustomerID") %>' MaxLength="15" ToolTip="ID must match customer ID in accounting system."></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Notes" SortExpression="ContractNotes">
                                        <InsertItemTemplate>
                                            <asp:TextBox ID="TextBoxNotes" runat="server" Text='<%# Bind("ContractNotes") %>' TextMode="MultiLine" Rows="5"></asp:TextBox>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <InsertItemTemplate>
                                            <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="validators" DisplayMode="BulletList" EnableClientScript="true" ShowSummary="true" ValidationGroup="validInsert" />
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <InsertItemTemplate>
                                            <div style="text-align:center;font-size:larger;">
                                                <asp:LinkButton ID="IInsert" runat="server" CausesValidation="true" CommandName="Insert" CssClass="fas fa-save glyphButton mGridLinkButton" ForeColor="Green" Text="Add" ToolTip="Add" ValidationGroup="validInsert" Width="100%" />
                                            </div>
                                        </InsertItemTemplate>
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>

                            <hr />
                        </div>
                    </div>
                    
                </div>


                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-primary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="myAttachmentModal" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title"><i class="fas fa-file" style="color:steelblue"></i><strong>Attachments</strong></h4>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                

                        <div class="modal-body">
                            <div class="row">
                                <div class="col col-md-1">&nbsp;</div>
                                <div class="col col-md-11">
                                    Contract:
                                    <asp:Label ID="LabelSelectedContract" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="row justify-content-md-center">
                            <div class="col col-md-1">&nbsp;</div>
                            <div class="col col-md-11">
                                <asp:GridView ID="GridViewFileAttachments" runat="server" DataSourceID="SqlDataSourceDocList" AutoGenerateColumns="false" DataKeyNames="AttachmentID,DistrictID,ContractID"
                                        EmptyDataText="There aren't any attachments for this contract." ShowHeader="false" OnRowDeleted="GridViewFileAttachments_RowDeleted">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="LinkButtonFileName" runat="server" Text='<%# Eval("FileName") %>' OnClick="LinkButtonFileName_Click" CommandArgument='<%# Eval("AttachmentID") %>'></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="IDeleteFile" runat="server" CausesValidation="False" CommandName="Delete"
                                                    ToolTip="Delete" Text="Delete" CssClass="fas fa-trash-alt glyphButton mGridLinkButton" ForeColor="DarkRed" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <hr />
                            </div>
                        </div>
                        <div class="row justify-content-md-center"> 
                            <div class="col col-md-1">&nbsp;</div>
                            <div class="col col-md-11">  
                                <div class="form-group">  
                                    <label>Choose or drag import file(s)</label>  
                                    <div class="input-group">  
                                        <div class="custom-file" style="text-align:center;">  
                                            <%--CssClass="custom-file-input upload"--%>
                                            <asp:FileUpload ID="FileUpload1"  runat="server" AllowMultiple="true" onchange="displayNames(this, 'names')"   />  
                                    
                                        </div>  

                                    </div>  
                      <ul id="names" ></ul>
                                </div>  
                            </div>  
                        </div>



                        <div class="row">
                            <div class="col col-md-1">&nbsp;</div>
                            <div class="col col-md-6">&nbsp;</div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="ButtonUpld" runat="server" CssClass="btn btn-outline-primary" Text="Upload" OnClick="ButtonUpld_Click"  /> 
                            <button type="button" class="btn btn-outline-primary" data-dismiss="modal">Close</button>
                        </div>
            </div>
        </div>
    </div>

    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceDocList" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT FileName, AttachmentID, DistrictID, ContractID FROM DistrictContractAttachments WHERE DistrictID=@DistrictID AND ContractID=@ContractID"
            DeleteCommand="DELETE [DistrictContractAttachments] WHERE DistrictID=@DistrictID AND ContractID=@ContractID AND AttachmentID=@AttachmentID">
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                <asp:ControlParameter Name="ContractID" Type="String" ControlID="LabelSelectedContract" PropertyName="Text" />
            </SelectParameters>
            <DeleteParameters>
                <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                <asp:ControlParameter Name="ContractID" Type="String" ControlID="LabelSelectedContract" PropertyName="Text" />
                <asp:Parameter Name="AttachmentID" Type="Int32" />
            </DeleteParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceContracts" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            DeleteCommand="DELETE [DistrictContracts] WHERE DistrictID=@DistrictID AND ContractID=@ContractID; DELETE [DistrictContractAttachments] WHERE DistrictID=@DistrictID AND ContractID=@ContractID"
            InsertCommand="INSERT INTO [DistrictContracts] ([DistrictID],[ContractID],[ContractDescription],[ContractStartDate],[ContractEndDate],[ContractBillingName]
        ,[ContractAddress1],[ContractAddress2],[ContractCity],[ContractState],[ContractZIP],[ContractPhone1],[ContractPhone2]
        ,[ContractContact],[ContractNotes],[Inactive],[ContractFax],[ContractEmail], [AccountingCustomerID]) 
        VALUES (@DistrictID,RTRIM(UPPER(@ContractID)),@ContractDescription,@ContractStartDate,@ContractEndDate,@ContractBillingName
        ,@ContractAddress1,@ContractAddress2,@ContractCity,UPPER(@ContractState),@ContractZIP,@ContractPhone1,@ContractPhone2
        ,@ContractContact,@ContractNotes,0, @ContractFax, @ContractEmail, @AccountingCustomerID)"
            SelectCommand="SELECT DC.[DistrictID],DC.[ContractID],[ContractDescription],[ContractStartDate],[ContractEndDate],[ContractBillingName]
                ,[ContractAddress1],[ContractAddress2],[ContractCity],[ContractState],[ContractZIP],[ContractPhone1],[ContractPhone2],[ContractContact]
                ,[ContractNotes],[Inactive],[ContractEmail],[ContractFax],[AccountingCustomerID],CAST(IIF(ISNULL(AttCount,0)>0,1,0) AS BIT) HasAttachments 
            FROM DistrictContracts DC
                LEFT OUTER JOIN (SELECT DistrictID, ContractID, Count(*) AttCount FROM  [DistrictContractAttachments] 
                GROUP BY DistrictID, ContractID) CCA ON DC.DistrictID=CCA.DistrictID AND DC.ContractID=CCA.ContractID
            WHERE DC.DistrictID=@DistrictID ORDER BY ContractStartDate DESC"
            
            
            UpdateCommand="UPDATE [DistrictContracts] SET [ContractDescription]=@ContractDescription
                    ,[ContractStartDate]=@ContractStartDate,[ContractEndDate]=@ContractEndDate,[ContractBillingName]=@ContractBillingName
                    ,[ContractAddress1]=@ContractAddress1,[ContractAddress2]=@ContractAddress2,[ContractCity]=@ContractCity,[ContractState]=UPPER(@ContractState)
                    ,[ContractZIP]=@ContractZIP,[ContractPhone1]=@ContractPhone1,[ContractPhone2]=@ContractPhone2
                    ,[ContractContact]=@ContractContact,[ContractNotes]=@ContractNotes,[Inactive]=@Inactive, ContractFax=@ContractFax, ContractEmail=@ContractEmail
                    ,AccountingCustomerID=@AccountingCustomerID
                    WHERE [DistrictID]=@DistrictID AND [ContractID]=@ContractID" 
            FilterExpression="([Inactive] <= {0}) AND ([ContractID] LIKE '%{1}%' OR [ContractDescription] LIKE '%{1}%' OR [ContractBillingName] LIKE '%{1}%' OR [ContractAddress1] LIKE '%{1}%' OR [ContractAddress2] LIKE '%{1}%' OR [ContractCity] LIKE '%{1}%' OR [ContractContact] LIKE '%{1}%')"
            OnFiltering="SqlDataSourceContracts_Filtering" >
                
            <SelectParameters>
                <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
            </SelectParameters>
            <DeleteParameters>
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="ContractID" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                <asp:Parameter Name="ContractID" Type="String" />
                <asp:Parameter Name="ContractDescription" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractStartDate" Type="DateTime" />
                <asp:Parameter Name="ContractEndDate" Type="DateTime" />
                <asp:Parameter Name="ContractBillingName" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractAddress1" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractAddress2" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractCity" Type="String" ConvertEmptyStringToNull="false"  DefaultValue=" "/>
                <asp:Parameter Name="ContractState" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractZIP" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractPhone1" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractPhone2" Type="String" ConvertEmptyStringToNull="false"  DefaultValue=" "/>
                <asp:Parameter Name="ContractContact" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractNotes" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractFax" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractEmail" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="AccountingCustomerID" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
            </InsertParameters>
            <UpdateParameters>
                <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                <asp:Parameter Name="ContractID" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractDescription" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractStartDate" Type="DateTime" DefaultValue="1/1/1900" />
                <asp:Parameter Name="ContractEndDate" Type="DateTime" DefaultValue="1/1/2200" />
                <asp:Parameter Name="ContractBillingName" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractAddress1" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractAddress2" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractCity" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractState" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractZIP" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractPhone1" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractPhone2" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractContact" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractNotes" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="Inactive" Type="Byte" DefaultValue="0" />
                <asp:Parameter Name="ContractFax" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="ContractEmail" Type="String" ConvertEmptyStringToNull="false" DefaultValue=" " />
                <asp:Parameter Name="AccountingCustomerID" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
            </UpdateParameters>
            <FilterParameters >
                <asp:ControlParameter ControlID="DDLShowInactives" PropertyName="SelectedValue" DbType="Int16" Name="Inactive" DefaultValue="0" />
                <asp:ControlParameter ControlID="TextBoxTextSearch" PropertyName="Text" DbType="String" ConvertEmptyStringToNull="false" />
            </FilterParameters>
        </asp:SqlDataSource>
    </div>


        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="GridViewFileAttachments"  />
            <asp:PostBackTrigger ControlID="ButtonUpld" />
            <asp:PostBackTrigger ControlID="DetailsViewContractEntry" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
