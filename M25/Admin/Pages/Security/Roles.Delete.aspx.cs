using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_PayGate_Roles_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        AppUtils.CheckRoles(Resources.Url.RolesDelete);
        var _Roles = new Roles() { Id = Convert.ToInt32(AppUtils.Request("id")) };
        _Roles = _Roles.Get();
        if (_Roles != null)
        {
            _Roles.Delete(); 
            //var _Roleservice = new Roleservice().GetList(_Roles.RoleId, 0);
            //if (_Roleservice != null)
            //    foreach (var item in _Roleservice)
            //    {
            //        item.Delete();
            //    }
        } 
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.RolesList);
    }
}