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
using DocumentFormat.OpenXml.Drawing;
using System.IO;
using Libs.CardTelco;
using OfficeOpenXml;


public partial class Pages_Monitor_BankGateAPI_Monitor : System.Web.UI.Page
{
    public string Lang { get; set; }
    public bool RoleFix { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIMonitor);
        RoleFix = AppUtils.CheckRolesPermission(Resources.Url.BankGateAPIFixStatus);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        Lang = Libs.Utils.GlobalHelper.GetLanguage();
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
        BankGateAPI _CardAPILog = new BankGateAPI();
        System.Threading.Thread.Sleep(300);
        //var tranid = AppUtils.ToInt64(txtTransactionID.Text);
        _CardAPILog.TransactionID = TransId;
        _CardAPILog = _CardAPILog.Get(TransId);


        //CallBack Partner
        var partner = new Partners().Get(_CardAPILog.PartnerCode);
        var privateKey = partner.PrivateKey;
        var url = _CardAPILog.ReturnUrl;
        if (string.IsNullOrEmpty(url))
            url = partner.SMSPlusUrl;

        if (!string.IsNullOrEmpty(url))
        {


            var datacb = new DataCallback()
            {
                AccountInfo = _CardAPILog.Mobile,
                RefCode = _CardAPILog.RefCode,
                OrderNo = _CardAPILog.FullName,
                OrderInfo = _CardAPILog.OrderInfo,
                Amount = Convert.ToInt32(_CardAPILog.TotalAmount),

                Type = "bank"
            };
            if (_CardAPILog.BankCode.ToLower() == "momo")
            {
                datacb.Type = "momo";
            }
            var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {

            };
            datacb.ResponseCode = apiResponse.ResponseCode;
            datacb.Description = apiResponse.Description;
            datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode + datacb.Amount, partner.PrivateKey, partner.SignatureType);

            if (_CardAPILog.TotalAmount <= 0)
                return;
            //apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, privateKey, 1);
            //var ignorePartners = new HashSet<string> { "bp7", "b23", "sn1", "tmtm", "lime" };
            //if (_CardAPILog.Amount != _CardAPILog.TotalAmount)
            //{
                
