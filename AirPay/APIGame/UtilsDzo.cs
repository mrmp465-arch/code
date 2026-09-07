using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIGame.Entity;
using HtmlAgilityPack;
using Libs.Utils;
using Lib.Captcha;
using System.Configuration;

namespace APIGame
{

    public class UtilsDzo
    {

        //public static HttpClient _client;
        // public static readonly HttpClient Client = new HttpClient();
        private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "luminati.io";
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-route_err-block";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";
        private static string ProxyListContry = ConfigurationManager.AppSettings["Proxy_List_Contry"] ?? "vn";

        public static async Task<GameCookie> GetImageBase64(string url, CookieContainer cookieContainer)
        {
            var uri = new Uri(url);
            var smasCookie = new GameCookie();

            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
            HttpClientHandler handler;
            //switch (ProxySource)
            //{
            //    case "luminati.io":
            //        handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
            //        break;
            //    case "smartproxy.io":
            //        handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //        break;
            //    case "none":
            //        handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };
            //        break;
            //    default:
            //        handler = new HttpClientHandler { UseCookies = true };
            //        break;
            //}
            handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };

            handler.AllowAutoRedirect = true;
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");

            try
            {
                var response = await httpClient.GetByteArrayAsync(uri);
                var imgBase64 = Convert.ToBase64String(response);
                smasCookie.CookieContainer = cookieContainer;
                smasCookie.CaptChaBase64 = imgBase64;
                return smasCookie;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "GetImageBase64", "Exception", e.Message });
            }
            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId, CookieContainer cookieContainer, string tranId, string tranId3rd)
        {
            try
            {
                var capImageBase64 = Task.Run(() => GetImageBase64(url, cookieContainer)).Result;
                if (capImageBase64 != null)
                {
                    var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64.CaptChaBase64, 0);
                    if (!string.IsNullOrEmpty(captcha))
                    {
                        var cap = captcha.Split('|');
                        NLogLogger.Info(new string[] { "UtilsDzo", tranId, tranId3rd, "DeCaptcha", "OK", cap[0] });
                        var captchaSession = new Captcha()
                        {
                            SessionId = sessionId,
                            Value = cap[0],
                            TaskId = Convert.ToInt32(cap[1])
                        };
                        //captchaSession.Add();
                        return captchaSession;
                    }
                }
                return null;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "DeCaptcha", "Exception", e.Message });
                return null;
            }

        }
        public static async Task<GameCookie> PostTask(string url, string postData, CookieContainer cookieContainer)
        {
            var uri = new Uri(url);
            //var handler = new HttpClientHandler() { UseCookies = true, CookieContainer = cookieContainer, AllowAutoRedirect = true };
            //var client = new HttpClient(handler);
            //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
            var smasCookie = new GameCookie();
            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
            HttpClientHandler handler;
            //switch (ProxySource)
            //{
            //    case "luminati.io":
            //        handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
            //        break;
            //    case "smartproxy.io":
            //        handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //        break;
            //    case "none":
            //        handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };
            //        break;
            //    default:
            //        handler = new HttpClientHandler { UseCookies = true };
            //        break;
            //}
            handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };

            handler.AllowAutoRedirect = true;
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");

            try
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.PostAsync(uri, httpContent);

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //httpClient.Dispose();
                    smasCookie.CookieContainer = cookieContainer;
                    smasCookie.HtmlContent = responseContent;
                    return smasCookie;

                }

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "PostTask", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "PostTask", "TaskCanceledException", e.Message });
                return new GameCookie()
                {
                    IsTimeout = true
                };

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "PostTask", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "PostTask", "Exception", e.Message });
            }
            //httpClient.Dispose();
            return null;


        }
        public static async Task<GameCookie> PostTask(string url, Dictionary<string, string> postData, CookieContainer cookieContainer)
        {

            var uri = new Uri(url);
            var smasCookie = new GameCookie();

            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
            HttpClientHandler handler;
            //switch (ProxySource)
            //{
            //    case "luminati.io":
            //        handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
            //        break;
            //    case "smartproxy.io":
            //        handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //        break;
            //    case "none":
            //        handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };
            //        break;
            //    default:
            //        handler = new HttpClientHandler { UseCookies = true };
            //        break;
            //}
            handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };

            handler.AllowAutoRedirect = true;
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");


            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"));

                var response = await httpClient.PostAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var doc = new HtmlDocument();
                    //doc.LoadHtml(responseContent);

                    //foreach (var c in cookieContainer.GetCookies(uri).Cast<Cookie>())
                    //{
                    //    Console.WriteLine(c.Name + ":" +c.Value);
                    //}
                    //Console.WriteLine("--------------------------");

                    smasCookie.CookieContainer = cookieContainer;
                    smasCookie.HtmlContent = responseContent;
                    return smasCookie;
                }

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "PostTask", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "PostTask", "TaskCanceledException", e.Message });
                return new GameCookie()
                {
                    IsTimeout = true
                };

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "PostTask", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "PostTask", "Exception", e.Message });
            }

            return null;

        }

        public static async Task<GameCookie> GetTask(string url, CookieContainer cookieContainer)
        {

            var uri = new Uri(url);
            //var cookieContainer = new CookieContainer();
            var gosuCookie = new GameCookie();

            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
            HttpClientHandler handler;
            //switch (ProxySource)
            //{
            //    case "luminati.io":
            //        handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials), CookieContainer = cookieContainer };
            //        break;
            //    case "smartproxy.io":
            //        handler = new HttpClientHandler { UseCookies = true, UseProxy = true, Proxy = new WebProxy(GenProxy()), CookieContainer = cookieContainer };
            //        break;
            //    case "none":
            //        handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };
            //        break;
            //    default:
            //        handler = new HttpClientHandler { UseCookies = true };
            //        break;
            //}
            handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };

            handler.AllowAutoRedirect = true;
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");

            try
            {
                var response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(responseContent);
                    if (doc.GetElementbyId("captcha") != null)
                        gosuCookie.CaptChaLink = doc.GetElementbyId("captcha").GetAttributeValue("src", "");
                    gosuCookie.CookieContainer = cookieContainer;
                    gosuCookie.HtmlContent = responseContent;

                }
                return gosuCookie;

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "UtilsDzo", "GetTask", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "UtilsDzo", "GetTask", "TaskCanceledException", e.Message, e.InnerException.Message });

            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "UtilsDzo", "GetTask", "WebException", e.Message, e.InnerException.Message });

            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "UtilsDzo", "GetTask", "Exception", e.Message, e.InnerException.Message });
            }

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
                NLogLogger.Info(new string[] { "UtilsDzo", "GetCookieCache", "Exception", e.Message });
                return null;
            }
        }

        public static void SetCookieCache(string sid, GameCookie gameCookie)
        {
            string KeyCache = sid;
            try
            {
                Libs.Utils.SharedCache.Add(KeyCache, gameCookie);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "UtilsDzo", "SetCookieCache", "Exception", e.Message });

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
        //        NLogLogger.Info(new string[] {"UtilsDzo", "SetTokenCache", "Exception", e.Message });
        //    }
        //}

        public static string GenSid(string accountName)
        {
            //string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
            //string tmp = "";
            //Random rd = new Random();
            //for (int i = 1; i < 16; i++)
            //{
            //    tmp += pp[rd.Next(0, pp.Length - 1)];
            //}
            //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return "dzo:" + accountName; // + ":" + tmp.ToLower(); //+ timeSpan.ToString();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
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