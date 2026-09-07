using SMS.Data.DTO;
using SMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.Api
{
    public class SmsSend
    {
        private static readonly string Url = ConfigurationManager.AppSettings["SMSApi"];
        private static readonly string keySecret = "RfD2gjyPrJH8";
        private static readonly int partnerId = 45;
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int SendSMS(string number, string content, int id)
        {
            try
            {
                var data = new
                {
                    requestId = id,
                    targetNumber = number,
                    content = content,
                  
                };
                var jsonData = JsonConvert.SerializeObject(data);
                NLogLogger.DebugMessage(jsonData);
                String Encrypt = SMS.Utility.Encrypt.EncryptTripleDES2(jsonData, keySecret);

                var request = new
                {
                    partnerId = partnerId,
                    data = Encrypt,
                    chksum = 1,
                    time = (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds / 1000,

                };
                var postData = JsonConvert.SerializeObject(request);

                NLogLogger.DebugMessage(postData);
                //NLogLogger.DebugMessage(requestUrl);
                var apiResponseText = Utilities.HttpRequestPostData(Url, postData);
                NLogLogger.DebugMessage(apiResponseText);
                var apiResponse = JsonConvert.DeserializeObject<ResponeSMSData>(apiResponseText);
                if (apiResponse.errorCode < 0)
                {
                    NLogLogger.DebugMessage(apiResponse);
                    NLogLogger.DebugMessage(postData);
                }
                return apiResponse.errorCode;

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;

            }
        }
       
    }

    public class ResponeSMSData
    {
       
        public int errorCode { get; set; }
        public string message { get; set; }
       
    }
}
