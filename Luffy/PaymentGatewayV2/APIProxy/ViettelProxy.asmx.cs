using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using APIProxy.Service;

namespace APIProxy
{
    /// <summary>
    /// Summary description for MyViettel
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ViettelProxy : System.Web.Services.WebService
    {

        string serviceMyViettelUrl = "http://127.0.0.1:1585/";
        string serviceSmasUrl = "http://127.0.0.1:1585/";

        [WebMethod (EnableSession = true)]
        public string RequestTopup(string transactionId, string telco, string partnerCode, string providerCode, string sim, string simTarget, string clientId, string cardSerial, string cardCode, int slot, int amount)
        {

            if (telco.ToUpper() == "VTT")
            {
                switch (Type)
                {
                        
                }


                MyViettel _VPGService = new MyViettel(serviceMyViettelUrl + "MyViettel.asmx");
                string result = string.Empty;
                result = _VPGService.RequestTopup(transactionId, telco, partnerCode, providerCode, sim, simTarget, clientId, cardSerial, cardCode, slot, amount);
                 var response = result.Split('|');
                return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";
            }

            return "{\"code\":-373,\"message\":\"Request fail.\",\"amount\":0}";
        }
    }
}
