using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Huy.Entity;
using Libs.Report;
using Libs.Utils;

namespace Huy
{
    internal class TopupApp
    {


        private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "https://gate.huyphuho.xyz/api";
        private string CallbackUrl = ConfigurationManager.AppSettings["CallbackUrl"] ?? "http://207.148.121.182:1586/CardCallback.ashx";
        private string CallbackUrl2 = ConfigurationManager.AppSettings["CallbackUrl2"] ?? "http://207.148.121.182:1586/CardCallback2.ashx";
        private const string partner = "d394d0d6-e356-4934-9996-d3d672396335";
        private const string key = "9a829c48ddbca0abdc11990b8747e5a2";
        private const string SecretKey = "d2d90872cf914df466210af20d3242da";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public async Task<string> HuyTopupCallback(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {


            var type = string.Empty;

            switch (telco.ToLower())
            {
                case "vtt":
                    type = "1";
                    break;
                case "vms":
                    type = "3";
                    break;
                case "vnp":
                    type = "2";
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Huy";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;
            var refId = topup.Id ;
            try
            {
                var message = "";
                string data = $"{refId}|{partner}|{message}|{pin}|{serial}|{amount}|{type}|{key}";
                var signature = HmacSha256Digest(data, SecretKey);
                var url = ServiceUrl + "/v1/charge?data=" + Base64Encode($"{refId}|{partner}|{message}|{pin}|{serial}|{amount}|{type}|{key}|{signature}");



                NLogLogger.Info(new string[] { "APIHuy", "Request", transactionId, refId.ToString(), url, data });

                var response = Task.Run(async () => await GetTask(url)).Result;

                NLogLogger.Info(new string[] { "APIHuy", "Response", transactionId, refId.ToString(), response });



                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);


                    switch (res.rc)
                    {
                        case 0:
                            return "0|0";
                        case 44:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";
                        case 10001:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -2, string.Empty, string.Empty);
                            return "-2|0";
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }


                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIHuy", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        public async Task<string> HuyTopup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {


            var type = string.Empty;

            switch (telco.ToLower())
            {
                case "vtt":
                    type = "1";
                    break;
                case "vms":
                    type = "3";
                    break;
                case "vnp":
                    type = "2";
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Huy";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;
            var refId = topup.Id ;
            try
            {
                var message = "";
                string data = $"{refId}|{partner}|{message}|{pin}|{serial}|{amount}|{type}|{key}";
                var signature = HmacSha256Digest(data, SecretKey);
                var url = ServiceUrl + "/v1/charge?data=" + Base64Encode($"{refId}|{partner}|{message}|{pin}|{serial}|{amount}|{type}|{key}|{signature}");


                NLogLogger.Info(new string[] { "APIHuy", "Request", transactionId, refId.ToString(), url, data });

                var response = Task.Run(async () => await GetTask(url)).Result;

                NLogLogger.Info(new string[] { "APIHuy", "Response", transactionId, refId.ToString(), response });



                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);


                    switch (res.rc)
                    {
                        case 0:
                            var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                            if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                            break;
                        case 44:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";
                        case 10001:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -2, string.Empty, string.Empty);
                            return "-2|0";
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }


                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIHuy", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "APIHuy", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 90; i++)
            {
                //Check DB 30 lan tuong ung 30s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                    {
                        NLogLogger.Info(new string[] { "APIHuy", "CheckStatusAsync", "Response", id.ToString(), string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount) });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }

                System.Threading.Thread.Sleep(1000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty, string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "APIHuy", "CheckStatusAsync", id.ToString(), "Timeout" });
            return "-326|0";
        }

        public async Task<string> PostTask(string url, Dictionary<string, string> postData)
        {
            try
            {
                var uri = new Uri(url);
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var response = await client.PostAsync(uri, httpContent);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APIHuy", "Exeption Post", e.Message });
                return string.Empty;
            }

            return string.Empty;


        }

        public static async Task<string> PostTask(string url, string postData)
        {

            var uri = new Uri(url);
            var httpClient = new HttpClient();
            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

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
                NLogLogger.Info(new string[] { "APIHuy", "PostTask", "Exception", e.Message });
            }

            httpClient.Dispose();
            return string.Empty;

        }

        public static async Task<string> GetTask(string url)
        {

            var uri = new Uri(url);
            var tryAgain = 0;
            var httpClient = new HttpClient();
            while (tryAgain < 3)
            {

                try
                {
                    var response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseConten = await response.Content.ReadAsStringAsync();
                        httpClient.Dispose();
                        tryAgain = 3;
                        return responseConten;
                    }

                }

                catch (Exception e)
                {
                    tryAgain++;
                    NLogLogger.Info(new string[] { "APIHuy", "GetTask", "Exception", url, e.Message, e.StackTrace });
                }
            }

            httpClient.Dispose();
            return String.Empty;

        }

        public static string HmacSha256Digest(string message, string secretKey)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
            return base64String;
        }

        public static string Base64Encode(object plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText.ToString());
            return System.Convert.ToBase64String(plainTextBytes);
        }

        
    }


}

