<%@ Page Title="District Vendor Selection" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupDistrictVendors.aspx.cs" Inherits="Paperwork.SetupDistrictVendors" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var h = $(window).height();
        var topAndBottom = 300;
        var timer,
            $win = $(window);

        //records the positions of the DIV scrollbars for postbacks
        function SetDivPosition() {
            var intY = document.getElementById("GridViewHeightController").scrollTop;
            document.cookie = "yPos=!~" + intY + "~!";

            var intY = document.getElementById("GridViewHeightController2").scrollTop;
            document.cookie = "yPos2=!|" + intY + "|!";
        }
        

        function pageLoad() {
            //resize the divs to fit the window
            $('#GridViewHeightController').css("height", h - topAndBottom );
            $('#GridViewHeightController').css("overflow", "auto")
            
            $('#GridViewHeightController2').css("height", h - topAndBottom);
            $('#GridViewHeightController2').css("overflow", "auto");

            //reset the scrollbar to where it belongs
            var strCook = document.cookie;
            if (strCook.indexOf("!~") != 0) {
                var intS = strCook.indexOf("!~");
                var intE = strCook.indexOf("~!");
                var strPos = strCook.substring(intS + 2, intE);
                document.getElementById("GridViewHeightController").scrollTop = strPos;
            }

            if (strCook.indexOf("!|") != 0) {
                var intS = strCook.indexOf("!|");
                var intE = strCook.indexOf("|!");
                var strPos = strCook.substring(intS + 2, intE);
                document.getElementById("GridViewHeightController2").scrollTop = strPos;
            }
        }

        //when window resizes
        $win.on('resize', function () {
            //update the height variable
            h = $(window).height();
            clearTimeout(timer);
            timer = setTimeout(function () {
                $("#GridViewHeightController").height(h - topAndBottom);
                $("#GridViewHeightController2").height(h - topAndBottom);
            }, 200);

        });
    </script>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
              <asp:Image CssClass="loadingImage" ID="ImageLoading" runat="server" ImageUrl="~/Images/loading.gif"  />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <h2><%: Title %></h2>
    <p>
        This page shows the selected vendors which will appear in the transmittal entry window.<br />
        Use the button below to request a vendor to be added to your list by the purchasing staff.<br />
        <div style="width:500px;">
            <a class="btn btn-info btn-sm btn-block" href="VendorRequest.aspx" >Request New Vendor &raquo;</a>
        </div>
    </p>
    <asp:Label ID="LabelVendorRequestStatus" runat="server" ForeColor="#009933"></asp:Label>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="col-md-7">
                    <h4>Available Vendors</h4>
                    <div>
                        Number of records to display:
                        <asp:DropDownList ID="DropDownListNumRecs1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownListNumRecs1_SelectedIndexChanged">
                            <asp:ListItem Selected="True">50</asp:ListItem>
                            <asp:ListItem>100</asp:ListItem>
                            <asp:ListItem>500</asp:ListItem>
                        </asp:DropDownList>

                        &nbsp;Search:<asp:TextBox ID="TextBoxVndSearch" runat="server"></asp:TextBox>
                        <asp:LinkButton ID="ISearch" runat="server" CausesValidation="False" 
                                    ToolTip="Search" Text="Search"
                                    CssClass="fas fa-search" />
                        &nbsp;
                        <asp:LinkButton ID="IClear" runat="server" CausesValidation="False" 
                                    ToolTip="Clear Search" forecolor="#333333" Text="Clear"
                                    CssClass="fas fa-undo" OnClick="IClear_Click" />

                    </div>
                    <div id="GridViewHeightController" onscroll="SetDivPosition()" >
                        <asp:GridView ID="GridViewAvailVendors" runat="server" AutoGenerateColumns="False" DataKeyNames="VENDORID" 
                            DataSourceID="SqlDataSourceAvailableVendors" CssClass="mGrid" OnSelectedIndexChanged="GridViewAvailVendors_SelectedIndexChanged" 
                            EmptyDataText="No vendors are available." PagerStyle-CssClass="mGridPager"
                            AllowSorting="true" AllowPaging="true" PageSize='<%# int.Parse( DropDownListNumRecs1.SelectedValue) %>'  >
                            <Columns>
                                <asp:BoundField DataField="VENDORID" HeaderText="Vendor ID" ReadOnly="True" SortExpression="VENDORID"  />
                                <asp:BoundField DataField="VENDNAME" HeaderText="Name" SortExpression="VENDNAME" />
                                <asp:BoundField DataField="CITY" HeaderText="City" SortExpression="CITY" />
                                <asp:BoundField DataField="STATE" HeaderText="State" SortExpression="STATE" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="IAdd" CssClass="fas fa-arrow-right mGridLinkIconButton" runat="server" CommandName="Select" ToolTip="Add" ></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <br />
                    </div>
                </div>
                <div class="col-md-5">
                    <h4>Selected Vendors</h4>
                    <div>&nbsp;</div>
                    <div id="GridViewHeightController2" onscroll="SetDivPosition()">
                        <asp:GridView ID="GridViewSelectedVendors" runat="server" AutoGenerateColumns="False" CssClass="mGrid" DataSourceID="SqlDataSourceSelectedVendors"
                                DataKeyNames="VENDORID" Width="100%"
                                EmptyDataText="No vendors selected.<br/>Select a Vendor from the available vendors list."
                                AllowSorting="true" AllowPaging="true" PageSize="500" >
                            <Columns>
                                <asp:BoundField DataField="VENDORID" HeaderText="Vendor ID" SortExpression="VENDORID" />
                                <asp:BoundField DataField="VENDNAME" HeaderText="Name" SortExpression="VENDNAME" />
                                <asp:CommandField ShowDeleteButton="True" ButtonType="Button" ControlStyle-Width="75" ControlStyle-Height="25" ControlStyle-Font-Bold="true" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    
                    <input type="hidden" id="div_position" name="div_position" value="50" />
                    <asp:HiddenField ID="scroll2_pos" runat="server" Value="75" />
                </div>
            </div>
            <div class="row">
                <div class="col col-md-5 col-md-offset-1" style="padding-top:5px;">
                    
                </div>
            </div>

            <div id="DataSources">
                <asp:SqlDataSource ID="SqlDataSourceAvailableVendors" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT RTRIM([VENDORID]) VENDORID, RTRIM([VENDNAME]) VENDNAME, RTRIM([CITY]) CITY, RTRIM([STATE]) 'STATE' FROM [Vendors] 
                        WHERE [VENDORID] NOT IN (SELECT [VENDORID] FROM [DistrictVendors] WHERE [DistrictID]=@DistrictID) AND VENDSTTS=1 "
                    FilterExpression="[VENDORID] LIKE '%{0}%' OR [VENDNAME] LIKE '%{0}%' ">
                    <FilterParameters>
                        <asp:ControlParameter ControlID="TextBoxVndSearch" Name="SearchVendor" PropertyName="Text" Type="String" />
                    </FilterParameters>
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSourceSelectedVendors" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT [DistrictVendors].[VENDORID], [VENDORS].[VENDNAME] FROM [DistrictVendors] 
                        JOIN [VENDORS] ON [DistrictVendors].[VENDORID]=[VENDORS].[VENDORID]
                        WHERE [DistrictVendors].[DistrictID]=@DistrictID"
                    DeleteCommand="DELETE FROM [DistrictVendors] WHERE [DistrictID]=@DistrictID AND [VENDORID]=@VENDORID" OnDeleted="SqlDataSourceSelectedVendors_Deleted">
                    <SelectParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                        <asp:Parameter Name="VENDORID" Type="String" />
                    </DeleteParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
