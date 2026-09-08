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
using Tiger.Entity;
using Libs.Report;
using Libs.Utils;

namespace Tiger
{
    internal class TopupApp
    {

        private string Token = "ODdkYzc2YzBjNTk5ZGU4MmU5NTQ4NGE2Njc4OWNhNTE=";
        private string RequestCode = "PcfEoU";
        private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "https://congthe.biz/apis/card/add ";
        //private string CallbackUrl = ConfigurationManager.AppSettings["CallbackUrl"] ?? "http://149.28.130.246:1598/CardCallback.ashx";

        //private const string key = "c3c4fd1d4425ee9f20842369a035197a";

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public async Task<string> TigerTopup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
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
                    type = "VNP";
                    break;
                case "zing":
                    type = "ZING";
                    break;
                case "vnm":
                    type = "VNM";
                    break;
                default:
                    type = "ZING";
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Tiger";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {

                var url = ServiceUrl;
                if (type == "ZING")
                    url = "https://congthe.biz/apis/card/add-game";
                var signature = Encrypts.MD5(Token + RequestCode);
                var data = string.Format("client_token_api={0}&client_code_request={1}&client_signature={2}&client_type_card={3}&client_seri_card={5}&client_code_card={4}&client_value_card={6}&client_transaction_id={7}", this.Token, this.RequestCode, signature, type, pin, serial, amount, topup.Id);
                NLogLogger.Info(new string[] { "APITiger", "Request", transactionId, topup.Id.ToString(), data });
                var response = Task.Run(async () => await PostTask(url, data)).Result;
                NLogLogger.Info(new string[] { "APITiger", "Response", transactionId, topup.Id.ToString(), response });
                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);


                    switch (res.code)
                    {
                        case 999:
                            if(topup.PartnerCode == "hyn7" || topup.PartnerCode == "hyn6" || topup.PartnerCode == "imd")
                            {
                                return "0|0";
                            }
                            else
                            {
                                var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                                if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                            }
                           
                            break;
                        case 104:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                            return "-334|0";
                        case 181:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";
                        case 103:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                            return "-7|0";
                        //case "receive_success":
                        //    var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                        //    if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                        //    break;
                        //case "card_duplicate":
                        //    DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                        //    return "-7|0";
                        //case "card_format_wrong":
                        //    DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                        //    return "-334|0";
                        //case "type_maintenance":
                        //    DataRequest.UpdateTopupCard(topup.Id, 0, -300, string.Empty, string.Empty);
                        //    return "-300|0";
                        //case "type_error":
                        //    DataRequest.UpdateTopupCard(topup.Id, 0, -336, string.Empty, string.Empty);
                        //    return "-336|0";


                        default:
                            switch (res.reject_message.ToLower())
                            {
                                case "card_format_wrong":
                                    DataRequest.UpdateTopupCard(topup.Id, 0, -335, string.Empty, string.Empty);
                                    return "-335|0";
                                case "card_code_existed":
                                    DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                                    return "-330|0";
                                case "seri_format_wrong":
                                case "seri_wrong_format":
                                    DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                                    return "-334|0";
                                default:

                                    DataRequest.UpdateTopupCard(topup.Id, 0, -324, "", string.Empty);
                                    return "-324|0";
                            }
                    }


                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APITiger", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        public async Task<string> TigerTopupCallback(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
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
                    type = "VNP";
                    break;
                case "zing":
                    type = "ZING";
                    break;
                case "vnm":
                    type = "VNM";
                    break;
                default:
                    type = "ZING";
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Tiger";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            try
            {

                var url = ServiceUrl;
                if (type == "ZING")
                    url = "https://congthe.biz/apis/card/add-game";
                var signature = Encrypts.MD5(Token + RequestCode);
                var data = string.Format("client_token_api={0}&client_code_request={1}&client_signature={2}&client_type_card={3}&client_seri_card={5}&client_code_card={4}&client_value_card={6}&client_transaction_id={7}", this.Token, this.RequestCode, signature, type, pin, serial, amount, topup.Id);
                NLogLogger.Info(new string[] { "APITiger", "Request", transactionId, topup.Id.ToString(), data });
                var response = Task.Run(async () => await PostTask(url, data)).Result;
                NLogLogger.Info(new string[] { "APITiger", "Response", transactionId, topup.Id.ToString(), response });
                if (!string.IsNullOrEmpty(response))
                {
                    var res = serializer.Deserialize<TopupResponse>(response);


                    switch (res.code)
                    {
                        case 999:
                            return "0|0";
                        case 103:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                            return "-7|0";
                        case 104:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                            return "-334|0";
                        case 181:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                            return "-330|0";
                        //case "receive_success":
                        //    var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                        //    if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                        //    break;
                        //case "card_duplicate":
                        //    DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                        //    return "-7|0";
                        //case "card_format_wrong":
                        //    DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                        //    return "-334|0";
                        //case "type_maintenance":
                        //    DataRequest.UpdateTopupCard(topup.Id, 0, -300, string.Empty, string.Empty);
                        //    return "-300|0";
                        //case "type_error":
                        //    DataRequest.UpdateTopupCard(topup.Id, 0, -336, string.Empty, string.Empty);
                        //    return "-336|0";


                        default:
                            switch (res.reject_message.ToLower())
                            {
                                case "card_format_wrong":
                                    DataRequest.UpdateTopupCard(topup.Id, 0, -335, string.Empty, string.Empty);
                                    return "-335|0";
                                case "card_code_existed":
                                    DataRequest.UpdateTopupCard(topup.Id, 0, -330, string.Empty, string.Empty);
                                    return "-330|0";
                                case "seri_format_wrong":
                                case "seri_wrong_format":
                                    DataRequest.UpdateTopupCard(topup.Id, 0, -334, string.Empty, string.Empty);
                                    return "-334|0";
                                default:

                                    DataRequest.UpdateTopupCard(topup.Id, 0, -324, "", string.Empty);
                                    return "-324|0";
                            }
                    }


                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "APITiger", "Response failed Exeption", topup.Id.ToString(), exp.Message, exp.StackTrace });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "APITiger", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 90; i++)
            {
                //Check DB 30 lan tuong ung 30s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                    {
                        NLogLogger.Info(new string[] { "APITiger", "CheckStatusAsync", "Response", id.ToString(), string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount) });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }

                System.Threading.Thread.Sleep(1000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty, string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "APITiger", "CheckStatusAsync", id.ToString(), "Timeout" });
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
                NLogLogger.Info(new string[] { "APITiger", "Exeption Post", e.Message });
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
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

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
                NLogLogger.Info(new string[] { "APITiger", "PostTask", "Exception", e.Message });
            }

            httpClient.Dispose();
            return string.Empty;

        }

        public static async Task<string> GetTask(string url)
        {

            var uri = new Uri(url);
            var tryAgain = 0;
            var httpClient = new HttpClient();
            while (tryAgain < 3)
            {

                try
                {
                    var response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseConten = await response.Content.ReadAsStringAsync();
                        httpClient.Dispose();
                        tryAgain = 3;
                        return responseConten;
                    }

                }

                catch (Exception e)
                {
                    tryAgain++;
                    NLogLogger.Info(new string[] { "APITiger", "GetTask", "Exception", url, e.Message, e.StackTrace });
                }
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


    }

}