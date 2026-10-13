using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Libs.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class CardTelcoTest
    {

        [TestMethod]
        public void usecardWS()
        {
            //string urlService = "http://149.28.130.246:1581/VPGService.asmx";
            string urlService = "https://apicard.bb2d.com/VPGService.asmx";
            //string partnerKey = "31629fbc7f89d34771319c544a55e839";
            //string partnerCode = "big";
            string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
            string partnerCode = "pp";
            string serviceCode = "cardtelco";
            string commandCode = "usecard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = "59000004110222",
                CardCode = "58592457412998",
                CardType = "vnp",
                AccountName = "pp.002",
                AppCode = "UnitTest USSD",
                RefCode = "00049",
                AmountUser = 10000
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);

            VPGService _VPGService = new VPGService(urlService);
            string serviceResponse = string.Empty;
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            Console.WriteLine(serviceResponse);
        }

        [TestMethod]
        public void usecardWS_Napho()
        {
            string urlService = "https://apicard.bb2d.com/VPGService.asmx";
            //string urlService = "http://149.28.130.246:1581/VPGService.asmx";
            //string partnerKey = "31629fbc7f89d34771319c544a55e839";
            //string partnerCode = "big";
            string partnerKey = "11347cd92cb2bb1b0674c9a4754fdb3e";
            string partnerCode = "gm3d";
            string serviceCode = "cardtelco";
            string commandCode = "usecard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = "10000145500783",
                CardCode = "512225291775122",
                CardType = "vnp",
                AccountName = "pp.02",
                AppCode = "UnitTest",
                RefCode = "0008",
                AmountUser = 10000
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);

            VPGService _VPGService = new VPGService(urlService);
            string serviceResponse = string.Empty;
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            Console.WriteLine(serviceResponse);
        }

        [TestMethod]
        public void usecardWS_Callback()
        {
            string urlService = "http://149.28.130.246:1581/VPGService.asmx";
            //string partnerKey = "31629fbc7f89d34771319c544a55e839";
            //string partnerCode = "big";
            string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
            string partnerCode = "pp";
            string serviceCode = "cardtelco";
            string commandCode = "usecard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = "10001718534319",
                CardCode = "814804849601405",
                CardType = "viettel",
                AccountName = "pp.001",
                AmountUser = 50000,
                CallbackUrl = "http://149.28.130.246:1586/CardCallback.aspx",
                AppCode = "UnitTest USSD",
                RefCode = "00032"
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);

            VPGService _VPGService = new VPGService(urlService);
            string serviceResponse = string.Empty;
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            Console.WriteLine(serviceResponse);
        }

        [TestMethod]
        public void usecardForm()
        {
            string urlService = "http://149.28.130.246:1581/VPGFormService.ashx";
            string partnerKey = "31629fbc7f89d34771319c544a55e839";
            string partnerCode = "big";
            string serviceCode = "cardtelco";
            string commandCode = "usecard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = "0729010001712",
                CardCode = "916683375105",
                CardType = "vms",
                AccountName = "big.001",
                AppCode = "UnitTest",
                RefCode = "00011"
            });
            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            string parameters = string.Format("PartnerCode={0}&ServiceCode={1}&CommandCode={2}&RequestContent={3}&Signature={4}", partnerCode, serviceCode, commandCode, requestContent, signature);
            var serviceResponse = PostForm(urlService, parameters);
            Console.WriteLine(serviceResponse);
        }

        [TestMethod]
        public void usecardJson()
        {
            string urlService = "https://apicard.atheriz.xyz/VPGJsonService.ashx";
            //string urlService = "http://localhost:8080/VPGJsonService.ashx";
            string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
            string partnerCode = "pp";
            string serviceCode = "cardtelco";
            string commandCode = "usecard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = "CJ02566123",
                CardCode = "9247720888",
                CardType = "gate",
                AccountName = "big.002",
                AppCode = "UnitTest",
                RefCode = "00008",
                AmountUser = 0,
                CallbackUrl = "http://dichvu001.com"
            });
            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,
                CommandCode = commandCode,
                RequestContent = requestContent,
                ServiceCode = serviceCode,
                Signature = signature
            };
            Console.WriteLine(serializer.Serialize(requestData));
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
            Console.WriteLine(serviceResponse);
        }

        [TestMethod]
        public void checktransJson()
        {
            string urlService = "https://apicard.thumuathe.shop/VPGJsonService.ashx";
            string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
            string partnerCode = "pp";
            string serviceCode = "cardtelco";
            string commandCode = "checktrans";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                RefCode = "20190308165556"
            });
            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,
                CommandCode = commandCode,
                RequestContent = requestContent,
                ServiceCode = serviceCode,
                Signature = signature
            };
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
            Console.WriteLine(serviceResponse);
        }

        [TestMethod]
        public void usecard_CallbackTest()
        {
            string urlService = "https://faac.club/apiv2/callback.php?uid=c5a8db48e03588205452c10c5e6b6f407067e838";
            var data = "{\"RefCode\": \"5b967df6213dc\",\"Amount\" : 10000, \"Status\" : 1, \"Signature\": \"mrBill\" }";
            var serviceResponse = CallbackJson(urlService, data).Result;
            Console.WriteLine(serviceResponse);
        }
        public class RequestData
        {
            public string PartnerCode { get; set; }
            public string ServiceCode { get; set; }
            public string CommandCode { get; set; }
            public string RequestContent { get; set; }
            public string Signature { get; set; }

        }

        public class UseCardRequest
        {
            public string CardSerial { get; set; }
            public string CardCode { get; set; }
            public string CardType { get; set; }
            public string AccountName { get; set; }
            public string AppCode { get; set; }
            public string RefCode { get; set; }
            public int Amount { get; set; }
            public int AmountUser { get; set; }
            public string CallbackUrl { get; set; }

        }

        private string PostJson(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
            //request.Accept = "JSON";
            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            if (webResponse == null)
            {
                return "Unable to connect to the remote server";
            }
            var sr = new StreamReader(webResponse.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        public static string PostForm(string url, string parameters)
        {

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
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

        public async Task<string> CallbackJson(string url, string postData)
        {
            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.PostAsync(url, httpContent).Result;

                    if (response.Content != null)
                    {
                        var responseContent = response.Content.ReadAsStringAsync().Result;
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

    }
}
