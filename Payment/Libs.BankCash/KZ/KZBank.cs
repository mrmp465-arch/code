using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using static Libs.BankCash.BankCashService;

using static Libs.BankCash.KZ.KZBankLib;


namespace Libs.BankCash.KZ
{
    public class KZBank : IBankCashHandler
    {
        // Production MoBo
        private const string urlBaseService = "http://144.126.242.45/api/bank/register";
        private const string urlBaseServiceMomo = "http://144.126.242.45/api/mmo/register";
        private const string callbackbank = "https://bankgate.coroach.xyz/KZ/BankOut.ashx";
        private const string callbackmomo = "https://bankgate.coroach.xyz/KZ/MomoOut.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string Partner = "GRB";

        private const string PartnerCode = "08bdda2ce7a3ffe45d45bbd1f499fb96";

        public APIResponse ReCallBack(long id)
        {
            throw new NotImplementedException();
        }
        public APIResponse Check(APITransaction transaction)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();




            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public APIResponse Cash(APITransaction transaction)
        {
            //TelegramNotify.SendNotify(-845553760, "Có lệnh bank cần duyệt !!!");
            //return new APIResponse((int)ResponseCode.TransactionFailed);
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderRequest request = new OrderRequest();
            request = serializer.Deserialize<OrderRequest>(transaction.RequestContent);
            if (request.BankName == "MBB")
                request.BankName = "MB";

            if (request.BankName == "AGR")
                request.BankName = "VBA";

            //if (request.BankName == "STB")
            //    request.BankName = "SACOMBANK";

            if (request.BankName == "SHIB")
                request.BankName = "SHBVN";

            //if (request.BankName == "VCCB")
            //    request.BankName = "VIETCAPITALBANK";

            //if (request.BankName == "BAB")
            //    request.BankName = "BACABANK";

            //if (request.BankName == "DAB")
            //    request.BankName = "DONGABANK";

            //if (request.BankName == "GPB")
            //    request.BankName = "GPBANK";

            if (request.BankName == "PVB")
                request.BankName = "PVCB";

            //if (request.BankName == "PGB")
            //    request.BankName = "PGBANK";

            if (request.BankName == "SAB")
                request.BankName = "SEABANK";

            if (request.BankName == "ABB")
                request.BankName = "ABBANK";

            if (request.BankName == "VTB")
                request.BankName = "VIETINBANK";

            request.BankAccountName = GlobalHelper.ReplaceVietnameseChar(request.BankAccountName);
            APIResponse _APIResponse = new APIResponse();
            var _BankCashAPI = new BankCashAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = KZBankLib.GenOrderCode(),
                OrderInfo = string.Empty,
                Amount = request.Amount,
                TotalAmount = request.Amount,
                Currency = "VND",
                ReturnUrl = request.CallbackUrl,
                RequestTime = 0,
                Signature = "",
                LogContent = "Add Order",
                //Note = request.Note,
                BankCode = request.BankName,
                FullName = request.RefCode,
                Mobile = string.Empty,
                RefCode = request.RefCode,
                BankAccountName = request.BankAccountName,
                BankAccountNumber = request.BankAccountNumber
            };
            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;

                //if (request.Amount < 1000 || request.Amount > 10000000)
                //{
                //    return new APIResponse((int)ResponseCode.BankAmountInvalid);
                //}
                if (request.Type == "momocash")
                {
                    if (request.Amount < 20000)
                    {
                        return new APIResponse((int)ResponseCode.BankAmountInvalid);
                    }
                    if (!CheckValidMobile(request.BankAccountNumber))
                        return new APIResponse((int)ResponseCode.ParameterInvalid);
                }
                else
                {
                    if (transaction.PartnerCode == "tiger" || transaction.PartnerCode == "winplay" || transaction.PartnerCode == "wolf")
                    {
                        if (request.Amount < 50000 || request.Amount > 50000000)
                        {
                            return new APIResponse((int)ResponseCode.BankAmountInvalid);


                        }
                        if (request.Amount > 10000000)
                            TelegramNotify.SendTeleV2("-4031707938", "Có lệnh rút tiền lớn  " + request.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + transaction.PartnerCode + " Mã giao dịch " + request.RefCode);
                    }

                    else
                    {
                        if (request.Amount < 50000 || request.Amount > 12000000)
                        {
                            return new APIResponse((int)ResponseCode.BankAmountInvalid);


                        }
                    }

                }

