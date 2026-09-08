using Libs.API;
using Libs.Report;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Libs.Utils;

public partial class Pages_PayGate_Roles_Delete : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string url = Request["u"];
        url = url == null ? Constant.ADMIN_PATH + "" : url;
        AppUtils.CheckRoles(Resources.Url.TopupDelete);
        var _TopupMobileLog = new TopupMobileLog() { TransactionID = AppUtils.Request("id") };
        _TopupMobileLog = _TopupMobileLog.Get();

        if (_TopupMobileLog != null)
        {
            if (AppUtils.IsAdmin)
                _TopupMobileLog.Delete();
            else
            {
                if (_TopupMobileLog.AmountTopupSuccess != 0)
                {
                    NLogLogger.Info(new string[] { "Topup.Delete", AppUtils.UserName, _TopupMobileLog.TransactionID.ToString() });
                    Message.AlertAndRedirect(this.Page, "Order đã được xử lý không thể xóa!", url);
                }
                else
                {
                    var lst = new Users().GetList();
                    if (lst != null)
                    {
                        lst = lst.Where(x => x.UserID == AppUtils.UserID || x.ParentId == AppUtils.UserID).ToList();
                        if (lst != null & lst.Any(x => x.UserID == _TopupMobileLog.UserId))
                            _TopupMobileLog.Delete();
                    }
                }

            }
        }

        Response.Redirect(url);
    }
}