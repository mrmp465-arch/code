using APIMobiNext.GSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using APIMobiNext;
using APIMobiNext.Entity;
using APIMobiNext.Sercurity;
using APIMyViettel.Service;
using Libs.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTest
{
    [TestClass]
    public class MobiNextTest
    {
        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static async Task<string> GeTask(string url, string token)
        {
            try
            {
                var handler = new WebRequestHandler();
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.4.1");
                client.DefaultRequestHeaders.Add("token", token);
                System.Net.ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                handler.ServerCertificateValidationCallback = AcceptAllCertifications;
                var response = await client.GetAsync(url);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }

            }
            catch (HttpRequestException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"TopupMobiNext", "PostTask", "HttpRequestException", e.Message, e.InnerException.Message});
                return string.Empty;
            }
            catch (TaskCanceledException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"TopupMobiNext", "PostTask", "TaskCanceledException", e.Message, e.InnerException.Message});
                return string.Empty;
            }
            catch (WebException e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"TopupMobiNext", "PostTask", "WebException", e.Message, e.InnerException.Message});
                return string.Empty;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    NLogLogger.Info(new string[]
                        {"TopupMobiNext", "PostTask", "Exception", e.Message, e.InnerException.Message});
                return string.Empty;
            }


            return string.Empty;

        }

        public static bool AcceptAllCertifications(object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certification,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        [TestMethod()]
        public void TopuCardTest()
        {
            //var url = string.Format("https://next.mobifone.vn/SmartTopupApi2/webresources/topup/manualTopup?phoneNumber={0}&pin={1}&serial=&promoCode=&valueCaptcha={2}","789016400", "596484213594", "PI77vH");
            var url = "https://next.mobifone.vn/SmartTopupApi2/webresources/topup/manualTopup?phoneNumber=0904559259&pin=671968159668&serial=&promoCode=&valueCaptcha=";
            //var res = Task.Run(() => GeTask(url)).Result;
            var res = Task.Run(() => GeTask(url, "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiJjYmUyOTUzYjIxMTQxMzkzIiwiaWF0IjoxNTM5OTc0NjQ1LCJzdWIiOiIiLCJpc3MiOiIwOTA0NTU5MjU5In0.-3w4bMZLnuGEykUxFZo7IBW1zywX1ioPtECzRgfLW88"));
            res.Wait();
            Console.WriteLine(res.Result);
        }

        [TestMethod()]
        public void TopuCardDetailTest()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var res =
                "{\"code\":\"701\",\"message\":\"Card is already used. Please try again with another card.\",\"fields\":\"Card is already used. Please try again with another card.\"}";

            try
            {
                var resCard = JsonConvert.DeserializeObject<TopupResponse>(res);
                Console.WriteLine(resCard.isSuccess.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        [TestMethod()]
        public void SendOTPTest()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string deviceId = Libs.Utils.Encrypts.MD5(DateTime.Now.ToString()).Substring(0, 0x10).ToLower();
            var res = MobiNextService.GetOTP("0904559259", deviceId);
            Console.WriteLine(serializer.Serialize(res));
        }

        [TestMethod()]
        public void GetTokenTest()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string deviceId = Libs.Utils.Encrypts.MD5(DateTime.Now.ToString()).Substring(0, 0x10).ToLower();
            var res = MobiNextService.GetToken("0904559259", "126647", deviceId);
            Console.WriteLine(serializer.Serialize(res));
        }

        [TestMethod]
        public void Param_Test()
        {
            ///Build Key:
            var msisdn = "0936999961";
            var token = "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIzOWEzZWM4ODAxNGU4OGViIiwiaWF0IjoxNTU4NzUwMjc4LCJzdWIiOiIiLCJpc3MiOiIwOTM2OTk5OTYxIn0.juENxC1cWvqj9-0mGDf5jhngnAAty0oaoHsUA50KTDo";
            var tokenIndex = token.ToCharArray();
            StringBuilder key = new StringBuilder();
            int i = 0;
            foreach (var c in msisdn.TrimStart('0'))
            {
                //if (i % 2 == 0 || i == 3 || i == 7)
                if (i % 4 != 0)
                {
                    var j = Convert.ToInt32(c.ToString());
                    key.Append(tokenIndex[j]);
                    //if (i == 3 || i == 7)
                    //    key.Append(tokenIndex[i]);
                }
                i++;
            }

            //i = 0;
            //foreach (var c in msisdn.TrimStart('0'))
            //{
            //   if (i == 3 || i == 7)
            //            key.Append(tokenIndex[i]);
            //   i++;
            //}
            //Console.WriteLine(key);

            int m = 1;
            foreach (var c in msisdn)
            {

                var j = Convert.ToInt32(c.ToString());
                key.Append(tokenIndex[m * j]);
                m++;

            }

            Console.WriteLine(key);

            //var param = string.Format(
            //    "phoneNumber={0}&pin={1}&serial={2}&promoCode={3}&valueCaptcha={4}&appVersion={5}", "936999961","821731671472", "", "","", "5.1");

            //Console.WriteLine(param);

        }

        [TestMethod]
        public void Decrypt_Test()
        {
            var token = "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIzM2U4ZjY0ZjA5MGE3YmVlIiwiaWF0IjoxNTU4ODk0NjI1LCJzdWIiOiIiLCJpc3MiOiIwOTM2MjkyMzI2In0.xKnO7XczGxUpeUekYCzcUIXWMc6S6j9XLbW75uXjp8I";
            var msisdn = "0936292326";
            var key = APIMobiNext.Sercurity.Encrypts.BuildKey(msisdn, token);
            Console.WriteLine(key);

            //var data = "2ZD7S7561poK8ksXrW+7vxymx2S6bYWmdj2zT1TwPMqf/ndsCBsLTN+XHBBveLb49gGPgRCIOguNj60StUkXAzBDOrKoXld2YALygSRVYPiQY/1g8MXru+Tdm1YQWNQAjtKuCXheD3O9OEOl1gsWGJltz7mZyvgUY10TGKj9mnVNMOoUwErwkDkW3650ZOVtoiBA3X0WOwf2SyYzrps19npOVoWx1M2Bm4y4CkvbXVaAgyYJoZkDQPSr5DlyLW0N0EpgcJauEtaZ9BwvL0/QcdeSD/SUSAjOlqQ9HBrl4D6zbnpOILvnrCg20/caPsCW7TaMHmPO9yjmpeBU05siF89x0s2SF8ly8QszM4zhnWyh3S2iDUOr87f1o385Y36rkGhtUXQoB6arsN2PMf+aqK8VJE49DOOuG1uXmWuCMQDHj12CdqqEBfeXtTkdCwL+aNO0x21OVhLfRK/IPrCDYdTiU4hAu04vfp7U01jKI2YnZxdpalktF64yipXxNsCnUlE4o7iDOiC/ML9mvBMBvDFMU+xLYTAIbb+n9KsCOkRIE2vdan6XFOQeIDyOMaxYfcMUCjwvU7bP8OjarNn1rg==";
            //var result = APIMobiNext.Sercurity.Encrypts.Decrypt(data, key);

            //Console.WriteLine(result);

            //var param = string.Format("phoneNumber={0}&pin={1}&serial={2}&promoCode={3}&valueCaptcha={4}&appVersion={5}", "0936999961", "821731671472", "", "", "", "5.1");
            //var result =  APIMobiNext.Sercurity.Encrypts.Encrypt(param, key);
            //Console.WriteLine(result);

        }

        [TestMethod]
        public void APINext_Test()
        {
            var token = "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI4NzMzOWVkNDZkMzk3MDZlIiwiaWF0IjoxNTU5MjgyNjM3LCJzdWIiOiIiLCJpc3MiOiIwOTA0OTYxNDk0In0.SEKilrBHLKn1QOUZxX4GCQ1FavXiTtFlrfnz4zOnOQU";
            var msisdn = "0904961494";
            var serial = "080584000014164";
            var pin = "768014786485";
            var res = new MobiNextBiz().SendWebTopupCardAPI(1, "NEXT01", serial, pin, msisdn, 50000, token);
            Console.WriteLine(res);

            //var data = "2ZD7S7561poK8ksXrW+7vxymx2S6bYWmdj2zT1TwPMqf/ndsCBsLTN+XHBBveLb49gGPgRCIOguNj60StUkXAzBDOrKoXld2YALygSRVYPiQY/1g8MXru+Tdm1YQWNQAjtKuCXheD3O9OEOl1gsWGJltz7mZyvgUY10TGKj9mnVNMOoUwErwkDkW3650ZOVtoiBA3X0WOwf2SyYzrps19npOVoWx1M2Bm4y4CkvbXVaAgyYJoZkDQPSr5DlyLW0N0EpgcJauEtaZ9BwvL0/QcdeSD/SUSAjOlqQ9HBrl4D6zbnpOILvnrCg20/caPsCW7TaMHmPO9yjmpeBU05siF89x0s2SF8ly8QszM4zhnWyh3S2iDUOr87f1o385Y36rkGhtUXQoB6arsN2PMf+aqK8VJE49DOOuG1uXmWuCMQDHj12CdqqEBfeXtTkdCwL+aNO0x21OVhLfRK/IPrCDYdTiU4hAu04vfp7U01jKI2YnZxdpalktF64yipXxNsCnUlE4o7iDOiC/ML9mvBMBvDFMU+xLYTAIbb+n9KsCOkRIE2vdan6XFOQeIDyOMaxYfcMUCjwvU7bP8OjarNn1rg==";
            //var result = APIMobiNext.Sercurity.Encrypts.Decrypt(data, key);

            //var sign = Libs.Utils.Encrypts.MD5(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", "1", "NEXT01", "080584000014164", "768014786485", "50000", "0936292326", "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIzOWEzZWM4ODAxNGU4OGViIiwiaWF0IjoxNTYxNjA2MTI4LCJzdWIiOiIiLCJpc3MiOiIwOTM2MjkyMzI2IiwiZXhwIjoxNTYxNjEzMzI4fQ.jvzWVcKohXj-hC5S6U85MKR65Jnz8pdWUiYeXaObyDI", "2fc427ed4bcc70e1f20861ecaba9cdde"));
            //Console.WriteLine(sign);

            //var param = string.Format("phoneNumber={0}&pin={1}&serial={2}&promoCode={3}&valueCaptcha={4}&appVersion={5}", "0936999961", "821731671472", "", "", "", "5.1");
            //var result =  APIMobiNext.Sercurity.Encrypts.Encrypt(param, key);
            //Console.WriteLine(result);

        }

        [TestMethod]
        public void Unescape_Test()
        {
            //var data = "E7RLRYva+bYRPQPKsS5+IjCtPT9LkZHjjCMzFXG8uXaydEWyrPmB9q/flThMunMdv35zpyDto30ZzhpGulmy5Kf6jx/kT0ZvUo36JUGKe0+s5BKmIVI0IbaNIGJg5fYvteugzsl0xt0+FniV0doBwbq1tO36jsAMV2TAy23RlmPnYFPYl07HHCou8CDzxX8n/9Ze1REGqLF981psrUdi3voWnq3wJ0LJ8FmM735W07/KYGK5JIX4dQw8oU/xt4CZ8OM+q7WRRBoc/Tr+o3tFDTKZOl9mozf+hWdl7gkQcRQofuxlHslPSSbCBKFEDyZYtc3A2Nct1mRR7mV3/ezWaYetBX+7RLxgWO1oH1+SW7TaxXDeaMdC/9iRxb7VL4Q6Sggdog9+9U4Zd/fhGerOHNzGaE18YqQiH0eEymxezW+wh7VIyrw2FYR0KO163dgEJV0tXAnCybLTMllDBGCSUZBsnMSzc8qscje3FmODkIlCaqggceCaz0txcjSBJjYsE8OtMbeRG4juiPjl7YDjE3/CgP8dzBuo84nwQSmnc1Vx+hNPn4cMwLUrX5XN3Ceq3AldYzswXmk+Hiju+dQZQg\u003d\u003d";

            //Console.WriteLine(Regex.Unescape(data));

            var data = "nN0Qyo72GUdMKnpZxyQD/btuj/VMK3UXdH71UnNuLSXdfd1VQIoU2VO5VvB3b/nLujH3l2O9VjjK0N0+2CRSdGi9+nvd2XOQmY4u0MKQfhfpvtVsCMs9qJCc09lyeQEK";
            var key = "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIzOWEzZWM4ODAxNGU4OGViIiwiaWF0IjoxNTYxNjA2MTI4LCJzdWIiOiIiLCJpc3MiOiIwOTM2MjkyMzI2IiwiZXhwIjoxNTYxNjEzMzI4fQ.jvzWVcKohXj-hC5S6U85MKR65Jnz8pdWUiYeXaObyDI";
            var result = APIMobiNext.Sercurity.Encrypts.Decrypt(data, APIMobiNext.Sercurity.Encrypts.BuildKey("0936292326", key));

            Console.WriteLine(result);

            //var param = string.Format("phoneNumber={0}&pin={1}&serial={2}&promoCode={3}&valueCaptcha={4}&appVersion={5}", "0936999961", "821731671472", "", "", "", "5.1");
            //var result =  APIMobiNext.Sercurity.Encrypts.Encrypt(param, key);
            //Console.WriteLine(result);

        }

        [TestMethod()]
        public void SendChardRequestTest()
        {
            var res = GSMServiceBiz.SendChardRequest("1", "vms", "0705249260", "0705249260", "080271000008336", "719583111920", 1);
            Console.Write(serializer.Serialize(res));
        }

        [TestMethod()]
        public void GetListSimGSMStatusTest()
        {
            var res = GSMServiceBiz.GetListSimGSMStatus();
            Console.Write(serializer.Serialize(res));
        }
    }
}
