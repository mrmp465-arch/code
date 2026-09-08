using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using APIAloPay.Entity;
using Libs.Report;
using Libs.Utils;
using RestSharp;

namespace APIAloPay
{
    internal class TopupApp
    {

        private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "https://alopay.xyz/";
        //private string CallbackUrl = ConfigurationManager.AppSettings["CallbackUrl"] ?? "https://ship.zoroshop.xyz/CardCallback.ashx";
        //private string CallbackUrlV2 = "https://ship.zoroshop.xyz/CardCallbackV2.ashx";
        private string CallbackUrlV2 = "http://149.28.151.96:1588/CardCallbackV2.ashx";
        private string CallbackUrl = "http://149.28.151.96:1588/CardCallback.ashx";
        private const string apiKey = "vitngo@";
        private const string username = "vitngo@gmail.com";
        private const int ClientID = 2;

        private const string SecretKey = "rbpc3QluU2dmNY5XkpAoDKY4ROU8pBq9qUImgfU0";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private string getToken()
        {
            var token = "";
            var dataCache = DataCaching.GetCache<string>("AloPaysTokenV3");
            if (dataCache != null)
            {
                token = dataCache.ToString();
                //NLogLogger.Info(new string[] { "APIAloPays", "Token", "", "", token});
            }
            else
            {
                var data = new { client_id = ClientID, client_secret = SecretKey, username = username, grant_type = "password", password = apiKey, scope = "*" };
                //NLogLogger.Info(new string[] { "APIAloPays", "Request", "", "", serializer.Serialize((data)) });
                var response = Task.Run(async () => await PostTask(string.Format("{0}//oauth/token", ServiceUrl), serializer.Serialize(data))).Result;
                //NLogLogger.Info(new string[] { "APIAloPays", "Response", "", "", response });
                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TokenResponse>(response);
                    token = res.access_token;
                    DataCaching.SetCache("AloPaysTokenV3", token, 3600*4);
                }
            }

