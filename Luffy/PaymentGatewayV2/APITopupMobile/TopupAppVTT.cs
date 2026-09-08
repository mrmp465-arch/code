using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using APITopupMobile.Service;
using FirebaseNet.Messaging;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APITopupMobile
{
    internal class TopupAppVTT
    {
        //private const string ServiceUrl = "http://27.72.63.148/Views/AutoBanTien/ServiceBanTien.asmx";
        private string ServiceUrl = ConfigurationManager.AppSettings["AppAddPrepaidViettel"] ?? "http://118.70.109.146/Views/AutoBanTien/ServiceBanTien.asmx";
        private const string keycode = "lx1902";
        //private string ServerCallBackUrl = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerCallBackUrl"]) ? ConfigurationManager.AppSettings["ServerCallBackUrl"] : "http://35.240.137.60:1583";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public string SendAppVTTTopup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {

            //Lấy ra để Process 

            var topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode);
            if (topupProcess == null)
            {
                var chat_id = -318818065;
                switch (providerCode)
                {
                    case "glbappvtt":
                        chat_id = -315078196;
                        break;
                    case "datappvtt":
                        chat_id = -284948741;
                        break;
                    case "mrxappvtt":
                        chat_id = -155536217;
                        break;
                }

                var tms = Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, string.Format("Có nghi vấn hết đơn hàng {0} hiện tại thẻ mệnh giá {1} không tìm thấy đơn phù hợp. Các anh check giúp em (^_^)", telco, amount)));
                tms.Wait();

                NLogLogger.Info(new string[] { "SendAppVTTTopup", "Order Not Found 1", transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            if (topupProcess.TransactionID == 0)
            {
                NLogLogger.Info(new string[] { "SendAppVTTTopup", "Order Not Found 2", transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            var topupType = Convert.ToInt32(topupProcess.TopupType);
            switch (topupType)
            {
                case 1:
                    topupType = 1;
                    break;
                case 2:
                    topupType = 2;
                    break;
                case 3:
                    topupType = 3;
                    break;
                case 4:
                    topupType = 4;
                    break;
                case 5:
                    topupType = 5;
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = topupProcess.TransactionID; //Id của bảng Order
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.SimTarget = topupProcess.Mobile;
            topup.Add();
            topup.Id = topup.ReturnValue;


            try
            {
                var client = new ServiceBanTien(ServiceUrl);
                NLogLogger.Info(new string[] { "SendAppVTTTopup", "Request", serializer.Serialize(topup) });
                var response = client.AddQueue(topup.Id.ToString(), topupType, topup.CardSerial, topup.CardCode, Convert.ToInt32(topup.Amount), topup.SimTarget, keycode);
                NLogLogger.Info(new string[] { "SendAppVTTTopup", "Response", topup.Id.ToString(), response.ToString() });
                if (response <= 0)
                {
                    NLogLogger.Info(new string[] { "SendAppVTTTopup", "Response failed", topup.Id.ToString() });
                    //Update Service Not Exists
                    topupProcess.Topup(0, 0);
                    topup.Status = (int)ResponseCode.ServiceNotExists;
                    topup.Update();
                    return "-317|0"; // 
                }


            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "SendAppVTTTopup", "Response failed Exeption", topup.Id.ToString(), exp.Message });
                //Update Service Not Exists
                topupProcess.Topup(0, 0);
                topup.Status = (int)ResponseCode.ServiceNotExists;
                topup.Update();
                return "-317|0"; // 
            }
            
            var t = Task.Run(() => CheckStatusAsync(topup.ReturnValue, topupProcess));
            return t.Result;

        }
        private async Task<string> CheckStatusAsync(long id, TopupMobileLog topupMobileLog)
        {
            var topup = new TopupMobile3rdLog();
            for (int i = 0; i < 30; i++)
            {
                //Check DB 30 lan tuong ung 30s
                topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                    {
                        //if (cardRequest.Status == 1)
                        //{
                        //    topupMobileLog.Topup(1, Convert.ToInt32(cardRequest.Amount)); // Thành công update Amount và trạng thái
                        //    return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                        //}
                        //if (cardRequest.Status == -2)
                        //{
                        //    topupMobileLog.Topup(1, Convert.ToInt32(cardRequest.Amount == 0 ? cardRequest.AmountUser : cardRequest.Amount)); // Thành công update Amount và trạng thái
                        //    return string.Format("{0}|{1}", cardRequest.Status, 0);
                        //}
                        //else if (cardRequest.Status == -7) //Bo qua đơn hàng
                        //{
                        //    topupMobileLog.Topup(-1, 0);
                        //}
                        //else
                        //{
                        //    topupMobileLog.Topup(0, 0);
                        //}
                        //if (cardRequest.Status == -7) //Bo qua đơn hàng
                        //{
                        //    topupMobileLog.Topup(-1, 0);
                        //}
                        //else
                        //{
                        //    topupMobileLog.Topup(0, 0);
                        //}
                        //else if (topupMobileLog.Amount > topupMobileLog.AmountTopupSuccess) //Tiep tuc chuyen trang thai de chay
                        //{
                        //    topupMobileLog.Topup(0, 0);
                        //}

                        NLogLogger.Info(new string[] { "SendAppVTTTopup", "CheckDB Sync", id.ToString(), string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount) });
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                    }

                System.Threading.Thread.Sleep(1000);
            }
            //Update Timeout
            //topupMobileLog.Topup(0, 0);
            topup.Status = -326;
            topup.Update();
            NLogLogger.Info(new string[] { "SendAppVTTTopup", "CheckDB Sync", id.ToString(), "-326 | 0" });
            return "-326|0";
        }

    }


}