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

using static Libs.BankCash.Drum.DrumBankLib;


namespace Libs.BankCash.Drum
{
    public class DrumBank : IBankCashHandler
    {
        // Production MoBo
        private const string urlBaseService = "http://127.0.0.1:9001/MomoService.ashx";
        private const string callbackurl = "http://127.0.0.1:1592/Callback/DrumCash.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();

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
            //DrumaScriptSerializer serializer = new DrumaScriptSerializer();
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
                OrderNo = DrumBankLib.GenOrderCode(),
                OrderInfo = string.Empty,
                Amount = request.Amount,
                TotalAmount = request.Amount,
                Currency = "VND",
                ReturnUrl = request.CallbackUrl,
                RequestTime = 0,
                Signature = "",
                LogContent = "Add Order",
                Note = request.RefCode,
                BankCode = request.BankName.ToUpper(),
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

                if (request.Amount < 2000 || request.Amount > 2000000)
                {
                    return new APIResponse((int)ResponseCode.BankAmountInvalid);
                }
                if (request.Type == "momocash")
                {
                    if (!CheckValidMobile(request.BankAccountNumber))
                        return new APIResponse((int)ResponseCode.ParameterInvalid);
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
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.BankAccountNumber, request.RefCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse((int)_BankCashAPI.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;
                CashBankRequest _cashRequest = new CashBankRequest();

                _cashRequest.MomoName = request.BankAccountName;
                _cashRequest.MomoId = request.BankAccountNumber;
                _cashRequest.Note = _BankCashAPI.ReturnValue.ToString();
                _cashRequest.Amount = request.Amount;
                _cashRequest.CallbackUrl = callbackurl;
                _cashRequest.TransId = _BankCashAPI.ReturnValue.ToString();
                var signature = "";
                var requestData = new RequestData()
                {
                    PartnerCode = "order",
                    CommandCode = "CASHOUT",
                    RequestContent = serializer.Serialize(_cashRequest),
                    Signature = signature
                };
                if (transaction.PartnerCode == "247pay")
                    requestData.PartnerCode = "24h";
                CashRespone cashResult = new CashRespone();

                NLogLogger.Info(new string[] { "MDrum", "Cash Request",serializer.Serialize(requestData), urlBaseService
                });
                var response = Task.Run(async () => await PostTask(urlBaseService, serializer.Serialize(requestData))).Result;
                NLogLogger.Info(new string[] { "MDrum", "Cash Response", response, urlBaseService
            });


                var resObj = serializer.Deserialize<DrumBankLib.CashRespone>(response);
                if (resObj.ResponseCode == 1)
                {
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
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _BankCashAPI.Status = _APIResponse.ResponseCode;
            }


            return _APIResponse;


        }

        public APIResponse Callback(DrumBankLib.BankResponse callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            //var signature = Utils.Encrypts.MD5(callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + pw);
            //if (signature != callback.signature)
            //{
            //    NLogLogger.Info(new string[] { "VNPAY", "Callback", "Signature Failed", signature, callback.signature });
            //    return new APIResponse((int)ResponseCode.SignatureInvalid);
            //}
            var newclObj = serializer.Deserialize<DrumBankLib.Callback>(callback.ResponseContent);
            if (callback.ResponseCode > 0)
            {

                var order = new BankCashAPI().Get(newclObj.transId);
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "VNPAY", "Callback", "Order NULL", serializer.Serialize(callback) });
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
                        Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID).ConfigureAwait(false));
                    }

                }


            }
            if (callback.ResponseCode < 0)
            {
                var order = new BankCashAPI().Get(newclObj.transId);
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "Drum", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status == 0)
                {

                    order.Status = -1;
                    order.TotalAmount = 0;
                    order.LastTime = DateTime.Now;
                    order.Mobile = "";
                    order.LogContent = callback.Description;
                    order.Update();

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
                        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        //datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID).ConfigureAwait(false));
                    }
                }


            }

            return apiResponse;
        }


    }

}