            return token;
        }
        public async Task<string> VinaPayTopupCallBack(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {


            var type = 0;

            switch (telco.ToLower())
            {
                case "vtt":
                    type = 0;
                    break;
                case "vms":
                    type = 2;
                    break;
                case "vnp":
                    type = 1;
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Alo";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {
                TopupRequest cardRequest = new TopupRequest();

                cardRequest.serial = serial;
                cardRequest.pin = pin;
                cardRequest.carrier = type;
                cardRequest.email = username;
                cardRequest.amount = amount.ToString();
                cardRequest.url_callback = CallbackUrlV2;
                cardRequest.custom_trans = topup.Id.ToString();

                var token = getToken();
                var getUrl = $"{ServiceUrl}/api/payment?pin={cardRequest.pin}&serial={cardRequest.serial}&carrier={cardRequest.carrier}&amount={cardRequest.amount}&email={cardRequest.email}&url_callback={cardRequest.url_callback}&custom_trans={cardRequest.custom_trans}";
                NLogLogger.Info(new string[] { "APIAloPays", "Request", transactionId, topup.Id.ToString(), getUrl });
                //var sw = new Stopwatch();
                //sw.Start();
                var response = Task.Run(async () => await GetTask(getUrl, token)).Result;
                //if(response.Contains("error_code\":1"))
                //{
                //    token = getToken();
                //    response = Task.Run(async () => await GetTask(getUrl, token)).Result;
                //}    
                //sw.Stop();
                NLogLogger.Info(new string[] { "APIAloPays", "Response", transactionId, topup.Id.ToString(), response });
                //if (sw.ElapsedMilliseconds > 5000)
                //{
                //    TelegramNotify.SendNotify(-400169942, $"Nghi vấn thẻ {serial} bị nuốt");
                //}
                if (!string.IsNullOrEmpty(response))
                {
                    if(response.Contains("Server Error"))
                    {
                        TelegramNotify.SendWarning("1690000254", "Provider alo bị lỗi");
                        TelegramNotify.SendTeleZoro(-937600543, "Server Error");

                        return "-1|0";
                    }    
                    var res = serializer.Deserialize<TopupResponse>(response);

                    //if(res.error_code==6)
                    //{
                    //    System.Threading.Thread.Sleep(3000);
                    //    response = Task.Run(async () => await GetTask(getUrl, token)).Result;
                    //    res = serializer.Deserialize<TopupResponse>(response);
                    //}    

                    switch (res.error_code)
                    {
                        case 0:
                            return "0|0";
                           

                       
                        case 101:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";
                        case 104:
                        case 12:
                        case 3:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                            return "-334|0";
                        case 13:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -327, string.Empty, string.Empty);
                            return "-327|0";
                        case 4:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";

                        case 103:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                            return "-7|0";
                        case 6:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            TelegramNotify.SendWarning("1690000254", "Provider big bị lỗi");
                            TelegramNotify.SendTeleZoro(-937600543, "Hệ thống không đủ tài nguyên hoặc đang bảo trì");
                            return "-1|0";
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }

                }

            }

            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIAloPay", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        public async Task<string> VinaPayTopup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {


            var type = 0;

            switch (telco.ToLower())
            {
                case "vtt":
                    type = 0;
                    break;
                case "vms":
                    type = 2;
                    break;
                case "vnp":
                    type = 1;
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Push247";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {
                TopupRequest cardRequest = new TopupRequest();

                cardRequest.serial = serial;
                cardRequest.pin = pin;
                cardRequest.carrier = type;
                cardRequest.email = username;
                cardRequest.amount = amount.ToString();
                cardRequest.url_callback = CallbackUrl;
                cardRequest.custom_trans = topup.Id.ToString();

                var token = getToken();
                var getUrl = $"{ServiceUrl}/api/payment?pin={cardRequest.pin}&serial={cardRequest.serial}&carrier={cardRequest.carrier}&amount={cardRequest.amount}&email={cardRequest.email}&url_callback={cardRequest.url_callback}&custom_trans={cardRequest.custom_trans}";
                NLogLogger.Info(new string[] { "APIAloPays", "Request", transactionId, topup.Id.ToString(), getUrl});
                var sw = new Stopwatch();
                sw.Start();
                var response = Task.Run(async () => await GetTask( getUrl, token)).Result;
                sw.Stop();
                NLogLogger.Info(new string[] { "APIAloPays", "Response", transactionId, topup.Id.ToString(), response });
                //if (sw.ElapsedMilliseconds > 5000)
                //{
                //    TelegramNotify.SendNotify(-400169942, $"Nghi vấn thẻ {serial} bị nuốt");
                //}
                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);

                    switch (res.error_code)
                    {
                        case 0:
                            var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                            if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                            break;

                        case 3:
                        case 101:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";
                        case 104:
                        case 12:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                            return "-334|0";
                        case 13:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -327, string.Empty, string.Empty);
                            return "-327|0";
                        case 4:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";
                       
                        case 103:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                            return "-7|0";
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                            return "-1|0";
                    }

                }

            }

            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APIAloPay", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "APIAloPay", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 90; i++)
            {
                //Check DB 30 lan tuong ung 30s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                    {
                        NLogLogger.Info(new string[] { "APIAloPay", "CheckStatusAsync", "Response", id.ToString(), string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount) });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }

                System.Threading.Thread.Sleep(1000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty, string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "APIAloPay", "CheckStatusAsync", id.ToString(), "Timeout" });
            return "-326|0";
        }

        public async Task<string> PostTask(string url, Dictionary<string, string> postData)
        {
            try
            {
                var uri = new Uri(url);
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var response = await client.PostAsync(uri, httpContent);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APIAloPay", "Exeption Post", e.Message });
                return string.Empty;
            }

            return string.Empty;


        }
        public static string PostJson(string uri, string postData)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Method = "POST";//GET
                request.Timeout = 20000;
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
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    //return "{\"status\":-5002,\"data\":{\"amount\": 0},\"amount\": 0,\"msg\": \"Ngắt kết nối đến nhà cung cấp Mobo. Timeout exception\"}";
                    NLogLogger.Info(new string[] { "BicBic", "BicBicRequest", postData, "Timeout exception" });
                    return "{\"code\":-5002,\"desc\":\"Ngắt kết nối đến nhà cung cấp BicBIC. Timeout exception\"}";
                }
                else
                {
                    throw;
                }
            }
        }

        [Obsolete]
        public static async Task<string> PostTask(string url, string postData, string token = "")
        {

            var client = new RestClient(url);

            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(token))
            {
                //httpContent.Headers.Add("Authorization", "bearer " + token);
                request.AddHeader("Authorization", "Bearer " + token);
            }
           
            request.AddParameter("application/json", postData, ParameterType.RequestBody);
            var response = client.ExecuteTaskAsync(request);
            return response.Result.Content;
            
        }

        public static async Task<string> GetTask(string url, string token = "")
        {


            var client = new RestClient(url);

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(token))
            {
                //httpContent.Headers.Add("Authorization", "bearer " + token);
                request.AddHeader("Authorization", "Bearer " + token);
                //NLogLogger.Info(new string[] { "APIAloPays", "Authorization", "", "", "Bearer " + token });
            }
            //request.AddParameter("application/json", postData, ParameterType.RequestBody);
            //NLogLogger.Info(new string[] { "APIAloPays", "", "", "", postData });
            var response = client.ExecuteTaskAsync(request);
            return response.Result.Content;

        }
        public static String GetHashHMACSHA256(String text, String key)
        {
            Byte[] textBytes = Encoding.UTF8.GetBytes(text);
            Byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            return Convert.ToBase64String(hashBytes);
        }


    }

}