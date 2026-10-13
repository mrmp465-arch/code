using APIMyVNTP;
using Lib.Captcha;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Script.Serialization;
using System.Web;
using Libs.API;
using Libs.Utils;

namespace UnitTest
{
    [TestClass]
    public class MyVNTPTest
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod]
        public void PostAppForm()
        {
            try
            {
                string uri = "https://api-myvnpt.vnpt.vn/mapi/services/mobile_payment_recharge";
                string parameters = "{\"card_id\":\"07869042740939\",\"msisdn\":\"0012594529111\"}";
                var res = Task.Run(() => CallbackJson(uri, parameters));
                //var res = CallbackJson(uri, parameters).Result;
                res.Wait();
                Console.WriteLine(res.Result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        public async Task<string> CallbackJson(string url, string postData)
        {
            NLogLogger.Info(new string[] { "TopupCallbackAppVTT", "Callback Partner", "Request", url, postData });

            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(url, httpContent);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupCallbackAppVTT", "Callback Partner", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
        public static string PostForm(string url, string parameters, string cookie)
        {

            var uri = new Uri(url);
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.Method = "POST";
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(new Cookie("ASP.NET_SessionId", cookie, "/", uri.Host));
            //req.CookieContainer.Add(new Cookie("TS012b5e7b", "014743ac897e51c60c57a0fb3f2128a55b81b75e77419aee9e1ab1a275d81d22dc8b93b1e2f93283f6c0aa353a9d4e0971e1d1c2c938d3d7ed1fee75135d78089e1ed051c2", "/", uri.Host));
            //req.Timeout = 30000;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();

        }

        [TestMethod()]
        public void Topup()
        {
            var mobile = "0912440644";
            var pin = "18767163678480";
            var session = new DeCaptchaWebVina().DeCaptcha();
            string uri = "http://naptien.vinaphone.com.vn/Home/AddPrepaid";
            string parameters = string.Format("PhoneNum={0}&MaThe={1}&Answer={2}", mobile, pin, session.Value);
            var res = PostForm(uri, parameters, session.SessionId);
            Console.WriteLine(res);


            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //Console.WriteLine(serializer.Serialize(new DeCaptchaWebVina().DeCaptcha()));
        }

        [TestMethod()]
        public void MessageTest()
        {

            Console.WriteLine(serializer.Serialize(new APIResponse(ConvertResponCode("Mã thẻ 91577693193314 không tồn tại"))));
        }


        public int ConvertResponCode(string message)
        {

            if (message.Contains("Mã thẻ") && message.Contains("không tồn tại"))
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", message });
                return (int)ResponseCode.CardCodeInvalid;
            }
            else if (message.Contains("đã được sử dụng"))
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", message });
                return (int)ResponseCode.CardUsed;
            }

            else if (message.Contains("Thuê bao không tồn tại") || message.Contains("Không khởi tạo được tài khoản Ezpay"))
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", message });
                return (int)ResponseCode.ServiceIsLocked;
            }

            else
            {
                NLogLogger.Info(new string[] { "TopupAppVNTP", "LogMessage", "Failded" });
                return (int)ResponseCode.TransactionFailed;
            }
        }

        public class CardResponse
        {
            public string error_code { get; set; }
            public string message { get; set; }
            public string result { get; set; }
            public string debug { get; set; }
        }

        [TestMethod()]
        public void MessageResultTest()
        {
            var res =
                @"{""error_code"":""1"",""message"":""Nạp thẻ không thành công, Mã thẻ 49441873166410"",""debug"":""\u003c?xml version\u003d\""1.0\"" ?\u003e\u003cS:Envelope xmlns:S\u003d\""http://schemas.xmlsoap.org/soap/envelope/\""\u003e\u003cS:Body\u003e\u003cns2:rechargeCardEZPayResponse xmlns:ns2\u003d\""http://outerservice.ezpay.elcom.com/\""\u003e\u003cntResponse\u003e\u003cerrorcode\u003e0\u003c/errorcode\u003e\u003cmsisdn\u003e84912426271\u003c/msisdn\u003e\u003ccard_balance\u003e0.0\u003c/card_balance\u003e\u003c/ntResponse\u003e\u003c/ns2:rechargeCardEZPayResponse\u003e\u003c/S:Body\u003e\u003c/S:Envelope\u003e\n""}";
            var resObj = serializer.Deserialize<CardResponse>(res);
            if (!string.IsNullOrEmpty(resObj.result))
                Console.WriteLine(serializer.Serialize(resObj));
        }

        [TestMethod()]
        public void GetTopupTest()
        {
            //var resule = MyVNTPWebService.GetTopup("84859452910", "quocviet0907");
            var resule = MyVNTPWebService.GetTopup("84912440644", "trungdt0106");
            Console.WriteLine(resule.IsTopup);

        }

        [TestMethod()]
        public void TopupCardTest()
        {
            var result = VNTPWebService.TopupCard(1, "59000005344766", "28909094927800", "84912440644", "vnp", "84912440644", "trungdt0106", 2);
            //var result = MyVNTPWebService.TopupCard("59000005320675", "03870222707975", "84912440644", "vnp", "84912440644", "trungdt0106");
            //var result = MyVNTPWebService.TopupCard("59000005310323", "31677818490069", "84859452910", "vnp", "84859452910", "quocviet0907");

            //var result = VNTPWebService.TopupCard("59000005344766", "28909094927800", "84859452910", "vnp", "84859452910", "quocviet0907", 1);
            Console.WriteLine(serializer.Serialize(result));

        }

        [TestMethod()]
        public void TopupCardV2Test()
        {
            //var result = TopupAppVNP.TopupCardV2("84219418212550", "0912440644");
            var result = TopupAppVNP.TopupCardV2(1, "59000010626988", "80858600262368", "0912440644");
            //var result = TopupAppVNP.TopupCardV2(1, "59000010626988", "38939175102253", "0912440644", "0912440644", "trungdt0106");
            Console.WriteLine(serializer.Serialize(result));
        }
    }

}

