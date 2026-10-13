using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using APINTNet.Entity;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APINTNet
{
    internal class TopupApp
    {

        //private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "http://api.naptheck.net:8888/";
        //private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "https://naptien247.net/api/v1/sendcard";
        //private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "https://naptien247.pro/api/v1/sendcard";
        private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "https://xuantocdo.pro/api/v1/sendcard";
        private string CalbackUrl = ConfigurationManager.AppSettings["CalbackUrl"] ?? "http://45.32.115.186:1590/CardCallback.ashx";
        private string CalbackUrlV2 = "http://45.32.115.186:1590/CardCallbackV2.ashx";
        private const string usercode = "son02";
        //private const string userpass = "son02@";
        private const string userpass = "21AC3C3C-D86A-483F-9F80-6E86BA1C5AD8";

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public async Task<string> NTNetTopup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {
            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Napthe247";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            var type = string.Empty;

            switch (telco.ToLower())
            {
                case "vtt":
                    type = "VIETTEL";
                    break;
                case "vms":
                    type = "MOBI";
                    break;
                case "vnp":
                    type = "VINA";
                    break;
            }

            var topupRequest = new TopupRequest
            {
                usercode = usercode,
                telco = type,
                cardcode = pin,
                cardseri = serial,
                amount = amount,
                refcode = topup.Id.ToString(),
                appcode = string.Empty,
                acconame = topup.Id.ToString(),
                callurl = CalbackUrl,
                sign = Encrypts.MD5(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", usercode, type, pin, serial, amount, topup.Id, string.Empty, topup.Id, CalbackUrl, userpass))
            };



            try
            {
                var param = new Dictionary<string, string>();
                param.Add("usercode", topupRequest.usercode);
                param.Add("telco", topupRequest.telco);
                param.Add("cardcode", topupRequest.cardcode);
                param.Add("cardseri", topupRequest.cardseri);
                param.Add("amount", topupRequest.amount.ToString());
                param.Add("refcode", topupRequest.refcode);
                param.Add("appcode", topupRequest.appcode);
                param.Add("acconame", topupRequest.acconame);
                param.Add("callurl", topupRequest.callurl);
                param.Add("sign", topupRequest.sign);

                NLogLogger.Info(new string[] { "NTNetTopup", "Request", topup.Id.ToString(), serializer.Serialize(topupRequest) });
                var response = Task.Run(async () => await PostTask(ServiceUrl, param)).Result;
                NLogLogger.Info(new string[] { "NTNetTopup", "Response", topup.Id.ToString(), response });

                if (!string.IsNullOrEmpty(response))
                {

                    var res = serializer.Deserialize<TopupResponse>(response);

                    switch (res.status)
                    {
                        case "0":
                            var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                            if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                            break;
                        case "94":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionFailed, string.Empty, string.Empty);
                            return  string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);
                        case "95":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.CardAmountInvalid, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.CardAmountInvalid, 0);
                        case "96":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.CardSerialInvalid, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.CardSerialInvalid, 0);
                        case "97":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.CardFormatInvalid, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.CardFormatInvalid, 0);
                        case "98":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.CardProviderInvalid, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.CardProviderInvalid, 0);
                        case "99":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.TransactionRejected, 0);
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.TransactionRejected, 0);
                    }       
                    
                    
                    //if (res.status == "0")
                    //{
                    //    var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                    //    if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                    //}

                    //if (res.status != "0")
                    //{
                    //    DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                    //    return "-7|0";
                    //}

                    

                    DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                    return "-1|0";

                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "NTNetTopup", "Response failed Exeption", topup.Id.ToString(), exp.Message });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        public async Task<string> NTNetTopupCallback(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {
            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Napthe247";
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.AmountUser = amount;
            topup.Add();
            topup.Id = topup.ReturnValue;

            var type = string.Empty;

            switch (telco.ToLower())
            {
                case "vtt":
                    type = "VIETTEL";
                    break;
                case "vms":
                    type = "MOBI";
                    break;
                case "vnp":
                    type = "VINA";
                    break;
            }

            var topupRequest = new TopupRequest
            {
                usercode = usercode,
                telco = type,
                cardcode = pin,
                cardseri = serial,
                amount = amount,
                refcode = topup.Id.ToString(),
                appcode = string.Empty,
                acconame = topup.Id.ToString(),
                callurl = CalbackUrlV2,
                sign = Encrypts.MD5(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", usercode, type, pin, serial, amount, topup.Id, string.Empty, topup.Id, CalbackUrlV2, userpass))
            };



            try
            {
                var param = new Dictionary<string, string>();
                param.Add("usercode", topupRequest.usercode);
                param.Add("telco", topupRequest.telco);
                param.Add("cardcode", topupRequest.cardcode);
                param.Add("cardseri", topupRequest.cardseri);
                param.Add("amount", topupRequest.amount.ToString());
                param.Add("refcode", topupRequest.refcode);
                param.Add("appcode", topupRequest.appcode);
                param.Add("acconame", topupRequest.acconame);
                param.Add("callurl", topupRequest.callurl);
                param.Add("sign", topupRequest.sign);

                NLogLogger.Info(new string[] { "NTNetTopup", "Request", topup.Id.ToString(), serializer.Serialize(topupRequest) });
                var response = Task.Run(async () => await PostTask(ServiceUrl, param)).Result;
                NLogLogger.Info(new string[] { "NTNetTopup", "Response", topup.Id.ToString(), response });

                if (!string.IsNullOrEmpty(response))
                {

                    var res = serializer.Deserialize<TopupResponse>(response);

                    switch (res.status)
                    {
                        case "0":
                            return "0|0";
                            break;
                        case "94":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionFailed, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);
                        case "95":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.CardAmountInvalid, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.CardAmountInvalid, 0);
                        case "96":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.CardSerialInvalid, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.CardSerialInvalid, 0);
                        case "97":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.CardFormatInvalid, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.CardFormatInvalid, 0);
                        case "98":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.CardProviderInvalid, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.CardProviderInvalid, 0);
                        case "99":
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.TransactionRejected, 0);
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, (int)ResponseCode.TransactionRejected, string.Empty, string.Empty);
                            return string.Format("{0}|{1}", (int)ResponseCode.TransactionRejected, 0);
                    }


                    //if (res.status == "0")
                    //{
                    //    var task = Task.Run(async () => await CheckStatusAsync(topup.Id));
                    //    if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                    //}

                    //if (res.status != "0")
                    //{
                    //    DataRequest.UpdateTopupCard(topup.Id, 0, -7, string.Empty, string.Empty);
                    //    return "-7|0";
                    //}



                    DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, string.Empty);
                    return "-1|0";

                }

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "NTNetTopup", "Response failed Exeption", topup.Id.ToString(), exp.Message });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, string.Empty);
            return "-326|0";

        }
        private async Task<string> CheckStatusAsync(long id)
        {
            NLogLogger.Info(new string[] { "NTNetTopup", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 90; i++)
            {
                //Check DB 30 lan tuong ung 30s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                    {
                        NLogLogger.Info(new string[] { "NTNetTopup", "CheckStatusAsync", "Response", id.ToString(), string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount) });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }
                        
                System.Threading.Thread.Sleep(1000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, "Timeout", string.Empty); // Update Timeout
            NLogLogger.Info(new string[] { "NTNetTopup", "CheckStatusAsync", id.ToString(), "Timeout" });
            return "-326|0";
        }

        public async Task<string> PostTask(string url, Dictionary<string, string> postData)
        {

            var uri = new Uri(url);
            var httpContent = new FormUrlEncodedContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.Timeout = TimeSpan.FromSeconds(60);
            try
            {
                var response = await client.PostAsync(uri, httpContent);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    client.Dispose();
                    return responseContent;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "NTNetTopup", "Exeption Post", e.Message });
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }

        public static async Task<string> GetTask(string url)
        {
            var uri = new Uri(url);
            HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);

            NLogLogger.Info(new string[] { "NTNetTopup", "GetTask", url });

            try
            {
                var response = await client.GetAsync(uri);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    client.Dispose();
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "NTNetTopup", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;


        }


    }

}