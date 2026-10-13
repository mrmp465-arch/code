<%@ WebHandler Language="C#" Class="GetBankBalance" %>

using System;
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

public class GetBankBalance : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //context.Response.ContentType = "text/plain";
        //context.Response.Write("Hello World");
        var data = new PostGetHelper().GetFromQueryString<InputData>();
        var result = GetBalanceBank(data.BankCode, data.BankId);
        context.Response.Write(result);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
    private string GetBalanceBank(string bankCode, string bankId)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var UrlBaseService = "http://127.0.0.1:9002/BankService.ashx";
        var requesData = new RequestData()
        {
            CommandCode = "BALANCE_GET",
            RequestContent = string.Format("{0},{1}", bankCode, bankId)
        };

        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null && resObj.ResponseCode == (int)ResponseCode.TransactionSuccessful)
        {
            return Convert.ToInt64(resObj.ResponseContent).ToString("N0");
        }
        return "-1";
    }
    public static async Task<string> CallbackJson(string url, string postData)
    {
        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(300);
        try
        {
            NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Request", postData });
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Response", responseContent });
                client.Dispose();
                return responseContent;
            }
            else
            {
                NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Response Is Null" });
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
    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
    }
    public class InputData
    {
        public string BankCode { get; set; }
        public string BankId { get; set; }

    }
}