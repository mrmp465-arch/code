using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Libs.API;
using Libs.CardTelco;
using Libs.Report;
using Libs.Utils;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using RestSharp.Serializers;

public partial class Pages_Monitor_BankCash_App : System.Web.UI.Page
{
    public bool RoleAppAuto { get; set; }
   
    private const string urlBaseServiceGetAccount = "http://45.32.115.186:1592/ServiceHandler/GetAcountInfo.ashx";
  
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankCashApp);
        //RoleAppAuto = AppUtils.CheckRolesPermission(Resources.Url.BankCashAppAuto);
        Page.Culture = "vi-vn";
        Page.UICulture = "vi-vn";
        if (!IsPostBack)
        {
            init();
            GetList();
        }
    }
    public class TransferRespone
    {
        public string requestId { get; set; }
        public Transfer transfer { get; set; }

    }

    public class Transfer
    {
        public string id { get; set; }
        public string state { get; set; }
        public string fromAccountNumber { get; set; }
        public string toAccountNumber { get; set; }
        public string toBin { get; set; }
        public int totalAmount { get; set; }
        public string description { get; set; }
        public string transactionDateTime { get; set; }
        public string refId { get; set; }
    }
    public class CashRespone
    {
        public string requestId { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string errorType { get; set; }
    }
    public class CasBankOut
    {
        public int amount { get; set; }
        public string fromAccountNumber { get; set; }
        public string toBin { get; set; }
        public string toAccountNumber { get; set; }
        public string description { get; set; }
    }
    public class Account
    {
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public int balance { get; set; }
        public string currency { get; set; }
    }



    public class CaspayAccountRespone
    {
        public string requestId { get; set; }
        public List<Account> accounts { get; set; }

    }

    private void init()
    {
        //btView.Text = Resources.Pay.View;
        //btExcel.Text = Resources.Pay.ExportExcel;
        btApp.Visible = true;
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

        //else
        //    lst = new Partners().GetListByUserId(AppUtils.UserID);

        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác", ""));



        //drpBankCode.DataSource = new BankCashAPI().GetBankCode();
        //drpBankCode.DataTextField = "BankCode";
        //drpBankCode.DataValueField = "BankCode";
        //drpBankCode.DataBind();
        //drpBankCode.Items.Insert(0, new ListItem("BankCode:", ""));

        txtCreatTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1).ToString("dd/MM/yyyy HH:mm:ss");
        //txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy HH:mm:ss");
        txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-1).ToString("dd/MM/yyyy HH:mm:ss");
    }

    private void GetList()
    {
        int top = Convert.ToInt32(drpTop.SelectedValue);

        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
        //requestTime = requestTime.AddDays(1).AddMilliseconds(-1);


        int? status = -2;


        string partnerCodes = drpPartner.SelectedValue;
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


        string bankCode = string.Empty;
        BankCashAPI _BankCash = new BankCashAPI();
        long transId = AppUtils.ToInt64(txtTransactionID.Text);
        if (transId > 0)
        {
            rptList.DataSource = _BankCash.Search(transId);
        }
        else
        {
            rptList.DataSource = _BankCash.GetTableV2(top, partnerCodes, string.Empty, fromDate, requestTime, status, bankCode, txtRefCode.Text);
        }

        rptList.DataBind();


    }


    private decimal getck(string PartnerCode, string Type)
    {
        decimal ck = 1;
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
    private int UpdatePartnerBalance(string PartnerCode, long Amount, long Fee, string Note, string RefCode)
    {
        try
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Fee.ToString(), Note.ToString(), RefCode });
            //var partner = new Partners().GetCache(PartnerCode);
            //if (string.IsNullOrEmpty(partner.Hotline))
            //{
            //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
            //    return 0;
            //}
            //var user = new Users().GetByUserName(partner.Hotline.Trim());
            //if (user == null)
            //{
            //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
            //    return 0;
            //}




            long realAmount = Amount + Fee;
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            return new Users().Deduct(realAmount, PartnerCode, PartnerCode, Note, RefCode);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
            return -99;
        }


    }

    private int UpdatePartnerBalanceTopup(string PartnerCode, long Amount, long Fee, string TranId, string RefCode)
    {
        try
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Fee.ToString(), TranId.ToString(), RefCode });
            //var partner = new Partners().GetCache(PartnerCode);
            //if (string.IsNullOrEmpty(partner.Hotline))
            //{
            //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
            //    return 0;
            //}
            //var user = new Users().GetByUserName(partner.Hotline.Trim());
            //if (user == null)
            //{
            //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
            //    return 0;
            //}


            long realAmount = Amount + Fee;
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            return new Users().Topup(realAmount, PartnerCode, PartnerCode, TranId, RefCode);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
            return -99;
        }


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
   
    //hủy xuất khuẩn
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
                if (_CardAPILog.Status == -2)
                {
                    var oldstatus = _CardAPILog.Status;
                    _CardAPILog.Status = -1;
                    _CardAPILog.TotalAmount = 0;
                    _CardAPILog.LastTime = DateTime.Now;
                    _CardAPILog.LogContent = " hủy bởi " + AppUtils.UserName;
                    //_CardAPILog.Mobile = "";
                    _CardAPILog.Fee = 0;
                    _CardAPILog.ApproveUser = AppUtils.UserName;
                    //_CardAPILog.Update();
                    var resultupdate = _CardAPILog.UpdateApp();

                    if (resultupdate < 0)
                    {
                        continue;
                    }
                    LogCache.RemovePBankInfo(_CardAPILog.RefCode);
                    var _userLog = new UserLog
                    {
                        UserName = AppUtils.UserName,
                        Action = "bankoutcancel",
                        ActionName = "Hủy lệnh bankout",
                        Description = "Hủy lệnh bankout" + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode
                    };
                    _userLog.Add();
                    TelegramClient.SendTeleV2("-5154675123", "Hủy lệnh bankout  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode + " Từ tài khoản " + AppUtils.UserName);
                    var partner = new Partners().GetCache(_CardAPILog.PartnerCode);
                    //if (!string.IsNullOrEmpty(partner.SMSUrl))
                    //{
                    //    TelegramClient.SendWarning(partner.SMSUrl, "Hủy duyệt lệnh bankout  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode );

                    //}
                    //CallBack Partner
                    var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                    var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * ck / 100);
                    UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Refund bank withdrawal amount: {2} transId: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());



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

                        //apiResponse.ResponseContent = serializer.Serialize(datacb);
                        //apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        datacb.ResponseCode = apiResponse.ResponseCode;
                        datacb.Description = apiResponse.Description;
                        datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode, partner.PrivateKey, partner.SignatureType);

                        Task.Run(async () => await CallbackJson(_CardAPILog.ReturnUrl, serializer.Serialize(datacb), _CardAPILog.TransactionID).ConfigureAwait(false));
                    }
                }

            }

        }

        AlertSuccesss.Text = "Hủy duyệt thành công";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        System.Threading.Thread.Sleep(2000);
        Response.Redirect(Request.RawUrl);
    }

    public string GenerateCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random = new Random();
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }

        return new string(result);
    }
    //duyệt tay
    protected void btAppClick(object sender, EventArgs e)

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
                if (_CardAPILog.Status == -2)
                {
                    var oldstatus = _CardAPILog.Status;
                    _CardAPILog.Status = 1;
                    _CardAPILog.TotalAmount = _CardAPILog.Amount;
                    _CardAPILog.LastTime = DateTime.Now;

                    var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                    var rw = getrw(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                    var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * ck / 100);
                    _CardAPILog.TotalAmount = _CardAPILog.Amount;
                    _CardAPILog.LastTime = DateTime.Now;
                    _CardAPILog.LogContent = " duyệt tay bởi " + AppUtils.UserName;
                    //_CardAPILog.Mobile = "";
                    _CardAPILog.Fee = Fee;
                    _CardAPILog.Reward = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * (rw) / 100);
                    _CardAPILog.ApproveUser = AppUtils.UserName;
                    _CardAPILog.Mobile = txtNote.Text;
                    var resultupdate = _CardAPILog.UpdateApp();

                    if (resultupdate < 0)
                    {
                        continue;
                    }
                    LogCache.RemovePBankInfo(_CardAPILog.RefCode);

                    var _userLog = new UserLog
                    {
                        UserName = AppUtils.UserName,
                        Action = "bankoutapp",
                        ActionName = "Duyệt tay bankout",
                        Description = "Duyệt tay bankout " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode
                    };
                    _userLog.Add();
                    TelegramClient.SendTeleV2("-5154675123", "Duyệt tay lệnh bankout  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode + " Từ tài khoản " + AppUtils.UserName);

                    //bắn tele
                    //if (_CardAPILog.PartnerCode == "sn1")
                    //{
                    //    var mess = $"<b>Tài khoản</b>: ({_CardAPILog.BankCode}) {_CardAPILog.BankAccountNumber} -  {_CardAPILog.BankAccountName} %0A<b>Mã giao dịch</b>: {_CardAPILog.BankTransId} -  {_CardAPILog.RefCode} %0A<b>Số tiền</b>: -{_CardAPILog.Amount.ToString("#,#").Replace(",", ".")} %0A<b>Thời gian</b>: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
                    //    TelegramNotify.SendTeleV4("-1002857697732", mess);
                    //}
                    //bắn tele
                  
                    var partner = new Partners().GetCache(_CardAPILog.PartnerCode);

                    //if (!string.IsNullOrEmpty(partner.SMSUrl))
                    //{

                    //    TelegramClient.SendWarning(partner.SMSUrl, "Duyệt tay lệnh bankout  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode );

                    //}
                    //CallBack Partner
                    //var partner = new Partners().Get(_CardAPILog.PartnerCode);
                    var privateKey = partner.PrivateKey;
                    if (!string.IsNullOrEmpty(_CardAPILog.ReturnUrl))
                    {
                        var datacb = new DataCallback()
                        {
                            Amount = Convert.ToInt32(_CardAPILog.Amount),
                            RefCode = _CardAPILog.RefCode,
                            TransactionID = _CardAPILog.TransactionID.ToString(),
                            OrderInfo = _CardAPILog.TransactionID.ToString(),
                            //TransactionID = _CardAPILog.TransactionID.ToString(),
                            Type = "bankout"
                        };
                        var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {

                        };
                        datacb.ResponseCode = apiResponse.ResponseCode;
                        datacb.Description = apiResponse.Description;
                        datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode, partner.PrivateKey, partner.SignatureType);

                        //apiResponse.ResponseContent = serializer.Serialize(datacb);
                        //apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await CallbackJson(_CardAPILog.ReturnUrl, serializer.Serialize(datacb), _CardAPILog.TransactionID).ConfigureAwait(false));
                    }

                }

            }

        }
        AlertSuccesss.Text = "Duyệt tay thành công";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        System.Threading.Thread.Sleep(2000);
        Response.Redirect(Request.RawUrl);
    }
    //duyệt api
    protected void btApp2Click(object sender, EventArgs e)
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        var systemDataConfig = new SystemDataConfig();
        var lstConfig = systemDataConfig.GetListCache();

        if (systemDataConfig.GetKey(lstConfig, "BankCashEnable") == "0")
        {
            AlertBans.Text = "Hệ thống rút bank đang bảo trì. Vui lòng chuyển sang rút tay";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertBan').modal()}); ", true);

            return;
        }
        else
        {
            var _Bank = new BankAccounts();
           // var data = _Bank.GetList().OrderBy(x => x.Id).Where(x => x.Type.Contains("OUT") && x.Status == 1 && x.StatusExtra == 1).ToList();
            var MinBankAproveAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MinBankAproveAmount"));
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
                    //check
                    var ischeck = 1;
                    if (_CardAPILog.Status == -2 )
                    {
                        var check = new BankTransaction().GetByRefcode(_CardAPILog.TransactionID.ToString() + "_re2call");
                        if (check != null)
                        {
                            //var check2 = new BankTransaction().GetByRefcode(_CardAPILog.TransactionID.ToString() + "_re2call");
                            //{
                            TelegramClient.SendTeleV2("-5154675123", "[Hết lượt] Lệnh bankout  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode + " Hết lượt duyệt tự động vui lòng huỷ lệnh");
                            //_CardAPILog.Status = -1;
                            //_CardAPILog.LogContent = " Huỷ duyệt vì hết lượt";
                            ////_CardAPILog.ApproveUser = AppUtils.UserName;
                            //_CardAPILog.LastTime = DateTime.Now;
                            //_CardAPILog.Update();
                            ischeck = 0;

                            // UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Refund bank withdrawal amount: {2} transId: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                            //}

                        }
                    }    
                      
                    
                    if (_CardAPILog.Status == -2  && ischeck == 1)
                    {
                        TelegramClient.SendTeleV2("-5154675123", "Duyệt tự động lệnh bankout  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode + " Từ tài khoản " + AppUtils.UserName);
                        System.Threading.Thread.Sleep(100);


                        _CardAPILog.Status = 0;
                        _CardAPILog.LogContent = " duyệt tự động bởi " + AppUtils.UserName;
                        //_CardAPILog.ApproveUser = AppUtils.UserName;
                        _CardAPILog.LastTime = DateTime.Now;
                         var resultupdate= _CardAPILog.UpdateApp();



                        if (resultupdate < 0)
                        {
                            continue;
                        }

                        var pbanklog = new PBankInfo
                        {
                            RefCode = _CardAPILog.RefCode,
                            Status = 0,
                            Time = DateTime.Now
                        };
                        LogCache.AddPBankInfo(pbanklog);

                        var _userLog = new UserLog
                        {
                            UserName = AppUtils.UserName,
                            Action = "bankoutappapi",
                            ActionName = "Duyệt tự động bankout",
                            Description = "Duyệt tự động bankout" + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode
                        };
                        _userLog.Add();



                        if (_CardAPILog.ProviderCode == "drumbankcash")
                        {
                            CashBankRequestV2 _cashRequest = new CashBankRequestV2();

                            _cashRequest.BankName = _CardAPILog.BankAccountName;
                            _cashRequest.BankId = _CardAPILog.BankAccountNumber;
                            _cashRequest.BankCode = _CardAPILog.BankCode;
                            //_cashRequest.Comment = "chuyen khoan " + GenerateCode(6);
                            _cashRequest.Comment = "";

                            if (_CardAPILog.PartnerCode == "cn02" || _CardAPILog.PartnerCode == "pp")
                            {
                                //_cashRequest.Comment = _CardAPILog.Note +;
                                _cashRequest.BankName = "NOCHECK";
                            }
                            _cashRequest.Amount = Convert.ToInt32(_CardAPILog.Amount);
                            _cashRequest.CallbackUrl = callbackurl2;
                            _cashRequest.TransId = _CardAPILog.TransactionID.ToString() + "_recall";


                            var check2 = new BankTransaction().GetByRefcode(_CardAPILog.TransactionID.ToString() + "_recall");
                            if (check2 != null)
                            {

                                _cashRequest.TransId = _CardAPILog.TransactionID.ToString() + "_re2call";
                            }
                            //else
                            //{
                            //    var check3 = new BankTransaction().GetByRefcode(_CardAPILog.TransactionID.ToString());
                            //    if (check3 == null)
                            //        _cashRequest.TransId = _CardAPILog.TransactionID.ToString();
                            //}

                            var signature = "";
                            var requestData = new RequestData()
                            {
                                PartnerCode = "order",
                                CommandCode = "TRANS_OUT",
                                Source = _CardAPILog.PartnerCode,
                                RequestContent = serializer.Serialize(_cashRequest),
                                Signature = signature
                            };
                            //var partner = new Partners().Get(transaction.PartnerCode);
                            //if (string.IsNullOrEmpty(partner.SMSPlusUrl))
                            //{
                            //    requestData.PartnerCode = "order";
                            //}
                            CashResponeV2 cashResult = new CashResponeV2();

                            NLogLogger.Info(new string[] { "MDrum", "Cash Request",serializer.Serialize(requestData), urlBaseService2
                                     });
                            var response = Task.Run(async () => await CallbackJson(urlBaseService2, serializer.Serialize(requestData), 0)).Result;
                            NLogLogger.Info(new string[] { "MDrum", "Cash Response", response, urlBaseService2
                                    });
                            var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                            var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * ck / 100);
                            try
                            {

                                var resObj = serializer.Deserialize<CashResponeV2>(response);
                                if (resObj.ResponseCode == 1)
                                {

                                }
                                else
                                {
                                    if (resObj.Description.Contains("is busy") || resObj.Description.Contains("đủ điều kiện"))
                                    {
                                        _CardAPILog.Status = -2;
                                        _CardAPILog.LogContent = "Chờ duyệt";
                                        _CardAPILog.LastTime = DateTime.Now;
                                        _CardAPILog.Update();


                                        TelegramClient.SendTeleV2("-5154675123", "Có lệnh bankout cần duyệt  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode);

                                        //return new APIResponse(1);
                                    }
                                    else
                                    {
                                        UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Refund bank withdrawal amount: {2} transId: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());


                                        _CardAPILog.Status = -1;

                                        _CardAPILog.LogContent = resObj.Description;
                                        _CardAPILog.LastTime = DateTime.Now;
                                        _CardAPILog.Update();
                                    }



                                }
                            }
                            catch (Exception ex)
                            {
                                NLogLogger.Info(ex.Message);
                                _CardAPILog.Status = -3;

                                _CardAPILog.LogContent = "suspicious";
                                _CardAPILog.LastTime = DateTime.Now;
                                _CardAPILog.Update();
                                //UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                            }

                        }



                    }

                }
            }
        }
        AlertSuccesss.Text = "Duyệt tự động thành công";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        System.Threading.Thread.Sleep(500);
        GetList();
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

        if (statusOver.ToString() == "-2")
        {
            return "<div class=\"label label-warning\">" + Resources.Pay.WaitApproval + " </ div > ";
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
        public string Source { get; set; }
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
    //public string GetQR(string BankCode, string BankId, string Amount)
    //{
    //    if (BankCode == "NVB")
    //        BankCode = "NCB";
    //    return String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?amount={2}", BankCode, BankId, Amount);
    //}
    public string GetQR(string BankCode, string BankId, string Amount, string AccountName)
    {
        if (BankCode == "NVB")
            BankCode = "NCB";
        return String.Format("https://img.vietqr.io/image/{0}-{1}-print.jpg?amount={2}&accountName={3}", BankCode, BankId, Amount, AccountName);
    }
    private const string urlBaseService2 = "http://127.0.0.1:9002/BankService.ashx";
    private const string callbackurl2 = "http://127.0.0.1:1592/Callback/DrumBankCash.ashx";
    public string GetLog(string log)
    {
        if (log.Contains("xác nhận"))
            return "Chờ xác nhận";
        return "Chờ duyệt";


    }

    public static async Task<string> PostTask(string url, string postData, string clientid = "", string key = "", string token = "")
    {

        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(120);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        if (!string.IsNullOrEmpty(clientid))
        {
            client.DefaultRequestHeaders.Add("x-client-id", clientid);
            client.DefaultRequestHeaders.Add("x-secret-key", key);
            client.DefaultRequestHeaders.Add("Authorization", token);
            Guid id = Guid.NewGuid();
            client.DefaultRequestHeaders.Add("x-interaction-id", id.ToString());
        }
        try
        {
            var response = await client.PostAsync(uri, httpContent);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                client.Dispose();
                return responseContent;
            }

        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "MDrum", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }
    public static async Task<string> GetTask(string url, string clientid = "", string key = "", string token = "")
    {
        var uri = new Uri(url);
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);

        NLogLogger.Info(new string[] { "MDrum", "GetTask", url });
        if (!string.IsNullOrEmpty(clientid))
        {
            client.DefaultRequestHeaders.Add("x-client-id", clientid);
            client.DefaultRequestHeaders.Add("x-secret-key", key);
            client.DefaultRequestHeaders.Add("Authorization", token);

        }
        try
        {
            var response = await client.GetAsync(uri);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                client.Dispose();
                return responseContent;
            }
        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "MDrum", "GetTask", "Exception", url, e.Message });
        }
        client.Dispose();
        return string.Empty;
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
    public string GetAmountStyle(object log)
    {
        var amount = Convert.ToInt64(log);
        if (amount >= 20000000)
            return "style='color:red;font-weight:bold'";
        return "";


    }

}