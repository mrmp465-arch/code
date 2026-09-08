using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Libs.Utils;


namespace APIAloPay
{
    /// <summary>
    /// Summary description for TopupMobile
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TopupCardService : System.Web.Services.WebService
    {

        [WebMethod]
        public string RequestTopup(string transactionId, string telco, string partnerCode, string providerCode, string cardSerial, string cardCode, int amount)
        {
            //NLogLogger.Info(new string[] { "IZISoftTopupPService", transactionId,telco, partnerCode, providerCode, cardSerial, cardCode, amount.ToString() });
            if (telco.ToUpper() == "VNP" || telco.ToUpper() == "VMS" || telco.ToUpper() == "VTT")
            {
                if (!ConfigurationManager.AppSettings["PartnerDirect"].ToString().Contains("," + partnerCode + ","))
                {
                    var result = new TopupApp().VinaPayTopupCallBack(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, amount).Result;
                    var response = result.Split('|');
                    return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";
                }
                else
                {
                    var result = new TopupApp().VinaPayTopup(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, amount).Result;
                    var response = result.Split('|');
                    return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";
                }
                   
            }

            return "{\"code\":-373,\"message\":\"Request fail.\",\"amount\":0}";
        }
    }
}
