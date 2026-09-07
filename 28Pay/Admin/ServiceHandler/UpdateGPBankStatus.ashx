<%@ WebHandler Language="C#" Class="UpdateGPBankStatus" %>

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
public class UpdateGPBankStatus : IHttpHandler,IReadOnlySessionState
{

    public void ProcessRequest(HttpContext context)
    {
        //if(!AppUtils.CheckRolesPermission(Resources.Url.BankAccount))
        //{
        //    return;
        //}
        var Url = Resources.Url.GpayAccount;
        if (HttpContext.Current.Session["UserID"] == null)
            return;

        var UserID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
        if (UserID != 1)
        {
            var lst = new UsersRole().GetListByUser(UserID);
            if (!(lst != null && lst.Exists(e => e.Url.ToLower() == Url.ToLower())))
                return;
        }


        var data = new PostGetHelper().GetFromQueryString<InputData>();
        var id = int.Parse(data.id);
        var _Bank = new GPBank();
        _Bank.Id = id;
        _Bank = _Bank.Get();
        if (_Bank.Status == 1)
        {
            _Bank.Status = 0;
        }
        else
        {
            _Bank.Status = 1;
        }
        _Bank.UpdateStatus(_Bank.Id,_Bank.Status);




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