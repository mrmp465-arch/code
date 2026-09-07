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
using ServiceStack.Common.Extensions;
using System.Configuration;

public partial class Pages_Monitor_Add_TranIn : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    static string UrlBaseService = ConfigurationManager.AppSettings["Momo_service"] ?? "http://149.28.130.246:9001/MomoService.ashx";

    public string Solution
    {
        get
        {
            return ViewState["Solution"].ToString();
        }
        set
        {
            ViewState["Solution"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Server.ScriptTimeout = 360;

        AppUtils.CheckRoles(Resources.Url.AddTranIn);

        if (!IsPostBack)
            resultDiv.Visible = false;

    }

    public class ReCharge
    {
        public long TransId { get; set; } // TranId hệ thống
        public string PartnerMomoId { get; set; } // 
        public string MomoId { get; set; } // Id khách hàng
        public string MomoName { get; set; } // Name khách hàng
        public string MomoTransId { get; set; }
        public int Amount { get; set; }
        public DateTime? TimeMomoSuccess { get; set; }
        public string Note { get; set; }
        public bool Approve { get; set; }
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

    protected void txtRecheck_Click(object sender, EventArgs e)
    {

        //var handler = CardTelcoFactory.GetHandler(lblProvider.Text);
        //var result = handler.ReCheck(lblTransactionID.Text);
        //lblRecheck.Text = result.Description;

    }

    protected void btnAcceptedTran_Click(object sender, EventArgs e)
    {
        var COMMAND = string.Empty;
        if (ViewState["Solution"] == "LD")
        {
            COMMAND = "RECHARGE_ACCEPT";
        }
        else
        {
            COMMAND = "RECHARGE_CHECK";
        }

        var recharge = new ReCharge()
        {
            MomoTransId = MomoTransactionId.Value,
            MomoId = MomoUserId.Value,
            PartnerMomoId = MomoPartnerId.Value,
            Approve = true
        };

        var request = new RequestData()
        {
            CommandCode = COMMAND,
            RequestContent = serializer.Serialize(recharge)
        };
        var result = PostJson(UrlBaseService, serializer.Serialize(request));
        var objResult = serializer.Deserialize<APIResponse>(result.Result);
        if (objResult == null)
        {
            btnAcceptedTran.Disabled = true;
            callout.Attributes.Add("class", "callout callout-danger");
            contentSpan.InnerText = "Accept Failed";
            return;
        }

        if (objResult.ResponseCode != (int)ResponseCode.TransactionSuccessful)
        {
            btnAcceptedTran.Disabled = true;
            callout.Attributes.Add("class", "callout callout-danger");
        }
        else
        {
            callout.Attributes.Add("class", "callout callout-success");
        }

        var jsonResult = Newtonsoft.Json.Linq.JValue.Parse(result.Result).ToString(Newtonsoft.Json.Formatting.Indented);
        contentSpan.InnerText = jsonResult;
        btnAcceptedTran.Visible = false;

    }

    protected void btnCheckTran_Click(object sender, EventArgs e)
    {
        btnAcceptedTran.Visible = true;
        btnAcceptedTran.Disabled = false;

        if (string.IsNullOrEmpty(MomoTransactionId.Value) || string.IsNullOrEmpty(MomoPartnerId.Value))
        {
            resultDiv.Visible = true;
            btnAcceptedTran.Disabled = true;
            callout.Attributes.Add("class", "callout callout-danger");
            contentSpan.InnerText = "Thông tin không được để trống";
            return;
        }

        var _Momo = new MomoAccounts();
        _Momo = _Momo.Get(MomoPartnerId.Value);
        if (_Momo == null)
        {
            resultDiv.Visible = true;
            btnAcceptedTran.Disabled = true;
            callout.Attributes.Add("class", "callout callout-danger");
            contentSpan.InnerText = "Không tìm thấy tài khoản hệ thống này";
            return;
        }

        ViewState["Solution"] = _Momo.Solution;

        if (ViewState["Solution"] == "LD")
        {
            UrlBaseService = "http://127.0.0.1:9001/MomoService.ashx";
        }
        else
        {

            UrlBaseService = "http://127.0.0.1:9002/MomoService.ashx";
            //UrlBaseService = "https://localhost:44373/MomoService.ashx";
            //UrlBaseService = "http://149.28.130.246:9002/MomoService.ashx";
        }

        var recharge = new ReCharge()
        {
            MomoTransId = MomoTransactionId.Value,
            MomoId = MomoUserId.Value,
            PartnerMomoId = MomoPartnerId.Value,
            Approve = false
        };

        var request = new RequestData()
        {
            CommandCode = "RECHARGE_CHECK",
            RequestContent = serializer.Serialize(recharge)
        };
        var result = PostJson(UrlBaseService, serializer.Serialize(request));
        var objResult = serializer.Deserialize<APIResponse>(result.Result);
        if (objResult == null)
        {
            btnAcceptedTran.Disabled = true;
            callout.Attributes.Add("class", "callout callout-danger");
            contentSpan.InnerText = "Check Failed";
            return;
        }

        if (objResult.ResponseCode != (int)ResponseCode.TransactionSuccessful)
        {
            btnAcceptedTran.Disabled = true;
            callout.Attributes.Add("class", "callout callout-danger");
        }
        else
        {
            callout.Attributes.Add("class", "callout callout-success");
        }

        var jsonResult = Newtonsoft.Json.Linq.JValue.Parse(result.Result).ToString(Newtonsoft.Json.Formatting.Indented);
        contentSpan.InnerText = jsonResult;
        btnAcceptedTran.Visible = true;
        resultDiv.Visible = true;
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
        client.Timeout = TimeSpan.FromSeconds(300);
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