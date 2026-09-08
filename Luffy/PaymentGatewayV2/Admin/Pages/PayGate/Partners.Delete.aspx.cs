using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_PayGate_Partner_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        AppUtils.CheckRoles(Resources.Url.PartnersDelete);
        var _Partner = new Partners();
        _Partner = _Partner.Get(Convert.ToInt32(AppUtils.Request("id")));
        if (_Partner != null)
        {
            _Partner.Delete(_Partner.PartnerID); 
            //var _PartnerService = new PartnerService().GetList(_Partner.PartnerID, 0);
            //if (_PartnerService != null)
            //    foreach (var item in _PartnerService)
            //    {
            //        item.Delete();
            //    }
        } 
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnersList);
    }
}