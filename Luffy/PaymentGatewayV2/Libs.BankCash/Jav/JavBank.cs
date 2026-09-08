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

using static Libs.BankCash.Jav.JavBankLib;


namespace Libs.BankCash.Jav
{
    public class JavBank : IBankCashHandler
    {
        // Production MoBo

        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        private string urlService = "https://bankgate.coroach.xyz//VPGJsonService.ashx";
        private string partnerKey1 = "941d69b5bfe82f513072c8545dd40bb2";
        private string partnerCode1 = "hyn5";

        private string partnerKey2 = "941d69b5bfe82f513072c8545dd40bb2";
        private string partnerCode2 = "hyn5";
        private string serviceCode = "bankcash";
        private readonly string commandCode = "cash";
        private const string serviceIp = "149.28.130.246";

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


            APIResponse _APIResponse = new APIResponse();
            var _BankCashAPI = new BankCashAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = JavBankLib.GenOrderCode(),
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

                if (request.Amount < 10000 || request.Amount > 20000000)
                {
                    return new APIResponse((int)ResponseCode.BankAmountInvalid);
                }
                if (request.Type == "momocash")
                {
                    if (request.Amount < 20000 | request.Amount >5000000)
                    {
                        return new APIResponse((int)ResponseCode.BankAmountInvalid);
                    }
                    if (!CheckValidMobile(request.BankAccountNumber))
                        return new APIResponse((int)ResponseCode.ParameterInvalid);
                }
                // Bước: Ghi log giao dịch
                step = 2;

                _BankCashAPI.ReturnValue = _BankCashAPI.Add();
                // Nếu thêm giao dịch không hợp lệ
                if (_BankCashAPI.ReturnValue < 0)
                {
                    if (_BankCashAPI.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "HynBank", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.BankAccountNumber, request.RefCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse((int)_BankCashAPI.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;

                var partnerCode = partnerCode1;
                var partnerKey = partnerKey1;
                //if(transaction.PartnerCode=="panpan")
                //{
                //    partnerCode = partnerCode1;
                //    partnerKey = partnerKey1;
                //}    
                //string commandCode = "cash";
                var order = new OrderRequest2();
                order.Type = "momocash";
                order.AccountName = DateTime.Now.ToString("ddMMyyyyy");
                order.Amount = request.Amount;
                order.AppCode = " ";
                order.Note = " ";
                order.CallbackUrl = "https://pm.kudopay.xyz/rutbankcoroach";
                order.RefCode = _BankCashAPI.ReturnValue.ToString();
                order.BankName = "momo";
                order.BankAccountName = request.BankAccountName;
                order.BankAccountNumber = request.BankAccountNumber;
                if (request.Type != "momocash")
                {
                    order.Type = "bankcash";
                    order.BankName = request.BankName;
                }

                var requestContent = serializer.Serialize(order);
                var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
                var requestData = new RequestData()
                {
                    PartnerCode = partnerCode,
                    CommandCode = commandCode,
                    RequestContent = requestContent,
                    ServiceCode = serviceCode,
                    Signature = signature
                };

                var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));

                if (!string.IsNullOrEmpty(serviceResponse))
                {
                    var resObj = serializer.Deserialize<APIResponse>(serviceResponse);

                    //var resObj = serializer.Deserialize<HynBankLib.CashRespone>(response);
                    if (resObj.ResponseCode == 1)
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    }
                    else
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _BankCashAPI.Status = -1;
                        _BankCashAPI.LogContent = resObj.Description;
                        _BankCashAPI.LastTime = DateTime.Now;
                        if(resObj.ResponseCode==-352)
                        {
                            _BankCashAPI.LogContent = "Bank " + order.BankName + "bao tri";
                        }    
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
                        NLogLogger.Info(new string[] { "HynBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "HynBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "HynBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "HynBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "HynBank", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _BankCashAPI.Status = _APIResponse.ResponseCode;
            }


            return _APIResponse;


        }

        public APIResponse Callback(JavBankLib.CallbackResponse response)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            //var signature = Utils.Encrypts.MD5(callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + pw);
            //if (signature != callback.signature)
            //{
            //    NLogLogger.Info(new string[] { "HynBank", "Callback", "Signature Failed", signature, callback.signature });
            //    return new APIResponse((int)ResponseCode.SignatureInvalid);
            //}
            var callback = serializer.Deserialize<JavBankLib.Callback>(response.ResponseContent);
            if (response.ResponseCode == 1)
            {
                var order = new BankCashAPI().Get(long.Parse(callback.RefCode));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Order NULL", serializer.Serialize(callback) });
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
                    order.OrderInfo = callback.MomoTransId;
                    order.Mobile = "";
                    order.Update();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);

                    //if (order.PartnerCode == "azt")
                    //{
                    //    Action<string, long, string, long> send = UpdatePartnerBalance;
                    //    var asynSend = send.BeginInvoke(order.PartnerCode, int.Parse(callback.Amount.ToString()), order.BankCode, order.TransactionID, null, null);
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
                            Amount = int.Parse(callback.Amount.ToString().Replace(".00", "")),
                        };
                        NLogLogger.Info(new string[] { "Jav", "PartnerCallback", serializer.Serialize(datacb), order.ReturnUrl });
                        datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await JavBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
                    }
                }
                apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);

              
            }
            if (response.ResponseCode < 1)
            {
                var order = new BankCashAPI().Get(long.Parse(callback.RefCode));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status < 1)
                {
                    order.LogContent = response.Description;
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
                            TransactionID = order.TransactionID.ToString(),
                            Amount = 0,
                            Status = -1
                        };
                        datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await JavBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb)).ConfigureAwait(false));
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
