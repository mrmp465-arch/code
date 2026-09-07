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
using System.Reflection;
using Telegram.Bot.Types.Payments;

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
        //var datacb = new DataCallback();
        if (!string.IsNullOrEmpty(url))
        {
            var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
            var datacb = new DataCallback()
            {
                Mobile = _CardAPILog.Mobile,
                RefCode = _CardAPILog.RefCode,
                OrderNo = _CardAPILog.FullName,
                OrderInfo = _CardAPILog.OrderInfo,
                Fee=_CardAPILog.Fee,
                Amount = Convert.ToInt32(_CardAPILog.TotalAmount),
                Type = "bank"
            };
            if (_CardAPILog.BankCode.ToLower() == "momo")
            {
                datacb = new DataCallback()
                {
                    Mobile = _CardAPILog.Mobile,
                    RefCode = _CardAPILog.RefCode,
                    OrderNo = _CardAPILog.FullName,
                    OrderInfo = _CardAPILog.OrderInfo,
                    Amount = Convert.ToInt32(_CardAPILog.TotalAmount),
                    Type = "momo"
                };

            }
               
            apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(datacb)
            };
            if (_CardAPILog.TotalAmount <= 0)
                return;
            //if (partner.PartnerID > 1284)
            //{
            //    var datacb2 = new DataCallbackV3();
            //    datacb2.RefCode = datacb.RefCode;
            //    datacb2.Amount = datacb.Amount;
            //    datacb2.Type = datacb.Type;
            //    datacb2.OrderInfo = datacb.OrderInfo;
            //    datacb2.OrderNo = datacb.OrderNo;
            //    datacb2.ResponseCode = apiResponse.ResponseCode;
            //    datacb2.Description = apiResponse.Description;
            //    datacb2.Signature = PaymentUtils.Signature(datacb2.ResponseCode.ToString() + datacb2.Description + datacb2.RefCode, partner.PrivateKey, partner.SignatureType);
            //    Task.Run(() => CallbackJson(url, serializer.Serialize(apiResponse), TransId).ConfigureAwait(false));
            //}
            //else
            //{
                apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, privateKey, 1);
                Task.Run(() => CallbackJson(url, serializer.Serialize(apiResponse), TransId).ConfigureAwait(false));
            //}

        }
        GetList();
    }
    public class DataCallback
    {
        public string RefCode { get; set; }
        public string Mobile { get; set; }
        public string OrderNo { get; set; }
        public string OrderInfo { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public long Fee { get; set; }

    }
    public class DataCallbackV3
    {
        public string RefCode { get; set; }
        public string OrderNo { get; set; }
        public int Amount { get; set; }
        //public string Mobile { get; set; }
        //public string MomoName { get; set; }
        public string OrderInfo { get; set; }
        public string Type { get; set; }

        public int ResponseCode { get; set; }

        public string Description { get; set; }
        public string Signature { get; set; }
        //public string MomoTransId { get; set; }


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
        {

            var listbyUser = new Partners().GetListByUserId(AppUtils.UserID);
            if (listbyUser == null || listbyUser.Count == 0)
            {
                lst = new Partners().GetList();
            }
            else
            {
                lst = listbyUser;
            }
        }
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
            txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-30).ToString("dd/MM/yyyy HH:mm:ss");


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
        string providerCodes = drpProvider.SelectedValue;
        if (AppUtils.IsProvider && !AppUtils.IsAdmin)
        {
            if (string.IsNullOrEmpty(providerCodes))
            {
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(x => x.ProviderCode).ToArray());
                }
            }

        }
        if (AppUtils.IsAdmin)
        {
            var listbyUser = new Partners().GetListByUserId(AppUtils.UserID);
            if (listbyUser == null || listbyUser.Count == 0)
            {
                //lst = new Partners().GetList();
            }
            else
            {
                partnerCodes = string.Join(",", listbyUser.Select(x => x.PartnerCode).ToArray());
            }
        }
        var data = new BankGateAPI().GetTable(100000, partnerCodes, providerCodes, fromDate, requestTime, 1, string.Empty, string.Empty, string.Empty, string.Empty, null, txtBankId.Text,1, cbDay.Checked);
        var lstData = new List<BankGateAPIExcel>();
        foreach (var bank in GlobalHelper.ConvertToList<BankGateAPI>(data))
        {
            var item = new BankGateAPIExcel();
            item.TransactionID = bank.TransactionID;
            item.LastTime = bank.LastTime;
            item.CreatedTime = bank.CreatedTime;
            item.OrderNo = bank.OrderNo;
            item.BankCode = bank.BankCode;
            item.Amount = Convert.ToInt32(bank.TotalAmount);
            item.Fee = bank.Fee;
            item.RefCode = bank.RefCode;
            lstData.Add(item);
        }

        ExportToExcel(lstData, "in-" + partnerCodes.Replace(",", "") + fromDate.ToString("ddMMyyy"));
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
                var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = serializer.Serialize(new DataCallback()
                    {
                        Mobile = bank.Mobile,
                        RefCode = bank.RefCode,
                        OrderNo = bank.FullName,
                        OrderInfo = bank.OrderInfo,
                        Amount = Convert.ToInt32(bank.TotalAmount),
                    })
                };

                apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, privateKey, 1);
                Task.Run(() => CallbackJson(url, serializer.Serialize(apiResponse), bank.TransactionID).ConfigureAwait(false));
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
            if (AppUtils.IsAdmin)
            {
                var listbyUser = new Partners().GetListByUserId(AppUtils.UserID);
                if (listbyUser == null || listbyUser.Count == 0)
                {
                    //lst = new Partners().GetList();
                }
                else
                {
                    partnerCodes = string.Join(",", listbyUser.Select(e => e.PartnerCode).ToArray());
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
                rptList.DataSource = _BankGateAPI.GetTable(top, partnerCodes, providerCodes, fromDate, requestTime, status, bankCode, txtRefCode.Text, txtOrderNo.Text, txtOrderInfo.Text, amount, txtBankId.Text, int.Parse(drpType.SelectedValue), cbDay.Checked);
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
    public string GetStatusExtra(object statusOver)
    {

        if (statusOver.ToString() == "1")
        {
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
        Response.Clear();
        Response.Buffer = true;
        //Response.Charset = "UTF-8"; 
        Response.AppendHeader("Content-Disposition", "attachment;filename=" + name + ".xls");
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
        Response.ContentType = "application/ms-excel";
        EnableViewState = false;
        var myCItrad = new CultureInfo("VI-VN", true);
        var oStringWriter = new StringWriter(myCItrad);
        var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);


        var grid = new DataGrid { DataSource = data };
        grid.DataBind();
        grid.RenderControl(oHtmlTextWriter);

        Response.Write(oStringWriter.ToString());
        Response.Flush();
        Response.End();
    }
    public class BankGateAPIExcel
    {
        public long TransactionID { get; set; }
        public string RefCode { get; set; }
        public string BankCode { get; set; }
        public string OrderNo { get; set; }
        public int Amount { get; set; }
        public long Fee { get; set; }


        public DateTime CreatedTime { get; set; }
        public DateTime LastTime { get; set; }

    }
}