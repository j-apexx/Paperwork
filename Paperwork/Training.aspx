<%@ Page Title="Training" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Training.aspx.cs" Inherits="Paperwork.Training" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <contenttemplate>
        <div class="container">
            <div id="topSection" class="row">

                <asp:HiddenField ID="HFTrainingID" runat="server" Value='<%# Eval("TrainingID") %>' />
                <asp:HiddenField ID="HFAttendeeID" runat="server" Value='<%# Eval("HFAttendeeID") %>' />
                <asp:HiddenField ID="HFAttendeeName" runat="server" Value='<%# Eval("AttendeeName") %>' />
                <div id="hdrgroup" class="col-sm-12">
                    <div id="hdrDiv" style="float: left; width: 40%">
                        <h2><%: Title %></h2>
                    </div>
                    <div id="menuDiv">
                        <div class="row">
                            <div class="col-sm-12">
                                <strong>Program Year:</strong>
                                <asp:DropDownList ID="DDLProgramYears" runat="server" DataSourceID="SqlDataSourceProgramYears" DataTextField="ProgramYear"
                                    DataValueField="ProgramYear" AutoPostBack="true" OnSelectedIndexChanged="DDLProgramYears_SelectedIndexChanged">
                                </asp:DropDownList>
                                <%--   <asp:HiddenField ID="HFStartDate" runat="server" />
                                <asp:HiddenField ID="HFEndDate" runat="server"  />--%>
                            </div>
                            <div class="messagealert" id="alert_container"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="dataentry">
            <div id="accordian">
                <div class="card">
                    <div class="card-header h5">
                        <a class="card-link" data-toggle="collapse" href="#cardCollapse1">Select/Create Training
                        </a>
                    </div>

                    <div class="card-body">
                        <asp:GridView ID="GridViewTrainings" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,TrainingID"
                            DataSourceID="SqlDataSourceTrainHdrsList" CssClass="mGrid" AllowPaging="true" PageSize="25" PagerSettings-Mode="NumericFirstLast"
                            PagerStyle-HorizontalAlign="Center" PagerStyle-Font-Size="Large" PagerSettings-PageButtonCount="10" PagerStyle-Width="100"
                            PagerStyle-CssClass="mGridPager" EnableViewState="true"
                            OnPageIndexChanged="GridViewTrainings_PageIndexChanged"
                            OnSelectedIndexChanged="GridViewTrainings_SelectedIndexChanged"
                            OnDataBound="GridViewTrainings_DataBound"
                            OnRowDataBound="GridviewClickable_RowDatabound"
                            ShowHeaderWhenEmpty="true"
                            AllowSorting="true">
                            <Columns>
                                <asp:TemplateField HeaderText="Training ID" SortExpression="TrainingID">
                                    <ItemTemplate>
                                        <asp:Label ID="LabelTrainingID" runat="server" Text='<%# Eval("TrainingID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="TrainingTitle" SortExpression="TrainingTitle">
                                    <HeaderTemplate>
                                        <p>Training Title</p>
                                        <asp:TextBox ID="TextBoxTrainingTitle" runat="server"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("TrainingTitle") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Training Date" SortExpression="TrainingDate">
                                    <HeaderTemplate>
                                        <p>Training Date</p>
                                        <asp:TextBox  ID="TextBoxTrainingDate" runat="server" TextMode="Date"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# DateTime.Parse(Eval("TrainingDate").ToString()).Date.ToString("MM/dd/yyyy") %>' DataFormatString="{0:d}"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField SortExpression="TrainingTime" HeaderText="Start Time">
                                    <HeaderTemplate>
                                        <p>Training Time</p>
                                        <asp:TextBox ID="TextBoxTrainingTime" runat="server" TextMode="Time"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("TrainingTime") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Training Description" SortExpression="TrainingDescription">
                                    <HeaderTemplate>
                                        <p>Training Description</p>
                                        <asp:TextBox ID="TextBoxTrainingDescription" runat="server"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("TrainingDescription") %>' DataFormatString="{0:d}"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField SortExpression="TrainerName" HeaderText="Trainier Name">
                                    <HeaderTemplate>
                                        <p>Trainer Name</p>
                                        <asp:TextBox ID="TextBoxTrainerName" runat="server"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("TrainerName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div style="width: 60px;">
                                            <asp:LinkButton ID="LinkButtonCreateTraining" runat="server"
                                                ToolTip="Create New Training" ForeColor="Green"
                                                CssClass="fa fa-plus mGridLinkButton" OnClick="ICreateClick" Width="25" />
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div style="width: 60px; text-align: center;">
                                            <asp:LinkButton runat="server"
                                                ID="IDelete" ToolTip="Delete" CommandName="Delete" CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                />

                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                </div>

                <div class="card">
                    <div class="card-header h5">
                        <a class="card-link" data-toggle="collapse" href="#cardCollapse2">Training Details
                        </a>
                    </div>

                    <!-- Bottom Section -->
                    <%--     <div id="cardCollapse2" class="collapse show">--%>

                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanelTrainings" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col-sm-12 " style="padding-top: 5px; border-style: solid; border-width: 1px; border-color: dimgray;">
                                        <asp:FormView ID="FormViewSpFnOrg" Visible="true" runat="server" CssClass="mGridRecaps" DefaultMode="Edit" DataSourceID="SqlDataSourceTrainHdr" DataKeyNames="ProgramYear,DistrictID,TrainingID"
                                            Width="100%">
                                            <EditItemTemplate>
                                                <div class="container">
                                                    <div class="row">
                                                        <div class="col-sm-5">
                                                            <asp:TextBox ID="TxtBxTrainingID" runat="server" Text='<%# Bind("TrainingID") %>' Width="150" MaxLength="15"></asp:TextBox>&nbsp;Training ID.
                                                           
                                                        </div>
                                                        <%-- <div class="col-sm-2" style="text-align: center; color: red;">
                                                                    <asp:Label ID="LabelLocked" runat="server" Text="**Locked**" />
                                                                </div>--%>
                                                        <div style="float: right; text-align: right; position: absolute; right: 20px;">
                                                            <%-- <asp:Image ID="ImageLogo" runat="server" ImageUrl="~/Images/InvoiceLogoSM.png" ImageAlign="Right" />--%>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-sm-12" style="text-align: center;">
                                                            <h5>Training</h5>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="padding-top: 5px; padding-bottom: 5px;">
                                                        <div class="col-sm-5">
                                                            <asp:Label ID="LabelOrgBilled" runat="server" Text="TRAINING TITLE:" Width="200"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-7" style="text-align: right;">
                                                            <asp:HiddenField ID="HFDistID" runat="server" Value='<%# Eval("TrainingTitle") %>' />
                                                            <asp:TextBox ID="TrainingTitle" Value='<%# Eval("TrainingTitle") %>' runat="server" Width="100%" CssClass="longTextBox autosuggest" MaxLength="50" AutoCompleteType="Search" AutoPostBack="true"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="padding-top: 5px; padding-bottom: 5px;">
                                                        <div class="col-sm-5">
                                                            <asp:Label ID="Label1" runat="server" Text="TRAINING DESCRIPTION:" Width="200"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-7" style="text-align: right;">
                                                            <asp:TextBox ID="TrainingDescription" Value='<%# Eval("TrainingDescription") %>' runat="server" Width="100%" CssClass="longTextBox" MaxLength="50" AutoCompleteType="Disabled"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="row" style="padding-top: 5px; padding-bottom: 5px;">
                                                        <div class="col-sm-5">
                                                            <asp:Label ID="Label2" runat="server" Text="TRAINING DATE:" Width="200"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-7" style="text-align: right;">
                                                            <asp:TextBox ID="TrainingDate" Value='<%# DateTime.Parse(Eval("TrainingDate").ToString()).Date.ToString("yyyy-MM-dd") %>' runat="server" Width="100%" CssClass="longTextBox" MaxLength="50" AutoCompleteType="Disabled" TextMode="Date"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="padding-top: 5px; padding-bottom: 5px;">
                                                        <div class="col-sm-5">
                                                            <asp:Label ID="Label5" runat="server" Text="TRAINING TIME:" Width="200"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-7" style="text-align: right;">
                                                            <asp:HiddenField ID="HiddenField3" runat="server" Value='<%# Eval("TrainingTime") %>' />
                                                            <asp:TextBox ID="TrainingTime" Value='<%# Eval("TrainingTime") %>' runat="server" Width="100%" CssClass="longTextBox autosuggest" MaxLength="50" AutoCompleteType="Search" AutoPostBack="true" TextMode="Time"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="padding-top: 5px; padding-bottom: 5px;">
                                                        <div class="col-sm-5">
                                                            <asp:Label ID="Label3" runat="server" Text="TRAINIER NAME:" Width="200"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-7" style="text-align: right;">
                                                            <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("TrainerName") %>' />
                                                            <asp:TextBox ID="TrainerName" runat="server" Value='<%# Eval("TrainerName") %>' Width="100%" CssClass="longTextBox autosuggest" MaxLength="50" AutoCompleteType="Search" AutoPostBack="true"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="padding-top: 5px; padding-bottom: 5px;">
                                                        <div class="col-sm-5">
                                                            <asp:Label ID="Label4" runat="server" Text="LOCATION:" Width="200"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-7" style="text-align: right;">
                                                            <asp:HiddenField ID="HiddenField2" runat="server" Value='<%# Eval("Location") %>' />
                                                            <asp:TextBox ID="Location" Value='<%# Eval("Location") %>' runat="server" Width="100%" CssClass="longTextBox autosuggest" MaxLength="50" AutoCompleteType="Search" AutoPostBack="true"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="padding-top: 5px; padding-bottom: 5px;">
                                                        <div class="col-sm-5"></div>
                                                        <div class="col-sm-7" style="text-align: right;">
                                                            <asp:LinkButton ID="ISave" runat="server"
                                                                ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green" OnClick="ISave_Click"
                                                                CssClass="fas fa-save mGridLinkButton" Width="150"></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                    <br />

                                                </div>
                                            </EditItemTemplate>
                                        </asp:FormView>
                                        <div>
                                            <asp:GridView ID="GridViewTrainingAttendees" runat="server" AutoGenerateColumns="False" DataKeyNames="AttendeeID,AttendeeName"
                                                DataSourceID="SqlDataSourceTrainAttendeesList" CssClass="mGrid" EnableViewState="true"
                                                OnRowDataBound="GridviewTrainingAttendeesClickable_RowDatabound"
                                                ShowHeaderWhenEmpty="true"
                                                ShowFooter="true" AllowSorting="true"
                                                OnRowCommand="GridViewAttendees_RowCommand">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Attendee ID" SortExpression="AttendeeID" HeaderStyle-Width="100">
                                                        <ItemTemplate>
                                                            <asp:Label ID="LabelAttendeeID" runat="server" Text='<%# Eval("AttendeeID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Attendee Name" SortExpression="AttendeeName">
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="TextBoxEAttendeeName" Text='<%# Bind("AttendeeName") %>' runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" Text='<%# Eval("AttendeeName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="TextBoxAttendeeName" runat="server"></asp:TextBox>
                                                        </FooterTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderStyle-Width="80">
                                                        <EditItemTemplate>
                                                            <div>
                                                                <asp:LinkButton runat="server" CommandName="Update" ID="ESaveAttendee" ToolTip="Save" 
                                                                    CssClass="fas fa-save mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Edit" />
                                                                <asp:LinkButton runat="server" CommandName="Cancel" ID="ECancel" ToolTip="Cancel" 
                                                                    CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                                            </div>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <div>
                                                                <asp:LinkButton runat="server" CommandName="Edit" ID="IEditAttendee" ToolTip="Edit" 
                                                                    CssClass="fas fa-pencil-alt mGridLinkIconButton" ForeColor="#336699"  />
                                                                <asp:LinkButton runat="server" CommandName="Delete" ID="IDeleteAttendee" ToolTip="Delete"
                                                                    CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                                                    OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                                                            </div>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            <div>
                                                                <asp:LinkButton runat="server" CommandName="Insert" ID="FSaveAttendee" ToolTip="Add" 
                                                                    CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                                                <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancelAttendee" ToolTip="Cancel" 
                                                                    CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                                            </div>
                                                        </FooterTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <table cellspacing="0" style="border-collapse: collapse;width: 100%;">
                                                        <tr>
                                                            <td style="width: 100px">
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TextBoxAttendeeNameInEmpty" Text='<%# Bind("AttendeeName") %>' runat="server" Width="100%"></asp:TextBox>
                                                            </td>
                                                            <td style="width: 80px">
                                                                <div style="width:60px;text-align:center;" >
                                                                    <asp:LinkButton runat="server" CommandName="Insert" ID="FSaveAttendee" ToolTip="Add" 
                                                                        CssClass="fas fa-plus mGridLinkIconButton" ForeColor="Green" CausesValidation="true" ValidationGroup="Insert" />
                                                                    <asp:LinkButton runat="server" CommandName="Cancel" ID="FCancelAttendee" ToolTip="Cancel" 
                                                                        CssClass="fas fa-undo-alt mGridLinkIconButton" ForeColor="Red" CausesValidation="false" />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                                </div>

                    <div id="datasources">
                        <asp:SqlDataSource ID="SqlDataSourceProgramYears" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                            SelectCommand="SELECT RTRIM(ProgramYear) ProgramYear, StartDate, EndDate, BudgetYear FROM [ProgramYear] ORDER BY [EndDate] DESC"></asp:SqlDataSource>

                        <asp:SqlDataSource ID="SqlDataSourceTrainHdrsList" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                            SelectCommand="SELECT DistrictID, ProgramYear, TrainingID, TrainingTitle, TrainingDescription, 
                   TrainingDate, TrainingTime, TrainerName, Location
                   FROM PAPERWORKWEB.[dbo].TrainingHDR ORDER BY TrainingTitle DESC"
                            DeleteCommand="DELETE FROM TrainingHDR WHERE ((ProgramYear = @ProgramYear) AND (DistrictID = @DistrictID) AND (TrainingID = @TrainingID))"
                            OnDeleted="SqlDataSourceTrainHdrsListDeleted">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>

                        <asp:SqlDataSource ID="SqlDataSourceTrainAttendeesList" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                            SelectCommand="SELECT AttendeeID, AttendeeName 
                                FROM PAPERWORKWEB.[dbo].TrainingAttendees WHERE TrainingID = @TrainingID AND ProgramYear = @ProgramYear ORDER BY AttendeeID ASC"
                            DeleteCommand="DELETE FROM [dbo].TrainingAttendees WHERE ((ProgramYear = @ProgramYear) AND (DistrictID = @DistrictID) AND (AttendeeID = @AttendeeID))"
                            InsertCommand="INSERT INTO [dbo].TrainingAttendees ([DistrictID],[ProgramYear],[TrainingID],[AttendeeName])
                                VALUES (@DistrictID, @ProgramYear, @TrainingID, @AttendeeName)"
                            UpdateCommand="UPDATE [dbo].TrainingAttendees SET [AttendeeName] = @AttendeeName WHERE [AttendeeID] = @AttendeeID"
                            OnDeleted="SqlDataSourceTrainAttendeesListDeleted"
                            OnInserting="SqlDataSourceAttendees_Inserting"
                            OnUpdating="SqlDataSourceAttendees_Updating">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                                <asp:ControlParameter ControlID="HFTrainingID" Name="TrainingID" PropertyName="Value" Type="String" />
                            </SelectParameters>
                            <DeleteParameters>
                                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                                <asp:ControlParameter ControlID="HFAttendeeID" Name="AttendeeID" PropertyName="Value" Type="String" />
                            </DeleteParameters>
                            <InsertParameters>
                                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                                <asp:ControlParameter ControlID="HFTrainingID" Name="TrainingID" PropertyName="Value" Type="String" />
                                <asp:ControlParameter ControlID="HFAttendeeName" Name="AttendeeName" PropertyName="Value" Type="String" />
                            </InsertParameters>
                            <UpdateParameters>
                                <asp:ControlParameter ControlID="HFAttendeeID" Name="AttendeeID" PropertyName="Value" Type="String" />
                                <asp:ControlParameter ControlID="HFAttendeeName" Name="AttendeeName" PropertyName="Value" Type="String" />
                            </UpdateParameters>
                        </asp:SqlDataSource>

                        <asp:SqlDataSource ID="SqlDataSourceTrainHdr" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>"
                            SelectCommand="SELECT DistrictID, ProgramYear, TrainingID, TrainingTitle, TrainingDescription, 
                                        TrainingDate, TrainingTime, TrainerName, Location
                                        FROM PAPERWORKWEB.[dbo].TrainingHDR 
                            WHERE ((ProgramYear = @ProgramYear) AND (DistrictID = @DistrictID) AND (TrainingID = @TrainingID))"
                            DeleteCommand="DELETE FROM TrainingHDR WHERE ((ProgramYear = @ProgramYear) AND (DistrictID = @DistrictID) AND (TrainingID = @TrainingID))">
                            <DeleteParameters>
                                <asp:Parameter Name="ProgramYear" Type="String" />
                                <asp:Parameter Name="DistrictID" Type="String" />
                                <asp:Parameter Name="TrainingID" Type="String" />
                            </DeleteParameters>

                            <SelectParameters>
                                <asp:ControlParameter ControlID="DDLProgramYears" PropertyName="SelectedValue" Name="ProgramYear" Type="String" />
                                <asp:SessionParameter Name="DistrictID" SessionField="CurrentDistrict" Type="String" />
                                <asp:ControlParameter Name="TrainingID" ControlID="HFTrainingID" PropertyName="Value" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>

                    </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <%-- </div>--%>
                </div>
            </div>
        </div>
    </contenttemplate>


</asp:Content>
