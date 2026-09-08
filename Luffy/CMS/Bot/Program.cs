using Card.Utility;
using Libs.API;
using Libs.Report;

using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Bot
{
    class Program
    {
        private const string urlBaseService = "http://mopay2.vnm.bz:10007/api/";
        private const string urlBaseService2 = "http://imopay.vnm.bz:10007/api/";

        private const string Apikey = "136bf507-eb74-4edf-a3dc-a9a01c58c35c";

        private const string Apikey2 = "0f43db36-1aa4-4389-8f03-5218912feed9";

        private const string ApiSecret = "113355a@";
        static void Main(string[] args)
        {
            //ReportPanda();

            UpdateBank();

        }
        public class BankAccount
        {

            //public string AccountNumber { get; set; }
            //public string AccountName { get; set; }
            public string BankCode { get; set; }
            public string DisplayName { get; set; }
        }
        static void UpdateBank()
        {
            var result = GetBank();
            if(result!=null)
            {
                result = result.Where(x => x.BankCode != "VCB").ToList();
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

                Card.Utility.RedisCaching.Add("ImoBanks", javaScriptSerializer.Serialize(result), 86400 * 5);
            }    
                
        }
        public static List<BankAccount> GetBank()
        {

            string KeyCache = "ImoBanks";
            var cachedata = Card.Utility.RedisCaching.GetData(KeyCache);
            if (cachedata == null)
            {

                return null;
            }
            else
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                return javaScriptSerializer.Deserialize<List<BankAccount>>(cachedata.ToString());
            }
        }
        static void ReportPanda()
        {
            if (DateTime.Now.Minute == 59)
            {

                if (DateTime.Now.Hour == 23 || DateTime.Now.Hour == 8 || DateTime.Now.Hour == 11 || DateTime.Now.Hour == 14 || DateTime.Now.Hour == 17 || DateTime.Now.Hour == 20)
                {
                    DateTime fromdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    DateTime todate = DateTime.Now;
                    BankGateAPI _CardAPILog = new BankGateAPI();
                    var Data = _CardAPILog.ReportDoiSoat("panda,panpan", "", "", fromdate, todate, 1);
                    if (Data != null)
                    {
                        TelegramNotify.SendTele(-4081312824, $"Báo cáo {DateTime.Now.Hour + 1} giờ");
                        if (Data.Exists(x => x.Type == 2))
                        {
                            TelegramNotify.SendTele(-4081312824, $"Nạp bank : {Data.Where(x => x.Type == 2).Sum(x => x.ReturnTotalValue).ToString("N0")}");
                        }
                        if (Data.Exists(x => x.Type == 1))
                        {
                            TelegramNotify.SendTele(-4081312824, $"Nạp momo : {Data.Where(x => x.Type == 1).Sum(x => x.ReturnTotalValue).ToString("N0")}");
                        }
                        if (Data.Exists(x => x.Type == 3))
                        {
                            TelegramNotify.SendTele(-4081312824, $"Nạp vtp : {Data.Where(x => x.Type == 3).Sum(x => x.ReturnTotalValue).ToString("N0")}");
                        }
                    }
                    BankCashAPI _cash = new BankCashAPI();
                    try
                    {
                        var Datacash = _cash.ReportDoiSoat("panda,panpan", "", "", fromdate, todate, 1);
                        if (Datacash != null)
                        {
                            if (Datacash.Exists(x => x.Type == 2))
                            {
                                TelegramNotify.SendTele(-4081312824, $"Rút bank : {Datacash.Where(x => x.Type == 2).Sum(x => x.ReturnTotalValue).ToString("N0")}");
                            }
                            if (Datacash.Exists(x => x.Type == 1))
                            {
                                TelegramNotify.SendTele(-4081312824, $"Rút momo : {Datacash.Where(x => x.Type == 1).Sum(x => x.ReturnTotalValue).ToString("N0")}");
                            }

                        }
                    }
                    catch
                    {

                    }
                   
                }

            }

        }
        static void GetCHZ()
        {
            var data = download();
            if (data.Contains("$SCCP"))
            {
                TelegramNotify.SendTele(-992709124, "Buy SCCP");
            }
            if (data.Contains("$EFC"))
            {
                TelegramNotify.SendTele(-992709124, "Buy Everton");
            }
            if (data.Contains("$LUFC"))
            {
                TelegramNotify.SendTele(-992709124, "Buy Leeds");
            }
            if (data.Contains("$SPFC"))
            {
                TelegramNotify.SendTele(-992709124, "Buy Sao Paulo");
            }
        }
        static string download()
        {
            var response = Task.Run(async () => await GetTask("https://explorer.chiliz.com/address/0x6F4557853Cab0F6fFB69d5e66696275c3e41a33D/token-transfers?type=JSON")).Result;


            //var client = new RestClient("https://explorer.chiliz.com/address/0x6F4557853Cab0F6fFB69d5e66696275c3e41a33D/token-transfers?type=JSON");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.GET);
            //request.AddParameter("text/plain", "", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);
            // Console.WriteLine(response);
            //NLogLogger.DebugMessage(response);

            return response;
        }
        static void RequestBankV2()
        {
            var lstBank = GetBanksV2();
            if (lstBank != null)
            {

                foreach (var bank in lstBank)
                {
                    if (bank.code != "mb")
                    {
                        var requestId = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var urlService = "https://api.ipay.vin/api?c=RegCharge&apiKey=93d70008-09e7-4fbd-af01-5dd67c736b48&chargeType=bank&amount=50000&requestId=" + requestId + "&subType=" + bank.code;

                        var response = Task.Run(async () => await GetTask(urlService)).Result;
                    }
                }
            }
        }
        static void RequestBank()
        {
            var requestId = DateTime.Now.ToString("yyyyMMddHHmmss");
            var urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType=bank&amount=50000&requestId={requestId}&subType=BIDV";
            var response = Task.Run(async () => await GetTask(urlService)).Result;
            //var lstBank = GetBanks();
            //if (lstBank != null)
            //{

            //    foreach (var bank in lstBank)
            //    {
            //        if (bank.code != "MB")
            //        {
            //            var requestId = DateTime.Now.ToString("yyyyMMddHHmmss");
            //            var urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType=bank&amount=50000&requestId={requestId}&subType={bank.code}";
            //            var response = Task.Run(async () => await GetTask(urlService)).Result;
            //        }
            //    }
            //}
        }
        static void RequestBankV3()
        {
            var requestId = DateTime.Now.ToString("yyyyMMddHHmmss");
            var sign = Libs.Utils.Encrypts.MD5("50000bank" + requestId + ApiSecret);
            var urlService = $"{urlBaseService2}MM/RegCharge?apiKey={Apikey2}&chargeType=bank&amount=50000&requestId={requestId}&subType=BIDV";
            var response = Task.Run(async () => await GetTask(urlService)).Result;
            //var lstBank = GetBanks();
            //if (lstBank != null)
            //{

            //    foreach (var bank in lstBank)
            //    {
            //        if (bank.code != "MB")
            //        {
            //            var requestId = DateTime.Now.ToString("yyyyMMddHHmmss");
            //            var urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType=bank&amount=50000&requestId={requestId}&subType={bank.code}";
            //            var response = Task.Run(async () => await GetTask(urlService)).Result;
            //        }
            //    }
            //}
        }
        public static List<BankResponseData> GetBanksV2()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var urlService = "https://api.ipay.vin/api?c=GetBankAvailable&apiKey=93d70008-09e7-4fbd-af01-5dd67c736b48";
            var response = Task.Run(async () => await GetTask(urlService)).Result;
            if (!string.IsNullOrEmpty(response))
            {

                var resObj = serializer.Deserialize<BankResponse>(response);
                return resObj.data;
            }
            return null;
        }
        public static List<BankResponseData> GetBanks()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var urlService = urlBaseService + "Bank/getBankAvailable?apiKey=" + Apikey;
            var response = Task.Run(async () => await GetTask(urlService)).Result;
            if (!string.IsNullOrEmpty(response))
            {

                var resObj = serializer.Deserialize<BankResponse>(response);
                return resObj.data;
            }
            return null;
        }
        public class BankResponse
        {
            public int stt { get; set; }
            public string msg { get; set; }
            public List<BankResponseData> data { get; set; }
        }
        public class BankResponseData
        {
            public string code { get; set; }
            public string name { get; set; }
        }
        public static async Task<string> GetTask(string url)
        {
            var uri = new Uri(url);
            HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);

            //NLogLogger.Info(new string[] { "VNPAY", "GetTask", url });

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

            }
            client.Dispose();
            return string.Empty;
        }
        static void GetMess(long chatid)
        {
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            var data = TelegramNotify.GetTele();
            var listMessage = data.result.Where(x => x.message != null).ToList();
            listMessage = listMessage.Where(x => x.message.chat.id == chatid).ToList();
            if (listMessage.Exists(x => x.message.time.AddSeconds(61) >= DateTime.Now))
            {
                foreach (var item in listMessage.Where(x => x.message.time.AddSeconds(61) >= DateTime.Now))
                {
                    if (item.message.text != null)
                    {
                        var seri = item.message.text.ToLower().Replace("/seri", "");
                        seri = StringUtils.RemoveNonNumeric(seri);

                        seri = seri.Replace(" ", "");
                        seri = seri.TrimStart();
                        seri = seri.TrimEnd();
                        if (!string.IsNullOrEmpty(seri))
                            CheckSeri(seri, chatid);
                    }

                }
            }


            //Console.WriteLine(serializer.Serialize(lastMessage));
        }
        static void CheckSeri(string seri, long chatid)
        {
            if (seri.Length < 5 || seri.Length > 18)
                return;
            var lstSeri = TelegramNotify.GetSeri();
            if (!lstSeri.Contains(seri))
            {
                TelegramNotify.SetSeri(seri);
                var _CardAPILog = new CardAPILog().GetBySeri(seri);
                if (_CardAPILog == null)
                {
                    TelegramNotify.SendTele(chatid, String.Format("Seri {0} không sang bên mình, vui lòng kiểm tra lại", seri));
                    return;
                }
                if (_CardAPILog.Status > 0)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var amount = Math.Min(_CardAPILog.Amount, _CardAPILog.AmountUser);
                    TelegramNotify.SendTele(chatid, String.Format("Seri {0} nạp thành công, mệnh giá thật :{2} , mệnh giá khai báo : {3}, mệnh giá chốt : {1} , đã callback lại", seri, amount, _CardAPILog.Amount, _CardAPILog.AmountUser));
                    var privateKey = new Partners().Get(_CardAPILog.PartnerCode).PrivateKey;
                    if (!string.IsNullOrEmpty(_CardAPILog.CallbackUrl))
                    {
                        var datacb = new DataCallbackCard()
                        {
                            Amount = amount,
                            RefCode = _CardAPILog.RequestNo,
                            Status = 1,
                            Signature = Libs.Utils.Encrypts.MD5(_CardAPILog.RequestNo + _CardAPILog.Status + amount + privateKey)
                        };
                        Task.Run(() => CallbackJson(_CardAPILog.CallbackUrl, serializer.Serialize(datacb)).ConfigureAwait(false));
                    }
                }
                else
                {
                    switch (_CardAPILog.Status)
                    {
                        case -334:
                            TelegramNotify.SendTele(chatid, String.Format("Seri {0} nhập sai định dạng seri, vui lòng kiểm tra lại", seri));
                            break;
                        case -335:
                            TelegramNotify.SendTele(chatid, String.Format("Seri {0} nhập mã thẻ sai, vui lòng kiểm tra lại mã thẻ", seri));
                            break;
                        case -7:
                            TelegramNotify.SendTele(chatid, String.Format("Seri {0} đã được sử dụng trước khi vào bên mình", seri));
                            break;
                        default:
                            TelegramNotify.SendTele(chatid, "Seri " + seri + "  cần check tay. bạn vui lòng đợi suport bên mình gọi tổng đài");
                            //TelegramNotify.SendTele(-1001686543415, String.Format("@yoichisagichi @cavendish1368 @meliodassupport Seri {0} Provider {1}  Status {2} cần check tay", seri, _CardAPILog.Provider, _CardAPILog.Status));
                            if (_CardAPILog.Provider.Contains("ship"))
                            {
                                TelegramNotify.SendTele(-659770536, String.Format("@xoainon2k Check giúp mình Seri {0} ", seri));
                            }
                            else
                            {
                                TelegramNotify.SendTele(-921899167, String.Format("Check giúp mình Seri {0} ", seri));
                            }

                            break;
                    }
                }

            }
            else
            {
                TelegramNotify.SendTele(chatid, String.Format("Seri {0} đã check phía bên trên. Vui lòng xem lại", seri));
            }
        }
        public static async Task<string> CallbackJson(string url, string postData)
        {

            NLogLogger.Info("CallbackJson Callback Partner Request " + postData);
            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            client.Timeout = TimeSpan.FromSeconds(60);
            try
            {
                var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info("CMS Callback Partner Response" + responseContent);
                    client.Dispose();
                    return responseContent;
                }
                else
                {
                    NLogLogger.Info("CMS Callback Partner Response Is Null");
                }

            }
            catch (Exception e)
            {

                NLogLogger.Info("CMS Exeption Post" + e.Message);
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }
        public class DataCallbackCard
        {

            public long Amount { get; set; }
            public string RefCode { get; set; }
            public int Status { get; set; }
            public string Content { get; set; }
            public string Signature { get; set; }


        }
    }
}
