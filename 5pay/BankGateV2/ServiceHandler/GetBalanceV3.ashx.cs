using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetBalanceV3
    /// </summary>
    public class GetBalanceV3 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
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