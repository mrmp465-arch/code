using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Libs.Utils;

namespace Khoai
{
    public class TelegramNotify
    {
        public static void SendNotify(int id, string message)
        {
            Task.Run(() => TelegramClient.TelegramSendMessage(id, message));
        }
        public static void SendWarning(string id, string message)
        {
            Task.Run(() => SendTele( id, message));
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