<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Callback.aspx.cs" Inherits="BankGateTest.CallbackUrl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:DropDownList ID="ddl" runat="server">
                <asp:ListItem Value="1">Đã giao hàng</asp:ListItem>
                <asp:ListItem Value="0">Giao thât bại</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnConfirm" runat="server" OnClick="btnConfirm_Click" Text="Xác nhận Giao Hàng" />
        </div>
    </form>
</body>
</html>
