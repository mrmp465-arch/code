using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;


namespace Libs.Utils
{


    public class TelegramClient
    {
        private int api_id = 462366;
        private string api_hash = "2a89239ffb313420d9dc54a0c1ab84cd";
        const string tokent = "8010825769:AAEbExMZtB9twOsAb8uyjM6noeWfWlPm5yg";

        public static string token = ConfigurationManager.AppSettings["TokenTele"];
        public TelegramClient()
        {
            var botClient = new TelegramBotClient(tokent);
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );
        }

        public static async Task TelegramSendMessage(int chatId, string messageSend)
        {
            var botClient = new TelegramBotClient(tokent);
            await botClient.SendTextMessageAsync(
                //chatId: -231681139,
                chatId: chatId,
                text: messageSend
            );

            NLogLogger.Info(new string[] { "Send Telegram", chatId.ToString() });

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

                var requestUrl = string.Format("https://api.telegram.org/bot{2}/sendMessage?chat_id={1}&parse_mode=html&text={0}", message, id, token);
                var webclient = new WebClient();
                NLogLogger.Info(requestUrl);
                var result=webclient.DownloadString(requestUrl);
                //NLogLogger.Info(result);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }

        }
    }
}
