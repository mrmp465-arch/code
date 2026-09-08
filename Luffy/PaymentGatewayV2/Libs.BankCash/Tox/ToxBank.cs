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
using static Libs.BankCash.Tox.ToxBankLib;

namespace Libs.BankCash.Tox
{
    public class ToxBank : IBankCashHandler
    {
        // Production MoBo
        private const string urlBaseService = "http://45.32.109.186:6677/api/";
        private const string callbackurl = "https://bank.namipay.xyz/Callback/VNPayCallbackV2.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string secretKey = "136bf507-eb74-4edf-a3dc-a9a01c58c35c";
        private const string secretKey2 = "136bf507-eb74-4edf-a3dc-a9a01c58c35c";
        private const string pw = "Tox1BM";
        private const string pw2 = "Tox1BM";
        private const string serviceIp = "139.180.206.12";
        private const string Apikey = "nwy4AsjI-CWTI1Ptb-bD3ew3pk";

        private const string ApiSecret = "gQYqar8s";
        public APIResponse ReCallBack(long id)
        {
            throw new NotImplementedException();
        }
        public APIResponse Check(APITransaction transaction)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();




            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public List<ToxBankLib.BankResponse> GetBankAPI()
        {
            string cacheKey = "BpayBanksOut";
            var result = DataCaching.GetCache<List<ToxBankLib.BankResponse>>(cacheKey);
            if (result != null)
            {
                return result;
            }
            else
            {
                var urlService = urlBaseService + "B_REQUEST_BANK_LIST/?api_key=" + Apikey;


                // NLogLogger.Info(new string[] { "Bicbic", "GetBanks Request", urlService });
                //var response = Task.Run(async () => await VNPayBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
                var response = Task.Run(async () => await ToxBankLib.GetTask(urlService)).Result;
                //NLogLogger.Info(new string[] { "IMO", "GetBanks Response", response, urlService });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<List<ToxBankLib.BankResponse>>(response);

                    DataCaching.SetCache(cacheKey, resObj, 60 * 600);
                    return resObj;
                }

            }
            return null;
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

            if (request.BankName == "VTB")
                request.BankName = "VIETINBANK";

          
            APIResponse _APIResponse = new APIResponse();
            var _BankCashAPI = new BankCashAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = ToxBankLib.GenOrderCode(),
                OrderInfo = string.Empty,
                Amount = request.Amount,
                TotalAmount = request.Amount,
                Currency = "VND",
                ReturnUrl = request.CallbackUrl,
                RequestTime = 0,
                Signature = "",
                LogContent = " ",
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
               
                // Bước: Ghi log giao dịch
                step = 2;

                _BankCashAPI.ReturnValue = _BankCashAPI.Add();
                // Nếu thêm giao dịch không hợp lệ
                if (_BankCashAPI.ReturnValue < 0)
                {
                    if (_BankCashAPI.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "ToxBank", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.BankAccountNumber, request.RefCode, "Error Insert Data" });
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
                    return new  APIResponse((int)ResponseCode.TransactionSuccessful);
                }
                CashRespone cashResult = new CashRespone();
                CashRequest _cashRequest = new CashRequest();
                var signature = Utils.Encrypts.MD5(Apikey+ _BankCashAPI.ReturnValue.ToString() + ApiSecret);
                var listbank = GetBankAPI();
                var bankno = listbank.FirstOrDefault(x => x.shortBankName.Replace(" ","") == ToxBankLib.getBankShortName(_BankCashAPI.BankCode)).bankNo;

                var urlService = $"{urlBaseService}B_REQUEST_PAY_OUT/?api_key={Apikey}&bankno={bankno}&amount={request.Amount}&account_number={request.BankAccountNumber}&account_name={request.BankAccountName.ToLower()}&request_id={_BankCashAPI.ReturnValue}&msg={_BankCashAPI.ReturnValue}&signature={signature}";
               
                NLogLogger.Info(new string[] { "VNPAY", "cash Request", urlService });
                var response = Task.Run(async () => await ToxBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "VNPAY", "cash Response", response });


                var resObj = serializer.Deserialize<ToxBankLib.CashRespone>(response);
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
                    if (resObj.message.Contains(" vượt hạn mức") ||resObj.message.Contains("han muc"))
                    {
                        //var provider = new Providers().Get("bpaybankcash");
                        //provider.Status = 0;
                        //provider.Update();
                        TelegramNotify.SendWarning("-4287153905", resObj.message);
                    }
                }


            }
            catch (Exception ex)
            {
                TelegramNotify.SendWarning("-4197623889", "lỗi bank out");
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "ToxBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "ToxBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "ToxBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "ToxBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "ToxBank", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _BankCashAPI.Status = _APIResponse.ResponseCode;
                _BankCashAPI.Update();
            }


            return _APIResponse;


        }

        public APIResponse Callback(ToxBankLib.Callback callback)
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
            var signature = Utils.Encrypts.MD5(callback.requestId + callback.transId + ApiSecret);
            if (signature != callback.signature)
            {
                NLogLogger.Info(new string[] { "VNPAY", "Callback", "Signature Failed", signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }
            if (callback.status == 1)
            {
                var order = new BankCashAPI().Get(long.Parse(callback.requestId));
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

                    //if (order.PartnerCode == "azt")
                    //{
                    //    Action<string, long, string, long> send = UpdatePartnerBalance;
                    //    var asynSend = send.BeginInvoke(order.PartnerCode, int.Parse(callback.chargeAmount), order.BankCode, order.TransactionID, null, null);
                    //}
                    //Callback for Partner
                    if (!string.IsNullOrEmpty(order.ReturnUrl))
                    {
                        var partner = new Partners().Get(order.PartnerCode);
                        var datacb = new DataCallback()
                        {
                            Status = 1,
                            RefCode = order.RefCode,
                            TransactionID = order.TransactionID.ToString(),
                            Amount = callback.amount,
                        };
                        datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await ToxBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
                    }
                }
                apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);

                
            }
            if (callback.status != 1)
            {
                var order = new BankCashAPI().Get(long.Parse(callback.requestId));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (callback.message.Contains("timeout"))
                {
                   
                    TelegramNotify.SendWarning("-4287153905", callback.message);
                }
                if (callback.message.Contains("gioi han"))
                {
                    TelegramNotify.SendWarning("-4287153905", callback.message);
                    var provider = new Providers().Get("bpaybankcash");
                    provider.Status = 0;
                    provider.Update();
                }
                if (order.Status < 1)
                {
                    order.LogContent = callback.message;
                    order.Status = -1;
                    order.TotalAmount = 0;
                    order.LastTime = DateTime.Now;
                    order.Mobile = "";
                    order.Update();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);
                    //Callback for Partner
                    if (!string.IsNullOrEmpty(order.ReturnUrl))
                    {
                        var partner = new Partners().Get(order.PartnerCode);
                        var datacb = new DataCallback()
                        {
                            RefCode = order.RefCode,
                            Desciption = callback.message,
                            TransactionID = order.TransactionID.ToString(),
                            Amount = 0,
                            Status = -1
                        };
                        datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await ToxBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
                    }

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
