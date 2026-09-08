using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using Libs.API;
using Libs.Utils;

namespace APIMyViettel
{
    /// <summary>
    /// Summary description for CheckCard
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CheckCard : System.Web.Services.WebService
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string privateKey = "1e7f56ca5fcbf781fa022f4f5dff74d6";

        [WebMethod]
        public string CheckSerial(string serial, string sign)
        {

            var signature = Encrypts.MD5(string.Format("{0}|{1}", serial, privateKey));
            if (string.IsNullOrEmpty(serial) || string.IsNullOrEmpty(sign))
                return serializer.Serialize(new APIResponse((int)ResponseCode.TransactionFailed));
            if (signature != sign)
                return serializer.Serialize(new APIResponse((int)ResponseCode.SignatureInvalid));
           
            return serializer.Serialize(MyViettelService.CheckCard(serial));
        }
    }
}
