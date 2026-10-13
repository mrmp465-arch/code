using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libs.API;

namespace APIV2
{
    /// <summary>
    /// Summary description for VPGService1
    /// </summary>
    public class VPGFormService : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.ContentType = "application/json";
            string partnerCode = context.Request.Form["PartnerCode"];
            string serviceCode = context.Request.Form["ServiceCode"];
            string commandCode = context.Request.Form["CommandCode"];
            string requestContent = context.Request.Form["RequestContent"];
            string signature = context.Request.Form["Signature"];

            if (string.IsNullOrEmpty(partnerCode) || string.IsNullOrEmpty(serviceCode) ||
                string.IsNullOrEmpty(commandCode) || string.IsNullOrEmpty(requestContent) ||
                string.IsNullOrEmpty(signature))
                context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
            else
                context.Response.Write(VPGUtils.Request(partnerCode, serviceCode, commandCode, requestContent, signature));

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}