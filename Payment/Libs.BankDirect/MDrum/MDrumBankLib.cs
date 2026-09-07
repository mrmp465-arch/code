using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Libs.Utils;
using System.Net;
using System.IO;

namespace Libs.BankDirect.MDrum
{
    public class MDrumBankLib
    {

        public class RequestData
        {
            public string PartnerCode { get; set; }
            public string ServiceCode { get; set; }
            public string CommandCode { get; set; }
            public string RequestContent { get; set; }

            public string Source { get; set; }


            public string Signature { get; set; }
        }
        public class RequestBank
        {
            public string BankCode { get; set; }
            public int Amount { get; set; }

        }
        public class BankResponse
        {
            public int ResponseCode { get; set; }
            public string Description { get; set; }
            public string ResponseContent { get; set; }
            public string Signature { get; set; }
        }
        public class BankAccountReceive
        {
            public string OrderNo { get; set; }
            public DateTime? CreateDate { get; set; }
            public string QRCodeBase64 { get; set; }
            public string LinkOpenApp { get; set; }
            public string MomoId { get; set; }
            public string MomoName
            {
                get; set;
            }
            public string BankId { get; set; }
            public string BankName
            {
                get; set;
            }
            public string BankCode
            {
                get; set;
            }
            public int Amount { get; set; }
            public string Key { get; set; }
            public string Status { get; set; }
        }
        public class Bank
        {
            public string MomoId { get; set; }
            public string MomoName { get; set; }
        }
        public class BankV2
        {
            public string BankCode { get; set; }

        }
        public class Callback
        {
            public long TransId { get; set; }
            public string MomoId { get; set; }
            public string MomoName { get; set; }

            public string PartnerBankId { get; set; }

            public string PartnerMomoId { get; set; }
            public string PartnerMomoName { get; set; }
            public string BankTransId { get; set; }

            public string MomoTransId { get; set; } // Là OrderNo
            public int Amount { get; set; }
            public string Note { get; set; }
            public string NoteFull { get; set; }
            public string CheckTransId { get; set; }
            //public DateTime TimeMomoSuccess { get; set; }
            //public DateTime? TimeBankSuccess { get; set; }

        }
        public class CallbackV3
        {
            public long TransId { get; set; }
            public string PartnerBankId { get; set; }
            public string PartnerBankName { get; set; }
            public string PartnerBankCode { get; set; }
            public string BankTransId { get; set; }

            public string MomoTransId { get; set; } // Là OrderNo
            public int Amount { get; set; }
            public string Note { get; set; }

            public string NoteFull { get; set; }
            public DateTime TimeBankSuccess { get; set; }

            public string CheckTransId { get; set; }

        }
        public class CallbackV2
        {
            public string requestTime { get; set; }
            public string momo_transId { get; set; }
            public string message { get; set; } // Là OrderNo
            public string money { get; set; }
            public string phone { get; set; }
            public int type { get; set; }
            public string authKey { get; set; }
            public string pid { get; set; }
        }

        public class CallbackResponse
        {
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
        }
        public static async Task<string> CallbackJsonV2(string url, string postData, long Id = 0, string refcode = "", int maxRetry = 3)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpClient client = null;

            // Retry delays: retry #1=30s, retry #2=5m, retry #3=10m
            var retryDelays = new[]
            {
                TimeSpan.FromSeconds(60),
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(10)
            };

