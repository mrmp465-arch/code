using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BankGateV2.bankin.Order;
using static BankGateV2.bankout.cash;

namespace BankGateV2.bankout
{
    public partial class cash1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var request = new PostGetHelper().GetFromQueryString<RequestCash>();
               
                // NLogLogger.Info(new string[] { "VPGJsonService", "Request", jsonString });
                var result = VPGUtils.RequestCash(request.PartnerCode, request.AccountNumber, request.AccountName, request.Amount, request.RefCode, request.BankCode, request.Signature, request.CallbackUrl,"");
                
                Response.Write(result);


            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VPGJsonService", "Exeption", exp.Message });

            }
        }
    }
}