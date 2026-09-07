<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BankTranfer.aspx.cs" Inherits="BankGateTest.BankTranfer" %>

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
    <div class="container" style="max-width:1200px">
        <div class="form-wrapper cf">
            <div class="four col">
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
            <div class="eight col">
                <form class="form" id="form1" runat="server"  >
                    <asp:Panel runat="server" ID="pnSt1">
                        <div class="select-wrapper">
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged" required >
                                <asp:ListItem  Selected="True" >Chọn loại Ngân hàng</asp:ListItem>
                                <asp:ListItem Value="banktranfer">Ngân hàng nội địa</asp:ListItem>
                                <asp:ListItem Value="wallet">Ví Momo</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <asp:TextBox runat="server" ID="txtVND" placeholder="Nhâp số tiền cần nạp" ></asp:TextBox>
                        <div class="edate" >
                            <div class="select-wrapper">
                                <asp:DropDownList ID="ddlBanks" runat="server" CssClass="form-control" required>
                                    <asp:ListItem Selected="True" Value="N/A">Chọn ngân hàng</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnSt2" Visible="False">
                        <asp:TextBox runat="server" ID="txtBankName" placeholder="Tên ngân hàng" ReadOnly="True" required ></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtAccountNumber" placeholder="Số tài khoản" ReadOnly="True" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtAccountName" placeholder="Tên chủ tài khoản" ReadOnly="True" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtAmoutTranfer" placeholder="Số tiền cần chuyển" ReadOnly="True" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtReason" placeholder="Lý do" ReadOnly="True" required></asp:TextBox>
                    </asp:Panel>
                     <asp:Panel runat="server" ID="pnSt3" Visible="False">
                         <iframe src="<%=url %>" width="700" height="500"> </iframe>
                          </asp:Panel>
                    <div class="submit-wrapper">
                        <asp:Button runat="server" ID="CheckOut" class="submit" value="Buy Now" Text="LẤY THÔNG TIN" OnClick="CheckOut_Click"></asp:Button>
                        <asp:Button runat="server" ID="CheckTran" class="submit" value="Buy Now" Text="TRẠNG THÁI GIAO DỊCH" Visible="False" OnClick="CheckTran_Click"></asp:Button>
                    </div>
                    <div>
                        <asp:Label runat="server" ID="lblWarning" Visible="False">
                            <p style="color:red;"><b>Lưu ý:</b> <i>Vui lòng chuyển khoản đúng nội dung phía trên. Thời gian thực hiện giao dịch là <b>10 phút</b></i></p>
                        </asp:Label>
                    </div>
                </form>
            </div>
        </div>
    </div>

</body>

</html>
