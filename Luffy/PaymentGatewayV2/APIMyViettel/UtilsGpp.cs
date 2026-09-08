using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using APIMyViettel.Entity;
using HtmlAgilityPack;
using Libs.Utils;
using Newtonsoft.Json.Linq;

namespace APIMyViettel
{
    public class UtilsGpp
    {
        public static async Task<GppCookie> GetTask(string url, GppCookie cookieContainer)
        {
            try
            {

                var uri = new Uri(url);
                //var gppCookie = new GppCookie();
                var httpClientHandler = new HttpClientHandler() { CookieContainer = cookieContainer.CookieContainer };
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", cookieContainer.X_XSRF_TOKEN);
                var response = await httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var responseCookies = cookieContainer.CookieContainer.GetCookies(uri);
                    //gppCookie.CookieContainer = cookieContainer.CookieContainer;
                    foreach (Cookie cookie in responseCookies)
                    {
                        if (cookie.Name == "XSRF-TOKEN")
                            cookieContainer.X_XSRF_TOKEN = cookie.Value;
                    }

                }

                return cookieContainer;

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "GppYTe", "GetLogin", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "GppYTe", "GetLogin", "TaskCanceledException", e.Message, e.InnerException.Message });

            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "GppYTe", "GetLogin", "WebException", e.Message, e.InnerException.Message });

            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "GppYTe", "GetLogin", "Exception", e.Message, e.InnerException.Message });
            }

            return null;

        }

        public static async Task<GppCookie> PostTaskLogin(string url, Dictionary<string, string> postData, GppCookie cookieContainer)
        {

            var uri = new Uri(url);
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer.CookieContainer };
            var httpClient = new HttpClient(handler);
            //httpClient.Timeout = TimeSpan.FromMinutes(1);
            //var gppCookie = new GppCookie();

            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", cookieContainer.X_XSRF_TOKEN);

                var response = await httpClient.PostAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    NLogLogger.Info(new string[] { "GppYTe", "PostTask", "LoginResponse", responseContent });
                    var resLogin = (bool)JObject.Parse(responseContent)["success"];
                    if (resLogin)
                    {
                        var responseCookies = cookieContainer.CookieContainer.GetCookies(uri);
                        cookieContainer.IsLogin = true;
                        foreach (Cookie cookie in responseCookies)
                        {
                            if (cookie.Name == "XSRF-TOKEN")
                                cookieContainer.X_XSRF_TOKEN = cookie.Value;
                        }
                        return cookieContainer;
                    }

                    else
                    {
                        return new GppCookie()
                        {
                            IsLogin = false,
                            ResponseMsg = (string)JObject.Parse(responseContent)["error"]["message"]
                        };
                    }

                }

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "PostTask", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "PostTask", "TaskCanceledException", e.Message });
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "PostTask", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "PostTask", "Exception", e.Message });
            }

            return null;

        }

        public static async Task<string> PostTaskTopup(string url, string postData, GppCookie cookieContainer)
        {

            var uri = new Uri(url);
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer.CookieContainer };
            var httpClient = new HttpClient(handler);
            //httpClient.Timeout = TimeSpan.FromMinutes(1);
            //var gppCookie = new GppCookie();

            try
            {

                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", cookieContainer.X_XSRF_TOKEN);

                var response = await httpClient.PostAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    return "Service Unavailable";
                }
                else
                {
                    return "Token Invalid";
                }

            }
            catch (HttpRequestException e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "PostTask", "HttpRequestException", e.Message });
            }
            catch (TaskCanceledException e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "PostTask", "TaskCanceledException", e.Message });
            }
            catch (WebException e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "PostTask", "WebException", e.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "PostTask", "Exception", e.Message });
            }

            return null;

        }

        public static GppCookie GetCookieCache(string sid)
        {
            string KeyCache = sid;
            try
            {
                var result = (GppCookie)Libs.Utils.SharedCache.Get(KeyCache);
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "GetCookieCache", "Exception", e.Message });
                return null;
            }
        }

        public static void SetCookieCache(string sid, GppCookie GppCookie)
        {
            string KeyCache = sid;
            try
            {
                Libs.Utils.SharedCache.Add(KeyCache, GppCookie);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GppYTe", "SetCookieCache", "Exception", e.Message });

            }
        }

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
            return "gpp:" + accountName + ":" + tmp.ToLower(); //+ timeSpan.ToString();
        }
    }
}