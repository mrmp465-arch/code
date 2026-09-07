using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_PayGate_Provider_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.ProvidersDelete);
        var _Provider = new Providers();
        _Provider = _Provider.Get(Convert.ToInt32(AppUtils.Request("id")));
        if (_Provider != null) 
            _Provider.Delete();  
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderList+"?Type="+ _Provider.Type);

    }
}