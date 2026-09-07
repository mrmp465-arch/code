using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace APIV2
{
    /// <summary>
    /// Summary description for VPGService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class VPGService : System.Web.Services.WebService
    {

        [WebMethod]
        public string Request(string partnerCode, string serviceCode, string commandCode, string requestContent, string signature)
        {
            return VPGUtils.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
        }
    }
}
