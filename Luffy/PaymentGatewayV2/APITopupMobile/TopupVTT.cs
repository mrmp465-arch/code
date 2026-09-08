using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using FirebaseNet.Messaging;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APITopupMobile
{
    internal class TopupVTT
    {
        private const string ServerApiKey = "AAAAuEdUvsQ:APA91bGF1yohDDTbj7DmpC_bzLRqV-UiFAW3lwrWTS8oYDQziSWzauS5sOv7UwLk3OCerYzHLYtNh4DdalvHfdvIpmeYdIzMtK_eNVHOt78IQKtFTE4wAuR9UdEhlLHa0o2Y3uG8BL7J";
        private string ServerCallBackUrl = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerCallBackUrl"]) ? ConfigurationManager.AppSettings["ServerCallBackUrl"] : "http://35.240.137.60:1583";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public async Task<string> SendUSSDTopupCard(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        {
            var topup = new TopupMobile3rdLog();
            topup.RequestNo = 0;
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.Sim = sim;
            topup.SimTarget = simTarget;
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.ClientId = clientId;
            topup.Slot = slot;
            topup.Amount = 0;
            topup.Add();

            var simUSSD = new SimUSSD().Get(clientId, slot);

            FCMClient client = new FCMClient(ServerApiKey);

            var regIds = new List<string>
            {
                clientId
            };

            DataRequest content;
            if (string.IsNullOrEmpty(simTarget))
            {
                content = new DataRequest()
                {
                    call_id = topup.ReturnValue.ToString(),
                    call_number = "*100*" + pin,
                    call_provider = telco.ToUpper(),
                    call_type = 3,
                    send_message = pin,
                    other_message = serial,
                    call_slot = slot,
                    url = ServerCallBackUrl,
                    quota = Convert.ToInt32(simUSSD.Quota)

                };
            }
            else
            {
                content = new DataRequest()
                {
                    call_id = topup.ReturnValue.ToString(),
                    call_provider = telco,
                    call_type = 2,
                    send_message = pin,
                    other_message = serial,
                    target_Number = simTarget,
                    call_slot = 0,
                    url = ServerCallBackUrl
                };
            }

            //content = new DataRequest()
            //{
            //    call_id = "1",
            //    call_number = "*100*" + pin,
            //    call_provider = "VTT",
            //    call_type = 3,
            //    send_message = pin,
            //    other_message = serial,
            //    call_slot = 0

            //};

            //content = string.Format("{{\"call_data\":\"*100*{0}\",\"need_last_comma\":true,\"result_data\":\"\",\"selected_sim\":0,\"messaging_id\":\"{1}\"}}", pin, topup.ReturnValue);

            var message = new Message()
            {
                RegistrationIds = regIds,
                Priority = MessagePriority.high,
                Notification = new AndroidNotification()
                {
                    Body = "USSD Topup for this Mobile",
                    Title = "USSD Topup",
                    Icon = "myIcon",
                },
                Data = new Dictionary<string, string>
                {
                    { "content", serializer.Serialize(content)}
                }
            };

            NLogLogger.Info(new string[] { "TopupMobile", "TopupMobile", serializer.Serialize(content) });

            await client.SendMessageAsync(message);
            return await CheckStatusAsync(topup.ReturnValue);

        }
        private async Task<string> CheckStatusAsync(long id)
        {
            for (int i = 0; i < 30; i++)
            {
                //Check DB 30 lan tuong ung 30s

                var topup = new TopupMobile3rdLog();
                topup.Id = id;
                var cardRequest = topup.Get();
                if (cardRequest != null)
                    if (cardRequest.Status != 0)
                        return string.Format("{0}|{1}", cardRequest.Status, cardRequest.Amount);
                System.Threading.Thread.Sleep(1000);
            }

            //Update Timeout
            DataRequest.UpdateTopupCard(id, 0, -326, string.Empty, string.Empty); // Update Timeout
            return "-326|0";
        }

        //public int UpdateTopupCard(long id, int amount, int status)
        //{
        //    var topup = new TopupMobile3rdLog();
        //    topup.Id = id;
        //    topup.Amount = amount;
        //    topup.Status = status;
        //    topup.Update();
        //    return topup.ReturnValue;
        //}
    }

   
}