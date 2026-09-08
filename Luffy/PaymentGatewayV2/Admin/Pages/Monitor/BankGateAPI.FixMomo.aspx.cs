using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;

public partial class Pages_Monitor_BankGateAPI_FixMomo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIMonitor);

        if (!IsPostBack)
        {
            var lst = new List<Partners>();
            if (AppUtils.IsAdmin)
                lst = new Partners().GetList();
            else
                lst = new Partners().GetListByUserId(AppUtils.UserID);
            txtTransactionID.Text = Request["id"];
            Init(lst);
        }

    }

    protected void Init(List<Partners> lst)
    {

        lst = lst.Where(x => x.SMSPlusUrl != "").OrderBy(x => x.PartnerCode).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        BankGateAPI _BankGateAPI = new BankGateAPI();
        _BankGateAPI.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);

        _BankGateAPI = _BankGateAPI.Get();


        if (_BankGateAPI == null)
        {
            txtOrderInfo.Text = "";

            AlertInfos.Text = "Không tồn tại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        }
        else
        {
            //if(lst.Exists(x=>x.PartnerCode.Equals(_BankGateAPI.PartnerCode)))
            //{
            //    txtOrderInfo.Text = "";

            //    AlertInfos.Text = "Không tồn tại giao dịch";
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            //}
            //else
            //{
            drpPartner.SelectedValue = _BankGateAPI.PartnerCode.ToString();
            txtOrderInfo.Text = _BankGateAPI.OrderInfo.ToString();
            txOrderNo.Text = _BankGateAPI.OrderNo.ToString();
            hdPartner.Value = _BankGateAPI.PartnerCode.ToString();
            //}    

        }
    }
    protected void btUpdate_Click(object sender, EventArgs e)
    {
        BankGateAPI _BankGateAPI = new BankGateAPI();
        _BankGateAPI.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);
        _BankGateAPI = _BankGateAPI.Get();
        if (_BankGateAPI == null)
        {
            txtOrderInfo.Text = "";

            AlertInfos.Text = "Không tồn tại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        }
        else
        {
            if (drpPartner.SelectedValue == "pp")
            {
                txtOrderInfo.Text = "";

                AlertInfos.Text = "Vui lòng chọn lại đối tác";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
                return;
            }
            _BankGateAPI.PartnerCode = drpPartner.SelectedValue;
            var partner = new Partners().Get(_BankGateAPI.PartnerCode);
            _BankGateAPI.PartnerID = partner.PartnerID;
            _BankGateAPI.OrderNo = txOrderNo.Text;
            _BankGateAPI.UpdateContent();
            //CallBack Partner
            //var partner = new Partners().Get(_BankGateAPI.PartnerCode);
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var apiResponse = new DataCallback()
            {

                TransId = _BankGateAPI.RefCode,
                Content = _BankGateAPI.OrderNo,
                Amount = Convert.ToInt32(_BankGateAPI.Amount),
            };
            apiResponse.Signature = PaymentUtils.Signature(apiResponse.TransId + apiResponse.Amount + apiResponse.Content, partner.PrivateKey, partner.SignatureType);
            NLogLogger.Info(new string[] { "Momo", "PartnerCallback", partner.SMSPlusUrl, serializer.Serialize(apiResponse) });
            if (!string.IsNullOrEmpty(partner.SMSPlusUrl))
            {

                Task.Run(async () => await CallbackJson(partner.SMSPlusUrl, serializer.Serialize(apiResponse)).ConfigureAwait(false));
            }
            AlertSuccesss.Text = "Cập nhập thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
            HttpContext.Current.Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankGateAPIMonitor);
        }
    }

    public class DataCallback
    {
        public string TransId { get; set; }
        public int Amount { get; set; }
        public string Content { get; set; }
        public string Signature { get; set; }


    }
    public static async Task<string> CallbackJson(string url, string postData)
    {

        NLogLogger.Info(new string[] { "CallbackJson", "Callback", "Partner", "Request", postData });
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
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }
}