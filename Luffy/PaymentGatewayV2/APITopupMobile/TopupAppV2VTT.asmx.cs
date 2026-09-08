using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Libs.Utils;


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
    public class TopupAppVTTV2Service : System.Web.Services.WebService
    {

        [WebMethod]
        public string RequestTopup(string transactionId, string telco, string partnerCode, string providerCode, string cardSerial, string cardCode, int amount)
        {
            NLogLogger.Info(new string[] { "TopupAppVTTV2Service", transactionId,telco, partnerCode, providerCode, cardSerial, cardCode, amount.ToString() });
            if (telco.ToUpper() == "VTT")
            {
                var result = new TopupAppVTTV2().SendAppVTTV2Topup(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, amount);
                var response = result.Split('|');
                return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";
            }

            return "{\"code\":-373,\"message\":\"Request fail.\",\"amount\":0}";
        }
    }
}
