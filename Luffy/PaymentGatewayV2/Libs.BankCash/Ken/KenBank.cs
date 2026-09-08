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
using static Libs.BankCash.Ken.KenBankLib;

namespace Libs.BankCash.Ken
{
    public class KenBank : IBankCashHandler
    {
        // Production MoBo
        // Production MoBo
        private const string urlBaseService = "http://128.199.254.52/api/bank/register";
        private const string urlBaseServiceMomo = "http://128.199.254.52/api/mmo/register";


        private const string callbackbank = "https://bank.kudopay.xyz/Ken/BankOut.ashx";
        private const string callbackmomo = "https://bank.kudopay.xyz/Ken/MomoOut.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string Partner = "THA";

        private const string PartnerCode = "0175f2c7e8df1763305d41060248f9e5";
        private const string serviceIp = "139.180.206.12";
        public APIResponse ReCallBack(long id)
        {
            throw new NotImplementedException();
        }
        public APIResponse Check(APITransaction transaction)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();




            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public ResponeDetail Get(string refcode, string partnercode)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var result = new ResponeDetail { status = -99, desc = "" };
            var order = new BankCashAPI().GetByRefcode(refcode, partnercode);
            if (order == null)
            {
                result = new ResponeDetail { status = -2, desc = "Giao dịch không tồn tại" };
            }

            if (order.Status >= 1)
            {
                result = new ResponeDetail { status = 1, desc = "Thành công" };
            }
            if (order.Status == 0)
            {
                result = new ResponeDetail { status = 0, desc = "Đang xử lý" };
            }
            if (order.Status < 0)
            {
                result = new ResponeDetail { status = order.Status, desc = order.LogContent.Replace("Add Order", "") };
            }
            return result;
        }
        public APIResponse Cash(APITransaction transaction)
        {
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


            APIResponse _APIResponse = new APIResponse();
            var _BankCashAPI = new BankCashAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = KenBankLib.GenOrderCode(),
                OrderInfo = string.Empty,
                Amount = request.Amount,
                TotalAmount = request.Amount,
                Currency = "VND",
                ReturnUrl = request.CallbackUrl,
                RequestTime = 0,
                Signature = "",
                LogContent = "Add Order",
                Note = request.Note,
                BankCode = request.BankName,
                FullName = request.AccountName,
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

                if (request.Amount < 10000 || request.Amount > 30000000)
                {
                    return new APIResponse((int)ResponseCode.BankAmountInvalid);
                }
                //if (request.Type == "momocash")
                //{
                //    if (request.Amount < 20000)
                //    {
                //        return new APIResponse((int)ResponseCode.BankAmountInvalid);
                //    }
                //    if (!CheckValidMobile(request.BankAccountNumber))
                //        return new APIResponse((int)ResponseCode.ParameterInvalid);
                //}
                // Bước: Ghi log giao dịch
                step = 2;

                _BankCashAPI.ReturnValue = _BankCashAPI.Add();
                // Nếu thêm giao dịch không hợp lệ
                if (_BankCashAPI.ReturnValue < 0)
                {
                    if (_BankCashAPI.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "KenBank", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.BankAccountNumber, request.RefCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse((int)_BankCashAPI.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;
                if (request.Amount >= 15000000)
                {
                    _BankCashAPI.Status = -2;
                    _BankCashAPI.LogContent = "Đợi duyệt";
                    _BankCashAPI.LastTime = DateTime.Now;
                    _BankCashAPI.Update();
                    return new APIResponse((int)ResponseCode.TransactionSuccessful);
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


                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<KenBankLib.CashRespone>(response);
                    if (resObj.status == 1)
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    }
                    else
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _BankCashAPI.Status = -1;
                        _BankCashAPI.LogContent = serializer.Serialize(resObj);
                        _BankCashAPI.LastTime = DateTime.Now;
                        _BankCashAPI.Update();
                        if (resObj.msg.Contains("giới hạn cashout") || resObj.msg.Contains("bảo trì"))
                        {
                            var provider = new Providers().Get("kenbankcash");
                            provider.Status = 0;
                            provider.Update();
                            TelegramNotify.SendWarning("-4197623889", resObj.msg);
                        }
                    }
                }
                else
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                    _BankCashAPI.Status = -1;
                    _BankCashAPI.LogContent = "System Busy";
                    _BankCashAPI.LastTime = DateTime.Now;
                    _BankCashAPI.Update();
                }



            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "KenBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "KenBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "KenBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "KenBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "KenBank", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _BankCashAPI.Status = _APIResponse.ResponseCode;
            }


