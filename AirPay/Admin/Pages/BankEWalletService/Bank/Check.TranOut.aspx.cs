using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.CardTelco;
using Libs.Utils;
using CardAPILog = Libs.Report.CardAPILog;
using System.Configuration;
using DocumentFormat.OpenXml.Vml.Office;
using System.Web.UI.HtmlControls;

public partial class Pages_BankEWalletService_Bank_TranOut : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    static string Bank_Service_Url = ConfigurationManager.AppSettings["Bank_service"] ?? "http://149.28.130.246:9001/BankService.ashx";
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Server.ScriptTimeout = 360;

        AppUtils.CheckRoles(Resources.Url.CheckBankTranOut);

        if (!IsPostBack)
            resultDiv.Visible = false;

        if (!IsPostBack)
        {
            BindData();
        }
    }

    public class CashOut
    {
        public string TransId { get; set; }
        public string BankTransId { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }
        public int Amount { get; set; }
        public object TimeBankSuccess { get; set; }
        public string Note { get; set; }
        public object CallbackUrl { get; set; }
    }

    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string ServiceCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Description { get; set; }
        public string Signature { get; set; }

    }
    public string getBankId(string requestcontent)
    {
        //return requestcontent;
        try
        {
            var data = serializer.Deserialize<CashOut>(requestcontent);
            return data.BankId;
        }
        catch
        {
            return "";
        }
    }
    public string getBankNote(string requestcontent)
    {
        //return requestcontent;
        try
        {
            var data = serializer.Deserialize<CashOut>(requestcontent);
            return data.Note;
        }
        catch
        {
            return "";
        }
    }

    private void BindData()
    {
        BankTransaction _bankTranLog = new BankTransaction();
        _bankTranLog.Id = Convert.ToInt64(Request["id"]);
        _bankTranLog = _bankTranLog.Get();
        lblId.Text = _bankTranLog.Id.ToString();
        lblRefCode.Text = _bankTranLog.RefCode;
        lblPartnerCode.Text = _bankTranLog.PartnerCode;
        lblCommandCode.Text = _bankTranLog.CommandCode;
        lblPartnerBankCode.Text = _bankTranLog.PartnerBankCode;


        try
        {
            var bank = new BankAccounts().Get(_bankTranLog.PartnerBankId, _bankTranLog.PartnerBankCode);
            lblPartnerBankId.Text = _bankTranLog.PartnerBankId + "-" + bank.PhoneDevice + "-" + bank.Computer;

        }
        catch
        {
            lblPartnerBankId.Text = _bankTranLog.PartnerBankId;
        }
        lblBankCode.Text = _bankTranLog.BankCode;
        try
        {
            lblBankName.Text = BankList.GetBankList().Find(c => c.napasCode.ToUpper() == _bankTranLog.BankCode.ToUpper()).bankName;
            lblPartnerBankName.Text = BankList.GetBankList().Find(c => c.napasCode.ToUpper() == _bankTranLog.PartnerBankCode.ToUpper()).bankName;
        }
        catch
        {
            lblBankName.Text = _bankTranLog.BankCode;
            lblPartnerBankName.Text = _bankTranLog.PartnerBankCode;
        }

        lblUserBankId.Text = _bankTranLog.BankId;
        lblAmount.Text = Convert.ToInt32(_bankTranLog.Amount).ToString("N0").Replace(",", ".");
        lblCreatTime.Text = Convert.ToDateTime(_bankTranLog.CreatedTime).ToString("dd/MM/yyyy HH:mm:ss");
        lblStatus.Text = _bankTranLog.Status + " (" + ResponseUtils.Description(Convert.ToInt32(_bankTranLog.Status)) + ")";
        lblCallbackUrl.Text = _bankTranLog.CallbackUrl;
        lblNote.Text = _bankTranLog.CommentOrg;
        lblDescription.Text = _bankTranLog.Description;

        if (_bankTranLog.Status == 1)
        {
            btnCheckTran.Visible = false;
        }

        btnCheckTran.Disabled = true;
    }

    protected void btnAcceptedTran_Click(object sender, EventArgs e)
    {
        var request = new RequestData()
        {
            PartnerCode = lblPartnerCode.Text,
            CommandCode = "CASHOUT_ACCEPT",
            RequestContent = lblId.Text
        };
        btnAcceptedTran.Enabled = false;
        var result = PostJson(Bank_Service_Url, serializer.Serialize(request));
        //var result = PostJson("https://localhost:44373/BankService.ashx", serializer.Serialize(request));
        var objResult = serializer.Deserialize<APIResponse>(result.Result);
        if (objResult.ResponseCode != (int)ResponseCode.TransactionSuccessful)
        {
            callout.Attributes.Add("class", "callout callout-danger");
        }
        else
        {
            callout.Attributes.Add("class", "callout callout-success");
        }

        var jsonResult = Newtonsoft.Json.Linq.JValue.Parse(result.Result).ToString(Newtonsoft.Json.Formatting.Indented);
        contentSpan.InnerText = jsonResult;
        resultDiv.Visible = true;
        
    }

    protected void btnUpdateLog_Click(object sender, EventArgs e)
    {
        btnUpdateLog.Enabled = false;

        BankTransaction _bankTranLog = new BankTransaction();
        _bankTranLog.Id = Convert.ToInt64(Request["id"]);
        _bankTranLog = _bankTranLog.Get();
        _bankTranLog.UpdateTime = DateTime.Now;
        _bankTranLog.Status = 2;
        var result = _bankTranLog.Update();
        if (result > 0)
        {
            contentSpan.InnerText = "Thành công";
            callout.Attributes.Add("class", "callout callout-success");
        }
        else
        {
            contentSpan.InnerText = "Thất bại";
            callout.Attributes.Add("class", "callout callout-danger");
        }

        resultDiv.Visible = true;


    }

    protected void btnCheckTran_Click(object sender, EventArgs e)
    {
        btnCheckTran.Visible = false;
        return;


        if (lblStatus.Text.Equals("0 (Card processing)"))
        {
            btnAcceptedTran.Visible = true;
            btnAcceptedTran.Enabled = true;

            var request = new RequestData()
            {
                PartnerCode = lblPartnerCode.Text,
                CommandCode = "CASHOUT_CHECK",
                RequestContent = lblId.Text
            };
            var result = PostJson(Bank_Service_Url, serializer.Serialize(request));
            var objResult = serializer.Deserialize<APIResponse>(result.Result);
            if (objResult.ResponseCode != (int)ResponseCode.TransactionSuccessful)
            {
                btnAcceptedTran.Text = "Lệnh Client chuyển khoản lại";
                callout.Attributes.Add("class", "callout callout-danger");
            }
            else
            {
                btnAcceptedTran.Text = "Thực hiện Callback lại";
                callout.Attributes.Add("class", "callout callout-success");
            }

            var jsonResult = Newtonsoft.Json.Linq.JValue.Parse(result.Result).ToString(Newtonsoft.Json.Formatting.Indented);
            contentSpan.InnerText = jsonResult;
            resultDiv.Visible = true;
        }

        if (lblStatus.Text.Equals("-2 (Transaction suspicious)"))
        {
            btnUpdateLog.Visible = true;
            btnUpdateLog.Enabled = true;

            btnAcceptedTran.Visible = true;
            btnAcceptedTran.Enabled = true;

            callout.Attributes.Add("class", "callout callout-danger");
            contentSpan.InnerHtml = "<b>Chú ý quan trọng: Đây là trường hợp nghi vấn.</b><br>1. Kiểm tra bằng mắt thường tại <b>Lịch sử giao dịch Chuyển Tiền</b> trên Client." +
                                    "<br>2. Nếu thực sự có giao dịch rồi thì click <b>CẬP NHẬT LOG THÀNH CÔNG</b> để update log." +
                                    "<br>3. Nếu chưa có giao dịch thì click <b>LỆNH CLIENT CHUYỂN KHOẢN LẠI</b> để chuyên khoản lại cho khách hàng ";
            btnAcceptedTran.Text = "Lệnh Client chuyển khoản lại";
            resultDiv.Visible = true;
        }

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankTransaction);
    }

    public static async Task<string> PostJson(string url, string postData)
    {

        //NLogLogger.Info(new string[] { "MDrum", "Callback", "Partner", "Request", postData });
        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);
        try
        {
            NLogLogger.Info(new string[] { "CMS", "R", "Request", postData });
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "R", "Response", responseContent });
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
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }


}