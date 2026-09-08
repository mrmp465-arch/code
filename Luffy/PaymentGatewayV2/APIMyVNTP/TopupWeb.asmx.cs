using System.Web.Services;

namespace APIMyVNTP
{
    /// <summary>
    /// Summary description for TopupMobile
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TopupAppMobile : System.Web.Services.WebService
    {

        [WebMethod]
        public string RequestTopup(string transactionId, string telco, string partnerCode, string providerCode, string sim, string simTarget, string clientId, string cardSerial, string cardCode, int slot, int amount)
        {

            if (telco.ToUpper() == "VNP")
            {
               var result = new MyVNTPBiz().SendWebTopupCardV2(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount);
               var response = result.Split('|');
                return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";
            }

            return "{\"code\":-373,\"message\":\"Request fail.\",\"amount\":0}";
        }
    }
}