                // Bước: Ghi log giao dịch
                step = 2;

                _BankCashAPI.ReturnValue = _BankCashAPI.Add();
                // Nếu thêm giao dịch không hợp lệ
                if (_BankCashAPI.ReturnValue < 0)
                {
                    if (_BankCashAPI.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "KZBank", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.BankAccountNumber, request.RefCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse((int)_BankCashAPI.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;
                if (transaction.PartnerCode == "tiger" || transaction.PartnerCode == "winplay" || transaction.PartnerCode == "wolf")
                {
                    if (request.Amount > 10000000)
                    {
                        _BankCashAPI.Status = -2;
                        _BankCashAPI.LogContent = "Chờ duyệt";
                        _BankCashAPI.LastTime = DateTime.Now;
                        _BankCashAPI.Update();
                        return new APIResponse(1);
                    }

                }
                CashRespone cashResult = new CashRespone();
                CashRequest _cashRequest = new CashRequest();

                _cashRequest.partner = Partner;
                _cashRequest.requestId = _BankCashAPI.ReturnValue.ToString();
                _cashRequest.user_id = _BankCashAPI.ReturnValue.ToString();
                _cashRequest.bank_code = request.BankName;
                _cashRequest.bank_account = request.BankAccountNumber.ToString();
                _cashRequest.bank_fullname = request.BankAccountName.ToString();
                _cashRequest.account = request.BankAccountNumber.ToString();
                _cashRequest.amount = request.Amount.ToString();
                _cashRequest.callback_url = callbackbank;
                _cashRequest.signature = Utils.Encrypts.MD5(_BankCashAPI.ReturnValue + request.BankAccountNumber.ToString() + request.Amount.ToString() + PartnerCode);

                var url = urlBaseService;

                //var data = $"partner=GRB&bank_code={_BankCashAPI.BankCode}&amount={request.Amount}&bank_account={request.BankAccountNumber}&bank_fullname={request.BankAccountName}&requestId={_BankCashAPI.ReturnValue}&user_id={_BankCashAPI.ReturnValue}&signature={signature}&callback_url={callbackbank}";
                if (request.Type == "momocash")
                {
                    url = urlBaseServiceMomo;
                    _cashRequest.callback_url = callbackmomo;
                    // data = $"partner=GRB&amount={request.Amount}&account={request.BankAccountNumber}&requestId={_BankCashAPI.ReturnValue}&user_id={_BankCashAPI.ReturnValue}&signature={signature}&callback_url={callbackbank}";
                }
                NLogLogger.Info(new string[] { "KZCash", "Request", _BankCashAPI.ReturnValue.ToString(), url, serializer.Serialize(_cashRequest) });
                var response = PostTask2(url, _cashRequest);
                NLogLogger.Info(new string[] { "KZCash", "Response", _BankCashAPI.ReturnValue.ToString(), response });

                var resObj = serializer.Deserialize<KZBankLib.CashRespone>(response);
                if (resObj.status == 1)
                {
                    //bắt bot
                    //TelegramNotify.SendNotify(-845553760, "Có lệnh bank cần duyệt !!!");
                    //System.Threading.Thread.Sleep(300);

                    Action<string, long, string, string,string> send = UpdatePartnerBalance;
                    var asynSend = send.BeginInvoke(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), _BankCashAPI.BankCode, String.Format("Trừ tiền rút bank số tiền: {2} mgd: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", ".")), "BankOut_" + _BankCashAPI.TransactionID.ToString(), null, null);


                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                }
                else
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                    _BankCashAPI.Status = -1;
                    _BankCashAPI.LogContent = serializer.Serialize(cashResult);
                    _BankCashAPI.LastTime = DateTime.Now;
                    _BankCashAPI.Update();
                }


            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "KZBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "KZBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "KZBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "KZBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "KZBank", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _BankCashAPI.Status = _APIResponse.ResponseCode;
            }


