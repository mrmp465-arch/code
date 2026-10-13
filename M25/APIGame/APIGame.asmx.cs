using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Libs.API;

namespace APIGame
{
    /// <summary>
    /// Summary description for MyViettel
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class APIGameService : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        public string RequestTopup(string transactionId, string telco, string partnerCode, string providerCode, string sim, string simTarget, string clientId, string cardSerial, string cardCode, int slot, int amount)
        {

            if (telco.ToUpper() == "GATE" || telco.ToUpper() == "BIT" || telco.ToUpper() == "VCOIN" ||
                telco.ToUpper() == "GOSU" || telco.ToUpper() == "ZING" || telco.ToUpper() == "GARENA")
            {
                //bool tryAgain = true;
                var result = new GameBiz().SendWebTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount);
                var response = result.Split('|');
                //while (Convert.ToInt32(response[0]) == (int) ResponseCode.ParameterInvalid && tryAgain)
                //{
                //    result = new MyViettelBiz().SendWebTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount);
                //    response = result.Split('|');
                //    tryAgain = false;
                //}

                return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";

            }

            return "{\"code\":-373,\"message\":\"Request fail.\",\"amount\":0}";
        }

        [WebMethod(EnableSession = true)]
        public string RequestByCard(string transactionId, string partnerCode, string providerCode, string telco, int amount, int quantity, string clientId)
        {
            if (telco.ToUpper() == "VMS" || telco.ToUpper() == "VNP" || telco.ToUpper() == "VTT")
            {
                var result = new GameBiz().SendWebBuyCard(transactionId, telco, amount, quantity, partnerCode, providerCode, clientId);
                return result;
            }
            else
            {
                return "{\"code\":-373,\"message\":\"Card provider invalid\",\"content\":\"\"}";
            }
        }

        [WebMethod(EnableSession = true)]
        public string RequestTranferBalance(string transactionId, string partnerCode, string providerCode, string telco, int amount, string simTarget, string clientId)
        {
            if (telco.ToUpper() == "VMS" || telco.ToUpper() == "VNP" || telco.ToUpper() == "VTT")
            {
                var result = new GameBiz().SendWebTranferBalance(transactionId, telco, amount, simTarget, partnerCode, providerCode, clientId);
                return result;
            }
            else
            {
                return "{\"code\":-373,\"message\":\"Provider invalid\"}";
            }
        }

        [WebMethod(EnableSession = true)]
        public string RequestTranferAccountBalance(string transactionId, string telco, int type, string clientId)
        {
            if (telco.ToUpper() == "GATE")
            {
                var result = new GameBiz().SendWebTranferAccountBalance(transactionId, telco, type, clientId);
                return result;
            }
            else
            {
                return "{\"code\":-373,\"message\":\"Provider invalid\"}";
            }
        }
    }

   

}
