using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Security_User_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.UsersDelete);
        var _User = new Users();
        _User = _User.Get(Convert.ToInt32(AppUtils.Request("id")));
        if (_User != null)
        {
            _User.Delete();
            //var _UserPartner = new UserPartner().GetListByUser(AppUtils.Request("id"));
            //if (_UserPartner != null)
            //    foreach (var item in _UserPartner)
            //    {
            //        item.Delete();
            //    }
        }
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersList);
    }
}