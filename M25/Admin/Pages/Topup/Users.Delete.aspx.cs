using Libs.API;
using System;

public partial class Pages_Topup_User_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupUsersDelete);
        var _User = new Users();
        _User = _User.Get(Convert.ToInt32(AppUtils.Request("id")));
        if (_User != null && _User.UserID != AppUtils.UserID)
        {
            _User.Delete();
            var _UserPartner = new UserPartner().GetListByUser(Convert.ToInt32(AppUtils.Request("id")));
            if (_UserPartner != null)
                foreach (var item in _UserPartner)
                {
                    item.Delete();
                }
        }
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupUsersList);
    }
}