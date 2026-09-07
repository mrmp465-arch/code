using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class Pages_BankEWalletService_Bank_Edit_TranIn : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    //MomoTransaction _bankTranLog = new MomoTransaction();

    public BankTransaction _bankTranLog
    {
        get { return (BankTransaction)ViewState["Results"]; }
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

    public class RequestContent
    {
        public int TransId { get; set; }
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }
        public string BankTransId { get; set; }
        public int Amount { get; set; }
        public DateTime TimeBankSuccess { get; set; }
        public string Note { get; set; }
        public string CheckTransId { get; set; }

        public string PartnerBankId { get; set; }
        public string PartnerBankName { get; set; }
        public string PartnerBankCode { get; set; }
        public string NoteFull { get; set; }
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
        if (_bankTranLog == null)
        {
            _bankTranLog = new BankTransaction();
        }
        _bankTranLog.Id = Convert.ToInt64(Request["id"]);
        _bankTranLog = _bankTranLog.Get();
        lblId.Text = _bankTranLog.Id.ToString();
        lblRefCode.Text = _bankTranLog.RefCode;
        lblBankTransId.Text = _bankTranLog.BankTransId;
        lblBankCode.Text = _bankTranLog.BankCode;
        lblPartnerCode.Text = _bankTranLog.PartnerCode;
        lblCommandCode.Text = _bankTranLog.CommandCode;
        lblPartnerBankId.Text = _bankTranLog.PartnerBankId;
        lblUserBankId.Text = _bankTranLog.BankId;
        lblContent.Text = _bankTranLog.CommentOrg;
        lblAmount.Text = Convert.ToInt32(_bankTranLog.Amount).ToString("N0").Replace(",", ".");
        lblCreatTime.Text = Convert.ToDateTime(_bankTranLog.CreatedTime).ToString("dd/MM/yyyy HH:mm:ss");
        lblStatus.Text = _bankTranLog.Status + " (" + ResponseUtils.Description(Convert.ToInt32(_bankTranLog.Status)) + ")";
        lblCallbackUrl.Text = _bankTranLog.CallbackUrl;
        txtNote.Text = _bankTranLog.Comment;
        lblBankTime.Text = string.Format("{0:dd/MM/yyyy HH:mm:ss}", _bankTranLog.UpdateTime);


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

       

        var objContent = serializer.Deserialize<RequestContent>(_bankTranLog.RequestContent);
        objContent.Note = txtNote.Text;
        objContent.CheckTransId = "1";
        var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
        apiResponse.ResponseContent = JsonConvert.SerializeObject(objContent);
        var result = PostJson(_bankTranLog.CallbackUrl, serializer.Serialize(apiResponse));
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
           
            //update
            _bankTranLog.Id = Convert.ToInt64(Request["id"]);
            _bankTranLog.Comment = txtNote.Text;
            _bankTranLog.UpdateComment();


            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "bankcontentupdate",
                ActionName = "Cập nhật nội dung giao dịch bank",
                Description = "Cập nhật nội dung giao dịch bank " + _bankTranLog.Id + " | " + _bankTranLog.Comment
            };
            _userLog.Add();
            TelegramClient.SendTeleV2("-1003929087364", "Cập nhật nội dung giao dịch bank " + _bankTranLog.Id + " | " + _bankTranLog.Comment + " | " + _bankTranLog.Amount.GetValueOrDefault().ToString("#,#").Replace(",", ".") + " Từ tài khoản " + AppUtils.UserName);

            //TelegramClient.SendWarning("-4280016789", "Cập nhật nội dung giao dịch bank " + _bankTranLog.Id + " | " + _bankTranLog.Comment + " Từ tài khoản " + AppUtils.UserName);

            callout.Attributes.Add("class", "callout callout-success");
        }
        var jsonResult = JValue.Parse(result.Result).ToString(Formatting.Indented);
        contentSpan.InnerText = jsonResult;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankTransaction);
    }

    public static async Task<string> PostJson(string url, string postData)
    {

        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);
        try
        {
            NLogLogger.Info(new string[] { "CMS", "Callback Partner", "Request", postData });
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "Callback Partner", "Response", responseContent });
                client.Dispose();
                return responseContent;
            }
            else
            {
                NLogLogger.Info(new string[] { "CMS", "Callback Partner", "Response Is Null" });
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