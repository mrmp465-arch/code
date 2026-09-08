using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Libs.Utils;
using System.Net;

namespace Libs.BankDirect.Jav
{
    public class JavBankLib
    {
        public class BankAccountInfo
        {
           
            public string AccountNumber { get; set; }
            public string AccountName { get; set; }
            
        }
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
            public int id { get; set; }
            public string code { get; set; }
            public string bank_name { get; set; }
        }

        public class Bank
        {
            public string phone { get; set; }
            public string name { get; set; }
        }

        public class Callback
        {
            public string type { get; set; }
            public string trans_id { get; set; }
            public string sender { get; set; }
            public string receiver { get; set; }
            public int amount { get; set; }
            public string message { get; set; }
            public string bank_type { get; set; }
            public long timestamp { get; set; }
            public string sign { get; set; }
            public string body { get; set; }
        }

        public class CallbackResponse
        {
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
        }
        public class OrderResponse
        {
            
            public string errorCode { get; set; }

            public string redirectLink { get; set; }

          
            public string banknumber { get; set; }
           
            public string bankname { get; set; }

            public string content { get; set; }

            
            public string timeactive { get; set; }

           
            public string message { get; set; }
        }
        public  class InfoMomo
        {
            public string name { get; set; }
            public string phone { get; set; }
        }

        public  class InfoBank
        {
            public string name { get; set; }
            public string bank_number { get; set; }
            public string bank_type { get; set; }
            public string shortcode { get; set; }
        }

        public  class InfoCallback
        {
            public List<InfoMomo> momo { get; set; }
            //public List<InfoBank> bank { get; set; }
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
                NLogLogger.Info(new string[] { "Jav", "Exeption Post", e.Message });
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

            NLogLogger.Info(new string[] { "Jav", "GetTask", url });

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
                NLogLogger.Info(new string[] { "Jav", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;
        }

        public static async Task<string> CallbackJson(string url, string postData)
        {

            NLogLogger.Info(new string[] { "Jav", "Callback", "Partner", "Request", url, postData });
            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            try
            {
                var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Partner", "Response", responseContent });
                    client.Dispose();
                    return responseContent;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Jav", "Exeption Post", e.Message });
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
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
    }
}
