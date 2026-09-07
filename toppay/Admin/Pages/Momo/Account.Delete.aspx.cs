using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class Pages_Momo_Account_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoAccountDelete);
        var _Momo = new MomoAccounts();
        _Momo = _Momo.Get(Convert.ToInt32(AppUtils.Request("id")));
       
        if (_Momo != null)
        {
            if (!AppUtils.UserName.Contains("admin"))
            {
                if (_Momo.Source != AppUtils.UserName)
                    Response.Redirect(Resources.Url.MomoAccount);
            }
            _Momo.Delete();
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "momodelete",
                ActionName = "Xóa momo",
                Description = "Xóa momo " + _Momo.MomoId
            };
            _userLog.Add();
        }    
            
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount );

    }
}