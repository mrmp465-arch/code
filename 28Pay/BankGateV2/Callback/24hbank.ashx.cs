using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankCash._24h;
using Libs.BankDirect.MDrum;
using Libs.BankDirect.MDrumV2;
using Libs.Utils;
using static Libs.BankDirect_24h._24hBankLib;

namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for _24hbank
    /// </summary>
    public class _24hbank : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            Libs.BankDirect.MDrum.MDrumBankLib.CallbackResponse callbacResponse;

            //var partnercode = HttpContext.Current.Request.QueryString["partnercode"];
            var jsonString = String.Empty;
            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }

            NLogLogger.Info(new string[] { "MDrumBankCalback", "ProcessRequest", jsonString });
            if (string.IsNullOrEmpty(jsonString))
            {
                context.Response.Write("99|Data empty");
                return;
            }
            //return;
            var callbackdata = serializer.Deserialize<Libs.BankDirect_24h._24hBankLib.Callback>(jsonString);
            //var callbackdata = new PostGetHelper().GetFromQueryString<MDrumBankLib.BankResponse>();

            //check sign

          
            if(callbackdata.status==1)
            {
                try
                {
                    var callback = new Libs.BankDirect._24h._24hBank().Callback(callbackdata);


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

                }
                catch (Exception exp)
                {
                    NLogLogger.Info(new string[] { "24hCalback", "ProcessRequest", exp.Message });
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
            //NLogLogger.Info(new string[] { "MDrumCalback", "ProcessRequest", serializer.Serialize(newclObj) });
           


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