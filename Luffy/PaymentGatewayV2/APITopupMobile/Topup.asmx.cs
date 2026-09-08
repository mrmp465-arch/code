using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;


namespace APITopupMobile
{
    /// <summary>
    /// Summary description for TopupMobile
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TopupMobile : System.Web.Services.WebService
    {

        [WebMethod]
        public string RequestTopup(string transactionId, string telco, string partnerCode, string providerCode, string sim, string simTarget, string clientId, string cardSerial, string cardCode, int slot, int amount)
        {

            if (telco.ToUpper() == "VTT")
            {
                var result = new TopupVTT().SendUSSDTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount).Result;
                var response = result.Split('|');
                return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";
            }

            if (telco.ToUpper() == "VMS")
            {
                var result = new TopupVMS().SendUSSDTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount).Result;
                var response = result.Split('|');
                return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";
            }

            if (telco.ToUpper() == "VNP")
            {
                var result = new TopupVNP().SendUSSDTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount).Result;
                var response = result.Split('|');
                return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";
            }

            return "{\"code\":-373,\"message\":\"Request fail.\",\"amount\":0}";
        }
    }
}
