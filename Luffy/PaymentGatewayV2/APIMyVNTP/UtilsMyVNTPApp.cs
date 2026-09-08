using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Libs.Utils;


namespace APIMyVNTP
{
    public class UtilsMyVNTPApp
    {
        private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "luminati.io";
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-route_err-block";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";
        private static string ProxyListContry = ConfigurationManager.AppSettings["Proxy_List_Contry"] ?? "vn"; //"vn,us,jp,sg,hk";
        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static async Task<string> PostTask(string url, string postData)
        {

            var session_id = new Random().Next().ToString();
            var proxyContry = GenUserProxy(ProxyListContry);
            var credentials = new NetworkCredential(proxyContry + "-session-" + session_id, ProxyPass);

            HttpClientHandler handler;
            switch (ProxySource)
            {
                case "luminati.io":
                    handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
                    break;
                case "smartproxy.io":
                    handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };
                    break;
                default:
                    handler = new HttpClientHandler { UseCookies = false };
                    break;
            }

            try
            {


                NLogLogger.Info(new string[] { "TopupAppVNTP", "PostTask", proxyContry, "Request", url, postData });
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
                    client.Timeout = TimeSpan.FromSeconds(60);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    var response = await client.PostAsync(url, httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        NLogLogger.Info(new string[] { "TopupAppVNTP", "PostTask", proxyContry, "Response", responseContent, responseContent });
                        return responseContent;
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        NLogLogger.Info(new string[] { "TopupAppVNTP", "PostTask", proxyContry, "Status", response.StatusCode.ToString(), responseContent });
                    }
                }
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "TopupAppVNTP", "PostTask", proxyContry, "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "TopupAppVNTP", "PostTask", proxyContry, "TaskCanceledException", e.Message, e.InnerException.Message });
            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "TopupAppVNTP", "PostTask", proxyContry, "WebException", e.Message, e.InnerException.Message });
            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "TopupAppVNTP", "PostTask", proxyContry, "Exception", e.Message, e.InnerException.Message });
            }
            return string.Empty;
        }

        public static async Task<string> GetTask(string url)
        {
            try
            {

                var uri = new Uri(url);
                var httpClientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
                var response = await httpClient.GetAsync(uri);
                var responseContent = string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }
                return responseContent;

            }


            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNPApp", "GetTask", "Exception", e.Message });
            }

            return null;

        }

        public static string GetTokenCache(string userName)
        {
            string KeyCache = string.Format("{0}:{1}", "myvnp", userName);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                return result;
            }

            catch (ThreadAbortException exp)
            {
                NLogLogger.Info(new string[] { "MyVNPApp", "Login", "ThreadAbortException", KeyCache, exp.Message });
                Thread.ResetAbort();
                return null;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNPApp", "GetTokenCache", "Exception", KeyCache, e.Message });
                return null;
            }
        }

        public static string SetTokenCache(string userName, string token)
        {
            string KeyCache = string.Format("{0}:{1}", "myvnp", userName);
            try
            {

                var result = DataCaching.SetCache(KeyCache, token);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNPApp", "SetTokenCache", "Exception", e.Message });
                return null;
            }
        }

        public static string GenUserProxy(string listContry = "")
        {
            if (!string.IsNullOrEmpty(listContry))
            {
                string[] pp = listContry.Split(',');
                //string[] pp = ("us,jp,vn,sg").Split(',');
                Random rd = new Random();
                var contryStr = pp[rd.Next(0, pp.Length)];
                var country = ProxyUserName + "-country-" + contryStr;
                return country;
            }
            else
            {
                return ProxyUserName;
            }

        }

        public static string GenProxy()
        {
            ////string[] pp = ("us.smartproxy.io,jp.smartproxy.io,my.smartproxy.io,th.smartproxy.io,kr.smartproxy.io,ph.smartproxy.io,vn.smartproxy.io,sg.smartproxy.io").Split(',');
            //string[] pp = ("vn.smartproxy.io").Split(',');
            //Random rd = new Random();
            //var proxySr = pp[rd.Next(0, pp.Length)];
            //var proxy = proxySr + ":";

            //switch (proxySr)
            //{
            //    case "vn.smartproxy.io":
            //        proxy = proxy + rd.Next(46001, 46999 + 1);
            //        break;
            //    case "us.smartproxy.io":
            //        proxy = proxy + rd.Next(20000, 29999 + 1);
            //        break;
            //    case "kr.smartproxy.io":
            //    case "sg.smartproxy.io":
            //        proxy = proxy + rd.Next(10001, 19999 + 1);
            //        break;
            //    case "jp.smartproxy.io":
            //    case "my.smartproxy.io":
            //    case "th.smartproxy.io":
            //    case "ph.smartproxy.io":
            //        proxy = proxy + rd.Next(30001, 39999 + 1);
            //        break;
            //}
            //return proxy;

            return "vn.smartproxy.com:46000";

        }
    }
}

public class DataRequest
{
    public string card_id { get; set; }
    public string api_secret { get; set; }
    public string fcm_otp { get; set; }
    public string session { get; set; }
    public string for_msisdn { get; set; }
    public string fcm_token { get; set; }
    public string msisdn { get; set; }
}

public class DataRequestV2
{
    public string for_msisdn { get; set; }
    public string card_id { get; set; }
    public string session { get; set; }
    public string api_secret { get; set; }
    public string otp { get; set; }
    public string msisdn { get; set; }

}

public class CardResponse
{

    public string error_code { get; set; }
    public string message { get; set; }
    public string result { get; set; }
    public string debug { get; set; }
}
