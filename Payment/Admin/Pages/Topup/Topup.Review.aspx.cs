using Libs.API;
using Libs.Report;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Libs.Utils;

public partial class Pages_PayGate_Roles_Review : System.Web.UI.Page
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
            var _TopupMobile3rdLog = new TopupMobile3rdLog() { RequestNo = Convert.ToInt64(_TopupMobileLog.TransactionID) };
            _TopupMobile3rdLog.UpdateByReviewByRequestNo();

            _TopupMobileLog.Status = 1;
            _TopupMobileLog.Update();
        }

        Response.Redirect(url);
    }
}