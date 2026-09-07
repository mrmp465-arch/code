using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Lib.Captcha;
using Libs.Utils;


namespace APIMobiNext
{
    public class Utils
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-route_err-pass_dyn";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";

        private static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";
        public static async Task<string> GeTask(string url, string token)
        {
            try
            {


                var session_id = new Random().Next().ToString();
                var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);

                var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
                //var handler = new HttpClientHandler { UseCookies = false} ;
                //var handler = new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };

                //var handler = new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };

                ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
                client.DefaultRequestHeaders.Add("token", token);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    NLogLogger.Info(new string[] { "TopupMobiNext", "GeTask", "StatusCode", response.StatusCode.ToString(), response.RequestMessage.ToString() });
                    return "Token Invalid";
                }
                else if (response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    return "Gateway Time-out";
                }
                else if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }


            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "TopupMobiNext", "GeTask", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "TopupMobiNext", "GeTask", "TaskCanceledException", e.Message, e.InnerException.Message });
            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "TopupMobiNext", "GeTask", "WebException", e.Message, e.InnerException.Message });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupMobiNext", "GeTask", "Exception", e.Message });
            }

            return string.Empty;

        }
        public static async Task<string> PostTask(string url, Dictionary<string, string> postData, string deviceId)
        {
            try
            {
                NLogLogger.Info(new string[] { "TopupMobiNext", "PostTask", "Request", url, serializer.Serialize(postData) });
                //string deviceId = Encrypts.MD5(DateTime.Now.ToString()).Substring(0, 0x10).ToLower();
                var uri = new Uri(url);
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                //var handler = new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };

                var session_id = new Random().Next().ToString();
                var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);
                var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
                ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var client = new HttpClient(handler);

                client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
                client.DefaultRequestHeaders.Add("deviceId", deviceId);
                client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.8");
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.Timeout = TimeSpan.FromMinutes(1);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var response = await client.PostAsync(uri, httpContent);
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                //var responseContent = await response.Content.ReadAsStringAsync();
                var responseContent = Encoding.UTF8.GetString(byteArray);
                return responseContent;


            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"TopupMobiNext", "PostTask", "HttpRequestException", e.Message, e.InnerException.Message});
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"TopupMobiNext", "PostTask", "TaskCanceledException", e.Message, e.InnerException.Message});
            }
            catch (WebException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"TopupMobiNext", "PostTask", "WebException", e.Message, e.InnerException.Message});
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupMobiNext", "PostTask", "Exception", e.Message });

            }
            return string.Empty;
        }

        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static string GetImageBase64(string url)
        {

            //HttpClient client = new HttpClient();
            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(ProxyUserName + "-session-" + session_id, ProxyPass);
            var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };

            //var handler = new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };

            ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
            try
            {
                var uri = new Uri(url);
                var response = Task.Run(() => client.GetByteArrayAsync(uri));
                response.Wait();
                var imgBase64 = Convert.ToBase64String(response.Result);
                return imgBase64;
            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "APIMobiNext", "GetImageBase64", "HttpRequestException", e.Message, e.InnerException.Message });
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "APIMobiNext", "GetImageBase64", "TaskCanceledException", e.Message, e.InnerException.Message });
            }
            catch (WebException e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "APIMobiNext", "GetImageBase64", "WebException", e.Message, e.InnerException.Message });
            }
            catch (Exception e)
            {
                if (e.InnerException != null) NLogLogger.Info(new string[] { "APIMobiNext", "GetImageBase64", "Exception", e.Message, e.InnerException.Message });
            }

            return null;
        }
        public static Captcha DeCaptcha(string url, string sessionId)
        {
            try
            {
                var capImageBase64 = GetImageBase64(url);
                if (capImageBase64 != null)
                {
                    NLogLogger.Info(new string[] { "APIMobiNext", "DeCaptcha", "OK" });
                    var handler = CaptchaFactory.GetHandler(CaptchaProvider);
                    var captcha = handler.ImageToText(capImageBase64, 0);
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
                        {"APIMobiNext", "DeCaptcha", "HttpRequestException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"APIMobiNext", "DeCaptcha", "TaskCanceledException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (WebException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"APIMobiNext", "DeCaptcha", "WebException", e.Message, e.InnerException.Message});
                return null;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"APIMobiNext", "DeCaptcha", "Exception", e.Message, e.InnerException.Message});
                return null;
            }

        }
        public static string GenProxy()
        {
            return "vn.smartproxy.com:46000";
            //return "p.webshare.io:20024";
        }
    }

    public class CardResponse
    {
        public int id { get; set; }
        public string phone_number { get; set; }
        public int amount { get; set; }
        public string pin_code { get; set; }
        public DateTime time { get; set; }
    }


}

