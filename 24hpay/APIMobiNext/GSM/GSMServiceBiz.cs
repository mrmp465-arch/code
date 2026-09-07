using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;
using Newtonsoft.Json;

namespace APIMobiNext.GSM
{
    public class GSMServiceBiz
    {
        private static string gsmServiceObj = ConfigurationManager.AppSettings["ServiceUrl"] ?? "http://113.160.184.38:1582";
        private static string callbackUrl = ConfigurationManager.AppSettings["CalbackUrl"] ?? "http://149.28.130.246:1586/GSMCallback.ashx";
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static bool GetSimGSMStatus(string mobiNumber)
        {
            var apiUrl = gsmServiceObj + "/api/GsmModem/GetAllDeviceInfo";
            var res = Task.Run(async () => await GetTask(apiUrl)).Result;

            if (!string.IsNullOrEmpty(res))
            {
                var response = JsonConvert.DeserializeObject<GetAllPortInfoResponse>(res);
                foreach (var r in response.ResponseContent)
                {
                    if (mobiNumber == r.Number && r.State == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<string> GetListSimGSMStatus()
        {
            var apiUrl = gsmServiceObj + "/api/GsmModem/GetAllDeviceInfo";
            var res = Task.Run(async () => await GetTask(apiUrl)).Result;

            if (!string.IsNullOrEmpty(res))
            {
                
                var response = JsonConvert.DeserializeObject<GetAllPortInfoResponse>(res);
                var lstSim = new List<string>();
                foreach (var r in response.ResponseContent)
                {
                    if (r.State == 1 && !string.IsNullOrEmpty(r.Number))
                    {
                        lstSim.Add(r.Number);
                    }
                }

                return lstSim;
            }
            return null;
        }

        public static APIResponse SendChardRequest(string transactionId, string telco, string sim, string simTarget, string cardSerial, string cardCode, int actionType)
        {

            var apiUrl = gsmServiceObj + "/api/GsmModem/RequestUssd";

            if (GetSimGSMStatus(sim))
            {
                var request = new UssdRequest()
                {
                    transId = transactionId,
                    actionType = actionType,
                    callbackUrl = callbackUrl,
                    cardCode = cardCode,
                    cardSerial = cardSerial,
                    cardValue = 0,
                    quota = 0,
                    sim = sim,
                    simTarget = simTarget,
                    telco = telco
                };

                //var res = gsmServiceObj.UssdRequest(transactionId, telco, sim, simTarget, cardSerial, cardCode, actionType, callbackUrl, -1, -1);
                var res = Task.Run(async () => await PosTask(apiUrl,JsonConvert.SerializeObject(request))).Result;

                NLogLogger.Info(new string[] { "GSMService", "UssdRequest", res });
                if (!string.IsNullOrEmpty(res))
                {
                    var response = serializer.Deserialize<APIResponse>(res);
                    return response;
                }
            }

            return new APIResponse((int)ResponseCode.SimNotActive);
        }

        private static async Task<string> PosTask(string url, string postData)
        {
            NLogLogger.Info(new string[] { "GSMService", "PosTask", "Request", url, postData });
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            var httpClient = new HttpClient();
            try
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.PostAsync(url, httpContent);

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    httpClient.Dispose();
                    return responseContent;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GSMService", "PosTask", "Error", e.Message });
                return string.Empty;
            }

            httpClient.Dispose();
            return null;

        }

        private static async Task<string> GetTask(string url)
        {
            var uri = new Uri(url);
            var httpClient = new HttpClient();
            var res = string.Empty;
            //httpClient.Timeout = TimeSpan.FromSeconds(60);
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
            try
            {
                var response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    res = await response.Content.ReadAsStringAsync();
                }
                httpClient.Dispose();
                return res;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "GSMService", "GetTask", "Exception", url, e.Message, e.StackTrace });
            }

            httpClient.Dispose();
            return null;

        }
    }
}