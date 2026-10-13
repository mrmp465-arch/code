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

public partial class Pages_Monitor_Check_TranOut : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    static string Momo_Service_Url = ConfigurationManager.AppSettings["Momo_service"] ?? "http://149.28.130.246:9001/MomoService.ashx";
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Server.ScriptTimeout = 360;

        AppUtils.CheckRoles(Resources.Url.CheckTranOut);

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
        public string MomoTransId { get; set; }
        public string MomoId { get; set; }
        public string MomoName { get; set; }
        public int Amount { get; set; }
        public object TimeMomoSuccess { get; set; }
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
    public string getMomoId(string requestcontent)
    {
        //return requestcontent;
        try
        {
            var data = serializer.Deserialize<CashOut>(requestcontent);
            return data.MomoId;
        }
        catch
        {
            return "";
        }
    }
    public string getMomoNote(string requestcontent)
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
        MomoTransaction _momoTranLog = new MomoTransaction();
        _momoTranLog.Id = Convert.ToInt64(Request["id"]);
        _momoTranLog = _momoTranLog.Get();
        lblId.Text = _momoTranLog.Id.ToString();
        lblRefCode.Text = _momoTranLog.RefCode;
        lblPartnerCode.Text = _momoTranLog.PartnerCode;
        lblCommandCode.Text = _momoTranLog.CommandCode;
        lblPartnerMomoId.Text = _momoTranLog.PartnerMomoId;
        lblUserMomoId.Text = _momoTranLog.MomoId;
        lblAmount.Text = Convert.ToInt32(_momoTranLog.Amount).ToString("N0").Replace(",", ".");
        lblCreatTime.Text = Convert.ToDateTime(_momoTranLog.CreatedTime).ToString("dd/MM/yyyy HH:mm:ss");
        lblUpdateTime.Text = Convert.ToDateTime(_momoTranLog.UpdateTime).ToString("dd/MM/yyyy HH:mm:ss");
        lblStatus.Text = _momoTranLog.Status + " (" + ResponseUtils.Description(Convert.ToInt32(_momoTranLog.Status)) + ")";
        lblCallbackUrl.Text = _momoTranLog.CallbackUrl;
        lblNote.Text = _momoTranLog.Comment;
        lblDescription.Text= _momoTranLog.Description;

        if (_momoTranLog.Status == 1)
        {
            btnCheckTran.Visible = false;
        }

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
        var result = PostJson(Momo_Service_Url, serializer.Serialize(request));
        //var result = PostJson("https://localhost:44373/MomoService.ashx", serializer.Serialize(request));
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

        MomoTransaction _momoTranLog = new MomoTransaction();
        _momoTranLog.Id = Convert.ToInt64(Request["id"]);
        _momoTranLog = _momoTranLog.Get();
        _momoTranLog.UpdateTime = DateTime.Now;
        _momoTranLog.Status = 2;
        var result = _momoTranLog.Update();
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
            var result = PostJson(Momo_Service_Url, serializer.Serialize(request));
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
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoTransaction);
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