            //    if (!ignorePartners.Contains(_CardAPILog.PartnerCode))
            //    {
            //        return;
            //    }
            //}
            Task.Run(() => CallbackJson(url, serializer.Serialize(datacb), TransId).ConfigureAwait(false));




        }
        GetList();



    }
    public class DataCallback
    {
        public string RefCode { get; set; }
        public string AccountInfo { get; set; }
        public string OrderNo { get; set; }
        public string OrderInfo { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }

        public int ResponseCode { get; set; }

        public string Description { get; set; }

        public string Signature { get; set; }

    }
    public static async Task<string> CallbackJson(string url, string postData, long Id)
    {

        NLogLogger.Info(new string[] { "BankGate Moniter", "Callback", "Partner", "Request", postData });
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
    private void init()
    {

        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);


        btView.Text = Resources.Pay.View;
        btExcel.Text = Resources.Pay.ExportExcel;
        //drpStatus.Items.Insert(0, new ListItem(Resources.Pay.Status, ""));


        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();

        drpPartner.Items.Insert(0, new ListItem(Resources.Pay.Partner, ""));

        drpStatus.Items.Insert(0, new ListItem(Resources.Pay.Status, "-99"));
        drpStatus.Items.Insert(1, new ListItem(Resources.Pay.Success, "1"));
        drpStatus.Items.Insert(2, new ListItem(Resources.Pay.Processing, "0"));
        drpStatus.Items.Insert(3, new ListItem(Resources.Pay.Fail, "-1"));
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

        drpBankCode.DataSource = new BankGateAPI().GetBankCode();
        drpBankCode.DataTextField = "BankCode";
        drpBankCode.DataValueField = "BankCode";
        drpBankCode.DataBind();
        drpBankCode.Items.Insert(0, new ListItem(Resources.Pay.BankCode, ""));

        txtCreatTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1).ToString("dd/MM/yyyy HH:mm:ss");
        //txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy HH:mm:ss");
        txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("dd/MM/yyyy HH:mm:ss");
        if (AppUtils.IsAdmin)
            txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-20).ToString("dd/MM/yyyy HH:mm:ss");


    }
    protected void ExportTran2_Click(object sender, EventArgs e)
    {
        //if (AppUtils.IsAdmin)
        //{

        //    if (string.IsNullOrEmpty(drpPartner.SelectedValue) & string.IsNullOrEmpty(txtBankId.Text))
        //        return;
        //}
        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
        string partnerCodes = drpPartner.SelectedValue;
        if (!AppUtils.IsAdmin)
        {

            if (string.IsNullOrEmpty(partnerCodes))
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(x => x.PartnerCode).ToArray());
                }
            }
        }
        var data = new BankGateAPI().GetTable(100000, partnerCodes, string.Empty, fromDate, requestTime, 1, string.Empty, string.Empty, string.Empty, string.Empty, null, txtBankId.Text);
        var lstData = new List<BankGateAPIExcel>();
        foreach (var bank in GlobalHelper.ConvertToList<BankGateAPI>(data))
        {
            var item = new BankGateAPIExcel();
            item.TransactionID = bank.TransactionID;
            item.LastTime = bank.LastTime.ToString("dd/MM/yyyy HH:mm:ss");
            item.BankAccountNumber = bank.BankAccountNumber;
            item.CreatedTime = bank.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss");
            item.OrderNo = bank.OrderNo;
            item.BankCode = bank.BankCode;
            item.Amount = Convert.ToInt32(bank.TotalAmount);
            item.Fee = bank.Fee;
            item.RefCode = bank.RefCode;
            item.AppUser = bank.Signature;
            lstData.Add(item);
        }

        ExportToExcel(lstData, "recharge-" + partnerCodes.Replace(",", "") + fromDate.ToString("ddMMyyy"));
    }
    protected void Callbackall_Click(object sender, EventArgs e)
    {

        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
        string partnerCodes = drpPartner.SelectedValue;
        if (AppUtils.IsPartner)
        {

            if (string.IsNullOrEmpty(partnerCodes))
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(x => x.PartnerCode).ToArray());
                }
            }
        }
        var data = new BankGateAPI().GetTable(10000, partnerCodes, string.Empty, fromDate, requestTime, 1, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty);

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        foreach (var bank in GlobalHelper.ConvertToList<BankGateAPI>(data))
        {
            //CallBack Partner
            var partner = new Partners().GetCache(bank.PartnerCode);
            var privateKey = partner.PrivateKey;
            var url = bank.ReturnUrl;
            if (string.IsNullOrEmpty(url))
                url = partner.SMSPlusUrl;

            if (!string.IsNullOrEmpty(url))
            {
                if (partner.PartnerCode == "kuipay")
                {
                    var datacb = new DataCallbackV3()
                    {

                        Content = bank.FullName,
                        Amount = Convert.ToInt32(bank.TotalAmount),
                        AccountInfo = bank.Mobile,
                        BankTransId = bank.OrderInfo,
                        TimeBankSuccess = DateTime.Now,
                    };
                    datacb.Signature = PaymentUtils.Signature(datacb.Amount.ToString() + datacb.BankTransId + datacb.AccountInfo + datacb.Content, partner.PrivateKey, partner.SignatureType);
                    Task.Run(() => CallbackJson(url, serializer.Serialize(datacb), bank.TransactionID).ConfigureAwait(false));

                }
                else
                {
                    var datacb = new DataCallback()
                    {
                        //Mobile = _CardAPILog.Mobile,
                        RefCode = bank.RefCode,
                        OrderNo = bank.FullName,
                        OrderInfo = bank.OrderInfo,
                        Amount = Convert.ToInt32(bank.TotalAmount),
                        Type = "bank"
                    };
                    var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(new DataCallback()
                        {
                            //Mobile = bank.Mobile,
                            RefCode = bank.RefCode,
                            OrderNo = bank.FullName,
                            OrderInfo = bank.OrderInfo,
                            Amount = Convert.ToInt32(bank.TotalAmount),
                        })
                    };
                    datacb.ResponseCode = apiResponse.ResponseCode;
                    datacb.Description = apiResponse.Description;
                    datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                    //apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, privateKey, 1);
                    Task.Run(() => CallbackJson(url, serializer.Serialize(datacb), bank.TransactionID).ConfigureAwait(false));
                }

            }
            System.Threading.Thread.Sleep(50);
        }

        GetList();
    }
    protected string FixtUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.BankGateAPIFixStatus + "?id=" + id;
    }
    private void GetList()
    {
        try
        {
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
            long? amount = null;
            if (!string.IsNullOrEmpty(txtAmount.Text))
            {
                amount = long.Parse(txtAmount.Text);
            }


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
                    var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();
                    if (lstProvider != null && lstProvider.Count > 0)
                    {
                        providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                    }
                }

            }
            string bankCode = drpBankCode.SelectedValue;
            BankGateAPI _BankGateAPI = new BankGateAPI();

            long transId = AppUtils.ToInt64(txtTransactionID.Text);

            if (transId > 0)
            {
                rptList.DataSource = _BankGateAPI.Search(transId);

            }
            else
            {
                rptList.DataSource = _BankGateAPI.GetTable(top, partnerCodes, providerCodes, fromDate, requestTime, status, bankCode, txtRefCode.Text, txtOrderNo.Text, txtOrderInfo.Text, amount, txtBankId.Text, int.Parse(drpType.SelectedValue));
            }
            var lstDataBank = new BankGateAPI().ReportDashboard(partnerCodes, fromDate, requestTime, txtBankId.Text, int.Parse(drpType.SelectedValue));
            if (lstDataBank != null)
                lblTotal.Text = String.Format("{0} : {1}/{2} - {3} : {4} - {5} : {6}", Resources.Pay.DepositOrderNumber, lstDataBank.Sum(x => x.TotalTransSuccess).ToString(), lstDataBank.Sum(x => x.TotalTrans).ToString(), Resources.Pay.DepositAmount, lstDataBank.Sum(x => x.TotalAmountSuccess).ToString("N0").Replace(".", ","), Resources.Pay.DepositFee, lstDataBank.Sum(x => x.TotalFee).ToString("N0").Replace(".", ","));
            rptList.DataBind();
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
            Response.Redirect(Constant.ADMIN_PATH + "500.html");
        }
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }
    public string GetSignature(object statusOver)
    {
        var user = statusOver.ToString();
        if (user.Length > 10)
            return "auto";

        return user;

    }
    public string GetOrderNo(object orderNo, object AccountName, object BankCode, object PartnerCode)
    {
        if (PartnerCode.ToString() == "sn2" && BankCode.ToString().ToUpperInvariant() != "MOMO")
        {
            if (orderNo.ToString().Length > 15)
            {
                return AccountName.ToString();
            }
            return orderNo.ToString();

        }

        return orderNo.ToString();
    }
    public string GetStatusExtra(object statusOver, object partnercode, object amountuser, object amount)
    {

        if (statusOver.ToString() == "1")
        {
            if (partnercode.ToString() != "bp7" && partnercode.ToString() != "b23" && partnercode.ToString() != "sn1" && partnercode.ToString() != "tmtm" && partnercode.ToString() != "lime")
            {

                if (amountuser.ToString() != amount.ToString())
                {
                    return "<div class=\"label label-warning\">Sai tiền</div>";
                }
            }
            return "<div class=\"label label-success\">" + Resources.Pay.Success + "</div>";
        }
        if (statusOver.ToString() == "-1")
        {
            return "<div class=\"label label-danger\">" + Resources.Pay.Fail + "</div>";
        }
        if (statusOver.ToString() == "0")
        {
            return "<div class=\"label label-info\">" + Resources.Pay.Processing + "</div>";
        }



        return "<div class=\"label label-info\">" + statusOver + "</div>";
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
    protected void ExportToExcel(List<BankGateAPIExcel> data, string name)
    {
        using (var package = new ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("Data");
            ws.Cells["A1"].LoadFromCollection(data, true);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(package.GetAsByteArray());
            Response.End();
        }
    }

    public class BankGateAPIExcel
    {
        public long TransactionID { get; set; }
        public string RefCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankCode { get; set; }
        public string OrderNo { get; set; }
        public int Amount { get; set; }
        public long Fee { get; set; }


        public string CreatedTime { get; set; }
        public string LastTime { get; set; }
        public string AppUser { get; set; }
        

    }
    public class DataCallbackV3
    {

        public string Content { get; set; }
        public int Amount { get; set; }
        public string AccountInfo { get; set; }
        public string BankTransId { get; set; }
        public string Signature { get; set; }
        public DateTime TimeBankSuccess { get; set; }
    }
    public static async Task<string> CallbackJsonV2(string url, string postData, long Id = 0, string refcode = "", int maxRetry = 3)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        HttpClient client = null;

        // Retry delays: retry #1=30s, retry #2=5m, retry #3=10m
        var retryDelays = new[]
        {
        TimeSpan.FromSeconds(60),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10)
    };

        try
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(90);

            // Tổng số lần gọi = 1 (lần đầu) + maxRetry (số lần retry)
            for (int attempt = 0; attempt <= maxRetry; attempt++)
            {
                try
                {
                    var attemptNo = (attempt + 1).ToString(); // để log dễ đọc (1..)

                    NLogLogger.Info(new[] { "MDrum", "Callback", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, "Url", url });

                    using (var httpContent = new StringContent(postData ?? "", Encoding.UTF8, "application/json"))
                    {
                        var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                        var responseContent = response.Content != null
                            ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                            : string.Empty;

                        LogCache.LogBank(new LogInfo
                        {
                            LogTime = DateTime.Now,
                            Url = url,
                            TransactionID = Id,
                            Request = postData,
                            Respone = "HTTP " + ((int)response.StatusCode) + " " + response.ReasonPhrase + " | " + responseContent
                        });

                        if ((int)response.StatusCode == 200)
                            return responseContent;

                        NLogLogger.Info(new[] { "MDrum", "Callback", "StatusNot200", "Attempt", attemptNo, "Status", ((int)response.StatusCode).ToString(), responseContent });
                    }
                }
                catch (TaskCanceledException ex)
                {
                    var attemptNo = (attempt + 1).ToString();

                    NLogLogger.Info(new[] { "MDrum", "Callback", "Timeout", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, ex.Message });

                    LogCache.LogBank(new LogInfo
                    {
                        LogTime = DateTime.Now,
                        Url = url,
                        TransactionID = Id,
                        Request = postData,
                        Respone = "Timeout: " + ex.ToString()
                    });
                }
                catch (HttpRequestException ex)
                {
                    var attemptNo = (attempt + 1).ToString();

                    NLogLogger.Info(new[] { "MDrum", "Callback", "HttpRequestException", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, ex.Message });

                    LogCache.LogBank(new LogInfo
                    {
                        LogTime = DateTime.Now,
                        Url = url,
                        TransactionID = Id,
                        Request = postData,
                        Respone = "HttpRequestException: " + ex.ToString()
                    });
                }
                catch (Exception ex)
                {
                    var attemptNo = (attempt + 1).ToString();

                    NLogLogger.Info(new[] { "MDrum", "Callback", "Exception", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, ex.Message });

                    LogCache.LogBank(new LogInfo
                    {
                        LogTime = DateTime.Now,
                        Url = url,
                        TransactionID = Id,
                        Request = postData,
                        Respone = "Exception: " + ex.ToString()
                    });

                    return string.Empty; // lỗi không retry tiếp (theo logic cũ của bạn)
                }

                // Nếu đã hết lượt (lần cuối) thì dừng
                if (attempt == maxRetry)
                    break;

                // Delay theo lịch: retry #1=30s, #2=5m, #3=10m
                var delayIndex = attempt; // attempt=0 -> delay[0] (30s), attempt=1 -> delay[1] (5m), attempt=2 -> delay[2] (10m)
                var delay = retryDelays[Math.Min(delayIndex, retryDelays.Length - 1)];

                NLogLogger.Info(new[] { "MDrum", "Callback", "DelayBeforeRetry", delay.ToString(), "AttemptNext", (attempt + 2).ToString(), "Transid", Id.ToString(), "Refcode", refcode });

                await Task.Delay(delay).ConfigureAwait(false);
            }

            return string.Empty;
        }
        finally
        {
            if (client != null)
                client.Dispose();
        }
    }
}