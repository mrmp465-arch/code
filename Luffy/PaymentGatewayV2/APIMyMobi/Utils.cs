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
using System.Web.Script.Serialization;
using Lib.Captcha;
using Libs.Utils;


namespace APIMyMobi
{
    public class Utils
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "luminati.io";
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-static-route_err-pass_dyn";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "fuxjiv7btklo";

        //private static string fcmtoken = "eEnD44afvHE:APA91bFrzG3h84hY73XwTAWjhZXYWTDLvd6mGAygGyAI542CJMji8jn6K34gUqNCtbPyy4M6HMvHs8-y0k22MfwCo1M66lrXpWFplWLE7RxuyV7vSWTmlx09vohOgNKSsIVTHW0skexL";
        private static string fcmtoken = "e57ihuEYSUDtpuqA4D-pSk:APA91bGbaEN68CqI-5gv2Rv-9WDHW3U3DBRIZ-xokHy2jxosl4qUcOFgN0GXNNXFcmDFkNF5mg0KS1o2cHpMUUCNREg_Ok15HH8vHkd09FEZYgJOPDYsAU9dhTrhbkHRClQD30IhYG8L";

        //private static string osinfo = "iOS, 12.2";
        //private static string appversion = "3.3.4";

        private static string osinfo = "iOS, 12.4.6";
        private static string appversion = "3.10.2";

        //private static string deviceinfo = "SM-G532G";
        private static string deviceinfo = "iPhone 6";
        private static string apisecret = "UEJ34gtH345DFG45G3ht1";
        private static string langCode = "vi";
        private static string uuid = "A38B4D16-1AD0-43E3-973C-B1A6FD8C8CCB";


        //public static async Task<string> GeTask(string url, string phone, string userId, string apiSecret)
        //{
        //    try
        //    {

        //        var session_id = new Random().Next().ToString();
        //        var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);

        //        HttpClientHandler handler;
        //        switch (ProxySource)
        //        {
        //            case "luminati.io":
        //                handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
        //                break;
        //            case "smartproxy.io":
        //                handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };
        //                break;
        //            default:
        //                handler = new HttpClientHandler { UseCookies = false };
        //                break;
        //        }


        //        //var handler = new HttpClientHandler { UseCookies = false };
        //        //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };

        //        ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications;
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        //        var client = new HttpClient(handler);

        //        client.DefaultRequestHeaders.Add("User-Agent", "MyMobiFone/3.10.2 (vms.com.MyMobifone; build:20210513.2; iOS 12.4.6) Alamofire/5.4.3");
        //        client.DefaultRequestHeaders.Add("fcmtoken", fcmtoken);
        //        client.DefaultRequestHeaders.Add("phone", phone);
        //        client.DefaultRequestHeaders.Add("osinfo", osinfo);
        //        client.DefaultRequestHeaders.Add("appversion", appversion);
        //        client.DefaultRequestHeaders.Add("deviceinfo", deviceinfo);
        //        client.DefaultRequestHeaders.Add("userid", userId);
        //        client.DefaultRequestHeaders.Add("apisecret", apiSecret);

        //        var response = await client.GetAsync(url);
        //        if (response.Content != null)
        //        {
        //            var responseContent = await response.Content.ReadAsStringAsync();
        //            return responseContent;
        //        }


        //    }
        //    catch (Exception e)
        //    {
        //        NLogLogger.Info(new string[] { "MyMobi", "GeTask", "Exception", e.Message, e.StackTrace });
        //    }

        //    return string.Empty;

