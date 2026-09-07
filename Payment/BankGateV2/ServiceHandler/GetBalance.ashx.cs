using Libs.API;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetBalance
    /// </summary>
    public class GetBalance : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var partnerCode = HttpContext.Current.Request.QueryString["partnerCode"];
            var usser = new Users().GetByUserName(partnerCode);
            context.Response.Write(usser.Balance);
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