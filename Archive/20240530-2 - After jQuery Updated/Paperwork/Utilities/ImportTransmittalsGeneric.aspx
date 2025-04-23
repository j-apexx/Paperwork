<%@ Page Title="Import - Transmittals" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportTransmittalsGeneric.aspx.cs" Inherits="Paperwork.ImportTransmittalsGeneric" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        $(function() {
            BindEvents();
        });

        function BindEvents() {
            var PYearStart = $("[id*=HFProgYearStartDate]").val();
            var PYearEnd = $("[id*=HFProgYearEndDate]").val();

            $(function () {
                $("[id*=TextBoxPostDateSpecified]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxMonthEndDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            $(function () {
                $("[id*=TextBoxInvoiceDateSpecified]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    minDate: PYearStart,
                    maxDate: PYearEnd
                });
            });

            //When an input is focused - select the text
            $('body').on('focus', 'input', function (e) {
                $(this)
                    .one('mouseup', function () {
                        $(this).select();
                        return false;
                    })
                    .select();
            });

            /* Code required to update the file name in the upload box */
            $(".custom-file-input").on("change", function () {
                var fileName = $(this).val().split("\\").pop();
                $(this).siblings(".custom-file-label").html(fileName);
            });
        };
    </script>
    <div id="dataUpload">
        <div class="container py-3">  
            <div class="card">  
                <div class="card-header bg-primary text-uppercase text-white">  
                    <h5>Generic Transmittals Import </h5>  
                </div>  
                <div class="card-header bg-secondary text-uppercase text-white">  
                    Use existing import configuration:  
                    <asp:DropDownList ID="DDLConfigs" runat="server" DataSourceID="SqlDataSourceImportConfigs" DataTextField="ImportConfigurationName" DataValueField="ImportConfigurationName" AutoPostBack="true" Width="300"
                         OnSelectedIndexChanged="DDLConfigs_SelectedIndexChanged"></asp:DropDownList>
                    <asp:LinkButton ID="IUpdate"  runat="server" ToolTip="Update Settings" OnClick="IUpdate_Click" Width="150" >
                        <span class="fa-stack fa-2x" >
                            <i class="fas fa-save fa-stack-1x" style="color:palegreen;" ></i>
                        </span>
                        <span style="font-weight:bold;text-decoration: none; color:white;left:-25px;position:relative;vertical-align:middle;">Update</span>
                    </asp:LinkButton>
                    <asp:LinkButton ID="ISaveAs"  runat="server" ToolTip="Save As New Setting" OnClick="ISaveAs_Click" Width="150" >
                        <span class="fa-stack fa-2x" >
                            <i class="fas fa-save fa-stack-1x" style="color:palegreen;" ></i>
                            <i class="fas fa-plus fa-stack-1x fa-pull-right" style="color:gold;"></i>
                        </span>
                        <span style="font-weight:bold;text-decoration: none; color:white;left:-15px;position:relative;vertical-align:middle;">Save As</span>
                    </asp:LinkButton>
                    <asp:LinkButton ID="iDelete"  runat="server" ToolTip="Delete Settings"  Width="150" >
                        <span class="fa-stack fa-2x" >
                            <i class="fas fa-trash-alt fa-stack-1x" style="color:indianred;" ></i>
                        </span>
                        <span style="font-weight:bold;text-decoration: none; color:white;left:-25px;position:relative;vertical-align:middle;">Delete</span>
                    </asp:LinkButton>



                </div> 
                <div class="card-body">  
                    <div class="row">
                        <div class="col-sm-8 " >
                            Imports to Paperwork Districts
                        </div>
                    </div> 
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Program Year:
                        </div>
                        <div class="col-sm-4 " >
                            <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                            <asp:HiddenField ID="HFProgYearStartDate" runat="server" />
                            <asp:HiddenField ID="HFProgYearEndDate" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Post Date:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBPostDateFromImport" runat="server" Text="Get Post Date From Import" AutoPostBack="true" OnCheckedChanged="CBPostDateFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelPostDateCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxPostDateCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelPostDateSpecified" runat="server" Text="Post Date:" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxPostDateSpecified" runat="server" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-1" >&nbsp;</div>
                        <div class="col-sm-7 " >
                            <asp:Label ID="LabelSplitDate" runat="server">
                                At month end in a split week enter the later date to post invoices to.  Otherwise enter the same date as above.  Invoices dated after the above date will post to this date.
                            </asp:Label>
                        </div>
                        <div class="col-sm-4 " >
                            <asp:Label ID="Label1" runat="server" Text="Post Date2:" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxMonthEndDate" runat="server" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Vendor:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBVendorFromImport" runat="server" Text="Get Vendor ID From Import" AutoPostBack="true" OnCheckedChanged="CBVendorFromImport_CheckedChanged" Visible="false" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelVendorIDCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxVendorIDCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelVendorIDSpecified" runat="server" Text="Vendor ID:"  Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxVendorIDSpecified" runat="server" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>




                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Customer Number Mapping (check box and specify vendor customer column):
                        </div>
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;" >
                            <asp:CheckBox ID="CBUseVendCustMap" runat="server" Text="Use Vendor-Customer Mapping" AutoPostBack="true" OnCheckedChanged="CBUseVendCustMap_CheckedChanged" />
                            <br />
                            *Retrieves Dist and School from setup
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelVendCustCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxVendCustCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" id="divDistrict" runat="server">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            District:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBDistFromImport" runat="server" Text="Get District ID From Import" AutoPostBack="true" OnCheckedChanged="CBDistFromImport_CheckedChanged" />
                            
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelDistCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxDistCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelDistSpecified" runat="server" Text="District ID:" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxDistSpecified" runat="server" Text="" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" id="divSchool" runat="server">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            School/Bldg:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBSchoolNumFromImport" runat="server" Text="Get School From Import" AutoPostBack="true" OnCheckedChanged="CBSchoolNum_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBUseDefaultSchool" runat="server" Text="Use Default School" AutoPostBack="true" OnCheckedChanged="CBSchoolNum_CheckedChanged" />
                            
                            <asp:Label ID="LabelSchoolNumCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxSchoolNumCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>

                            <asp:Literal ID="LiteralSchoolNumSpecified" runat="server" ><br /></asp:Literal>
                            <asp:Label ID="LabelSchoolNumSpecified" runat="server" Text="School:" Width="125" ></asp:Label>
                            <asp:TextBox ID="TextBoxSchoolNumSpecified" runat="server" Text="" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-4" style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Invoice Date:
                        </div>
                        <div class="col-sm-4" style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBInvDateFromImport" runat="server" Text="Get Invoice Date From Import" AutoPostBack="true" OnCheckedChanged="CBInvDateFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4" style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelInvDateCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxInvDateCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelInvDateSpecified" runat="server" Text="Invoice Date:" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxInvoiceDateSpecified" runat="server" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>



                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Invoice Number:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBInvNumFromImport" runat="server" Text="Get Invoice Number From Import" AutoPostBack="true" OnCheckedChanged="CBInvNumFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelInvNumCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxInvNumCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelInvNumSpecified" runat="server" Text="Invoice Number:" Width="125" ></asp:Label>
                            <asp:TextBox ID="TextBoxInvNumSpecified" runat="server" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            $-Food:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBFoodAmtFromImport" runat="server" Text="Get Food Amount From Import" AutoPostBack="true" OnCheckedChanged="CBFoodAmtFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelFoodAmtCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxFoodAmtCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelFoodAmtSpecified" runat="server" Text="$-Food:"  Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxFoodAmtSpecified" runat="server" Text="0" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            $-Milk:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBMilkAmtFromImport" runat="server" Text="Get Milk Amount From Import" AutoPostBack="true" OnCheckedChanged="CBMilkAmtFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelMilkAmtCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxMilkAmtCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelMilkAmtSpecified" runat="server" Text="$-Milk:" Width="125" ></asp:Label>
                            <asp:TextBox ID="TextBoxMilkAmtSpecified" runat="server" Text="0" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            $-Paper:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBPaperAmtFromImport" runat="server" Text="Get Paper Amount From Import" AutoPostBack="true" OnCheckedChanged="CBPaperAmtFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelPaperAmtCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxPaperAmtCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelPaperAmtSpecified" runat="server" Text="$-Paper:" Width="125" ></asp:Label>
                            <asp:TextBox ID="TextBoxPaperAmtSpecified" runat="server" Text="0" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            $-Cleaning:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBCleaningAmtFromImport" runat="server" Text="Get Cleaning Amount From Import" AutoPostBack="true" OnCheckedChanged="CBCleaningAmtFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelCleanAmtCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxCleanAmtCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelCleanAmtSpecified" runat="server" Text="$-Cleaning:" Width="125" ></asp:Label>
                            <asp:TextBox ID="TextBoxCleanAmtSpecified" runat="server" Text="0" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            $-Other:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBOtherAmtFromImport" runat="server" Text="Get Other Amount From Import" AutoPostBack="true" OnCheckedChanged="CBOtherAmtFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelOtherAmtCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxOtherAmtCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelOtherAmtSpecified" runat="server" Text="$-Other:" Width="125" ></asp:Label>
                            <asp:TextBox ID="TextBoxOtherAmtSpecified" runat="server" Text="0" Width="100" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Other Category:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBOtherCatFromImport" runat="server" Text="Get Other Category From Import" AutoPostBack="true" OnCheckedChanged="CBOtherCatFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelOtherCatCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxOtherCatCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelOtherCatSpecified" runat="server" Text="Other:" ></asp:Label>
                            <asp:DropDownList ID="ddlOtherCats" runat="server" DataSourceID="SqlDataSourceOtherCats" DataTextField="OtherCategoryDesc" DataValueField="OtherCategoryID"  ></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Purchase Comments:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBPurchCommntsFromImport" runat="server" Text="Get Comments From Import" AutoPostBack="true" OnCheckedChanged="CBPurchCommntsFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelPurchCmmntCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxPurchCmmntCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>
                            <asp:Label ID="LabelPurchCmmntSpecified" runat="server" Text="Comments:"  Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxPurchCmmntSpecified" runat="server" Text="" Width="200" AutoCompleteType="Disabled"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Record status after import:
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBLockRecords" runat="server" Text="Lock Records" />
                        </div>
                        <div class="col-sm-4 "  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            &nbsp;
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4 " style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            Program Type:
                        </div>
                        <div class="col-sm-4"  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:CheckBox ID="CBProgramTypeFromImport" runat="server" Text="Get Program Type From Import" AutoPostBack="true" OnCheckedChanged="CBProgramTypeFromImport_CheckedChanged" />
                        </div>
                        <div class="col-sm-4"  style="border:dotted;border-width:1px 0px 0px 0px;border-color:darkgrey;">
                            <asp:Label ID="LabelProgTypeCol" runat="server" Text="Column#:" Visible="false" Width="125"></asp:Label>
                            <asp:TextBox ID="TextBoxProgTypeCol" runat="server" Text="" Width="100" Visible="false"></asp:TextBox>

                            <asp:Label ID="LabelProgTypeSpecified" runat="server" Text="School:"  Width="125"></asp:Label>
                            <asp:DropDownList ID="DDLProgramType" runat="server" DataSourceID="SqlDataSourceProgramTypes" DataTextField="ProgramType"
                                DataValueField="ProgramType" TabIndex="0" OnDataBound="DDLProgramType_DataBound" ></asp:DropDownList>

                        </div>
                    </div>
                    <hr />
                    <div class="row" style="background-color:lightgrey;border-radius:5px;padding-top:10px;padding-bottom:10px;">
                        <div class="col-sm-4">
                            File Type:
                            <asp:DropDownList ID="DDLFileType" runat="server" >
                                <asp:ListItem Text="Text File - *.csv" Value="CSV" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Excel File - *.xlsx" Value="XLSX"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-4" >
                            <asp:CheckBox ID="CBFileHasHeaders" runat="server" Text="Import file has headers" />
                        </div>
                        <div class="col-sm-4" >
                            <asp:LinkButton ID="BtnImportFile" runat="server" CssClass="btn btn-primary" data-target="#myModal" CausesValidation="true" ValidationGroup="Import" OnClick="BtnImportFile_Click">
                                <i class="fas fa-plus"></i>
                                Import Transmittal File  
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-4">
                            <asp:ValidationSummary ID="ValidImportSummary" runat="server" DisplayMode="BulletList" HeaderText="Before Importing:" ValidationGroup="Import" CssClass="validators"/>
                        </div>
                    </div>

                    <asp:UpdatePanel ID="UpdatePanelOpexWrapper" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            Status:<br />
                            <asp:TextBox ID="TextBoxStatus" runat="server" TextMode="MultiLine" Rows="1" ReadOnly="true" Width="100%" ></asp:TextBox>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <div class="modal fade" id="myModal">  
                        <div class="modal-dialog" >  
                            <div class="modal-content" style="width:800px;" > 
                                <div class="modal-header">  
                                    <h4 class="modal-title">Select Import File</h4>  
                                    <button type="button" class="close" data-dismiss="modal">×</button>  
                                </div>  
                                <div class="modal-body">  
                                    <div class="row">  
                                        <div class="col-md-12">  
                                            <div class="form-group">  
                                                <label>Choose import file</label>  
                                                <div class="input-group">  
                                                    <div class="custom-file">  
                                                        <asp:FileUpload ID="FileUpload1" CssClass="custom-file-input" runat="server"  />  
                                                        <label class="custom-file-label"></label>  
                                                    </div>  
                                                    
                                                    &nbsp;
                                                    <div>
                                                        <asp:CheckBox ID="CB_VerboseErrors" runat="server" Text="Show detailed errors" Checked="true"/>
                                                    </div>
                                                </div>  
                                                 
                                            </div>  
                                        </div>  
                                    </div>  
                                </div>  
                                <div class="modal-footer">  
                                    <asp:Button ID="ButtonUpld" runat="server" CssClass="btn btn-outline-primary" Text="Upload" OnClick="btnUpload_Click" /> 
                                    <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Close</button>  
                                </div>  
                            </div>  
                        </div>  
                    </div>  

                    <div class="modal fade" id="myModalConfigName">
                        <div class="modal-dialog" >  
                            <div class="modal-content" style="width:400px;" > 
                                <div class="modal-header">  
                                    <h4 class="modal-title">Specify Configuration Name</h4>  
                                    <button type="button" class="close" data-dismiss="modal">×</button>  
                                </div>  
                                <div class="modal-body">  
                                    <div class="row">  
                                        <div class="col-md-12">  
                                            <div class="form-group">  
                                                <label>Name</label>  
                                                <div class="input-group">  
                                                    <asp:TextBox ID="TextBoxConfigName" runat="server" MaxLength="50" Width="300"></asp:TextBox>
                                                </div>  
                                            </div>  
                                        </div>  
                                    </div>  
                                </div>  
                                <div class="modal-footer">  
                                    <asp:Button ID="ButtonSaveName" runat="server" CssClass="btn btn-outline-primary" Text="Save" OnClick="ButtonSaveName_Click" /> 
                                    <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Close</button>  
                                </div>  
                            </div>  
                        </div>  
                    </div>


                </div>  
            </div>  
        </div>  

    </div>
    <div id="datasources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT * FROM [ProgramYear] ORDER BY [StartDate] DESC" >
        </asp:SqlDataSource>  
        <asp:SqlDataSource ID="SqlDataSourceProgramTypes" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader"
            SELECTCOMMAND="SELECT 'School Year' ProgramType 
            UNION ALL 
            SELECT ProgramType FROM [ProgramTypeSetup] Order By ProgramType">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceOtherCats" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [OtherCategoryID], [OtherCategoryDesc] FROM [OtherPurchaseCategories] 
            WHERE [ProgramYear]=@ProgramYear 
            ORDER BY [OtherCategoryDesc]">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceImportConfigs" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT '' ImportConfigurationName UNION ALL            
            SELECT [ImportConfigurationName] FROM [PurchasesImportSettings] ORDER BY [ImportConfigurationName]" >
        </asp:SqlDataSource>  

    </div>
</asp:Content>
