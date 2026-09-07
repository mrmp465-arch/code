<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail2ACB.aspx.cs" Inherits="BankGateV2.Pages.Detail2ACB" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <meta charset="UTF-8">
<title>ACB</title>
<meta name="viewport" content="width=device-width, initial-scale=0.7" />

    <link rel="stylesheet" href="/bank/css/style.css?vs=03" />
</head>
<body>
   <div class="container-wrap" id="bill">
         <div class="sidebar">
             <div class="logo-box">
                 <a href="#"><img src="/bank/images/logo.png" alt="ACB"></a>
             </div>
             <div class="nav-box">
                 <ul class="nav-list">
                     <li><a href="#">Thông tin cá nhân </a></li>
                     <li class="active">
                         <a href="#">Quản lý tài khoản</a>
                         <ul class="sub-nav-list">
                             <li><a href="#" class="active">Thông tin tài khoản</a></li>
                             <li><a href="#">Truy vấn tài khoản</a></li>
                             <li><a href="#">Liệt kê giao dịch trực tuyến</a></li>
                             <li><a href="#">Hóa đơn điện tử</a></li>
                             <li><a href="#">Quản lý chi tiêu</a></li>
                         </ul>
                     </li>
                     <li><a href="#">Chuyển tiền</a></li>
                     <li><a href="#">Thanh toán</a></li>
                     <li><a href="#">Western Union</a></li>
                     <li><a href="#">Nộp thuế trực tuyến</a></li>
                     <li><a href="#">Tiền gửi</a></li>
                     <li><a href="#">Tín dụng</a></li>
                     <li><a href="#">Dịch vụ thẻ</a></li>
                 </ul>
             </div>

         </div>
         <div class="main-content">
             <div class="title-box">
                 <p class="title">Xin chào, <span><%=BankInfo.Mobile.Split('-')[2] %></span></p>
                 <p class="sub-title">Chi tiết giao dịch</p>
             </div>
             <div id="dvACB" class="acb-wrapper">
                 <div class="acb-header">Chi tiết giao dịch</div>
                 <div class="acb-section">
                     <div class="acb-grid">
                         <div class="label">Số</div>
                         <div class="value">&nbsp;</div>

                         <div class="label">Ngày lập</div>
                         <div class="value">03/06/2026</div>

                         <div class="label">Trạng thái</div>
                         <div class="value">GD đã hoàn tất</div>

                         <div class="label">Tên đơn vị trả tiền</div>
                         <div class="value"> <%=BankInfo.Mobile.Split('-')[2] %></div>

                         <div class="label">Tài khoản số</div>
                         <div class="value"> <%=BankInfo.Mobile.Split('-')[1] %> </div>

                         <div class="label">Tại ngân hàng</div>
                         <div class="value">ACB - PGD KIM DONG</div>
                     </div>
                 </div>

                 <div class="acb-section">
                     <div class="acb-grid">
                         <div class="label">Tên đơn vị nhận tiền</div>
                         <div class="value"><%= BankInfo.BankAccountName %></div>

                         <div class="label">Số CMND / Passport</div>
                         <div class="value">&nbsp;</div>

                         <div class="label">Ngày cấp</div>
                         <div class="value">&nbsp;</div>

                         <div class="label">Nơi cấp</div>
                         <div class="value">&nbsp;</div>

                         <div class="label">Tài khoản số</div>
                         <div class="value"><%= BankInfo.BankAccountNumber %></div>

                         <div class="label">Tại ngân hàng</div>
                         <div class="value"><%= BankInfo.BankCode %></div>
                     </div>
                 </div>

                 <div class="acb-section">
                     <div class="acb-grid">
                         <div class="label">Số tiền</div>
                         <div class="value"><%= Convert.ToInt64(BankInfo.Amount).ToString("#,#").Replace(".", ",") %></div>

                         <div class="label">Số tiền bằng chữ</div>
                         <div class="value"><%=DocTien( Convert.ToInt32(BankInfo.Amount))%></div>

                         <div class="label">Nội dung chuyển khoản</div>
                         <div class="value mono"><%= BankInfo.Note %>-<%= BankInfo.LastTime.ToString("ddMMyy-HH:mm:ss") %></div>
                     </div>
                 </div>
             </div>
         </div>
     </div>
     
</body>
</html>
