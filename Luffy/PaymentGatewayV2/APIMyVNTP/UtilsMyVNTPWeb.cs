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

    public class UtilsMyVNTPWeb
    {

        //public static HttpClient _client;
        // public static readonly HttpClient Client = new HttpClient();
        private static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";
        public static async Task<MyVNTPWebCookie> GetImageBase64(string url, CookieContainer cookieContainer)
        {
            var uri = new Uri(url);
            //var cookieContainer = new CookieContainer();
            var smasCookie = new MyVNTPWebCookie();
            var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
            //var httpClientHandler = new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            var httpClient = new HttpClient(httpClientHandler);
            //httpClient.Timeout = TimeSpan.FromSeconds(180);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");

            try
            {
                //var response = await httpClient.GetByteArrayAsync(uri);
                var response = await httpClient.GetAsync(uri);
                //var imgBase64 = Convert.ToBase64String(response);
                smasCookie.CookieContainer = cookieContainer;
                //smasCookie.CaptChaBase64 = imgBase64;
                var responseContent = await response.Content.ReadAsStringAsync();
                smasCookie.CaptChaBase64 = responseContent;
                return smasCookie;
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "MyVNTPWeb", "GetImageBase64", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "MyVNTPWeb", "GetImageBase64", "TaskCanceledException", e.Message, e.InnerException.Message });
            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "MyVNTPWeb", "GetImageBase64", "WebException", e.Message, e.InnerException.Message });
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {

                    NLogLogger.Info(new string[] { "MyVNTPWeb", "GetImageBase64", "Exception", e.Message, e.InnerException.Message });
                }
            }


            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId, MyVNTPWebCookie smasCookie)
        {
            try
            {
                var capImageBase64 = Task.Run(() => GetImageBase64(url, smasCookie.CookieContainer)).Result;
                if (capImageBase64 != null)
                {
                    NLogLogger.Info(new string[] { "MyVNTPWeb", "DeCaptcha", "OK" });
                    //var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64.CaptChaBase64);
                    var handler = CaptchaFactory.GetHandler(CaptchaProvider);
                    var captcha = handler.ImageToText(capImageBase64.CaptChaBase64, 3);
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

                return null;
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"MyVNTPWeb", "DeCaptcha", "HttpRequestException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"MyVNTPWeb", "DeCaptcha", "TaskCanceledException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (WebException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"MyVNTPWeb", "DeCaptcha", "WebException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"MyVNTPWeb", "DeCaptcha", "Exception", e.Message, e.InnerException.Message});
                return null;
            }

        }

        public static async Task<MyVNTPWebCookie> PostTask(string url, Dictionary<string, string> postData, CookieContainer cookieContainer)
        {

            var uri = new Uri(url);
            //var httpClientHandler = new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
            //httpClient.Timeout = TimeSpan.FromSeconds(300);
            var smasCookie = new MyVNTPWebCookie();
            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var response = await httpClient.PostAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();                    
                    smasCookie.CookieContainer = cookieContainer;
                    smasCookie.HtmlContent = responseContent;
                    return smasCookie;
                }

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "TaskCanceledException", e.Message });
                return new MyVNTPWebCookie()
                {
                    IsTimeout = true
                };

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyVNTPWeb", "PostTask", "Exception", e.Message });
            }

            return null;

        }

        public static async Task<MyVNTPWebCookie> GetTask(string url, CookieContainer cookieContainer)
        {
            try
            {

                var uri = new Uri(url);
                //var cookieContainer = new CookieContainer();
                var myVNPTCookie = new MyVNTPWebCookie();
                var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
                //var httpClientHandler = new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
                var httpClient = new HttpClient(httpClientHandler);
                //httpClient.Timeout = TimeSpan.FromSeconds(180);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");

                var response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(responseContent);
                    if (doc.GetElementbyId("m_imgCaptcha") != null)
                        //gosuCookie.CaptChaLink = doc.GetElementbyId("m_imgCaptcha").GetAttributeValue("src", "");
                        myVNPTCookie.CaptChaLink = "https://my.vnpt.com.vn/Payment/GenerateCaptcha";
                    myVNPTCookie.CookieContainer = cookieContainer;
                    myVNPTCookie.HtmlContent = responseContent;

                }
                return myVNPTCookie;

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "MyVNTPWeb", "GetTask", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "MyVNTPWeb", "GetTask", "TaskCanceledException", e.Message, e.InnerException.Message });

            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "MyVNTPWeb", "GetTask", "WebException", e.Message, e.InnerException.Message });

            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "MyVNTPWeb", "GetTask", "Exception", e.Message, e.InnerException.Message });
            }

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
            return "vnpw:" + accountName + ":" + tmp.ToLower(); // + timeSpan;
        }

        public static string GenProxy()
        {
            string[] pp = ("vn.smartproxy.io").Split(',');
            Random rd = new Random();
            var proxySr = pp[rd.Next(0, pp.Length)];
            var proxy = proxySr + ":";

            switch (proxySr)
            {
                case "vn.smartproxy.io":
                    proxy = proxy + rd.Next(46001, 46999 + 1);
                    break;
                case "us.smartproxy.io":
                    proxy = proxy + rd.Next(20000, 29999 + 1);
                    break;
                case "kr.smartproxy.io":
                    proxy = proxy + rd.Next(10001, 19999 + 1);
                    break;
                case "jp.smartproxy.io":
                case "my.smartproxy.io":
                case "th.smartproxy.io":
                case "ph.smartproxy.io":
                    proxy = proxy + rd.Next(30001, 39999 + 1);
                    break;
            }
            return proxy;

        }

       

    }
}