using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Utils;

public partial class Pages_NganLuong_NganLuong_Cancel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        NLogLogger.Info(new string[] { "BankGate", "NganLuong", "Response", "Cancel", Request.Url.ToString() });
        Response.Redirect(Constant.HOME_ROOT + Resources.Url.Message + "?m=" + HttpUtility.UrlEncode("Giao dịch đã bị hủy!"));

    }
}