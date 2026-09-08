using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMyViettel.Entity;
using Libs.Utils;
using Lib.Captcha;
using Microsoft.Web.Administration;
using System.Configuration;

namespace APIMyViettel
{

    public class Utils
    {

        //public static HttpClient _client;
        //public static readonly HttpClient Client = new HttpClient(new HttpClientHandler() { UseCookies = false });
        //public static 
        //public static readonly HttpClient Client = new HttpClient(new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy("us.smartproxy.io:" + rnd.Next(20000, 29999)) });
        private static string ProxySource = ConfigurationManager.AppSettings["Proxy_Source"] ?? "luminati.io";
        private static string ProxyServer = ConfigurationManager.AppSettings["Proxy_Server"] ?? "zproxy.lum-superproxy.io:22225";
        private static string ProxyUserName = ConfigurationManager.AppSettings["Proxy_User_Name"] ?? "lum-customer-hl_37347aa4-zone-datacenter-country-vn";
        private static string ProxyPass = ConfigurationManager.AppSettings["Proxy_Pass"] ?? "wud8xp4slx75";
        private static string ProxyListContry = ConfigurationManager.AppSettings["Proxy_List_Contry"] ?? string.Empty;

        private static string CaptchaProvider = ConfigurationManager.AppSettings["Captcha_Provider"] ?? "anti-captcha.com";

        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        //{
        //    get
        //    {

        //        if (_client == null)
        //        {
        //            NLogLogger.Info(new string[] { "MyViettelApp", "HttpClient", "Init" });
        //            var handler = new WebRequestHandler();
        //            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
        //            handler.ServerCertificateValidationCallback = AcceptAllCertifications;
        //            handler.UseCookies = false;

        //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //            _client = new HttpClient(handler);
        //            _client.DefaultRequestHeaders.Clear();
        //            _client.DefaultRequestHeaders.ExpectContinue = false;
        //            //_client.DefaultRequestHeaders.ConnectionClose = true;
        //            _client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");

        //            //_client.DefaultRequestHeaders.Add("User-Agent", "Dalvik/1.6.0");

        //        }
        //        return _client;
        //    }

        //}

        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


        public static string GetImageBase64(string url)//, HttpClient Client)
        {
            //var handler = new WebRequestHandler();
            //handler.UseCookies = false;
            //HttpClient client = new HttpClient(handler);
            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");

            //Client.DefaultRequestHeaders.Clear();
            //Client.DefaultRequestHeaders.Add("user-agent", "curl/7.54.0");
            //Client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");

            //


            for (int i = 0; i < 5; i++)
            {
                //string proxy = Utils.GenProxy();
                //string proxy = "127.0.0.1";
                //HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(proxy) });
                //HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000, UseProxy = true, Proxy = new WebProxy(proxy), ServerCertificateValidationCallback = AcceptAllCertifications });
                //HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000, ServerCertificateValidationCallback = AcceptAllCertifications });

                //HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false });
                var session_id = new Random().Next().ToString();
                var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);

                //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
                //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };
                HttpClientHandler handler;
                switch (ProxySource)
                {
                    case "luminati.io":
                        handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
                        break;
                    case "smartproxy.io":
                        handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };
                        break;
                    default:
                        handler = new HttpClientHandler { UseCookies = false };
                        break;
                }

                var client = new HttpClient(handler);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                client.DefaultRequestHeaders.Clear();
                //client.DefaultRequestHeaders.Connection.Add("Keep-Alive");
                client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
                client.Timeout = TimeSpan.FromSeconds(5);


                NLogLogger.Info(new string[] { "MyViettelApp", "GetImageBase64", session_id, "Try", i.ToString(), url });
                //Log proxy Ip
                //Task.Run(async () => await LogIpProxy(client, url, session_id).ConfigureAwait(false));
                try
                {
                    var uri = new Uri(url);
                    var response = Task.Run(async () => await client.GetByteArrayAsync(uri)).Result;
                    //var response = Task.Run(() => client.GetByteArrayAsync(uri)).Result;
                    var imgBase64 = Convert.ToBase64String(response);
                    client.Dispose();
                    return imgBase64;
                }
                //catch (HttpRequestException e)
                //{
                //    if (e.InnerException != null) NLogLogger.Info(new string[] { "MyViettelApp", "GetImageBase64", "HttpRequestException", e.Message, e.InnerException.Message });
                //}
                //catch (TaskCanceledException e)
                //{
                //    if (e.InnerException != null) NLogLogger.Info(new string[] { "MyViettelApp", "GetImageBase64", "TaskCanceledException", e.Message, e.InnerException.Message });
                //}
                //catch (WebException e)
                //{
                //    if (e.InnerException != null) NLogLogger.Info(new string[] { "MyViettelApp", "GetImageBase64", "WebException", e.Message, e.InnerException.Message });
                //}
                //catch (Exception e)
                //{
                //    if (e.InnerException != null)
                //    {
                //        if (e.InnerException.Message.Contains("Response status code does not indicate success") || e.InnerException.Message.Contains("Value cannot be null"))
                //        {
                //            var chat_id = -318818065;
                //            Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, "Captcha gặp lỗi: 400 Bad Request")).Wait();

