<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Card.aspx.cs" Inherits="BankGateTest.Card" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <title>Nặp tiền</title>
    <script src="https://use.fontawesome.com/f56e4513c5.js"></script>
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
    <link rel="stylesheet" href="css/style.css">
</head>
<body>
<div class="container">
    <div class="form-wrapper cf">
        <div class="five col">
            <div class="title">

                <h2>Nạp thẻ Telco</h2>
                <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/1/18/IPhone_7_Jet_Black.svg/200px-IPhone_7_Jet_Black.svg.png" alt="iPhone">
                <p class="item">VTT, VMS, VNP</p>
                <asp:panel visible="True" id="pnInfo" runat="server">
                <p class="price">10k - 1M</p>
                </asp:panel>
                <asp:panel visible="False" id="pnError" runat="server">
                    <asp:Label class="price" runat="server" ID="lbError"></asp:Label>
                </asp:panel>
            </div>
        </div>
        <div class="seven col">
            <form class="form" id="form1" runat="server">
                <asp:textbox runat="server" id="txtAccountName" placeholder="Tên tài khoản" required>MrX</asp:textbox>
                <div class="edate">
                    <div class="select-wrapper">
                        <asp:dropdownlist id="txtTelco" runat="server" cssclass="form-control" required>
                            <asp:ListItem Value="">Nhà mạng:</asp:ListItem>
                            <asp:ListItem Value="vms" Selected="True">Mobifone</asp:ListItem>
                            <asp:ListItem Value="vnp" >Vinaphone</asp:ListItem>
                            <asp:ListItem Value="viettel" >Viettel</asp:ListItem>
                             <asp:ListItem Value="zing" >Zing</asp:ListItem>
                            <asp:ListItem Value="gate" >Gate</asp:ListItem>
                        </asp:dropdownlist>
                    </div>
                </div>
                <asp:textbox runat="server" id="txtSerial" placeholder="Serial" required></asp:textbox>
                <asp:textbox runat="server" id="txtPin" placeholder="Pin" required></asp:textbox>
                <div class="edate">
                    <div class="select-wrapper">
                        <asp:dropdownlist id="txtAmout" runat="server" cssclass="form-control" required>
                            <asp:ListItem Value="0">Mệnh giá thẻ</asp:ListItem>
                            <asp:ListItem Selected="True" Value="10000">10.000</asp:ListItem>
                            <asp:ListItem Value="20000">20.000</asp:ListItem>
                            <asp:ListItem Value="50000">50.000</asp:ListItem>
                            <asp:ListItem Value="100000">100.000</asp:ListItem>
                            <asp:ListItem Value="200000">200.000</asp:ListItem>
                            <asp:ListItem Value="300000">300.000</asp:ListItem>
                            <asp:ListItem Value="500000">500.000</asp:ListItem>
                            <asp:ListItem Value="1000000">1.000.000</asp:ListItem>
                        </asp:dropdownlist>
                    </div>
                </div>
                <div class="submit-wrapper">
                    <asp:button runat="server" id="CheckOut" class="submit" value="Buy Now" text="Nạp Thẻ" onclick="CheckOut_Click"></asp:button>

                </div>
            </form>
        </div>
    </div>
</div>

</body>

</html>
