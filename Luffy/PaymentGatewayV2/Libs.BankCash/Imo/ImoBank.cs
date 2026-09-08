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
using static Libs.BankCash.Imo.ImoBankLib;

namespace Libs.BankCash.Imo
{
    public class ImoBank : IBankCashHandler
    {
        // Production MoBo
        private const string urlBaseService = "http://imopay.vnm.bz:10007/api/";
        private const string callbackurl = "https://bank.kudopay.xyz/Callback/ImoCallback.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string secretKey = "0f43db36-1aa4-4389-8f03-5218912feed9";
        private const string secretKey2 = "0f43db36-1aa4-4389-8f03-5218912feed9";
        private const string pw = "113355a@";
        private const string pw2 = "113355a@";
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

            if (request.BankName.ToLower() == "vtp")
            {
                return new APIResponse((int)ResponseCode.BankCodeInvalid);
            }    
                APIResponse _APIResponse = new APIResponse();
            var _BankCashAPI = new BankCashAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = ImoBankLib.GenOrderCode(),
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

                if (request.Amount < 50000 || request.Amount > 30000000)
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
                        NLogLogger.Info(new string[] { "ImoBank", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.BankAccountNumber, request.RefCode, "Error Insert Data" });
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
                var signature = Utils.Encrypts.MD5(request.BankAccountNumber + request.Amount.ToString() + _BankCashAPI.ReturnValue.ToString() + pw);
                var urlService = $"{urlBaseService}/Bank/ChargeOut?apiKey={secretKey}&bank_code={_BankCashAPI.BankCode}&amount={request.Amount}&bank_account={request.BankAccountNumber}&bank_accountName={request.BankAccountName}&requestId={_BankCashAPI.ReturnValue}&msg={_BankCashAPI.ReturnValue}&signature={signature}&callback={callbackurl}";
                if (request.Type == "momocash")
                {
                    urlService = $"{urlBaseService}MM/ChargeOut?apiKey={secretKey}&amount={request.Amount}&account={request.BankAccountNumber}&requestId={_BankCashAPI.ReturnValue}&msg={_BankCashAPI.ReturnValue}&signature={signature}&callback={callbackurl}";
                }
                NLogLogger.Info(new string[] { "Imo", "Order Request", urlService });
                var response = Task.Run(async () => await ImoBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "Imo", "Order Response", response });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<ImoBankLib.CashRespone>(response);
                    if (resObj.stt == 1)
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    }
                    else
                    {
                        if (resObj.stt == 0)
                        {
                            TelegramNotify.SendWarning("1690000254", "imo hết số dư");
                            //var _Provider = new Providers().Get(51);
                            //_Provider.Status = 0;
                            //_Provider.Update();
                        }
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _BankCashAPI.Status = -1;
                        _BankCashAPI.LogContent = serializer.Serialize(resObj);
                        _BankCashAPI.LastTime = DateTime.Now;
                        _BankCashAPI.Update();
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
                        NLogLogger.Info(new string[] { "ImoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "ImoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "ImoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "ImoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "ImoBank", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _BankCashAPI.Status = _APIResponse.ResponseCode;
            }


            return _APIResponse;


        }

        public APIResponse Callback(ImoBankLib.Callback callback)
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
            var signature = Utils.Encrypts.MD5(callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + pw);
            if (signature != callback.signature)
            {
                NLogLogger.Info(new string[] { "IMO", "Callback", "Signature Failed", signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }
            if (callback.status == "success")
            {
                var order = new BankCashAPI().Get(long.Parse(callback.requestId));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "IMO", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status < 1)
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

                    if (order.PartnerCode == "azt")
                    {
                        Action<string, long, string, long> send = UpdatePartnerBalance;
                        var asynSend = send.BeginInvoke(order.PartnerCode, int.Parse(callback.chargeAmount), order.BankCode, order.TransactionID, null, null);
                    }
                }
                apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);

                //Callback for Partner
                if (!string.IsNullOrEmpty(order.ReturnUrl))
                {
                    var partner = new Partners().Get(order.PartnerCode);
                    var datacb = new DataCallback()
                    {
                        Status=1,
                        RefCode = order.RefCode,
                        TransactionID = order.TransactionID.ToString(),
                        Amount =int.Parse(callback.chargeAmount),
                    };
                    datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                    Task.Run(async () => await ImoBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
                }
            }
            if (callback.status == "deleted"|| callback.status == "timeout" || callback.status == "cancel")
            {
                var order = new BankCashAPI().Get(long.Parse(callback.requestId));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                //if (order.Status < 1)
                //{
                    order.LogContent = callback.result;
                    order.Status = -1;
                    order.TotalAmount = 0;
                    order.LastTime = DateTime.Now;
                    order.Mobile = "";
                    order.Update();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);


                //}

                //Callback for Partner
                if (!string.IsNullOrEmpty(order.ReturnUrl))
                {
                    var partner = new Partners().Get(order.PartnerCode);
                    var datacb = new DataCallback()
                    {
                        RefCode = order.RefCode,
                        Desciption = callback.result,
                        TransactionID = order.TransactionID.ToString(),
                        Amount = 0,
                        Status=-1
                    };
                    datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                    Task.Run(async () => await ImoBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
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
