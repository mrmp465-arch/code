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

           
            ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        }
    }
}