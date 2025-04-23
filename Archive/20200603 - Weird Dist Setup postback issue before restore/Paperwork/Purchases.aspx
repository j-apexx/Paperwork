<%@ Page Title="Transmittal Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Purchases.aspx.cs" Inherits="Paperwork.Purchases" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
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
        } );

        function BindEvents() {
            var PYearStart = $("[id*=HFProgYearStartDate]").val();
            var PYearEnd = $("[id*=HFProgYearEndDate]").val();

            $(function () {
                $( "[id*=TextBoxInvoiceDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $( "[id*=TextBoxWEDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });
            $(function () {
                $( "[id*=TextBoxEditInvoiceDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true
                });
            });
            $(function () {
                $( "[id*=TextBoxEditWEDate]" ).datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });


            //Maintain Card Collapse States
            $("#cardCollapse1").on('shown.bs.collapse', function () {
                $('#<%= HFcollapse1State.ClientID %>').val("showcollapse1");
            });
            $("#cardCollapse1").on('hidden.bs.collapse', function () {
                $('#<%= HFcollapse1State.ClientID %>').val(null);
            });

            $("#cardCollapse2").on('shown.bs.collapse', function () {
                $('#<%= HFcollapse2State.ClientID %>').val("showcollapse2");
            });
            $("#cardCollapse2").on('hidden.bs.collapse', function () {
                $('#<%= HFcollapse2State.ClientID %>').val(null);
            });

            var last1 = $('#<%= HFcollapse1State.ClientID %>').val();
            var last2 = $('#<%= HFcollapse2State.ClientID %>').val();

            if (last1 != null && last1 != "") {
                <%--$("#accordian .panel-collapse").addClass('in');--%>
                $("#cardCollapse1").addClass('show');
            }
            else {
                <%--$("#accordian .panel-collapse").removeClass('in');--%>
                $("#cardCollapse1").removeClass('show');
            }

            if (last2 != null && last2 != "") {
                <%--$("#accordian .panel-collapse").addClass('in');--%>
                $("#cardCollapse2").addClass('show');
            }
            else {
                <%--$("#accordian .panel-collapse").removeClass('in');--%>
                $("#cardCollapse2").removeClass('show');
            }

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


        };


        //When an input is focused - select the entire text
        //also if focused field is not one of the date fields - hide the calendar
        $('body').on('focus', 'input', function (e) {
            
            $(this)
                .one('mouseup', function () {
                    $(this).select();
                    return false;
                })
                .select();
            $(this).css("color", "DimGray");
        });

        $("body").on('focusout', 'input', function (e) {
            var cellVal = parseFloat($(this).val());
            if (cellVal == 0) {
                $(this).css("color", "White");
            }
            else {
                $(this).css("color", "DimGray");
            }
           
        });

        //apply highlight 
        $('body').on('focus', 'input,textarea,select', function (e) {
            $(this).addClass("highlight");
        });
        // remove highlight
        $('body').on('focusout', 'input,textarea,select', function (e) {
            $(this).removeClass("highlight");
        });

        jQuery(function ($) {
            var focusedElementId = "";
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_beginRequest(function (source, args) {
                var fe = document.activeElement;
                if (fe != null) {
                    focusedElementId = fe.id;
                } else {
                    focusedElementId = "";
                }
            });

            prm.add_endRequest(function (source, args) {
                if (focusedElementId != "") {
                    $("#" + focusedElementId).focus();
                }
            });
        });

        function ValidateOtherCats(source, args) {
            if (!isNaN(parseFloat($("[id*=TextBoxOther]").val())) && parseFloat($("[id*=TextBoxOther]").val()) != 0) {
                if(args.Value == "NONE")
                    args.IsValid = false;
            }
        };

        function ShowMessage(message, messagetype) {
            var cssclass;
            switch (messagetype) {
                case 'Success':
                    cssclass = 'alert-success';
                    break;
                case 'Error':
                    cssclass = 'alert-danger';
                    break;
                case 'Warning':
                    cssclass = 'alert-warning';
                    break;
                default:
                    cssclass = 'alert-info';
            }
            $('#alert_container').append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert alert-dismissible fade show '
                + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><a href="#" data-dismiss="alert" ><strong>'
                + messagetype + '!</strong> <span>' + message + '</span></a></div>');
            window.setTimeout(function () { $('#alert_div').alert('close'); }, 3000);
        }

        //// the following allows you to specify a specific alert container if more than one is on the page
        //function ShowMessageByName(message, messagetype, containerName) {
        //    var cssclass;
        //    var msgduration = 20000;
        //    switch (messagetype) {
        //        case 'Success':
        //            cssclass = 'alert-success';
        //            break;
        //        case 'Error':
        //            cssclass = 'alert-danger';
        //            break;
        //        case 'Warning':
        //            cssclass = 'alert-warning';
        //            break;
        //        default:
        //            cssclass = 'alert-info';
        //    }
        //    $('#' + containerName).append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert alert-dismissible fade show '
        //        + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><a href="#" data-dismiss="alert" ><strong>'
        //        + messagetype + '!</strong> <span>' + message + '</span></a></div>');
        //    window.setTimeout(function () { $('#alert_div').alert('close'); }, msgduration);
        //}
    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="hdrGroup">
        <div id="hdrPrintButton" class="HDRPrint">
            <asp:LinkButton ID="IPrint" Width="200" runat="server" 
            class="fas fa-print mGridLinkButton" 
            ToolTip="Print" Text=" Transmittal Report" OnClick="IPrint_Click"></asp:LinkButton>
            <asp:HiddenField ID="HFProgYearStartDate" runat="server" />
            <asp:HiddenField ID="HFProgYearEndDate" runat="server" />
        </div>
        <h2><%: Title %></h2>
    </div>
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always" >
        <ContentTemplate>
            <div id="dataentry">
                <asp:HiddenField ID="HFcollapse1State" runat="server" Value="showcollapse1" />
                <asp:HiddenField ID="HFcollapse2State" runat="server" Value="showcollapse2" />

                <div id="accordian">
                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse1">
                                New Transmittal Entry
                            </a>
                        </div>
                        <div id="cardCollapse1" class="collapse show" >
                            <div class="card-body">
                                <asp:DetailsView ID="DetailsViewInsert" runat="server" AutoGenerateRows="False" DataKeyNames="Counter" DataSourceID="SqlDataSourcePurchases" Width="500px" DefaultMode="Insert" 
                                    CssClass="mGrid" OnPreRender="DetailsViewInsert_PreRender" OnItemInserted="DetailsViewInsert_ItemInserted" OnItemInserting="DetailsViewInsert_ItemInserting">
                                    <HeaderTemplate>
                                        <h5>Enter transmittals for your unit.</h5>
                                    </HeaderTemplate>
                                    <Fields>
                                        <asp:TemplateField HeaderText="Vendor" SortExpression="VendorID">
                                            <InsertItemTemplate>
                                                <asp:DropDownList ID="ddlVendors" runat="server" DataSourceID="SqlDataSourceDistVendors" DataTextField="VENDNAME" 
                                                    DataValueField="VENDORID" SelectedValue='<%# Bind("VendorID") %>' TabIndex="1" AutoPostBack="true" OnSelectedIndexChanged="ddlVendors_SelectedIndexChanged" ></asp:DropDownList>
                                            </InsertItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="School">
                                            <InsertItemTemplate>
                                                <asp:DropDownList ID="DropDownListSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" DataValueField="SchoolNum" TabIndex="2" SelectedValue='<%# Bind("SchoolNum") %>'></asp:DropDownList>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Invoice Date" SortExpression="InvoiceDate">
                                            <InsertItemTemplate>
                                                <asp:TextBox ID="TextBoxInvoiceDate" runat="server" Text='<%# Bind("InvoiceDate") %>' TabIndex="3" AutoPostBack="true" OnTextChanged="TextBoxDate_TextChanged"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredInvoiceDate" ControlToValidate="TextBoxInvoiceDate" runat="server" Display="Dynamic" ErrorMessage="Invoice Date is required." ForeColor="Red" ValidationGroup="DetailsView" EnableClientScript="false">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidatorIDate" runat="server" ControlToValidate="TextBoxInvoiceDate" Display="Dynamic" ErrorMessage="Invalid Invoice Date" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="DetailsView">*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Invoice #" SortExpression="InvoiceNumber">
                                            <InsertItemTemplate>
                                                <asp:TextBox ID="TextBoxInvoice" runat="server" Text='<%# Bind("InvoiceNumber") %>' TabIndex="4" OnTextChanged="TextBoxInvoice_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredInvoice" ControlToValidate="TextBoxInvoice" runat="server" Display="Dynamic" ErrorMessage="Invoice Number is required." ForeColor="Red" ValidationGroup="DetailsView" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                                <div class="messagealert" id="invoiceexists_alert_container"></div>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="W/E Date" SortExpression="PostDate">
                                            <InsertItemTemplate>
                                                <asp:TextBox ID="TextBoxWEDate" runat="server" Text='<%# Bind("PostDate") %>' TabIndex="5" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredWEDate" ControlToValidate="TextBoxWEDate" runat="server" Display="Dynamic" ErrorMessage="W/E Date is required." ForeColor="Red" ValidationGroup="DetailsView" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidatorWEDate" runat="server" ControlToValidate="TextBoxWEDate" Display="Dynamic" ErrorMessage="Invalid W/E Date" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="DetailsView">*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                            <InsertItemTemplate>
                                                <asp:Label ID="LabelTotal" runat="server" Text="$0.00"></asp:Label>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Food" SortExpression="Groceries">
                                            <InsertItemTemplate>
                                                <asp:TextBox ID="TextBoxGroceries" runat="server" Text='<%# Bind("Groceries") %>' TabIndex="6" AutoPostBack="true" OnTextChanged="UpdateInsertTotal"></asp:TextBox>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Milk" SortExpression="Groceries">
                                            <InsertItemTemplate>
                                                <asp:TextBox ID="TextBoxMilk" runat="server" Text='<%# Bind("Milk") %>' TabIndex="7" AutoPostBack="true" OnTextChanged="UpdateInsertTotal"></asp:TextBox>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Paper" SortExpression="Paper">
                                            <InsertItemTemplate>
                                                <asp:TextBox ID="TextBoxPaper" runat="server" Text='<%# Bind("Paper") %>' TabIndex="8" AutoPostBack="true" OnTextChanged="UpdateInsertTotal"></asp:TextBox>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cleaning" SortExpression="Cleaning">
                                            <InsertItemTemplate>
                                                <asp:TextBox ID="TextBoxCleaning" runat="server" Text='<%# Bind("Cleaning") %>' TabIndex="9" AutoPostBack="true" OnTextChanged="UpdateInsertTotal"></asp:TextBox>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Other" SortExpression="Other">
                                            <InsertItemTemplate>
                                                <asp:TextBox ID="TextBoxOther" runat="server" Text='<%# Bind("Other") %>' TabIndex="10" AutoPostBack="true" OnTextChanged="UpdateInsertTotal"></asp:TextBox>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Other Category" SortExpression="OtherCategoryID">
                                            <InsertItemTemplate>
                                                <asp:DropDownList ID="ddlOtherCats" runat="server" DataSourceID="SqlDataSourceOtherCats" DataTextField="OtherCategoryDesc" DataValueField="OtherCategoryID" SelectedValue='<%# Bind("OtherCategoryID") %>' TabIndex="11" ></asp:DropDownList>
                                                <asp:CustomValidator ID="CustomValidatorOtherCat" runat="server" ControlToValidate="ddlOtherCats" Display="Dynamic" ErrorMessage="Other Category must be selected if an other amt is entered." 
                                                    ForeColor="Red" OnServerValidate="CustomValidatorOtherCat_ServerValidate" ClientValidationFunction="ValidateOtherCats" ValidationGroup="DetailsView" EnableClientScript="true">*</asp:CustomValidator>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Notes" >
                                            <InsertItemTemplate>
                                                <asp:TextBox ID="TexBoxNotes" runat="server" Text='<%# Bind("PurchaseComments") %>' Rows="3" TextMode="MultiLine" TabIndex="12"></asp:TextBox>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ShowHeader="false">
                                            <InsertItemTemplate>
                                                <div style="text-align:center;font-size:larger;">
                                                    <asp:LinkButton runat="server" CommandName="insert"
                                                        ID="IInsert" ToolTip="Add" Text=" Add"
                                                        CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="DetailsView"  Width="100%"></asp:LinkButton>
                                                </div>
                                            </InsertItemTemplate>
                                        </asp:TemplateField>
                                    </Fields>
                                </asp:DetailsView>
                                <asp:ValidationSummary ID="ValidationSummaryInsert" runat="server" ForeColor="Red" ValidationGroup="DetailsView"  />
                                <div class="messagealert" id="alert_container">
                                </div>
                                
                            </div>
                        </div>
                    </div>
                    <div class="card">
                        <div class="card-header h5">
                            <a class="card-link" data-toggle="collapse" href="#cardCollapse2">
                                Existing Transmittals
                            </a>
                        </div>
                        <div id="cardCollapse2" class="collapse show" >
                            <div class="card-body">
                                <div id="DivFilters" class="row">
                                    <div class="col-sm-3 ">
                                        Import Status
                                        <asp:DropDownList ID="DDL_ImportStatus" runat="server" AutoPostBack="true" Height="30">
                                            <asp:ListItem Text="Editable Only" Value="0" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Locked & Editable" Value="1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-3 ">
                                        Date Range
                                        <asp:DropDownList ID="DDLDateRanges" runat="server" AutoPostBack="true" Height="30" OnSelectedIndexChanged="DDLDateRanges_SelectedIndexChanged">
                                            <asp:ListItem Text="Last 30 Days" Value="30" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Last 60 Days" Value="60" ></asp:ListItem>
                                            <asp:ListItem Text="Last 90 Days" Value="90" ></asp:ListItem>
                                            <asp:ListItem Text="All Dates" Value="99999" ></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:HiddenField ID="HFBeginningDate" runat="server" />
                                    </div>
                                    <div class="col-sm-5 ">
                                        Search:<asp:TextBox ID="TextBoxClientSearch" runat="server" onFocus="this.select()"></asp:TextBox>
                                        <asp:LinkButton ID="ISearch" runat="server" CausesValidation="False" 
                                            ToolTip="Search" Text="Search"
                                            CssClass="fas fa-search" OnClick="ISearch_Click"
                                            />
                                        &nbsp;
                                        <asp:LinkButton ID="IClear" runat="server" CausesValidation="False" 
                                            ToolTip="Clear Search" forecolor="#333333" Text="Clear"
                                            CssClass="fas fa-undo-alt" OnClick="IClear_Click"   />
                                    </div>
                                </div>
                                <asp:GridView ID="GridViewPurchases" runat="server" AutoGenerateColumns="False" DataKeyNames="Counter" DataSourceID="SqlDataSourcePurchases" 
                                    CssClass="mGrid" AllowSorting="true" OnRowUpdating="GridViewPurchases_RowUpdating" OnRowCommand="GridViewPurchases_RowCommand" >
                                    <Columns>
                                        <asp:TemplateField HeaderText="Vendor" SortExpression="VENDNAME">
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlVendorsEdit" runat="server" DataSourceID="SqlDataSourceDistVendors" DataTextField="VENDNAME" DataValueField="VENDORID" 
                                                        TabIndex="21" OnDataBound="ddlVendorsEdit_DataBound" AppendDataBoundItems="true" Width="200"  >

                                                </asp:DropDownList><br />
                                                <asp:Label ID="LabelInactiveNote" runat="server" Text="*=Inactive Vendor" Visible="false"></asp:Label>
                                                <asp:HiddenField ID="HFSelectedVendorID" runat="server" Value='<%# Eval("VendorID") %>' />
                                                <asp:HiddenField ID="HFVendorName" runat="server" Value='<%# Eval("VENDNAME") %>' />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LabelVendor" runat="server" Text='<%# Eval("VENDNAME") %>' Width="200"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="School" SortExpression="SchoolNum" >
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="DropDownListSchools" runat="server" DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" 
                                                    DataValueField="SchoolNum" TabIndex="2" SelectedValue='<%# Bind("SchoolNum") %>' AppendDataBoundItems="true">
                                                    <asp:ListItem Text="--Select One--" Value="0"></asp:ListItem>
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LabelSchoolName" runat="server" Text='<%# Eval("SchoolName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Invoice Date" SortExpression="InvoiceDate">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBoxEditInvoiceDate" runat="server" Text='<%# Bind("InvoiceDate", "{0:d}") %>' OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="90" 
                                                    TabIndex="21"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredInvDate" ControlToValidate="TextBoxEditInvoiceDate" runat="server" Display="Dynamic" ErrorMessage="Invoice Date is required." ForeColor="Red" ValidationGroup="GridView" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidatorInvDate" runat="server" ControlToValidate="TextBoxEditInvoiceDate" Display="Dynamic" ErrorMessage="Invalid Invoice Date" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="GridView">*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LabelInvoiceDate" runat="server" Text='<%# Bind("InvoiceDate", "{0:d}" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Invoice#" SortExpression="InvoiceNumber">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBoxEditInvoice" runat="server" Text='<%# Bind("InvoiceNumber") %>' Width="80" TabIndex="21" OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredInvoiceEdit" ControlToValidate="TextBoxEditInvoice" runat="server" Display="Dynamic" ErrorMessage="Invoice Number is required." ForeColor="Red" ValidationGroup="GridView" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LabelInvoiceNumber" runat="server" Text='<%# Bind("InvoiceNumber") %>' Width="80"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="W/E Date" SortExpression="PostDate">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBoxEditWEDate" runat="server" Text='<%# Bind("PostDate", "{0:d}") %>' OnTextChanged="TextBoxDate_TextChanged" AutoPostBack="true" Width="80" 
                                                    TabIndex="21"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredWEDateEdit" ControlToValidate="TextBoxEditWEDate" runat="server" Display="Dynamic" ErrorMessage="W/E Date is required." ForeColor="Red" ValidationGroup="GridView" EnableClientScript="true">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidatorWEDateEdit" runat="server" ControlToValidate="TextBoxEditWEDate" Display="Dynamic" ErrorMessage="Invalid W/E Date" ForeColor="Red" ValidationExpression="^\d{1,2}/\d{1,2}/\d{4}$" ValidationGroup="GridView">*Use mm/dd/yyyy</asp:RegularExpressionValidator>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LabelPostDate" runat="server" Text='<%# Bind("PostDate", "{0:d}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total">
                                            <ItemTemplate>
                                                <div style="text-align:center;font-weight:bold;">
                                                    <asp:Label ID="LabelTotal" runat="server" Text='<%# (decimal)Eval("Groceries") + (decimal)Eval("Milk") + (decimal)Eval("Paper") + (decimal)Eval("Cleaning") + (decimal)Eval("Other") %>' Width="70"></asp:Label>
                                                </div>
                                            </ItemTemplate>                        
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Food" SortExpression="Groceries">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBoxEditGroceries" runat="server" Text='<%# Bind("Groceries") %>' Width="60" TabIndex="21" ForeColor='<%# (Decimal)Eval("Groceries")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <div style="text-align:center;">
                                                    <asp:Label ID="LabelGroceries" runat="server" Text='<%# Eval("Groceries") %>' ForeColor='<%# (Decimal)Eval("Groceries")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Milk" SortExpression="Milk">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBoxEditMilk" runat="server" Text='<%# Bind("Milk") %>' Width="60" TabIndex="21" ForeColor='<%# (Decimal)Eval("Milk")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <div style="text-align:center;">
                                                    <asp:Label ID="LabelMilk" runat="server" Text='<%# Eval("Milk") %>' ForeColor='<%# (Decimal)Eval("Milk")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Paper" SortExpression="Paper">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBoxEditPaper" runat="server" Text='<%# Bind("Paper") %>' Width="60" TabIndex="21" ForeColor='<%# (Decimal)Eval("Paper")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <div style="text-align:center;">
                                                    <asp:Label ID="LabelPaper" runat="server" Text='<%# Eval("Paper") %>' ForeColor='<%# (Decimal)Eval("Paper")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cleaning" SortExpression="Cleaning">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBoxEditCleaning" runat="server" Text='<%# Bind("Cleaning") %>' Width="60" TabIndex="21" ForeColor='<%# (Decimal)Eval("Cleaning")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <div style="text-align:center;">
                                                    <asp:Label ID="LabelCleaning" runat="server" Text='<%# Eval("Cleaning") %>' ForeColor='<%# (Decimal)Eval("Cleaning")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Other" SortExpression="Other">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBoxEditOther" runat="server" Text='<%# Bind("Other") %>' Width="60" TabIndex="21" ForeColor='<%# (Decimal)Eval("Other")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <div style="text-align:center;">
                                                    <asp:Label ID="LabelOther" runat="server" Text='<%# Eval("Other") %>' ForeColor='<%# (Decimal)Eval("Other")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Other Category" SortExpression="OtherCategoryID">
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="HFOtherPurchCatID" runat="server" Value='<%# Eval("OtherCategoryID") %>' />
                                                <asp:HiddenField ID="HFOtherPurchCatName" runat="server" Value='<%# Eval("OtherCategoryDesc") %>' />
                                                <asp:DropDownList ID="ddlOtherCatsEdit" runat="server" DataSourceID="SqlDataSourceOtherCats" DataTextField="OtherCategoryDesc" DataValueField="OtherCategoryID" 
                                                    TabIndex="21" OnDataBound="ddlOtherCatsEdit_DataBound" AppendDataBoundItems="true" ></asp:DropDownList>
                                                <asp:Label ID="LabelInactiveCat" runat="server" Text="*=Inactive Category" Visible="false"></asp:Label>
                                                <asp:CustomValidator ID="CustomValidatorOtherCatEdit" runat="server" ControlToValidate="ddlOtherCatsEdit" Display="Dynamic" ErrorMessage="Other Category must be selected if an other amt is entered." 
                                                    ForeColor="Red" OnServerValidate="CustomValidatorOtherCat_ServerValidate" ClientValidationFunction="ValidateOtherCats" ValidationGroup="GridView" EnableClientScript="true">*</asp:CustomValidator>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LabelInactiveCats" runat="server" Text='<%# Eval("InactiveCats") %>' ForeColor='<%# (Decimal)Eval("Other")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>'></asp:Label>
                                                <asp:Label ID="LabelOtherCat" runat="server" Text='<%# Eval("OtherCategoryDesc") %>' ForeColor='<%# (Decimal)Eval("Other")==0? System.Drawing.Color.White:System.Drawing.Color.DimGray %>' ></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Notes">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TexBoxNotes" runat="server" Text='<%# Bind("PurchaseComments") %>' Rows="3" TextMode="MultiLine" TabIndex="21"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="LabelNotes" runat="server" Text='<%# Eval("PurchaseComments") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div style="min-width:165px;" <%# Eval("Imported").ToString() =="0" && (bool)Eval("Locked")==false ? " " : "hidden='hidden'" %> >
                                                        <asp:LinkButton runat="server" CommandName="Edit"
                                                            ID="IEdit" ToolTip="Edit" Text=" Edit"
                                                            CssClass="fas fa-pencil-alt mGridLinkButton" ForeColor="#336699"    />
                                                        <asp:LinkButton runat="server" CommandName="Delete"
                                                            ID="IDelete" ToolTip="Delete" Text=" Delete"
                                                            CssClass="fa fa-trash-alt mGridLinkButton" ForeColor="Crimson"
                                                            OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                                                    </div>
                                                    <div style="text-align:center;">
                                                        <asp:Label runat="server" ID="LabelImported" Text="*LOCKED*" Visible='<%# Eval("Imported").ToString() =="0" && (bool)Eval("Locked")==false ? false : true %>' ></asp:Label>
                                                        
                                                            <asp:LinkButton runat="server" CommandName="Unlock" ID="LinkButtonUnlock" ToolTip="Unlock" Text=" Unlock"
                                                                CssClass="fas fa-unlock mGridLinkButton" ForeColor="Green" Visible='<%# Session["AccessLevel"].ToString().ToUpper()=="ADMIN" && (Eval("Imported").ToString() =="1" || (bool)Eval("Locked")==true) ? true : false %>' 
                                                                CommandArgument='<%# Eval("Counter") %>' />
                                                        
                                                    </div>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <div style="min-width:165px;">
                                                        <asp:LinkButton runat="server" CommandName="Update"
                                                            ID="ISave" ToolTip="Save" Text=" Save" TabIndex="21"
                                                            CssClass="fas fa-save mGridLinkButton" ForeColor="Green" CausesValidation="true" ValidationGroup="GridView" />
                                                        <asp:LinkButton runat="server" CommandName="Cancel"
                                                            ID="ICancel" ToolTip="Cancel" Text=" Cancel" TabIndex="21"
                                                            CssClass="fas fa-undo-alt mGridLinkButton" ForeColor="Red" CausesValidation="false" />
                                                    </div>
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div id="datasources">
                <asp:SqlDataSource ID="SqlDataSourceOtherCats" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT [OtherCategoryID], [OtherCategoryDesc] FROM [OtherPurchaseCategories] 
                    WHERE [ProgramYear]=@ProgramYear AND [OtherCategoryID] IN (SELECT [OtherCategoryID] FROM [DistrictAllowedOtherCats] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID )
                    ORDER BY [OtherCategoryDesc]">
                    <SelectParameters>
                        <asp:SessionParameter Name="ProgramYear" Type="String" SessionField="ProgramYear" />
                        <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceDistVendors" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT D.VENDORID, V.VENDNAME FROM [dbo].[DistrictVendors] D 
                        INNER JOIN [dbo].[Vendors] V ON D.VENDORID=V.VENDORID 
                        WHERE VENDSTTS=1 AND [DistrictID]=@DistrictID 
                        ORDER BY VENDNAME">
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourcePurchases" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    DeleteCommand="DELETE FROM [Purchases] WHERE [Counter] = @Counter" 
                    InsertCommand="INSERT INTO [Purchases] ([DistrictID], [InvoiceDate], [PostDate], [VendorID], [InvoiceNumber], [Groceries], [Paper], 
                        [Cleaning], [Other], [OtherCategoryID], [Imported], [ImportDate], [PurchaseComments], [Milk], [SchoolNum], [ProgramYear]) 
                        VALUES (@DistrictID, @InvoiceDate, @PostDate, @VendorID, @InvoiceNumber, @Groceries, @Paper, 
                        @Cleaning, @Other, @OtherCategoryID, @Imported, @ImportDate, @PurchaseComments, @Milk, @SchoolNum, @ProgramYear)" 
                    SelectCommand="SELECT [Purchases].*, ISNULL(V.VENDNAME, '*NOT FOUND* (' + [Purchases].VENDORID + ')' ) VENDNAME, O.OtherCategoryDesc, ISNULL(DS.SchoolName, '--None Selected--') SchoolName,
                        CAST([Groceries] AS VARCHAR(MAX)) GrocString, CAST([Milk] AS VARCHAR(MAX)) MilkString, CAST([Paper] AS VARCHAR(MAX)) PaperString, CAST([Cleaning] AS VARCHAR(MAX)) CleanString,
                        CAST([Other] AS VARCHAR(MAX)) OtherString, CAST([Groceries]+[Milk]+[Paper]+[Cleaning]+[Other] AS VARCHAR(MAX)) TotalString, IIF(DA.OtherCategoryID IS NULL, '*', '') InactiveCats
                        FROM [Purchases] 
                        LEFT OUTER JOIN [Vendors] V ON [Purchases].VENDORID=V.VENDORID 
                        LEFT OUTER JOIN [OtherPurchaseCategories] O ON [PURCHASES].[OtherCategoryID]=O.[OtherCategoryID] AND [PURCHASES].[ProgramYear]=O.[ProgramYear]
                        LEFT OUTER JOIN [DistrictSchools] DS ON [PURCHASES].[DistrictID]=DS.[DistrictID] AND [PURCHASES].[SchoolNum]=DS.[SchoolNum]
                        LEFT OUTER JOIN [DistrictAllowedOtherCats] DA ON [PURCHASES].[ProgramYear]=DA.[ProgramYear] AND [PURCHASES].[DistrictID]=DA.[DistrictID] AND [PURCHASES].[OtherCategoryID]=DA.[OtherCategoryID]
                        WHERE [Purchases].[DistrictID]=@DistrictID AND [Purchases].ProgramYear=@ProgramYear
                        ORDER BY [InvoiceDate] DESC" 
                    UpdateCommand="UPDATE [Purchases] SET [InvoiceDate] = @InvoiceDate, [PostDate] = @PostDate, 
                        [VendorID] = @VendorID, [InvoiceNumber] = @InvoiceNumber, [Groceries] = @Groceries, [Paper] = @Paper, [Cleaning] = @Cleaning, 
                        [Other] = @Other, [OtherCategoryID] = CASE WHEN @Other=0 THEN 'NONE' ELSE @OtherCategoryID END, [PurchaseComments]=@PurchaseComments, [Milk]=@Milk, [SchoolNum]=@SchoolNum 
                        WHERE [Counter] = @Counter" OnInserted="SqlDataSourcePurchases_Inserted" OnUpdating="SqlDataSourcePurchases_Updating"
                    FilterExpression="[Imported]<={0} AND [InvoiceDate]>='{1}' AND [Locked]<={0} AND (
                        VENDNAME LIKE '%{2}%' OR
                        SchoolName LIKE '%{2}%' OR
                        [InvoiceNumber] LIKE '%{2}%' OR
                        PurchaseComments LIKE '%{2}%' OR
                        OtherCategoryDesc LIKE '%{2}%' OR
                        GrocString LIKE '%{2}%' OR
                        MilkString LIKE '%{2}%' OR
                        PaperString LIKE '%{2}%' OR
                        CleanString LIKE '%{2}%' OR
                        OtherString LIKE '%{2}%' OR
                        TotalString LIKE '%{2}%'
                    )">
                    <FilterParameters>
                        <asp:ControlParameter ControlID="DDL_ImportStatus" PropertyName="SelectedValue" Type="Int16" Name="Imported" />
                        <asp:ControlParameter ControlID="HFBeginningDate" PropertyName="Value" DbType="DateTime" Name="DateRange" />
                        <asp:ControlParameter ControlID="TextBoxClientSearch" Name="SearchString" PropertyName="Text" Type="String" DefaultValue="%" />
                    </FilterParameters>
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                        <asp:SessionParameter Name="ProgramYear" Type="String" SessionField="ProgramYear" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="Counter" Type="Int64" />
                    </DeleteParameters>
                    <InsertParameters>
                        <asp:SessionParameter Name="DistrictID" Type="String" SessionField="CurrentDistrict" />
                        <asp:Parameter Name="InvoiceDate" Type="DateTime" />
                        <asp:Parameter Name="PostDate" Type="DateTime" />
                        <asp:Parameter Name="VendorID" Type="String" />
                        <asp:Parameter Name="InvoiceNumber" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"/>
                        <asp:Parameter Name="Groceries" Type="Decimal" DefaultValue="0"/>
                        <asp:Parameter Name="Paper" Type="Decimal" DefaultValue="0"/>
                        <asp:Parameter Name="Cleaning" Type="Decimal" DefaultValue="0"/>
                        <asp:Parameter Name="Other" Type="Decimal" DefaultValue="0"/>
                        <asp:Parameter Name="OtherCategoryID" Type="String" DefaultValue=""/>
                        <asp:Parameter Name="Imported" Type="Int16" DefaultValue="0" />
                        <asp:Parameter Name="ImportDate" Type="DateTime" DefaultValue="1/1/1900" />
                        <asp:Parameter Name="PurchaseComments" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"/>
                        <asp:Parameter Name="Milk" Type="Decimal" DefaultValue="0"/>
                        <asp:Parameter Name="SchoolNum" Type="Int16" DefaultValue="0"/>
                        <asp:SessionParameter Name="ProgramYear" Type="String" SessionField="ProgramYear" />
                    </InsertParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="DistrictID" Type="String" />
                        <asp:Parameter Name="InvoiceDate" Type="DateTime" />
                        <asp:Parameter Name="PostDate" Type="DateTime" />
                        <asp:Parameter Name="VendorID" Type="String"   />
                        <asp:Parameter Name="InvoiceNumber" Type="String" />
                        <asp:Parameter Name="Groceries" Type="Decimal" />
                        <asp:Parameter Name="Paper" Type="Decimal" />
                        <asp:Parameter Name="Cleaning" Type="Decimal" />
                        <asp:Parameter Name="Other" Type="Decimal" />
                        <asp:Parameter Name="OtherCategoryID" Type="String" />
                        <asp:Parameter Name="Imported" Type="Int16" />
                        <asp:Parameter Name="ImportDate" Type="DateTime" />
                        <asp:Parameter Name="Counter" Type="Int64" />
                        <asp:Parameter Name="PurchaseComments" Type="String" DefaultValue="" ConvertEmptyStringToNull="false"/>
                        <asp:Parameter Name="Milk" Type="Decimal" DefaultValue="0"/>
                        <asp:Parameter Name="SchoolNum" Type="Int16" DefaultValue="0"/>
                    </UpdateParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
                    SELECTCOMMAND="SELECT SchoolNum, SchoolName FROM DistrictSchools WHERE DistrictID=@DistrictID AND INACTIVE=0 ORDER BY SortOrder, SchoolName">
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
