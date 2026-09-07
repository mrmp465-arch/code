using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APIMyMobi
{
    /// <summary>
    /// Summary description for MyViettel
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MyMobi : System.Web.Services.WebService
    {

        [WebMethod (EnableSession = true)]
        public string RequestTopup(string transactionId, string telco, string partnerCode, string providerCode, string sim, string simTarget, string clientId, string cardSerial, string cardCode, int slot, int amount)
        {

            if (telco.ToUpper() == "VMS")
            {
                //bool tryAgain = true;
                //NLogLogger.Info(new string[] { "MobiNext", "OK Start" });
                Partners _Partner = new Partners().GetCache(partnerCode);
                if (_Partner.RequestType == 1)
                {
                    var result = new MyMobiBiz().SendWebTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount);
                    var response = result.Split('|');
                    //while (Convert.ToInt32(response[0]) == (int) ResponseCode.ParameterInvalid && tryAgain)
                    //{
                    //    result = new MyViettelBiz().SendWebTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount);
                    //    response = result.Split('|');
                    //    tryAgain = false;
                    //}

                    return "{\"code\":" + response[0] + ",\"message\":\"Request success.\",\"amount\":" + response[1] + "}";
                }
                else
                {
                    //sử dụng callback
                    Task.Run(() => SendWebTopupCard(transactionId, telco, partnerCode, providerCode, cardSerial, cardCode, sim, simTarget, clientId, slot, amount)).ConfigureAwait(false);
                   
                    return "{\"code\":0,\"message\":\"Request success.\",\"amount\":0}";
                }
            }

            return "{\"code\":-373,\"message\":\"Request fail.\",\"amount\":0}";
        }
        private void SendWebTopupCard(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        {
            var result = new MyMobiBiz().SendWebTopupCard(transactionId, telco, partnerCode, providerCode, serial, pin, sim, simTarget, clientId, slot, amount);
            var response = result.Split('|');
            var cardAPILog = new CardAPILog().Get(Convert.ToInt32(transactionId));
            cardAPILog.Amount = Convert.ToInt64(response[1]);
            cardAPILog.Description = "Callback " + response;
            cardAPILog.Status = Convert.ToInt32(response[0]);
            cardAPILog.Update();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var privateKey = new Partners().Get(cardAPILog.PartnerCode).PrivateKey;
            var datacb = new DataCallback()
            {
                Amount = (int)cardAPILog.Amount,
                RefCode = cardAPILog.RequestNo,
                Status = cardAPILog.Status,
                Signature = Libs.Utils.Encrypts.MD5(cardAPILog.RequestNo + cardAPILog.Status + cardAPILog.Amount + privateKey)
            };

            if (!string.IsNullOrEmpty(cardAPILog.CallbackUrl))
            {
                Task.Run(async () => await CallbackJson(cardAPILog.CallbackUrl, serializer.Serialize(datacb), cardAPILog.PartnerCode + " " + cardAPILog.RequestNo).ConfigureAwait(false));
                NLogLogger.Info(new string[] { "MyMobi", "Callback Partner", transactionId.ToString() });
            }
        }
        public async Task<string> CallbackJson(string url, string postData, string code)
        {
            NLogLogger.Info(new string[] { "MyMobi", "Callback Partner", "Request", code, url, postData });

            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        NLogLogger.Info(new string[] { "MyMobi", "Callback Partner", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MyMobi", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }
    


}
