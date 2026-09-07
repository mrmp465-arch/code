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
using Libs.API;
using System.Text.RegularExpressions;

namespace Libs.BankCash.Drum
{
    public class DrumBankLib
    {
        private const string AccessKey = "161e64fd87f42b74bed84780337b2c1b";
        public class CallbackResponse
        {
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
        }
        public class BankResponse
        {
            public int ResponseCode { get; set; }
            public string Description { get; set; }
            public string ResponseContent { get; set; }
            public string Signature { get; set; }
        }

        public class RequestData
        {
            public string PartnerCode { get; set; }
            public string ServiceCode { get; set; }
            public string CommandCode { get; set; }
            public string RequestContent { get; set; }
            public string Signature { get; set; }
            public string Source { get; set; }
        }
        public class CashRespone
        {
            public int ResponseCode { get; set; }
            public string Description { get; set; }
            public string ResponseContent { get; set; }
            public string Signature { get; set; }

        }
        public class MobileRequest
        {
            public string requestTime { get; set; }
            public string userName { get; set; }
            public string accountCheck { get; set; }
            public string authKey { get; set; }
        }
        public class MobileRespone
        {
            public int errorCode { get; set; }
            public string errorDesc { get; set; }
            public MobileMsg msg { get; set; }
        }
        public class MobileMsg
        {

            public string name { get; set; }

        }
        public class UserInfo
        {
            public string name { get; set; }
            //public string userName { get; set; }
            //public string accountCheck { get; set; }
            //public string authKey { get; set; }
        }
        public class Callback
        {
            public string transId { get; set; }
            public string MomoId { get; set; }
            public string MomoName { get; set; }
            public string MomoTransId { get; set; }
            public int Amount { get; set; }
            //public DateTime TimeMomoSuccess { get; set; }


            public string BankName { get; set; }
            public string Note { get; set; }
            public string BankTransId { get; set; }
            public string Comment { get; set; }

            public string PartnerBankCode { get; set; }
            public string PartnerBankId { get; set; }
            public string PartnerBankName { get; set; }

            public string MomoPartnerId { get; set; } // NG Chuyển
            public string MomoPartnerName { get; set; }

        }
        public class DataCallback
        {
            public string RefCode { get; set; }
            //public string MomoTransId { get; set; }
            public string TransactionID { get; set; }
            public int Amount { get; set; }
            //public string Signature { get; set; }
            public string OrderInfo { get; set; }
            public string Type { get; set; }

            public int ResponseCode { get; set; }

            public string Description { get; set; }

            public string Signature { get; set; }
        }
        public class CashBankRequest
        {

            public string TransId { get; set; }
            public string MomoId { get; set; }
            public string MomoName { get; set; }

            public int Amount { get; set; }
            public string Note { get; set; }
            public string CallbackUrl { get; set; }
        }
        public class CashBankRequestV2
        {
            public string TransId { get; set; } //Transaction cua he thông Pay
            public string BankTransId { get; set; }
            public string BankCode { get; set; }
            public string BankId { get; set; }
            public string BankName { get; set; } // 
            public int Amount { get; set; }
            public string Comment { get; set; }
            public string CallbackUrl { get; set; }
        }
        public class CashRequest
        {
            public string type { get; set; }
            public string stk { get; set; }
            public string bank_type { get; set; }
            public int amount { get; set; }
            public string message { get; set; }
            public string ref_id { get; set; }
            public string receiver { get; set; }

        }
        public class CashMsg
        {
            public string momoTransId { get; set; }
            public string finishTime { get; set; }
            public string userName { get; set; }
            public string accountReceive { get; set; }
            public string accountName { get; set; }
            public int amount { get; set; }
            public string comment { get; set; }
            public string authKey { get; set; }
        }
        public static bool CheckValidMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return false;

            string patternDienThoai = @"^[0]\d{9}$";
            Regex myRegexDienThoai = new Regex(patternDienThoai);

