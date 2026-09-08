using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_PayGate_Products_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.ProductDelete);
        var _Product = new Products();
        _Product = _Product.Get(Convert.ToInt32(AppUtils.Request("id")));
        if (_Product != null) 
            _Product.Delete();  
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductList+"?Type="+ _Product.Type);

    }
}