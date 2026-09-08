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
using Khoai.Entity;
using Libs.Report;
using Libs.Utils;

namespace Khoai
{
    internal class TopupApp
    {

        //private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "https://khoainuong.info";
        private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "https://cms.khoainuong.info/sendcard.php";
        private string CallbackUrl = ConfigurationManager.AppSettings["CallbackUrl"] ?? "http://149.28.151.96:1597/CardCallback.ashx";
        private string CallbackUrlV2 = ConfigurationManager.AppSettings["CallbackUrlV2"] ?? "http://149.28.151.96:1597/CardCallbackV2.ashx";
        private string CallbackUrlV3 = ConfigurationManager.AppSettings["CallbackUrlV3"] ?? "http://149.28.151.96:1597/CardCallbackV3.ashx";
        private string ZingCallbackUrl = ConfigurationManager.AppSettings["ZingCallbackUrl"] ?? "http://149.28.151.96:1597/ZingCallback.ashx";
        private const string key = "b0e08528827db52f07eabddc088000d3";
        private const string key2 = "e07166726f03b5bc51f15410efed00b8";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public async Task<string> KhoaiTopup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {

            var Key = key;
            var Url = CallbackUrl;
            if (providerCode.Contains("nem"))
            {
                Key = key2;
                Url = CallbackUrlV2;
            }

            var type = string.Empty;
            //var Url = CallbackUrl;
            switch (telco.ToLower())
            {
                case "vtt":
                    type = "VTT";
                    break;
                case "vms":
                    type = "VMS";
                    break;
                case "vnp":
                    type = "VNP";
                    break;
                case "zing":
                    type = "ZING";
                    Url = ZingCallbackUrl;
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Khoai";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {

                var signature = Encrypts.MD5(Key + topup.CardCode + topup.CardSerial);
                //var url = ServiceUrl + "/api/sendCard_v3?" + string.Format("key={0}&cardSeri={1}&cardType={2}&cardValue={3}&cardCode={4}&callbackUrl={5}&refcode={6}&signature={7}", key, serial, type, amount, pin, CallbackUrl, topup.Id, signature);
                var url = ServiceUrl + "?" + string.Format("key={0}&cardSeri={1}&cardType={2}&cardValue={3}&cardCode={4}&callbackUrl={5}&refcode={6}&signature={7}", Key, serial, type, amount, pin, Url, topup.Id, signature);
                var sw = new Stopwatch();
                sw.Start();
                NLogLogger.Info(new string[] { "APIKhoai", "Request", transactionId, topup.Id.ToString(), url });

                var response = Task.Run(async () => await GetTask(url)).Result;
                sw.Stop();
                NLogLogger.Info(new string[] { "APIKhoai", "Response", transactionId, topup.Id.ToString(), response, sw.ElapsedMilliseconds.ToString() });


                if (sw.ElapsedMilliseconds > 5000)
                {
                    TelegramNotify.SendWarning("1690000254", $"Nghi vấn thẻ {serial} nhận đơn chậm");
                }
                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);


                    switch (res.status_code)
                    {
                        case "receive_success":
                            var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                            if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                            break;
                        case "card_duplicate":
                            if (type == "ZING" && partnerCode=="hyn2")
                               TelegramNotify.SendWarning("1690000254", $"-7 (Transaction Fail) -{ topup.CardSerial} . Các anh check hệ thống giúp em (^_^)");
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                            return "-7|0";
                        case "card_format_wrong":
                            DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                            return "-334|0";
                        case "type_maintenance":
                            DataRequest.UpdateTopupCard(topup.Id, 0, -300, string.Empty, string.Empty);
                            return "-300|0";
                        case "type_error":
                            DataRequest.UpdateTopupCard(topup.Id, 0, -336, string.Empty, string.Empty);
                            return "-336|0";
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }


                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIKhoai", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        public async Task<string> KhoaiTopupCallBack(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {

            var Key = key;
            var Url = CallbackUrlV3;


            var type = string.Empty;
            //var Url = CallbackUrl;
            switch (telco.ToLower())
            {
                case "vtt":
                    type = "VTT";
                    break;
                case "vms":
                    type = "VMS";
                    break;
                case "vnp":
                    type = "VNP";
                    break;
                case "zing":
                    type = "ZING";
                    Url = ZingCallbackUrl;
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Khoai";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {

                var signature = Encrypts.MD5(Key + topup.CardCode + topup.CardSerial);
                //var url = ServiceUrl + "/api/sendCard_v3?" + string.Format("key={0}&cardSeri={1}&cardType={2}&cardValue={3}&cardCode={4}&callbackUrl={5}&refcode={6}&signature={7}", key, serial, type, amount, pin, CallbackUrl, topup.Id, signature);
                var url = ServiceUrl + "?" + string.Format("key={0}&cardSeri={1}&cardType={2}&cardValue={3}&cardCode={4}&callbackUrl={5}&refcode={6}&signature={7}", Key, serial, type, amount, pin, Url, topup.Id, signature);
                var sw = new Stopwatch();
                sw.Start();
                NLogLogger.Info(new string[] { "APIKhoai", "Request", transactionId, topup.Id.ToString(), url });

                var response = Task.Run(async () => await GetTask(url)).Result;
                sw.Stop();
                NLogLogger.Info(new string[] { "APIKhoai", "Response", transactionId, topup.Id.ToString(), response, sw.ElapsedMilliseconds.ToString() });


                if (sw.ElapsedMilliseconds > 5000)
                {
                    TelegramNotify.SendWarning("1690000254", $"Nghi vấn thẻ {serial} nhận đơn chậm");
                }
                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);


                    switch (res.status_code)
                    {
                        case "receive_success":
                            return "0|0";
                        //break;
                        case "card_duplicate":
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                            return "-7|0";
                        case "card_format_wrong":
                            DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                            return "-334|0";
                        case "type_maintenance":
                            DataRequest.UpdateTopupCard(topup.Id, 0, -300, string.Empty, string.Empty);
                            return "-300|0";
                        case "type_error":
                            DataRequest.UpdateTopupCard(topup.Id, 0, -336, string.Empty, string.Empty);
                            return "-336|0";
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }


                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIKhoai", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "APIKhoai", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 30; i++)
            {
                //Check DB 30 lan tuong ung 90s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                    {
                        NLogLogger.Info(new string[] { "APIKhoai", "CheckStatusAsync", "Response", id.ToString(), string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount) });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }

                System.Threading.Thread.Sleep(3000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty, string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "APIKhoai", "CheckStatusAsync", id.ToString(), "Timeout" });
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
                NLogLogger.Info(new string[] { "APIKhoai", "Exeption Post", e.Message });
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
                NLogLogger.Info(new string[] { "APIKhoai", "PostTask", "Exception", e.Message });
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
                    NLogLogger.Info(new string[] { "APIKhoai", "GetTask", "Exception", url, e.Message, e.StackTrace });
                }
            }

            httpClient.Dispose();
            return String.Empty;

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