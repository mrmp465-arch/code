<%@ WebHandler Language="C#" Class="UpdateBankStatus" %>


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

public class UpdateBankStatus : IHttpHandler, IReadOnlySessionState
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    public void ProcessRequest(HttpContext context)
    {
        //if(!AppUtils.CheckRolesPermission(Resources.Url.BankAccount))
        //{
        //    return;
        //}
        var Url = Resources.Url.BankAccount;
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
        var _Bank = new BankAccounts();
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
        if (_Bank.Solution == "APIV3")
            NotifyBank(_Bank.BankId, _Bank.BankCode, _Bank.Status);
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "bankupdate",
            ActionName = "Cập nhật bank",
            Description = "Cập nhật trạng thái bank " + _Bank.BankCode + " |" + _Bank.BankId + " |" + _Bank.Status.ToString()
        };
        _userLog.Add();

    }
    public void NotifyBank(string bankId, string bankCode, int status)
    {
        if (bankCode != "ACB")
            return;
        var UrlBaseService = "http://127.0.0.1:9003/BankService.ashx";
        var obj = new { BankId = bankId, BankCode = bankCode, Action = "start" };
        if (status != 1)
            obj = new { BankId = bankId, BankCode = bankCode, Action = "stop" };
        var requesData = new RequestData()
        {
            CommandCode = "NOTIFY_ACTION",
            RequestContent = serializer.Serialize(obj),
        };

        Task.Run(async () => await CallbackJson(
            UrlBaseService,
            serializer.Serialize(requesData)
        ));
    }
    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
    }
    public static async Task<string> CallbackJson(string url, string postData)
    {
        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        try
        {
            NLogLogger.Info(new string[] { "CMS", "UpdateMomoStatus", "Request", postData });
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "UpdateMomoStatus", "Response", responseContent });
                client.Dispose();
                return responseContent;
            }
            else
            {
                NLogLogger.Info(new string[] { "CMS", "UpdateMomoStatus", "Response Is Null" });
            }

        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
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