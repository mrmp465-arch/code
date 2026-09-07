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
using DocumentFormat.OpenXml.Math;
using System.IO;



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
            var partner = new Partners().Get(_CardAPILog.PartnerCode);
            if (oldStatus < 1)
            {
                var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.CardType);
                var rw = getrw(_CardAPILog.PartnerCode, _CardAPILog.CardType);
                var feeProvider = getfeeProvider(_CardAPILog.CardType);
                var Amount = Math.Min(_CardAPILog.AmountUser, Convert.ToInt64(_CardAPILog.Amount));
                _CardAPILog.Fee = Convert.ToInt32(Amount * ck);
                _CardAPILog.Reward = Convert.ToInt32(Amount * rw);
                _CardAPILog.FeeProvider = Convert.ToInt32(Amount * feeProvider);
                UpdatePartnerBalance(partner.PartnerCode, Amount, _CardAPILog.Fee, String.Format("Topup to recharge card {4} transId: {0}-{1}-{2}-{3}", _CardAPILog.TransactionID, _CardAPILog.CardType, _CardAPILog.CardSerial, _CardAPILog.CardCode, Amount.ToString("#,#").Replace(",", ".")), "Card_" + _CardAPILog.TransactionID);
                //UpdatePartnerBalanceReward(partner.SMSUrl, _CardAPILog.Reward, String.Format("Cộng tiền hoa hồng nạp thẻ đối tác {5} {4} mgd: {0}-{1}-{2}-{3}", _CardAPILog.TransactionID, _CardAPILog.CardType, _CardAPILog.CardSerial, _CardAPILog.CardCode, Amount.ToString("#,#").Replace(",", "."), partner.PartnerCode), "RCard_" + _CardAPILog.TransactionID.ToString());
            }
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

                //if (oldStatus < 1)
                //{
                //    var Amount = Math.Min(Convert.ToInt64(_topupMobile3rdLog.Amount), _topupMobile3rdLog.AmountUser);
                //    if (_CardAPILog.CardType == "gate")
                //        Amount = Convert.ToInt64(_topupMobile3rdLog.Amount);
                //    Action<string, long, string, string, string> send = UpdatePartnerBalance;
                //    var asynSend = send.BeginInvoke(_CardAPILog.PartnerCode, Amount, _CardAPILog.CardType.ToLower(), String.Format("Cộng tiền nạp thẻ {4} mgd: {0}-{1}-{2}-{3}", _CardAPILog.TransactionID, _CardAPILog.CardType, _CardAPILog.CardSerial, _CardAPILog.CardCode, Amount.ToString("#,#").Replace(",", ".")), "Card_" + _CardAPILog.TransactionID, null, null);

                //}

                //if (!string.IsNullOrEmpty(_topupMobileLog.CallbackUrl)) // Callback Provider
                //{
                //    var callbackData = new DataCallbackOrder()
                //    {
                //        OrderId = _topupMobileLog.TransactionID,
                //        Amount = Convert.ToInt32(_topupMobile3rdLog.Amount),
                //        Status = 1,
                //        CardSerial = _topupMobile3rdLog.CardSerial,
                //        CardCode = _topupMobile3rdLog.CardCode,
                //        BidRate = _topupMobile3rdLog.BidRate,
                //        UpdateTime = DateTime.Now,
                //        CreatTime = _topupMobile3rdLog.CreateTime,
                //        Signature = string.Empty
                //    };

                //    Task.Run(() => CallbackJson(_CardAPILog.CallbackUrl, serializer.Serialize(callbackData), _topupMobileLog.AccountName, _topupMobile3rdLog.Id, 1).ConfigureAwait(false));
                //}
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
                Task.Run(() => CallbackJson(_CardAPILog.CallbackUrl, serializer.Serialize(datacb), _CardAPILog.PartnerCode, _CardAPILog.TransactionID).ConfigureAwait(false));
            }

            NLogLogger.Info(new string[] { "CardAPI.FixStatus", "UPDATE", AppUtils.UserName, _CardAPILog.TransactionID.ToString(), _CardAPILog.CardSerial, _CardAPILog.Amount.ToString(), _CardAPILog.Status.ToString() });

            AlertSuccesss.Text = "Cập nhập thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
        }

    }
    private decimal getck(string PartnerCode, string Type)
    {
        decimal ck = 0;
        //var partner = new Partners().GetCache(PartnerCode);
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
        ck = _partnerDiscount.DiscountVTT;
        switch (Type)
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
            case "vcoin":
                ck = _partnerDiscount.DiscountGATE;
                break;

        }
        // ck = _partnerDiscount.DiscountVTT;
        return ck;
    }
    private decimal getfeeProvider( string Type)
    {
        decimal ck = 0;
        switch (Type)
        {
            case "vms":
                ck = 18/100;
                break;
            case "vnp":
                ck = 18 / 100;
                break;
            case "viettel":
                ck = 18 / 100;
                break;
            case "zing":
                ck = 18 / 100;
                break;
            case "vcoin":
                ck = 18 / 100;
                break;

        }
        return ck;
    }

    private decimal getrw(string PartnerCode, string Type)
    {
        decimal ck = 0;
        //var partner = new Partners().GetCache(PartnerCode);
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
        ck = _partnerDiscount.RewardVTT;
        switch (Type)
        {
            case "vms":
                ck = _partnerDiscount.RewardVMS;
                break;
            case "vnp":
                ck = _partnerDiscount.RewardVNP;
                break;
            case "viettel":
                ck = _partnerDiscount.RewardVTT;
                break;
            case "zing":
                ck = _partnerDiscount.RewardZING;
                break;
            case "vcoin":
                ck = _partnerDiscount.RewardGATE;
                break;

        }
        // ck = _partnerDiscount.RewardVTT;
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
            //   // return;
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
    private void UpdatePartnerBalanceReward(string PartnerCode, long realAmount, string TranId, string RefCode)
    {
        try
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, realAmount.ToString(), TranId.ToString(), RefCode });
            var partner = new Partners().GetCache(PartnerCode);
            //if (string.IsNullOrEmpty(partner.Hotline))
            //{
            //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
            //    // return;
            //}
            //var user = new Users().GetByUserName(partner.Hotline.Trim());
            //if (user == null)
            //{
            //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
            //    return;
            //}
            if (realAmount == 0)
            {
                //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return;
            }

            //long realAmount = Amount - fee;
            // NLogLogger.Info(new string[] { "Bank Topup", realAmount.ToString(), ck.ToString() });
            new Users().Topup(realAmount, PartnerCode, PartnerCode, TranId, RefCode);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
        }


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