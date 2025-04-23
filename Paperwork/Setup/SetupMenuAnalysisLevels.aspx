<%@ Page Title="Setup Menu Analysis Levels" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SetupMenuAnalysisLevels.aspx.cs" Inherits="Paperwork.SetupMenuAnalysisLevels" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequest);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        var focusedElementId = "";


        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };

            if (focusedElementId != "") {
                $("#" + focusedElementId).focus();
            }
        }; 


        function BeginRequest(sender, args) {
            var fe = document.activeElement;
            if (fe != null) {
                focusedElementId = fe.id;
            } else {
                focusedElementId = "";
            }
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
    <div class="row">
        <div class="col">
            <h5><%: Title %></h5>
        </div>
    </div>
    <div class="row">
        <div class="col">
            Enter the ranges for the mid values below which determine the low / medium / high thresholds for menu items.<br />

        </div>
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>    
            <div id="DataEntry">

                <div class="messagealert" id="alert_container"></div>
                <div class="row">
                    <div class="col-2">
                        Program Year: 
                    </div>
                    <div class="col-10">
                        <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear" DataValueField="ProgramYear" 
                            AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged" ></asp:DropDownList>
                    </div>
                </div>
                <div class="row">
                    <div class="col">
                        <asp:GridView ID="gvMenuAnalysisLevels" runat="server" AutoGenerateColumns="false" CssClass="mGrid" DataSourceID="SqlDataSourceMenuAnalysisRanges" DataKeyNames="ProgramYear,ItemCategoryID"
                            AllowSorting="False">
                            <Columns>
                                <asp:BoundField DataField="ItemCategoryString" HeaderText="Category" SortExpression="ItemCategoryString" ReadOnly="true" HeaderStyle-Width="200" />
                                <asp:TemplateField HeaderText="Mid Range Lower">
                                    <ItemTemplate>
                                        <div style="text-align:center;">
                                            <asp:TextBox ID="TextBoxMidRangeLower" runat="server" Text='<%# Bind("MidRangeLower") %>' AutoCompleteType="Disabled" AutoPostBack="true" Width="100" OnTextChanged="TextBoxRange_TextChanged"></asp:TextBox>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Mid Range Upper">
                                    <ItemTemplate>
                                        <div style="text-align:center;">
                                            <asp:TextBox ID="TextBoxMidRangeUpper" runat="server" Text='<%# Bind("MidRangeUpper") %>' AutoCompleteType="Disabled" AutoPostBack="true" Width="100" OnTextChanged="TextBoxRange_TextChanged"></asp:TextBox>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Low/Med/High">
                                    <ItemTemplate>
                                        <div style="text-align:center;">
                                            <asp:Label ID="LabelLowRange" runat="server" Text='<%# "<" + Eval("MidRangeLower") %>' BackColor="LawnGreen"></asp:Label>
                                            /
                                            <asp:Label ID="LabelLowMid" runat="server" Text='<%# Eval("MidRangeLower") + " - " + Eval("MidRangeUpper") %>' BackColor="Yellow"></asp:Label>
                                            /
                                            <asp:Label ID="LabelLowHigh" runat="server" Text='<%# ">" + Eval("MidRangeUpper") %>' BackColor="Red"></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>


            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT [ProgramYear], BudgetYear FROM [ProgramYear] WHERE EndDate>='1/1/2024' ORDER BY [StartDate] DESC" >
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceMenuAnalysisRanges" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            SelectCommand="SELECT PY.ProgramYear, ItemCategorySeed.ItemCategoryID,
            CHOOSE(ItemCategorySeed.ItemCategoryID, 'Breakfast','Lunch','Vegetable','Fruit','','','','','','Total Meal') ItemCategoryString,
            ISNULL(MidrangeLower,0.00) MidrangeLower, ISNULL(MidrangeUpper,0.00) MidrangeUpper
              FROM ( SELECT * FROM
	              (SELECT 1 ItemCategoryID UNION ALL SELECT 2 ItemCategoryID UNION ALL SELECT 3 ItemCategoryID UNION ALL SELECT 4 ItemCategoryID UNION ALL SELECT 10 ItemCategoryID) ItemCategorySeed
              ) ItemCategorySeed 
            CROSS JOIN ProgramYear PY
            FULL OUTER JOIN [MenuCostRangeSetup] MCRS ON PY.ProgramYear=MCRS.ProgramYear AND MCRS.ItemCategoryID=ItemCategorySeed.ItemCategoryID
            WHERE PY.ProgramYear=@ProgramYear">
            <SelectParameters>
                <asp:ControlParameter ControlID="DDLProgramYears" Name="ProgramYear" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
