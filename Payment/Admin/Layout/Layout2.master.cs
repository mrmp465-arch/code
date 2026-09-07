using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Layout_Layout2 : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var temp = Constant.ADMIN_PATH;
        string host = Request.Url.Host.ToLower();
        if (host.Contains("qxpay.info"))
        {
            Page.Title = "QXPayCMS";
        }
        else
        {
            Page.Title = "FastPayCMS";
        }
        AppUtils.CheckLogin();
    }
}
