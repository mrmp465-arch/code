using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;

public partial class Pages_Monitor_CardAPI_Search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPISearch);


        if (!IsPostBack)
        {
            init();
            //GetList();
        }

    }

    private void init()
    {
        txtCreatTime.Text = DateTime.Now.AddDays(1).ToString();
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
            Task.Run(() => CallbackJson(_CardAPILog.CallbackUrl, serializer.Serialize(datacb), _CardAPILog.PartnerCode, 0, 2).ConfigureAwait(false));
        }
        GetList();
    }
    private void GetList()
    {
        string partnerCodes = string.Empty;
        string providerCodes = string.Empty;

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
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID);
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }

        int top = Convert.ToInt32(drpTop.SelectedValue);
        string accountName = txtAccountName.Text.Trim().ToLower();
        string cardSerial = txtCardSerial.Text.Trim().ToLower();
        string cardCode = txtCardCode.Text.Trim().ToLower();
        string refCode = txtRefCode.Text.Trim().ToLower();

        CardAPILog _CardAPILog = new CardAPILog();
        if (txtCreatTime.Text.Trim() == "")
        {
            
            rptList.DataSource = _CardAPILog.Search(partnerCodes, top, accountName, cardSerial,cardCode, refCode, providerCodes);
        }
        else
        {
            DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
            rptList.DataSource = _CardAPILog.Search(partnerCodes, top, accountName, cardSerial, cardCode, refCode, creatTime, providerCodes);
        }
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
    public string CheckPartner(object str)
    {

        //if (!AppUtils.IsAdmin)
        //{
        //    if (str.ToString() != AppUtils.PartnerCode)
        //        return "Đối tác khác";
        //}
        return str.ToString();
    }
    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public long Amount { get; set; }
        public string Signature { get; set; }

    }
    public async Task<string> CallbackJson(string url, string postData, string code, long tranId = 0, int type = 0)
    {
        NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Request", code, tranId.ToString(), url, postData });

        try
        {
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Response", code, tranId.ToString(), url, postData, responseContent });
                    try
                    {
                        if (responseContent.Contains("1|"))
                        {
                            NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Process TRUE", responseContent });
                            if (type == 1)
                            {
                                new TopupMobile3rdLog().UpdateCallback(tranId, 1, null);
                            }
                            else if (type == 2)
                            {
                                new TopupMobile3rdLog().UpdateCallback(tranId, null, 1);
                            }
                        }
                        else
                        {
                            NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Process FAIL", responseContent });
                            if (type == 1)
                            {
                                new TopupMobile3rdLog().UpdateCallback(tranId, -1, null);
                            }
                            else if (type == 2)
                            {
                                new TopupMobile3rdLog().UpdateCallback(tranId, null, -1);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Error", e.Message });
                    }
                    return responseContent;
                }
            }
        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Error", e.Message });
            return string.Empty;
        }

        return string.Empty;

    }
}