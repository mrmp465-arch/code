using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;


namespace APIV2
{
    /// <summary>
    /// Summary description for Balance
    /// </summary>
    public class Balance : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            var jsonString = String.Empty;
            var result = string.Empty;
            context.Request.InputStream.Position = 0;
            try
            {
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var request = javaScriptSerializer.Deserialize<RequestDataBalance>(jsonString);
                Partners _Partner = new Partners().Get(request.PartnerCode);
                //Kiểm tra _Partner tồn tại hoặc Active không
                if (_Partner == null || _Partner.Status == 0)
                {
                    result = ResponseUtils.Response((int)ResponseCode.PartnerNotExistsNotActive);
                   
                }
                APIResponse apiResponse = new APIResponse(1);
                apiResponse.ResponseContent = _Partner.Balance.ToString() ;
                result = new JavaScriptSerializer().Serialize(apiResponse);

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VPGJsonService", "ProcessRequest", exp.Message });
                context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
            }
            context.Response.Write(result);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public class RequestDataBalance
        {
            public string PartnerCode { get; set; }
            public string Signature { get; set; }

        }
    }
}