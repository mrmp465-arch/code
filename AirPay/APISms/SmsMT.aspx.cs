using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Utils;

namespace APISms
{
    public partial class SmsMT : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var rt = "{\"status\":1,\"sms\":\"Ok PL.\",\"type\":\"text\"}";
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(rt);
            Response.End();
        }
    }
}