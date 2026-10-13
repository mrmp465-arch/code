using Libs.API;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BankGateV2.bankin.Info;

namespace BankGateV2.bankin
{
    public partial class getbank : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               
                var request = new PostGetHelper().GetFromQueryString<RequestGetBank>();
                //NLogLogger.Info(new string[] { "VPGJsonService", "Request", jsonString });
                var result = VPGUtils.RequestGetBank(request.PartnerCode, "banktranfer", "");
                Response.Write(result);
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VPGJsonService", "Exeption", exp.Message });
               
            }
        }
    }
}