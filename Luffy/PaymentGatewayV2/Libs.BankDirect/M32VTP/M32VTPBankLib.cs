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

namespace Libs.BankDirect.M32VTP
{
    public class M32VTPBankLib
    {

        public class BankRequest
        {
            public string requestTime { get; set; }
            public int type { get; set; }
            public string userName { get; set; }
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
            public string requestTime { get; set; }
            public string vtp_transId { get; set; }
            public string message { get; set; } // Là OrderNo
            public string money { get; set; }
            public string phone { get; set; }
            public int type { get; set; }
            public string authKey { get; set; }

           // public string account_receive { get; set; }
        }
        public class CallbackV2
        {
            public string requestTime { get; set; }
            public string momo_transId { get; set; }
            public string message { get; set; } // Là OrderNo
            public string money { get; set; }
            public string phone { get; set; }
            public int type { get; set; }
            public string authKey { get; set; }
            public string pid { get; set; }
            public string account_receive { get; set; }
            
        }

        public class CallbackResponse
        {
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
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
                NLogLogger.Info(new string[] { "M32VTP", "Exeption Post", e.Message });
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

            NLogLogger.Info(new string[] { "M32VTP", "GetTask", url });

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
                NLogLogger.Info(new string[] { "M32VTP", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;
        }

        public static async Task<string> CallbackJson(string url, string postData)
        {

            NLogLogger.Info(new string[] { "M32VTP", "Callback", "Partner", "Request", postData, url });
            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            try
            {
                var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "M32VTP", "Callback", "Partner", "Response", responseContent });
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
                        NLogLogger.Info(new string[] { "M32VTP", "Exeption Post", reader.ReadToEnd() });
                        //return result;
                    }
                }
                NLogLogger.Info(new string[] { "M32VTP", "Exeption Post", e.Message });
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }
    }
}
