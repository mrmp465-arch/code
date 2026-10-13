using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Libs.Utils;

namespace APIGame
{
    public class TelegramNotify
    {
        public static void SendNotify(string providerCode, string userName, string mobile, string telco, int amount, int notifyType, string serial = "", int errorCode = 0)
        {
            List<int> chat_id = new List<int>();


            if (string.IsNullOrEmpty(userName))
            {
                switch (providerCode)
                {

                    case "shovtc":
                    case "shogarena":
                    case "shozing":
                    case "shodzo":
                    case "shogosu":
                        chat_id.Add(-243126431); //Thanh
                        //chat_id.Add(-301219310); //Thuy
                        //chat_id.Add(-225466378); //Van
                        break;
                    default:
                        chat_id.Add(-318818065);
                        break;
                }
            }
            else
            {
                switch (userName)
                {
                    
                    case "sho_dl1":
                    case "sho_dl1_api":
                        chat_id.Add(-243126431);
                        break;
                    default:
                        chat_id.Add(-318818065);
                        break;
                }
            }



            try
            {
                switch (notifyType)
                {
                    case 1:
                        foreach (var id in chat_id)
                        {
                            Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Có nghi vấn hết đơn hàng {0} hiện tại thẻ mệnh giá {1} không tìm thấy đơn phù hợp (-320). Các anh check giúp em (^_^)", telco, amount)));
                        }
                        break;
                    case 2:
                        foreach (var id in chat_id)
                        {
                            Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Thuê bao {0} của mạng {1} đã bị Telco khóa nạp (-314). Các anh check đơn giúp em (^_^)", mobile, telco)));
                        }
                        break;
                    case 3:
                        foreach (var id in chat_id)
                        {
                            Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Pool {0} vừa được Recycle. Các anh check hệ thống giúp em (^_^)", telco)));
                        }
                        break;
                    case -2:
                        foreach (var id in chat_id)
                        {
                            Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Giao dịch mã thẻ {0} của mạng {1} đã bị nghi vấn (-2). Các anh check review lại giúp (^_^)", serial, telco)));
                        }
                        break;
                    case -55:
                        foreach (var id in chat_id)
                        {
                            Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Giao dịch của mạng {0} lỗi không login được với mã (-55). Các anh check lại giúp (^_^)", telco)));
                        }
                        break;
                    case -99:
                        foreach (var id in chat_id)
                        {
                            Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Giao dịch mã thẻ {0} vào thuê bao {1} của mạng {2} lỗi không thể Handler được với mã ({3}). Các anh check lại giúp (^_^)", serial, mobile, telco, errorCode)));
                        }
                        break;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TelegramNotify", "Error", "Send Telegram", chat_id.ToString(), e.Message });
            }
        }
    }
}