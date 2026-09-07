using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Libs.Utils;

namespace Libs.BankDirect
{
    public class TelegramNotify
    {
        public static string token = ConfigurationManager.AppSettings["TokenTele"];
        public static void SendNotify(int id, string message)
        {
            Task.Run(() => TelegramClient.TelegramSendMessage(id, message));
        }
        public static void SendWarning(string id, string message)
        {
            Task.Run(() => SendTele(id, message));
        }
        public static void SendTele(string id, string message)
        {

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var requestUrl = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id=-100{1}&text={0}", message, id,token);
            var webclient = new WebClient();

            webclient.DownloadString(requestUrl);
            ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        }
        public static void SendTeleV2(string id, string message)
        {
            Task.Run(() => SendTeleV3(id, message));

            ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        }
        public static void SendTeleV3(string id, string message)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var requestUrl = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id={1}&text={0}", message, id, token);
                var webclient = new WebClient();

                webclient.DownloadString(requestUrl);
            }
            catch
            {

            }

        }
        public static void SendTeleV4(string id, string message)
        {
            Task.Run(() => SendTeleV5(id, message));

            ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        }
        //public static void SendTeleV5(string id, string message)
        //{
        //    try
        //    {
        //        ServicePointManager.Expect100Continue = true;
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        //        var requestUrl = string.Format("https://api.telegram.org/bot8881845685:AAHUx2_QaWyiFaNoC0N5w-dles0_qeBuMlU/sendMessage?chat_id={1}&parse_mode=html&text={0}", message, id);
        //        var webclient = new WebClient();

        //        webclient.DownloadString(requestUrl);
        //    }
        //    catch
        //    {

        //    }

        //}
        public static void SendTeleV5(string chatId, string message)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // ⚠️ Không nên tắt kiểm tra chứng chỉ trong môi trường thật
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            // Token nên lưu trong biến môi trường hoặc file cấu hình
            var token1 = token;
          

            //var encodedMessage = WebUtility.UrlEncode(message);
            var requestUrl = $"https://api.telegram.org/bot{token1}/sendMessage?chat_id={chatId}&parse_mode=html&text={message}";

            const int maxRetries = 3;
            const int delayMs = 2000; // chờ 2 giây giữa các lần thử

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        string response = webClient.DownloadString(requestUrl);
                        //Console.WriteLine($"✅ Gửi Telegram thành công (lần {attempt}).");
                        return; // thành công → thoát hàm
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"❌ Lỗi gửi Telegram (lần {attempt}): {ex.Message}");
                }

                if (attempt < maxRetries)
                {
                    //Console.WriteLine($"⏳ Thử lại sau {delayMs / 1000} giây...");
                    Thread.Sleep(delayMs);
                }
            }

            Console.WriteLine("🚫 Gửi Telegram thất bại sau 3 lần thử.");
        }
        public static async Task SendTelegramMessage(string id, string message)
        {
            using (var client = new HttpClient())
            {
                string url = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id={1}&parse_mode=html&text={0}", message, id, token);

                int maxRetries = 3;
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Message sent successfully.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"Failed attempt {attempt}: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogLogger.Info(ex.Message);
                        Console.WriteLine($"Exception attempt {attempt}: {ex.Message}");
                        if (attempt == maxRetries)
                            throw;
                    }

                    await Task.Delay(1000); // Delay 1s trước khi thử lại
                }
            }
        }
        public static void SendTeleFast(string id, string message)
        {
            Task.Run(() => SendTelegramMessageV2(id, message));

            ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        }
        public static async Task SendTelegramMessageV2(string id, string message)
        {
            using (var client = new HttpClient())
            {
                string url = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id={1}&text={0}", message, id, token);

                int maxRetries = 3;
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Message sent successfully.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"Failed attempt {attempt}: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception attempt {attempt}: {ex.Message}");
                        if (attempt == maxRetries)
                            throw;
                    }

                    await Task.Delay(1000); // Delay 1s trước khi thử lại
                }
            }
        }
    }
}