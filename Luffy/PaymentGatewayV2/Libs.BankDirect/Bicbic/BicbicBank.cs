using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;

using System.Threading.Tasks;
using Libs.BankGate.Entity;
using BankAccount = Libs.BankGate.Entity.BankAccount;
using Libs.Report;
using System.Linq;
using Libs.BankDirect.HynBank;
using System.Web.Caching;
using Libs.BankDirect.Bicbic;
using static Libs.BankDirect.Bicbic.BicbicBankLib;

namespace Libs.BankDirect.Bicbic
{
    public class BicbicBank : IBankDirectV2Handler
    {


        private const string urlBaseService = "https://m2hub.net/api/";
        private const string callbackurl = "https://bank.kudopay.xyz/Callback/ImoCallback.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //private const string secretKey = "fd110f27-233b-4267-bb7e-000adca17bb0";
        //private const string secretKey2 = "fd110f27-233b-4267-bb7e-000adca17bb0";
        //private const string pw = "68686868";
        //private const string pw2 = "68686868";

        private const string Apikey = "42b33ad2-5b38-458d-8e4f-133e12d310df";

        private const string ApiSecret = "23116821";
        public APIResponse CheckTrans(APITransaction transaction)
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public List<BicbicBankLib.BankResponse> GetBankAPI()
        {
            string cacheKey = "KenBanksV2";
            var result = DataCaching.GetCache<List<BicbicBankLib.BankResponse>>(cacheKey);
            if (result != null)
            {
                return result;
            }
            else
            {
                var urlService = urlBaseService + "GET_ACTIVE_BANKS/?api_key=" + Apikey;


                // NLogLogger.Info(new string[] { "Bicbic", "GetBanks Request", urlService });
                //var response = Task.Run(async () => await VNPayBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
                var response = Task.Run(async () => await BicbicBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "IMO", "GetBanks Response", response, urlService });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<List<BicbicBankLib.BankResponse>>(response);

                    DataCaching.SetCache(cacheKey, resObj, 60 * 2);
                    return resObj;
                }

            }
            return null;
        }
        public APIResponse GetBanksV2(string Type, string Code)
        {



            return new APIResponse((int)ResponseCode.TransactionFailed);


        }
        public APIResponse GetBanks()
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public APIResponse Order(APITransaction transaction)
        {


            var tran = serializer.Deserialize<BankDirectV2Service.OrderRequest>(transaction.RequestContent);
            if (tran.Amount < 1)
            {
                return new APIResponse((int)ResponseCode.BankAmountInvalid);
            }
            //InitBank 
            var addTran = new BankGateAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = "A", // Tự động lấy bằng ID Table
                OrderInfo = tran.AccountName,
                Amount = 0,
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
                //BankAccountName = tran.BankAccountName,
                //BankAccountNumber = tran.BankAccountNumber,

            };


            addTran.Amount = tran.Amount;
            //string chargeType = "bank";
            //if (tran.BankName.ToUpper() == "MOMO")
            //{
            //    chargeType = "momo";
            //}
          
           



            var add = addTran.Add();

            if (add > 0)
            {
                //var OrderNo = add.ToString();
                var sign = Encrypts.MD5(Apikey+ + add + ApiSecret);
                var listBank = GetBankAPI();
                var bid = listBank.FirstOrDefault(x => x.bankCode.Contains(tran.BankName)).bankId;
                var urlService = $"{urlBaseService}B_REQUEST_PAY_IN/?api_key={Apikey}&request_id={add}&amount={tran.Amount}&requestId={add}&bid={bid}&signature={sign}";

              
                //NLogLogger.Info(new string[] { "IMO", "Order Request", urlService });

                var response = Task.Run(async () => await BicbicBankLib.GetTask(urlService)).Result;
                // NLogLogger.Info(new string[] { "IMO", "Order Response", response });


                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<BicbicBankLib.OrderResponse>(response);


                    if (resObj.errorCode == 1)
                    {
                        resObj.code = resObj.code.ToUpper();
                        addTran.TransactionID = add;
                        addTran.BankAccountName = resObj.bankAccountName;
                        addTran.BankAccountNumber = resObj.bankAccountNumber;
                        addTran.OrderNo = resObj.code;
                        addTran.UpdateBank();
                        var orderRes = new Order()
                        {
                            Status = resObj.errorCode.ToString(),
                            Amount = tran.Amount,
                            RefCode = tran.RefCode,
                            OrderNo = resObj.code,
                            Timeout = 30,
                            Url = resObj.qrCode,
                            BankName = tran.BankName,
                            BankAccountNumber = resObj.bankAccountNumber,
                            BankAccountName = resObj.bankAccountName
                        };
                        //if (chargeType == "usdt")
                        //    orderRes.Url = orderRes.Url.Remove(orderRes.Url.Length - 1, 1);

                        var lstBank = DataCaching.GetCache<List<BankAccountV3>>("BankInfoKZ");
                        if (lstBank == null)
                        {
                            lstBank = new List<BankAccountV3>();

                        }
                        if (!lstBank.Exists(x => x.AccountId == resObj.bankAccountNumber))
                        {
                            lstBank.Add(new BankAccountV3
                            {
                                AccountId = resObj.bankAccountNumber,
                                AccountName = resObj.bankAccountName,
                                BankCode = tran.BankName
                            });
                            DataCaching.SetCache("BankInfoKZ", lstBank, 86400 * 15);
                        }
                        return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {
                            ResponseContent = serializer.Serialize(orderRes)
                        };
                    }
                    else
                    {
                        return new APIResponse((int)ResponseCode.TransactionFailed)
                        {
                            Description = resObj.message
                        };
                    }



                }

                return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            else
            {
                switch (add)
                {
                    case (int)ResponseCode.AccessDenied:
                        return new APIResponse((int)ResponseCode.AccessDenied);
                    case (int)ResponseCode.TransactionDuplicate:
                        return new APIResponse((int)ResponseCode.TransactionDuplicate);
                }
            }


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

       

        public APIResponse Callback(Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var signature = Utils.Encrypts.MD5(callback.requestId + callback.transId  + ApiSecret);
            if (signature != callback.signature)
            {
                NLogLogger.Info(new string[] { "IMO", "Callback", "Signature Failed", signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }
            //NLogLogger.Info(new string[] { "VNPAY", "Callback", callback.status });
            if (callback.status == 1)
            {
                var order = new BankGateAPI().Get(long.Parse(callback.requestId));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "bicbic", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                string mobile = "";
                

                if (order.Status != (int)ResponseCode.TransactionSuccessful)
                {
                    order.Status = (int)ResponseCode.TransactionSuccessful;
                    order.TotalAmount = Convert.ToDecimal(callback.amount);
                    order.LastTime = DateTime.Now;
                    order.Mobile = mobile;
                    order.Email = callback.transId;
                    order.OrderInfo = callback.transId;

                    order.Update();
                    //Action<string, long, string, long> send = UpdatePartnerBalance;
                    //var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(callback.chargeAmount), order.BankCode, order.TransactionID, null, null);
                }
                apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                var datacb = new DataCallback()
                {
                    Content = order.OrderNo,
                    TransId = order.RefCode,
                    Amount = Convert.ToInt32(callback.amount),
                    BankCode = order.BankCode,
                    Mobile = mobile,
                    Result = callback.transId,
                    mTransId = callback.transId
                };
                if (string.IsNullOrEmpty(datacb.mTransId))
                    datacb.mTransId = callback.requestId;
                //bank
                if (order.Type == 2)
                    datacb.mTransId = callback.requestId;
               
                //Callback for Partner
                if (!string.IsNullOrEmpty(order.ReturnUrl))
                {
                    var Partner = new Partners().Get(order.PartnerCode);
                    datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);
                    //NLogLogger.Info(new string[] { "VNPay", "PartnerCallback", serializer.Serialize(datacb), order.ReturnUrl });
                    Task.Run(async () => await BicbicBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
                }

                //Callback for Partner
                //if (!string.IsNullOrEmpty(order.ReturnUrl))
                //{
                //    var partner = new Partners().Get(order.PartnerCode);
                //    apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                //    Task.Run(async () => await ImoBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse)).ConfigureAwait(false));
                //}

                return apiResponse;
            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
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

            if (ck == 0)
                return;

            long realAmount = Amount - Convert.ToInt64(Amount * ck);
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Partners().Topup(realAmount, PartnerCode, $"Cộng tiền nạp bank mã giao dịch {TranId}");

        }

    }


}


