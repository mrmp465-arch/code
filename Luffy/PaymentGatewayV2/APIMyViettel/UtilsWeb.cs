using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMyViettel.Entity;
using HtmlAgilityPack;
using Libs.Utils;
using Lib.Captcha;

namespace APIMyViettel
{

    public class UtilsWeb
    {


        private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "n/a";// "luminati.io";
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-country-vn";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";
        private static string ProxyListContry = ConfigurationManager.AppSettings["Proxy_List_Contry"] ?? string.Empty;

        //public static HttpClient _client;
        public static readonly HttpClient Client = new HttpClient();
        private static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";

        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static string GetImageBase64(string url)
        {

            for (int i = 0; i < 5; i++)
            {

                var session_id = new Random().Next().ToString();
                var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
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

                var client = new HttpClient(handler);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36");
                client.Timeout = TimeSpan.FromSeconds(5);


                NLogLogger.Info(new string[] { "MyViettelWeb", "GetImageBase64", session_id, "Try", i.ToString(), url });
                //Log proxy Ip
                //Task.Run(async () => await LogIpProxy(client, url, session_id).ConfigureAwait(false));
                try
                {
                    var uri = new Uri(url);
                    var response = Task.Run(async () => await client.GetByteArrayAsync(uri)).Result;
                    var imgBase64 = Convert.ToBase64String(response);
                    client.Dispose();
                    return imgBase64;
                }

                catch (Exception ex)
                {

                    NLogLogger.Info(new string[] { "MyViettelWeb", "GetImageBase64", credentials.UserName, "Exception", url, ex.Message, ex.StackTrace });


                }
                client.Dispose();
                Thread.Sleep(1000);
            }

            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId)//, HttpClient Client)
        {
            try
            {

                var capImageBase64 = GetImageBase64(url);//, Client);
                if (capImageBase64 != null)
                {
                    //NLogLogger.Info(new string[] { "MyViettelWeb", "DeCaptcha", "OK" });

                    var handler = CaptchaFactory.GetHandler(CaptchaProvider);
                    var captcha = handler.ImageToText(capImageBase64, 1);
                    //var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64);
                    //var captcha = new Lib.Captcha.CaptchaComVn.CaptchaComVnService().ImageToText(capImageBase64, 1);
                    if (captcha != null)
                    {
                        var cap = captcha.Split('|');
                        var captchaSession = new Captcha()
                        {
                            SessionId = sessionId,
                            Value = cap[0],
                            TaskId = Convert.ToInt32(cap[1]),
                            ImgBase64 = capImageBase64
                        };
                        //captchaSession.Add();
                        return captchaSession;
                    }
                }

            }

            catch (AggregateException e)
            {
                foreach (var errInner in e.InnerExceptions)
                {
                    NLogLogger.Info(new string[]
                        {"MyViettelWeb", "DeCaptcha", "AggregateException", errInner.Message});
                }
            }

            return null;

        }

        public static async Task<string> PostTask(string url, string postData)
        {

            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
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
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(60);

            NLogLogger.Info(new string[] { "MyViettelWeb", "PostTask", session_id, url, postData });

            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Log proxy Ip
            //Task.Run(async () => await LogIpProxy(client, url, session_id).ConfigureAwait(false));
            try
            {
                //var response = client.PostAsync(uri, httpContent).Result;
                var response = await client.PostAsync(uri, httpContent);

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    client.Dispose();
                    return responseContent;
                }

            }

            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "MyViettelWeb", "PostTask", credentials.UserName, "Exception", serializer.Serialize(postData), ex.Message, ex.StackTrace });
            }


            client.Dispose();
            return string.Empty;

        }
        public static async Task<string> GetTask(string url, bool isProxy = true)//, HttpClient Client)
        {

            var uri = new Uri(url);
            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);

            HttpClientHandler handler;

            if (isProxy)
            {
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
            }
            else
            {
                handler = new HttpClientHandler { UseCookies = false };
            }

            var client = new HttpClient(handler);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(60);

            NLogLogger.Info(new string[] { "MyViettelWeb", "GetTask", session_id, url });
            //Log proxy Ip
            //Task.Run(async () => await LogIpProxy(client, url, session_id).ConfigureAwait(false));

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

            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "MyViettelWeb", "GetTask", credentials.UserName, "Exception", url, ex.Message, ex.StackTrace });
            }
            client.Dispose();
            return string.Empty;


        }

        public static WebCookie GetCookieCache(string sid)
        {
            string KeyCache = sid;
            try
            {
                var result = (WebCookie)Libs.Utils.SharedCache.Get(KeyCache);
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVTTWeb", "GetCookieCache", "Exception", e.Message });
                return null;
            }
        }

        public static void SetCookieCache(string sid, WebCookie webCookie)
        {
            string KeyCache = sid;
            try
            {
                Libs.Utils.SharedCache.Add(KeyCache, webCookie);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVTTWeb", "SetCookieCache", "Exception", e.Message });

            }
        }

        //public static void RemoveCookieCache(string sid)
        //{
        //    string KeyCache = sid;
        //    try
        //    {
        //        CacheLayer.Clear(KeyCache);
        //    }
        //    catch (Exception e)
        //    {
        //        NLogLogger.Info(new string[] { "MyVTTWeb", "SetTokenCache", "Exception", e.Message });
        //    }
        //}

        public static string GenSid(string accountName)
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i < 16; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return "vttweb:" + accountName + ":" + tmp.ToLower(); //+ timeSpan.ToString();
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
            return "vn.smartproxy.com:46000";
        }

        public static string SetCardSerialCache(string cardSerial, string value)
        {
            string KeyCache = string.Format("{0}:{1}", "serial", cardSerial);
            try
            {

                var result = DataCaching.SetCache(KeyCache, value);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelWeb", "SetCardSerialCache", "Exception", e.Message });
                return null;
            }
        }

        public static string GetCardSerialCache(string cardSerial)
        {
            string KeyCache = string.Format("{0}:{1}", "serial", cardSerial);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                return result;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelWeb", "GetCardSerialCache", "Exception", KeyCache, e.Message });
                return null;
            }
        }

        public static string SetTokenCache(string userName, string token)
        {
            string KeyCache = string.Format("{0}:{1}", "myvtt", userName);
            try
            {

                var result = DataCaching.SetCache(KeyCache, token, 2592000);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelWeb", "SetTokenCache", "Exception", e.Message });
                return null;
            }
        }

        public static void RemoveTokenCache(string userName)
        {
            string KeyCache = string.Format("{0}:{1}", "myvtt", userName);
            try
            {
                DataCaching.RemoveCache(KeyCache);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelWeb", "SetTokenCache", "Exception", e.Message });
            }
        }

        public static string GetTokenCache(string userName)
        {
            string KeyCache = string.Format("{0}:{1}", "myvtt", userName);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                return result;
            }

            catch (ThreadAbortException exp)
            {
                NLogLogger.Info(new string[] { "MyViettelWeb", "Login", "ThreadAbortException", KeyCache, exp.Message });
                Thread.ResetAbort();
                return null;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelWeb", "GetTokenCache", "Exception", KeyCache, e.Message });
                return null;
            }
        }

        public static string SetConfigCache(string key, string value)
        {
            string KeyCache = string.Format("{0}:{1}", "config", key);
            try
            {

                var result = DataCaching.SetCache(KeyCache, value);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Pay", "SetConfigCache", "Exception", e.Message });
                return null;
            }
        }
    }
}