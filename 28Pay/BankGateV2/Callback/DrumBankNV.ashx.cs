using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankDirect.MDrum;
using Libs.BankDirect.MDrumV2;
using Libs.Utils;

namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for MDrumCallback
    /// </summary>
    public class DrumBankNV : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            MDrumBankLib.CallbackResponse callbacResponse;
            try
            {
                var partnercode = HttpContext.Current.Request.QueryString["partnercode"];
                var jsonString = String.Empty;
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }

                NLogLogger.Info(new string[] { "MDrumCalback", "ProcessRequest", jsonString, partnercode });
                if (string.IsNullOrEmpty(jsonString))
                {
                    context.Response.Write("99|Data empty");
                    return;
                }
                var callbackdata = serializer.Deserialize<MDrumBankLib.BankResponse>(jsonString);
                //var callbackdata = new PostGetHelper().GetFromQueryString<MDrumBankLib.BankResponse>();

                //check sign

                var newclObj = serializer.Deserialize<MDrumBankLib.CallbackV3>(callbackdata.ResponseContent);

                var callback = new MDrumV2Bank().CallbackV3(newclObj, partnercode);
                //NLogLogger.Info(new string[] { "MDrumCalback", "ProcessRequest", serializer.Serialize(newclObj), partnercode });

                if (callback.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    callbacResponse = new MDrumBankLib.CallbackResponse()
                    {
                        errorCode = 1,
                        errorDescription = callback.Description
                    };
                }
                else
                {
                    callbacResponse = new MDrumBankLib.CallbackResponse()
                    {
                        errorCode = callback.ResponseCode,
                        errorDescription = callback.Description
                    };
                }
                NLogLogger.Info(new string[] { "MDrumCalback", "Approve Request", serializer.Serialize(callback) });
                context.Response.Write(serializer.Serialize(callbacResponse));
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "MDrumCalback", "ProcessRequest", exp.Message });
                callbacResponse = new MDrumBankLib.CallbackResponse()
                {
                    errorCode = (int)ResponseCode.ParameterInvalid,
                    errorDescription = "Parameter Invalid"
                };
                context.Response.Write(serializer.Serialize(callbacResponse));
            }


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