            return _APIResponse;


        }

        public APIResponse Callback(KenBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);
            var ipRequest = Libs.Utils.IPAddress.Get();
            //NLogLogger.Info(new string[] { "M32", "Callback", "Ip Invalid", ipRequest, serviceIp });
            //if (ipRequest != serviceIp)
            //{
            //    NLogLogger.Info(new string[] { "M32", "Callback", "Ip Invalid", ipRequest, serviceIp });
            //    return new APIResponse((int)ResponseCode.IpInvalid);
            //}
            //va
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
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status < 1)
                {


                    order.Status = (int)ResponseCode.TransactionSuccessful;
                    order.TotalAmount = order.Amount;
                    order.LastTime = DateTime.Now;
                    order.Mobile = "";
                    order.Update();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);

                    if (order.PartnerCode == "azt")
                    {
                        Action<string, long, string, long> send = UpdatePartnerBalance;
                        var asynSend = send.BeginInvoke(order.PartnerCode, int.Parse(callback.amount), order.BankCode, order.TransactionID, null, null);
                    }
                }
                apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);

                //Callback for Partner
                if (!string.IsNullOrEmpty(order.ReturnUrl))
                {
                    var partner = new Partners().Get(order.PartnerCode);
                    var datacb = new DataCallback()
                    {
                        Status = 1,
                        RefCode = order.RefCode,
                        TransactionID = order.TransactionID.ToString(),
                        Amount = int.Parse(callback.amount),
                    };
                    datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                    Task.Run(async () => await KenBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
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
                //if (order.Status < 1)
                //{
                order.LogContent = callback.msg;
                order.Status = -1;
                order.TotalAmount = 0;
                order.LastTime = DateTime.Now;
                order.Mobile = "";
                order.Update();

                //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);


                //}
                if (callback.msg.Contains("giới hạn cashout")|| callback.msg.Contains("bảo trì"))
                {
                    var provider = new Providers().Get("kenbankcash");
                    provider.Status = 0;
                    provider.Update();
                    TelegramNotify.SendWarning("-4197623889", callback.msg);
                    TelegramNotify.SendWarning("-4089257485", callback.msg + " @akari_mitani96 @Genie20000");

                }

                //Callback for Partner
                if (!string.IsNullOrEmpty(order.ReturnUrl))
                {
                    var partner = new Partners().Get(order.PartnerCode);
                    var datacb = new DataCallback()
                    {
                        RefCode = order.RefCode,
                        Desciption = callback.msg,
                        TransactionID = order.TransactionID.ToString(),
                        Amount = 0,
                        Status = -1
                    };
                    datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                    Task.Run(async () => await KenBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
                }
            }

            return apiResponse;
        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, string Type, long TranId)
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Type, TranId.ToString() });
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
            if (listpartnerDiscount == null)
                return;
            if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                return;

            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
            decimal ck = _partnerDiscount.DiscountBANKTRANFER;
            if (Type == "MOMO")
                ck = _partnerDiscount.DiscountMOMO;
            if (ck == 0)
                return;

            long realAmount = Convert.ToInt64(Amount * ck) + Amount;
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Partners().Deduct(realAmount, PartnerCode, $"Trừ tiền rút bank mã giao dịch {TranId}");

        }

    }

}
