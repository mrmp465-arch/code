using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using IZISoft.Entity;
using Libs.Report;
using Libs.Utils;

namespace IZISoft
{
    internal class TopupApp
    {

        private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "http://izisoft.info/api";
        private string CallbackUrl = ConfigurationManager.AppSettings["CallbackUrl"] ?? "http://149.28.130.246:1592/CardCallback.ashx";

        private const string username = "corona";
        private const string passwork = "12346789";

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public async Task<string> IZISoftTopup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {


            var type = string.Empty;

            switch (telco.ToLower())
            {
                case "vtt":
                    type = "VTT";
                    break;
                case "vms":
                    type = "VMS";
                    break;
                case "vnp":
                    type = "VINA";
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "IZISoft";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {
                //var signData = string.Format("requestId|nccCode|gameCode|account|cardNumber|serialNumber|cardValue|provider|type|accessKey")
                var auth = Task.Run(async () => await GetAuth()).Result;
                var topupRequest = new TopupRequest
                {
                    telcoId = Task.Run(async () => await GetTelcoId(auth, type)).Result,
                    denId = Task.Run(async () => await GetAmoutId(auth, amount.ToString())).Result,
                    code = pin,
                    serial = serial,
                    scratchCallbackUrl = CallbackUrl + "?tranid=" + topup.Id
                };

                var url = ServiceUrl + "/v2/cp/card?auth=" + auth;
                NLogLogger.Info(new string[] { "IZISoftTopup", "Request", transactionId, topup.Id.ToString(), url, serializer.Serialize(topupRequest) });
                var response = Task.Run(async () => await PostTask(url, serializer.Serialize(topupRequest))).Result;
                NLogLogger.Info(new string[] { "IZISoftTopup", "Response", transactionId, topup.Id.ToString(), url, response });
                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);

                    if (res.code == "4003" || res.code == "4004")
                    {
                        Utils.RemoveCache("sid");
                        auth = Task.Run(async () => await GetAuth()).Result;
                        url = ServiceUrl + "/v2/cp/card?auth=" + auth;
                        response = Task.Run(async () => await PostTask(url, serializer.Serialize(topupRequest))).Result;
                        if (!string.IsNullOrEmpty(response))
                        {
                            res = serializer.Deserialize<TopupResponse>(response);
                        }
                    }

                    if (res.code == "200")
                    {
                        var task = Task.Run(() => CheckStatusAsync(topup.Id));
                        if (task.Wait(TimeSpan.FromSeconds(120)))
                            return task.Result;
                    }
                    else
                    {
                        switch (res.code)
                        {
                            case "4002":
                                DataRequest.UpdateTopupCard(topup.Id, 0, -323, string.Empty, string.Empty);
                                return "-323|0";
                            case "4005":
                                DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                                return "-7|0";
                            case "4006":
                                DataRequest.UpdateTopupCard(topup.Id, 0, -337, string.Empty, string.Empty);
                                return "-337|0";
                            case "4007":
                                DataRequest.UpdateTopupCard(topup.Id, 0, -372, string.Empty, string.Empty);
                                return "-372|0";
                            case "503":
                                DataRequest.UpdateTopupCard(topup.Id, 0, -301, string.Empty, string.Empty);
                                return "-301|0";

                            default:
                                DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                                return "-1|0";
                        }
                    }

                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "IZISoftTopup", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "IZISoftTopup", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 100; i++)
            {
                //Check DB 30 lan tuong ung 30s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                System.Threading.Thread.Sleep(1000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty, string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "IZISoftTopup", "CheckStatusAsync", id.ToString(), "Timeout" });
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
                NLogLogger.Info(new string[] { "IZISoftTopup", "Exeption Post", e.Message });
                return string.Empty;
            }

            return string.Empty;


        }

