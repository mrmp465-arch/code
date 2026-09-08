using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using APIBB2D.Entity;
using Libs.Report;
using Libs.Utils;

namespace APIBB2D
{
    internal class TopupApp
    {

        private string ServiceUrl = (ConfigurationManager.AppSettings["ServiceUrl"] ?? "https://apicard.coroach.xyz/VPGJsonService.ashx");
        private string CallbackUrl = (ConfigurationManager.AppSettings["CallbackUrl"] ?? "https://cb.kudopay.xyz/coroachtt");
        private string CallbackUrlV2 = (ConfigurationManager.AppSettings["CallbackUrlV2"] ?? "https://cb.kudopay.xyz/coroachcb");
        private const string _partnerCode = "hyn3";
        private const string _partnerKey = "e474a13eec00a7d878f219e34ad1e11c";
        private string serviceCode = "cardtelco";
        private string commandCode = "usecard";
        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        public async Task<string> Topup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {

          
            var Url = CallbackUrl;
            
               
            var type = string.Empty;
            //var Url = CallbackUrl;
            switch (telco.ToLower())
            {
                case "vtt":
                    type = "viettel";
                    break;
                case "vms":
                    type = "vms";
                    break;
                case "vnp":
                    type = "vnp";
                    break;
                
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "BB2D";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {


                string requestContent = serializer.Serialize(new CardRequest()
                {
                    CardSerial = topup.CardSerial,
                    CardCode = topup.CardCode,
                    CardType = type,
                    AccountName = transactionId,
                    AppCode = string.Empty,
                    RefCode = topup.Id.ToString(),
                    AmountUser = topup.AmountUser,
                    CallBackUrl = Url

                });
                var signature = Encrypts.MD5(_partnerCode + serviceCode + commandCode + requestContent + _partnerKey);
                var requestData = new RequestData()
                {
                    PartnerCode = _partnerCode,
                    CommandCode = commandCode,
                    RequestContent = requestContent,
                    ServiceCode = serviceCode,
                    Signature = signature
                };

                NLogLogger.Info(new string[] { "BB2D", topup.TransactionId.ToString(), "BB2DRequest", serializer.Serialize(requestData) });
                var serviceResponse = PostJson(ServiceUrl, serializer.Serialize(requestData));
                NLogLogger.Info(new string[] { "BB2D", topup.TransactionId.ToString(), "BB2DResponse", serviceResponse });

                if (!string.IsNullOrEmpty(serviceResponse))
                {
                    var cardResult = serializer.Deserialize<CardResponse>(serviceResponse);


                    switch (cardResult.ResponseCode)
                    {
                        case 1:
                        case 2:
                        case -372:
                        case 6:
                            var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                            if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                            break;
                        case -7:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                            return "-7|0";
                        case -334:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                            return "-334|0";
                        case -330:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";
                       
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }


                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIBB2D", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        public async Task<string> TopupCallBack(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {


            var Url = CallbackUrlV2;


            var type = string.Empty;
            //var Url = CallbackUrl;
            switch (telco.ToLower())
            {
                case "vtt":
                    type = "viettel";
                    break;
                case "vms":
                    type = "vms";
                    break;
                case "vnp":
                    type = "vnp";
                    break;

            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "BB2D";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {


                string requestContent = serializer.Serialize(new CardRequest()
                {
                    CardSerial = topup.CardSerial,
                    CardCode = topup.CardCode,
                    CardType = type,
                    AccountName = transactionId,
                    AppCode = string.Empty,
                    RefCode = topup.Id.ToString(),
                    AmountUser = topup.AmountUser,
                    CallBackUrl = Url

                });
                var signature = Encrypts.MD5(_partnerCode + serviceCode + commandCode + requestContent + _partnerKey);
                var requestData = new RequestData()
                {
                    PartnerCode = _partnerCode,
                    CommandCode = commandCode,
                    RequestContent = requestContent,
                    ServiceCode = serviceCode,
                    Signature = signature
                };

                NLogLogger.Info(new string[] { "BB2D", topup.TransactionId.ToString(), "BB2DRequest", serializer.Serialize(requestData) });
                var serviceResponse = PostJson(ServiceUrl, serializer.Serialize(requestData));
                NLogLogger.Info(new string[] { "BB2D", topup.TransactionId.ToString(), "BB2DResponse", serviceResponse });

                if (!string.IsNullOrEmpty(serviceResponse))
                {
                    var cardResult = serializer.Deserialize<CardResponse>(serviceResponse);


                    switch (cardResult.ResponseCode)
                    {
                        case 6:
                            return "0|0";
                            break;
                        case -7:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                            return "-7|0";
                        case -334:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                            return "-334|0";
                        case -330:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";

                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }


                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIBB2D", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }

        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "APIBB2D", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 30; i++)
            {
                //Check DB 30 lan tuong ung 90s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                    {
                        NLogLogger.Info(new string[] { "APIBB2D", "CheckStatusAsync", "Response", id.ToString(), string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount) });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }

                System.Threading.Thread.Sleep(3000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty, string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "APIBB2D", "CheckStatusAsync", id.ToString(), "Timeout" });
            return "-326|0";
        }

        public static string PostJson(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
            //request.Accept = "JSON";
            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            if (webResponse == null)
            {
                return "Unable to connect to the remote server";
            }
            var sr = new StreamReader(webResponse.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        public static String GetHashHMACSHA256(String text, String key)
        {
            Byte[] textBytes = Encoding.UTF8.GetBytes(text);
            Byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            return Convert.ToBase64String(hashBytes);
        }


    }

}