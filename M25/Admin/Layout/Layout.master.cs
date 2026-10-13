using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Layout_Layout : System.Web.UI.MasterPage
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
            if (host.Contains("tn99.info"))
            {
                Page.Title = "TN99CMS";
            }
            else
            {
                Page.Title = "FastPayCMS";
            }
            
        }
        AppUtils.CheckLogin();
    }
}
