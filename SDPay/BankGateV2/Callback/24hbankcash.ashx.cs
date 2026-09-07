using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankCash._24h;
using Libs.BankCash.Drum;
using Libs.BankCash.DrumV2;
using Libs.Utils;

namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for _24hbankcash
    /// </summary>
    public class _24hbankcash : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            DrumBankLib.CallbackResponse callbacResponse;
            try
            {
                //var partnercode = HttpContext.Current.Request.QueryString["partnercode"];
                var jsonString = String.Empty;
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }


                if (string.IsNullOrEmpty(jsonString))
                {
                    context.Response.Write("99|Data empty");
                    return;
                }
                NLogLogger.Info(new string[] { "MDrumCalback", "ProcessRequest", jsonString });
                var callbackdata = serializer.Deserialize<_24hBankLib.Callback>(jsonString);
                //var callbackdata = new PostGetHelper().GetFromQueryString<MDrumBankLib.BankResponse>();

                //check sign

                //var newclObj = serializer.Deserialize<DrumBankLib.Callback>(callbackdata.ResponseContent);

                var callback = new _24hBank().Callback(callbackdata);
                //NLogLogger.Info(new string[] { "MDrumCalback", "ProcessRequest", serializer.Serialize(callbackdata) });

                if (callback.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    callbacResponse = new DrumBankLib.CallbackResponse()
                    {
                        errorCode = 1,
                        errorDescription = callback.Description
                    };
                }
                else
                {
                    callbacResponse = new DrumBankLib.CallbackResponse()
                    {
                        errorCode = callback.ResponseCode,
                        errorDescription = callback.Description
                    };
                }
                NLogLogger.Info(new string[] { "24hCalback", "Approve Request", serializer.Serialize(callback) });
                context.Response.Write(serializer.Serialize(callbacResponse));
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "24hCalback", "ProcessRequest", exp.Message });
                callbacResponse = new DrumBankLib.CallbackResponse()
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