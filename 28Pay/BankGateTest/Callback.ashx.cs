using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankGateTest
{
    /// <summary>
    /// Summary description for Callback
    /// </summary>
    public class Callback : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("ok");
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