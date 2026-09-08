using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Libs.Utils;
using System.Security.Cryptography;
using System.Net;
using System.IO;

namespace Libs.BankDirect.HynBank
{
    public class HynBankLib
    {
        public class GetBankRequest
        {
            public string Type { get; set; }
            public string AccountName { get; set; }
        }
        public class RequestData
        {
            public string PartnerCode { get; set; }
            public string ServiceCode { get; set; }
            public string CommandCode { get; set; }
            public string RequestContent { get; set; }
            public string Signature { get; set; }
        }
        public class BankRequest
        {
            public string requestTime { get; set; }
            public int type { get; set; }
            public string username { get; set; }
            public string authKey { get; set; }
        }

        public class BankResponse
        {
            public string errorCode { get; set; }
            public string errorDescription { get; set; }
            public string destinationInfo { get; set; }
        }

        public class Bank
        {
            public string phone { get; set; }
            public string name { get; set; }
        }

        public class Callback
        {
            public string RefCode { get; set; }

            public string OrderNo { get; set; }
            public int Amount { get; set; }
            public string Mobile { get; set; } 
            public string OrderInfo { get; set; }
            
        }
       

        public class CallbackResponse
        {
            public int ResponseCode { get; set; }
            public string Description { get; set; }

            public string ResponseContent { get; set; }
        }
        public class OrderRequest
        {
            public string Type { get; set; }

            public string AccountName { get; set; }

            public string BankName { get; set; }

            public string AppCode { get; set; }

            public string RefCode { get; set; }

            public int Amount { get; set; }

            public string CallbackUrl { get; set; }
        }

        public class OrderResponse
        {
            public string QRCode { get; set; }

            public string LinkOpenApp { get; set; }

            public string BankAccountName { get; set; }

            public string BankAccountNumber { get; set; }

            public string OrderNo { get; set; }
        }

        public class OrderResponseData
        {
            public string transaction_id { get; set; }

            public string url { get; set; }

            public string full_name { get; set; }

            public string phone_number { get; set; }

            public string match_code { get; set; }
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
                NLogLogger.Info(new string[] { "Hyn", "Exeption Post", e.Message });
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

            NLogLogger.Info(new string[] { "Hyn", "GetTask", url });

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
                NLogLogger.Info(new string[] { "Hyn", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;
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
        public static async Task<string> CallbackJson(string url, string postData, long Id = 0)
        {

            NLogLogger.Info(new string[] { "hyn", "Callback", "Partner", "Request", postData, url });
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
                        NLogLogger.Info(new string[] { "hyn", "Exeption Post", reader.ReadToEnd() });
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
                NLogLogger.Info(new string[] { "hyn", "Exeption Post", e.Message });
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
        public static string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.Default;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }
    }
}
