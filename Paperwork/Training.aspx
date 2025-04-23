<%@ Page Title="Training" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Training.aspx.cs" Inherits="Paperwork.Training" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <contenttemplate>
        <div class="container">
            <div id="topSection" class="row">

                <asp:HiddenField ID="HFTrainingID" runat="server" Value='<%# Eval("TrainingID") %>' />
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
                        <div style="top: -10px; position: relative; text-align: center;">
                            <asp:LinkButton ID="LinkButtonNewInvoice" runat="server"
                                ToolTip="Create New Training" ForeColor="Green"
                                CssClass=" mGridLinkButton" Width="200" OnClick="ICreateClick">
                                        <i class="fas fa-asterisk"></i> Create New Training
                            </asp:LinkButton>
                            <%-- <asp:LinkButton ID="LinkButtonPrintInvoices" Width="150" runat="server" 
                                        class="fas fa-print mGridLinkButton" 
                                        ToolTip="Opens Print Range Dialog" Text=" Print Trainings" OnClick="IPrintRangeOpen_Click" ></asp:LinkButton>--%>
                        </div>
                        <asp:GridView ID="GridViewTrainings" runat="server" AutoGenerateColumns="False" DataKeyNames="ProgramYear,DistrictID,TrainingID"
                            DataSourceID="SqlDataSourceTrainHdrsList" CssClass="mGrid" AllowPaging="true" PageSize="25" PagerSettings-Mode="NumericFirstLast"
                            PagerStyle-HorizontalAlign="Center" PagerStyle-Font-Size="Large" PagerSettings-PageButtonCount="10" PagerStyle-Width="100"
                            PagerStyle-CssClass="mGridPager" EnableViewState="true"
                            OnPageIndexChanged="GridViewTrainings_PageIndexChanged"
                            OnSelectedIndexChanged="GridViewTrainings_SelectedIndexChanged"
                            OnDataBound="GridViewTrainings_DataBound"
                            OnRowDataBound="GridviewClickable_RowDatabound"
                            AllowSorting="true">
                            <Columns>
                                <asp:BoundField DataField="TrainingID" HeaderText="Training ID" SortExpression="TrainingID" ReadOnly="True" HeaderStyle-Width="150" />
                                <asp:BoundField DataField="TrainingTitle" HeaderText="TrainingTitle" SortExpression="TrainingTitle" />
                                <asp:BoundField DataField="TrainingDate" HeaderText="Training Date" SortExpression="TrainingDate" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="TrainingTime" SortExpression="TrainingTime" HeaderText="Start Time" />
                                <asp:BoundField DataField="TrainingDescription" HeaderText="Training Description" SortExpression="TrainingDescription" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="TrainerName" SortExpression="TrainerName" HeaderText="Trainier Name" />

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%--    <asp:HiddenField ID="HFInvoiceRowLocked" runat="server" Value='<%# Eval("Locked") %>' />--%>
                                        <div style="width: 60px; text-align: center; font-size: xx-small; font-weight: bold;">
                                            <%--                                                 <asp:Label runat="server" ID="LabelImported" Text="*LOCKED*" Visible='<%# (bool)Eval("Locked")==false ? false : true %>' ></asp:Label>--%>
                                        </div>
                                        <div style="width: 60px; text-align: center;">
                                            <asp:LinkButton runat="server"
                                                ID="IDelete" ToolTip="Delete" CommandName="Delete" CssClass="fa fa-trash-alt mGridLinkIconButton" ForeColor="Crimson"
                                              OnClientClick="return confirm('Are you sure you want to delete this Traing?');"
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

                                <div class="row" style="text-align: center; position: relative; top: -10px;">
                                    <div class="col-sm-2">
                                        <div class="messagealert" id="alert_container_InvoiceHdr"></div>
                                    </div>
                                    <div class="col-sm-10">


                                        <asp:LinkButton ID="ISave" runat="server"
                                            ToolTip="Save Changes" Text=" Save Changes" ForeColor="Green" OnClick="ISave_Click"
                                            CssClass="fas fa-save mGridLinkButton" Width="150"></asp:LinkButton>


                                    </div>

                                </div>
                                <div class="row">
                                    <div class="col-sm-10 " style="padding-top: 5px; border-style: solid; border-width: 1px; border-color: dimgray;">
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
                                                            <asp:TextBox ID="TrainingDate" Value='<%# Eval("TrainingDate") %>' runat="server" Width="100%" CssClass="longTextBox" MaxLength="50" AutoCompleteType="Disabled"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row" style="padding-top: 5px; padding-bottom: 5px;">
                                                        <div class="col-sm-5">
                                                            <asp:Label ID="Label5" runat="server" Text="TRAINING TIME:" Width="200"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-7" style="text-align: right;">
                                                            <asp:HiddenField ID="HiddenField3" runat="server" Value='<%# Eval("TrainingTime") %>' />
                                                            <asp:TextBox ID="TrainingTime" Value='<%# Eval("TrainingTime") %>' runat="server" Width="100%" CssClass="longTextBox autosuggest" MaxLength="50" AutoCompleteType="Search" AutoPostBack="true"></asp:TextBox>
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
                                                    <br />
                                                    <br />
                                                    <div class="row" style="padding-top: 5px; border-style: solid; border-width: 2px 1px 1px 1px; border-color: darkslategrey; text-align: center;">
                                                        <div class="col-sm-12">
                                                            ATTENDEES
                                                        </div>
                                                    </div>
                                                    <div class="row" style="padding-bottom: 5px; border-style: solid; border-width: 1px 1px 2px 1px; border-color: darkslategrey;">
                                                        <div class="col-sm-12" style="padding-top: 5px;">
                                                        </div>
                                                    </div>

                                                </div>
                                            </EditItemTemplate>
                                        </asp:FormView>
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
