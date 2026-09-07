using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Reflection;
using Libs.Utils;
using Libs.API;

/// <summary>
/// Summary description for VPGService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class VPGService : System.Web.Services.WebService {

    public VPGService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string Request(string partnerCode, string serviceCode, string commandCode, string requestContent, string signature)
    {
        return VPGUtils.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
    }
    
}
