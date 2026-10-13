using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            var requestUrl = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id=-100{1}&text={0}", message, id, token);
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
        public static void SendTeleV5(string id, string message)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var requestUrl = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id={1}&text={0}&parse_mode=html", message, id, token);
                var webclient = new WebClient();

                webclient.DownloadString(requestUrl);
            }
            catch
            {

            }

        }
        public static async Task SendTelegramMessage(string id, string message)
        {
            using (var client = new HttpClient())
            {
                string url = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id={1}&text={0}&parse_mode=html", message, id, token);

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