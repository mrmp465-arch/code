using Libs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetBanks
    /// </summary>
    public class GetBanks : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var lstbankcode = new BankCodeTranfer().GetListCache();
            lstbankcode = lstbankcode.Where(x => x.isTransfer == 1).ToList();
            context.Response.Write(serializer.Serialize(lstbankcode));

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