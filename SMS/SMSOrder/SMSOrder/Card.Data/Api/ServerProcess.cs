using SMS.Data.DTO;
using SMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.Data.Factory;
using System.Net;
using RestSharp;

namespace SMS.Data.Api
{
    public class ServerProcess
    {
        public class ApiResponseSpam
        {
            public string Data { get; set; }

            public int Code { get; set; }

            public string Message { get; set; }

            //public long Total { get; set; }
        }
        private static readonly string Url = ConfigurationManager.AppSettings["Api"];
       
      
        
       
        public static void SetSystemStatusCache(int status)
        {

            var keycache = "SMSSystemStatus";
           
            RedisCaching.Add(keycache, status.ToString(), 3600 * 12);
            var obj = AbstractDAOFactory.Instance().ContentsService().GetTop(1).FirstOrDefault();
            obj.Status = status;
            AbstractDAOFactory.Instance().ContentsService().InsertUpdate(obj);

        }
        public static SMSRespone SendMultiSMS(List<SMSRequest> smsParam )
        {
            try
            {

                string serviceurl = "http://127.0.0.1:9002/MomoService.ashx";
                var requestData = new RequestData()
                {
                    PartnerCode = "spam",
                    CommandCode = "SPAM_OUT",
                    Signature = ""
                };
                requestData.RequestContent = JsonConvert.SerializeObject(smsParam).Replace("\n", " ");
               
                var postData = JsonConvert.SerializeObject(requestData);

              
                NLogLogger.DebugMessage(postData);
                //NLogLogger.DebugMessage(postData);
                var apiResponseText = Utilities.HttpRequestPostData(serviceurl, postData);
                NLogLogger.DebugMessage(apiResponseText);
                //NLogLogger.DebugMessage(apiResponseText);
                var apiResponse = JsonConvert.DeserializeObject<SMSRespone>(apiResponseText);
                //if (apiResponse.ResponseCode < 1)
                //{
                //    NLogLogger.DebugMessage(apiResponse);
                //    NLogLogger.DebugMessage(postData);
                //}
               
                return apiResponse;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new SMSRespone { ResponseCode = -99, Description = "Có lỗi trong quá trình xử lý" };

            }
        }
        public static string GetSystemStatusCache()
        {

            var keycache = "SMSSystemStatus";
            var cachedata = RedisCaching.GetData(keycache);
            if (cachedata == null)
            {
                var content = AbstractDAOFactory.Instance().ContentsService().GetTop(1).FirstOrDefault().Status.ToString();

                RedisCaching.Add(keycache, content, 3600 * 12);
                return content;
            }
            else
            {
                return cachedata.ToString();
            }
        }
        
    }
}
