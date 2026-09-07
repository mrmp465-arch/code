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

    public class UtilsGate
    {

        //public static HttpClient _client;
        // public static readonly HttpClient Client = new HttpClient();
        private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "luminati.io";
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-route_err-block";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";
        private static string ProxyListContry = ConfigurationManager.AppSettings["Proxy_List_Contry"] ?? "vn";
        private static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";

        public static async Task<GameCookie> GetImageBase64(string url, CookieContainer cookieContainer)
        {
            var uri = new Uri(url);
            var smasCookie = new GameCookie();

            //var session_id = new Random().Next().ToString();
            //var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
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
                NLogLogger.Info(new string[] { "UtilsGate", "GetImageBase64", "Exception", e.Message });
            }
            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId, GameCookie smasCookie, string captchaProvider = "")
        {
            try
            {
                if (string.IsNullOrEmpty(captchaProvider))
                {
                    captchaProvider = CaptchaProvider;
                }

                var capImageBase64 = Task.Run(async () => await GetImageBase64(url, smasCookie.CookieContainer)).Result;
                //var capImageBase64 = Task.Run(() => GetImageBase64(url, smasCookie.CookieContainer)).Result;

                if (capImageBase64 != null)
                {
                    if (!string.IsNullOrEmpty(capImageBase64.CaptChaBase64))
                    {
                        NLogLogger.Info(new string[] { "GateUtils", "DeCaptcha", "OK" });
                        capImageBase64.CaptChaBase64 = capImageBase64.CaptChaBase64.TrimStart('"').TrimEnd('"');

                        var handler = CaptchaFactory.GetHandler(captchaProvider);
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
                NLogLogger.Info(new string[] { "UtilsGate", "DeCaptcha", "ThreadAbortException", e.Message });
                //Thread.ResetAbort();
            }

            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "DeCaptcha", "HttpRequestException", e.Message, e.StackTrace });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "DeCaptcha", "TaskCanceledException", e.Message, e.StackTrace });
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "DeCaptcha", "WebException", e.Message, e.StackTrace });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "DeCaptcha", "Exception", e.Message, e.StackTrace, url, smasCookie.SessionId });
            }

            return null;
        }

        public static async Task<GameCookie> PostTask(string url, Dictionary<string, string> postData, CookieContainer cookieContainer, long id = 0)
        {

            var uri = new Uri(url);
            var smasCookie = new GameCookie();

            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
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

            //handler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };

            handler.AllowAutoRedirect = true;
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.74 Safari/537.36");


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
            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", id.ToString(), "PostTask", "ThreadAbortException", e.Message });
                Thread.ResetAbort();
            }

            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", id.ToString(), "PostTask", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", id.ToString(), "PostTask", "TaskCanceledException", e.Message });
                return new GameCookie()
                {
                    IsTimeout = true
                };

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "PostTask", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "PostTask", "Exception", e.Message });
            }

            return null;

        }

        public static async Task<GameCookie> PostTaskV2(string url, Dictionary<string, string> postData, CookieContainer cookieContainer, string session_id)
        {

            var uri = new Uri(url);
            var smasCookie = new GameCookie();

            //var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
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

            //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.74 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 5.1; rv:31.0) Gecko/20100101 Firefox/31.0");
            //httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var response = await httpClient.PostAsync(uri, httpContent);
                NLogLogger.Info(new string[] { "UtilsGate", "PostTaskV2", "HTTP CODE", response.StatusCode.ToString() });
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    smasCookie.CookieContainer = cookieContainer;
                    smasCookie.HtmlContent = responseContent;

                    return smasCookie;
                }

            }
            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "PostTaskV2", "ThreadAbortException", e.Message });
                Thread.ResetAbort();
            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "PostTaskV2", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "PostTaskV2", "TaskCanceledException", e.Message });
                return new GameCookie()
                {
                    IsTimeout = true,
                    IsTopup = false

                };

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "PostTaskV2", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "UtilsGate", "PostTaskV2", "Exception", e.Message });
            }

            return null;

        }

        public static async Task<GameCookie> GetTask(string url, CookieContainer cookieContainer)
        {

            var uri = new Uri(url);
            //var cookieContainer = new CookieContainer();
            var gateCookie = new GameCookie();

            HttpClientHandler handler;

            //var session_id = new Random().Next().ToString();
            //var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
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
                    if (doc.GetElementbyId("imageverifier") != null)
                    {
                        gateCookie.CaptChaLink = doc.GetElementbyId("imageverifier").GetAttributeValue("src", "");
                    }

                    //var location = response.Headers.Location;

                    //var headers = response.Headers.Concat(response.Headers);


                    //foreach (var c in cookieContainer.GetCookies(uri).Cast<Cookie>())
                    //{
                    //    Console.WriteLine(c.Name + ":" + c.Value);
                    //}
                    //Console.WriteLine("--------------------------");

                    gateCookie.CookieContainer = cookieContainer;
                    gateCookie.HtmlContent = responseContent;

                }
                return gateCookie;

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "UtilsGate", "GetTask", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "UtilsGate", "GetTask", "TaskCanceledException", e.Message, e.InnerException.Message });

            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "UtilsGate", "GetTask", "WebException", e.Message, e.InnerException.Message });

            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "UtilsGate", "GetTask", "Exception", e.Message, e.InnerException.Message });
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
                NLogLogger.Info(new string[] { "UtilsGate", "GetCookieCache", "Exception", e.Message });
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
                NLogLogger.Info(new string[] { "UtilsGate", "SetCookieCache", "Exception", e.Message });

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
        //        NLogLogger.Info(new string[] {"UtilsGate", "SetTokenCache", "Exception", e.Message });
        //    }
        //}

        public static string GenSid(string accountName)
        {
            //string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
            //string tmp = "";
            //Random rd = new Random();
            //for (int i = 1; i < 8; i++)
            //{
            //    tmp += pp[rd.Next(0, pp.Length - 1)];
            //}
            //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            //return accountName + ":" + tmp.ToLower(); //+ timeSpan.ToString();

            return accountName;
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