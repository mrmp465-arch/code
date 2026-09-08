using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libs.BankCash.Ken;
using System.Web.Script.Serialization;

namespace BankGateV2
{
    /// <summary>
    /// Summary description for GetCashInfo
    /// </summary>
    public class GetCashInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var cbObj = new PostGetHelper().GetFromQueryString<KenBankLib.RequestDetail>();
            var callbacResponse = new KenBank().Get(cbObj.refcode,cbObj.partnercode);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(callbacResponse));
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