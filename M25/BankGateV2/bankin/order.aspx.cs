using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BankGateV2.bankin.Info;
using static BankGateV2.bankin.Order;

namespace BankGateV2.bankin
{
    public partial class order : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var request = new PostGetHelper().GetFromQueryString<RequestOrder>();
                string type = "banktranfer";

                if (request.BankCode.ToUpper() == "MOMO")
                    type = "momov2";
               // NLogLogger.Info(new string[] { "VPGJsonService", "Request", jsonString });
                var result = VPGUtils.Order(request.PartnerCode, request.Amount, request.RefCode, request.BankCode, request.CallbackUrl, request.Signature, type);

                Response.Write(result);


            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VPGJsonService", "Exeption", exp.Message });

            }
        }
    }
}