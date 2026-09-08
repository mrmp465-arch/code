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

    public class UtilsSmas
    {

        //public static HttpClient _client;
        public static readonly HttpClient Client = new HttpClient();
        private static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";
        public static async Task<SmasCookie> GetImageBase64(string url, CookieContainer cookieContainer)
        {
            var uri = new Uri(url);
            //var cookieContainer = new CookieContainer();
            var smasCookie = new SmasCookie();

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
                if (e.InnerException != null) NLogLogger.Info(new string[] { "SmasEdu", "GetImageBase64", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "SmasEdu", "GetImageBase64", "TaskCanceledException", e.Message, e.InnerException.Message });
            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "SmasEdu", "GetImageBase64", "WebException", e.Message, e.InnerException.Message });
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {

                    NLogLogger.Info(new string[] { "SmasEdu", "GetImageBase64", "Exception", e.Message, e.InnerException.Message });
                }
            }


            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId, SmasCookie smasCookie)
        {
            try
            {
                var capImageBase64 = Task.Run(() => GetImageBase64(url, smasCookie.CookieContainer)).Result;
                if (capImageBase64 != null)
                {
                    NLogLogger.Info(new string[] { "SmasEdu", "DeCaptcha", "OK" });
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
                        {"SmasEdu", "DeCaptcha", "HttpRequestException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"SmasEdu", "DeCaptcha", "TaskCanceledException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (WebException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"SmasEdu", "DeCaptcha", "WebException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"SmasEdu", "DeCaptcha", "Exception", e.Message, e.InnerException.Message});
                return null;
            }

        }

        public static async Task<SmasCookie> PostTask(string url, Dictionary<string, string> postData, CookieContainer cookieContainer)
        {

            //Client.DefaultRequestHeaders.Clear();
            //Client.DefaultRequestHeaders.Add("user-agent", "curl/7.54.0");

            var uri = new Uri(url);
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMinutes(1);
            var smasCookie = new SmasCookie();
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
                    var token = input.Attributes["value"].Value;
                    smasCookie.RequestVerificationToken = token;
                    smasCookie.CookieContainer = cookieContainer;
                    smasCookie.HtmlContent = responseContent;
                    return smasCookie;
                }

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "SmasEdu", "PostTask", "HttpRequestException", e.Message});
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "SmasEdu", "PostTask", "TaskCanceledException", e.Message });
                return new SmasCookie()
                {
                    IsTimeout = true
                };
                
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "SmasEdu", "PostTask", "WebException", e.Message});
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "SmasEdu", "PostTask", "Exception", e.Message});
            }

            return null;

        }

        public static async Task<SmasCookie> GetTask(string url, CookieContainer cookieContainer)
        {
            try
            {

                var uri = new Uri(url);
                //var cookieContainer = new CookieContainer();
                var smasCookie = new SmasCookie();

                var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var httpClient = new HttpClient(httpClientHandler);
                var response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(responseContent);
                    var input = doc.DocumentNode.SelectSingleNode("//*[@name='__RequestVerificationToken']");
                    var token = input.Attributes["value"].Value;
                    if (doc.GetElementbyId("captcha") != null)
                        smasCookie.CaptChaLink = doc.GetElementbyId("captcha").GetAttributeValue("src", "");
                    smasCookie.RequestVerificationToken = token;
                    smasCookie.CookieContainer = cookieContainer;
                    smasCookie.HtmlContent = responseContent;

                }
                return smasCookie;

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "SmasEdu", "GetTask", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "SmasEdu", "GetTask", "TaskCanceledException", e.Message, e.InnerException.Message });

            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "SmasEdu", "GetTask", "WebException", e.Message, e.InnerException.Message });

            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "SmasEdu", "GetTask", "Exception", e.Message, e.InnerException.Message });
            }

            return null;

        }

        public static SmasCookie GetCookieCache(string sid)
        {
            string KeyCache = sid;
            try
            {
                var result = (SmasCookie)Libs.Utils.SharedCache.Get(KeyCache);
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "SmasEdu", "GetCookieCache", "Exception", e.Message });
                return null;
            }
        }

        public static void SetCookieCache(string sid, SmasCookie smasCookie)
        {
            string KeyCache = sid;
            try
            {
                Libs.Utils.SharedCache.Add(KeyCache, smasCookie);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "SmasEdu", "SetCookieCache", "Exception", e.Message });

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
        //        NLogLogger.Info(new string[] { "SmasEdu", "SetTokenCache", "Exception", e.Message });
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
            return "smas:" + accountName + ":" + tmp.ToLower(); //+ timeSpan.ToString();
        }



    }
}