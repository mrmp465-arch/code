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


namespace Libs.Utils
{


    public class TelegramClient
    {
        private int api_id = 462366;
        private string api_hash = "2a89239ffb313420d9dc54a0c1ab84cd";
        const string tokent = "641419717:AAE-s-_e5QonVa1oa7xgkbDED6NnctneunU";
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
        public static void SendWarning(string id, string message)
        {
            Task.Run(() => SendTele(id, message));
        }
        public static void SendTele(string id, string message)
        {

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var requestUrl = string.Format("https://api.telegram.org/bot1400701792:AAHnR8euw6c4l_gHQLZgg_GZcRByHEG1VR4/sendMessage?chat_id=-100{1}&text={0}", message, id);
            var webclient = new WebClient();

            webclient.DownloadString(requestUrl);
            ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        }
    }
}
