using Libs.API;
using Libs.Utils;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetBalanceV2
    /// </summary>
    public class GetBalanceV2 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var partnerCode = HttpContext.Current.Request.QueryString["partnerCode"];
            var signature = HttpContext.Current.Request.QueryString["signature"];
            var partner = new Partners().GetCache(partnerCode);

            var sig = Libs.Utils.Encrypts.MD5(partnerCode + partner.PublicKey);
            if(sig!=signature)
            {
                
                context.Response.Write("-1");
                return;
            }    
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