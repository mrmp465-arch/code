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

    public class UtilsShopOne
    {

        //public static HttpClient _client;
        public static readonly HttpClient Client = new HttpClient();
        private static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";
        public static async Task<ShopOneCookie> GetImageBase64(string url, CookieContainer cookieContainer)
        {
            var uri = new Uri(url);
            //var cookieContainer = new CookieContainer();
            var shopCookie = new ShopOneCookie();

            var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var httpClient = new HttpClient(httpClientHandler);
            try
            {
                var response = await httpClient.GetByteArrayAsync(uri);
                var imgBase64 = Convert.ToBase64String(response);
                shopCookie.CookieContainer = cookieContainer;
                shopCookie.CaptChaBase64 = imgBase64;
                return shopCookie;
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "ShopOne", "GetImageBase64", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "ShopOne", "GetImageBase64", "TaskCanceledException", e.Message, e.InnerException.Message });
            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "ShopOne", "GetImageBase64", "WebException", e.Message, e.InnerException.Message });
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {

                    NLogLogger.Info(new string[] { "ShopOne", "GetImageBase64", "Exception", e.Message, e.InnerException.Message });
                }
            }


            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId, ShopOneCookie shopCookie)
        {
            try
            {
                var capImageBase64 = Task.Run(() => GetImageBase64(url, shopCookie.CookieContainer)).Result;
                if (capImageBase64 != null)
                {
                    NLogLogger.Info(new string[] { "ShopOne", "DeCaptcha", "OK" });
                    //var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64.CaptChaBase64);
                    var handler = CaptchaFactory.GetHandler(CaptchaProvider);
                    var captcha = handler.ImageToText(capImageBase64.CaptChaBase64, 1);

                    var cap = captcha.Split('|');
                    var captchaSession = new Captcha()
                    {
                        SessionId = sessionId,
                        Value = cap[0],
                        TaskId = Convert.ToInt32(cap[1])
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
                        {"ShopOne", "DeCaptcha", "HttpRequestException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"ShopOne", "DeCaptcha", "TaskCanceledException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (WebException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"ShopOne", "DeCaptcha", "WebException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"ShopOne", "DeCaptcha", "Exception", e.Message, e.InnerException.Message});
                return null;
            }

        }

        public static async Task<ShopOneCookie> PostTask(string url, Dictionary<string, string> postData, CookieContainer cookieContainer)
        {

            //Client.DefaultRequestHeaders.Clear();
            //Client.DefaultRequestHeaders.Add("user-agent", "curl/7.54.0");

            var uri = new Uri(url);
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMinutes(1);
            var shopCookie = new ShopOneCookie();
            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var response = await client.PostAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    shopCookie.CookieContainer = cookieContainer;
                    shopCookie.HtmlContent = responseContent;
                    //var doc = new HtmlDocument();
                    //doc.LoadHtml(responseContent);
                    //var input = doc.DocumentNode.SelectSingleNode("//*[@name='__RequestVerificationToken']");
                    //var token = input.Attributes["value"].Value;
                    //smasCookie.RequestVerificationToken = token;
                    return shopCookie;
                }

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "ShopOne", "PostTask", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "ShopOne", "PostTask", "TaskCanceledException", e.Message });
                return new ShopOneCookie()
                {
                    IsTimeout = true
                };

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "ShopOne", "PostTask", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "ShopOne", "PostTask", "Exception", e.Message });
            }

            return null;

        }

        public static async Task<ShopOneCookie> GetTask(string url, CookieContainer cookieContainer)
        {
            try
            {

                var uri = new Uri(url);
                //var cookieContainer = new CookieContainer();
                var shopCookie = new ShopOneCookie();

                var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var httpClient = new HttpClient(httpClientHandler);
                var response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(responseContent);
                    var input = doc.DocumentNode.SelectSingleNode("//*[@name='token']");
                    if (input != null)
                        shopCookie.RequestVerificationToken = input.Attributes["value"].Value;
                    else
                    {
                        var inputhd = doc.DocumentNode.SelectSingleNode("//*[@id='hd_token']");
                        if (inputhd != null)
                        {
                            shopCookie.RequestVerificationToken = inputhd.Attributes["value"].Value;
                        }
                    }

                    if (doc.GetElementbyId("imgCaptcha") != null)
                    {
                        //shopCookie.CaptChaLink = doc.GetElementbyId("imgCaptcha").GetAttributeValue("src", "");
                        //var uriCaptcha = new Uri("https://shopone.com.vn/captcha.jpeg");
                        //var responseCaptcha = await httpClient.GetAsync(uriCaptcha);
                        //if (responseCaptcha.IsSuccessStatusCode)
                        //{
                            shopCookie.CaptChaLink = "/captcha.jpeg";
                        //}

                    }

                    shopCookie.CookieContainer = cookieContainer;
                    shopCookie.HtmlContent = responseContent;

                }
                return shopCookie;

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "ShopOne", "GetTask", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "ShopOne", "GetTask", "TaskCanceledException", e.Message, e.InnerException.Message });

            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "ShopOne", "GetTask", "WebException", e.Message, e.InnerException.Message });

            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "ShopOne", "GetTask", "Exception", e.Message, e.InnerException.Message });
            }

            return null;

        }

        public static ShopOneCookie GetCookieCache(string sid)
        {
            string KeyCache = sid;
            try
            {
                var result = (ShopOneCookie)Libs.Utils.SharedCache.Get(KeyCache);
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "ShopOne", "GetCookieCache", "Exception", e.Message });
                return null;
            }
        }

        public static void SetCookieCache(string sid, ShopOneCookie shopCookie)
        {
            string KeyCache = sid;
            try
            {
                Libs.Utils.SharedCache.Add(KeyCache, shopCookie);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "ShopOne", "SetCookieCache", "Exception", e.Message });

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
        //        NLogLogger.Info(new string[] { "ShopOne", "SetTokenCache", "Exception", e.Message });
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
            return "shop:" + accountName + ":" + tmp.ToLower(); //+ timeSpan.ToString();
        }



    }
}