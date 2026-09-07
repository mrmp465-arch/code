using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Message : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
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
            default:
                lblMessage.Text = HttpUtility.UrlDecode(Request["m"]);
                break;
        }
    }
}