        public static async Task<string> PostTask(string url, string postData)
        {

            var uri = new Uri(url);
            var httpClient = new HttpClient();
            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await httpClient.PostAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    return responseContent;
                }
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "IZISoft", "PostTask", "Exception", e.Message });
            }

            httpClient.Dispose();
            return string.Empty;

        }

        public static async Task<string> GetTask(string url)
        {

            var uri = new Uri(url);
            var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var responseConten = await response.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    return responseConten;
                }

            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "IZISoft", "GetTask", "Exception", url, e.Message, e.StackTrace });
            }

            httpClient.Dispose();
            return String.Empty;

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

        public async Task<string> GetAuth()
        {
            var url = ServiceUrl + "/user/login";
            var postData = new ProfileRequest()
            {
                username = username,
                password = passwork
            };
            var sid = string.Empty;
            try
            {


                sid = Utils.GetCache("sid");

                if (!string.IsNullOrEmpty(sid))
                {
                    NLogLogger.Info(new string[] { "IZISoft", "GetAuth", "Return Cached", sid });
                    return sid;
                }

                NLogLogger.Info(new string[] { "IZISoft", "GetAuth", "Request", url, serializer.Serialize(postData) });
                var response = Task.Run(async () => await PostTask(url, serializer.Serialize(postData))).Result;
                NLogLogger.Info(new string[] { "IZISoft", "GetAuth", "Response", url, response });

                if (!string.IsNullOrEmpty(response))
                {
                    sid = serializer.Deserialize<ProfileResponse>(response).data.sid;
                    Utils.SetCache("sid", sid);
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "IZISoft", "GetAuth", "Exeption", e.Message, e.StackTrace });
            }

            NLogLogger.Info(new string[] { "IZISoft", "GetAuth", "Return", sid });
            return sid;
        }

        public async Task<int> GetTelcoId(string auth, string code)
        {
            var url = ServiceUrl + "/telcos?auth=" + auth;
            var strTelco = Utils.GetCache("telco");
            TelcoResponse telco;
            int telcoId = 0;
            try
            {
                if (!string.IsNullOrEmpty(strTelco))
                {
                    NLogLogger.Info(new string[] { "IZISoft", "GetTelcoId", "JSON", strTelco });
                    telco = serializer.Deserialize<TelcoResponse>(strTelco);
                    telcoId = telco.data.FirstOrDefault(tc => tc.code == code).id;
                    if (telcoId > 0)
                    {
                        NLogLogger.Info(new string[] { "IZISoft", "GetTelcoId", "Return Cached", telcoId.ToString() });
                        return telcoId;
                    }

                }

                NLogLogger.Info(new string[] { "IZISoft", "GetTelcoId", "Request", url, code });
                var response = Task.Run(async () => await GetTask(url)).Result;
                NLogLogger.Info(new string[] { "IZISoft", "GetTelcoId", "Response", url, response });

                if (!string.IsNullOrEmpty(response))
                {
                    telco = serializer.Deserialize<TelcoResponse>(response);

                    if (telco.code == "4004")
                    {
                        Utils.RemoveCache("sid");
                        auth = Task.Run(async () => await GetAuth()).Result;
                        url = ServiceUrl + "/denominations?auth=" + auth;
                        response = Task.Run(async () => await GetTask(url)).Result;
                        if (!string.IsNullOrEmpty(response))
                        {
                            telco = serializer.Deserialize<TelcoResponse>(response);
                        }

                    }

                    Utils.SetCache("telco", response);
                    telcoId = telco.data.FirstOrDefault(tc => tc.code == code).id;
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "IZISoft", "GetTelcoId", "Exeption", e.Message, e.StackTrace });
            }

            NLogLogger.Info(new string[] { "IZISoft", "GetTelcoId", "Return", telcoId.ToString() });
            return telcoId;
        }

        public async Task<int> GetAmoutId(string auth, string code)
        {

            var url = ServiceUrl + "/denominations?auth=" + auth;
            var strAmount = Utils.GetCache("amount");
            AmountResponse amount;
            int amountId = 0;
            try
            {
                if (!string.IsNullOrEmpty(strAmount))
                {
                    NLogLogger.Info(new string[] { "IZISoft", "GetAmoutId", "JSON", strAmount });
                    amount = serializer.Deserialize<AmountResponse>(strAmount);
                    amountId = amount.data.FirstOrDefault(tc => tc.code == code).id;
                    if (amountId > 0)
                    {
                        NLogLogger.Info(new string[] { "IZISoft", "GetAmoutId", "Return Cached", amountId.ToString() });
                        return amountId;
                    }
                }
                NLogLogger.Info(new string[] { "IZISoft", "GetAmoutId", "Request", url, code });
                var response = Task.Run(async () => await GetTask(url)).Result;

                NLogLogger.Info(new string[] { "IZISoft", "GetAmoutId", "Response", url, response });

                if (!string.IsNullOrEmpty(response))
                {
                    amount = serializer.Deserialize<AmountResponse>(response);
                    if (amount.code == "4004")
                    {
                        Utils.RemoveCache("sid");
                        auth = Task.Run(async () => await GetAuth()).Result;
                        url = ServiceUrl + "/denominations?auth=" + auth;
                        response = Task.Run(async () => await GetTask(url)).Result;

                        if (!string.IsNullOrEmpty(response))
                        {
                            amount = serializer.Deserialize<AmountResponse>(response);
                        }

                    }

                    Utils.SetCache("amount", response);
                    amountId = amount.data.FirstOrDefault(tc => tc.code == code).id;
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "IZISoft", "GetAmoutId", "Exeption", e.Message, e.StackTrace });
            }

            NLogLogger.Info(new string[] { "IZISoft", "GetAmoutId", "Return", amountId.ToString() });
            return amountId;
        }


    }

}