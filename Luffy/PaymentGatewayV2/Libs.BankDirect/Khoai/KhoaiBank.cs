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

namespace Libs.BankDirect.Khoai
{
    public class KhoaiBank : IBankDirectV2Handler
    {

        
        private const string urlBaseService = "http://36s.biz:10007/api/MM/RegCharge";
        
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string apiKey = "62086bd7-17fd-4179-b2f1-a73cb4dbec48";
        private const string signKey = "62086bd7";

        public APIResponse CheckTrans(APITransaction transaction)
        {
            

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse GetBanksV2(string ParnerCode,string AccountName)
        {
            var ApiKey = apiKey;
            var SignKey = signKey;
            var urlService = urlBaseService + "/services/GETINFO";
            //switch (ParnerCode)
            //{
            //    case "pp":

            //        ApiKey = username2;
            //        SignKey = secretKey2;
            //        break;

            //}

            //var bankRequest = new KhoaiBankLib.BankRequest()
            //{
            //    requestTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
            //    type = 2, // Momo
            //    username = selectUsername
            //};
            var requestId = DateTime.Now.ToString("yyyyMMddHHmmss");
            var authKey = Utils.Encrypts.MD5("10000momo"+ requestId + SignKey);
            var url = urlBaseService + "?" + string.Format("apiKey={0}&chargeType=momo&amount=10000&requestId={1}&sign={2}", ApiKey, requestId, authKey);


            NLogLogger.Info(new string[] { "Khoai", "GetBanks Request",url
                });
            var response = Task.Run(async () => await KhoaiBankLib.GetTask(url)).Result;
            NLogLogger.Info(new string[] { "Khoai", "GetBanks Response", response,
            });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<KhoaiBankLib.BankResponse>(response);
                if (resObj.stt >=1)
                {
                    
                    //var bank = serializer.Deserialize<KhoaiBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccount>();
                   
                    listBankObj.Add(new BankAccount()
                    {
                       
                        //AccountName = KhoaiBankLib.ReplaceVietnameseChar(resObj.data.phoneName),
                        //AccountNumber = resObj.data.phoneNum,
                        
                    });

                    if(DateTime.Now.Hour<=10)
                    {
                        var keyCache = String.Format("MomoAccount:{0}", resObj.data.phoneNum);
                        var dataCache = DataCaching.GetCache<string>(keyCache);
                        if (dataCache == null)
                        {
                            DataCaching.SetCache(keyCache, KhoaiBankLib.ReplaceVietnameseChar(resObj.data.phoneName), 1440*5*60);
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
        public APIResponse CallbackV2(KhoaiBankLib.CallbackV2 callback)
        {
           

            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var Partner = new Partners().Get(callback.pid);
            var loginPW = "rliqr2vtsiv3";
            //switch (Partner.PartnerCode)
            //{
            //    case "anhx2":

            //        selectSecretKey = secretKey2;
            //        break;
            //    case "xv":

            //        selectSecretKey = secretKey3;
            //        break;

            //    case "gvmn":

            //        selectSecretKey = secretKey5;
            //        break;

            //    case "anhx3":
            //        selectSecretKey = secretKey4;
            //        break;
            //}
            var signature = callback.chargeId + callback.chargeType + callback.chargeCode + callback.status + callback.chargeAmount + loginPW;
            if (Utils.Encrypts.MD5(signature) != callback.signature)
            {
                NLogLogger.Info(new string[] { "Khoai", "Callback", "Signature Failed", signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }

            //if(callback.message.Length>15)
            //{
            //    callback.message = callback.message.Replace("Tiềnvàotiềnra,cótiềnlàđạigia😄", "");
            //}    
            var AcountName = callback.chargeCode;
            
            var neworder = new BankGateAPI()
            {
                PartnerID = Partner.PartnerID,
                PartnerCode = Partner.PartnerCode,
                ProviderCode = "khoaimomo",
                OrderNo = callback.chargeCode,
                OrderInfo = callback.momoTransId,
                Amount = Convert.ToDecimal(callback.chargeAmount),
                TotalAmount = Convert.ToDecimal(callback.chargeAmount),
                Currency = "VND",
                ReturnUrl = "",
                RequestTime = 0,
                Signature = Encrypts.MD5(DateTime.Now.ToString()),
                LogContent = "Add Order "+ callback.chargeCode,
                BankCode = "MOMO",
                FullName = AcountName,
                Mobile = "",
                RefCode = callback.momoTransId,
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
                TransId = neworder.RefCode,
                Amount = Convert.ToInt32(callback.chargeAmount),
            };
            NLogLogger.Info(new string[] { "Khoai", "PartnerCallback",  Partner.SMSPlusUrl });
            //Callback for Partner
            if (!string.IsNullOrEmpty(Partner.SMSPlusUrl))
            {

                datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);
                Task.Run(async () => await KhoaiBankLib.CallbackJson(Partner.SMSPlusUrl, serializer.Serialize(apiResponse)).ConfigureAwait(false));
            }
            

            return apiResponse;
        }
        public APIResponse Callback(KhoaiBankLib.Callback callback)
        {

            return null;
        }
    }


}


