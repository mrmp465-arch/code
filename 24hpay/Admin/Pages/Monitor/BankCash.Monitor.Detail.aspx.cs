using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.CardTelco;
using Libs.Report;
using Libs.Utils;

public partial class Pages_Monitor_BankCash_Monitor_Detail : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankCashMonitorDetail);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        Libs.Report.BankCashAPI _BankCash = new Libs.Report.BankCashAPI();
        _BankCash.TransactionID = Convert.ToInt64(Request["id"]);

        _BankCash = _BankCash.Get();

        if (_BankCash.Status == 1 || _BankCash.Status == -1)
        {
            lnCallback.Visible = true;
        }
        else
        {
            lnCallback.Visible = false;
        }

        lblTransactionID.Text = _BankCash.TransactionID.ToString();
        lblAccountName.Text = _BankCash.Note;

        //lblAmount.Text = _BankCash.Amount.ToString();
        lblTotalAmount.Text = _BankCash.Amount.ToString("#,#").Replace(",", ".");

        lblProvider.Text = _BankCash.ProviderCode;
        lblCreatTime.Text = _BankCash.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss");
        lblLastTime.Text = _BankCash.LastTime.ToString("dd/MM/yyyy HH:mm:ss");
        lblRefCode.Text = _BankCash.RefCode;
        lblBankCode.Text = _BankCash.BankCode;
        lblOrderNo.Text = _BankCash.OrderInfo;
        //lblOrderInfo.Text = _BankCash.OrderInfo;
        //lblMobile.Text = _BankCash.Mobile;
        //lblEmail.Text = _BankCash.Email;
        lblBankAccountName.Text = _BankCash.BankAccountName;
        lblBankAccountNumber.Text = _BankCash.BankAccountNumber;
        lblCallbackUrl.Text = _BankCash.ReturnUrl;
        lblStatus.Text = _BankCash.Status.ToString();
        txtLog.Text = _BankCash.LogContent;

        //log callback
        var lstLogData = LogCache.GetLogBankCash(_BankCash.TransactionID);
        rptList.DataSource = lstLogData;
        rptList.DataBind();
        //if (_BankCash.Status != 0)
        //{
        //    lnCallback.Visible = true;
        //}
        if (!AppUtils.IsAdmin)
        {
            if (_BankCash.PartnerCode != AppUtils.UserName)
            {
                Response.Redirect(Constant.ADMIN_PATH + "500.html");
            }
        }
    }
    protected void txtRecheck_Click(object sender, EventArgs e)
    {

        //var handler = BankGateV2Factory.GetHandler(lblProvider.Text);
        //var result = handler.ReCheck(lblTransactionID.Text);
        //lblRecheck.Text = result.Description;

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

    //callback
    protected void lnCallback_Click(object sender, EventArgs e)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        Libs.Report.BankCashAPI _CardAPILog = new Libs.Report.BankCashAPI();
        _CardAPILog.TransactionID = Convert.ToInt64(Request["id"]);

        _CardAPILog = _CardAPILog.Get(Convert.ToInt64(Request["id"]));

        //CallBack Partner
        var partner = new Partners().Get(_CardAPILog.PartnerCode);
        var privateKey = partner.PrivateKey;
        if (!string.IsNullOrEmpty(_CardAPILog.ReturnUrl))
        {
            var datacb = new DataCallback()
            {
                Amount = Convert.ToInt32(_CardAPILog.Amount),
                RefCode = _CardAPILog.RefCode,
                OrderInfo = _CardAPILog.TransactionID.ToString(),
                TransactionID = _CardAPILog.TransactionID.ToString(),
                Type = "bankout"
            };
            if (_CardAPILog.BankCode == "MOMO")
                datacb.Type = "momoout";
            var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {


            };
            if (_CardAPILog.Status < 0)
            {
                if (!_CardAPILog.PartnerCode.Contains("mark"))
                    datacb.Amount = 0;
                apiResponse = new APIResponse((int)ResponseCode.TransactionFailed)
                {


                };
            }
            datacb.ResponseCode = apiResponse.ResponseCode;
            datacb.Description = apiResponse.Description;
            datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode, partner.PrivateKey, partner.SignatureType);

            // apiResponse.ResponseContent = serializer.Serialize(datacb);
            //apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
            Task.Run(async () => await CallbackJson(_CardAPILog.ReturnUrl, serializer.Serialize(datacb), _CardAPILog.TransactionID).ConfigureAwait(false));
        }
        System.Threading.Thread.Sleep(500);
        BindData();

    }
    public class DataCallback
    {
        public string RefCode { get; set; }
        //public int Status { get; set; }
        public string TransactionID { get; set; }
        public Decimal Amount { get; set; }
        //public string Signature { get; set; }
        public string Type { get; set; }
        public string OrderInfo { get; set; }
        public int ResponseCode { get; set; }

        public string Description { get; set; }

        public string Signature { get; set; }
    }
    public static async Task<string> CallbackJson(string url, string postData, long Id)
    {

        NLogLogger.Info(new string[] { "MDrum", "Callback", "Partner", "Request", postData });
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
                LogCache.LogBankCash(log);
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