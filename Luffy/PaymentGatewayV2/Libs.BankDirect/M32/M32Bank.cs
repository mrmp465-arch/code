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
using Libs.Report;
using BankAccount = Libs.BankGate.Entity.BankAccount;

namespace Libs.BankDirect.M32
{
    public class M32Bank : IBankDirectV2Handler
    {

        private const string username = "LUFFY";
        private const string username2 = "LUFFY2";
        private const string username3 = "LUFFY3";
        private const string username4 = "LUFFY4";
        private const string username5 = "LUFFY5";
        //private const string username5 = "PEOPEO5";
        private const string urlBaseService = "http://139.180.206.12:6001/";
        //private const string urlCallback = "https://bankgate.drumpal.info/callback/m32.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string secretKey = "Uqi63728bshHFNIowueZZ8272";
        private const string secretKey2 = "Y78dXC3hTWpv4mKkCwOTwDx1qVGQ5DzG";
        private const string secretKey3 = "HM5j4+6r?^2Zw$NMP9e^tEKPa&YdSB99";
        private const string secretKey4 = "HM5jYdSB99Dsd4r5632d2drwdfsdrwer";
        private const string secretKey5 = "HM5jYdSB99Dsd4r5632d2drwdfsd1234";
        //private const string secretKey5 = "WJmTU4jU3eMganCM5ngwpA9C7gUYsPxd";
        private const string serviceIp = "139.180.206.12";
        public APIResponse CheckTrans(APITransaction transaction)
        {
            //var urlService = urlBaseService + string.Format("/v1/orders/{0}/", serializer.Deserialize<BankDirectV2Service.CheckOrderRequest>(transaction.RequestContent).OrderNo);
            //NLogLogger.Info(new string[] { "M32", "CheckTrans Request", urlService });
            //var response = Task.Run(async () => await M32BankLib.GetTask(urlService)).Result;
            //NLogLogger.Info(new string[] { "M32", "CheckTrans Response", response, urlService });

            //if (!string.IsNullOrEmpty(response))
            //{
            //    var resObj = serializer.Deserialize<M32BankLib.OrderResponse>(response);
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
            var selectUsername = "";
            var selectSecretKey = secretKey2;
            var urlService = urlBaseService + "BankAPI/getInfo";
            //azt key1
            switch (ParnerId)
            {
                case "hyn15":
               
                    selectUsername = username;
                    selectSecretKey = secretKey;
                    break;
                case "hyn3":

                    selectUsername = username3;
                    selectSecretKey = secretKey3;
                    break;
                case "m1pro":

                    selectUsername = username4;
                    selectSecretKey = secretKey4;
                    break;
                case "bigwin":

                    selectUsername = username5;
                    selectSecretKey = secretKey5;
                    break;
                case "pt86":

                    selectUsername = username2;
                    selectSecretKey = secretKey2;
                    break;
            }

            var bankRequest = new M32BankLib.BankRequest()
            {
                requestTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                type = 2, // Momo
                username = selectUsername
            };
            bankRequest.authKey = Utils.Encrypts.MD5(bankRequest.requestTime + "|" + bankRequest.type + "|" + selectSecretKey);

            NLogLogger.Info(new string[] { "M32", "GetBanks Request",serializer.Serialize(bankRequest), urlService
                });
            var response = Task.Run(async () => await M32BankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
            NLogLogger.Info(new string[] { "M32", "GetBanks Response", response, urlService
            });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<M32BankLib.BankResponse>(response);
                if (resObj.errorCode == "0")
                {
                    var bank = serializer.Deserialize<M32BankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccount>();
                    listBankObj.Add(new BankAccount()
                    {

                        //AccountName = bank.name,
                        //AccountNumber = bank.phone,
                        BankCode="MOMO"
                    });

                    if (DateTime.Now.Hour <= 10)
                    {
                        var keyCache = String.Format("MomoAccount:{0}", bank.phone);
                        var dataCache = DataCaching.GetCache<string>(keyCache);
                        if (dataCache == null)
                        {
                            DataCaching.SetCache(keyCache, bank.name, 1440 * 5 * 60);
                        }
                    }

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
            

            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public APIResponse Order(APITransaction transaction)
        {
            


            return new APIResponse((int)ResponseCode.TransactionFailed);

        }
        public APIResponse CallbackV2(M32BankLib.CallbackV2 callback)
        {
            var ipRequest = Libs.Utils.IPAddress.Get();
            if (ipRequest != serviceIp)
            {
                NLogLogger.Info(new string[] { "M32", "Callback", "Ip Invalid", ipRequest, serviceIp });
                return new APIResponse((int)ResponseCode.IpInvalid);
            }
            //var ipRequest = Libs.Utils.IPAddress.Get();
            //NLogLogger.Info(new string[] { "M32", "Callback", "Ip Invalid", ipRequest, serviceIp });
            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);

            var Partner = new Partners().Get(int.Parse(callback.pid));
            var selectSecretKey = secretKey;
            switch (Partner.PartnerCode)
            {
                case "hyn15":
                
                    selectSecretKey = secretKey2;
                    break;
                case "fun":

                    selectSecretKey = secretKey3;
                    break;
                case "mb86":

                    selectSecretKey = secretKey4;
                    break;
            }
            //var signature = Utils.Encrypts.MD5(callback.requestTime + "|" + callback.message + "|" + callback.money + "|" + callback.phone + "|" + selectSecretKey);
            //if (signature != callback.authKey)
            //{
            //    NLogLogger.Info(new string[] { "M32", "Callback", "Signature Failed", signature, callback.requestTime + "|" + callback.message + "|" + callback.money + "|" + callback.phone + "|" + selectSecretKey });
            //    return new APIResponse((int)ResponseCode.SignatureInvalid);
            //}

            //if(callback.message.Length>15)
            //{
            //    callback.message = callback.message.Replace("Tiềnvàotiềnra,cótiềnlàđạigia😄", "");
            //}    
            var AcountName = callback.message;
            
            var neworder = new BankGateAPI()
            {
                PartnerID = Partner.PartnerID,
                PartnerCode = Partner.PartnerCode,
                ProviderCode = "m32momo",
                OrderNo = callback.message,
                OrderInfo = callback.momo_transId,
                Amount = Convert.ToDecimal(callback.money),
                TotalAmount = Convert.ToDecimal(callback.money),
                Currency = "VND",
                ReturnUrl = "",
                RequestTime = 0,
                Signature = Encrypts.MD5(DateTime.Now.ToString()),
                LogContent = "Add Order "+ callback.message,
                BankCode = "MOMO",
                FullName = AcountName,
                Mobile = callback.phone,
                RefCode = callback.momo_transId,
                BankAccountName = string.Empty,
                BankAccountNumber = callback.account_receive,
                LastTime = DateTime.Now,
                Status = (int)ResponseCode.TransactionSuccessful
            };
            var keyCache = String.Format("MomoAccount:{0}", callback.account_receive);
            var dataCache = DataCaching.GetCache<string>(keyCache);
            if (dataCache != null)
            {
                neworder.BankAccountName = dataCache.ToString();
            }
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
                BankCode = "MOMO"
            };
            datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);
            if(Partner.PartnerCode=="azt")
            {
                Action<string, long, string, long> send = UpdatePartnerBalance;
                var asynSend = send.BeginInvoke(neworder.PartnerCode, Convert.ToInt64(callback.money), neworder.BankCode, neworder.TransactionID, null, null);
            }    
           
            //NLogLogger.Info(new string[] { "M32", "PartnerCallback", serializer.Serialize(datacb), Partner.SMSPlusUrl });
            //Callback for Partner
            if (!string.IsNullOrEmpty(Partner.SMSPlusUrl))
            {


                Task.Run(async () => await M32BankLib.CallbackJson(Partner.SMSPlusUrl, serializer.Serialize(datacb)).ConfigureAwait(false));
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
            decimal ck = _partnerDiscount.DiscountMOMO;

            if (ck == 0)
                return;

            long realAmount = Amount - Convert.ToInt64(Amount * ck);
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Partners().Topup(realAmount, PartnerCode, $"Cộng tiền nạp bank mã giao dịch {TranId}");

        }
        public APIResponse Callback(M32BankLib.Callback callback)
        {



            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
       
    }


}