            return _APIResponse;


        }

        public APIResponse Callback(KZBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var signature = Utils.Encrypts.MD5(callback.request_id + callback.status + callback.amount + PartnerCode);
            if (signature != callback.signature)
            {
                NLogLogger.Info(new string[] { "KZ", "Callback", "Signature Failed", signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }
            if (callback.status == 2)
            {
                var order = new BankCashAPI().Get(long.Parse(callback.request_id));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "KZ", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status == 0)
                {
                    //apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                    //{
                    //    ResponseContent = serializer.Serialize(new DataCallback()
                    //    {
                    //        RefCode = order.RefCode,
                    //        TransactionID = order.TransactionID.ToString(),
                    //        Amount = order.Amount,
                    //    })
                    //};

                    order.Status = (int)ResponseCode.TransactionSuccessful;
                    order.TotalAmount = order.Amount;
                    order.LastTime = DateTime.Now;
                    order.Mobile = "";
                    order.Update();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);
                    //Action<string, long, string, string> send = UpdatePartnerBalance;
                    //var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(order.Amount), order.BankCode, String.Format("Trừ tiền rút bank số tiền: {2} mgd: {0}-{1}", order.TransactionID, order.BankCode, Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", ".")), null, null);

                    //Callback for Partner
                    if (!string.IsNullOrEmpty(order.ReturnUrl))
                    {
                        var partner = new Partners().Get(order.PartnerCode);
                        var datacb = new DataCallback()
                        {
                            RefCode = order.RefCode,
                            TransactionID = order.TransactionID.ToString(),
                            Amount = order.Amount,
                        };
                        apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {

                        };
                        apiResponse.ResponseContent = serializer.Serialize(datacb);
                        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        //apiResponse.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await KZBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode).ConfigureAwait(false));
                    }
                }


            }
            else
            {
                var order = new BankCashAPI().Get(long.Parse(callback.request_id));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status == 0)
                {

                    order.Status = -1;
                    order.TotalAmount = 0;
                    order.LastTime = DateTime.Now;
                    order.Mobile = "";
                    order.LogContent = callback.msg;
                    order.Update();

                    Action<string, long, string, string,string> send = UpdatePartnerBalanceTopup;
                    var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(order.Amount), order.BankCode, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", order.TransactionID, order.BankCode, Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + order.TransactionID.ToString(), null, null);

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);
                    //Callback for Partner
                    if (!string.IsNullOrEmpty(order.ReturnUrl))
                    {
                        var partner = new Partners().Get(order.PartnerCode);
                        var datacb = new DataCallback()
                        {
                            RefCode = order.RefCode,
                            TransactionID = order.TransactionID.ToString(),
                            Amount = order.Amount,

                        };
                        apiResponse.ResponseContent = serializer.Serialize(datacb);
                        apiResponse.Description = callback.msg;
                        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        //datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await KZBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode).ConfigureAwait(false));
                    }

                }


            }

            return apiResponse;
        }

        private void UpdatePartnerBalance(string PartnerCode, long Amount, string Type, string Note, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Type, Note.ToString(), RefCode });
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
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
                if (listpartnerDiscount == null)
                {
                    TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                    return;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
                decimal ck = _partnerDiscount.DiscountBANKOUTTRANFER;
                if (Type == "MOMO")
                    ck = _partnerDiscount.DiscountMOMOOUT;
                if (ck == 0)
                {
                    TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                long realAmount = Convert.ToInt64(Amount * ck);
                //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
                new Users().Deduct(realAmount, user.UserName, PartnerCode, Note, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }


        }
        private void UpdatePartnerBalanceTopup(string PartnerCode, long Amount, string Type, string TranId, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Type, TranId.ToString(), RefCode });
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
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
                if (listpartnerDiscount == null)
                {
                    TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                    return;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
                decimal ck = _partnerDiscount.DiscountBANKOUTTRANFER;
                if (Type == "MOMO")
                    ck = _partnerDiscount.DiscountMOMOOUT;
                if (ck == 0)
                {
                    TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                long realAmount = Convert.ToInt64(Amount * ck);
                //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
                new Users().Topup(realAmount, user.UserName, PartnerCode, TranId, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }


        }
    }

}


