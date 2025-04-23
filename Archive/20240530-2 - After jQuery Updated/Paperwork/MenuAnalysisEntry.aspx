<%@ Page Title="Menu Analysis Entry" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MenuAnalysisEntry.aspx.cs" Inherits="Paperwork.MenuAnalysisEntry" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);

        var w = $(window).width();
        var h = $(window).height();
        var wMargin = 10;
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

        $(function () {
            BindEvents();
        });


        function BindEvents() {
            var PosRefY = $("#DataEntryReferenceLocation").position().top;
            $('#SpecialFitDiv').width(w - wMargin);
            $("#SpecialFitDiv").height(h - hMargin);
            $('#SpecialFitDiv').css("top", PosRefY + 0 );
            $('#SpecialFitDiv').css("left", wMargin);


            //Enable Enter Key tabbing
            $('input,select,textarea').on("keypress", function (e) {
                /* ENTER PRESSED*/
                if (e.keyCode == 13) {

                    //This code moves between focusable elements. 
                    var $canfocus = $(':focusable').not("a[title|='Show All Items']");//"title", "Show All Items"
                    //var index = $canfocus.index(document.activeElement) + 1;
                    var index = $canfocus.index(this) + 1;

                    if (index >= $canfocus.length) index = 0;
                    $canfocus.eq(index).focus();//Handles Buttons
                    $canfocus.eq(index).select();//helps with dropdowns

                }

            });

            //Scroll fix to keep the day entry in view
            $("[id$=4_TextBoxFQty],[id$=9_TextBoxFQty]").on('focus', function (e) {
                var leftPos = $(this).offset().left;
                var scrLeft = $('#SpecialFitDiv').scrollLeft();
                //alert(leftPos + scrLeft);
                $('#SpecialFitDiv').animate({ scrollLeft: leftPos + scrLeft - wMargin - 3 }, 400);
            });

        };

        //When an input is focused - select the text
        $('body').on('focus', 'input', function (e) {
            $(this)
                .one('mouseup', function () {
                    $(this).select();
                    return false;
                })
                .select();
        });


        //apply highlight 
        $('body').on('focus', 'input,textarea,select', function (e) {
            $(this).addClass("highlight");
        });
        // remove highlight
        $('body').on('focusout', 'input,textarea,select', function (e) {
            $(this).removeClass("highlight");
        });
    </script>
    <style>
        .ui-datepicker-calendar {
            display: none;
        }
    </style>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"  >
        <ContentTemplate> 
            <div id="DataEntryReferenceLocation">
                <asp:HiddenField ID="hdnScrollPos" runat="server"/>
                <asp:HiddenField ID="HFProgramYearStartDate" runat="server" />
                <asp:HiddenField ID="HFSchoolType" runat="server" />
            </div>
            <div id="SpecialFitDiv" style="overflow-y:auto;background-color:white; position:absolute;" >
                <div id="topSection" style="width:99%;">
                    <div class="row">
                        <div class="col-4">
                            <div class="row">
                                <div class="col">
                                    <h2><%: Title %></h2>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-4">
                                    <strong>Program Year:</strong>
                                </div>
                                <div class="col-8">
                                    <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" 
                                    DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                                    <asp:Label ID="LabelYearWarning" runat="server" Text="<br/>Select previous program year to enter Menu Analysis." ForeColor="Red" Visible="false"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-4">
                                    <strong>District:</strong>
                                </div>
                                <div class="col-8">
                                        <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                                            DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                                            OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                                        </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-4">
                                    <strong>School:</strong>
                                </div>
                                <div class="col-8">
                                        <asp:DropDownList ID="DropDownListSchools" runat="server" AutoPostBack="True" 
                                            DataMember="DefaultView" DataSourceID="SqlDataSourceSchools" DataTextField="SchoolName" DataValueField="SchoolNum" ValidationGroup="GridView"
                                            OnSelectedIndexChanged="DropDownListSchools_SelectedIndexChanged">
                                        </asp:DropDownList> &nbsp;
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-4">
                                    <strong>Menu Cost Sheet:</strong>
                                </div>
                                <div class="col-8">
                                    <asp:DropDownList ID="DDLCategories" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DDLCategories_SelectedIndexChanged" >
                                        <asp:ListItem Text="Breakfast Entree" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Breakfast Fruit" Value="2" ></asp:ListItem>
                                        <asp:ListItem Text="Breakfast Cycle Avg Cost" Value="3" ></asp:ListItem>
                                        <asp:ListItem Text="Lunch Entree" Value="4" ></asp:ListItem>
                                        <asp:ListItem Text="Lunch Fruit" Value="5" ></asp:ListItem>
                                        <asp:ListItem Text="Lunch Vegetable" Value="6" ></asp:ListItem>
                                        <asp:ListItem Text="Lunch Cycle Avg Cost" Value="7" ></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="HFItemCategoryID" runat="server" />
                                    <asp:LinkButton ID="iCopy" runat="server" ToolTip="Copy" CssClass="far fa-copy " ForeColor="CadetBlue" Height="30" Width="30" OnClick="iCopy_Click" data-target="#myModal" ></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        <div class="col-3">
                            <div class="row">
                                <div class="col">&nbsp;</div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    Select District and School
                                    <br />
                                    In each block select the menu item offered according to the cycle menu
                                    <br />
                                    Leave No School Days Blank
                                    <br />
                                    Average costs will calculate automatically
                                    <div class="messagealert" id="alert_container" style="text-align:left;"></div>
                                </div>
                            </div>
                        </div>
                        <div class="col-5">
                            <div class="row">
                                <div class="col">&nbsp;</div>
                            </div>
                            <div class="row">
                                <div class="col" id="divFruitVegButtons" runat="server">
                                    <asp:LinkButton ID="IButtonAddVeg1" runat="server" ToolTip="Add Vegetable-1 to all menu days" OnClick="ApplyFruitVegAllChanges" Width="150" >
                                        <span class="fa-stack fa-1x">
                                            <i class="fas fa-carrot fa-stack-2x" style="color:coral;"></i>
                                            <i class="fas fa-plus fa-stack-1x" style="color:lawngreen;"></i>
                                        </span>
                                        <span style="font-weight:bold;text-decoration: none; color:gray;left:-10px;position:relative;vertical-align:middle;">Add Veg-1 All</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="IButtonAddVeg2" runat="server" ToolTip="Add Vegetable-2 to all menu days" OnClick="ApplyFruitVegAllChanges"  Width="150">
                                        <span class="fa-stack fa-1x">
                                            <i class="fas fa-carrot fa-stack-2x" style="color:coral;"></i>
                                            <i class="fas fa-plus fa-stack-1x" style="color:lawngreen;"></i>
                                        </span>
                                        <span style="font-weight:bold;text-decoration: none; color:gray;left:-10px;position:relative;vertical-align:middle;">Add Veg-2 All</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="IButtonAddFrt1" runat="server" ToolTip="Add Fruit-1 to all menu days" OnClick="ApplyFruitVegAllChanges"  Width="150">
                                        <span class="fa-stack fa-1x">
                                            <i class="fas fa-apple-alt fa-stack-2x" style="color:red;"></i>
                                            <i class="fas fa-plus fa-stack-1x" style="color:lawngreen;"></i>
                                        </span>
                                        <span style="font-weight:bold;text-decoration: none; color:gray;left:-10px;position:relative;vertical-align:middle;">Add Fruit-1 All</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="IButtonAddFrt2" runat="server" ToolTip="Add Fruit-2 to all menu days" OnClick="ApplyFruitVegAllChanges"  Width="150">
                                        <span class="fa-stack fa-1x">
                                            <i class="fas fa-apple-alt fa-stack-2x" style="color:red;"></i>
                                            <i class="fas fa-plus fa-stack-1x" style="color:lawngreen;"></i>
                                        </span>
                                        <span style="font-weight:bold;text-decoration: none; color:gray;left:-10px;position:relative;vertical-align:middle;">Add Fruit-2 All</span>
                                    </asp:LinkButton>
                                    <br />
                                    <asp:LinkButton ID="IButtonRemoveVeg1" runat="server" ToolTip="Remove Vegetable-1 from all menu days" OnClick="ApplyFruitVegAllChanges"  Width="150">
                                        <span class="fa-stack fa-1x">
                                            <i class="fas fa-carrot fa-stack-2x" style="color:darkgray;"></i>
                                            <i class="fas fa-minus fa-stack-1x" style="color:red;"></i>
                                        </span>
                                        <span style="font-weight:bold;text-decoration: none; color:gray;left:-10px;position:relative;vertical-align:middle;">Rem Veg-1 All</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="IButtonRemoveVeg2" runat="server" ToolTip="Remove Vegetable-2 from all menu days" OnClick="ApplyFruitVegAllChanges"  Width="150">
                                        <span class="fa-stack fa-1x">
                                            <i class="fas fa-carrot fa-stack-2x" style="color:darkgray;"></i>
                                            <i class="fas fa-minus fa-stack-1x" style="color:red;"></i>
                                        </span>
                                        <span style="font-weight:bold;text-decoration: none; color:gray;left:-10px;position:relative;vertical-align:middle;">Rem Veg-2 All</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="IButtonRemoveFrt1" runat="server" ToolTip="Remove Fruit-1 to from menu days" OnClick="ApplyFruitVegAllChanges"  Width="150">
                                        <span class="fa-stack fa-1x">
                                            <i class="fas fa-apple-alt fa-stack-2x" style="color:darkgray;"></i>
                                            <i class="fas fa-minus fa-stack-1x" style="color:red;"></i>
                                        </span>
                                        <span style="font-weight:bold;text-decoration: none; color:gray;left:-10px;position:relative;vertical-align:middle;">Rem Fruit-1 All</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="IButtonRemoveFrt2" runat="server" ToolTip="Remove Fruit-2 from all menu days" OnClick="ApplyFruitVegAllChanges"  Width="150">
                                        <span class="fa-stack fa-1x">
                                            <i class="fas fa-apple-alt fa-stack-2x" style="color:darkgray;"></i>
                                            <i class="fas fa-minus fa-stack-1x" style="color:red;"></i>
                                        </span>
                                        <span style="font-weight:bold;text-decoration: none; color:gray;left:-10px;position:relative;vertical-align:middle;">Rem Fruit-2 All</span>
                                    </asp:LinkButton>
                                    <br />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col" id="divMenuAdd" runat="server">
                                    <asp:DropDownList ID="DDLMenuItemsAll" runat="server" DataSourceID="SqlDataSourceMenuItems" DataTextField="ItemDescription" DataValueField="ItemID" Width="250" Height="30" ></asp:DropDownList>
                                    <asp:LinkButton ID="IButtonAddItem" runat="server" ToolTip="Add menu item to all menu days" OnClick="ApplyMenuItemAllChanges"  Width="90" CssClass="fas fa-plus mGridLinkButton" ForeColor="LawnGreen" Text="Add" />
                                    <asp:LinkButton ID="iButtonRemoveItem" runat="server" ToolTip="Remove menu item from all menu days" OnClick="ApplyMenuItemAllChanges"  Width="90" CssClass="fas fa-minus-circle mGridLinkButton" ForeColor="Red" Text="Remove" />
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                
           
                <div id="DataEntry" style="width:99%;" >
                    <asp:HiddenField ID="HFCatMidLower" runat="server" />
                    <asp:HiddenField ID="HFCatMidUpper" runat="server" />

                    <asp:ListView runat="server" ID="ListViewMenuDays" OnItemInserting="ListViewMenuDays_ItemInserting"
                        DataSourceID="SqlDataSourceDayNumbers" GroupItemCount="5" OnItemDataBound="ListViewMenuDays_ItemDataBound" >
                      <LayoutTemplate>
                        <table runat="server" id="table1" style="width:100%;"  >
                          <tr runat="server" id="groupPlaceholder" >
                          </tr>
                        </table>
                      </LayoutTemplate>
                      <GroupTemplate>
                        <tr runat="server" id="tableRow">
                          <td runat="server" id="itemPlaceholder" />
                        </tr>
                      </GroupTemplate>
                      <ItemTemplate>
                        <td runat="server" style="width:20%;height:80px;border-block-style:solid;border-width:1px;border-color:grey;vertical-align:top;">
                            <%-- Data-bound content. --%>
                            <asp:UpdatePanel ID="UpdatePanelLVItem" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="DDLVeg1Outside" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="DDLVeg2Outside" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="DDLFrt1Outside" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="DDLFrt2Outside" EventName="SelectedIndexChanged" />
                                </Triggers>
                                <ContentTemplate>
                                    <strong><asp:Label ID="LabelDayNumber" runat="server" Text='<%#Eval("DayNumber") %>' /></strong>

                                    <table id="AveragesTable" runat="server" class="AveragesTableCenter" >
                                        <tr>
                                            <td style="width:175px;">
                                                <asp:TextBox ID="TextBoxLineLabelOutside" runat="server" Text="Average Entree" Width="200" BackColor="Orange" Enabled="false" Height="30"></asp:TextBox>
                                                <asp:DropDownList ID="DDLVeg1Outside" runat="server" AutoPostBack="true" BackColor="#e7eed6" Width="200" Height="30" OnSelectedIndexChanged="DDLFrtVeg_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Text="" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Vegetable Average" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="DDLVeg2Outside" runat="server" AutoPostBack="true" BackColor="#e7eed6" Width="200" Height="30" OnSelectedIndexChanged="DDLFrtVeg_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Text="" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Vegetable Average" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="DDLFrt1Outside" runat="server" AutoPostBack="true" BackColor="#fee7da" Width="200" Height="30" OnSelectedIndexChanged="DDLFrtVeg_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Text="" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Fruit Average" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="DDLFrt2Outside" runat="server" AutoPostBack="true" BackColor="#fee7da" Width="200" Height="30" OnSelectedIndexChanged="DDLFrtVeg_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Text="" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Fruit Average" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td style="text-align:center !important;width:40px;">
                                                <asp:TextBox ID="TextBoxAverageEntreeOutside" runat="server" Text="0.00" Width="42" Enabled="false" Height="30"></asp:TextBox>
                                                <asp:TextBox ID="TextBoxVeg1AvgOutside" runat="server" Text="0.00" Width="42" BackColor="#e7eed6" Enabled="false" Height="30"></asp:TextBox>
                                                <asp:TextBox ID="TextBoxVeg2AvgOutside" runat="server" Text="0.00" Width="42" BackColor="#e7eed6" Enabled="false" Height="30"></asp:TextBox>
                                                <asp:TextBox ID="TextBoxFrt1AvgOutside" runat="server" Text="0.00" Width="42" BackColor="#fee7da" Enabled="false" Height="30"></asp:TextBox>
                                                <asp:TextBox ID="TextBoxFrt2AvgOutside" runat="server" Text="0.00" Width="42" BackColor="#fee7da" Enabled="false" Height="30"></asp:TextBox>
                                            </td>
                                            <td style="width:50px;">
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </table>

                                    <asp:GridView ID="gvMenuDay" runat="server" AutoGenerateColumns="False"
                                        DataKeyNames="ProgramYear,DistrictID,SchoolNum,MenuCategory,DayNumber,ORD" DataSourceID="SqlDataSourceDayMenuEntry"
                                        ShowFooter="true" ShowHeader="true" ShowHeaderWhenEmpty="true" 
                                        OnRowCommand="gvMenuDay_RowCommand" OnRowDataBound="gvMenuDay_RowDataBound" 
                                        OnPreRender="gvMenuDay_PreRender" OnDataBinding="gvMenuDay_DataBinding" >
                                        <FooterStyle BorderStyle="None"  />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Qty" >
                                                <ItemTemplate>
                                                    <div style="text-align:center;">
                                                        <asp:Label ID="LabelQty" runat="server" Text='<%# Bind("Quantity", "{0:F0}") %>' Width="40"></asp:Label>
                                                    </div>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="border-top:2px solid black;"></div>
                                                    <asp:TextBox ID="TextBoxMenuQtyTotal" runat="server" Width="40" Enabled="false" BackColor="#e7eed6" Height="30" CssClass="MenuAnalysisCenterBold"></asp:TextBox>
                                                    <asp:Literal ID="Literal1" runat="server"><br /></asp:Literal>
                                                    <asp:TextBox ID="TextBoxFQty" runat="server" Width="40" Text="1" Height="30" Enabled="false" CssClass="MenuAnalysisCenterBold"></asp:TextBox>
                                                </FooterTemplate>                                    
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Menu Item" ItemStyle-Width="200" FooterStyle-Width="200" >
                                                <HeaderTemplate>
                                                    <asp:Label ID="LabelHdrMenuItem" runat="server" Text="Entree" Width="200"></asp:Label>
                                                </HeaderTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="DDLMenuItemsE" runat="server" DataSourceID="SqlDataSourceMenuItems" DataTextField="ItemDescription" DataValueField="ItemID" SelectedValue='<%# Bind("ItemID") %>' Width="200"></asp:DropDownList>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LabelItemDesc" runat="server" Text='<%# Eval("ItemDescription") %>' Width="200"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="border-top:2px solid black;"></div>
                                                    <asp:TextBox ID="TextBoxMenuAverageCost" runat="server" Enabled="false" BackColor="#dce6f1" Width="200" Height="30" CssClass="MenuAnalysisCenterBold" ></asp:TextBox>
                                                    <asp:Literal ID="Literal2" runat="server"><br /></asp:Literal>
                                                    <asp:DropDownList ID="DDLMenuItems" runat="server" DataSourceID="SqlDataSourceMenuItems" DataTextField="ItemDescription" DataValueField="ItemID" Width="200" Height="30"></asp:DropDownList>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cost" ItemStyle-Width="40" >
                                                <HeaderTemplate>
                                                    <asp:Label ID="LabelHdrCost" runat="server" Text="Cost" Width="40"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div style="text-align:center;width:40px;">
                                                        <asp:Label ID="LabelMenuLineCost" runat="server" Text='<%# Eval("ItemCost") %>' Width="40"></asp:Label>
                                                    </div>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="border-top:2px solid black;"></div>
                                                    <div style="width:40px;text-align:center;" >
                                                        <asp:TextBox ID="TBBar1" runat="server" BackColor="DarkGray" Enabled="false" Width="40" Height="30"></asp:TextBox>
                                                        <asp:Literal ID="Literal3" runat="server"><br /></asp:Literal>
                                                        <asp:LinkButton runat="server" CommandName="InsertItem" ID="FSave" ToolTip="Add" 
                                                            CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="InsertItem" Height="30" Width="30" />
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Total" ItemStyle-Width="50" >
                                                <ItemTemplate>
                                                    <div style="text-align:center;">
                                                        <asp:Label ID="LabelMenuLineTotal" runat="server" Text='<%# Eval("Total","{0:F2}") %>' Width="50" ></asp:Label>
                                                    </div>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="border-top:2px solid black;"></div>
                                                        <asp:TextBox ID="TextBoxMenuTotalCost" runat="server" BackColor="#ccffcc" Enabled="false" Width="50" Height="30" CssClass="MenuAnalysisCenterBold"></asp:TextBox>
                                                        <asp:Literal ID="Literal4" runat="server"><br /></asp:Literal>
                                                        <asp:CustomValidator ID="validDDLMenuItems" runat="server"  CssClass="validators" Text="*" ErrorMessage="Select an item" Display="Dynamic" ValidationGroup="InsertItem"></asp:CustomValidator>
                                                        <asp:Label ID="LabelBlank1" runat="server" Width="25" Height="25" Text=" "></asp:Label>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="60">
                                                <EditItemTemplate>
                                                    <div style="width:60px;text-align:center;" >
                                                        <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="EditItem" />
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
                                                             />
                                                    </div>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div style="border-top:2px solid black;"></div>
                                                        <asp:TextBox ID="TBBar2" runat="server" BackColor="DarkGray" Enabled="false" Width="60" Height="30"></asp:TextBox>
                                                        <asp:Literal ID="Literal5" runat="server"><br /></asp:Literal>
                                                        <asp:Label ID="LabelBlank2" runat="server" Width="60" Height="25" Text=" "></asp:Label>
                                                    </div>
                                                </FooterTemplate>

                                                <ItemStyle Width="60" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>

                                    <asp:TextBox ID="TextBoxMenuDayTotal" CssClass="MenuAnalysisTotal" runat="server" Text="0.00" Width="202" Enabled="false" Height="30"></asp:TextBox>

                                    <asp:HiddenField ID="HFTotalQty" runat="server" />
                                    <asp:HiddenField ID="HFTotalCost" runat="server" />
                                    <asp:HiddenField ID="HFAverageEntreeBrk" runat="server" />
                                    <asp:HiddenField ID="HFAverageFruitBrk" runat="server" />
                                    <asp:HiddenField ID="HFAverageEntreeLun" runat="server" />
                                    <asp:HiddenField ID="HFAverageVegLun" runat="server" />
                                    <asp:HiddenField ID="HFAverageFruitLun" runat="server" />

                                    <div id="DataSources">
                                        <asp:SqlDataSource ID="SqlDataSourceDayMenuEntry" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                                            SelectCommand="SELECT @Programyear [ProgramYear],@DistrictID [DistrictID],@SchoolNum [SchoolNum],@MenuCategory [MenuCategory],@DayNumber [DayNumber],0 [ORD],'' [ItemID],1 [ItemCategoryID],0 [Quantity],'' ItemDescription,0 ItemCost, 0 Total
                                            UNION ALL
                                            SELECT ENT.[ProgramYear],ENT.[DistrictID],ENT.[SchoolNum],ENT.[MenuCategory],ENT.[DayNumber],ENT.[ORD],ENT.[ItemID],ENT.[ItemCategoryID],ENT.[Quantity],SETP.[ItemDescription], SETP.ItemCost, (ENT.[Quantity]*SETP.ItemCost) Total
                                                FROM [MenuAnalysisEntry] ENT
                                                INNER JOIN [MenuCostItemsSetup] SETP ON ENT.ProgramYear=SETP.ProgramYear AND ENT.DistrictID=SETP.DistrictID AND ENT.ItemCategoryID=SETP.ItemCategoryID AND ENT.ItemID=SETP.ItemID
                                                WHERE ENT.ProgramYear=@Programyear AND ENT.DistrictID=@DistrictID AND ENT.SchoolNum=@SchoolNum AND ENT.MenuCategory=@MenuCategory AND ENT.DayNumber=@DayNumber"
                                            DeleteCommand="DELETE FROM [MenuAnalysisEntry] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND SchoolNum=@SchoolNum AND MenuCategory=@MenuCategory AND DayNumber=@DayNumber AND ORD=@ORD"
                                            InsertCommand="INSERT INTO [MenuAnalysisEntry] ([ProgramYear],[DistrictID],[SchoolNum],[MenuCategory],[DayNumber],[ORD],[ItemID],[ItemCategoryID],[Quantity]) 
                                            VALUES (@ProgramYear,@DistrictID,@SchoolNum,@MenuCategory,@DayNumber,
                                                (SELECT ISNULL(MAX(ORD), 99)+1 FROM [MenuAnalysisEntry] WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND SchoolNum=@SchoolNum AND MenuCategory=@MenuCategory AND DayNumber=@DayNumber AND [ORD]<300 ),
                                                @ItemID,@ItemCategoryID,@Quantity) "
                                            UpdateCommand="UPDATE [MenuAnalysisEntry] SET [Quantity]=@Quantity, [ItemID]=@ItemID WHERE [ProgramYear]=@ProgramYear AND [DistrictID]=@DistrictID AND SchoolNum=@SchoolNum AND MenuCategory=@MenuCategory AND DayNumber=@DayNumber AND ORD=@ORD"
                                            OnInserted="SqlDataSourceDayMenuEntry_Inserted" OnInserting="SqlDataSourceDayMenuEntry_Inserting"
                                            OnDeleted="SqlDataSourceDayMenuEntry_Deleted" OnUpdating="SqlDataSourceDayMenuEntry_Updating" OnUpdated="SqlDataSourceDayMenuEntry_Updated">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                                                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                                                <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int32" />
                                                <asp:ControlParameter ControlID="DDLCategories" Name="MenuCategory" PropertyName="SelectedValue" Type="Int32" />
                                                <asp:ControlParameter ControlID="LabelDayNumber" Name="DayNumber" PropertyName="Text" Type="Int32" />
                                            </SelectParameters>
                                            <DeleteParameters>
                                                <asp:Parameter Name="ProgramYear" Type="String" />
                                                <asp:Parameter Name="DistrictID" Type="String" />
                                                <asp:Parameter Name="SchoolNumber" Type="Int32" />
                                                <asp:Parameter Name="MenuCategory" Type="Int32" />
                                                <asp:Parameter Name="DayNumber" Type="Int32" />
                                            </DeleteParameters>
                                            <InsertParameters>
                                                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                                                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                                                <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int32" />
                                                <asp:ControlParameter ControlID="DDLCategories" Name="MenuCategory" PropertyName="SelectedValue" Type="Int32" />
                                                <asp:ControlParameter ControlID="LabelDayNumber" Name="DayNumber" PropertyName="Text" Type="Int32" />
                                                <asp:Parameter Name="ItemID" Type="Int32" />
                                                <asp:Parameter Name="Quantity" Type="Decimal" />
                                                <asp:ControlParameter ControlID="HFItemCategoryID" Name="ItemCategoryID" Type="Int32" PropertyName="Value" />
                                            </InsertParameters>
                                            <UpdateParameters>
                                                <asp:Parameter Name="ProgramYear" Type="String" />
                                                <asp:Parameter Name="DistrictID" Type="String" />
                                                <asp:Parameter Name="SchoolNum" Type="Int32" />
                                                <asp:Parameter Name="MenuCategory" Type="Int32" />
                                                <asp:Parameter Name="DayNumber" Type="Int32" />
                                                <asp:Parameter Name="ORD" Type="Int32" />
                                                <asp:Parameter Name="Quantity" Type="Decimal" DefaultValue="1" />
                                                <asp:Parameter Name="ItemID" Type="String" />
                                            </UpdateParameters>
                                        </asp:SqlDataSource>
                                        <asp:SqlDataSource ID="SqlDataSourceDayAverages" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                                            SelectCommand="SELECT * FROM vw_MenuAnalysisAverages WHERE ProgramYear=@Programyear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum AND DayNumber=@DayNumber">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                                                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                                                <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int32" />
                                                <asp:ControlParameter ControlID="LabelDayNumber" Name="DayNumber" PropertyName="Text" Type="Int32" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>

                                    </div>

                                </ContentTemplate>
                            </asp:UpdatePanel>

                          </td>
                      </ItemTemplate>
                    </asp:ListView>


                </div>
                <div id="divSummary" runat="server"  style="width:99%;">
                    <div class="row">
                        <div class="col">
                            <hr />
                            <asp:HiddenField ID="HFEntreeMidLower" runat="server" />
                            <asp:HiddenField ID="HFEntreeMidUpper" runat="server" />
                            <asp:HiddenField ID="HFTotalMealMidLower" runat="server" />
                            <asp:HiddenField ID="HFTotalMealMidUpper" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-3">
                            Number of Days: 
                            <asp:Label ID="LabelNumDays" runat="server"></asp:Label>
                        </div>
                        <div class="col-sm-3">
                            Average Food Cost Per Day:
                            <asp:Label ID="LabelAvgFoodCostPerDay" runat="server"></asp:Label>
                            <br />
                            Average Entree Cost:
                            <asp:Label ID="LabelAvgEntreeCost" runat="server"></asp:Label>
                        </div>
                        <div class="col-sm-3">
                            Total Food Cost:
                            <asp:Label ID="LabelTotalFoodCost" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        &nbsp;
                    </div>
                </div>
            </div>
            <div class="modal fade" id="myModal">  
                <div class="modal-dialog" >  
                    <div class="modal-content" style="width:800px;" > 
                        <div class="modal-header">  
                            <h4 class="modal-title">Copy Current Menu</h4>  
                            <button type="button" class="close" data-dismiss="modal">×</button>  
                        </div>  
                        <div class="modal-body">  
                            <div class="row">  
                                <div class="col-md-12">  
                                    <div class="form-group">  
                                        <label>Select day range and destination below.</label>  
                                        <div class="input-group">  
                                            <div>
                                                Starting Day:
                                                <asp:DropDownList ID="DDLStartingDay" runat="server" DataSourceID="SqlDataSourceDayNumbers" DataTextField="DayNumber" DataValueField="DayNumber" CausesValidation="false"></asp:DropDownList>
                                                <br />
                                            </div>  
                                            <div>
                                                Ending Day:
                                                <asp:DropDownList ID="DDLEndingDay" runat="server" DataSourceID="SqlDataSourceDayNumbers" DataTextField="DayNumber" DataValueField="DayNumber"></asp:DropDownList>
                                                <asp:CompareValidator ID="CompareDDLEnding" runat="server" ControlToCompare="DDLStartingDay" ControlToValidate="DDLEndingDay" CssClass="validators" Type="Integer"
                                                     Display="Dynamic" Text="Must be greater or equal to starting day" Operator="GreaterThanEqual" SetFocusOnError="true" ValidationGroup="CopyRange"></asp:CompareValidator>
                                                <br />
                                            </div>
                                            <div>
                                                Destination:
                                                <asp:DropDownList ID="DDLDestinationSchools" runat="server" DataSourceID="SqlDataSourceSchoolsAll" DataTextField="SchoolName" DataValueField="SchoolNum" ></asp:DropDownList>
                                            </div>
                                        </div>  
                                                 
                                    </div>  
                                </div>  
                            </div>  
                        </div>  
                        <div class="modal-footer">  
                            <asp:Button ID="btnCopy" runat="server" CssClass="btn btn-outline-success" Text="Copy" OnClick="btnCopy_Click" CausesValidation="true" ValidationGroup="CopyRange" />
                            <button type="button" class="btn btn-outline-danger" data-dismiss="modal">Close</button>  
                        </div>  
                    </div>  
                </div>  
            </div>  


       </ContentTemplate>
   </asp:UpdatePanel>

    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceDayNumbers" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                SelectCommand="select top 30 row_number() over(order by t1.number) as DayNumber from master..spt_values t1">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate FROM [ProgramYear] WHERE EndDate>='1/1/2024' ORDER BY [EndDate] DESC">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceSchools" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SELECTCOMMAND="SELECT DS.SchoolNum, DS.SchoolName, DS.SchoolType FROM DistrictSchools DS 
            INNER JOIN DistrictSchoolYearSetup DSYS on DS.DistrictID=DSYS.DistrictID AND DS.SchoolNum=DSYS.SchoolNum
            WHERE DS.DistrictID=@DistrictID AND ProgramYear=@ProgramYear AND INACTIVE=0 ORDER BY DS.SortOrder, DS.SchoolName">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceSchoolsAll" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" DataSourceMode="DataReader" 
            SELECTCOMMAND="SELECT -1 SchoolNum, '-All Schools-' SchoolName, -1 SortOrder
            UNION ALL
            SELECT DS.SchoolNum, DS.SchoolName, DS.SortOrder FROM DistrictSchools DS 
            INNER JOIN DistrictSchoolYearSetup DSYS on DS.DistrictID=DSYS.DistrictID AND DS.SchoolNum=DSYS.SchoolNum
            WHERE DS.DistrictID=@DistrictID AND ProgramYear=@ProgramYear AND INACTIVE=0 AND DS.SchoolNum<>@CurrentSchoolNum ORDER BY SortOrder, SchoolName">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListSchools" Name="CurrentSchoolNum" PropertyName="SelectedValue" Type="Int16" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceMenuItems" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
            SelectCommand="SELECT '0' ItemID, '- SELECT -' ItemDescription
            UNION ALL
            SELECT ItemID, ItemDescription FROM MenuCostItemsSetup WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND ItemCategoryID=@ItemCategoryID ORDER BY ItemDescription">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="HFItemCategoryID" Name="ItemCategoryID" PropertyName="Value" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceRanges" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" OnDataBinding="SqlDataSourceRanges_DataBinding"
                SelectCommand="SELECT MidrangeLower, MidrangeUpper FROM MenuCostRangeSetup WHERE ProgramYear=@ProgramYear AND ItemCategoryID=@ItemCategoryID">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:Parameter Name="ItemCategoryID" Type="Int16"  />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceRangesForOverall" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                SelectCommand="SELECT MidrangeLower, MidrangeUpper FROM MenuCostRangeSetup WHERE ProgramYear=@ProgramYear AND ItemCategoryID=10">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceAveragesOverall" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
            SelectCommand="SELECT MenuCategory, OverallAverage, NumDays, SumAvgCost FROM [vw_MenuAnalysisAveragesOverall] WHERE ProgramYear=@ProgramYear AND DistrictID=@DistrictID AND SchoolNum=@SchoolNum">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="DropDownListSchools" Name="SchoolNum" PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
