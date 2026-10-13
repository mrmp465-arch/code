using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using System.Globalization;
using Libs.API;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Libs.Utils;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Net;

public partial class Pages_Monitor_BankGateAPI_Search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPISearch);

        if (!IsPostBack)
        {
            init();
            //GetList();
        }
    }

    private void init()
    {
        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));

        var lstProvider = new List<Providers>();
        if (AppUtils.IsAdmin)
            lstProvider = new Providers().GetList(13);
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();

        drpProvider.DataSource = lstProvider;
        drpProvider.DataTextField = "Name";
        drpProvider.DataValueField = "ProviderCode";
        drpProvider.DataBind();
        drpProvider.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));

        txtCreatTime.Text = DateTime.Now.ToString("MM/dd/yyyy");

       
        drpBankCode.DataSource = new BankGateAPI().GetBankCode();
        drpBankCode.DataTextField = "BankCode";
        drpBankCode.DataValueField = "BankCode";
        drpBankCode.DataBind();
        drpBankCode.Items.Insert(0, new ListItem("BankCode:", ""));
    }
    protected void Callback_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        long TransId = Convert.ToInt64(e.CommandArgument.ToString());
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        BankGateAPI _CardAPILog = new BankGateAPI();

        //var tranid = AppUtils.ToInt64(txtTransactionID.Text);
        _CardAPILog.TransactionID = TransId;
        System.Threading.Thread.Sleep(300);
        _CardAPILog = _CardAPILog.Get(TransId);
        //CallBack Partner
        var partner = new Partners().Get(_CardAPILog.PartnerCode);
        var privateKey = partner.PrivateKey;
        var url = _CardAPILog.ReturnUrl;
        if (string.IsNullOrEmpty(url))
            url = partner.SMSPlusUrl;
        if (!string.IsNullOrEmpty(url))
        {
            var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(new DataCallback()
                {
                    Mobile = _CardAPILog.Mobile,
                    RefCode = _CardAPILog.RefCode,
                    OrderNo = _CardAPILog.OrderNo,
                    OrderInfo = _CardAPILog.OrderInfo,
                    Amount = Convert.ToInt32(_CardAPILog.TotalAmount),
                })
            };
            if (_CardAPILog.TotalAmount <= 0)
                return;
            apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, privateKey, 1);
            Task.Run(() => CallbackJson(url, serializer.Serialize(apiResponse), TransId).ConfigureAwait(false));
        }
        GetList();
    }
    public class DataCallback
    {
        public string RefCode { get; set; }
        public string Mobile { get; set; }
        public string OrderNo { get; set; }
        public string OrderInfo { get; set; }
        public int Amount { get; set; }

    }
    public static async Task<string> CallbackJson(string url, string postData, long Id)
    {

        NLogLogger.Info(new string[] { "BankGate Search", "Callback", "Partner", "Request", postData });
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
    private void GetList()
    {
        BankGateAPI _BankGateAPI = new BankGateAPI();
        //bool erro = false;
        //rptList.DataSource = _BankGateAPI.Search(AppUtils.ToInt64(txtTransactionID.Text));
        //if (erro) {
        //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfos').html('Nhập TransactionID sai'); $('#AlertInfo').modal()}); ", true);
        //    return;
        //}
        //else
        //{
        //    rptList.DataBind();
        //    return;
        //} 
        long transId = AppUtils.ToInt64(txtTransactionID.Text);
        int top = Convert.ToInt32(drpTop.SelectedValue);
        string refCode = txtRefCode.Text.Trim();
        string orderInfo = txtOrderInfo.Text.Trim();
        string orderNo = txtOrderNo.Text.Trim();
        string mobile = txtMobile.Text.Trim();
        string email = txtEmail.Text.Trim();
        string bankCode = drpBankCode.SelectedValue;
        string partnerCodes = drpPartner.SelectedValue;
        string providerCodes = drpProvider.SelectedValue;


        if (AppUtils.IsPartner && !AppUtils.IsAdmin)
        {

            if (string.IsNullOrEmpty(partnerCodes))
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                }
            }


        }

        if (AppUtils.IsProvider && !AppUtils.IsAdmin)
        {
            if (string.IsNullOrEmpty(providerCodes))
            {
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }

        if(transId>0)
        {
            rptList.DataSource = _BankGateAPI.Search(transId);
        }
        else
        {
            if (txtCreatTime.Text.Trim() == "")
            {
                rptList.DataSource = _BankGateAPI.Search(top, refCode, orderNo, orderInfo, mobile, email, bankCode, partnerCodes, providerCodes);
            }
            else
            {
                DateTime requestTime = AppUtils.ToDateTime(txtCreatTime.Text).AddDays(1);
                rptList.DataSource = _BankGateAPI.Search(top, refCode, orderNo, orderInfo, mobile, email, bankCode, partnerCodes, providerCodes, requestTime);
            }
        }
        
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }
    public string GetStatus(int status)
    { 
            return status.ToString();
    }
}