using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.BankDirect;
using Libs.Report;
using Libs.Utils;
namespace BankGateV2
{
    public partial class Message : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //ReportSN();
            //ReportSN();
           var mess= " <b>Tài khoản</b>: (ACB) 21639371 -  PHAN QUYET THANG %0A<b>Mã giao dịch</b>: 890751-100925-7527 %0A<b>Số tiền</b>: %2B2.000 %0A<b>Thời gian</b>: 09/10/2025 09:09:51 %0A<b>Nội dung</b>: 123 FT25282517926030 GD 890751-100925 09:09:50";
            TelegramNotify.SendTeleV4("-4838040115", mess);
            switch (Request["error"])
            {
                case "-1":
                    lblMessage.Text = "Giao dịch không hợp lệ!";
                    break;
                case "-2":
                    lblMessage.Text = "Đã hết phiên làm việc!";
                    break;
                case "-3":
                    lblMessage.Text = "Giao dịch đã thực hiện!";
                    break;
                case "-4":
                    lblMessage.Text = "Hệ thống chưa được mở!";
                    break;
                case "-5":
                    lblMessage.Text = "Hệ thống chưa hỗ trợ loại thanh toán này!";
                    break;
                default:
                    lblMessage.Text = HttpUtility.UrlDecode(Request["m"]);
                    break;
            }
        }
        
    }
}