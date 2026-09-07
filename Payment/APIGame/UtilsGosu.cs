using System;
using System.Collections;
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


namespace APIGame
{

    public class UtilsGosu
    {

        //public static HttpClient _client;
        // public static readonly HttpClient Client = new HttpClient();

        public static async Task<GameCookie> GetImageBase64(string url, CookieContainer cookieContainer)
        {
            var uri = new Uri(url);
            //var cookieContainer = new CookieContainer();
            var smasCookie = new GameCookie();

            var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var httpClient = new HttpClient(httpClientHandler);
            try
            {
                var response = await httpClient.GetByteArrayAsync(uri);
                var imgBase64 = Convert.ToBase64String(response);
                smasCookie.CookieContainer = cookieContainer;
                smasCookie.CaptChaBase64 = imgBase64;
                return smasCookie;
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Gosu", "GetImageBase64", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Gosu", "GetImageBase64", "TaskCanceledException", e.Message, e.InnerException.Message });
            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Gosu", "GetImageBase64", "WebException", e.Message, e.InnerException.Message });
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {

                    NLogLogger.Info(new string[] { "Gosu", "GetImageBase64", "Exception", e.Message, e.InnerException.Message });
                }
            }


            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId, GameCookie smasCookie)
        {
            try
            {
                //var capImageBase64 = Task.Run(() => GetImageBase64(url, smasCookie.CookieContainer)).Result;
                //if (capImageBase64 != null)
                //{
                //    NLogLogger.Info(new string[] { "Gosu", "DeCaptcha", "OK" });
                //    var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64.CaptChaBase64, 0);
                //    var cap = captcha.Split('|');
                //    var captchaSession = new Captcha()
                //    {
                //        SessionId = sessionId,
                //        Value = cap[0],
                //        TaskId = Convert.ToInt32(cap[1])
                //    };
                //    //captchaSession.Add();
                //    return captchaSession;
                //}

                //return null;

                /////////////////////////
                //NLogLogger.Info(new string[] { "Gosu", "DeCaptcha", "OK" });
                //var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64.CaptChaBase64, 0);
                //var cap = captcha.Split('|');
                var captchaSession = new Captcha()
                {
                    SessionId = sessionId,
                    Value = String.Empty,
                    TaskId = 0
                };
                return captchaSession;

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"Gosu", "DeCaptcha", "HttpRequestException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"Gosu", "DeCaptcha", "TaskCanceledException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (WebException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"Gosu", "DeCaptcha", "WebException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"Gosu", "DeCaptcha", "Exception", e.Message, e.InnerException.Message});
                return null;
            }

        }

        public static async Task<GameCookie> PostTask(string url, Dictionary<string, string> postData, CookieContainer cookieContainer)
        {

            CookieCollection cookieClection = UtilsGosu.GetAllCookies(cookieContainer);
            foreach (var c in cookieClection)
            {
                NLogLogger.Info(new string[] { "GosuService", "PostTask", "GameCookie", c.ToString() });
            }

            var uri = new Uri(url);
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer, AllowAutoRedirect = true};
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
            client.Timeout = TimeSpan.FromMinutes(1);
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            
            var gosuCookie = new GameCookie();
            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var response = await client.PostAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var doc = new HtmlDocument();
                    doc.LoadHtml(responseContent);
                    var input = doc.DocumentNode.SelectSingleNode("//*[@name='__RequestVerificationToken']");
                    if (input != null)
                    {
                        gosuCookie.RequestVerificationToken = input.Attributes["value"].Value;
                        NLogLogger.Info(new string[] { "Gosu", "PostTask", "__RequestVerificationToken", input.Attributes["value"].Value });
                    }

                    gosuCookie.CookieContainer = cookieContainer;
                    gosuCookie.HtmlContent = responseContent;
                    return gosuCookie;
                }

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "Gosu", "PostTask", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "Gosu", "PostTask", "TaskCanceledException", e.Message });
                return new GameCookie()
                {
                    IsTimeout = true
                };

            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "Gosu", "PostTask", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Gosu", "PostTask", "Exception", e.Message });
            }

            return null;

        }

        public static async Task<GameCookie> GetTask(string url, CookieContainer cookieContainer)
        {
            try
            {
                CookieCollection cookieClection = UtilsGosu.GetAllCookies(cookieContainer);
                foreach (var c in cookieClection)
                {
                    NLogLogger.Info(new string[] { "GosuService", "GetTask", "GameCookie", c.ToString() });
                }

                var uri = new Uri(url);
                //var cookieContainer = new CookieContainer();
                var gosuCookie = new GameCookie();

                var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer, AllowAutoRedirect = true };
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

                var response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(responseContent);
                    //if (doc.GetElementbyId("wCaptcha") != null)
                    //    gosuCookie.CaptChaLink = doc.DocumentNode.SelectSingleNode("//div[@id='wCaptcha']//div[@class='col3']//img").GetAttributeValue("src", "");
                    
                    var input = doc.DocumentNode.SelectSingleNode("//*[@name='__RequestVerificationToken']");
                    if (input != null)
                    {
                        gosuCookie.RequestVerificationToken = input.Attributes["value"].Value;
                        NLogLogger.Info(new string[] { "Gosu", "GetTask", "__RequestVerificationToken", input.Attributes["value"].Value });
                    }
                      

                    gosuCookie.CookieContainer = cookieContainer;
                    gosuCookie.HtmlContent = responseContent;

                }
                return gosuCookie;

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Gosu", "GetTask", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Gosu", "GetTask", "TaskCanceledException", e.Message, e.InnerException.Message });

            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Gosu", "GetTask", "WebException", e.Message, e.InnerException.Message });

            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "Gosu", "GetTask", "Exception", e.Message, e.InnerException.Message });
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
                NLogLogger.Info(new string[] { "Gosu", "GetCookieCache", "Exception", e.Message });
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
                NLogLogger.Info(new string[] { "Gosu", "SetCookieCache", "Exception", e.Message });

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
        //        NLogLogger.Info(new string[] { "Gosu", "SetTokenCache", "Exception", e.Message });
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
            return "gosu:" + accountName + ":" + tmp.ToLower(); //+ timeSpan.ToString();
        }

        public static CookieCollection GetAllCookies(CookieContainer container)
        {
            var allCookies = new CookieCollection();
            var domainTableField = container.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == "m_domainTable");
            var domains = (IDictionary)domainTableField.GetValue(container);

            foreach (var val in domains.Values)
            {
                var type = val.GetType().GetRuntimeFields().First(x => x.Name == "m_list");
                var values = (IDictionary)type.GetValue(val);
                foreach (CookieCollection cookies in values.Values)
                {
                    allCookies.Add(cookies);
                }
            }
            return allCookies;
        }

    }
}