                //            //ServerManager serverManager = new ServerManager();
                //            //ApplicationPool appPool = serverManager.ApplicationPools["APIMyViettel"];
                //            //if (appPool != null)
                //            //{
                //            //    if (appPool.State == ObjectState.Started)
                //            //    {
                //            //        appPool.Recycle();
                //            //    }
                //            //}
                //            //serverManager.CommitChanges();
                //        }
                //        NLogLogger.Info(new string[] { "MyViettelApp", "GetImageBase64", "Exception", e.Message, e.InnerException.Message });
                //    }
                //}
                catch (Exception ex)
                {

                    NLogLogger.Info(new string[] { "MyViettelApp", "GetImageBase64", credentials.UserName, "Exception", url, ex.Message, ex.StackTrace });

                    //ServerManager serverManager = new ServerManager();
                    //ApplicationPool appPool = serverManager.ApplicationPools["APIMyViettel"];
                    //if (appPool != null)
                    //{
                    //    if (appPool.State == ObjectState.Started)
                    //    {
                    //        appPool.Recycle();
                    //        serverManager.CommitChanges();
                    //    }
                    //}
                }
                client.Dispose();
                Thread.Sleep(1000);
            }

            return null;
        }

        public static Captcha DeCaptcha(string url, string sessionId)//, HttpClient Client)
        {
            try
            {

                var capImageBase64 = GetImageBase64(url);//, Client);
                if (capImageBase64 != null)
                {
                    //NLogLogger.Info(new string[] { "MyViettelApp", "DeCaptcha", "OK" });

                    var handler = CaptchaFactory.GetHandler(CaptchaProvider);
                    var captcha = handler.ImageToText(capImageBase64, 1);
                    //var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64);
                    //var captcha = new Lib.Captcha.CaptchaComVn.CaptchaComVnService().ImageToText(capImageBase64, 1);
                    if (captcha != null)
                    {
                        var cap = captcha.Split('|');
                        var captchaSession = new Captcha()
                        {
                            SessionId = sessionId,
                            Value = cap[0],
                            TaskId = Convert.ToInt32(cap[1]),
                            ImgBase64 = capImageBase64
                        };
                        //captchaSession.Add();
                        return captchaSession;
                    }
                }

            }
            //catch (HttpRequestException e)
            //{
            //    if (e.InnerException != null)
            //        NLogLogger.Info(new string[]
            //            {"MyViettelApp", "DeCaptcha", "HttpRequestException", e.Message, e.InnerException.Message});
            //    return null;
            //}
            //catch (TaskCanceledException e)
            //{
            //    if (e.InnerException != null)
            //        NLogLogger.Info(new string[]
            //            {"MyViettelApp", "DeCaptcha", "TaskCanceledException", e.Message, e.InnerException.Message});
            //    return null;
            //}
            //catch (WebException e)
            //{
            //    if (e.InnerException != null)
            //        NLogLogger.Info(new string[]
            //            {"MyViettelApp", "DeCaptcha", "WebException", e.Message, e.InnerException.Message});
            //    return null;
            //}
            //catch (Exception e)
            //{
            //    if (e.InnerException != null)
            //        NLogLogger.Info(new string[]
            //            {"MyViettelApp", "DeCaptcha", "Exception", e.Message, e.InnerException.Message});
            //    return null;
            //}
            catch (AggregateException e)
            {
                foreach (var errInner in e.InnerExceptions)
                {
                    NLogLogger.Info(new string[]
                        {"MyViettelApp", "DeCaptcha", "AggregateException", errInner.Message});
                }
            }

            return null;

        }


        public static async Task<string> PostTask(string url, Dictionary<string, string> postData)//, HttpClient Client)
        {

            //var handler = new WebRequestHandler();
            //handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            //handler.ServerCertificateValidationCallback = AcceptAllCertifications;
            //handler.UseCookies = false;
            //var client = new HttpClient(handler);
            //client.DefaultRequestHeaders.Clear();
            //client.DefaultRequestHeaders.ExpectContinue = false;
            ////_client.DefaultRequestHeaders.ConnectionClose = true;
            //client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.7.0");
            //Client.DefaultRequestHeaders.Add("user-agent", "curl/7.54.0");

            //string[] agentArg = {"okhttp/3.4.1", "okhttp/3.5.0", "okhttp/3.6.0", "okhttp/3.7.0", "okhttp/3.8.0", "okhttp/3.8.1", "okhttp/3.9.0", "okhttp/3.9.1", "okhttp/3.10.0",
            //    "okhttp/3.11.0", "okhttp/3.12.0", "curl/7.54.0" };
            //string[] agentArg = {"okhttp/3.7.0", "curl/7.54.0" };
            //Random random = new Random();
            //string agent = agentArg[random.Next(0, agentArg.Length)];

            //Client.DefaultRequestHeaders.Clear();

            //Client.DefaultRequestHeaders.ConnectionClose = true;
            //Client.DefaultRequestHeaders.Add("User-Agent", "curl/7.54.0");
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            //Client.DefaultRequestHeaders.Connection.Clear();
            //Client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");

            //
            //HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy("jp.smartproxy.io:" + rnd.Next(30001, 39999)) });
            //client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");

            //Thread.Sleep(1000);

            //string proxy = Utils.GenProxy();
            //HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(proxy) });
            //HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, UseProxy = true, ReadWriteTimeout = 60000, Proxy = new WebProxy(proxy), ServerCertificateValidationCallback = AcceptAllCertifications });

            var session_id = new Random().Next().ToString();
            NetworkCredential credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);
            //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
            //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };

            HttpClientHandler handler;
            switch (ProxySource)
            {
                case "luminati.io":
                    handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
                    break;
                case "smartproxy.io":                    
                    handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy(), false, new string[] { }, credentials) };
                    break;
                default:
                    handler = new HttpClientHandler { UseCookies = false };
                    break;
            }

            ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Clear();
            //client.DefaultRequestHeaders.Connection.Add("Keep-Alive");
            client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
            client.Timeout = TimeSpan.FromSeconds(60);

            NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", session_id, url, serializer.Serialize(postData) });

            var uri = new Uri(url);
            var httpContent = new FormUrlEncodedContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            //Log proxy Ip
            //Task.Run(async () => await LogIpProxy(client, url, session_id).ConfigureAwait(false));
            try
            {
                //var response = client.PostAsync(uri, httpContent).Result;
                var response = await client.PostAsync(uri, httpContent);

                if (response.Content != null)
                {
                    //var responseContent = response.Content.ReadAsStringAsync().Result;
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //if (responseContent.Contains("400 Bad Request"))
                    //{
                    //    var chat_id = -318818065;
                    //    Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, "PostTask gặp lỗi: 400 Bad Request")).Wait();

                    //    //ServerManager serverManager = new ServerManager();
                    //    //ApplicationPool appPool = serverManager.ApplicationPools["APIMyViettel"];
                    //    //if (appPool != null)
                    //    //{
                    //    //    if (appPool.State == ObjectState.Started)
                    //    //    {
                    //    //        appPool.Recycle();
                    //    //    }
                    //    //}
                    //    //serverManager.CommitChanges();
                    //}
                    client.Dispose();
                    return responseContent;
                }

            }
            //catch (HttpRequestException e)
            //{
            //    NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", "HttpRequestException", serializer.Serialize(postData), e.Message });
            //    //ServerManager serverManager = new ServerManager();
            //    //ApplicationPool appPool = serverManager.ApplicationPools["APIMyViettel"];
            //    //if (appPool != null)
            //    //{
            //    //    if (appPool.State == ObjectState.Started)
            //    //    {
            //    //        appPool.Recycle();
            //    //        serverManager.CommitChanges();
            //    //    }
            //    //}
            //}

            //catch (TimeoutException e)
            //{
            //    NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", "TimeoutException", serializer.Serialize(postData), e.Message });
            //}

            //catch (TaskCanceledException e)
            //{
            //    NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", "TaskCanceledException", serializer.Serialize(postData), e.Message });
            //}

            //catch (WebException e)
            //{
            //    NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", "WebException", serializer.Serialize(postData), e.Message });
            //}

            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", credentials.UserName, "Exception", serializer.Serialize(postData), ex.Message, ex.StackTrace });
            }

            //catch (AggregateException e)
            //{
            //    foreach (var errInner in e.InnerExceptions)
            //    {
            //        NLogLogger.Info(new string[]
            //            {"MyViettelApp", "PostTask", "AggregateException", errInner.Message, errInner.StackTrace});
            //    }
            //}
            client.Dispose();
            return string.Empty;

        }


        //public static async Task<string> PostTaskWeb(string url, Dictionary<string, string> postData)
        //{

        //    // string proxy = Utils.GenProxy();
        //    NLogLogger.Info(new string[] { "MyViettelApp", proxy, "PostTask", url, serializer.Serialize(postData) });

        //    var postDataForm = string.Format("{0}", string.Join("&", postData.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))));

        //    try
        //    {
        //        var request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Proxy = new WebProxy(proxy, false);
        //        request.ContentType = "application/x-www-form-urlencoded";
        //        request.Method = "POST"; //GET
        //        request.UserAgent = "okhttp/3.4.1";
        //        request.Timeout = 10000;
        //        System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
        //        byte[] postDatabytes = Encoding.UTF8.GetBytes(postDataForm);
        //        request.ContentLength = postDatabytes.Length;
        //        Stream dataStream = request.GetRequestStream();
        //        dataStream.Write(postDatabytes, 0, postDatabytes.Length);

        //        var webResponse = request.GetResponse();
        //        dataStream = webResponse.GetResponseStream();
        //        if (dataStream == null)
        //        {
        //            webResponse.Close();
        //            return string.Empty;
        //        }

        //        var sr = new StreamReader(dataStream);
        //        var response = sr.ReadToEnd().Trim();

        //        sr.Close();
        //        dataStream.Close();
        //        webResponse.Close();

        //        return response;

        //    }
        //    catch (HttpRequestException e)
        //    {
        //        NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", "HttpRequestException", proxy, serializer.Serialize(postData), e.Message });
        //    }

        //    catch (TimeoutException e)
        //    {
        //        NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", "TimeoutException", proxy, serializer.Serialize(postData), e.Message });
        //    }

        //    catch (TaskCanceledException e)
        //    {
        //        NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", "TaskCanceledException", proxy, serializer.Serialize(postData), e.Message });
        //    }

        //    catch (WebException e)
        //    {
        //        NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", "WebException", proxy, serializer.Serialize(postData), e.Message });
        //    }

        //    catch (Exception e)
        //    {
        //        NLogLogger.Info(new string[] { "MyViettelApp", "PostTask", "Exception", proxy, serializer.Serialize(postData), e.Message });
        //    }

        //    return string.Empty;

        //}

        public static async Task<string> GetTask(string url, bool isProxy = true)//, HttpClient Client)
        {


            var uri = new Uri(url);

            //string proxy = Utils.GenProxy();
            //HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false, UseProxy = true, Proxy = new WebProxy(proxy) });
            //HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, UseProxy = true, ReadWriteTimeout = 60000, Proxy = new WebProxy(proxy), ServerCertificateValidationCallback = AcceptAllCertifications });
            //string proxy = "127.0.0.1";
            //HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false });

            var session_id = new Random().Next().ToString();
            var credentials = new NetworkCredential(GenUserProxy(ProxyListContry) + "-session-" + session_id, ProxyPass);

            //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
            //var handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };
            HttpClientHandler handler;

            if (isProxy)
            {
                switch (ProxySource)
                {
                    case "luminati.io":
                        handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(ProxyServer, false, new string[] { }, credentials) };
                        break;
                    case "smartproxy.io":
                        handler = new HttpClientHandler { UseCookies = false, UseProxy = true, Proxy = new WebProxy(GenProxy()) };
                        break;
                    default:
                        handler = new HttpClientHandler { UseCookies = false };
                        break;
                }
            }
            else
            {
                handler = new HttpClientHandler { UseCookies = false };
            }

            var client = new HttpClient(handler);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            client.DefaultRequestHeaders.Clear();
            //client.DefaultRequestHeaders.Connection.Add("Keep-Alive");
            client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
            client.Timeout = TimeSpan.FromSeconds(60);

            NLogLogger.Info(new string[] { "MyViettelApp", "GetTask", session_id, url });
            //Log proxy Ip
            //Task.Run(async () => await LogIpProxy(client, url, session_id).ConfigureAwait(false));

            try
            {
                var response = await client.GetAsync(uri);
                //var response = client.GetAsync(uri).Result;

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //var responseContent = response.Content.ReadAsStringAsync().Result;
                    client.Dispose();
                    return responseContent;
                }

                //var response = await Client.GetAsync(uri);
                //if (response.Content != null)
                //{
                //    var responseContent = await response.Content.ReadAsStringAsync();

                //    return responseContent;
                //}

            }
            //catch (HttpRequestException e)
            //{
            //    if (e.InnerException != null)
            //    {
            //        NLogLogger.Info(new string[] {"MyViettelApp", "GetTask", "HttpRequestException", e.Message, e.InnerException.Message});
            //        if (e.InnerException.Message.Contains("(502) Bad Gateway"))
            //        {

            //            var chat_id = -318818065;
            //            Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, "GetTask gặp lỗi: 502 Bad Gateway")).Wait();

            //            //ServerManager serverManager = new ServerManager();
            //            //ApplicationPool appPool = serverManager.ApplicationPools["APIMyViettel"];
            //            //if (appPool != null)
            //            //{
            //            //    if (appPool.State == ObjectState.Started)
            //            //    {
            //            //        appPool.Recycle();
            //            //    }
            //            //}
            //            //serverManager.CommitChanges();
            //        }
            //    }

            //    return string.Empty;
            //}
            //catch (TaskCanceledException e)
            //{
            //    if (e.InnerException != null)
            //        NLogLogger.Info(new string[]
            //            {"MyViettelApp", "GetTask", "TaskCanceledException", e.Message, e.InnerException.Message});
            //    return string.Empty;
            //}
            //catch (WebException e)
            //{
            //    if (e.InnerException != null)
            //        NLogLogger.Info(new string[]
            //            {"MyViettelApp", "GetTask", "WebException", e.Message, e.InnerException.Message});
            //    return string.Empty;
            //}
            //catch (Exception e)
            //{
            //    if (e.InnerException != null)
            //        NLogLogger.Info(new string[]
            //            {"MyViettelApp", "GetTask", "Exception", e.Message, e.InnerException.Message});
            //    return string.Empty;
            //}
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "GetTask", credentials.UserName, "Exception", url, ex.Message, ex.StackTrace });
            }
            client.Dispose();
            return string.Empty;


        }

        public static string SetConfigCache(string key, string value)
        {
            string KeyCache = string.Format("{0}:{1}", "config", key);
            try
            {

                var result = DataCaching.SetCache(KeyCache, value);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Pay", "SetConfigCache", "Exception", e.Message });
                return null;
            }
        }

        public static string GetConfigCache(string key)
        {
            string KeyCache = string.Format("{0}:{1}", "config", key);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                return result;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Pay", "GetConfigCache", "Exception", KeyCache, e.Message });
                return "false";
            }
        }

        public static string SetCardSerialCache(string cardSerial, string value)
        {
            string KeyCache = string.Format("{0}:{1}", "serial", cardSerial);
            try
            {

                var result = DataCaching.SetCache(KeyCache, value);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "SetCardSerialCache", "Exception", e.Message });
                return null;
            }
        }

        public static string GetCardSerialCache(string cardSerial)
        {
            string KeyCache = string.Format("{0}:{1}", "serial", cardSerial);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                return result;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "GetCardSerialCache", "Exception", KeyCache, e.Message });
                return null;
            }
        }

        public static string GetTokenCache(string userName)
        {
            string KeyCache = string.Format("{0}:{1}", "myvtt", userName);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                return result;
            }

            catch (ThreadAbortException exp)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "Login", "ThreadAbortException", KeyCache, exp.Message });
                Thread.ResetAbort();
                return null;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "GetTokenCache", "Exception", KeyCache, e.Message });
                return null;
            }
        }

        public static string SetTokenCache(string userName, string token)
        {
            string KeyCache = string.Format("{0}:{1}", "myvtt", userName);
            try
            {

                var result = DataCaching.SetCache(KeyCache, token, 2592000);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "SetTokenCache", "Exception", e.Message });
                return null;
            }
        }

        public static string GetRecycleStatus(string poolName)
        {
            string KeyCache = string.Format("{0}:{1}", "pool", poolName);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                if (string.IsNullOrEmpty(result)) result = "0";
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "GetTokenCache", "Exception", e.Message });
                return null;
            }
        }

        public static string SetRecycleStatus(string poolName, string value)
        {
            string KeyCache = string.Format("{0}:{1}", "pool", poolName);
            try
            {

                var result = DataCaching.SetCache(KeyCache, value);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "SetTokenCache", "Exception", e.Message });
                return null;
            }
        }

        public static void RemoveTokenCache(string userName)
        {
            string KeyCache = string.Format("{0}:{1}", "myvtt", userName);
            try
            {
                DataCaching.RemoveCache(KeyCache);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "SetTokenCache", "Exception", e.Message });
            }
        }

        public static string GenSid()
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i < 27; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return tmp.ToLower(); //+ timeSpan.ToString();
        }

        public static string GenDeviceId()
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i < 17; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return tmp.ToLower(); //+ timeSpan.ToString();
        }

        public static string GenDeviceName()
        {
            string[] pp = ("1,2,3,4,5,6,7,8,9").Split(',');
            Random rd = new Random();
            var name = "" + pp[rd.Next(0, pp.Length)];
            return name;
        }

        public static async Task<string> PostTaskXml(string url, string postData)
        {
            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/xml");
                var handler = new WebRequestHandler();
                var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                handler.ServerCertificateValidationCallback = AcceptAllCertifications;
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                var response = await client.PostAsync(url, httpContent);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "CheckSerial", "PostTaskXml", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }

        public static string GenProxy()
        {
            string[] pp = ("us.smartproxy.com,jp.smartproxy.com,my.smartproxy.com,th.smartproxy.com,kr.smartproxy.com,ph.smartproxy.com,vn.smartproxy.com,sg.smartproxy.com").Split(',');

            //string[] pp = ("vn.smartproxy.com").Split(',');
            Random rd = new Random();
            var proxySr = pp[rd.Next(0, pp.Length)];
            var proxy = proxySr + ":";

            switch (proxySr)
            {
                case "vn.smartproxy.com":
                    proxy = proxy + rd.Next(46001, 46999 + 1);
                    break;
                case "us.smartproxy.com":
                    proxy = proxy + rd.Next(20000, 29999 + 1);
                    break;
                case "kr.smartproxy.com":
                case "sg.smartproxy.com":
                    proxy = proxy + rd.Next(10001, 19999 + 1);
                    break;
                case "jp.smartproxy.com":
                case "my.smartproxy.com":
                case "th.smartproxy.com":
                case "ph.smartproxy.com":
                    proxy = proxy + rd.Next(30001, 39999 + 1);
                    break;
            }
            return proxy;

            //return "vn.smartproxy.com:46000";

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

        //public static bool RecyclePool(string poolName)
        //{
        //    try
        //    {
        //        var serverManager = new ServerManager();
        //        var appPool = serverManager.ApplicationPools[poolName];
        //        //Restart Pool
        //        if (appPool != null && GetRecycleStatus(poolName) == "0")
        //        {
        //            if (appPool.State == ObjectState.Started)
        //            {
        //                appPool.Recycle();
        //                serverManager.CommitChanges();
        //                SetRecycleStatus(poolName, "1");
        //                TelegramNotify.SendNotify("System", string.Empty, poolName, 0, 3);
        //                return true;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        NLogLogger.Info(new string[] { "MyViettelApp", "RecyclePool", "Exception", e.Message });
        //    }
        //    return false;
        //}

        private static async Task<string> LogIpProxy(HttpClient client, string desUrl, string sessionId)//, HttpClient Client)
        {
            string url = "http://lumtest.com/myip.json";
            var uri = new Uri(url);
            NLogLogger.Info(new string[] { "MyViettelApp", "Proxy Request", url });
            try
            {
                var response = await client.GetAsync(uri).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "MyViettelApp", "Proxy Response", responseContent, desUrl, sessionId });
                    client.Dispose();
                    return responseContent;
                }
            }

            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "MyViettelApp", "Proxy Exception", url, ex.Message, ex.StackTrace });
            }
            client.Dispose();
            return string.Empty;


        }
    }
}
