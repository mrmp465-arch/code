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
using APIGame.Entity;
using HtmlAgilityPack;
using Libs.Utils;
using Lib.Captcha;


namespace APIGame
{

    public class UtilsVTC
    {

        //public static HttpClient _client;
        // public static readonly HttpClient Client = new HttpClient();
        //private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "luminati.io";
        private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "none";
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-country-vn";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";
        private static string ProxyListContry = ConfigurationManager.AppSettings["Proxy_List_Contry"] ?? "vn";
        private static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";

        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static async Task<GameCookie> GetImageBase64(string url, CookieContainer cookieContainer)
        {
            var uri = new Uri(url);
            var smasCookie = new GameCookie();
            var proxyContry = GenUserProxy(ProxyListContry);
            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(proxyContry + "-session-" + session_id, ProxyPass);

            HttpClientHandler handler;
            switch (ProxySource)
            {
                case "luminati.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
                    break;
                case "smartproxy.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
                    break;
                case "none":
                    handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };
                    break;

                default:
                    handler = new HttpClientHandler { UseCookies = true };
                    break;
            }
            handler.AllowAutoRedirect = true;
            var httpClient = new HttpClient(handler);

            httpClient.Timeout = TimeSpan.FromSeconds(60);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");

            try
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetCaptcha", "Request", proxyContry, url });
                var response = await httpClient.GetByteArrayAsync(uri);
                var imgBase64 = Convert.ToBase64String(response);
                smasCookie.CookieContainer = cookieContainer;
                smasCookie.CaptChaBase64 = imgBase64;
                httpClient.Dispose();
                return smasCookie;
            }
            catch (TimeoutException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "PostTask", "TimeoutException", url, e.Message, e.StackTrace });
            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetImageBase64", "HttpRequestException", e.Message, e.StackTrace });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetImageBase64", "TaskCanceledException", e.Message, e.StackTrace });
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetImageBase64", "WebException", e.Message, e.StackTrace });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetImageBase64", "Exception", e.Message, e.StackTrace });

            }

            httpClient.Dispose();
            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId, GameCookie smasCookie)
        {
            try
            {
                var capImageBase64 = Task.Run(async () => await GetImageBase64(url, smasCookie.CookieContainer)).Result;
                //var capImageBase64 = Task.Run(() => GetImageBase64(url, smasCookie.CookieContainer)).Result;

                if (capImageBase64 != null)
                {
                    if (!string.IsNullOrEmpty(capImageBase64.CaptChaBase64))
                    {
                        NLogLogger.Info(new string[] { "VTCUtils", "DeCaptcha", "OK" });
                        capImageBase64.CaptChaBase64 = capImageBase64.CaptChaBase64.TrimStart('"').TrimEnd('"');
                        //var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64.CaptChaBase64);
                        //var captcha = new Lib.Captcha.CaptchaComVn.CaptchaComVnService().ImageToText(capImageBase64.CaptChaBase64, 3);
                        var handler = CaptchaFactory.GetHandler(CaptchaProvider);
                        var captcha = handler.ImageToText(capImageBase64.CaptChaBase64, 3);

                        if (captcha != null)
                        {
                            var cap = captcha.Split('|');
                            var captchaSession = new Captcha()
                            {
                                SessionId = sessionId,
                                Value = cap[0],
                                TaskId = Convert.ToInt32(cap[1]),
                                ImgBase64 = capImageBase64.CaptChaBase64

                            };
                            //captchaSession.Add();
                            return captchaSession;
                        }
                    }
                }
                return null;
            }

            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "DeCaptcha", "ThreadAbortException", e.Message });
                Thread.ResetAbort();
            }

            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "DeCaptcha", "HttpRequestException", e.Message, e.StackTrace });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "DeCaptcha", "TaskCanceledException", e.Message, e.StackTrace });
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "DeCaptcha", "WebException", e.Message, e.StackTrace });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "DeCaptcha", "Exception", e.Message, e.StackTrace, url, smasCookie.SessionId });
            }

            return null;
        }

        public static Captcha NoCaptcha(string sessionId, GameCookie smasCookie)
        {
            var websiteKey = "6LeFvJsUAAAAAOEszZLFnZKuLn_lFLZHtnPP5djw";
            var websiteURL = "https://vtcgame.vn/nap-vcoin/qua-the-cao.html";
            try
            {

                NLogLogger.Info(new string[] { "VTCUtils", "NoCaptcha", "Request" });

                //var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64.CaptChaBase64);
                //var captcha = new Lib.Captcha.CaptchaComVn.CaptchaComVnService().ImageToText(capImageBase64.CaptChaBase64, 3);
                var handler = CaptchaFactory.GetHandler(CaptchaProvider);
                var captcha = handler.NoCaptchaTaskProxyless(websiteURL, websiteKey);
                if (captcha != null)
                {
                    var cap = captcha.Split('|');
                    var captchaSession = new Captcha()
                    {
                        SessionId = sessionId,
                        Value = cap[0],
                        TaskId = Convert.ToInt32(cap[1]),
                        ImgBase64 = string.Empty

                    };

                    return captchaSession;
                }

                return null;
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "DeCaptcha", "Exception", e.Message, e.StackTrace, smasCookie.SessionId });
            }

            return null;
        }

        public static async Task<GameCookie> PostTask(string url, Dictionary<string, string> postData, CookieContainer cookieContainer)
        {

            var uri = new Uri(url);
            var proxyContry = GenUserProxy(ProxyListContry);
            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(proxyContry + "-session-" + session_id, ProxyPass);

            HttpClientHandler handler;
            switch (ProxySource)
            {
                case "luminati.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
                    break;
                case "smartproxy.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
                    break;
                case "none":
                    handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };
                    break;
                default:
                    handler = new HttpClientHandler { UseCookies = true };
                    break;
            }
            handler.AllowAutoRedirect = true;
            var httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://vtcgame.vn/nap-vcoin/qua-the-cao.html");
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            var smasCookie = new GameCookie();
            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                NLogLogger.Info(new string[] { "VTCUtils", "PostTask", "Request", proxyContry, url, serializer.Serialize(postData) });

                var response = await httpClient.PostAsync(uri, httpContent);
                //var response = httpClient.PostAsync(uri, httpContent).Result;

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var responseContent = response.Content.ReadAsStringAsync().Result;
                    smasCookie.CookieContainer = cookieContainer;
                    smasCookie.HtmlContent = responseContent;
                    httpClient.Dispose();
                    return smasCookie;
                }

            }

            catch (TimeoutException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "PostTask", "TimeoutException", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "PostTask", "HttpRequestException", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "PostTask", "TaskCanceledException", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "PostTask", "WebException", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "PostTask", "Exception", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }

            httpClient.Dispose();
            return null;

        }

        public static async Task<GameCookie> GetTask(string url, CookieContainer cookieContainer)
        {


            var uri = new Uri(url);
            var vtcCookie = new GameCookie();

            var proxyContry = GenUserProxy(ProxyListContry);
            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(proxyContry + "-session-" + session_id, ProxyPass);

            HttpClientHandler handler;
            switch (ProxySource)
            {
                case "luminati.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
                    break;
                case "smartproxy.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
                    break;
                case "none":
                    handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };
                    break;
                default:
                    handler = new HttpClientHandler { UseCookies = true };
                    break;
            }
            handler.AllowAutoRedirect = true;
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://vtcgame.vn/nap-vcoin/qua-the-cao.html");
            try
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetTask", "Request", proxyContry, url });
                var response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var responseContent = response.Content.ReadAsStringAsync().Result;

                    var doc = new HtmlDocument();
                    doc.LoadHtml(responseContent);
                    if (doc.GetElementbyId("ImgcaptchaCard") != null)
                    {
                        vtcCookie.CaptChaLink = doc.GetElementbyId("ImgcaptchaCard").GetAttributeValue("src", "");
                    }

                    if (doc.DocumentNode.SelectSingleNode("//*[@name='__RequestVerificationToken']") != null)
                    {
                        var input = doc.DocumentNode.SelectSingleNode("//*[@name='__RequestVerificationToken']");
                        var token = input.Attributes["value"].Value;
                        vtcCookie.RequestVerificationToken = token;
                    }

                    if (doc.GetElementbyId("key") != null)
                    {
                        vtcCookie.userID = doc.GetElementbyId("key").GetAttributeValue("value", "");
                    }

                    vtcCookie.CookieContainer = cookieContainer;
                    vtcCookie.HtmlContent = responseContent;

                }
                httpClient.Dispose();
                return vtcCookie;

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetTask", "HttpRequestException", url, e.Message, e.StackTrace });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetTask", "TaskCanceledException", url, e.Message, e.StackTrace });

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetTask", "WebException", url, e.Message, e.StackTrace });

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetTask", "Exception", url, e.Message, e.StackTrace });
            }

            httpClient.Dispose();
            return null;

        }

        public static GameCookie GetCookieCache(string sid)
        {
            string KeyCache = sid;
            try
            {
                var result = (GameCookie)Libs.Utils.SharedCache.Get(KeyCache);
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "GetCookieCache", "Exception", e.Message });
                return null;
            }
        }

        public static void SetCookieCache(string sid, GameCookie smasCookie)
        {
            string KeyCache = sid;
            try
            {
                Libs.Utils.SharedCache.Add(KeyCache, smasCookie);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VTCUtils", "SetCookieCache", "Exception", e.Message });

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
        //        NLogLogger.Info(new string[] { "VTCUtils", "SetTokenCache", "Exception", e.Message });
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
            return "vtc:" + accountName + ":" + tmp.ToLower();// + timeSpan;
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

    }
}