using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Libs.Utils;

namespace APIMyMobi
{
    public class TelegramNotify
    {
        public static void SendNotify(string providerCode, string userName, string mobile, string telco, int amount, int notifyType, string serial = "", int errorCode = 0, string coreEngine = "")
        {
            List<int> chat_id = new List<int>();

            if (string.IsNullOrEmpty(userName))
            {
                switch (providerCode)
                {
                    case "thuyappmobi":
                        chat_id.Add(-301219310);
                        break;
                    case "shoappmobi":
                        chat_id.Add(-243126431); //Thanh
                        chat_id.Add(-301219310); //Thuy
                        break;
                    case "vanappmobi":
                        chat_id.Add(-225466378);
                        break;
                    case "htoappmobi":
                        chat_id.Add(-493607290);
                        break;
                    default:
                        chat_id.Add(-400169942);
                        break;
                }
            }
            else
            {
                switch (userName)
                {
                    //case "glbappvtt":
                    //    chat_id = -315078196;
                    //    break;
                    //case "datappvtt":
                    //    chat_id = -284948741;
                    //    break;
                    //case "mrxappvtt":
                    //    chat_id = -155536217;
                    //    break;
                    case "thuy_dl1":
                    case "thuy_dl1_api":
                        chat_id.Add(-301219310);
                        break;
                    //case "kenappvtt":
                    //    chat_id = -211343140;
                    //    break;
                    case "sho_dl1":
                    case "sho_dl1_api":
                        chat_id.Add(-243126431);
                        break;
                    //case "htoappvtt":
                    //    chat_id = -277049167;
                    //    break;
                    case "van_dl1":
                    case "van_dl1_api":
                        chat_id.Add(-225466378);
                        break;
                    case "hto_dl1":
                        chat_id.Add(-493607290);
                        break;
                    //case "bigappvtt":
                    //    chat_id = -306296234;
                    //    break;
                    //case "mraappvtt":
                    //    chat_id = -290598105;
                    //    break;
                    default:
                        chat_id.Add(-400169942);
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
                            Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Giao dịch của mạng {0} lỗi không thể Handler được với mã ({1}). Các anh check lại giúp (^_^)", telco, errorCode)));
                        }
                        break;

                    case -303:
                        foreach (var id in chat_id)
                        {
                            Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Giao dịch trên CoreEngine {2}, của mạng {0} lỗi không thể xửa lý được với mã ({1}) quá số lần Retry. Các anh check lại giúp (^_^)", telco, errorCode, coreEngine)));
                        }
                        break;

                        //case -401:
                        //    foreach (var id in chat_id)
                        //    {
                        //        Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Giao dịch của mạng {0} lỗi không thể Handler được với mã ({1}). Các anh check lại giúp (^_^)", telco, errorCode)));
                        //    }
                        //    break;
                        //case -310:
                        //    foreach (var id in chat_id)
                        //    {
                        //        Task.Run(() => TelegramClient.TelegramSendMessage(id, string.Format("Giao dịch của mạng {0} lỗi không thể Handler được với mã ({1}). Các anh check lại giúp (^_^)", telco, errorCode)));
                        //    }
                        //    break;


                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TelegramNotify", "Error", "Send Telegram", chat_id.ToString(), e.Message });
            }
        }
    }
}