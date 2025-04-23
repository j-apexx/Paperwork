<%@ Page Title="Notifications" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="Paperwork.Notifications" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequest);
        function EndRequest(sender, args) {
            if (args.get_error() == undefined) {
                BindEvents();
            };
        };

        $(function () {
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

            $(function () {
                $("[id*=TexttBoxFNotificationDate]").datepicker({
                    changeMonth: true,
                    changeYear: true,
                    beforeShowDay: function (date) {
                        var isEomDate = isEom(date);
                        var show = true;
                        if (date.getDay() != 5 && !isEomDate) show = false;
                        return [show];
                    }
                });
            });

        }


        function isEom(date) {
            var dMonth = date.getMonth();
            var dYear = date.getFullYear();

            dMonth++;
            if (dMonth == 12) {
                dMonth = 0;
                dYear++;
            }

            var newDate = new Date(dYear, dMonth, 1);

            Date(newDate.setDate(0));//wierd but 1 would equal the first, 0 equals -1, -1 is actually subtracting 2 days.

            if (date.getFullYear() == newDate.getFullYear() && date.getMonth() == newDate.getMonth() && date.getDate() == newDate.getDate()) {
                return true;
            }
            else
                return false;
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="topSection">
                <div id="hdrDiv" style="float:left;width:30%;">
                    <h2><%: Title %></h2>
                </div>
                <div id="menuDiv" >
                    <div class="row">
                        <strong>Program Year:</strong>
                        <asp:Label ID="LabelProgramYear" runat="server"></asp:Label>
                    </div>
                    <div class="row">
                        <strong>District:</strong>
                        <asp:DropDownList ID="DropDownListDistricts" runat="server" AppendDataBoundItems="True" AutoPostBack="true" 
                            DataTextField="Desc" DataValueField="DistrictID" ValidationGroup="GridView"
                            OnSelectedIndexChanged="DropDownListDistricts_SelectedIndexChanged">
                        </asp:DropDownList> &nbsp;
                    </div>
                    <br />
                </div>
            </div>
            <div id="Records">
                <asp:GridView ID="GridViewNotifications" CssClass="mGrid" runat="server" DataSourceID="SqlDataSourceNotifications" AutoGenerateColumns="False"
                    DataKeyNames="ProgramYear,DistrictID,NotificationDate" ShowFooter="True" OnRowCommand="GridViewNotifications_RowCommand" ShowHeaderWhenEmpty="True" 
                        OnRowDataBound="GridViewNotifications_RowDataBound" AllowSorting="False"  >
                    <Columns>
                        <asp:TemplateField HeaderText="Date" SortExpression="NotificationDate" HeaderStyle-Width="100">
                            <EditItemTemplate>
                                <asp:Label ID="LabelENotificationDate" runat="server" Text='<%# Eval("NotificationDate", "{0:d}") %>' Width="100"></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelNotificationDate" runat="server" Text='<%# Eval("NotificationDate", "{0:d}") %>' Width="100"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="TexttBoxFNotificationDate" runat="server"  Width="100" OnTextChanged="TexttBoxFNotificationDate_TextChanged" AutoPostBack="true" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Transmittal" SortExpression="Transmittals" HeaderStyle-Width="150">
                            <EditItemTemplate>
                                <div style="padding-left:40%;">
                                    <asp:CheckBox ID="cbETrans" runat="server" Checked='<%# Bind("Transmittals") %>' OnCheckedChanged="cbTransNonTrans_CheckedChanged" AutoPostBack="true" />
                                </div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <div style="padding-left:40%;">
                                    <asp:CheckBox ID="cbTrans" runat="server" Checked='<%# Eval("Transmittals") %>' Enabled="false"  />
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="padding-left:40%;">
                                    <asp:CheckBox ID="cbFTrans" runat="server"  OnCheckedChanged="cbTransNonTrans_CheckedChanged" AutoPostBack="true" />
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="No Transmittal Due" SortExpression="NoTransmittals">
                            <EditItemTemplate>
                                <div style="padding-left:40%;">
                                    <asp:CheckBox ID="cbENoTrans" runat="server" Checked='<%# Bind("NoTransmittals") %>' OnCheckedChanged="cbTransNonTrans_CheckedChanged" AutoPostBack="true" />
                                </div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <div style="padding-left:40%;">
                                    <asp:CheckBox ID="cbNoTrans" runat="server" Checked='<%# Eval("NoTransmittals") %>' Enabled="false" />
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="padding-left:40%;">
                                    <asp:CheckBox ID="cbFNoTrans" runat="server" OnCheckedChanged="cbTransNonTrans_CheckedChanged" AutoPostBack="true"  />
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Recap" SortExpression="Recaps">
                            <EditItemTemplate>
                                <div style="padding-left:30%;">
                                    <asp:CheckBox ID="cbERecaps" runat="server" Checked='<%# Bind("Recaps") %>' />
                                </div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <div style="padding-left:30%;">
                                    <asp:CheckBox ID="cbRecaps" runat="server" Checked='<%# Eval("Recaps") %>' Enabled="false" />
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="padding-left:30%;">
                                    <asp:CheckBox ID="cbFRecaps" runat="server"  />
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Month End Closing" SortExpression="MonthEnd">
                            <EditItemTemplate>
                                <div style="padding-left:40%;">
                                    <asp:CheckBox ID="cbEMonthEnd" runat="server" Checked='<%# Bind("MonthEnd") %>'
                                        enabled='<%# Session["AccessLevel"].ToString().ToUpper()=="USER"?false:true %>'/>
                                </div>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <div style="padding-left:40%;">
                                    <asp:CheckBox ID="cbMonthEnd" runat="server" Checked='<%# Eval("MonthEnd") %>' Enabled="false" />
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="padding-left:40%;">
                                    <asp:CheckBox ID="cbFMonthEnd" runat="server"  
                                        enabled='<%# Session["AccessLevel"].ToString().ToUpper()=="USER"?false:true %>'/>
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Note" SortExpression="Note" HeaderStyle-Width="150">
                            <EditItemTemplate>
                                <div style="width:100px;text-align:center;">
                                    <asp:TextBox ID="TextBoxENote" runat="server" Text='<%# Bind("Note") %>' MaxLength="50" Width="150" />
                                </div>
                            </EditItemTemplate> 
                            <ItemTemplate>
                                <div style="width:100px;text-align:center;">
                                    <asp:Label ID="LabelNote" runat="server" Text='<%# Eval("Note") %>'  Width="150" />
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <div style="width:100px;text-align:center;">
                                    <asp:TextBox ID="TextBoxFNote" runat="server" MaxLength="50" Width="150" />
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Entry Date" SortExpression="EntryDate">
                            <EditItemTemplate>
                                <asp:Label ID="LabelEEntryDate" runat="server" Text='<%# Eval("EntryDate") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelEntryDate" runat="server" Text='<%# Eval("EntryDate") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                &nbsp;
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Change Date" SortExpression="ChangeDate">
                            <EditItemTemplate>
                                <asp:Label ID="LabelEChangeDate" runat="server" Text='<%# Eval("ChangeDate") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelChangeDate" runat="server" Text='<%# Eval("ChangeDate") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                &nbsp;
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                    <div style="width:60px;text-align:center;" <%# (bool)Eval("Locked")==true?"hidden='hidden'":"" %> >
                                        <asp:LinkButton runat="server" CommandName="Update" ID="ESave" ToolTip="Save" 
                                            CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
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
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="DataSources">
        <asp:SqlDataSource ID="SqlDataSourceNotifications" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
            OnInserting="SqlDataSourceNotifications_Inserting" OnInserted="SqlDataSourceNotifications_Inserted" OnDeleted="SqlDataSourceNotifications_Deleted"
            DeleteCommand="DELETE FROM [Notifications] WHERE [NotificationDate] = @NotificationDate AND [DistrictID] = @DistrictID AND [ProgramYear] = @ProgramYear" 
            InsertCommand="INSERT INTO [Notifications] ([Note], [MonthEnd], [Recaps], [NoTransmittals], [Transmittals], [NotificationDate], [DistrictID], [ProgramYear], 
            [Locked], [ChangeDate], [EntryDate]) VALUES (@Note, @MonthEnd, @Recaps, @NoTransmittals, @Transmittals, @NotificationDate, @DistrictID, @ProgramYear, 
            @Locked, GetDate(), GetDate())" 
            SelectCommand="SELECT [Note], [MonthEnd], [Recaps], [NoTransmittals], [Transmittals], [NotificationDate], [DistrictID], [ProgramYear], [Locked], [ChangeDate], [EntryDate] 
            FROM [Notifications] WHERE (([ProgramYear] = @ProgramYear) AND ([DistrictID] = @DistrictID)) 
            UNION ALL
            SELECT '' [Note], CAST(0 AS bit) [MonthEnd], CAST(0 AS bit) [Recaps],CAST(0 AS bit) [NoTransmittals], CAST(0 AS bit) [Transmittals], '1/1/1900' [NotificationDate], '' [DistrictID], 
            '' [ProgramYear], CAST(0 AS bit) [Locked], '1/1/1900' [ChangeDate], '1/1/1900' [EntryDate] " 
            UpdateCommand="UPDATE [Notifications] SET [Note] = @Note, [MonthEnd] = @MonthEnd, [Recaps] = @Recaps, [NoTransmittals] = @NoTransmittals, [Transmittals] = @Transmittals, 
             [ChangeDate] = GetDate() WHERE [NotificationDate] = @NotificationDate AND [DistrictID] = @DistrictID AND [ProgramYear] = @ProgramYear">
            <DeleteParameters>
                <asp:Parameter DbType="Date" Name="NotificationDate" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="ProgramYear" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
                <asp:Parameter DbType="Date" Name="NotificationDate" />
                <asp:Parameter Name="Note" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="MonthEnd" Type="Boolean" />
                <asp:Parameter Name="Recaps" Type="Boolean" />
                <asp:Parameter Name="NoTransmittals" Type="Boolean" />
                <asp:Parameter Name="Transmittals" Type="Boolean" />
                <asp:Parameter Name="Locked" Type="Boolean" DefaultValue="False" />
            </InsertParameters>
            <SelectParameters>
                <asp:SessionParameter Name="ProgramYear" SessionField="ProgramYear" Type="String" />
                <asp:ControlParameter ControlID="DropDownListDistricts" Name="DistrictID" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="Note" Type="String" ConvertEmptyStringToNull="false" DefaultValue="" />
                <asp:Parameter Name="MonthEnd" Type="Boolean" />
                <asp:Parameter Name="Recaps" Type="Boolean" />
                <asp:Parameter Name="NoTransmittals" Type="Boolean" />
                <asp:Parameter Name="Transmittals" Type="Boolean" />
                <asp:Parameter DbType="Date" Name="NotificationDate" />
                <asp:Parameter Name="DistrictID" Type="String" />
                <asp:Parameter Name="ProgramYear" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>

    </div>
</asp:Content>
