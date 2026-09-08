using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankDirect.Bicbic;
using Libs.Utils;
using Libs.BankDirect.VNPayBank;
using Libs.BankCash.Jav;

namespace BankGateV2.Callback
{
    /// <summary>
    /// Summary description for VNPay
    /// </summary>
    public class VNPayCallback : IHttpHandler
    {

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            BicbicBankLib.CallbackResponse callbacResponse;
           
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