            Match mDienThoai = myRegexDienThoai.Match(mobile);

            if (!mDienThoai.Success)
            {
                return false;
            }

            return true;
        }

        public class CashResponeApi
        {
            public string Status { get; set; }
            public string BankName { get; set; }
            public string BankAccountNumber { get; set; }
            public string BankAccountName { get; set; }
            public int Amount { get; set; }
            public string RefCode { get; set; }
            public string OrderNo { get; set; }
            public int Timeout { get; set; }
        }
        public static string GenOrderCode()
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i <= 8; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            tmp = DateTime.Now.ToString("yyMMddHH") + tmp;
            return tmp.ToUpper();
        }
        public static string PostJson(string uri, string postData)
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
        public static string PostJson(string uri, string postData, string sign)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
                                    //request.Accept = "JSON";
            request.Headers.Add("X-Access-Key", AccessKey);
            request.Headers.Add("X-Signature", sign);

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
        public static async Task<string> CallbackJson(string url, string postData, long Id = 0, string refcode = "")
        {

            NLogLogger.Info(new string[] { "Drum", "Callback", "Partner", "Request", postData });
            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            client.Timeout = TimeSpan.FromSeconds(90);
            try
            {
                var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "Drum", "Callback", "TransId", "Refcode", "Response", Id.ToString(), refcode, responseContent });
                    var log = new LogInfo
                    {
                        LogTime = DateTime.Now,
                        Url = url,
                        TransactionID = Id,
                        Request = postData,
                        Respone = responseContent
                    };
                    LogCache.LogBankCash(log);
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
                        LogCache.LogBankCash(log1);
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
                LogCache.LogBankCash(log);
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
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

                            LogCache.LogBankCash(new LogInfo
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

                        LogCache.LogBankCash(new LogInfo
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

                        LogCache.LogBankCash(new LogInfo
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

                        LogCache.LogBankCash(new LogInfo
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
        public static string GetChatId(string id)
        {
            string partnecode = "";
            switch (id)
            {
                case "jst":
                    partnecode = "-5217120856";
                    break;

                case "go99":
                    partnecode = "-1002191671235";
                    break;
                case "nohu888":
                    partnecode = "-1002627003321";
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
                //case "bp7":
                //    partnecode = "-4820262837";
                //    break;

            };
            return partnecode;
        }
        public static async Task<string> GetTask(string url)
        {
            var uri = new Uri(url);
            HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);

            NLogLogger.Info(new string[] { "VNPAY", "GetTask", url });

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
                NLogLogger.Info(new string[] { "VNPAY", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;
        }
        public static string HmacSha256Digest(string message, string secretKey)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
            return base64String;
        }
        public static int ConvertResponCode(int responseStatus)
        {
            switch (responseStatus)
            {

                case -1:
                    return (int)ResponseCode.TransactionFailed;
                case 1:
                    return (int)ResponseCode.ParameterInvalid;
                case 2:
                    return (int)ResponseCode.BankAccountInvalid;
                case 71:
                case 4010:
                case -999:
                    return (int)ResponseCode.BankAccountInvalid;
                //case -102:
                //    return (int)ResponseCode.AccountNotExists;
                //case -104:
                //    return (int)ResponseCode.LoginFail;
                //case -105:
                //    return (int)ResponseCode.BankCodeInvalid;
                //case -108:
                //    return (int)ResponseCode.BankAmountInvalid;
                //case -109:
                //    return (int)ResponseCode.BankCardInfoInvalid;
                //case -115:
                //    return (int)ResponseCode.TransactionFailed;
                default:
                    return (int)ResponseCode.UndefinedError;
            }
        }
        public static void SendBill(string url)
        {

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                NLogLogger.Info(url);
                var requestUrl = url;
                var webclient = new WebClient();

                webclient.DownloadString(requestUrl);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }
        }

    }
}