        //}
        public static async Task<string> PostTask(string url, Dictionary<string, string> postData, string phone, string apiSecret, string userId)
        {
            try
            {
                NLogLogger.Info(new string[] { "MyMobi", "PostTask", "Request", url, serializer.Serialize(postData) });
                //string deviceId = Encrypts.MD5(DateTime.Now.ToString()).Substring(0, 0x10).ToLower();
                var uri = new Uri(url);

                FormUrlEncodedContent httpContent = null;
                if (postData != null)
                {
                    httpContent = new FormUrlEncodedContent(postData);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                }

                //var client = new HttpClient(new WebRequestHandler() { UseProxy = true, ReadWriteTimeout = 60000, Proxy = new WebProxy(GenProxy()), ServerCertificateValidationCallback = AcceptAllCertifications });

                var session_id = new Random().Next().ToString();
                var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);
                //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
                //var handler = new HttpClientHandler { UseCookies = false };
                //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };

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

                ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var client = new HttpClient(handler);

                //client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.10.0");
                //client.DefaultRequestHeaders.Add("User-Agent", "MyMobiFone/200107 CFNetwork/978.0.7 Darwin/18.5.0");
                client.DefaultRequestHeaders.Add("User-Agent", "MyMobiFone/3.10.2 (vms.com.MyMobifone; build:20210513.2; iOS 12.4.6) Alamofire/5.4.3");
                client.DefaultRequestHeaders.Add("fcmtoken", fcmtoken);
                client.DefaultRequestHeaders.Add("phone", phone);
                client.DefaultRequestHeaders.Add("osinfo", osinfo);
                client.DefaultRequestHeaders.Add("appversion", appversion);
                client.DefaultRequestHeaders.Add("deviceinfo", deviceinfo);
                client.DefaultRequestHeaders.Add("userid", userId);
                client.DefaultRequestHeaders.Add("apisecret", apiSecret);
                client.DefaultRequestHeaders.Add("langcode", langCode);
                client.DefaultRequestHeaders.Add("timeStamp", (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString("##.0000000"));
                client.DefaultRequestHeaders.Add("uuid", uuid);

                NLogLogger.Info(new string[] { "MyMobi", "PostTask", "Request Header", "appversion:", appversion, "deviceinfo:", deviceinfo, "osinfo:", osinfo });

                client.Timeout = TimeSpan.FromSeconds(60);
                var response = await client.PostAsync(uri, httpContent);
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                var responseContent = Encoding.UTF8.GetString(byteArray);
                return responseContent;

            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyMobi", "PostTask", "Exception", url, serializer.Serialize(postData), e.Message, e.StackTrace });
                if (e.Message.Equals("A task was canceled")) // Timeout tự ngắt
                    return "POST_CANCELED";

            }
            return string.Empty;
        }


        //public static async Task<string> PostLoginTask(string url, Dictionary<string, string> postData)
        //{
        //    try
        //    {
        //        NLogLogger.Info(new string[] { "MyMobi", "PostLoginTask", "Request", url, serializer.Serialize(postData) });
        //        var uri = new Uri(url);
        //        var httpContent = new FormUrlEncodedContent(postData);
        //        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        //        var session_id = new Random().Next().ToString();
        //        var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);
        //        var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
        //        //var handler = new HttpClientHandler { UseCookies = false };
        //        ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications;
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //        var client = new HttpClient(handler);

        //        client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.10.0");
        //        client.DefaultRequestHeaders.Add("fcmtoken", fcmtoken);
        //        client.DefaultRequestHeaders.Add("phone", "");
        //        client.DefaultRequestHeaders.Add("osinfo", osinfo);
        //        client.DefaultRequestHeaders.Add("appversion", appversion);
        //        client.DefaultRequestHeaders.Add("deviceinfo", deviceinfo);
        //        client.DefaultRequestHeaders.Add("userid", "0");
        //        client.DefaultRequestHeaders.Add("apisecret", apisecret);

        //        client.Timeout = TimeSpan.FromSeconds(15);
        //        var response = await client.PostAsync(uri, httpContent);
        //        var byteArray = await response.Content.ReadAsByteArrayAsync();
        //        var responseContent = Encoding.UTF8.GetString(byteArray);
        //        return responseContent;


        //    }
        //    catch (Exception e)
        //    {
        //        NLogLogger.Info(new string[] { "MyMobi", "PostTaskLogin", "Exception", e.Message, e.StackTrace });

        //    }
        //    return string.Empty;
        //}

        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static string GetTokenCache(string userName)
        {
            string KeyCache = string.Format("{0}:{1}", "mymobi", userName);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                return result;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyMobiApp", "GetTokenCache", "Exception", KeyCache, e.Message });
                return null;
            }
        }

        public static string SetTokenCache(string userName, string token)
        {
            string KeyCache = string.Format("{0}:{1}", "mymobi", userName);
            try
            {

                var result = DataCaching.SetCache(KeyCache, token, 600 * 3);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyMobiApp", "SetTokenCache", "Exception", e.Message });
                return null;
            }
        }
        public static void RemoveTokenCache(string userName, string token)
        {
            string KeyCache = string.Format("{0}:{1}", "mymobi", userName);
            try
            {

                DataCaching.RemoveCache(KeyCache);


            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyMobiApp", "RemoveTokenCache", "Exception", e.Message });

            }
        }
        public static string GenProxy()
        {
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
            //return "vn.smartproxy.com:46094";


        }
    }




}

