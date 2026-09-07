using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Libs.Utils;

namespace APIMobiNext
{
    public class TelegramNotify
    {
        public static void SendNotify(string providerCode, string mobile, string telco, int amount, int notifyType)
        {
            var chat_id = -318818065;
            switch (providerCode)
            {
                case "datappmobi":
                    chat_id = -284948741;
                    break;
                case "htoappmobi":
                    chat_id = -277049167;
                    break;
                case "vanappmobi":
                    chat_id = -225466378;
                    break;
                case "hocappmobi":
                    chat_id = -197438228;
                    break;
                case "bigappmobi":
                    chat_id = -306296234;
                    break;
                case "mrxappmobi":
                    chat_id = -367583747;
                    break;
                case "shoappmobi":
                    chat_id = -243126431;
                    break;
                case "thuyappmobi":
                    chat_id = -301219310;
                    break;
            }

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
                        tms = Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, string.Format("Thuê bao {0} của mạng {1} đã bị vượt quá giới hạn trong ngày hệ thống tự động khóa. Các anh check đơn mở lại giúp em vào 0h ngày hôm sau (^_^)", mobile, telco)));
                        tms.Wait();
                        break;
                    case 4:
                        tms = Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, string.Format("Thuê bao {0} của mạng {1} đã nạp đủ anh chị thay sim trong thiết bị giúp em (^_^)", mobile, telco)));
                        tms.Wait();
                        break;

                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupCallbackMobiNext", "Error", "Send Telegram", chat_id.ToString(), e.Message });
            }
        }
    }
}