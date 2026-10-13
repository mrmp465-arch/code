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
      
        <div style="clear: both"></div>
    </div>
    <script>
        let socket = new WebSocket("wss://bankgate.coluber.xyz/ws");

        socket.onopen = function () {
            socket.send("Hello from client!");
            console.log("Connect Succcess ws://45.32.115.186:1586/ws");
        };

        socket.onmessage = function (event) {
            console.log("Received from server: " + event.data);
        };
    </script>
</asp:Content>