            try
            {
                client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
                client.Timeout = TimeSpan.FromSeconds(90);

                // Tổng số lần gọi = 1 (lần đầu) + maxRetry (số lần retry)
                for (int attempt = 0; attempt <= maxRetry; attempt++)
                {
                    try
                    {
                        var attemptNo = (attempt + 1).ToString(); // để log dễ đọc (1..)

                        NLogLogger.Info(new[] { "MDrum", "Callback", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, "Url", url });

                        using (var httpContent = new StringContent(postData ?? "", Encoding.UTF8, "application/json"))
                        {
                            var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                            var responseContent = response.Content != null
                                ? await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                                : string.Empty;

                            LogCache.LogBank(new LogInfo
                            {
                                LogTime = DateTime.Now,
                                Url = url,
                                TransactionID = Id,
                                Request = postData,
                                Respone = "HTTP " + ((int)response.StatusCode) + " " + response.ReasonPhrase + " | " + responseContent
                            });

                            if ((int)response.StatusCode == 200)
                                return responseContent;

                            NLogLogger.Info(new[] { "MDrum", "Callback", "StatusNot200", "Attempt", attemptNo, "Status", ((int)response.StatusCode).ToString(), responseContent });
                        }
                    }
                    catch (TaskCanceledException ex)
                    {
                        var attemptNo = (attempt + 1).ToString();

                        NLogLogger.Info(new[] { "MDrum", "Callback", "Timeout", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, ex.Message });

                        LogCache.LogBank(new LogInfo
                        {
                            LogTime = DateTime.Now,
                            Url = url,
                            TransactionID = Id,
                            Request = postData,
                            Respone = "Timeout: " + ex.ToString()
                        });
                    }
                    catch (HttpRequestException ex)
                    {
                        var attemptNo = (attempt + 1).ToString();

                        NLogLogger.Info(new[] { "MDrum", "Callback", "HttpRequestException", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, ex.Message });

                        LogCache.LogBank(new LogInfo
                        {
                            LogTime = DateTime.Now,
                            Url = url,
                            TransactionID = Id,
                            Request = postData,
                            Respone = "HttpRequestException: " + ex.ToString()
                        });
                    }
                    catch (Exception ex)
                    {
                        var attemptNo = (attempt + 1).ToString();

                        NLogLogger.Info(new[] { "MDrum", "Callback", "Exception", "Attempt", attemptNo, "Transid", Id.ToString(), "Refcode", refcode, ex.Message });

                        LogCache.LogBank(new LogInfo
                        {
                            LogTime = DateTime.Now,
                            Url = url,
                            TransactionID = Id,
                            Request = postData,
                            Respone = "Exception: " + ex.ToString()
                        });

                        return string.Empty; // lỗi không retry tiếp (theo logic cũ của bạn)
                    }

                    // Nếu đã hết lượt (lần cuối) thì dừng
                    if (attempt == maxRetry)
                        break;

                    // Delay theo lịch: retry #1=30s, #2=5m, #3=10m
                    var delayIndex = attempt; // attempt=0 -> delay[0] (30s), attempt=1 -> delay[1] (5m), attempt=2 -> delay[2] (10m)
                    var delay = retryDelays[Math.Min(delayIndex, retryDelays.Length - 1)];

                    NLogLogger.Info(new[] { "MDrum", "Callback", "DelayBeforeRetry", delay.ToString(), "AttemptNext", (attempt + 2).ToString(), "Transid", Id.ToString(), "Refcode", refcode });

                    await Task.Delay(delay).ConfigureAwait(false);
                }

                return string.Empty;
            }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }

        public static async Task<string> PostTask(string url, string postData)
        {

            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
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
                NLogLogger.Info(new string[] { "MDrum", "Exeption Post", e.Message });
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }
        public static string GetSplit(string input)
        {


            // Tách chuỗi thành mảng các phần tử
            string[] items = input.Split(',');

            // Khởi tạo bộ sinh số ngẫu nhiên
            Random random = new Random();

            // Lấy một phần tử ngẫu nhiên
            int index = random.Next(items.Length);
            return items[index];

        }
        public static async Task<string> GetTask(string url)
        {
            var uri = new Uri(url);
            HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);

