using APIMyViettel.Service;
using APIMyViettel;
using Lib.Captcha;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Script.Serialization;
using System.Web;
using System.Xml;
using APIMyViettel.Entity;
using Libs.API;
using Libs.TopupPartner.VNPTEPAY;
using Libs.Utils;
using System.Xml.Serialization;

namespace UnitTest
{
    [TestClass]
    public class MyViettelTest
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public class MyClass
        {
            public string username { set; get; }
            public string password { set; get; }
            public string actionForm { set; get; }
            public string device_name { set; get; }
            public string device_id { set; get; }
            public string os_type { set; get; }
            public string os_version { set; get; }
            public string app_version { set; get; }
            public string imei { set; get; }
            public string model { set; get; }
            public string app_id { set; get; }

        }


        [TestMethod]
        public void LoginTest()
        {

            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i < 17; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }

            //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            Console.WriteLine(tmp.ToLower()); //+ timeSpan.ToString();

            //var account = new Account();
            //account.AccountName = "t008_gftth_huongltt170";
            //account.Password = "Yen@123456";

            //string url = "https://apivtp.vietteltelecom.vn:6768/myviettel.php/loginV2";
            //var parameters = new Dictionary<string, string>();
            //parameters.Add("username", account.AccountName);
            //parameters.Add("password", account.Password);
            ////parameters.Add("actionForm", "mob");
            //parameters.Add("actionForm", "adsl");
            //parameters.Add("device_name", "SM-N90");
            //parameters.Add("device_id", "359093054986361");
            //parameters.Add("os_type", "21");
            //parameters.Add("app_version", "100");
            //parameters.Add("imei", "359093054986361");
            //parameters.Add("model", "samsung_SM-N9005");
            //parameters.Add("app_id", "com.vttm.vietteldiscovery");

            //try
            //{
            //    var res = Task.Run(() => Utils.PostTask(url, parameters)).Result;
            //    Console.WriteLine(res);

