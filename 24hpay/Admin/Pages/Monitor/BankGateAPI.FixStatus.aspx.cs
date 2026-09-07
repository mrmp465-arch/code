using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.CardTelco;
using Libs.Report;
using Libs.Utils;

public partial class Pages_Monitor_BankGateAPI_FixStatus : System.Web.UI.Page
{
    public long MaxAmount { get; set; }

    public bool RoleFixAdmin { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIFixStatus);

        RoleFixAdmin = AppUtils.CheckRolesPermission("pages/monitor/bankgateapi.fixstatusbig.aspx");
        var lstconfig = new SystemDataConfig().GetListCache();
        MaxAmount = long.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("MaxMomoCashAmount")).DataValue);
        if (!IsPostBack)
        {
            var lst = new List<Partners>();
            if (AppUtils.IsAdmin)
                lst = new Partners().GetList();
            else
                lst = new Partners().GetListByUserId(AppUtils.UserID);
            drpPartner.DataSource = lst;
            drpPartner.DataTextField = "Name";
            drpPartner.DataValueField = "PartnerID";
            drpPartner.DataBind();
            drpPartner.Items.Insert(0, new ListItem("Đối tác:", "-1"));
            var id = Request["id"];
            //var refCode = Request["refCode"];
            getOrdernO(id);
        }

    }
    protected void getOrdernO(string id)
    {
        BankGateAPI _BankGateAPI = new BankGateAPI();
        // _BankGateAPI.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);

        _BankGateAPI = _BankGateAPI.Get(long.Parse(id));



        if (_BankGateAPI == null)
        {
            txtStatus.Text = "";
            txtAmount.Text = "";
            txtLastTime.Text = "";
            txtRefCode.Text = "";
            txtOrderNO.Text = "";
            txtBankAccountName.Text = "";
            txtBankAccountNumber.Text = "";
            AlertInfos.Text = "Không tồn tại giao dịch";
            btUpdate.Enabled = false;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        }
        else
        {
            if(_BankGateAPI.Status==1)
            {
                AlertInfos.Text = "Giao dịch đã thành công";
                btUpdate.Enabled = false;
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
                return;
            }
            if (!RoleFixAdmin)
            {
                if (Convert.ToInt32(_BankGateAPI.Amount) >= MaxAmount)
                {
                    AlertInfos.Text = "Bạn chưa có quyền sửa đơn lớn. Vui lòng liên hệ với quản trị để xử lý";
                    btUpdate.Enabled = false;
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
                    return;
                }
            }

            txtStatus.Text = "1";
            txtAmount.Text = Convert.ToInt32(_BankGateAPI.Amount).ToString();
            txtLastTime.Text = _BankGateAPI.LastTime.ToString();
            txtOrderInfo.Text = _BankGateAPI.RefCode.ToString();
            txtRefCode.Text = _BankGateAPI.RefCode.ToString();
            txtOrderNO.Text = _BankGateAPI.OrderNo.ToString();
            txtBankAccountName.Text = _BankGateAPI.BankAccountName.ToString();
            txtBankAccountNumber.Text = _BankGateAPI.BankAccountNumber.ToString();
            txtMobile.Text = _BankGateAPI.Mobile;
            drpPartner.SelectedValue = _BankGateAPI.PartnerID.ToString();
        }
    }    
    protected void btView_Click(object sender, EventArgs e)
    {
       
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        BankGateAPI _BankGateAPI = new BankGateAPI();
        btUpdate.Enabled = false;
        //_BankGateAPI.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);
        var id = Request["id"];
        //var refCode = Request["refCode"];
        _BankGateAPI = _BankGateAPI.Get(long.Parse(id));
        var oldstatus = _BankGateAPI.Status;
        if (_BankGateAPI == null)
        {
            txtStatus.Text = "";
            txtAmount.Text = "";
            txtOrderInfo.Text = "";
            txtRefCode.Text = "";
            txtRefCode.Text = "";
            txtBankAccountName.Text = "";
            txtBankAccountNumber.Text = "";
            AlertInfos.Text = "Không tồn tại giao dịch";
            
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        }
        else
        {
            //if (_BankGateAPI.Status < 1)
            //{
            //    var checkBankgate = new BankGateAPI().GetByOrderInfo(txtOrderInfo.Text);
            //    if (checkBankgate != null)
            //    {
            //        if(checkBankgate.OrderInfo!= _BankGateAPI.OrderInfo)
            //        {
            //            AlertInfos.Text = "OrderInfo đã thành công với nội dung " + checkBankgate.OrderNo;
            //            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            //            return;
            //        }
            //    }    

            //}

            //if (int.Parse(txtAmount.Text) > 20000000)
            //{
            //    AlertInfos.Text = "Số tiền vượt quá quy định";
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            //    return;
            //}
            var partner = new Partners().Get(_BankGateAPI.PartnerCode);

            _BankGateAPI.Status = AppUtils.ToInt32(txtStatus.Text);
            _BankGateAPI.TotalAmount = Convert.ToDecimal(txtAmount.Text);
            _BankGateAPI.LastTime = DateTime.Now;
            _BankGateAPI.Mobile = txtMobile.Text;
            _BankGateAPI.LogContent = txtLogContent.Text;
            _BankGateAPI.BankAccountName = txtBankAccountName.Text;
            _BankGateAPI.OrderInfo = txtOrderInfo.Text;
            _BankGateAPI.LogContent = "update by " + AppUtils.UserName;
            _BankGateAPI.Signature= AppUtils.UserName;
            _BankGateAPI.BankAccountNumber = txtBankAccountNumber.Text;
            var ck = getck(_BankGateAPI.PartnerCode, _BankGateAPI.BankCode);
            var rw = getrw(_BankGateAPI.PartnerCode, _BankGateAPI.BankCode);
            _BankGateAPI.Fee= Convert.ToInt64(_BankGateAPI.TotalAmount * ck / 100);
            _BankGateAPI.Reward = Convert.ToInt64(_BankGateAPI.TotalAmount * rw / 100);
           // var partner = new Partners().GetCache(order.PartnerCode);
            if (!string.IsNullOrEmpty(partner.SMSCommand))
            {
                _BankGateAPI.Fee = int.Parse(partner.SMSCommand);
            }
            //_BankGateAPI.PartnerID = AppUtils.ToInt32(drpPartner.SelectedValue);
            //if (_BankGateAPI.PartnerID > 0)
            //{
            //    _BankGateAPI.PartnerCode = new Partners().Get(_BankGateAPI.PartnerID).PartnerCode;
            //}
            _BankGateAPI.UpdateCMS();
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "bankinupdate",
                ActionName = "Cập nhật lệnh nạp bank",
                Description = "Cập nhật lệnh nạp bank " + _BankGateAPI.Amount.ToString("#,#").Replace(",", ".") + " mã code " + _BankGateAPI.OrderNo + " RefCode " + _BankGateAPI.RefCode
            };
            _userLog.Add();
            TelegramClient.SendTeleV2("-5227754468", "Cập nhật đúng lệnh nạp bank  " + _BankGateAPI.TotalAmount.ToString("#,#").Replace(",", ".") + " mã code " + _BankGateAPI.OrderNo + " RefCode " + _BankGateAPI.RefCode + " Từ tài khoản " + AppUtils.UserName);
            if (_BankGateAPI.Status == 1 && oldstatus < 1)
            {
                //Action<string, long, string, string, string> send = UpdatePartnerBalance;
                //var asynSend = send.BeginInvoke(_BankGateAPI.PartnerCode, Convert.ToInt64(_BankGateAPI.TotalAmount), _BankGateAPI.BankCode, String.Format("Cộng tiền nạp bank số tiền: {3} mgd: {0}-{1}-{2}", _BankGateAPI.TransactionID, _BankGateAPI.BankCode, _BankGateAPI.OrderNo, Convert.ToInt64(_BankGateAPI.TotalAmount).ToString("#,#").Replace(",", ".")), "BankIn_" + _BankGateAPI.TransactionID.ToString(), null, null);

                var Balancedesc = String.Format("Topup to recharge bank amount: {3} transId: {0}-{1}-{2}", _BankGateAPI.TransactionID, _BankGateAPI.BankCode, _BankGateAPI.OrderNo + "-" + _BankGateAPI.RefCode, Convert.ToInt64(_BankGateAPI.TotalAmount).ToString("#,#").Replace(",", "."));
                if (_BankGateAPI.BankCode == "MOMO")
                {
                    Balancedesc = String.Format("Topup to recharge bank amount: {3} transId: {0}-{1}-{2}", _BankGateAPI.TransactionID, _BankGateAPI.BankCode, _BankGateAPI.OrderNo + "-" + _BankGateAPI.RefCode, Convert.ToInt64(_BankGateAPI.TotalAmount).ToString("#,#").Replace(",", "."));
                }
                UpdatePartnerBalance(_BankGateAPI.PartnerCode, Convert.ToInt64(_BankGateAPI.TotalAmount), _BankGateAPI.Fee, Balancedesc, "BankIn_" + _BankGateAPI.TransactionID.ToString());

            }
            //if (_BankGateAPI.Status < 1 && oldstatus== 1)
            //{
            //    Action<string, long, string, string,string> send = UpdatePartnerBalanceDeduct;
            //    var asynSend = send.BeginInvoke(_BankGateAPI.PartnerCode, Convert.ToInt64(_BankGateAPI.Amount), _BankGateAPI.BankCode, String.Format("Trừ tiền nạp bank thất bại số tiền: {3} mgd: {0}-{1}-{2}", _BankGateAPI.TransactionID, _BankGateAPI.BankCode, _BankGateAPI.OrderNo, Convert.ToInt64(_BankGateAPI.Amount).ToString("#,#").Replace(",", ".")), "BankInFail_" + _BankGateAPI.TransactionID.ToString(), null, null);

            //}
            if (_BankGateAPI.Status > 0)
            {


                //CallBack Partner
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var datacb = new DataCallback()
                {
                    //Mobile = _BankGateAPI.Mobile,
                    RefCode = _BankGateAPI.RefCode,
                    OrderNo = _BankGateAPI.FullName,
                    OrderInfo = _BankGateAPI.OrderInfo,
                    Amount = Convert.ToInt32(_BankGateAPI.TotalAmount),
                    AccountInfo=txtAccountInfo.Text,
                    Type = "bank"
                };
                if(_BankGateAPI.BankCode.ToLower()=="momo")
                {
                    datacb.Type = "momo";
                }
                var checkOrder = new CheckOrder
                {
                    LasTime = DateTime.Now,
                    RefCode = _BankGateAPI.RefCode,
                    Amount = Convert.ToInt32(_BankGateAPI.Amount),
                    TransactionID = _BankGateAPI.TransactionID.ToString()
                };
                DataCaching.SetCache("CheckOrder:" + _BankGateAPI.PartnerCode + _BankGateAPI.RefCode, checkOrder, 900);

                var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                {

                };
                var url = _BankGateAPI.ReturnUrl;

                datacb.ResponseCode = apiResponse.ResponseCode;
                datacb.Description = apiResponse.Description;
                datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                //apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                NLogLogger.Info(new string[] { "Momo", "PartnerCallback", url, serializer.Serialize(datacb) });
                if (!string.IsNullOrEmpty(url))
                {

                    Task.Run(async () => await CallbackJson(url, serializer.Serialize(datacb), _BankGateAPI.TransactionID).ConfigureAwait(false));
                }
            }
            AlertSuccesss.Text = "Cập nhập thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
            System.Threading.Thread.Sleep(200);
            Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankGateAPIMonitor);
        }

    }
    public class CheckOrder
    {

        public int Amount { get; set; }
        public string RefCode { get; set; }
        public string TransactionID { get; set; }
        public DateTime LasTime { get; set; }
    }
    private decimal getck(string PartnerCode, string Type)
    {
        decimal ck = 0;

        var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
        if (listpartnerDiscount == null)
        {
            //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }

        if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
        {
            //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }
        var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
        ck = _partnerDiscount.DiscountBANKTRANFER;
        if (Type == "MOMO")
            ck = _partnerDiscount.DiscountMOMO;


        return ck;
    }
    private decimal getrw(string PartnerCode, string Type)
    {
        decimal ck = 0;

        var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
        if (listpartnerDiscount == null)
        {
            //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }

        if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
        {
            //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }
        var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
        ck = _partnerDiscount.RewardBANKTRANFER;
        if (Type == "MOMO")
            ck = _partnerDiscount.RewardMOMO;


        return ck;
    }
    private void UpdatePartnerBalance(string PartnerCode, long Amount, long fee, string TranId, string RefCode)
    {
        try
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), fee.ToString(), TranId.ToString(), RefCode });
            //var partner = new Partners().GetCache(PartnerCode);
            //if (string.IsNullOrEmpty(partner.Hotline))
            //{
            //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
            //    return;
            //}
            //var user = new Users().GetByUserName(partner.Hotline.Trim());
            //if (user == null)
            //{
            //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
            //    return;
            //}
            if (fee == 0)
            {
                //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return;
            }

            long realAmount = Amount - fee;
            // NLogLogger.Info(new string[] { "Bank Topup", realAmount.ToString(), ck.ToString() });
            new Users().Topup(realAmount, PartnerCode, PartnerCode, TranId, RefCode);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
        }


    }
   
    public class DataCallback
    {
        public string RefCode { get; set; }
        public string Type { get; set; }
        public string OrderNo { get; set; }
        public string OrderInfo { get; set; }
        public string AccountInfo { get; set; }
        public int Amount { get; set; }

        public int ResponseCode { get; set; }

        public string Description { get; set; }

        public string Signature { get; set; }

    }
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
}