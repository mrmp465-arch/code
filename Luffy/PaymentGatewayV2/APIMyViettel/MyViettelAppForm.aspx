<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyViettelAppForm.aspx.cs" Inherits="APIMyViettel.MyViettelAppForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Serial<asp:TextBox ID="txtCardSerial" runat="server"></asp:TextBox>
            <asp:Button ID="btnOk" runat="server" OnClick="btnOk_Click" Text="Checking" />
            <br/>
            <br/>
            Code<asp:TextBox ID="txtCode" runat="server"></asp:TextBox>
            <br/>
            Mobile<asp:TextBox ID="txtMobie" runat="server"></asp:TextBox>
            <asp:Button ID="btnTopup" runat="server" Text="Recharge" OnClick="btnTopup_Click" />
        </div>
        <br/>
        <br/>
        <br/>
        SMAS
        <div>
            Serial<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" OnClick="btnOk_Click" Text="Checking" />
            <br/>
            <br/>
            Code<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
            <br/>
            Mobile<asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
            <asp:Button ID="Button2" runat="server" Text="Recharge" OnClick="btnTopup_Click" />
        </div>
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Button" />
    </form>
</body>
</html>
