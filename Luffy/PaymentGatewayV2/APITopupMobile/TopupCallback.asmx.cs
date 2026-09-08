using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Services;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APITopupMobile
{
    /// <summary>
    /// Summary description for TopupCallback
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TopupCallback : System.Web.Services.WebService
    {

        [WebMethod]

        public string Callback(string messageId, int amount, int status, string logContent, string sim, string sign)
        {
            if (!string.IsNullOrEmpty(messageId))

            {

                //Boc content : "[Carrier info, Ma so the cao khong hop le hoac da duoc su dung. Xin quy khach vui long thu lai sau!, OK]"
                //              "[Carrier info, Tai khoan cua Quy khach la 10000 dong., OK]"
                NLogLogger.Info(new string[] { "TopupCallback USSD", "Request", messageId, amount.ToString(), status.ToString() });
                
                var messageDb = DataRequest.GetTopupCardLog(messageId);
                if (messageDb != null)
                {
                    var signature = Encrypts.MD5(string.Format("{0}|{1}|{2}|{3}", messageId, amount, status, messageDb.ClientId));
                    if (signature == sign)
                    {
                        var simUSSD = new SimUSSD().Get(messageDb.ClientId, messageDb.Slot);
                        if (status == (int)ResponseCode.TransactionSuccessful)
                        {
                            if ((messageDb.Status != 0) && (status == 1 || status == -4))
                            {
                                NLogLogger.Info(new string[] { "TopupCallback USSD", "Request", "Transaction Suspicious", messageId, amount.ToString(), status.ToString() });
                                //var topupmobile = new TopupMobileLog();
                                //topupmobile.TransactionID = Convert.ToInt64(messageDb.RequestNo);
                                //topupmobile.Topup(1, amount);

                                if (simUSSD.Quota > 0)
                                {
                                   new SimUSSD().UpdateAmout(messageDb.ClientId, messageDb.Slot, amount);
                                }

                                var cardAPILog = new CardAPILog();
                                cardAPILog.TransactionID = messageDb.TransactionId;
                                cardAPILog.Status = (int)ResponseCode.TransactionSuspicious;
                                cardAPILog.Amount = amount;
                                cardAPILog.Description = "Callback";
                                cardAPILog.Update();
                                status = (int)ResponseCode.TransactionSuspicious;
                            }

                        }

                        var result = DataRequest.UpdateTopupCard(Convert.ToInt64(messageId), amount, status, logContent, sim);
                        //NLogLogger.Info(new string[] { "TopupCallback", "Response", messageId, amount.ToString(), status.ToString() });

                        if (result == 0) return "{\"status\":1,\"message\":\"Callback success\"}";
                    }
                    else
                    {
                        return "{\"status\":-1,\"message\":\"Callback failed, Sign invalid\"}";
                    }
                }

            }
            //NLogLogger.Info(new string[] { "TopupCallback", "Response", messageId, amount.ToString(), status.ToString() });
            return "{\"status\":0,\"message\":\"Callback failed\"}";
        }

        [WebMethod]
        public string SimStatus(string clientId)
        {
            return DataRequest.GetListSimUSSDStatus(clientId);
        }

        [WebMethod]
        public string SimUpdate(string clientId, int slot, string sim, int status)
        {
            NLogLogger.Info(new string[] { "SimUpdate", "Request", clientId, slot.ToString(), sim, status.ToString() });
            var response = DataRequest.UpdateSim(clientId, slot, sim, status);
            //NLogLogger.Info(new string[] { "SimUpdate", "Reponse", response.ToString() });
            return response < 0 ? "{\"status\":0,\"message\":\"Update failed\"}" : "{\"status\":1,\"message\":\"Update success\"}";
        }
    }
}
