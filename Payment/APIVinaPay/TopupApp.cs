using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using APIVinaPay.Entity;
using Libs.Report;
using Libs.Utils;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace APIVinaPay
{
    internal class TopupApp
    {

        private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "http://66.42.62.119:8082/partner/RequestPayment";
        private string CallbackUrl = ConfigurationManager.AppSettings["CallbackUrl"] ?? "http://45.32.115.186:1596/CardCallback.ashx";
        private string CallbackUrlV2 = "http://45.32.115.186:1596/CardCallbackV2.ashx";
        private const string apiKey = "vdt50@";
        private const string username = "vdt50@gmail.com";
        private const int ClientID = 20;

        private const string SecretKey = "AUcajG1Z1L2Q5Y43foh51VmaFvygKZaVS8PmmggEyVNPWKhghQPYWgOPyL5NtdQQ";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
       
        public async Task<string> VinaPayTopupCallBack(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {


            var type = "";

            switch (telco.ToLower())
            {
                case "vtt":
                    type = "VT";
                    break;
                case "vms":
                    type = "Mobi";
                    break;
                case "vnp":
                    type = "Vina";
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "ttp";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {
                TopupRequest cardRequest = new TopupRequest();

                cardRequest.serial = serial;
                cardRequest.pin = pin;
                cardRequest.carrier = type;
                cardRequest.email = username;
                cardRequest.amount = amount.ToString();
                cardRequest.url_callback = CallbackUrlV2;
                cardRequest.custom_trans = topup.Id.ToString();
                var signature = Encrypts.MD5(SecretKey + amount+ topup.Id+ serial);
                var data = string.Format("ApiToken={0}&TransID={1}&Signature={2}&CardType={3}&CardSeri={5}&CardCode={4}&Amount={6}&UrlCallBack={7}", SecretKey, topup.Id, signature, type, pin, serial, amount, cardRequest.url_callback);
                NLogLogger.Info(new string[] { "APITiger", "Request", transactionId, topup.Id.ToString(), data });
                var response = Task.Run(async () => await PostTask(ServiceUrl, data)).Result;
                NLogLogger.Info(new string[] { "APITiger", "Response", transactionId, topup.Id.ToString(), response });
                //if (sw.ElapsedMilliseconds > 5000)
                //{
                //    TelegramNotify.SendNotify(-845553760, $"Nghi vấn thẻ {serial} bị nuốt");
                //}
                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);

                    switch (res.errorCode)
                    {
                        case 0:
                            return "0|0";
                            break;
                            
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }

                }

            }

            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIVinaPay", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        public async Task<string> VinaPayTopup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {


            var type = "";

            switch (telco.ToLower())
            {
                case "vtt":
                    type = "VT";
                    break;
                case "vms":
                    type = "Mobi";
                    break;
                case "vnp":
                    type = "Vina";
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "ship";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {
                TopupRequest cardRequest = new TopupRequest();

                cardRequest.serial = serial;
                cardRequest.pin = pin;
                cardRequest.carrier = type;
                cardRequest.email = username;
                cardRequest.amount = amount.ToString();
                cardRequest.url_callback = CallbackUrl;
                cardRequest.custom_trans = topup.Id.ToString();

                var signature = Encrypts.MD5(SecretKey + amount + topup.Id + serial);
                var data = string.Format("ApiToken={0}&TransID={1}&Signature={2}&CardType={3}&CardSeri={5}&CardCode={4}&Amount={6}&UrlCallBack={7}", SecretKey, topup.Id, signature, type, pin, serial, amount, cardRequest.url_callback);
                NLogLogger.Info(new string[] { "APITiger", "Request", transactionId, topup.Id.ToString(), data });
                var response = Task.Run(async () => await PostTask(ServiceUrl, data)).Result;
                NLogLogger.Info(new string[] { "APITiger", "Response", transactionId, topup.Id.ToString(), response });
                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);

                    switch (res.errorCode)
                    {
                        case 0:
                            var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                            if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                            break;
                      
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }

                }

            }

            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIVinaPay", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "APIVinaPay", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 90; i++)
            {
                //Check DB 30 lan tuong ung 30s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                    {
                        NLogLogger.Info(new string[] { "APIVinaPay", "CheckStatusAsync", "Response", id.ToString(), string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount) });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }

                System.Threading.Thread.Sleep(1000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty, string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "APIVinaPay", "CheckStatusAsync", id.ToString(), "Timeout" });
            return "-326|0";
        }

        public static async Task<string> PostTask(string url, string postData)
        {

            var uri = new Uri(url);
            var httpClient = new HttpClient();
            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var response = await httpClient.PostAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    return responseContent;
                }
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APITiger", "PostTask", "Exception", e.Message });
            }

            httpClient.Dispose();
            return string.Empty;

        }
        public static string PostJson(string uri, string postData)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Method = "POST";//GET
                request.Timeout = 20000;
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
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    //return "{\"status\":-5002,\"data\":{\"amount\": 0},\"amount\": 0,\"msg\": \"Ngắt kết nối đến nhà cung cấp Mobo. Timeout exception\"}";
                    NLogLogger.Info(new string[] { "BicBic", "BicBicRequest", postData, "Timeout exception" });
                    return "{\"code\":-5002,\"desc\":\"Ngắt kết nối đến nhà cung cấp BicBIC. Timeout exception\"}";
                }
                else
                {
                    throw;
                }
            }
        }

       

        public static async Task<string> GetTask(string url, string token = "")
        {


            var client = new RestClient(url);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(token))
            {
                //httpContent.Headers.Add("Authorization", "bearer " + token);
                request.AddHeader("Authorization", "Bearer " + token);
                //NLogLogger.Info(new string[] { "APIVinaPays", "Authorization", "", "", "Bearer " + token });
            }
            //request.AddParameter("application/json", postData, ParameterType.RequestBody);
            //NLogLogger.Info(new string[] { "APIVinaPays", "", "", "", postData });
            var response = client.ExecuteTaskAsync(request);
            return response.Result.Content;

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