using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using System.Security.Cryptography;

using System.Configuration;
using System.Threading;

using System.Threading.Tasks;
using Libs.BankGate.Entity;


namespace Libs.BankDirect.M32VTP
{
    public class M32VTPBank : IBankDirectV2Handler
    {

        private const string username = "LUFFY";

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string secretKey = "Uqi63728bshHFNIowueZZ8272";
        private const string urlBaseService = "http://108.160.140.162:6060/vtp/GETINFO";
        private const string serviceIp = "108.160.140.162";
        public APIResponse CheckTrans(APITransaction transaction)
        {
            //var urlService = urlBaseService + string.Format("/v1/orders/{0}/", serializer.Deserialize<BankDirectV2Service.CheckOrderRequest>(transaction.RequestContent).OrderNo);
            //NLogLogger.Info(new string[] { "M32VTP", "CheckTrans Request", urlService });
            //var response = Task.Run(async () => await M32VTPBankLib.GetTask(urlService)).Result;
            //NLogLogger.Info(new string[] { "M32VTP", "CheckTrans Response", response, urlService });

            //if (!string.IsNullOrEmpty(response))
            //{
            //    var resObj = serializer.Deserialize<M32VTPBankLib.OrderResponse>(response);
            //    var orderRes = new Order()
            //    {
            //        Status = resObj.status,
            //        Amount = Convert.ToInt32(resObj.note.target_amount),
            //        OrderNo = resObj.code,
            //        Timeout = resObj.timeout
            //    };

            //    //InitBank 



            //    return new APIResponse((int)ResponseCode.TransactionSuccessful)
            //    {
            //        ResponseContent = serializer.Serialize(orderRes)
            //    };

            //}

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse GetBanksV2(string ParnerId, string AccoutName)
        {

            var urlService = urlBaseService;
            var bankRequest = new M32VTPBankLib.BankRequest()
            {
                requestTime = DateTime.Now.ToString("yyyyMMddHHmmss"),

                userName = username
            };
            bankRequest.authKey = Utils.Encrypts.MD5(bankRequest.requestTime + "|" + bankRequest.type + "|" + secretKey);

            NLogLogger.Info(new string[] { "M32VTP", "GetBanks Request", urlService, serializer.Serialize(bankRequest) });
            var response = Task.Run(async () => await M32VTPBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
            NLogLogger.Info(new string[] { "M32VTP", "GetBanks Response", response, urlService });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<M32VTPBankLib.BankResponse>(response);
                if (resObj.errorCode == "0")
                {
                    var bank = serializer.Deserialize<M32VTPBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccount>();
                    //var listBankObj = new List<BankAccount>();
                    listBankObj.Add(new BankAccount()
                    {
                       
                        //AccountNumber = bank.name,
                        //AccountName = bank.phone,
                        BankCode = "VTP"
                    });
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(listBankObj)
                    };
                }



            }

            return new APIResponse((int)ResponseCode.TransactionFailed);


        }
        public APIResponse GetBanks()
        {
            var urlService = urlBaseService;
            var bankRequest = new M32VTPBankLib.BankRequest()
            {
                requestTime = DateTime.Now.ToString("yyyyMMddHHmmss"),

                userName = username
            };
            bankRequest.authKey = Utils.Encrypts.MD5(bankRequest.requestTime + "|" + bankRequest.type + "|" + secretKey);

            NLogLogger.Info(new string[] { "M32VTP", "GetBanks Request", urlService, serializer.Serialize(bankRequest) });
            var response = Task.Run(async () => await M32VTPBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
            NLogLogger.Info(new string[] { "M32VTP", "GetBanks Response", response, urlService });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<M32VTPBankLib.BankResponse>(response);
                if (resObj.errorCode == "0")
                {
                    var bank = serializer.Deserialize<M32VTPBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccount>();
                    //listBankObj.Add(new BankAccount()
                    //{

                    //    BankAccountName = bank.name,
                    //    BankAccountNumber = bank.phone,
                    //    BankName = "VTP"
                    //});
                    //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    //{
                    //    ResponseContent = serializer.Serialize(listBankObj)
                    //};
                }



            }

            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public APIResponse Order(APITransaction transaction)
        {
            var tran = serializer.Deserialize<BankDirectV2Service.OrderRequest>(transaction.RequestContent);
            if (tran.Amount < 1000 || tran.Amount > 20000000)
            {
                return new APIResponse((int)ResponseCode.BankAmountInvalid);
            }

            var addTran = new BankGateAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = "A", // Tự động lấy bằng ID Table
                OrderInfo = string.Empty,
                Amount = tran.Amount,
                TotalAmount = 0, // Hứng tiền sau khi xử lý thật
                Currency = "VND",
                ReturnUrl = tran.CallbackUrl,
                RequestTime = 0,
                Signature = Encrypts.MD5(DateTime.Now.ToString()),
                LogContent = "Add Order",
                BankCode = tran.BankName,
                FullName = tran.AccountName,
                Mobile = string.Empty,
                RefCode = tran.RefCode,
                BankAccountName = tran.BankAccountName,
                BankAccountNumber = tran.BankAccountNumber
            };
            var addId = addTran.Add();

            //var orderRes = new Order()
            //{
            //    Status = "pending",
            //    Amount = tran.Amount,
            //    RefCode = tran.RefCode,
            //    OrderNo = addId.ToString(),
            //    Timeout = 120,
            //    BankName = "VTP",
            //    BankAccountNumber = tran.BankAccountNumber,
            //    BankAccountName = tran.BankAccountName
            //};

            if (addId > 0)
            {
                var urlService = urlBaseService;
                var bankRequest = new M32VTPBankLib.BankRequest()
                {
                    requestTime = DateTime.Now.ToString("yyyyMMddHHmmss"),

                    userName = username
                };
                bankRequest.authKey = Utils.Encrypts.MD5(bankRequest.requestTime + "|" + bankRequest.type + "|" + secretKey);

                NLogLogger.Info(new string[] { "M32VTP", "GetBanks Request", urlService, serializer.Serialize(bankRequest) });
                var response = Task.Run(async () => await M32VTPBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
                NLogLogger.Info(new string[] { "M32VTP", "GetBanks Response", response, urlService });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<M32VTPBankLib.BankResponse>(response);
                    if (resObj.errorCode == "0")
                    {
                        var bank = serializer.Deserialize<M32VTPBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                        addTran.BankAccountName = bank.name;
                        addTran.BankAccountNumber = bank.phone;
                        addTran.OrderNo = addId.ToString();
                        addTran.UpdateBank();
                        var orderRes = new Order()
                        {
                            Status = resObj.errorCode,
                            Amount = tran.Amount,
                            RefCode = tran.RefCode,
                            OrderNo = addId.ToString(),
                            Timeout = 30,
                            //Url = resObj.redirectLink,
                            BankName = tran.BankName,
                            BankAccountNumber = bank.phone,
                            BankAccountName = bank.name
                        };

                        return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {
                            ResponseContent = serializer.Serialize(orderRes)
                        };
                        //var listBankObj = new List<BankAccount>();
                        //listBankObj.Add(new BankAccount()
                        //{

                        //    BankAccountName = bank.name,
                        //    BankAccountNumber = bank.phone,
                        //    BankName = "VTP"
                        //});
                        //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        //{
                        //    ResponseContent = serializer.Serialize(listBankObj)
                        //};
                    }



                }
                //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                //{
                //    ResponseContent = serializer.Serialize(orderRes)
                //};
            }
            else
            {
                switch (addId)
                {
                    case (int)ResponseCode.AccessDenied:
                        return new APIResponse((int)ResponseCode.AccessDenied);
                    case (int)ResponseCode.TransactionDuplicate:
                        return new APIResponse((int)ResponseCode.TransactionDuplicate);
                }
            }




            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public APIResponse Callback(M32VTPBankLib.Callback callback)
        {
            var ipRequest = Libs.Utils.IPAddress.Get();
            if (ipRequest != serviceIp)
            {
                NLogLogger.Info(new string[] { "M32VTP", "Callback", "Ip Invalid", ipRequest, serviceIp });
                return new APIResponse((int)ResponseCode.IpInvalid);
            }

            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            //var signature = Utils.Encrypts.MD5(callback.requestTime + "|" + callback.message + "|" + callback.money + "|" + callback.phone + "|" + secretKey);
            //if (signature != callback.authKey)
            //{
            //    NLogLogger.Info(new string[] { "M32VTP", "Callback", "Signature Failed", signature, callback.authKey });
            //    return new APIResponse((int)ResponseCode.SignatureInvalid);
            //}
            var Partner = new Partners().Get(60);
            var AcountName = callback.message;

            var neworder = new BankGateAPI()
            {
                PartnerID = 60,
                PartnerCode = "hyn3",
                ProviderCode = "m32vtp",
                OrderNo = callback.message,
                OrderInfo = callback.requestTime,
                Amount = Convert.ToDecimal(callback.money),
                TotalAmount = Convert.ToDecimal(callback.money),
                Currency = "VND",
                ReturnUrl = "",
                RequestTime = 0,
                Signature = Encrypts.MD5(DateTime.Now.ToString()),
                LogContent = "Add Order " + callback.message,
                BankCode = "VTP",
                FullName = AcountName,
                Mobile = callback.phone,
                RefCode = callback.requestTime,
                BankAccountName = string.Empty,
                BankAccountNumber = "",
                LastTime = DateTime.Now,
                Status = (int)ResponseCode.TransactionSuccessful
            };
            //var keyCache = String.Format("MomoAccount:{0}", callback.account_receive);
            //var dataCache = DataCaching.GetCache<string>(keyCache);
            //if (dataCache != null)
            //{
            //    neworder.BankAccountName = dataCache.ToString();
            //}
            var addId = neworder.AddV2();
            if (addId < 0)
            {
                //neworder.Status = (int)addId;
                //neworder.Update();
                return new APIResponse((int)addId);
            }
            var datacb = new DataCallback()
            {
                Content = neworder.OrderNo,
                Mobile = callback.phone,
                TransId = neworder.RefCode,
                Amount = Convert.ToInt32(callback.money),
                BankCode = "VTP"
            };
            datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);


            //NLogLogger.Info(new string[] { "M32", "PartnerCallback", serializer.Serialize(datacb), Partner.SMSPlusUrl });
            //Callback for Partner
            if (!string.IsNullOrEmpty(Partner.SMSPlusUrl))
            {


                Task.Run(async () => await M32VTPBankLib.CallbackJson(Partner.SMSPlusUrl, serializer.Serialize(datacb)).ConfigureAwait(false));
            }



            return apiResponse;

        }
    }

}


