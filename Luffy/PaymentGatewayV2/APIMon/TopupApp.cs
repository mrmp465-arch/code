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
using APIMon.Entity;
using Libs.Report;
using Libs.Utils;

namespace APIMon
{
    internal class TopupApp
    {


        private string ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"] ?? "http://dellgr-ba88c5e62d1c.chipchip.banglangtim.club/api/recharge";
        private string CalbackUrl = ConfigurationManager.AppSettings["CalbackUrl"] ?? "http://149.28.130.246:1590/CardCallback.ashx";
        private const string api_key = "e912af09-b266-4274-a50d-ba88c5e62d1c";

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public async Task<string> MonTopup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {
            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.SimTarget = "Mon";
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
                    type = "VT";
                    break;
                case "vms":
                    type = "Mobi";
                    break;
                case "vnp":
                    type = "Vina";
                    break;
            }

            var topupRequest = new TopupRequest
            {
                api_key = api_key,
                card_seri = serial,
                card_code = pin,
                card_amount = amount,
                request_id = topup.Id.ToString(),
                card_type = type,
                signature = Encrypts.MD5(string.Format("{0}{1}{2}{3}", api_key, amount, pin, serial))
            };


            try
            {
                var param = new Dictionary<string, string>();
                param.Add("api_key", api_key);
                param.Add("card_seri", topupRequest.card_seri);
                param.Add("card_code", topupRequest.card_code);
                param.Add("card_amount", topupRequest.card_amount.ToString());
                param.Add("request_id", topupRequest.request_id);
                param.Add("card_type", topupRequest.card_type);
                param.Add("signature", topupRequest.signature);

                NLogLogger.Info(new string[] { "MonTopup", "Request", topup.Id.ToString(), serializer.Serialize(topupRequest) });
                var response = Task.Run(async () => await PostTask(ServiceUrl, param)).Result;
                NLogLogger.Info(new string[] { "MonTopup", "Response", topup.Id.ToString(), response });

                if (!string.IsNullOrEmpty(response))
                {

                    var res = serializer.Deserialize<TopupResponse>(response);

                    switch (res.status)
                    {
                        case 0:
                            var task = Task.Run(async () => await CheckStatusAsync(topup.Id, res.tran_id));
                            if (task.Wait(TimeSpan.FromSeconds(130))) return task.Result;
                            break;
                        case -1:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, "THIẾU THAM SỐ - LỖI CHƯA XÁC ĐỊNH", res.tran_id);
                            break;
                        case -2:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -337, "THẺ SAI ĐỊNH DẠNG, VÍ DỤ: VTT MÃ THẺ 13 HOẶC 15 KÝ TỰ SỐ", res.tran_id);
                            return "-337|0";
                        case -3:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, "SAI CHỮ KÝ", res.tran_id);
                            return "-1|0";
                        case -4:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -300, "HỆ THỐNG API BẢO TRÌ", res.tran_id);
                            return "-300|0";
                        case -5:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -303, "THẺ CHƯA ĐC NẠP, GỬI TRẢ LẠI THẺ - (HỆ THỐNG ĐANG BẬN)", res.tran_id);
                            return "-303|0";
                        case -6:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, "USER ĐẨY THẺ KHÔNG HOẠT ĐỘNG (INACTIVED)", res.tran_id);
                            return "-1|0";
                        case -98:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, "THẺ ĐÃ NẠP TRƯỚC ĐÓ, THẺ TRÙNG", res.tran_id);
                            return "-7|0";
                        case -99:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -7, "THẺ ĐANG ĐƯỢC CHỜ XỬ LÝ. VUI LÒNG ĐỢI TRONG GIẤY LÁT", res.tran_id);
                            return "-7|0";
                        default:
                            DataRequest.UpdateTopupCard(topup.Id, 0, -1, string.Empty, res.tran_id);
                            return "-1|0";

                    }

                }
                else
                {
                    NLogLogger.Info(new string[] { "MonTopup", "Response", topup.Id.ToString(), "EMPTY" });
                }
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "MonTopup", "Response failed Exeption", topup.Id.ToString(), exp.Message });

            }

            DataRequest.UpdateTopupCard(topup.Id, 0, -326, string.Empty, 0);
            return "-326|0";

        }
        private async Task<string> CheckStatusAsync(long id, long requestNo)
        {
            NLogLogger.Info(new string[] { "MonTopup", "CheckStatusAsync", id.ToString() });
            for (int i = 0; i < 90; i++)
            {
                //Check DB 30 lan tuong ung 30s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                    {
                        NLogLogger.Info(new string[] { "MonTopup", "CheckStatusAsync", "Response", id.ToString(), string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount) });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }

                System.Threading.Thread.Sleep(1000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty, requestNo); // Update Timeout
            NLogLogger.Info(new string[] { "MonTopup", "CheckStatusAsync", id.ToString(), "Timeout" });
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
                NLogLogger.Info(new string[] { "MonTopup", "Exeption Post", e.Message });
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

            NLogLogger.Info(new string[] { "MonTopup", "GetTask", url });

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
                NLogLogger.Info(new string[] { "MonTopup", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;


        }


    }

}