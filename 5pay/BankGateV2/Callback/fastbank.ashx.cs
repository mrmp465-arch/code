using Libs.API;
using Libs.BankDirect.FastPay;
using Libs.BankDirect.MDrum;
using Libs.BankDirect.MDrumV2;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for fastbank
    /// </summary>
    public class fastbank : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            MDrumBankLib.CallbackResponse callbacResponse;

            //var partnercode = HttpContext.Current.Request.QueryString["partnercode"];
            var jsonString = String.Empty;
            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }

            NLogLogger.Info(new string[] { "fastcallback", "ProcessRequest", jsonString });
            if (string.IsNullOrEmpty(jsonString))
            {
                context.Response.Write("99|Data empty");
                return;
            }
            //return;

            var callbackdata = serializer.Deserialize<APIResponse>(jsonString);



            var newclObj = serializer.Deserialize<fastBankLib.Callback>(callbackdata.ResponseContent);


            //NLogLogger.Info(new string[] { "MDrumCalback", "ProcessRequest", serializer.Serialize(newclObj) });
            try
            {

                var callback = new fastBank().Callback(newclObj);


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
                //NLogLogger.Info(new string[] { "MDrumCalback", "Approve Request", newclObj.Note, serializer.Serialize(callback) });
                context.Response.Write(serializer.Serialize(callbacResponse));
                //}

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "MDrumCalback", "ProcessRequest", exp.Message });
                //System.Threading.Thread.Sleep(1000);
                // new MDrumV2Bank().Callback(newclObj);
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