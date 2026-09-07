<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NapBank.aspx.cs" Inherits="BankGateV2.NapBank" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <title>Nặp tiền</title>
    <script src="https://use.fontawesome.com/f56e4513c5.js"></script>
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
    <style>
        .select-wrapper:before {
            right: -157px !important;
            top: 8px !important;
        }
    </style>
    <link rel="stylesheet" href="css/style.css">

    <script
        src="https://code.jquery.com/jquery-3.7.1.min.js"
        integrity="sha256-/JqT3SQfawRcv/BIHPThkBvs0OEvtFFmqPF/lYI/Cxo="
        crossorigin="anonymous"></script>
    <!-- Latest compiled and minified CSS -->

    <!-- Optional theme -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" integrity="sha384-rHyoN1iRsVXV4nD0JutlnGaslCJuC7uwjduW9SVrLvRYooPp2bWYgmgJQIXwl/Sp" crossorigin="anonymous">

    <!-- Latest compiled and minified JavaScript -->
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>
    <script src="/js/bootbox.min.js"></script>

</head>
<body>
    <div id="dvnotify" style="display: none">
      
        <iframe width="858" height="900" src="<%=url %>" style="border:none" ></iframe>
    </div>
    <script type="text/javascript">

        function openWin(url) {
            var myWindow = window.open(url, '', 'width=700,height=700');
            //myWindow.focus();
        }
        function PopupManager(pageurl) {
            message = $('#dvnotify').html();
            
            bootbox.alert({
                backdrop: true,
                centerVertical: true,
                className: "dlnotify",
                message: message,
                size: 'large'
            });
        };
    </script>
    <div class="container" style="max-width: 1200px">
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
                <form class="form" id="form1" runat="server">
                    <asp:Panel runat="server" ID="pnSt1">

                        <asp:TextBox runat="server" ID="txtVND" placeholder="Nhâp số tiền cần nạp"></asp:TextBox>
                        <div class="edate" style="display:none">
                            <div class="select-wrapper">
                                <asp:DropDownList ID="ddlBanks" runat="server" CssClass="form-control">
                                    <asp:ListItem Selected="True" Value="N/A">Chọn ngân hàng</asp:ListItem>
                                     <asp:ListItem Value="TCB">TCB</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnSt2" Visible="False">
                        <asp:TextBox runat="server" ID="txtBankName" placeholder="Tên ngân hàng" ReadOnly="True" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtAccountNumber" placeholder="Số tài khoản" ReadOnly="True" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtAccountName" placeholder="Tên chủ tài khoản" ReadOnly="True" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtAmoutTranfer" placeholder="Số tiền cần chuyển" ReadOnly="True" required></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtReason" placeholder="Lý do" ReadOnly="True" required></asp:TextBox>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnSt3" Visible="False">
                        <iframe src="<%=url %>" width="700" height="500"></iframe>
                    </asp:Panel>
                    <div class="submit-wrapper">
                        <asp:Button runat="server" ID="CheckOut" class="submit" value="Buy Now" Text="LẤY THÔNG TIN" OnClick="CheckOut_Click"></asp:Button>
                        <asp:Button runat="server" ID="CheckTran" class="submit" value="Buy Now" Text="TRẠNG THÁI GIAO DỊCH" Visible="False" OnClick="CheckTran_Click"></asp:Button>
                    </div>
                    <div>
                        <asp:Label runat="server" ID="lblWarning" Visible="False">
                            
                        </asp:Label>
                    </div>

                </form>
            </div>
        </div>
    </div>

</body>

</html>
