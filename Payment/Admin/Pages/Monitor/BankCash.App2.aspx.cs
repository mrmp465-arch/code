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
using Google.Authenticator;
using System.Threading.Tasks;
using Libs.API;
using Libs.CardTelco;
using Libs.Report;
using Libs.Utils;
using System.Security.Cryptography;
public partial class Pages_Monitor_BankCash_App2 : System.Web.UI.Page
{
    private const string urlBaseService = "http://rin.vnm.bz:10007/api/";
    private const string pw = "!lp08fAc";
    private const string secretKey = "5a562e41-a6a0-4288-975f-dd5a269076b8";

    private const string pwsm = "6bCG3N2uej";
    private const string urlBaseServicesm = "https://api.bobap.xyz/api/";
    private const string secretKeysm = "2c4bc033e05e45db807ab9b2ff07d165";
    private const string callbackurl = "https://bankgate.coroach.xyz//Callback/SimexCash.ashx";
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankCashApp2);

        //RoleAppAuto = AppUtils.CheckRolesPermission(Resources.Url.BankCashAppAuto);
        Page.Culture = "vi-vn";
        Page.UICulture = "vi-vn";
        if (!IsPostBack)
        {
            init();
            GetList();
        }
        if (IsPostBack)
        {
            string eventTarget = Request["__EVENTTARGET"];
            string eventArgument = Request["__EVENTARGUMENT"];

            if (eventTarget == "ConfirmOtp")
            {
                var parts = eventArgument.Split('|');
                string id = parts[0];
                string otp = parts[1];
                var _User = new Users().GetByUserName(AppUtils.UserName);

                TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
                bool isValid = TwoFacAuth.ValidateTwoFactorPIN(_User.F2a, otp, false);
                if (!isValid)
                {
                    AlertBans.Text = "Sai F2A";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertBan').modal()}); ", true);

                    return;
                }
                btApp2Click(id);
            }
            if (eventTarget == "ConfirmOtp2")
            {
                var parts2 = eventArgument.Split('|');
                string id2 = parts2[0];
                string otp2 = parts2[1];
                var _User = new Users().GetByUserName(AppUtils.UserName);

                TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
                bool isValid = TwoFacAuth.ValidateTwoFactorPIN(_User.F2a, otp2, false);
                if (!isValid)
                {
                    AlertBans.Text = "Sai F2A";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertBan').modal()}); ", true);

                    return;
                }
                btCancelClick(id2);
            }
        }
    }
    protected void btApp2Click(string Id)
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

            var MinBankAproveAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MinBankAproveAmount"));


            long TransId = Convert.ToInt64(Id);

            BankCashAPI _CardAPILog = new BankCashAPI();


            _CardAPILog.TransactionID = TransId;
            _CardAPILog = _CardAPILog.Get(TransId);
            //check


            if (_CardAPILog.Status == -5)
            {
                System.Threading.Thread.Sleep(100);

                _CardAPILog.Status = 0;
                _CardAPILog.LogContent = " duyệt tự động bởi " + AppUtils.UserName;
                //_CardAPILog.ApproveUser = AppUtils.UserName;
                _CardAPILog.LastTime = DateTime.Now;
                _CardAPILog.Update();

                // TelegramClient.SendTeleV2("-4762440012", "Duyệt tự động lệnh xuất khoản  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode + " Từ tài khoản " + AppUtils.UserName);





                if (_CardAPILog.ProviderCode == "drumbankcash")
                {
                    CashBankRequestV2 _cashRequest = new CashBankRequestV2();

                    _cashRequest.BankName = _CardAPILog.BankAccountName;
                    _cashRequest.BankId = _CardAPILog.BankAccountNumber;
                    _cashRequest.BankCode = _CardAPILog.BankCode;
                    _cashRequest.Comment = "";

                    _cashRequest.Amount = Convert.ToInt32(_CardAPILog.Amount);
                    _cashRequest.CallbackUrl = callbackurl2;
                    _cashRequest.TransId = _CardAPILog.TransactionID.ToString();

                    var signature = "";
                    var requestData = new RequestData()
                    {
                        PartnerCode = "order",
                        CommandCode = "TRANS_OUT",
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
                    var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * (ck - 1));
                    try
                    {

                        var resObj = serializer.Deserialize<CashResponeV2>(response);
                        if (resObj.ResponseCode == 1)
                        {

                        }
                        else
                        {
                            if (resObj.Description.Contains("is busy"))
                            {
                                _CardAPILog.Status = -2;
                                _CardAPILog.LogContent = "Chờ duyệt";
                                _CardAPILog.LastTime = DateTime.Now;
                                _CardAPILog.Update();


                                TelegramNotify.SendTeleV2("-4762440012", "Có lệnh bankout cần duyệt  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode);

                                //return new APIResponse(1);
                            }
                            else
                            {
                                UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());


                                _CardAPILog.Status = -1;

                                _CardAPILog.LogContent = resObj.Description;
                                _CardAPILog.LastTime = DateTime.Now;
                                _CardAPILog.Update();
                            }



                        }
                    }
                    catch
                    {
                        _CardAPILog.Status = -3;
                        _CardAPILog.LogContent = "suspicious";
                        _CardAPILog.LastTime = DateTime.Now;
                        _CardAPILog.Update();
                        //UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                    }

                }

                if (_CardAPILog.ProviderCode == "vnpaycash")
                {


                    var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                    var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * (ck - 1));
                    CashRespone cashResult = new CashRespone();
                    CashRequest _cashRequest = new CashRequest();
                    var signature = Libs.Utils.Encrypts.MD5(_CardAPILog.BankAccountNumber + Convert.ToInt32(_CardAPILog.Amount).ToString() + _CardAPILog.TransactionID.ToString() + pw);
                    var urlService = string.Format(
                       "{0}/Bank/ChargeOut?apiKey={1}&bank_code={2}&amount={3}&bank_account={4}&bank_accountName={5}&requestId={6}&msg={7}&signature={8}",
                       urlBaseService,
                       secretKey,
                       _CardAPILog.BankCode,
                       Convert.ToInt32(_CardAPILog.Amount),
                       _CardAPILog.BankAccountNumber,
                       _CardAPILog.BankAccountName,
                       _CardAPILog.TransactionID,
                       _CardAPILog.TransactionID,
                       signature
                   );
                    NLogLogger.Info(new string[] { "VNPAY", "Order Request", urlService });
                    var response = Task.Run(async () => await GetTask(urlService)).Result;
                    NLogLogger.Info(new string[] { "VNPAY", "Order Response", response });
                    try
                    {

                        var resObj = serializer.Deserialize<CashRespone>(response);
                        if (resObj.stt == 1)
                        {

                        }
                        else
                        {
                            if (_CardAPILog.PartnerCode == "cn02")
                            {
                                UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Refund withdrawal amount: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                            }
                            else
                            {
                                UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                            }

                            _CardAPILog.Status = -1;

                            _CardAPILog.LogContent = serializer.Serialize(cashResult);
                            _CardAPILog.LastTime = DateTime.Now;
                            _CardAPILog.Update();



                        }
                    }
                    catch
                    {
                        _CardAPILog.Status = -3;
                        _CardAPILog.LogContent = "suspicious";
                        _CardAPILog.LastTime = DateTime.Now;
                        _CardAPILog.Update();
                        //UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                    }

                }
                if (_CardAPILog.ProviderCode == "simexcash")
                {


                    var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                    var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * (ck - 1));
                    CashRespone cashResult = new CashRespone();
                    CashRequest _cashRequest = new CashRequest();
                    var signature = Libs.Utils.Encrypts.MD5(_CardAPILog.BankAccountNumber + Convert.ToInt32(_CardAPILog.Amount).ToString() + _CardAPILog.TransactionID.ToString() + pwsm);
                    var urlService = string.Format("{0}Bank/ChargeOut?apiKey={1}&bank_code={2}&amount={3}&bank_account={4}&bank_accountName={5}&requestId={6}&member_identity={7}&callback={8}&msg={9}&signature={10}",
                     urlBaseServicesm,
                     secretKeysm,
                     _CardAPILog.BankCode,
                     Convert.ToInt32(_CardAPILog.Amount),
                     _CardAPILog.BankAccountNumber,
                     _CardAPILog.BankAccountName,
                     _CardAPILog.TransactionID,
                     _CardAPILog.TransactionID,
                     "https://bankgate.coroach.xyz//Callback/SimexCash.ashx",
                     _CardAPILog.ReturnValue,
                     signature);
                    NLogLogger.Info(new string[] { "Simex", "Cash Request", urlService });
                    var response = Task.Run(async () => await GetTask(urlService)).Result;
                    NLogLogger.Info(new string[] { "Simex", "Cash Response", response });
                    try
                    {

                        var resObj = serializer.Deserialize<CashRespone>(response);
                        if (resObj.stt == 1)
                        {

                        }
                        else
                        {
                            if (_CardAPILog.PartnerCode == "cn02")
                            {
                                UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Refund withdrawal amount: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                            }
                            else
                            {
                                UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                            }

                            _CardAPILog.Status = -1;

                            _CardAPILog.LogContent = serializer.Serialize(cashResult);
                            _CardAPILog.LastTime = DateTime.Now;
                            _CardAPILog.Update();



                        }
                    }
                    catch
                    {
                        _CardAPILog.Status = -3;
                        _CardAPILog.LogContent = "suspicious";
                        _CardAPILog.LastTime = DateTime.Now;
                        _CardAPILog.Update();
                        //UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                    }

                }

                if (_CardAPILog.ProviderCode == "24hbankcash")
                {


                    var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                    var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * (ck - 1));
                    string callbackurl = "https://bankgate.coroach.xyz/Callback/24hbankcash.ashx";
                    CashRespone cashResult = new CashRespone();
                    CashRequestV3 _cashRequest = new CashRequestV3();
                    _cashRequest.apiKey = "eba4fa0d4e90ef5ebb49be8e401580b84cda2dedb07b24a6adb3b3d1e096bd3f";
                    _cashRequest.customer_name = _CardAPILog.BankAccountName;
                    _cashRequest.amount = Convert.ToInt32(_CardAPILog.Amount);

                    _cashRequest.url_callback = callbackurl;
                    _cashRequest.request_id = _CardAPILog.TransactionID.ToString();
                    _cashRequest.bank_code = _CardAPILog.BankCode;
                    _cashRequest.bank_account = _CardAPILog.BankAccountNumber;

                    var lstbankcode = new BankCodeTranfer().GetListCache();

                    if (lstbankcode.Exists(x => x.code == _cashRequest.bank_code))
                        _cashRequest.bank_code = lstbankcode.FirstOrDefault(x => x.code == _cashRequest.bank_code).bin;

                    var signature = ToSha256(_cashRequest.bank_code + _CardAPILog.BankAccountNumber + _cashRequest.amount.ToString() + _CardAPILog.TransactionID.ToString() + "Vidy7vYrM8vPaOisRAlosd34wbHEuCqd");
                    _cashRequest.vsign = signature;
                    var urlService = "https://247pay.vip/api/v1/Cashout/Create";

                    NLogLogger.Info(new string[] { "24h", "Order Request", urlService });
                    var response = Task.Run(async () => await PostTask(urlService, serializer.Serialize(_cashRequest))).Result;
                    NLogLogger.Info(new string[] { "24h", "Order Response", response });
                    try
                    {

                        var resObj = serializer.Deserialize<CashResponeV4>(response);
                        if (resObj.code == 200)
                        {

                        }
                        else
                        {
                            if (_CardAPILog.PartnerCode == "cn02")
                            {
                                UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Refund withdrawal amount: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                            }
                            else
                            {
                                UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                            }

                            _CardAPILog.Status = -1;

                            _CardAPILog.LogContent = serializer.Serialize(resObj);
                            _CardAPILog.LastTime = DateTime.Now;
                            _CardAPILog.Update();



                        }
                    }
                    catch
                    {
                        _CardAPILog.Status = -3;
                        _CardAPILog.LogContent = "suspicious";
                        _CardAPILog.LastTime = DateTime.Now;
                        _CardAPILog.Update();
                        //UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                    }

                }


            }

        }
        AlertSuccesss.Text = "Duyệt thành công";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        System.Threading.Thread.Sleep(500);
        Response.Redirect(Request.RawUrl);
    }
    public class CashRequestV3
    {
        public string apiKey { get; set; }
        public string bank_account { get; set; }
        public int amount { get; set; }
        public string customer_name { get; set; }
        public string bank_code { get; set; }
        public string url_callback { get; set; }
        public string request_id { get; set; }
        public string vsign
        {
            get; set;

        }
    }
    public static string ToSha256(string input)
    {
        if (input == null)
            input = string.Empty;

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
    public class CashResponeV4
    {
        public bool success { get; set; }
        public int code { get; set; }
        public string message { get; set; }

    }
    protected void btCancelClick(string id)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();



        long TransId = Convert.ToInt64(id);

        BankCashAPI _CardAPILog = new BankCashAPI();


        _CardAPILog.TransactionID = TransId;
        _CardAPILog = _CardAPILog.Get(TransId);
        if (_CardAPILog.Status == -5)
        {
            var oldstatus = _CardAPILog.Status;
            _CardAPILog.Status = -1;
            _CardAPILog.TotalAmount = 0;
            _CardAPILog.LastTime = DateTime.Now;
            _CardAPILog.LogContent = " hủy bởi " + AppUtils.UserName;
            //_CardAPILog.Mobile = "";
            _CardAPILog.Fee = 0;
            _CardAPILog.ApproveUser = AppUtils.UserName;
            _CardAPILog.Update();

            //TelegramClient.SendTeleV2("-4762440012", "Hủy duyệt lệnh xuất khoản  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode + " Từ tài khoản " + AppUtils.UserName);
            var partner = new Partners().GetCache(_CardAPILog.PartnerCode);
            //if (!string.IsNullOrEmpty(partner.SMSUrl))
            //{
            //    TelegramClient.SendWarning(partner.SMSUrl, "Hủy duyệt lệnh xuất khoản  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + "  đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode );

            //}
            //CallBack Partner
            var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
            var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * (ck - 1));
            UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());



            //var partner = new Partners().Get(_CardAPILog.PartnerCode);
            var privateKey = partner.PrivateKey;
            if (!string.IsNullOrEmpty(_CardAPILog.ReturnUrl))
            {
                var datacb = new DataCallback()
                {
                    Amount = _CardAPILog.Amount,
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
                if (_CardAPILog.PartnerCode == "cn02")
                {
                    var datacb2 = new DataCallbackV3();
                    datacb2.RefCode = datacb.RefCode;
                    datacb2.Amount = 0;
                    datacb2.Type = datacb.Type;
                    datacb2.OrderInfo = datacb.OrderInfo;
                    datacb2.ResponseCode = apiResponse.ResponseCode;
                    datacb2.Description = apiResponse.Description;
                    datacb2.Signature = PaymentUtils.Signature(datacb2.ResponseCode.ToString() + datacb2.Description + datacb2.RefCode, partner.PrivateKey, partner.SignatureType);
                    Task.Run(async () => await CallbackJson(_CardAPILog.ReturnUrl, serializer.Serialize(datacb2), _CardAPILog.TransactionID).ConfigureAwait(false));
                }
                else
                {
                    Task.Run(async () => await CallbackJson(_CardAPILog.ReturnUrl, serializer.Serialize(apiResponse), _CardAPILog.TransactionID).ConfigureAwait(false));
                }

            }
        }





        AlertSuccesss.Text = "Hủy lệnh thành công";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        System.Threading.Thread.Sleep(2000);
        Response.Redirect(Request.RawUrl);
    }
    private void init()
    {
        //btView.Text = Resources.Pay.View;
        //btExcel.Text = Resources.Pay.ExportExcel;
        //if(RoleAppAuto)
        //{
        //    btApp2.Visible = true;
        //}    
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
        txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("dd/MM/yyyy HH:mm:ss");
    }

    private void GetList()
    {
        int top = Convert.ToInt32(drpTop.SelectedValue);

        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
        //requestTime = requestTime.AddDays(1).AddMilliseconds(-1);


        int? status = -5;


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
        var partner = new Partners().GetCache(PartnerCode);
        var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
        if (listpartnerDiscount == null)
        {
            //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }

        if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
        {
            //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
            return ck;
        }
        var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
        ck = _partnerDiscount.DiscountBANKOUTTRANFER;
        if (Type == "MOMO")
            ck = _partnerDiscount.DiscountMOMOOUT;
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
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                return;
            }
            var user = new Users().GetByUserName(partner.Hotline.Trim());
            if (user == null)
            {
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
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
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                return;
            }
            var user = new Users().GetByUserName(partner.Hotline.Trim());
            if (user == null)
            {
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
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
        public Decimal Amount { get; set; }
        //public string Signature { get; set; }
        public string Type { get; set; }
        public string OrderInfo { get; set; }
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
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
    public string GetQR(string BankCode, string BankId, string Amount)
    {
        return String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?amount={2}", BankCode, BankId, Amount);
    }
    private const string urlBaseService2 = "http://127.0.0.1:9002/BankService.ashx";
    private const string callbackurl2 = "http://127.0.0.1:1592/Callback/DrumBankCash.ashx";
    public string GetLog(string log)
    {
        if (log.Contains("xác nhận"))
            return "Chờ xác nhận";
        return "Chờ duyệt";


    }
    public class DataCallbackV3
    {
        public string RefCode { get; set; }
        //public string MomoTransId { get; set; }
        public string TransactionID { get; set; }
        public int Amount { get; set; }
        //public string Signature { get; set; }
        public string OrderInfo { get; set; }
        public string Type { get; set; }

        public int ResponseCode { get; set; }

        public string Description { get; set; }

        public string Signature { get; set; }
    }
    public static async Task<string> GetTask(string url)
    {
        var uri = new Uri(url);
        HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(160);

        NLogLogger.Info(new string[] { "VNPAY", "GetTask", url });

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
            NLogLogger.Info(new string[] { "VNPAY", "GetTask", "Exception", url, e.Message });
        }
        client.Dispose();
        return string.Empty;
    }
    public static async Task<string> PostTask(string url, string postData)
    {

        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(120);
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
    public class CashRespone
    {
        public int stt { get; set; }

    }
    public class CashRequest
    {
        public string type { get; set; }
        public string stk { get; set; }
        public string bank_type { get; set; }
        public int amount { get; set; }
        public string message { get; set; }
        public string ref_id { get; set; }
        public string receiver { get; set; }

    }
    public class CashRequestV2
    {
        public string PartnerCode { get; set; }
        public string AccountNumber { get; set; }
        public int Amount { get; set; }
        public string AccountName { get; set; }
        public string BankCode { get; set; }
        public string CallbackUrl { get; set; }
        public string RefCode { get; set; }
        public string Signature
        {
            get; set;

        }
    }
    public class CashResponeV3
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }

    }
}