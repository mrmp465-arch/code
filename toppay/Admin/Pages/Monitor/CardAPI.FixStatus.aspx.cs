using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.Utils;
using System.Net.Http;
using System.Net.Http.Headers;
using Libs.API;
using System.Net;

public partial class Pages_Monitor_CardAPI_FixStatus : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPIFixStatus);

    }


    protected void btView_Click(object sender, EventArgs e)
    {
        CardAPILog _CardAPILog = new CardAPILog();
        _CardAPILog.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);

        _CardAPILog = _CardAPILog.Get();

        if (_CardAPILog == null)
        {
            txtCardSerial.Text = "";
            txtCardCode.Text = "";
            txtStatus.Text = "";
            txtAmount.Text = "";
            AlertInfos.Text = "Không tồn tại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
        }
        else
        {
            txtCardSerial.Text = _CardAPILog.CardSerial;
            txtCardCode.Text = _CardAPILog.CardCode;
            txtStatus.Text = _CardAPILog.Status.ToString();
            txtAmount.Text = _CardAPILog.Amount.ToString();
        }
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        CardAPILog _CardAPILog = new CardAPILog();
        TopupMobileLog _topupMobileLog = new TopupMobileLog();
        var tranid = AppUtils.ToInt64(txtTransactionID.Text);
        _CardAPILog.TransactionID = tranid;
        _CardAPILog = _CardAPILog.Get();
        var oldStatus = _CardAPILog.Status;

        NLogLogger.Info(new string[] { "CardAPI.FixStatus", "GET CardAPILog", AppUtils.UserName, serializer.Serialize(_CardAPILog) });

        //TopupMobile3rdLog _topupMobile3rdLog = new TopupMobile3rdLog();
        //_topupMobile3rdLog.TransactionId = tranid;
        //_topupMobile3rdLog.GetByTransactionIdSuccess(tranid);


        if (_CardAPILog == null)
        {
            txtCardSerial.Text = "";
            txtCardCode.Text = "";
            txtStatus.Text = "";
            txtAmount.Text = "";
            AlertInfos.Text = "Không tồn tại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);

        }
        else
        {
            _CardAPILog.Status = AppUtils.ToInt32(txtStatus.Text);
            _CardAPILog.Amount = AppUtils.ToInt64(txtAmount.Text);
            _CardAPILog.Description = "Fix Status";
            _CardAPILog.Update();

            var _topupMobile3rdLog = new TopupMobile3rdLog().GetByTransactionIdSuccess(tranid);

            if (_topupMobile3rdLog != null)
            {
                var amountChange = AppUtils.ToInt32(txtAmount.Text) - Convert.ToInt32(_topupMobile3rdLog.Amount);
                if (_topupMobile3rdLog.Status == -2)
                {
                    amountChange = AppUtils.ToInt32(txtAmount.Text) - Convert.ToInt32(_topupMobile3rdLog.AmountUser);
                }
                _topupMobile3rdLog.Amount = AppUtils.ToInt32(txtAmount.Text);
                _topupMobile3rdLog.Status = AppUtils.ToInt32(txtStatus.Text);
                _topupMobile3rdLog.LogContent = "Fix Satus";
                _topupMobile3rdLog.Update();

                NLogLogger.Info(new string[] { "CardAPI.FixStatus", "UPDATE", serializer.Serialize(_topupMobile3rdLog) });

                if (AppUtils.ToInt32(txtStatus.Text) > 0)
                {
                    _topupMobileLog.TransactionID = Convert.ToInt64(_topupMobile3rdLog.RequestNo);
                    _topupMobileLog.Get();
                    NLogLogger.Info(new string[] { "CardAPI.FixStatus", "GET TopupMobileLog", AppUtils.UserName, serializer.Serialize(_topupMobileLog) });
                    _topupMobileLog.Topup(1, amountChange, _topupMobile3rdLog.BidRate);

                    if (_topupMobile3rdLog.Telco == "zing")
                    {
                        Task.Run(() => CallBackProvider(_topupMobile3rdLog.Id.ToString(), _topupMobile3rdLog.Amount.GetValueOrDefault()).ConfigureAwait(false));
                    }
                }

            }
            else
            {

                NLogLogger.Info(new string[] { "CardAPI.FixStatus", "NONE 3RD", serializer.Serialize(_topupMobile3rdLog) });
            }

            int cbStatus = 0;
            if (_CardAPILog.Status == 1 || _CardAPILog.Status == 2)
            {
                cbStatus = _CardAPILog.Amount != _CardAPILog.AmountUser ? (int)ResponseCode.CardAmountInvalid : (int)ResponseCode.TransactionSuccessful;
                if (_CardAPILog.CardType == "gate")
                    cbStatus = (int)ResponseCode.TransactionSuccessful;

                //update balance
                //NLogLogger.Info(new string[] { "CardTelco Topup", transaction.PartnerCode, result.ResponseContent, request.CardType.ToLower() });

                if (oldStatus < 1)
                {
                    var Amount = Math.Min(Convert.ToInt64(_topupMobile3rdLog.Amount), _topupMobile3rdLog.AmountUser);
                    if (_CardAPILog.CardType == "gate")
                        Amount = Convert.ToInt64(_topupMobile3rdLog.Amount);
                    Action<string, long, string, string, string> send = UpdatePartnerBalance;
                    var asynSend = send.BeginInvoke(_CardAPILog.PartnerCode, Amount, _CardAPILog.CardType.ToLower(), String.Format("Cộng tiền nạp thẻ {4} mgd: {0}-{1}-{2}-{3}", _CardAPILog.TransactionID, _CardAPILog.CardType, _CardAPILog.CardSerial, _CardAPILog.CardCode, Amount.ToString("#,#").Replace(",", ".")), "Card_" + _CardAPILog.TransactionID, null, null);

                }

                if (!string.IsNullOrEmpty(_topupMobileLog.CallbackUrl)) // Callback Provider
                {
                    var callbackData = new DataCallbackOrder()
                    {
                        OrderId = _topupMobileLog.TransactionID,
                        Amount = Convert.ToInt32(_topupMobile3rdLog.Amount),
                        Status = 1,
                        CardSerial = _topupMobile3rdLog.CardSerial,
                        CardCode = _topupMobile3rdLog.CardCode,
                        BidRate = _topupMobile3rdLog.BidRate,
                        UpdateTime = DateTime.Now,
                        CreatTime = _topupMobile3rdLog.CreateTime,
                        Signature = string.Empty
                    };

                    Task.Run(() => CallbackJson(_CardAPILog.CallbackUrl, serializer.Serialize(callbackData), _topupMobileLog.AccountName, _topupMobile3rdLog.Id, 1).ConfigureAwait(false));
                }
            }
            else
            {
                cbStatus = -1;
            }

            //CallBack Partner
            var privateKey = new Partners().Get(_CardAPILog.PartnerCode).PrivateKey;
            if (!string.IsNullOrEmpty(_CardAPILog.CallbackUrl))
            {
                var datacb = new DataCallback()
                {
                    Amount = _CardAPILog.Amount,
                    RefCode = _CardAPILog.RequestNo,
                    Status = cbStatus,
                    Signature = Libs.Utils.Encrypts.MD5(_CardAPILog.RequestNo + cbStatus + _CardAPILog.Amount + privateKey)
                };
                Task.Run(() => CallbackJson(_CardAPILog.CallbackUrl, serializer.Serialize(datacb), _CardAPILog.PartnerCode, 0, 2).ConfigureAwait(false));
            }

            NLogLogger.Info(new string[] { "CardAPI.FixStatus", "UPDATE", AppUtils.UserName, _CardAPILog.TransactionID.ToString(), _CardAPILog.CardSerial, _CardAPILog.Amount.ToString(), _CardAPILog.Status.ToString() });

            AlertSuccesss.Text = "Cập nhập thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
        }

    }
    private void UpdatePartnerBalance(string PartnerCode, long Amount, string CardType, string Note, string RefCode)
    {
        try
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), CardType, Note });
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
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode,  DateTime.Now.Year, DateTime.Now.Month);
            if (listpartnerDiscount == null)
            {
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return;
            }

            if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                return;

            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
            decimal ck = 0;
            switch (CardType)
            {
                case "vms":
                    ck = _partnerDiscount.DiscountVMS;
                    break;
                case "vnp":
                    ck = _partnerDiscount.DiscountVNP;
                    break;
                case "viettel":
                    ck = _partnerDiscount.DiscountVTT;
                    break;
                case "zing":
                    ck = _partnerDiscount.DiscountZING;
                    break;
                case "gate":
                    ck = _partnerDiscount.DiscountGATE;
                    break;
            }
            if (ck == 0)
                return;

            long realAmount = Amount - Convert.ToInt64(Amount * ck);
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Users().Topup(realAmount, user.UserName, PartnerCode, Note, RefCode);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
        }
    }
    protected async Task<string> CallBackProvider(string Id, int Amount)
    {
        var urlConfirm = "http://localhost:1583/TopupCallbackAppVTT.asmx";
        var privateKey = "1e7f56ca5fcbf781fa022f4f5dff74d6";
        var signature = Encrypts.MD5(string.Format("{0}|{1}|{2}|{3}|{4}|{5}", Id, -6, Amount, string.Empty, string.Empty, privateKey));
        var service = new APIProxy.VTTService.TopupCallbackAppVTT(urlConfirm);
        var res = service.Callback(Id, -6, Amount, string.Empty, string.Empty, signature);
        Response.Redirect(Request.RawUrl);
        return "";
    }
    /// <summary>
    /// Callback for Provider or Partner
    /// </summary>
    /// <param name="url"></param>
    /// <param name="postData"></param>
    /// <param name="code"></param>
    /// <param name="tranId"></param>
    /// <param name="type">1: Provider, 2: Partner</param>
    /// <returns></returns>
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

    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public long Amount { get; set; }
        public string Signature { get; set; }

    }

    public class DataCallbackOrder
    {
        public long OrderId { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public decimal? BidRate { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime? CreatTime { get; set; }
        public string Description { get; set; }

    }

}