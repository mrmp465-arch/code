using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.IO;

public partial class Pages_Monitor_CardAPI_Monitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPIMonitor);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        if (!IsPostBack)
        {
            init();
            GetList();
        }

    }
    protected void Callback_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        long TransId = Convert.ToInt64(e.CommandArgument.ToString());
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        CardAPILog _CardAPILog = new CardAPILog();
        TopupMobileLog _topupMobileLog = new TopupMobileLog();
        //var tranid = AppUtils.ToInt64(txtTransactionID.Text);
        _CardAPILog.TransactionID = TransId;
        _CardAPILog = _CardAPILog.Get();
        //CallBack Partner
        var privateKey = new Partners().Get(_CardAPILog.PartnerCode).PrivateKey;
        if (!string.IsNullOrEmpty(_CardAPILog.CallbackUrl))
        {
            var datacb = new DataCallback()
            {
                Amount = _CardAPILog.Amount,
                RefCode = _CardAPILog.RequestNo,
                Status = _CardAPILog.Status,
                Signature = Libs.Utils.Encrypts.MD5(_CardAPILog.RequestNo + _CardAPILog.Status + _CardAPILog.Amount + privateKey)
            };
            Task.Run(() => CallbackJson(_CardAPILog.CallbackUrl, serializer.Serialize(datacb), _CardAPILog.PartnerCode, _CardAPILog.TransactionID).ConfigureAwait(false));
        }
        GetList();
    }
    private void init()
    {
        btView.Text = Resources.Pay.View;
        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);


        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();

        drpPartner.Items.Insert(0, new ListItem(Resources.Pay.Partner, ""));
        var lstProvider = new List<Providers>();
        if (AppUtils.IsAdmin)
            lstProvider = new Providers().GetList(7);
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x=>x.Type==7).ToList();
        lstProvider = lstProvider.OrderBy(x => x.ProviderCode).ToList();
        drpProvider.DataSource = lstProvider;
        drpProvider.DataTextField = "Name";
        drpProvider.DataValueField = "ProviderCode";
        drpProvider.DataBind();
        drpProvider.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));
        
        drpCardType.DataSource = new Products().GetList(7, 1);
        drpCardType.DataTextField = "Name";
        drpCardType.DataValueField = "Code";
        drpCardType.DataBind();
        drpCardType.Items.Insert(0, new ListItem(Resources.Pay.CardType, ""));

        drpStatus.Items.Insert(0, new ListItem(Resources.Pay.Status, "-999"));
        drpStatus.Items.Insert(1, new ListItem(Resources.Pay.Success, "1"));
        drpStatus.Items.Insert(2, new ListItem(Resources.Pay.Processing, "0"));
        drpStatus.Items.Insert(3, new ListItem(Resources.Pay.Fail, "-1"));

        txtCreatTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1).ToString("dd/MM/yyyy HH:mm:ss");
        //txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy HH:mm:ss");
        txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("dd/MM/yyyy HH:mm:ss");
        if (AppUtils.IsAdmin)
            txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-10).ToString("dd/MM/yyyy HH:mm:ss");
    }
    public DateTime ToDateTime(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                return DateTime.Parse(value, cul);
                //return Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
        return DateTime.Now;
    }
    private void GetList()
    {
        //NLogLogger.Info(new string[] { "IsProvider", AppUtils.IsProvider.ToString(), "IsPartner", AppUtils.IsPartner.ToString() });
        int top = Convert.ToInt32(drpTop.SelectedValue);
        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
        //if(AppUtils.UserName=="admin")
        //{
        //    NLogLogger.Info(new string[] { "date", txtFromDate.Text, fromDate.ToString("dd/MM/yyyy HH:mm:sss") });
        //}    

        //bool erro = false;
        int? status = null;
        if (int.Parse(drpStatus.SelectedValue) > -1)
        {
            status = int.Parse(drpStatus.SelectedValue);
        }


        string cardType = drpCardType.SelectedValue;


        string partnerCodes = drpPartner.SelectedValue;
        string providerCodes = drpProvider.SelectedValue;


        if (!AppUtils.IsAdmin)
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
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 7).ToList(); ;
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }
        
        CardAPILog _CardAPILog = new CardAPILog();
        rptList.DataSource = _CardAPILog.GetTable(top, partnerCodes, fromDate, requestTime, status, cardType, providerCodes,txtOrderNo.Text.Trim());
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string DetailUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.CardAPIMonitorDetail + "?id=" + id;
    }
    protected int Datediff(string LastTime, string CreatTime)
    {
        
        return (DateTime.ParseExact(LastTime, "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture) - DateTime.ParseExact(CreatTime, "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture)).Seconds;
    }
    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public long Amount { get; set; }
        public string Signature { get; set; }

    }
    public async Task<string> CallbackJson(string url, string postData, string code, long Id = 0)
    {
        NLogLogger.Info(new string[] { "NTNet", "Callback Partner", "Request", code, url, postData });

        try
        {
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "NTNet", "Callback Partner", "Response", code, url, postData, responseContent });

                    var log = new LogInfo
                    {
                        LogTime = DateTime.Now,
                        Url = url,
                        TransactionID = Id,
                        Request = postData,
                        Respone = responseContent
                    };
                    LogCache.LogCard(log);
                    return responseContent;
                }
            }
        }
        catch (WebException e)
        {
            var responseStream = e.Response.GetResponseStream();

            if (responseStream != null)
            {
                using (var reader = new StreamReader(responseStream))
                {
                    NLogLogger.Info(new string[] { "MDrum", "Exeption Post", reader.ReadToEnd() });
                    var log1 = new LogInfo
                    {
                        LogTime = DateTime.Now,
                        Url = url,
                        TransactionID = Id,
                        Request = postData,
                        Respone = reader.ReadToEnd()
                    };
                    LogCache.LogCard(log1);
                    //return result;
                }
            }
            NLogLogger.Info(new string[] { "MDrum", "Exeption Post", e.Message });
            var log = new LogInfo
            {
                LogTime = DateTime.Now,
                Url = url,
                TransactionID = Id,
                Request = postData,
                Respone = e.Message
            };
            LogCache.LogCard(log);
            return string.Empty;
        }

        return string.Empty;

    }
}