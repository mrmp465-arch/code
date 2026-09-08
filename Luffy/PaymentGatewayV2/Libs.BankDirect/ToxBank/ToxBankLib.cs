using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Libs.Utils;
using System.Net;
using System.IO;

namespace Libs.BankDirect.ToxBank
{
    public class ToxBankLib
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
            public int stt { get; set; }
            public string msg { get; set; }
            public List<BankResponseData> data { get; set; }
        }
        public class BankResponseData
        {
            public string code { get; set; }
            public string name { get; set; }
        }
        public class Bank
        {
            public string phone { get; set; }
            public string name { get; set; }
        }

        public class Callback
        {
            public string requestId { get; set; }
            public string bank { get; set; }
            public string result { get; set; } // Là OrderNo
            public string chargeAmount { get; set; }
            public string signature { get; set; }
            public string status { get; set; }

            public string chargeType { get; set; }
            public string chargeId { get; set; }
            public string chargeCode { get; set; }// Là OrderNo
            public string momoTransId { get; set; }
            
        }

        public class CallbackResponse
        {
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
        }
        public class OrderResponse
        {

            public int stt { get; set; }
            public string msg { get; set; }
            public OrderResponseData data { get; set; }
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
                NLogLogger.Info(new string[] { "VNPAY", "Exeption Post", e.Message });
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }

        public static async Task<string> GetTask(string url)
        {
            var uri = new Uri(url);
            HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);

            //NLogLogger.Info(new string[] { "VNPAY", "GetTask", url });

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
                NLogLogger.Info(new string[] { "VNPAY", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;
        }

        public static async Task<string> CallbackJson(string url, string postData,long Id=0)
        {

            NLogLogger.Info(new string[] { "VNPAY", "Callback", "Partner", "Request", postData, url });
            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            client.Timeout = TimeSpan.FromSeconds(120);
            try
            {
                var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "VNPAY", "Callback", "Partner", "Response", responseContent });
                    var log =new LogInfo
                    {
                        LogTime=DateTime.Now,
                        Url= url,
                        TransactionID=Id,
                        Request= postData,
                        Respone= responseContent
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
                        NLogLogger.Info(new string[] { "tox", "Exeption Post", reader.ReadToEnd() });
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
                NLogLogger.Info(new string[] { "tox", "Exeption Post", e.Message });
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
