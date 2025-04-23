<%@ Page Title="Vendor Request Form" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VendorRequest.aspx.cs" Inherits="Paperwork.VendorRequest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="5" Visible="true">
        <ProgressTemplate>
            <div class="loadingDiv" >
                <div class="spinner-border text-info loadingImage"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <h2><%: Title %></h2>
    <p>
        Use this form to request a new vendor to be setup by the purchasing department.
    </p>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
                        <div class="row">
                <div class="col col-md-3">Vendor Name:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxVendorName" runat="server" MaxLength="65" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorName" runat="server" ControlToValidate="TextBoxVendorName" CssClass="validators" ErrorMessage="Vendor Name is required.">*</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">Vendor Contact:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxContact" runat="server" MaxLength="60" Width="200px"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">Address 1:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxAddress1" runat="server" MaxLength="60" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorAddress1" ControlToValidate="TextBoxAddress1" runat="server" CssClass="validators" ErrorMessage="Address 1 is required.">*</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">Address 2:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxAddress2" runat="server" MaxLength="60" Width="200px"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">City:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxCity" runat="server" MaxLength="35" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorCity" ControlToValidate="TextBoxCity" runat="server" CssClass="validators" ErrorMessage="City is required.">*</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">State:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxState" runat="server" MaxLength="2" Width="100px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorState" ControlToValidate="TextBoxState" runat="server" CssClass="validators" ErrorMessage="State is required.">*</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">Zip:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxZip" runat="server" MaxLength="10" Width="100px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorZip" ControlToValidate="TextBoxZip" runat="server" CssClass="validators" ErrorMessage="Zipcode is required.">*</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">Phone:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxPhone" runat="server" MaxLength="20" Width="150px"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">Fax:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxFax" runat="server" MaxLength="20" Width="150px"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">Additional Info:</div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxNotes" runat="server" TextMode="MultiLine" Rows="4" Width="100%"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">Your Phone or Email:<br /><asp:LoginName runat="server" /></div>
                <div class="col col-md-9">
                    <asp:TextBox ID="TextBoxUserInfo" runat="server" TextMode="MultiLine" Rows="2" Width="100%"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">&nbsp;</div>
                <div class="col col-md-9">
                    <asp:Button ID="ButtonSubmit" runat="server" Text="Submit" Font-Bold="True" Height="50px" OnClick="ButtonSubmit_Click" Width="200px" CausesValidation="true" />
                </div>
            </div>
            <div class="row">
                <div class="col col-md-3">&nbsp;</div>
                <div class="col col-md-9">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="validators" />
                </div>
            </div>
            <div id="customErrors">
                <asp:Label ID="LabelErrors" runat="server" Text="" CssClass="validators"></asp:Label>
            </div>
            <div id="datasources">
                <asp:SqlDataSource ID="SqlDataSourceEmailSettings" runat="server" ConnectionString="<%$ ConnectionStrings:connPaperwork %>" 
                    SelectCommand="SELECT * FROM [PaperworkWebEmailSettings] WHERE ([Index] = @Index)">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="Index" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
