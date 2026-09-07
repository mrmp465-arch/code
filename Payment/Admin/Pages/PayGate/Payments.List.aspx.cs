using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_PayGate_Payments_List : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PaymentsList);
        if (!IsPostBack)
        {
            var _Payment = new Payments();
            rptList.DataSource = _Payment.GetTable();
            rptList.DataBind();
        }
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PaymentsAdd);
    }
}