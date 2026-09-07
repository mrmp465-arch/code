using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

using Libs.API;
using Libs.CardTelco;
using Libs.Report;
using Libs.Utils;
using OfficeOpenXml;

public partial class Pages_Monitor_BankCash_Monitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankCashMonitor);

        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        if (!IsPostBack)
        {
            init();
            GetList();
        }
    }

    private void init()
    {
        btView.Text = Resources.Pay.View;
        btExcel.Text = Resources.Pay.ExportExcel;
        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);

        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        if (AppUtils.UserName == "sn1support")
        {
            lst = new List<Partners>();
            lst.Add(new Partners { Name = "sn1", PartnerCode = "sn1" });
        }
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem(Resources.Pay.Partner, ""));

        drpStatus.Items.Insert(0, new ListItem(Resources.Pay.Status, "-999"));
        drpStatus.Items.Insert(1, new ListItem(Resources.Pay.Success, "1"));
        drpStatus.Items.Insert(2, new ListItem(Resources.Pay.WaitApproval, "-2"));
        drpStatus.Items.Insert(2, new ListItem(Resources.Pay.Suspect, "-3"));
        drpStatus.Items.Insert(2, new ListItem(Resources.Pay.Processing, "0"));
        drpStatus.Items.Insert(3, new ListItem(Resources.Pay.Fail, "-1"));
        drpStatus.Items.Insert(3, new ListItem(Resources.Pay.Pedding, "-4"));
        drpStatus.Items.Insert(4, new ListItem(Resources.Pay.WrongAccount, "-357"));
       
        var lstProvider = new List<Providers>();
        if (AppUtils.IsAdmin)
            lstProvider = new Providers().GetList(18);
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 13).ToList();

        drpProvider.DataSource = lstProvider;
        drpProvider.DataTextField = "Name";
        drpProvider.DataValueField = "ProviderCode";
        drpProvider.DataBind();
        drpProvider.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));

        //drpBankCode.DataSource = new BankCashAPI().GetBankCode();
        //drpBankCode.DataTextField = "BankCode";
        //drpBankCode.DataValueField = "BankCode";
        //drpBankCode.DataBind();
        //drpBankCode.Items.Insert(0, new ListItem("BankCode:", ""));

        txtCreatTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1).ToString("dd/MM/yyyy HH:mm:ss");
        //txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy HH:mm:ss");
        txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("dd/MM/yyyy HH:mm:ss");
        if (AppUtils.IsAdmin)
            txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-10).ToString("dd/MM/yyyy HH:mm:ss");

    }

    private void GetList()
    {
        int top = Convert.ToInt32(drpTop.SelectedValue);

        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
        //requestTime = requestTime.AddDays(1).AddMilliseconds(-1);


        int? status = null;
        if (int.Parse(drpStatus.SelectedValue) > -999)
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
                if (AppUtils.UserName == "sn1support")
                {
                    lstPartner = new List<Partners>();
                    lstPartner.Add(new Partners { Name = "sn1", PartnerCode = "sn1" });
                }
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                }
            }


        }

        if (AppUtils.IsAdmin)
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
        string bankCode = txtBankCode.Text.Trim();
        BankCashAPI _BankCash = new BankCashAPI();
        long transId = AppUtils.ToInt64(txtTransactionID.Text);
        if (transId > 0)
        {
            rptList.DataSource = _BankCash.Search(transId);
        }
        else
        {
            rptList.DataSource = _BankCash.GetTable(top, partnerCodes, txtMobile.Text, txtMobile2.Text, fromDate, requestTime, status, bankCode, txtRefCode.Text, amount, providerCodes);
        }

        rptList.DataBind();
        var lstDataBank = new BankCashAPI().ReportDashboardV2(partnerCodes, fromDate, requestTime, providerCodes);
        if (lstDataBank != null)
            lblTotal.Text = String.Format("{0} : {1}/{2} - {3} : {4} - {5} : {6}", Resources.Pay.CashOrderNumber, lstDataBank.Sum(x => x.TotalTransSuccess).ToString(), lstDataBank.Sum(x => x.TotalTrans).ToString(), Resources.Pay.CashAmount, lstDataBank.Sum(x => x.TotalAmountSuccess).ToString("N0").Replace(".", ","), Resources.Pay.CashFee, lstDataBank.Sum(x => x.TotalFee).ToString("N0").Replace(".", ","));
        if (AppUtils.IsAdmin)
        {
            lblTotal.Text = String.Format("{0} : {1}/{2} - {3} : {4} - {5} : {6} - Số lệnh đang xử lý :{7}", Resources.Pay.CashOrderNumber, lstDataBank.Sum(x => x.TotalTransSuccess).ToString(), lstDataBank.Sum(x => x.TotalTrans).ToString(), Resources.Pay.CashAmount, lstDataBank.Sum(x => x.TotalAmountSuccess).ToString("N0").Replace(".", ","), Resources.Pay.CashFee, lstDataBank.Sum(x => x.TotalFee).ToString("N0").Replace(".", ","), lstDataBank.Sum(x => x.TotalTransProcess.GetValueOrDefault()).ToString("N0").Replace(".", ","));
        }
    
    }


    //cập nhật đúng
    protected void Callback_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        long TransId = Convert.ToInt64(e.CommandArgument.ToString());
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        BankCashAPI _CardAPILog = new BankCashAPI();

        _CardAPILog.TransactionID = TransId;
        _CardAPILog = _CardAPILog.Get(TransId);
        if (((_CardAPILog.Status == 0 || _CardAPILog.Status == -3)) || _CardAPILog.Status == -4 || _CardAPILog.Status == -1)
        {
            _CardAPILog.Status = 1;
            _CardAPILog.TotalAmount = _CardAPILog.Amount;
            _CardAPILog.LastTime = DateTime.Now;
            _CardAPILog.LogContent = " cập nhật đúng bởi " + AppUtils.UserName;
            _CardAPILog.ApproveUser = AppUtils.UserName;

            var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
            var rw = getrw(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
            var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * ck / 100);
            _CardAPILog.Fee = Fee;
            _CardAPILog.Reward = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * (rw) / 100);
            _CardAPILog.Update();
            //TelegramClient.SendWarning("-4278595388", "Cập nhật đúng lệnh xuất khoản  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode + " Từ tài khoản " + AppUtils.UserName);
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "bankoutapp",
                ActionName = "Duyệt tay xuất khoản",
                Description = "Cập nhật đúng xuất khoản" + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode
            };
            _userLog.Add();
            //CallBack Partner
            var partner = new Partners().Get(_CardAPILog.PartnerCode);
            var privateKey = partner.PrivateKey;
            if (!string.IsNullOrEmpty(_CardAPILog.ReturnUrl))
            {
                var datacb = new DataCallback()
                {
                    Amount = Convert.ToInt32(_CardAPILog.Amount),
                    RefCode = _CardAPILog.RefCode,
                    OrderInfo = _CardAPILog.TransactionID.ToString(),
                    TransactionID = _CardAPILog.TransactionID.ToString(),
                    Type = "bankout"
                };
                if (_CardAPILog.BankCode == "MOMO")
                    datacb.Type = "momoout";
                var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                {

                };
                apiResponse.ResponseContent = serializer.Serialize(datacb);
                apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                datacb.ResponseCode = apiResponse.ResponseCode;
                datacb.Description = apiResponse.Description;
                datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode, partner.PrivateKey, partner.SignatureType);

                Task.Run(async () => await CallbackJson(_CardAPILog.ReturnUrl, serializer.Serialize(datacb), _CardAPILog.TransactionID).ConfigureAwait(false));
            }
            AlertSuccesss.Text = "Cập nhật thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);
            System.Threading.Thread.Sleep(8000);
            Response.Redirect(Request.RawUrl);
        }

    }
    private decimal getck(string PartnerCode, string Type)
    {
        decimal ck = 1;
        var partner = new Partners().GetCache(PartnerCode);
        var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
        if (listpartnerDiscount == null)
        {
            //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }

        if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
        {
            //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }
        var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
        ck = _partnerDiscount.DiscountBANKOUTTRANFER;
        if (Type == "MOMO")
            ck = _partnerDiscount.DiscountMOMOOUT;
        return ck;
    }
    private decimal getrw(string PartnerCode, string Type)
    {
        decimal ck = 0;
        ///var partner = new Partners().GetCache(PartnerCode);
        var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
        if (listpartnerDiscount == null)
        {
            //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }

        if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
        {
            //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }
        var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
        ck = _partnerDiscount.RewardBANKOUTTRANFER;
        if (Type == "MOMO")
            ck = _partnerDiscount.RewardMOMOOUT;
        return ck;
    }
    private void UpdatePartnerBalance(string PartnerCode, long Amount, long Fee, string TranId, string RefCode)
    {
        try
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Fee.ToString(), TranId.ToString(), RefCode });
            var partner = new Partners().GetCache(PartnerCode);
            if (string.IsNullOrEmpty(partner.Hotline))
            {
                //TelegramNotify.SendTeleV2("-1003939306637", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                return;
            }
            var user = new Users().GetByUserName(partner.Hotline.Trim());
            if (user == null)
            {
                //TelegramNotify.SendTeleV2("-1003939306637", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                return;
            }


            long realAmount = Convert.ToInt64(Amount) + Fee;
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Users().Deduct(realAmount, user.UserName, PartnerCode, TranId, RefCode);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
        }


    }
    private void UpdatePartnerBalanceTopup(string PartnerCode, long Amount, long Fee, string TranId, string RefCode)
    {
        try
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Fee.ToString(), TranId.ToString(), RefCode });
            var partner = new Partners().GetCache(PartnerCode);
            if (string.IsNullOrEmpty(partner.Hotline))
            {
                //TelegramNotify.SendTeleV2("-1003939306637", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                return;
            }
            var user = new Users().GetByUserName(partner.Hotline.Trim());
            if (user == null)
            {
                //TelegramNotify.SendTeleV2("-1003939306637", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                return;
            }

            long realAmount = Convert.ToInt64(Amount) + Fee;
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Users().Topup(realAmount, user.UserName, PartnerCode, TranId, RefCode);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
        }


    }
    //cập nhật sai


    public static async Task<string> CallbackJson(string url, string postData, long Id)
    {

        NLogLogger.Info(new string[] { "MDrum", "Callback", "Partner", "Request", postData });
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
                client.Dispose();
                var log = new LogInfo
                {
                    LogTime = DateTime.Now,
                    Url = url,
                    TransactionID = Id,
                    Request = postData,
                    Respone = responseContent
                };
                LogCache.LogBankCash(log);
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
            LogCache.LogBankCash(log);
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }
    public class DataCallback
    {
        public string RefCode { get; set; }
        //public int Status { get; set; }
        public string TransactionID { get; set; }
        public int Amount { get; set; }
        //public string Signature { get; set; }
        public string Type { get; set; }
        public string OrderInfo { get; set; }

        public int ResponseCode { get; set; }

        public string Description { get; set; }

        public string Signature { get; set; }
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }
    public string GetStatusExtra(object statusOver, bool isAdmin)
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
        if (!isAdmin)
        {
            if (statusOver.ToString() == "-2" | statusOver.ToString() == "-3" || statusOver.ToString() == "-3")
            {
                return "<div class=\"label label-success\">" + Resources.Pay.Processing + "</div>";
            }
        }
        if (statusOver.ToString() == "-357")
        {
            return "<div class=\"label label-danger\">" + Resources.Pay.WrongAccount + " </ div > ";
        }
        if (statusOver.ToString() == "-2")
        {
            return "<div class=\"label label-primary\">" + Resources.Pay.WaitApproval + " </ div > ";
        }
        if (statusOver.ToString() == "-3")
        {
            return "<div class=\"label label-warning\">" + Resources.Pay.Suspect + " </ div > ";
        }
        if (statusOver.ToString() == "-4")
        {
            return "<div class=\"label label-default\">" + Resources.Pay.Pedding + " </ div > ";
        }

        return "<div class=\"label label-info\">" + statusOver + "</div>";
    }
    public class CashBankRequestV2
    {
        public string TransId { get; set; } //Transaction cua he thông Pay
        public string BankTransId { get; set; }
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; } // 
        public int Amount { get; set; }
        public string Comment { get; set; }
        public string CallbackUrl { get; set; }
    }
    public class CashResponeV2
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }

    }
    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string ServiceCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
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
    private const string urlBaseService2 = "http://127.0.0.1:9002/BankService.ashx";
    private const string callbackurl2 = "http://127.0.0.1:1592/Callback/DrumBankCash.ashx";

    //protected void ExportToExcel(List<BankGateAPIExcel> data, string name)
    //{
    //    Response.Clear();
    //    Response.Buffer = true;
    //    //Response.Charset = "UTF-8"; 
    //    Response.AppendHeader("Content-Disposition", "attachment;filename=" + name + ".xls");
    //    Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
    //    Response.ContentType = "application/ms-excel";
    //    EnableViewState = false;
    //    var myCItrad = new CultureInfo("VI-VN", true);
    //    var oStringWriter = new StringWriter(myCItrad);
    //    var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);


    //    var grid = new DataGrid { DataSource = data };
    //    grid.DataBind();
    //    grid.RenderControl(oHtmlTextWriter);

    //    Response.Write(oStringWriter.ToString());
    //    Response.Flush();
    //    Response.End();
    //}
    //protected void ExportToExcel(List<BankGateAPIExcel> data, string name)
    //{
    //    Response.Clear();
    //    Response.Buffer = true;

    //    string fileName = HttpUtility.UrlEncode(name, Encoding.UTF8) + ".xls";
    //    Response.AppendHeader("Content-Disposition", "attachment;filename=" + name + ".xls");
    //    Response.ContentEncoding = Encoding.UTF8;
    //    Response.ContentType = "application/vnd.ms-excel";
    //    Response.Charset = "UTF-8";

    //    // Ghi BOM để Excel nhận dạng UTF-8
    //    Response.BinaryWrite(Encoding.UTF8.GetPreamble());

    //    var myCItrad = new CultureInfo("vi-VN", true);
    //    using (var sw = new StringWriter(myCItrad))
    //    using (var htw = new HtmlTextWriter(sw))
    //    {
    //        var grid = new DataGrid
    //        {
    //            DataSource = data
    //        };
    //        grid.DataBind();
    //        grid.RenderControl(htw);

    //        Response.Write(sw.ToString());
    //    }

    //    Response.Flush();
    //    Http
    //    Context.Current.ApplicationInstance.CompleteRequest();
    //}
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
        public string ReceiveAccount { get; set; }
        public string TranferAccount { get; set; }
        public int Amount { get; set; }
        public long Fee { get; set; }
        public string CreatedTime { get; set; }
        public string LastTime { get; set; }
        public string OrderInfo { get; set; }
        public int Status { get; set; }
        public string ProviderCode { get; set; }
        public string ApproveUser { get; set; }

    }
    protected void ExportTran2_Click(object sender, EventArgs e)
    {
        //if (AppUtils.IsAdmin)
        //{

        //    if (string.IsNullOrEmpty(drpPartner.SelectedValue) && string.IsNullOrEmpty(txtMobile.Text))
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
        string providerCodes = drpProvider.SelectedValue;
        var data = new BankCashAPI().GetTable(200000, partnerCodes, txtMobile.Text, txtMobile2.Text, fromDate, requestTime, null, string.Empty, string.Empty,null, providerCodes);
        var lstData = new List<BankGateAPIExcel>();
        if (AppUtils.IsAdmin)
        {
            foreach (var bank in GlobalHelper.ConvertToList<BankCashAPI>(data))
            {
                var item = new BankGateAPIExcel();
                item.TransactionID = bank.TransactionID;
                item.CreatedTime = bank.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss");
                item.LastTime = bank.LastTime.ToString("dd/MM/yyyy HH:mm:ss");
                item.ReceiveAccount = string.Format("{0}-{1}-{2}-{3}", bank.BankCode, bank.BankAccountNumber, bank.BankAccountName, bank.Note);
                item.TranferAccount = bank.Mobile;
                item.Amount = Convert.ToInt32(bank.TotalAmount);
                item.Fee = bank.Fee;
                item.RefCode = bank.RefCode;
                item.Status = bank.Status;
                item.OrderInfo = bank.OrderInfo;
                item.ProviderCode = bank.ProviderCode;
                item.ApproveUser = bank.ApproveUser;
                lstData.Add(item);
            }
        }
        else
        {
            foreach (var bank in GlobalHelper.ConvertToList<BankCashAPI>(data))
            {
                var item = new BankGateAPIExcel();
                item.TransactionID = bank.TransactionID;
                item.CreatedTime = bank.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss");
                item.LastTime = bank.LastTime.ToString("dd/MM/yyyy HH:mm:ss");
                item.ReceiveAccount = string.Format("{0}-{1}-{2}-{3}", bank.BankCode, bank.BankAccountNumber, bank.BankAccountName, bank.Note);
                item.TranferAccount = bank.Mobile;
                item.Amount = Convert.ToInt32(bank.TotalAmount);
                item.Fee = bank.Fee;
                item.RefCode = bank.RefCode;
                item.Status = bank.Status;

                lstData.Add(item);
            }
        }


        ExportToExcel(lstData, "withdrawal-" + partnerCodes.Replace(",", "") + fromDate.ToString("ddMMyyy"));
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
        var data = new BankCashAPI().GetTable(10000, partnerCodes, string.Empty, string.Empty, fromDate, requestTime, -1, string.Empty, string.Empty, null);
        var lstData = new List<BankGateAPIExcel>();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        foreach (var bank in GlobalHelper.ConvertToList<BankCashAPI>(data))
        {
            if (bank.Status == 1 || bank.Status == -1)
            {
                //CallBack Partner
                var partner = new Partners().GetCache(bank.PartnerCode);
                var privateKey = partner.PrivateKey;
                var url = bank.ReturnUrl;
                if (string.IsNullOrEmpty(url))
                    url = partner.SMSPlusUrl;
                if (!string.IsNullOrEmpty(url))
                {
                    var datacb = new DataCallback()
                    {
                        Amount = Convert.ToInt32(bank.Amount),
                        RefCode = bank.RefCode,
                        TransactionID = bank.TransactionID.ToString(),

                    };
                    var apiResponse = new APIResponse(bank.Status)
                    {

                    };
                    apiResponse.ResponseContent = serializer.Serialize(datacb);
                    apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);

                    //apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, privateKey, 1);
                    Task.Run(() => CallbackJson(url, serializer.Serialize(apiResponse), bank.TransactionID).ConfigureAwait(false));
                }
                System.Threading.Thread.Sleep(50);
            }

        }
        GetList();

    }
    protected void btApp2Click(object sender, EventArgs e)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _Partner = new Partners();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label TransactionID = (Label)rptList.Items[i].FindControl("ID");
            if (cbx.Checked)
            {
                long TransId = Convert.ToInt64(TransactionID.Text);

                BankCashAPI _CardAPILog = new BankCashAPI();


                _CardAPILog.TransactionID = TransId;
                _CardAPILog = _CardAPILog.Get(TransId);

                if (((_CardAPILog.Status == 0 || _CardAPILog.Status == -3)) || _CardAPILog.Status == -4)
                {
                    _CardAPILog.Status = 1;
                    _CardAPILog.TotalAmount = _CardAPILog.Amount;
                    _CardAPILog.LastTime = DateTime.Now;
                    _CardAPILog.LogContent = " cập nhật đúng bởi " + AppUtils.UserName;
                    _CardAPILog.ApproveUser = AppUtils.UserName;

                    var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                    var rw = getrw(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                    var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * ck / 100);
                    _CardAPILog.Fee = Fee;
                    _CardAPILog.Reward = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * (rw) / 100);
                    _CardAPILog.Update();
                    //TelegramClient.SendWarning("-4278595388", "Cập nhật đúng lệnh xuất khoản  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode + " Từ tài khoản " + AppUtils.UserName);
                    var _userLog = new UserLog
                    {
                        UserName = AppUtils.UserName,
                        Action = "bankoutapp",
                        ActionName = "Duyệt tay xuất khoản",
                        Description = "Cập nhật đúng xuất khoản" + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode
                    };
                    _userLog.Add();
                    //CallBack Partner
                    var partner = new Partners().Get(_CardAPILog.PartnerCode);
                    var privateKey = partner.PrivateKey;
                    if (!string.IsNullOrEmpty(_CardAPILog.ReturnUrl))
                    {
                        var datacb = new DataCallback()
                        {
                            Amount = Convert.ToInt32(_CardAPILog.Amount),
                            RefCode = _CardAPILog.RefCode,
                            OrderInfo = _CardAPILog.TransactionID.ToString(),
                            TransactionID = _CardAPILog.TransactionID.ToString(),
                            Type = "bankout"
                        };
                        if (_CardAPILog.BankCode == "MOMO")
                            datacb.Type = "momoout";
                        var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {

                        };
                        apiResponse.ResponseContent = serializer.Serialize(datacb);
                        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        datacb.ResponseCode = apiResponse.ResponseCode;
                        datacb.Description = apiResponse.Description;
                        datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode, partner.PrivateKey, partner.SignatureType);

                        Task.Run(async () => await CallbackJson(_CardAPILog.ReturnUrl, serializer.Serialize(datacb), _CardAPILog.TransactionID).ConfigureAwait(false));
                    }
                    //AlertSuccesss.Text = "Cập nhật thành công";
                    //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);
                    //System.Threading.Thread.Sleep(8000);
                    //Response.Redirect(Request.RawUrl);
                }
            }
        }
        AlertSuccesss.Text = "Cập nhập đơn đúng thành công";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);


        System.Threading.Thread.Sleep(5000);
        Response.Redirect(Request.RawUrl);
    }

    //hủy
    protected void btCancelClick(object sender, EventArgs e)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _Partner = new Partners();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label TransactionID = (Label)rptList.Items[i].FindControl("ID");
            if (cbx.Checked)
            {
                long TransId = Convert.ToInt64(TransactionID.Text);

                BankCashAPI _CardAPILog = new BankCashAPI();


                _CardAPILog.TransactionID = TransId;
                _CardAPILog = _CardAPILog.Get(TransId);
                if (((_CardAPILog.Status == 0 || _CardAPILog.Status == -3)) || _CardAPILog.Status == -4)
                {
                    if (_CardAPILog.Status == 0)
                    {
                        var refcode = _CardAPILog.TransactionID.ToString();
                        int count = Regex.Matches(_CardAPILog.LogContent.ToLower(), "chờ").Count;
                        if (count == 1)
                        {
                            refcode += "_recall";
                        }
                        if (count == 2)
                        {
                            refcode += "_re2call";
                        }
                        var UrlBaseService = "http://127.0.0.1:9002/BankService.ashx";

                        var requesData = new RequestData()
                        {
                            CommandCode = "TRANS_OUT_CANCEL",
                            RequestContent = refcode,
                        };
                        NLogLogger.Info(new string[] { "MDrum", " Request",serializer.Serialize(requesData), UrlBaseService
                            });
                        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData), 0)).Result;
                        NLogLogger.Info(new string[] { "MDrum", " Response", res
                        });
                        var resObj = serializer.Deserialize<APIRespone>(res);
                        //-1 là không huỷ đc
                        if (resObj.ResponseCode == -1)
                        {

                            continue;
                        }
                    }

                    var oldstatus = _CardAPILog.Status;
                    _CardAPILog.Status = -1;
                    _CardAPILog.TotalAmount = 0;
                    _CardAPILog.LastTime = DateTime.Now;
                    _CardAPILog.LogContent = " hủy bởi " + AppUtils.UserName;
                    _CardAPILog.Fee = 0;
                    _CardAPILog.ApproveUser = AppUtils.UserName;
                    //_CardAPILog.Update();
                    var resultupdate = _CardAPILog.UpdateApp2();

                    if (resultupdate < 0)
                    {
                        continue;
                    }

                    var _userLog = new UserLog
                    {
                        UserName = AppUtils.UserName,
                        Action = "bankoutcancel",
                        ActionName = "Hủy xuất khoản",
                        Description = "Hủy xuất khoản" + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode
                    };
                    _userLog.Add();
                    //TelegramClient.SendWarning("-4278595388", "Hủy lệnh xuất khoản  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode + " Từ tài khoản " + AppUtils.UserName);
                    var partner = new Partners().GetCache(_CardAPILog.PartnerCode);

                    //CallBack Partner
                    var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                    var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * ck / 100);
                    var des = String.Format("Refund bank withdrawal amount: {2} transId: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", "."));

                    if (_CardAPILog.PartnerCode == "cn02")
                    {
                        des = String.Format("Refund withdrawal amount: {2} transId: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", "."));
                    }
                    UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, des, "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                    //var partner = new Partners().Get(_CardAPILog.PartnerCode);
                    var privateKey = partner.PrivateKey;
                    if (!string.IsNullOrEmpty(_CardAPILog.ReturnUrl))
                    {
                        var datacb = new DataCallback()
                        {
                            Amount = 0,
                            RefCode = _CardAPILog.RefCode,
                            TransactionID = _CardAPILog.TransactionID.ToString(),
                            OrderInfo = _CardAPILog.TransactionID.ToString(),
                            Type = "bankout"
                        };

                        if (_CardAPILog.BankCode == "MOMO")
                            datacb.Type = "momoout";
                        var apiResponse = new APIResponse((int)ResponseCode.TransactionFailed)
                        {

                        };
                        apiResponse.ResponseContent = serializer.Serialize(datacb);
                        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        datacb.ResponseCode = apiResponse.ResponseCode;
                        datacb.Description = apiResponse.Description;
                        datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode, partner.PrivateKey, partner.SignatureType);

                        Task.Run(async () => await CallbackJson(_CardAPILog.ReturnUrl, serializer.Serialize(datacb), _CardAPILog.TransactionID).ConfigureAwait(false));
                    }
                }

            }

        }

        AlertSuccesss.Text = "Hủy đơn thành công";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);


        System.Threading.Thread.Sleep(5000);
        Response.Redirect(Request.RawUrl);
    }
    public class APIRespone
    {
        public int ResponseCode { get; set; }
        //public string Description { get; set; }
        //public string ResponseContent { get; set; }
        //public string Signature { get; set; }
    }
    public string GetAmountStyle(object log)
    {
        var amount = Convert.ToInt64(log);
        if (amount >= 20000000)
            return "style='color:red;font-weight:bold'";
        return "";


    }
}