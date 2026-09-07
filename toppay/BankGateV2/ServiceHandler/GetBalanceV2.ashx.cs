using Libs.API;
using Libs.Utils;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Script.Serialization;

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
            var type = HttpContext.Current.Request.QueryString["type"];
            var partner = new Partners().GetCache(partnerCode);

            var sig = Libs.Utils.Encrypts.MD5(partnerCode + partner.PublicKey);
            if(sig!=signature)
            {
                
                context.Response.Write("-1");
                return;
            }    
            var usser = new Users().GetByUserName(partnerCode);
            if (type == "json")
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var rate = new RateInfo
                {
                    Balance = usser.Balance

                };
                context.Response.Write(serializer.Serialize(rate));
                return;
            }
            context.Response.Write(usser.Balance);
        }
        public class RateInfo
        {
            public long Balance { get; set; }


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