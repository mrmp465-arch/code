using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.BankDirect.MDrum;
using Libs.BankDirect.MDrumV2;
using Libs.Utils;



namespace BankGateV2.Pages
{
    public partial class MomoLink : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            //var type = AppUtils.GetParam("type", string.Empty);
            var orderNo = AppUtils.GetParam("orderNo", string.Empty);
            //NLogLogger.Info("orderNo"+ orderNo);
            if (String.IsNullOrEmpty(orderNo))
            {
                //pn_hide_info.Visible = true;
                //pn_show_info.Visible = false;
                return;
            }
            
            var bankinfo = MDrumBankLib.GetBankInfo(orderNo);
            NLogLogger.Info("bankinfo LinkOpenApp" + orderNo + " "+ bankinfo.LinkOpenApp);
            if (bankinfo == null)
            {

                return;
            }
            Response.Redirect(bankinfo.LinkOpenApp);
            return;

        }
    }
}