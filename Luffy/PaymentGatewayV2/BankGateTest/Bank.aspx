<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bank.aspx.cs" Inherits="BankGateTest.Bank" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label4" runat="server" Text="Full Name:"></asp:Label>
            <asp:TextBox ID="txtFullName" runat="server">Mr Bill</asp:TextBox>
            <br />
            <br />
            <asp:Label ID="Label5" runat="server" Text="Mobile:"></asp:Label>
            <asp:TextBox ID="txtMobile" runat="server">0922238765</asp:TextBox>
            <br />
            <br />
            <asp:Label ID="Label1" runat="server" Text="OrderNo:"></asp:Label>
            <asp:TextBox ID="txtOrderNo" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="Label2" runat="server" Text="OrderInfo:"></asp:Label>
            <asp:TextBox ID="txtOrderInfo" runat="server">UnitTest</asp:TextBox>
            <br />
            <br />
            <asp:Label ID="Label3" runat="server" Text="Amout:"></asp:Label>
            <asp:TextBox ID="txtAmount" runat="server">1</asp:TextBox>

            <br />
            <br />
            <asp:Button ID="btnPay" runat="server" OnClick="btnPay_Click" Text="Thanh Toán" />

        </div>
    </form>
</body>
</html>
