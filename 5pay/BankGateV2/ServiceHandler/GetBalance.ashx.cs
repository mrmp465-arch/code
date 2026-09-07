using Libs.API;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetBalance
    /// </summary>
    public class GetBalance : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var partnerCode = HttpContext.Current.Request.QueryString["partnerCode"];
            var usser = new Users().GetByUserName(partnerCode);
            var rate = new RateInfo
            {
                Balance = usser.Balance

            };
            context.Response.Write(serializer.Serialize(rate));
        }
       
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public class RateInfo
        {
            public long Balance { get; set; }
           

        }
    }
}