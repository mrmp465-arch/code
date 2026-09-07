using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Libs.Utils;

namespace Libs.BankCash
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

            var requestUrl = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id=-100{1}&text={0}", message, id, token);
            var webclient = new WebClient();

            webclient.DownloadString(requestUrl);
            ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        }
        public static void SendTeleV2(string id, string message)
        {
            Task.Run(() => SendTeleV3(id, message));
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
            ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        }
        public static void SendTeleV4(string id, string message)
        {
            Task.Run(() => SendTeleV5(id, message));

            ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        }
        public static void SendTeleV5(string id, string message)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var requestUrl = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id={1}&parse_mode=html&text={0}", message, id, token);
                var webclient = new WebClient();

                webclient.DownloadString(requestUrl);
            }
            catch
            {

            }

        }
        public static void SendConfirmMessageOld(string chatId, string message, string requestId)
        {
            string BotToken = token;
            string keyboard =
                "{\"inline_keyboard\":[[" +
                "{\"text\":\"✅ Confirm\",\"callback_data\":\"confirm:" + requestId + "\"}," +
                "{\"text\":\"❌ Cancel\",\"callback_data\":\"cancel:" + requestId + "\"}" +
                "]]}";



            string url =
                "https://api.telegram.org/bot" +
                BotToken +
                "/sendMessage";

            using (var client = new WebClient())
            {
                var data = new NameValueCollection();

                data["chat_id"] = chatId;
                data["text"] = message;
                data["reply_markup"] = keyboard;
                data["parse_mode"] = "HTML";
                client.UploadValues(url, "POST", data);
            }
        }
        public static void SendConfirmMessage(string chatId, string message, string requestId)
        {
            Task.Run(() => SendConfirmMessageV2(chatId, message, requestId));
        }
        public static void SendConfirmMessageV2(string chatId, string message, string requestId)
        {
            string BotToken = token;
            string url =
                "https://api.telegram.org/bot" +
                BotToken +
                "/sendMessage";

            string keyboard =
                "{\"inline_keyboard\":[[" +
                "{\"text\":\"✅ 确认 Confirm\",\"callback_data\":\"confirm:" + requestId + "\"}," +
                "{\"text\":\"❌ 取消 Cancel\",\"callback_data\":\"cancel:" + requestId + "\"}" +
                "]]}";

            int maxRetry = 3;

            for (int i = 1; i <= maxRetry; i++)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        var data = new NameValueCollection
                        {
                            ["chat_id"] = chatId,
                            ["text"] = message,
                            ["parse_mode"] = "HTML",
                            ["reply_markup"] = keyboard
                        };

                        byte[] response = client.UploadValues(url, "POST", data);
                        string result = Encoding.UTF8.GetString(response);

                        // Log thành công nếu cần
                        // NLogLogger.Info(new[] { "Telegram", "SendConfirmMessage", result });

                        return; // gửi thành công thì thoát hàm void
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi mỗi lần retry
                    NLogLogger.Info(new[] { "Telegram", "Retry " + i, ex.ToString() });

                    if (i < maxRetry)
                    {
                        Thread.Sleep(i * 2000); // 2s, 4s
                    }
                    else
                    {
                        // Hết retry thì chỉ log, không return bool
                        // NLogLogger.Info(new[] { "Telegram", "SendConfirmMessage Failed", ex.ToString() });
                    }
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