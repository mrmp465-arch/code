using System;
using System.Collections.Generic;
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
using System.Configuration;

namespace APIGame
{

    public class UtilsZing
    {
        private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "none";
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-country-vn";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";
        private static string ProxyListContry = ConfigurationManager.AppSettings["Proxy_List_Contry"] ?? string.Empty;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

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
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");

            var smasCookie = new GameCookie();
            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                
                NLogLogger.Info(new string[] { "ZingUtils", "PostTask", "Request", proxyContry, url, serializer.Serialize(postData) });

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
                NLogLogger.Info(new string[] { "Zing", "PostTask", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "Zing", "PostTask", "TaskCanceledException", e.Message });
                return new GameCookie()
                {
                    IsTimeout = true
                };

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "Zing", "PostTask", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Zing", "PostTask", "Exception", e.Message });
            }

            return null;

        }

        public static async Task<GameCookie> GetTask(string url, CookieContainer cookieContainer)
        {
            try
            {

                var uri = new Uri(url);
                var gosuCookie = new GameCookie();
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
                
                NLogLogger.Info(new string[] { "ZingUtils", "GetTask", "Request", proxyContry, url });
                var response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var doc = new HtmlDocument();
                    //doc.LoadHtml(responseContent);
                    //if (doc.GetElementbyId("wCaptcha") != null)
                    //    gosuCookie.CaptChaLink = doc.DocumentNode.SelectSingleNode("//div[@id='wCaptcha']//div[@class='col3']//img").GetAttributeValue("src", "");

                    //foreach (var c in cookieContainer.GetCookies(uri).Cast<Cookie>())
                    //{
                    //    Console.WriteLine(c.Name + ":" + c.Value);
                    //}
                    //Console.WriteLine("--------------------------");

                    gosuCookie.CookieContainer = cookieContainer;
                    gosuCookie.HtmlContent = responseContent;

                }
                return gosuCookie;

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Zing", "GetTask", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Zing", "GetTask", "TaskCanceledException", e.Message, e.InnerException.Message });

            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Zing", "GetTask", "WebException", e.Message, e.InnerException.Message });

            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Zing", "GetTask", "Exception", e.Message, e.InnerException.Message });
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
                NLogLogger.Info(new string[] { "Zing", "GetCookieCache", "Exception", e.Message });
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
                NLogLogger.Info(new string[] { "Zing", "SetCookieCache", "Exception", e.Message });

            }
        }

        public static string GetTokenCache(string appName, string userName)
        {
            string KeyCache = string.Format("{0}:{1}", appName, userName);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                return result;
            }

            catch (ThreadAbortException exp)
            {
                NLogLogger.Info(new string[] { "Zing", "Login", "ThreadAbortException", KeyCache, exp.Message });
                Thread.ResetAbort();
                return null;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Zing", "GetTokenCache", "Exception", KeyCache, e.Message });
                return null;
            }
        }

        public static string SetTokenCache(string appName, string userName, string token)
        {
            string KeyCache = string.Format("{0}:{1}", appName, userName);
            try
            {

                var result = DataCaching.SetCache(KeyCache, token);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Zing", "SetTokenCache", "Exception", e.Message });
                return null;
            }
        }

        public static SessionSid GenSid(string accountName, int gameType)
        {
            //string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
            //string tmp = "";
            //Random rd = new Random();
            //for (int i = 1; i < 16; i++)
            //{
            //    tmp += pp[rd.Next(0, pp.Length - 1)];
            //}
            //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

            string prefix;
            int sessionType;
            switch (gameType)
            {
                case 1: //Thanh toán sản phẩm Võ Lâm Truyền Kỳ Miễn Phí
                    prefix = "z_vltkfree:";
                    sessionType = 10;
                    break;
                case 2: //Thanh toán sản phẩm - Công Thành Chiến
                    prefix = "z_vltkctc:"; 
                    sessionType = 12;
                    break;
                case 3: //Thanh toán sản phẩm VLTK - Võ Lâm Truyền Kỳ
                    prefix = "z_vltk1:";
                    sessionType = 13;
                    break;
                case 4: //Thanh toán sản phẩm - Kiếm Thế
                    prefix = "z_kiemthe:";
                    sessionType = 14;
                    break;
                case 5: //Thanh toán sản phẩm - Tân Thiên Long 3D
                    prefix = "z_tanthienlong3d:";
                    sessionType = 15;
                    break;
                case 6: //Thanh toán sản phẩm - Võ Lâm Truyền Kỳ 2
                    prefix = "z_vltk2:";
                    sessionType = 16;
                    break;
                default:
                    prefix = string.Empty;
                    sessionType = 0;
                    break;
            }

            return new SessionSid()
            {
                Sid = prefix + accountName,
                SessionType = sessionType
            };

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