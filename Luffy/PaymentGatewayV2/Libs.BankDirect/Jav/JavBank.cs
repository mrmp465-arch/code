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
using static Libs.BankDirect.Jav.JavBankLib;

namespace Libs.BankDirect.Jav
{
    public class JavBank : IBankDirectV2Handler
    {

        private const string username = "portzoro";//vngate
		private const string username2 = "portzoro2";
        private const string username3 = "portzoro3";
        //private const string username3 = "minisuper";
        //      private const string username5 = "italya";//gsun
        //      private const string username6 = "italya02";//gusn
        //      private const string username7 = "chelsea";
        private const string urlBaseService = "http://202.182.118.37:6688";
        //private const string AccessKey = "6e672de404fd6edd17aa792ef2b45ba7";
        private const string SecretKey = "2f0bcad15e34537504230af61fd9786f";

        private const string SecretKeyV2 = "ffb6a4cc70751410a0cbb1969f235bcf";
        private const string SecretKeyV3 = "9753159328f907f08af334e4e80e3add";

        //private const string SecretKeyV3 = "9662854fb31a14a563b42aba4f09cf5e";
        //private const string SecretKeyV4 = "b52c4b52998cc76599cc01f5e1050c56";

        //private const string SecretKeyV5 = "6e6c93e3d988925c6ec4b4b9abf2d76e";
        //private const string SecretKeyV6 = "c547a1883d6c8e4300dbda0745277d94";
        //private const string SecretKeyV7 = "61613e5a55d1339a26a60842a710087c";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //private const string secretKey = "superman68a@123";
        //private const string password = "superman68a@";
        public APIResponse CheckTrans(APITransaction transaction)
        {
           
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse GetBanksV2(string Type, string PartnerCode)
        {
            var keyCache = "JAVBanksV2";

            if (PartnerCode == "hyn7")
                keyCache = "JAVBankss";

            if (PartnerCode == "xv")
                keyCache = "JAVBanksV3";
            var dataCache = DataCaching.GetCache<string>(keyCache);
            if (!string.IsNullOrEmpty(dataCache))
            {
                var lstBank = serializer.Deserialize<List<BankAccountInfo>>(dataCache);
               
                NLogLogger.Info(new string[] { "Jav", "JAVBanksV2 Response", dataCache });

                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = serializer.Serialize(lstBank)
                };
            }



            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
       
        public void UpdateBank(JavBankLib.InfoCallback callback)
        {
            var listBankObj = new List<BankAccountInfo>();
            foreach (var item in callback.momo)
            {
                listBankObj.Add(new BankAccountInfo
                {
                   
                   AccountName = item.name,
                    AccountNumber = item.phone,
                });
            }
            //foreach (var item in callback.bank)
            //{
            //    listBankObj.Add(new BankAccountInfo
            //    {
                   
            //        AccountName = item.name,
            //        AccountNumber = item.bank_number,
            //    });
            //}
            var keyCache = "JAVBankss";
            DataCaching.SetCache(keyCache, serializer.Serialize(listBankObj), 1440 * 30 * 60);


        }
        public void UpdateBankV2(JavBankLib.InfoCallback callback, string key)
        {
            var listBankObj = new List<BankAccountInfo>();
            foreach (var item in callback.momo)
            {
                listBankObj.Add(new BankAccountInfo
                {

                    AccountName = item.name,
                    AccountNumber = item.phone,
                });
            }
            
            //var keyCache = "JAVBanksV2";
            DataCaching.SetCache(key, serializer.Serialize(listBankObj), 1440 * 30 * 60);


        }
      
        public APIResponse GetBanks()
        {
            var keyCache = "JAVBankss";

            var dataCache = DataCaching.GetCache<string>(keyCache);
            if (!string.IsNullOrEmpty(dataCache))
            {
                var lstBank = serializer.Deserialize<List<BankAccountInfo>>(dataCache);
                //lstBank = lstBank.Where(x => x.BankCode != "MOMO").ToList();
                NLogLogger.Info(new string[] { "Jav", "GetBanks Response", dataCache });

                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = serializer.Serialize(lstBank)
                };
            }


            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public APIResponse Order(APITransaction transaction)
        {
            

            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public APIResponse CallbackV2(JavBankLib.Callback callback, string partnerCode, string SecretKey, string keybank)
        {
            var Partner = new Partners().Get(partnerCode);

            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var signature = JavBankLib.HmacSha256Digest(callback.body, SecretKey);
            if (signature != callback.sign)
            {
                NLogLogger.Info(new string[] { "Jav", "Callback", "Signature Failed", signature, callback.sign });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }
            var AcountName = callback.message;
            var partner = new Partners().Get(partnerCode);
            var neworder = new BankGateAPI()
            {
                PartnerID = partner.PartnerID,
                PartnerCode = partnerCode,
                ProviderCode = "javmomo",
                OrderNo = callback.message,
                OrderInfo = callback.trans_id,
                Amount = Convert.ToDecimal(callback.amount),
                TotalAmount = Convert.ToDecimal(callback.amount),
                Currency = "VND",
                ReturnUrl = "",
                RequestTime = 0,
                Signature = Encrypts.MD5(DateTime.Now.ToString()),
                LogContent = "Add Order " + callback.message,
                //BankCode = "MOMO",
                FullName = AcountName,
                RefCode = callback.trans_id,
                BankAccountName = string.Empty,
                BankAccountNumber = string.Empty,
                LastTime = DateTime.Now,
                Status = (int)ResponseCode.TransactionSuccessful
            };
            if (callback.type == "momo")
            {

                neworder.Mobile = callback.sender;
                neworder.BankAccountNumber = callback.receiver;
                neworder.BankCode = "MOMO";
                neworder.ProviderCode = "javmomo";
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
                TransId = neworder.RefCode,
                Amount = Convert.ToInt32(callback.amount),
            };
           
           

            //var Partner = new Partners().Get(neworder.PartnerID);
            NLogLogger.Info(new string[] { "Jav", "PartnerCallback", apiResponse.ResponseContent, partner.SMSPlusUrl });

            //Callback for Partner
            if (!string.IsNullOrEmpty(Partner.SMSPlusUrl))
            {

                datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);
                Task.Run(async () => await JavBankLib.CallbackJson(Partner.SMSPlusUrl, serializer.Serialize(datacb)).ConfigureAwait(false));
            }
            return apiResponse;
        }


        public APIResponse Callback(JavBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            

            return apiResponse;
        }
    }


}


