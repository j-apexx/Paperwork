<%@ Page Title="Unauthorized" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Unauthorized.aspx.cs" Inherits="Paperwork.Unauthorized" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Unauthorized</h1>
    <h3>Contact IT support to request access to the Paperwork Web application. You will need to provide your windows ID listed below, and the district to which you need access.</h3>
    <h4><%: Context.User.Identity.Name  %></h4>
</asp:Content>