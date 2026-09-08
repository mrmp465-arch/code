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
using BankAccount = Libs.BankGate.Entity.BankAccount;
using System.Threading.Tasks;
using Libs.BankGate.Entity;
using static Libs.BankDirect.HynBank.HynBankLib;
using System.Web.Routing;
using Libs.Report;

namespace Libs.BankDirect.HynBank
{
    public class HynBank : IBankDirectV2Handler
    {


        private string urlService = "https://bankgate.coroach.xyz//VPGJsonService.ashx";
        private string partnerKey1 = "941d69b5bfe82f513072c8545dd40bb2";
        private string partnerCode1 = "hyn5";
        private string serviceCode = "bankdirect";
        private string partnerKey2 = "941d69b5bfe82f513072c8545dd40bb2";
        private string partnerCode2 = "hyn5";
        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string serviceIp = "149.28.130.246";
        public APIResponse CheckTrans(APITransaction transaction)
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);

        }
        public APIResponse GetBanks()
        {
            string cacheKey = "HynBank";
            List<BankAccount> result = new List<BankAccount>();
            List<BankAccount> cache = DataCaching.GetCache<List<BankAccount>>(cacheKey);
            if (cache != null)
            {
                return new APIResponse(1) { ResponseContent = this.serializer.Serialize(cache) };
            }
            string str2 = "getbanks";
            string str3 = this.partnerCode2;
            string str4 = this.partnerKey2;
            HynBankLib.GetBankRequest request1 = new HynBankLib.GetBankRequest
            {
                Type = "banktranfer"
            };
            string str5 = this.serializer.Serialize(request1);
            string str6 = Encrypts.MD5(str3 + this.serviceCode + str2 + str5 + str4);
            HynBankLib.RequestData data = new HynBankLib.RequestData
            {
                PartnerCode = str3,
                CommandCode = str2,
                RequestContent = str5,
                ServiceCode = this.serviceCode,
                Signature = str6
            };
            string input = HynBankLib.PostJson(this.urlService, this.serializer.Serialize(data));
            APIResponse response = this.serializer.Deserialize<APIResponse>(input);
            foreach (BankAccountV6 tv in this.serializer.Deserialize<List<BankAccountV6>>(response.ResponseContent))
            {
                BankAccount item = new BankAccount
                {
                    BankCode = tv.BankName,
                    DisplayName = tv.Name
                };
                result.Add(item);
            }
            DataCaching.SetCache<List<BankAccount>>(cacheKey, result, 120);
            return new APIResponse(1) { ResponseContent = this.serializer.Serialize(result) };

        }
        public APIResponse GetBanksV2(string type, string Code)
        {
            string cacheKey = "HynBank";
            List<BankAccount> result = new List<BankAccount>();
            List<BankAccount> cache = DataCaching.GetCache<List<BankAccount>>(cacheKey);
            if (cache != null)
            {
                return new APIResponse(1) { ResponseContent = this.serializer.Serialize(cache) };
            }
            string str2 = "getbanks";
            string str3 = this.partnerCode2;
            string str4 = this.partnerKey2;
            HynBankLib.GetBankRequest request1 = new HynBankLib.GetBankRequest
            {
                Type = "banktranfer"
            };
            string str5 = this.serializer.Serialize(request1);
            string str6 = Encrypts.MD5(str3 + this.serviceCode + str2 + str5 + str4);
            HynBankLib.RequestData data = new HynBankLib.RequestData
            {
                PartnerCode = str3,
                CommandCode = str2,
                RequestContent = str5,
                ServiceCode = this.serviceCode,
                Signature = str6
            };
            string input = HynBankLib.PostJson(this.urlService, this.serializer.Serialize(data));
            APIResponse response = this.serializer.Deserialize<APIResponse>(input);
            foreach (BankAccountV6 tv in (from x in this.serializer.Deserialize<List<BankAccountV6>>(response.ResponseContent)
                                          where x.BankName != "VIETINBANK"
                                          select x).ToList<BankAccountV6>())
            {
                BankAccount item = new BankAccount
                {
                    BankCode = tv.BankName,
                    DisplayName = tv.Name
                };
                result.Add(item);
            }
            DataCaching.SetCache<List<BankAccount>>(cacheKey, result, 120);
            return new APIResponse(1) { ResponseContent = this.serializer.Serialize(result) };
        }
        public APIResponse Order(APITransaction transaction)
        {
            BankDirectV2Service.OrderRequest request = this.serializer.Deserialize<BankDirectV2Service.OrderRequest>(transaction.RequestContent);
            if (request.Amount < 0x2710)
            {
                return new APIResponse(-356);
            }
            if (request.BankName.ToUpper() == "MBB")
            {
                request.BankName = "MB";
            }
            BankGateAPI addTran = new BankGateAPI
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = "A",
                OrderInfo = string.Empty,
                Amount = decimal.Zero,
                TotalAmount = decimal.Zero,
                Currency = "VND",
                ReturnUrl = request.CallbackUrl,
                RequestTime = 0L,
                Signature = Encrypts.MD5(DateTime.Now.ToString()),
                LogContent = " ",
                BankCode = request.BankName,
                FullName = request.AccountName,
                Mobile = string.Empty,
                RefCode = request.RefCode
            };
            addTran.Amount = request.Amount;
            long num = addTran.Add();
            string refCode = num.ToString();
            if (num > 0L)
            {
                string partnerCode = this.partnerCode1;
                string partnerKey = this.partnerKey1;
                string CallbackUrl = "https://pm.kudopay.xyz/napbankcoroach";
                if (transaction.PartnerCode == "panpan")
                {
                    partnerCode = this.partnerCode2;
                    partnerKey = this.partnerKey2;
                    refCode = request.RefCode;
                    CallbackUrl = "https://momo.robingate.xyz/momocb";
                }
                HynBankLib.OrderRequest request1 = new HynBankLib.OrderRequest
                {
                    Type = "momov2",
                    AccountName = request.AccountName,
                    Amount = request.Amount,
                    AppCode = "",
                    CallbackUrl = CallbackUrl,
                    RefCode = refCode,
                    BankName = "MOMO"
                };
                string RequestContent = this.serializer.Serialize(request1);
                if (addTran.BankCode.ToUpper() != "MOMO")
                {
                    HynBankLib.OrderRequest request2 = new HynBankLib.OrderRequest
                    {
                        Type = "banktranfer",
                        AccountName = DateTime.Now.ToString("ddMMyyyy"),
                        Amount = request.Amount,
                        AppCode = "",
                        CallbackUrl = CallbackUrl,
                        RefCode = refCode,
                        BankName = addTran.BankCode.ToUpper()
                    };
                    RequestContent = this.serializer.Serialize(request2);
                }
                string sign = Encrypts.MD5(partnerCode + this.serviceCode + "order" + RequestContent + partnerKey);
                HynBankLib.RequestData data = new HynBankLib.RequestData
                {
                    PartnerCode = partnerCode,
                    CommandCode = "order",
                    RequestContent = RequestContent,
                    ServiceCode = this.serviceCode,
                    Signature = sign
                };
                string serviceResponse = HynBankLib.PostJson(this.urlService, this.serializer.Serialize(data));
                if (string.IsNullOrEmpty(serviceResponse))
                {
                    return new APIResponse(-1);
                }
                APIResponse response = this.serializer.Deserialize<APIResponse>(serviceResponse);
                if (response.ResponseCode == 1)
                {
                    HynBankLib.OrderResponse orderResponse = this.serializer.Deserialize<HynBankLib.OrderResponse>(response.ResponseContent);
                    addTran.TransactionID = num;
                    addTran.BankAccountName = orderResponse.BankAccountName;
                    addTran.BankAccountNumber = orderResponse.BankAccountNumber;
                    addTran.OrderNo = orderResponse.OrderNo;
                    addTran.UpdateBank();
                    Libs.BankDirect.Order order = new Libs.BankDirect.Order
                    {
                        Status = "1",
                        Amount = request.Amount,
                        RefCode = request.RefCode,
                        OrderNo = orderResponse.OrderNo,
                        Timeout = 30,
                        Url = orderResponse.LinkOpenApp,
                        BankName = request.BankName,
                        BankAccountNumber = orderResponse.BankAccountNumber,
                        BankAccountName = orderResponse.BankAccountName
                    };
                    if (addTran.BankCode.ToUpper() == "MOMO")
                    {
                        order.QRCode = "data:image/png;base64," + orderResponse.QRCode;
                        //order.Url = "data:image/png;base64," + orderResponse.QRCode;
                    }
                    else
                    {
                        order.Url = $"https://img.vietqr.io/image/{addTran.BankCode}-{orderResponse.BankAccountNumber}-compact.jpg?amount={request.Amount}&addInfo={orderResponse.OrderNo}";
                    }
                    if (addTran.BankCode.ToUpper() != "MOMO")
                    {
                        List<BankAccountV3> cache = DataCaching.GetCache<List<BankAccountV3>>("BankInfoHyn");
                        if (cache == null)
                        {
                            cache = new List<BankAccountV3>();
                        }
                        if (!cache.Exists(x => x.AccountId == orderResponse.BankAccountNumber))
                        {
                            BankAccountV3 item = new BankAccountV3
                            {
                                AccountId = orderResponse.BankAccountNumber,
                                AccountName = orderResponse.BankAccountName,
                                BankCode = request.BankName
                            };
                            cache.Add(item);
                            DataCaching.SetCache<List<BankAccountV3>>("BankInfoHyn", cache, 0x13_c680);
                        }
                    }
                    return new APIResponse(1) { ResponseContent = this.serializer.Serialize(order) };
                }
                return new APIResponse(-1) { Description = "" };
            }
            switch (num)
            {
                case -319L:
                    return new APIResponse(-319);

                case -310L:
                    return new APIResponse(-310);
            }
            return new APIResponse(-1);
        }


        public APIResponse Callback(HynBankLib.Callback callback)
        {
            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);
            return apiResponse;
            
        //    var Partner = new Partners().Get(161);//154 (panda) 161 (panpan)
        //    var AcountName = callback.OrderNo;

        //    var neworder = new BankGateAPI()
        //    {
        //        PartnerID = Partner.PartnerID,
        //        PartnerCode = Partner.PartnerCode,
        //        ProviderCode = "hynmomo",
        //        OrderNo = callback.OrderNo,
        //        OrderInfo = callback.RefCode,
        //        Amount = Convert.ToDecimal(callback.Amount),
        //        TotalAmount = Convert.ToDecimal(callback.Amount),
        //        Currency = "VND",
        //        ReturnUrl = "",
        //        RequestTime = 0,
        //        Signature = Encrypts.MD5(DateTime.Now.ToString()),
        //        LogContent = "Add Order " + callback.OrderNo,
        //        BankCode = "MOMO",
        //        FullName = AcountName,
        //        Mobile = callback.Mobile,
        //        RefCode = callback.RefCode,
        //        BankAccountName = string.Empty,
        //        BankAccountNumber = "",
        //        LastTime = DateTime.Now,
        //        Status = (int)ResponseCode.TransactionSuccessful
        //    };
        //    //var keyCache = String.Format("MomoAccount:{0}", callback.account_receive);
        //    //var dataCache = DataCaching.GetCache<string>(keyCache);
        //    //if (dataCache != null)
        //    //{
        //    //    neworder.BankAccountName = dataCache.ToString();
        //    //}
        //    var addId = neworder.AddV2();
        //    if (addId < 0)
        //    {
        //        //neworder.Status = (int)addId;
        //        //neworder.Update();
        //        return new APIResponse((int)addId);
        //    }
        //    var datacb = new DataCallback()
        //    {
        //        Content = neworder.OrderNo,
        //        Mobile = callback.Mobile,
        //        TransId = neworder.RefCode,
        //        Amount = Convert.ToInt32(callback.Amount),
        //        BankCode = "MOMO"
        //    };
        //    datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);


        //    //NLogLogger.Info(new string[] { "M32", "PartnerCallback", serializer.Serialize(datacb), Partner.SMSPlusUrl });
        //    //Callback for Partner
        //    if (!string.IsNullOrEmpty(Partner.SMSPlusUrl))
        //    {


        //        Task.Run(async () => await HynBankLib.CallbackJson(Partner.SMSPlusUrl, serializer.Serialize(datacb), addId).ConfigureAwait(false));
        //    }


        //    return apiResponse;
        }
        public APIResponse CallbackV3(Libs.BankDirect.HynBank.HynBankLib.Callback callback)
        {
            APIResponse response = new APIResponse(-1);
            BankGateAPI order = new BankGateAPI().Get(long.Parse(callback.RefCode));
            if (order == null)
            {
                string[] list = new string[] { "Ken", "Callback", "Order NULL", this.serializer.Serialize(callback) };
                NLogLogger.Info(list);
                TimeSpan span = (TimeSpan)(DateTime.UtcNow - new DateTime(0x7b2, 1, 1, 0, 0, 0));
                double totalSeconds = span.TotalSeconds;
                return response;
            }
            if (order.Status != 1)
            {
                order.Status = 1;
                order.TotalAmount = Convert.ToDecimal(callback.Amount);
                order.LastTime = DateTime.Now;
                order.Mobile = callback.Mobile;
                order.Email = callback.OrderInfo;
                order.OrderInfo = callback.OrderInfo;
                order.Update();
                response = new APIResponse(1);
                DataCallback datacb = new DataCallback
                {
                    Content = order.OrderNo,
                    TransId = order.RefCode,
                    Amount = Convert.ToInt32(callback.Amount),
                    BankCode = order.BankCode,
                    Mobile = callback.Mobile,
                    Result = callback.OrderInfo,
                    mTransId = callback.OrderInfo
                };
                if (!string.IsNullOrEmpty(order.ReturnUrl))
                {
                    
                    Partners partners = new Partners().Get(order.PartnerCode);
                    datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount.ToString() + datacb.Content, partners.PrivateKey, partners.SignatureType);
                    Task.Run(async () => await CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
                }
            }
            return response;
        }

        public APIResponse CallbackV2(HynBankLib.Callback callback)
        {
            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);
            return apiResponse;
            //var ipRequest = Libs.Utils.IPAddress.Get();
            //if (ipRequest != serviceIp)
            //{
            //    NLogLogger.Info(new string[] { "Hyn2", "Callback", "Ip Invalid", ipRequest, serviceIp });
            //    return new APIResponse((int)ResponseCode.IpInvalid);
            //}
            //var Partner = new Partners().Get(161);//161
            //var AcountName = callback.OrderNo;

            //var neworder = new BankGateAPI()
            //{
            //    PartnerID = Partner.PartnerID,
            //    PartnerCode = Partner.PartnerCode,
            //    ProviderCode = "hynmomo",
            //    OrderNo = callback.OrderNo,
            //    OrderInfo = callback.RefCode,
            //    Amount = Convert.ToDecimal(callback.Amount),
            //    TotalAmount = Convert.ToDecimal(callback.Amount),
            //    Currency = "VND",
            //    ReturnUrl = "",
            //    RequestTime = 0,
            //    Signature = Encrypts.MD5(DateTime.Now.ToString()),
            //    LogContent = "Add Order " + callback.OrderNo,
            //    BankCode = "MOMO",
            //    FullName = AcountName,
            //    Mobile = callback.Mobile,
            //    RefCode = callback.RefCode,
            //    BankAccountName = string.Empty,
            //    BankAccountNumber = "",
            //    LastTime = DateTime.Now,
            //    Status = (int)ResponseCode.TransactionSuccessful
            //};
            ////var keyCache = String.Format("MomoAccount:{0}", callback.account_receive);
            ////var dataCache = DataCaching.GetCache<string>(keyCache);
            ////if (dataCache != null)
            ////{
            ////    neworder.BankAccountName = dataCache.ToString();
            ////}
            //var addId = neworder.AddV2();
            //if (addId < 0)
            //{
            //    //neworder.Status = (int)addId;
            //    //neworder.Update();
            //    return new APIResponse((int)addId);
            //}
            //var datacb = new DataCallback()
            //{
            //    Content = neworder.OrderNo,
            //    Mobile = callback.Mobile,
            //    TransId = neworder.RefCode,
            //    Amount = Convert.ToInt32(callback.Amount),
            //    BankCode = "MOMO"
            //};
            //datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);


            ////NLogLogger.Info(new string[] { "M32", "PartnerCallback", serializer.Serialize(datacb), Partner.SMSPlusUrl });
            ////Callback for Partner
            //if (!string.IsNullOrEmpty(Partner.SMSPlusUrl))
            //{


            //    Task.Run(async () => await HynBankLib.CallbackJson(Partner.SMSPlusUrl, serializer.Serialize(datacb), addId).ConfigureAwait(false));
            //}


            //return apiResponse;
        }
    }


}


