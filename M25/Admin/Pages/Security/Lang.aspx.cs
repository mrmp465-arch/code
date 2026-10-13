using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Utils; 

public partial class Pages_Security_Lang : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var lang = Request["lang"];
        GlobalHelper.SetLanguage(lang);
        Response.Redirect(Constant.ADMIN_PATH);
    }
}