using Libs.API;
using Libs.Report;
using Libs.Utils;
using System;
using System.Configuration;
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

        static void Main(string[] args)
        {


            //ReportSN();
            //ReportSN();
            //var currenttime = DateTime.Now.AddHours(-12);
            //DateTime fromdate = new DateTime(currenttime.Year, currenttime.Month, currenttime.Day);
            //DateTime todate = new DateTime(currenttime.Year, currenttime.Month, currenttime.Day).AddDays(1).AddMilliseconds(-5);
            //var data = new UserDaily().GetList("", fromdate, todate);

            //ReportSN(data, fromdate, todate);
            //System.Threading.Thread.Sleep(300);
            //ReportB23(data, fromdate, todate);
            //ReportBP7(data, fromdate, todate);
            //ReportTMTM(data, fromdate, todate);

            //NLogLogger.Info("abc");
            ReporOtp();
            //CheckSuccess();
        }
        static void CheckSuccess()
        {
            var result = DataCaching.GetCache<string>("LastOrderSuccess");
            if (result == null)
            {

                SendTeleV3("-5375462347", "Không có giao dịch thành công trong 5 phút=> ae check lại nhé");
            }

        }
        static void ReporOtp()
        {
            try
            {
                string groupId = ConfigurationManager.AppSettings["GroupId"];
                //NLogLogger.Info("abc");
                var _Bank = new BankAccounts();
                var data = _Bank.GetList().Where(x => x.Status == 1 && x.StatusExtra != -4 && x.BankCode == "VPB").ToList();

                if (data != null)
                {
                    foreach (var bank in data)
                    {
                        if (!string.IsNullOrEmpty(bank.AppDeviceId))
                        {

                            var key = string.Format("OTP:{0}", bank.AppDeviceId);
                            // NLogLogger.Info(key);
                            var result = OTPDataCaching.GetCache<OTPRequest>(key);
                            if (result == null)
                            {

                                SendTeleV3(groupId, "[OTP] Tài khoản VPB:  " + bank.BankName + " phone " + bank.PhoneDevice + " không có OTP => Ae tắt app đi bật lại. pin OTP " + bank.PinOtp);
                            }
                        }

                    }
                }
                //if (DateTime.Now.Hour == 9)
                //{
                //    if (DateTime.Now.Minute <= 2)
                //    {
                //        var bank = new BankAccounts().Get("100884883297", "ICB");
                //        if (bank.Status == 1)
                //            bank.StopScanAt_Update(bank.Id, DateTime.MaxValue);
                //    }
                //}
                //if (DateTime.Now.Hour == 22)
                //{
                //    if (DateTime.Now.Minute == 51 || DateTime.Now.Minute == 52)
                //    {
                //        var bank = new BankAccounts().Get("100884883297", "ICB");
                //        if (bank.Status == 1)
                //            bank.StopScanAt_Update(bank.Id, DateTime.Now.AddMinutes(10));
                //    }
                //}
                if (DateTime.Now.Hour == 0 || DateTime.Now.Hour == 3 || DateTime.Now.Hour == 6 || DateTime.Now.Hour == 9 || DateTime.Now.Hour == 12 | DateTime.Now.Hour == 15 || DateTime.Now.Hour == 18 || DateTime.Now.Hour == 21)
                {
                    //if (DateTime.Now.Minute==0)
                    //{
                    //    var user = new Users().GetByUserName("cn02");
                    //    if (user != null)
                    //        SendTeleV3("-5062147933", "Balance:  " + user.Balance.ToString("N0").Replace(".", ","));

                    //}



                }
                if (DateTime.Now.Minute == 0)
                {
                    var data2 = _Bank.GetList().Where(x => x.Status == 1 && x.StatusExtra != -4 && x.BankCode == "NAB").ToList();
                    foreach (var bank in data2)
                    {
                        NotifyBank(bank.BankId, bank.BankCode);
                    }
                }
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }
        }
        public class RequestData
        {
            public string PartnerCode { get; set; }
            public string CommandCode { get; set; }
            public string RequestContent { get; set; }
            public string Signature { get; set; }
        }
        public static void NotifyBank(string BankId, string BankCode)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var obj = new { BankCode = BankCode, BankId = BankId };

            var requesData = new RequestData()
            {
                CommandCode = "TRANS_SCAN",
                RequestContent = BankCode + "," + BankId
            };
            var UrlBaseService = "http://127.0.0.1:9002/BankService.ashx";
            var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
        }
        public static async Task<string> CallbackJson(string url, string postData)
        {
            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(300);
            try
            {
                NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Request", postData });
                var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Response", responseContent });
                    client.Dispose();
                    return responseContent;
                }
                else
                {
                    NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Response Is Null" });
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }
        public class OTPRequest
        {
            public string pin { get; set; }
            public string state { get; set; }
            public string otp { get; set; }
            public int time { get; set; }
            public string extra { get; set; }
            public string timeCreate { get; set; }

        }
        static string formatData(long input)
        {
            var output = input.ToString("#,#").Replace(".", ",");

            if (string.IsNullOrEmpty(output))
                output = "0";
            return output;
        }
        static void ReportPartner(string partnercode, List<UserDaily> lstData)
        {
            try
            {
                if (lstData.Exists(x => x.UserName == partnercode))
                {
                    var data = lstData.FirstOrDefault(x => x.UserName == partnercode);
                    string bankin1 = formatData(data.AmountBankin);
                    string bankin2 = formatData(data.TotalBankIn);
                    string bankout1 = formatData(data.AmountBankout);
                    string bankout2 = formatData(data.TotalBankOut);
                    string TotalRecharge = formatData(data.TotalRecharge);
                    string TotalCash = formatData(data.TotalCash);
                    string BalanceAfter = formatData(data.BalanceAfter);
                    string BalanceBefore = formatData(data.BalanceBefore);
                    long total = data.TotalBankIn - data.TotalBankOut;
                    int col1Width = 8;   // Payment
                    int col2Width = 12;  // Số lượng
                    int col3Width = 12;  // Thành tiền

                    // Hàm dựng 1 dòng
                    string Row(string c1, string c2, string c3) =>
                        $" |{c1.PadRight(col1Width)}|{c2.PadLeft(col2Width)}|{c3.PadLeft(col3Width)}| ";

                    string text = $@"<pre>
          ========= Report {DateTime.Now.AddHours(-1).ToString("dd/MM/yyyy")} ========= 
         = Partner: {partnercode}
         {new string('-', col1Width + col2Width + col3Width + 4)}
         {Row("Payment", "Số lượng", "Thành tiền")}
         {new string('-', col1Width + col2Width + col3Width + 4)}
         {Row("BankIn", bankin1, bankin2)}
         {new string('-', col1Width + col2Width + col3Width + 4)}
         {Row("BankOut", bankout1, bankout2)}
         {new string('-', col1Width + col2Width + col3Width + 4)}
         = Tổng: {formatData(total)}

         Số dư đầu: {BalanceBefore}
         Rút số dư trong ngày: {TotalCash}
         Nạp số dư trong ngày: {TotalRecharge}
         Số dư cuối:{BalanceAfter}

        </pre>";
                    NLogLogger.Info(text);
                    var chatId = GetChatId(partnercode);
                    SendTeleV3(chatId, text);
                }

            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }

        }
        public static string GetChatId(string id)
        {
            string partnecode = "";
            switch (id)
            {
                case "sgm":
                    partnecode = "-5086735436";
                    break;



                    //case "bp7":
                    //    partnecode = "-4820262837";
                    //    break;

            };
            return partnecode;
        }

        static void UpdateMomo()
        {
            if (DateTime.Now.Hour == 1 || DateTime.Now.Hour == 7 || DateTime.Now.Hour == 13 || DateTime.Now.Hour == 20)
            {

            }
        }

        public static void SendTeleV3(string id, string message)
        {
            try
            {
                string token = ConfigurationManager.AppSettings["TokenTele"];

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var requestUrl = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id={1}&parse_mode=html&text={0}", message, id, token);
                var webclient = new WebClient();

                // NLogLogger.Info(requestUrl);
                webclient.DownloadString(requestUrl);
            }
            catch (Exception e)
            {
                NLogLogger.Info(e.Message);
            }

        }


    }
}
