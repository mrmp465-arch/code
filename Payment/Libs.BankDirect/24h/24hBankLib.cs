using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Libs.Utils;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace Libs.BankDirect_24h
{
    public class _24hBankLib
    {

        public class BankRequest
        {
            public string requestTime { get; set; }
            public int type { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string authKey { get; set; }
        }

        public class BankResponse
        {
            public bool success { get; set; }
            public int code { get; set; }
            public string message { get; set; }
            public BankResponseData data { get; set; }

        }
        public class BankResponseData
        {
            public string cash_in_id { get; set; }
            public string request_id { get; set; }
            public string code { get; set; }
            public int amount { get; set; }
            public string type { get; set; }
            public string acc_no { get; set; }
            public string acc_name { get; set; }
            public string qr_code { get; set; }
            public string image_qrcode { get; set; }
            public string bank_name { get; set; }
            public string bank_code { get; set; }
        }
        public class Bank
        {
            public string phone { get; set; }
            public string name { get; set; }
        }

        public class Callback
        {
            public string request_id { get; set; }
            public string code { get; set; }
            public int amount_transfer { get; set; }

            public int amount { get; set; }
            public string cash_in_id { get; set; }
            public int status { get; set; }

            public string type { get; set; }

            public string vsign { get; set; }

            
        }

        public class CallbackResponse
        {
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
        }
        public class OrderResponse
        {
            public string Url { get; set; }
            public string AccountName { get; set; }
            public string AccountNumber { get; set; }
            public string BankCode { get; set; }
            public string QRCode { get; set; }
            public string OrderNo { get; set; }
            //public int Timeout { get; set; }
        }
        public class OrderResponseData
        {

            public int id { get; set; }
            public string code { get; set; }
            public string qr_url { get; set; }
            public string payment_url { get; set; }
            public string phoneNum { get; set; }
            public string phoneName { get; set; }
            public int amount { get; set; }
            public string bank_provider { get; set; }
            public object redirect { get; set; }
            public int timeToExpired { get; set; }
        }
        public static async Task<string> PostTask(string url, string postData)
        {

            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);
            try
            {
                var response = await client.PostAsync(uri, httpContent);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    client.Dispose();
                    return responseContent;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Bicbic", "Exeption Post", e.Message });
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }

        public static async Task<string> GetTask(string url)
        {
            var uri = new Uri(url);
            HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(120);

            NLogLogger.Info(new string[] { "Bicbic", "GetTask", url });

            try
            {
                var response = await client.GetAsync(uri);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    client.Dispose();
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Bicbic", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;
        }
        public static string ToSha256(string input)
        {
            if (input == null)
                input = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
        public static async Task<string> CallbackJson(string url, string postData, long Id = 0, string refcode = "")
        {

            NLogLogger.Info(new string[] { "VNpay", "Callback", "Partner", "Request", postData });
            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(90);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            try
            {
                var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "MDrum", "Callback", "Transid", "Refcode", "Response", Id.ToString(), refcode, responseContent });
                    var log = new LogInfo
                    {
                        LogTime = DateTime.Now,
                        Url = url,
                        TransactionID = Id,
                        Request = postData,
                        Respone = responseContent
                    };
                    LogCache.LogBank(log);
                    client.Dispose();
                    return responseContent;
                }

            }
            catch (WebException e)
            {
                var responseStream = e.Response.GetResponseStream();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        NLogLogger.Info(new string[] { "VNpay", "Exeption Post", reader.ReadToEnd() });
                        var log1 = new LogInfo
                        {
                            LogTime = DateTime.Now,
                            Url = url,
                            TransactionID = Id,
                            Request = postData,
                            Respone = reader.ReadToEnd()
                        };
                        LogCache.LogBank(log1);
                        //return result;
                    }
                }
                NLogLogger.Info(new string[] { "VNpay", "Exeption Post", e.Message });
                var log = new LogInfo
                {
                    LogTime = DateTime.Now,
                    Url = url,
                    TransactionID = Id,
                    Request = postData,
                    Respone = e.Message
                };
                LogCache.LogBank(log);
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }
    }
}
