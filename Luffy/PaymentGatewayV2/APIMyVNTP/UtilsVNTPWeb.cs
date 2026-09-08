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
using APIMyVNTP.Entity;
using HtmlAgilityPack;
using Libs.Utils;
using Lib.Captcha;


namespace APIMyVNTP
{

    public class UtilsVNTPWeb
    {

        //public static HttpClient _client;
        // public static readonly HttpClient Client = new HttpClient();
        private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "luminati.io";
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-country-vn";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";

        private static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";

        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static async Task<MyVNTPWebCookie> GetImageBase64(string url, CookieContainer cookieContainer)
        {
            var uri = new Uri(url);
            //var cookieContainer = new CookieContainer();
            var smasCookie = new MyVNTPWebCookie();
            //var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
            //var httpClientHandler = new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //var httpClientHandler = new WebRequestHandler() { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //var httpClient = new HttpClient(new WebRequestHandler() { UseProxy = true, ReadWriteTimeout = 60000, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer, ServerCertificateValidationCallback = AcceptAllCertifications });
            
            //var credentials = new NetworkCredential(ProxyUserName, ProxyPass);
            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);

            //var handler = new HttpClientHandler { UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
            //var handler = new HttpClientHandler { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };

            HttpClientHandler handler;
            switch (ProxySource)
            {
                case "luminati.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
                    break;
                case "smartproxy.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
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

                var response = await httpClient.GetAsync(uri);
                //var response = httpClient.GetAsync(uri).Result;

                smasCookie.CookieContainer = cookieContainer;
                //smasCookie.CaptChaBase64 = imgBase64;
                var responseContent = await response.Content.ReadAsStringAsync();
                //var responseContent = response.Content.ReadAsStringAsync().Result;
                smasCookie.CaptChaBase64 = responseContent;
                httpClient.Dispose();
                return smasCookie;
            }
            catch (TimeoutException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "TimeoutException", url, e.Message, e.StackTrace });
            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetImageBase64", "HttpRequestException", e.Message, e.StackTrace });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetImageBase64", "TaskCanceledException", e.Message, e.StackTrace });
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetImageBase64", "WebException", e.Message, e.StackTrace });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetImageBase64", "Exception", e.Message, e.StackTrace });

            }

            httpClient.Dispose();
            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId, MyVNTPWebCookie smasCookie)
        {
            try
            {
                var capImageBase64 = Task.Run(async () => await GetImageBase64(url, smasCookie.CookieContainer)).Result;
                //var capImageBase64 = Task.Run(() => GetImageBase64(url, smasCookie.CookieContainer)).Result;

                if (capImageBase64 != null)
                {
                    if (!string.IsNullOrEmpty(capImageBase64.CaptChaBase64))
                    {
                        NLogLogger.Info(new string[] { "MyVNTPWeb", "DeCaptcha", "OK" });
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
                NLogLogger.Info(new string[] { "MyVNTPWeb", "DeCaptcha", "ThreadAbortException", e.Message });
                Thread.ResetAbort();
            }

            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "DeCaptcha", "HttpRequestException", e.Message, e.StackTrace });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "DeCaptcha", "TaskCanceledException", e.Message, e.StackTrace });
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "DeCaptcha", "WebException", e.Message, e.StackTrace });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "DeCaptcha", "Exception", e.Message, e.StackTrace, url, smasCookie.SessionId });
            }

            return null;
        }

        public static async Task<MyVNTPWebCookie> PostTask(string url, Dictionary<string, string> postData, CookieContainer cookieContainer)
        {

            var uri = new Uri(url);
            //var httpClientHandler = new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //var httpClientHandler = new WebRequestHandler() { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
            //var httpClient = new HttpClient(new WebRequestHandler() { UseProxy = true, ReadWriteTimeout = 60000, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer, ServerCertificateValidationCallback = AcceptAllCertifications });

            //var credentials = new NetworkCredential(ProxyUserName, ProxyPass);
            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);

            //var handler = new HttpClientHandler { UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
            //var handler = new HttpClientHandler { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };

            HttpClientHandler handler;
            switch (ProxySource)
            {
                case "luminati.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
                    break;
                case "smartproxy.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
                    break;
                default:
                    handler = new HttpClientHandler { UseCookies = true };
                    break;
            }
            handler.AllowAutoRedirect = true;
            var httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");

            httpClient.Timeout = TimeSpan.FromSeconds(60);
            var smasCookie = new MyVNTPWebCookie();
            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

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
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "TimeoutException", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "HttpRequestException", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "TaskCanceledException", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "WebException", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "Exception", url, serializer.Serialize(postData), e.Message, e.StackTrace });
            }

            httpClient.Dispose();
            return null;

        }

        public static async Task<MyVNTPWebCookie> GetTask(string url, CookieContainer cookieContainer)
        {


            var uri = new Uri(url);
            //var cookieContainer = new CookieContainer();
            var myVNPTCookie = new MyVNTPWebCookie();
            //var httpClientHandler = new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //var httpClientHandler = new WebRequestHandler() { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
            //var httpClient = new HttpClient(new WebRequestHandler() { UseProxy = true, ReadWriteTimeout = 60000, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer, ServerCertificateValidationCallback = AcceptAllCertifications });


            //var handler = new HttpClientHandler { UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
            //var handler = new HttpClientHandler { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };

            //var credentials = new NetworkCredential(ProxyUserName, ProxyPass);

            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);

            HttpClientHandler handler;
            switch (ProxySource)
            {
                case "luminati.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
                    break;
                case "smartproxy.io":
                    handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
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
                var response = await httpClient.GetAsync(uri);
                //var response = httpClient.GetAsync(uri).Result;

                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetTask", "StatusCode", response.IsSuccessStatusCode.ToString() });

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var responseContent = response.Content.ReadAsStringAsync().Result;

                    var doc = new HtmlDocument();
                    doc.LoadHtml(responseContent);
                    if (doc.GetElementbyId("m_imgCaptcha") != null)
                        //gosuCookie.CaptChaLink = doc.GetElementbyId("m_imgCaptcha").GetAttributeValue("src", "");
                        myVNPTCookie.CaptChaLink = "http://naptien.vinaphone.com.vn/Home/GenerateCaptcha";
                    myVNPTCookie.CookieContainer = cookieContainer;
                    myVNPTCookie.HtmlContent = responseContent;

                }
                httpClient.Dispose();
                return myVNPTCookie;

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetTask", "HttpRequestException", url, e.Message, e.StackTrace });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetTask", "TaskCanceledException", url, e.Message, e.StackTrace });

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetTask", "WebException", url, e.Message, e.StackTrace });

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetTask", "Exception", url, e.Message, e.StackTrace });
            }

            httpClient.Dispose();
            return null;

        }

        public static MyVNTPWebCookie GetCookieCache(string sid)
        {
            string KeyCache = sid;
            try
            {
                var result = (MyVNTPWebCookie)Libs.Utils.SharedCache.Get(KeyCache);
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "GetCookieCache", "Exception", e.Message });
                return null;
            }
        }

        public static void SetCookieCache(string sid, MyVNTPWebCookie smasCookie)
        {
            string KeyCache = sid;
            try
            {
                Libs.Utils.SharedCache.Add(KeyCache, smasCookie);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "SetCookieCache", "Exception", e.Message });

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
        //        NLogLogger.Info(new string[] { "MyVNTPWeb", "SetTokenCache", "Exception", e.Message });
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
            return "vnpweb:" + accountName + ":" + tmp.ToLower();// + timeSpan;
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