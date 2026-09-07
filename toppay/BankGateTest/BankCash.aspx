<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BankCash.aspx.cs" Inherits="BankGateTest.BankCash" %>

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

                    <h2>Nạp Tiền</h2>
                    <img src="/Images/banking.PNG" alt="iPhone">
                    <p class="item">Bank Tranfer</p>
                    <asp:Panel Visible="True" ID="pnInfo" runat="server">
                        <p class="price">VD: 1 Xu = 1.000 VNĐ</p>
                        <p class="price"><b>Lưu ý:</b> <i>Chuyển khoản trong cùng một hệ thống ngân hàng hoặc chuyển khoản nhanh</i></p>
                    </asp:Panel>
                    <asp:Panel Visible="False" ID="pnError" runat="server">
                        <asp:Label class="price" runat="server" ID="lbError"></asp:Label>
                    </asp:Panel>
                </div>
            </div>
            <div class="seven col">
                <form class="form" id="form1" runat="server">
                    <asp:Panel runat="server" ID="pnSt1" Visible="false">

                        <asp:TextBox runat="server" ID="txtVND" placeholder="Nhâp số tiền cần nạp"></asp:TextBox>
                        <div class="edate">
                            <div class="select-wrapper">
                                <asp:DropDownList ID="ddlBanks" runat="server" CssClass="form-control" required>
                                    <asp:ListItem Selected="True" Value="N/A">Chọn ngân hàng</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnSt2" Visible="true">
                        <div class="select-wrapper">
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control" AutoPostBack="false" required>
                                <asp:ListItem Value="N/A">Chọn loại ngần hàng</asp:ListItem>
                                <asp:ListItem Value="bankcash">Ngân hàng nội địa</asp:ListItem>
                                <asp:ListItem Value="momocash"  Selected="True">Ví điện tử MoMo</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <asp:TextBox runat="server" ID="txtBankName" placeholder="Tên ngân hàng" value="momo" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtAccountNumber" placeholder="Số tài khoản" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtAccountName" placeholder="Tên chủ tài khoản" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtAmoutTranfer" placeholder="Số tiền cần chuyển" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtReason" placeholder="Lý do" required></asp:TextBox>
                    </asp:Panel>
                    <div class="submit-wrapper">
                        <asp:Button runat="server" ID="CheckOut" class="submit" value="Buy Now" Text="LẤY THÔNG TIN" OnClick="CheckOut_Click" Visible="False"></asp:Button>
                        <asp:Button runat="server" ID="CheckTran" class="submit" value="Buy Now" Text="Xác nhận" OnClick="CheckTran_Click"></asp:Button>
                    </div>
                    <div>
                        <asp:Label runat="server" ID="lblWarning" Visible="False">
                            <p style="color:red;"><b>Lưu ý:</b> <i>Vui lòng chuyển khoản đúng nội dung phía trên. Thời gian thực hiện giao dịch là <b>120 phút</b></i></p>
                        </asp:Label>
                    </div>
                </form>
            </div>
        </div>
    </div>

</body>

</html>
