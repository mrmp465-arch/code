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
using Newtonsoft.Json;

public partial class Pages_Monitor_Edit_TranIn : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    //MomoTransaction _momoTranLog = new MomoTransaction();

    public MomoTransaction _momoTranLog
    {
        get { return (MomoTransaction)ViewState["Results"]; }
        set { ViewState["Results"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.EditTranIn);

        if (!IsPostBack)
            resultDiv.Visible = false;

        if (!IsPostBack)
        {
            BindData();
        }
    }

    public class ReCharge
    {
        public long TransId { get; set; } // TranId hệ thống
        public string MomoId { get; set; } // Id khách hàng
        public string MomoName { get; set; } // Name khách hàng
        public string MomoTransId { get; set; }
        public int Amount { get; set; }
        public DateTime? TimeMomoSuccess { get; set; }
        public string Note { get; set; }
        public string CheckTransId { get; set; }
        public string PartneMomoId { get; set; }
        public string PartnerMomoName { get; set; }
    }

    public class PostResponse
    {
        public int errorCode { get; set; }
        public string errorDescription { get; set; }

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

    private void BindData()
    {
        if (_momoTranLog == null)
        {
            _momoTranLog = new MomoTransaction();
        }
        _momoTranLog.Id = Convert.ToInt64(Request["id"]);
        _momoTranLog = _momoTranLog.Get();
        lblId.Text = _momoTranLog.Id.ToString();
        lblRefCode.Text = _momoTranLog.RefCode;
        lblPartnerCode.Text = _momoTranLog.PartnerCode;
        lblCommandCode.Text = _momoTranLog.CommandCode;
        lblPartnerMomoId.Text = _momoTranLog.PartnerMomoId;
        lblUserMomoId.Text = serializer.Deserialize<ReCharge>(_momoTranLog.RequestContent).MomoId;
        lblAmount.Text = Convert.ToInt32(_momoTranLog.Amount).ToString("N0").Replace(",", ".");
        lblCreatTime.Text = Convert.ToDateTime(_momoTranLog.CreatedTime).ToString("dd/MM/yyyy HH:mm:ss");
        lblStatus.Text = _momoTranLog.Status + " (" + ResponseUtils.Description(Convert.ToInt32(_momoTranLog.Status)) + ")";
        lblCallbackUrl.Text = _momoTranLog.CallbackUrl;
        txtNote.Text = _momoTranLog.Comment;
        lblMomoTime.Text = string.Format("{0:dd/MM/yyyy HH:mm:ss}", _momoTranLog.TimeMomoSuccess);


    }

    protected void btnEditTran_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtNote.Text))
        {
            resultDiv.Visible = true;
            callout.Attributes.Add("class", "callout callout-danger");
            contentSpan.InnerText = "Nội dung chuyển khoản không được để trống";
            return;
        }
        //else
        //{
        //    callout.Attributes.Add("class", "callout callout-success");
        //}

        var objContent = serializer.Deserialize<ReCharge>(_momoTranLog.RequestContent);
        objContent.Note = txtNote.Text;
        objContent.CheckTransId = "1";
        var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
        apiResponse.ResponseContent = JsonConvert.SerializeObject(objContent);
        var result = PostJson(_momoTranLog.CallbackUrl, serializer.Serialize(apiResponse));
        var objResult = serializer.Deserialize<PostResponse>(result.Result);
        //{ "errorCode":1,"errorDescription":"Transaction is successful"}
        if (objResult == null)
        {
            resultDiv.Visible = true;
            callout.Attributes.Add("class", "callout callout-danger");
            contentSpan.InnerText = "Post Back Failed";
            return;
        }

        resultDiv.Visible = true;
        if (objResult.errorCode != (int)ResponseCode.TransactionSuccessful)
        {

            callout.Attributes.Add("class", "callout callout-danger");
        }
        else
        {
            callout.Attributes.Add("class", "callout callout-success");
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "momocontentupdate",
                ActionName = "Cập nhật nội dung giao dịch momo",
                Description = "Cập nhật nội dung giao dịch momo " + _momoTranLog.MomoTransId + " | " + _momoTranLog.Comment
            };
            _userLog.Add();
            //TelegramClient.SendWarning("-4280016789", "Cập nhật nội dung giao dịch momo " + _momoTranLog.MomoTransId + " | " + _momoTranLog.Comment + " Từ tài khoản " + AppUtils.UserName);

        }
        var jsonResult = Newtonsoft.Json.Linq.JValue.Parse(result.Result).ToString(Newtonsoft.Json.Formatting.Indented);
        contentSpan.InnerText = jsonResult;
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