            //}
            //catch (Exception exp)
            //{
            //    Console.WriteLine(exp.Message);
            //}

        }


        [TestMethod]
        public void Login()
        {
            try
            {
                string ck = string.Empty;
                //string urlCap = "http://naptien.vinaphone.com.vn/Home/GenerateCaptcha";
                //var cap = GetForm(urlCap, ref ck);
                //Console.WriteLine(cap);

                ck = "wvim10nlvi1jkrfxsk4kok5z";

                string url = "https://apivtp.vietteltelecom.vn:6768/myviettel.php/loginV2";
                //string parameters = "username=01645929958&password=1234567b&actionForm=mob&device_name=SM-N9005&device_id=359093054986361&os_type=0&os_version=21&app_version=100&imei=359093054986361&model=samsung_SM-N9005&app_id=com.vttm.vietteldiscovery";
                var parameters = new Dictionary<string, string>();

                parameters.Add("username", "343912585");
                parameters.Add("password", "123456a");
                parameters.Add("actionForm", "mob");
                parameters.Add("device_name", "SM-N90");
                parameters.Add("device_id", "359093054986361");
                parameters.Add("os_type", "21");
                parameters.Add("app_version", "100");
                parameters.Add("imei", "359093054986361");
                parameters.Add("model", "samsung_SM-N9005");
                parameters.Add("app_id", "com.vttm.vietteldiscovery");

                //parameters.Add("username", "347446904");
                //parameters.Add("password", "123456a");
                //parameters.Add("actionForm", "mob");
                //parameters.Add("device_name", "Mi MIX 2");
                //parameters.Add("device_id", "ecbb6a6f275a0a2a");
                //parameters.Add("os_type", "0");
                //parameters.Add("os_version", "26");
                //parameters.Add("app_version", "157");
                //parameters.Add("imei", "ecbb6a6f275a0a2a");
                //parameters.Add("model", "Xiaomi_Mi MIX 2");
                //parameters.Add("app_id", "com.vttm.vietteldiscovery");

                //var t = Task.Run(() => APIMyViettel.Utils.PostTaskWeb(url, parameters));
                //var res = t.Result;
                //Console.WriteLine(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        [TestMethod]
        public CaptchaResponse GetCaptchaLink()
        {
            //string token = "98C57BAD-A1CD-FC4E-1B6E-987534FA0275";
            string url = "https://apivtp.vietteltelecom.vn:6768/myviettel.php/getCaptcha";
            //var res = GetForm(url);
            var res = Task.Run(() => GeTask(url));
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var response = serializer.Deserialize<CaptchaResponse>(res.Result);
            Console.WriteLine(serializer.Serialize(response));
            return response;
        }

        [TestMethod()]
        public void TopupCard()
        {


            var captcha = MyViettelService.GetCaptchaLink();

            string phone = "0366059816";
            string cardCode = "412434581781845";
            string token = "B66FBF37-DBA2-5200-DEC5-FD3265FF269E";

            var decaptcha = Utils.DeCaptcha(captcha.url, captcha.sid);

            string url = "https://apivtp.vietteltelecom.vn:6768/myviettel.php/paymentOnline";
            var parameters = new Dictionary<string, string>();
            parameters.Add("token", token);
            parameters.Add("phone", phone);
            parameters.Add("cardcode", cardCode);
            parameters.Add("type", "1");
            parameters.Add("sid", captcha.sid);
            parameters.Add("captcha", decaptcha.Value);

            var res = Task.Run(async () => await Utils.PostTask(url, parameters)).Result;
            Console.WriteLine(res);

        }

        [TestMethod()]
        public void CheckCard()
        {
            //var ck = "bus806qd0r24mcc4sn9ba9uea3";
            var captchaObj = GetCaptchaLink();

            string cardSerial = "10001234812766";
            string token = "A6D74663-D5F5-FA0A-9C3E-DE788FE3C9FD";
            var captchaurl = captchaObj.data.url;
            var sid = captchaObj.data.sid;
            //var decaptcha = Task.Run(() => GetImageAsBase64Url(captchaurl)).Result;
            //https://apivtp.vietteltelecom.vn:6768/myviettel.php/getcardinfo?device_name=CHC-U01&version_app=3.8&build_code=144&os_type=android
            string url = "https://apivtp.vietteltelecom.vn:6768/myviettel.php/getcardinfo";
            //string parameters = string.Format("token={0}&serial={1}&captcha={2}&sid={3}", token, cardSerial, "a5M3", sid);
            var parameters = new Dictionary<string, string>();
            parameters.Add("token", token);
            parameters.Add("serial", cardSerial);
            parameters.Add("captcha", "Azhy");
            parameters.Add("sid", sid);


            var res = Task.Run(() => PostTask(url, parameters))
                .Result; //{ "errorCode":3,"message":"Truy\u1ec1n thi\u1ebfu tham s\u00f4 captcha"}
            Console.WriteLine(res);

        }

        private static string GetForm(string url)
        {
            Uri uri = new Uri(url);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = new CookieContainer();
            var response = (HttpWebResponse)request.GetResponse();
            Stream data = response.GetResponseStream();
            string html = String.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }

            //CookieCollection ckc = response.Cookies;
            //Cookie ck = ckc["ASP.NET_SessionId"];
            //if (null != ck)
            //{
            //    aspck = ck.Value;
            //}
            return html;

        }

        public static bool AcceptAllCertifications(object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certification,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            //if (sslPolicyErrors == SslPolicyErrors.None)
            //    return true;

            //Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            //NLogLogger.Info(new string[] { "Certificate error: {0}", sslPolicyErrors.ToJSON() });
            //// Do not allow this client to communicate with unauthenticated servers.
            //return false;
            return true;
        }

        public static string PostFormHttp(string url, string parameters)
        {

            var uri = new Uri(url);
            var formVariables = new List<KeyValuePair<string, string>>();
            formVariables.Add(new KeyValuePair<string, string>("id", "ho"));
            var formContent = new FormUrlEncodedContent(formVariables);

            // submit the form
            HttpClient client = new HttpClient();
            var response = client.PostAsync(uri, formContent).Result;
            return response.Content.ToString();
        }

        public static string PostForm(string url, string parameters, string cookie)
        {

            var uri = new Uri(url);
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.ContentType = "application/x-www-form-urlencoded;";
            req.Method = "POST";
            //req.CookieContainer = new CookieContainer();
            //req.CookieContainer.Add(new Cookie("ASP.NET_SessionId", cookie, "/", uri.Host));
            //req.CookieContainer.Add(new Cookie("TS012b5e7b", "014743ac897e51c60c57a0fb3f2128a55b81b75e77419aee9e1ab1a275d81d22dc8b93b1e2f93283f6c0aa353a9d4e0971e1d1c2c938d3d7ed1fee75135d78089e1ed051c2", "/", uri.Host));
            //req.Timeout = 30000;
            //NetworkCredential nc = new NetworkCredential("01643470580", "1234567b");
            //req.Credentials = nc;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();

            var resp = (HttpWebResponse)req.GetResponse();
            CookieCollection ckc = resp.Cookies;
            Cookie ck = ckc["apimyvt_session"];
            //cookie = ck.Value;
            //var resp = req.GetResponse();

            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();

        }

        public async Task<string> PostTask(string url, Dictionary<string, string> postData)
        {
            //try
            //{
            var httpContent = new FormUrlEncodedContent(postData);
            var handler = new WebRequestHandler();
            //var client = new HttpClient(new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Automatic });
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            handler.ServerCertificateValidationCallback = AcceptAllCertifications;
            //httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await client.PostAsync(url, httpContent);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }

            //}
            //catch (Exception e)
            //{
            //    NLogLogger.Info(new string[] { "TopupCallbackAppVTT", "Callback Partner", e.Message });
            //    return string.Empty;
            //}

            return string.Empty;

        }

        public async Task<string> PostTaskXml(string url, string postData)
        {
            //try
            //{
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/xml");
            var handler = new WebRequestHandler();
            //var client = new HttpClient(new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Automatic });
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
            System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            handler.ServerCertificateValidationCallback = AcceptAllCertifications;
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            var response = await client.PostAsync(url, httpContent);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }

            //}
            //catch (Exception e)
            //{
            //    NLogLogger.Info(new string[] { "TopupCallbackAppVTT", "Callback Partner", e.Message });
            //    return string.Empty;
            //}

            return string.Empty;

        }

        public async Task<string> GeTask(string url)
        {
            try
            {
                var handler = new WebRequestHandler();
                handler.ServerCertificateValidationCallback = AcceptAllCertifications;
                //var client = new HttpClient(new HttpClientHandler{ClientCertificateOptions = ClientCertificateOption.Automatic});
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
                System.Net.ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var response = await client.GetAsync(url);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupCallbackAppVTT", "Callback Partner", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }

        public async Task<string> GetImageAsBase64Url(string url)
        {

            try
            {
                var handler = new WebRequestHandler();
                handler.ServerCertificateValidationCallback = AcceptAllCertifications;
                //var client = new HttpClient(new HttpClientHandler{ClientCertificateOptions = ClientCertificateOption.Automatic});
                var client = new HttpClient(handler);
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                //var response = await client.GetAsync(url);
                //if (response.Content != null)
                //{
                var bytes = await client.GetByteArrayAsync(url);
                return "image/jpeg;base64," + Convert.ToBase64String(bytes);
                //}

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupCallbackAppVTT", "Callback Partner", e.Message });
                return string.Empty;
            }

            return string.Empty;

            //var credentials = new NetworkCredential(user, pw);
            //using (var handler = new HttpClientHandler { Credentials = credentials })
            //using (var client = new HttpClient(handler))
            //{
            //    var bytes = await client.GetByteArrayAsync(url);
            //    return "image/jpeg;base64," + Convert.ToBase64String(bytes);
            //}
        }

        public async Task<string> PostCard(string url, Dictionary<string, string> postData)
        {
            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                //var client = new HttpClient(new HttpClientHandler{ClientCertificateOptions = ClientCertificateOption.Automatic});
                var client = new HttpClient();
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var response = await client.PostAsync(url, httpContent);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "SendAppVTTV2Topup", "Callback Partner", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }



        [TestMethod()]
        public void TestHong()
        {

            string url = "";
            var parameters = new Dictionary<string, string>();
            parameters.Add("transid", "19001572");
            parameters.Add("seri", "10001387807875");
            parameters.Add("code", "415807050996267");
            parameters.Add("phone", "h004_gftth_mypnt");
            parameters.Add("type", "0");
            parameters.Add("amount", 200000.ToString());
            parameters.Add("callback", "http://149.28.130.246:1583/SendAppVTTV2Topup.ashx");
            var responseJSON = PostCard("http://192.64.115.34:21080/api/myvt/topup", parameters).Result;
            Console.WriteLine(responseJSON);

        }

        [TestMethod()]
        public void GetImageBase64(string url)
        {
            HttpClient client = new HttpClient();
            var response = client
                .GetByteArrayAsync(
                    "http://apivtp.vietteltelecom.vn/myviettel.php/gen-img-captcha?sid=mhra1q9bcgjh5lvo9otrhs6n53&rand=1290e03517d41159daed449cc0c59af9")
                .Result;
            var imgBase64 = Convert.ToBase64String(response);
            Console.Write(imgBase64);
        }



        [TestMethod()]
        public void GetToken()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            long curtime = (DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks) / 10000;
            var url = "https://171.255.192.120:8115/BCCSGatewayWS/BCCSGatewayWS?wsdl";
            var sb = new StringBuilder();

            //sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            //sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.bccsgw.viettel.com/\">");
            //sb.Append("<soapenv:Header />");
            //sb.Append("<soapenv:Body>");
            //sb.Append("<web:gwOperation>");
            //sb.Append("<Input>");
            //sb.Append("<!--Validate BCCSGateway:-->");
            //sb.Append("<username>1dd54a3a8a09bd77c380604c3d4a06d4</username>");
            //sb.Append("<password>00d1fb1ab0c4d66af7d5f797cbcb600d</password>");
            //sb.Append("<wscode>mbccs_loginBccs2</wscode>");
            //sb.Append("<!--Zero or more repetitions:-->");
            //sb.Append("<rawData><![CDATA[");
            //sb.Append("<ws:login>");
            //sb.Append("<mbccsRequestCode>MBCCS1</mbccsRequestCode>");
            //sb.AppendFormat("<requestId>mbccs_loginBccs2;{0};375105705genClientKey</requestId>", curtime.ToString());
            //sb.Append("<userName>c89cce73f89529309145c9563377d904</userName>");
            //sb.Append("<osType>Android</osType>");
            //sb.Append("<networkType>PUBLIC</networkType>");
            //sb.Append("<userName>c89cce73f89529309145c9563377d904</userName>");
            //sb.Append("<passWord>b594a563bcd54c28ede52753392ea35e</passWord>");
            //sb.Append("<addInfo>14fc22840211ab1ed617be90f58390ef</addInfo>");
            //sb.AppendFormat("<clientTime>{0}</clientTime>", curtime.ToString());
            //sb.Append("<version>3.3.8</version>");
            //sb.Append("<serialSim>49389178951284723342</serialSim><osType>Android</osType><networkType>PUBLIC</networkType>");
            //sb.Append("</ws:login>");
            //sb.Append("]]></rawData>");
            //sb.Append("</Input>");
            //sb.Append("</web:gwOperation>");
            //sb.Append("</soapenv:Body>");
            //sb.Append("</soapenv:Envelope>");
            //sb.Append("");

            //var res = Task.Run(() => PostTaskXml(url, sb.ToString())).Result;
            ////Console.WriteLine(res);
            //var token = Regex.Match(res, @"(?<=token&gt;).*?(?=\&lt;)").Value;
            //Console.WriteLine(token);


            var tokenSave = "0940c69338a1484d9bc6f00fc13cd9c7";

            sb.Clear();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append(
                "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.bccsgw.viettel.com/\">");
            sb.Append("<soapenv:Header />");
            sb.Append("<soapenv:Body>");
            sb.Append("<web:gwOperation>");
            sb.Append("<Input>");
            sb.Append("<!--Validate BCCSGateway:-->");
            sb.Append("<username>1dd54a3a8a09bd77c380604c3d4a06d4</username>");
            sb.Append("<password>00d1fb1ab0c4d66af7d5f797cbcb600d</password>");
            sb.Append("<wscode>mbccs_getInforCardNumber</wscode>");
            sb.Append("<!--Zero or more repetitions:-->");
            sb.Append("<rawData><![CDATA[<ws:getInforCardNumber><mbccsRequestCode>MBCCS1</mbccsRequestCode>");
            sb.AppendFormat("<input><requestId>mbccs_getInforCardNumber;{0};994864615</requestId>", curtime.ToString());
            sb.Append("<userName>c89cce73f89529309145c9563377d904</userName>");
            //sb.Append("<passWord>b594a563bcd54c28ede52753392ea35e</passWord>");
            sb.Append(
                "<osType>Android</osType><vsaMenu>;2Gto3G.mbccs2;</vsaMenu><networkType>PUBLIC</networkType><version>3.3.8</version>");
            sb.AppendFormat(
                "<token>{0}</token><serial>{1}</serial><regType>0</regType></input></ws:getInforCardNumber>]]></rawData>",
                tokenSave, "10000990644556");
            sb.Append("</Input>");
            sb.Append("</web:gwOperation>");
            sb.Append("</soapenv:Body>");
            sb.Append("</soapenv:Envelope>");
            sb.Append("");

            var resCheck = Task.Run(() => PostTaskXml(url, sb.ToString())).Result;

            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(resCheck);
            //XmlNodeList original = xmlDoc.GetElementsByTagName("original");

            //xmlDoc.LoadXml(original[0].InnerText);
            //XmlNodeList errorCode = xmlDoc.GetElementsByTagName("errorCode");

            //Console.WriteLine(errorCode[0].InnerText);


            var errorCode = Regex.Match(resCheck, @"(?<=errorCode&gt;).*?(?=\&lt;)").Value;
            if (errorCode == "0")
            {
                var cardInfor = new CardCheckResponse()
                {
                    cardExpired = Regex.Match(resCheck, @"(?<=cardExpired&gt;).*?(?=\&lt;)").Value,
                    cardValue = Regex.Match(resCheck, @"(?<=cardValue&gt;).*?(?=\&lt;)").Value,
                    isdn = Regex.Match(resCheck, @"(?<=isdn&gt;).*?(?=\&lt;)").Value,
                    ownerName = Regex.Match(resCheck, @"(?<=ownerName&gt;).*?(?=\&lt;)").Value,
                    dateUsed = Regex.Match(resCheck, @"(?<=dateUsed&gt;).*?(?=\&lt;)").Value,
                    responeCode = Regex.Match(resCheck, @"(?<=responeCode&gt;).*?(?=\&lt;)").Value

                };
            }

            //Console.WriteLine(resCheck);

            //var cardInfor = new CardCheckResponse()
            //{
            //    cardExpired = Regex.Match(resCheck, @"(?<=cardExpired&gt;).*?(?=\&lt;)").Value,
            //    cardValue = Convert.ToInt32(Regex.Match(resCheck, @"(?<=cardValue&gt;).*?(?=\&lt;)").Value),
            //    isdn = Regex.Match(resCheck, @"(?<=isdn&gt;).*?(?=\&lt;)").Value,
            //    ownerName = Regex.Match(resCheck, @"(?<=ownerName&gt;).*?(?=\&lt;)").Value,
            //    dateUsed = Regex.Match(resCheck, @"(?<=dateUsed&gt;).*?(?=\&lt;)").Value,
            //    responeCode = Regex.Match(resCheck, @"(?<=responeCode&gt;).*?(?=\&lt;)").Value

            //};


            //Console.WriteLine(serializer.Serialize(cardInfor));
        }

        public class CardCheckResponse
        {

            public string cardValue { set; get; }
            public string cardExpired { set; get; }
            public string isdn { set; get; }
            public string ownerName { set; get; }
            public string dateUsed { set; get; }
            public string responeCode { set; get; }

        }

        [TestMethod()]
        public void CheckCardTest()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var response = CheckSerial.CheckCard("10001354143425");
            Console.WriteLine(serializer.Serialize(response));
        }

        [TestMethod()]
        public void CheckCardTest2()
        {

            var mobile = "1669870364";
            var ResponseContent =
                "{\"cardSerial\":\"10001262827622\",\"cardValue\":\"50000\",\"cardExpired\":\"31/12/2024\",\"isdn\":\"369870364\",\"ownerName\":\"\",\"dateUsed\":\"04/10/2018 00:06:16\"}";

            var responseToup = "{\"errorCode\":\"1\",\"message\":\"Thuê bao cố định không được nạp thẻ hộ cho các thuê bao khác.\",\"data\":null}";

            try
            {
                //var cardDetail = serializer.Deserialize<CheckSerialResponse>(ResponseContent);
                //if (cardDetail.isdn.Substring(cardDetail.isdn.Length - 7) == mobile.Substring(mobile.Length - 7)
                //    && (DateTime.Now - DateTime.ParseExact(cardDetail.dateUsed, "dd/MM/yyyy HH:mm:ss",
                //            CultureInfo.InvariantCulture)).TotalSeconds < 2)
                //{
                //    Console.WriteLine((DateTime.Now - Convert.ToDateTime(cardDetail.dateUsed)).TotalSeconds);
                //    Console.WriteLine(cardDetail.cardValue);
                //}
                //else
                //{
                //    Console.WriteLine(DateTime.Now - DateTime.ParseExact(cardDetail.dateUsed, "dd/MM/yyyy HH:mm:ss",
                //                          CultureInfo.InvariantCulture));
                //    Console.WriteLine("Failed");
                //}

                var cardDetail = serializer.Deserialize<TopupResponse>(responseToup);
                Console.WriteLine(cardDetail.errorCode);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }



        }

        [TestMethod()]
        public void TopupCardTest()
        {

            DateTime lastTime = DateTime.ParseExact("6/19/2019 1:55:06 PM", "M/d/yyyy h:mm:ss tt",
                CultureInfo.InvariantCulture);
            DateTime dateUsed =
                DateTime.ParseExact("19/06/2019 14:01:12", "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
            var totalsec = (dateUsed - lastTime).TotalSeconds;
            Console.Write(serializer.Serialize(totalsec));

            //var result = MyViettelService.TopupCard("10003172808703", "018898192780181", "0986273273", 1);
            //Console.Write(serializer.Serialize(result));
        }

        [TestMethod()]
        public void AmoutTest()
        {
            var a = Regex
                .Match(
                    "Bạn đã nạp thẻ thành công. Hệ thống đang xử lý yêu cầu. Vui lòng giữ thẻ chờ tin nhắn thông báo thanh toán thành công về máy điện thoại của Quý khách. Xin cảm ơn!",
                    @"\d+").Value;
            int amountresponse;
            int.TryParse(a, out amountresponse);
            Console.Write(amountresponse);
        }

        [TestMethod()]
        public void ChangePassTest()
        {
            var token = "286F8062-177C-545C-D04A-7AFF6AC827A3";
            //var token = "FC812DF0-92D5-81D3-5B1B-E6F15B93EB8B";
            var accountName = "865162686";
            var oldPass = "123456a";
            var newPass = "a123456a";
            //var newPass = "123456a";
            var change = MyViettelService.ChangePass(accountName, oldPass, newPass, token);
            Console.WriteLine(change);

        }

        [TestMethod()]
        public void Logout()
        {
            var token = "286F8062-177C-545C-D04A-7AFF6AC827A3";
            var accountName = "84342724919";
            var Logout = MyViettelService.Logout(accountName, token);
            Console.WriteLine(Logout);

        }

        [TestMethod()]
        public void LoginTest1()
        {
            var account = new Account()
            {
                AccountName = "865162686",
                Password = "a123456a"
            };

            var login = MyViettelService.Login(0, ref account);
            Console.WriteLine(serializer.Serialize(login));
        }

        [TestMethod]
        public void Time()
        {
            //Console.WriteLine(DateTime.Now.ToString("o"));

            //var jsonPost = string.Format("{{orderid:{0},status={1},amount={2}}}", 1111, 1, 1000);
            //Console.WriteLine(jsonPost);
            var cardResult = "{\"cardSerial\":\"10004088112070\",\"cardValue\":\"50000\",\"cardExpired\":\"31/12/2025\",\"isdn\":\"98397xxxx\",\"ownerName\":\"\",\"dateUsed\":\"07/10/2019 17:19:59\"}";
            var card = serializer.Deserialize<CheckSerialResponse>(cardResult);
            DateTime timeCheck = DateTime.Now;
            DateTime dateUsed = DateTime.ParseExact(card.dateUsed, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
            var totalsec = (timeCheck - dateUsed).TotalSeconds;
            Console.WriteLine(totalsec);
        }
    }

}


