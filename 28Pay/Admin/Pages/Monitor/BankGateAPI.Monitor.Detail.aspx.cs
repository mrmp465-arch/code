using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using Telegram.Bot.Types.Payments;

public partial class Pages_Monitor_BankGateAPI_Monitor_Detail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIMonitorDetail);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        Libs.Report.BankGateAPI _BankGateAPI = new Libs.Report.BankGateAPI();
        _BankGateAPI.TransactionID = Convert.ToInt64(Request["id"]);

        _BankGateAPI = _BankGateAPI.Get();

        lblTransactionID.Text = _BankGateAPI.TransactionID.ToString();
        lblAccountName.Text = _BankGateAPI.FullName;

        lblAmount.Text = Convert.ToInt64(_BankGateAPI.Amount).ToString("N0").Replace(",", ".");
        lblTotalAmount.Text = Convert.ToInt64(_BankGateAPI.TotalAmount).ToString("N0").Replace(",", ".");

        lblProvider.Text = _BankGateAPI.ProviderCode;
        lblCreatTime.Text = _BankGateAPI.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss");
        lblLastTime.Text = _BankGateAPI.LastTime.ToString("dd/MM/yyyy HH:mm:ss");
        lblRefCode.Text = _BankGateAPI.RefCode;
        lblBankCode.Text = _BankGateAPI.BankCode;
        lblOrderNo.Text = _BankGateAPI.OrderNo;
        lblOrderInfo.Text = _BankGateAPI.OrderInfo;
        lblMobile.Text = _BankGateAPI.Mobile;
        lblEmail.Text = _BankGateAPI.Email;
        lblBankAccountName.Text = _BankGateAPI.BankAccountName;
        lblBankAccountNumber.Text = _BankGateAPI.BankAccountNumber;
        lblCallbackUrl.Text = _BankGateAPI.ReturnUrl;

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
        {
            ResponseContent = serializer.Serialize(new DataCallback()
            {
                RefCode = _BankGateAPI.RefCode,
                OrderNo = _BankGateAPI.OrderNo,
                Amount = Convert.ToInt32(_BankGateAPI.Amount),
            })
        };
        var partner = new Partners().Get(_BankGateAPI.PartnerCode);
        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
        // Task.Run(async () => await M32VTPBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse)).ConfigureAwait(false));
        //lblCallbackdata.Text = serializer.Serialize(apiResponse);
        lblStatus.Text = _BankGateAPI.Status + " (" + ResponseUtils.Description(_BankGateAPI.Status) + ")";

        txtLog.Text = _BankGateAPI.LogContent;

        //log callback
        var lstLogData = LogCache.GetLogBank(_BankGateAPI.TransactionID);
        rptList.DataSource = lstLogData;
        rptList.DataBind();
        //if (_BankGateAPI.Status >= 1)
        //    lnCallback.Visible = true;

        if (!AppUtils.IsAdmin)
        {
            //if (_BankGateAPI.PartnerCode != AppUtils.UserName)
            //{
            //    Response.Redirect(Constant.ADMIN_PATH + "500.html");
            //}
        }
    }
    protected void txtRecheck_Click(object sender, EventArgs e)
    {

        //var handler = BankGateV2Factory.GetHandler(lblProvider.Text);
        //var result = handler.ReCheck(lblTransactionID.Text);
        //lblRecheck.Text = result.Description;

    }
    public class DataCallback
    {
        public string RefCode { get; set; }
        public string OrderNo { get; set; }
        public int Amount { get; set; }
        //public string Mobile { get; set; }
        public string OrderInfo { get; set; }
        public string Type { get; set; }


        public int ResponseCode { get; set; }

        public string Description { get; set; }

        public string Signature { get; set; }

    }
    public string DecodeFromUtf8(string str)
    {
        // copy the string as UTF-8 bytes.
        return Regex.Replace(str, @"\\u([0-9a-fA-F]{4})", match =>
        {
            var unicodeValue = Convert.ToInt32(match.Groups[1].Value, 16);
            var unicodeChar = char.ConvertFromUtf32(unicodeValue);
            return unicodeChar;
        });
    }

    protected void lnCallback_Click(object sender, EventArgs e)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        Libs.Report.BankGateAPI _CardAPILog = new Libs.Report.BankGateAPI();
        //_CardAPILog.TransactionID = Convert.ToInt64(Request["id"]);

        _CardAPILog = _CardAPILog.Get(Convert.ToInt64(Request["id"]));
        var partner = new Partners().Get(_CardAPILog.PartnerCode);
        var privateKey = partner.PrivateKey;
        var url = _CardAPILog.ReturnUrl;
        if (string.IsNullOrEmpty(url))
            url = partner.SMSPlusUrl;
        if (!string.IsNullOrEmpty(url))
        {

            var datacb = new DataCallback()
            {
                //Mobile = _CardAPILog.Mobile,
                RefCode = _CardAPILog.RefCode,
                OrderNo = _CardAPILog.FullName,
                OrderInfo = _CardAPILog.OrderInfo,
                Amount = Convert.ToInt32(_CardAPILog.TotalAmount),
                Type = "bank"
            };
            if (_CardAPILog.BankCode == "MOMO")
                datacb.Type = "momo";

            var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {

            };
            datacb.ResponseCode = apiResponse.ResponseCode;
            datacb.Description = apiResponse.Description;
            if (_CardAPILog.TotalAmount <= 0)
                return;
            datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode + datacb.Amount, partner.PrivateKey, partner.SignatureType);

            Task.Run(() => CallbackJson(url, serializer.Serialize(datacb), _CardAPILog.TransactionID).ConfigureAwait(false));
        }
        System.Threading.Thread.Sleep(500);
        Response.Redirect(Request.RawUrl);
    }
    //public class DataCallback
    //{
    //    public string RefCode { get; set; }
    //    public string Mobile { get; set; }
    //    public string OrderNo { get; set; }
    //    public string OrderInfo { get; set; }
    //    public int Amount { get; set; }

    //}
    public static async Task<string> CallbackJson(string url, string postData, long Id)
    {

        NLogLogger.Info(new string[] { "BankGate Detail", "Callback", "Partner", "Request", postData });
        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        client.Timeout = TimeSpan.FromSeconds(60);
        try
        {
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "Callback", "Partner", "Response", responseContent });
                var log = new LogInfo
                {
                    LogTime = DateTime.Now,
                    Url = url,
                    TransactionID = Id,
                    Request = postData,
                    Respone = responseContent
                };
                LogCache.LogBank(log);
                client.Dispose();
                return responseContent;
            }
            else
            {
                NLogLogger.Info(new string[] { "CMS", "Callback", "Partner", "Response Is Null" });
            }

        }
        catch (Exception e)
        {
            //var responseStream = e.Response.GetResponseStream();

            //if (responseStream != null)
            //{
            //    using (var reader = new StreamReader(responseStream))
            //    {
            //        NLogLogger.Info(new string[] { "CMS", "Exeption Post", reader.ReadToEnd() }); 
            //        
            //    }
            //}
            var log = new LogInfo
            {
                LogTime = DateTime.Now,
                Url = url,
                TransactionID = Id,
                Request = postData,
                Respone = e.Message
            };
            LogCache.LogBank(log);
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }
}