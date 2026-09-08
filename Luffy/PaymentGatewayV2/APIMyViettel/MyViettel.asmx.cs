using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Libs.API;

namespace APIMyViettel
{
    /// <summary>
    /// Summary description for MyViettel
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MyViettel : System.Web.Services.WebService
    {

        [WebMethod (EnableSession = true)]
        public string RequestTopup(string transactionId, string telco, string partnerCode, string providerCode, string sim, string simTarget, string clientId, string cardSerial, string cardCode, int slot, int amount)
        {

            if (telco.ToUpper() == "VTT")
            {
                //bool tryAgain = true;
                var result = new MyViettelBiz().SendWebTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount);
                //var result = new MyViettelBiz().SendAppTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount);
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
    }
}
