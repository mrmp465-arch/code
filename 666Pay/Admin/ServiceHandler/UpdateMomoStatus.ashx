<%@ WebHandler Language="C#" Class="UpdateMomoStatus" Debug=true %>

using System;

using System.IO;
using System.Web;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
using System.Web.Script.Serialization;
using System.Web.SessionState;

public class UpdateMomoStatus : IHttpHandler,IReadOnlySessionState
{

    public void ProcessRequest(HttpContext context)
    {
        //if (!AppUtils.CheckRolesPermission(Resources.Url.BankAccount))
        //{
        //    return;
        //}
        var Url = Resources.Url.MomoAccount;
        if (HttpContext.Current.Session["UserID"] == null)
            return;

        var UserID = Convert.ToInt32( HttpContext.Current.Session["UserID"] );
        if (UserID != 1)
        {
            var lst = new UsersRole().GetListByUser(UserID);
            if (!(lst != null && lst.Exists(e => e.Url.ToLower() == Url.ToLower())))
                return ;
        }


        var data = new PostGetHelper().GetFromQueryString<InputData>();
        var id = int.Parse(data.id);
        var _Bank = new MomoAccounts();
        _Bank = _Bank.Get(id);
        if (_Bank.Status == 1)
        {
            _Bank.Status = 0;
        }
        else
        {
            _Bank.Status = 1;
        }
        _Bank.Update();
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "momoupdate",
            ActionName = "Cập nhật momo",
            Description = "Cập nhật trạng thái momo " + _Bank.MomoId + " |" + _Bank.Status.ToString()
        };
        _userLog.Add();

    }
    public class InputData
    {
        //public string BankCode { get; set; }
        public string id { get; set; }

    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}