            NLogLogger.Info(new string[] { "MDrum", "GetTask", url });

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
                NLogLogger.Info(new string[] { "MDrum", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;
        }
        public static string getBankName(string BankCode)
        {
            switch (BankCode)
            {
                case "BIDV":
                    return "Ngân hàng đầu tư và phát triển BIDV";

                case "VCB":
                    return "Ngân hàng Ngoại thương VietComBank";
                case "ACB":
                    return "Ngân hàng Á Châu ACB";
                case "MB":
                    return "Ngân hàng Quân đội MB";
                case "VPB":
                    return "Ngân hàng VPBank";
                case "SEAB":
                    return "Ngân hàng SeaBank";
            }
            return BankCode;
        }
        public static string GetChatId(string id)
        {
            string partnecode = "";
            switch (id)
            {
                case "shdsn555":
                    partnecode = "-4922017479";
                    break;

                case "go99":
                    partnecode = "-1003135161687";
                    break;
                case "nohu888":
                    partnecode = "-1003208708575";
                    break;
                case "shdsn777":
                    partnecode = "-4573996267";
                    break;
                case "hn002":
                    partnecode = "-4284901114";
                    break;
                case "shdsn444":
                    partnecode = "-1002393586188";
                    break;
                case "shdsn666":
                    partnecode = "-4245345680";
                    break;
                case "shdsn888":
                    partnecode = "-4586763139";
                    break;
                case "shdsn999":
                    partnecode = "-4527995497";
                    break;
                case "vs8":
                    partnecode = "-5186836224";
                    break;

            };
            return partnecode;
        }
        public static void LogBankInfo(BankAccountReceive log)
        {

            string KeyCache = string.Format("{0}:{1}", "LogBankInfo", log.Key);
            DataCaching.SetCache(KeyCache, log, 86400 / 48);
        }
        public static BankAccountReceive GetBankInfo(string OrderNo)
        {

            string KeyCache = string.Format("{0}:{1}", "LogBankInfo", OrderNo);
            return DataCaching.GetCache<BankAccountReceive>(KeyCache);

        }
        public static string GetBankSuccess(string OrderNo)
        {

            string KeyCache = string.Format("{0}:{1}", "LogBankSuccess", OrderNo);
            return DataCaching.GetCache<string>(KeyCache);

        }
        public static void SetBankSuccess(string OrderNo)
        {

            string KeyCache = string.Format("{0}:{1}", "LogBankSuccess", OrderNo);
            DataCaching.SetCache(KeyCache, OrderNo, 3600 / 15);
        }
        public static string GetBankSuccess2(string OrderNo)
        {

            string KeyCache = string.Format("{0}:{1}", "LogBankSuccess2", OrderNo);
            return DataCaching.GetCache<string>(KeyCache);

        }

        public static void SetBankSuccess2(string OrderNo)
        {

            string KeyCache = string.Format("{0}:{1}", "LogBankSuccess2", OrderNo);
            DataCaching.SetCache(KeyCache, OrderNo, 3600 * 24);
        }
        public static void SetBankSuccess3(string OrderNo)
        {

            string KeyCache = "LogBankSuccess3";
            var data = DataCaching.GetCache<List<string>>(KeyCache);
            if (data == null)
                data = new List<string>();
            data.Add(OrderNo);
            DataCaching.SetCache(KeyCache, data, 3600 * 24);
        }
        public static string GetBankSuccess3(string OrderNo)
        {

            string KeyCache = "LogBankSuccess3";
            var data = DataCaching.GetCache<List<string>>(KeyCache);
            if(data==null)
                return "";
            if (data.Exists(x => x.Equals(OrderNo)))
                return OrderNo;
            return "";

        }
        public static async Task<string> CallbackJson(string url, string postData, long Id = 0, string refcode = "")
        {

            NLogLogger.Info(new string[] { "MDrum", "Callback", "Partner", "Request", postData });
            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(90);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            try
            {
                var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "MDrum", "Callback", "Transid", "Refcode", "Response", Id.ToString(), refcode, responseContent });
                    var log = new LogInfo
                    {
                        LogTime = DateTime.Now,
                        Url = url,
                        TransactionID = Id,
                        Request = postData,
                        Respone = responseContent
                    };
                    LogCache.LogBank(log);
                    client.Dispose();
                    return responseContent;
                }

            }
            catch (WebException e)
            {
                var responseStream = e.Response.GetResponseStream();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        NLogLogger.Info(new string[] { "MDrum", "Exeption Post", reader.ReadToEnd() });
                        var log1 = new LogInfo
                        {
                            LogTime = DateTime.Now,
                            Url = url,
                            TransactionID = Id,
                            Request = postData,
                            Respone = reader.ReadToEnd()
                        };
                        LogCache.LogBank(log1);
                        //return result;
                    }
                }
                NLogLogger.Info(new string[] { "MDrum", "Exeption Post", e.Message });
                var log = new LogInfo
                {
                    LogTime = DateTime.Now,
                    Url = url,
                    TransactionID = Id,
                    Request = postData,
                    Respone = e.Message
                };
                LogCache.LogBank(log);
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }
    }
}
