using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Libs.Utils;
using Telegram.Bot.Types;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

public partial class Pages_Momo_Transaction : System.Web.UI.Page
{
    public JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoTransaction);

        if (!IsPostBack)
        {
            try
            {
                init();
                GetList();
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "Pages_Momo_Transaction", "Page_Load", exp.Message });
            }

        }
    }
    private void init()
    {
        txtCreatTime.Text = DateTime.Now.AddDays(1).ToString();
        var lst = new PartnerMomo().GetListPartner();
        lst = lst.OrderBy(x => x.Code).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "Code";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));
    }
    public string getMomoId(string requestcontent)
    {
        //return requestcontent;
        try
        {
            var data = serializer.Deserialize<RequestData>(requestcontent);
            return data.MomoId;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
        
    }
    public string getMomoTransId(string requestcontent)
    {
        //return requestcontent;
        try
        {
            var data = serializer.Deserialize<RequestData>(requestcontent);
            return data.MomoTransId;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
       
    }

    public string getNote(string requestcontent)
    {
        //return requestcontent;
        try
        {
            var data = serializer.Deserialize<RequestData>(requestcontent);
            return data.Note;
        }
        catch (Exception e)
        {
            return string.Empty;
        }

    }

    public class RequestData
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

    private void GetList()
    {
        int top = Convert.ToInt32(drpTop.SelectedValue);
        DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
        int? status = null;

        Int64? id = AppUtils.ToInt64(txtId.Text);
        if (id == 0) id = null;

        //if (drpStatus.SelectedValue != "-1")
        //    status = AppUtils.ToInt32(drpStatus.SelectedValue);

        if (string.IsNullOrEmpty(txtStatus.Text))
            status = null;
        else
            status = AppUtils.ToInt32(txtStatus.Text);



        var lstdata = new MomoTransaction().GetList(top, id, drpPartner.SelectedValue, txtPartnerMomoId.Text, txMomoTransId.Text, txtMomoId.Text, drpType.SelectedValue, creatTime, status, cbByPass10k.Checked, txtRefCode.Text, txtComment.Text,drpSolution.SelectedValue);
        rptList.DataSource = lstdata;
        rptList.DataBind();

        long Balance = GetBalanceCache();
        if (Balance > 300000)
        {
            lblTotal.Text = "Số dư VSIGN: " + Balance.ToString("#,#").Replace(",", ".");
        }
        else
        {
            lblTotal.Text = "Số dư VSIGN: " + Balance.ToString("#,#").Replace(",", ".");
            lblTotal.CssClass = "bwarning";
        }
        //else
        //{
        //    lblTotal.Text = "Số dư VSIGN: " + Balance.ToString("#,#").Replace(",", ".");
        //    lblTotal.CssClass = "bwarning";
        //}
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string CheckTranOutUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.CheckTranOut + "?id=" + id;
    }

    protected string EditTranInUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.EditTranIn + "?id=" + id;
    }

    protected void AddTranIn_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.AddTranIn);
    }
    protected void Callback_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        long Id = Convert.ToInt64(e.CommandArgument.ToString());
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        MomoTransaction _momoTranLog = new MomoTransaction();
        _momoTranLog.Id = Id;

        //CallBack Partner
        _momoTranLog = _momoTranLog.Get();
        var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
        apiResponse.ResponseContent = _momoTranLog.RequestContent;
        var unused = PostJson(_momoTranLog.CallbackUrl, serializer.Serialize(apiResponse));
        GetList();
    }
    protected void UpdateCash_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        long Id = Convert.ToInt64(e.CommandArgument.ToString());
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        MomoTransaction _momoTranLog = new MomoTransaction();
        _momoTranLog.Id = Id;
        //CallBack Partner
        _momoTranLog=_momoTranLog.Get();
       // NLogLogger.Info(new string[] { "UpdateCash", _momoTranLog.Amount.GetValueOrDefault().ToString(), _momoTranLog.PartnerMomoId });
        _momoTranLog.UpdateCash(_momoTranLog.Amount.GetValueOrDefault(), _momoTranLog.PartnerMomoId);
        System.Threading.Thread.Sleep(500);
        GetList();
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
    public long GetBalanceCache()
    {
        string KeyCache = "VSIGN_BALANCE";
        try
        {
            var result = DataCaching.GetCache<string>(KeyCache);
            if (result == null)
            {
                result = getBalance().Result;
                result = DataCaching.SetCache(KeyCache, result, 300);
            }
            return long.Parse(result);
        }
        catch (Exception ex)
        {
            //   ExceptionHandler.Handle(ex, "Partners", "Get:" + KeyCache);
            return 0;
        }
    }
    public static async Task<string> getBalance()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:9002/MomoService.ashx");
        var content = new StringContent("{\"PartnerCode\":\"\",\"CommandCode\":\"VSIGN_BALANCE\",\"RequestContent\":\"\",\"Description\":\"\",\"Signature\":\"\"}", null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var resObj = serializer.Deserialize<BalanceResponse>(result);

        return resObj.ResponseContent;



    }
    public string GetCodeStatus(string type, string code, string orderNo)
    {
        if (type == "IN")
        {
            if (code.Contains("SYS"))
            {
                return "THÀNH CÔNG";
            }
            if (string.IsNullOrEmpty(orderNo))
            {
                return code;
            }
            if (code == "[COMMENT_INCORRECT_FORMAT]" && string.IsNullOrEmpty(orderNo.Trim()))
            {
                return "<div style='color:red'>SAI CODE</div>";
            }
            if (orderNo == "[COMMENT_INCORRECT_FORMAT]" || orderNo == "[COMMENT_INCORRECT]" || orderNo.ToUpper() == "[ORDER_NOTFOUND]")
            {
                return "<div style='color:red'>SAI CODE</div>";
            }
            else
            {
                if (orderNo.ToUpper() == "[ORDER_DUPLICATE]")
                {
                    return "<div style='color:red'>TRÙNG CODE</div>";
                }
                else
                {
                    if (orderNo.ToUpper() == "[ORDER_WRONGAMOUNT]")
                    {
                        return "<div style='color:red'>SAI SỐ TIỀN</div>";
                    }
                    if (orderNo.ToUpper() == "[ORDER_OVERTIME]")
                    {
                        return "<div style='color:red'>OVERTIME</div>";
                    }
                    if (!string.IsNullOrEmpty(orderNo.Trim()))
                        return orderNo;
                    return code;

                }
            }

        }
        return "";
    }
    public class BalanceResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }
}