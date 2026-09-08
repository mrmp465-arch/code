<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/HomePage.Master" AutoEventWireup="true" CodeBehind="Message.aspx.cs" Inherits="BankGateV2.Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="step_block">
        <div style="clear: both"></div>
    </div>
    <div class="form_1">
        <ul class="col_box">
            <li class="col_box1">Thông báo: </li>
            <li class="col_box2"><asp:Label ID="lblMessage" runat="server" Text=""></asp:Label></li>
            <div style="clear: both"></div>
        </ul>
        <!--end col_box-->
       <%-- <p class="text_cpn">Công ty cổ phần XYP - 324 Cầu Giấy - Hà Nội</p>--%>
        <div style="clear: both"></div>
    </div>
</asp:Content>
