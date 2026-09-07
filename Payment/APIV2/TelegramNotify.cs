using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Libs.Utils;

namespace APIV2
{
    public class TelegramNotify
    {
        public static void SendNotify(string providerCode, string mobile, string telco, int amount, int notifyType)
        {
            var chat_id = -845553760;
            //switch (providerCode)
            //{
            //    case "glbappvtt":
            //        chat_id = -315078196;
            //        break;
            //    case "datappvtt":
            //        chat_id = -284948741;
            //        break;
            //    case "mrxappvtt":
            //        chat_id = -155536217;
            //        break;
            //    case "thuyappvtt":
            //        chat_id = -301219310;
            //        break;
            //    case "kenappvtt":
            //        chat_id = -211343140;
            //        break;
            //    case "shoappvtt":
            //        chat_id = -243126431;
            //        break;
            //    case "htoappvtt":
            //        chat_id = -277049167;
            //        break;
            //    case "vanappvtt":
            //        chat_id = -225466378;
            //        break;
            //    case "hocappvtt":
            //        chat_id = -197438228;
            //        break;
            //    case "bigappvtt":
            //        chat_id = -306296234;
            //        break;
            //    case "mraappvtt":
            //        chat_id = -290598105;
            //        break;

            //}

            try
            {
                Task tms;
                switch (notifyType)
                {
                    case 1:
                        tms = Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, string.Format("Có nghi vấn hết đơn hàng {0} hiện tại thẻ mệnh giá {1} không tìm thấy đơn phù hợp. Các anh check giúp em (^_^)", telco, amount)));
                        tms.Wait();
                        break;
                    case 2:
                        tms = Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, string.Format("Thuê bao {0} của mạng {1} đã bị Telco khóa nạp. Các anh check đơn giúp em (^_^)", mobile, telco)));
                        tms.Wait();
                        break;
                    case 3:
                        tms = Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, string.Format("Pool {0} vừa được Recycle. Các anh check hệ thống giúp em (^_^)", telco)));
                        tms.Wait();
                        break;
                    case 4:
                        tms = Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, string.Format("{0} {1}. Các anh check hệ thống giúp em (^_^)", telco, providerCode)));
                        tms.Wait();
                        break;

                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VPGUtils", "Error", "Send Telegram", chat_id.ToString(), e.Message });
            }
        }
    }
}